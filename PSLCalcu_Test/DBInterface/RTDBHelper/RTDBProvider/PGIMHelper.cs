using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using PCCommon;
using System.Diagnostics;               //使用计时器
using System.Text.RegularExpressions;   //使用正则表达式

namespace DBInterface.RTDBInterface
{
    using PgimNet;
    using PgimNet.PGIM;
    /// <summary>
    /// PGIMHelper
    /// PGIM实时数据库的连接方法。
    /// 
    /// 版本：1.0
    /// 
    /// 特别注意
    ///     1、关于异常的处理。Helper层不设异常处理机制，不使用用try..catch语句。在Helper层出现的异常，统一由调用RTDbHelper的DAO层进行处理。
    ///         ——这么处理的原因是，不同的DAO对异常的处理要求不同。有效率要求的DAO可能出现错误立刻跳过，不去反复尝试。有可用性要求的DAO可能要求要尝试N次连接。
    ///     2、PGIM数据库不提供查询服务器方法，因此GetHostDateTime()返回的是本机系统时间。
    /// 
    /// 修改纪录
    ///
    ///     2017.01.12 版本：1.1 gaofeng 修改。
    ///		2016.12.28 版本：1.0 gaofeng 创建。   
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2016.12.28</date>
    /// </author> 
    /// </summary>
    public class PGIMHelper : BaseRTDbHelper, IRTDbHelper
    {
        public string _server = "127.0.0.2";
        public string _port = "6327";
        public string _userId = "administrator";
        public string _password = "PlantConnect";
        public string _connectionString;
        private string _exception;
        //private bool _isLogon;

        //PGIM数据库连接对象
        //——特别注意，在这里PGIM的连接对象_pgimDb是一个静态对象。
        //——因此，在PGIMHelper外部，不管怎么new，实际上所有不同的PGIMHelper实例，都公用一个连接对象。
        //——1、这里首先保证了计算引擎连接PGIM的安全性。
        //      也就是说，从PGIMHelper底层，限定了只能对PGIM Server创建一个公用的连接对象.
        //      避免对PGIM Server创建多个连接，从而造成PGIM Server可用连接数超限。
        //——2、在上述方式下，在PGIMHelper内部，除了该位置，其余任何位置，不能把_pgimDb置为空。或者重新new PgimDatabase()。
        //      在一个使用该数据接口的应用中，无论什么地方使用实时接口，都使用顶部这个唯一类静态私有对象。
        //——3、PGIM是否支持多线程。如果_pgimDb不是静态类。则Pgim可能是能够支持多线程的，这点需要进行测试。
        //      但是，基于上面安全性的原因，（PGIM现场一般作为重要的服务器，避免因连接超限出问题），建议仅适用静态单线程连接的方式。
        PgimDatabase _pgimDb = new PgimDatabase();
        

