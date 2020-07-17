using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBInterface.RTDBInterface
{
    /// <summary>
    /// CurrentRTDbType
    /// 有关实时数据库连接类型定义。
    /// 
    /// 版本：1.0
    /// 
    /// 修改纪录
    /// 
    ///		2016.12.18 版本：1.0 arrow 调整“实时数据库类型”定义，调整“按照实时数据库类型获取实时数据库访问实现类”的函数。
    ///   
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.18</date>
    /// </author> 
    /// </summary>
    public enum CurrentRTDbType
    {
        /// <summary>
        /// 实时数据库类型：PGIM
        /// </summary>
        PGIM,
        /// <summary>
        /// 实时数据库类型：Golden
        /// </summary>
        Golden,
        /// <summary>
        /// 实时数据库类型：Pi
        /// </summary>
        PI,
        /// <summary>
        /// 实时数据库类型：XIANDB
        /// </summary>
        XIANDB,
        /// <summary>
        /// 实时数据库类型：TOM
        /// </summary>
        TOM
    }
    public class SupportRTDbType
    {
        /// <summary>
        /// 按数据库类型获取数据库访问实现类
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns>数据库访问实现类</returns>
        public static string GetRTDbHelperClass(CurrentRTDbType rdbType)
        {
            string returnValue = BaseRTDbInfo.RTDbHelperNameSpace + ".GoldenHelper";    //默认golden数据库
            switch (rdbType)
            {
                case CurrentRTDbType.PGIM:
                    returnValue = BaseRTDbInfo.RTDbHelperNameSpace + ".PGIMHelper";
                    break;
                case CurrentRTDbType.Golden:
                    returnValue = BaseRTDbInfo.RTDbHelperNameSpace + ".GoldenHelper";
                    break;
                case CurrentRTDbType.PI:
                    returnValue = BaseRTDbInfo.RTDbHelperNameSpace + ".PIHelper";
                    break;
                case CurrentRTDbType.XIANDB:
                    returnValue = BaseRTDbInfo.RTDbHelperNameSpace + ".XIANDBHelper";
                    break;
                case CurrentRTDbType.TOM:
                    returnValue = BaseRTDbInfo.RTDbHelperNameSpace + ".TOMHelper";
                    break;
            }
            return returnValue;
        }
    }
    public class SignalComm
    {
        public string tagname { get; set; }             //标签名称
        public string tagdesc { get; set; }             //标签描述
        public string tagunit { get; set; }
        public string tagmin { get; set; }
        public string tagmax { get; set; }
        public string tagdatetype { get; set; }

        public SignalComm(string tagname, string tagdesc)
        {
            this.tagname = tagname;
            this.tagdesc = tagdesc;
        }
        public SignalComm(string tagname, string tagdesc, string tagunit, string tagmin, string tagmax, string tagdatetype)
        {
            this.tagname = tagname;
            this.tagdesc = tagdesc;
            this.tagunit = tagunit;
            this.tagmin = tagmin;
            this.tagmax = tagmax;
            this.tagdatetype = tagdatetype;
        }
    }
}

