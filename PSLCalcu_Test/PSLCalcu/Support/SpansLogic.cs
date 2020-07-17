using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon; //使用PValue

namespace PSLCalcu
{
    /// <summary>
    /// SpanSeries
    /// 时间段逻辑运算 
    /// ——对时间段进行与或非等逻辑运算
    /// ——其中，对源数据进行过滤的SpansFilter()函数，将源数据inputs视为阶梯型离散数据。在数据被分割时，使用阶梯型方法进行分割。追加结束点时，也使用
    /// 版本：1.1
    ///    
    /// 修改纪录
    ///     2018.04.02 版本：1.2 截止时刻值问题已处理。
    ///     2017.07.28 版本：1.1 改为类封装。
    ///		2017.06.16 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.16</date>
    /// </author> 
    /// </summary>
    public class SpanSeries 
    {
        public DateTime Datetime{ get; set; }
        public string Flag { get; set; }
        public SpanSeries(DateTime datetime, string flag)
        {
            this.Datetime = datetime;
            this.Flag = flag;
        }        
    }
    public class SpanFilterSeries 
    {
        public DateTime Datetime { get; set; }
        public string Flag { get; set; }
        public double Value { get; set; }
        public long Status { get; set; }
        public SpanFilterSeries(DateTime datetime, string flag, double value, long status)
        {
            this.Datetime = datetime;
            this.Flag = flag;
            this.Value = value;
            this.Status = status;
        }
        //二级排序，先按照Datetime升序排。如果Datetime相同，再按照Flag排序，inputs和filters排前面。inpute和filtere排后面。  
        //SpanFilterSeries的对字段排序使用lambda表示式完成。    
       
    }
    public class SpanLogic
    {
        #region 公有变量
        public static bool ErrorFlag = false;                                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        public static string ErrorInfo = "";
        #endregion

        ///功能：计算两个时间段数组的相互重叠的部分（时间与）
        ///spanSerials1：第一个时间段组，每个元素由起始时间和截止时间组成，格式如[[startTime1,endTime1],[startTime2,endTime2],,,,]
        ///spanSerials2：第二个时间段组，格式同上。	
        ///spanThreshold：小时间过滤，重叠的时间必须大于spanThreshold，才记入结果。spanThreshold单位为分钟
        ///所有时间逻辑运算的时间序列值，均不带截止时刻值去除功能。因此要求传入的时间序列本身不带截止时刻值。
        public static List<PValue> SpansAnd(List<PValue> spans1, List<PValue> spans2, int spanthreshold = 0)
        {
            //对两个时间序列中的每个时间做处理：
            //1、时间字符串中的-替换为/；
            //2、将时间字符串变为以ms为单位的长整形；
            //3、添加属性，用于标记该时间属于哪个时间序列，属于起始时间还是截止时间
            //将两个时间序列的处理后的内容，添加到一个总时间序列中

            double spanthresholdm = spanthreshold * 60 * 1000;//将阈值由分钟转为毫秒
            List<SpanSeries> spanconcat = new List<SpanSeries>();
            foreach (PValue pv in spans1)
            {
                spanconcat.Add(new SpanSeries (pv.Timestamp, "s1"));
                spanconcat.Add(new SpanSeries (pv.Endtime, "e1"));
            }
            foreach (PValue pv in spans2)
            {
                spanconcat.Add(new SpanSeries (pv.Timestamp, "s2"));
                spanconcat.Add(new SpanSeries (pv.Endtime, "e2"));
            }
            //先按照时间顺序排序，
            //同一时刻如果既是结束时刻（e1，e2），又是开始时刻（s1，s2），则结束e1,e2排在开始s1,s2之前。
            //
            spanconcat = spanconcat.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList();  
            

            List<PValue> result = new List<PValue>();
            bool span1valid = false;
            bool span2valid = false;
            DateTime currentStart =new DateTime();
            DateTime currentEnd = new DateTime();
            foreach (SpanSeries span in spanconcat)
            {
                if (span.Flag == "s1" || span.Flag == "s2")
                {
                    //遇到s1或s2置各自的有效标志位
                    if (span.Flag == "s1") span1valid = true;
                    if (span.Flag == "s2") span2valid = true;
                    //记录最后的startTime
                    currentStart = span.Datetime;
                }
                else
                {
                    //遇到e1或e2，只要两个时间段均有效，则最后一个starttime和当前endtime就是要找的时间段。
                    currentEnd = span.Datetime;
                    if (span1valid && span2valid && ((currentEnd - currentStart).TotalMilliseconds > spanthresholdm))
                    {
                        result.Add(new PValue(1, currentStart, currentEnd, 0));
                    }
                    //遇到e1或e2复位各自的有效标志位
                    if (span.Flag == "e1") span1valid = false;
                    if (span.Flag == "e2") span2valid = false;

                }
            }
            return result;
        }

