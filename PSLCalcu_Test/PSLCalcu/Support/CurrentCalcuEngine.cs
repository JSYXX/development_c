using Config;
using PCCommon;
using PSLCalcu.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//实时计算并发计算参考  https://www.cnblogs.com/heweijian/p/11330282.html
//
namespace PSLCalcu
{
    public class CurrentCalcuEngine
    {
        public void calcu(LogHelper logHelper, Dictionary<int, List<PSLCalcuItem>> map, int CALCUMODULE_THRESHOLD, ref long errorCount, ref long warningCount, ref long calcount, ref long beforeCalcount, DateTime stop)
        {

            bool minflag = simpleCalcu(map, logHelper, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, 0, true, stop);    //分钟计算
        }
        public bool calcu(LogHelper logHelper, List<PSLCalcuItem> PslCalcuItems, int CALCUMODULE_THRESHOLD, ref long errorCount, ref long warningCount, ref long calcount, ref long beforeCalcount, ref long calstatus)
        {

            if (null == PslCalcuItems || PslCalcuItems.Count == 0)
            {
                return false;
            }
            if (errorCount == float.MaxValue) errorCount = 0;         //如果达到最大值，则复位
            if (warningCount == float.MaxValue) warningCount = 0;     //如果达到最大值，则复位
            if (calcount == float.MaxValue)
            {
                calcount = 0;
                beforeCalcount = 0;
            }
            foreach (PSLCalcuItem pslcalcuitem in PslCalcuItems)
            {
                if (Interlocked.Read(ref calstatus) == 1)
                {
                    actualCalcu(logHelper, pslcalcuitem, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, ref beforeCalcount, DateTime.Now);
                }
                else
                {
                    break;
                }
            }
            return true;

        }
        public void paracalcu(LogHelper logHelper, Dictionary<int, List<PSLCalcuItem>> secondMap, Dictionary<int, List<PSLCalcuItem>> minMap,
            Dictionary<int, List<PSLCalcuItem>> hourMap, Dictionary<int, List<PSLCalcuItem>> dayMap,
            Dictionary<int, List<PSLCalcuItem>> monthMap, Dictionary<int, List<PSLCalcuItem>> yearMap,
            int CALCUMODULE_THRESHOLD, ref long errorCount, ref long warningCount, ref long calcount, ref long beforeCalcount, DateTime stop)
        {
            bool calflag = true;
            bool minflag = simpleCalcu(minMap, logHelper, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, 0, calflag, stop);    //分钟计算
            bool hourflag = simpleCalcu(hourMap, logHelper, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, 1, minflag, stop);  //小时计算
            //bool dayflag=simpleCalcu(dayMap, logHelper, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, 0,hourflag,stop);      //天计算
            //bool monthflag=simpleCalcu(monthMap, logHelper, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, 0,dayflag,stop);   //月计算
            //simpleCalcu(yearMap, logHelper, CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, 0, monthflag,stop);
        }

