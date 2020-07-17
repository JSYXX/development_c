using System;

namespace PCCommon
{
    /// <summary>
    /// PCCommon.dll内的PCCommon命名空间
    /// 与工业数据处理相关的基本类。
    /// 
    /// 版本：1.6
    /// 
    /// 特别说明：
    ///     1、PValue的实时值Value，在不同数据库中存储，对数据类型的要求：
    ///         ——在关系型数据库中，存储计算值，要求用双精度浮点型double。比如mysql数据库，double数据类型为8字节，即64bit。
    ///         ——在实时数据库中，存储实时值，一般的非累计点，可选用单精度浮点型，这样可以节省大量的存储空间。当然也可以选择双精度浮点型。对于累积量点，则必须选择双精度浮点型。以防止累积超限。
    ///         ——无论在数据库中如何存储，数据从数据库中读取出来转由PValue处理时，统一都用双精度类型。
    ///     2、PValue的时间戳Timestamp。
    ///         ——对于实时数据，Timestamp表示Value值对应的时刻：
    ///         ——对于计算数据，如果该计算数据代表一个时间段的统计值，Timestamp表示统计值的起始时刻。例如平均值，Timestamp表示该平均值统计的起始时间
    ///         ——对于计算数据，如果该计算数据代表一个时间段内某个特别的特征值，Timestamp代表该特征值的时刻。如最大值，Timestamp表示该最大值对应的时刻。这种情况下，区别不同的最大值（不同点，不同统计时间段）的任务由点的id来完成。
    ///           基于上面的约定，PValue不再对最大值，最小值这类统计值，单设一个时间戳标志。
    ///     3、PValue的值有效性结束时刻Endtime：
    ///         ——对于实时数据，由于采用例外报告的形式，或者由于采样周期的存在，实时数据是离散的。因此Endtime表示当前实时值保持的结束时刻。
    ///         ——对于统计数据，Endtime表示统计值的结束时刻。
    /// 
    /// 修改纪录
    ///     
    ///     2017.6.22 版本：1.2 arrow 修改Timespan为毫秒值;改Timespan为只读属性;移除Log类及log4net等dll。
    ///		2016.5.18 版本：1.1 arrow 修改，整理。
    ///		    -重新讨论和分析PValue各属性的数据类型经。
    /// <author>
    ///		<name>xunxiangmin</name>
    ///		<date>2014.1.18</date>
    /// </author> 
    /// </summary>
    public class PValue:IComparable<PValue>
    {
        public int Tagid { get; set; }              //工业数据实时值、计算值。
        public double Value { get; set; }           //工业数据实时值、计算值。
        public long Status { get; set; }            //工业数据实时值、计算值所对应的状态。
        public DateTime Timestamp { get; set; }     //工业数据实时值、计算值所对应的时间戳。        
        public DateTime Endtime { get; set; }       //工业数据实时值、计算值所对应的值有效性结束时刻。

        private double _timespan;
        public double Timespan                      //工业数据实时值、计算值数据有效周期            
        {
            get
            {
                TimeSpan ts;
                if (Endtime >= Timestamp)
                {
                    ts = Endtime.Subtract(Timestamp);
                    _timespan = ts.TotalMilliseconds;   //2017.6.22，arrow修改，由获取秒数改为获取毫秒数。原因，GPS时间戳精度为毫秒，获取秒数不够精确
                }
                else
                {
                    _timespan = 0;
                }
                return _timespan;
            }
            //set                                       //2017.6.22，arrow修改，将Timespan改为只读属性，不能从外部设置该属性。
            //{
            //    _timespan = value;                    
            //}
        }
        public string Remark { get; set; }          //工业数据实时值，注释项。
        //四种重载方式
        public PValue() { }
        public PValue(double value, DateTime timestamp, long status)
        {
            this.Value = value;
            this.Timestamp = timestamp;
            this.Status = status;
            this.Endtime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        }
        public PValue(double value, DateTime timestamp, DateTime endtime, long status)
        {
            this.Value = value;
            this.Timestamp = timestamp;
            this.Endtime = endtime;
            this.Status = status;
        }
        public PValue(PValue pvalue)
        {
            this.Value = pvalue.Value;
            this.Timestamp = pvalue.Timestamp;
            this.Status = pvalue.Status;
            this.Endtime = pvalue.Endtime;
        }
        //根据前后点，以及中间的指定时刻，计算该时刻值，返回该时刻Pvalue值
        public static PValue Interpolate(PValue before,PValue after,DateTime middledate)
        {
            /*20181009之前版本，必须before.Timestamp<middledate<after.Timestamp
            //如果前一点的时刻与后一点的时刻相等，则数据有问题，返回前一点
            //如果前一点的时刻大于后一点的时刻，则数据有问题，返回前一点
            if (before.Timestamp >= after.Timestamp) return before;
            //如果中间点时刻大于后一点的时刻，则数据有问题，返回前一点
            if (middledate > after.Timestamp) return before;
            //如果中间点时刻等于前一点的时刻，则返回前一点；
            else if (middledate == before.Timestamp) return before;
            //如果中间点时刻等于后一点的时刻，则返回后一点；
            else if (middledate == after.Timestamp) return after;
            //如果在中间
            else
            {
                double middlevalue = before.Value + (after.Value - before.Value) * middledate.Subtract(before.Timestamp).TotalMilliseconds / after.Timestamp.Subtract(before.Timestamp).TotalMilliseconds;
                return new PValue(middlevalue, middledate, middledate, before.Status);
            }
            */

            /*20181009之后版本，不限定before.Timestamp<middledate<after.Timestamp之间的关系*/
            if (before.Timestamp == after.Timestamp)
            {
                if (middledate == before.Timestamp)
                    return before;
                else
                    return new PValue(double.MinValue, before.Timestamp, before.Timestamp, (long)StatusConst.InsertError);
            }
            else if (middledate == before.Timestamp) return before;
            else if (middledate == after.Timestamp) return after;
            else
            {
                //如果middledate != before.Timestamp和middledate != after.Timestamp同时成立，则说明前后三个时间点不相同。
                //则不考虑before.Timestamp和after.Timestamp的大小，也不要求middledate必须在中间。
                //即使before.Timestamp>after.Timestamp，或者middledate在外侧也应该返回该有的值
                double middlevalue = before.Value + (after.Value - before.Value) * middledate.Subtract(before.Timestamp).TotalMilliseconds / after.Timestamp.Subtract(before.Timestamp).TotalMilliseconds;
                return new PValue(middlevalue, middledate, middledate, before.Status);
            }
        }
        //根据前后点，以及中间的指定值，计算该值对应时刻，返回该时刻Pvalue值
        public static PValue Interpolate(PValue before, PValue after, double middlevalue)
        {
            //如果前一点的时刻与后一点的时刻相等，则数据有问题，返回前一点
            //如果前一点的时刻大于后一点的时刻，则数据有问题，返回前一点
            if (before.Timestamp >= after.Timestamp) return before;
            //本算法要在外部保证middle在before.value和after.value之间
            //如果中间点时刻的值等于前一点时刻的值，返回前一点
            if (middlevalue == before.Value) return before;
            //如果中间点时刻的值等于后一点时刻的值，返回后一点
            else if (middlevalue == after.Value) return after;
            //如果在中间
            else
            {
                DateTime middledate = before.Timestamp.AddMilliseconds((middlevalue - before.Value) * (after.Timestamp.Subtract(before.Timestamp).TotalMilliseconds) / (after.Value - before.Value));
                return new PValue(middlevalue, middledate, middledate, 0);
            }
        }
        public int CompareTo(PValue other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
