//使用实时数据库接口。如果实时数据库有连接池技术，这里相当于使用连接池。如果实时数据库没有连接池技术。这里相当于使用短连接。
//PGIM实时数据库的长连接类。长远看，通过调试，看能否将PGIM和PI的长连接类合并。
//PI实时数据库的长连接类。长远看，通过调试，看能否将PGIM和PI的长连接类合并。
//XIANDB实时数据库的长连接类。
using Config;   //使用log
using PCCommon;                                 //使用PValue
using System;
using System.Collections.Generic;
using System.Threading;

namespace PSLCalcu
{
    /// <summary>
    /// 实时数据DAO
    /// 1、关于质量位的处理原则
    ///——特别注意，计算引擎读接口的本质，就是要读有效数据。如果数据无效，应该在DAO层处理过滤掉。而不是在交给计算引擎处理。
    ///——所以，如果底层原始数据无效，（尤其是实时数据和OPC数据），对应时间段应该是无数据
    /// </summary>
    public class RTDBDAO
    {
        //使用实时数据库接口读取实时数据库数据的DAO层
        //实时数据库接口的错误，主要在DAO层处理
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(RTDBDAO));         //DAO层log
        
        #region 公有变量
        public static bool ErrorFlag = false;       //全局ErrorFlag
        public static string ErrorInfo = "";        //全局ErrorInfo，用于保存发生错误时返回发生错误消息

        public static bool WarningFlag = false;       //全局WarningFlag
        public static string WarningInfo = "";        //全局WarningInfo，用于保存发生错误时返回发生错误消息
      
