using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using PCCommon;
using PISDK;            //注意，必须放在DBInterface.RTDBInterface之外，与golden、pgim不同
using PISDKCommon;      //注意，必须放在DBInterface.RTDBInterface之外，与golden、pgim不同

namespace DBInterface.RTDBInterface
{


    /// <summary>
    /// PIHelper
    /// PI实时数据库的连接方法。
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
    public class PIHelper : BaseRTDbHelper, IRTDbHelper, IRTDbHelperExpand
    {
        public string _server = "192.168.1.55";
        public string _port = "5450";                   //PI的默认端口
        public string _userId = "piadmin";              //PI的默认管理员账户
        public string _password = "";                   //PI的管理员账户piadmin默认没有密码
        public string _connectionString;                //PI连接字符串，特殊，仅需要UID和PWD         

        private PISDK.PISDK _piSDK = new PISDK.PISDK(); //PISDK实例，PI数据库连接对象
        private PISDK.Server _piServer;                 //server实例
        private PISDK.PIPoint _piPoint;                 //point实例    
        private string _exception;                      //内部异常
        //private bool _isLogon;                        //不在使用内部_isLogon，因为不能及时反映连接的的状态

        /// <summary>
        /// 当前实时库数据库类型。该参数必须在具体的实时数据库连接方法中重定义。
        /// </summary>
        public override CurrentRTDbType CurrentRTDbType
        {
            get
            {
                return CurrentRTDbType.PI;
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
                if (this._piServer != null && _piServer.Connected==true)                
                    return true;                
                else
                    return false;   
            }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public override string ConnectionString
        {
            //在PI数据库中，连接数据库函数不直接使用连接字符串连接，因此在PI中需要对set重写。
            //当发生set时，对连接字符串进行解析,找出_server、_port、_userId、_password
            set
            {
                string[] _connArray = value.Split(';');

                this._server = _connArray.Length >= 1 ? _connArray[0].Substring(_connArray[0].IndexOf('=') + 1) : this._server;
                this._port =  _connArray.Length >= 2 ? _connArray[1].Substring(_connArray[1].IndexOf('=') + 1) : this._port;
                this._userId = _connArray.Length >= 3 ? _connArray[2].Substring(_connArray[2].IndexOf('=') + 1) : this._userId;
                this._password=_connArray.Length >= 4 ?  _connArray[3].Substring(_connArray[3].IndexOf('=') + 1) : this._password;
                //外部的Connectstring保持xml格式统一
                //内部的_connectionString按照各数据库要求走，比如PGIM、golden、Pi各不相同
                this._connectionString = string.Format("UID={0};PWD={1};", this._userId, this._password);
            }
        }

        #region public PIHelper() 构造函数：不带连接字符串的构造函数。工厂的反射调用该方法。
        /// <summary>
        /// 构造函数
        /// </summary>
        public PIHelper()
        {
            //不带连接字符串参数的构造函数，
            //——用实时数据库连接方法的内部属性值_server、_port、_userId、_password以有的值，构建ConnectionString属性  
            //——在ConnectionString内部，_server、_port、_userId、_password将获得原值，但是真正的连接_connectionString，将获得组合值
            this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
            FileName = "PIHelper.txt";   // sql查询句日志文件名称
        }
        #endregion

        #region public PIHelper(string connectionString) 构造函数：带连接字符串的构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public PIHelper(string connecStr)
            : this()
        {
            //带连接字符串参数的构造方法
            //——用传入的外部参数connecStr的值取初始化接字符串属性ConnectionString；            
            //当参数为空时，相当于不带参数的构造方法，所执行的内容。
            if (connecStr != null)
            {
                //初始化时，如果外部给进连接字符串，就要用外部给的字符串给ConnectionString赋值。外部字符串是符合统一标准的格式
                //ConnectionString赋值时，就会去调用set
                //此时，实例对象会获得新的_server、_port、_userId、_password值以及Pi所需要的新的连接字符串_connectionString。
                this.ConnectionString = connecStr;
            }
            else
            {
                //如果外部connectionString为空
                //——用实时数据库连接方法的内部属性值_server、_port、_userId、_password以有的值，构建ConnectionString属性
                //——初始化ConnectionString时，会调用set，
                //此时，实例对象_server、_port、_userId、_password值保持不变，但是连接字符串_connectionString会获得PI所需要的组合值。
               
                this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
            }
            FileName = "PIHelper.txt";   // sql查询句日志文件名称
        }
        #endregion

        //数据库连接与断开
        //——由于初始化时，可以选择带字符串的初始化，或者不带字符串的初始化，因此Logon时，就不再提供两种情况
        //——即，在初始化时，可以选择用连接字符串取初始化不同连接。
        //——但是，连接时，应当认为，在初始化的时候，已经选好了连接目标
        #region public String Logon() 获取数据库连接：不带连接字符串的连接函数
        public String Logon()
        {
            try
            {
                //_piServer==null,是第一次建立logon的时候
                //_piServer==null==false
                if (this._piServer == null || this._piServer.Connected == false)
                {
                    this._piServer = this._piSDK.Servers.DefaultServer;     //服务器对象
                    this._piServer.Path = this._server;                     //ip地址
                    this._piServer.Port = int.Parse(this._port);            //端口
                    this._piServer.Open(_connectionString);                  //用户名称，密码


                }
                if (this._piServer.Connected)
                {
                    //this._isLogon = true; 不再使用_isLogon内部变量，而直接使用公用变量isLogOn的get获得_piServer.Connected的当前值
                    return "logon";
                }
                else
                {
                    //this._isLogon = true; 不再在使用_isLogon内部变量，而直接使用公用变量isLogOn的get获得_piServer.Connected的当前值
                    return "logoff";
                }
            }
            catch (Exception ex)
            {
                _exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pi的异常继续向外传递。   
               
            }
        }
        public String Logon(string connectionString)
        {
            string[] _connArray = connectionString.Split(';');

            this._server = _connArray.Length >= 1 ? _connArray[0].Substring(_connArray[0].IndexOf('=') + 1) : this._server;
            this._port = _connArray.Length >= 2 ? _connArray[1].Substring(_connArray[1].IndexOf('=') + 1) : this._port;
            this._userId = _connArray.Length >= 3 ? _connArray[2].Substring(_connArray[2].IndexOf('=') + 1) : this._userId;
            this._password = _connArray.Length >= 4 ? _connArray[3].Substring(_connArray[3].IndexOf('=') + 1) : this._password;
            //外部的Connectstring保持xml格式统一
            //内部的_connectionString按照各数据库要求走，比如PGIM、golden、Pi各不相同
            this._connectionString = string.Format("UID={0};PWD={1};", this._userId, this._password);
            try
            {
                //_piServer==null,是第一次建立logon的时候
                //_piServer==null==false
                if (this._piServer == null || this._piServer.Connected == false)
                {
                    this._piServer = this._piSDK.Servers.DefaultServer;     //服务器对象
                    this._piServer.Path = this._server;                     //ip地址
                    this._piServer.Port = int.Parse(this._port);            //端口
                    this._piServer.Open(_connectionString);                  //用户名称，密码


                }
                if (this._piServer.Connected)
                {
                    //this._isLogon = true; 不再使用_isLogon内部变量，而直接使用公用变量isLogOn的get获得_piServer.Connected的当前值
                    return "logon";
                }
                else
                {
                    //this._isLogon = true; 不再在使用_isLogon内部变量，而直接使用公用变量isLogOn的get获得_piServer.Connected的当前值
                    return "logoff";
                }
            }
            catch (Exception ex)
            {
                _exception = ex.ToString();
                throw new Exception(ex.ToString());     //将pi的异常继续向外传递。   

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
            if (this._piServer != null)
            {
                this._piServer.Close();                
            }
        }
        #endregion

        //数据库服务
        #region 获取数据库服务器当前时间
        public DateTime GetHostDateTime()
        {
            return DateTime.Now;
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


            List<int> ids = new List<int>();

            return ids.ToArray();
        }
        #endregion

        //检查标签是否存在(2018.04.12完成)
        #region 检查标签是否存在    public List<string> CheckTags(string[] tagnames)
        public List<string> CheckTags(string[] tagnames)
        {
            List<string> results = new List<string>();
            PISDK.PointList QuerySignals = new PISDK.PointList();
            foreach (string tagname in tagnames)
            {
                try
                {
                    QuerySignals = this._piServer.GetPoints(tagname);
                    if (QuerySignals.Count == 0)
                    {
                        results.Add(tagname);   //返回没有找到的标签
                    }
                }
                catch (Exception ex)     //后面这个异常的类型应该改为PI的异常类型
                {
                    results.Add(tagname);   //返回没有找到的标签
                }
            }
            return results;                                     //返回没有找到的标签
        }
        #endregion

        //获取标签列表(2018.04.12完成)
        #region 获取标签列表 public  List<String> GetTagList()
        public List<SignalComm> GetTagList(string tagname, string tablename = "")
        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <returns>在数据库所有符合tagname搜索条件的标签列表（在所有表中查询）</returns>
        {
            List<SignalComm> results = new List<SignalComm>();      //存放标签名
            PISDK.PointList QuerySignals = new PISDK.PointList();

            //对过滤条件tagname进行整理，如果tagname为空，则返回所有标签
            //——这意味着GetTagList的参数tagname，即可以是具体的过滤条件，返回符合条件的对象
            //——也可以是tagname为空，返回所有的标签对象。
            if (tagname == "")
            {
                tagname = "Tag = '*'";  //返回全部标签的写法，参考PI-SDK手册，Server对象，GetPoints()方法
            }

            try
            {
                QuerySignals = this._piServer.GetPoints(tagname);
            }
            catch (Exception ex)     //后面这个异常的类型应该改为PI的异常类型
            {
                //将PI的异常记录在_exception中，并以ex的名称继续抛出异常
                this._exception = ex.ToString();
                throw new Exception(ex.ToString());     //将PI的异常继续向外传递。
            }

            foreach (PIPoint pipoint in QuerySignals)
            {
                results.Add(new SignalComm(pipoint.Name, pipoint.PathName));    //返回标签名和描述。PI数据点，目前没有在手册中找到描述字段的位置。
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


            return pvalues;
        }
        #endregion

        #region 单个读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValue(string[] tagnames)
        public PValue GetActValue(string tagname)
        {
            List<PValue> pvalues = new List<PValue>();


            //准备读值列表PIValues           
            PISDK.PIValue pival;
            this._piPoint = this._piServer.PIPoints[tagname];
            pival = this._piPoint.Data.Snapshot;             //PI的快照值为只读

            //未做完。待续

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

            //PISDKDemo中更新快照值也是使用UpdateValue
            //PISDK.PIPoint point;
            //point = this._piServer.PIPoints[tbxTagName.Text];
            //point.Data.UpdateValue(tbxValue.Text, null, PISDK.DataMergeConstants.dmReplaceDuplicates, status);


            return 0;
        }
        public int SetActValues(int[] tagids, List<PValue> pvalues)
        {

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
        /// 要求起始时刻和截止时刻，必须为精确的实时值。如果恰好有这两个时刻点的值，就直接取得。如果没有就采用插值。
        /// 对于PI数据库，读取历史数据接口，当第三个参数设定为btInterp时，接口会自动的获取起始时刻和截止时刻的插值。
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns>历史数据，PValue集合</returns>
        public List<PValue> GetRawValues(string tagname, DateTime startdate, DateTime enddate)
        {
            //经过测试，PI数据库读取原始数据时，在起始时刻点，和截止时刻点，可以通过参数设置实现以下几种方式：
            //在piPoint.Data.RecordedValues()中的第三个参数，BoundaryType，可以选择：
            //——auto，自动决定边界点
            //——btInside，只取时间戳大于起始时间的点和时间戳小于截止时刻的点。
            //——btOutside，取得实时数据包含起始时刻前一个有效时刻的值，和截止时刻后一个有效时刻的值。
            //——btInterp，在起始时刻和截止时刻，进行插值。
            //对于计算引擎，需要的就是btInterp这种方式，即在起始时刻和截止时刻，进行精确的插值
            //如果已经取到数据库最早的时间值，则不再向前补值。比如数据最早是2018-1-2 00：00：00的值。取值周期起始时刻是2018-1-1。RecordedValues返回的第一个值是2018-1-2 00：00：00的，不会向前补值。
            
            //为了在使用长连接时保险起见，这里仅检查是否连接。如果未连接就直接返回空。
            if (this._piServer==null ||!this._piServer.Connected) return null; //如果未登陆，直接返回空值
             
            //准备读值列表PIValues
            PIValues pivalues = new PIValues();
            if (enddate <= startdate)
            {
                return null;
            }
            try
            {
                this._piPoint = this._piServer.PIPoints[tagname];
                pivalues = this._piPoint.Data.RecordedValues(startdate, enddate, BoundaryTypeConstants.btOutside);   //BoundaryTypeConstants.btInterp采用边界取外值的办法，起始时刻取前值，截止时刻取后值
               
            }
            catch (Exception ex)
            {
                //将pi的异常记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PI其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！详细错误信息：" + ex.ToString();
                throw ex;     //将PI的异常继续向外传递。
            }

            //PI的RecordedValues()接口，返回的数据，起始时刻和截止时刻的值，均为根据前后值插值得到。除此以外，PI还会返回其他标志值，比如停止，IO超驰等。
            //所以，不能依据pivalues>=2，来判断是否包含了完整的起始时刻和截止时刻的值。
            //如果count为0,直接返回空。外部程序可以根据pvalues==null来处理            
            if (pivalues == null || pivalues.Count == 0 ) return null;
            
            //将pivalues转换为Pvalue值
            //——返回的pvalues，由于采用了BoundaryTypeConstants.btOutside读法，正常情况应该包含一个等于或早于起始时刻的首值，包含一个早于、等于或者晚于截止时刻的后值。
            //——pivalues中可能有非数值数据，转换后的pivalues只包含数值型数据
            List<PValue> pvalues = PIValues2PValuesHist(pivalues);
            if (pvalues==null || pvalues.Count == 0)                return null;
           
            //处理首值
            if (pvalues[0].Timestamp < startdate)
            {
                pvalues[0].Timestamp = startdate;       //如果首值的时刻小于起始时刻，说明btOutside方式正确取到了前点。则将前点时刻改为起始时刻
            }
            else if (pvalues[0].Timestamp == startdate)
            {
                //如果首值的时刻恰好等于起始时刻，则什么也不做
            }
            else
            {
                //将pi的异常记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PI接口读取原始数据错误。未正常读取到起始时刻前值！";
                throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。               
            }

            //处理尾值
            if (pvalues[pvalues.Count - 1].Timestamp < enddate)
            {
                //如果最后一个点的时间戳小于截止时刻，则说明没有找到截止时刻后值
                pvalues[pvalues.Count - 1].Endtime = enddate;
                pvalues.Add(new PValue(pvalues[pvalues.Count - 1].Value, enddate, enddate,pvalues[pvalues.Count - 1].Status)); //添加截止时刻点                
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
            //PI不支持tagid读写
            List<PValue> pvalues = new List<PValue>();
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
        private List<PValue> GetFloatIntervalValues(string name, DateTime begin, DateTime end, int count)
        {
            List<PValue> pvalues = new List<PValue>();


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
            //注意，只能向数据库第一个有效数据的时间之后，插入历史数据。
            //——如果插入的值得历史时刻在第一个有效值前，则插入不返回错误，但是数据写不进去。
            //准备数据           
            PISDK.PIValues pivalues = new PISDK.PIValues(); //新建PIValues，ReadOnly属性为true
            pivalues.ReadOnly = false;                      //只有将PIValues的readonly属性置为false，才能写入。

            //数据点的描述，是一个NamedValues类型。该类型是一个集合。可以添加不同的描述项，并给项设定值
            NamedValues nvatts = new NamedValues();
            nvatts.Add("questionable", true);

            //将数据写入pivalues
            //——只使用第1个值即实时值，第2个值时间。
            int i = 0;
            //try
            //{
            for (i = 0; i < data.Length; i++)
            {
                try
                {
                    //在PI导出的数据中，value有可能是字符串。有时候表示服务器启停，有时候表示“IO timeout”等，这些值都无法转换成数值型，必须跳过。
                    if (data[i] != null && data[i].Length != 0)
                        pivalues.Add(Convert.ToDateTime(data[i][2]), Convert.ToDouble(data[i][1]), nvatts);
                }
                catch
                {
                    continue;
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    //将pi的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
            //    this._exception = "写历史数据时，转换数据格式错误！错误行数"+i.ToString();
            //    throw ex;     //将pgim的异常继续向外传递。  
            //}

            //使用piPoint.Data.UpdateValues更新点历史数据
            //this._piPoint = new PISDK.PIPoint();
            try
            {
                this._piPoint = this._piServer.PIPoints[tagname];
                PISDKCommon.PIErrors pierror = this._piPoint.Data.UpdateValues(pivalues);
                //处理写入结果
                return pierror.Count;
            }
            catch (Exception ex)
            {
                //将pi的异常PgimDataException记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "PI其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！";
                throw ex;     //将pgim的异常继续向外传递。  
                return -1;
            }

        }
        public int PutArchivedValues(int tagid, string[][] data)
        {
            //PI不支持tagid
            return 0;
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


        #region 辅助函数：数据格式转换
        #region 为读取历史数据原始值准备的转换函数 private  List<PValue> Signals2PValuesHist(List<SignalValue> values)        
        private List<PValue> PIValues2PValuesHist(PIValues pivalues)
        {
            //为读取历史数据原始值准备的转换函数       
            //values为空时，上面直接退出，返回空list。
            //values不为空时，至少会有两个值。PI会在起始时间和截止时间插值
            List<PValue> pvalues = new List<PValue>();
            //pivalues下标从1开始
            foreach (PIValue pivalue in pivalues)
            {
                //20181224日，重新编写转换函数
                /*
                try
                {
                    DateTime timestamp = DateTime.Parse(pivalue.TimeStamp.LocalDate.ToString());

                    //这个在转换过程中可能出错，出错时，直接跳过
                    //——一是有可能是非数值变量，那不可能转换成双精度。
                    //——二是数值型，在测试时，有时也不能转换为双精度。没有搞清楚
                    double value = Convert.ToDouble(pivalue.Value);

                    //——对于PI数据状态位，比较复杂，还没哟搞清楚，暂不处理。状态位先统一给好质量0.
                    //——后面需要对状态位进行判断。

                    pvalues.Add(new PValue(value, timestamp, timestamp, 0));

                    if (pvalues.Count > 1) pvalues[pvalues.Count - 2].Endtime = timestamp;  //修改上一个数据的endtime
                    //上面这条语句，PI取出的最后一条数据不会执行。
                    //PI语句只要能取出取数，在插值方式下，最后一个有效数据一定为取数周期截止时刻的数据。
                    //因此，pvalues的最后一个数据，就是截止时刻数据，并且该Pvalue数据的起始时刻和截止时刻，均为取数周期的截止时刻。
                }
                catch (Exception ex)
                {
                    continue;
                }
                */

                try
                {
                    //pivalue.IsGood()，见PI-SDK Help文档
                    //isGood()方法用来判断PIValue对象的PIValue.Value属性值是一个有效数据值，还是一个错误状态值。
                    //——如果isGood返回false，则PIValue.Value属性将会包含一个从系统数字状态集（System digital state set）中返回的数字状态。具体见DigitalState Object主题
                    //——但是isGood返回true，则不能作为判断PIValue是否好数据关键。因为任何一个PIValue只要包含DigitalState，那么即使isGood为true，数据仍然是坏数据。
                    
                    if(pivalue.IsGood())
                    {
                        //时间
                        DateTime timestamp = DateTime.Parse(pivalue.TimeStamp.LocalDate.ToString());
                        
                        //状态
                        long status=0; 
                        //这个在转换过程中可能出错，出错时，直接跳过
                        //——一是有可能是非数值变量，那不可能转换成双精度。
                        //——二是数值型，在测试时，有时也不能转换为双精度。没有搞清楚
                        double pv=0;
                        if (double.TryParse(pivalue.Value.ToString(), out pv))
                            pv = Convert.ToDouble(pivalue.Value);
                        else
                        {
                            pv = 0;
                            status = 1;
                        }
                        //——对于PI数据状态位，比较复杂，还没哟搞清楚，暂不处理。状态位先统一给好质量0.
                        //——后面需要对状态位进行判断。

                        pvalues.Add(new PValue(pv, timestamp, timestamp, status));

                        if (pvalues.Count > 1) pvalues[pvalues.Count - 2].Endtime = timestamp;  //修改上一个数据的endtime
                        //上面这条语句，PI取出的最后一条数据不会执行。
                        //PI语句只要能取出取数，在btOutside方式下，最后一个有效数据有可能是enddate前，enddate后，也可能等于enddate。 
                    }
                    else
                    {
                        //时间
                        DateTime timestamp = DateTime.Parse(pivalue.TimeStamp.LocalDate.ToString());
                        
                        //状态
                        long status=1; 

                        //值
                        double pv=0;

                        //添加结果
                        pvalues.Add(new PValue(pv, timestamp, timestamp, status));

                        //修改时间
                        if (pvalues.Count > 1) pvalues[pvalues.Count - 2].Endtime = timestamp;  //修改上一个数据的endtime
                    }
                }
                catch (Exception ex)
                {
                    //将pi的异常记录在_exception中，并以PgimDataException的名称继续抛出异常
                    this._exception = "PI数据查询结果PIValue数据格式转换错误。通常应该是pivalue.TimeStamp类型转换错误！" + ex.ToString();
                    throw ex;     //将PI的异常继续向外传递。
                }
            }

            return pvalues;
        }
        #endregion
        #endregion
    }
}
