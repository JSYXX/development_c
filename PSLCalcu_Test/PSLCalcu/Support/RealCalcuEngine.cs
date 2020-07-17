using System;
using System.Collections.Generic;       //使用List
using PCCommon;                         //使用PValue
using System.Diagnostics;               //使用计时器
using System.Text.RegularExpressions;   //使用正则表达式
using Config;                           //使用Config配置
using System.Reflection;                //使用反射
using PSLCalcu.Module;                  //使用计算模块
using DBInterface.RTDBInterface;        //使用实时数据库接口
using DBInterface.RDBInterface;         //使用关系数据库接口

namespace PSLCalcu
{
    /// <summary>
    /// RealCalcuEngine
    /// 实时数据计算引擎。一次仅对一个计算项进行计算，无并发计算。
    /// 
    ///——对当前计算项currentCalcuItem进行计算（读、算、写）
    ///——入参：计算引擎当次计算对象currentCalcuItem
    ///——计算任务共享变量：实时数据存放变量，RealDataResults
    ///——计算任务共享变量：条件数据存放变量，CondDataResults
    ///——计算任务共享变量：计算结果存放变量，CalcuResults
    /// 
    /// 版本：1.1
    ///    
    /// 修改纪录
    ///     2017.07.28 版本：1.1 改为类封装。
    ///		2017.06.16 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.16</date>
    /// </author> 
    /// </summary>
    class RealCalcuEngine
    {
        private LogHelper logHelper = LogFactory.GetLogger(typeof(HistoryCalcu));       //全局log

        static public Dictionary<string, System.UInt32> TagName2Id; //概化库标签名id字典
        public bool errorFlag { get; set; }                         //初始化错误标志位
        public long errorCount { get; set; }                        //概化计算主线程警告统计
        public bool warningFlag { get; set; }                       //初始化警告标志位
        public long warningCount { get; set; }                      //概化计算主线程警告统计

        private List<PValue>[] CalcuData;       //计算用数据
        private List<PValue> CondSpan;          //条件数据
        private List<PValue>[] CalcuResults;    //计算结果