        public bool actualCalcu(LogHelper logHelper, PSLCalcuItem pslcalcuitem, int CALCUMODULE_THRESHOLD, ref long errorCount, ref long warningCount, ref long calcount, ref long beforeCalcount, DateTime stop)
        {
            if (stop > pslcalcuitem.fnextstarttime)             //如果当前系统时间，大于最小nextstarttime，则表明对应的item已到计算时间
            {
                //UpdateCalcuIndex(pslcalcuitem.fid.ToString());      //更新界面当前计算项
                TimeRecord timerecord = new TimeRecord();   //初始化时间记录对象
                try
                {
                    Interlocked.Increment(ref calcount); //计算+1
                    timerecord.ModuleName = pslcalcuitem.fmodulename;
                    #region 1、从数据库读取数据
                    //1.1 读取计算源数据
                    timerecord.BeforeReadData = DateTime.Now;
                    //string[] sourcetagname = Regex.Split(pslcalcuitem.sourcetagname.Replace("^", "\\"), ";|；");             //对PGIM标签进行特别处理。标签中的^在程序变量中全部替回\。(对于pgim，pslcalcuitem中路径均用^表示。)
                    string[] sourcetagname = Regex.Split(pslcalcuitem.sourcetagname, ";|；");                                  //对PGIM标签的特别处理，放到PGIMHelper层。
                    List<PValue>[] inputs = new List<PValue>[sourcetagname.Length];                                            //参与计算标签的历史数据，数据格式List<PValue>
                    DataSet newInputs = new DataSet();//新版长周期算法取短周期算法的数据
                    if (pslcalcuitem.sourcetagdb == "rtdb")
                    {

                        //实时库实时数据读取
                        //新版长周期算法去短周期数据
                        if (APPConfig.caculateLongFunction.Contains(pslcalcuitem.fmodulename))
                        {
                            goto CURRENTCalcu;
                        }
                        else
                        {
                            for (int i = 0; i < sourcetagname.Length; i++)
                            {
                                inputs[i] = RTDBDAO.ReadData(sourcetagname[i], pslcalcuitem.fstarttime, pslcalcuitem.fendtime);        //获取历史数据
                                                                                                                                       //读取时DAO层发生错误
                                if (null == inputs[i])
                                {
                                    Interlocked.Increment(ref errorCount); //错误计数+1
                                                                           //记录LOG
                                    string errInfo;
                                    errInfo = string.Format("计算引擎错误{0}：读取实时数据错误，RTDBDAO层出错!", errorCount) + Environment.NewLine;
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                    logHelper.Fatal(errInfo);
                                    inputs[i] = null;
                                    goto CURRENTCalcu;
                                }
                                //读取某一个数据为空。实时数据读取如果为空，则应该是在该时间之前没有任何数据导致的，这个应该报错，一般用户可以得知情况，去检查标签点。
                                //还有一种情况是，实时数据读取不为空，但全部是非好质量点，这样情况应该在内部报警。
                                else if (inputs[i].Count == 0) //多个源数据标签，如果有一个标签读取的数据为空，则跳过当前标签，去循环读取下一个标签
                                {
                                    Interlocked.Increment(ref errorCount); //错误计数+1
                                                                           //记录LOG
                                    string errInfo;
                                    errInfo = string.Format("计算引擎错误{0}：读取实时数据为空，第{1}个标签对应时间段内没有实时数据!", errorCount.ToString(), i.ToString()) + Environment.NewLine;
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                    logHelper.Error(errInfo);
                                }
                            }//end for
                        }
                    }//end实时数据库读取
                    else if (pslcalcuitem.sourcetagdb == "opc")
                    {
                        //OPC数据读取：OPC数据读取要求与实时数据库一致，需要在起始时刻和截止时刻插值。
                        for (int i = 0; i < sourcetagname.Length; i++)
                        {
                            inputs[i] = OPCDAO.Read(sourcetagname[i], pslcalcuitem.fstarttime, pslcalcuitem.fendtime);        //获取概化数据
                                                                                                                              //读取数据时发生错误  返回值为空
                            if (null == inputs[i])
                            {
                                //——在OPCDAO.Read发生错误的情况下，在OPCDAO.Read中记录log。
                                //——在主引擎层记录发生错误的计算项log。
                                //更新UI
                                Interlocked.Increment(ref errorCount); //错误计数+1
                                                                       //记录LOG
                                string errInfo;
                                errInfo = string.Format("计算引擎错误{0}：读取OPC数据错误，OPCDAO层出错!", errorCount.ToString()) + Environment.NewLine;
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                logHelper.Fatal(errInfo);
                                //出错策略：多个源数据中的一个出现错误，就跳过当前数据读取。置当前源数据为null。并跳转至计算调用，交计算模块处理空数据。
                                //goto NEXTCalcu;       //2018.04.23修改发生报警后，不再直接跳过当前计算，而是给计算数据赋null值，进入计算来处理
                                inputs[i] = null;
                                goto CURRENTCalcu;

                            }
                            //没有数据
                            else if (inputs[i].Count == 0) //多个源数据标签，如果有一个标签读取的数据为空，则跳过当前标签，去循环读取下一个标签
                            {
                                //——在OPCDAO未发生错误的情况下返回空数据，记录错误，读取下一个标签
                                //——读取OPC数据为空时记录错误的原因：在OPC数据读取为起止时刻点插值的方式下，OPC数据读取，正常情况下必然有数据，如果没有数据，一定是有问题的。
                                //更新UI  
                                Interlocked.Increment(ref errorCount); //错误计数+1
                                                                       //记录LOG
                                string errInfo;
                                errInfo = string.Format("计算引擎错误{0}：读取OPC数据为空，第{1}个标签对应时间段内没有OPC数据！", errorCount.ToString(), i.ToString()) + Environment.NewLine;
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.sourcetagname, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                logHelper.Error(errInfo);
                            }
                        }//end for
                    }//endOPC数据读取
                    else if (pslcalcuitem.sourcetagdb == "rdb")
                    {
                        //概化库概化数据读取
                        for (int i = 0; i < pslcalcuitem.fsourtagids.Length; i++)
                        {
                            inputs[i] = PSLDataDAO.Read(pslcalcuitem.fsourtagids[i], pslcalcuitem.fstarttime, pslcalcuitem.fendtime);               //获取概化数据
                                                                                                                                                    //读取数据时发生错误
                            if (null == inputs[i])
                            {
                                Interlocked.Increment(ref errorCount); //错误计数+1
                                                                       //记录LOG
                                string errInfo;
                                errInfo = string.Format("计算引擎错误{0}：读取概化数据错误，PSLDataDAO层出错!", errorCount.ToString()) + Environment.NewLine;
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                logHelper.Fatal(errInfo);
                                goto CURRENTCalcu;

                            }
                            //读取标签数据，如果标签没有数据，则应该记录报警。但是前提是该标签本身在配置时被配置为保存数据，即pslcalcuitem.fsourtagflags[i]==true。
                            //如果标签本身在配置时被配置为不保存，则该标签一定没有数据。没有数据即为正常状态。不记录log。在并行计算下，这种情况仍然报警。
                            else if (pslcalcuitem.fsourtagflags[i] && inputs[i].Count == 0)   //多个源数据标签，如果有一个标签读取的数据为空，则跳过当前标签，去循环读取下一个标签
                            {
                                //2018.1.2修改
                                //——在PSLDataDAO未发生错误的情况下返回空数据，记录报警，读取下一个标签                        
                                //更新UI
                                Interlocked.Increment(ref warningCount); //警告计数+1
                                                                         //UpdateCalcuWarning(string.Format("计算过程中发生{0}次警告!", warningCount.ToString()));
                                                                         //记录LOG
                                string warningInfo;
                                warningInfo = string.Format("计算引擎警告{0}：读取概化数据为空，第{1}个标签对应时间段内没有概化数据！", warningCount.ToString(), i.ToString()) + Environment.NewLine;
                                warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                logHelper.Warn(warningInfo);
                            }
                        }//end for
                    }
                    else if (pslcalcuitem.sourcetagdb == "rdbset")
                    {
                        //在rdbset下，所有数据一次性读取
                        inputs = PSLDataDAO.ReadMulti(pslcalcuitem.fsourtagids, pslcalcuitem.fstarttime, pslcalcuitem.fendtime);        //获取概化数据
                                                                                                                                        //读取时发生错误
                                                                                                                                        //——在rdbset情况下，一次读取所哟标签，如果有错，也退出
                        if (null == inputs)
                        {
                            Interlocked.Increment(ref errorCount); //错误计数+1
                                                                   //UpdateCalcuError(string.Format("计算过程中发生{0}次错误", errorCount));
                                                                   //记录LOG
                            string errInfo;
                            errInfo = string.Format("计算引擎错误{0}：读取概化数据错误，PSLDataDAO层出错!", errorCount.ToString()) + Environment.NewLine;
                            errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                            logHelper.Error(errInfo);
                            goto CURRENTCalcu;
                        }
                        //读取报警
                        //——在rdbset下，要一次判断每一个标签数据
                        string warningInfo = "";
                        string indexInfo = "";
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            //标签数据为空要记录报警log。但是前提是该标签本身在配置时被配置为保存数据，即pslcalcuitem.fsourtagflags[i]==true。
                            //如果标签本身在配置时被配置为不保存，则该标签一定没有数据。没有数据即为正常状态。不记录log。
                            if (pslcalcuitem.fsourtagflags[i] && (inputs[i] == null || inputs[i].Count == 0))   //多个源数据标签，如果有一个标签读取的数据为空，则跳过当前标签，去循环读取下一个标签
                            {
                                //报警策略：多个源数据中的一个数据为null，记录报警，并接着去读取下一个数据。
                                indexInfo = indexInfo + i.ToString() + "、";
                            }
                        }
                        if (indexInfo != "")
                        {
                            //多个标签中如果有至少一个数据为空，则     indexInfo！=""          
                            //更新UI
                            Interlocked.Increment(ref warningCount); //警告计数+1
                                                                     //UpdateCalcuWarning(string.Format("计算过程中发生{0}次警告!", warningCount.ToString()));
                                                                     //记录LOG
                            warningInfo += string.Format("计算引擎警告{0}：读取概化数据为空，第{1}个标签对应时间段内没有概化数据！", warningCount.ToString(), indexInfo) + Environment.NewLine;
                            warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。" + Environment.NewLine, pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                            logHelper.Warn(warningInfo);  //提高写log效率。有时候rdbset下，一次读取一个设备的某计算结果，有74*30=2100点，此时可能会有多点因为根本没设置为保持结果，而没有数据。
                        }
                    }
                    else if (pslcalcuitem.sourcetagdb == "noinput")
                    {
                        //一些特殊的算法，不需要源数据的，使用noinput源数据类型。
                        //——对于这种类型，直接跳过读取数据阶段，不报警也不报错，直接到计算阶段
                        inputs[0] = null;
                        goto CURRENTCalcu;
                    }
                    //end概化数据读取 

                    //1.2 读取条件过滤数据
                    timerecord.BeforeFilter = DateTime.Now;
                    List<PValue> filterspan = new List<PValue>();
                    //过滤条件分为两种情况
                    //——第一种情况，仅有1个条件。这种情况下不需要过滤条件逻辑表达式
                    //——第二种情况，有多个条件。 这种情况下必须有条件逻辑表达式
                    //————尤其注意，单对某一个条件时间序列取反的逻辑运算，也属于有两个条件的情况。其中第一个条件是CurrentSpan，在映射表中id为保留至100。条件标签为CurrentSpan;Tag01，条件逻辑是{0}!{1}
                    if ((pslcalcuitem.fcondpsltagids != null) && (pslcalcuitem.fcondpsltagids.Length > 0))
                    {
                        //对条件逻辑字符串进行解析，前半段为时间逻辑表达式，后半段为最小时间段阈值
                        //——前半部分是条件的逻辑表达式。如string CalcuStr = "({1}!({2}&{3}))"。
                        //——后半部分是最小时间段阈值，单位是分钟。如string CalcuStr = "({1}!({2}&{3}));5"。也可以不填写。
                        //1.2.1、最小时间段阈值，如果给了设定值，就用设定值，否则使用默认值。默认值可以在“配置”中配置
                        string[] condExpressionStr = Regex.Split(pslcalcuitem.fcondexpression, ";|；");
                        int filterThreshold;
                        if (condExpressionStr.Length > 1)
                            filterThreshold = int.Parse(condExpressionStr[1]);  //如果表达式给了最小时间段阈值设定值，就用设定值
                        else
                            filterThreshold = CALCUMODULE_THRESHOLD;  //如果表达式不给定最小时间段阈值设定值，则采用默认值CALCUMODULE_THRESHOLD
                                                                      //1.2.2、读取条件过滤时间序列:分两种情况，一种是条件数量大于1，一种是条件数量等于1
                        if (pslcalcuitem.fcondpsltagids.Length > 1)//如果条件数量>1
                        {
                            //根据fcondpsltagids读取各限值时间序列。
                            //在条件数量大于1的情况下，先读取每一个条件的时间序列spanseries，在由逻辑运算计算得出总的过滤条件filterspan
                            List<PValue>[] spanseries = new List<PValue>[pslcalcuitem.fcondpsltagids.Length];
                            string[] condpslnames = Regex.Split(pslcalcuitem.fcondpslnames, ";|；");
                            for (int i = 0; i < pslcalcuitem.fcondpsltagids.Length; i++)
                            {
                                //tagname2id对照表是tagid是从100开始
                                spanseries[i] = new List<PValue>();

                                if (condpslnames[i] == "CURRENTSPAN")   //注意这里不要使用标签id判断。标签id可能会变。
                                {   //tagid=100，表示要取当前计算周期时间段，对应特殊标签名CurrentSpan，通常用于取反时用于范围限制，如{0}!{100}，在当前计算周期的范围内对tagid=100代表的时间序列取反
                                    spanseries[i].Add(new PValue(pslcalcuitem.fendtime.Millisecond - pslcalcuitem.fstarttime.Millisecond, pslcalcuitem.fstarttime, pslcalcuitem.fendtime, 0));    //当前计算周期时间段
                                    spanseries[i].Add(new PValue(0, pslcalcuitem.fendtime, pslcalcuitem.fendtime, 0));      //当前计算周期时间段的截止时刻值（在时间逻辑运算前，统一去掉）
                                }
                                else
                                {
                                    spanseries[i] = PSLDataDAO.Read(pslcalcuitem.fcondpsltagids[i], pslcalcuitem.fstarttime, pslcalcuitem.fendtime);     //返回值带截止时刻值， 在时间逻辑运算前，统一去掉                                  
                                }

                                if (null == spanseries[i])
                                {
                                    Interlocked.Increment(ref errorCount); //错误计数+1
                                    string errInfo;
                                    errInfo = string.Format("计算引擎错误{0}：读取第{1}条件时间序列错误!", errorCount.ToString(), i.ToString()) + Environment.NewLine;
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                    logHelper.Fatal(errInfo);
                                    //出错策略：多个条件数据中的一个出现错误，就跳过当前条件数据读取。置当前源数据为null。并跳转至计算调用，交计算模块处理空数据。
                                    //goto NEXTCalcu;       //2018.04.23修改发生报警后，不再直接跳过当前计算，而是给计算数据赋null值，进入计算来处理
                                    filterspan = null;      //如果条件变量有一个变量读取出错。则由于当前过滤条件不能准确计算，应视为当前输入为空，直接进入到计算项
                                    inputs = null;
                                    goto CURRENTCalcu;
                                }
                                else if (spanseries[i].Count == 0)
                                {
                                    //更新UI
                                    Interlocked.Increment(ref warningCount); //警告计数+1
                                                                             //记录LOG
                                    string warningInfo;
                                    warningInfo = string.Format("计算引擎警告{0}：第{1}个条件时间序列数据为空，没有符合计算条件的时间段！", warningCount.ToString(), i.ToString()) + Environment.NewLine;
                                    warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                    logHelper.Warn(warningInfo);
                                    //报警策略：多个条件数据中的一个数据为null，记录报警，并接着去读取下一个数据。
                                }
                                else
                                {
                                    spanseries[i].RemoveAt(spanseries[i].Count - 1);    //进入逻辑运算的序列应该去掉截止时刻。
                                }
                            }//endfor读取条件变量数据                               
                             //按照时间逻辑表达式，对filterspans进行时间逻辑运算。特别注意，进入逻辑运算的条件时间序列spanseries，不带截止时刻值。返回的filterspan也不带截止时刻值
                            ICondEvaluatable exp;
                            exp = new CondExpression(condExpressionStr[0]);
                            filterspan = exp.Evaluate(spanseries);          //spanseries进入时间逻辑运算时是不带截止数据的。逻辑运算内不提供对截止数据的去除功能，返回的值也不带截止时刻功能                 
                        }
                        else if (pslcalcuitem.fcondpsltagids.Length == 1)   //如果条件的数量=1，则直接读取限值时间序列
                        {
                            //在条件数量小于1的情况下，直接读取filterspan
                            filterspan = PSLDataDAO.Read(pslcalcuitem.fcondpsltagids[0], pslcalcuitem.fstarttime, pslcalcuitem.fendtime);
                            if (null == filterspan)
                            {
                                //更新UI
                                Interlocked.Increment(ref errorCount); //错误计数+1
                                                                       //记录LOG
                                string errInfo;
                                errInfo = string.Format("计算引擎错误{0}：读取单个条件时间序列错误!", errorCount.ToString()) + Environment.NewLine;
                                errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                logHelper.Fatal(errInfo);
                                //出错策略：读取单个条件数据出现错误，就跳过当前条件数据读取。置当前源数据为null。并跳转至计算调用，交计算模块处理空数据。
                                //goto NEXTCalcu;       //2018.04.23修改发生报警后，不再直接跳过当前计算
                                filterspan = null;
                                inputs = null;
                                goto CURRENTCalcu;
                            }
                            else if (filterspan != null && filterspan.Count != 0)
                            {
                                filterspan.RemoveAt(filterspan.Count - 1);    //进入时间逻辑运算的序列应该去掉截止时刻。
                            }
                        }
                        //1.2.3、如果时间序列为空，则该数据忽略
                        if (filterspan == null || filterspan.Count == 0)
                        {
                            //更新UI
                            Interlocked.Increment(ref warningCount); //警告计数+1
                                                                     //记录LOG
                            string warningInfo;
                            warningInfo = string.Format("计算引擎警告{0}：总条件时间序列数据为空，没有符合计算条件的时间段！", warningCount.ToString()) + Environment.NewLine;
                            warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                            logHelper.Warn(warningInfo);
                            //报警策略：单个条件数据为null，记录报警，跳转计算。
                            //goto NEXTCalcu;       //2018.04.23修改发生报警后，不再直接跳过当前计算
                            inputs = null;          //如果总条件时间序列为空，则意味着当前计算没有条件有效时间段，应视为当前输入为空，直接进入到计算项
                            goto CURRENTCalcu;
                        }
                        //1.2.4、使用限值时间序列对输出数据进行过滤
                        bool flag = SpanLogic.SpansFilter(ref inputs, filterspan, filterThreshold);     //inputs要求带截止时刻值，总过滤条件filterspan不带截止时刻值。
                        if (flag)
                        {
                            //更新UI
                            Interlocked.Increment(ref errorCount); //错误计数+1
                                                                   //记录LOG
                            string errInfo;
                            errInfo = string.Format("计算引擎错误{0}：用条件时间序列过滤源数据时错误!", errorCount.ToString()) + Environment.NewLine;
                            errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                            logHelper.Fatal(errInfo);
                            //出错策略：用条件时间序列过滤源数据时错误，置当前源数据为null。并跳转至计算调用，交计算模块处理空数据。
                            //goto NEXTCalcu;       //2018.04.23修改发生报警后，不再直接跳过当前计算
                            filterspan = null;
                            inputs = null;
                            goto CURRENTCalcu;
                        }
                        for (int ipoint = 0; ipoint < inputs.Length; ipoint++)
                        {
                            if (inputs[ipoint] == null || inputs[ipoint].Count == 0)
                            {
                                //更新UI
                                Interlocked.Increment(ref warningCount); //警告计数+1
                                                                         //记录LOG
                                string warningInfo;
                                warningInfo = string.Format("计算引擎警告{0}：输入数据经条件时间序列过滤后有效数据为空，没有符合计算条件的输入数据！", warningCount.ToString()) + Environment.NewLine;
                                //logHelper.Warn(warningInfo);
                                warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                logHelper.Warn(warningInfo);
                                //报警策略：过滤后源数据为空，记录报警，跳转到计算。
                                //goto NEXTCalcu;       //2018.04.23修改发生报警后，不再直接跳过当前计算。过滤后的inputs为空，记录报警后直接进入计算                                    
                            }
                        }
                    }//end 1.2 读取条件过滤数据                      
                    #endregion  1、从数据库读取数据

                    #region 2、计算：
                    CURRENTCalcu:
                    //2.1 计算：获取计算对象（反射法）
                    timerecord.BeforeReflection = DateTime.Now;
                    Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);                                   //获得PSLCalcu.exe

                    Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + pslcalcuitem.fmodulename);   //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”
                    PropertyInfo inputData = calcuclass.GetProperty("inputData");                                           //获得算法类的静态参数inputData
                    PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");                                           //获得算法类的静态参数calcuInfo                    
                    MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { });                                       //获得算法类的Calcu()方法。注意，Calcu方法有重载，这里需要指明获得哪个具体对象，否则会报错