        /// <summary>
        /// 当前实时库数据库类型。该参数必须在具体的实时数据库连接方法中重定义。
        /// </summary>
        public override CurrentRTDbType CurrentRTDbType
        {
            get
            {
                return CurrentRTDbType.PGIM;
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
            //这里要特别注意，_isLogon一定是只读，一定是直接返回Server的Connected属性。
            //——因为在建立连接后，如果Server对象的状态发生变化，会直接体现到Connected上。
            //——如果是在helper中用_isLogOn=_piServer.Connected。那么在连接状态发生变化后，_piServer.Connected并不能及时的反应到_isLogOn上。
            //——外部变量必须在需要的时候，才通过实时数据的服务对象获取状态，进行判断。才能准确代表当时的状态。
            get
            {
                if (this._pgimDb != null && this._pgimDb.IsLogon != null)
                    return this._pgimDb.IsLogon;
                else
                    return false;
            }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public override string ConnectionString
        {
            //在PGIM数据库中，连接数据库函数不直接使用连接字符串连接，因此在PGIM中需要对set重写。
            //当发生set时，对连接字符串进行解析,找出_server、_port、_userId、_password
            set
            {
                string[] _connArray = value.Split(';');
                this._server = _connArray[0].Substring(_connArray[0].IndexOf('=') + 1);
                this._port = _connArray[1].Substring(_connArray[1].IndexOf('=') + 1);
                this._userId = _connArray[2].Substring(_connArray[2].IndexOf('=') + 1);
                this._password = _connArray[3].Substring(_connArray[3].IndexOf('=') + 1);
                this._connectionString = value;         //这里主要是存储后，对外查询时用
            }
        }

        #region public PGIMHelper() 构造函数：不带连接字符串的构造函数。工厂的反射调用该方法。
        /// <summary>
        /// 构造方法
        /// </summary>
        public PGIMHelper()
        {
            //不带连接字符串参数的构造函数，用实时数据库连接方法的内部属性值构建ConnectionString属性
            this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
            FileName = "PGIMHelper.txt";   // sql查询句日志文件名称
            //_pgimDb = null;
        }
        #endregion

        #region public PGIMHelper(string connectionString) 构造函数：带连接字符串的构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public PGIMHelper(string connectionString)
            : this()
        {
            //带连接字符串参数的构造方法，用传入的参数初始化接字符串属性ConnectionString；当参数为空时，仍用内部默认值
            //在PGIM数据库中，连接数据库函数不直接使用连接字符串连接，因此在PGIM中需要对set重写。
            //当发生set时，对连接字符串进行解析,找出_server、_port、_userId、_password
            if (connectionString != null)
            {
                this.ConnectionString = connectionString;
                /* this.ConnectionString被设定值时，会自动执行set的内容
                string[] _connArray=connectionString.Split(';');
                this._server=_connArray[0].Substring(_connArray[0].IndexOf('=') + 1);
                this._port=_connArray[1].Substring(_connArray[1].IndexOf('=') + 1);
                this._userId=_connArray[2].Substring(_connArray[2].IndexOf('=') + 1);
                this._password = _connArray[3].Substring(_connArray[3].IndexOf('=') + 1);
                 */
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
            //不带连接字符串的连接函数，采用实例化时对象属性this.ConnectionString决定的_server、 _userId、 _password进行连接 
            //this._pgimDb = new PgimDatabase();            
            try
            {
                this._pgimDb.Logon(this._server, this._userId, this._password);
                if (this._pgimDb.IsLogon)
                {
                    return "logon";
                }
                else
                {
                    //this._pgimDb = null;
                    return "logoff";
                }
            }
            catch (Exception ex)
            {
                return "logoff";
            }

        }
        #endregion

        #region public String Logon(string connectionString) 获取数据库连接：带连接字符串的连接函数
        public String Logon(string connectionString)
        {
            //带连接字符串的连接函数，采用传入参数connectionString决定的_server、 _userId、 _password进行连接
            string[] _connArray = connectionString.Split(';');
            this._server = _connArray[0].Substring(_connArray[0].IndexOf('=') + 1);
            this._port = _connArray[1].Substring(_connArray[1].IndexOf('=') + 1);
            this._userId = _connArray[2].Substring(_connArray[2].IndexOf('=') + 1);
            this._password = _connArray[3].Substring(_connArray[3].IndexOf('=') + 1);
            this._pgimDb.Logon(this._server, this._userId, this._password);

            if (this._pgimDb.IsLogon)
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
            if (this._pgimDb != null)
                this._pgimDb.Logoff();
            //this._pgimDb = null;
        }
        #endregion

        //数据库服务,PGIM不提供相应接口，直接返回当前系统时间
        #region 获取数据库服务器当前时间
        public DateTime GetHostDateTime()
        {
            DateTime serverdate = DateTime.Now;
            return serverdate;
        }
        #endregion

        //检查标签点是否存在
        #region 检查标签点在实时数据库中是否存在 public  List<String> CheckTags(string[] tagnames)
        public List<string> CheckTags(string[] tagnames)
        /// <summary>
        /// 检查标签点在实时数据库中是否存在
        /// </summary>
        /// <returns>在数据库中找不到的标签名称</returns>
        {
            List<string> results = new List<string>();      //存放找不到的标签

            for (int i = 0; i < tagnames.Length; i++)
            {
                //在pgim三个获得标签信息的函数中，只有该函数能够依次查询每个标签信息。并根据返回值做标签是否存在的判断。
                PgimSignal taginfo = this._pgimDb.ReadDescription("administrator", tagnames[i]);  //使用标签名称，在PGIM数据库中查询标签信息，如果标签不存在，则会返回空值。
                if (taginfo == null)
                {
                    results.Add(tagnames[i]);
                }
            }
            return results;         //返回那些数据库中没有找到的标签
        }
        #endregion

        //查找标签
        #region 获取标签列表 public List<SignalComm> GetTagList(string tagfilter, string tablename = "")
        public List<SignalComm> GetTagList(string tagfilter, string tablename = "")
        /// <summary>
        /// 查找标签
        /// tagfilter是标签名称过滤器，可以是全标签名称，可以是部分标签名称，比如*pump*，则查找名称中含有
        /// </summary>
        /// <returns>在数据库中找到的符合条件的标签名称</returns>
        {
            //PGIM的标签完整描述分为几个部分：
            //——第一段第一部分，表示pgim_server服务器名称
            //——第二段第一部分，表示scanner的名称
            //——第二段第二部分，表示通讯协议名称
            //——第二段第三部分，包含设备及标签点名称

            //QuerySignals()仅返回标签的第二段，还需要手动加上第一段，才能返回完整标签路径            
            List<SignalComm> results = new List<SignalComm>();      //存放找不到的标签
            List<Signal> QuerySignals = new List<Signal>();         //读普通信息：最大、最小、工程单位
            List<PgimSignal> readSignals = new List<PgimSignal>();  //读特殊信息：数据类型，压缩，存储等
            try
            {
                QuerySignals = this._pgimDb.QuerySignals(this._server, tagfilter);  //在pgim三个获得标签信息的函数中，只有该函数是一次获得所有标签的主要信息。注意该接口区别与ReadSignals。这里用ReadSignals不合适
                readSignals = this._pgimDb.ReadSignals(this._userId, tagfilter);     //标签的数据类型，需要这个接口读出来
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }
            try
            {
                //构成标签数据类型字典
                Dictionary<string, string> datatype = new Dictionary<string, string>();
                for (int i = 0; i < readSignals.Count; i++)
                {
                    datatype.Add(readSignals[i].ANum, readSignals[i].ValueClass);
                }

                for (int i = 0; i < QuerySignals.Count; i++)
                {

                    results.Add(new SignalComm("\\\\" + this._server + "\\" + QuerySignals[i].Name,
                                                QuerySignals[i].Description,
                                                QuerySignals[i].Dimension,
                                                QuerySignals[i].Mba.ToString(),
                                                QuerySignals[i].Mbe.ToString(),
                                                datatype[QuerySignals[i].Name]
                                                ));
                }
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            return results;         //返回那些数据库中没有找到的标签

        }
        #endregion



        //当前值（即时值、快照值）的读写
        #region 批量读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValues(string[] tagnames)
        public List<PValue> GetActValues(string[] tagnames)
        {
            List<PValue> pvalues = new List<PValue>();
            List<SignalValue> values = new List<SignalValue>();
            if (!this._pgimDb.IsLogon) return pvalues;
            try
            {
                values = this._pgimDb.ReadCurrents(tagnames);
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }

            //WriteValues()函数写入的是格林威治时间。该接口会根据PGIM服务器windows的时区设定，自动把写入的时间转为格林威治时间
            //PGIM中存储的要求的是格林威治时间。WriteValues也是这么做的，会根据windows设定，自动转换
            //ReadCurrents()函数读出的时间是PGIM数据库存储的原始治时间值。如果使用WriteValues()正确的写入了格林威治时间，那么这里读取的也是格林威治时间。
            SignalsToLocalTime(ref values);             //将读取出来的格林威治时间，自动的转换成服务器时区的时间。
            pvalues = Signals2PValues(values);

            return pvalues;

        }
        public List<PValue> GetActValues(int[] tagids)
        {
            //PGIM不存在通过id访问的方式
            List<PValue> pvalues = new List<PValue>();
            return pvalues;
        }
        #endregion

        #region 单个读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValue(string[] tagnames)
        public PValue GetActValue(string tagname)
        {
            List<PValue> pvalues = new List<PValue>();
            List<SignalValue> values = new List<SignalValue>();
            string[] tagnames = { tagname };
            if (!this._pgimDb.IsLogon) return pvalues[0];

            try
            {
                values = this._pgimDb.ReadCurrents(tagnames);
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }

            //ReadCurrents()函数读出的时间不是本地时间，需要单独对时间值进行转换
            SignalsToLocalTime(ref values);
            pvalues = Signals2PValues(values);
            //下面是记录读取指令的log
            //LogHelper.Write(LogType.Debug, string.Format("PGIM: {0},{1},{2},{3}", tag, starttime, endtime, values.Count));            

            return pvalues[0];

        }
        public PValue GetActValue(int tagid)
        {
            //PGIM不存在通过id访问的方式
            List<PValue> pvalues = new List<PValue>();
            return pvalues[0];
        }
        #endregion

        #region 批量写入数据库标签点的当前值（即时值、快照值）：public int SetActValues(string tag,PValue pvalue)
        public int SetActValues(string[] tagnames, List<PValue> pvalues)
        {

            for (int i = 0; i < tagnames.Length; i++)
            {
                SetActValue(tagnames[i], pvalues[i]);
            }
            return 0;

        }
        public int SetActValues(int[] tagids, List<PValue> pvalue)
        {
            //PGIM不支持tagid的读写
            List<SignalValue> signalvalues = new List<SignalValue>();
            return 0;
        }
        #endregion

        #region 写入单个数据库标签点的当前值（即时值、快照值）：public int SetActValue(string tag,PValue pvalue)
        public int SetActValue(string tag, PValue pvalue)
        {
            //PGIM写入时间，会自动转换成格林威治时间。
            List<SignalValue> signalvalues = new List<SignalValue>();
            signalvalues.Add(PValue2SignalValue(pvalue));
            try
            {
                this._pgimDb.WriteValues(tag, signalvalues);
                return 0;
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }

        }
        public int SetActValue(int tagid, PValue pvalue)
        {
            //PGIM不支持tagid的读写
            List<SignalValue> signalvalues = new List<SignalValue>();
            return 0;
        }
        #endregion

        //历史值的读写
        #region 读取历史数据原始值：public List<PValue> GetRawValues(string tagname, DateTime startdate, DateTime enddate)重点函数，概化计算引擎用。
        /// <summary>
        /// GetRawValues是实时库读取历史数据接口的对外封装。无论何种实时数据库，读取历史数据功能都是指，读取某一个指定标签（tagname）在一段时间内（startdate、enddate）的历史值（浮点型）
        /// 而不同的实时数据库，真正的读取方法和返回值的数据类型略有区别
        /// ——经测试，_pgimDb.ReadRaw读取数据的数据量又限值
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns>历史数据，PValue集合</returns>
        public List<PValue> GetRawValues(string tagname, DateTime startdate, DateTime enddate)
        {
            //什么时候需要抛出异常：如果在Helper层抛出异常，才RTDBDAO层，就会捕捉异常，并且根据参数设置进行重读。
            //——所以，需要进行数据重新读取的地方，要抛出异常
            //——无需进行数据重新读取，需要直接跳下一个数据去读的地方，直接返回null

            List<PValue> pvalues = new List<PValue>();

            if (!this._pgimDb.IsLogon) 
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet登录参数IsLogon为false，连接已经关闭！";
                throw new Exception(this._exception);     //将pgim的异常继续向外传递。
            }
            if (startdate >= enddate) return null;   //如果起始和截止时刻大小不对，直接返回null。不报错。计算引擎中不会出现
                
            //标签名称的的处理
            //——tagname标签名称中的路径\\在计算引擎读取时为了不发生转义符歧义错误，统一转换为^。在真正读取的时候,读写接口，还是要转换为\
            tagname = tagname.Replace("^", "\\");

            #region 20181227修改，新的实时数据库接口，仅返回阶梯状曲线，不再提供标志位，以不同方式返回曲线
            /*
                //读取方式判断
            //——如果标签名称中有用逗号分割，并带标志位step，则在起始点没有值时，使用端点前的值直接作为端点值。即视曲线为阶梯状
            //——如果标签名称中没有逗号分割的标志位inner，则需要在端点进行插值。即视曲线为折线状。
            bool InterploationTerminal=true;                                                //在端点进行插值的标志位，正常情况下都要进行插值
            string[] tagnameStrs = Regex.Split(tagname, ",|，");
            if (tagnameStrs.Length == 2 && tagnameStrs[1].Trim().ToUpper() == "STEP")    //如果标签名称中含有“step”标志位，比如01U_UL52-9XI01_F_VALUE,inner，则不在端点进行插值，而直接使用前值。
            {
                InterploationTerminal = false;
                tagname = tagnameStrs[0];
            }
            */
            #endregion

            //关于返回数据与时间段的关系：
            //——1、ReadRaw()返回的头一个数据是startdate时间前，离startdate最近的一个原始值。包括这个时刻和这个时刻的值。如果startdate时刻点恰好有值，则返回的及时startdate时刻的值
            //——2、ReadRaw()返回的最后一个数据是enddate时间前，离enddate最近的一个原始值。包括这个时刻和这个时刻的值。如果enddate时刻点恰好有值，则返回的及时startdate时刻的值
            //——3、如果读取时间段内没有数据，ReadRaw()会返回截止时刻前第一个有效数据。如果一直没有有效数据，会返回标签初始化时1970年的初始化值。如果初始化值没有则返回截止时刻以后的第一个有效值。
            //关于时区：
            //ReadRaw()方法与上面的ReadCurrent不同。PGIM数据库一般存储的是格林威治标准时间。而ReadRaw()读出的时间有两种情况。
            //——1、第一种情况，ReadRaw()不加时区参数，则此时ReadRaw()根据PGIM\plantconnect.sys\syskonfi\timezone\timezone.txt的设定，对数据时间进行自动转换。
            //   这种情况下，入参的startdate和endate，会根据timezone.txt，转换到对应的格林威治时间段，然后从从数据库取出数据，再根据timezone.txt转为指定时区的时间段
            //   总结一句话，就是入参startdate、enddate，返回的数据，都会跟着timezone.txt定义的时区差来转
            //——2、第二种情况，ReadRaw()加时区参数，则此时ReadRaw()根据时区参数自动转换时间。
            //   这种情况下，startdate和enddate，会根据时区参数，转为格林威治时间，然后从数据库找对应时间的数据，取出后，在把时间转化为指定时区的时间
            //——3、在本helper中，为了和其他数据库保持通用和一直，我们规定GetRawValues读取的时间，都要能够自动的转换成为helper所在客户端的时区时间。
            //   因此我们采用带时区的ReadRaw()，设定时区参数为GMT，
            //   这样读取的数据返回的都是pgim的格林威治时间，在读出以后转成客户端时区时间。
            //   当时区参数设置为GMT时，ReadRaw()默认需要startdate，enddate也是格林威治时间，因此我们要先把startdate、enddate从本地时间转为格林威治时间
            //关于读取数量：
            //——ReadRaw()一次读取的数据是有限的，如果startdate和enddate之间的数据超过22w条，则ReadRaw()会直接报错，这点和golden数据库不同。
            //——第四个参数top不能够防止startdate和enddate之间的数据过多时的出错。仅仅是在数据量小于22w条，ReadRaw()可以读取历史数据的情况下，限定返回这些数据的前多少条
            List<SignalValue> list = new List<SignalValue>();
            TimeZoneType timezone = TimeZoneType.GMT;
            
            try
            {
                //var swTimer = Stopwatch.StartNew();               //用于测试读接口速度
                list = this._pgimDb.ReadRaw(this._userId, tagname, startdate.ToUniversalTime(), enddate.ToUniversalTime(), 200000, timezone);
                //realSpan = swTimer.Elapsed.ToString();            //用于测试读接口速度
            }
            //关于出错处理
            //由于helper的原则是不对错误进行处理，错误处理放在DAO层。由不同的DAO层根据自己的需要来处理错误。
            //因此需要对helper层出错的可能性有大概了解。不同数据库的接口，出错机制不一样。以下是pgim的ReadRaw()常见出错情况，需要在DAO层做相应处理
            //0、服务器的available client没有空余，会报错
            //1、pgim的标签名称中含有服务器节点名称。当服务器节点名称与实际连接的服务器名称不一致，会报此错误。
            //2、如果标签中，表示scanner服务器名称、scanner协议名称、点名的部分不对。会报错。
            //3、如果读取时间内，返回的数据量过多，则报错
            //以上错误，需要在DAO中进行处理。
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中路径部分对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }
            
            //以下判断条件包含了以下非正常情况：
            //1、如果读取时间段内没有数据，且起始时刻前有非正常状态（停机，dcs断电），则返回list.count=0.
            //2、如果读取时间段内没有数据，起始时刻之前也没有数据，则返回1970年初始化值
            //3、如果读取时间段内没有数据，且初始化值也没有，会返回截止时刻以后的第一个有效值。list[list.Count - 1].Timestamp > enddate.ToUniversalTime()
            //4、其他非正常状态，list=null
            DateTime iniDateTime = DateTime.Parse("1971-01-01");
            if (list == null || list.Count == 0 || list[0].Timestamp < iniDateTime ||list[list.Count - 1].Timestamp > enddate.ToUniversalTime() ) return null; //直接返null,外部程序可以根据pvalues.count是否为0来处理
            

            //——pgim在数据库中寻找与startdate相等于或早于startdate的第一个值，作为返回的第一个值。list第一个值即为startdate时刻或之前第一个有效值
            //——pgim在数据库中寻找与enddate相等或早于endate的第一个值，作为返回的最后一个值。list最后一个值即为endtime时刻或之前第一个有效值
            //——注意，此时读取出的数据，时间均为格林威治时间                        
            #region 20181227修改，新的实时数据库接口，仅返回阶梯状曲线，不再设法去取截止时刻后的点来做截止时刻的插值
            /*
            //对最后一个点的处理
            //——pgim默认返回截止时刻或之前一个时刻点的值，在计算引擎体系中，默认需要最后截止时刻的插值，因此需要取找最后时刻后的值
            //——经测试，PGIM的ReadAggregates，使用起始时刻和截止时刻，得不到这些时刻点的插值。PGIM提供插值的方式，仍然是阶梯状的。
            //——因此，想要得到截止时刻的插值，必须通过ReadRaw找到截止时刻后面的值，才能去求插值            
           
            if (list[list.Count - 1].Timestamp < enddate.ToUniversalTime())
            {
                //从截止时刻开始向后一天，返回最前面的两个值。因为list[list.Count - 1].Timestamp < enddate.ToUniversalTime。
                //所以，第一个值，肯定是结束时刻前的一个值。第二个值肯定是结束时刻后的时间最近的一个值。
                List<SignalValue> last = new List<SignalValue>();
                try
                {
                    //var swTimer1 = Stopwatch.StartNew();             //用于测试读接口速度
                    last = this._pgimDb.ReadRaw(this._userId, tagname, enddate.ToUniversalTime(), enddate.AddDays(1).ToUniversalTime(), 2, timezone);   //向后1天如果查不到合适的数据，就用最末尾的数据。向后的时间不能太长，否则ReadRaw的耗时将大幅增加
                    //realSpan = swTimer1.Elapsed.ToString();          //用于测试读接口速度
                }
                catch (PgimNet.Exceptions.PgimDataException ex)
                {
                    //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                    this._exception = ex.Message.ToString();
                    throw ex;     //将pgim的异常继续向外传递。                
                }
                catch (PgimNet.Exceptions.PgimException ex)
                {
                    //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                    this._exception = ex.Message.ToString();
                    throw ex;     //将pgim的异常继续向外传递。                
                }
                catch (Exception ex)
                {
                    //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                    this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                    throw ex;     //将pgim的异常继续向外传递。                
                }

                if (last != null && last.Count != 0 && last.Count == 2)
                {
                    list.Add(last[1]);  //如果在截止时刻后一个月之内，可以找到截止时刻后面的一个值，则将该值加入到list
                }
                else
                    list.Add(new SignalValue(enddate.ToUniversalTime(), list[list.Count - 1].Value, list[list.Count - 1].Status));  //如果在截止时刻后一个月之内，找不到截止时刻后的有效值，则用截止时刻前的值作为截止时刻值。

            }
            */
            #endregion
            pvalues = Signals2PValuesHist(list);

            //最后得到的list是：
            //——从startdate或之前第一个有效值从开始，到截止时间enddate或之后第一个有效值为止的点集合. 
            //ReadRaw()返回0个点的情况，上面直接退出，返回空list。由外部RTDBDAO处理。
            //ReadRaw()如果返回至少一个值，经过补足，list最后至少有两个点
            //将结果转换为List<PValus>
            //list的时间均为GMT时间，pvalues的时间均为本地时间
            //调整起始时刻的值。
            
            //如果恰好起始时刻前无前值，但是起始时刻和截止时刻间有值，则只会返回缺失起止时刻到第一个有效值之间数据的结果，此时用第一个数据去补全缺失段（会造成计算不准确，但是这种情况是特例）。
            if (pvalues[0].Timestamp > startdate)
            {
                pvalues[0].Timestamp = startdate;
            }
            //如果第一个值小于起始时刻，则直接用前一时刻的值，作为起始时刻的值。
            if (pvalues[0].Timestamp < startdate)
            {                 
                pvalues[0].Timestamp = startdate;     //将第一个点的时间戳替换为起始时间
            }
            //调整截止时刻的值。
            if (pvalues[pvalues.Count-1].Timestamp < enddate)
            {
                //如果最后一个点的时间戳小于截止时刻，则说明没有找到截止时刻后值
                pvalues[pvalues.Count - 1].Endtime = enddate;
                pvalues.Add(new PValue(pvalues[pvalues.Count - 1].Value, enddate, enddate, pvalues[pvalues.Count - 1].Status)); //添加截止时刻点 
            }
            else if (pvalues[pvalues.Count - 1].Timestamp == enddate)
            {
                //恰好有截止时刻点，将该值作为截止时刻值
                pvalues[pvalues.Count - 1].Endtime = enddate;
            }
            else
            {
                //取到截止时刻点后值
                pvalues[pvalues.Count - 2].Endtime = enddate;           //修改前一个值的结束时刻到enddate
                //将最后一个值，变为截止时刻点
                pvalues[pvalues.Count - 1].Value = pvalues[pvalues.Count - 2].Value;
                pvalues[pvalues.Count - 1].Timestamp = enddate;
                pvalues[pvalues.Count - 1].Endtime = enddate;
                pvalues[pvalues.Count - 1].Status = pvalues[pvalues.Count - 2].Status;
            }  

            return pvalues;

        }
        public List<PValue> GetRawValues(int tagid, DateTime startdate, DateTime enddate)
        {
            //PGIM不支持tagid读写
            List<PValue> pvalues = new List<PValue>();
            return pvalues;
        }
        #endregion

        #region 读取历史数据插值，固定间隔：public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int count)
        public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int count)
        {
            List<PValue> pvalues = new List<PValue>();
            List<SignalValue> values = new List<SignalValue>();
            if (!this._pgimDb.IsLogon) return pvalues;

            //由时间间隔和指定的读取量，计算取数时间间隔
            TimeSpan ts = enddate - startdate;
            double interval = Math.Floor(ts.TotalSeconds / count);

            //关于ReadAggregates接口定义，请直接到定义中查找
            CompressType compress = CompressType.Value;     //关于CompressType请到定义中去查找。这里可选选择去实际值、最小值、最大值、均值、积分、求和等等
            string resolution = interval.ToString() + " sec";
            try
            {
                values = this._pgimDb.ReadAggregates(tagname, startdate, enddate, compress, resolution, count);
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }

            //根据取整的时间间隔，重新计算结束时间
            enddate = startdate.AddSeconds(interval * count);
            pvalues = Signals2PValuesHistForInterval(values, startdate, enddate);
            //下面是记录读取指令的log
            //LogHelper.Write(LogType.Debug, string.Format("PGIM: {0},{1},{2},{3}", tag, starttime, endtime, values.Count));

            return pvalues;

        }
        public List<PValue> GetIntervalValuesFixInterval(int tagid, DateTime startdate, DateTime enddate, int count)
        {
            //PGIM不支持tagid读写
            List<PValue> pvalues = new List<PValue>();
            return pvalues;
        }
        #endregion

        #region 读取历史数据统计值：public List<PValue> GetSummaryValues(string tagname,DateTime startdate,DateTime enddate,string type)
        public PValue GetSummaryValues(string tagname, DateTime startdate, DateTime enddate, string type)
        {
            PValue pvalue = new PValue();
            List<SignalValue> values = new List<SignalValue>();
            if (!this._pgimDb.IsLogon) return pvalue;

            //由时间间隔和指定的读取量，计算取数时间间隔
            //ReadAggregates()函数取统计值，是按照resolution的间隔得时间段来取统计值。
            //比如起始时间和结束时间，间隔5分钟，而resolution是"1 min"，则ReadAggregates()返回每一个分钟内的统计值。
            //如果要找起始时间和结束时间这个间隔的统计值，必须把resolution和这个间隔设置为一致
            TimeSpan ts = enddate - startdate;
            double interval = ts.TotalSeconds;

            //关于ReadAggregates接口定义，请直接到定义中查找               
            string resolution = interval.ToString() + " sec";

            //确定要取得统计值
            CompressType compress;
            switch (type.ToLower())
            {
                case "calcavg":
                    compress = CompressType.Avg;
                    break;
                case "total":
                    compress = CompressType.Sum;
                    break;
                case "max":
                    compress = CompressType.Max;
                    break;
                case "min":
                    compress = CompressType.Min;
                    break;
                default:
                    compress = CompressType.Avg;
                    break;
            }
            //读取统计值
            try
            {
                values = this._pgimDb.ReadAggregates(tagname, startdate, enddate, compress, resolution, 1);
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。                
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。                
            }
            pvalue = new PValue(values[0].Value, enddate, values[0].Status);

            return pvalue;

        }
        public PValue GetSummaryValues(int tagid, DateTime startdate, DateTime enddate, string type)
        {
            //PGIM不支持tagid读写
            List<PValue> pvalues = new List<PValue>();
            return pvalues[0];
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
            List<SignalValue> signalvalues = new List<SignalValue>();
            for (int i = 0; i < data.Length; i++)
            {
                signalvalues.Add(String2SignalValue(data[i]));
            }
            //PGIM的写历史数据对一次写入数据量有要求。经过测试，一次写入50000条数据可以。写入100000条数据会报错。
            //需要在DAO层，对写入的signalvalues进行限制，并做错误处理
            //该接口的tagname，在访问本地pgim server时，tagname可以不包含服务器那一段标识。
            //该接口的tagname，在访问远端pgim server时，tagname必须包含服务器那一段标识。
            try
            {
                this._pgimDb.OverwriteValues(tagname, signalvalues);
                return signalvalues.Count();
            }
            catch (PgimNet.Exceptions.PgimDataException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。 
                return -1;
            }
            catch (PgimNet.Exceptions.PgimException ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = ex.Message.ToString();
                throw ex;     //将pgim的异常继续向外传递。
                return -1;
            }
            catch (Exception ex)
            {
                //将pgim的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PgimNet其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。  
                return -1;
            }
        }
        public int PutArchivedValues(int tagid, string[][] data)
        {
            //PGIM不支持tagid的读写
            List<SignalValue> signalvalues = new List<SignalValue>();
            return 0;
        }
        #endregion

        //辅助函数：数据格式转换
        #region 为写入当前值（即时值、快照值）准备的转换函数,将PValue转换为SignalValue public SignalValue PValue2SignalValue(PValue pvalue)
        public SignalValue PValue2SignalValue(PValue pvalue)
        {
            SignalValue signalvalue = new SignalValue(pvalue.Timestamp, pvalue.Value, (int)pvalue.Status);
            return signalvalue;
        }
        public SignalValue String2SignalValue(string[] svalue)
        {
            DateTime timestamp = DateTime.Parse(svalue[2]);
            double value = Convert.ToDouble(svalue[1]);
            int status = int.Parse(svalue[3]);
            SignalValue signalvalue = new SignalValue(timestamp, value, status);
            return signalvalue;
        }
        #endregion

        #region 为读取当前值（即时值、快照值）准备的转换函数, private List<PValue> Signals2PValues(List<SignalValue> values)
        //为读取当前值（即时值、快照值）准备的转换函数
        //读取原始数据时，PGIM返回starttime前最近的一个时间，endtime后最近的一个时间。
        //格式转换时，需要对首位的时间进行截取
        private List<PValue> Signals2PValues(List<SignalValue> values)
        {
            List<PValue> pvs = new List<PValue>();
            switch (values.Count)
            {
                case 0: break;
                default:
                    for (int i = 0; i < values.Count; i++)
                    {
                        pvs.Add(new PValue(values[i].Value, values[i].Timestamp, values[i].Status));
                    }
                    break;
            }
            return pvs;
        }
        #endregion

        #region 为读取历史数据原始值准备的转换函数 private  List<PValue> Signals2PValuesHist(List<SignalValue> values)
        //为读取历史数据原始值准备的转换函数
        //读取原始数据时，如果截止时刻点后面有值，PGIM返回starttime前最近的一个时间，endtime后最近的一个时刻值。
        //如果截止时刻点后面没有值，PGIM仅返回endtime前最近一个时刻值。
        //其头部begin时间点，有可能恰好对应一个原始值，有可能是时间经过startdate调整的值
        //其尾部end时间点，有可能恰好对应一个原始值，有可能是用end时刻补足的一个值。
        //ReadRaw()返回0个点的情况，上面直接退出，返回空list。
        //ReadRaw()如果返回至少一个值，经过补足，list最后至少有两个点
        private List<PValue> Signals2PValuesHist(List<SignalValue> values)
        {
            List<PValue> pvalues = new List<PValue>();

            for (int i = 0; i < values.Count; i++)
            {
                pvalues.Add(new PValue(values[i].Value, values[i].Timestamp.ToLocalTime(), values[i].Timestamp.ToLocalTime(), values[i].Status));
                if (i != 0)
                    pvalues[i - 1].Endtime = values[i].Timestamp.ToLocalTime();
            }
            
            return pvalues;
        }
        #endregion

        #region 为读取历史数据插值准备的转换函数 private List<PValue> Signals2PValuesHistForInterval(List<SignalValue> values, DateTime startime, DateTime endtime)
        //读取插值数据时，结束时间endtime是经过调整的时间
        private List<PValue> Signals2PValuesHistForInterval(List<SignalValue> values, DateTime startime, DateTime endtime)
        {
            List<PValue> pvs = new List<PValue>();
            switch (values.Count)
            {
                case 0: break;
                case 1: pvs.Add(new PValue(values[0].Value, startime, endtime, values[0].Status)); break;
                default:

                    for (int i = 0; i < values.Count - 1; i++)
                    {
                        pvs.Add(new PValue(values[i].Value, values[i].Timestamp, values[i + 1].Timestamp, values[i].Status));
                    }
                    pvs.Add(new PValue(values[values.Count - 1].Value, values[values.Count - 1].Timestamp, endtime, values[values.Count - 1].Status));
                    break;
            }
            return pvs;
        }
        #endregion

        #region 时区转换
        //ReadCurrents()接口读出的时间不是本地时间，而是PGIM库中存储的原始时区，即格林威治时间。需要转换为本地时区。
        private void SignalsToLocalTime(ref List<SignalValue> values)
        {
            DateTime temp = new DateTime();
            for (int i = 0; i < values.Count; i++)
            {
                temp = values[i].Timestamp.ToLocalTime();
                values[i].Timestamp = temp;
            }
        }
        #endregion
    }
}
