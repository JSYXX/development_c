using System;
using System.Collections.Generic;       //使用List
using PCCommon;                         //使用PValue
using System.Diagnostics;               //使用计时器
using System.Threading;                 //使用线程
using System.Threading.Tasks;           //使用任务
using System.Collections.Concurrent;    //使用并发集合
using System.Text.RegularExpressions;   //使用正则表达式
using System.Reflection;                //使用反射
using PSLCalcu.Module;                  //使用计算模块
using Config;                           //使用Config配置
using DBInterface.RTDBInterface;        //使用实时数据库接口
using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Linq;                      //使用list的orderby，测试用

namespace PSLCalcu
{   
    /// <summary>
    /// HistoryParallelCalcuEngine
    /// 历史数据并发计算引擎。
    /// 
    ///——在选定的要进行历史计算的配置对象集SelectedCalcuItems中，对于可以组织在一起进行一次并发计算的对象集currentCalcuItems，在指定的时间段内进行并发计算（读、算、写）
    //
    ///讨论：大方向上，首先考虑是仅在单项计算项上实现并发计算，还是在多项需要数据相同的计算项上实现同时并发。
    ///——比如，对于同一个实时数据标签，同样的计算周期配置了十种算法。
    ///——如果一项一项的并发，每一项一次取一天数据（实时数据按天并发），算一年需要取365次数据。按照典型的PGIM取数据时间，需要200ms。一年数据需要200*365=73秒。10项计算需要=730秒~12分钟
    ///——如果，每取一次数据，十项一起算，则算一年数据只需要一分钟。
    ///结论，对于可以多项计算项并发的一定要进行多项并发。对于不能进行多项并发的，则进行单项计算并发。

