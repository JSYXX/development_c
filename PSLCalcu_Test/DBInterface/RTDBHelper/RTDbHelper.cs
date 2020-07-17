using System;
using System.Collections.Generic;   //使用List<>
using PCCommon;                     //使用PValue类
using Config;

namespace DBInterface.RTDBInterface
{
    /// <summary>
    /// RTDbHelper
    /// 有关实时数据库连接的方法。
    /// 
    /// 版本：1.0
    /// 
    /// 特别说明：
    ///     1、对于使用原始数据进行概化统计计算，一定要使用GetRawValues()获得实时数据库的原始值进行计算，而不能使用插值。因为各个数据库的插值方式不同。
    ///     2、关于RTDBHelper的异常的处理。
    ///     ——总体原则是不在RTDBHelper进行异常处理。异常处理放到DAO层。
    ///     ——与RDB在接口层面有标准的ODBC可以传递异常相比，RTDB底层的（无论是pgim或者是golden）异常，都无法直接传递到rtdbhelper。因此DAO层无法通过try语句直接看到底层驱动（pgim.net\golen.net）产生的异常信息。
    ///     ——为了解决上述问题，采用的方式是，在RTDBhelper层，添加异常信息字段。在底层（各个helper接口）中，对异常信息进行捕捉，放入到异常信息字段中。然后继续传递异常。    
    ///     3、特别注意，rdbhelper的方式与rtdb不同。 
    ///
    /// 
    /// 修改纪录
    ///     
    ///		2016.12.18 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.18</date>
    /// </author> 
    /// </summary>
    public class RTDbHelper
    {
        /// <summary>
        /// 实时数据库类型        
        /// </summary>
        //1、放到计算引擎中使用时，引用config中的参数。
        //应用config参数，请使用Config命名空间。using Config;
         public static CurrentRTDbType RTDbType = (CurrentRTDbType)Enum.Parse(typeof(CurrentRTDbType), APPConfig.rtdb_Type);
        //测试用，golden实时数据库
        //public static CurrentRTDbType RTDbType = CurrentRTDbType.Golden;
        //测试用，PGIM实时数据库
        //public static CurrentRTDbType RTDbType = CurrentRTDbType.PGIM;        
        //测试用，XIANDB实时数据库
        //public static CurrentRTDbType RTDbType = CurrentRTDbType.XIANDB;

        /// <summary>
        /// 实时数据库连接串        
        /// </summary>
        //1、放到计算引擎中使用时，引用config中的参数
        //应用config参数，请使用Config命名空间。using Config;
        public static string RTDbConnection = APPConfig.rtdb_connString;
        //测试用，golden实时数据库连接字符串
        //public static string RTDbConnection = "server=192.168.1.56;port=6327;username=sa;password=golden;";
        //public static string RTDbConnection = "server=192.168.1.54;port=6327;username=sa;password=golden;pooling=true;maxpoolsize=13;minpoolsize=4";
        //测试用，PGIM实时数据库连接字符串
        //public static string RTDbConnection = "server=localhost;port=6327;username=administrator;password=PlantConnect;";
        // public static string RTDbConnection = "server=JSY_RTDBServer;port=6327;username=administrator;password=PlantConnect;";
        //测试用，PI连接字符串
        //public static string RTDbConnection = "server=192.168.1.54;port=5450;username=pidemo;password=;";
        //测试用，XIANDBHelper连接字符串
        //public static string RTDbConnection = "server=192.168.1.54;port=12084;username=admin;password=admin";

        //实时数据库 连接方法对象 rtdbHelper
        private readonly IRTDbHelper rtdbHelper;

        /// <summary> 构造函数
        /// 构造函数，在创建实例时，从指定的实时数据库连接对象工厂中创建一个实例，并返回给rtdbHelper
        /// </summary>
        public RTDbHelper()
        {
            rtdbHelper = RTDbHelperFactory.GetHelper(RTDbType, RTDbConnection);
        }

