using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using PCCommon;

namespace DBInterface.RTDBInterface
{
    using Golden.DotNetSDK.Common;
    using Golden.DotNetSDK.Impl;
    using Golden.DotNetSDK.Inter;
    using Golden.DotNetSDK.Model;
    using Golden.DotNetSDK.Model.Base;
    using Golden.DotNetSDK.Model.Data;
    using Golden.DotNetSDK.Model.Historian;
    using Golden.DotNetSDK.Util;
    /// <summary>
    /// GoldenHelper
    /// golden实时数据库的连接方法。
    /// 
    /// 版本：1.0
    /// 
    /// 特别注意
    ///     1、关于异常的处理。Helper层不设异常处理机制，不使用用try..catch语句。在Helper层出现的异常，统一由调用RTDbHelper的DAO层进行处理。
    ///     ——这么处理的原因是，不同的DAO对异常的处理要求不同。有效率要求的DAO可能出现错误立刻跳过，不去反复尝试。有可用性要求的DAO可能要求要尝试N次连接。
    /// 修改纪录
    ///
    ///     2017.01.12 版本：1.1 gaofeng 修改。
    ///		2016.12.18 版本：1.0 gaofeng 创建。   
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2016.12.18</date>
    /// </author> 
    /// </summary>
    public class GoldenHelper : BaseRTDbHelper, IRTDbHelper, IRTDbHelperExpand
    {
        public string _server = "192.168.1.55";
        public string _port = "6327";
        public string _userId = "sa";
        public string _password = "golden";       

        private RTDBConnection _rtdbconnection;     //golden数据库连接对象
        private IServer _iServer;                   //数据库连接信息服务接口
        private IBase _iBase;                       //数据库、数据表信息服务接口
        private ISnapshot _iSnapshot;               //快照数据服务接口
        private IHistorian _iHistorian;             //历史数据服务接口
        private string _exception;                  //内部异常
        //private bool _isLogon;                    //不在使用内部_isLogon，因为不能及时反映连接的的状态

        /// <summary>
        /// 当前实时库数据库类型。该参数必须在具体的实时数据库连接方法中重定义。
        /// </summary>
        public override CurrentRTDbType CurrentRTDbType
        {
            get
            {
                return CurrentRTDbType.Golden;
            }
        }

        /// <summary>
        /// 异常信息
        /// </summary>
        public override string Exception
        {
            get
            {
                return this._exception;
            }

        }

        /// <summary>
        /// 是否登录
        /// </summary>
        public bool isLogOn
        {
            //这里要特别注意，isLogon一定是只读，一定是直接返回Server的Connected属性。
            //——因为在建立连接后，如果Server对象的状态发生变化，会直接体现到Connected上。
            //——如果是在helper中用内部变量_isLogOn=_piServer.Connected去传递。那么在连接状态发生变化后，_piServer.Connected的变化并不能及时的反应到_isLogOn上。
            //——因此外部变量isLogOn必须在需要的时候，直接通过实时数据的服务对象获取状态，进行判断。才能准确代表当时的状态。
            get
            {
                if (this._rtdbconnection != null && this._rtdbconnection.State.ToString() != "Open")
                    return true;
                else
                    return false;
            }
        }

        #region public GoldenHelper() 构造函数：不带连接字符串的构造函数。工厂的反射调用该方法。
        /// <summary>
        /// 构造函数
        /// </summary>
        public GoldenHelper()
        {
            //不带连接字符串参数的构造函数，用实时数据库连接方法的内部属性值构建ConnectionString属性
            this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
            FileName = "GoldenHelper.txt";   // sql查询句日志文件名称
        }
        #endregion

        #region public GoldenHelper(string connectionString) 构造函数：带连接字符串的构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public GoldenHelper(string connectionString)
            : this()
        {
            //带连接字符串参数的构造方法，用传入的参数初始化接字符串属性ConnectionString；当参数为空时，仍用内部默认值
            if (connectionString != null)
            {
                this.ConnectionString = connectionString;
            }
            else
            {
                this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
            }

        }
        #endregion

        //数据库连接与断开        
        #region public String Logon() 获取数据库连接：不带连接字符串的连接函数
        public String Logon()
        {
            //golden数据库连接说明
            //连接字符串中如果配置连接池参数为true，则建立数据库连接时，自动使用连接池。
            //在连接字符串不设定的情况下，2.1版本golden默认不适用连接池，3.0默认使用连接池。
            //如果启动连接池，则应用程序每次向数据库发送链接请求，如果连接字符串不变，则数据库会用连接池中已经有的连接，而不会建立新连接。
            //如果不启动连接池，则应用程序每次向数据库发送链接请求，数据库都会建立新的连接。（这样，数据库连接数量会快速增加，可通过golden管理界面的“连接管理”选项来查看。）
            //不带连接字符串的连接函数，采用实例化时对象的属性this.ConnectionString被初始化的值进行连接
            return this.Logon(ConnectionString);
        }
        #endregion

