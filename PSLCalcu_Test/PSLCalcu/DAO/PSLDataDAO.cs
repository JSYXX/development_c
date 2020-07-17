using System;
using System.Collections.Generic;
using System.Linq;                      //使用list.orderby
using System.Text;
using System.Threading.Tasks;
using System.Collections;               //使用hashtable
using PCCommon;                         //使用PValue
using DBInterface.RDBInterface;         //使用关系数据库接口
using Config;                           //使用配置模块
using System.Data;                      //使用IDataReader
using System.Windows.Forms;             //使用msgbox
using System.Diagnostics;               //使用计时器
using System.ComponentModel;            //使用BackgroundWorker

namespace PSLCalcu
{
    /// <summary>
    /// 概化数据表DAO
    /// 1、关于质量位的处理原则
    ///——特别注意，计算引擎读接口的本质，就是要读有效数据。如果数据无效，应该在DAO层处理过滤掉。而不是在交给计算引擎处理。
    ///——所以，如果底层原始数据无效，（尤其是实时数据和OPC数据），对应时间段应该是无数据
    /// </summary>
    public class PSLDataDAO
    {
        
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLHistoryCalcuConfigDAO));       //全局log
        private static string psldataTableName="psldata";

        #region 公有变量
        public static bool ErrorFlag = false;                                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        public static string ErrorInfo = "";
        public static int MAX_NUMBER_CONSTTAG = APPConfig.rdbtable_constmaxnumber;
        #endregion

        #region 公用函数
        //psldata数据表优化
        public static bool optiTable_psldata(int startYear, int endYear,int intervalMonth)
        {
            //优化psldata数据表
            //1、采用没有事物的MyISAM引擎。
            //2、采用tagid和tagstarttime做聚合索引
            string sqlStr;
            string databasename = "psldb";
            string tablename = "psldata";
            try
            {
                DbHelper dbhelper = new DbHelper();
                for (int i = startYear; i <= endYear; i++)
                {
                    for (int j = 1; j <= 12 / intervalMonth; j++)
                    {
                        if (APPConfig.rdb_Type == "MySql")
                        {
                            string tablenamefull= tablename + i.ToString() + j.ToString("D2");
                            
                            //采用MyISAM，无事物，处理速度快
                            //sqlStr = String.Format("use {0};alter table {1} engine=MyISAM", databasename, tablenamefull);
                            //dbhelper.ExecuteNonQuery(sqlStr);
                            //20181019，经玉环电厂金属壁温项目测试，在多达10000项计算，60w个标签的情况下，使用innodb引擎写入速度更快。因此注销上面的MyISAM。psldata主表采用innodb引擎。
                            //另，底层的所有数据表最好采用相同的数据引擎。因此要设引擎，也改在IniTable中的createtabl函数中来设置

                            //在innodb引擎下，究竟如何建立引擎最为合理，首先参考文章: http://blog.codinglabs.org/articles/theory-of-mysql-index.html  《MySQL索引背后的数据结构及算法原理》，该文章讲的较为细致
                            //根据上述文章的思想，innodb引擎下的psldata表，采用自增的id号为主键，innodb将采用该id生成主键索引。这样保证的一般情况下，计算引擎写入数据时会顺序插入。
                            //根据上述文章思想，由tagid和tagstarttime构成多列辅助索引，提供对常用的 where tagid=? and tagstarttime between ？ and ?的优化。                            
                            //优化效果见测试文档 E:\公司_开发_信息系统_吴鸿(需备份移动硬盘)\02_MySQL\10_mysql概化库应用测试
                            sqlStr = String.Format("use {0};alter table {1} add index idandtime(tagid,tagstarttime)", databasename,tablenamefull);  //tagid和tagstarttime多列索引，为最常用的点查找提供优化
                            dbhelper.ExecuteNonQuery(sqlStr);

                            //sqlStr = String.Format("use {0};alter table {1} add index tagid(tagid)", databasename,tablenamefull);  //为tagid创建单独索引。有了上面多列索引。这里不再必要
                            //dbhelper.ExecuteNonQuery(sqlStr);

                            //提高查询tagstarttime是否有重复是，对tagstarttime做group的速度
                            //sqlStr = String.Format("use {0};alter table {1} add index starttime(tagstarttime)", databasename,tablenamefull);
                            //dbhelper.ExecuteNonQuery(sqlStr);
                        }
                        else
                        {
                            string msgStr = String.Format("未定义{0}数据库下{1}表的优化方式！", APPConfig.rdb_Type, tablename);
                            MessageBox.Show(msgStr);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string msgStr = String.Format("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                MessageBox.Show(msgStr);
                return false;
            }
        }
        //psldata关闭psldata索引
        public static bool CloseIndex(DateTime startdate, DateTime enddate)
        { 
            //在并发计算时，为了提高批量的插入速度，先关闭索引           
            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr = "";
           
            try
            {
                //数据库连接
                DbHelper dbhelper = new DbHelper();
                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";                
                string tablename = "";
                
                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                      //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month - 1) / APPConfig.psldata_intervalmonth;    //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month - 1) / APPConfig.psldata_intervalmonth;         //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号  

                if (startYearIndex == endYearIndex)
                {
                    tablename = tablepre + startYearIndex.ToString();
                    sqlStr = String.Format("use {0};alter table {1} disable keys", databasename, tablename);
                    dbhelper.ExecuteNonQuery(sqlStr);
                }
                else if (startYearIndex < endYearIndex)
                {
                    //找起止时间段内包含的第一个分表后面的表                        
                    int currentYearIndex;
                    int currentIndex = 0;
                    do
                    {   //从startYearIndex开始，到endYearIndex一共有多少张表。                        
                        currentYearIndex = startYear100 + ((currentIndex + startIndex - 1) / maxIndexInYear) * 100 + 1 + (currentIndex + startIndex - 1) % maxIndexInYear;  //从startYearIndex不断向上得到新表的方法
                        tablename = tablepre + currentYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                        //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                        sqlStr = String.Format("use {0};alter table {1} disable keys", databasename, tablename);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        currentIndex = currentIndex + 1;

                    } while (currentYearIndex < endYearIndex);  
                }
                else
                {
                    return true;
                }
                
                
                return true;
            }
            catch (Exception ex)
            {
                string msgStr = String.Format("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                MessageBox.Show(msgStr);
                return false;
            }
        }
        //psldata重开psldata索引
        public static bool OpenIndex(DateTime startdate, DateTime enddate)
        {
            //在并发计算时，为了提高批量的插入速度，先关闭索引           
            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr = "";

            try
            {
                //数据库连接
                DbHelper dbhelper = new DbHelper();
                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";                
                string tablename = "";

                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                      //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month - 1) / APPConfig.psldata_intervalmonth;    //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month - 1) / APPConfig.psldata_intervalmonth;         //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号  

                if (startYearIndex == endYearIndex)
                {
                    tablename = tablepre + startYearIndex.ToString();
                    sqlStr = String.Format("use {0};alter table {1} enable keys", databasename, tablename);
                    dbhelper.ExecuteNonQuery(sqlStr);
                }
                else if (startYearIndex < endYearIndex)
                {
                    //找起止时间段内包含的第一个分表后面的表                        
                    int currentYearIndex;
                    int currentIndex = 0;
                    do
                    {   //从startYearIndex开始，到endYearIndex一共有多少张表。                        
                        currentYearIndex = startYear100 + ((currentIndex + startIndex - 1) / maxIndexInYear) * 100 + 1 + (currentIndex + startIndex - 1) % maxIndexInYear;  //从startYearIndex不断向上得到新表的方法
                        tablename = tablepre + currentYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                        //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                        sqlStr = String.Format("use {0};alter table {1} enable keys", databasename, tablename);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        currentIndex = currentIndex + 1;

                    } while (currentYearIndex < endYearIndex);
                }
                else
                {
                    return true;
                }


                return true;
            }
            catch (Exception ex)
            {
                string msgStr = String.Format("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                MessageBox.Show(msgStr);
                return false;
            }
        }
        //psldata数据表数据写入接口：写实时计算结果。写单个标签的批量实时概化计算结果。tagid是概化标签id号。
        public static bool WriteOrUpdate(System.UInt32 tagid,List<PValue> pvalues,DateTime startdate,DateTime enddate)
        {
            //——通常每次调用写接口，对应tagid，pvalues只包含一个值，该值是统计值。
            //——如果tagid，当pvalues的长度大于1时，该值为时间序列值。时间序列值的数量一般会比较多，一个小时的原始数据，时间序列一般可能是100到500个时间对。这种值的写入需要优化。
            //接口的startdata和enddate是指数据点的timestamp字段的起始和截止时间
            string sqlStr="";
            ErrorFlag = false;
            ErrorInfo = "";
            try
            {
                //数据库连接
                DbHelper dbhelper = new DbHelper();

                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";
                string readfields = "tagid,tagstarttime,tagendtime,tagvalue,tagstatus";
                string tablename = "";
                
                //——如果pvalues仅有一个值，则该结果是一般统计值，采用先查后插的办法
                //——如果pvalue仅有一个值，也不存在多表问题。startdate落在哪个表就是哪个表
                if(pvalues.Count==1)
                {                    
                    PValue pvalue = pvalues[0];                   
                    //获取当前pvalue的starttime的年份,以判断对应的数据表                    
                    int startYearIndex = pvalue.Timestamp.Year * 100 + 1 + (pvalue.Timestamp.Month - 1) / APPConfig.psldata_intervalmonth;
                    tablename = tablepre + startYearIndex.ToString();
                    //先查找要写入的点是否存在
                    sqlStr = String.Format("use {0};select id from {1} where tagid='{2}' and tagstarttime='{3}'", 
                                            databasename,
                                            tablename, 
                                            tagid, 
                                            pvalue.Timestamp.Ticks
                                            );
                    object obj = dbhelper.ExecuteScalar(sqlStr);
                    //如果找到了id就为记录的id号，如果找不到，id为0
                    int id = Convert.ToInt32(obj);  //如果obj为null，id=0
                    if (id == 0) //如果没有该点，则直接插入数据
                    {
                        sqlStr = String.Format("use {0};insert into {1}({2}) values ('{3}','{4}','{5}','{6}','{7}')",
                                                databasename,                
                                                tablename, 
                                                readfields,
                                                tagid, 
                                                pvalue.Timestamp.Ticks,
                                                pvalue.Endtime.Ticks, 
                                                pvalue.Value, 
                                                pvalue.Status);
                        dbhelper.ExecuteNonQuery(sqlStr);
                    }
                    else        //如果有该点，则更新数据
                    {
                        sqlStr = String.Format("use {0};update {1} set tagid='{2}',tagstarttime='{3}',tagendtime='{4}',tagvalue='{5}',tagstatus='{6}' where id='{7}'",
                                                databasename, 
                                                tablename, 
                                                tagid, 
                                                pvalue.Timestamp.Ticks,
                                                pvalue.Endtime.Ticks, 
                                                pvalue.Value, 
                                                pvalue.Status, 
                                                id);
                        dbhelper.ExecuteNonQuery(sqlStr);
                    }                   
                }
                //如果pvalues有不止一个值，则该结果是时间序列，通常数量较大，采用先删除，后批量写入的办法
                else if (pvalues.Count > 1)
                {
                    //——为批量插入语句生成插入值字符串
                    //——批量写入语句形式为：
                    //——insert into tablename(field1,field2,...) values (value1,value2,...),(value1,value2,...),(value1,value2,...),...(value1,value2,...);
                    //——该语句，在写入800条结果的情况下，可以使写入时间从2000ms降到80ms左右。
                    //——时间序列可能会跨表
                    Hashtable hashTableYearIndex = new Hashtable();
                    foreach (PValue pvalue in pvalues)
                    {
                        //考虑到时间序列有可能跨年，不同年年的时间span要写在不同psldata表中。
                        //需要对时间序列按timestamp的年份，进行分别组合。同一年份的时间序列组合在一起。
                        //一般情况下，时间序列都在一年之中。hash表也仅有一个年份的key值。                        
                        int timestampYearIndex = pvalue.Timestamp.Year * 100 + (1 + (pvalue.Timestamp.Month - 1) / APPConfig.psldata_intervalmonth);
                        if (hashTableYearIndex.ContainsKey(timestampYearIndex))
                        {
                            hashTableYearIndex[timestampYearIndex] = hashTableYearIndex[timestampYearIndex] + "," + String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, pvalue.Timestamp.Ticks, pvalue.Endtime.Ticks, pvalue.Value, pvalue.Status);
                        }
                        else
                        {
                            hashTableYearIndex.Add(timestampYearIndex, String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, pvalue.Timestamp.Ticks, pvalue.Endtime.Ticks, pvalue.Value, pvalue.Status));
                        }
                    }
                    //对hash表进行遍历，写入字符串
                    foreach (DictionaryEntry hasnitem in hashTableYearIndex)
                    {
                        //删掉对应id在startdate和enddate之间的所有值
                        tablename = tablepre + hasnitem.Key.ToString();
                        //特别注意，startdata和enddate是指数据点的starttime字段的起始和截止时间。
                        //由于要写入的数据在不同的表中，在每一个数据表psldataYYYY中，都要先删除一遍timestamp在starttime和endtime之间额记录。
                        //实际上每张表删除的记录是不一样的。比如记录是2016~2017。那么在psldata2016中，删除的仅仅是2016年后半年中对应时间段内的一些记录。在psldata2017中删除的是2017前半段对应时间段内的一些记录
                        if (pvalues[0].Timestamp < startdate) startdate = pvalues[0].Timestamp;                             //某些情况下，要写的数据并不在计算周期范围内。比如读值次信息算法
                        if (pvalues[pvalues.Count - 1].Endtime > enddate) enddate = pvalues[pvalues.Count - 1].Endtime;     //某些情况下，要写的数据并不在计算周期范围内。比如读值次信息算法。特别注意，这里用点的时间取enddate时，要用点的endtime。
                                                                                                                            //——因为很明显，最后一个点肯定是要写入数据库的。那么这个点的timestamp时刻的原有值是要删除的。
                                                                                                                            //——而下面的语句时tagstarttime>=startdate and tagstarttime<enddate。如果用结束点的timestamp做enddate，那么对应时刻的原有点就不会被删除
                        //先删除
                        sqlStr = String.Format("use {0};delete from {1} where tagid='{2}' and tagstarttime>='{3}' and tagstarttime<'{4}'", 
                                                databasename,
                                                tablename, 
                                                tagid, 
                                                startdate.Ticks, 
                                                enddate.Ticks);                        
                        dbhelper.ExecuteNonQuery(sqlStr);
                        //批量写入时间序列
                        sqlStr = String.Format("use {0};insert into {1}({2}) values {3}",
                                                databasename,
                                                tablename, 
                                                readfields,
                                                hasnitem.Value);
                        dbhelper.ExecuteNonQuery(sqlStr);
                    }

                }
                else
                {
                    //如果pvalues.Count==0，则什么也不写入。
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLDataDAO.WriteOrUpdate()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //psldata数据表数据写入接口：写历史计算结果。批量写入某年一段时间的概化计算结果。HistoryResults是准备好的计算结果字符串。
        public static bool WriteHistoryCalcuResults(DateTime startdate,DateTime enddate,string[] HistoryResults)
        {
            //——负责历史计算引擎一段时间数据集中计算产生的批量计算结果的写入。
            //——计算结果已经被组织成一个字符串数组，其中每一个字符串为一个计算结果。使用字符串数据作为计算结果存储主要是多线程并发计算时，解决竞争问题，框架提供的原子操作仅支持字符串这种简单类型。
            //——字符串结构为String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, tagtimestamp, tagendtime, tagvalue, tagstatus)，即已经按照sql写入需要的格式进行组织。
            //——特别注意，HistoryResults格式做成直接可以通过String.Join()连接的方式，效率高。
            //  如果直接通过String.Join()连接，1000个元素大概需要10ms；
            //  如果用foreach连接，1000个元素大概需要90ms；
            //——计算结果不存在常数标签
            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr = "";
            try
            {
 //var sw = Stopwatch.StartNew();                   //计时器，用于测试 
                //数据库连接
                DbHelper dbhelper = new DbHelper();

                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";
                string readfields = "tagid,tagstarttime,tagendtime,tagvalue,tagstatus";
                string tablename = "";

                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                      //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month - 1) / APPConfig.psldata_intervalmonth;   //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month - 1) / APPConfig.psldata_intervalmonth;       //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号 

                if (startYearIndex == endYearIndex)
                {
                    //——注意，由于并发计算并不是按时间先后计算的。因此写入的计算结果也不是按照时间先后。这些记录在读取的时候，需要按照TimeStamp排序。
                    //——此处不考虑删除数据。并发计算引擎，在并发计算前，统一一次性删除。
                    //——不考虑实际写入数据的时间大于起始时间或者小于截止时间的情况。读取值次信息的算法，自己在内部实现删除和写入。
                    tablename = tablepre + startYearIndex.ToString();
                    
                    int iCount = 0;
                    string resultsStr = "";                   
                    foreach (string result in HistoryResults)
                    {
                        resultsStr = resultsStr+result+",";       //连接所有计算结果

                        iCount = iCount + 1;
                        //每读取500条记录写入一次
                        if (iCount % 500 == 0 || iCount == HistoryResults.Length)
                        {
                            resultsStr = resultsStr.Substring(0, resultsStr.Length - 1);
                            sqlStr = String.Format("use {0};insert into {1}({2}) values {3}",
                                                    databasename,
                                                    tablename,
                                                    readfields,
                                                    resultsStr
                                                    );
                            dbhelper.ExecuteNonQuery(sqlStr);
                            resultsStr = "";
                        }
                    }
                }
                else if (startYearIndex < endYearIndex)
                {
                    //如果起始时间小于截止时间，则说明计算结果跨分区表。比如计算2016.6月到2017年6月。大于等于hour的计算，并发周期是30天。因此计算结果在2016年6月份就可能跨年
                    //——不考虑删除数据。并发计算引擎，在并发计算前，统一一次性删除。
                    //——不考虑实际写入数据的时间大于起始时间或者小于截止时间的情况。读取值次信息的算法，自己在内部实现删除和写入。
                    //——按照不同的分区表组织各自的字符串

                    //——并行计算最大单位是天，如果按6个月一个表划分，201801存放1月1日到6月30日的数据。
                    //——并行计算6月30日到7月2日数据，则计算第一天，即6月30日 00:00 到 7月1日 00:00。因为结束时间的endYearIndex为201801，所以肯定走该分支startYearIndex < endYearIndex。
                    //——但是，由于最晚的一个timestamp是6月30日，23:59:00。所以不会有数据落在201802的表内。因此正常情况下，下面的hashTableYearIndex一般，也就只有一个key。不会有两个key。
                   
                    Hashtable hashTableYearIndex = new Hashtable();
                    int iCount=0;
                    foreach (string result in HistoryResults)
                    {
                        string timestamptickStr = result.Split(',')[1];     //起始时间tick字符串
                        long timestamptick = long.Parse(timestamptickStr.Substring(1, timestamptickStr.Length - 2));  //起始时间tick值
                        DateTime timestamp = new DateTime(timestamptick);
                        int timestampYearIndex = timestamp.Year * 100 + (1 + (timestamp.Month - 1)/ APPConfig.psldata_intervalmonth);
                        if (hashTableYearIndex.ContainsKey(timestampYearIndex))
                        {
                            hashTableYearIndex[timestampYearIndex] = hashTableYearIndex[timestampYearIndex] + "," + result;
                        }
                        else
                        {
                            hashTableYearIndex.Add(timestampYearIndex, result);
                        }

                        iCount = iCount + 1;

                        //每读取500条记录写入一次
                        //————————特别注意，这里最开始并没有分批，而是所有HistoryResults加入到一个insert中，向sql写入。
                        //——在2018年10月24日做了测试。
                        //——如果一次性在insert中添加10000个values值，向psldata中写入需要10s。
                        //——如果10000个values值分批，一次性向insert中添加50个value值，向psldata中写入需要677ms。
                        //——如果10000个values值分批，一次性向insert中添加100个value值，向psldata中写入需要507ms。
                        //——如果10000个values值分批，一次性向insert中添加500个value值，向psldata中写入需要420ms。
                        //——如果10000个values值分批，一次性向insert中添加1000个value值，向psldata中写入需要900ms。
                        //——以上情况说明，insert后的value值个数并不是越多越好。具体参考帖子https://blog.csdn.net/willson_l/article/details/73558666。
                        if (iCount % 500 == 0 || iCount == HistoryResults.Length)
                        {
                            //将各分区表自己的字符串写入数据表
                            foreach (DictionaryEntry hasnitem in hashTableYearIndex)
                            {
                                tablename = tablepre + hasnitem.Key.ToString();
                                sqlStr = String.Format("use {0};insert into {1}({2}) values {3} ",
                                                        databasename,
                                                        tablename,
                                                        readfields,
                                                        hasnitem.Value
                                                        );
                                dbhelper.ExecuteNonQuery(sqlStr);

                            }

                            hashTableYearIndex = new Hashtable();
                        }
                    }
                    

                }
                else
                { 
                    //什么也不做，直接返回true
                }
                //Debug.WriteLine("--WriteHistoryCalcuResults complete:" + sw.Elapsed.ToString());
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLDataDAO.WriteHistoryCalcuResults()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //psldata数据表数据读取接口：读取概化数据。读取单个标签的批量概化数据。tagid是概化标签id号。        
        public static List<PValue> Read(System.UInt32 tagid,DateTime startdate,DateTime enddate)
        {
            //——读接口根据starttime和endtime的年份和月份决定是单表查询还是多表联合查询。
            //——读接口的startdata和enddate是指数据点的starttime字段的起始和截止时间。
            //——读接口发生任何错误，均返回null，并置错误标志，由主引擎处理。
            //——读接口取到的数据中，有可能存在状态位不为0的数据。由计算引擎交由算法自己进行过滤或做相应的处理。
            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr="";
             
            try
            {
//var readTimer = Stopwatch.StartNew();          //计时器，用于测试
                //数据库连接
                DbHelper dbhelper = new DbHelper();                              
                IDataReader reader;
                //计算结果
                List<PValue> results = new List<PValue>(); 
                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";
                string readfields = "tagid,tagstarttime,tagendtime,tagvalue,tagstatus";
                string tablename = "";

                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                      //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month -1) / APPConfig.psldata_intervalmonth;    //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month-1) / APPConfig.psldata_intervalmonth;         //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号                   

                //如果tagid >= MAX_NUMBER_CONSTTAG ，则读取的是正常概化标签。
                if (tagid >= MAX_NUMBER_CONSTTAG)
                {                    
                    //如果起始时间和结束时间在同一年同一个分表，则单表查询                
                    if (startYearIndex == endYearIndex)     //满足此条件，说明在同一张表
                    {
                        tablename = tablepre + startYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——概化计算点全部是其他计算的结果，在结构设计中，要求任何计算无论是空值还是错误，都需要有值。因此概化数据，在起始时刻和截止时刻应该有值。入股没有值，则说明发生了不可控的错误。
                        //——基于以上规定，概化数据直接用 tagstarttime<='{3}'来取值。而且当在起始时刻和截止时刻没有值得时候，就仅返回内部值，而不需要再到两个时刻之外，去取值然后在时刻点插值。 
                        //20181019，根据参考文章: http://blog.codinglabs.org/articles/theory-of-mysql-index.html 
                        //对语句SELECT * FROM psldb.psldata201801 where tagid = 10001   and tagstarttime  >= a  and tagstarttime  <=b;用explain进行测试分析，发现该语句并不能利用由tagid和tagstarttime构成的辅助索引
                        //该语句需要改为 SELECT * FROM psldb.psldata201801 where tagid = 10001   and tagstarttime  between a  and b;
                        sqlStr = String.Format("use {0};select {1} from {2} where tagid='{3}' and tagstarttime between '{4}' and '{5}'",
                                                databasename,
                                                readfields,
                                                tablename,                                                
                                                tagid, 
                                                startdate.Ticks, 
                                                enddate.Ticks);
                        //var sqlTimer = Stopwatch.StartNew();          //用于测试读取一个月分钟数据的sql执行所耗时长。一个月分钟数据45000条左右，在公司服务测试结果是耗时4ms-5ms            
                        reader = dbhelper.ExecuteReader(sqlStr);
                        //string sqlspan = sqlTimer.Elapsed.ToString();
                        //var transTimer = Stopwatch.StartNew();        //用于测试读取一个月分钟数据的转换格式所耗时长。一个月分钟数据45000条左右，耗时长300ms左右。改进后减少到200ms。       
                        results = IDataReader2PSLDataItems(reader);                        
                        // string transspan = transTimer.Elapsed.ToString();
                        reader.Close();
                    }
                    //如果起始时间和结束时间不在同一年，则多表联合查询
                    else if (startYearIndex < endYearIndex)
                    {
                        tablename = tablepre + startYearIndex.ToString();                        
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                        //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                        sqlStr = String.Format("use {0};select {1} from {2} where tagid='{3}' and tagstarttime between '{4}' and '{5}'",
                                                databasename,
                                                readfields,
                                                tablename,                                               
                                                tagid, 
                                                startdate.Ticks, 
                                                enddate.Ticks);
                        //找起止时间段内包含的第一个分表后面的表                        
                        int currentYearIndex;                        
                        int currentIndex=0;
                        do
                        {   //从startYearIndex开始，到endYearIndex一共有多少张表。
                            currentIndex = currentIndex + 1;
                            currentYearIndex = startYear100 + ((currentIndex + startIndex-1) / maxIndexInYear) * 100 + 1+(currentIndex + startIndex-1) % maxIndexInYear;  //从startYearIndex不断向上得到新表的方法
                            tablename = tablepre + currentYearIndex.ToString();
                            //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                            //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                            //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                            //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                            sqlStr = sqlStr + String.Format("union all select {0} from {1} where tagid='{2}' and tagstarttime between '{3}' and '{4}'",
                                                readfields,                
                                                tablename,                                                
                                                tagid, 
                                                startdate.Ticks, 
                                                enddate.Ticks);

                        } while (currentYearIndex < endYearIndex);  //循环到currentYearIndex >或者=endYearIndex时推出。正常应该是currentYearIndex=endYearIndex就说明添加到最后一个，就应该退出了。
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间                   
                        reader = dbhelper.ExecuteReader(sqlStr);
                        results = IDataReader2PSLDataItems(reader);
                        reader.Close();
                    }
                    else
                    {
                        return null;
                    }

                    /*将PSLDataItem转为PValue格式。20180712，为了提高效率，IDataReader2PSLDataItems改为直接读取PValue格式的数据，而不再进行转换
                    //foreach (PSLDataItem item in psldataitems)
                    //{
                    //    results.Add(new PValue(item.tagvalue, new DateTime(item.tagstarttime), new DateTime(item.tagendtime), item.tagstatus));
                    //}
                    */

                    //排序。由于并发计算引擎，计算的顺序不是按照时间的先后。写入的顺序也不是按照时间的先后。因此，如果读取的数据是并发引擎计算的。读取出的数据，也不是按照时间先后顺序排列的。
                    if (results != null || results.Count != 0)
                        results = results.OrderBy(m => m.Timestamp).ToList();   
                                       
                    //根据情况添加第一个点。                   
                    //——对于RDB来说，由于RDB已经是计算结果。其值都是在整点整刻。因此从数据库中找到的第一个值都是 tagstarttime={2}的那个值。因此无需对第一个点进行特殊处理。
                    //————在某些特殊情况下，在整点时刻的值，由于计算错误或者前置计算无值，其状态位为1.
                    //————在某些情况下，发生了不可控的错误，则会出现这一刻数据库没有响应的计算结果。出现这种错误，则只能取到起始时刻和截止时刻之间的存在的值。

                    //添加截止时刻点
                    if (results != null && results.Count != 0 && results[results.Count - 1].Timestamp == enddate)
                    {
                        //如果返回数据包含结束时刻
                        results[results.Count - 1].Endtime = enddate;      //如果取到了结束时刻点的值，则修改结束点的Endtime与Timestamp一致。
                    }
                    else if (results != null && results.Count != 0 && results[results.Count - 1].Timestamp < enddate)
                    {
                        //如果返回数据不包含结束时刻，说明该时刻数据库没有对应的值。只能用最后一个有效值填补。
                        results.Add(new PValue(results[results.Count - 1].Value, enddate, enddate, 0));
                    }
                    else
                    {
                        //否则什么也不做
                    }                   

                    /*
                    //***********************基于新的错误处理机制，一般情况下，概化数据在起始时间和截止时间均会有值，只不过这些值有可能有效或者无效（状态位）
                    //*************所以，对于概化数据，如果边界点无数据，就是发生了不可控的错误。不需要再去边界外去取值，然后插值****************************             
                    */     
                
                }//end 读概化标签
                //如果tagid < MAX_NUMBER_CONSTTAG ，则读取的是常量标签
                else
                {
                    //——读接口根据starttime，读取对应标签在starttime之前的最近一个有效值。            
                    //——读接口发生任何错误，均返回null，并置错误标志，交由主引擎处理。                    
                    //获取当前pvalue的starttime的年份
                    int startYear = startdate.Year;
                    //如果年份小于概化数据表起始年份，则直接返回空数据
                    if (startYear < APPConfig.psldata_startyear) return null;
                    //如果年份大于等于概化数据表起始年份，则从后向前遍历，直到找到最后一个有效数据
                    int currentYearIndex;                    
                    int currentIndex=0;
                    do
                    {
                        //maxIndexInYear-endIndex+currentIndex 当前分页currentIndex到enddate分页所在年份顶端分页maxIndexInYear的实际距离。
                        //(maxIndexInYear-endIndex+currentIndex )% maxIndexInYear 上面的实际距离折算到某一年份顶端分页maxIndexInYear的取余距离。
                        //maxIndexInYear-(maxIndexInYear-endIndex+currentIndex )% maxIndexInYear,currentIndex在具体年份内的实际分页数。
                        currentYearIndex = endYear100 - ((maxIndexInYear - endIndex + currentIndex) / maxIndexInYear) * 100 + maxIndexInYear - (maxIndexInYear - endIndex + currentIndex) % maxIndexInYear;
                        tablename = tablepre + currentYearIndex.ToString();
                        sqlStr = String.Format("use {0};select {1} from {2} where tagid='{3}' and tagstarttime=(select Max(tagstarttime) from {2} where tagid='{3}' and tagstarttime<='{4}')",
                                                databasename,
                                                readfields,
                                                tablename,
                                                tagid,
                                                startdate.Ticks
                                               );
                        reader = dbhelper.ExecuteReader(sqlStr);
                        List<PValue> beforestart = IDataReader2PSLDataItems(reader);        //上述sql语句一般返回一条记录，或者不返回记录
                        reader.Close();

                        if (beforestart.Count != 0)
                        {
                            //注意，读取到的值，但是时间上不能用读取到的这个pvalue的时间，而应该以读取周期的时间来给定。这样这个常数值，才能看起来像一个计算周期内的普通pvalue值
                            results.Add(new PValue(beforestart[0].Value, startdate, enddate, 0));
                            results.Add(new PValue(beforestart[0].Value, enddate, enddate, 0));       //添加截止时刻值，使常数标签最终的返回结果，像一个普通概化标签
                            return results;
                        }
                        else
                            currentIndex += 1;
                            continue;
                    } while (currentYearIndex > APPConfig.psldata_startyear*100 + 1);
                    //如果全部循环完，仍然没有值，则要报错，返回空值
                    ErrorFlag = true;
                    ErrorInfo = "找不到有效的常数值，请检查常数配置表和常数值导入！";
                    return null;
                }//end 读常数标签
                //string readspan = readTimer.Elapsed.ToString();         
                return results;
            }
            catch (Exception ex)
            {        
                //标志位
                ErrorFlag = true;
                ErrorInfo = ex.ToString();
                //log
                string messageStr;
                messageStr = String.Format("DAO层PSLDataDAO.Read()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                //读取中发生任何错误，返回null。主引擎会进行错误计数
                return null;
            }
        }
        //psldata数据表数据读取接口：读取概化数据。读取多个标签的批量概化数据。tagids是概化标签id号。
        public static List<PValue>[] ReadMulti(System.UInt32[] tagids, DateTime startdate, DateTime enddate)
        {
            //一次性读取tagids所包含的所有标签的pvalue数据，然后进行组织，返回List<PValue>[]格式。            
            //——读接口根据starttime和endtime的年份和月份决定是单表查询还是多表联合查询。
            //——读接口的startdata和enddate是指数据点的starttime字段的起始和截止时间。
            //——读接口发生任何错误，均返回null，并置错误标志，交由主引擎处理。
            //——读接口取到的数据中，有可能存在状态位不为0的数据。由计算引擎交由算法自己进行过滤或做相应的处理。
            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr = "";
            try
            {
//var readTimer = Stopwatch.StartNew();          //计时器，用于测试
                
                //数据库连接
                DbHelper dbhelper = new DbHelper();
                IDataReader reader;
                //计算结果存储初始化
                List<PValue>[] resultsMulti = new List<PValue>[tagids.Length];
                for (int i = 0; i < tagids.Length; i++)
                {
                    resultsMulti[i] = new List<PValue>();       //特别注意，这里的处理，决定了如果对应的tagid在规定的周期内取不到值，则对应的位置不是null，而是count=0。这个与Read()接口不同。
                }
                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";
                string tablename = "";
                string readfields = "tagid,tagstarttime,tagendtime,tagvalue,tagstatus";

                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                  //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month - 1) / APPConfig.psldata_intervalmonth;    //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month - 1) / APPConfig.psldata_intervalmonth;         //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号     

                //根据tagids分别组织普通标签和常数标签。这里和单点读取不同。
                //根据tagids的顺序建立字典，以便按照id号组织计算结果List<PValue>[]
                Dictionary<System.UInt32, int> tagidsIndex = new Dictionary<System.UInt32, int>();  //标签id序号字典
                string tagidsStr = "";                                                              //普通标签可以一次读取出来
                List<System.UInt32> tagidsStr4Const = new List<System.UInt32>();                    //常数标签必须一个一个的读取
                for (int i = 0; i < tagids.Length; i++)         //组织标签id序号字典，组织普通标签字符串，组织常数标签数组
                {
                    //找tagid在tagids中的序号字典，以便根据tagid找到对应序号来组织计算结果
                    if (!tagidsIndex.ContainsKey(tagids[i])) {
                        tagidsIndex.Add(tagids[i], i);
                    }
                    
                    //根据tagid大小区别是普通实时标签还是常数标签
                    if (tagids[i] >= MAX_NUMBER_CONSTTAG)
                        tagidsStr += "'"+tagids[i] +"'" + ",";      //普通标签字符串                                  
                    else
                        tagidsStr4Const.Add(tagids[i]);             //常数标签列表         
                }
                if (tagidsStr != "") tagidsStr = tagidsStr.Substring(0, tagidsStr.Length - 1);      //普通标签字符串去掉结束的逗号 

                //根据普通标签字符串tagidsStr先读取普通标签数据******************************************************************************************************
                if (tagidsStr != "")
                { 
                    //如果起始时间和结束时间在同一年，则单表查询                
                    if (startYearIndex == endYearIndex)
                    {
                        tablename = tablepre + startYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——概化计算点全部是其他计算的结果，在结构设计中，要求任何计算无论是空值还是错误，都需要有值。因此概化数据，在起始时刻和截止时刻应该有值。如果没有值，则说明发生了不可控的错误。
                        //——基于以上规定，概化数据直接用 tagstarttime<='{3}'来取值。而且当在起始时刻和截止时刻没有值得时候，就仅返回内部值，而不需要再到两个时刻之外，去取值然后在时刻点插值。
                        sqlStr = String.Format("use {0};select {1} from {2} where tagid in({3}) and tagstarttime between '{4}' and '{5}'", 
                                                databasename,
                                                readfields,
                                                tablename, 
                                                tagidsStr, 
                                                startdate.Ticks, 
                                                enddate.Ticks
                                                );
                        //var sqlTimer = Stopwatch.StartNew();          //用于测试读取一个月分钟数据的sql执行所耗时长。一个月分钟数据45000条左右，在公司服务测试结果是耗时4ms-5ms            
                        reader = dbhelper.ExecuteReader(sqlStr);
                        //string sqlspan = sqlTimer.Elapsed.ToString();
                        //var transTimer = Stopwatch.StartNew();        //用于测试读取一个月分钟数据的转换格式所耗时长。一个月分钟数据45000条左右，耗时长300ms左右。改进后减少到200ms。       
                        IDataReader2PSLDataItemsMulti(reader, tagidsIndex, ref resultsMulti);
                        // string transspan = transTimer.Elapsed.ToString();
                        reader.Close();
                    }
                    //如果起始时间和结束时间不在同一年，则多表联合查询
                    else if (startYearIndex < endYearIndex)
                    {
                        tablename = tablepre + startYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                        //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                        sqlStr = String.Format("use {0};select {1} from {2} where tagid in({3}) and tagstarttime between '{4}' and '{5}'",
                                                databasename,   
                                                readfields,
                                                tablename, 
                                                tagidsStr, 
                                                startdate.Ticks, 
                                                enddate.Ticks
                                                );
                        //找起止时间段内包含的第一个分表后面的表      
                        int currentYearIndex;                        
                        int currentIndex=0;
                        do
                        {
                            //从startYearIndex开始，到endYearIndex一共有多少张表。
                            currentIndex = currentIndex + 1;
                            currentYearIndex = startYear100 + ((currentIndex + startIndex - 1) / maxIndexInYear) * 100 + 1 + (currentIndex + startIndex - 1) % maxIndexInYear;  //从startYearIndex不断向上得到新表的方法
                            tablename = tablepre + currentYearIndex.ToString();
                            //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                            //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                            //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                            //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                            sqlStr = sqlStr + String.Format("union select {0} from {1} where tagid in({2}) and tagstarttime between '{3}' and '{4}'",
                                                readfields,                
                                                tablename, 
                                                tagidsStr, 
                                                startdate.Ticks, 
                                                enddate.Ticks
                                                );

                        } while (currentYearIndex < endYearIndex);
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间                   
                        reader = dbhelper.ExecuteReader(sqlStr);
                        IDataReader2PSLDataItemsMulti(reader, tagidsIndex,ref resultsMulti);
                        reader.Close();
                    }
                    else
                    {
                        return null;
                    }

                    //对每个标签对应的List<PValue>进行处理
                    for (int i = 0; i < tagids.Length; i++)
                    {
                        //排序。由于并发计算引擎，计算的顺序不是按照时间的先后。写入的顺序也不是按照时间的先后。因此，如果读取的数据是并发引擎计算的。读取出的数据，也不是按照时间先后顺序排列的。
                        //var sortTimer = Stopwatch.StartNew();
                        if (resultsMulti[i] != null || resultsMulti[i].Count != 0)
                            resultsMulti[i] = resultsMulti[i].OrderBy(m => m.Timestamp).ToList();

                        //string sortspan = sortTimer.Elapsed.ToString();
                        //添加第一个点。                        
                        //——对于RDB来说，由于RDB已经是计算结果。其值都是在整点整刻。因此从数据库中找到的第一个值都是 tagstarttime={2}的那个值。因此无需对第一个点进行特殊处理。
                        //————在某些特殊情况下，在整点时刻的值，由于计算错误或者前置计算无值，其状态位为1.
                        //————在某些情况下，发生了不可控的错误，则会出现这一刻数据库没有响应的计算结果。出现这种错误，则只能去到起始时刻和截止时刻直接的存在的值。

                        //添加截止时刻点
                        if (resultsMulti[i] != null && resultsMulti[i].Count != 0 && resultsMulti[i][resultsMulti[i].Count - 1].Timestamp == enddate)
                        {
                            //如果返回数据包含结束时刻(sql语句是between，就是startdate<=xdate<=enddate)，因此返回的数据可能会包含Timestamp恰好在enddate出的点。概化数据正常情况下必返回此点。
                            resultsMulti[i][resultsMulti[i].Count - 1].Endtime = enddate;      //如果取到了结束时刻点的值，则修改结束点的Endtime与Timestamp一致。
                        }
                        else if (resultsMulti[i] != null && resultsMulti[i].Count != 0 && resultsMulti[i][resultsMulti[i].Count - 1].Timestamp < enddate)
                        {
                            //如果返回数据不包含结束时刻，则用最后一个有效点填充
                            resultsMulti[i].Add(new PValue(resultsMulti[i][resultsMulti[i].Count - 1].Value, enddate, enddate, 0));
                        }
                        else
                        {
                            //否则什么也不做
                        }
                    }//endfor                
                }
                //再读取常数标签***********************************************************************************************************************************************
                if (tagidsStr4Const.Count!=0)
                {
                    //常数标签需要一个个读取，每个常数标签仅返回一个数值加一个截止值                
                    foreach(System.UInt32 tagid in tagidsStr4Const)
                    {
                        //——读接口根据starttime，读取对应标签在starttime之前的最近一个有效值            
                        //——读接口发生任何错误，均返回null，并置错误标志。交易主引擎处理 
                        int i=0;
                        //获取当前pvalue的starttime的年份
                        int startYear = startdate.Year;
                        //如果年份小于概化数据表起始年份，则直接返回空数据
                        if (startYear < APPConfig.psldata_startyear)
                        {
                            //如果startYear小于数据库定义起始时间，则对应的常数标签没有有效值
                            //常数变量没有有效值不允许发生，无论是实时计算还是历史计算必须马上解决，因此发生这种情况要报error，并返回null。提醒使用这立即解决。
                            //如果常数没有有效值，则整个读取，不管其他tagid有没有值，都要返回Error，并置整个计算结果为空
                            ErrorFlag = true;
                            ErrorInfo = "找不到有效的常数值，请检查常数配置表和常数值导入！";
                            return null;
                        }
                        else
                        {
                            //如果年份大于等于概化数据表起始年份，则从后向前遍历，直到找到最后一个有效数据
                            int currentYearIndex;
                            int currentIndex = 0;
                            do
                            {
                                //maxIndexInYear-endIndex+currentIndex 当前分页currentIndex到enddate分页所在年份顶端分页maxIndexInYear的实际距离。
                                //(maxIndexInYear-endIndex+currentIndex )% maxIndexInYear 上面的实际距离折算到某一年份顶端分页maxIndexInYear的取余距离。
                                //maxIndexInYear-(maxIndexInYear-endIndex+currentIndex )% maxIndexInYear,currentIndex在具体年份内的实际分页数。
                                currentYearIndex = endYear100 - ((maxIndexInYear - endIndex + currentIndex) / maxIndexInYear) * 100 + maxIndexInYear - (maxIndexInYear - endIndex + currentIndex) % maxIndexInYear;
                                tablename = tablepre + currentYearIndex.ToString();
                                sqlStr = String.Format("use {0};select {1} from {2} where tagid='{3}' and tagstarttime=(select Max(tagstarttime) from {2} where tagid='{3}' and tagstarttime<='{4}')",
                                                        databasename,
                                                        readfields,
                                                        tablename,
                                                        tagid,
                                                        startdate.Ticks
                                                       );
                                reader = dbhelper.ExecuteReader(sqlStr);
                                List<PValue> beforestart = IDataReader2PSLDataItems(reader);        //上述sql语句一般返回一条记录，或者不返回记录
                                reader.Close();

                                if (beforestart.Count != 0)
                                {
                                    //注意，读取到的值，但是时间上不能用读取到的这个pvalue的时间，而应该以读取周期的时间来给定。这样这个常数值，才能看起来像一个计算周期内的普通pvalue值
                                    resultsMulti[tagidsIndex[tagid]].Add(new PValue(beforestart[0].Value, startdate, enddate, 0));
                                    resultsMulti[tagidsIndex[tagid]].Add(new PValue(beforestart[0].Value, enddate, enddate, 0));       //添加截止时刻值，使常数标签最终的返回结果，像一个普通概化标签                                    
                                    goto next;   //当前常数标签找到值，继续下一个常数标签
                                }
                                else
                                    currentIndex += 1;
                                
                            } while (currentYearIndex > APPConfig.psldata_startyear*100 + 1);
                            //如果任意一个常数标签全部循环完，仍然没有值，
                            //则要报错，无论其他tagid有么有值，整体返回空值
                            //常数变量没有有效值不允许发生，无论是实时计算还是历史计算必须马上解决，因此发生这种情况要报error，并返回null。提醒使用这立即解决。
                            ErrorFlag = true;
                            ErrorInfo = "找不到有效的常数值，请检查常数配置表和常数值导入！";
                            resultsMulti[tagidsIndex[tagid]] = null;                           

                        }//endif

                        next:
                        int temp;//这里是为了next标记有效
                    }//end for下一个常数标签
                }//end if 常数标签情况

                return resultsMulti;
            }
            catch (Exception ex)
            { 
                //标志位
                ErrorFlag = true;
                ErrorInfo = ex.ToString();
                //log
                string messageStr;
                messageStr = String.Format("DAO层PSLDataDAO.ReadMulti()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                //读取中发生任何错误，返回null。主引擎会进行错误计数
                return null;
            }
        }
        //psldata数据表数据删除接口：删除概化数据。删除单个标签一段时间内的概化数据。tagid是概化标签id号。
        public static int Delete(System.UInt32[] tagids, DateTime startdate, DateTime enddate)
        {
            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr = "";
            try
            {               
                //数据库连接
                DbHelper dbhelper = new DbHelper();
                //计算结果存储初始化
                int results = 0;

                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";
                string tablename = "";

                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                      //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month - 1) / APPConfig.psldata_intervalmonth;   //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month - 1) / APPConfig.psldata_intervalmonth;       //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号 
                
                //组织id字符串，注意这里没有常数标签
                string idsStr = "";
                for (int i = 0; i < tagids.Length; i++)
                {
                    idsStr = idsStr + tagids[i] + ",";
                }
                idsStr = idsStr.Substring(0, idsStr.Length - 1);

                //依次从每张表中删除id字符串
                int currentYearIndex;                    
                int currentIndex=0;
                int deletecount=0;
                sqlStr =String.Format("use {0}",databasename);
                do
                {
                    currentYearIndex = startYear100 + ((currentIndex + startIndex - 1) / maxIndexInYear) * 100 + 1 + (currentIndex + startIndex - 1) % maxIndexInYear;  //从startYearIndex不断向上得到新表的方法
                    tablename = tablepre + currentYearIndex.ToString();
                    //特别注意，一段时间的统计量，其计算结果的tagstarttime存的是这个时间段的开始时间。
                    //因此当我要重新计算一段时间内的统计量时，应该删除那些tagstarttime大于等于开始时间，tagstarttime小于结束时间的值。
                    sqlStr = sqlStr + ";" + String.Format("delete from {0} where tagid in ({1}) and tagstarttime between {2} and {3}",                               
                                tablename,
                                idsStr,
                                startdate.Ticks,
                                enddate.Ticks
                                );                    
                    currentIndex = currentIndex + 1;
                } while (currentYearIndex < endYearIndex);
                
                //特别注意大量删除记录后，（这里一个id一年的概化数据为1w条数据），是否要重新设定自增id，以避免id断层
                deletecount = dbhelper.ExecuteNonQuery(sqlStr);
                results = results + deletecount;
                return results;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLDataDAO.Delete()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return -1;
            }
        }

        public static BackgroundWorker worker;      //给UniqeCheck，用于更新进度条界面
        //psldata数据表数据唯一性检查接口：检查某个标签是否存在重复数据，即某个标签在一个Timestamp下，是否存在多个数据
        public static List<PValue> UniqeCheck(System.UInt32 tagid, DateTime startdate, DateTime enddate)
        {

            ErrorFlag = false;
            ErrorInfo = "";
            string sqlStr = "";
            
            try
            {
                //数据库连接
                DbHelper dbhelper = new DbHelper();
                IDataReader reader;
                //计算结果
                List<PValue> results = new List<PValue>();
                //数据库及数据表
                string databasename = "psldb";
                string tablepre = "psldata";
                string readfields = "tagid,tagstarttime,tagendtime,tagvalue,tagstatus";
                string tablename = "";

                //数据时间到数据表转换所需变量        
                int maxIndexInYear = 12 / APPConfig.psldata_intervalmonth;                      //按当前间隔划分，一年时间中分表的最大序号。比如间隔是6个月，序号就是1~2。最大序号就是2。间隔是3个月，序号就是1~4，最大序号就是4。                
                int startYear100 = startdate.Year * 100;                                        //起始时间对应的数据表的年份×100
                int startIndex = 1 + (startdate.Month - 1) / APPConfig.psldata_intervalmonth;    //起始时间对应的数据表的分表序号
                int startYearIndex = startYear100 + startIndex;                                 //起始时间折算出的所属数据表的名称， 数据表的年份×100+分表序号             
                int endYear100 = enddate.Year * 100;                                            //截止时间对应的数据表的年份×100
                int endIndex = 1 + (enddate.Month - 1) / APPConfig.psldata_intervalmonth;         //截止时间对应的数据表的分表序号       
                int endYearIndex = endYear100 + endIndex;                                       //截止时间折算出的所属数据表的名称， 数据表的年份×100+分表序号   

                if (tagid >= MAX_NUMBER_CONSTTAG)       //tagid > MAX_NUMBER_CONSTTAG 读取的是正常概化标签。仅检查正常概化标签。不检查常数标签
                { 
                    //如果起始时间和结束时间在同一年，则单表查询                
                    if (startYearIndex == endYearIndex)
                    {
                        tablename = tablepre + startYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——概化计算点全部是其他计算的结果，在结构设计中，要求任何计算无论是空值还是错误，都需要有值。因此概化数据，在起始时刻和截止时刻应该有值。入股没有值，则说明发生了不可控的错误。
                        //——基于以上规定，概化数据直接用 tagstarttime<='{3}'来取值。而且当在起始时刻和截止时刻没有值得时候，就仅返回内部值，而不需要再到两个时刻之外，去取值然后在时刻点插值。

                        sqlStr = String.Format("use {0};select {1} from {2} where tagid='{3}' and tagstarttime>='{4}' and tagstarttime<='{5}' group by tagstarttime having count(*)>1", 
                                            databasename,
                                            readfields,
                                            tablename, 
                                            tagid, 
                                            startdate.Ticks, 
                                            enddate.Ticks
                                            );
                        //var sqlTimer = Stopwatch.StartNew();          //用于测试读取一个月分钟数据的sql执行所耗时长。一个月分钟数据45000条左右，在公司服务测试结果是耗时4ms-5ms            
                        reader = dbhelper.ExecuteReader(sqlStr);
                        //string sqlspan = sqlTimer.Elapsed.ToString();
                        //var transTimer = Stopwatch.StartNew();        //用于测试读取一个月分钟数据的转换格式所耗时长。一个月分钟数据45000条左右，耗时长300ms左右。改进后减少到200ms。       
                        results = IDataReader2PSLDataItems(reader);
                        // string transspan = transTimer.Elapsed.ToString();
                        reader.Close();
                    }
                    //如果起始时间和结束时间不在同一年，则多表联合查询
                    else if (startYearIndex < endYearIndex)
                    {
                        tablename = tablepre + startYearIndex.ToString();
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                        //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                        //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                        //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                        sqlStr = String.Format("use {0};select {1} from {2} where tagid='{3}' and tagstarttime>='{4}' and tagstarttime<='{5}' group by tagstarttime having count(*)>1", 
                                            databasename,
                                            readfields,
                                            tablename, 
                                            tagid, 
                                            startdate.Ticks, 
                                            enddate.Ticks);
                        //找起止时间段内包含的第一个分表后面的表                        
                        int currentYearIndex;                        
                        int currentIndex=0;
                        do
                        {   //从startYearIndex开始，到endYearIndex一共有多少张表。
                            currentIndex = currentIndex + 1;
                            currentYearIndex = startYear100 + ((currentIndex + startIndex - 1) / maxIndexInYear) * 100 + 1 + (currentIndex + startIndex - 1) % maxIndexInYear;  //从startYearIndex不断向上得到新表的方法
                            tablename = tablepre + currentYearIndex.ToString();
                            //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间
                            //——目前要求返回的数据的特点和实时数据一样，即在起始时刻点必须有值，在截止时刻点也必须有值。截止时刻点的值在梯形计算中不用，在线性计算中，要用于最后一段的插值。
                            //——但是截至时刻的值，不能直接用 tagstarttime<='{3}'来取。因为，如果在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，就应该返回空值。而不需要添加截止值。
                            //——直接用 tagstarttime<='{3}'来取，可能在tagstarttime>='{2}' and tagstarttime<'{3}'内没有值，但是确把截止值取进来。
                            sqlStr = sqlStr + String.Format(" union select {0} from {1} where tagid='{2}' and tagstarttime>='{3}' and tagstarttime<='{4}' group by tagstarttime having count(*)>1", 
                                                        readfields,
                                                        tablename, 
                                                        tagid, 
                                                        startdate.Ticks, 
                                                        enddate.Ticks);

                        } while (currentYearIndex < endYearIndex);  //循环到currentYearIndex >或者=endYearIndex时推出。正常应该是currentYearIndex=endYearIndex就说明添加到最后一个，就应该退出了。
                        //特别注意，接口的startdata和enddate是指数据点的starttime字段的起始和截止时间                   
                        reader = dbhelper.ExecuteReader(sqlStr);
                        results = IDataReader2PSLDataItems(reader);
                        reader.Close();                      
                    }
                    else
                    {
                        return null;
                    }
                                        
                    //排序。由于并发计算引擎，计算的顺序不是按照时间的先后。写入的顺序也不是按照时间的先后。因此，如果读取的数据是并发引擎计算的。读取出的数据，也不是按照时间先后顺序排列的。                    
                    if (results != null || results.Count != 0)
                        results = results.OrderBy(m => m.Timestamp).ToList();


                    return results;
                }//只检查计算结果标签，不检查常数标签
                return null;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLDataDAO.UniqeCheck()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        #endregion

        #region 辅助函数
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集
        private static List<PValue> IDataReader2PSLDataItems(IDataReader reader)
        {
    //var transTimer = Stopwatch.StartNew();
            List<PValue> items = new List<PValue>();
            while (reader.Read())
            {
                PValue item = new PValue();
                //DAOSupport.ReaderToObject(reader, item);   //20180711,经测试，在读取1个月的分钟概化数据时，返回数据在45000条，使用该 ReaderToObject转换程序，IDataReader2PSLDataItems整体耗时300ms。效率较低。
                //DAOSupport.ReaderToObject是一个通用格式的转换程序。在单条数据转换时，会去循环读取。在对效率要求比较高的psldataXXXX数据库的数据进行转换时，需要使用专用的转换程序，以便保证效率。
                //在psldata表的读写中，由于数据量大，要提高读写效率，需要注意不要使用反射
                //关于反射效率的问题，在这个地址的帖子中，有所讨论。https://bbs.csdn.net/topics/391910616
                //关于IDataReader的速度，在这个地址的帖子中，有所讨论。https://blog.csdn.net/lilong_herry/article/details/79993907
                
                //*************************getvalue读法
                //try{item.tagid = Convert.ToUInt32(reader.GetValue(0));}catch{item.tagid=0;};
                //try{item.tagstarttime = Convert.ToInt64(reader.GetValue(1));}catch{item.tagstarttime=0;};
                //try{item.tagendtime = Convert.ToInt64(reader.GetValue(2));}catch{item.tagendtime=0;};
                //try{item.tagvalue = Convert.ToDouble(reader.GetValue(3));}catch{item.tagvalue=0;};
                //try{item.tagstatus = Convert.ToInt64(reader.GetValue(4)); }catch{item.tagstatus = 0; };
                

                //*************************reader[]读法 
                try { item.Tagid = Convert.ToInt32(reader["tagid"]);}
                catch { };
                try { item.Timestamp =  new DateTime(Convert.ToInt64(reader["tagstarttime"])); }
                catch {};
                try { item.Endtime = new DateTime(Convert.ToInt64(reader["tagendtime"])); }
                catch {  };
                try { item.Value = Convert.ToDouble(reader["tagvalue"]); }
                catch { item.Value = 0; };
                try { item.Status = Convert.ToInt64(reader["tagstatus"]); }
                catch { item.Status = 0; };

                //*************************reader[]读法，不带try
                //item.tagid = Convert.ToUInt32(reader["tagid"]); 
                //item.tagstarttime = Convert.ToInt64(reader["tagstarttime"]); 
                //item.tagendtime = Convert.ToInt64(reader["tagendtime"]); 
                //item.tagvalue = Convert.ToDouble(reader["tagvalue"]); 
                //item.tagstatus = Convert.ToInt64(reader["tagstatus"]); 
                
                items.Add(item);
            }
    //string transspan = transTimer.Elapsed.ToString();
            return items;
        }
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集
        private static void IDataReader2PSLDataItemsMulti(IDataReader reader, Dictionary<System.UInt32, int> tagidsIndex, ref List<PValue>[] resultsMulti)
        {
            var transTimer = Stopwatch.StartNew();
           
            while (reader.Read())
            {
                PValue item = new PValue();
                System.UInt32 tagid=0;
                //DAOSupport.ReaderToObject(reader, item);   //20180711,经测试，在读取1个月的分钟概化数据时，返回数据在45000条，使用该 ReaderToObject转换程序，IDataReader2PSLDataItems整体耗时300ms。效率较低。
                //DAOSupport.ReaderToObject是一个通用格式的转换程序。在单条数据转换时，会去循环读取。在对效率要求比较高的psldataXXXX数据库的数据进行转换时，需要使用专用的转换程序，以便保证效率。
                //在psldata表的读写中，由于数据量大，要提高读写效率，需要注意不要使用反射
                //关于反射效率的问题，在这个地址的帖子中，有所讨论。https://bbs.csdn.net/topics/391910616
                //关于IDataReader的速度，在这个地址的帖子中，有所讨论。https://blog.csdn.net/lilong_herry/article/details/79993907

                //*************************getvalue读法
                //try{item.tagid = Convert.ToUInt32(reader.GetValue(0));}catch{item.tagid=0;};
                //try{item.tagstarttime = Convert.ToInt64(reader.GetValue(1));}catch{item.tagstarttime=0;};
                //try{item.tagendtime = Convert.ToInt64(reader.GetValue(2));}catch{item.tagendtime=0;};
                //try{item.tagvalue = Convert.ToDouble(reader.GetValue(3));}catch{item.tagvalue=0;};
                //try{item.tagstatus = Convert.ToInt64(reader.GetValue(4)); }catch{item.tagstatus = 0; };


                //*************************reader[]读法    
                tagid = Convert.ToUInt32(reader["tagid"]); 
                
                try { item.Timestamp = new DateTime(Convert.ToInt64(reader["tagstarttime"])); }
                catch { };
                try { item.Endtime = new DateTime(Convert.ToInt64(reader["tagendtime"])); }
                catch { };
                try { item.Value = Convert.ToDouble(reader["tagvalue"]); }
                catch { item.Value = 0; };
                try { item.Status = Convert.ToInt64(reader["tagstatus"]); }
                catch { item.Status = 0; };

                //*************************reader[]读法，不带try
                //item.tagid = Convert.ToUInt32(reader["tagid"]); 
                //item.tagstarttime = Convert.ToInt64(reader["tagstarttime"]); 
                //item.tagendtime = Convert.ToInt64(reader["tagendtime"]); 
                //item.tagvalue = Convert.ToDouble(reader["tagvalue"]); 
                //item.tagstatus = Convert.ToInt64(reader["tagstatus"]); 

                resultsMulti[tagidsIndex[tagid]].Add(item);
               
            }
            string transspan = transTimer.Elapsed.ToString();
            return;
        }
        //SQL数据库返回结果类型转换：由IDataReader中读取时间
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

        #endregion
    }
}
