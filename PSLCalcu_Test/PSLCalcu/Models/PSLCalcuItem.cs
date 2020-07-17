using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;   //使用字符串的正则拆分

namespace PSLCalcu
{
    public class PSLCalcuItem : IComparable<PSLCalcuItem>
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLCalcuItem));         //DAO层log

        #region 公有变量
        public static bool ErrorFlag = false;                                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        #endregion

        //PSLCalcuItem，概化计算配置对象，
        //该对象同时作为pslcalcuconfig表的持久化类，其要从pslcalcuconfig中读取数据的属性，名称必须与pslcalcuconfig数据表的字段名称相同，对应的值才能被ReaderToObject读取到属性中
        //具体见ReaderToObject()函数说明

        public int index { get; set; }                      //pslcalcuitem对象的序号，用于界面更新
        //计算项信息
        public int fid { get; set; }                        //计算序号。计算项的总序号。数据表pslcalcuconfig的fid字段。
        //源标签信息
        public string sourcetagname { get; set; }           //计算使用源数据的标签名：采用“表名.标签名”的格式。数据表pslcalcu的sourcetag字段。
        public string sourcetagdb { get; set; }             //源标签所在数据库类型：来源于底层实时库，还是上层关系型数据仓库。数据表pslcalcu的sourcedb字段。
        public string sourcetagdesc { get; set; }           //源标签描述。数据表pslcalcu的sourcedesc字段。
        public string sourcetagdim { get; set; }            //源标签工程单位。数据表pslcalcu的sourcedim字段。
        public double sourcetagmrb { get; set; }            //源标签量程下限。数据表pslcalcug的mrb字段。
        public double sourcetagmre { get; set; }            //源标签量程上限。数据表pslcalcu的mre字段。

        //采用的计算公式信息：1、名称
        public string fmodulename { get; set; }             //计算公式名称。数据表pslcalcu的fname字段  
        public string fnode { get; set; }                      //计算公式所属节点号。
        public string fgroup { get; set; }                  //计算公式所属计算组。数据表pslcalcu的fgroup字段。（预留）
        public string forder { get; set; }                     //计算公式的计算顺序号。数据表pslcalcu的forder字段。（预留）
        public string fclass { get; set; }                  //计算公式所属类名称。用于反射调用。   该字段从pslmodule表来，因此名称必须用pslmodule表字段名。  

        //计算公式信息：2、算法    
        public string falgorithmsflag { get; set; }          //计算公式的算法有效标志。数据表pslcalcu的falgorithmflag字段。   
        public string fparas { get; set; }                  //计算公式的计算参数。分号分隔。参数不在计算引擎中处理，而是作为字符串整体传入算法，由算法负责解析和使用。
        public string fcondpslnames { get; set; }           //计算公式的条件标签。分号分隔。    
        public string fcondexpression { get; set; }         //计算公式的条件逻辑表达式。

        //计算公式信息：3、结果
        public string foutputtable { get; set; }            //计算公式计算结果输出表名称。数据表pslcalcu的foutputtable字段。
        public int foutputnumber { get; set; }              //计算公式计算结果输出项数量。数据表pslcalcu的foutputnumber字段。 
        public string foutputpsltagnames { get; set; }      //计算公式计算结果输出项名称。数据表pslcalcu的foutputname字段。
        public bool foutputpermitnull { get; set; }         //计算公式计算结果是否允许空值。

        //计算公式信息：4、循环
        public int finterval { get; set; }                  //计算间隔时间。数据表pslcalcu的interval字段。
        public string fintervaltype { get; set; }           //计算间隔周期。数据表pslcalcu的method字段。
        public int fdelay { get; set; }                     //计算延时。数据表pslcalcu的delay字段。      

        //当次计算
        public DateTime fstarttime { get; set; }            //当次计算对应的起始时间。
        //这个属性初值是数据表pslconfig的starttime字段。
        //后面是根据数据表pslconfig的starttime字段和interval、method字段循环计算的。
        public DateTime fendtime { get; set; }              //当次计算对应的结束时间。
        public DateTime fnextstarttime { get; set; }        //下一次计算开始时间。通过方法GetCondEndTime()赋值。


        //扩展属性：计算引擎使用。    
        //public PSLModuleType fmoduletype{ get; set; }         //计算引擎用。由fmodulename决定的枚举类型，用于反射调用。 
        public System.UInt32[] fsourtagids { get; set; }        //计算引擎用。有sourcetagname决定的源数据id号。 
        public bool[] fsourtagflags { get; set; }               //计算引擎用。源数据对应的概化数据标签，在前值计算时，是否进行了保存。
        public System.UInt32[] fcondpsltagids { get; set; }     //计算引擎用。条件psl标签的id号。方便计算引擎快速调用。
        public bool[] fcondpsltagflags { get; set; }            //计算引擎用。条件psl概化数据标签，在前值计算时，是否进行了保存。
        public System.UInt32[] foutputpsltagids { get; set; }   //计算引擎用。计算结果psl标签的id号。方便计算引擎快速调用。
        public bool[] falgorithmsflagbool { get; set; }         //计算引擎用。由falgorithmsflag拆分而来。方便计算引擎快速调用
        //准备扩展属性:准备fmoduletype、falgorithmsflagbool、fcondpsltagids、foutputpsltagids
        public bool Prepare(ref Dictionary<string, System.UInt32> tagname2idmap, ref Dictionary<string, bool> tagname2flagmap)
        {
            string tagname = "";
            try
            {
                //fmoduletype = PSLFunctionTypeConvert.Parse(fmodulename);
                //oftype = "Compress";
                //准备falgorithmsflagbool
                
                char[] flag = this.falgorithmsflag.ToCharArray();
                this.falgorithmsflagbool = new bool[flag.Length];
                for (int i = 0; i < flag.Length; i++)
                {
                    this.falgorithmsflagbool[i] = (flag[i] == 'Y');
                }

                //准备fsourtagids、fsourtagflags
                //——只有源数据是概化计算的才能获得sourcetagid,获得sourcetagid的目的在于取概化数据时更加高效。避免每次都取找id
                if (this.sourcetagname.Trim() != "" )
                {
                    string[] tagnames = Regex.Split(this.sourcetagname, ";|；");
                    this.fsourtagids = new System.UInt32[tagnames.Length];
                    this.fsourtagflags = new bool[tagnames.Length];

                    for (int i = 0; i < tagnames.Length; i++)
                    { 
                        try
                        {   //只有计算数据源为rdb和rdbset时，才能够找到对应的id号
                            this.fsourtagids[i] = tagname2idmap[tagnames[i]];
                            this.fsourtagflags[i] = tagname2flagmap[tagnames[i]];
                        }
                        catch(Exception ex)
                        {                            
                            continue;
                        }
                    }
                }

                //准备fcondpsltagids、fcondpsltagflags
                //如果fcondpslnames==""空，则fcondpsltagids=null。这点在计算主引擎时要注意
                if (this.fcondpslnames.Trim() != "")
                {
                    string messageStr="";
                    string[] condpslnames = Regex.Split(this.fcondpslnames, ";|；");
                    this.fcondpsltagids = new System.UInt32[condpslnames.Length];
                    this.fcondpsltagflags = new bool[condpslnames.Length];

                    for (int i = 0; i < condpslnames.Length; i++)
                    {

                        if (condpslnames[i].Length != 0)
                        {
                            tagname = condpslnames[i];  //用于字典出错时的记录
                            if (tagname2idmap.ContainsKey(condpslnames[i]))
                            {
                                this.fcondpsltagids[i] = tagname2idmap[condpslnames[i]];
                                this.fcondpsltagflags[i] = tagname2flagmap[condpslnames[i]];
                            }
                            else 
                            {
                                ErrorFlag = true;
                                
                                messageStr += String.Format("Model层PSLCalcuItem.Prepare()错误：---------->") + Environment.NewLine;                               
                                messageStr += String.Format("PSLCalcuItem对象：{0}", tagname) + Environment.NewLine;                                
                                messageStr += String.Format("错误信息：标签不在字典中");
                                logHelper.Error(messageStr);
                                return false;
                            }
                        }
                    }
                }//
                
                //准备foutputpsltagids
                string[] outputplstagnames = Regex.Split(this.foutputpsltagnames, ";|；");
                this.foutputpsltagids = new System.UInt32[outputplstagnames.Length];
                for (int i = 0; i < outputplstagnames.Length; i++)
                {
                    if (outputplstagnames[i].Length != 0)
                    {
                        tagname = outputplstagnames[i]; //用于字典出错时的记录
                        if (tagname2idmap.ContainsKey(outputplstagnames[i]))                    //用判断语句替代try，这样效率比较高。如果用try语句等待key值不存在，速度非常慢。
                            this.foutputpsltagids[i] = tagname2idmap[outputplstagnames[i]];
                        else
                        {
                            ErrorFlag = true;
                            string messageStr;
                            messageStr = String.Format("Model层PSLCalcuItem.Prepare()错误：---------->");
                            logHelper.Error(messageStr);
                            messageStr = String.Format("PSLCalcuItem对象：{0}", tagname);
                            logHelper.Error(messageStr);
                            messageStr = String.Format("错误信息：标签不在字典中");
                            logHelper.Error(messageStr);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("Model层PSLCalcuItem.Prepare()错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("PSLCalcuItem对象：{0}", tagname);
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;

            }



        }
        //初始化：根据fstarttime对fendtime、fnexttime初始化
        public void IniDate()
        {
            this.fendtime = GetEndTime();
            this.fnextstarttime = this.fendtime.AddSeconds(this.fdelay);   //fdelay表示的是秒值，如果要修改，需要修改两个位置。主引擎中还有一处
        }

        //扩展属性：界面显示使用   
        public string resolution                        //计算频率：根据计算间隔时间interval和计算间隔周期method，在界面显示计算频率
        {   //仅用于页面显示
            get
            {
                switch (this.fintervaltype.ToLower())
                {
                    case "s":
                    case "second":
                    case "seconds":
                        if(this.finterval == 1)
                            return string.Format("{0} second", this.finterval);
                        else
                            return string.Format("{0} seconds", this.finterval);
                    case "min":
                    case "minute":
                    case "minutes":
                        if (this.finterval == 1)
                            return string.Format("{0} minute", this.finterval);
                        else
                            return string.Format("{0} minutes", this.finterval);
                    case "h":
                    case "hour":
                    case "hours":
                        if (this.finterval == 1)
                            return string.Format("{0} hour", this.finterval);
                        else
                            return string.Format("{0} hours", this.finterval);
                    case "d":
                    case "day":
                    case "days":
                        if (this.finterval == 1)
                            return string.Format("{0} day", this.finterval);
                        else
                            return string.Format("{0} days", this.finterval);
                    case "w":
                    case "week":
                    case "weeks":
                        if (this.finterval == 1)
                            return string.Format("{0} week", this.finterval);
                        else
                            return string.Format("{0} weeks", this.finterval);
                    case "m":
                    case "month":
                    case "months":
                        if (this.finterval == 1)
                            return string.Format("{0} month", this.finterval);
                        else
                            return string.Format("{0} months", this.finterval);
                    case "y":
                    case "year":
                    case "years":
                        if (this.finterval == 1)
                            return string.Format("{0} year", this.finterval);
                        else
                            return string.Format("{0} years", this.finterval);
                    default:
                        if (this.finterval == 1)
                            return string.Format("{0} hour", this.finterval);
                        else
                            return string.Format("{0} hours", this.finterval);
                }
            }
        }
        //扩展属性：计算间隔秒数，注意计算引擎并没有使用该属性进行操作。来推算下一次计算时间！！！！而是使用GetEndTime()方法
        private int _intervalseconds;
        public int intervalseconds
        {
            get
            {
                switch (this.fintervaltype.ToLower())
                {
                    case "s":
                    case "second":
                    case "seconds":
                        return this.finterval;
                    case "min":
                    case "minute":
                    case "minutes":
                        return this.finterval * 60;
                    case "h":
                    case "hour":
                    case "hours":
                        return this.finterval * 60 * 60;
                    case "d":
                    case "day":
                    case "days":
                        return this.finterval * 24 * 60 * 60;
                    case "w":
                    case "week":
                    case "weeks":
                        return this.finterval * 7 * 24 * 60 * 60;
                    case "m":
                    case "month":
                    case "months":
                        return this.finterval * 30* 24 * 60 * 60; ;   //如果按月计算，需要去找月首和月末。这里计算时为了给并发计算引擎判断是否为按月计算。只参与判断，不参与计算。
                    case "y":
                    case "year":
                    case "years":
                        return this.finterval * 365 * 24 * 60 * 60; ;   //如果按年计算，需要去找年首和年末。这里计算时为了给并发计算引擎判断是否为按年计算。只参与判断，不参与计算。
                    default:        //默认按hour计算
                        return this.finterval * 60 * 60;
                }
            }
        }

        //配置对象方法：根据finterval和fintervaltype计算当次的endtime
        public DateTime GetEndTime()
        {
            DateTime newtime;
            switch (this.fintervaltype.ToLower())
            {
                case "s":
                case "second":
                case "seconds":
                    return this.fstarttime.AddSeconds(this.finterval);
                case "min":
                case "minute":
                case "minutes":
                    return this.fstarttime.AddMinutes(this.finterval);
                case "h":
                case "hour":
                case "hours":
                    return this.fstarttime.AddHours(this.finterval);
                case "d":
                case "day":
                case "days":
                    return this.fstarttime.AddDays(this.finterval);
                case "m":
                case "month":
                case "months":
                    //如果起始时间是2017-6-1 00:00:00秒，截止时间应该是2017-7-1 00:00:00
                    //如果起始时间是2017-6-2 00:00:00秒，截止时间应该是2017-7-1 00:00:00
                    //如果起始时间是2017-5-31 23:00:00秒，截止时间应该是2017-6-1 00:00:00
                    newtime = this.fstarttime.AddMonths(this.finterval);
                    return new DateTime(newtime.Year, newtime.Month, 1,this.fstarttime.Hour,this.fstarttime.Minute,this.fstarttime.Second);
                case "y":
                case "year":
                case "years":
                    //如果起始时间是2014-1-1 00:00:00秒，截止时间应该是2015-1-1 00:00:00
                    //如果起始时间是2014-1-1 10:23:12秒，截止时间应该是2015-1-1 00:00:00
                    //如果起始时间是2014-1-8 00:00:00秒，截止时间应该是2015-1-1 00:00:00
                    //如果起始时间是2013-12-31 23:59:59秒，截止时间应该是2014-1-1 00:00:00
                    newtime = this.fstarttime.AddYears(this.finterval);
                    return new DateTime(newtime.Year, 1, 1, this.fstarttime.Hour, this.fstarttime.Minute, this.fstarttime.Second);
                default://hour
                    return this.fstarttime.AddHours(this.finterval);
            }
        }



        #region 辅助函数
        //根据枚举类型PSLFunctionType对type属性进行检查，并给oftype赋值

        //根据“当次计算起始时间”和method和interval参数确定“当次计算截止时间”
        //由于starttime动态更新，是每次计算的起始时间，因此enntime仅需根据当次计算的starttime添加间隔来计算


        //list通过实体类实现IComparable接口，而且必须实现CompareTo方法实现排序。
        //根 /*
        int IComparable<PSLCalcuItem>.CompareTo(PSLCalcuItem other)
        {
            if (other.fnextstarttime > this.fnextstarttime)
                return -1;
            else if (other.fnextstarttime == this.fnextstarttime)
                return 0;
            else
                return 1;
        }
       
        #endregion
    }
}