        public static string rtdbConnStr = "";      //实时数据库DAO层专用，用于多实时数据库实现
        public static string testStatus = "";       //实时数据库DAO层专用，用于多实时数据库实现
        #endregion
        //实时数据库连接测试
        public static bool connectTest()
        {
            ErrorFlag = false;
            ErrorInfo = "";
            try
            {
                rtdbConnStr = (APPConfig.rtdb_connString);
              

                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用长连接类
                {
                    DBInterface.RTDBInterface.LongPI.RTDbHelper rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper();       //如果出现“类型初始值设定项引发异常”的错误，请检查RTDbType、RTDbConnection两个静态参数的设置是否正确。尤其是改为xml配置时，如果配置不正确，则或报出该错误。
                    testStatus = rtdbhelperpi.ConnTest(); 

                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用长连接类
                {
                    DBInterface.RTDBInterface.LongPGIM.RTDbHelper rtdbhelperpgim = new DBInterface.RTDBInterface.LongPGIM.RTDbHelper();
                    testStatus = rtdbhelperpgim.ConnTest(); 
                }
                else if (APPConfig.rtdb_Type == "XIANDB") //XIANDB数据库的情况下，使用长连接类
                {
                    DBInterface.RTDBInterface.LongXIANDB.RTDbHelper rtdbhelperxiandb = new DBInterface.RTDBInterface.LongXIANDB.RTDbHelper();
                    testStatus = rtdbhelperxiandb.ConnTest();
                }
                else if (APPConfig.rtdb_Type == "TOM") //XIANDB数据库的情况下，使用长连接类
                {
                    DBInterface.RTDBInterface.LongTOM.RTDbHelper rtdbhelpertom = new DBInterface.RTDBInterface.LongTOM.RTDbHelper();
                    testStatus = rtdbhelpertom.ConnTest();
                }
                else
                {
                    DBInterface.RTDBInterface.RTDbHelper rtdbhelper = new DBInterface.RTDBInterface.RTDbHelper();
                    testStatus = rtdbhelper.ConnTest(); 
                }               
                               
                return true;
            }
            catch (Exception ex)
            {
                ErrorInfo = ex.ToString();

                string messageStr;
                messageStr = String.Format("DAO层connectTest()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("详细错误信息:"+ex.ToString());
                logHelper.Error(messageStr);
                return false;               
            }
        }
        
        //实时数据库logoff        
        public static void disconnect()
        {
            ErrorFlag = false;
            ErrorInfo = "";
            //——实时数据库使用长连接的情况下，需要logoff
            DBInterface.RTDBInterface.LongPI.RTDbHelper rtdbhelperpi = null;
            DBInterface.RTDBInterface.LongPGIM.RTDbHelper rtdbhelperpgim = null;
            DBInterface.RTDBInterface.LongXIANDB.RTDbHelper rtdbhelperxiandb = null;
            try
            { 
                //每次执行，都是在新的线程中，这个方式，PGIM会不会有问题，要测试
                //关于异常处理方式的说明：
                //——总体原则1：不在helper层处理异常，将异常放到DAO层处理。               

                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper();
                    rtdbhelperpi.Logoff();
                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpgim = new DBInterface.RTDBInterface.LongPGIM.RTDbHelper();
                    rtdbhelperpgim.Logoff();
                }
                else if (APPConfig.rtdb_Type == "PI") //PGIM数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper();
                    rtdbhelperpi.Logoff();
                }
                else if (APPConfig.rtdb_Type == "XIANDB") //PGIM数据库的情况下，使用PI长连接类
                {
                    rtdbhelperxiandb = new DBInterface.RTDBInterface.LongXIANDB.RTDbHelper();
                    rtdbhelperxiandb.Logoff();
                }
                else
                {
                                       
                }    
                
               
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                //RTDBHelper在底层（pgimhelper、goldenhelper）对异常进行处理，将异常信息记录到rtdbhelper.Exception中，并将异常转发
                //在这里，如果Exception不为nulll，则说明底层捕捉到了异常信息，应该将底层捕捉到的Exception计入log
                //如果Exception为null，则说明底层没有捕捉到异常信息。这里应该记录ex内容
                string messageStr;
                messageStr = String.Format("DAO层readRTDBData()错误：---------->");
                logHelper.Error(messageStr);

                string exceptionStr = "";
                
                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用PI长连接类
                {
                    exceptionStr = rtdbhelperpi.Exception;
                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用PI长连接类
                {
                    exceptionStr = rtdbhelperpgim.Exception;
                }
                else
                {
                    
                }     

                if (exceptionStr != null)
                {
                    
                }
                else
                {
                    messageStr = String.Format("错误信息：disconnect()未知错误。{0}", ex.ToString());
                }
                logHelper.Error(messageStr);
               
            }
        }
        
        //读取实时数据原始值
        public static List<PValue> ReadData(string tagname,DateTime startdate,DateTime enddate)
        {
            ErrorFlag = false;
            ErrorInfo = "";           

            List<PValue> results = new List<PValue>();
            DBInterface.RTDBInterface.LongPI.RTDbHelper rtdbhelperpi = null;
            DBInterface.RTDBInterface.LongPGIM.RTDbHelper rtdbhelperpgim = null;
            DBInterface.RTDBInterface.LongXIANDB.RTDbHelper rtdbhelperxiandb = null;
            DBInterface.RTDBInterface.LongTOM.RTDbHelper rtdbhelpertom = null;
            DBInterface.RTDBInterface.RTDbHelper rtdbhelper = null;

            int errorcount = 0;     //错误统计计数
            //特别注意上面，如果在上面的位置不是给null，而是用new实例化。则某种数据库就会被两个对象初始化。
            //这样会造成问题！！！
            //出现问题的现象是：比如PI数据库，
            //——1、运行rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper()时，不执行任何操作，但是RTDBInterface.LongPI.RTDbHelper第一次执行时，会创建一个静态的rtdbhelper
            //——2、运行rtdbhelper = new DBInterface.RTDBInterface.RTDbHelper()时，该构造函数，会创建非静态rtdbhelper，对静态rtdbhelper造成干扰。使的静态rtdbhelper中的pihelper指向发生变化。
            beginRead:
            try
            {            
                //每次执行，都是在新的线程中，这个方式，PGIM会不会有问题，要测试
                //关于异常处理方式的说明：
                //——总体原则1：不在底层helper层处理异常，将异常放到DAO层处理。               

                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper();
                    results = rtdbhelperpi.GetRawValues(tagname, startdate, enddate);
                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpgim = new DBInterface.RTDBInterface.LongPGIM.RTDbHelper();
                    results = rtdbhelperpgim.GetRawValues(tagname, startdate, enddate);
                }
                else if (APPConfig.rtdb_Type == "XIANDB")
                {
                    rtdbhelperxiandb = new DBInterface.RTDBInterface.LongXIANDB.RTDbHelper();
                    results = rtdbhelperxiandb.GetRawValues(tagname, startdate, enddate);
                }
                else if (APPConfig.rtdb_Type == "TOM")
                {
                    rtdbhelpertom = new DBInterface.RTDBInterface.LongTOM.RTDbHelper();
                    results = rtdbhelpertom.GetRawValues(tagname, startdate, enddate);
                }
                else
                {
                    rtdbhelper = new DBInterface.RTDBInterface.RTDbHelper();
                    results = rtdbhelper.GetRawValues(tagname, startdate, enddate);
                }     
                
                return results;
            }
            catch (Exception ex)
            {
                errorcount = errorcount + 1;
                if (errorcount > APPConfig.rtdb_maxrepeatonerror)
                {
                    //RTDBHelper在底层（pgimhelper、goldenhelper）对异常进行处理，将异常信息记录到rtdbhelper.Exception中，并将异常转发
                    //在这里，如果Exception不为null，则说明底层捕捉到了异常信息，应该将底层捕捉到的Exception计入log
                    //如果Exception为null，则说明底层没有捕捉到异常信息。这里应该记录ex内容    
                    errorcount = 0;
                    ErrorFlag = true;
                    if (APPConfig.rtdb_Type == "PI" && rtdbhelperpi.Exception != null)        //PI数据库的情况下，使用PI长连接类
                    {
                        ErrorInfo = rtdbhelperpi.Exception;
                    }
                    else if (APPConfig.rtdb_Type == "PGIM" && rtdbhelperpgim.Exception != null) //PGIM数据库的情况下，使用PI长连接类
                    {
                        ErrorInfo = rtdbhelperpgim.Exception;
                    }
                    else if (APPConfig.rtdb_Type == "XIANDB" && rtdbhelperxiandb.Exception != null)
                    {
                        ErrorInfo = rtdbhelperxiandb.Exception;
                    }
                    else
                    {
                        ErrorInfo = ex.ToString();
                    }
                    //log
                    string messageStr;
                    messageStr = String.Format("DAO层RTDBDAO.ReadData()错误：---------->") + Environment.NewLine;
                    //logHelper.Error(messageStr);
                    messageStr += String.Format("超过最大重读次数{0}次，跳过此次实时数据读取。错误信息：{1}",APPConfig.rtdb_maxrepeatonerror, ErrorInfo);
                    logHelper.Error(messageStr);
                    //实时数据库RTDB出现任何异常，返回null
                    return null;
                }
                else
                {
                    if (APPConfig.rtdb_Type == "PI" && rtdbhelperpi.Exception != null)        //PI数据库的情况下，使用PI长连接类
                    {
                        ErrorInfo = rtdbhelperpi.Exception;
                    }
                    else if (APPConfig.rtdb_Type == "PGIM" && rtdbhelperpgim.Exception != null) //PGIM数据库的情况下，使用PI长连接类
                    {
                        ErrorInfo = rtdbhelperpgim.Exception;
                    }
                    else if (APPConfig.rtdb_Type == "XIANDB" && rtdbhelperxiandb.Exception != null)
                    {
                        ErrorInfo = rtdbhelperxiandb.Exception;
                    }
                    else
                    {
                        ErrorInfo = ex.ToString();
                    }
                    //log
                    string messageStr;
                    messageStr = String.Format("DAO层RTDBDAO.ReadData()错误，正在尝试第{0}次重新读取。错误信息：{1}", errorcount,ErrorInfo);                    
                    logHelper.Info(messageStr); //注意，未超过最大读取次数时，先记录info。超过后，才记录error
                    //等待设定的毫秒数
                    Thread.Sleep(APPConfig.rtdb_waitsecondonerror*1000);
                    //重新读取
                    goto beginRead;
                }
            }
        }