        /// 功能：计算两个时间段数组的总的覆盖的部分（时间或）
        /// spanSerials1：第一个时间段组，每个元素由起始时间和截止时间结束，格式如[[startTime1,endTime1],[startTime2,endTime2],,,,]
        /// spanSerials2：第二个时间段组，格式同上。
        public static List<PValue> SpansOr(List<PValue> spans1, List<PValue> spans2, int spanthreshold = 0)
        {
            //对两个时间序列中的每个时间做处理：
            //1、时间字符串中的-替换为/；
            //2、将时间字符串变为以ms为单位的长整形；
            //3、添加属性，用于标记该时间属于哪个时间序列，属于起始时间还是截止时间
            //将两个时间序列的处理后的内容，添加到一个总时间序列中
            double spanthresholdm = spanthreshold * 60 * 1000;//将阈值由分钟转为毫秒
            List<SpanSeries> spanconcat = new List<SpanSeries>();
            foreach (PValue pv in spans1)
            {
                spanconcat.Add(new SpanSeries(pv.Timestamp, "s1"));
                spanconcat.Add(new SpanSeries(pv.Endtime, "e1"));
            }
            foreach (PValue pv in spans2)
            {
                spanconcat.Add(new SpanSeries(pv.Timestamp, "s2"));
                spanconcat.Add(new SpanSeries(pv.Endtime, "e2"));
            }
            //先按照时间顺序排序，
            //同一时刻如果既是结束时刻（e1，e2），又是开始时刻（s1，s2），则结束e1,e2排在开始s1,s2之前。
            //
            spanconcat = spanconcat.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList();  
            

            List<PValue> result = new List<PValue>();
            bool span1valid = false;
            bool span2valid = false;
            DateTime currentStart = new DateTime();
            DateTime currentEnd = new DateTime();
            foreach (SpanSeries span in spanconcat)
            {
                if (span.Flag == "s1" || span.Flag == "s2")
                {
                    //只有两个标志位均无效的情况下，遇到s1或s2，才是要找的时间段起始时间
                    if (!span1valid && !span2valid) currentStart = span.Datetime;
                    //遇到s1或s2置各自的有效标志位
                    if (span.Flag == "s1") span1valid = true;
                    if (span.Flag == "s2") span2valid = true;
                }
                else
                {
                    //遇到e1或e2复位各自的有效标志位
                    if (span.Flag == "e1") span1valid = false;
                    if (span.Flag == "e2") span2valid = false;
                    //如果两段时间均无效的情况下，遇到的e1或e2才是截止时间
                    if (!span1valid && !span2valid)
                    {
                        currentEnd = span.Datetime;
                        if ((currentEnd - currentStart).TotalMilliseconds > spanthresholdm)
                            result.Add(new PValue(1, currentStart, currentEnd, 0));
                    }
                }
            }
            return result;

        }

