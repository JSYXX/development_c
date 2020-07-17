using System;

namespace DBInterface.RDBInterface
{
    public class BaseDbInfo
    {
        /// <summary>
        /// BaseSystemInfo
        /// 这是基础信息部分
        /// 
        /// 版本：1.0
        ///            
        ///		修改记录
        ///     2016.12.16 版本：1.0 arrow    整理数据库接口基础信息。
        /// 
        /// <author>
        ///		<name>arrow</name>
        ///		<date>2016.12.16</date>
        /// </author>
        /// </summary>
        /// 

            #region 动态加载程序配置
            
            /// <summary>
            /// 数据库接口应用用程序集名称。（用于数据库接口工厂反射自身）
            /// </summary>            
            public static string DBInterfaceAssmely = "DBInterface";    //数据库接口dll文件名称，用于DbHelperFactory工厂反射时装载dll文件。
            public static string RDbHelperNameSpace = "DBInterface.RDBInterface";   //关系数据库接口所在的命名空间    

            #endregion

            #region LOG功能需要的一些参数
            /// <summary>
            /// 是否记录数据库操作
            /// </summary>
            public static bool LogSQL = false;

            /// <summary>
            /// 目前的安装位置
            /// </summary>
            public static string StartupPath = string.Empty;

            /// <summary>
            /// 时间格式
            /// </summary>
            public static string TimeFormat = "HH:mm:ss";

            /// <summary>
            /// 日期短格式
            /// </summary>
            public static string DateFormat = "yyyy-MM-dd";

            /// <summary>
            /// 日期长格式
            /// </summary>
            public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            #endregion
          
    }
}
