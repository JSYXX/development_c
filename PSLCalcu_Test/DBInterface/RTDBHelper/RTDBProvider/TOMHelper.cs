using DBInterface.RTDBInterface;
using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBInterface.RTDBInterface
{
    public class TOMHelper : BaseRTDbHelper, IRTDbHelper, IRTDbHelperExpand
    {
        public string _server = "127.0.0.1";
        public string _port = "921";
        public string _userId = "admin";
        public string _password = "admin";
        public string _connectionString;
        private string _exception = "";       //例外信息
        int _rtdbconnection =-1;    //连接句柄
        int nret = -1;
        private System.Collections.ArrayList _objs = new System.Collections.ArrayList();//对象数组，用于存放回调函数读取的标签或历史记录
        
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
                this._server = _connArray[0].Substring(_connArray[0].IndexOf('=') + 1);
                this._port = _connArray[1].Substring(_connArray[1].IndexOf('=') + 1);
                this._userId = _connArray[2].Substring(_connArray[2].IndexOf('=') + 1);
                this._password = _connArray[3].Substring(_connArray[3].IndexOf('=') + 1);
                this._connectionString = value;     //这里主要是存储后，对外查询时用
            }
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        public String Logon()
        {
            try
            {
                //不带连接字符串的连接函数，采用实例化时对象属性this.ConnectionString决定的_server、 _userId、 _password进行连接 
                string serverip = this._server;
                System.UInt16 serverport = System.UInt16.Parse(this._port);
                string username = this._userId;
                string password = this._password;
                if (this._rtdbconnection == -1 && this.nret==-1)
                {
                   // Console.WriteLine("开始连接");
                   // Console.WriteLine("连接信息" + this._rtdbconnection + " " + serverip + " " + serverport + " " + username + " " + password);
                    this._rtdbconnection = rdb_create();  //创建句柄
                    this.nret = rdb_connect(this._rtdbconnection, serverip, serverport, username, password);//连接数据库
                   // Console.WriteLine("this._rtdbconnection:" + this._rtdbconnection + ",this.nret" + this.nret);
                }
                if (this._rtdbconnection>=0)
                {
                    if (this.nret != def.DBR_SUCCESS)
                        return "logoff";
                    else
                        return "logon";
                }
                else
                    this._rtdbconnection = -1;
                return "logoff";
            }
            catch (Exception ex)
            {
                this._rtdbconnection = -1;
                _exception = ex.ToString();
                throw new Exception(ex.ToString());     //将数据库的异常继续向外传递。                  
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
                try
                {
                    if (this.nret != def.DBR_SUCCESS)
                        return false;
                    else
                        return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Logoff() {
            rdb_disconnect(this._rtdbconnection);
            this._rtdbconnection = -1;  //创建句柄
            this.nret =-1;              //数据库连接状态
        }
        public DateTime GetHostDateTime()
        {
            return DateTime.Now;
        }
        public List<string> CheckTags(string[] tagnames) {
            return null;
        }
        public List<SignalComm> GetTagList(string tagname, string tablename = "")
        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <returns>在数据库所有符合tagname搜索条件的标签列表（在所有表中查询）</returns>
        {
            int nret = rdb_tagquery(this._rtdbconnection, tagname, "", -1, -1, OnTag, (IntPtr)null); ///读取全部标签
            List<SignalComm> results = new List<SignalComm>();      //存放标签名 
            foreach (rec_tag obj in _objs)
            {
                //sname  标签名称
                //sdes   标签描述
                //sunit  工程单位
                //fdnval 下限
                //fupval 上限
                SignalComm sig = new SignalComm(obj.sname, obj.sdes);
                results.Add(sig);
            }    
            return results;         //返回那些数据库中没有找到的标签
        }

        //当前值（即时值、快照值）的读写（未完成）
        #region 批量读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValues(string[] tagnames)
        public List<PValue> GetActValues(string[] tagnames)
        {
            List<PValue> pvalues = new List<PValue>();
            return pvalues;
        }
        public List<PValue> GetActValues(int[] tagids)
        {
            List<PValue> pvalues = new List<PValue>();

            return pvalues;
        }
        #endregion

        #region 单个读取数据库标签点的当前值（即时值、快照值）：public List<PValue> GetActValue(string[] tagnames)
        public PValue GetActValue(string tagname)
        {
            List<PValue> pvalues = new List<PValue>();
            return pvalues[0];
        }
        public PValue GetActValue(int tagid)
        {
            List<PValue> pvalues = new List<PValue>();      //结果存储List
            return pvalues[0];
        }
        #endregion

        #region 批量写入数据库标签点的当前值（即时值、快照值）：public int SetActValues(string[] tagnames,List<PValue> pvalues)
        public int SetActValues(string[] tagnames, List<PValue> pvalues)
        {
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

        //历史数据原始值的读写:(2018.05.23完成)
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
            List<PValue> pvalues = new List<PValue>();
            long lts = (startdate.Ticks - 621356256000000000L) / 10000 / 100;
            long lte = (enddate.Ticks - 621356256000000000L) / 10000 / 100;
            long lds = 200;//插值时间间隔
            _objs.Clear();///清空
            String tag = tagname.ToLower();
            //Console.Write("数据库连接状态：" + this._rtdbconnection + "\n");
            //Console.WriteLine("标签名称；"+tag);
            int nret = rdb_valquery(this._rtdbconnection, tag, "", lts, lte, lds, 0, OnReadVals, (IntPtr)null);
            //Console.WriteLine("nret:" + nret + "\n");
            if (nret != def.SE_OK)
            {
                if (nret == def.SE_NOTCONNECT)
                    Console.Write("没有连接实时库，请先使用connect命令连接实时库\n");
                else
                    Console.Write("rdb_valquery errcode = {0}\n", nret);
                return null;
            }
            foreach (rec_val obj in _objs)
            {
                PValue d=convertToPValue(obj);
                pvalues.Add(d);
            }
            for (int i = 0; i < pvalues.Count - 1; i++) {
                pvalues[i].Endtime = pvalues[i + 1].Timestamp;
            }
            return pvalues;
        }
        public List<PValue> GetRawValues(int tagid, DateTime startdate, DateTime enddate)
        {
            //不支持tagid读写            
            return null;
        }
        #endregion
        //历史数据差值的读写:(2018.10.11完成)
        #region 读取历史数据插值，固定间隔：public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int interval)
        /// <summary>
        /// GetIntervalValuesFixCount是实时库读取历史数据接口的对外封装。无论何种实时数据库，读取历史数据功能都是指，读取某一个指定标签（tagname）在一段时间内（startdate、enddate）的历史插值（浮点型）
        /// 而不同的实时数据库，真正的读取方法和返回值得内容略有区别
        /// </summary>
        /// <param name="tagname">标签名</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="interval">差值的时间间隔，单位秒</param>
        /// <returns>历史插值数据，PValue集合</returns>
        public List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int interval)
        {
            return null;
        }
        public List<PValue> GetIntervalValuesFixInterval(int tagid, DateTime startdate, DateTime enddate, int interval)
        {
            return null;
        }
        #endregion

        #region 读取历史数据统计值（不支持）：public List<PValue> GetSummaryValues(string tagname,DateTime startdate,DateTime enddate,string type)
        public PValue GetSummaryValues(string tagname, DateTime startdate, DateTime enddate, string type)
        {
            PValue pvalue = new PValue();
            //不支持统计值功能
            return pvalue;
        }
        public PValue GetSummaryValues(int tagid, DateTime startdate, DateTime enddate, string type)
        {
            PValue pvalue = new PValue();
            return pvalue;
        }
        #endregion

        #region 写入历史数据,单个标签点（不支持）：public int PutArchivedValues(string tagname, string[][] data)
        /// <summary>
        /// 写入单个标签点的历史数据
        /// 这个方法主要用于从csv文件导入历史数据到golden数据库中。从csv读取的数据是字符串型二维数组，因此这里的历史数据入参是字符串型二维数组，而不是List<PValue>
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int PutArchivedValues(string tagname, string[][] data)
        {
            return 0;
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
        /// <summary>
        /// 读取数据回调函数
        /// </summary>
        /// <param name="pobjs"></param>
        /// <param name="nitems"></param>
        /// <param name="pParam"></param>
        /// <returns></returns>
        public bool OnReadVals([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 1),
        System.Runtime.InteropServices.In]  rec_val[] pvals,
        int nitems, System.IntPtr pParam)
        {
            int i;
            for (i = 0; i < nitems; i++)
            {
                _objs.Add(pvals[i]);
            }
            return true;
        }
        /// <summary>
        /// 标签查询回调函数
        /// </summary>
        /// <param name="ptags"> 标签数组</param>
        /// <param name="ntags"> 标签数</param>
        /// <param name="lparam">回调参数</param>
        /// <returns> true 继续，false停止查询</returns>
        public bool OnTag([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 1),
            System.Runtime.InteropServices.In] rec_tag[] ptags,
            int ntags, System.IntPtr lparam)
        {
            int i;
            for (i = 0; i < ntags; i++)
            {
                _objs.Add(ptags[i]);
            }
            return true;
        }
        /// <summary>
        /// 数据转换 将实时库数据转化成PValue
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public PValue convertToPValue(rec_val obj)
        {
            PValue data = new PValue();
            data.Timestamp = new DateTime((obj.time * 100) * 10000 + 621356256000000000L);
            data.Endtime = data.Timestamp;
            data.Status = obj.cqa;
            if (obj.cvt == def.DT_FLOAT32)
                 data.Value=obj.v.f32;
            else if (obj.cvt == def.DT_FLOAT64)
                data.Value = obj.v.f64;
            else if (obj.cvt == def.DT_INT32)
                data.Value = obj.v.i32;
            else if (obj.cvt == def.DT_INT64)
                data.Value = obj.v.i64;
            return data;
        }
        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_create",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_create();

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_destory",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_destory(int h);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_connect",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_connect(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sServerIP,
            ushort wPort,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string lpszUser,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string lpszPass);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_connectasyn",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_connectasyn(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sServerIP,
            ushort wPort,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string lpszUser,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string lpszPass);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_disconnect",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_disconnect(int h);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_isconnected",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public static extern bool rdb_isconnected(int h);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_actoradd",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_actoradd(int h, ref rec_actor pa);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_actordel",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_actordel(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_actorsget",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_actorsget(int h,
            [System.Runtime.InteropServices.Out] rec_actor[] actors, int nrecbufsize, ref int pnacts);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_operatoradd",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_operatoradd(int h, ref rec_operator po);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_operatordel",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_operatordel(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_operatorsget",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_operatorsget(int h,
            [System.Runtime.InteropServices.Out] rec_operator[] opts,
            int nrecbufsize,
            ref int pnopts);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_operatormodifypass",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_operatormodifypass(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string soldpass,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string snewpass);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_taginport",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_taginport(int h, uint dwflag, ref rec_tag ptag, ref int presult);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_tagget",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_tagget(int h, ref rec_tag ptag);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_tagdel",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_tagdel(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_tagquery",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_tagquery(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string snamefilter,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sdesfilter,
            int ndatatype,
            int ntagclass,
            rdbcb_OnReadTags pfun,
            System.IntPtr pParam);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_valputsnapshot",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_valputsnapshot(int h,
            [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] rec_tagval[] vals,
            int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_valgetsnapshot",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_valgetsnapshot(int h,
            [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] rec_tagval[] vals,
            int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_valinsert",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_valinsert(int h, rec_tagval[] vals, int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_valgetsection",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_valgetsection(int h,
            long ltime,
            int nflag,
            [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] rec_tagval[] vals,
            int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_valquery",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_valquery(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sexp,
            long lts,
            long lte,
            long lds,
            int lflag,
            rdbcb_OnReadVals pfun,
            System.IntPtr pParam);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_objputsnapshot",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_objputsnapshot(int h,
            [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] rec_tagobj[] pobjs, int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_objgetsnapshot",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_objgetsnapshot(int h,
            [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] rec_tagobj[] pobjs,
            int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_objinsert",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_objinsert(int h, rec_tagobj[] pobjs, int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_objget",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_objget(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string stagname,
            long lts,
            long lte,
            rdbcb_OnReadObjs pfun,
            System.IntPtr pParam);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_soeput",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_soeput(int h, rec_soe[] psoe, int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_soequery",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_soequery(int h,
            long lts,
            uint autokey,
            long lte,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sexp,
            rdbcb_OnReadSoes pfun,
            System.IntPtr pParam);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_soeupdate",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_soeupdate(int h,
            [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] rec_soe[] psoe,
            int nsize);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_countvalue",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_countvalue(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sexp,
            long lts,
            long lte,
            ref rec_val pMin,
            ref rec_val pMax,
            ref double pavg,
            ref double psum,
            ref int plrecs);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rbd_countstatuschang",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rbd_countstatuschang(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname,
            long lts,
            long lte,
            int nlowval,
            int nhighval,
            ref int pnl2h,
            ref int pnh2l);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_countvaltime",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_countvaltime(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sname,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string sexp,
            long lts,
            long lte,
            ref long pltime,
            ref int plrecs);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_time",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern long rdb_time();

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_localtime2rdbtime",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern long rdb_localtime2rdbtime(int nyear, int nmon, int nday, int nhour, int nmin, int nsec, int nmsec);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_rdbtime2localtime",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)]
        public static extern bool rdb_rdbtime2localtime(long ltime, ref int pnyear, ref int pnmon, ref int pnday, ref int pnhour, ref int pnmin, ref int pnsec, ref int pnmsec);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_initwritetodevice",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_initwritetodevice(int h, ctrl_OnValWriteToDevice funvalw, System.IntPtr paramvalw, ctrl_OnObjWriteToDevice funobjw, System.IntPtr paramobjw);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_regctrltag",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_regctrltag(int h,
            [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string stagname);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_writevaltodevice",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_writevaltodevice(int h, ref rec_tagval pval);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_writeobjtodevice",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_writeobjtodevice(int h, ref rec_tagobj pobj);

        [System.Runtime.InteropServices.DllImportAttribute("rdbapi.dll", EntryPoint = "rdb_setmessagenotify",
            CharSet = System.Runtime.InteropServices.CharSet.Ansi,
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        public static extern int rdb_setmessagenotify(int h, rdbcd_onmessage fun, System.IntPtr param);
    }
    public partial class def
    {
        public const int DT_VOID = 0;
        public const int DT_DIGITAL = 1;
        public const int DT_INT32 = 2;
        public const int DT_FLOAT32 = 3;
        public const int DT_INT64 = 4;
        public const int DT_FLOAT64 = 5;
        public const int DT_STRING = 6;
        public const int DT_OBJECT = 7;

        public const int TGCLS_DEC = 0;
        public const int TGCLS_DEFINE = 1;
        public const int TGCLS_PRESET = 2;
        public const int TGCLS_CURVE = 3;

        public const int TA_COMPNO = 0;
        public const int TA_COMPPER = 1;
        public const int TA_COMPVAL = 2;

        public const int QA_OK = 0;
        public const int QA_SHUTDOWN = 1;
        public const int QA_ERRDATA = 2;
        public const int QA_NOTAG = 3;
        public const int QA_ERRTIME = 4;

        public const int OPT_POW_READ = 1;
        public const int OPT_POW_WRITE = 2;
        public const int OPT_POW_WTAG = 4;
        public const int OPT_POW_MAN = 8;
        public const int OPT_POW_CTRL = 16;

        public const int TAG_MASK_DES = 1;
        public const int TAG_MASK_UNIT = 2;
        public const int TAG_MASK_COMPEXC = 8;
        public const int TAG_MASK_CLASS = 16;
        public const int TAG_MASK_ARCHIVE = 32;
        public const int TAG_MASK_VDIGITS = 64;
        public const int TAG_MASK_STEP = 128;
        public const int TAG_MASK_LIMIT = 256;
        public const int TAG_MASK_BKSYN = 512;
        public const int TAG_MASK_ALARM = 1024;

        public const int TAGIN_NOTMODIFY = 0;
        public const int TAGIN_MODIFY = 1;
        public const int TAGIN_ADD = 2;

        public const int SECTION_INSERT = 0;
        public const int SECTION_AFTER = 1;
        public const int SECTION_BEFORE = -1;

        public const int MAX_OBJ_LEN = 1000;

        public const int SE_ERRHANDLE = -1;

        public const int DBR_SUCCESS = 0;
        public const int SE_OK = 0;
        public const int SE_SUCCESS = 0;

        public const int SE_FAILED = 1;
        public const int SE_TCP_CONNECT = 2;
        public const int SE_TCP_PROXY_CONNECT = 3;
        public const int SE_TCP_PROXY_AUTH = 4;
        public const int SE_TCP_DISCONNECT = 5;
        public const int SE_TCP_IO = 6;
        public const int SE_TCP_CONSVRFAIL = 7;
        public const int SE_TCP_SENDBLOCK = 8;
        public const int SE_TCP_ERRPKG = 10;
        public const int SE_TCPCALLTIMEOUT = 11;
        public const int SE_ERRARGS = 12;
        public const int SE_EXP = 16;
        public const int SE_EXPVAR = 17;
        public const int SE_EXPVAROVER = 18;
        public const int SE_BUSY = 19;
        public const int SE_NOTCONNECT = 31;
        public const int SE_OPT_OK = 0;
        public const int SE_OPT_NOACTOR = 32;
        public const int SE_OPT_NOUSER = 33;
        public const int SE_OPT_PASSERR = 34;
        public const int SE_OPT_NOPOWER = 35;
        public const int SE_OPT_USEREXIST = 36;
        public const int SE_OPT_ACTOREXIST = 37;
        public const int SE_OPT_ACTORUSE = 38;
        public const int SE_OPT_NOLOGIN = 39;
        public const int SE_OPT_NOTACTIVE = 40;
        public const int SE_OPT_OPTFULL = 41;
        public const int SE_OPT_ACTFULL = 42;
        public const int SE_TG_NOTAG = 48;
        public const int SE_TG_TAGFULL = 49;
        public const int SE_TG_NOARCHIVE = 50;
        public const int SE_DA_TIME = 64;
        public const int SE_DA_TYPE = 65;
        public const int SE_DA_QA = 66;
        public const int SE_DA_NODATA = 67;
        public const int SE_CTRLNOROUTE = 97;
        public const int SE_CTRLOUTERR = 98;
        public const int SE_SRV_FRMNOOD = 255;
        public const int SE_SRV_FRMERR = 254;
        public const int SE_SRV_DBVER = 253;
        public const int SE_SRV_BACKUOPSRV = 251;
        public const int SE_SRV_DEMOOVER = 252;

        public const int MSGCODE_CONNECT_SUCCESS = 60001;
        public const int MSGCODE_CONNECT_FAILED = 60002;
        public const int MSGCODE_DISCONNECTED = 60003;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_tag
    {
        public uint uid;
        public byte cdatatype;
        public byte ctagtype;
        public byte ccomptype;
        public byte cres;
        public float fcompval;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string sname;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string sdes;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sunit;
        public byte cdigits;
        public byte cclass;
        public byte cstep;
        public byte carchive;
        public float fdnval;
        public float fupval;
        public float fexcdev;
        public short snexcmin;
        public short snexcmax;
        public short sncompmin;
        public short sncompmax;
        public int alarmtype;
        public float alarm_llv;
        public float alarm_lv;
        public float alarm_hv;
        public float alarm_hhv;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 24)]
        public string sres;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct unionval
    {
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int i32;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public float f32;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public long i64;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public double f64;
        [System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public ulong u64;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct rec_val
    {
        public long time;
        public unionval v;
        public byte cvt;
        public byte cqa;
        public byte cerr;
        public byte cres;
        public uint unres;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_tagval
    {
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string sname;
        public rec_val val;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_obj
    {
        public long time;
        public uint uobjtype;
        public uint ures;
        public byte cvt;
        public byte cqa;
        public byte cerr;
        public byte cres;
        public ushort usres;
        public ushort uslen;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 1000)]
        public string sdata;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_tagobj
    {
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string sname;
        public rec_obj var;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_operator
    {
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sname;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
        public string smd5pass;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sactor;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string sdes;
        public int lbactive;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
        public string sres;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_actor
    {
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
        public string sname;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string sdes;
        public uint dwpower;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 28)]
        public string sres;
    }

    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
    public struct rec_soe
    {
        public long time;
        public uint uautokey;
        public int type;
        public short argtype;
        public ushort arglen;
        public int level;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 80)]
        public string source;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 160)]
        public string sdes;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 240)]
        public string sarg;
        public byte cflag;
        public byte cstatus;
        [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 6)]
        public string res;
    }

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool rdbcb_OnReadTags(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 1),
        System.Runtime.InteropServices.In]  rec_tag[] ptags,
        int nitems,
        System.IntPtr pParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool rdbcb_OnReadVals(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 1),
        System.Runtime.InteropServices.In]  rec_val[] pvals,
        int nitems, System.IntPtr pParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool rdbcb_OnReadObjs(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 1),
        System.Runtime.InteropServices.In]  rec_obj[] pobjs,
        int nitems, System.IntPtr pParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool rdbcb_OnReadSoes(
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 1),
        System.Runtime.InteropServices.In]  rec_soe[] psoes,
        int nitems, System.IntPtr pParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool ctrl_OnValWriteToDevice(ref rec_tagval pval, System.IntPtr pParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool ctrl_OnObjWriteToDevice(ref rec_tagobj pobj, System.IntPtr pParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate void rdbcd_onmessage(int nmsgcode, [System.Runtime.InteropServices.InAttribute()] [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] string smsg, System.IntPtr param);


   
}
