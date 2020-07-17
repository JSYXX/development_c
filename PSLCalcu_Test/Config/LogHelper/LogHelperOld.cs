using System;
using System.IO;
using System.Windows.Forms;     //使用Application
using log4net;                  //使用log4net组件
using log4net.Config;           //使用log4net配置信息组件

public static class LogFactory
{
    //LogFactory静态类，初始化时，按照指定位置读取log4net的配置信息，并放入静态属性XmlConfigurator.Configure中
    static LogFactory()
    {        
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(Application.StartupPath + "\\LogConfig\\log4net.config"); //从指定的位置读取log4net的配置文件
        XmlConfigurator.Configure(fileInfo);
    }
   
    //使用LogFactory.LogHelper时，log4net利用已经读取的静态属性XmlConfigurator.Configure中的配置信息，创建loghelper对象。该对象由传入参数的类来管理。
    static public LogHelper GetLogger(System.Type type)
    {   
        //GetLogger()静态方法，用提供的名字（type名或者str）来检索已经存在的Logger对象。如果log4net框架中已经存在该对象，则返回该对象，如果不存在则重新创建一个Logger对象。
        //GetLogger()获得的Logger对象，付给LogHelper一个实例的内部变量ILog logger。使用LogHelper对象，就是使用该对象的私有ILog logger对象。
        return new LogHelper(LogManager.GetLogger(type));
    }
    //和上面的方法一样，无论是string，还是Type，最终都是获得传入类的类名
    static public LogHelper GetLogger(string str)
    {
        return new LogHelper(LogManager.GetLogger(str));
    }
    
}
public class LogHelper
{
    private ILog logger;

    public LogHelper(ILog log)
    {
        this.logger = log;
    }


    public void Debug(object message)
    {
        this.logger.Debug(message);
    }

    public void Debug(object message, System.Exception e)
    {
        this.logger.Debug(message, e);
    }

    public void Info(object message)
    {
        this.logger.Info(message);
    }

    public void Info(object message, System.Exception e)
    {
        this.logger.Info(message, e);
    }

    public void Warn(object message)
    {
        this.logger.Warn(message);
    }

    public void Warn(object message, System.Exception e)
    {
        this.logger.Warn(message, e);
    }

    public void Error(object message)
    {
        this.logger.Error(message);
    }
    public void Error(object message, System.Exception e)
    {
        this.logger.Error(message, e);
    }

    public void Fatal(object message)
    {
        this.logger.Fatal(message);
    }

    public void Fatal(object message, System.Exception e)
    {
        this.logger.Fatal(message, e);
    }


    public void InfoFormat(string format, object arg0)
    {
        this.logger.InfoFormat(format, arg0);
    }

    public void InfoFormat(string format, params object[] args)
    {
        this.logger.InfoFormat(format, args);
    }

    public void InfoFormat(string format, object arg0, object arg1)
    {
        this.logger.InfoFormat(format, arg0, arg1);
    }

    public void InfoFormat(string format, object arg0, object arg1, object arg2)
    {
        this.logger.InfoFormat(format, arg0, arg1, arg2);
    }

    //变更log的路径和文件名称
    public void ChangeLog4netLogFileName(string fileName)
    {
        
        
        log4net.Core.LogImpl logImpl = this.logger as log4net.Core.LogImpl;
        if (logImpl != null)
        {
            log4net.Repository.Hierarchy.Logger temp = (log4net.Repository.Hierarchy.Logger)logImpl.Logger;

            log4net.Appender.AppenderCollection ac = ((log4net.Repository.Hierarchy.Logger)logImpl.Logger).Appenders;
            for (int i = 0; i < ac.Count; i++)
            {     // 这里我只对RollingFileAppender类型做修改
                log4net.Appender.RollingFileAppender rfa = ac[i] as log4net.Appender.RollingFileAppender;
                if (rfa != null)
                {
                    rfa.File = fileName;
                    if (!System.IO.File.Exists(fileName))
                    {
                        System.IO.File.Create(fileName);
                    }
                    // 更新Writer属性
                    rfa.Writer = new System.IO.StreamWriter(rfa.File, rfa.AppendToFile, rfa.Encoding);
                }
            }
        }
    }
}

