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
using System.Windows.Forms;             //转置报错

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
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(HistoryParallelCalcuEngine));      //全局log
        private long _errorCount;
        private long _errorCountBase;           //错误数量初值
        public long errorCount { get { return _errorCount; } set { this._errorCount = value; } }            //历史数据计算错误统计。long类型，不是引用类型，不能被lock。应使用interlock操作。
        private bool _errorFlag;
        public bool errorFlag { get { return _errorFlag; } set { this._errorFlag = value; } }                 //历史数据计算错误标志。
        private long _warningCount;
        private long _warningCountBase;         //报警数量初值
        public long warningCount { get { return _warningCount; } set { this._warningCount = value; } }      //历史数据计算警告统计。long类型，不是引用类型，不能被lock。应使用interlock操作。
        private bool _warningFlag;
        public bool warningFlag { get { return _warningFlag; } set { this.warningFlag = value; } }          //历史数据计算警告标志。

        public bool IsParallelCalcu { get; set; }                       //是否并发计算
        public int ParallelPeriod { get; set; }                       //并发计算的并发周期。秒值。即一次取多长时间的实时数据进行并发

        private List<PValue>[][] previousReadDataResults;               //上一天的实时数据：并发计算时，仅从该变量读取实时数据。
        private List<PValue>[][] currentReadDataResults;                //当天的实时数据：并发读取实时数据后写入该变量。虽然并发，但是采用在线程内加锁的方式来控制。
        private List<PValue>[] previousCondReadDataResults;             //上一天的条件数据：并发计算时，仅从该变量读取条件数据（关系库）。
        private List<PValue>[] currentCondReadDataResults;              //当天的条件数据：并发读取条件数据（关系库）。虽然并发，但是采用在线程内加锁的方式来控制。
        private string[] previousCalcuResults;                          //上一天的计算结果：写计算结果到数据库的程序，从该变量读取数据，组合成字符串，用一条sql一次写入。非并发程序。

        //private ConcurrentBag<string> currentCalcuResults;            //当天的计算结果:并发线程会向其写数。采用不带锁的并发集合，以便提高效率。已经被currentResultsDic替代
        private ConcurrentDictionary<string, string> currentResultsDic; //当天计算结果字典:并发线程会向其写数。采用不带锁的并发集合，以便提高效率。用来判断同一个id，同一时刻是否有重复值
        private List<PValue>[] currentResultsPValue;                    //用于测试观察计算结果

        public event Action<int, string> ProcessUpdateAction;            //观察者模式下，通知被观察者的通知事件。这里用于通知观察者并发计算执行的百分比情况
        public event Action<long, string> WarningUpdateAction;           //观察者模式下，通知被观察者的通知事件。这里用于通知观察这并发计算报警计数更新
        public event Action<long, string> ErrorUpdateAction;             //观察者模式下，通知被观察者的通知事件。这里用于通知观察这并发计算错误计数更新

        //入参：currentCalcuItems：当次选定的可以在组织在一起进行一次并发计算的对象集
        //入参：StartDateForHistoryCalcu，EndDateForHistoryCalcu：历史计算的时间区间
        public void MainHistoryParallelCalcu(List<PSLCalcuItem> currentCalcuItems, DateTime StartDateForHistoryCalcu, DateTime EndDateForHistoryCalcu)
        {
            //计时，用于并发计算调试的时间测试。
            //System.Diagnostics.Debug类提供的方法只会在调试版本运行，在正式编译的程序中不运行。
            //Debug内容在output窗口中显示，是并发计算的主要调试手段。
            var swSingleParallel = Stopwatch.StartNew();
            _errorCountBase = this.errorCount;
            _warningCountBase = this.warningCount;

            //0、计算起始时间调整。实际的计算起始时间，由界面设定的年月日，和计算对象的时分秒构成。
            var startSecond = currentCalcuItems[0].fstarttime.Second;       //获取计算项计算起始时间中的秒
            var startMinute = currentCalcuItems[0].fstarttime.Minute;       //获取计算项计算起始时间中的分钟
            var startHour = currentCalcuItems[0].fstarttime.Hour;           //获取计算项计算起始时间中的小时

            //前期准备
            //1、删除选定计算项在要计算的时间区间内在数据仓库里已经存在的计算结果
            //——计算项在startIndex, endIndex之间，在StartDateForHistoryCalcu, EndDateForHistoryCalcu时间范围内，包含的的所有outputtagid记录全部删除。
            //——注意，由于并发计算，最小单位是天（比如实时数据是按1天或n天并发，关系数据是按30天并发。app参数historycalcu_period4RTD="1" historycalcu_period4PSL="30"的设置也是按天）
            //——因此，历史数据计算界面，计算的起始和截止时间只能选年月日。意味着计算周期只能是年月日。时刻分必须是从00:00~00:00。
            //——对于本身计算周期为分钟、小时的计算项来说。直接按照历史数据计算界面选定的时间来删除数据，没有问题。因为实际的计算周期粒度小于并发粒度。
            //——对于本身计算周期为日的计算项来说，可能存在计算起始时间的小时和分不是从整刻00:00开始，这种情况下需要根据计算项的实际时刻分，对并发时刻进行调整。
            DateTime startdateDelete = new DateTime();
            DateTime enddateDelete = new DateTime();
            if (currentCalcuItems[0].fintervaltype == "d" || currentCalcuItems[0].fintervaltype == "day" || currentCalcuItems[0].fintervaltype == "days")
            {
                //对于本身就是天周期的计算项来说，由于起始时刻不一定从整时整分整秒开始，因此做并发计算删除时，界面设定的起始时刻和截止时刻，要根据实际计算项的时分秒进行调整
                startdateDelete = StartDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                enddateDelete = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
            }
            else if (currentCalcuItems[0].fintervaltype == "h" || currentCalcuItems[0].fintervaltype == "hour" || currentCalcuItems[0].fintervaltype == "hours")
            {
                //对于本身就是hour周期的计算项来说，由于起始时刻不一定整分整秒开始，因此做并发计算删除时，界面设定的起始时刻和截止时刻，要根据实际计算项的分秒进行调整
                startdateDelete = StartDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond);
                enddateDelete = EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond);
            }
            else if (currentCalcuItems[0].fintervaltype == "min" || currentCalcuItems[0].fintervaltype == "minute" || currentCalcuItems[0].fintervaltype == "minutes")
            {
                //对于本身就是minutes周期的计算项来说，由于起始时刻不一定整秒开始，因此做并发计算删除时，界面设定的起始时刻和截止时刻，要根据实际计算项的秒进行调整
                startdateDelete = StartDateForHistoryCalcu.AddSeconds(startSecond);
                enddateDelete = EndDateForHistoryCalcu.AddSeconds(startSecond);
            }
            else
            {
                startdateDelete = StartDateForHistoryCalcu;
                enddateDelete = EndDateForHistoryCalcu;
            }
            bool deleteflag = DeletePSLData(currentCalcuItems, startdateDelete, enddateDelete);
            Debug.WriteLine("delete all existing data complete:" + swSingleParallel.Elapsed.ToString());
            /*
             在DeletePSLData内部报错。
            if (!deleteflag) 
            {
                _errorCount = _errorCount + 1;
                //更新UI
                this.ErrorUpdateAction(this._errorCount,"");
                //记录log
                string errInfo;
                errInfo = string.Format("并发计算引擎错误{0}：删除数据错误!", this._errorCount.ToString()) + Environment.NewLine;
                //logHelper.Error(errInfo);
                errInfo += string.Format("——并发计算项的序号是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItems[0].fid, currentCalcuItems[currentCalcuItems.Count-1].fid,StartDateForHistoryCalcu.ToString(), EndDateForHistoryCalcu.ToString());
                logHelper.Error(errInfo); 
                goto NEXTCalcu;   
            }
            */
            //2、为并发计算准备参数
            int ParallelPeriodDays = ParallelPeriod / (60 * 60 * 24);                                                   //并发计算的的并发周期，这里用日。如果是实时数据并发（rtdb、opc）一般是1天。概化数据一般是30天。
            int totalSpans = (int)Math.Ceiling((double)EndDateForHistoryCalcu.Subtract(StartDateForHistoryCalcu).Days / (double)ParallelPeriodDays);       //按并发周期分割，计算时间区间的分割段数。

            //2.1、为ParallelReadRealData准备参数                    
            int intervalseconds = currentCalcuItems[0].intervalseconds;    //获取当前计算项（如果是一组，则是第一项）的实时计算要求的计算周期
            string[] sourcetagnames = Regex.Split(currentCalcuItems[0].sourcetagname, ";|；");
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



            for (int spanIndex = 0; spanIndex < totalSpans + 2; spanIndex++)   //这里多循环两次，可以保证能将正常选定日期最后一天的数据计算完，并写入数据库
            {
                var sw = Stopwatch.StartNew();

                //准备每一次并发计算需要数据的时间段。                
                //——注意，由于并发计算，最小单位是天（比如实时数据是按1天或n天并发，关系数据是按30天并发。app参数historycalcu_period4RTD="1" historycalcu_period4PSL="30"的设置也是按天）
                //——因此，历史数据计算界面，计算的起始和截止时间只能选年月日。意味着计算周期只能是年月日。时刻分必须是从00:00~00:00。
                //——对于本身计算周期为分钟、小时的计算项来说。直接按照历史数据计算界面选定的时间来删除数据，没有问题。因为实际的计算周期粒度小于并发粒度。
                //——对于本身计算周期为日的计算项来说，可能存在计算起始时间的小时和分并不是从整刻00:00开始，这种情况下需要根据计算项的实际时刻分，对并发时刻进行调整。
                //——小时分秒，用计算项的设定来计算。这是因为计算项并不一定是在整时、整分、整秒来计算的。比如带值次条件的计算，计算周期为1d。但是起始时间可能是2016年1月1日 2:00。
                //那每次取一个月的数据，应该是2016年1月1日 2:00 到 2016年1月30日 2:00。
                DateTime startdate = new DateTime();
                DateTime enddate = new DateTime();
                DateTime previousstartdate = new DateTime();
                DateTime previousenddate = new DateTime();
                DateTime beforestartdate = new DateTime();
                DateTime beforeenddate = new DateTime();
                if (currentCalcuItems[0].fintervaltype == "d" || currentCalcuItems[0].fintervaltype == "day" || currentCalcuItems[0].fintervaltype == "days")
                {   
                    //起始时间
                    startdate = StartDateForHistoryCalcu.AddDays(spanIndex * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    //截止时间
                    enddate = StartDateForHistoryCalcu.AddDays((spanIndex + 1) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    previousstartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    previousenddate = StartDateForHistoryCalcu.AddDays((spanIndex) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    beforestartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 2) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    beforeenddate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays).AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                }
                else if (currentCalcuItems[0].fintervaltype == "h" || currentCalcuItems[0].fintervaltype == "hour" || currentCalcuItems[0].fintervaltype == "hours")
                {
                    startdate = StartDateForHistoryCalcu.AddDays(spanIndex * ParallelPeriodDays).AddMinutes(startMinute).AddSeconds(startSecond);
                    enddate = StartDateForHistoryCalcu.AddDays((spanIndex + 1) * ParallelPeriodDays).AddMinutes(startMinute).AddSeconds(startSecond);
                    previousstartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays).AddMinutes(startMinute).AddSeconds(startSecond);
                    previousenddate = StartDateForHistoryCalcu.AddDays((spanIndex) * ParallelPeriodDays).AddMinutes(startMinute).AddSeconds(startSecond);
                    beforestartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 2) * ParallelPeriodDays).AddMinutes(startMinute).AddSeconds(startSecond);
                    beforeenddate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays).AddMinutes(startMinute).AddSeconds(startSecond);
                }
                else if (currentCalcuItems[0].fintervaltype == "min" || currentCalcuItems[0].fintervaltype == "minute" || currentCalcuItems[0].fintervaltype == "minutes")
                {
                    startdate = StartDateForHistoryCalcu.AddDays(spanIndex * ParallelPeriodDays).AddSeconds(startSecond);
                    enddate = StartDateForHistoryCalcu.AddDays((spanIndex + 1) * ParallelPeriodDays).AddSeconds(startSecond);
                    previousstartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays).AddSeconds(startSecond);
                    previousenddate = StartDateForHistoryCalcu.AddDays((spanIndex) * ParallelPeriodDays).AddSeconds(startSecond);
                    beforestartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 2) * ParallelPeriodDays).AddSeconds(startSecond);
                    beforeenddate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays).AddSeconds(startSecond);
                }
                else
                {
                    startdate = StartDateForHistoryCalcu.AddDays(spanIndex * ParallelPeriodDays);
                    enddate = StartDateForHistoryCalcu.AddDays((spanIndex + 1) * ParallelPeriodDays);
                    previousstartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays);
                    previousenddate = StartDateForHistoryCalcu.AddDays((spanIndex) * ParallelPeriodDays);
                    beforestartdate = StartDateForHistoryCalcu.AddDays((spanIndex - 2) * ParallelPeriodDays);
                    beforeenddate = StartDateForHistoryCalcu.AddDays((spanIndex - 1) * ParallelPeriodDays);
                }
                //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据。
                //——但是同时保留原循环周期的小时，分钟，秒的信息。比如值此计算，如果循环周期是每天2:00到第二天2:00。当最后一个月取值时，就不会截止到0:00
                if (currentCalcuItems[0].fintervaltype == "d" || currentCalcuItems[0].fintervaltype == "day" || currentCalcuItems[0].fintervaltype == "days")
                {
                    if (startdate > EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond)) startdate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    if (enddate > EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond)) enddate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);                        //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据
                    if (previousstartdate > EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond)) previousstartdate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                    if (previousenddate > EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond)) previousenddate = EndDateForHistoryCalcu.AddHours(startHour).AddMinutes(startMinute).AddSeconds(startSecond);
                }
                else if (currentCalcuItems[0].fintervaltype == "h" || currentCalcuItems[0].fintervaltype == "hour" || currentCalcuItems[0].fintervaltype == "hours")
                {
                    if (startdate > EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond)) startdate = EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond);
                    if (enddate > EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond)) enddate = EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond);                        //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据
                    if (previousstartdate > EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond)) previousstartdate = EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond);
                    if (previousenddate > EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond)) previousenddate = EndDateForHistoryCalcu.AddMinutes(startMinute).AddSeconds(startSecond);
                }
                else if (currentCalcuItems[0].fintervaltype == "min" || currentCalcuItems[0].fintervaltype == "minute" || currentCalcuItems[0].fintervaltype == "minutes")
                {
                    if (startdate > EndDateForHistoryCalcu.AddSeconds(startSecond)) startdate = EndDateForHistoryCalcu.AddSeconds(startSecond);
                    if (enddate > EndDateForHistoryCalcu.AddSeconds(startSecond)) enddate = EndDateForHistoryCalcu.AddSeconds(startSecond);                        //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据
                    if (previousstartdate > EndDateForHistoryCalcu.AddSeconds(startSecond)) previousstartdate = EndDateForHistoryCalcu.AddSeconds(startSecond);
                    if (previousenddate > EndDateForHistoryCalcu.AddSeconds(startSecond)) previousenddate = EndDateForHistoryCalcu.AddSeconds(startSecond);
                }
                else
                {
                    if (startdate > EndDateForHistoryCalcu) startdate = EndDateForHistoryCalcu;
                    if (enddate > EndDateForHistoryCalcu) enddate = EndDateForHistoryCalcu;                        //为超时的两个循环准备，当enddate大于实际历史计算截止时刻时，enddate等于该时刻，这样保证超时的两个循环取不出数据
                    if (previousstartdate > EndDateForHistoryCalcu) previousstartdate = EndDateForHistoryCalcu;
                    if (previousenddate > EndDateForHistoryCalcu) previousenddate = EndDateForHistoryCalcu;
                }

                Debug.WriteLine("for i=" + spanIndex + " has started: startdate is {0}...enddate is {1}..............", startdate.ToString(), enddate.ToString());

                //读、算、写共四个任务并行执行
                var taskReadRealData = Task.Factory.StartNew(() => ParallelReadRealData(currentCalcuItems, startdate, enddate, intervalseconds));   //读取下一条计算需要的实时数据：结果放入全局变量currentReadDataResults
                var taskReadCondData = Task.Factory.StartNew(() => ParallelReadCondData(currentCalcuItems, startdate, enddate, intervalseconds));   //读取下一条计算需要的条件数据：结果放入全局变量currentCondReadDataResults
                var taskDoParallelCalcu = Task.Factory.StartNew(() => ParallelCalcu(currentCalcuItems, previousstartdate, previousenddate, intervalseconds, spanIndex, totalSpans));       //计算当前天的结果：根据previousReadDataResults、previousCondReadDataResults计算，结果放入全局变量currentResultsDic
                var taskWritePSLData = Task.Factory.StartNew(() => WritePSLData(currentCalcuItems, beforestartdate, beforeenddate));                                                       //写入上一天计算结果：将previousCalcuResults写入数据库
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
                //——当输入中有任何一个标签在对应时间段内（一天）的值为空值时，仍然要保证数组能够顺利转换，只不过对应的数组数值均为null
                //if (this.currentReadDataResults != null && Array.IndexOf(this.currentReadDataResults, null) == -1)
                if (this.currentReadDataResults != null)
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
                    this.ProcessUpdateAction((spanIndex + 1) * 100 / (totalSpans + 2), (this.warningCount - _warningCountBase).ToString() + ";" + (this.errorCount - _errorCountBase).ToString());
                }


            }//end for               


            //Debug.WriteLine("*************CalcuItem " + currentCalcuItems[0].sourcetagname + " is complete:" + swSingleParallel.Elapsed.ToString());
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
            System.UInt32[] tagidsarray = tagids.ToArray();
            int deleteNumber = PSLDataDAO.Delete(tagidsarray, startdate, enddate);

            //DAO层发生错误时
            //——PSLDataDAO.Error为true
            //——PSLDataDAO.Delete()会记录log
            //——PSLDataDAO.Delete()返回-1，

            //在这个要根据Error，更新全局错误记录数
            if (PSLDataDAO.ErrorFlag)
            {
                //更新计数器
                Interlocked.Increment(ref _errorCount);  //读算写并发多线程下，采用原子操作。此处不能使用lock
                errorFlag = true;
                //更新UI
                this.ErrorUpdateAction(_errorCount, "");
                //更新Log
                string errInfo;
                errInfo = string.Format("历史数据计算引擎错误{0}：删除计算周期内的实时数据错误!", this.errorCount.ToString()) + Environment.NewLine;
                //logHelper.Error(errInfo);                
                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                currentCalcuItems[0].fid,
                                                                currentCalcuItems[0].fmodulename,
                                                                startdate.ToString(),
                                                                enddate.ToString()
                                                                );
                logHelper.Fatal(errInfo);
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
            string[] sourcetagnames = Regex.Split(currentCalcuItems[0].sourcetagname, ";|；");
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
                Debug.WriteLine("--ParallelReadRealData has started ........");
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
                            //测试，看能否正确划分数据
                            //string[][] csvdata = CsvFileReader.Read("D:\\现场数据_岳阳\\岳阳数据检测_并发计算引擎数据分组错误\\#4机9月10日到11日主气压实时值_修正.csv");
                            //pvalues = CSV2PValue(csvdata);
                            //RTDBDAO.ErrorFlag = false;
                            //以上为测试
                            if (RTDBDAO.ErrorFlag)
                            {
                                //更新计数器
                                Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                                errorFlag = true;
                                //更新UI
                                this.ErrorUpdateAction(_errorCount, "");
                                //更新Log
                                string errInfo;
                                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误，RTDBDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算错误详细信息：{0}。", RTDBDAO.ErrorInfo) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                                currentCalcuItems[0].fid,
                                                                                currentCalcuItems[0].fmodulename,
                                                                                startdate.ToString(),
                                                                                enddate.ToString()
                                                                                );
                                logHelper.Fatal(errInfo);

                            }

                            //特别注意:
                            //——当pvalues为空时，应该将pvaluesSpan的每一段置为null。
                            //——如果标签名称中有用逗号分割，并带标志位inner，则仅读取起始时间和截止时间内的原始数据，不在端点时刻进行插值。对于并发计算的数据分割来说，也不存在端点插值。
                            //——如果标签名称中没有逗号分割的标志位inner，则需要在端点进行插值。

                            string[] tagnameStrs = Regex.Split(sourcetagnames[i], ",|，");
                            if (tagnameStrs.Length == 2 && tagnameStrs[1].Trim().ToUpper() == "STEP")
                            {
                                //如果标签名称中含有“inner”标志位，比如01U_UL52-9XI01_F_VALUE,inner，则不在端点进行取前值或插值。仅读取起止时刻点之间的值
                                //pvaluesSpan = SpanPValues4rtdbStep(pvalues, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割    
                            }
                            else
                            {
                                pvaluesSpan = SpanPValues4rtdbStep(pvalues, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割                            
                            }
                            //*********测试：保存分割数据**********
                            //string filePath = "D:\\RealDataExport";
                            //string filename = filePath + "\\" + currentCalcuItems[0].sourcetagname.Replace("\\", "^") + startdate.ToString("_S_yyyy-MM-dd_HH-mm-ss") + enddate.ToString("_E_yyyy-MM-dd_HH-mm-ss") + "_span" + DateTime.Now.ToString("HHmmss") + ".csv";

                            //string [][] csvdata = PValue2CSV(pvaluesSpan);
                            //CsvFileReader.Save(csvdata, filename);
                            //*******以上为测试*********

                            //lock (this.currentReadDataResults)    //顺序取数据，无需对变量加锁
                            //{
                            this.currentReadDataResults[i] = pvaluesSpan;
                            //}
                        }
                        break;
                    case "opc":     //opc之所以要和rdb和rdbset放在一起，是因为opc也是从关系库读取数据，可以使用并发读取功能
                    case "rdb":
                    case "rdbset":  //如果从关系数据库取数据，目前使用的mysql和oracle都支持多线程，可以并发取数据
                        #region 关系库下的并发读取。20181024修改，关系数据库，可以用一条指令直接读取多个id。不需要再进行并发读取。
                        /*
                        //opc在取数据时，是关系库，可以使用并发的取法
                        Parallel.For(0, sourcetagnames.Length, (int i) =>       //对sourcetagnames中的每一个tag，用一个并发的任务单独读取
                        {
                            //初始化，每天一的List<PValue>大概需要15M空间
                            List<PValue> pvaluesParallel = new List<PValue>();
                            int spanNumberParallel = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                            List<PValue>[] pvaluesSpanParallel = new List<PValue>[spanNumberParallel];
                            //读取数据
                            if (currentCalcuItems[0].sourcetagdb == "opc")
                            {
                                pvaluesParallel = OPCDAO.Read(sourcetagnames[i], startdate, enddate);
                                if (OPCDAO.ErrorFlag)
                                {
                                    //更新计数器
                                    Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                                    errorFlag = true;
                                    //更新UI
                                    this.ErrorUpdateAction(_errorCount, "");
                                    //更新Log
                                    string errInfo;
                                    errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误，OPCDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                                    //logHelper.Error(errInfo);
                                    errInfo += string.Format("——计算错误详细信息：{0}。", OPCDAO.ErrorInfo) + Environment.NewLine;
                                    //logHelper.Error(errInfo);
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                                                    currentCalcuItems[0].fid,
                                                                                    currentCalcuItems[0].fmodulename,
                                                                                    currentCalcuItems[0].sourcetagname,
                                                                                    startdate.ToString(),
                                                                                    enddate.ToString()
                                                                                    );
                                    logHelper.Error(errInfo);

                                }
                            }
                            else if (currentCalcuItems[0].sourcetagdb == "rdb" || currentCalcuItems[0].sourcetagdb == "rdbset")
                            {
                                //20180720，在历史计算引擎下面，每个标签是并发读取，所以rdbset暂时不改为PSLDataDAO.ReadMulti。仍然使用PSLDataDAO.Read。在使用中看看效果
                                pvaluesParallel = PSLDataDAO.Read(sourcetagids[i], startdate, enddate);
                                if (PSLDataDAO.ErrorFlag)
                                {
                                    //更新计数器
                                    Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                                    errorFlag = true;
                                    //更新UI
                                    this.ErrorUpdateAction(_errorCount, "");
                                    //更新Log
                                    string errInfo;
                                    errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误，PSLDataDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                                    //logHelper.Error(errInfo);
                                    errInfo += string.Format("——计算错误详细信息：{0}。", PSLDataDAO.ErrorInfo) + Environment.NewLine;
                                    //logHelper.Error(errInfo);
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                                                    currentCalcuItems[0].fid,
                                                                                    currentCalcuItems[0].fmodulename,
                                                                                    currentCalcuItems[0].sourcetagname,
                                                                                    startdate.ToString(),
                                                                                    enddate.ToString()
                                                                                    );
                                    logHelper.Error(errInfo);

                                }
                            }
                            
                            //opc数据在分割时，应该当做实时数据分割。特别注意：
                            //——当pvalues为空时，应该将pvaluesSpanParallel的每一段置为null。
                            //——如果标签名称中有用逗号分割，并带标志位inner，则仅读取起始时间和截止时间内的原始数据，不在端点时刻进行插值。对于并发计算的数据分割来说，也不存在端点插值。
                            //——如果标签名称中没有逗号分割的标志位inner，则需要在端点进行插值。
                            if (currentCalcuItems[0].sourcetagdb == "opc")
                            {
                                string[] tagnameStrs = Regex.Split(sourcetagnames[i], ",|，");
                                if (tagnameStrs.Length == 2 && tagnameStrs[1].Trim().ToUpper() == "STEP")
                                {
                                    //如果标签名称中含有“inner”标志位，比如01U_UL52-9XI01_F_VALUE,inner，则不在端点去前值或者插值，仅取起止时刻点之间的值
                                    //pvaluesSpanParallel = SpanPValues4rtdbStep(pvaluesParallel, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                                }
                                else
                                {
                                    pvaluesSpanParallel = SpanPValues4rtdbStep(pvaluesParallel, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                                }
                            }
                            else
                                pvaluesSpanParallel = SpanPValues4rdb(pvaluesParallel, startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                            
                                                    
                           
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
                        */
                        #endregion

                        #region 20181024修改 一条sql指令读取多个id的新方式。无须再并发读取，不方便调试
                        //1、读取数据
                        List<PValue>[] pvaluesMulti = new List<PValue>[sourcetagnames.Length];
                        //1.1、读取OPC数据
                        if (currentCalcuItems[0].sourcetagdb == "opc")
                        {
                            //OPC目前仅支持一次读取一个opctagid。如果后面需要一次读取多个tagid，请完善OPCDAO.ReadMulti()接口
                            if (sourcetagnames.Length > 1)
                            {
                                //更新计数器
                                Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                                errorFlag = true;
                                //更新UI
                                this.ErrorUpdateAction(_errorCount, "");
                                //更新Log
                                string errInfo;
                                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误，OPCDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算错误详细信息：OPCDAO目前仅支持一次读取一个opctagid。如需一次读取多个opctagid，请完善OPCDAO.ReadMulti()接口！") + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                                currentCalcuItems[0].fid,
                                                                                currentCalcuItems[0].fmodulename,
                                                                                startdate.ToString(),
                                                                                enddate.ToString()
                                                                                );
                                logHelper.Fatal(errInfo);
                            }
                            else if (sourcetagnames.Length == 1)
                            {
                                pvaluesMulti[0] = OPCDAO.Read(sourcetagnames[0], startdate, enddate);
                                if (OPCDAO.ErrorFlag)
                                {
                                    //更新计数器
                                    Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                                    errorFlag = true;
                                    //更新UI
                                    this.ErrorUpdateAction(_errorCount, "");
                                    //更新Log
                                    string errInfo;
                                    errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误，OPCDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                                    //logHelper.Fatal(errInfo);
                                    errInfo += string.Format("——计算错误详细信息：{0}。", OPCDAO.ErrorInfo) + Environment.NewLine;
                                    //logHelper.Fatal(errInfo);
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                                    currentCalcuItems[0].fid,
                                                                                    currentCalcuItems[0].fmodulename,
                                                                                    startdate.ToString(),
                                                                                    enddate.ToString()
                                                                                    );
                                    logHelper.Fatal(errInfo);

                                }

                            }
                        }
                        //1.2、读取关系数据。关系数据有两种定义，一种rdb，一种rdbset。rdb下有可能是一个标签，也有可能是多个标签
                        else if (currentCalcuItems[0].sourcetagdb == "rdb" || currentCalcuItems[0].sourcetagdb == "rdbset")
                        {
                            pvaluesMulti = PSLDataDAO.ReadMulti(sourcetagids, startdate, enddate);  //数据已经分组完成的数据
                            if (PSLDataDAO.ErrorFlag)
                            {
                                //更新计数器
                                Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                                errorFlag = true;
                                //更新UI
                                this.ErrorUpdateAction(_errorCount, "");
                                //更新Log
                                string errInfo;
                                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData错误，PSLDataDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算错误详细信息：{0}。", PSLDataDAO.ErrorInfo) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                                currentCalcuItems[0].fid,
                                                                                currentCalcuItems[0].fmodulename,
                                                                                startdate.ToString(),
                                                                                enddate.ToString()
                                                                                );
                                logHelper.Fatal(errInfo);
                            }
                        }

                        //对数据进行分割
                        //无论是OPC还是关系数据，都采用相同的分割方式
                        int spanNumberParallel = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                        List<PValue>[] pvaluesSpanParallel = new List<PValue>[spanNumberParallel];
                        for (int i = 0; i < sourcetagnames.Length; i++)
                        {
                            string[] tagnameStrs = Regex.Split(sourcetagnames[i], ",|，");
                            if (tagnameStrs.Length == 2 && tagnameStrs[1].Trim().ToUpper() == "STEP")
                            {
                                //如果标签名称中含有“inner”标志位，比如01U_UL52-9XI01_F_VALUE,inner，则不在端点进行取前值或插值。仅读取起止时刻点之间的值
                                //pvaluesSpanParallel = SpanPValues4rtdbStep(pvaluesMulti[i], startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                            }
                            else
                            {
                                pvaluesSpanParallel = SpanPValues4rdb20190318(pvaluesMulti[i], startdate, enddate, currentCalcuItems[0].intervalseconds);   //对数据进行分割
                            }

                            this.currentReadDataResults[i] = pvaluesSpanParallel;
                        }
                        #endregion

                        break;
                    case "noinput": //noinput情况下，跳过读取数据阶段，直接返回空数据                        
                        List<PValue> pvaluesNoTag = new List<PValue>();
                        int spanNumberNoTag = (int)(enddate.Subtract(startdate).TotalSeconds / currentCalcuItems[0].intervalseconds);
                        List<PValue>[] pvaluesSpanNoTag = new List<PValue>[spanNumberNoTag];
                        pvaluesSpanNoTag = SpanPValues4rdb(pvaluesNoTag, startdate, enddate, currentCalcuItems[0].intervalseconds);

                        this.currentReadDataResults[0] = pvaluesSpanNoTag;

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
                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadRealData未知错误!", this.errorCount.ToString()) + Environment.NewLine;
                //logHelper.Fatal(errInfo);
                errInfo += string.Format("——计算错误详细信息：{0}。", ex.ToString()) + Environment.NewLine;
                //logHelper.Fatal(errInfo);
                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                currentCalcuItems[0].fid,
                                                                currentCalcuItems[0].fmodulename,
                                                                startdate.ToString(),
                                                                enddate.ToString()
                                                                );
                logHelper.Fatal(errInfo);
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
            string[] condpsltagnames = Regex.Split(currentCalcuItems[0].fcondpslnames, ";|；");
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
                string[] condExpressionStr = Regex.Split(currentCalcuItems[0].fcondexpression, ";|；");
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
                    if (PSLDataDAO.ErrorFlag)
                    {
                        //更新计数器
                        Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                        errorFlag = true;
                        //更新UI
                        this.ErrorUpdateAction(_errorCount, "");
                        //更新Log
                        string errInfo;
                        errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadCondData错误，PSLDataDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                        //logHelper.Fatal(errInfo);
                        errInfo += string.Format("——计算错误详细信息：{0}。", PSLDataDAO.ErrorInfo) + Environment.NewLine;
                        //logHelper.Fatal(errInfo);
                        errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                        currentCalcuItems[0].fid,
                                                                        currentCalcuItems[0].fmodulename,
                                                                        startdate.ToString(),
                                                                        enddate.ToString()
                                                                        );
                        logHelper.Fatal(errInfo);

                    }

                    //去掉spanserie的截止时刻值
                    if (spanserie != null && spanserie.Count != 0) spanserie.RemoveAt(spanserie.Count - 1);

                    //将时间序列结果进行分段:对条件时间序列进行分段的函数，要求输入不能有截止时刻，必须在传入之前删除截止时刻值
                    //特别注意，如果读取的pvalues为空，不能把pvaluesSpan。这样与实时计算不一致。
                    //——当pvalues为空时，应该将pvaluesSpanParallel的每一段置为null。
                    //if (spanserie != null && spanserie.Count != 0)
                    filterSpan = SpanPValues4SpanFilter20190318(spanserie, startdate, enddate, currentCalcuItems[0].intervalseconds);
                    //else
                    //    filterSpan = null;
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
                        if (condpsltagnames[i] == "CURRENTSPAN")     //注意这里不要使用标签id判断。标签id可能会变。
                        {
                            //tagid=100，表示要取当前计算周期时间段，对应特殊标签名CurrentSpan，通常用于取反时用于范围限制，如{0}!{100}，在当前计算周期的范围内对tagid=100代表的时间序列取反
                            pvaluesParallel.Add(new PValue(enddate.Millisecond - startdate.Millisecond, startdate, enddate, 0));
                            pvaluesParallel.Add(new PValue(0, enddate, enddate, 0));

                        }
                        else
                        {
                            pvaluesParallel = PSLDataDAO.Read(condpsltagids[i], startdate, enddate);
                        }
                        if (PSLDataDAO.ErrorFlag)
                        {
                            //更新计数器
                            Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                            errorFlag = true;
                            //更新UI
                            this.ErrorUpdateAction(_errorCount, "");
                            //更新Log
                            string errInfo;
                            errInfo = string.Format("历史数据计算引擎错误{0}：并发读取实时数据ParallelReadCondData错误，PSLDataDAO层出错!", this.errorCount.ToString()) + Environment.NewLine;
                            //logHelper.Fatal(errInfo);
                            errInfo += string.Format("——计算错误详细信息：{0}。", PSLDataDAO.ErrorInfo) + Environment.NewLine;
                            //logHelper.Fatal(errInfo);
                            errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                            currentCalcuItems[0].fid,
                                                                            currentCalcuItems[0].fmodulename,
                                                                            startdate.ToString(),
                                                                            enddate.ToString()
                                                                            );
                            logHelper.Fatal(errInfo);

                        }
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

                    //将逻辑运算的结果进行分段：对条件时间序列进行分段的函数，要求输入不能有截止时刻，必须在传入之前删除截止时刻值
                    //特别注意，如果读取的filterspan为空，不能把filterSpanParallel置为null。这样与实时计算不一致。
                    //——当filterspan为空时，应该将filterSpanParallel的每一段置为null。
                    filterSpanParallel = SpanPValues4SpanFilter(filterspan, startdate, enddate, currentCalcuItems[0].intervalseconds);

                    //将分段结果付给全局变量
                    if (filterSpanParallel == null || filterSpanParallel.Count() == 0)
                    {
                        this.currentCondReadDataResults = null;       //理论上不应该存在filterSpanParallel整体为空的情况。必须是filterSpanParallel的每一段为空。
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
                errInfo = string.Format("历史数据计算引擎错误{0}：并发读取条件数据ParallelReadCondData未知错误!", this.errorCount.ToString()) + Environment.NewLine;
                //logHelper.Fatal(errInfo);
                errInfo += string.Format("——计算错误详细信息：{0}。", ex.ToString()) + Environment.NewLine;
                //logHelper.Fatal(errInfo);
                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                currentCalcuItems[0].fid,
                                                                currentCalcuItems[0].fmodulename,
                                                                startdate.ToString(),
                                                                enddate.ToString()
                                                                );
                logHelper.Fatal(errInfo);
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
        private void ParallelCalcu(List<PSLCalcuItem> currentCalcuItems, DateTime startdate, DateTime enddate, int intervalseconds, int spanIndex, int totalspan)
        {
            //——对于一天的数据，对于项拥有共同的tagid和intervalsecond的SelectedItems项进行并发计算
            //——SelectedItems的startIndex到endIndex项，当前并发计算要处理的计算项
            //——previousDayRealDataResulte是并发计算要处理的数据。该数据是List<PValue>[spanCount][inputNumber]二维数组，spanCount是1天的时间按计算间隔划分的时间段，inputNumber是计算公式对应的输入个数
            //——并发计算项是SelectedItems的startIndex到endIndex项，在1天划分的每个时间段spanCount上执行一遍算法的并发。
            var sw = Stopwatch.StartNew();
            Debug.WriteLine("--ParallelCalcu task has started ........");
            if (spanIndex == 0 || spanIndex == totalspan + 1) return; //如果是第一个循环，此时还处于读取数据阶段。直接跳过。如果是最后一个循环，处于写最后一个结果阶段。跳过。
            if (this.previousReadDataResults == null || this.previousReadDataResults.Length == 0)   //当前计算数据为空，直接跳过
            {
                Debug.WriteLine("--ParallelCalcu task has skipped ........");

                //在当前的计算结构下，实时数据会在起始时刻和截止时刻插值，概化数据应该在计算周期时刻位置均有待状态位的值，
                //另外，整个并发周期源数据为空时，会转换为每个计算间隔为空，因而正常情况下，不应该出现整个并发周期为null的情况
                //因此，如果整个并发周期都没有数据，属于重大错误

                //更新计数器
                Interlocked.Increment(ref this._errorCount);//多线程下，采用原子操作。此处不能使用lock                
                //更新Log                           
                string errInfo;
                errInfo = string.Format("历史数据计算引擎错误{0}：整个并发周期读取数据为空，不应该出现该情况，数据转换错误！并发周期为空的情况下，应该转换为每个分段为空。", this._errorCount.ToString()) + Environment.NewLine;
                //logHelper.Fatal(errInfo);                
                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                    currentCalcuItems[0].fid,
                                                    currentCalcuItems[0].fmodulename,
                                                    startdate,
                                                    enddate
                                        );
                logHelper.Fatal(errInfo);
                //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                this.ErrorUpdateAction(this._errorCount, "");
                //策略
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
            if (parallelcalculist.Length % (Environment.ProcessorCount - 1) == 0)
                maxNumberEachPartion = (int)parallelcalculist.Length / (Environment.ProcessorCount - 1);
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
                        if (this.previousReadDataResults[parallelcalculist[i].spanIndex] == null ||                     //整个分段数据为空
                            this.previousReadDataResults[parallelcalculist[i].spanIndex].Length == 0 ||               //整个分段数据长度为0
                            Array.IndexOf(this.previousReadDataResults[parallelcalculist[i].spanIndex], null) != -1     //分段里有个别数据为null，计算结果不一定为空。
                            )   //当前计算数据为空，根据源数据类型，记报警或者记错误，将空数据传入计算
                        {
                            //如果标签本身在配置时被配置为不保存，则该标签一定没有数据。没有数据即为正常状态。
                            //在实时计算条件下，依靠pslcalcuitem.fsourtagflags[i] 来判断，不报警。
                            //但是在并行计算条件下，这里不好写判断条件，仍然报警。但是不影响计算。
                            if (currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagdb == "rtdb" || currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagdb == "opc")
                            {
                                //***********在实时数据情况下，读入数据整体为空，记报错，
                                Debug.WriteLine("--ParallelCalcu task{0} has skipped ........", i);
                                //更新计数器
                                Interlocked.Increment(ref this._errorCount);  //读算写并发多线程下，采用原子操作。此处不能使用lock
                                errorFlag = true;
                                //更新Log
                                string errInfo;
                                errInfo = string.Format("历史数据计算引擎错误{0}：并发计算内部，分时间段读取数据为空，对应时间段内没有实时数据!", this._errorCount.ToString()) + Environment.NewLine;
                                //logHelper.Error(errInfo);                               
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}，数据为空的标签是:第{4}个。",
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                            parallelcalculist[i].startdate.ToString(),
                                                            parallelcalculist[i].enddate.ToString(),
                                                            Array.IndexOf(this.previousReadDataResults[parallelcalculist[i].spanIndex], null).ToString()
                                                            );
                                logHelper.Error(errInfo);
                                //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                                this.ErrorUpdateAction(this._errorCount, "");
                            }
                            else
                            {
                                //***********在概化数据情况下，读入数据整体为空，只记报警
                                //Debug.WriteLine("--ParallelCalcu task{0} has skipped ........", i);   //20181111，概化数据下只报警不跳过，因此不再显示skipped
                                //更新计数器
                                Interlocked.Increment(ref this._warningCount);  //多线程下，采用原子操作。此处不能使用lock
                                _warningFlag = true;
                                //更新log
                                string warningInfo;
                                warningInfo = string.Format("历史数据计算引擎警告{0}：并发计算内部，分时间段读取数据为空，对应时间段内没有概化数据！", this._warningCount.ToString()) + Environment.NewLine;
                                //logHelper.Warn(warningInfo);
                                if (Array.IndexOf(currentCalcuItems[parallelcalculist[i].selectItemsIndex].fsourtagflags, false) != -1)
                                {
                                    warningInfo += string.Format("——当前计算项源标签中的一些标签，在其所属计算项中被配置为不保存结果！此处报警或可以忽略！") + Environment.NewLine;
                                }
                                warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}，数据为空的标签是:第{4}个(从0开始)。",
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                            parallelcalculist[i].startdate.ToString(),
                                                            parallelcalculist[i].enddate.ToString(),
                                                            Array.IndexOf(this.previousReadDataResults[parallelcalculist[i].spanIndex], null).ToString()
                                                            );
                                logHelper.Warn(warningInfo);
                                //更新UI:放在log后面。log紧跟锁定值，记录更精确。UI对数据统计精确要求不高               
                                this.WarningUpdateAction(this._warningCount, "");

                            }
                        }
                        //else if (Array.IndexOf(this.previousReadDataResults[parallelcalculist[i].spanIndex], null) != -1)   //分段里有个别数据为null，计算结果不一定为空。这里虽然报警，但是计算仍可能有结果。
                        //当前计算数据不为空，根据源数据类型，记报警或者记错误，将空数据传入计算
                        //{ 

                        //}



                        //获得一般属性或方法，要通过实例
                        //Object obj = assembly.CreateInstance(APPConfig.NAMESPACE_CALCUMODULE + "." + SelectedItems[calculist[i].selectItemsIndex].fmodulename);

                        //1.2、条件过滤
                        //使用限值时间序列对输出数据进行过滤    
                        //特别注意previousCondReadDataResults是在ParallelReadCondData()中已经处理好的计算条件时间段。
                        //计算条件时间段，在ParallelReadCondData中先分条件数量大于1或者等于1两种情况读取。然后合并。
                        //再将最终合并的有效时间段拆分到每个小周期内
                        //这里得到的previousCondReadDataResults是对用到每个计算周期内，真正有效的条件时间段

                        //注意这里与实时计算不同。实时计算的条件处理在并行计算中被分为两部分。
                        //条件时间段的读取和逻辑计算，放到了条件读取函数
                        //用条件时间段过滤实时数据，仍在这里实现。

                        //获取filterThreshold。
                        string[] condExpressionStr = Regex.Split(currentCalcuItems[parallelcalculist[i].selectItemsIndex].fcondexpression, ";|；");
                        int filterThreshold;
                        if (condExpressionStr.Length > 1)
                            filterThreshold = int.Parse(condExpressionStr[1]);  //如果表达式给了最小时间段阈值设定值，就用设定值
                        else
                            filterThreshold = APPConfig.CALCUMODULE_THRESHOLD;  //如果表达式不给定最小时间段阈值设定值，则采用默认值CALCUMODULE_THRESHOLD


                        //使用过滤条件对输入进行过滤
                        if (currentCalcuItems[parallelcalculist[i].selectItemsIndex].fcondpsltagids != null && currentCalcuItems[parallelcalculist[i].selectItemsIndex].fcondpsltagids.Length > 0)
                        {

                            if (this.previousCondReadDataResults == null)   //当前过滤时间整体为空
                            {
                                //更新计数器
                                Interlocked.Increment(ref this._errorCount);//多线程下，采用原子操作。此处不能使用lock                                
                                //更新Log                           
                                string errInfo;
                                errInfo = string.Format("历史数据计算引擎错误{0}：整个并发周期条件数据为空，不应该出现该情况，数据转换错误！并发周期为空的情况下，应该转换为每个分段为空。", this._errorCount.ToString()) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                    currentCalcuItems[0].fid,
                                                                    currentCalcuItems[0].fmodulename,
                                                                    startdate,
                                                                    enddate
                                                        );
                                logHelper.Fatal(errInfo);
                                //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                                this.ErrorUpdateAction(this._errorCount, "");
                                //策略
                                this.currentResultsDic = null;  //当前并发计算时间段内（比如对于实时数据的一天，或者对于概化数据的一个月），没有任何数据，也就没有任何计算结果
                                return;
                            }
                            else if (this.previousCondReadDataResults[parallelcalculist[i].spanIndex] == null ||     //previousCondReadDataResults对应的当前时间段内为空
                                     this.previousCondReadDataResults[parallelcalculist[i].spanIndex].Count == 0
                                    )
                            {
                                //if (true)     //用于测试时，屏蔽log，以避免速度太慢
                                //{
                                Debug.WriteLine("--ParallelCalcu task{0} has skipped ........", i);
                                //更新计数器
                                Interlocked.Increment(ref this._warningCount);  //多线程下，采用原子操作。此处不能使用lock
                                _warningFlag = true;
                                //更新log
                                string warningInfo;
                                warningInfo = string.Format("历史数据计算引擎警告{0}：分时间段过滤条件时间序列为空，对应时间段内没有符合条件的过滤时间段！", this._warningCount.ToString()) + Environment.NewLine;
                                //logHelper.Warn(warningInfo);
                                warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                            parallelcalculist[i].startdate.ToString(),
                                                            parallelcalculist[i].enddate.ToString()
                                                            );
                                logHelper.Warn(warningInfo);
                                //更新UI:放在log后面。log紧跟锁定值，记录更精确。UI对数据统计精确要求不高               
                                this.WarningUpdateAction(this._warningCount, "");
                                //策略
                                //如果当前时间段条件整体为空，则过滤后的输入数据应该为空，记录log，并以空数据进入计算
                                this.previousReadDataResults[parallelcalculist[i].spanIndex] = null;      //这里仅仅是一天并发周期中的一个时间段，如果该时间段没有数据，则仅仅是该时间段没有计算结果。也就是不向currentResultsDic增加数据
                                goto CURRENTCalcu;
                                //}
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
                                Interlocked.Increment(ref this._warningCount);  //多线程下，采用原子操作。此处不能使用lock
                                _warningFlag = true;
                                //更新log
                                string warningInfo;
                                warningInfo = string.Format("历史数据计算引擎警告{0}：分时间段内经条件时间序列过滤后的数据为空！", this._warningCount.ToString()) + Environment.NewLine;
                                //logHelper.Warn(warningInfo);
                                warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                            currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                            parallelcalculist[i].startdate.ToString(),
                                                            parallelcalculist[i].enddate.ToString()
                                                            );
                                logHelper.Warn(warningInfo);
                                //更新UI:放在log后面。log紧跟锁定值，记录更精确。UI对数据统计精确要求不高               
                                this.WarningUpdateAction(this._warningCount, "");
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
                        //20180528，算法内部的错误标志，改用算法内部变量，然后在结果中返回。不再使用静态变量。静态变量在并行计算中，会造成竞争错误。
                        //注意，Calcu方法有重载。在并发计算中，不能用静态变量传参。这里需要指明获得带两个参数的Calcu重载方法。这一点和实时计算引擎不同。
                        //PropertyInfo ErrorFlag = calcuclass.GetProperty("errorFlag");                   //获得算法类的静态参数ErrorFlag，bool类型
                        //PropertyInfo ErrorInfo = calcuclass.GetProperty("errorInfo");                   //获得算法类的静态参数ErrorInfo，string类型
                        //PropertyInfo WarnFlag = calcuclass.GetProperty("warningFlag");                  //获得算法类的静态参数warningFlag，bool类型
                        //PropertyInfo WarnInfo = calcuclass.GetProperty("warningInfo");                  //获得算法类的静态参数warningInfo，string类型

                        //inputData.SetValue(null, this.previousReadDataResults[parallelcalculist[i].spanIndex]);              //将输入数据给入算法。在并发计算中，使用静态属性赋值，有线程安全问题。
                        //calcuInfo.SetValue(null, new CalcuInfo(currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,      //源标签名称   在并发计算中，使用静态属性赋值，有线程安全问题。
                        //currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,        //计算模件名称
                        //currentCalcuItems[parallelcalculist[i].selectItemsIndex].fparas,             //计算参数
                        //parallelcalculist[i].startdate,                                              //计算起始时间
                        //parallelcalculist[i].enddate));                                              //计算结束时间
                        List<PValue>[] inputs = this.previousReadDataResults[parallelcalculist[i].spanIndex];
                        CalcuInfo currentCalcu = new CalcuInfo(currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagname,            //源标签名称
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].fsourtagids,            //源标签id
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].fsourtagflags,          //源标签是否保存
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,            //计算模件名称
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].fparas,                 //计算参数
                                                               parallelcalculist[i].startdate,                                                  //计算起始时间
                                                               parallelcalculist[i].enddate,                                                    //计算结束时间
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagmrb,           //下限
                                                               currentCalcuItems[parallelcalculist[i].selectItemsIndex].sourcetagmre            //上限
                                                               );
                        Results Results = (Results)Calcu.Invoke(null, new Object[2] { inputs, currentCalcu });                       //在并发计算引擎中，必须直接调用带参数的Calcu。不能使用静态属性传参，否则就存在线程安全问题。

                        if (Results.fatalFlag)
                        {
                            //如果计算组件的fatalFlag为true则说明发生计算错误
                            //更新计数器
                            Interlocked.Increment(ref this._errorCount);        //fatal错误记在errorcount中
                            //更新Log                           
                            string errInfo;
                            errInfo = string.Format("历史数据计算引擎错误{0}：计算内部错误!", this._errorCount.ToString()) + Environment.NewLine;
                            //logHelper.Fatal(errInfo);
                            errInfo += string.Format("——计算错误详细信息：{0}。", Results.fatalInfo) + Environment.NewLine;
                            //logHelper.Fatal(errInfo);
                            errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                                parallelcalculist[i].startdate.ToString(),
                                                                parallelcalculist[i].enddate.ToString()
                                                                );
                            logHelper.Fatal(errInfo);
                            //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                            this.ErrorUpdateAction(this._errorCount, "");
                            //策略
                            //不跳过计算过程，要写默认结果      
                        }
                        if (Results.errorFlag)
                        {
                            //如果计算组件的errorflag为true则说明发生计算错误
                            //更新计数器
                            Interlocked.Increment(ref this._errorCount);
                            //更新Log                           
                            string errInfo;
                            errInfo = string.Format("历史数据计算引擎错误{0}：计算内部错误!", this._errorCount.ToString()) + Environment.NewLine;
                            //logHelper.Error(errInfo);
                            errInfo += string.Format("——计算错误详细信息：{0}。", Results.errorInfo) + Environment.NewLine;
                            //logHelper.Error(errInfo);
                            errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                                currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                                parallelcalculist[i].startdate.ToString(),
                                                                parallelcalculist[i].enddate.ToString()
                                                                );
                            logHelper.Error(errInfo);
                            //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                            this.ErrorUpdateAction(this._errorCount, "");
                            //策略
                            //不跳过计算过程，要写默认结果                           
                        }
                        else if (Results.warningFlag)
                        {

                            //更新计数器
                            Interlocked.Increment(ref this._warningCount);  //多线程下，采用原子操作。此处不能使用lock                                                      
                            //更新log                            
                            string warningInfo;
                            warningInfo = string.Format("历史数据计算引擎警告{0}：并发计算报警，计算组件内部报警!", this._warningCount.ToString()) + Environment.NewLine;
                            //logHelper.Info(warningInfo);
                            warningInfo += string.Format("——计算报警详细信息：{0}。", Results.warningInfo) + Environment.NewLine;
                            //logHelper.Info(warningInfo);
                            warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                        currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                        currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                        parallelcalculist[i].startdate.ToString(),
                                                        parallelcalculist[i].enddate.ToString()
                                                        );
                            logHelper.Warn(warningInfo);
                            //更新UI:放在log后面。log紧跟锁定值，记录更精确。UI对数据统计精确要求不高               
                            this.WarningUpdateAction(this._warningCount, "");
                            //不跳过计算过程，要写默认结果

                        }
                        //end 3、计算：主算法

                        //4、处理结算结果。写入存储变量currentResultsDic
                        List<PValue>[] results = Results.results;
                        if (results != null && results.Length > 0)
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
                            //for (int iPointOutput = 0; iPointOutput < currentCalcuItems[parallelcalculist[i].selectItemsIndex].foutputnumber; iPointOutput++)
                            //20181030日，改为按照实际计算结果数量来循环。但是必须在算法内部保证实际计算结果数量只能小于等与定义好的foutputnumber
                            for (int iPointOutput = 0; iPointOutput < results.Length; iPointOutput++)
                            {
                                //如果对应的计算为Y，则保存其结果
                                if (currentCalcuItems[parallelcalculist[i].selectItemsIndex].falgorithmsflagbool[iPointOutput] &&       //计算结果对应标志位为1
                                    results[iPointOutput] != null &&                                                                    //计算结果列表不为空
                                    results[iPointOutput].Count > 0 &&                                                                  //计算结果列表count大于0
                                    Array.IndexOf(results[iPointOutput].ToArray(), null) == -1                                          //计算结果列表元素不含null
                                    )
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
                        else
                        {
                            //results == null的情况
                            //在目前的计算结构中，除“值次读取算法”MReadShift外，不允许任何计算组件的任何一次计算结果整体为空。但允许部分结算结果为空（比如关于有效时间段序列的计算结果）。
                            //如果出现计算结果整体为null，说明计算组件内部的结算结果处理不正常。需要计错误。并及时查找算法内部的bug。   
                            if (currentCalcuItems[parallelcalculist[i].selectItemsIndex].foutputpermitnull != true)
                            {
                                //更新计数器
                                Interlocked.Increment(ref this._errorCount);
                                //更新Log                           
                                string errInfo;
                                errInfo = string.Format("历史数据计算引擎错误{0}：并发计算错误，计算结果整体为空!", this._errorCount.ToString()) + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算错误详细信息：{0}。", "计算结果整体为空。在计算组件的设计中，任何情况下，不允许计算结果整体为空！") + Environment.NewLine;
                                //logHelper.Fatal(errInfo);
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                    currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                                    currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                                    parallelcalculist[i].startdate.ToString(),
                                                                    parallelcalculist[i].enddate.ToString()
                                                                    );
                                logHelper.Fatal(errInfo);
                                //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                                this.ErrorUpdateAction(this._errorCount, "");
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        //更新计数器
                        Interlocked.Increment(ref this._errorCount);  //多线程下，采用原子操作。此处不能使用lock
                        errorFlag = true;
                        //更新Log                           
                        string errInfo;
                        errInfo = string.Format("历史数据计算引擎错误{0}：读算写错误中未知错误!", this._errorCount.ToString()) + Environment.NewLine;
                        //logHelper.Fatal(errInfo);
                        errInfo += string.Format("——详细错误信息：{0}", ex.ToString()) + Environment.NewLine;
                        //logHelper.Fatal(errInfo);
                        errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                   currentCalcuItems[parallelcalculist[i].selectItemsIndex].fid,
                                                                   currentCalcuItems[parallelcalculist[i].selectItemsIndex].fmodulename,
                                                                   parallelcalculist[i].startdate.ToString(),
                                                                   parallelcalculist[i].enddate.ToString()
                                                                   );
                        logHelper.Fatal(errInfo);
                        //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                        this.ErrorUpdateAction(this._errorCount, "");
                        //策略
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
        private void WritePSLData(List<PSLCalcuItem> currentCalcuItems, DateTime startdate, DateTime enddate)
        {
            var sw = Stopwatch.StartNew();
            Debug.WriteLine("--WritePSLData has started:" + sw.Elapsed.ToString());

            if (this.previousCalcuResults != null && this.previousCalcuResults.Length != 0)
            {

                bool writeflag = PSLDataDAO.WriteHistoryCalcuResults(startdate, enddate, this.previousCalcuResults);
                if (!writeflag || PSLDataDAO.ErrorFlag)
                {
                    //更新计数器
                    Interlocked.Increment(ref _errorCount);  //多线程下，采用原子操作。此处不能使用lock
                    errorFlag = true;
                    //更新LOG
                    string errInfo;
                    errInfo = string.Format("历史数据计算引擎错误{0}：写计算结果WritePSLData错误！", this._errorCount.ToString());
                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。",
                                                                                currentCalcuItems[0].fid,
                                                                                currentCalcuItems[0].fmodulename,
                                                                                startdate.ToString(),
                                                                                enddate.ToString()
                                                                                );
                    logHelper.Fatal(errInfo);    //错误详细信息记录在DAO中   
                    //更新UI：放到log后面。UI显示不要求精确。log紧跟锁定计数更精确。
                    this.ErrorUpdateAction(this._errorCount, "");
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
            try
            {
                //inputList为多个标签（二维数组第一维）的，不同分割时段（二维数组第二维）List<PValue>值
                //——首先考虑inputList整体为空
                if (inputList != null)
                {
                    //——考虑inputList部分标签为空：在公式计算中，这种情况下，仍然有可能有计算结果。比如多变量求平均。当其中某一个变量为空时，采用其他变量
                    if (inputList.Count(n => n != null) != 0)
                    {
                        //不是所有标签均为空的情况下，找一个不为空的标签，找长度，即span数量
                        int spannumber = 0;
                        for (int i = 0; i < inputList.Length; i++)
                            if (inputList[i] != null && inputList[i].Length != 0)
                            { spannumber = inputList[i].Length; break; }

                        //根据span数据建立输出数组
                        List<PValue>[][] outputList = new List<PValue>[spannumber][];

                        //进行转置
                        for (int i = 0; i < spannumber; i++)
                        {
                            outputList[i] = new List<PValue>[inputList.Length];     //outputList[i]对应每个span
                            for (int j = 0; j < inputList.Length; j++)
                            {
                                if (inputList[j] != null && inputList[j].Length != 0)
                                    outputList[i][j] = inputList[j][i];
                                else
                                    outputList[i][j] = null;
                            }
                        }
                        return outputList;
                    }
                    else
                    {
                        //——inputList所有标签均为null的情况
                        return null;
                    }
                }
                else
                {
                    //——inputList整体为null的情况
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("转置过程中出现错误，请调试检查！！");
                return null;
            }
        }
        //计算标签数据分割，针对实时数据，将实时数据视为折线，有截止时刻值。分割的结果也有截止时刻值。特别注意rtdb和opc均用这种方式分割。opc数据视为实时数据。        
        public List<PValue>[] SpanPValues4rtdbLinear(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds, bool InterploationTerminal)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，只有两种结果，第一种至少有两个值，即至少有一个起始时刻有效数据点和一个截止时刻值。另一种结果就是null。
            //——不为空的情况下pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——不为空的情况下pvalue的各个数据点起始时间和截止时间一定前后相连！！！
            //——在具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐。如果不对齐，需要对数据进行插值。这个插值与实时数据库对起始时刻和截止时刻插值相同。

            //*************请使用主界面的“并行时间分割spanPValues4rtdbToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);      //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。

            //********仅用于测试*********
            //List<string> resultsStr = new List<string>();
            //**************************

            if (pvalues != null && pvalues.Count != 0)              //如果数据源整体为空，则直接走下面的分支，返回每个分段均为null的数据。否则每个分段必有数据
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());        //分隔点   
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点，即起始时候向后第一个间隔位置
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果。只要在读取数据的时候不出错。pvaluesSpan必然不会整体为空。可以所有的分段为空。

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                //每一个点分三种情况处理：
                //——第一种情况，当前点pvalues[ipoint]没有超过下一个分割位置
                //——第二种情况，当前点pvalues[ipoint]刚好在下一个分割位置
                //——第三种情况，当前点pvalues[ipoint]超过了下一个分割位置
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当前点的时刻小于下一个分隔时刻时，应该将该点计入该段
                        //——首先要判断该段是否开始
                        //——如果该段已经开始（即已经有点，count！=0），则修改上一个点的结束时刻，再添加当前点
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0) pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint].Timestamp;
                        //——如果该段未开始（还没有点，count=0），则说明当前点是开始点，直接添加。（开始点，由于做了初始化 pvaluesSpan[spanIndex] = new List<PValue>()，所以pvaluesSpan不再为空，而是count=0）
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 

                            //resultsStr.Add("End--:" + pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString());        //测试
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空。实际上这种情况不会出现
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)                    //最后一个点是原数据的截止时刻点，如果已经到截止时刻点，就不再添加新段
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //当前点超出下一个分割时刻位置，该情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                        {
                            //当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去找结束点。                            
                            //——首先，修改当前时间段最后一个有效数据的结束位置为当前分割位置。
                            //——然后，应该根据isolatedata时刻前后的点的值对isolatedata时刻得值进行计算。给上一个时间段添加截止时刻点，即timestamp和enddate都一致的结束点。                            
                            //——最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段并增加分割点。
                            //——回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                            PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                            if (InterploationTerminal)                                                                                          //如果端点插值标志位为true，则在端点处插值。插值用上一个点的状态位
                                lastpvalue = PValue.Interpolate(pvalues[ipoint - 1], pvalues[ipoint], isolatedata);                             //按照线性，计算边界时刻isolatedata的值，                            
                            else                                                                                                                //如果端点插值标志位为false，则无需在端点处插值。则使用上一个值作为端点值
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);
                            pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status));     //给上当前时间段添加截止时刻值

                            //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                            //准备新一时间段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint - 1].Status));    //给当前段，添加起始点。用计算间隔点的值，作为边界点值。用计算间隔时间，作为起始时间。用当前点时间做结束时间。

                            //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }



                    }//结束三种情况分支

                }//结束for
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点                
                return pvaluesSpan;
            }
            else
            {
                //如果数据源整体为空，则直接返回每个分段均为null的数据
                return pvaluesSpan;         //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空           
            }

        }
        //计算标签数据分割，针对实时数据，将实时数据视为离散阶梯型，有截止时刻值。分割的结果也有截止时刻值。特别注意rtdb和opc均用这种方式分割。opc数据视为实时数据。
        public List<PValue>[] SpanPValues4rtdbStep(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，只有两种结果，第一种至少有两个值，即至少有一个起始时刻有效数据点和一个截止时刻值。另一种结果就是null。
            //——不为空的情况下pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——不为空的情况下pvalue的各个数据点起始时间和截止时间一定前后相连！！！
            //——在具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐。如果不对齐，需要对数据进行插值。这个插值与实时数据库对起始时刻和截止时刻插值相同。

            //*************请使用主界面的“并行时间分割spanPValues4rtdbToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);      //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。

            //********仅用于测试*********
            //List<string> resultsStr = new List<string>();
            //**************************

            if (pvalues != null && pvalues.Count != 0)              //如果数据源整体为空，则直接走下面的分支，返回每个分段均为null的数据。否则每个分段必有数据
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());        //分隔点   
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点，即起始时候向后第一个间隔位置
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果。只要在读取数据的时候不出错。pvaluesSpan必然不会整体为空。可以所有的分段为空。

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                //每一个点分三种情况处理：
                //——第一种情况，当前点pvalues[ipoint]没有超过下一个分割位置
                //——第二种情况，当前点pvalues[ipoint]刚好在下一个分割位置
                //——第三种情况，当前点pvalues[ipoint]超过了下一个分割位置
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当前点的时刻小于下一个分隔时刻时，应该将该点计入该段
                        //——首先要判断该段是否开始
                        //——如果该段已经开始（即已经有点，count！=0），则修改上一个点的结束时刻，再添加当前点
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint].Timestamp;
                        //——如果该段未开始（还没有点，count=0），则说明当前点是开始点，直接添加。（开始点，由于做了初始化 pvaluesSpan[spanIndex] = new List<PValue>()，所以pvaluesSpan不再为空，而是count=0）
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 

                            //resultsStr.Add("End--:" + pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString());        //测试
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空。实际上这种情况不会出现
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)                    //最后一个点是原数据的截止时刻点，如果已经到截止时刻点，就不再添加新段
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //当前点超出下一个分割时刻位置，该情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                        {
                            //当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去找结束点。                            
                            //——首先，修改当前时间段最后一个有效数据的结束位置为当前分割位置。
                            //——然后，应该根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。                            
                            //——最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，并用上一个时刻点的值作为起始点值。
                            //——回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                            PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                            //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                            //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                            //else                                                                                                                
                            lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                            pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                            //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                            //准备新一时间段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, lastpvalue.Status));

                            //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }



                    }//结束三种情况分支

                }//结束for
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点                
                return pvaluesSpan;
            }
            else
            {
                //如果数据源整体为空，则直接返回每个分段均为null的数据
                return pvaluesSpan;         //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空           
            }
        }
        //计算标签数据分割，针对实时数据，将实时数据视为离散阶梯型，有截止时刻值。分割的结果也有截止时刻值。特别注意rtdb和opc均用这种方式分割。opc数据视为实时数据。

        //*******20190322，下面的修改可能有误，实时数据应该不存在连续不连续的问题。所有，实时数据的分割改回原来的SpanPValues4rtdbStep
        //*******——实时数据一定是连续的。即上一个数据截止时刻，应该是下一个数据起始时刻。
        //20190317修改，原分割方式，应对不连续数据有问题。进行修正
        /*
        public List<PValue>[] SpanPValues4rtdbStep20190317(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，只有两种结果，第一种至少有两个值，即至少有一个起始时刻有效数据点和一个截止时刻值。另一种结果就是null。
            //——不为空的情况下pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——不为空的情况下pvalue的各个数据点起始时间和截止时间一定前后相连！！！
            //——在具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐。如果不对齐，需要对数据进行插值。这个插值与实时数据库对起始时刻和截止时刻插值相同。

            //*************请使用主界面的“并行时间分割spanPValues4rtdbToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);      //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。

            //********仅用于测试*********
            //List<string> resultsStr = new List<string>();
            //**************************

            if (pvalues != null && pvalues.Count != 0)              //如果数据源整体为空，则直接走下面的分支，返回每个分段均为null的数据。否则每个分段必有数据
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());        //分隔点   
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点，即起始时候向后第一个间隔位置
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果。只要在读取数据的时候不出错。pvaluesSpan必然不会整体为空。可以所有的分段为空。

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                //每一个点分三种情况处理：
                //——第一种情况，当前点pvalues[ipoint]没有超过下一个分割位置
                //——第二种情况，当前点pvalues[ipoint]刚好在下一个分割位置
                //——第三种情况，当前点pvalues[ipoint]超过了下一个分割位置
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当前点的时刻小于下一个分隔时刻时，应该将该点计入该段
                        //——首先要判断该段是否开始
                        //——如果该段已经开始（即已经有点，count！=0），则修改上一个点的结束时刻，再添加当前点
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint-1].Endtime;
                        //——如果该段未开始（还没有点，count=0），则说明当前点是开始点，直接添加。（开始点，由于做了初始化 pvaluesSpan[spanIndex] = new List<PValue>()，所以pvaluesSpan不再为空，而是count=0）
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 

                            //resultsStr.Add("End--:" + pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString());        //测试
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空。实际上这种情况不会出现
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)                    //最后一个点是原数据的截止时刻点，如果已经到截止时刻点，就不再添加新段
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //pvalues[ipoint].Timestamp > isolatedata 
                        //当前点超出下一个分割时刻位置，该情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                        {
                            //当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去找结束点。                            
                            //——首先，修改当前时间段最后一个有效数据的结束位置：这里又分为两种情况：
                            //————如果上一个点结束位置大于isolatedata：
                            //——————在isolatedata结束上一个段最后一个数据。
                            //——————然后，根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。                            
                            //——————最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，并用上一个时刻点的值作为起始点值。
                            //——————回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            //————如果上一个点结束位置小于等于isolatedata：
                            //——————在pvalues[ipoint - 1].Endtime结束上一个段最后一个数据。
                            //——————然后，根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。
                            //——————最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，但是不添加新点
                            //——————回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            if (pvalues[ipoint - 1].Endtime > isolatedata)
                            {
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                                PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                                //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                                //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                                //else                                                                                                                
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                                pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                                //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                                //准备新一时间段数据
                                spanIndex = spanIndex + 1;
                                pvaluesSpan[spanIndex] = new List<PValue>();
                                pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, lastpvalue.Status));

                                //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                                //调到新的边界时刻，对当前点再次进行判断
                                isolatedata = isolatedata.AddSeconds(intervalseconds);

                                //回到当前点
                                goto currentPoint;
                            }
                            else
                            { 
                                //pvalues[ipoint - 1].Endtime < isolatedata
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                                PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                                //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                                //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                                //else                                                                                                                
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                                pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                                //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                                //准备新一时间段数据
                                spanIndex = spanIndex + 1;
                                pvaluesSpan[spanIndex] = new List<PValue>();
                                //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));

                                //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                                //调到新的边界时刻，跳过当前点
                                isolatedata = isolatedata.AddSeconds(intervalseconds);

                                //回到当前点
                                goto currentPoint;
                            }
                            
                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }



                    }//结束三种情况分支

                }//结束for
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点                
                return pvaluesSpan;
            }
            else
            {
                //如果数据源整体为空，则直接返回每个分段均为null的数据
                return pvaluesSpan;         //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空           
            }
        }
        */

        //********特别注意针对关系数据库的划分SpanPValues4rdb与针对实时数据SpanPValues4rtdb的划分有重大区别***************
        //********——实时数据划分，如果一段分割时间内没有值，应该采用前值。
        //********——关系数据划分，如果一段分割时间内没有值，应该采用null。
        //计算标签数据分割，针对关系数据，有截止时刻值。分割的结果也有截止时刻值。rdb和rdbset均用这种方式分割。
        public List<PValue>[] SpanPValues4rdb(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将关系数据分隔成 1days/intervalseconds段
            //——注意经rdbhelper.GetRawValues()读出的数据，pvalue的第一点起始时间，和最后一点的结束时间，有可能与入参startdate和enddate对齐。也有可能在startdate和enddate内。这点与实时数据不同
            //——概化数据的数据分割，一般都是诸如，一下取一整天的分钟数据，分割成每小时。一下取30天的小时数据，分割到每一天。因此，不存在跨分割点的数据点。
            //——但是在主引擎发生不可控错误是，有可能缺少部分周期的数据。
            //——再具体分割时，只需要将对应的点放入对应的段，无需在分割点插值。

            //*************请使用主界面的“并行时间分割SpanPValuesToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds); //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。


            if (pvalues != null && pvalues.Count > 2)
            {
                //大于两点时的分割方法
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果，不存在单独某一段数据为null的情况。要么整个pvaluesSpan为空，要么每一段都有数据

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当当前点的时间小于下一个分隔时，应该将该点计入该段
                        //——首先要判断该段是否开始，如果未开始，在先初始化

                        if (pvaluesSpan[spanIndex] == null)
                        {
                            //——如果该段未开始，则说明当前点是开始点，先初始化
                            pvaluesSpan[spanIndex] = new List<PValue>();
                        }
                        else
                        {
                        }
                        //添加当前点
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));  //概化数据，状态位有效！！！
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
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
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else  //pvalues[ipoint].Timestamp > isolatedata
                    {
                        //当前点超出边界点，在概化数据的情况下，由于边界点必然有值，除非不可控错误导致边界点没值。所以，当发生该情况时，一定是边界点因为不可控错误，缺少对应时刻的值造成的。
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该首先去结束上面一段。然后添加截止时刻点。
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] == null || pvaluesSpan[spanIndex].Count == 0)
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。什么都没有。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——对于概化数据，不存在跨分隔点的数据，因此说明当前段已经结束，需要过渡到新段。
                            //pvaluesSpan[spanIndex] = null;
                            spanIndex = spanIndex + 1;
                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去结束上面一段。并添加截止时刻点。
                            //——首先，说明上一点已经自然结束，并且一定不会超过分割点。（见上面解释，概化数据不存在跨越分割点的情况），
                            //——添加截止时刻点
                            pvaluesSpan[spanIndex].Add(new PValue(pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Value, isolatedata, isolatedata, 0));
                            spanIndex = spanIndex + 1;
                        }

                        //对于概化数据来说，还要开始新的一段。
                        //又分两种情况，
                        //——当前点的时间戳大于当前分割点isolatedata，但小于下一个分割点isolatedata
                        //——当前点的时间戳大于当前分割点isolatedata，也大于下一个分割点isolatedata.AddSeconds(intervalseconds)
                        //准备新一段数据。不在isolatedata位置进行插值，直接将新的点加入新的段
                        if (pvalues[ipoint].Timestamp < isolatedata.AddSeconds(intervalseconds))
                        {
                            //如果当前点起始时刻大于当前分割点，但小于下一个分割点说明。说明加1过的当前分段内应该有值。
                            //此时直接跳转到下一个分割点，对当前值进行判断。（这里理解非常重要，用测试程序来理解）
                            //pvaluesSpan[spanIndex] = new List<PValue>();
                            //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));    //给
                        }
                        else
                        {

                        }
                        //调到新的边界时刻，对当前点再次进行判断
                        isolatedata = isolatedata.AddSeconds(intervalseconds);
                        goto currentPoint;

                    }

                }
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点
                return pvaluesSpan;
            }
            else if (pvalues != null && pvalues.Count == 2)
            {

                //等于2点的时候的分割方法
                //在pvalues长度为2的时候，只有起始点和截止点。用这两个点插出其余个点的值
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果，不存在单独某一段数据为null的情况。要么整个pvaluesSpan为空，要么每一段都有数据

                //处理起始点
                pvaluesSpan[spanIndex] = new List<PValue>();
                pvaluesSpan[spanIndex].Add(pvalues[0]);

                //循环处理从1到Count-1点
                for (ipoint = 1; ipoint < spanNumber; ipoint++)
                {

                    pvaluesSpan[spanIndex][0].Endtime = isolatedata;                            //修改上一段起始点的结束时刻
                    PValue endPoint = PValue.Interpolate(pvalues[0], pvalues[1], isolatedata);  //找结束时刻PValue点。该点的状态采用上一点的状态                    
                    pvaluesSpan[spanIndex].Add(new PValue(endPoint.Value, endPoint.Timestamp, endPoint.Endtime, endPoint.Status));     //添加上一段的截止点不能直接用endPoint

                    spanIndex++;
                    pvaluesSpan[spanIndex] = new List<PValue>();                                //创建新一段
                    pvaluesSpan[spanIndex].Add(new PValue(endPoint.Value, endPoint.Timestamp, endPoint.Endtime, endPoint.Status));     //给新一段添加新点。不能直接用endPoint

                    isolatedata = isolatedata.AddSeconds(intervalseconds);
                }

                //处理最后一个段的结束点
                pvaluesSpan[spanIndex][0].Endtime = enddate;
                pvaluesSpan[spanIndex].Add(pvalues[1]);

                return pvaluesSpan;
            }
            else
            {
                return pvaluesSpan; //此时pvaluesSpan = new List<PValue>[spanNumber]; 刚好每一段均为空              
            }

        }
        public List<PValue>[] SpanPValues4rdb20190318(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将关系数据分隔成 1days/intervalseconds段
            //——注意经rdbhelper.GetRawValues()读出的数据，pvalue的第一点起始时间，和最后一点的结束时间，有可能与入参startdate和enddate对齐。也有可能在startdate和enddate内。这点与实时数据不同
            //——概化数据的数据分割，一般都是诸如，一下取一整天的分钟数据，分割成每小时。一下取30天的小时数据，分割到每一天。因此，不存在跨分割点的数据点。
            //——但是在主引擎发生不可控错误是，有可能缺少部分周期的数据。
            //——再具体分割时，只需要将对应的点放入对应的段，无需在分割点插值。

            //*************请使用主界面的“并行时间分割SpanPValuesToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds); //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。


            if (pvalues != null && pvalues.Count > 2)
            {
                //大于两点时的分割方法
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果，不存在单独某一段数据为null的情况。要么整个pvaluesSpan为空，要么每一段都有数据

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当当前点的时间小于下一个分隔时，应该将该点计入该段
                        //——首先要判断该段是否开始，如果未开始，在先初始化

                        if (pvaluesSpan[spanIndex] == null)
                        {
                            //——如果该段未开始，则说明当前点是开始点，先初始化
                            pvaluesSpan[spanIndex] = new List<PValue>();
                        }
                        else
                        {
                        }
                        //添加当前点
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));  //概化数据，状态位有效！！！
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
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
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else  //pvalues[ipoint].Timestamp > isolatedata
                    {
                        //当前点超出边界点，在概化数据的情况下，由于边界点必然有值，除非不可控错误导致边界点没值。所以，当发生该情况时，一定是边界点因为不可控错误，缺少对应时刻的值造成的。
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该首先去结束上面一段。然后添加截止时刻点。
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] == null || pvaluesSpan[spanIndex].Count == 0)
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。什么都没有。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——对于概化数据，不存在跨分隔点的数据，因此说明当前段已经结束，需要过渡到新段。
                            //pvaluesSpan[spanIndex] = null;
                            spanIndex = spanIndex + 1;
                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去结束上面一段。并添加截止时刻点。
                            //——首先，说明上一点已经自然结束，并且一定不会超过分割点。（见上面解释，概化数据不存在跨越分割点的情况），
                            //——添加截止时刻点
                            pvaluesSpan[spanIndex].Add(new PValue(pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Value, isolatedata, isolatedata, 0));
                            spanIndex = spanIndex + 1;
                        }

                        //对于概化数据来说，还要开始新的一段。
                        //又分两种情况，
                        //——当前点的时间戳大于当前分割点isolatedata，但小于下一个分割点isolatedata
                        //——当前点的时间戳大于当前分割点isolatedata，也大于下一个分割点isolatedata.AddSeconds(intervalseconds)
                        //准备新一段数据。不在isolatedata位置进行插值，直接将新的点加入新的段
                        if (pvalues[ipoint].Timestamp < isolatedata.AddSeconds(intervalseconds))
                        {
                            //如果当前点起始时刻大于当前分割点，但小于下一个分割点说明。说明加1过的当前分段内应该有值。
                            //此时直接跳转到下一个分割点，对当前值进行判断。（这里理解非常重要，用测试程序来理解）
                            //pvaluesSpan[spanIndex] = new List<PValue>();
                            //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));    //给
                        }
                        else
                        {

                        }
                        //调到新的边界时刻，对当前点再次进行判断
                        isolatedata = isolatedata.AddSeconds(intervalseconds);
                        goto currentPoint;

                    }

                }
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点
                return pvaluesSpan;
            }
            else if (pvalues != null && pvalues.Count == 2)
            {

                //等于2点的时候的分割方法
                //在pvalues长度为2的时候，只有起始点和截止点。用这两个点插出其余个点的值
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果，不存在单独某一段数据为null的情况。要么整个pvaluesSpan为空，要么每一段都有数据

                //处理起始点
                pvaluesSpan[spanIndex] = new List<PValue>();
                pvaluesSpan[spanIndex].Add(pvalues[0]);

                //循环处理从1到Count-1点
                for (ipoint = 1; ipoint < spanNumber; ipoint++)
                {

                    pvaluesSpan[spanIndex][0].Endtime = isolatedata;                            //修改上一段起始点的结束时刻
                    PValue endPoint = PValue.Interpolate(pvalues[0], pvalues[1], isolatedata);  //找结束时刻PValue点。该点的状态采用上一点的状态                    
                    pvaluesSpan[spanIndex].Add(new PValue(endPoint.Value, endPoint.Timestamp, endPoint.Endtime, endPoint.Status));     //添加上一段的截止点不能直接用endPoint

                    spanIndex++;
                    pvaluesSpan[spanIndex] = new List<PValue>();                                //创建新一段
                    pvaluesSpan[spanIndex].Add(new PValue(endPoint.Value, endPoint.Timestamp, endPoint.Endtime, endPoint.Status));     //给新一段添加新点。不能直接用endPoint

                    isolatedata = isolatedata.AddSeconds(intervalseconds);
                }

                //处理最后一个段的结束点
                pvaluesSpan[spanIndex][0].Endtime = enddate;
                pvaluesSpan[spanIndex].Add(pvalues[1]);

                return pvaluesSpan;
            }
            else
            {
                return pvaluesSpan; //此时pvaluesSpan = new List<PValue>[spanNumber]; 刚好每一段均为空              
            }

        }
        //计算条件数据分割，针对关系数据，但是没有截止时刻值。分割的结果也没有截止时刻值
        public List<PValue>[] SpanPValues4SpanFilter(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues没有截止时刻点。传入之前删掉

            //将条件数据分隔成 calcuPeriod/intervalseconds段
            //——注意经rdbhelper.GetRawValues()读出的概化条件数据，其数据是符合条件的有效时间段序列，数据的值是有效时间段的长度（毫秒数）\            
            //————pvalue的最早时间和最晚时间，与入参startdate和enddate不一定对齐。
            //————时间段之间也不一定连续。
            //——在具体分割时，需要把这些不连续的时间段，按照分割时间分割成组
            //*************请使用主界面的“并行时间分割ToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds); //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。并且初始化后每个pvaluesSpan[ipoint]均为null
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。
            if (pvalues != null && pvalues.Count != 0)
            {
                //初始化
                DateTime isolatedata = DateTime.Parse(startdate.ToString());
                isolatedata = isolatedata.AddSeconds(intervalseconds);          //初始化为第一分割点
                int spanIndex = 0;                                              //分段指针，第0段
                int ipoint = 0;                                                 //被分割数据指针

                //try   //这个try语句仅调试用，正常情况下，如果分隔出错，并行计算应该停止。因为正确的分割数据对于并行计算是关键
                //{
                //循环处理从0到Count-1点

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
                            //——如果该段已经开始，当前情况下，该段最后一个点的截止时刻无需调整
                            //pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;
                        }
                        //——添加当前点
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //未创建新段spanIndex，未移动分割点isolatedata，判断下一个源数据时间段next
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明当前段最后一个有效时间段的结束时间肯定在边界之前。当前段应该结束 
                        //并且创建新的段

                        if (pvaluesSpan[spanIndex] == null || pvaluesSpan[spanIndex].Count == 0)
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            //pvaluesSpan[spanIndex] = null;

                        }
                        else
                        {
                            //*****filter不需要修改上一个点的结束时刻。因为filter的各个时间段之间不连贯。当前点是开始点，但是上一个点的结束时刻可能和当前点开始时刻不一致。
                            //pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;     //说明当前段结束，修改上次结束时间。时间序列无需修改上一次结束点
                            //*****filter不需要添加截止时刻点
                            //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, isolatedata, isolatedata, 0));         //添加当前段截止时刻值。时间序列不需要添加截止时刻值。

                        }

                        //创建新段。
                        //if (ipoint != pvalues.Count - 1)      //条件时间数据，没有截止时刻，最后一个点也应该照常执行
                        //{
                        spanIndex = spanIndex + 1;                      //创建新段
                        pvaluesSpan[spanIndex] = new List<PValue>();    //初始化新段，并添加点
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                        //添加点完毕后，并不清楚当前点是否在该段结束，应当继续用当前点取比较
                        isolatedata = isolatedata.AddSeconds(intervalseconds);
                        //}

                        //创建新段spanIndex+，移动分割点isolatedata.Add，判断下一个源数据时间段next
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
                        //创建新段spanIndex+，移动分割点isolatedata.Add，仍使用当前点goto currentPoint
                    }//结束三种情况的分支

                    //下一个点
                }//end for
                //添加截止时刻值。
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //用原pvalues的截止时刻作为最后一个分段的截止时刻。因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点
                return pvaluesSpan;

                //}     //这个try语句仅调试用，正常情况下，如果分隔出错，并行计算应该停止。因为正确的分割数据对于并行计算是关键
                //catch (Exception ex)
                //{
                //    int temp = 10;  
                //}
            }
            else
            {
                //如果pvalues没有数据，则对应pvaluesSpan对应所有分段内均为null。直接返回这个所有段均为null的值     
                return pvaluesSpan; //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空
            }//endif pvalues有数据或者没有数据
        }
        public List<PValue>[] SpanPValues4SpanFilter20190318(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues没有截止时刻点。传入之前删掉

            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，只有两种结果，第一种至少有两个值，即至少有一个起始时刻有效数据点和一个截止时刻值。另一种结果就是null。
            //——不为空的情况下pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——不为空的情况下pvalue的各个数据点起始时间和截止时间一定前后相连！！！
            //——在具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐。如果不对齐，需要对数据进行插值。这个插值与实时数据库对起始时刻和截止时刻插值相同。

            //*************请使用主界面的“并行时间分割spanPValues4rtdbToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);      //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。

            //********仅用于测试*********
            //List<string> resultsStr = new List<string>();
            //**************************

            if (pvalues != null && pvalues.Count != 0)              //如果数据源整体为空，则直接走下面的分支，返回每个分段均为null的数据。否则每个分段必有数据
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());        //分隔点   
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点，即起始时候向后第一个间隔位置
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果。只要在读取数据的时候不出错。pvaluesSpan必然不会整体为空。可以所有的分段为空。

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                //每一个点分三种情况处理：
                //——第一种情况，当前点pvalues[ipoint]没有超过下一个分割位置
                //——第二种情况，当前点pvalues[ipoint]刚好在下一个分割位置
                //——第三种情况，当前点pvalues[ipoint]超过了下一个分割位置
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当前点的时刻小于下一个分隔时刻时，应该将该点计入该段
                        //——首先要判断该段是否开始
                        //——如果该段已经开始（即已经有点，count！=0），则修改上一个点的结束时刻，再添加当前点
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;
                        //——如果该段未开始（还没有点，count=0），则说明当前点是开始点，直接添加。（开始点，由于做了初始化 pvaluesSpan[spanIndex] = new List<PValue>()，所以pvaluesSpan不再为空，而是count=0）
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            //条件不添加截止点
                            //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 

                            //resultsStr.Add("End--:" + pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString());        //测试
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空。实际上这种情况不会出现
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)                    //最后一个点是原数据的截止时刻点，如果已经到截止时刻点，就不再添加新段
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //pvalues[ipoint].Timestamp > isolatedata 
                        //当前点超出下一个分割时刻位置，该情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                        {
                            //当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去找结束点。                            
                            //——首先，修改当前时间段最后一个有效数据的结束位置：这里又分为两种情况：
                            //————如果上一个点结束位置大于isolatedata：
                            //——————在isolatedata结束上一个段最后一个数据。
                            //——————然后，根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。                            
                            //——————最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，并用上一个时刻点的值作为起始点值。
                            //——————回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            //————如果上一个点结束位置小于等于isolatedata：
                            //——————在pvalues[ipoint - 1].Endtime结束上一个段最后一个数据。
                            //——————然后，根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。
                            //——————最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，但是不添加新点
                            //——————回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            if (pvalues[ipoint - 1].Endtime > isolatedata)
                            {
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                                PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                                //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                                //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                                //else  
                                //条件不添加截止点                                                                              
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                                //pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                                //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                                //准备新一时间段数据
                                spanIndex = spanIndex + 1;
                                pvaluesSpan[spanIndex] = new List<PValue>();
                                pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, lastpvalue.Status));

                                //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                                //调到新的边界时刻，对当前点再次进行判断
                                isolatedata = isolatedata.AddSeconds(intervalseconds);

                                //回到当前点
                                goto currentPoint;
                            }
                            else
                            {
                                //pvalues[ipoint - 1].Endtime < isolatedata
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                                PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                                //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                                //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                                //else                                                                                                                
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                                //条件不添加截止点
                                //pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                                //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                                //准备新一时间段数据
                                spanIndex = spanIndex + 1;
                                pvaluesSpan[spanIndex] = new List<PValue>();
                                //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));

                                //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                                //调到新的边界时刻，跳过当前点
                                isolatedata = isolatedata.AddSeconds(intervalseconds);

                                //回到当前点
                                goto currentPoint;
                            }

                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }



                    }//结束三种情况分支

                }//结束for
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点                
                return pvaluesSpan;
            }
            else
            {
                //如果数据源整体为空，则直接返回每个分段均为null的数据
                return pvaluesSpan;         //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空           
            }
        }

        //csv数据转pvalue数据
        private List<PValue> CSV2PValue(string[][] csvdata)
        {
            List<PValue> pvalues = new List<PValue>();
            for (int i = 0; i < csvdata.Length - 1; i++)
            {
                pvalues.Add(new PValue(double.Parse(csvdata[i][0]), DateTime.Parse(csvdata[i][1]), DateTime.Parse(csvdata[i + 1][1]), int.Parse(csvdata[i][2])));
            }
            int lastIndex = csvdata.Length - 1;
            pvalues.Add(new PValue(double.Parse(csvdata[lastIndex][0]), DateTime.Parse(csvdata[lastIndex][1]), DateTime.Parse(csvdata[lastIndex][1]), int.Parse(csvdata[lastIndex][2]))); //添加截止时刻值
            return pvalues;
        }
        //pvalue转csv
        private string[][] PValue2CSV(List<PValue>[] pvalues)
        {
            try
            {
                //首先把pvalues展平
                List<PValue> allpvalues = new List<PValue>();
                for (int i = 0; i < pvalues.Length; i++)
                {
                    for (int j = 0; j < pvalues[i].Count; j++)
                    {
                        allpvalues.Add(pvalues[i][j]);
                    }
                }

                //把有效数据一次转为csv
                string[][] csvdata = new string[allpvalues.Count][];
                for (int i = 0; i < allpvalues.Count; i++)
                {
                    csvdata[i] = new string[3];
                    csvdata[i][0] = allpvalues[i].Value.ToString();
                    csvdata[i][1] = allpvalues[i].Timestamp.ToString("yyyy/MM/dd HH:mm:ss");
                    csvdata[i][2] = allpvalues[i].Status.ToString();
                }

                return csvdata;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
