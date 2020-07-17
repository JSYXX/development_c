using System;
using System.Collections.Generic;
using System.Linq;                      //使用linq
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 数字量加法   
    /// ——限定的时间段内，多个数字量相加。其中有一个数字量发生置位或复位，总体的相加结果就会改变。
    /// ——输出是一个整取阶梯状的趋势图。
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///     2018.01.09 arrwo测试
    ///		2018.01.08 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.01.08</date>
    /// </author> 
    /// </summary>
   
    public class MMultiDigitalSum : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MultiDigitalSum";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "数字量加法";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 0;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "最少两个，最多不限";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "DSum";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "Y";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private int _outputNumber = 1;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "DSum";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "数字量和";
        public string outputDescsCN
        {
            get
            {
                return _outputDescsCN;
            }
        }
        private string _outputTable = "plsdata";
        public string outputTable
        {
            get
            {
                return _outputTable;
            }
        }
        private bool _outputPermitNULL = false;
        public bool outputPermitNULL
        {
            get
            {
                return _outputPermitNULL;
            }
        }
        #endregion

        #region 输入输出参数的读写接口
        //输入输出参数读写，之所以要单独做成类的静态变量，主要是考虑有外部计算条件的计算模块，外部条件完成对时间帅选后，可以直接用时间去处理inputData
        //这样能够保持带条件和不带条件的计算，Calcu方法在形式上可以统一。
        private static List<PValue>[] _inputData;
        public static List<PValue>[] inputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
            }
        }

        private static CalcuInfo _calcuInfo;
        public static CalcuInfo calcuInfo
        {
            get
            {
                return _calcuInfo;
            }
            set
            {
                _calcuInfo = value;
            }
        }

        private static List<PValue>[] _outputData;
        public static List<PValue>[] outputData
        {
            get
            {
                return _outputData;
            }
            set
            {
                _outputData = value;
            }
        }        
        #endregion

        #region 计算模块算法
        /// <summary>
        /// 计算模块算法实现:求几个数字量之和
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>计算结果数字量之和</returns>
       
        public static Results Calcu()
        {
            return Calcu(_inputData, _calcuInfo);
        }
        public static Results Calcu(List<PValue>[] inputs, CalcuInfo calcuinfo)
        {
            //公用变量           
            bool _errorFlag = false;
            string _errorInfo = "";
            bool _warningFlag = false;
            string _warningInfo = "";
            bool _fatalFlag = false;
            string _fatalInfo = "";

            int i;

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，则输出项也为空.
            List<PValue>[] results = new List<PValue>[1];
            results[0] = new List<PValue>();
            results[0].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));

            try
            {
                //0、输入                
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                //这里注意，如果某一个标签为空，则不退出。利用剩下的标签求平均值
                if (inputs == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值
                }

                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。 
                for (i = 0; i < inputs.Length; i++)
                    if (inputs[i] == null || inputs[i].Count == 1)
                    {
                        _warningFlag = true;  //报错
                        _warningInfo = "输入标签有一个数据为空。M2DigitalSum算法要求两个输入量必须都有有效数据";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                    else
                        inputs[i].RemoveAt(inputs[i].Count - 1);

                //0.3、输入处理：标志位。过滤标志位。并将非空点取出来
                int validinputsNumber = 0;                

                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] != null)
                    {
                        for (int j = inputs[i].Count - 1; j >= 0; j--)
                        {
                            if (inputs[i][j].Status != 0) inputs[i].RemoveAt(j);
                        }
                        //删除完异常点后，检查长度
                        if (inputs[i].Count != 0)
                        {                            
                            validinputsNumber = validinputsNumber + 1;
                        }
                    }

                }

                //0.5对于过滤完非正常状态和除去截止时刻的点
                if (validinputsNumber < 2)
                {
                    _warningFlag = true;  //报错
                    _warningInfo = "输入数据经状态位过滤后，至少有一个数据为空。M2DigitalSum算法要求两个输入量必须都有有效数据。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //1、将输入的几个数字量组合成一个列表spanconcat
                //——在数字量变化的收尾添加si和ei的标志。
                //——比如inputs[1]代表的数字量的值为1,0,1,0,1,1,0,0。形成的标志是：
                //——(0.Timestamp,s0),(0.EndTime,e0),(1.Timestamp,e0),(1.EndTime,s0),(2.Timestamp,s0),(2.EndTime,e0),(3.Timestamp,e0),(3.EndTime,s0),(4.Timestamp,s0),(4.EndTime,e0),(5.Timestamp,s0),(5.EndTime,e0)
                //——对于第四位和第五位连续的两个1，虽然在第4点结束的时刻有一个(4.EndTime,e0)标志，但是同样的时刻(5.Timestamp,s0)，在计算时，会把同一个时刻值都分析并对各自flag置位，然后才求和因此不会发生错误。
                //——为了兼容数字量里面有连续的1,1,1，即状态标志会出现(s,e),(s,e),(s,e)，要保证同一时刻，结尾的e和开头的s的位置不能改变。否则就会出现Flag置位错误。
                //——为了兼容数字量里面有连续的0,0,0，即状态标志会出现(e,s),(e,s),(e,s)，要保证同一时刻，结尾的s和开头的e的位置不能改变。否则就会出现Flag置位错误。
                List<SpanSeries> spanconcat = new List<SpanSeries>();
                for (i = 0; i < inputs.Length; i++)
                {
                    foreach (PValue pv in inputs[i] )
                    {                       
                            if (pv.Value == 1)
                            {
                                spanconcat.Add(new SpanSeries(pv.Timestamp, "s" + i.ToString(),"0"));
                                spanconcat.Add(new SpanSeries(pv.Endtime, "e" + i.ToString(),"0"));
                            }
                            else
                            {
                                spanconcat.Add(new SpanSeries(pv.Timestamp, "e" + i.ToString(),"0"));
                                spanconcat.Add(new SpanSeries(pv.Endtime, "s" + i.ToString(),"0"));
                            }
                    }
                        
                }

                //2、对列表按发生时间进行排序
                //各个数字量的开始和结束标志，统一按照放生时间进行排序。然后从头到尾，对标志位进行便利。
                //每当标志改变时，说明对应的数字量的值发生改变（从1变成0，或者从零变成1）。此时应当对各数字量的标志位进行置位或者复位。然后计算标志位的总和。
                //在同一时刻会出现多个标志的改变情况，这里分三种：
                //——不同数字的的值改变发生在同一时间，这种标志指向不同的标志位，相互不影响。
                //——同一的数字的值，因为1中整理过程的原因，必然是前一个值结尾处的标志和后一个值起始时间的标志，时间上完全一致。对于正常的数字量，应该是0和1间隔，前一个值结尾处的标志和后一个值起始时间的标志应该相同。他们表明的是同一个意思。因此置flag的结果也相同。
                //——对于非正常的数字量，会出现连续0或者连续1的情况，前一个值结尾处的标志和后一个值起始时间的标志，就相反。此时，排序后的数组，必须保持这两个标志的原有顺序。
                //spanconcat = spanconcat.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList();    //这个排序会改变上述第三种情况下的标志排序
                spanconcat = spanconcat.OrderBy(m => m.Datetime).ToList();                          //这个排序不会改变上述第三种情况下的标志排序
                
                //3、对spanconcat进行遍历，计算逻辑和。                
                int[] flagSeries=new int[inputs.Length];            //入参的数学标签对应的逻辑位。当标签值为0，对应的逻辑位为0.当标签值为1，对应的逻辑为为1。
                for(i=0;i<flagSeries.Length;i++)
                {
                    flagSeries[i]=0;
                }
                DateTime currentDate=spanconcat[0].Datetime;        //当前计算时间
                DateTime lastDate=spanconcat[0].Datetime;           //上一次计算时间
                double   currentSum=0;                              //当前标志位的和
                int currentIndex = 0;                               //当前标志位对应的数字量序号
                string currentStatus = "";                          //当前标志位对应的数字量状态

                List<PValue> result = new List<PValue>();               
                for (i = 0; i < spanconcat.Count; i++) 
                {
                    //读取当前时间
                    currentDate=spanconcat[i].Datetime;
                    //如果当前时间等于上一次时间，则把统一时间的状态变化依次进行处理，改变flagSeries
                    if(currentDate ==lastDate)
                    {  
                        //如果当前的时间等于上一次时间，则仅更新flagSeries
                        currentStatus = spanconcat[i].Flag.Substring(0, 1);
                        currentIndex = int.Parse(spanconcat[i].Flag.Substring(1, spanconcat[i].Flag.Length - 1));
                        if (currentStatus == "s")
                            flagSeries[currentIndex] = 1;
                        else
                            flagSeries[currentIndex] = 0;
                    }
                    else
                    {//如果当前时间不等于上一次时间，说明上一个时刻的所有状态变化处理完毕，flagSeries以代表上一个时刻最后状态
                        
                        currentSum=flagSeries.Sum();    //先求当前和
                        if(result.Count==0)//第一个点    //再写入点的值
                        {
                            //写入上一个时刻点的最后总和
                            result.Add(new PValue(currentSum, lastDate, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));                            
                        }
                        else//后面的点
	                    { 
                            //填写上上时刻点的结束时间
                            result[result.Count-1].Endtime=lastDate;
                            //写入上一个时刻点的最后总和
                            result.Add(new PValue(currentSum, lastDate, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                        }
                        lastDate = currentDate;
                    }    
                }//endfor
                //写结束点的末尾
                result[result.Count - 1].Endtime = lastDate;                

                //组织计算结果
                results = new List<PValue>[1];
                results[0]=result;
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                

            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuInfo.fmodulename, calcuInfo.sourcetagname, calcuinfo.fstarttime.ToString(), calcuinfo.fendtime.ToString());
                //LogHelper.Write(LogType.Error, moduleInfo);
                //计算引擎报错具体信息
                //string errInfo = string.Format("——具体报错信息：{0}。", ex.ToString());
                //LogHelper.Write(LogType.Error, errInfo);
                //返回null供计算引擎处理
                _fatalFlag = true;
                _fatalInfo = ex.ToString();
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
        #endregion
    }
}