        public void MainRealCalcu(PSLCalcuItem currentCalcuItem)
        {
            //计时，用于计算时间测试和保存
            var swRealCalcu = Stopwatch.StartNew();

            //1、读取当前计算项数据
            ReadCalcuData(currentCalcuItem);

            //2、读取当前计算项条件数据
            ReadCondData(currentCalcuItem);

            //3、计算
            RealCalcu(currentCalcuItem);
            
            //4、写计算结果
            WriteCalcuResult(currentCalcuItem);

        }
        //读取计算数据
        private void ReadCalcuData(PSLCalcuItem currentCalcuItem)
        {
            List<PValue>[] inputs;
            try
            {
                RTDbHelper rtdbhelper = new RTDbHelper();
                //1、从数据库读取数据                       
                string[] sourcetagname = Regex.Split(currentCalcuItem.sourcetagname, ";|；");
                inputs = new List<PValue>[sourcetagname.Length];             //参与计算的标签历史数据PValue格式
                if (currentCalcuItem.sourcetagdb == "rtdb" )
                {
                    //实时库实时数据
                    for (int i = 0; i < sourcetagname.Length; i++)
                    {
                        inputs[i] = rtdbhelper.GetRawValues(currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime, currentCalcuItem.fendtime);        //获取历史数据,100000该参数是一次读取数据的最大条数，建议改为xml文件
                        if (inputs[i] == null || inputs[i].Count == 0)
                        {   //如果任一数据源取数为空，则该数据一定出错，计数器递增。出错信息已经在取数据的DAO中记录。直接跳到下一次计算                        
                            warningCount = warningCount + 1;
                            warningFlag = true;
                            //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                            logHelper.Error("计算引擎警告!");
                            string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                            logHelper.Error(moduleInfo);
                            string errInfo = string.Format("——警告统计序号{0}：对应时间段内没有实时数据!", warningCount.ToString());
                            logHelper.Error(errInfo);
                            //goto NEXTCalcu;
                        }
                    }                    
                }
                else
                {
                    //概化库概化数据
                    for (int i = 0; i < sourcetagname.Length; i++)
                    {
                        inputs[i] = PSLDataDAO.Read(TagName2Id[currentCalcuItem.sourcetagname], currentCalcuItem.fstarttime, currentCalcuItem.fendtime);        //获取历史数据
                        if (inputs[i] == null || inputs[i].Count == 0)
                        {   //如果任一数据源取数为空，则该数据一定出错，计数器递增。出错信息已经在取数据的DAO中记录。直接跳到下一次计算                        
                            warningCount = warningCount + 1;
                            warningFlag = true;
                            //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                            logHelper.Error("计算引擎警告!");
                            string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                            logHelper.Error(moduleInfo);
                            string errInfo = string.Format("——警告统计序号{0}。对应时间段内没有概化数据！", warningCount.ToString());
                            logHelper.Error(errInfo);
                            //goto NEXTCalcu;
                        }
                    }

                }
                CalcuData = inputs;
            }
            catch (Exception ex)
            {
                errorCount = errorCount + 1;
                errorFlag = true;
                //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                logHelper.Error("计算引擎主循环错误!");
                string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                logHelper.Error(moduleInfo);
                string errInfo = string.Format("——错误统计序号{0}:详细错误信息{1}", errorCount.ToString(), ex);
                logHelper.Error(errInfo);                
            } //end try
        }
        //读取条件数据
        private void ReadCondData(PSLCalcuItem currentCalcuItem) 
        {
            List<PValue> filterspan;
            try
            {                
                if ((currentCalcuItem.fcondpsltagids != null) && (currentCalcuItem.fcondpsltagids.Length > 0))
                {
                    filterspan = new List<PValue>();
                    //对条件逻辑字符串进行解析，前半段为时间逻辑表达式，后半段为最小时间段阈值
                    //——前半部分是条件的逻辑表达式。如string CalcuStr = "({1}!({2}&{3}))"。
                    //——后半部分是最小时间段阈值，单位是分钟。如string CalcuStr = "({1}!({2}&{3}));5"。也可以不填写。
                    string[] condExpressionStr = Regex.Split(currentCalcuItem.fcondexpression, ";|；");
                    //最小时间段阈值，如果给了设定值，就用设定值，否则使用默认值。默认值可以在“配置”中配置
                    int filterThreshold = APPConfig.CALCUMODULE_THRESHOLD;      //如果表达式不给定最小时间段阈值，则采用默认值0分钟
                    if (condExpressionStr.Length > 1)
                        filterThreshold = int.Parse(condExpressionStr[1]);
                    //读取条件过滤时间序列:分两种情况，一种是条件数量大于1，一种是添加数量等于1
                    if (currentCalcuItem.fcondpsltagids.Length > 1)//如果条件数量>1
                    {
                        //根据fcondpsltagids读取各限值时间序列
                        List<PValue>[] spanseries = new List<PValue>[currentCalcuItem.fcondpsltagids.Length];
                        for (int i = 0; i < currentCalcuItem.fcondpsltagids.Length; i++)
                        {   //tagname2id对照表是tagid是从100开始
                            spanseries[i] = new List<PValue>();
                            if (currentCalcuItem.fcondpsltagids[i] == 100)
                            {   //tagid=100，表示要取当前计算周期时间段，对应特殊标签名CurrentSpan，通常用于取反时用于范围限制，如{0}!{100}，在当前计算周期的范围内对tagid=100代表的时间序列取反
                                spanseries[i].Add(new PValue(1, currentCalcuItem.fstarttime, currentCalcuItem.fendtime, 0));
                            }
                            else
                            {
                                spanseries[i] = PSLDataDAO.Read(currentCalcuItem.fcondpsltagids[i], currentCalcuItem.fstarttime, currentCalcuItem.fendtime);
                            }
                            if (spanseries[i] == null)
                            {   //如果任一数据源取数为空，则该数据一定出错，计数器递增。出错信息已经在取数据的DAO中记录。直接跳到下一次计算                        
                                warningCount = warningCount + 1;
                                warningFlag = true;
                                //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                                logHelper.Error("计算引擎警告!");
                                string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                                logHelper.Error(moduleInfo);
                                string errInfo = string.Format("——警告统计序号{0}。对应时间段内的过滤条件时间序列为空!", warningCount.ToString());
                                logHelper.Error(errInfo);
                                //goto NEXTCalcu;
                            }
                        }
                        //按照时间逻辑表达式，对filterspans进行时间逻辑运算
                        ICondEvaluatable exp;
                        exp = new CondExpression(condExpressionStr[0]);
                        filterspan = exp.Evaluate(spanseries);
                    }
                    else if (currentCalcuItem.fcondpsltagids.Length == 1)//如果条件的数量=1，则直接读取限值时间序列
                    {
                        filterspan = PSLDataDAO.Read(currentCalcuItem.fcondpsltagids[0], currentCalcuItem.fstarttime, currentCalcuItem.fendtime);
                    }
                    if (filterspan == null || filterspan.Count == 0)
                    {   //如果时间序列为空，则该条计算项所处时间段无任何有效时间                    
                        warningCount = warningCount + 1;
                        warningFlag = true;
                        //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                        logHelper.Error("计算引擎警告!");
                        string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                        logHelper.Error(moduleInfo);
                        string errInfo = string.Format("——警告统计序号{0}。对应时间段内的过滤条件时间序列为空!", warningCount.ToString());
                        logHelper.Error(errInfo);
                        //goto NEXTCalcu;
                    }
                    else 
                    {
                        CondSpan = filterspan;
                    }
                }                
            }
            catch (Exception ex)
            {
                errorCount = errorCount + 1;
                errorFlag = true;
                //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                logHelper.Error("计算引擎主循环错误!");
                string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                logHelper.Error(moduleInfo);
                string errInfo = string.Format("——错误统计序号{0}:详细错误信息{1}", errorCount.ToString(), ex);
                logHelper.Error(errInfo);                
            }//end try
        }
        //计算
        private void RealCalcu(PSLCalcuItem currentCalcuItem)
        {
            try
            {
                //1、计算：获取计算对象（反射法）
                Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);                      //获得PSLCalcu.exe
                Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + currentCalcuItem.fmodulename); //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”
                PropertyInfo inputData = calcuclass.GetProperty("inputData");               //获得算法类的静态参数inputData
                PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");               //获得算法类的静态参数calcuInfo                    
                MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { });           //获得算法类的Calcu()方法。注意，Calcu方法有重载，这里需要指明获得哪个具体对象，否则会报错

