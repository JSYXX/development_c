using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;   //使用dllimport，载入C++接口
using System.Text;                      //使用encoding
using PCCommon;                         //使用pccomon

namespace DBInterface.RTDBInterface
{
    /// <summary>
    /// XIANDBHelper
    /// 西安热工院实时数据库的连接方法。
    /// 
    /// 版本：1.0
    /// 
    /// 特别注意
    ///     1、西安热工院的api为C/C++接口的api。
    ///     2、关于异常的处理。Helper层不设异常处理机制，不使用用try..catch语句。在Helper层出现的异常，统一由调用RTDbHelper的DAO层进行处理。
    ///     ——这么处理的原因是，不同的DAO对异常的处理要求不同。有效率要求的DAO可能出现错误立刻跳过，不去反复尝试。有可用性要求的DAO可能要求要尝试N次连接。
    /// 修改纪录
    ///
    ///     
    ///		2018.5.23 版本：1.0 gaofeng 创建。   
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2016.12.18</date>
    /// </author> 
    /// </summary>
    //实时数据结构
    public struct realdata
    {
        uint vt;                        //数据类型，DWord对应uint，
        long lvlen;                     //可变部分长度，对于双精度浮点为0,8字节
        double dblval;                  //实时值，
        System.Int32 ltime;             //时间戳
        System.Int16 snqa;              //质量
        System.Int16 sneer;             //错误代码
    }
    public class XIANDBHelper : BaseRTDbHelper, IRTDbHelper, IRTDbHelperExpand
    {
        public string _server = "127.0.0.2";
        public string _port = "6327";
        public string _userId = "administrator";
        public string _password = "PlantConnect";
        public string _connectionString;
        
        
        private string _exception="";       //例外信息
        System.UInt32 _rev=0;               //执行结果的错误码        

        //以下为测试************************
        /*
        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int myadd(int a, int b);

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.IntPtr DBP_Open2M(string servername, System.UInt16 port, string userid, string password, bool usergroup);

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.UInt32 DBP_CloseM(System.IntPtr connection);        //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.UInt32 DBP_ConnectM(System.IntPtr connection);      //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.UInt32 DBP_IsConnectM(System.IntPtr connection);    //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.UInt32 DBP_DisConnectM(System.IntPtr connection);   //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.UInt32 DBP_GetHisValM(System.IntPtr connection, string tagname, System.Int32 startdate, System.Int32 enddate, System.Int32 interval, System.Int32 flag, byte[] databytes, int datalength, ref int count);

        [DllImport("dbapim.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern System.UInt32 DBP_GetMultiPointHisValM(System.IntPtr connection, System.Int32 lmode,byte[] tagnames, byte[] readdate,byte[] databytes, int datalength);
        */
        //以上为测试******************************

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.IntPtr DBP_Open2(string servername, System.UInt16 port, string userid, string password, bool usergroup);
        //——返回值，是void*指针，C#用System.IntPtr
        //——servername，是const char *szIP类型，C#中输入char *用string。这里要测试是否能正确运行。可以存在编码不统一的问题。
        //——port，是WORD类型，c#中用ushort类型，在.net标准中，即UInt16
        //——userid，password，是const char *类型。同servername。
        //——usergroup，是bool类型，在C#中，也使用bool型。通常为false。

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_Close(System.IntPtr connection);        //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_Connect(System.IntPtr connection);      //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_IsConnect(System.IntPtr connection);    //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_DisConnect(System.IntPtr connection);   //返回值DWORD是错误代码，是一个正整数，所以使用System.UInt32

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_GetTagAttr(System.IntPtr connection, byte[] tags, int number);
        //——connection，指针，用System.IntPtr
        //——tags，是TTAGITEM[]结构数组，在C#中，使用字节数组byte[]传递
        //——number，查询标签的数量，是int类型，在C#中，使用int类型（不同位数操作系统长度不同）。

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_GetHisVal(System.IntPtr connection, string tagname, System.Int32 startdate, System.Int32 enddate, System.Int32 interval, System.Int32 flag, byte[] databytes, int datalength, ref int count);
        //——connection，指针，用System.IntPtr
        //——tagname，是char*类型， 仅作为输入，在c#对应string。这里要测试是否能正确运行。可能存在编码不统一的问题。
        //——startdate，是long类型，在c#对应System.Int32，代表1970年的秒值
        //——enddate,使用long类型，在c#对应iSystem.Int32，代表1970年的秒值
        //——interval,使用long类型，在c#对应iSystem.Int32，代表间隔秒数
        //——flag,使用long类型，在c#对应iSystem.Int32，代表实时库标志，取样本值时设0，取插值时设置为1
        //——databytes，是TVVAL[],结构数组，在C#中，使用字节数组byte[]传递。
        //——datalength，是int类型，在C#中直接用int（不同位数操作系统长度不同）。是结构数组结构数，进入时只需要给1。
        //——count，是int *类型，即一个整形指针，在c#对应ref int，即一个变量的地址。DBP_GetHisVal将向这个地址指向的位置，写入查到的记录总数。

