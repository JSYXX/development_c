using System.Reflection;

namespace DBInterface.RDBInterface
{
    /// <summary>
    /// DbHelperFactory
    /// 数据库服务工厂。
    /// 
    /// 修改纪录
    /// 
    ///		2011.10.09 版本：2.1 arrow 改进直接用数据库连接就可以打开连接的方法。
    ///		2011.10.08 版本：2.0 arrow 支持多类型的多种数据库。
    ///		2011.09.18 版本：1.4 arrow 整理代码注释。
    ///		2011.03.30 版本：1.3 arrow 增加数据库连串的构造函数。
    ///		2009.07.23 版本：1.2 arrow 每次都获取一个新的数据库连接，解决并发错误问题。
    ///		2008.09.23 版本：1.1 arrow 优化改进为单实例模式。
    ///		2008.08.26 版本：1.0 arrow 创建数据库服务工厂。
    /// 
    /// 版本：2.1
    /// 
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2011.10.09</date>
    /// </author> 
    /// </summary>
    public class DbHelperFactory
    {
        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper GetHelper(string connectionString)
        {
            CurrentDbType dbType = CurrentDbType.MySql; //如果不给定数据库类型，默认使用MySql。
            return GetHelper(dbType, connectionString);
        }

        /// <summary>
        /// 获取指定的数据库连接
        /// </summary>
        /// <param name="dataBaseType">数据库类型</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <returns>数据库访问类</returns>
        public static IDbHelper GetHelper(CurrentDbType dbType = CurrentDbType.MySql, string connectionString = null)
        {
            // 这里是每次都获取新的数据库连接,否则会有并发访问的问题存在
            string dbHelperClass = SupportDbType.GetDbHelperClass(dbType);  //根据数据库类型获得数据库接口类型的名字
            IDbHelper dbHelper = (IDbHelper)Assembly.Load(BaseDbInfo.DBInterfaceAssmely).CreateInstance(dbHelperClass, true);
            //如果用反射的方法创建实例时出错，一定是dll文件名称BaseRTDbInfo.DBInterfaceAssmely不正确，或者命名空间+类名称dbHelperClass不正确
            //要重点检查BaseRTDbInfo.DBInterfaceAssmely和SupportDbType.GetDbHelperClass里面的的内容。
            //也就是检查dll文件名是否正确，每一个数据库连接类对应的命名空间+类名称是否正确。
            //在一个调试好的程序里，这里的创建过程是不会出错的，因此无需用try。
            if (!string.IsNullOrEmpty(connectionString))
            {
                dbHelper.ConnectionString = connectionString;
            }
            return dbHelper;
        }
    }
}