        /// <summary> 当前数据库类型
        /// 当前数据库类型
        /// </summary>
        public CurrentRTDbType CurrentRTDbType
        {
            get
            {
                return rtdbHelper.CurrentRTDbType;
            }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception 
        {
            get
            {
                return rtdbHelper.Exception;
            }
           
        }
        //数据库服务相关
        #region 数据库连接测试 public string ConnTest()
        public string ConnTest()
        {
            string connStatus = "failed";
            string DbConnection = rtdbHelper.Logon();
            if (DbConnection == "logon") connStatus = "sucess";
            rtdbHelper.Logoff();
            return connStatus;
        }
        #endregion

        #region 获得服务器时间：GetHostDateTime()
        /// <summary>
        ///  获得服务器时间
        ///  ——Golden支持。PGIM不支持，仅返回客户端时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetHostDateTime()
        {
            rtdbHelper.Logon();
            DateTime serverdate = rtdbHelper.GetHostDateTime();
            return serverdate;
        }
        #endregion

        //数据库连接
        #region 数据库连接（如果有连接池，尽量使用连接池技术）
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns>连接是否成功</returns>
        public bool Logon()
        {
            string DbConnection = rtdbHelper.Logon();
            if (DbConnection == "logon")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        //数据库关闭
        #region 数据库关闭（如果使用连接池技术，在退出程序前，一定要关闭数据库连接。以便释放连接池。）
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        public void Logoff()
        {
            rtdbHelper.Logoff();
        }
        #endregion

        //检查标签点是否存在
        #region 检查标签点在实时数据库中是否存在 public  List<String> CheckTags(string[] tagnames)
        public List<string> CheckTags(string[] tagnames)
        /// <summary>
        /// 检查标签点在实时数据库中是否存在
        /// </summary>
        /// <returns>在数据库中不存在的标签列表</returns>
        {
            List<string> results = new List<string>();
            rtdbHelper.Logon();
            results = rtdbHelper.CheckTags(tagnames);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return results;
        }
        #endregion
        //获取标签名称列表
        #region 获取标签名称列表
        public List<SignalComm> GetTagList(string tagfilter,string tablename="")
        /// <summary>
        /// 获取标签名称列表
        /// </summary>
        /// <returns>数据库中所有标签的列表值</returns>
        {
            List<SignalComm> results = new List<SignalComm>();
            rtdbHelper.Logon();
            string tagfileter = "";
            results = rtdbHelper.GetTagList(tagfilter,tablename);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return results;
        }
        #endregion

        //读取快照值
        #region 读取实时数据库标签的当前值（快照值、即时值）：public List<PValue> GetActValues(string[] tagnames)
        /// <summary>
        /// 读取数据库标签的当前值（快照值、即时值）
        /// </summary>
        /// <returns>当前值PValue</returns>
        public List<PValue> GetRawValues(string[] tagnames)
        {
            List<PValue> pvalues = new List<PValue>();
            rtdbHelper.Logon();
            pvalues = rtdbHelper.GetActValues(tagnames);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalues;
        }
        public List<PValue> GetRawValues(int[] tagids)
        {
            List<PValue> pvalues = new List<PValue>();
            rtdbHelper.Logon();
            pvalues = rtdbHelper.GetActValues(tagids);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalues;
        }
        #endregion

        //写入单个标签快照值
        #region 写入实时数据库标签的当前值（快照值、即时值）：public int SetActValue(string tagname,PValue pvalue)
        /// <summary>
        /// 读取数据库标签的当前值（快照值、即时值）
        /// </summary>
        /// <returns>当前值PValue</returns>
        public int SetActValue(string tagname, PValue pvalue)
        {

            rtdbHelper.Logon();
            int errorRecord = rtdbHelper.SetActValue(tagname, pvalue);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return errorRecord;
        }
        public int SetActValue(int tagid, PValue pvalue)
        {

            rtdbHelper.Logon();
            int errorRecord = rtdbHelper.SetActValue(tagid, pvalue);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return errorRecord;
        }
        #endregion

        //写入多个标签快照值
        #region 写入数据库标签的当前值（快照值、即时值）：public int SetActValues(string[] tagnames,List<PValue> pvalues)
        /// <summary>
        /// 写入多个标签的当前值（快照值、即时值）
        /// </summary>
        /// <returns>出错的数量</returns>
        public int SetActValues(string[] tagnames, List<PValue> pvalues)
        {

            rtdbHelper.Logon();
            int errorRecord = rtdbHelper.SetActValues(tagnames, pvalues);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return errorRecord;
        }
        public int SetActValues(int[] tagids, List<PValue> pvalues)
        {

            rtdbHelper.Logon();
            int errorRecord = rtdbHelper.SetActValues(tagids, pvalues);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return errorRecord;
        }
        #endregion

        //读取历史值
        #region 读取实时数据库标签的历史数据原始值：public List<PValue> GetRawValues(string tagnames, DateTime startDate, DateTime endDate)
        /// <summary>
        /// 读取数据库标签的历史数据原始值
        /// ——golden数据库一次读取的历史值不要超过20w条。如果超过22w条，golden数据接口会报错。
        /// ——pgim数据库一次读取的历史数据不要超过20w条。如果超过22w条，pgim数据接口会报错
        /// </summary>
        /// <returns>历史数据PValue</returns>
        public List<PValue> GetRawValues(string tagname, DateTime startDate, DateTime endDate)
        {
            List<PValue> pvalues = new List<PValue>();
            rtdbHelper.Logon();
            pvalues = rtdbHelper.GetRawValues(tagname, startDate, endDate);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalues;
        }
        public List<PValue> GetRawValues(int tagid, DateTime startDate, DateTime endDate)
        {
            List<PValue> pvalues = new List<PValue>();
            rtdbHelper.Logon();
            pvalues = rtdbHelper.GetRawValues(tagid, startDate, endDate);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalues;
        }
        #endregion

        #region 读取数据库标签的历史数据插值，固定数量：public List<PValue> GetIntervalValuesFixInterval(string tagnames, DateTime startDate, DateTime endDate, int interval)
        /// <summary>
        /// 读取数据库标签的历史数据插值:固定数量
        /// 这里特别注意：
        /// PGIM读取的插值，不是按前后两点的斜率计算的，而是按等于前一个时间点的值来考虑的。
        /// Golden读取的插值，是按照前后两点的斜率计算的插值。
        /// 因此，对于概化计算引擎，一定要用原始值来进行统计计算，而不要用插值来进行统计计算。
        /// </summary>
        /// <returns>历史数据PValue</returns>
        public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startDate, DateTime endDate, int interval)
        {
            List<PValue> pvalues = new List<PValue>();
            rtdbHelper.Logon();
            pvalues = rtdbHelper.GetIntervalValuesFixInterval(tagname, startDate, endDate, interval);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalues;
        }
        public List<PValue> GetIntervalValuesFixInterval(int tagid, DateTime startDate, DateTime endDate, int interval)
        {
            List<PValue> pvalues = new List<PValue>();
            rtdbHelper.Logon();
            pvalues = rtdbHelper.GetIntervalValuesFixInterval(tagid, startDate, endDate, interval);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalues;
        }
        #endregion

        #region 读取数据库标签的历史数据统计值：public List<PValue> GetSummaryValues(string tagname, DateTime startDate, DateTime endDate, string type)
        /// <summary>
        /// 读取数据库标签的历史数据统计值
        /// 这里特别注意：
        /// PGIM读取的插值，不是按前后两点的斜率计算的，而是按等于前一个时间点的值来考虑的。
        /// Golden读取的插值，是按照前后两点的斜率计算的插值。
        /// 因此，对于概化计算引擎，一定要用原始值来进行统计计算，而不要用插值来进行统计计算。
        /// </summary>
        /// <returns>历史数据PValue</returns>
        public PValue GetSummaryValues(string tagname, DateTime startDate, DateTime endDate, string type)
        {
            PValue pvalue = new PValue();
            rtdbHelper.Logon();
            pvalue = rtdbHelper.GetSummaryValues(tagname, startDate, endDate, type);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalue;
        }
        public PValue GetSummaryValues(int tagid, DateTime startDate, DateTime endDate, string type)
        {
            PValue pvalue = new PValue();
            rtdbHelper.Logon();
            pvalue = rtdbHelper.GetSummaryValues(tagid, startDate, endDate, type);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return pvalue;
        }
        #endregion

        //写入历史值
        #region 写入数据库标签的历史值：public int PutArchivedValues(string tagname, string[][] data)
        /// <summary>
        /// 读取数据库标签的历史数据统计值
        /// 这里特别注意：
        /// PGIM写入历史数据，一次最多5w条。超过6w条pgim接口会报错。
        /// Golden写入历史数据，一次最多10w条。超过10w条golden接口会报错。
        /// 因此，对于概化计算引擎，一定要用原始值来进行统计计算，而不要用插值来进行统计计算。
        /// </summary>
        /// <returns>历史数据PValue</returns>
        public int PutArchivedValues(string tagname, string[][] data)
        {
            rtdbHelper.Logon();
            int result = rtdbHelper.PutArchivedValues(tagname, data);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return result;
        }
        public int PutArchivedValues(int tagid, string[][] data)
        {
            rtdbHelper.Logon();
            int result = rtdbHelper.PutArchivedValues(tagid, data);
            rtdbHelper.Logoff();        //golden3.0采用连接池，每次使用完毕，使用logoff释放连接资源
            return result;
        }
        #endregion     
      

    }
}
