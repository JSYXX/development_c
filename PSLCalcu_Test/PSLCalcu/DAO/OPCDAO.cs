using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCCommon;                         //使用PValue
using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Data;                      //使用IDataReader
using Config;   //使用log
using System.Linq;                      //使用linq

namespace PSLCalcu
{
    /// <summary>
    /// OPC数据表DAO
    /// 1、关于质量位的处理原则
    ///——特别注意，计算引擎读接口的本质，就是要读有效数据。如果数据无效，应该在DAO层处理过滤掉。而不是在交给计算引擎处理。
    ///——所以，如果底层原始数据无效，（尤其是实时数据和OPC数据），对应时间段应该是无数据
    /// </summary>
    public class OPCDAO
    {
        //使用关系数据库接口读取OPC数据库数据的DAO层
        //关系数据库接口的错误，主要在DAO层处理
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(OPCDAO));          //DAO层log

        #region 公有变量
        public static bool ErrorFlag = false;                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        public static string ErrorInfo = "";
        public static long errorCount;                                                      //获取概化计算主线程错误统计
        public static bool errorFlag;                                                       //获取概化计算主线程错误标志

        #endregion

        #region 公有方法
        //读取opc标签和id映射
        public static List<OPCTagIdMap> ReadTagIdMap()
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                List<OPCTagIdMap> opctagidmap = new List<OPCTagIdMap>();
                //清空表数据
                sqlStr = String.Format("use opcdb;select opctagindex,opcserverindex,opctagname,opctagdesc,opctagdatatype,opcdbtagname from opctagconfig");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                opctagidmap = IDataReader2OPCTagIdMap(reader);
                reader.Close();
                return opctagidmap;
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("DAO层错误：------------------------------------------------------------------") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——OPCTagConfigDAO.ReadTagIdMap()错误！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("详细错误信息", ex) + Environment.NewLine;
                logHelper.Error(messageStr);

                //messageStr = String.Format("读取OPC标签信息时出错，请检查\\log目录下的log文件！！");
                //MessageBox.Show(messageStr);