        #region  public String Logon(string connectionString) 获取数据库连接：带连接字符串的连接函数
        public String Logon(string connectionString)
        {
            //带连接字符串的连接函数，采用传入参数connectionString进行连接。            
            if (this._rtdbconnection == null || this._rtdbconnection.State.ToString() != "Open")
            {
                this.ConnectionString = connectionString;
                this._rtdbconnection = new RTDBConnection(connectionString);
                this._rtdbconnection.Open();
                this._iBase = new BaseImpl(this._rtdbconnection);
                this._iSnapshot = new SnapshotImpl(this._rtdbconnection);
                this._iHistorian = new HistorianImpl(this._rtdbconnection);
                this._iServer = new ServerImpl(this._rtdbconnection);
                
            }

            if (this._rtdbconnection.State.ToString() == "Open")
            {                
                return "logon";
            }
            else
            {                
                return "logoff";
            }

        }
        #endregion

        #region public void Logoff() 关闭数据库连接
        public void Logoff()
        {
            //在golden数据库有连接池的情况下，
            //Close方法，即不销毁对象，也不注销内存，仅仅是把_rtdbconnection对象丢会连接池中
            //_rtdbconnection对象销毁，只有在连接池到达等待时间waitMaxTime时，才会发生，这也是连接池存在的意义。
            //close和dipose的区别，在于close的connection，连接字符串还在，还可以重连，而dispose的conncection被null，不能在进行重连。
            if (this._rtdbconnection != null)
            {
                this._rtdbconnection.Close();                
            }
        }
        #endregion

        //数据库服务
        #region 获取数据库服务器当前时间
        public DateTime GetHostDateTime()
        {
            DateTime serverdate = this._iServer.GetHostTime();
            return serverdate;
        }
        #endregion

        #region 数据表操作-根据标签名批量获取标签id：private int[] GetPointIDsbyName(string[] names) Golden数据库通过标签id查询
        /// <summary>
        /// 根据 "表名.标签点名" 格式批量获取标签点标识
        /// 对于找不到的标签点，在返回的标签点标识中，提供id=0的占位
        /// </summary>
        private int[] GetPointIDsbyName(string[] names)
        {
            //在每次只用_iBase前，读_conn状态，用了确定当前数据库连接是否正常
            //Log.Debug(_conn.State.ToString());

            //_iBase.FindPoints，仅返回在数据库中可以找到的标签点的id。找不到的标签没有占位
            //特别注意此处要保证本地项目中的SDK（三个dll）版本和要访问的数据库（本地或远程）版本要统一。
            List<MinPoint> result = this._iBase.FindPoints(names);

            List<int> ids = new List<int>();
            if (names.Length == result.Count)
            {   //如果所有点都找到，则快速生成ids
                foreach (MinPoint mp in result)
                {
                    ids.Add(mp.PointId);
                }
            }
            else
            {   //如果有些点找不到，则插ids
                int j = 0;
                for (int i = 0; i < result.Count; i++)
                {   //对result进行遍历
                    do
                    {
                        if (result[i].TableName + "." + result[i].Tag == names[j])
                        {   //如果result当前的TableName+Tag名称在names中能找到
                            ids.Add(result[i].PointId);     //则将当前id插入到结果ids中
                            j++;                            //跳到下一个name
                            break;                          //跳到下一个result
                        }
                        else
                        {   //如果result当前的TableName+Tag名称不能等于当前name
                            ids.Add(0);                     //则当前names在result中找不到，插入id=0
                            j++;                            //调到下一个name     
                        }
                    } while (j < names.Length);
                }
            }
            return ids.ToArray();
        }
        #endregion

        //检查标签是否存在
        #region 检查标签是否存在    public List<string> CheckTags(string[] tagnames)
        public List<string> CheckTags(string[] tagnames)
        {
            List<string> results = new List<string>();
            int[] tagids = GetPointIDsbyName(tagnames);     //获取标签名称对应的id号
            for (int i = 0; i < tagids.Length; i++)
            {
                if (tagids[i] == 0) results.Add(tagnames[i]);   //返回没有找到的标签
            }
            return results;                                     //返回没有找到的标签
        }
        #endregion

        //获取标签列表
        #region 获取标签列表 public  List<String> GetTagList()
        public List<SignalComm> GetTagList(string tagname, string tablename = "")
        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <returns>在数据库所有符合tagname搜索条件的标签列表（在所有表中查询）</returns>
        {
            List<SignalComm> results = new List<SignalComm>();      //存放标签名
            int[] ids;
            string[] tagnames;
            string[] descs;
            SearchCondition searchcondition = new SearchCondition();  //建立搜索条件
            searchcondition.Tag = tagname;                            //搜索的标签名称
            searchcondition.Table = tablename;                      //搜索的表名称

            try
            {
                ids = this._iBase.Search(searchcondition, 10000, 0);           //按条件搜索，得到id数组
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }

            tagnames = this._iBase.GetTagNames(ids);                       //按id数组，得到点表
            descs = this._iBase.GetDescs(ids);                             //按id数组，得到点表描述
            for (int i = 0; i < ids.Length; i++)
            {
                results.Add(new SignalComm(tagnames[i], descs[i]));
            }
            return results;         //返回那些数据库中没有找到的标签

        }
        #endregion