    ///对于可以组织在一起进行多项并发计算的对象集要求（一次同时计算多个计算项）：
    ///——1、前提条件：需要currentCalcuItems.intervalseconds（计算项目的计算周期）小于ParallelPeriod（并发计算周期），并且能够被ParallelPeriod（并发计算周期秒数）整除
    ///——2、没有计算条件标签。
    ///——3、源标签相同，计算周期相同。
    ///不满足上述条件的，一律进行单个计算项的并发计算
    ///——入参：计算引擎当次计算对象集currentCalcuItems
    ///——入参：计算引擎当次计算时间段StartDateForHistoryCalcu，EndDateForHistoryCalcu
    ///——并发任务共享变量：实时数据存放变量，previousReadDataResults，currentReadDataResults
    ///——并发任务共享变量：条件数据存放变量，previousCondReadDataResults，currentCondReadDataResults
    ///——并发任务共享变量：计算结果存放变量，previousCalcuResults，currentResultsDic（currentCalcuResults已经不用）
    /// 
    /// 版本：1.1
    ///    
    /// 修改纪录
    ///     2017.07.28 版本：1.1 改为类封装。
    ///		2017.06.16 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2017.09.05</date>
    /// </author> 
    /// </summary>
    public class HistoryParallelCalcuEngine 
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(HistoryParallelCalcuEngine));       //全局log
        private long _errorCount;
        public long errorCount { get { return _errorCount; } set { this._errorCount = value; } }          //历史数据计算错误统计。long类型，不是引用类型，不能被lock。应使用interlock操作。
        private bool _errorFlag;
        public bool errorFlag { get { return _errorFlag; } set { this._errorFlag = value;}}       //历史数据计算错误标志。
        private long _warningCount;
        public long warningCount { get { return _warningCount; } set { this._warningCount = value; } }         //历史数据计算警告统计。long类型，不是引用类型，不能被lock。应使用interlock操作。
        private bool _warningFlag;
        public bool warningFlag { get { return _errorFlag; } set { this.warningFlag = value; } }     //历史数据计算警告标志。

        public bool IsParallelCalcu { get; set; }                       //是否并发计算
        public int ParallelPeriod   { get; set; }                       //并发计算的并发周期。秒值。即一次取多长时间的实时数据进行并发

        private List<PValue>[][] previousReadDataResults;               //上一天的实时数据：并发计算时，仅从该变量读取实时数据。
        private List<PValue>[][] currentReadDataResults;                //当天的实时数据：并发读取实时数据后写入该变量。虽然并发，但是采用在线程内加锁的方式来控制。
        private List<PValue>[] previousCondReadDataResults;           //上一天的条件数据：并发计算时，仅从该变量读取条件数据（关系库）。
        private List<PValue>[] currentCondReadDataResults;            //当天的条件数据：并发读取条件数据（关系库）。虽然并发，但是采用在线程内加锁的方式来控制。
        private string[] previousCalcuResults;                          //上一天的计算结果：写计算结果到数据库的程序，从该变量读取数据，组合成字符串，用一条sql一次写入。非并发程序。
        
        //private ConcurrentBag<string> currentCalcuResults;            //当天的计算结果:并发线程会向其写数。采用不带锁的并发集合，以便提高效率。已经被currentResultsDic替代
        private ConcurrentDictionary<string, string> currentResultsDic; //当天计算结果字典:并发线程会向其写数。采用不带锁的并发集合，以便提高效率。用来判断同一个id，同一时刻是否有重复值
        private List<PValue>[] currentResultsPValue;   //用于测试观察计算结果

        public event Action<int,string> ProcessUpdateAction;            //观察者模式下，通知被观察者的通知事件。这里用于通知观察者并发计算执行的百分比情况
        public event Action<long,string> WarningUpdateAction;           //观察者模式下，通知被观察者的通知事件。这里用于通知观察这并发计算报警计数更新
        public event Action<long,string> ErrorUpdateAction;             //观察者模式下，通知被观察者的通知事件。这里用于通知观察这并发计算错误计数更新

        //入参：currentCalcuItems：当次选定的可以在组织在一起进行一次并发计算的对象集
        //入参：StartDateForHistoryCalcu，EndDateForHistoryCalcu：历史计算的时间区间
        public void MainHistoryParallelCalcu(List<PSLCalcuItem> currentCalcuItems,DateTime StartDateForHistoryCalcu,DateTime EndDateForHistoryCalcu)
        {
            //计时，用于并发计算调试的时间测试。
            //System.Diagnostics.Debug类提供的方法只会在调试版本运行，在正式编译的程序中不运行。
            //Debug内容在output窗口中显示，是并发计算的主要调试手段。
            var swSingleParallel = Stopwatch.StartNew();

            //0、计算起始时间调整。实际的计算起始时间，由界面设定的年月日，和计算对象的时分秒构成。
            var startSecond = currentCalcuItems[0].fstarttime.Second;       //获取计算项计算起始时间中的秒
            var startMinute = currentCalcuItems[0].fstarttime.Minute;       //获取计算项计算起始时间中的分钟
            var startHour = currentCalcuItems[0].fstarttime.Hour;           //获取计算项计算起始时间中的小时

            //前期准备
            //1、删除选定计算项在要计算的时间区间内在数据仓库里已经存在的计算结果
            //——计算项在startIndex, endIndex之间，在StartDateForHistoryCalcu, EndDateForHistoryCalcu时间范围内，包含的的所有outputtagid记录全部删除。
            DateTime startdateDelete = StartDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
            DateTime enddateDelete = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
            bool deleteflag = DeletePSLData(currentCalcuItems, startdateDelete, enddateDelete);
            Debug.WriteLine("delete all existing data complete:" + swSingleParallel.Elapsed.ToString());
            if (!deleteflag) 
            {
                _errorCount = _errorCount + 1;
                //更新UI
                this.ErrorUpdateAction(this._errorCount,"");
                //记录log
                string errInfo;
                errInfo = string.Format("并发计算引擎错误{0}：删除数据错误!", this._errorCount.ToString());
                logHelper.Error(errInfo);
                errInfo = string.Format("——并发计算项的序号是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItems[0].fid, currentCalcuItems[currentCalcuItems.Count-1].fid,StartDateForHistoryCalcu.ToString(), EndDateForHistoryCalcu.ToString());
                logHelper.Error(errInfo); 
                goto NEXTCalcu;   
            }

            //2、为并发计算准备参数
            int ParallelPeriodDays = ParallelPeriod / (60 * 60 * 24);                                                   //并发计算的的并发周期，这里用日。如果是实时数据并发（rtdb、opc）一般是1天。概化数据一般是30天。
            int totalSpans = (int)Math.Ceiling((double)EndDateForHistoryCalcu.Subtract(StartDateForHistoryCalcu).Days/(double)ParallelPeriodDays);       //按并发周期分割，计算时间区间的分割段数。
   
            //2.1、为ParallelReadRealData准备参数                    
            int intervalseconds = currentCalcuItems[0].intervalseconds;    //获取当前计算项（如果是一组，则是第一项）的实时计算要求的计算周期
            string[] sourcetagnames = Regex.Split(currentCalcuItems[0].sourcetagname, ";|；|，|,");
            this.currentReadDataResults = new List<PValue>[sourcetagnames.Length][];              //根据计算项的源标签数量对当天实时数据读取变量进行初始化

            //2.2、为ParallelReadCondData准备参数:一次读出所有需要条件数据
            //注意，每一个带condpsltagid的计算项，当前项单独走并发计算。不会与其他计算项一起进行并发计算
            //待完成
            //this.currentCondReadDataResults = new List<PValue>();

            //2.3、为ParallelCalcu准备参数。  ParallelCalcu当前计算，使用上一次读取的源数据previousReadDataResults和条件数据previousCondReadDataResults计算          
            this.previousReadDataResults = null;                                    //上一次读取数据的结果变量初始化。ParallelCalcu使用previousDayRealDataResulte中的数据计算，如果为空则跳过    
            this.previousCondReadDataResults = null;                                //上一次读取条件数据初始化。
            //this.currentCalcuResults = new ConcurrentBag<string>();               //当前计算结果初始化，已经被currentResultsDic替代                            
            this.currentResultsDic = new ConcurrentDictionary<string, string>();    //当前计算结果字典
            this.currentResultsPValue = new List<PValue>[currentCalcuItems[0].foutputnumber];
            for (int inumber = 0; inumber < this.currentResultsPValue.Length; inumber++)
            {
                this.currentResultsPValue[inumber] = new List<PValue>();
            }

            //2.4、为WritePSLData准备参数
            this.previousCalcuResults = null;                           //WritePSLData使用previousCalcuResults向数据库写入数据                

            
            //3、在历史数据计算起始时间和截止时间的时间段内，按照一次取一个并发计算周期ParallelPeriod4RTD的数据进行并发计算的方法，进行并发计算           
            //关于循环次数
            //——读算写三个阶段并发进行，但是是流水作业。第一个周期，只有取数。第二个周期，取第二阶段的数据，算第一阶段的数据。第三个周期取第三个阶段的数据，算第二阶段数据，存第一阶段的数据。
            //因此实际循环次数，要刚好比分段数量多2次。
            
            

            for (int spanIndex = 0; spanIndex < totalSpans + 2; spanIndex ++)   //这里多循环两次，可以保证能将正常选定日期最后一天的数据计算完，并写入数据库
            {
                var sw = Stopwatch.StartNew();
                             
                //准备每一次并发计算需要数据的时间段。
                //——该并发数据时间段的时间，年月日用历史计算起始时间和并发计算取数周期来推算。
                //——小时分秒，用计算项的设定来计算。这是因为计算项并不一定是在整时、整分、整秒来计算的。比如带值次条件的计算，计算周期为1d。但是起始时间可能是2016年1月1日 2:00。
                //那每次取一个月的数据，应该是2016年1月1日 2:00 到 2016年1月30日 2:00。
                DateTime startdate = StartDateForHistoryCalcu.AddDays(spanIndex * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                DateTime enddate = StartDateForHistoryCalcu.AddDays((spanIndex +1)* ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                DateTime previousstartdate = StartDateForHistoryCalcu.AddDays((spanIndex-1) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                DateTime previousenddate = StartDateForHistoryCalcu.AddDays((spanIndex) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                DateTime beforestartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 2) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                DateTime beforeenddate = StartDateForHistoryCalcu.AddDays((spanIndex-1) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);

                //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据。
                //——但是同时保留原循环周期的小时，分钟，秒的信息。比如值此计算，如果循环周期是每天2:00到第二天2:00。当最后一个月取值时，就不会截止到0:00
                if (startdate > EndDateForHistoryCalcu) startdate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);                    
                if (enddate > EndDateForHistoryCalcu) enddate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);                        //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据
                if (previousstartdate > EndDateForHistoryCalcu) previousstartdate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond); 
                if (previousenddate > EndDateForHistoryCalcu) previousenddate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond); 

                Debug.WriteLine("for i=" + spanIndex + " has started: startdate is {0}...enddate is {1}..............",startdate.ToString(),enddate.ToString());  

                //读、算、写共四个任务并行执行
                var taskReadRealData = Task.Factory.StartNew(() => ParallelReadRealData(currentCalcuItems, startdate, enddate, intervalseconds));   //读取下一天计算需要的实时数据：结果放入全局变量currentReadDataResults
                var taskReadCondData = Task.Factory.StartNew(() => ParallelReadCondData(currentCalcuItems, startdate, enddate, intervalseconds));   //读取下一条计算需要的条件数据：结果放入全局变量currentCondReadDataResults
                var taskDoParallelCalcu = Task.Factory.StartNew(() => ParallelCalcu(currentCalcuItems, previousstartdate, previousenddate, intervalseconds, spanIndex, totalSpans));       //计算当前天的结果：根据previousReadDataResults、previousCondReadDataResults计算，结果放入全局变量currentResultsDic
                var taskWritePSLData = Task.Factory.StartNew(() => WritePSLData(beforestartdate, beforeenddate));                                                        //写入上一天计算结果：将previousCalcuResults写入数据库
                Task.WaitAll(taskReadRealData, taskReadCondData, taskDoParallelCalcu, taskWritePSLData);

                //读、算、写共四个任务串行执行
                /*
                ParallelReadRealData(currentCalcuItems,startdate, enddate, intervalseconds);
                ParallelReadCondData(currentCalcuItems,startdate, enddate, intervalseconds);
                ParallelCalcu(currentCalcuItems, intervalseconds);
                WritePSLData(StartDate.AddDays(tablename);
                */

                Debug.WriteLine("for i=" + spanIndex + " complete:" + sw.Elapsed.ToString());

                //当前并发任务结束后的处理
                //1、处理读取结果
                //——公式支持多输入，每个输入对应currentReadDataResults的一个元素对象
                //——当输入中有任何一个标签在对应时间段内（一天）的值为空值时，则当前所有输入无效，不进行计算
                if (this.currentReadDataResults != null && Array.IndexOf(this.currentReadDataResults, null) == -1)
                {
                    this.previousReadDataResults = ListTrans(this.currentReadDataResults);        //将刚读出的数组currentReadDataResults转置后放入previousDayRealDataResulte。currentReadDataResults读出的是[点数][时间间隔数]的二维数组。需要转置为[时间间隔数][点数]
                }
                else
                {
                    this.previousReadDataResults = null;
                }
                this.currentReadDataResults = new List<PValue>[sourcetagnames.Length][];             //清空currentReadDataResults

                //2、处理条件数据读取结果
                if (this.currentCondReadDataResults != null)
                {
                    this.previousCondReadDataResults = this.currentCondReadDataResults;
                }
                else
                {
                    this.previousCondReadDataResults = null;
                }
                this.currentCondReadDataResults = null;

                //3、处理计算结果
                if (this.currentResultsDic != null && this.currentResultsDic.Count != 0)
                {
                    this.previousCalcuResults = new string[this.currentResultsDic.Count];   //按照当前计算结果currentResultsDic的长度取初始化previousCalcuResults，不初始化，下面不能赋值，会报错
                    this.currentResultsDic.Values.CopyTo(this.previousCalcuResults, 0);
                }
                else
                {
                    this.previousCalcuResults = null;
                }
                this.currentResultsDic = new ConcurrentDictionary<string, string>();
                //Thread.Sleep(50);

                //测试，用于观察测试结果。测试完成后，一定注释掉。包括并行计算中的lock(currentResultsPValue),这个lock开销非常大
                /*
                for (int inumber = 0; inumber < this.currentResultsPValue.Length; inumber++)
                {
                    this.currentResultsPValue[inumber] = this.currentResultsPValue[inumber].OrderBy(m => m.Timestamp).ToList();   //用于观察计算结果
                }
                //测试，观察完计算结果后，重新初始化
                for (int inumber = 0; inumber < this.currentResultsPValue.Length; inumber++)
                {
                    this.currentResultsPValue[inumber] = new List<PValue>();   
                }
                */

                //4、刷新界面：是否刷新界面，由窗口隐藏状态和界面选择决定  
                //外部通过给主题对象historycalcu添加观察者方法，historycalcu.ProcessUpdateAction += update; 从而把观察者的执行函数赋给代理ProcessUpdateAction。
                //这里调用代理，相当于调用观察者函数。比如观察者中有这样的更新UI的函数UpdateListView(startIndex, endIndex, String.Format("{0}%", spanIndex * 100 / (totalDays + 2))); 
                if (this.ProcessUpdateAction != null)
                {
                    this.ProcessUpdateAction((spanIndex+1) * 100 / (totalSpans + 2), "");
                }
               

            }//end for               
            
            
            Debug.WriteLine("*************CalcuItem " + currentCalcuItems[0].sourcetagname + " is complete:" + swSingleParallel.Elapsed.ToString()); 

            NEXTCalcu: 
            return;
        }            
        
        //删除已经存在的概化库历史数据
        private bool DeletePSLData(List<PSLCalcuItem> currentCalcuItems, DateTime startdate, DateTime enddate)
        {
            List<uint> tagids = new List<uint>();
            for (int i = 0; i < currentCalcuItems.Count; i++)
            {
                for (int j = 0; j < currentCalcuItems[i].foutputpsltagids.Length; j++)
                {
                    tagids.Add(currentCalcuItems[i].foutputpsltagids[j]);
                }
            }
            uint[] tagidsarray = tagids.ToArray();
            int deleteNumber= PSLDataDAO.Delete(tagidsarray, startdate, enddate);
            
            //DAO层发生错误时
            //——PSLDataDAO.Error为true
            //——PSLDataDAO.Delete()会记录log
            //——PSLDataDAO.Delete()返回-1，

            //在这个要根据Error，更新全局错误记录数
            if (PSLDataDAO.ErrorFlag)
            {
                this._errorCount = this._errorCount + 1;  //DeletePSLData不是另起的线程，对_errorCount无须原子操作
                return false;
            }

            return true;
        }
        //并发读取实时数据：保证currentCalcuItems中所有项的sourcetagname均相同
        private void ParallelReadRealData(List<PSLCalcuItem> currentCalcuItems, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            
            //——根据currentCalcuItems中的sourcetagname，startdate，enddate读取对应时间段内的数据。
            //——返回的结果放回全局变量currentReadDataResults。    
            //——currentReadDataResults的结构是，第一维对于当前计算配置多个标签。第二维对应每个标签数据按算法配置的实际周期对原始数据进行的分段。
            var sw1 = Stopwatch.StartNew();
            string[] sourcetagnames = Regex.Split(currentCalcuItems[0].sourcetagname, ";|；|，|,");
            System.UInt32[] sourcetagids = currentCalcuItems[0].fsourtagids;
            try
            { 
            //并行读实时库实时数据
            //——计算引擎支持多输入的公式计算，但是到20170705为止，还没有多输入的算法。因此目前已有的算法全部是单输入，也就是仅从实时数据库取一个标签的实时值。
            //——经过接口的单元测试，读取一个标签1天的数据需要120ms左右。
            //——在该程序中，将sourcetagnames设置为sourcetagnames = new string[1] { "test.DEMO_AI001"}，经测试，下面的并发读取Parallel.For，读取一个标签需要120ms左右的时间。
            //——在该程序中，将sourcetagnames设置为sourcetagnames = new string[2] { "test.DEMO_AI001", "test.DEMO_AI002" }，经测试，下面的并发读取Parallel.For，读取两个标签需要220ms左右的时间。
            //  并发读取达不到预期效果，需要进一步和golden沟通学习。1、是否和followingDayRealDataResulte写入有关。2、是否和服务器端有关。
            //——对于目前仅有一个变量的计算公式来说，该并发程序实际上仅相当于单线程，用时120ms。               
                Debug.WriteLine("--ParallelReadRealData has started ........" );
                //List<PValue> pvalues;                  //特别注意，由于读取概化数据库时，是并发读取，如果在这里设定变量，存在线程安全问题。多线程写同样的变量，测试中出现问题
                //int spanNumber ;                       //特别注意，由于读取概化数据库时，是并发读取，如果在这里设定变量，存在线程安全问题。多线程写同样的变量，测试中出现问题
                //List<PValue>[] pvaluesSpan ;
                if (startdate >= enddate)                //如果起始时间大于等于截止时间，直接给currentReadDataResults赋空值，退出。
                {
                    this.currentReadDataResults = null;
                    Debug.WriteLine("--ParallelReadRealData has skipped ........");
                    return;
                }
                switch (currentCalcuItems[0].sourcetagdb)
                {
                    case "rtdb":    //如果从实时数据库取数据，目前golden支持多线程，但是PGIM和PI不支持，因此只能顺序取数据
                        //初始化，每天一的List<PValue>大概需要15M空间
                        List<PValue> pvalues = new List<PValue>();
                        int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                        List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
                        for (int i = 0; i < sourcetagnames.Length; i++)
                        {
                            pvalues = RTDBDAO.ReadData(sourcetagnames[i], startdate, enddate);        //获取历史数据,100000该参数是一次读取数据的最大条数，建议改为xml文件
                            if (pvalues != null && pvalues.Count != 0)
                                pvaluesSpan = SpanPValues4rtdb(pvalues, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                            else
                                pvaluesSpan = null;
                            //lock (this.currentReadDataResults)
                            //{
                                this.currentReadDataResults[i] = pvaluesSpan;   //顺序取数据，无需对变量加锁
                            //}
                        }
                        break;
                    case "opc": 
                    case "rdb":
                    case "rdbset":  //如果从关系数据库取数据，目前使用的mysql和oracle都支持多线程，可以并发取数据
                        Parallel.For(0, sourcetagnames.Length, (int i) =>       //对sourcetagnames中的每一个tag，用一个并发的任务单独读取
                        {
                            //初始化，每天一的List<PValue>大概需要15M空间
                            List<PValue> pvaluesParallel = new List<PValue>();
                            int spanNumberParallel = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                            List<PValue>[] pvaluesSpanParallel = new List<PValue>[spanNumberParallel];
                            //读取数据
                            if (currentCalcuItems[0].sourcetagdb=="opc")
                                pvaluesParallel = OPCDAO.Read(sourcetagnames[i], startdate, enddate);
                            else if (currentCalcuItems[0].sourcetagdb == "rdb" || currentCalcuItems[0].sourcetagdb == "rdbset")
                                pvaluesParallel = PSLDataDAO.Read(sourcetagids[i], startdate, enddate);


                            if (pvaluesParallel != null && pvaluesParallel.Count != 0)
                                pvaluesSpanParallel = SpanPValues4rdb(pvaluesParallel, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                            else
                                pvaluesSpanParallel = null;                                
                           
                            //Parallel.For的每一个任务，负责一个tagid的读取，将处理结果反给全局变量
                            //并发写全局变量时，必须加锁，否则就要采用全局的并发集合
                            //在这里，由于概化计算引擎，虽然允许计算公式有多个输入，但是实际上目前仅有单输入公式，并且以后增加多输入公式的可能性不大。因此这里并不关注并发效率
                            //另外，虽然currentReadDataResults为数组，每个不同的线程操作的对象也不同，但是仍然需要加锁，否则就会出现意料不到的错误。
                            lock (this.currentReadDataResults)
                            {
                                this.currentReadDataResults[i] = pvaluesSpanParallel;
                            }
                            //每一个并发任务的完成时间
                            Debug.WriteLine("----ParallelReadRealData task " + i + " complete:" + sw1.Elapsed.ToString());
                        });//end Parallel.For
                        break;
                }
                Debug.WriteLine("--ParallelReadRealData has completed:" + sw1.Elapsed.ToString());
            }
            catch (Exception ex)
            {
                //更新计数器
                Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                errorFlag = true;
                //更新UI
                this.ErrorUpdateAction(_errorCount, "");                
                //更新Log
                string errInfo;
                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误!", this.errorCount.ToString());                
                logHelper.Error(errInfo);
                errInfo = string.Format("——计算错误详细信息：{0}。", ex.ToString());               
                logHelper.Error(errInfo);
                errInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                                currentCalcuItems[0].fid,
                                                                currentCalcuItems[0].fmodulename,
                                                                currentCalcuItems[0].sourcetagname,
                                                                startdate.ToString(),
                                                                enddate.ToString()
                                                                );
                logHelper.Error(errInfo);
            }
        }
        //并发读取条件数据
        private void ParallelReadCondData(List<PSLCalcuItem> currentCalcuItems, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //带计算条件的仅对单个计算对象进行并发
            //——根据pslcondnames，startdate，enddate读取对应时间段内的数据。
            //——返回的结果放回全局变量currentCondReadDataResults。
            var sw1 = Stopwatch.StartNew();
            uint[] condpsltagids = currentCalcuItems[0].fcondpsltagids;
            try
            {
                Debug.WriteLine("--ParallelReadCondData has started ........");
                
                //如果起始时间大于等于截止时间，直接给currentReadDataResults赋空值，退出。
                if (startdate >= enddate)                
                {
                    this.currentCondReadDataResults = null;
                    Debug.WriteLine("--ParallelReadCondData has skipped ........");
                    return;
                }
                 
                //读取条件数据，分两种情况处理，一种是条件数据大于1，需要参与逻辑表达式计算；一种是条件数据小于1不参加逻辑表达式计算
                if (condpsltagids == null || condpsltagids.Length == 0)
                {
                    this.currentCondReadDataResults = null;
                    Debug.WriteLine("--ParallelReadCondData has skipped ........");
                    return;
                }

                //最小时间段阈值，如果给了设定值，就用设定值，否则使用默认值。默认值可以在“配置”中配置
                string[] condExpressionStr = Regex.Split(currentCalcuItems[0].fcondexpression, ";|；|，|,");                
                int filterThreshold;
                if (condExpressionStr.Length > 1)
                    filterThreshold = int.Parse(condExpressionStr[1]);  //如果表达式给了最小时间段阈值设定值，就用设定值
                else
                    filterThreshold = APPConfig.CALCUMODULE_THRESHOLD;  //如果表达式不给定最小时间段阈值设定值，则采用默认值CALCUMODULE_THRESHOLD

                //仅有1个条件
                if (condpsltagids.Length == 1)//如果条件数量=1,则在关系库中直接的读取数据
                {
                    
                    List<PValue> spanserie = new List<PValue>();
                    int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                    List<PValue>[] filterSpan = new List<PValue>[spanNumber];

                    //根据fcondpsltagids读取各限值时间序列
                    spanserie = PSLDataDAO.Read(condpsltagids[0], startdate, enddate);

                    //去掉spanserie的截止时刻值
                    if (spanserie != null && spanserie.Count != 0) spanserie.RemoveAt(spanserie.Count - 1);

                    //将时间序列结果进行分段
                    if (spanserie != null && spanserie.Count != 0)
                        filterSpan = SpanPValues4SpanFilter(spanserie, startdate, enddate, currentCalcuItems[0].intervalseconds);
                    else
                        filterSpan = null;
                    //将分段结果付给全局变量
                    this.currentCondReadDataResults = filterSpan;
                }
                //有多个条件
                else if (condpsltagids.Length > 1)
                {
                    
                    int spanNumberParallel = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                    List<PValue>[] filterSpanParallel = new List<PValue>[spanNumberParallel];

                    //并发读取每一个条件的数据，放入到线程外变量spanserieParallel中
                    List<PValue>[] spanserieParallel = new List<PValue>[condpsltagids.Length];
                    Parallel.For(0, condpsltagids.Length, (int i) =>       //对sourcetagnames中的每一个tag，用一个并发的任务单独读取
                    {
                        //初始化，每天一的List<PValue>大概需要15M空间
                        List<PValue> pvaluesParallel = new List<PValue>();                       
                        //读取数据
                        pvaluesParallel = PSLDataDAO.Read(condpsltagids[i], startdate, enddate);
                        
                        //去掉pvaluesParallel的截止时刻。注意当某一个条件为null时，并不意味着最后的总条件为空。因为有可能最后是取反。
                        if (pvaluesParallel != null && pvaluesParallel.Count != 0) pvaluesParallel.RemoveAt(pvaluesParallel.Count - 1);
                        
                        //将读出的数据给到外部变量
                        lock (spanserieParallel)
                        {
                            spanserieParallel[i] = pvaluesParallel;
                        }
                        Debug.WriteLine("----ParallelReadCondData task " + i + " complete:" + sw1.Elapsed.ToString());
                    });

                    //按照时间逻辑表达式，对spanserieParallel进行时间逻辑运算                                       
                    ICondEvaluatable exp;
                    exp = new CondExpression(condExpressionStr[0]);
                    List<PValue> filterspan = exp.Evaluate(spanserieParallel);

                    //将逻辑运算的结果进行分段
                    filterSpanParallel = SpanPValues4SpanFilter(filterspan, startdate, enddate, currentCalcuItems[0].intervalseconds);

                    //将分段结果付给全局变量
                    if(filterSpanParallel==null || filterSpanParallel.Count() ==0)
                    {
                            this.currentCondReadDataResults=null;
                    }
                    else
                    {
                            this.currentCondReadDataResults = filterSpanParallel;
                    }
                }                   
               
                    
                Debug.WriteLine("--ParallelReadCondData has completed:" + sw1.Elapsed.ToString());          
            
            }
            catch (Exception ex)
            {
                //更新计数器
                Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                errorFlag = true;
                //更新UI
                this.ErrorUpdateAction(_errorCount, "");
                //更新Log
                string errInfo;
                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取条件数据ParallelReadCondData错误!", this.errorCount.ToString());
                logHelper.Error(errInfo);
                errInfo = string.Format("——计算错误详细信息：{0}。", ex.ToString());
                logHelper.Error(errInfo);
                errInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                                currentCalcuItems[0].fid,
                                                                currentCalcuItems[0].fmodulename,
                                                                currentCalcuItems[0].sourcetagname,
                                                                startdate.ToString(),
                                                                enddate.ToString()
                                                                );
                logHelper.Error(errInfo);
            }            
           
        }
        //并发计算：包括时间计算和公式计算两部分        
        private struct ParallelCalcuList
        {   
            //并发计算任务列表对象
            public int selectItemsIndex;
            public int spanIndex;
            public DateTime startdate;
            public DateTime enddate;
        }
        private void ParallelCalcu(List<PSLCalcuItem> currentCalcuItems, DateTime startdate, DateTime enddate, int intervalseconds,int spanIndex,int totalspan)
        {
            //——对于一天的数据，对于项拥有共同的tagid和intervalsecond的SelectedItems项进行并发计算
            //——SelectedItems的startIndex到endIndex项，当前并发计算要处理的计算项
            //——previousDayRealDataResulte是并发计算要处理的数据。该数据是List<PValue>[spanCount][inputNumber]二维数组，spanCount是1天的时间按计算间隔划分的时间段，inputNumber是计算公式对应的输入个数
            //——并发计算项是SelectedItems的startIndex到endIndex项，在1天划分的每个时间段spanCount上执行一遍算法的并发。
            var sw = Stopwatch.StartNew();
            Debug.WriteLine("--ParallelCalcu task has started ........");
            if (spanIndex == 0 || spanIndex == totalspan+1) return; //如果是第一个循环，此时还处于读取数据阶段。直接跳过。如果是最后一个循环，处于写最后一个结果阶段。跳过。
            if (this.previousReadDataResults == null || this.previousReadDataResults.Length==0)   //当前计算数据为空，直接跳过
            {
                Debug.WriteLine("--ParallelCalcu task has skipped ........");

                //在当前的计算结构下，实时数据会在起始时刻和截止时刻插值，概化数据应该在计算周期时刻位置均有待状态位的值，
                //因此，如果整个并发周期都没有数据，属于重大错误
                
                //更新计数器
                Interlocked.Increment(ref this._errorCount);//多线程下，采用原子操作。此处不能使用lock
                //更新UI
                this.ErrorUpdateAction(this._errorCount, "");
                //更新Log                           
                string errInfo;
                errInfo = string.Format("历史数据计算引擎错误{0}：整个并发周期读取数据为空，对应时间段内没有数据！", this.errorCount.ToString());
                logHelper.Error(errInfo);                
                errInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                    currentCalcuItems[0].fid,
                                                    currentCalcuItems[0].fmodulename,
                                                    currentCalcuItems[0].sourcetagname,
                                                    startdate,
                                                    enddate
                                        );
                logHelper.Error(errInfo);
               
                this.currentResultsDic = null;  //当前并发计算时间段内（比如对于实时数据的一天，或者对于概化数据的一个月），没有任何数据，也就没有任何计算结果
                return;
            }
           
            //准备并发任务表
            int spanCount = (int)enddate.Subtract(startdate).TotalSeconds / intervalseconds;    //一天时间被intervalseconds划分的时间段数量。这里必须能整除。但是为提高效率，能否整除，放在外部判断。不能整除的不适用并发计算优化。
            ParallelCalcuList[] parallelcalculist = new ParallelCalcuList[spanCount * currentCalcuItems.Count];  //根据SelectedItems被选的的计算公式数量和spanCount，初始化并发计算公式数组。
            int spanBlankBount = 0;

            int ipointcalculist = 0;
            //对于每一个并发周期（一天）内的计算，先准备这个周期内的并发算法列表
            //——并发算法就是：该并发周期内包含的所有算法SelectedItems[calcuitemStartIndex,calcuitemsEndIndex]，在0到spanCount-1个时间段上的计算。
            //_calculist数组的序号index和时间段有对应关系，spanIndex=index%spanCount
            //并发算法列表calculist顺序及其重要，这涉及到foreach分任务时，任务的平衡程度
            //如果是一个标签T，配置一个算法A，则list为，TA0，TA1，TA2....TA23。如果分成四段任务，每一个任务是，{TA0，TA1，TA2...TA5}、{TA6，TA7，TA8...TA11}、{TA12，TA13，TA14...TA17}、{TA18，TA19，TA20...TA21}
            //如果是一个标签T，配置两个算法A和B，则list为：TA0，TB0，TA1，TB1，TA2，TB2.....TA23,TB23。如果分成四段任务，每一个任务是，{TA0，TB0，TA1，TB1...TA5,TB5}，{TA6，TB6，TA7，TB7...TA11,TB11}
            //如果是一个标签，配置20个算法，则每一个任务分别是{TA0，TB0，TC0，TD0....TA5，TB5，TC5，TD5}...
            //可以，如果这么划分list，可以保证时间段是24段，cpu是4核的情况下，任务之间是平均分配的。
            for (int j = 0; j < spanCount; j++)
            {
                //previousReadDataResults对currentReadDataResults进行了转至，第一维是按计算周期的分段数，第二维对应不同输入点。

                //无论当前时间段的内容为不为空，都进行任务分配
                for (int iPointCurrentCalcuItem = 0; iPointCurrentCalcuItem < currentCalcuItems.Count; iPointCurrentCalcuItem++)
                {
                    parallelcalculist[ipointcalculist].selectItemsIndex = iPointCurrentCalcuItem;   //当前并发任务项，对应在currentCalcuItems中的计算项序号
                    parallelcalculist[ipointcalculist].spanIndex = j;                               //当前并发任务项，对应在并发时间段中的时间段序号
                    parallelcalculist[ipointcalculist].startdate = startdate.AddSeconds(j * intervalseconds);            //当前并发任务项起始时间，j表示第j段。第一个[0]表示计算公式输入参数第0标签，第二个[0]表示该标签第0个点
                    parallelcalculist[ipointcalculist].enddate = startdate.AddSeconds((j + 1) * intervalseconds);          //当前并发任务项截止时间
                    ipointcalculist = ipointcalculist + 1;
                }

            }

            //并发计算
            #region parallel.ForEach方式
            //使用分区器Partitioner分区,Partitioner.Create(0, parallelcalculist.Length, maxNumberEachPartion)
            //——从0到parallelcalculist.Length 进行分区，
            //——如果parallelcalculist.Length / (Environment.ProcessorCount-1) 可以整除，每个分区最多个parallelcalculist.Length / (Environment.ProcessorCount-1)元素，
            //——或者不能整除，每个分区最多个parallelcalculist.Length / (Environment.ProcessorCount-1)+1元素，
            //比如parallelcalculist.length为24，即计算24个小时的并发。
            //——就是从0到24进行分区，在四核处理器情况下，每个分区最多24/(4-1)=8个元素。
            //——Partitioner.Create(0, 24, 8)最后分区结果是[0,8},[8,16},[16,24},第一个分区有8个元素、第二个分区有8个元素，第三个分区有8
            //——内循环的计算范围是[0,7],[8,15],[16,23],第一个循环是8个元素，第二个循环是8个元素，第三个循环是8个元素。
            //比如，parallelcalculist.length为25，即计算25个小时的并发。    
            //——就是从0到24进行分区，在四核处理器情况下，每个分区最多25/(4-1)+1=9个元素。
            //——Partitioner.Create(0, 25, 9)最后分区结果是[0,9},[9,18},[18,25},第一个分区有9个元素、第二个分区有9个元素，第三个分区有7
            //——内循环的计算范围是[0,8],[9,17],[18,24],第一个循环是9个元素，第二个循环是9个元素，第三个循环是7个元素。
            
            int maxNumberEachPartion;
            if(parallelcalculist.Length % (Environment.ProcessorCount-1)==0)
                maxNumberEachPartion=(int)parallelcalculist.Length/(Environment.ProcessorCount-1);
            else
                maxNumberEachPartion = (int)(parallelcalculist.Length / (Environment.ProcessorCount - 1)) + 1;

            Parallel.ForEach(Partitioner.Create(0, parallelcalculist.Length - spanBlankBount, maxNumberEachPartion), range =>       
            {
                var sw1 = Stopwatch.StartNew();
               
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    try
                    {                       
                        //1.0 对具体时间分段的数据进行判断
                        if (this.previousReadDataResults[parallelcalculist[i].spanIndex] == null ||                     //整个分段为空
                            this.previousReadDataResults[parallelcalculist[i].spanIndex].Length == 0 ||                 //分段长度为0
                            Array.IndexOf(this.previousReadDataResults[parallelcalculist[i].spanIndex],null)!=-1        //分段里有数据为null
                            )   //当前计算数据为空，直接跳过
                        {
                            Debug.WriteLine("--ParallelCalcu task{0} has skipped ........",i);
                            //更新计数器
                            Interlocked.Increment(ref _warningCount);  //多线程下，采用原子操作。此处不能使用lock
                            _warningFlag = true;
                            //更新UI               
                            this.WarningUpdateAction(_warningCount, "");
                            //更新log
                            string warningInfo;
                            warningInfo = string.Format("历史数据计算引擎警告{0}：并发计算内部，分时间段读取数据为空，对应时间段内没有概化数据！", this.warningCount.ToString());
                            logHelper.Info(warningInfo);
                            warningInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。", 
                                                        currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid, 
                                                        currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                        currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,
                                                        parallelcalculist[i].startdate.ToString(),
                                                        parallelcalculist[i].enddate.ToString()
                                                        );
                            logHelper.Info(warningInfo);
                            //跳过计算过程
                            //this.currentResultsDic = null;      //这里仅仅是一天并发周期中的一个时间段，如果该时间段没有数据，则仅仅是该时间段没有计算结果。也就是不向currentResultsDic增加数据
                           
                            //continue;   数据为空，只记报警，不跳过
                        }
                        
                        

                        //获得一般属性或方法，要通过实例
                        //Object obj = assembly.CreateInstance(APPConfig.NAMESPACE_CALCUMODULE + "." + SelectedItems[calculist[i].selectItemsIndex].fmodulename);

                        //1.2、条件过滤
                        //使用限值时间序列对输出数据进行过滤    
                        //特别注意previousCondReadDataResults是在ParallelReadCondData()中已经处理好的计算条件时间段。
                        //计算条件时间段，在ParallelReadCondData中先分条件数量大于1或者等于1两种情况读取。然后合并。
                        //再将最终合并的有效时间段拆分到每个小周期内
                        //这里得到的previousCondReadDataResults是对用到每个计算周期内，真正有效的条件时间段

                        //获取filterThreshold
                        int filterThreshold;
                        string[] condExpressionStr = Regex.Split(currentCalcuItems[parallelcalculist[i].selectItemsIndex].fcondexpression, ";|；|，|,");
                        if (condExpressionStr.Length > 1)
                            filterThreshold = int.Parse(condExpressionStr[1]);  //如果表达式给了最小时间段阈值设定值，就用设定值
                        else
                            filterThreshold = APPConfig.CALCUMODULE_THRESHOLD;  //如果表达式不给定最小时间段阈值设定值，则采用默认值CALCUMODULE_THRESHOLD

                        //使用过滤条件对输入进行过滤
                        if (currentCalcuItems[parallelcalculist[i].selectItemsIndex].fcondpsltagids != null && currentCalcuItems[parallelcalculist[i].selectItemsIndex].fcondpsltagids.Length>0)
                        {
                            if (this.previousCondReadDataResults == null ||                                     //previousCondReadDataResults整体有可能本身就为空
                                this.previousCondReadDataResults[parallelcalculist[i].spanIndex] == null ||     //previousCondReadDataResults对应的当前时间段内为空
                                this.previousCondReadDataResults[parallelcalculist[i].spanIndex].Count == 0     //
                                )   //当前过滤时间为空，则经过过滤的输入值为空，直接以空值带入计算
                            {
                                Debug.WriteLine("--ParallelCalcu task{0} has skipped ........", i);
                                //更新计数器
                                Interlocked.Increment(ref _warningCount);  //多线程下，采用原子操作。此处不能使用lock
                                _warningFlag = true;
                                //更新UI               
                                this.WarningUpdateAction(_warningCount, "");
                                //更新log
                                string warningInfo;
                                warningInfo = string.Format("历史数据计算引擎警告{0}：分时间段过滤条件时间序列为空，对应时间段内没有符合条件的过滤时间段！", this.warningCount.ToString());
                                logHelper.Info(warningInfo);
                                warningInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,
                                                            parallelcalculist[i].startdate.ToString(),
                                                            parallelcalculist[i].enddate.ToString()
                                                            );
                                logHelper.Info(warningInfo);
                                //如果当前时间段条件整体为空，则过滤后的输入数据应该为空，记录log，并以空数据进入计算
                                this.previousReadDataResults[parallelcalculist[i].spanIndex] = null;      //这里仅仅是一天并发周期中的一个时间段，如果该时间段没有数据，则仅仅是该时间段没有计算结果。也就是不向currentResultsDic增加数据
                                goto CURRENTCalcu;   
                            }                          

                            //1.2.4、使用限值时间序列对输出数据进行过滤
                            SpanLogic.SpansFilter(ref this.previousReadDataResults[parallelcalculist[i].spanIndex], this.previousCondReadDataResults[parallelcalculist[i].spanIndex], filterThreshold);
                            //检查过滤后的inputs
                            if (this.previousReadDataResults[parallelcalculist[i].spanIndex] == null ||                     //整个分段为空
                                this.previousReadDataResults[parallelcalculist[i].spanIndex].Length == 0 ||                    //分段长度为0
                                Array.IndexOf(this.previousReadDataResults[parallelcalculist[i].spanIndex], null) != -1        //分段里有数据为null
                                )   //当前计算数据为空，直接跳过
                            {
                                Debug.WriteLine("--ParallelCalcu task{0} has skipped ........", i);
                                //更新计数器
                                Interlocked.Increment(ref _warningCount);  //多线程下，采用原子操作。此处不能使用lock
                                _warningFlag = true;
                                //更新UI               
                                this.WarningUpdateAction(_warningCount, "");
                                //更新log
                                string warningInfo;
                                warningInfo = string.Format("历史数据计算引擎警告{0}：分时间段内经条件时间序列过滤后的数据为空！", this.warningCount.ToString());
                                logHelper.Info(warningInfo);
                                warningInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,
                                                            parallelcalculist[i].startdate.ToString(),
                                                            parallelcalculist[i].enddate.ToString()
                                                            );
                                logHelper.Info(warningInfo);
                                //跳过计算过程
                                //this.currentResultsDic = null;      //这里仅仅是一天并发周期中的一个时间段，如果该时间段没有数据，则仅仅是该时间段没有计算结果。也就是不向currentResultsDic增加数据
                                //continue;   //过滤后的数据为空，只记报警不跳过
                            }
                            
                        }//end 2.2、计算：条件过滤


                        //3 计算：主算法
                        //1、这里需要判断Calcu是否为空，如果为空，要写log
                        //2、调用时也要做try处理   
                    CURRENTCalcu:
                        //3.1、计算：获取计算对象（反射法）
                        Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);           //获得PSLCalcu.exe
                        Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename);   //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”                    
                        //PropertyInfo inputData = calcuclass.GetProperty("inputData");                 //获得算法类的静态参数inputData。在并发计算中，使用静态属性赋值，有线程安全问题。
                        PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");                   //获得算法类的静态参数calcuInfo                    
                        MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { typeof(List<PValue>[]), typeof(CalcuInfo) });    //获得算法类中带两个参数的Calcu()方法。 
                        //注意，Calcu方法有重载。在并发计算中，不能用静态变量传参。这里需要指明获得带两个参数的Calcu重载方法。这一点和实时计算引擎不同。
                        PropertyInfo ErrorFlag = calcuclass.GetProperty("errorFlag");                   //获得算法类的静态参数ErrorFlag，bool类型
                        PropertyInfo ErrorInfo = calcuclass.GetProperty("errorInfo");                   //获得算法类的静态参数ErrorInfo，string类型

                        //inputData.SetValue(null, this.previousReadDataResults[parallelcalculist[i].spanIndex]);              //将输入数据给入算法。在并发计算中，使用静态属性赋值，有线程安全问题。
                        //calcuInfo.SetValue(null, new CalcuInfo(currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,      //源标签名称   在并发计算中，使用静态属性赋值，有线程安全问题。
                                                               //currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,        //计算模件名称
                                                               //currentCalcuItems[parallelcalculist[i].selectItemsIndex].fparas,             //计算参数
                                                               //parallelcalculist[i].startdate,                                              //计算起始时间
                                                               //parallelcalculist[i].enddate));                                              //计算结束时间
                        List<PValue>[] inputs=this.previousReadDataResults[parallelcalculist[i].spanIndex];
                        CalcuInfo currentCalcu=new CalcuInfo(currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,            //源标签名称
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,            //计算模件名称
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].fparas,                 //计算参数
                                                               parallelcalculist[i].startdate,                                                  //计算起始时间
                                                               parallelcalculist[i].enddate);                                                   //计算结束时间
                        List<PValue>[] results = (List<PValue>[])Calcu.Invoke(null, new Object[2] {inputs,currentCalcu});                       //在并发计算引擎中，必须直接调用带参数的Calcu。不能使用静态属性传参，否则就存在线程安全问题。
                        if (bool.Parse(ErrorFlag.GetValue(calcuclass, null).ToString()))
                        {
                            //如果计算组件的errorflag为true则说明发生计算错误
                            //更新计数器
                            Interlocked.Increment(ref this._errorCount);
                            //更新UI
                            this.ErrorUpdateAction(this._errorCount, "");
                            //更新Log                           
                            string errInfo;
                            errInfo = string.Format("历史数据计算引擎错误{0}：并发计算错误!", this.errorCount.ToString());
                            logHelper.Error(errInfo);
                            errInfo = string.Format("——计算错误详细信息：{0}。", ErrorInfo.GetValue(calcuclass, null).ToString());
                            logHelper.Error(errInfo);
                            errInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,
                                                                parallelcalculist[i].startdate.ToString(),
                                                                parallelcalculist[i].enddate.ToString()
                                                                );
                            logHelper.Error(errInfo);
                            //跳过计算过程
                            //this.currentResultsDic = null;      //这里仅仅是一天并发周期中的一个时间段，如果该时间段没有数据，则仅仅是该时间段没有计算结果。也就是不向currentResultsDic增加数据
                            continue;   //针对内部for，进行下一个计算
                        }//end 3、计算：主算法

                        //4、处理结算结果。写入存储变量currentResultsDic
                        if (results != null && results.Length>0)
                        {
                            //用于测试观察计算结果:
                            //特别注意！！不测试时，必须注释掉，对性能有非常大的影响
                            /*
                            lock (this.currentResultsPValue)
                            { 
                                for(int iPointOutput = 0; iPointOutput < currentCalcuItems[parallelcalculist[i].selectItemsIndex].foutputnumber; iPointOutput++)
                                {
                                     for (int iPointPvalue = 0; iPointPvalue < results[iPointOutput].Count; iPointPvalue++)
                                    {
                                        this.currentResultsPValue[iPointOutput].Add(results[iPointOutput][iPointPvalue]);
                                    }
                                }
                            }
                            */

                            string tagid, tagtimestamp, tagendtime, tagvalue, tagstatus;
                            string sqlStr;
                            for (int iPointOutput = 0; iPointOutput < currentCalcuItems[parallelcalculist[i].selectItemsIndex].foutputnumber; iPointOutput++)
                            {
                                //如果对应的计算为Y，则保存其结果
                                if (currentCalcuItems[parallelcalculist[i].selectItemsIndex].falgorithmsflagbool[iPointOutput])
                                {
                                    //String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, pvalue.Timestamp.Ticks, pvalue.Endtime.Ticks, pvalue.Value, pvalue.Status));
                                    tagid = currentCalcuItems[parallelcalculist[i].selectItemsIndex].foutputpsltagids[iPointOutput].ToString();
                                    for (int iPointPvalue = 0; iPointPvalue < results[iPointOutput].Count; iPointPvalue++)
                                    {
                                        tagtimestamp = results[iPointOutput][iPointPvalue].Timestamp.Ticks.ToString();
                                        tagendtime = results[iPointOutput][iPointPvalue].Endtime.Ticks.ToString();
                                        tagvalue = results[iPointOutput][iPointPvalue].Value.ToString();
                                        tagstatus = results[iPointOutput][iPointPvalue].Status.ToString();
                                        sqlStr = String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, tagtimestamp, tagendtime, tagvalue, tagstatus);

                                        //向结算结果变量中写结果
                                        //——只有当以tagid + tagtimestamp构成的key值不存在的时候，才能添加sqlStr。
                                        //——这是并发计算中的防止写重复值得检测机制
                                        this.currentResultsDic.TryAdd(tagid + tagtimestamp, sqlStr);
                                        //this.currentCalcuResults.Add(sqlStr); 、//已经被currentResultsDic替代
                                        
                                    }
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        //更新计数器
                        Interlocked.Increment(ref this._errorCount);  //多线程下，采用原子操作。此处不能使用lock
                        errorFlag = true;
                        //更新UI
                        this.ErrorUpdateAction(this._errorCount, "");
                        //更新Log                           
                        string errInfo;
                        errInfo = string.Format("历史数据计算引擎错误{0}：读算写错误中未知错误!", errorCount.ToString());
                        logHelper.Error(errInfo);
                        errInfo = string.Format("——详细错误信息：{0}", ex.ToString());
                        logHelper.Error(errInfo);
                        errInfo = string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                                   currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                                   currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                                   currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,
                                                                   parallelcalculist[i].startdate.ToString(),
                                                                   parallelcalculist[i].enddate.ToString()
                                                                   );
                        logHelper.Error(errInfo);
                        //跳过计算过程
                        //this.currentResultsDic = null;      //这里仅仅是一天并发周期中的一个时间段，如果该时间段没有数据，则仅仅是该时间段没有计算结果。也就是不向currentResultsDic增加数据
                        continue;   //针对内部for，进行下一个计算
                    }

                }//end for 结束每一个并发分区内的循环计算
                Debug.WriteLine("----ParallelCalcu range " + (range.Item1) + "~" + (range.Item2 - 1) + " complete:" + sw1.Elapsed.ToString());          


            });//end Parallel.ForEach
            #endregion

            #region 串行方式，不采用
            /*
            for (int i = 0; i < parallelcalculist.Length; i++)
            {
                var sw1 = Stopwatch.StartNew();
                //2.1 计算：获取计算对象（反射法）                
                //获得静态属性或方法
                Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);           //获得PSLCalcu.exe
                Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + SelectedItems[parallelcalculist[i].selectItemsIndex].fmodulename);   //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”                    
                PropertyInfo inputData = calcuclass.GetProperty("inputData");                   //获得算法类的静态参数inputData
                PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");                   //获得算法类的静态参数calcuInfo                    
                MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { });               //获得算法类的Calcu()方法。注意，Calcu方法有重载，这里需要指明获得哪个具体对象，否则会报错

                //获得一般属性或方法，要通过实例
                //Object obj = assembly.CreateInstance(APPConfig.NAMESPACE_CALCUMODULE + "." + SelectedItems[calculist[i].selectItemsIndex].fmodulename);


                //2.3 计算：主算法
                //1、这里需要判断Calcu是否为空，如果为空，要写log
                //2、调用时也要做try处理                
                inputData.SetValue(null, previousDayRealDataResulte[parallelcalculist[i].spanIndex]);   //将输入数据给入算法
                calcuInfo.SetValue(null, new CalcuInfo(SelectedItems[parallelcalculist[i].selectItemsIndex].sourcetagname, SelectedItems[parallelcalculist[i].selectItemsIndex].fmodulename, SelectedItems[parallelcalculist[i].selectItemsIndex].fparas, parallelcalculist[i].startdate, parallelcalculist[i].enddate));     //将当前计算信息给入算法
                List<PValue>[] results = (List<PValue>[])Calcu.Invoke(null, null);
                if (results == null)
                {   //如果计算结果为空，则说明计算过程中出错，计数器递增。出错信息已经在计算模块内部记录。直接跳到下一次计算。 
                    //计算模块错误，内部会记录。
                    //errorCount = errorCount + 1;
                    //errorFlag = true;
                    //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                    //string errInfo = string.Format("——错误统计序号{0}。", errorCount.ToString());
                    // LogHelper.Write(LogType.Error, errInfo);
                    // goto NEXTCalcu;
                }//end 2.3、计算：主算法

                //2.4、处理结算结果。写入存储变量_currentCalcuResults
                string tagid, tagtimestamp, tagendtime, tagvalue, tagstatus;
                string sqlStr;
                //--测试，为了看到结果计算是否正确，要对计算结果进行排序

                for (int iPointOutput = 0; iPointOutput < SelectedItems[parallelcalculist[i].selectItemsIndex].foutputnumber; iPointOutput++)
                {
                    //如果对应的计算为Y，则保存其结果
                    if (SelectedItems[parallelcalculist[i].selectItemsIndex].falgorithmsflagbool[iPointOutput])
                    {
                        //String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, pvalue.Timestamp.Ticks, pvalue.Endtime.Ticks, pvalue.Value, pvalue.Status));
                        tagid = SelectedItems[parallelcalculist[i].selectItemsIndex].foutputpsltagids[iPointOutput].ToString();
                        for (int iPointPvalue = 0; iPointPvalue < results[iPointOutput].Count; iPointPvalue++)
                        {
                            tagtimestamp = results[iPointOutput][iPointPvalue].Timestamp.Ticks.ToString();
                            tagendtime = results[iPointOutput][iPointPvalue].Endtime.Ticks.ToString();
                            tagvalue = results[iPointOutput][iPointPvalue].Value.ToString();
                            tagstatus = results[iPointOutput][iPointPvalue].Status.ToString();
                            sqlStr = String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, tagtimestamp, tagendtime, tagvalue, tagstatus);
                            currentCalcuResults.Add(sqlStr);
                        }
                    }
                }
                Debug.WriteLine("----ParallelCalcu rask " + i + "(" + SelectedItems[parallelcalculist[i].selectItemsIndex].fmodulename + ")" + " complete:" + sw1.Elapsed.ToString());
            }//end For
            
            */
            #endregion

            #region parallel.For方式，不采用
            /*
            Parallel.For(0, calculist.Length, (int i) =>       //对sourcetagnames中的每一个tag，用一个并发的任务单独读取
            {
                var sw1 = Stopwatch.StartNew();
                //2.1 计算：获取计算对象（反射法）                
                //获得静态属性或方法
                Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);           //获得PSLCalcu.exe
                Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + SelectedItems[calculist[i].selectItemsIndex].fmodulename);   //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”                    
                PropertyInfo inputData = calcuclass.GetProperty("inputData");                   //获得算法类的静态参数inputData
                PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");                   //获得算法类的静态参数calcuInfo                    
                MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { });               //获得算法类的Calcu()方法。注意，Calcu方法有重载，这里需要指明获得哪个具体对象，否则会报错

                //获得一般属性或方法，要通过实例
                //Object obj = assembly.CreateInstance(APPConfig.NAMESPACE_CALCUMODULE + "." + SelectedItems[calculist[i].selectItemsIndex].fmodulename);
                

                //2.3 计算：主算法
                //1、这里需要判断Calcu是否为空，如果为空，要写log
                //2、调用时也要做try处理                
                inputData.SetValue(null, previousDayRealDataResulte[calculist[i].spanIndex]);   //将输入数据给入算法
                calcuInfo.SetValue(null, new CalcuInfo(SelectedItems[calculist[i].selectItemsIndex].sourcetagname, SelectedItems[calculist[i].selectItemsIndex].fmodulename,SelectedItems[calculist[i].selectItemsIndex].fparas,calculist[i].startdate, calculist[i].enddate));     //将当前计算信息给入算法
                List<PValue>[] results = (List<PValue>[])Calcu.Invoke(null, null);
                if (results == null)
                {   //如果计算结果为空，则说明计算过程中出错，计数器递增。出错信息已经在计算模块内部记录。直接跳到下一次计算。 
                    //计算模块错误，内部会记录。
                    //errorCount = errorCount + 1;
                    //errorFlag = true;
                    //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                    //string errInfo = string.Format("——错误统计序号{0}。", errorCount.ToString());
                    // LogHelper.Write(LogType.Error, errInfo);
                    // goto NEXTCalcu;
                }//end 2.3、计算：主算法

                //2.4、处理结算结果。写入存储变量_currentCalcuResults
                string tagid, tagtimestamp, tagendtime, tagvalue, tagstatus;
                string sqlStr;
                    //--测试，为了看到结果计算是否正确，要对计算结果进行排序
                        
                for (int iPointOutput = 0; iPointOutput < SelectedItems[calculist[i].selectItemsIndex].foutputnumber; iPointOutput++)
                {
                    //如果对应的计算为Y，则保存其结果
                    if (SelectedItems[calculist[i].selectItemsIndex].falgorithmsflagbool[iPointOutput])
                    {
                        //String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, pvalue.Timestamp.Ticks, pvalue.Endtime.Ticks, pvalue.Value, pvalue.Status));
                        tagid = SelectedItems[calculist[i].selectItemsIndex].foutputpsltagids[iPointOutput].ToString();
                        for (int iPointPvalue = 0; iPointPvalue < results[iPointOutput].Count; iPointPvalue++)
                        {
                            tagtimestamp = results[iPointOutput][iPointPvalue].Timestamp.Ticks.ToString();
                            tagendtime = results[iPointOutput][iPointPvalue].Endtime.Ticks.ToString();
                            tagvalue = results[iPointOutput][iPointPvalue].Value.ToString();
                            tagstatus = results[iPointOutput][iPointPvalue].Status.ToString();
                            sqlStr = String.Format("('{0}','{1}','{2}','{3}','{4}')", tagid, tagtimestamp, tagendtime, tagvalue, tagstatus);
                            //currentCalcuResults.Add(sqlStr);
                        }
                    }
                }
                Debug.WriteLine("----ParallelCalcu rask "+i+" complete:" + sw1.Elapsed.ToString());
            });//end Parallel.For
            */
            #endregion
                        
            Debug.WriteLine("--ParallelCalcu has completed:" + sw.Elapsed.ToString());
            return;
        }
        //写计算结果
        private void WritePSLData(DateTime startDate,DateTime endDate)
        {           
            var sw = Stopwatch.StartNew();            
            Debug.WriteLine("--WritePSLData has started:" + sw.Elapsed.ToString());
            
            if (this.previousCalcuResults != null && this.previousCalcuResults.Length!=0)
            {

                bool writeflag = PSLDataDAO.WriteHistoryCalcuResults(startDate,endDate, this.previousCalcuResults);
                if (!writeflag)
                {
                    //更新计数器
                    Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                    errorFlag = true;
                    //更新UI
                    this.ErrorUpdateAction(this._errorCount, "");
                    //更新LOG
                    string errInfo;
                    errInfo = string.Format("历史数据计算引擎错误{0}：写计算结果WritePSLData错误！", errorCount.ToString());
                    logHelper.Error(errInfo);    //错误详细信息记录在DAO中                
                }
            }
            else
            {
                Debug.WriteLine("--WritePSLData has skipped ....");
            }
            Debug.WriteLine("--WritePSLData has completed:" + sw.Elapsed.ToString());
        }
        
        #region 辅助函数
        //List二维数组转置
        private List<PValue>[][] ListTrans(List<PValue>[][] inputList)
        {
            if (inputList != null)
            {
                List<PValue>[][] outputList = new List<PValue>[inputList[0].Length][];
                for (int i = 0; i < inputList[0].Length; i++)
                {
                    outputList[i] = new List<PValue>[inputList.Length];
                    for (int j = 0; j < inputList.Length; j++)
                    {
                        outputList[i][j] = inputList[j][i];

                    }
                }
                return outputList;
            }
            else
            {
                return null;
            }
        }
        //计算标签数据分割，针对实时数据
        public List<PValue>[] SpanPValues4rtdb(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——再具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐
            //——测试:产生一天的非整小时数据，看能否正确分隔成对应的段
            //pvalues = TestData_RandomPValue.Generate("sin", 600, 0, 100, startdate, enddate, 7);
            //——测试:产生一天的整小时数据，看能否正确分隔成对应的段
            //pvalues = TestData_RandomPValue.Generate("sin", 600, 0, 100, startdate, enddate, 1);
            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds /intervalseconds);
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            if (pvalues != null && pvalues.Count != 0)
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);
                int spanIndex = 0;
                pvaluesSpan[spanIndex] = new List<PValue>();

                //循环处理从0到Count-2点
                for (int ipoint = 0; ipoint < pvalues.Count ; ipoint++)
                {
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当当前点的时间小于下一个分隔时，应该将该点计入该段
                        //——首先要判断该段是否开始

                        //——如果该段已经开始，则先写上一个点的结束时间，再添加当前点
                        if (pvaluesSpan[spanIndex].Count!=0) pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint].Timestamp;
                        //——如果该段未开始，则说明当前点是开始点，直接添加
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value,pvalues[ipoint].Timestamp,pvalues[ipoint].Endtime,0));
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {   
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, 0));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, 0));       //给新段，添加开始点。用当前点加入下一段段初    

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {   
                        //当前点超出边界点，的情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex].Count != 0)
                        {
                            //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                            //首先，说明上一点结束时间已经超出，
                            //——完成上一个时间段最后一个有效数据的结束位置。
                            //——应该根据isolatedata时刻前后的点的值对isolatedata时刻得值进行计算。给上一个时间段添加timestamp和enddate都一致的结束点。
                            //——然后，isolatedata自增，回到当前点循环的currentPoint位置，继续看当前点否则还超越下一个计算周期。
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                 //上一个周期最后一个有效数据，填结束时间
                            PValue lastpvalue = PValue.Interpolation(pvalues[ipoint - 1], pvalues[ipoint], isolatedata);    //按照线性，计算边界时刻isolatedata的值，
                            pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, isolatedata, isolatedata, 0));          //给上一个周期添加截止时刻值

                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, 0));    //给当前段，添加起始点。用计算间隔点的值，作为边界点值。用计算间隔时间，作为起始时间。用当前点时间做结束时间。

                        }
                        else 
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();
                        
                        }
                       
                        //调到新的边界时刻，对当前点再次进行判断
                        isolatedata = isolatedata.AddSeconds(intervalseconds);
                        goto currentPoint;          
                        
                    }

                }
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点
                
            }
            else
            {
                pvaluesSpan = null;
                //如果任一数据源取数为空，则该数据一定出错，计数器递增。出错信息已经在取数据的DAO中记录。直接跳到下一次计算                        
                //warningCount = warningCount + 1;
                //warningFlag = true;
                //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                // LogHelper.Write(LogType.Warn, "计算引擎警告!");
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fmodulename, pslcalcuitem.sourcetagname, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                //LogHelper.Write(LogType.Warn, moduleInfo);
                //string errInfo = string.Format("——警告统计序号{0}：对应时间段内没有实时数据!", warningCount.ToString());
                //LogHelper.Write(LogType.Warn, errInfo);
                //goto NEXTCalcu;
            }
            return pvaluesSpan;
        }
        //计算标签数据分割，针对关系数据
        public List<PValue>[] SpanPValues4rdb(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //将关系数据分隔成 1days/intervalseconds段
            //——注意经rdbhelper.GetRawValues()读出的数据，pvalue的第一点起始时间，和最后一点的结束时间，有可能与入参startdate和enddate对齐。也有可能在startdate和enddate内。这点与实时数据不同
            //——再具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐
            //——测试:产生一天的非整小时数据，看能否正确分隔成对应的段
            //pvalues = TestData_RandomPValue.Generate("sin", 600, 0, 100, startdate, enddate, 7);
            //——测试:产生一天的整小时数据，看能否正确分隔成对应的段
            //pvalues = TestData_RandomPValue.Generate("sin", 600, 0, 100, startdate, enddate, 1);
            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            if (pvalues != null && pvalues.Count != 0)
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);
                int spanIndex = 0;
                pvaluesSpan[spanIndex] = new List<PValue>();

                //循环处理从0到Count-2点
                for (int ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当当前点的时间小于下一个分隔时，应该将该点计入该段
                        //——首先要判断该段是否开始

                        //——如果该段已经开始，则先写上一个点的结束时间，再添加当前点
                        if (pvaluesSpan[spanIndex].Count != 0) pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint].Timestamp;
                        //——如果该段未开始，则说明当前点是开始点，直接添加
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));  //概化数据，状态位有效！！！
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, 0));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime,  pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //当前点超出边界点，在概化数据的情况下，由于边界点必然有值，除非不可控错误导致边界点没值。所以，当发生该情况时，一定是边界点因为不可控错误，缺少对应时刻的值造成的。
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该首先去结束上面一段。然后添加截止时刻点。
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex].Count != 0)
                        {
                            //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去结束上面一段。并添加截止时刻点。
                            //首先，说明上一点结束时间已经超出，
                            //——完成上一个时间段最后一个有效数据的结束位置。
                            //——应该根据isolatedata时刻前后的点的值对isolatedata时刻得值进行计算。给上一个时间段添加timestamp和enddate都一致的结束点。
                            //——然后，isolatedata自增，回到当前点循环的currentPoint位置，继续看当前点否则还超越下一个计算周期。
                            //pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;               //保留当前段上一个点的结束时刻。在概化数据中，不存在一个计算结果起始时间和截止时间跨越isolatedata的情况。
                            //PValue lastpvalue = PValue.Interpolation(pvalues[ipoint - 1], pvalues[ipoint], isolatedata);    //按照线性，计算边界时刻isolatedata的值，
                            //pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, isolatedata, isolatedata, 0));          //给上一个周期添加截止时刻值
                            //************修改，与PSLDataDAO一样，发生这种情况时，用最后一个有效值，作为截止时刻的值。
                            pvaluesSpan[spanIndex].Add(new PValue(pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Value, isolatedata, isolatedata, 0));

                            //准备新一段数据。不在isolatedata位置进行插值，直接将新的点加入新的段
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));    //给当前段，添加起始点。用计算间隔点的值，作为边界点值。用计算间隔时间，作为起始时间。用当前点时间做结束时间。

                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                        }

                        //调到新的边界时刻，对当前点再次进行判断
                        isolatedata = isolatedata.AddSeconds(intervalseconds);
                        goto currentPoint;

                    }

                }
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点

            }
            else
            {
                pvaluesSpan = null;
                //如果任一数据源取数为空，则该数据一定出错，计数器递增。出错信息已经在取数据的DAO中记录。直接跳到下一次计算                        
                //warningCount = warningCount + 1;
                //warningFlag = true;
                //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                // LogHelper.Write(LogType.Warn, "计算引擎警告!");
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fmodulename, pslcalcuitem.sourcetagname, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                //LogHelper.Write(LogType.Warn, moduleInfo);
                //string errInfo = string.Format("——警告统计序号{0}：对应时间段内没有实时数据!", warningCount.ToString());
                //LogHelper.Write(LogType.Warn, errInfo);
                //goto NEXTCalcu;
            }
            return pvaluesSpan;
        }
        //计算条件数据分割
        public List<PValue>[] SpanPValues4SpanFilter(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //————该pvalues没有截止时刻点。

            //将条件数据分隔成 calcuPeriod/intervalseconds段
            //——注意经rdbhelper.GetRawValues()读出的概化条件数据，其数据是符合条件的有效时间段序列，数据的值是有效时间段的长度（毫秒数）\            
            //————pvalue的最早时间和最晚时间，与入参startdate和enddate不一定对齐。
            //————时间段之间也不一定连续。
            //——再具体分割时，仅主要把这些不连续的时间段，按照分割时间分割成组
            //*************请使用主界面的“并行时间分割ToolStripMenuItem_Click()”方法对此算法进行测试
            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            if (pvalues != null && pvalues.Count != 0)
            {
                //初始化
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);          //初始化为第一分割点
                int spanIndex = 0;                                              //分段指针，第0段
                int ipoint = 0;

                try
                {
                    //循环处理从0到Count-2点

                    for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                    {
                    currentPoint:
                        if (pvalues[ipoint].Timestamp < isolatedata)
                        {
                            //当前点的时间戳小于当前分隔点时，应该将该点计入该段
                            //——首先要判断该段是否开始
                            if (pvaluesSpan[spanIndex] == null)
                            {
                                //如果未开始，先初始化                           
                                pvaluesSpan[spanIndex] = new List<PValue>(); //pvaluesSpan[spanIndex]初始化前为null，初始化后.count==0
                            }
                            else
                            {
                                //——如果该段已经开始，当前情况下，该段最后一个点的截止时刻无需调整。
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;
                            }
                            //——添加当前点
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                            //当前段未结束，看下一个点
                        }
                        else if (pvalues[ipoint].Timestamp == isolatedata)
                        {
                            //当前点恰好是边界点的情况，这种情况比较简单                            
                            //如果当前点是边界点，说明当前段最后一个有效时间段的结束时间肯定在边界之前。当前段应该结束                        
                            if (pvaluesSpan[spanIndex] == null || pvaluesSpan[spanIndex].Count == 0)
                            {
                                //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                                //——直接到到下一个isolatedata。并且当前分段置为空。
                                pvaluesSpan[spanIndex] = null;

                            }
                            else
                            {
                                //*****filter不需要修改上一个点的结束时刻。因为filter的各个时间段之间不连贯。当前点是开始点，但是上一个点的结束时刻可能和当前点开始时刻不一致。
                                //pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;     //说明当前段结束，修改上次结束时间。时间序列无需修改上一次结束点
                                //*****filter不需要添加截止时刻点
                                //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, isolatedata, isolatedata, 0));         //添加当前段截止时刻值。时间序列不需要添加截止时刻值。

                            }

                            //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                            //if (ipoint != pvalues.Count - 1)      //条件时间数据，没有截止时刻
                            //{
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();    //初始化新段，并添加点
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //添加点完毕后，并不清楚当前点是否在该段结束，应用当前点取看
                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                            //}

                            //当前段未结束，看下一个点
                        }
                        else   //pvalues[ipoint].Timestamp > isolatedata
                        {
                            //当前点超出当前边界isolatedata时，
                            //对于filter，首先是要结束当前段,然后是开始新段

                            //————首先要结束当前段。                        
                            //——————当pvaluesSpan[spanIndex]为空或者.Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。或者两个有效点之间隔了好几个时间段。
                            //——————当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，并且这个段应该结束。
                            //————但是对于上一段来说，上一个点结束时刻可能在isolatedata前。也可能在后。要判断
                            //——————上一点结束时刻在isolatedata前，什么也不做。上一段已经正常借宿
                            //——————上一点结束时刻在isolatedata后，要进行阶段。一般在上一个分段。一半在新分段。

                            if (pvaluesSpan[spanIndex] == null || pvaluesSpan[spanIndex].Count == 0)
                            {
                                //如果当前段什么点都没有，说明该时间段内没有有效数据。则该段自动结束
                                //过渡到新的段
                                spanIndex = spanIndex + 1;
                            }
                            else
                            {
                                //如果当前段有点，则需要看该点的结束为止
                                DateTime lastDate = pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime;
                                long status = pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Status;
                                if (lastDate <= isolatedata)
                                {
                                    //如果上一个点的截止时刻小于或等于分割时间，则什么也不做 
                                    spanIndex = spanIndex + 1;
                                }
                                else
                                {
                                    //如果上一个点的截止时刻大分割时间，则要分割上一个点时间段
                                    pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;
                                    pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Value = pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Timespan; //时间段的值代表该时间段的长度，需要调整

                                    //准备新一段数据,放入后半部分时间段
                                    spanIndex = spanIndex + 1;
                                    pvaluesSpan[spanIndex] = new List<PValue>();
                                    pvaluesSpan[spanIndex].Add(new PValue(0, isolatedata, lastDate, status));
                                    pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Value = pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Timespan;
                                }
                            }

                            //对于filter来说，还要开始新的一段。
                            //又分两种情况，
                            //——当前点的时间戳大于当前分割点isolatedata，但小于下一个分割点isolatedata
                            //——当前点的时间戳大于当前分割点isolatedata，也大于下一个分割点isolatedata.AddSeconds(intervalseconds)
                            if (pvalues[ipoint].Timestamp < isolatedata.AddSeconds(intervalseconds))
                            {
                                //如果当前点起始时刻大于当前分割点，但小于下一个分割点说明。说明加1过的当前分段内应该有值。
                                //此时直接跳转到下一个分割点，对当前值进行判断。（这里理解非常重要，用测试程序来理解）
                                //这种情况下应该是跳到新的点，对当前边界进行判断
                                ///spanIndex = spanIndex + 1;

                            }
                            else
                            {
                                //如果当前点起始时刻大于当前分割点，而且还大于等于下一个分割点，说明加1过的当前分段内也没有值。应该跳过该段
                                //spanIndex = spanIndex + 1;
                                //调到新的边界时刻，对当前点再次进行判断                            

                            }

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                            goto currentPoint;
                        }//结束三种情况的分支

                        //下一个点
                    }//end for
                    //添加截止时刻值。
                    //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                    //用原pvalues的截止时刻作为最后一个分段的截止时刻。因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点

                }
                catch (Exception ex)
                {
                    int temp = 10;
                }
            }
            else
            {
               //如果pvalues没有数据，则对应pvaluesSpan对应所有分段内均为null。直接返回这个所有段均为null的值               
            }
            return pvaluesSpan;
        }
        
        #endregion
    }
}