                    //2.2 计算：主算法
                    //1、这里需要判断Calcu是否为空，如果为空，要写log
                    //2、调用时也要做try处理                   
                    timerecord.BeforeCalcu = DateTime.Now;
                    if (APPConfig.caculateLongFunction.Contains(pslcalcuitem.fmodulename))
                    {
                        inputData.SetValue(null, newInputs);            //将输入数据给入算法

                    }
                    else
                    {
                        inputData.SetValue(null, inputs);            //将输入数据给入算法
                    }

                    calcuInfo.SetValue(null, new CalcuInfo(pslcalcuitem.sourcetagname,
                                                            pslcalcuitem.fsourtagids,
                                                            pslcalcuitem.fsourtagflags,
                                                            pslcalcuitem.fmodulename,
                                                            pslcalcuitem.fparas,
                                                            pslcalcuitem.fstarttime,
                                                            pslcalcuitem.fendtime,
                                                            pslcalcuitem.sourcetagmrb,
                                                            pslcalcuitem.sourcetagmre));     //将当前计算信息给入算法
                    Results Results = (Results)Calcu.Invoke(null, null);
                    if (Results.fatalFlag == true)
                    {
                        //如果计算组件的fatalflag为true则说明发生意料外的计算错误，在这种情况下，必须在算法内部给出标志位不为0的计算结果，在外部记录log                            
                        //更新UI，意料外错误，也记录在错误计数器上
                        Interlocked.Increment(ref errorCount); //错误计数+1
                                                               //更新Log
                        string errInfo;
                        errInfo = string.Format("计算引擎错误{0}：计算模块内部意外错误!", errorCount.ToString()) + Environment.NewLine;
                        errInfo += string.Format("——计算错误详细信息：{0}。", Results.fatalInfo) + Environment.NewLine;
                        errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                        logHelper.Fatal(errInfo);
                    }
                    else if (Results.errorFlag == true)
                    {
                        //如果计算组件的errorflag为true则说明发生计算错误，在这种情况下，必须在算法内部给出标志位不为0的计算结果，在外部记录log
                        //——比如像模块内部数据检查时发现的数据源错误，比如模块内部计算时try语句内部的未知错误
                        Interlocked.Increment(ref errorCount); //错误计数+1
                                                               //更新Log
                        string errInfo;
                        errInfo = string.Format("计算引擎错误{0}：计算模块内部错误!", errorCount.ToString()) + Environment.NewLine;
                        errInfo += string.Format("——计算错误详细信息：{0}。", Results.errorInfo) + Environment.NewLine;
                        errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                        logHelper.Error(errInfo);
                        //错误策略，如果计算发生可控错误，计算结果给一个准备好的默认值，这个默认值仍然应该写入数据库，不跳过写结果阶段
                        //goto NEXTCalcu;
                        //2018.04.23修改。在计算模块内部发生错误，必须给出标志位不为0的计算结果，该计算结果要写入数据库。而不是跳过
                    }
                    else if (Results.warningFlag == true)
                    {
                        //如果计算组件的warningflag为true则说明发生计算报警，在这种情况下，必须在算法内部给出标志位不为0的计算结果，在外部记录log
                        //——比如像模块内部经状态位过滤后，源数据为空。这种情况通常是前置计算在当前计算周期内全部结算结果状态均异常。这种情况给出报警即可。
                        //更新UI
                        Interlocked.Increment(ref warningCount); //警告计数+1
                                                                 //记录LOG
                        string warningInfo;
                        warningInfo = string.Format("计算引擎警告{0}：计算模块内部警告!", warningCount.ToString()) + Environment.NewLine;
                        warningInfo += string.Format("计算警告详细信息：{0}！", Results.warningInfo) + Environment.NewLine;
                        warningInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                        logHelper.Warn(warningInfo);
                        //警告策略，如果计算发生警告，计算结果会给一个准备好的默认值，这个默认值仍然应该写入数据库，不跳过写结果阶段
                        //goto NEXTCalcu;
                    }
                    //end 2.2、计算：主算法
                    #endregion

