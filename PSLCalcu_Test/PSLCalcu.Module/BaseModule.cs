using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    using PCCommon;
    /// <summary>
    /// BaseModule
    /// 单入多出概化计算引擎计算模块基础类。
    /// 
    /// 版本：0.1
    ///  
    /// 修改纪录
    /// 
    ///     2017.02.27 版本：0.1 gaofeng     
    /// <author>
    ///		<name>GaoFeng</name>
    ///		<date>2017.02.27</date>
    /// </author> 
    /// </summary>
    public class SpanSeries
    {
        public DateTime Datetime { get; set; }
        public string Flag { get; set; }
        public string Status { get; set; }
        public SpanSeries(DateTime datetime, string flag,string status)
        {
            this.Datetime = datetime;
            this.Flag = flag;
            this.Status = status;
        }
    }
    public abstract class  BaseModule
    {
       
        public void Filter() 
        {
            
        }
       
    }

    public struct Results 
    {
        public List<PValue>[] results;
        public bool errorFlag;
        public string errorInfo;
        public bool warningFlag;
        public string warningInfo;
        public bool fatalFlag;
        public string fatalInfo;

        public Results(List<PValue>[] _results, bool _errorFlag, string _errorInfo, bool _warningFlag, string _warningInfo,bool _fatalFlag,string _fatalInfo) 
        {
            this.results = _results;
            this.errorFlag = _errorFlag;
            this.errorInfo = _errorInfo;
            this.warningFlag = _warningFlag;
            this.warningInfo=_warningInfo;
            this.fatalFlag = _fatalFlag;
            this.fatalInfo = _fatalInfo;
        }
    }
}