                //2 计算：条件过滤
                //使用限值时间序列对输出数据进行过滤
                int filterThreshold = APPConfig.CALCUMODULE_THRESHOLD;      //如果表达式不给定最小时间段阈值，则采用默认值0分钟
                SpanLogic.SpansFilter(ref CalcuData, CondSpan, filterThreshold);
                for (int ipoint = 0; ipoint < CalcuData.Length; ipoint++)
                {
                    if (CalcuData[ipoint] == null || CalcuData[ipoint].Count == 0)
                    {
                        //由于filterThreshold存在，即使之前filterspan不为空，但是经过SpansFilter()之后，inputs各元素仍有可能为空           
                        warningCount = warningCount + 1;
                        warningFlag = true;
                        //tslb_CalcuWarnings.Text = string.Format("{0} warnings occured in calcuting!", warningCount);
                        logHelper.Error("计算引擎警告!");
                        string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                        logHelper.Error(moduleInfo);
                        string errInfo = string.Format("——警告统计序号{0}。对应时间段内经过滤后的输入数据为空!请检查filterThreshold参数设置。", warningCount.ToString());
                        logHelper.Error(errInfo);
                        //goto NEXTCalcu;
                    }
                }

                //3 计算：主算法
                inputData.SetValue(null, CalcuData);            //将输入数据给入算法
                //calcuInfo.SetValue(null, new CalcuInfo(currentCalcuItem.sourcetagname, currentCalcuItem.fsourtagids, currentCalcuItem.fmodulename, currentCalcuItem.fparas, currentCalcuItem.fstarttime, currentCalcuItem.fendtime, currentCalcuItem.sourcetagmrb, currentCalcuItem.sourcetagmre));     //将当前计算信息给入算法
                CalcuResults = (List<PValue>[])Calcu.Invoke(null, null);
                if (CalcuResults == null)
                {   //如果计算结果为空，则说明计算过程中出错，计数器递增。出错信息已经在计算模块内部记录。直接跳到下一次计算。 
                    //计算模块错误，内部会记录。
                    errorCount = errorCount + 1;
                    errorFlag = true;
                    //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                    string errInfo = string.Format("——错误统计序号{0}。", errorCount.ToString());
                    logHelper.Error(errInfo);
                    //goto NEXTCalcu;
                }
            }
            catch (Exception ex)
            {
                errorCount = errorCount + 1;
                errorFlag = true;
                //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                logHelper.Error("计算引擎主循环错误!");
                string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                logHelper.Error(moduleInfo);
                string errInfo = string.Format("——错误统计序号{0}:详细错误信息{1}", errorCount.ToString(), ex);
                logHelper.Error(errInfo);
            }//end try
        }
        //写计算结果
        private void WriteCalcuResult(PSLCalcuItem currentCalcuItem)
        {
            bool writeflage = true;
           try
           {
               for (int i = 0; i < currentCalcuItem.foutputnumber; i++)
                {
                    //如果对应的计算为Y，则保存其结果
                    if (currentCalcuItem.falgorithmsflagbool[i])
                    {
                        //这里调用PSLDataDAO.SaveDAta()，一次写入一个List<PValue>。每个List<PValue>通常只有一个值。List<PValue>长度大于1，通常意味着该值是时间序列。
                        writeflage = PSLDataDAO.WriteOrUpdate(currentCalcuItem.foutputpsltagids[i], CalcuResults[i], currentCalcuItem.fstarttime, currentCalcuItem.fendtime);
                    }
                    if (!writeflage)
                    {  //如果写入返回结果为false，则说明写入过程中出错，计数器递增。出错信息已经在DAO写入接口中记录。
                        errorCount = errorCount + 1;
                        errorFlag = true;
                        //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
                        logHelper.Error("计算引擎错误!");
                        string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
                        logHelper.Error(moduleInfo);
                        string errInfo = string.Format("——错误统计序号{0}。resulte write error!", errorCount.ToString());
                        logHelper.Error(errInfo);                                
                        //goto NEXTCalcu;
                    }
                }
           }
           catch (Exception ex)
           {
               errorCount = errorCount + 1;
               errorFlag = true;
               //tslb_CalcuErrors.Text = string.Format("{0} errors occured in calcuting!", errorCount);
               logHelper.Error("计算引擎主循环错误!");
               string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", currentCalcuItem.fmodulename, currentCalcuItem.sourcetagname, currentCalcuItem.fstarttime.ToString(), currentCalcuItem.fendtime.ToString());
               logHelper.Error(moduleInfo);
               string errInfo = string.Format("——错误统计序号{0}:详细错误信息{1}", errorCount.ToString(), ex);
               logHelper.Error(errInfo);
           }//end try
        }

    }//end class
}//end namespace
