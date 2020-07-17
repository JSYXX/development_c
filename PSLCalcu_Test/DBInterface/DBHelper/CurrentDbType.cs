using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBInterface.RDBInterface
{
    /// <summary>
    /// CurrentDbType
    /// 有关数据库连接类型定义。
    /// 
    /// 版本：1.0
    /// 
    /// 修改纪录
    /// 
    ///		2016.12.16 版本：1.0 arrow 调整“数据库类型”定义，调整“按照数据库类型获取数据库访问实现类”的函数。
    ///   
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2007.04.14</date>
    /// </author> 
    /// </summary>
    public enum CurrentDbType
    {
        /// <summary>
        /// 数据库类型：SqlServer
        /// </summary>
        SqlServer,
       
        /// <summary>
        /// 数据库类型：Oracle(用Oracle专用连接访问Oracle)
        /// </summary>
        Oracle,

        /// <summary>
        /// 数据库类型：Oracle(用Ms类访问Oracle)
        /// </summary>
        MsOracle,

        /// <summary>
        /// 数据库类型：MySql
        /// </summary>
        MySql,

        /// <summary>
        /// 数据库类型：DB2
        /// </summary>
        DB2,

        /// <summary>
        /// 数据库类型：SQLite
        /// </summary>
        SQLite,
        

        /// <summary>
        /// 数据库类型：Access
        /// </summary>
        Access
       
       
        
    }
    public class SupportDbType
    {
        /// <summary>
        /// 按数据库类型获取数据库访问实现类
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns>数据库访问实现类</returns>
        public static string GetDbHelperClass(CurrentDbType dbType)
        {
            string returnValue =  BaseDbInfo.RDbHelperNameSpace +".SqlHelper";
            switch (dbType)
            {
                case CurrentDbType.SqlServer:
                    returnValue = BaseDbInfo.RDbHelperNameSpace + ".SqlHelper";
                    break;
                case CurrentDbType.MsOracle:
                    returnValue = BaseDbInfo.RDbHelperNameSpace +".MSOracleHelper";
                    break;
                case CurrentDbType.MySql:
                    returnValue = BaseDbInfo.RDbHelperNameSpace +".MySqlHelper";
                    break;
                case CurrentDbType.DB2:
                    returnValue = BaseDbInfo.RDbHelperNameSpace +".DB2Helper";
                    break;
                case CurrentDbType.SQLite:
                    returnValue = BaseDbInfo.RDbHelperNameSpace +".SqLiteHelper";
                    break;
                case CurrentDbType.Access:
                     returnValue = BaseDbInfo.RDbHelperNameSpace +".OleDbHelper";
                    break;
            }
            return returnValue;
        }
    }
}
