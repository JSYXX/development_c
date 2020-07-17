using System.Reflection;

namespace DBInterface.RTDBInterface
{
    /// <summary>
    /// RDbHelperFactory
    /// 数据库服务工厂。
    /// 
    /// 修改纪录
    ///     
    ///		2016.12.26 版本：1.0 arrow 创建数据库服务工厂。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.26</date>
    /// </author> 
    /// </summary>
    class RTDbHelperFactory
    {
        //工厂提供两种方式创建数据库连接方法对象，目前外部仅通过第二个方法，创建连接对象方法

        #region 获取指定的实时数据库的连接方法，仅传入数据库连接字符串：public static IRTDbHelper GetHelper(string connectionString =null)
        /// <summary>
        /// 获取指定的实时数据库的连接方法
        /// </summary>
        /// <param name="connectionString">实时数据库连接串</param>
        /// <returns>实时数据库访问类</returns>
        public static IRTDbHelper GetHelper(string connectionString =null)
        {
            CurrentRTDbType rtdbType = CurrentRTDbType.Golden; //如果不给定数据库类型，默认使用Golden。
            return GetHelper(rtdbType, connectionString);
        }
        #endregion

        #region 获取指定的实时数据库的连接方法，传入数据库类型和连接字符串：public static IRTDbHelper GetHelper(CurrentRTDbType rtdbType = CurrentRTDbType.Golden, string connectionString = null)
        /// <summary>
        /// 获取指定的实时数据库的连接方法
        /// </summary>
        /// <param name="dataBaseType">实时数据库类型</param>
        /// <param name="connectionString">实时数据库连接串</param>
        /// <returns>实时数据库访问类</returns>
        public static IRTDbHelper GetHelper(CurrentRTDbType rtdbType = CurrentRTDbType.Golden, string connectionString = null)
        {   
            // 这里是每次都获取新的数据库连接,否则会有并发访问的问题存在
            string rtdbHelperClass = SupportRTDbType.GetRTDbHelperClass(rtdbType);  //根据数据库类型获得数据库接口类型的名字            
            
            IRTDbHelper rtdbHelper = (IRTDbHelper)Assembly.Load(BaseRTDbInfo.DBInterfaceAssmely).CreateInstance(rtdbHelperClass, true);
            //如果用反射的方法创建实例时出错，一定是dll文件名称BaseRTDbInfo.DBInterfaceAssmely不正确，或者命名空间+类名称rtdbHelperClass不正确
            //要重点检查BaseRTDbInfo.DBInterfaceAssmely和SupportRTDbType.GetRTDbHelperClass里面的的内容。
            //也就是检查dll文件名是否正确，每一个数据库连接类对应的命名空间+类名称是否正确。
            //在一个调试好的程序里，这里的创建过程是不会出错的，因此无需用try。            
            if (!string.IsNullOrEmpty(connectionString))
            {                
                rtdbHelper.ConnectionString = connectionString;                
            }            
            return rtdbHelper;
        }
        #endregion
    }
}