        ///功能：在第一个时间段内，去掉第二个时间段序列中的所有时间段
        ///spanSerials1：第一个时间段，是有效时间组，只有一个元素，由起始时间和截止时间构成，格式如[[startTime1,endTime1]]
        ///spanSerials2：第二个时间段组，是要去掉的时间段组，每个元素均由起始时间和截止时间构成。	
        public static List<PValue> SpansNot(List<PValue> spans1, List<PValue> spans2, int spanthreshold = 0)
        {
            //对两个时间序列中的每个时间做处理：
            //1、时间字符串中的-替换为/；
            //2、将时间字符串变为以ms为单位的长整形；
            //3、添加属性，用于标记该时间属于哪个时间序列，属于起始时间还是截止时间
            //将两个时间序列的处理后的内容，添加到一个总时间序列中
            double spanthresholdm = spanthreshold * 60 * 1000;//将阈值由分钟转为毫秒
            List<SpanSeries> spanconcat = new List<SpanSeries>();
            foreach (PValue pv in spans1)
            {
                spanconcat.Add(new SpanSeries(pv.Timestamp, "s1"));
                spanconcat.Add(new SpanSeries(pv.Endtime, "e1"));
            }
            foreach (PValue pv in spans2)
            {
                spanconcat.Add(new SpanSeries(pv.Timestamp, "s2"));
                spanconcat.Add(new SpanSeries(pv.Endtime, "e2"));
            }
            //先按照时间顺序排序，
            //同一时刻如果既是结束时刻（e1，e2），又是开始时刻（s1，s2），则结束e1,e2排在开始s1,s2之前。
            //
            spanconcat=spanconcat.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList();           

            List<PValue> result = new List<PValue>();
            bool span1valid = false;
            bool span2valid = false;
            DateTime currentStart = new DateTime();
            DateTime currentEnd = new DateTime();
            foreach (SpanSeries span in spanconcat)
            {
                if (span.Flag == "s1" || span.Flag == "s2")
                {
                    //如果span1有效，span2无效情况下，遇s2，就是要找的时间段结束时间
                    if (span1valid && !span2valid)
                    {
                        currentEnd = span.Datetime;
                        if ((currentEnd - currentStart).TotalMilliseconds > spanthresholdm) result.Add(new PValue(1, currentStart, currentEnd, 0));
                    }
                    //如果span1无效，span2无效的情况下，遇到s1，则s1是第一段时间的起点(本情况仅用于处理span1的首部)
                    if (!span1valid && !span2valid) currentStart = span.Datetime;
                    //遇到s1或s2置各自的有效标志位
                    if (span.Flag == "s1") span1valid = true;
                    if (span.Flag == "s2") span2valid = true;
                }
                else
                {
                    //如果span1有效，span2有效的情况下，遇到e2，就是要找的时间段开始时间
                    if (span1valid && span2valid) currentStart = span.Datetime;

                    //如果span1有效，span2无效的情况下，遇到e1，e1就是要找的时间段结束时间（本情况仅用于处理span1的尾部）
                    if (span1valid && !span2valid)
                    {
                        currentEnd = span.Datetime;
                        if ((currentEnd - currentStart).TotalMilliseconds > spanthresholdm) result.Add(new PValue(1, currentStart, currentEnd, 0));
                    }
                    //遇到e1或e2复位各自的有效标志位
                    if (span.Flag == "e1") span1valid = false;
                    if (span.Flag == "e2") span2valid = false;

                }
            }
            return result;
        }

