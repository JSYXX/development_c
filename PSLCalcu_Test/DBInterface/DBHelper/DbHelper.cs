using System;
using System.Data;
using System.Data.Common;   //使用DbProviderFactory
using Config;

namespace DBInterface.RDBInterface
{
    /// <summary>
    /// DbHelper
    /// 有关数据库连接的方法。
    /// 
    /// 重要说明
    /// 在整体的异常处理放在DAO层的情况下，由于DAO层的读写每个动作都是先open，再close，因此，open动作不应该添加try语句包例外封装在这里。
    /// 而是应该在open异常的情况下，马上就能把异常反应到外层，从而使得DAO层面可以及时停止当前操作。同时可以使得DAO可以获得正确的exception信息
    /// 推而广之，在整体的异常处理放在DAO层的情况下，dbhelper任何位置都不应该有try语句
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     2017.06.22 版本：2.0 为并发连接，改进DbHelper为非静态类和方法。
    ///		2016.12.16 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.16</date>
    /// </author> 
    /// </summary>
    public class DbHelper
    {
        /// <summary>
        /// 关系数据库类型
        /// </summary>
        //1、放到计算引擎中使用时，引用config中的参数
        public static CurrentDbType DbType = (CurrentDbType)Enum.Parse(typeof(CurrentDbType), APPConfig.rdb_Type);
        //测试用：Mysql数据库
        //public static CurrentDbType DbType = CurrentDbType.MySql;  

        /// <summary>
        /// 关系数据库连接字符串
        /// </summary>
        //1、放到计算引擎中使用时，引用config中的参数
        public static string DbConnection = APPConfig.rdb_connString;
        //测试用：Mysql连接字符串
        //public static string DbConnection = "server=192.168.1.54;user id=root;password=mysql;database=test;min pool size=1;max pool size=20;Connection Lifetime=0;connection reset=false";

        //连接池参数说明：https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring%28v=VS.80%29.aspx
        //-pooling=true;                //是否开启连接池
        //-min pool size=5;             //连接池最小连接数
        //-max pool size=512;           //连接池最大连接数
        //-connection lifetime = 0;     //连接最大生存时间。当连接被返回到池时，将其创建时间与当前时间作比较，如果时间长度（以秒为单位）超出了由 Connection Lifetime 指定的值，该连接就会被销毁。
        //该值默认情况下是0，表示连接永远不会从池中被自动移走。
        //-connection reset=false;      //每次获取链接时，都进行reset_connection操作。经测试，这回造成每次从线程池中获取连接前复位连接。相当于每次重新建立连接。必须设定为false
        //-

        // 关系数据库 连接方法对象 dbHelper
        private readonly IDbHelper dbHelper;

        /// <summary> DbProviderFactory 实例
        /// DbProviderFactory实例
        /// 对外，提供工厂实例？？没有
        /// </summary>
        private DbProviderFactory factory = null;
        public DbProviderFactory Factory
        {
            get
            {
                if (factory == null)
                {
                    factory = dbHelper.GetInstance();
                }
                return factory;
            }
        }

        /// <summary> 构造函数
        /// 构造函数，在创建新实例时，从指定的数据库连接对象工厂中创建一个实例，并返回给dbHelper
        /// </summary>
        public DbHelper()
        {
            //使用工厂 创建 关系数据库 连接方法对象 dbHelper ，获得一个连接   
            //Console.WriteLine(DbConnection);
            dbHelper = DbHelperFactory.GetHelper(DbType, DbConnection);
        }

        //连接测试
        #region public string ConnTest() 数据库连接测试
        public string ConnTest()
        {
            string connStatus = "failed";
            IDbConnection DbConnection = dbHelper.Open();
            if (DbConnection != null) connStatus = "sucess";
            dbHelper.Close();
            return connStatus;
        }
        #endregion

        //属性
        #region public CurrentDbType 当前数据库类型
        /// <summary> 当前数据库类型
        /// 当前数据库类型
        /// </summary>
        public CurrentDbType CurrentDbType
        {
            get
            {
                return dbHelper.CurrentDbType;
            }
        }
        #endregion

        //服务器时间
        #region public string GetDBNow() 获得数据库服务器当前时间
        /// <summary>
        /// 获得数据库服务器当前时间
        /// </summary>
        /// <returns>时间</returns>
        public string GetDBNow()
        {
            return dbHelper.GetDBNow();
        }
        #endregion