                    #region 3、将计算结果写入数据库
                    //——将计算结果写入数据库，有一种提高效率的想法是，先把每次计算的结果存放在list中，当记录到一定数量时批量写入
                    //——上述方法要考虑两个问题：1、如果写入失败怎么处理。2、如果一个计算要用到上一个计算的结果，比如有计算条件的计算。这种算法必须让条件所在的计算马上进行存储。这样会有问题。并且这个问题处理起来比较复杂。
                    timerecord.BeforeWriteData = DateTime.Now;
                    bool writeflage = true;
                    List<PValue>[] results = Results.results;
                    if (results != null)    //Calcu.ErrorFlag为false的情况下，读取结果也有可能为空。比如读取值次的算法。每小时运行一次。但是大部分情况下，都没有值次信息更新
                    {
                        //新版算法直接跳入更新计算项下次计算时间
                        if (APPConfig.caculateFunction.Contains(pslcalcuitem.fmodulename))
                        {
                            goto NEXTCalcu;
                        }
                        else
                        {
                            //for (int i = 0; i < pslcalcuitem.foutputnumber; i++) 20181030日，改为按照实际计算结果数量来循环。但是必须在算法内部保证实际计算结果数量只能小于等与定义好的foutputnumber
                            for (int i = 0; i < results.Length; i++)
                            {
                                //如果对应的计算为N，则跳过。如果对应的某个结果results[i]为null，则跳过。
                                if (pslcalcuitem.falgorithmsflagbool[i] &&              //计算结果对应标志位为1
                                    (results[i] != null) &&                             //计算结果列表不为空
                                    results[i].Count > 0 &&                             //计算结果列表count大于0
                                    Array.IndexOf(results[i].ToArray(), null) == -1     //计算结果列表元素不含null
                                    )
                                {
                                    //这里调用PSLDataDAO.SaveDAta()，一次写入一个List<PValue>。每个List<PValue>通常只有一个值。List<PValue>长度大于1，通常意味着该值是时间序列。
                                    writeflage = PSLDataDAO.WriteOrUpdate(pslcalcuitem.foutputpsltagids[i], results[i], pslcalcuitem.fstarttime, pslcalcuitem.fendtime);
                                }
                                if (!writeflage)
                                {  //如果写入返回结果为false，则说明写入过程中出错，计数器递增。出错信息已经在DAO写入接口中记录。
                                   //更新UI
                                    Interlocked.Increment(ref errorCount); //错误计数+1
                                                                           //更新Log
                                    string errInfo;
                                    errInfo = string.Format("计算引擎错误{0}：写计算结果错误，PSLDataDAO层出错!", errorCount.ToString()) + Environment.NewLine;
                                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                                    logHelper.Fatal(errInfo);
                                    goto NEXTCalcu;
                                }
                            }
                        }//end 3、将计算结果写入数据库
                    }
                    else
                    {
                        if (!pslcalcuitem.foutputpermitnull)     //20190215,给算法内增加了outputpermitnull属性，用来制定每一个算法，是否允许空结果。如果算法允许空结果，则这里不记录错误。如果算法不允许空，这里要记录错误。
                        {
                            //更新UI
                            Interlocked.Increment(ref errorCount); //错误计数+1
                                                                   // UpdateCalcuError(string.Format("计算过程中发生{0}次错误!", errorCount));
                                                                   //更新Log
                            string errInfo;
                            errInfo = string.Format("计算引擎错误{0}：当前计算结果整体为null！", errorCount.ToString()) + Environment.NewLine;
                            errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                            logHelper.Fatal(errInfo);
                            goto NEXTCalcu;
                        }

                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    //更新UI
                    Interlocked.Increment(ref errorCount); //错误计数+1
                                                           //更新LOG
                    string errInfo;
                    errInfo = string.Format("计算引擎错误{0}：读算写错误中未知错误!!", errorCount.ToString()) + Environment.NewLine;
                    errInfo += string.Format("——计算错误详细信息：{0}。", ex.ToString()) + Environment.NewLine;
                    errInfo += string.Format("——计算模块的名称是：{0}-{1}，计算起始时间是：{2}，计算结束时间是：{3}。", pslcalcuitem.fid, pslcalcuitem.fmodulename, pslcalcuitem.fstarttime.ToString(), pslcalcuitem.fendtime.ToString());
                    logHelper.Fatal(errInfo);
                    goto NEXTCalcu;
                }//end 1读.2算.3写

