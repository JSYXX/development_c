using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace DBInterface.RDBInterface
{
    using MySql.Data.MySqlClient;

    /// <summary>
    /// MySqlHelper
    /// 有关数据库连接的方法。
    /// 
    /// 版本：1.0
    ///  
    /// 修改纪录
    ///     2017.06.23 版本：2.0 升级为MySql Connect Net 6.9.9，以便更好的支持Mysql Server 5.6和Mysql Server 5.7。
    ///     2016.12.23 版本：1.1 原版程序是针对MySql Connect Net 6.2.3写的，后将其更新为MySql Connect Net 6.5.7版，经测试可以正常运行。
    ///		2016.12.16 版本：1.0 arrow 创建。   
    ///		
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2017.06.23</date>    
    /// </author> 
    /// </summary>
    public class MySqlHelper : BaseDbHelper, IDbHelper
    {
        public override DbProviderFactory GetInstance()
        {
            return MySqlClientFactory.Instance;
        }

        #region public CurrentDbType 当前数据库类型
        public CurrentDbType CurrentDbType
        {
            get
            {
                return CurrentDbType.MySql; //MysqlHelper,操作的数据库类型是mysql！！
            }
        }
        #endregion

        #region public MySqlHelper() 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public MySqlHelper()
        {
            FileName = "MySqlHelper.txt";   // sql查询句日志
        }
        #endregion

        #region public MySqlHelper(string connectionString) 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connectionString">数据连接</param>
        public MySqlHelper(string connectionString)
            : this()
        {
            this.ConnectionString = connectionString;
        }
        #endregion

        #region public string GetDBNow() 获得数据库日期时间
        /// <summary>
        /// 获得数据库日期时间
        /// </summary>
        /// <returns>日期时间</returns>
        public string GetDBNow()
        {
            return " now() ";
        }
        #endregion

        #region public string GetDBDateTime() 获得数据库日期时间
        /// <summary>
        /// 获得数据库日期时间
        /// </summary>
        /// <returns>日期时间</returns>
        public string GetDBDateTime()
        {
            string commandText = " SELECT " + this.GetDBNow();
            this.Open();
            string dateTime = this.ExecuteScalar(commandText, null, CommandType.Text).ToString();
            this.Close();
            return dateTime;
        }
        #endregion

        #region public IDbDataParameter MakeInParam(string targetFiled, object targetValue) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeInParam(string targetFiled, object targetValue)
        {
            return new MySqlParameter(targetFiled, targetValue);
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string targetFiled, object targetValue) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数集</returns>
        public IDbDataParameter MakeParameter(string targetFiled, object targetValue)
        {
            IDbDataParameter dbParameter = null;
            if (targetFiled != null && targetValue != null)
            {
                dbParameter = this.MakeInParam(targetFiled, targetValue);
            }
            return dbParameter;
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="targetFiled">目标字段</param>
        /// <param name="targetValue">值</param>
        /// <returns>参数集</returns>
        public IDbDataParameter[] MakeParameters(string[] targetFileds, Object[] targetValues)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            if (targetFileds != null && targetValues != null)
            {
                for (int i = 0; i < targetFileds.Length; i++)
                {
                    if (targetFileds[i] != null && targetValues[i] != null)
                    {
                        dbParameters.Add(this.MakeInParam(targetFileds[i], targetValues[i]));
                    }
                }
            }
            return dbParameters.ToArray();
        }
        #endregion

        #region public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters) 获取参数
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <returns>参数集</returns>
        public IDbDataParameter[] MakeParameters(List<KeyValuePair<string, object>> parameters)
        {
            // 这里需要用泛型列表，因为有不合法的数组的时候
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.Key != null && parameter.Value != null && (!(parameter.Value is Array)))
                    {
                        dbParameters.Add(MakeParameter(parameter.Key, parameter.Value));
                    }
                }
            }
            return dbParameters.ToArray();
        }
        #endregion

        #region public IDbDataParameter MakeOutParam(string paramName, DbType DbType, int Size) 获取输出参数
        /// <summary>
        /// 获取输出参数
        /// </summary>
        /// <param name="paramName">目标字段</param>
        /// <param name="DbType">数据类型</param>
        /// <param name="Size">长度</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeOutParam(string paramName, DbType DbType, int Size)
        {
            return MakeParameter(paramName, null, DbType, Size, ParameterDirection.Output);
        }
        #endregion

        #region public IDbDataParameter MakeInParam(string paramName, DbType dbType, int size, object value) 获取输入参数
        /// <summary>
        /// 获取输入参数
        /// </summary>
        /// <param name="paramName">目标字段</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="size">长度</param>
        /// <param name="value">值</param>
        /// <returns>参数</returns>
        public IDbDataParameter MakeInParam(string paramName, DbType dbType, int size, object value)
        {
            return MakeParameter(paramName, value, dbType, size, ParameterDirection.Input);
        }
        #endregion

        #region public IDbDataParameter MakeParameter(string parameterName, object parameterValue, DbType dbType, Int32 parameterSize, ParameterDirection parameterDirection) 获取参数
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
            MySqlParameter parameter;

            if (parameterSize > 0)
            {
                parameter = new MySqlParameter(parameterName, (MySqlDbType)dbType, parameterSize);
            }
            else
            {
                parameter = new MySqlParameter(parameterName, (MySqlDbType)dbType);
            }

            parameter.Direction = parameterDirection;
            if (!(parameterDirection == ParameterDirection.Output && parameterValue == null))
            {
                parameter.Value = parameterValue;
            }

            return parameter;
        }
        #endregion

        #region public string GetParameter(string parameter) 获得参数Sql表达式
        /// <summary>
        /// 获得参数Sql表达式
        /// </summary>
        /// <param name="parameter">参数名称</param>
        /// <returns>字符串</returns>
        public string GetParameter(string parameter)
        {
            return " ?" + parameter;
        }
        #endregion

        #region string PlusSign(params string[] values) 获得Sql字符串相加符号
        /// <summary>
        ///  获得Sql字符串相加符号
        /// </summary>
        /// <param name="values">参数值</param>
        /// <returns>字符加</returns>
        public override string PlusSign(params string[] values)
        {
            string returnValue = string.Empty;
            returnValue = " CONCAT(";
            for (int i = 0; i < values.Length; i++)
            {
                returnValue += values[i] + " ,";
            }
            returnValue = returnValue.Substring(0, returnValue.Length - 2);
            returnValue += ")";
            return returnValue;
        }
        #endregion


    }
}
