using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace DBInterface.RTDBInterface
{
    /// <summary>
    /// BaseRDbHelper
    /// 实时数据库访问层基础类。
    /// 
    /// 版本：1.0
    ///  
    /// 修改纪录
    ///     
    ///     2016.12.18 版本：1.0 arrow  实现IDisposable接口。   
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.16</date>
    /// </author> 
    /// </summary>
    public class BaseRTDbHelper
    {
        //当前实时数据库类型。该参数必须在具体的实时数据库连接方法中重定义。
        /// <summary>
        /// 当前实时数据库类型，只读。该参数必须在具体的实时数据库连接方法中重定义，用来指定实际实现的数据库连接方法是哪种实时数据库的连接方法。
        /// 该参数只读，意味着外部RTDBHelper中的实时数据库类型参数并不改写具体数据库连接方法类的该参数。
        /// 实际上，RTDBHelper通过外部实时数据库类型参数，直接用反射的方法去寻找对应类型的数据库连接方法。
        /// </summary>
        public virtual CurrentRTDbType CurrentRTDbType
        {
            get
            {
                return CurrentRTDbType.Golden;  //由具体的实时数据库Helper重定义，默认返回Golden
            }
        }
        
        // 异常信息        
        public virtual string Exception
        {
            get
            {
                return Exception;
            }

        }

        // 数据库连接字符串
        private  string connectionString ;
        /// <summary>
        /// 数据库连接字符串
        /// -在PGIM数据库中，连接数据库函数不直接使用连接字符串连接，因此在PGIM中需要对set重写。当发生set时，对连接字符串进行解析
        /// </summary>
        public virtual string ConnectionString
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

        // sql查询日志文件名称。该参数必须在具体的实时数据库连接方法中重定义。
        public string FileName = "BaseRDbHelper.txt";    // sql查询句日志

        //默认打开关闭数据库选项（默认为否）
        private bool autoOpenClose = false;
        /// <summary>
        /// 默认打开关闭数据库选项（默认为否）
        /// </summary>
        public bool AutoOpenClose
        {
            get
            {
                return this.autoOpenClose;
            }
            set
            {
                this.autoOpenClose = value;
            }
        }


        #region public void Dispose() 内存回收
        /// <summary>
        /// 内存回收
        /// </summary>
        public void Dispose()
        {

        }
        #endregion
                      
    }
}