                #region 4、更新计算项下次计算时间
                NEXTCalcu:
                timerecord.BeforeUpdateCalcuInfo = DateTime.Now;
                pslcalcuitem.fstarttime = pslcalcuitem.fendtime;                                            //下一次计算结果对应时间段的起始时间。是当次计算结果对应的结束时间。
                pslcalcuitem.fendtime = pslcalcuitem.GetEndTime();                                          //下一次计算结果对应时间段的结束时间。
                pslcalcuitem.fnextstarttime = pslcalcuitem.fendtime.AddSeconds(pslcalcuitem.fdelay);        //下一次计算的开始时刻。对应时间段起始时间加计算延时。
                                                                                                            //将更新写入到数据库
                                                                                                            //PSLCalcuConfigDAO.
                PSLCalcuConfigDAO.UpdateStartTime(pslcalcuitem.fid, pslcalcuitem.fstarttime);
                #endregion
                #region 7、log清除：每晚12点30定时清理log。经测试，对五个级别的log都适用。
                //if(true)  //测试用
                if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 30) //每到整点时，看小时数是否符合要求。
                {
                    //logClear();
                }
                #endregion
                return false;
            }
            else
            {
                return true;
            }

        }
        public bool simpleCalcu(Dictionary<int, List<PSLCalcuItem>> map, LogHelper logHelper, int CALCUMODULE_THRESHOLD, ref long errorCount, ref long warningCount, ref long calcount, int mode, bool calflag, DateTime stop)
        {
            var flag = false;
            if (calflag)
            {
                foreach (int key in map.Keys)
                {
                    List<PSLCalcuItem> list = map[key];
                    if (null != list && list.Count > 0)
                    {
                        long error = 0;
                        long warn = 0;
                        long cal = 0;
                        long before = 0;
                        if (mode == 1) //并行计算
                        {
                            long start = DateTime.Now.Ticks;
                            list.AsParallel().ForAll(pslcalcuitem =>
                            {
                                flag = actualCalcu(logHelper, pslcalcuitem, CALCUMODULE_THRESHOLD, ref error, ref warn, ref cal, ref before, stop);
                                if (flag)
                                {
                                    return;
                                }
                            });
                            return flag;
                        }
                        else //串行计算
                        {
                            long start = DateTime.Now.Ticks;
                            foreach (PSLCalcuItem pslcalcuitem in list)
                            {
                                flag = actualCalcu(logHelper, pslcalcuitem, CALCUMODULE_THRESHOLD, ref error, ref warn, ref cal, ref before, stop);
                                if (flag)
                                {
                                    return flag;
                                }

                            }
                        }
                        errorCount = errorCount + error;
                        warningCount = warningCount + warn;
                        calcount = calcount + cal;
                    }
                }
            }

            return flag;
        }
        public void test()
        {
            /*
                       Parallel.ForEach(list, i => {
                           for (int k = 0; k < 10000; k++) {
                               int a = k * 1000;
                           }
                       });
                       **/
            /*
                               for (int k = 0; k < 10000; k++)
                               {
                                   int a = k * 1000;
                               }
                                */
        }
    }
}