        [DllImport("dbpapi.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern System.UInt32 DBP_GetMultiPointHisVal(System.IntPtr connection, System.Int32 lmode, byte[] tagnames, byte[] readdate, byte[] databytes, int datalength);
        //——connection，指针，用System.IntPtr
        //——lmode，是取值方式。取给定时刻前值设置为1，取给定时刻后值设定为2，给定时刻插值设置为3。具体草考dbapi.dll的设置。
        //——tagnames,是一个char[][]。实参是char[][]，形参是char(*)[]，即二维数组的二级指针。
        //      ——关于二级指针的理解，首先参考https://blog.csdn.net/wu_nan_nan/article/details/51741030，“C/C++二维数组名和二级指针”。
        //      ——关于和C#的互传，c#可以用一个byte[]来存储要传入的标签名，只不过byte[]中，每80个字节是一个标签名。按照这种方式组织标签名即可。
        //      ——C#将byte[]的变量名称tagnamesBytes传给C++接口，实际上是把byte[]地址指针传给C++接口。而DBP_GetMultiPointHisVal收到这个指针后，按照TagNameArray，即char(*)[80]的格式去对待这个指针。
        //      ——理解“char(*)[80]的格式去对待这个指针”，就是参考上面csdn的文章。
        //——readdate，取值时刻点。是long[]类型，在C#中使用byte[]传值，代表1970年的秒值
        //——databytes，读取结果。
        //——datalength，读取的标签数量，使用int类型，在C#中对应int。
        


        //数据库连接对象_rtdbconnection
        //——特别注意，在这里热工院的连接对象_rtdbconnection是一个静态对象。
        //——因此，在PGIMHelper外部，不管怎么new，实际上所有不同的PGIMHelper实例，都公用一个连接对象。
        //——1、这里首先保证了计算引擎连接PGIM的安全性。
        //      也就是说，从PGIMHelper底层，限定了只能对PGIM Server创建一个公用的连接对象.
        //      避免对PGIM Server创建多个连接，从而造成PGIM Server可用连接数超限。
        //——2、在上述方式下，在PGIMHelper内部，除了该位置，其余任何位置，不能把_pgimDb置为空。或者重新new PgimDatabase()。
        //      在一个使用该数据接口的应用中，无论什么地方使用实时接口，都使用顶部这个唯一类静态私有对象。
        //——3、PGIM是否支持多线程。如果_pgimDb不是静态类。则Pgim可能是能够支持多线程的，这点需要进行测试。
        //      但是，基于上面安全性的原因，（PGIM现场一般作为重要的服务器，避免因连接超限出问题），建议仅适用静态单线程连接的方式。
        IntPtr _rtdbconnection = new IntPtr();

        /// <summary>
        /// 当前实时库数据库类型。该参数必须在具体的实时数据库连接方法中重定义。
        /// </summary>
        public override CurrentRTDbType CurrentRTDbType
        {
            get
            {
                return CurrentRTDbType.XIANDB;
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
                try
                {
                    if (this._rtdbconnection != null || this._rtdbconnection != System.IntPtr.Zero)
                    {
                        if (DBP_IsConnect(this._rtdbconnection) == 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                catch
                {
                    return false;
                }
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
                this._server = _connArray[0].Substring(_connArray[0].IndexOf('=') + 1);
                this._port = _connArray[1].Substring(_connArray[1].IndexOf('=') + 1);
                this._userId = _connArray[2].Substring(_connArray[2].IndexOf('=') + 1);
                this._password = _connArray[3].Substring(_connArray[3].IndexOf('=') + 1);               
                this._connectionString = value;     //这里主要是存储后，对外查询时用
            }
        }

        #region public XIANDBHelper() 构造函数：不带连接字符串的构造函数。工厂的反射调用该方法。
        /// <summary>
        /// 构造函数
        /// </summary>
        public XIANDBHelper()
        {
            //不带连接字符串参数的构造函数，用实时数据库连接方法的内部属性值构建ConnectionString属性
            //外部的Connectstring保持xml格式统一
            this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
            FileName = "XIANDBHelper.txt";   // sql查询句日志文件名称
        }
        #endregion

        #region public XIANDBHelper(string connectionString) 构造函数：带连接字符串的构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public XIANDBHelper(string connectionString)
            : this()
        {
            //带连接字符串参数的构造方法，用传入的参数初始化接字符串属性ConnectionString；当参数为空时，仍用内部默认值
            if (connectionString != null)
            {
                //初始化时，如果外部给进连接字符串，就要用外部给的字符串给ConnectionString赋值。外部字符串是符合统一标准的格式
                //ConnectionString赋值时，就会去调用set，在set中，去设定内部this._connectionString
                //内部this._connectionString是各自数据自己的要求
                this.ConnectionString = connectionString;
            }
            else
            {
                //如果外部connectionString为空
                //则用this._server, this._port, this._userId, this._password去初始化ConnectionString
                //初始化ConnectionString时，会调用set，去设定内部this._connectionString
                this.ConnectionString = string.Format("server={0};port={1};username={2};password={3};", this._server, this._port, this._userId, this._password);
                
            }

        }
        #endregion

        //数据库连接与断开        
        #region public String Logon() 获取数据库连接：不带连接字符串的连接函数
        public String Logon()
        {
            try 
            {             

                //不带连接字符串的连接函数，采用实例化时对象属性this.ConnectionString决定的_server、 _userId、 _password进行连接 
                string serverip=this._server;
                System.UInt16 serverport=System.UInt16.Parse(this._port);
                string username=this._userId;
                string password=this._password;
                //private static extern System.UInt32 Open(string servername, UInt16 port, string userid, string password, Boolean usergroup);
                //myhandl = DBP_Open2("127.0.0.1",12084,"admin","admin",FALSE); // 默认端口为12084，群组必须为FALSE
                this._rtdbconnection = DBP_Open2(serverip, serverport, username, password, false);
                if (this._rtdbconnection != null || this._rtdbconnection != IntPtr.Zero)
                {
                    _rev = DBP_Connect(this._rtdbconnection);
                    if (_rev == 0)
                        return "logon";
                    else
                        this._rtdbconnection = IntPtr.Zero;
                        return "logoff";
                        
                }
                else
                    this._rtdbconnection = IntPtr.Zero;
                    return "logoff";
            }
            catch (Exception ex)
            {
                this._rtdbconnection = IntPtr.Zero;
                _exception = ex.ToString();
                throw new Exception(ex.ToString());     //将数据库的异常继续向外传递。                  
            }
        }
        #endregion

        #region  public String Logon(string connectionString) 获取数据库连接：带连接字符串的连接函数
        public String Logon(string connectionString)
        {
           try
           { 
                //带连接字符串的连接函数，采用传入参数connectionString决定的_server、 _userId、 _password进行连接
                string[] _connArray = connectionString.Split(';');
                this._server = _connArray[0].Substring(_connArray[0].IndexOf('=') + 1);
                this._port = _connArray[1].Substring(_connArray[1].IndexOf('=') + 1);
                this._userId = _connArray[2].Substring(_connArray[2].IndexOf('=') + 1);
                this._password = _connArray[3].Substring(_connArray[3].IndexOf('=') + 1);
                
                string serverip = this._server;
                System.UInt16 serverport = System.UInt16.Parse(this._port);
                string username = this._userId;
                string password = this._password;

                this._rtdbconnection = DBP_Open2(serverip, serverport, username, password, false);
                if (this._rtdbconnection != null || this._rtdbconnection != IntPtr.Zero)
                {
                    _rev = DBP_Connect(this._rtdbconnection);
                    if (_rev == 0)
                        return "logon";
                    else
                        this._rtdbconnection = IntPtr.Zero;
                        return "logoff";
                }
                else
                    this._rtdbconnection = IntPtr.Zero;
                    return "logoff";
           }
           catch (Exception ex)
           {
               this._rtdbconnection = IntPtr.Zero;
               _exception = ex.ToString();
               throw new Exception(ex.ToString());     //将数据库的异常继续向外传递。                  
           }

        }
        #endregion

        #region public void Logoff() 关闭数据库连接
        public void Logoff()
        {
            try
            {
                //在golden数据库有连接池的情况下，
                //Close方法，即不销毁对象，也不注销内存，仅仅是把_rtdbconnection对象丢会连接池中
                //_rtdbconnection对象销毁，只有在连接池到达等待时间waitMaxTime时，才会发生，这也是连接池存在的意义。
                //close和dipose的区别，在于close的connection，连接字符串还在，还可以重连，而dispose的conncection被null，不能在进行重连。
                if (this._rtdbconnection != null || this._rtdbconnection != IntPtr.Zero)
                {
                    _rev = DBP_DisConnect(this._rtdbconnection);
                    if (_rev == 0)
                    {
                        //正常断开连接后，关闭连接
                        _rev = DBP_Close(this._rtdbconnection);  
                        //无论是否正常断开，均将连接置为空。
                        this._rtdbconnection = IntPtr.Zero;   
                    }
                    else
                    {
                        this._rtdbconnection = IntPtr.Zero;
                        _exception = "尝试断开连接时出错！";
                        throw new Exception(_exception);     //将数据库的异常继续向外传递。
                    }
                      
                }
                //如果断开连接时，连接对象已经为空，则什么也不做
                /*
                else
                {
                    this._rtdbconnection = IntPtr.Zero;
                    _exception = "尝试关闭连接时，连接对象为空";
                    throw new Exception(_exception);     //将数据库的异常继续向外传递。
                }*/
            }
            catch (Exception ex)
           {
               this._rtdbconnection = IntPtr.Zero;
               _exception = ex.ToString();
               throw new Exception(ex.ToString());     //将数据库的异常继续向外传递。                  
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

        //检查标签是否存在(2018.05.23完成)
        #region 检查标签是否存在    public List<string> CheckTags(string[] tagnames)
        public List<string> CheckTags(string[] tagnames)
        {
            try
            {
                int sizeTTAGITEM = 288; //根据dbapi.h中定义的typedef struct t_tagitem{}TTAGITEM,*LPTAGITEM;，该类型长度为288字节
                int numberTTAGITEM = 1; //定义需要读取的tagitem个数
                byte[] tagsBytes = new byte[sizeTTAGITEM * numberTTAGITEM]; //为读取1个标签信息准备字节型存储空间。

                List<string> results = new List<string>();
                if (this._rtdbconnection == null || this._rtdbconnection == IntPtr.Zero)
                {
                    this._rtdbconnection = IntPtr.Zero;
                    _exception = "检查标签时，连接对象丢失";
                    throw new Exception(_exception);     //将数据库的异常继续向外传递。
                    return null;
                }
                foreach (string tagname in tagnames)
                {
                    tagsBytes = new byte[sizeTTAGITEM * numberTTAGITEM];                            //每次读取前必须，必须先清空。否则超过长度的字符不会被覆盖
                    byte[] tagnameByte = Encoding.ASCII.GetBytes(tagname);
                    int tagnameNumber = tagnameByte.Length < 80 ? tagnameByte.Length : 80;          //长度不足80，下面拷贝的时候，按照实际长度来，否则出错
                    Array.Copy(tagnameByte, 0, tagsBytes, 0, tagnameNumber);                        //t_tagitem结构变量，前80个字节用于存放标签名称。

                    try
                    {

                        _rev = DBP_GetTagAttr(this._rtdbconnection, tagsBytes, numberTTAGITEM);
                        byte[] tagIdByte = new byte[4];
                        Array.Copy(tagsBytes, 271, tagIdByte, 0, 4);
                        System.UInt32 tagId = BitConverter.ToUInt32(tagIdByte, 0);
                        if (tagId == 0)
                        {   //错误码_rev不为0，就没有找到标签
                            results.Add(tagname);       //记录没有找到的标签
                        }
                    }
                    catch (Exception ex)                //后面这个异常的类型应该改为数据库的异常类型
                    {
                        results.Add(tagname);           //记录没有找到的标签
                    }
                }
                return results;                                     //返回没有找到的标签
            }
            catch (Exception ex)
            {
                //将pi的异常记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "检测标签时出错！详细错误信息：" + ex.ToString();
                throw ex;     //将PI的异常继续向外传递。                
            }  

        }
        #endregion

        //获取标签列表(不支持)
        #region 获取标签列表 public  List<String> GetTagList()
        public List<SignalComm> GetTagList(string tagname, string tablename = "")
        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <returns>在数据库所有符合tagname搜索条件的标签列表（在所有表中查询）</returns>
        {
            List<SignalComm> results = new List<SignalComm>();      //存放标签名           
            return results;         //返回那些数据库中没有找到的标签
        }
        #endregion


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

            //为了在使用长连接时保险起见，这里仅检查是否连接。
            //如果连接为空，应该直接返回null，并报出正确的报警，一遍DAO层记录正确log。
            //——*******特别注意，这里还有连个问题有待测试。断网线后，直接回复网线，链接能否回复正常读取数据，还是必须重启。（目前没有机会测试）
            //——*******要根据上面的的具体情况，确定这里是否需要在判断连接断掉后，是否自动重连。
            //——*******确定自动重连的次数，超过后的处理方式。
            if (this._rtdbconnection == null || !this.isLogOn)
            {
                this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。数据库连接失效！请检查SIS数据库服务及网络。排除故障后对相应时间的数据进行重算！", _rev.ToString());
                throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。                
            }

            int sizeTVVAL;
            int numberTVVAL;
            byte[] dataBytes;
            int count;
            System.Int32 startsec;
            System.Int32 endsec;
            System.Int32 intervalsecond;
            System.Int32 flag;

            try
            {
                //System.UInt32 GetHisVal(System.IntPtr connection, string tagname, long startdate,long enddate,long interval,long flag,byte[] datebytes,int max,int count);
                //——connection:连接对象，句柄
                //——tagname：标签名称
                //——startdate：起始时间，从1970年1月1日开始的秒。参考GetDbProxy中，pTime_e采用如下赋值，pTime_e = (long)::time(NULL);time(NULL)返回的是从1970年1月1日开始的秒
                //——enddate：截止时间。
                //——interval：插值时间间隔，单位秒。取0时，为取样本值。
                //——flag：标志，实时库特有标志
                //——datebytes：接收数据的字符型数组
                //——max：结构数组中的结构个数

                //准备读取值的存储空间，参考api手册中的类型t_vval
                sizeTVVAL = 24; //根据dbapi.h中定义的typedef struct t_vval{}TVVAL,*LPVVA;，该类型长度为24字节
                numberTVVAL = 100000; //定义最多可以读取的数据个数，不能超过100000条数据
                dataBytes = new byte[sizeTVVAL * numberTVVAL];   //为1个数据准备byte[]类型的存储空间。
                count = 0;   //读取的记录总数。下面调用接口时，将该变量count的地址，ref count传给接口DBP_GetHisVal，接口会将查询到的记录数量写入该地址指向的变量。   
                //tagname：通过查询帖子，C++接口的输入型char*,在c#中直接可以用string类型。
                //特别注意数据库存储的是格林威治时间，接口要求的起始时间和截止时间都是要求距格林威治1970年1月1日开始的秒。
                //由于C# DateTime本身不含时区属性，因此DateTime表示什么时区，取决于表达式。
                //startdate.Subtract(DateTime.Parse("1970-1-1"))相当于一个本地时间距格林威治"1970-1-1"的距离，显然不是接口所要求的。
                //startdate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1"))才是当前时间对应的格林威治时间到格林威治"1970-1-1"的距离。接口要求的就是这个值           
                startsec = (System.Int32)(startdate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                endsec = (System.Int32)(enddate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                intervalsecond = 0;       //插值间隔，取0时表示去样本值
                flag = 0;

                _rev = DBP_GetHisVal(this._rtdbconnection, tagname, startsec, endsec, intervalsecond, flag, dataBytes, numberTVVAL, ref count);

                if (_rev != 0) 
                {
                    if (_rev==161)
                        this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。标签名称{0}不存在！", tagname);
                    else
                        this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。错误码{0}。请查询数据库错误代码手册！", _rev.ToString());
                    throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。                    
                }  ;
                //if (count == 0) return null;
                for (int i = 0; i < count; i++)
                {
                    byte[] pvdata = new byte[sizeTVVAL];
                    Array.Copy(dataBytes, i * sizeTVVAL, pvdata, 0, sizeTVVAL);     //一次将每一个值读入存储器
                    PValue pv = Byte2PValue(pvdata);                                //将读取出的值转换为一个pvalue，起始时刻和截止时刻均相同
                    if (i != 0) pvalues[pvalues.Count - 1].Endtime = pv.Timestamp;  //如果不是第一个数据，则修改上一个数据的结束时刻为当前值得开始时刻。最后一个pvalue则不修改。其起始时刻和截止时刻均为enddate
                    pvalues.Add(pv);
                }
                //排序，放到DAO层排序
                
                #region 用取断面值得方法取起止点时刻的值。需要测试断面取值与插值取值，得到的结果是否一致。
                /*
                //判断起始点
                if (pvalues==null || pvalues.Count==0 || pvalues[0].Timestamp != startdate)
                {                   
                    //1、用断面方法取起始时刻点
                    //准备标签名
                    int sizeTagname = 80;     //dbapi.dll中一个标签占80个char，char[80]
                    int numberTagname = 1;    //这里只查一个标签
                    byte[] tagnamesBytes = new byte[sizeTagname * numberTagname];
                    
                    byte[] tagnameByte = new byte[80];                                          //一个标签名，长度80
                    tagnameByte = Encoding.ASCII.GetBytes(tagname);                             //将tagname转换为char[]字符赋给tagnameByte,这里有可能超过80
                    int tagnameNumber = tagnameByte.Length < 80 ? tagnameByte.Length : 80;      //长度不足80，下面拷贝的时候，按照实际长度来，否则出错
                    Array.Copy(tagnameByte, 0, tagnamesBytes, 0, tagnameNumber);                //从tagnameByte的0处开始拷贝，放到tagnamesBytes从0处开始，拷贝80个字节。这样每次都仅取tagnameByte前80个字符
                    //准备其他参数
                    System.Int32 lmode = 1;                                                     //读取模式，dbapi.h中定义，RHV_Before=1取前值，RHV_After=2取后值，RHV_Interp=3取插值。
                    //准备时间参数，
                    int sizeDate = 4;
                    int nunmberDate = 1;
                    System.Int32 readdate = (System.Int32)(startdate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);        //读取时间

                    byte[] readdates = new byte[sizeDate * nunmberDate];
                    readdates = BitConverter.GetBytes(readdate);                //将单个时间放入时间数组的第一个位置
                    //存放结果
                    dataBytes = new byte[sizeTVVAL * numberTagname];            //存放取数结果，每次读取前必须初始化，清空
                    //用断面法读取
                    _rev = DBP_GetMultiPointHisVal(this._rtdbconnection, lmode, tagnamesBytes, readdates, dataBytes, numberTagname);
                    if (_rev != 0)
                    {
                        if (_rev == 161)
                            this._exception = String.Format("XIANDB错误。标签名称{0}不存在！", tagname);
                        else
                            this._exception = String.Format("XIANDB接口DBP_GetMultiPointHisVal()其他未知错误。错误码{0}。请查询数据库错误代码手册！", _rev.ToString());
                        throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。
                        return null;
                    }
                    //读读取结果进行转换
                    byte[] pvdata = new byte[sizeTVVAL];
                    Array.Copy(dataBytes, 0 * sizeTVVAL, pvdata, 0, sizeTVVAL); //一次将每一个值读入存储器
                    PValue pv = Byte2PValue(pvdata);
                    //添加读取结果                    
                    if(pv!=null) pvalues.Insert(0,pv);
                    //修改时间
                    if (pvalues != null && pvalues.Count > 1)
                        pvalues[0].Endtime = pvalues[1].Timestamp;      //区间可以读取出数据时，
                    else if (pvalues != null && pvalues.Count == 1)
                        pvalues[0].Endtime = enddate;                   //如果时间区间内没有值，仅起始时刻有值
                    else
                        return null;    //如果pvalues这里仍然没有有效数据，也没有在起始时间得到插值数据，就直接退出返回null

                }
                //判断截止点，如果最后一个点的Timestamp等于读取数据的截止时刻，那无需添加截止时刻点。当前的点恰好
                if (pvalues[pvalues.Count - 1].Timestamp != enddate)
                {
                    //1、修改最后一个值得截止时间到取数据截止时刻
                    pvalues[pvalues.Count - 1].Endtime = enddate;
                    //2、用断面方法取截止时刻点
                    //准备标签名
                    int sizeTagname = 80;     //dbapi.dll中一个标签占80个char，char[80]
                    int numberTagname = 3;    //这里只查一个标签
                    byte[] tagnamesBytes = new byte[sizeTagname * numberTagname];
                    
                    byte[] tagnameByte = new byte[80];                                          //一个标签名，长度80
                    tagnameByte = Encoding.ASCII.GetBytes(tagname);                             //将tagname转换为char[]字符赋给tagnameByte,这里有可能超过80
                    int tagnameNumber = tagnameByte.Length < 80 ? tagnameByte.Length : 80;      //长度不足80，下面拷贝的时候，按照实际长度来，否则出错
                    Array.Copy(tagnameByte, 0, tagnamesBytes, 0, tagnameNumber);                //从tagnameByte的0处开始拷贝，放到tagnamesBytes从0处开始，拷贝80个字节。这样每次都仅取tagnameByte前80个字符
                    //准备其他参数
                    System.Int32 lmode = 1;                                                     
                    //准备时间参数，
                    int sizeDate = 4;
                    int nunmberDate = 1;
                    System.Int32 readdate = (System.Int32)(enddate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);        //读取时间
                    
                    byte[] readdates=new byte[sizeDate*nunmberDate];
                    readdates = BitConverter.GetBytes(readdate);                //将单个时间放入时间数组的第一个位置
                    //存放结果
                    dataBytes = new byte[sizeTVVAL * numberTagname];            //存放取数结果，每次读取前必须初始化，清空
                    //用断面法读取
                    _rev = DBP_GetMultiPointHisVal(this._rtdbconnection, lmode, tagnamesBytes, readdates, dataBytes, numberTagname);
                    if (_rev != 0)
                    {   
                        //如果结尾值，不存在，则返回报错
                        if (_rev == 161)
                            this._exception = String.Format("XIANDB接口GetRawValues()错误。标签名称{0}不存在！", tagname);
                        else
                            this._exception = String.Format("XIANDB接口DBP_GetMultiPointHisVal()其他未知错误。错误码{0}。请查询数据库错误代码手册！", _rev.ToString());
                        throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。
                        return null;
                    }

                    //读读取结果进行转换
                    byte[] pvdata = new byte[sizeTVVAL];
                    Array.Copy(dataBytes, 0 * sizeTVVAL, pvdata, 0, sizeTVVAL); //一次将每一个值读入存储器
                    PValue pv = Byte2PValue(pvdata);
                   //添加读取结果
                    //如果时间段内有值且起始时间有值，或者时间段内没有值但起始时刻有值，才会执行到这里，
                    //那么这种情况下，意味着pvalues肯定已经有起始时刻的值。
                    //那么这种情况下，如果没有取到截止时刻值，则用最后一个值替代。
                    if (pv != null)
                        pvalues.Add(pv);
                    else
                        
                        pvalues.Add(new PValue(pvalues[pvalues.Count - 1].Value, enddate, enddate, 0));
                
                }
                */
                #endregion

                #region 用取插值的方法取起止时刻点的值。优点：只要一次就能取到两个时刻点的值。待定的地方，需要测试，插值读取方式在起止时刻点是否也插值。

                //判断起始点
                if (pvalues == null || pvalues.Count == 0 || pvalues[0].Timestamp != startdate || pvalues[pvalues.Count - 1].Timestamp != enddate)
                {
                    sizeTVVAL = 24; //根据dbapi.h中定义的typedef struct t_vval{}TVVAL,*LPVVA;，该类型长度为24字节
                    numberTVVAL = 100000; //定义最多可以读取的数据个数，不能超过100000条数据
                    dataBytes = new byte[sizeTVVAL * numberTVVAL];   //为1个数据准备byte[]类型的存储空间。
                    count = 0;   //读取的记录总数。下面调用接口时，将该变量count的地址，ref count传给接口DBP_GetHisVal，接口会将查询到的记录数量写入该地址指向的变量。
                    startsec = (System.Int32)(startdate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                    endsec = (System.Int32)(enddate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                    intervalsecond = (int)enddate.Subtract(startdate).TotalSeconds;       //插值间隔，取0时表示去样本值。这里让插值秒数等于起止时间总秒数，即可返回起始时刻插值
                    flag = 1;                                                             //flag=0读取样本值。flag=1读取插值

                    _rev = DBP_GetHisVal(this._rtdbconnection, tagname, startsec, endsec, intervalsecond, flag, dataBytes, numberTVVAL, ref count);

                    if (_rev != 0)
                    {
                        if (_rev == 161)
                            this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。标签名称{0}不存在！", tagname);
                        else
                            this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。错误码{0}。请查询数据库错误代码手册！", _rev.ToString());
                        throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。                    
                    };

                    if (count != 0)
                    {
                        //添加起始时刻点
                        if (pvalues == null || pvalues.Count == 0 || pvalues[0].Timestamp != startdate)
                        { 
                            //读读取结果进行转换
                            byte[] pvdata = new byte[sizeTVVAL];
                            Array.Copy(dataBytes, 0 * sizeTVVAL, pvdata, 0, sizeTVVAL); //一次将每一个值读入存储器。按上述插值读法，仅返回起始时刻和截止时刻两个值。第0个值起始时刻
                            PValue pv = Byte2PValue(pvdata);
                            //添加读取结果                    
                            if(pv!=null) pvalues.Insert(0,pv);
                            //添加读取结果后，如果长度大于1，则修改起始时刻值的截止时间                            
                            if (pvalues != null && pvalues.Count > 1)
                            pvalues[0].Endtime = pvalues[1].Timestamp;      //区间可以读取出数据时，
                        }
                        //添加截止时刻
                        if (pvalues[pvalues.Count - 1].Timestamp != enddate)
                        {
                            //读读取结果进行转换
                            byte[] pvdata = new byte[sizeTVVAL];
                            Array.Copy(dataBytes, 1 * sizeTVVAL, pvdata, 0, sizeTVVAL); //一次将每一个值读入存储器。按上述插值读法，仅返回起始时刻和截止时刻两个值。第1个值截止时刻
                            PValue pv = Byte2PValue(pvdata);
                            
                            //修改最后一个值的截止时刻，并添加截止时刻值
                            pvalues[pvalues.Count - 1].Endtime = enddate;
                            //添加截止值
                            pvalues.Add(pv);
                        }
                    }

                }
                
                #endregion

                return pvalues;
                
            }
            catch (Exception ex)
            {
                //将pi的异常记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "XIANDBHelper的GetRawValues()发生其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！详细错误信息：" + ex.ToString();
                throw ex;     //将XIANDB的异常继续向外传递。               
            }          
           
        }
        public List<PValue> GetRawValues(int tagid, DateTime startdate, DateTime enddate)
        {
            //西安热工院数据库不支持tagid读写            
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
            //对外映射的GetIntervalValuesFixCount，对内是获得浮点数据类型的原始值。实际上也有其他数据类型的原始值。
            //未来如果需要获得不同数据类型的历史值，可以在GetIntervalValuesFixCount接口上添加dbfieldtype字段来指定数据类型，然后在该程序内部，根据数据类型做分支，去调用不同数据类型的接口。
            //目前默认仅实现的浮点型的历史数据的读取
            
            List<PValue> pvalues = new List<PValue>();
            //为了在使用长连接时保险起见，这里仅检查是否连接。
            //如果连接为空，应该直接返回null，并报出正确的报警，以便DAO层记录正确log。
            //——*******特别注意，这里还有连个问题有待测试。断网线后，直接回复网线，链接能否回复正常读取数据，还是必须重启。（目前没有机会测试）
            //——*******要根据上面的的具体情况，确定这里是否需要在判断连接断掉后，是否自动重连。
            //——*******确定自动重连的次数，超过后的处理方式。
            if (this._rtdbconnection == null || !this.isLogOn)
            {
                this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。数据库连接失效！请检查SIS数据库服务及网络。排除故障后对相应时间的数据进行重算！", _rev.ToString());
                throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。                
            }
             //准备读取值的存储空间，参考api手册中的类型t_vval
            int sizeTVVAL = 24; //根据dbapi.h中定义的typedef struct t_vval{}TVVAL,*LPVVA;，该类型长度为24字节
            int numberTVVAL = 100000; //定义最多可以读取的数据个数，不能超过100000条数据。
            byte[] dataBytes = new byte[sizeTVVAL * numberTVVAL];   //为1个数据准备byte[]类型的存储空间。
            int count = 0;   //读取的记录总数。下面调用接口时，将该变量count的地址，ref count传给接口DBP_GetHisVal，接口会将查询到的记录数量写入该地址指向的变量。    
            try
            {
                //System.UInt32 GetHisVal(System.IntPtr connection, string tagname, long startdate,long enddate,long interval,long flag,byte[] datebytes,int max,int count);
                //——connection:连接对象，句柄
                //——tagname：标签名称
                //——startdate：起始时间，从1970年1月1日开始的秒。参考GetDbProxy中，pTime_e采用如下赋值，pTime_e = (long)::time(NULL);time(NULL)返回的是从1970年1月1日开始的秒
                //——enddate：截止时间。
                //——interval：插值时间间隔，单位秒。取0时，为取样本值。
                //——flag：标志，实时库特有标志。flag=0取样本值。flag=1取插值
                //——datebytes：接收数据的字符型数组
                //——max：结构数组中的结构个数

                //tagname：通过查询帖子，C++接口的输入型char*,在c#中直接可以用string类型。
                //特别注意数据库存储的是格林威治时间，接口要求的起始时间和截止时间都是要求距格林威治1970年1月1日开始的秒。
                //由于C# DateTime本身不含时区属性，因此DateTime表示什么时区，取决于表达式。
                //startdate.Subtract(DateTime.Parse("1970-1-1"))相当于一个本地时间距格林威治"1970-1-1"的距离，显然不是接口所要求的。
                //startdate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1"))才是当前时间对应的格林威治时间到格林威治"1970-1-1"的距离。接口要求的就是这个值           
                System.Int32 startsec = (System.Int32)(startdate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                System.Int32 endsec = (System.Int32)(enddate.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalSeconds);
                System.Int32 intervalsecond = interval;       //插值间隔，取0时表示去样本值。不为0时按照实际间隔插值。要测试结尾的情况，看能否返回截止时刻插值。
                System.Int32 flag = 1;

                _rev = DBP_GetHisVal(this._rtdbconnection, tagname, startsec, endsec, intervalsecond, flag, dataBytes, numberTVVAL, ref count);

                if (_rev != 0)
                {
                    if (_rev == 161)
                        this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。标签名称{0}不存在！", tagname);
                    else
                        this._exception = String.Format("XIANDB接口DBP_GetHisVal()接口错误。错误码{0}。请查询数据库错误代码手册！", _rev.ToString());
                    throw new Exception(this._exception);     //将XIANDB的异常继续向外传递。                    
                };
                //if (count == 0) return null;
                for (int i = 0; i < count; i++)
                {
                    byte[] pvdata = new byte[sizeTVVAL];
                    Array.Copy(dataBytes, i * sizeTVVAL, pvdata, 0, sizeTVVAL);     //一次将每一个值读入存储器
                    PValue pv = Byte2PValue(pvdata);                                //将读取出的值转换为一个pvalue，起始时刻和截止时刻均相同
                    if (i != 0) pvalues[pvalues.Count - 1].Endtime = pv.Timestamp;  //如果不是第一个数据，则修改上一个数据的结束时刻为当前值得开始时刻。最后一个pvalue则不修改。其起始时刻和截止时刻均为enddate
                    pvalues.Add(pv);
                }
                //排序，放到DAO层排序
                
                //待检测，插值取法，在起止时刻是否直接返回插值，如果直接返回插值，则将此测试结果写在此处。
                return pvalues;

            }
            catch (Exception ex)
            {
                //将pi的异常记录在_exception中，并以PgimDataException的名称继续抛出异常
                this._exception = "XIANDB其他未知错误。通常是服务器故障、标签中对应的服务器名称不正确导致！详细错误信息：" + ex.ToString();
                throw ex;     //将XIANDB的异常继续向外传递。               
            } 
        }
        public List<PValue> GetIntervalValuesFixInterval(int tagid, DateTime startdate, DateTime enddate, int interval)
        {
            //西安热工院数据库不支持tagid读写
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

        #region 辅助函数：数据格式转换
        private PValue Byte2PValue(byte[] dataByte)
        {
            //为读取历史数据原始值准备的转换函数      
            //dataBytes为C++接口返回的实时数据，以byte[]形式接受。
            //count为C++返回的数量。
            //struSize为一个基本实时数据结构单位的长度。根据dbapi.h定义得知其为24
            //——本程序根据t_vval定义，直接将其读成PValue结构
            /*
            typedef struct  t_vval		//变体结构
            {
	            DWORD	vt;		//数据类型，4个字节，0~3。3是float32类型。5是float64类型。需要根据这个字节对union进行类型转换
	            long	lvlen;	//可变部分长度,对pstr,pblob,psoe有效,其他已知长度的基本数据类型填0，4个字节，4~7
	            				//字符串长度不包含结尾的0字符
	            union 	
	            {
	            	long  	lval;		//存储didital、int32
	            	float  	fval;		//存储float32
	            	__int64 llval;		//存储int64
	            	double 	dblval;		//存储float64 ，8个字节，8~15
	            	char*	pstr;		//字符串首指针
	            	void*	pblob;		//存储pblob的存储区首地址
	            };
	            long  ltime;			//时标    ，4个字节，16~19
	            short snqa;				//质量    ，2个字节，20~21
	            short snerr;			//错误代码，2个字节，22~23
            }TVVAL,*LPVVAL; //sizeof() = 24 
            */

            //values为空时，上面直接退出，返回空list。
            //values不为空时，至少会有两个值。PI会在起始时间和截止时间插值
            
            PValue pvalue = new PValue();
            if (BitConverter.ToInt32(dataByte, 16) != 0)                    //如果时标不为0，则说明有值
            {
                int datatype = BitConverter.ToInt32(dataByte, 0);           //取dataByte第0到第3个字节，是数据类型。这里主要是分辨数据是float32还是float64。两者不一样。
                switch(datatype)
                { 
                    //根据不同的数据类型对取得的数据进行转换
                    case 3: //dataByte前4位表示类型，3是float32（通过实际读单精度的点测试得到的3），单精度浮点
                        pvalue.Value = BitConverter.ToSingle(dataByte, 8);  //从dataByte取数据，从第8位开始取4个字节
                        break;
                    case 5: //dataByte前4位表示类型，5是float64（通过实际读单精度的点测试得到的5），双精度浮点
                        pvalue.Value = BitConverter.ToDouble(dataByte, 8);  //从dataByte取数据，从第8位开始取8个字节
                        break;
                    //如果将来还要使用整型、bool型，依然需要通过读取实际的对应类型的变量来测试。
                    default:
                        pvalue.Value = float.MinValue;      //当转换结果时float最小值时，说明类型未知，或未做处理
                        break;
                }
                //特别注意时区问题：
                //XIAN热工院数据库，返回的数据是据格林威治时间1970-01-01 00:00:00的秒数
                //C# DataTime本身内部不含有时区属性。
                //DateTime.Parse("1970-01-01")即可以表示格林威治时间，也可以表示本地时间。主要看在使用环境中怎么定义。
                //在下面转换中，BitConverter.ToInt32(dataByte, 16)取出的是西安热工院的时间，它表示距离格林威治时间1970-01-01 00:00:00的秒数
                //因此当DateTime.Parse("1970-01-01").AddSeconds，就表示从格林威治时间1970-01-01 00:00:00开始经过n秒后的一个格林威治时间
                //需要转换成服务器时间，才能正常显示。
                pvalue.Timestamp = DateTime.Parse("1970-01-01").AddSeconds(BitConverter.ToInt32(dataByte, 16)).ToLocalTime();
                pvalue.Endtime = pvalue.Timestamp;    //datetime为值类型

                System.Int16 status = BitConverter.ToInt16(dataByte, 20);
                if (status == 0)
                    pvalue.Status = 0;
                else
                    pvalue.Status = 1;
            }
            else
                pvalue = null;

            return pvalue;
        }
        #endregion
    }
}