                return null;
            }
        }
        //读取OPC表数据，仅返回起止时刻间的原值，
        public static List<PValue> ReadRaw(string opctagname, DateTime startdate, DateTime enddate)
        { 
            ErrorFlag = false;
            string databasename = "opcdb";
            string tablepre = "opcdata";
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                List<PValue> results = new List<PValue>();
                List<OPCDataItem> psldataitems = new List<OPCDataItem>();
                IDataReader reader;
                string tablename = "";

                if (startdate.Year == enddate.Year && startdate.Month == enddate.Month)
                {
                    tablename = tablepre + startdate.ToString("yyyyMM");
                    //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                    sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{2}') and opctagstarttime>='{3}' and opctagstarttime<'{4}'", databasename, tablename, opctagname, startdate.Ticks, enddate.Ticks);
                    reader = dbhelper.ExecuteReader(sqlStr);
                    psldataitems = IDataReader2OPCDataItems(reader);
                    reader.Close();
                }
                //如果起始时间和结束时间不在同一年，则多表联合查询
                else
                {
                    tablename = tablepre + startdate.ToString("yyyyMM");
                    sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{2}') and opctagstarttime>='{3}' and opctagstarttime<'{4}'",
                                            databasename, 
                                            tablename, 
                                            opctagname, 
                                            startdate.Ticks, 
                                            enddate.Ticks);
                    DateTime currentDate = startdate.AddMonths(1);
                    do 
                    {
                        tablename = tablepre + currentDate.ToString("yyyyMM");
                        sqlStr = sqlStr + String.Format(" union select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{2}') and opctagstarttime>='{3}' and opctagstarttime<'{4}'",
                                            databasename,
                                            tablename, 
                                            opctagname, 
                                            startdate.Ticks, 
                                            enddate.Ticks);
                        currentDate = currentDate.AddMonths(1);
                    }while(int.Parse(currentDate.ToString("yyyyMM")) <= int.Parse(enddate.ToString("yyyyMM")));
                    //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间                   
                    reader = dbhelper.ExecuteReader(sqlStr);
                    psldataitems = IDataReader2OPCDataItems(reader);
                    reader.Close();
                }

                if (psldataitems.Count > 1)
                {
                    for (int i = 0; i < psldataitems.Count - 1; i++)
                    {
                        tablename = tablepre + startdate.ToString("yyyyMM");
                        results.Add(new PValue(psldataitems[i].opctagvalue, new DateTime(psldataitems[i].opctagstarttime), new DateTime(psldataitems[i + 1].opctagstarttime), 0));
                    }
                    results.Add(new PValue(psldataitems[psldataitems.Count - 1].opctagvalue, new DateTime(psldataitems[psldataitems.Count - 1].opctagstarttime), enddate, 0));
                }
                else if (psldataitems.Count == 1)
                {
                    results.Add(new PValue(psldataitems[0].opctagvalue, new DateTime(psldataitems[0].opctagstarttime), enddate, 0));
                }
                else
                {
                    results = null;
                }


                return results;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层OPCDataDAO.Read()错误：---------->")+Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        //读取OPC表数据，返回起止时刻间的原值，返回起止时刻点的前值(或插值)
        public static List<PValue> Read(string opctagname, DateTime startdate, DateTime enddate)
        {
            //优化opcdata数据表
            //1、采用没有事物的MyISAM引擎。
            //2、采用tagid和tagstarttime做聚合索引
            //——读接口根据starttime和endtime的年份决定是单表查询还是多表联合查询
            //——接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
            //3、该接口需要测一下几种情况。
            //——单月有值，有前值，已测。正常返回前值和中间值
            //——多月有值，有前值，已测。正常返回前值和中间值
            //——单月有值，无前值，已测。正常返回中间值。但起始时刻点无值。
            //——多月有值，无前值，已测。正常返回中间值。但起始时刻点无值。
            //——单月无值，有前值，已测。如果本月或前一个月内有前值，则起止时间均返回前值。
            //——多月无值，有前值，已测。如果本月或前一个月内有前值，则起止时间均返回前值。
            //——单月无值，无前值，已测。如果本月或前一个月内无前值，则返回空。
            //——多月无值，无前值，已测。如果本月或前一个月内无前值，则返回空。
            //4、关于质量位的处理原则
            //——特别注意，计算引擎读接口的本质，就是要读有效数据。如果数据无效，应该在DAO层处理过滤掉。而不是在交给计算引擎处理。
            //——所以，如果底层原始数据无效，（尤其是实时数据和OPC数据），对应时间段应该是无数据
            ErrorFlag = false;
            string databasename = "opcdb";
            string tablepre = "opcdata";
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                List<PValue> results = new List<PValue>();
                List<OPCDataItem> psldataitems = new List<OPCDataItem>();
                IDataReader reader;
                string tablename = "";
                //如果起始时间和结束时间在同一年同一月，则单表查询
                //
                if (startdate.Year == enddate.Year && startdate.Month == enddate.Month)
                {
                    tablename = tablepre + startdate.ToString("yyyyMM");
                    //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                    //特别注意，opctagindex才是标签的定义的序号。opctagid仅仅是一个记录好。
                    //——但是截至时刻的值，不能直接用 opctagstarttime<='{4}'来取。因为，如果在opctagstarttime>='{3}' and opctagstarttime<'{4}'内没有值，就应该返回空值。而不需要添加截止值。
                    //——直接用 tagstarttime<='{3}'来取，可能在opctagstarttime>='{3}' and opctagstarttime<'{4}'内没有值，但是确把截止值取进来。
                    sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{2}') and opctagstarttime>='{3}' and opctagstarttime<'{4}'", 
                                            databasename,
                                            tablename, 
                                            opctagname, 
                                            startdate.Ticks, 
                                            enddate.Ticks);
                    logHelper.Info(sqlStr);
                    reader = dbhelper.ExecuteReader(sqlStr);
                    psldataitems = IDataReader2OPCDataItems(reader);
                    reader.Close();
                }
                //如果起始时间和结束时间不在同一年，则多表联合查询
                else 
                {
                    tablename = tablepre + startdate.ToString("yyyyMM");
                    //——但是截至时刻的值，不能直接用 opctagstarttime<='{4}'来取。因为，如果在opctagstarttime>='{3}' and opctagstarttime<'{4}'内没有值，就应该返回空值。而不需要添加截止值。
                    //——直接用 tagstarttime<='{3}'来取，可能在opctagstarttime>='{3}' and opctagstarttime<'{4}'内没有值，但是确把截止值取进来。
                    sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{2}') and opctagstarttime>='{3}' and opctagstarttime<'{4}'", 
                                            databasename, 
                                            tablename, 
                                            opctagname,
                                            startdate.Ticks, 
                                            enddate.Ticks);
                    DateTime currentDate=startdate.AddMonths(1);
                    do 
                    {
                        tablename = tablepre + currentDate.ToString("yyyyMM");
                        //——但是截至时刻的值，不能直接用 opctagstarttime<='{4}'来取。因为，如果在opctagstarttime>='{3}' and opctagstarttime<'{4}'内没有值，就应该返回空值。而不需要添加截止值。
                        //——直接用 tagstarttime<='{3}'来取，可能在opctagstarttime>='{3}' and opctagstarttime<'{4}'内没有值，但是确把截止值取进来。
                        sqlStr = sqlStr + String.Format("union select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{2}') and opctagstarttime>='{3}' and opctagstarttime<'{4}'",
                                            databasename,        
                                            tablename,
                                            opctagname, 
                                            startdate.Ticks,
                                            enddate.Ticks);
                        currentDate = currentDate.AddMonths(1);
                    } while (int.Parse(currentDate.ToString("yyyyMM")) <= int.Parse(enddate.ToString("yyyyMM"))) ;
                    //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间                   
                    reader = dbhelper.ExecuteReader(sqlStr);
                    psldataitems = IDataReader2OPCDataItems(reader);
                    reader.Close();
                }
                if (psldataitems.Count > 0)
                {
                    for (int i = 0; i < psldataitems.Count - 1; i++)
                    {
                        tablename = tablepre + startdate.ToString("yyyyMM");
                        results.Add(new PValue(psldataitems[i].opctagvalue, new DateTime(psldataitems[i].opctagstarttime), new DateTime(psldataitems[i + 1].opctagstarttime), 0));
                    }
                    //处理最后一个点的截止时间
                    results.Add(new PValue(psldataitems[psldataitems.Count - 1].opctagvalue, new DateTime(psldataitems[psldataitems.Count - 1].opctagstarttime), enddate, 0));
                 
                }
                
                //排序。由于并发计算引擎，计算的顺序不是按照时间的先后。写入的顺序也不是按照时间的先后。因此，如果读取的数据是并发引擎计算的。读取出的数据，也不是按照时间先后顺序排列的。
                results = results.OrderBy(m => m.Timestamp).ToList();

                //处理第一个点
                #region 按折线型看待opc实时数据，则起始时刻点应该用前后点插值来求得
                /*
                //如果返回数据的第一个值得起始时间，不等于总起始时间，则取起始时间之前最近的一个OPC值。如果第一个值得起始时间等于总起始时间，则不用进行任何处理。
                if (results==null || results.Count == 0 || results[0].Timestamp != startdate)
                {
                    //1、找当前取数周期起始时间的前一个有效值时刻，仅在本月或上月的表中找。返回值可能是0个、1个、2个
                    string currenttablename = tablepre + startdate.ToString("yyyyMM");
                    string previoustablename = tablepre + startdate.AddMonths(-1).ToString("yyyyMM");
                    sqlStr = String.Format("use {0};select Max(opctagstarttime) from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime<'{4}' union select Max(opctagstarttime) from {2} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime<'{4}'",
                                            databasename, 
                                            currenttablename, 
                                            previoustablename, 
                                            opctagname,
                                            startdate.Ticks
                                            );  //在当前月份和上一个月份中联合查询opctagstarttime<startdate的opctagstarttime最大值
                    reader = dbhelper.ExecuteReader(sqlStr);
                    List<DateTime> maxtime = IDataReader2Starttime(reader);
                    reader.Close();
                    
                    //2、对找到的数据，按照时间进行排序。这里有可能取到多个时间，也可能没有数据：
                    //——1、因为本月和上月两个表，所以如果本月在起始时间前还有有效值，会返回一个值。上月的数据中也会返回一个值。
                    //——2、不排除同一时刻（连毫秒值也想同）有两个值。
                    //——3、可能本月在起始时间前，没有有效值了。上一个月没有有效的OPC数据。这样就不会返回任何值                   
                    if (maxtime.Count != 0)
                    {
                        //如果找到了前一时刻有效值，取最近的有效值。
                        maxtime.OrderBy(m => m).ToList();
                        long MaxDatetime = maxtime[0].Ticks;     //取返回的时间中最大的一个值，也就是在starttime前最近的一个值，注意这里是ticks值，long类型
                        sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime='{4}' union select opctagvalue,opctagstarttime,opctagstatus from {2} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime='{4}'",
                                        databasename,
                                        currenttablename,
                                        previoustablename,
                                        opctagname,
                                        MaxDatetime); //在当前月份和上一个月份中联合查询opctagstarttime=maxtime的tag值
                        reader = dbhelper.ExecuteReader(sqlStr);
                        psldataitems = IDataReader2OPCDataItems(reader);
                        reader.Close();
                        //如果psldataitems包含多个值，则说明这些值都处于同一时刻，正常情况下不应该发生。如果真出现这种情况，事实上也无法分清究竟应该取哪一个值。这里采用psldataitems第一个值。
                        if (results != null && results.Count > 0)
                        {   //之前的results如果有有效值，则根据前一个有效值previouspvalue和查询到的第一个值results[0]，计算出startdate的插值。
                            PValue previouspvalue = new PValue(psldataitems[0].opctagvalue, new DateTime(psldataitems[0].opctagstarttime), results[0].Timestamp, 0);
                            PValue firstpvalue = PValue.Interpolate(previouspvalue, results[0], startdate);   //PValue.Interpolation插值得到的点timestamp和endtime都为输入的时刻值
                            firstpvalue.Endtime = results[0].Timestamp; //PValue.Interpolation插值得到的点timestamp和endtime都为输入的时刻值
                            results.Insert(0, firstpvalue);     //在首位置插入第一个点
                        }
                        else
                        {   //之前的results如果没有有效值，则用该值填充全段
                            results.Add(new PValue(psldataitems[0].opctagvalue, startdate, enddate, 0));
                        }
                    }
                    else
                    {
                        //如果没有找到前一个时刻的有效数据，则起始时间到第一个有效时间段内。是无效时间。不添加任何值。
                        //——特别注意，计算引擎读接口的本质，就是要读有效数据。如果数据无效，应该在DAO层处理过滤掉。而不是在交给计算引擎处理。
                        //——所以，如果底层原始数据无效，（尤其是实时数据和OPC数据），对应时间段应该是无数据
                        if (results != null && results.Count > 0)
                        {
                            //之前的results如果有有效值，则在startdate和第一个有效值间，无数据
                        }
                        else
                        {   //之前的results如果也没有有效值，则整个results为空值。
                            results=null;
                        }
                    }
                }
                */
                #endregion
                #region 按阶梯型看待opc实时数据，则起始时刻点直接用前一个有效点的值替代。
                //如果返回数据的第一个值得起始时间，不等于总起始时间，则取起始时间之前最近的一个OPC值。如果第一个值得起始时间等于总起始时间，则不用进行任何处理。
                if (results == null || results.Count == 0 || results[0].Timestamp != startdate)
                {
                    //1、找当前取数周期起始时间的前一个有效值时刻，仅在本月或上月的表中找。返回值可能是0个、1个、2个
                    string currenttablename = tablepre + startdate.ToString("yyyyMM");
                    string previoustablename = tablepre + startdate.AddMonths(-1).ToString("yyyyMM");
                    sqlStr = String.Format("use {0};select Max(opctagstarttime) from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime<'{4}' union select Max(opctagstarttime) from {2} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime<'{4}'",
                                            databasename,
                                            currenttablename,
                                            previoustablename,
                                            opctagname,
                                            startdate.Ticks
                                            );  //在当前月份和上一个月份中联合查询opctagstarttime<startdate的opctagstarttime最大值
                    reader = dbhelper.ExecuteReader(sqlStr);
                    List<DateTime> maxtime = IDataReader2Starttime(reader);
                    reader.Close();

                    //2、对找到的数据，按照时间进行排序。这里有可能取到多个时间，也可能没有数据：
                    //——1、因为本月和上月两个表，所以如果本月在起始时间前还有有效值，会返回一个值。上月的数据中也会返回一个值。
                    //——2、不排除同一时刻（连毫秒值也想同）有两个值。
                    //——3、可能本月在起始时间前，没有有效值了。上一个月没有有效的OPC数据。这样就不会返回任何值                   
                    if (maxtime.Count != 0)
                    {
                        //如果找到了前一时刻有效值，取最近的有效值。
                        maxtime.OrderBy(m => m).ToList();
                        long MaxDatetime = maxtime[0].Ticks;     //取返回的时间中最大的一个值，也就是在starttime前最近的一个值，注意这里是ticks值，long类型
                        sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime='{4}' union select opctagvalue,opctagstarttime,opctagstatus from {2} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime='{4}'",
                                        databasename,
                                        currenttablename,
                                        previoustablename,
                                        opctagname,
                                        MaxDatetime); //在当前月份和上一个月份中联合查询opctagstarttime=maxtime的tag值
                        reader = dbhelper.ExecuteReader(sqlStr);
                        psldataitems = IDataReader2OPCDataItems(reader);
                        reader.Close();
                        //如果psldataitems包含多个值，则说明这些值都处于同一时刻，正常情况下不应该发生。
                        //如果真出现这种情况，事实上也无法分清究竟应该取哪一个值。这里采用psldataitems第一个值psldataitems[0]。
                        if (results != null && results.Count > 0)
                        {   
                            //之前的results如果有有效值，则用前一个有效值previouspvalue直接做startdate时刻的值。                           
                            PValue firstpvalue = new PValue(psldataitems[0].opctagvalue, startdate, startdate,psldataitems[0].opctagstatus);   //PValue.Interpolation插值得到的点timestamp和endtime都为输入的时刻值                            
                            results.Insert(0, firstpvalue);     //在首位置插入第一个点
                        }
                        else
                        {   //之前的results如果没有有效值，则用该值填充全段
                            results.Add(new PValue(psldataitems[0].opctagvalue, startdate, enddate, 0));
                        }
                    }
                    else
                    {
                        //如果没有找到前一个时刻的有效数据，则起始时间到第一个有效时间段内。是无效时间。不添加任何值。
                        //——特别注意，计算引擎读接口的本质，就是要读有效数据。如果数据无效，应该在DAO层处理过滤掉。而不是在交给计算引擎处理。
                        //——所以，如果底层原始数据无效，（尤其是实时数据和OPC数据），对应时间段应该是无数据
                        if (results != null && results.Count > 0)
                        {
                            //之前的results如果有有效值，则在startdate和第一个有效值间，无数据
                        }
                        else
                        {   //之前的results如果也没有有效值，则整个results为空值。
                            results = null;
                        }
                    }

                }
                else
                {
                    //第一个值的起始时间等于总起始时间则什么也不做
                }
                #endregion
                
                //处理最后一个点：
                #region 按折线型看待opc实时数据，则截止时刻点应该用前后点插值来求得
                /*
                //——按照最合理的PI数据库的处理方式，返回点的第一个点，是用starttime时刻前一个点和后一个点插值计算而的。返回点的最后一个点，使用endtime时刻前一个点和后一个点的插值计算而得。               
                if (results == null || results.Count == 0 || results[results.Count - 1].Timestamp != enddate)
                {
                    //1、找当前取数周期截止时间的后一个有效值时刻，仅在本月或下月的表中找。返回值可能是0个、1个、2个
                    string currenttablename = tablepre + startdate.ToString("yyyyMM");
                    string nexttablename = tablepre + startdate.AddMonths(1).ToString("yyyyMM");
                    sqlStr = String.Format("use {0};select Min(opctagstarttime) from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime>'{4}' union select Min(opctagstarttime) from {2} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime>'{4}'",
                                            databasename,
                                            currenttablename,
                                            nexttablename,
                                            opctagname,
                                            enddate.Ticks
                                            );  //在当前月份和上一个月份中联合查询opctagstarttime<startdate的opctagstarttime最大值
                    reader = dbhelper.ExecuteReader(sqlStr);
                    List<DateTime> mintime = IDataReader2Starttime(reader);
                    reader.Close();

                    //2、对找到的数据，按照时间进行排序。这里有可能取到多个时间，也可能没有数据：
                    //——1、因为本月和上月两个表，所以如果本月在起始时间前还有有效值，会返回一个值。上月的数据中也会返回一个值。
                    //——2、不排除同一时刻（连毫秒值也想同）有两个值。
                    //——3、可能本月在结束时间后，没有有效值了。下一个月没有有效的OPC数据。这样就不会返回任何值                   
                    if (mintime.Count != 0)
                    {
                        //如果找到了前一时刻有效值，取最近的有效值。
                        mintime.OrderByDescending(m => m).ToList();
                        long MinDatetime = mintime[0].Ticks;     //取返回的时间中最小的一个值，也就是在endtime后最近的一个值，注意这里是ticks值，long类型
                        sqlStr = String.Format("use {0};select opctagvalue,opctagstarttime,opctagstatus from {1} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime='{4}' union select opctagvalue,opctagstarttime,opctagstatus from {2} where opctagindex=(select opctagindex from opctagconfig where opcdbtagname='{3}') and opctagstarttime='{4}'",
                                        databasename,
                                        currenttablename,
                                        nexttablename,
                                        opctagname,
                                        MinDatetime); //在当前月份和上一个月份中联合查询opctagstarttime=maxtime的tag值
                        reader = dbhelper.ExecuteReader(sqlStr);
                        psldataitems = IDataReader2OPCDataItems(reader);
                        reader.Close();
                        //如果psldataitems包含多个值，则说明这些值都处于同一时刻，正常情况下不应该发生。如果真出现这种情况，事实上也无法分清究竟应该取哪一个值。这里采用psldataitems第一个值。
                        if (results != null && results.Count > 0)
                        {   //之前的results如果有有效值，则根据前一个有效值previouspvalue和查询到的第一个值results[0]，计算出startdate的插值。
                            PValue afterpvalue = new PValue(psldataitems[0].opctagvalue, new DateTime(psldataitems[0].opctagstarttime), results[0].Timestamp, 0);
                            PValue lastpvalue = PValue.Interpolate(results[results.Count - 1], afterpvalue, enddate);   //PValue.Interpolation插值得到的点timestamp和endtime都为输入的时刻值
                            results.Add(lastpvalue);
                        }
                        else
                        {   //之前的results如果没有有效值，则该时间段results应该为空值。                            
                            results = null;                            
                        }
                    }
                    else
                    {
                        //如果没有找到结束时刻后一个时刻的有效数据，则最后一个有效时间到截止时刻时间段内的值为0。状态位为1。
                        if (results != null &&  results.Count > 0)
                        {   //之前的results如果有有效值，则用最后一个有效数值代替结束值
                            results.Add(new PValue(results[results.Count - 1].Value, results[results.Count - 1].Endtime, enddate, 1));
                        }
                        else
                        {   //之前的results如果也没有有效值，则整个results为空值。
                            results = null;
                        }
                    }
                }
                */
                #endregion
                #region 按阶梯型看待opc实时数据，则截止时刻点直接用前一个有效点的值替代。由于阶梯型曲线，截止时刻点实际上也不参与计算，因此这样处理也会提高处理速度
                if (results != null && results.Count > 0)
                {
                    //如果前面result不为空，则直接用最后一个点做截止时刻点
                    results.Add(new PValue(results[results.Count - 1].Value, enddate, enddate, results[results.Count - 1].Status));
                }
                else
                { 
                    //否则直接返回空
                }
                #endregion
                return results;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                ErrorInfo = ex.ToString();
                string messageStr;
                messageStr = String.Format("DAO层OPCDataDAO.Read()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        //读取OPC表数据，返回起止时刻间的原值，返回起止时刻点的前值(或插值)
        #endregion
        
        #region 辅助函数
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集        
        private static List<OPCDataItem> IDataReader2OPCDataItems_old(IDataReader reader)
        {
            List<OPCDataItem> items = new List<OPCDataItem>();
            while (reader.Read())
            {
                OPCDataItem item = new OPCDataItem();
                DAOSupport.ReaderToObject(reader, item);
                items.Add(item);
            }
            return items;
        }
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集，不在用DAOSupport.ReaderToObject这种通用方法。该通用方法仅用于通用表，速度慢。
        private static List<OPCDataItem> IDataReader2OPCDataItems(IDataReader reader)
        {
            List<OPCDataItem> items = new List<OPCDataItem>();
            while (reader.Read())
            {
                OPCDataItem item = new OPCDataItem();

                try { item.opctagvalue = Convert.ToDouble(reader["opctagvalue"]); }
                catch { 
                    item.opctagvalue =float.MinValue; 
                };
                try { item.opctagstarttime = (long)Convert.ToUInt64(reader["opctagstarttime"]); }
                catch { 
                    item.opctagstarttime = 0; 
                };
                try { item.opctagstatus = Convert.ToInt64(reader["opctagstatus"]); }
                catch { 
                    item.opctagstatus = 1; 
                };
               

                items.Add(item);
            }
            return items;
        }
        private static List<DateTime> IDataReader2Starttime(IDataReader reader)
        {
            List<DateTime> items = new List<DateTime>();
            while (reader.Read())
            {
                //读取的MaxStarttime有可能是空值，因此需要
                try
                {
                    string tempStr = reader.GetValue(0).ToString();
                    DateTime datetime = new DateTime(long.Parse(tempStr));
                    items.Add(datetime);
                }
                catch { }
            }
            return items;
        }
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集
        private static List<OPCTagIdMap> IDataReader2OPCTagIdMap(IDataReader reader)
        {
            List<OPCTagIdMap> items = new List<OPCTagIdMap>();
            while (reader.Read())
            {
                OPCTagIdMap item = new OPCTagIdMap();
                DAOSupport.ReaderToObject(reader, item);
                items.Add(item);
            }
            return items;
        }
        #endregion

    }
}