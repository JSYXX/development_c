using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class CalcuInfo
    {
        public string sourcetagname { get; set; }           //计算使用源数据的标签名：采用“表名.标签名”的格式。数据表pslcalcu的sourcetag字段。
        public System.UInt32[] fsourtagids { get; set; }    //计算使用的源数据的标签名
        public bool[] fsourtagflags { get; set; }           //计算使用的源数据是否存储
        public string fmodulename { get; set; }             //计算公式名称。数据表pslcalcu的fname字段    
        public string fparas { get; set; }                  //计算公式的计算参数。分号分隔。参数不在计算引擎中处理，而是作为字符串整体传入算法，由算法负责解析和使用。
        public DateTime fstarttime { get; set; }            //当次计算对应的起始时间。
        //这个属性初值是数据表pslconfig的starttime字段。
        //后面是根据数据表pslconfig的starttime字段和interval、method字段循环计算的。
        public DateTime fendtime { get; set; }              //当次计算对应的结束时间。
        public double sourcetagmrb { get; set; }            //量程下限
        public double sourcetagmre { get; set; }            //量程上限
        
        public CalcuInfo()
        {
            this.sourcetagname = "";            
            this.fmodulename = "";
            this.fparas = "";
            this.fstarttime = DateTime.Now;
            this.fendtime = DateTime.Now;
            this.sourcetagmrb = 0;
            this.sourcetagmre = 100;
        }
        public CalcuInfo(string sourcetagname, System.UInt32[] tagids, bool[] tagflags,string fmodulename, string fparas, DateTime fstarttime, DateTime fendtime,double mrb,double mre)
        {
            this.sourcetagname =sourcetagname;
            this.fsourtagids = tagids;
            this.fsourtagflags = tagflags;
            this.fmodulename=fmodulename;
            this.fparas=fparas;
            this.fstarttime=fstarttime;
            this.fendtime=fendtime;
            this.sourcetagmrb = mrb;
            this.sourcetagmre = mre;
        }       
    }

}