        //读取实时数据插值
        public static List<PValue> ReadDataInterval(string tagname, DateTime startdate, DateTime enddate,int interval)
        {
            ErrorFlag = false;
            ErrorInfo = "";

            List<PValue> results = new List<PValue>();

            DBInterface.RTDBInterface.LongPI.RTDbHelper rtdbhelperpi = null;
            DBInterface.RTDBInterface.LongPGIM.RTDbHelper rtdbhelperpgim = null;
            DBInterface.RTDBInterface.LongXIANDB.RTDbHelper rtdbhelperxiandb = null;
            DBInterface.RTDBInterface.RTDbHelper rtdbhelper = null;
            //特别注意上面，如果在上面的位置不是给null，而是用new实例化。则某种数据库就会被两个对象初始化。
            //这样会造成问题！！！
            //出现问题的现象是：比如PI数据库，
            //——1、运行rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper()时，不执行任何操作，但是RTDBInterface.LongPI.RTDbHelper第一次执行时，会创建一个静态的rtdbhelper
            //——2、运行rtdbhelper = new DBInterface.RTDBInterface.RTDbHelper()时，该构造函数，会创建非静态rtdbhelper，对静态rtdbhelper造成干扰。使的静态rtdbhelper中的pihelper指向发生变化。

            try
            {
                //每次执行，都是在新的线程中，这个方式，PGIM会不会有问题，要测试
                //关于异常处理方式的说明：
                //——总体原则1：不在底层helper层处理异常，将异常放到DAO层处理。               

                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper();
                    results = rtdbhelperpi.GetIntervalValuesFixInterval(tagname, startdate, enddate, interval);
                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpgim = new DBInterface.RTDBInterface.LongPGIM.RTDbHelper();
                    results = rtdbhelperpgim.GetIntervalValuesFixInterval(tagname, startdate, enddate, interval);
                }
                else if (APPConfig.rtdb_Type == "XIANDB")
                {
                    rtdbhelperxiandb = new DBInterface.RTDBInterface.LongXIANDB.RTDbHelper();
                    results = rtdbhelperxiandb.GetIntervalValuesFixInterval(tagname, startdate, enddate, interval);
                }
                else
                {
                    rtdbhelper = new DBInterface.RTDBInterface.RTDbHelper();
                    results = rtdbhelper.GetIntervalValuesFixInterval(tagname, startdate, enddate, interval);
                }

                return results;
            }
            catch (Exception ex)
            {
                //RTDBHelper在底层（pgimhelper、goldenhelper）对异常进行处理，将异常信息记录到rtdbhelper.Exception中，并将异常转发
                //在这里，如果Exception不为null，则说明底层捕捉到了异常信息，应该将底层捕捉到的Exception计入log
                //如果Exception为null，则说明底层没有捕捉到异常信息。这里应该记录ex内容    
                ErrorFlag = true;
                if (APPConfig.rtdb_Type == "PI" && rtdbhelperpi.Exception != null)        //PI数据库的情况下，使用PI长连接类
                {
                    ErrorInfo = rtdbhelperpi.Exception;
                }
                else if (APPConfig.rtdb_Type == "PGIM" && rtdbhelperpgim.Exception != null) //PGIM数据库的情况下，使用PI长连接类
                {
                    ErrorInfo = rtdbhelperpgim.Exception;
                }
                else if (APPConfig.rtdb_Type == "XIANDB" && rtdbhelperxiandb.Exception != null)
                {
                    ErrorInfo = rtdbhelperxiandb.Exception;
                }
                else
                {
                    ErrorInfo = ex.ToString();
                }
                //log
                string messageStr;
                messageStr = String.Format("DAO层RTDBDAO.ReadData()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ErrorInfo);
                logHelper.Error(messageStr);
                //实时数据库RTDB出现任何异常，返回null
                return null;
            }
        }