        #region public string GetDBDateTime() 获得数据库服务器当前日期
        /// <summary>
        /// 获得数据库服务器当前日期
        /// </summary>
        /// <returns>日期</returns>
        public string GetDBDateTime()
        {
            return dbHelper.GetDBDateTime();
        }
        #endregion

        //sql语句处理
        #region public string SqlSafe(string value) 检查sql参数的安全性
        /// <summary>
        /// 检查参数的安全性
        /// </summary>
        /// <param name="value">参数</param>
        /// <returns>安全的参数</returns>
        public string SqlSafe(string value)
        {
            return dbHelper.SqlSafe(value);
        }
        #endregion

        #region public string PlusSign() Sql字符串加法
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <returns>字符加</returns>
        public string PlusSign()
        {
            return dbHelper.PlusSign();
        }
        #endregion

        #region public string PlusSign(params string[] values) Sql字符串加法
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>字符加</returns>
        public string PlusSign(params string[] values)
        {
            return dbHelper.PlusSign(values);
        }
        #endregion

        //sql语句参数
        #region public string GetParameter(string parameter) 获得参数Sql表达式
        /// <summary>
        /// 获得参数Sql表达式
        /// </summary>
        /// <param name="parameter">参数名称</param>
        /// <returns>字符串</returns>
        public string GetParameter(string parameter)
        {
            return dbHelper.GetParameter(parameter);
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string targetFiled, object targetValue) 生成sql单个参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeParameter(string targetFiled, object targetValue)
        {
            return dbHelper.MakeParameter(targetFiled, targetValue);
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues) 生成sql参数集
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数集</returns>
        public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues)
        {
            return dbHelper.MakeParameters(targetFileds, targetValues);
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection) 生成sql参数集
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterValue">值</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="parameterSize">大小</param>
        /// <param name="parameterDirection">输出方向</param>
        /// <returns>参数集</returns>
        public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection)
        {
            return dbHelper.MakeParameter(parameterName, parameterValue, dbType, parameterSize, parameterDirection);
        }
        #endregion

        //sql查询
        #region public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text) 执行返回DataReader数据集的查询
        /// <summary>
        /// 执行返回DataReader数据集的查询
        /// </summary>
        /// <param name="commandType">命令分类</param>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <returns>结果集流</returns>
        public IDataReader ExecuteReader(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text)
        {
            dbHelper.Open(DbConnection);
            dbHelper.AutoOpenClose = true;
            return dbHelper.ExecuteReader(commandText, dbParameters, commandType);

            //修改
            // IDataReader dataReader = dbHelper.ExecuteReader(commandText, dbParameters, commandType);
            // dbHelper.Close();
            // return dataReader;
        }
        #endregion

        #region public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text) 执行返回受影响行数的查询
        /// <summary>
        /// 执行返回受影响行数的查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>影响行数</returns>
        public int ExecuteNonQuery(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text)
        {
            int returnValue = 0;
            dbHelper.Open(DbConnection);
            returnValue = dbHelper.ExecuteNonQuery(commandText, dbParameters, commandType);
            dbHelper.Close();
            return returnValue;
        }
        #endregion

        #region public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text) 执行返回一个对象的查询
        /// <summary>
        /// 执行返回一个对象的查询
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text)
        {
            object returnValue = null;
            dbHelper.Open(DbConnection);
            returnValue = dbHelper.ExecuteScalar(commandText, dbParameters, commandType);
            dbHelper.Close();
            return returnValue;
        }
        #endregion

        #region public DataTable Fill(string commandText, IDbDataParameter[] dbParameters, CommandType commandType = CommandType.Text) 填充数据表
        /// <summary>
        /// 填充数据表
        /// </summary>
        /// <param name="commandText">sql查询</param>
        /// <param name="dbParameters">参数集</param>
        /// <param name="commandType">命令分类</param>
        /// <returns>数据表</returns>
        public DataTable Fill(string commandText, IDbDataParameter[] dbParameters = null, CommandType commandType = CommandType.Text)
        {
            DataTable dataTable = new DataTable("DotNet");
            dbHelper.Open(DbConnection);
            dbHelper.Fill(dataTable, commandText, dbParameters, commandType);
            dbHelper.Close();
            return dataTable;
        }
        #endregion
    }
}