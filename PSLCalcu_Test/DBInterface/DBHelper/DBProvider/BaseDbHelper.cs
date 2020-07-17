using System;
using System.Data;                  //使用DbDataAdapter类
using System.Data.Common;           //使用DbConnection类、DbCommand类
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace DBInterface.RDBInterface
{
    /// <summary>
    /// BaseDbHelper
    /// 数据库访问基础类。
    /// 
    /// 重要说明
    /// 在整体的异常处理放在DAO层的情况下，由于DAO层的读写每个动作都是先open，再close，因此，open动作不应该添加try语句包例外封装在这里。
    /// 而是应该在open异常的情况下，马上就能把异常反应到外层，从而使得DAO层面可以及时停止当前操作。同时可以使得DAO可以获得正确的exception信息
    /// 推而广之，在整体的异常处理放在DAO层的情况下，dbhelper任何位置都不应该有try语句
    /// 
    /// 版本：1.1
    ///  
    /// 修改纪录
    ///     
    ///     2016.12.16 版本：1.1 Arrow 修改PlusSign注释。
    ///     2016.12.15 版本：1.0 Arrow 实现IDisposable接口。   
    /// <author>
    ///		<name>Arrow</name>
    ///		<date>2016.12.16</date>
    /// </author> 
    /// </summary>
    public abstract class BaseDbHelper : IDisposable // IDbHelper部分功能由基础类实现（不同数据库实现方法相同的部分）
    {
        //字段：
        private DbTransaction dbTransaction = null;     //事务对象
        public string FileName = "BaseDbHelper.txt";    //sql查询句日志

        #region DbProviderFactory属性，该属性在具体的Helper中重写，此处没有意义
        private DbProviderFactory _dbProviderFactory = null;
        /// <summary>
        /// DbProviderFactory实例
        /// </summary>
        public virtual DbProviderFactory GetInstance()
        {
            if (_dbProviderFactory == null)
            {
                _dbProviderFactory = DbHelperFactory.GetHelper().GetInstance();
            }

            return _dbProviderFactory;
        }
        #endregion

        //属性：基类属性，非接口属性，
        #region public DbDataAdapter 当前数据库使用的适配器类型
        private DbDataAdapter dbDataAdapter = null;
        /// <summary>
        /// 数据库适配器
        /// </summary>
        public DbDataAdapter DbDataAdapter
        {
            get
            {
                return this.dbDataAdapter;
            }

            set
            {
                this.dbDataAdapter = value;
            }
        }
        #endregion

        #region public DbConnection 当前数据库连接实例
        //数据库连接
        private DbConnection dbConnection = null;
        /// <summary>
        /// 数据库连接
        /// </summary>
        public DbConnection DbConnection
        {
            get
            {
                if (this.dbConnection == null)
                {
                    // 若没打开，就变成自动打开关闭的
                    this.Open();
                    this.AutoOpenClose = true;
                }
                return this.dbConnection;
            }
            set
            {
                this.dbConnection = value;
            }
        }
        #endregion

        #region public DbCommand 当前数据库执行的命令
        private DbCommand dbCommand = null;
        /// <summary>
        /// 命令
        /// </summary>
        public DbCommand DbCommand
        {
            get
            {
                return this.dbCommand;
            }

            set
            {
                this.dbCommand = value;
            }
        }
        #endregion

        //属性：接口实现
        #region public bool AutoOpenClose 是否自动打开或关闭
        /// <summary>
        /// 默认打开关闭数据库选项（默认为否）
        /// </summary>
        private bool autoOpenClose = false; //默认打开关闭数据库选项（默认为否）        
        public bool AutoOpenClose
        {
            get
            {
                return autoOpenClose;
            }
            set
            {
                autoOpenClose = value;
            }
        }
        #endregion

        #region  public string ConnectionString 数据库连接字符串
        private string connectionString = "Data Source=localhost;Initial Catalog=UserCenterV36;Integrated Security=SSPI;";
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }
        #endregion

        #region public bool InTransaction 是否已在事务中
        /// <summary>
        /// 是否已采用事务
        /// </summary>
        private bool inTransaction = false; // 是否已在事务之中

        public bool InTransaction
        {
            get
            {
                return this.inTransaction;
            }

            set
            {
                this.inTransaction = value;
            }
        }
        #endregion

        //sql安全性及参数：接口实现
        #region public virtual string SqlSafe(string value) 检查sql参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>安全的参数</returns>
        public virtual string SqlSafe(string value)
        {
            value = value.Replace("'", "''");
            return value;
        }
        #endregion

        #region public virtual string PlusSign() 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <returns>字符加</returns>
        public virtual string PlusSign()
        {
            return " + ";
        }
        #endregion

        #region public virtual string PlusSign(params string[] values) 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>字符加</returns>
        public virtual string PlusSign(params string[] values)
        {
            string returnValue = string.Empty;
            for (int i = 0; i < values.Length; i++)
            {
                returnValue += values[i] + PlusSign();
            }
            if (!String.IsNullOrEmpty(returnValue))
            {
                returnValue = returnValue.Substring(0, returnValue.Length - 3);
            }
            else
            {
                returnValue = PlusSign();
            }
            return returnValue;
        }
        #endregion

        //数据库操作：接口实现
        #region public virtual IDbConnection GetDbConnection() 获取数据库连接
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection GetDbConnection()
        {
            return this.dbConnection;
        }
        #endregion

        #region public virtual IDbTransaction GetDbTransaction() 获取数据源上执行的事务
        /// <summary>
        /// 获取数据源上执行的事务
        /// </summary>
        /// <returns>数据源上执行的事务</returns>
        public virtual IDbTransaction GetDbTransaction()
        {
            return this.dbTransaction;
        }
        #endregion

        #region public virtual IDbCommand GetDbCommand() 获取数据源上命令
        /// <summary>
        /// 获取数据源上命令
        /// </summary>
        /// <returns>数据源上命令</returns>
        public virtual IDbCommand GetDbCommand()
        {
            return this.DbConnection.CreateCommand();
        }
        #endregion

        #region public virtual IDbConnection Open() 打开数据库
        /// <summary>
        /// 这时主要的获取数据库连接的方法
        /// </summary>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection Open()
        {
            // 写入调试信息
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //#endif

            // 这里是获取一个连接的详细方法
            if (String.IsNullOrEmpty(this.ConnectionString))
            {
                // 是否静态数据库里已经设置了连接？

                /*
                if (!string.IsNullOrEmpty(DbHelper.ConnectionString))
                {
                    this.ConnectionString = DbHelper.ConnectionString;
                }
                else
                {
                     读取配置文件？
                }
                */

                //gf修改，如果当前连接字符串为空应该怎么做？

            }

            this.Open(this.ConnectionString);

            // 写入调试信息
            //#if (DEBUG)
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            return this.dbConnection;
        }
        #endregion

        #region public virtual IDbConnection Open(string connectionString) 打开数据库
        /// <summary>
        /// 获得新的数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        public virtual IDbConnection Open(string connectionString)
        {
            // 写入调试信息
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " +
            //            MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            //try
            //{
            //注意，在整体的异常处理放在DAO层的情况下，由于DAO层的读写每个动作都是先open，再close，因此，open动作不应该添加try语句包例外封装在这里。
            //而是应该在open异常的情况下，马上就能把异常反应到外层，从而使得DAO层面可以及时停止当前操作。同时可以使得DAO可以获得正确的exception信息
            //推而广之，在整体的异常处理放在DAO层的情况下，dbhelper任何位置都不应该有try语句

            // 若是空的话才打开
            if (this.dbConnection == null || this.dbConnection.State == ConnectionState.Closed)
            {
                this.ConnectionString = connectionString;
                this.dbConnection = GetInstance().CreateConnection();
                this.dbConnection.ConnectionString = this.ConnectionString;
                //Console.WriteLine(this.dbConnection.ConnectionString);
                this.dbConnection.Open();

                // 创建对象
                // this.dbCommand = this.DbConnection.CreateCommand();
                // this.dbCommand.Connection = this.dbConnection;
                // this.dbDataAdapter = this.dbProviderFactory.CreateDataAdapter();
                // this.dbDataAdapter.SelectCommand = this.dbCommand;

                // 写入调试信息
                //#if (DEBUG)
                //                int milliEnd = Environment.TickCount;
                //                Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " +
                //                TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " +
                //                MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
                //#endif                   
            }
            this.AutoOpenClose = false;
            return this.dbConnection;
            //}
            //catch (Exception ex)
            //{
            //    this.dbConnection = null;
            //   return this.dbConnection;
            //}
        }
        #endregion

        #region public void Close() 关闭数据库连接
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            // 写入调试信息:单线程、多线程比较执行效率，必须关闭向控制台写信息的功能。向控制台写log会较大的消耗系统资源，形成执行瓶颈。
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            if (this.dbConnection != null)
            {
                this.dbConnection.Close();
                this.dbConnection.Dispose();
            }

            this.Dispose();

            // 写入调试信息:单线程、多线程比较执行效率，必须关闭向控制台写信息的功能。向控制台写log会较大的消耗系统资源，形成执行瓶颈。
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif
        }
        #endregion

        //事务管理：接口实现
        #region public IDbTransaction BeginTransaction() 事务开始
        /// <summary>
        /// 事务开始
        /// </summary>
        /// <returns>事务</returns>
        public IDbTransaction BeginTransaction()
        {
            // 写入调试信息
#if (DEBUG)
            int milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            if (!this.InTransaction)
            {
                this.InTransaction = true;
                this.dbTransaction = this.DbConnection.BeginTransaction();
                // this.dbCommand.Transaction = this.dbTransaction;
            }

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            return this.dbTransaction;
        }
        #endregion

        #region public void CommitTransaction() 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            // 写入调试信息
