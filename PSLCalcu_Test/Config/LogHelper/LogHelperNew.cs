using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Configuration;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]


//该log新类，参考https://blog.csdn.net/autfish/article/details/51462922，目前仅作为测试使用

/// <summary>
/// 自定义一个用于读取配置项的Appender，初始化Logger时首先读取配置项中的同名Appender，如果存在使用配置参数，如果不存在则使用默认配置。
/// </summary>
public class ReadParamAppender : log4net.Appender.AppenderSkeleton
{
    private string _file;
    public string File
    {
        get { return this._file; }
        set { _file = value; }
    }

    private int _maxSizeRollBackups;
    public int MaxSizeRollBackups
    {
        get { return this._maxSizeRollBackups; }
        set { _maxSizeRollBackups = value; }
    }

    private bool _appendToFile = true;
    public bool AppendToFile
    {
        get { return this._appendToFile; }
        set { _appendToFile = value; }
    }

    private string _maximumFileSize;
    public string MaximumFileSize
    {
        get { return this._maximumFileSize; }
        set { _maximumFileSize = value; }
    }

    private string _layoutPattern;
    public string LayoutPattern
    {
        get { return this._layoutPattern; }
        set { _layoutPattern = value; }
    }

    private string _datePattern;
    public string DatePattern
    {
        get { return this._datePattern; }
        set { _datePattern = value; }
    }

    private string _level;
    public string Level
    {
        get { return this._level; }
        set { _level = value; }
    }

    protected override void Append(log4net.Core.LoggingEvent loggingEvent)
    {
    }
}

/// <summary>
/// http://blog.csdn.net/autfish
/// </summary>
public static class LogHelperNew
{
    private static readonly ConcurrentDictionary<string, ILog> loggerContainer = new ConcurrentDictionary<string, ILog>();

    private static readonly Dictionary<string, ReadParamAppender> appenderContainer = new Dictionary<string, ReadParamAppender>();
    private static object lockObj = new object();

    //默认配置
    private const int MAX_SIZE_ROLL_BACKUPS = 20;
    private const string LAYOUT_PATTERN = "%d [%t] %-5p %c  - %m%n";
    private const string DATE_PATTERN = "yyyyMMdd\".txt\"";
    private const string MAXIMUM_FILE_SIZE = "2MB";
    private const string LEVEL = "debug";

    //读取配置文件并缓存
    static LogHelperNew()
    {
        IAppender[] appenders = LogManager.GetRepository().GetAppenders();
        for (int i = 0; i < appenders.Length; i++)
        {
            if (appenders[i] is ReadParamAppender)
            {
                ReadParamAppender appender = (ReadParamAppender)appenders[i];
                if (appender.MaxSizeRollBackups == 0)
                {
                    appender.MaxSizeRollBackups = MAX_SIZE_ROLL_BACKUPS;
                }
                if (appender.Layout != null && appender.Layout is log4net.Layout.PatternLayout)
                {
                    appender.LayoutPattern = ((log4net.Layout.PatternLayout)appender.Layout).ConversionPattern;
                }
                if (string.IsNullOrEmpty(appender.LayoutPattern))
                {
                    appender.LayoutPattern = LAYOUT_PATTERN;
                }
                if (string.IsNullOrEmpty(appender.DatePattern))
                {
                    appender.DatePattern = DATE_PATTERN;
                }
                if (string.IsNullOrEmpty(appender.MaximumFileSize))
                {
                    appender.MaximumFileSize = MAXIMUM_FILE_SIZE;
                }
                if (string.IsNullOrEmpty(appender.Level))
                {
                    appender.Level = LEVEL;
                }
                lock (lockObj)
                {
                    appenderContainer[appenders[i].Name] = appender;
                }
            }
        }
    }

    public static ILog GetCustomLogger(string loggerName, string category = null, bool additivity = false)
    {
        return loggerContainer.GetOrAdd(loggerName, delegate(string name)
        {
            RollingFileAppender newAppender = null;
            ReadParamAppender appender = null;
            if (appenderContainer.ContainsKey(loggerName))
            {
                appender = appenderContainer[loggerName];
                newAppender = GetNewFileApender(loggerName, string.IsNullOrEmpty(appender.File) ? GetFile(category, loggerName) : appender.File, appender.MaxSizeRollBackups,
                    appender.AppendToFile, true, appender.MaximumFileSize, RollingFileAppender.RollingMode.Composite, appender.DatePattern, appender.LayoutPattern);
            }
            else
            {
                newAppender = GetNewFileApender(loggerName, GetFile(category, loggerName), MAX_SIZE_ROLL_BACKUPS, true, true, MAXIMUM_FILE_SIZE, RollingFileAppender.RollingMode.Composite,
                    DATE_PATTERN, LAYOUT_PATTERN);
            }
            log4net.Repository.Hierarchy.Hierarchy repository = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            Logger logger = repository.LoggerFactory.CreateLogger(repository, loggerName);
            logger.Hierarchy = repository;
            logger.Parent = repository.Root;
            logger.Level = GetLoggerLevel(appender == null ? LEVEL : appender.Level);
            logger.Additivity = additivity;
            logger.AddAppender(newAppender);
            logger.Repository.Configured = true;
            return new LogImpl(logger);
        });
    }

    //如果没有指定文件路径则在运行路径下建立 Log\{loggerName}.txt
    private static string GetFile(string category, string loggerName)
    {
        if (string.IsNullOrEmpty(category))
        {
            return string.Format(@"Log\{0}.txt", loggerName);
        }
        else
        {
            return string.Format(@"Log\{0}\{1}.txt", category, loggerName);
        }
    }

    private static Level GetLoggerLevel(string level)
    {
        if (!string.IsNullOrEmpty(level))
        {
            switch (level.ToLower().Trim())
            {
                case "debug":
                    return Level.Debug;

                case "info":
                    return Level.Info;

                case "warn":
                    return Level.Warn;

                case "error":
                    return Level.Error;

                case "fatal":
                    return Level.Fatal;
            }
        }
        return Level.Debug;
    }

    private static RollingFileAppender GetNewFileApender(string appenderName, string file, int maxSizeRollBackups, bool appendToFile = true, bool staticLogFileName = false, string maximumFileSize = "5MB", RollingFileAppender.RollingMode rollingMode = RollingFileAppender.RollingMode.Composite, string datePattern = "yyyyMMdd\".txt\"", string layoutPattern = "%d [%t] %-5p %c  - %m%n")
    {
        RollingFileAppender appender = new RollingFileAppender
        {
            LockingModel = new FileAppender.MinimalLock(),
            Name = appenderName,
            File = file,
            AppendToFile = appendToFile,
            MaxSizeRollBackups = maxSizeRollBackups,
            MaximumFileSize = maximumFileSize,
            StaticLogFileName = staticLogFileName,
            RollingStyle = rollingMode,
            DatePattern = datePattern
        };
        PatternLayout layout = new PatternLayout(layoutPattern);
        appender.Layout = layout;
        layout.ActivateOptions();
        appender.ActivateOptions();
        return appender;
    }
}
