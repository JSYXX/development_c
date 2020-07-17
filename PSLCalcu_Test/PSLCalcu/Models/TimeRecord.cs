using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu
{
    //计算引擎用于记录时间的对象
    public class TimeRecord
    {
        public string ModuleName;
        public DateTime BeforeReadData;
        public DateTime BeforeReflection;
        public DateTime BeforeFilter;
        public DateTime BeforeCalcu;
        public DateTime BeforeWriteData;
        public DateTime BeforeUpdateCalcuInfo;
        public DateTime EndCurrent;
        public TimeRecord()
        {
            this.BeforeReadData = DateTime.Now;
            this.BeforeReflection = this.BeforeReadData;
            this.BeforeFilter = this.BeforeReadData;
            this.BeforeCalcu = this.BeforeReadData;
            this.BeforeWriteData = this.BeforeReadData;
            this.BeforeUpdateCalcuInfo = this.BeforeReadData;
            this.EndCurrent = this.BeforeReadData;           
        }
    }
    //用于读取时间的对象
    public class PSLTimeRecord
    {
        public string modulename { get; set; }
        public DateTime beforereaddata { get; set; }
        public double beforereaddatams { get; set; }
        public DateTime beforereflection { get; set; }
        public double beforereflectionms { get; set; }
        public DateTime beforefilter { get; set; }
        public double beforefilterms { get; set; }
        public DateTime beforecalcu { get; set; }
        public double beforecalcums { get; set; }
        public DateTime beforewritedata { get; set; }
        public double beforewritedatams { get; set; }
        public DateTime beforeupdatecalcuInfo { get; set; }
        public double beforeupdatecalcuInfoms { get; set; }
        public DateTime endcurrent { get; set; }
        public double endcurrentms { get; set; }
        public double readspan { get; set; }
        public double reflectionspan { get; set; }
        public double filterspan { get; set; }
        public double calcuspan { get; set; }
        public double writespan { get; set; }
        public double updatespan { get; set; }
        public double totalspan { get; set; }
         
       
    }
}
