using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBInterface.RTDBInterface
{
  
    public class BaseRTDbInfo
    {
        /// <summary>
        /// BaseSystemInfo
        /// 这是基础信息部分
        /// 
        /// 版本：1.0
        ///            
        ///		修改记录
        ///     2016.12.16 版本：1.0 gaofeng    整理数据库接口基础信息。
        /// 
        /// <author>
        ///		<name>高峰</name>
        ///		<date>2016.12.16</date>
        /// </author>
        /// </summary>
        /// 

            #region 动态加载程序配置
            
            /// <summary>
            /// 数据库接口应用用程序集名称。（用于数据库接口工厂反射自身）
            /// </summary>            
            public static string DBInterfaceAssmely = "DBInterface";    //数据库接口dll文件名称，用于RTDbHelperFactory工厂反射时装载dll文件。
            public static string RTDbHelperNameSpace = "DBInterface.RTDBInterface"; //实时数据库接口所在的命名空间          

            #endregion
    }

}