        //读取实时数据库标签列表       
        public static List<DBInterface.RTDBInterface.SignalComm> ReadTagList()
        {
            string tagfilter = "";
           
            return ReadTagList(tagfilter);
           

        }
        public static List<DBInterface.RTDBInterface.SignalComm> ReadTagList(string filter)
        {
            List<DBInterface.RTDBInterface.SignalComm> results = new List<DBInterface.RTDBInterface.SignalComm>();

            DBInterface.RTDBInterface.LongPI.RTDbHelper rtdbhelperpi = null;
            DBInterface.RTDBInterface.LongPGIM.RTDbHelper rtdbhelperpgim = null;
            DBInterface.RTDBInterface.RTDbHelper rtdbhelper = null;

            try
            {               
                
                string tagfilter = filter;                      //这条语句经过测试可用。会返回所有标签

                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpi = new DBInterface.RTDBInterface.LongPI.RTDbHelper();
                    results = rtdbhelperpi.GetTagList(tagfilter);
                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用PI长连接类
                {
                    rtdbhelperpgim=new DBInterface.RTDBInterface.LongPGIM.RTDbHelper();
                    results = rtdbhelperpgim.GetTagList(tagfilter);
                }
                else
                {
                    rtdbhelper=new DBInterface.RTDBInterface.RTDbHelper();
                    results = rtdbhelper.GetTagList(tagfilter);
                }                     
                
                return results;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                //RTDBHelper在底层（pgimhelper、goldenhelper）对异常进行处理，将异常信息记录到rtdbhelper.Exception中，并将异常转发
                //在这里，如果Exception不为nulll，则说明底层捕捉到了异常信息，应该将底层捕捉到的Exception计入log
                //如果Exception为null，则说明底层没有捕捉到异常信息。这里应该记录ex内容
                string messageStr;
                messageStr = String.Format("DAO层ReadTagList()错误：---------->");
                logHelper.Error(messageStr);

                string exceptionStr = "";

                if (APPConfig.rtdb_Type == "PI")        //PI数据库的情况下，使用PI长连接类
                {
                    exceptionStr = rtdbhelperpi.Exception;
                }
                else if (APPConfig.rtdb_Type == "PGIM") //PGIM数据库的情况下，使用PI长连接类
                {
                    exceptionStr = rtdbhelperpgim.Exception;
                }
                else
                {
                    exceptionStr = rtdbhelper.Exception;
                }

                if (exceptionStr != null)
                {
                    messageStr = String.Format("错误信息：rtdbhelper.GetTagList()错误。{0}", rtdbhelper.Exception);
                }
                else
                {
                    messageStr=String.Format("错误信息：ReadTagList()未知错误。{0}",ex.ToString());
                }
                logHelper.Error(messageStr);
                return null;
            }
        }
        
    }
}