        ///功能：取filterspan时间段内的inputs值
        ///inputs：实时库原始数据，可以是连续的，也可以是非连续的。
        ///filterspan：对inputs进行过滤的时间序列。	
        ///inputs：是要计算的数据，从DAO接口读进来，有截止时间数据，需要先去掉。等过滤完成，再加上。 
        public static bool SpansFilter(ref List<PValue>[] inputs, List<PValue> filterspan, int spanthreshold = 0)
        {
            ErrorFlag = false;
            ErrorInfo = "";
            bool flag = false;
            try
            {
                //先对filterspan用spanthreshold进行过滤。  凡是删除元素的，需要从后向前循环。
                for (int i = filterspan.Count - 1; i >= 0; i--)
                {
                    if (filterspan[i].Timespan < spanthreshold * 60) filterspan.RemoveAt(i);
                }
                //对于过滤时间filterspan，
                //当过滤条件仅有一个时，是直接从数据取出的，带截止时刻值
                //当过滤条件有多个时，是计算得出的。不带截止时刻值。            

                //再用filterspan处理每一个List<PValue>
                for (int i = 0; i < inputs.Length; i++)
                {
                    List<PValue> input = inputs[i];

                    //去掉截止时刻数据
                    if (input.Count > 1) input.RemoveAt(input.Count - 1);

                    //组合成大数组
                    double spanthresholdm = spanthreshold * 60 * 1000;//将阈值由分钟转为毫秒
                    List<SpanFilterSeries> spanconcatSeries = new List<SpanFilterSeries>();
                    foreach (PValue pv in input)
                    {
                        spanconcatSeries.Add(new SpanFilterSeries(pv.Timestamp, "inputs", pv.Value, pv.Status));
                        spanconcatSeries.Add(new SpanFilterSeries(pv.Endtime, "inpute", 0, 0));
                    }
                    foreach (PValue pv in filterspan)
                    {
                        spanconcatSeries.Add(new SpanFilterSeries(pv.Timestamp, "filters", 0, 0));
                        spanconcatSeries.Add(new SpanFilterSeries(pv.Endtime, "filtere", 0, 0));
                    }

                    //先按照时间顺序排序，
                    //同一时刻如果既是结束时刻（inpute，filtere），又是开始时刻（inputs，filters），。
                    //则filtere在最前，filters在第二个，inpute第三，inputs最后
                    //经过测试，只要filtere排在同时刻的inputs前，就不会错。
                    spanconcatSeries = spanconcatSeries.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList();

                    List<PValue> result = new List<PValue>();
                    bool inputvalid = false;
                    bool filtervalid = false;
                    DateTime currentStart = new DateTime();
                    DateTime currentEnd = new DateTime();
                    double currentValue = 0;
                    long currentStatus = 0;
                    foreach (SpanFilterSeries span in spanconcatSeries)
                    {
                        if (span.Flag == "inputs" || span.Flag == "filters")
                        {
                            //遇到input或filter各自的有效位
                            if (span.Flag == "inputs")
                            {
                                inputvalid = true;
                                currentValue = span.Value;
                                currentStatus = span.Status;
                            }
                            if (span.Flag == "filters") filtervalid = true;
                            //记录最后的startTime
                            currentStart = span.Datetime;
                        }
                        else
                        {
                            //遇到e1或e2，只要两个时间段均有效，则最后一个starttime和当前endtime就是要找的时间段。
                            currentEnd = span.Datetime;
                            if (inputvalid && filtervalid && ((currentEnd - currentStart).TotalMilliseconds > spanthresholdm))
                            {
                                result.Add(new PValue(currentValue, currentStart, currentEnd, currentStatus));
                            }
                            //遇到e1或e2复位各自的有效标志位
                            if (span.Flag == "inpute") inputvalid = false;
                            if (span.Flag == "filtere") filtervalid = false;
                        }
                    }

                    //给result添加截止时刻值。只要result不为空的情况下才能添加截止值。
                    if (result != null && result.Count > 0)
                        result.Add(new PValue(result[result.Count - 1].Value, result[result.Count - 1].Endtime, result[result.Count - 1].Endtime, 0));

                    inputs[i] = result;
                }//endfor 遍历每个list
                return flag;
            }
            catch(Exception ex)
            {
                ErrorFlag = true;
                flag = true;
                ErrorInfo = ex.ToString();
                return flag;
            }
            
        }
    }
}