#if (DEBUG)
            int milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            if (this.InTransaction)
            {
                // 事务已经完成了，一定要更新标志信息
                this.InTransaction = false;
                this.dbTransaction.Commit();
            }

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif
        }
        #endregion

        #region public void RollbackTransaction() 回滚事务
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            // 写入调试信息
#if (DEBUG)
            int milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            if (this.InTransaction)
            {
                this.InTransaction = false;
                this.dbTransaction.Rollback();
            }

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif
        }
        #endregion

        //sql查询：接口实现
        #region public virtual IDataReader ExecuteReader(string commandText) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>结果集流</returns>
        public virtual IDataReader ExecuteReader(string commandText)
        {
            // 写入调试信息
#if (DEBUG)
            int milliStart = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }

            this.dbCommand = this.DbConnection.CreateCommand();
            this.dbCommand.CommandType = CommandType.Text;
            this.dbCommand.CommandText = commandText;

            DbDataReader dbDataReader = null;
            dbDataReader = this.dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // 写入调试信息
#if (DEBUG)
            int milliEnd = Environment.TickCount;
            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
#endif

            // 写入日志
            this.WriteLog(commandText);

            return dbDataReader;
        }
        #endregion

        #region public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters); 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>结果集流</returns>
        public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters)
        {
            return this.ExecuteReader(commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>结果集流</returns>
        public virtual IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }
            //GF测试：如果已经打开，则需要先关闭
            //else {
            //    this.Close();
            //    this.Open();
            //}

            this.dbCommand = this.DbConnection.CreateCommand();
            this.dbCommand.CommandText = commandText;
            this.dbCommand.CommandType = commandType;
            if (this.dbTransaction != null)
            {
                this.dbCommand.Transaction = this.dbTransaction;
            }

            if (dbParameters != null)
            {
                this.dbCommand.Parameters.Clear();
                for (int i = 0; i < dbParameters.Length; i++)
                {
                    if (dbParameters[i] != null)
                    {
                        this.dbCommand.Parameters.Add(dbParameters[i]);
                    }
                }
            }

            // 这里要关闭数据库才可以的
            DbDataReader dbDataReader = null;
            dbDataReader = this.dbCommand.ExecuteReader(CommandBehavior.CloseConnection);

            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 写入日志
            this.WriteLog(commandText);

            return dbDataReader;
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText) 执行查询, SQL BUILDER 用了这个东西？参数需要保存, 不能丢失.
        /// <summary>
        /// 执行查询, SQL BUILDER 用了这个东西？参数需要保存, 不能丢失.
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText)
        {
            // 写入调试信息:向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //           int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }

            this.dbCommand = this.DbConnection.CreateCommand();
            this.dbCommand.CommandType = CommandType.Text;
            this.dbCommand.CommandText = commandText;
            if (this.InTransaction)
            {
                this.dbCommand.Transaction = this.dbTransaction;
            }

            int returnValue = this.dbCommand.ExecuteNonQuery();

            // 自动关闭
            if (this.AutoOpenClose)
            {
                this.Close();
            }

            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 写入日志
            this.WriteLog(commandText);
            return returnValue;
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters)
        {
            return this.ExecuteNonQuery(commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return this.ExecuteNonQuery(this.dbTransaction, commandText, null, commandType);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            return this.ExecuteNonQuery(this.dbTransaction, commandText, dbParameters, commandType);
        }
        #endregion

        #region public virtual int ExecuteNonQuery(IDbTransaction transaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteNonQuery(IDbTransaction transaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }

            this.dbCommand = this.DbConnection.CreateCommand();
            this.dbCommand.CommandText = commandText;
            this.dbCommand.CommandType = commandType;
            if (this.dbTransaction != null)
            {
                this.dbCommand.Transaction = this.dbTransaction;
            }
            if (dbParameters != null)
            {
                this.dbCommand.Parameters.Clear();
                for (int i = 0; i < dbParameters.Length; i++)
                {
                    // if (dbParameters[i] != null)
                    //{
                    this.dbCommand.Parameters.Add(dbParameters[i]);
                    //}
                }
            }
            int returnValue = this.dbCommand.ExecuteNonQuery();

            // 自动关闭
            if (this.AutoOpenClose)
            {
                this.Close();
            }
            else
            {
                this.dbCommand.Parameters.Clear();
            }

            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //           Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 写入日志
            this.WriteLog(commandText);

            return returnValue;
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <returns>object</returns>
        public virtual object ExecuteScalar(string commandText)
        {
            return this.ExecuteScalar(commandText, null, CommandType.Text);
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>Object</returns>
        public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters)
        {
            return this.ExecuteScalar(commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>Object</returns>
        public virtual object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            return this.ExecuteScalar(this.dbTransaction, commandText, dbParameters, commandType);
        }
        #endregion

        #region public virtual object ExecuteScalar(IDbTransaction transaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 执行查询
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>Object</returns>
        public virtual object ExecuteScalar(IDbTransaction transaction, string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }

            this.dbCommand = this.DbConnection.CreateCommand();
            this.dbCommand.CommandText = commandText;
            this.dbCommand.CommandType = commandType;
            if (this.dbTransaction != null)
            {
                this.dbCommand.Transaction = this.dbTransaction;
            }
            if (dbParameters != null)
            {
                this.dbCommand.Parameters.Clear();
                for (int i = 0; i < dbParameters.Length; i++)
                {
                    if (dbParameters[i] != null)
                    {
                        this.dbCommand.Parameters.Add(dbParameters[i]);
                    }
                }
            }
            object returnValue = this.dbCommand.ExecuteScalar();

            // 自动关闭
            if (this.AutoOpenClose)
            {
                this.Close();
            }
            else
            {
                this.dbCommand.Parameters.Clear();
            }

            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 写入日志
            this.WriteLog(commandText);
            return returnValue;
        }
        #endregion

        #region public virtual DataTable Fill(string commandText) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">查询</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText)
        {
            DataTable dataTable = new DataTable("DotNet");
            return this.Fill(dataTable, commandText, null, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(DataTable dataTable, string commandText) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="dataTable">目标数据表</param>
        /// <param name="commandText">查询</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(DataTable dataTable, string commandText)
        {
            return this.Fill(dataTable, commandText, null, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters)
        {
            DataTable dataTable = new DataTable("DotNet");
            return this.Fill(dataTable, commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(DataTable dataTable, string commandText, IDbDataParameter[] dbParameters) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="dataTable">目标数据表</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(DataTable dataTable, string commandText, IDbDataParameter[] dbParameters)
        {
            return this.Fill(dataTable, commandText, dbParameters, CommandType.Text);
        }
        #endregion

        #region public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            DataTable dataTable = new DataTable("DotNet");
            return this.Fill(dataTable, commandText, dbParameters, commandType);
        }
        #endregion

        #region public virtual DataTable Fill(DataTable dataTable, string commandText, IDbDataParameter[] dbParameters, CommandType commandType) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="dataTable">目标数据表</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>数据表</returns>
        public virtual DataTable Fill(DataTable dataTable, string commandText, IDbDataParameter[] dbParameters, CommandType commandType)
        {
            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }

            using (this.dbCommand = this.DbConnection.CreateCommand())
            {
                this.dbCommand.CommandTimeout = this.DbConnection.ConnectionTimeout;
                this.dbCommand.CommandText = commandText;
                this.dbCommand.CommandType = commandType;
                if (this.InTransaction)
                {
                    // 这个不这么写，也不行，否则运行不能通过的
                    this.dbCommand.Transaction = this.dbTransaction;
                }
                this.dbDataAdapter = this.GetInstance().CreateDataAdapter();
                this.dbDataAdapter.SelectCommand = this.dbCommand;
                if ((dbParameters != null) && (dbParameters.Length > 0))
                {
                    this.dbCommand.Parameters.AddRange(dbParameters);
                }
                this.dbDataAdapter.Fill(dataTable);
                this.dbDataAdapter.SelectCommand.Parameters.Clear();
            }

            // 自动关闭
            if (this.AutoOpenClose)
            {
                this.Close();
            }

            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " +
            //                TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " +
            //                MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 写入日志
            this.WriteLog(commandText);
            return dataTable;
        }
        #endregion

        #region public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName) 填充数据权限
        /// <summary>
        /// 填充数据权限
        /// </summary>
        /// <param name="dataSet">目标数据权限</param>
        /// <param name="commandText">查询</param>
        /// <param name="tableName">填充表</param>
        /// <returns>数据权限</returns>
        public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName)
        {
            return this.Fill(dataSet, CommandType.Text, commandText, tableName, null);
        }
        #endregion

        #region public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters) 填充数据权限
        /// <summary>
        /// 填充数据权限
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="tableName">填充表</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据权限</returns>
        public virtual DataSet Fill(DataSet dataSet, string commandText, string tableName, IDbDataParameter[] dbParameters)
        {
            return this.Fill(dataSet, CommandType.Text, commandText, tableName, dbParameters);
        }
        #endregion

        #region public virtual DataSet Fill(DataSet dataSet, CommandType commandType, string commandText, string tableName, IDbDataParameter[] dbParameters) 填充数据权限
        /// <summary>
        /// 填充数据权限
        /// </summary>
        /// <param name="dataSet">数据权限</param>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="tableName">填充表</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据权限</returns>
        public virtual DataSet Fill(DataSet dataSet, CommandType commandType, string commandText, string tableName, IDbDataParameter[] dbParameters)
        {
            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliStart = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " :Begin: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 自动打开
            if (this.DbConnection == null)
            {
                this.AutoOpenClose = true;
                this.Open();
            }
            else if (this.DbConnection.State == ConnectionState.Closed)
            {
                this.Open();
            }

            using (this.dbCommand = this.DbConnection.CreateCommand())
            {
                //this.dbCommand.Parameters.Clear();
                //if ((dbParameters != null) && (dbParameters.Length > 0))
                //{
                //    for (int i = 0; i < dbParameters.Length; i++)
                //    {
                //        if (dbParameters[i] != null)
                //        {
                //            this.dbDataAdapter.SelectCommand.Parameters.Add(dbParameters[i]);
                //        }
                //    }
                //}
                this.dbCommand.CommandText = commandText;
                this.dbCommand.CommandType = commandType;
                if ((dbParameters != null) && (dbParameters.Length > 0))
                {
                    this.dbCommand.Parameters.AddRange(dbParameters);
                }

                this.dbDataAdapter = this.GetInstance().CreateDataAdapter();
                this.dbDataAdapter.SelectCommand = this.dbCommand;
                this.dbDataAdapter.Fill(dataSet, tableName);

                if (this.AutoOpenClose)
                {
                    this.Close();
                }
                else
                {
                    this.dbDataAdapter.SelectCommand.Parameters.Clear();
                }
            }

            // 写入调试信息：向IDE输出调试信息很耗时，影响测试接口速度的准确性，因此需要注释掉
            //#if (DEBUG)
            //            int milliEnd = Environment.TickCount;
            //            Trace.WriteLine(DateTime.Now.ToString(BaseDbInfo.TimeFormat) + " Ticks: " + TimeSpan.FromMilliseconds(milliEnd - milliStart).ToString() + " :End: " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name);
            //#endif

            // 写入日志
            this.WriteLog(commandText);
            return dataSet;
        }
        #endregion

        #region public virtual int ExecuteProcedure(string procedureName) 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName">存储过程</param>
        /// <returns>int</returns>
        public virtual int ExecuteProcedure(string procedureName)
        {
            return this.ExecuteNonQuery(procedureName, null, CommandType.StoredProcedure);
        }
        #endregion

        #region public virtual int ExecuteProcedure(string procedureName, IDbDataParameter[] dbParameters) 执行代参数的存储过程
        /// <summary>
        /// 执行代参数的存储过程
        /// </summary>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>影响行数</returns>
        public virtual int ExecuteProcedure(string procedureName, IDbDataParameter[] dbParameters)
        {
            return this.ExecuteNonQuery(procedureName, dbParameters, CommandType.StoredProcedure);
        }
        #endregion

        #region public virtual DataTable ExecuteProcedureForDataTable(string procedureName, string tableName, IDbDataParameter[] dbParameters) 执行存储过程返回数据表
        /// <summary>
        /// 执行存储过程返回数据表
        /// </summary>
        /// <param name="procedureName">存储过程</param>
        /// <param name="tableName">填充表</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>数据权限</returns>
        public virtual DataTable ExecuteProcedureForDataTable(string procedureName, string tableName, IDbDataParameter[] dbParameters)
        {
            DataTable dataTable = new DataTable(tableName);
            this.Fill(dataTable, procedureName, dbParameters, CommandType.StoredProcedure);
            return dataTable;
        }
        #endregion

        #region public void SqlBulkCopyData(DataTable dataTable) 批量写入数据
        /// <summary>
        /// 使用mysql的bulkcopy接口批量写入数据
        /// </summary>
        /// <param name="dataTable">要写入的数据表</param>
        /// <returns></returns>
        public void SqlBulkCopyData(DataTable dataTable)
        {
        }
        #endregion
        //系统其他：接口实现
        #region public virtual void WriteLog(string commandText, string fileName = null) 写入sql查询句日志

        /// <summary>
        /// 写入sql查询句日志
        /// </summary>
        /// <param name="commandText"></param>
        public virtual void WriteLog(string commandText)
        {
            string fileName = DateTime.Now.ToString(BaseDbInfo.DateFormat) + " _ " + this.FileName;
            WriteLog(commandText, fileName);
        }

        /// <summary>
        /// 写入sql查询句日志
        /// </summary>
        /// <param name="commandText">异常</param>
        /// <param name="fileName">文件名</param>
        public virtual void WriteLog(string commandText, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString(BaseDbInfo.DateFormat) + " _ " + this.FileName;
            }
            string returnValue = string.Empty;
            // 系统里应该可以配置是否记录异常现象
            if (!BaseDbInfo.LogSQL)
            {
                return;
            }
            // 将异常信息写入本地文件中
            string logDirectory = BaseDbInfo.StartupPath + @"\\Log\\Query";
            if (!System.IO.Directory.Exists(logDirectory))
            {
                System.IO.Directory.CreateDirectory(logDirectory);
            }
            string writerFileName = logDirectory + "\\" + fileName;
            if (!File.Exists(writerFileName))
            {
                FileStream FileStream = new FileStream(writerFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                FileStream.Close();
            }
            StreamWriter streamWriter = new StreamWriter(writerFileName, true, Encoding.Default);
            streamWriter.WriteLine(DateTime.Now.ToString(BaseDbInfo.DateTimeFormat) + " " + commandText);
            streamWriter.Close();
        }
        #endregion

        #region public void Dispose() 内存回收
        /// <summary>
        /// 内存回收
        /// </summary>
        public void Dispose()
        {
            if (this.dbCommand != null)
            {
                this.dbCommand.Dispose();
            }
            if (this.dbDataAdapter != null)
            {
                this.dbDataAdapter.Dispose();
            }
            if (this.dbTransaction != null)
            {
                this.dbTransaction.Dispose();
            }
            // 关闭数据库连接
            if (this.dbConnection != null)
            {
                if (this.dbConnection.State != ConnectionState.Closed)
                {
                    this.dbConnection.Close();
                    this.dbConnection.Dispose();
                }
            }
            this.dbConnection = null;
        }
        #endregion
    }
}