        //当前值（即时值、快照值）的读写
        #region 批量读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValues(string[] tagnames)
        public List<PValue> GetActValues(string[] tagnames)
        {
            List<PValue> pvalues = new List<PValue>();
            //golden数据库读取标签点当前值（即时值、快照值）是根据标签点id号
            //需要首先根据标签点名获得对应id号            
            int[] ids = GetPointIDsbyName(tagnames); //批量获取标签名称对应的id号，找不到的点id=0            
            pvalues = GetActValues(ids);
            return pvalues;
        }
        public List<PValue> GetActValues(int[] tagids)
        {
            List<PValue> pvalues = new List<PValue>();
            //tagids=new int[7] {1,20000,4,5,6,7,8};
            Entity<FloatData> list = new Entity<FloatData>();
            try
            {
                list = this._iSnapshot.GetFloatSnapshots(tagids);
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            //如果要查询的id号在数据库中不存在，id为0，则对应list对应的位置有FloatData类型占位，Error值不为0。这个要考虑在FloatDatatoPValue中统一处理
            pvalues = FloatDatatoPValue(list);

            return pvalues;
        }
        #endregion

        #region 单个读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValue(string[] tagnames)
        public PValue GetActValue(string tagname)
        {
            List<PValue> pvalues = new List<PValue>();
            //golden数据库读取标签点当前值（即时值、快照值）是根据标签点id号
            //需要首先根据标签点名获得对应id号            
            string[] tagnames = { tagname };
            int[] ids = GetPointIDsbyName(tagnames); //批量获取标签名称对应的id号，找不到的点id=0
            pvalues = GetActValues(ids);
            return pvalues[0];
        }
        public PValue GetActValue(int tagid)
        {
            List<PValue> pvalues = new List<PValue>();      //结果存储List
            int[] tagids = { tagid };                       //构成id数组
            pvalues = GetActValues(tagids);                 //用批量读取
            return pvalues[0];
        }
        #endregion

        #region 批量写入数据库标签点的当前值（即时值、快照值）：public int SetActValues(string[] tagnames,List<PValue> pvalues)
        public int SetActValues(string[] tagnames, List<PValue> pvalues)
        {
            //golden数据库读取标签点当前值（即时值、快照值）是根据标签点id号
            //需要首先根据标签点名获得对应id号            
            int[] ids = GetPointIDsbyName(tagnames); //批量获取标签名称对应的id号，找不到的点id=0
            //将pvalues转换为floatdata类型
            List<FloatData> floatdata = PValues2FloatData(ids, pvalues);
            //写入floatdata类型的当前值
            EntityResult<FloatData> result = new EntityResult<FloatData>();
            try
            {
                result = this._iSnapshot.PutFloatSnapshotsCollection(floatdata);
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            if (result.ErrorList.Count != 0)
            {

                string errstr = RTDBError.GetError(result.ErrorList[0].Error);

            }
            return 0;
        }
        public int SetActValues(int[] tagids, List<PValue> pvalues)
        {
            //将pvalues转换为floatdata类型
            List<FloatData> floatdata = PValues2FloatData(tagids, pvalues);
            //写入floatdata类型的当前值
            EntityResult<FloatData> result = new EntityResult<FloatData>();
            try
            {
                result = this._iSnapshot.PutFloatSnapshotsCollection(floatdata);
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            if (result.ErrorList.Count != 0)
            {
                string errstr = RTDBError.GetError(result.ErrorList[0].Error);
            }
            return 0;
        }
        #endregion

        #region 单个写入数据库标签点的当前值（即时值、快照值）：public int SetActValue(string tagname,PValue pvalues)
        public int SetActValue(string tagname, PValue pvalue)
        {
            string[] tagnames = { tagname };
            List<PValue> pvalues = new List<PValue>();
            pvalues.Add(pvalue);
            int result = SetActValues(tagnames, pvalues);
            return result;
        }
        public int SetActValue(int tagid, PValue pvalue)
        {
            int[] tagids = { tagid };
            List<PValue> pvalues = new List<PValue>();
            pvalues.Add(pvalue);
            int result = SetActValues(tagids, pvalues);
            return result;
        }
        #endregion


        //历史值的读写
        #region 读取历史数据原始值：public List<PValue> GetRawValues(string tagname, DateTime startdate, DateTime enddate)。重点函数，概化计算引擎用。
        /// <summary>
        /// GetRawValues是实时库读取历史数据接口的对外封装。无论何种实时数据库，读取历史数据功能都是指，读取某一个指定标签（tagname）在一段时间内（startdate、enddate）的历史值（浮点型）
        /// 而不同的实时数据库，真正的读取方法和返回值的数据类型略有区别
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns>历史数据，PValue集合</returns>
        public List<PValue> GetRawValues(string tagname, DateTime startdate, DateTime enddate)
        {
            //对外映射的GetRawValues，对内是获得浮点数据类型的原始值。实际上也有其他数据类型的原始值。
            //未来如果需要获得不同数据类型的历史值，可以在GetRawValues接口上添加dbfieldtype字段来指定数据类型，然后在该程序内部，根据数据类型做分支，去调用不同数据类型的接口。
            //目前默认仅实现的浮点型的历史数据的读取
            return GetFloatArchivedValues(tagname, startdate, enddate);
        }
        public List<PValue> GetRawValues(int tagid, DateTime startdate, DateTime enddate)
        {
            //对外映射的GetRawValues，对内是获得浮点数据类型的原始值。实际上也有其他数据类型的原始值。
            //未来如果需要获得不同数据类型的历史值，可以在GetRawValues接口上添加dbfieldtype字段来指定数据类型，然后在该程序内部，根据数据类型做分支，去调用不同数据类型的接口。
            //目前默认仅实现的浮点型的历史数据的读取
            return GetFloatArchivedValues(tagid, startdate, enddate);
        }
        ///golden数据库读取历史数据原始值函数： private List<PValue> GetFloatArchivedValues(string name, DateTime begin, DateTime end)
        /// <summary>
        /// 读取 单个 浮点型 标签点 一段时间内的历史数据（原始数据，非插值，浮点型）
        /// 该接口会自动对起始时间和截止时间进行截取补齐（golden数据库的GetFloatArchivedValues返回的是起始时间后最近一个点，结束时间前最后一个点）        
        /// example：
        /// 获取历史数据的起始时间begin为8:00:00，会自动找到该时间前的最近一个数据，比如7:59:50,将该数据做为返回数据集的第一个数据，并将时间修正为8:00:00
        /// 获取历史数据的截止时间end为9:00:00,会根据返回数据的最后一个时间，比如8:59:30，在其后补充一个数据，没有值，仅有结束时间9:00:00，该值用于计算8:59:30这个值得Timespan
        /// </summary>
        private List<PValue> GetFloatArchivedValues(string name, DateTime begin, DateTime end)
        {
            List<PValue> pvalues = new List<PValue>();
            //golden数据库读取单个标签点历史数据是根据标签点id号
            //需要首先根据标签点名获得对应id号
            string[] tags = { name };
            int[] tagids = GetPointIDsbyName(tags); //获取标签名称对应的id号

            if (tagids.Length == 0 || tagids[0] <= 0) return pvalues;        //如果tagids.Length==0(新版本)或者id=0（旧版本）则找不到该点，返回空pvalues

            int tagid = tagids[0];

            pvalues = GetFloatArchivedValues(tagid, begin, end);

            return pvalues;
        }
        private List<PValue> GetFloatArchivedValues(int tagid, DateTime begin, DateTime end)
        {
            List<PValue> pvalues = new List<PValue>();
            //判断tagid范围是否正常
            if (tagid <= 0) return pvalues;        //tagid必须大于0，否则返回空pvalues
            //获取历史数据原始值
            //golden数据库获取浮点型历史原始数据的接口是GetFloatArchivedValues(id, count, begin, end)。（读插值是GetFloatInterpoValues()）
            //count是一个输入/输出参数。该接口根据输入的count申请相应大小的内存，输出的count为实际查询到的历史数据数量。
            //count大小没有限制，但是单个程序可以寻址的内存大小有限制.
            //——如果1秒一个实时数据，一天实时数据为86400条，读出的List<PValue>占用内存15M左右。
            //一般设置count大小为1000000，即一百万条记录，如果按照1秒一个实时数据，大概存放10天左右的数据，占用内存为150M数量级
            //在概化计算中，如果直接取用原始实时数据，计算周期不应超过1天。
            const int maxCount = 1000000;
            int count = this._iHistorian.GetNumberArchivedValuesCount(tagid, begin, end);        //实际情况，应该根据返回的数量做出不同的处理
            //
            Entity<FloatData> list = new Entity<FloatData>();
            if (count == 0) return pvalues;        //如果count为0，直接返回空pvalues。外部程序可以根据pvalues.count是否为0来处理
            if (count <= maxCount && count > 0)    //如果记录数量小于100000,且不为0，则直接读取         
            {

                try
                {
                    list = this._iHistorian.GetFloatArchivedValues(tagid, count, begin, end); //读取历史数据 
                }
                catch (RTDBException ex)
                {
                    //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                    this._exception = ex.ToString();
                    throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
                }
                //golden数据库读取历史数据，返回数据集的是从起始时间后最近一个数据开始
                //实际需要的是从起始时间前最近一个数据开始，只要返回的第一个数据时间戳不等于begin，就应该向前补充一个数据
                //获取历史数据的起始时间begin为8:00:00，会自动找到该时间前的最近一个数据，比如7:59:50,将该数据做为返回数据集的第一个数据，并将时间修正为8:00:00
                if (list.Data[0].Time > begin)
                {
                    FloatData firstdata = new FloatData();
                    try
                    {
                        firstdata = this._iHistorian.GetFloatSingleValue(tagid, begin, HistorianMode.PREVIOUS);//获取起始时刻前最近的一个数据
                    }
                    catch (RTDBException ex)
                    {
                        //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                        this._exception = ex.ToString();
                        throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
                    }
                    //这里也存在两种可能，一种是begin时间点之前确实再没有其他数据，该时间点后的数据已经是数据库存储的第一个数据；另一种是可以找到begin时间点前的数据
                    if (firstdata != null)
                    {   //一般情况下，都能在begin时间点前，找到一个数据点，用该数据点的值替代begin时刻的值
                        firstdata.Time = begin;
                        list.Data.Insert(0, firstdata);
                        list.Count += 1;
                    }
                    else
                    {   //特殊情况下，仅当当前时刻已经位于数据库存储数据的最前端，begin时刻点前再无其他时刻的值。
                        //该情况下，用已经取得的第一个点的值，作为begin时刻的值
                        firstdata = new FloatData();
                        firstdata.Id = tagid;
                        firstdata.Value = list.Data[0].Value;
                        firstdata.Time = begin;
                        firstdata.Error = 0;
                        firstdata.Qualitie = Quality.GOOD;
                        firstdata.Ms = 0;

                        list.Data.Insert(0, firstdata);
                        list.Count += 1;
                    }
                }
                //golden数据库读取历史数据，返回数据集的是到截止时间前最后一个数据为止。
                //根据需要，在后面补充一个空数据，仅记录截止时间end，用于计算截止时间前最后一个数据的timespan
                if (list.Data[list.Count - 1].Time < end)
                {
                    FloatData lastdata = new FloatData();
                    lastdata.Time = end;
                    list.Data.Add(lastdata);
                    list.Count += 1;
                }
                //list是一个起始时间从begin开始，截止时间为end的点集合.
                //其头部begin时间点，有可能恰好对应一个原始值，有可能用前一时刻的值补足的值。
                //其尾部end时间点，有可能恰好对应一个原始值，有可能是用end时刻补足的一个值。
                //GetFloatSingleValue如过返回0个值，则直接退出
                //GetFloatSingleValue如果仅返回一个值，则要么会在begin补值，要么会在end补值。要么会在两边都补。
                //GetFloatSingleValue如果仅返回两个值，有可能恰好在两边。这样两边都不需要补值
                //因此list一定是一个至少包含两个元素的列表集合。（GetFloatSingleValue返回0个值的情况，已经直接处理了）
                //将结果转换为pvalue
                pvalues = FloatDatatoPValueHist(list);
            }
            else if (count > maxCount)
            //如果记录数量大于100000，怎么处理
            {
                //按晋晖说的应该分批读取，但实际上，对于概化计算，还要送到计算引擎，没有办法分批，因此必须记录log，并略过
                //另外的处理方法就是做好规划控制每次读取的时间间隔
                //在概化计算引擎中，采用第二种方式来控制最大数量


            }
            return pvalues;
        }
        #endregion

        #region 读取历史数据插值，固定数量：public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int interval)
        /// <summary>
        /// GetIntervalValuesFixCount是实时库读取历史数据接口的对外封装。无论何种实时数据库，读取历史数据功能都是值，读取某一个指定标签（tagname）在一段时间内（startdate、enddate）的历史值（浮点型）
        /// 而不同的实时数据库，真正的读取方法和返回值得内容略有区别
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="count">获取插值记录的数量</param>
        /// <returns>历史数据，PValue集合</returns>
        public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int interval)
        {
            //对外映射的GetIntervalValuesFixCount，对内是获得浮点数据类型的原始值。实际上也有其他数据类型的原始值。
            //未来如果需要获得不同数据类型的历史值，可以在GetIntervalValuesFixCount接口上添加dbfieldtype字段来指定数据类型，然后在该程序内部，根据数据类型做分支，去调用不同数据类型的接口。
            //目前默认仅实现的浮点型的历史数据的读取
            return GetFloatIntervalValues(tagname, startdate, enddate, interval);
        }
        public List<PValue> GetIntervalValuesFixInterval(int tagid, DateTime startdate, DateTime enddate, int interval)
        {
            return GetFloatIntervalValues(tagid, startdate, enddate, interval);
        }
        ///golden数据库读取历史数据插值函数： private List<PValue> GetFloatInterpoValues(string name, DateTime begin, DateTime end,int count)
        /// <summary>
        /// 读取 单个 浮点型 标签点 一段时间内的历史数据（插值，浮点型）
        /// 该接口将begin到end时间区间分成count段，返回每一段起点的值和时间。第count段的结束时刻点不返回。
        /// 该接口会自动根据起始时间和截止时间秒数和分段数，计算每一个分段的秒数，秒数不为整数时取整。        
        /// example：
        /// 获取历史数据的起始时间begin为8:00:00，截止时间为8:30:00。若需要每个整秒返回一个值，count设为1800。若需要每5秒返回一个值，count设为360        
        /// </summary>
        private List<PValue> GetFloatIntervalValues(string name, DateTime begin, DateTime end, int interval)
        {
            List<PValue> pvalues = new List<PValue>();
            //golden数据库读取单个标签点历史数据是根据标签点id号
            //需要首先根据标签点名获得对应id号
            string[] tags = { name };
            int[] ids = GetPointIDsbyName(tags); //获取标签名称对应的id号
            int id = ids[0];
            if (id == 0) return pvalues;        //如果id=0则找不到该点，返回空pvalues
            /*20181011 将入口参数由插值的分段数量count改为插值间隔秒数interval，以下程序均需要重新修改为适应interval的形式。
             *改为interval的形式，是因为：1、在pgim、pi、西安热工院数据库下，均已interval为参数。2、interval只能用整数。如果入参是count存在转换为的秒数不为整数的情况。比较麻烦
            //获取历史数据插值
            //golden数据库获取浮点型历史原始数据的接口是GetFloatInterpoValues(id, count, begin, end)。
            //实际的取法是，将begin到end时间区间分成count段，返回每一段起点的值和时间。
            //比如从10:00到10:30取100个数据。一共1800秒，每一段18秒。返回的最后一个数据其对应的时间是10:29:48。
            //由于最小间隔只能是秒的整数，不是整秒的取整。因此返回的最后一个数据的时间可能与end不一样相同，有可能早于end。
            //比如从10:00到10:30取130个数据。一共1800秒，每一段13.8秒。那么实际上会按照13秒进行插值。那么就是从10:00开始没13秒取一个值（包含10:00），最后一个值对应的时间为10:27:57。

            Entity<FloatData> list = new Entity<FloatData>();
            try
            {
                list = this._iHistorian.GetFloatInterpoValues(id, count, begin, end);
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            //是否考虑读不到插值的情况
            if (count != 0)
            {
                pvalues = FloatDatatoPValue(list);
            }
            */
            return pvalues;
        }
        private List<PValue> GetFloatIntervalValues(int tagid, DateTime begin, DateTime end, int count)
        {
            List<PValue> pvalues = new List<PValue>();
            //golden数据库读取单个标签点历史数据是根据标签点id号           
            if (tagid <= 0) return pvalues;        //如果id=0则找不到该点，返回空pvalues
            //获取历史数据插值
            //golden数据库获取浮点型历史原始数据的接口是GetFloatInterpoValues(id, count, begin, end)。
            //实际的取法是，将begin到end时间区间分成count段，返回每一段起点的值和时间。
            //比如从10:00到10:30取100个数据。一共1800秒，每一段18秒。返回的最后一个数据其对应的时间是10:29:48。
            //由于最小间隔只能是秒的整数，不是整秒的取整。因此返回的最后一个数据的时间可能与end不一样相同，有可能早于end。
            //比如从10:00到10:30取130个数据。一共1800秒，每一段13.8秒。那么实际上会按照13秒进行插值。那么就是从10:00开始没13秒取一个值（包含10:00），最后一个值对应的时间为10:27:57。

            Entity<FloatData> list = new Entity<FloatData>();
            try
            {
                list = this._iHistorian.GetFloatInterpoValues(tagid, count, begin, end);
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            //是否考虑读不到插值的情况
            if (count != 0)
            {
                pvalues = FloatDatatoPValue(list);
            }

            return pvalues;
        }
        #endregion

        #region 读取历史数据统计值：public List<PValue> GetSummaryValues(string tagname,DateTime startdate,DateTime enddate,string type)
        public PValue GetSummaryValues(string tagname, DateTime startdate, DateTime enddate, string type)
        {
            PValue pvalue = new PValue();
            //golden数据库读取单个标签点历史数据是根据标签点id号
            //需要首先根据标签点名获得对应id号
            string[] tagnames = { tagname };
            int[] tagids = GetPointIDsbyName(tagnames); //获取标签名称对应的id号
            int tagid = tagids[0];
            if (tagid == 0) return pvalue;        //如果id=0则找不到该点，返回空pvalues

            pvalue = GetSummaryValues(tagid, startdate, enddate, type);
            return pvalue;
        }
        public PValue GetSummaryValues(int tagid, DateTime startdate, DateTime enddate, string type)
        {
            PValue pvalue = new PValue();
            //golden数据库读取单个标签点历史数据是根据标签点id号            
            if (tagid <= 0) return pvalue;        //如果id=0则找不到该点，返回空pvalues

            SummaryEntity result = new SummaryEntity();
            try
            {
                result = this._iHistorian.GetNumberSummary(tagid, startdate, enddate);
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }

            switch (type.ToLower())
            {
                case "calcavg":
                    pvalue = new PValue(result.CalcAvg, startdate, enddate, 0);
                    break;
                case "total":
                    pvalue = new PValue(result.Total, startdate, enddate, 0);
                    break;
                case "max":
                    pvalue = new PValue(result.Max, startdate, enddate, 0);
                    break;
                case "min":
                    pvalue = new PValue(result.Min, startdate, enddate, 0);
                    break;
                default:
                    break;
            }
            return pvalue;
        }
        #endregion

        #region 写入历史数据,单个标签点：public int PutArchivedValues(string tagname, string[][] data)
        /// <summary>
        /// 写入单个标签点的历史数据
        /// 这个方法主要用于从csv文件导入历史数据到golden数据库中。从csv读取的数据是字符串型二维数组，因此这里的历史数据入参是字符串型二维数组，而不是List<PValue>
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int PutArchivedValues(string tagname, string[][] data)
        {
            string[] tagnames = { tagname };
            //golden数据库读取标签点当前值（即时值、快照值）是根据标签点id号
            //需要首先根据标签点名获得对应id号            
            int[] ids = GetPointIDsbyName(tagnames); //批量获取标签名称对应的id号，找不到的点id=0

            //将string[][]格式转换为 List<FloatData>
            List<FloatData> floatdatas = StringDatatoFloatData(ids[0], data);                       //将string数组转换为FloatData数组，数据对应的标签用id指明
            EntityResult<FloatData> result = new EntityResult<FloatData>();
            try
            {
                result = this._iHistorian.PutFloatArchivedCollection(floatdatas);    //将floatdata写入数据库
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            return result.SuccessCount;
        }
        public int PutArchivedValues(int tagid, string[][] data)
        {
            //将string[][]格式转换为 List<FloatData>
            List<FloatData> floatdatas = StringDatatoFloatData(tagid, data);                       //将string数组转换为FloatData数组，数据对应的标签用id指明
            EntityResult<FloatData> result = new EntityResult<FloatData>();
            try
            {
                result = this._iHistorian.PutFloatArchivedCollection(floatdatas);    //将floatdata写入数据库
            }
            catch (RTDBException ex)
            {
                //将golden的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pgim的异常继续向外传递。                
            }
            return result.SuccessCount;
        }
        #endregion

        //断面数据的读写
        #region 读取断面数据：public List<PValue> GetCrossSectionValue(string[] tagnames, DateTime startdate)。
        /// <summary>
        /// GetRawValues是实时库读取历史数据接口的对外封装。无论何种实时数据库，读取历史数据功能都是指，读取某一个指定标签（tagname）在一段时间内（startdate、enddate）的历史值（浮点型）
        /// 而不同的实时数据库，真正的读取方法和返回值的数据类型略有区别
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns>历史数据，PValue集合</returns>
        public List<PValue> GetCrossSectionValue(string[] tagnames, DateTime startdate)
        {
            List<PValue> pvalues = new List<PValue>();
            return pvalues;
        }
        public List<PValue> GetCrossSectionValue(int[] tagids, DateTime startdate)
        {
            List<PValue> pvalues = new List<PValue>();
            return pvalues;
        }
        #endregion


        //辅助函数：数据格式转换
        //写入当前值时，数据来源往往是PValue形式，而写入历史值是，数据来源往往是csv文件的字符串数组形式，无论什么形式都要转换为float形式才能写入
        #region 写入当前值用：将PValues型数据转换成FloatData型数据 private List<FloatData> PValues2FloatData(int[] ids, List<PValue> pvalues)
        private List<FloatData> PValues2FloatData(int[] ids, List<PValue> pvalues)
        {
            List<FloatData> floatdata = new List<FloatData>();
            for (int i = 0; i < pvalues.Count; i++)
            {   //FloatData数据类型参考golden C#开发手册
                FloatData item = new FloatData();
                item.Id = ids[i];                       //标签点id号，唯一
                item.Time = pvalues[i].Timestamp;       //时间戳
                item.Value = pvalues[i].Value;          //值
                item.Error = 0;                         //错误码
                item.Qualitie = 0;                      //质量码，0代表GOOD
                item.Ms = 0;                            //实时数值时间
                floatdata.Add(item);
            }
            return floatdata;
        }
        #endregion

        #region 写入历史值用：将String数组类型数据转换成浮点型FloatData数据类型 List<FloatData> StringDatatoFloatData(int id, string[][] stringdata)
        //写入历史值时，一般是从csv读取数据。csv读取的数据是字符串型二维数组，因此这里的历史数据入参是字符串型二维数组，而不是List<PValue>
        //对stringdata的要求，第二列必须是值，第三列必须是时间戳，第四列必须是质量位，0表示Good。第一列是记录id号，但是实际写入时不用第一列
        List<FloatData> StringDatatoFloatData(int id, string[][] stringdata)
        {
            List<FloatData> floatdata = new List<FloatData>();

            for (int i = 0; i < stringdata.Length; i++)
            {
                FloatData item = new FloatData();
                //FloatData数据类型，一共6个属性，参看Golden二次开发手册，数据实体模型
                item.Id = id;                                           //编号
                item.Time = DateTime.Parse(stringdata[i][2]);           //时间戳
                item.Value = Convert.ToDouble(stringdata[i][1]);        //值
                item.Error = 0;                                         //错误码
                item.Qualitie = (Quality)int.Parse(stringdata[i][3]);   //质量码，0代表GOOD。先将字符转为整型，再将整型转为枚举。
                item.Ms = 0;                                            //实时数值时间

                floatdata.Add(item);
            }
            return floatdata;
        }
        #endregion

        //读取数据时，无论是读历史值还是当前值，数据来源都是floatdata形式，都要转换为pvalue形式来使用
        #region 读取用：将浮点数类型数据转换成PValue型数据 private List<PValue> FloatDatatoPValue(Entity<FloatData> floatdata)
        private List<PValue> FloatDatatoPValue(Entity<FloatData> floatdata)
        {
            int status = 0;
            List<PValue> pvalues = new List<PValue>();
            //实时数据处理版本：仅按实际的数据内容转换为PValue，PValue只准备时间、值、状态。             
            foreach (FloatData d in floatdata.Data)
            {
                if (d.Error == 0)
                {
                    //FloatData质量码d.Qualitie定义，见golden手册3.4.3
                    //质量码512后为自定义

                    //FloatData错误码d.Error,如何处理。
                    //在id=0的情况下，对应位置返回的数据，Qualitie为good，但是Error不为0

                    status = QualitieToInt(d.Qualitie);
                    pvalues.Add(new PValue(d.Value, d.Time, status));
                }
                else
                {
                    //如果error不为0，说明golden读取数据有误
                    //给pvalue赋一个初值，即timespan为1970-1-1 00:00:00
                    pvalues.Add(new PValue(0, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    //获得error描述，写入log
                    string errorStr = RTDBError.GetError(d.Error);
                    //Log.Error(errorStr);
                }
            }
            return pvalues;
        }
        #endregion

        #region 读取用：将浮点数类型数据转换成PValue型数据:历史数据处理版本：PValue除了准备时间、值、状态，还填写endtime和timespan private List<PValue> FloatDatatoPValueHist(Entity<FloatData> floatdata)
        private List<PValue> FloatDatatoPValueHist(Entity<FloatData> floatdata)
        {
            //list是一个起始时间从begin开始，截止时间为end的点集合.
            //其头部begin时间点，有可能恰好对应一个原始值，有可能用前一时刻的值补足的值。
            //其尾部end时间点，有可能恰好对应一个原始值，有可能是用end时刻补足的一个值。
            //GetFloatSingleValue如过返回0个值，则直接退出
            //GetFloatSingleValue如果仅返回一个值，则要么会在begin补值，要么会在end补值。要么会在两边都补。
            //GetFloatSingleValue如果仅返回两个值，有可能恰好在两边。这样两边都不需要补值
            //因此floatdata一定是一个至少包含两个元素的列表集合。（GetFloatSingleValue返回0个值的情况，已经直接处理了）
            //将结果转换为pvalue
            int status = 0;
            List<PValue> pvalues = new List<PValue>();
            for (int i = 0; i < floatdata.Count - 1; i++)       //floatdata一定是一个至少包含两个元素的列表集合。
            {
                if (floatdata.Data[i].Error == 0)
                {
                    //FloatData质量码定义，见golden手册3.4.3
                    //质量码512后为自定义
                    status = QualitieToInt(floatdata.Data[i].Qualitie);
                    pvalues.Add(new PValue(floatdata.Data[i].Value, floatdata.Data[i].Time, floatdata.Data[i + 1].Time, status));
                }
                else
                {
                    //如果error不为0，说明golden读取数据有误
                    //给pvalue赋一个初值，即timespan为1970-1-1 00:00:00
                    pvalues.Add(new PValue(0, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    //获得error描述，写入log
                    string errorStr = RTDBError.GetError(floatdata.Data[i].Error);
                    //Log.Error(errorStr);
                }
            }

            return pvalues;
        }
        #endregion

        #region 转换golden的Quality到int类型status private int QualitieToInt(Quality q)
        // 转换golden的Quality到int类型status        
        private int QualitieToInt(Quality q)
        {
            //参考golden .net手册 3.4.3 Quality Enumeration定义 P557
            int status = 0;
            switch (q)
            {
                case Quality.GOOD:
                    status = 0;
                    break;
                case Quality.BAD:
                    status = 5;
                    break;
                case Quality.CALCOFF:
                    status = 5;
                    break;
                case Quality.CREATED:
                    status = 2;
                    break;
                case Quality.DIVBYZERO:
                    status = 6;
                    break;
                case Quality.NODATA:
                    status = 1;
                    break;
                case Quality.REMOVED:
                    status = 7;
                    break;
                case Quality.SHUTDOWN:
                    status = 3;
                    break;
                case Quality.OPC:
                    status = 256;
                    break;
                case Quality.USER:
                    status = 512;
                    break;
                default:
                    break;
            }
            return status;
        }
        #endregion

        #region 获取Int类型数据 private List<PValue> IntDatatoPValue(Entity<IntData> intdata)
        // 获取Int类型数据        
        private List<PValue> IntDatatoPValue(Entity<IntData> intdata)
        {
            List<PValue> pvalues = new List<PValue>();
            foreach (IntData d in intdata.Data)
            {
                int status = int.Parse(d.Qualitie.ToString());
                pvalues.Add(new PValue(d.Value, d.Time, status));
            }
            return pvalues;
        }
        #endregion

        #region public void Dispose() 内存回收
        /// <summary>
        /// 内存回收,参照BaseDbHelper
        /// </summary>
        public void Dispose()
        {
            // 关闭数据库连接
            if (this._rtdbconnection != null)
            {
                if (this._rtdbconnection.State != ConnectionState.Closed)
                {
                    this._rtdbconnection.Close();
                    this._rtdbconnection.Dispose();
                }
            }
            this._rtdbconnection = null;
        }
        #endregion
    }
}
