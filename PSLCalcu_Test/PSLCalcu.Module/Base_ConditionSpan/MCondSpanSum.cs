using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 多个条件时间段数量求和
    /// ——入参是多个条件时间序列：比如1号机组启动状态时间序列，2号机组启动状态时间序列，3号机组启动状态时间序列，四号机组启动状态时间序列
    /// ——要计算，每个时刻处于启动状态的机组数量。
    /// 
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2018.01.13 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.01.13</date>
    /// </author> 
    /// </summary>
    public class MCondSpanSum:BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "CondSpanSum";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "多个条件时间段数量求和";
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
        private string _inputDescsCN = "至少2个；最多不限";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "CondSpanSum";
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
        private string _outputDescs = "CondSpanSum";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "有效时间段数量和";
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
        /// 计算模块算法实现:求同时有效的条件时间段数量
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>计算结果分别为最大值、最小值、极差值</returns>
       
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

            try
            {
                //1.2 非线性插值，不需要截止时刻数值
                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].Count > 1) inputs[i].RemoveAt(inputs[i].Count - 1);
                }
                
                //1、将输入的几个数字量组合成一个列表spanconcat
                List<SpanSeries> spanconcat = new List<SpanSeries>();                
                spanconcat.Add(new SpanSeries(calcuinfo.fstarttime, "e0","0"));     //将统计时间段的起始时间加入
                for (i = 0; i < inputs.Length; i++)
                {
                    foreach (PValue pv in inputs[i])
                    {
                        
                            spanconcat.Add(new SpanSeries(pv.Timestamp, "s" + i.ToString(),"0"));
                            spanconcat.Add(new SpanSeries(pv.Endtime, "e" + i.ToString(),"0"));
                        
                    }

                }
                spanconcat.Add(new SpanSeries(calcuinfo.fendtime, "e0","0"));     //将统计时间段的起始时间加入
                //2、对列表按发生时间进行排序
                spanconcat = spanconcat.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList();
                
                //3、对spanconcat进行遍历，计算逻辑和。 
                int[] flagSeries = new int[inputs.Length];            //入参的数学标签对应的逻辑位。当标签值为0，对应的逻辑位为0.当标签值为1，对应的逻辑为为1。
                for (i = 0; i < flagSeries.Length; i++)
                {
                    flagSeries[i] = 0;
                }
                DateTime currentDate = spanconcat[0].Datetime;        //当前计算时间
                DateTime lastDate = spanconcat[0].Datetime;           //上一次计算时间
                double currentSum = 0;                              //当前标志位的和
                int currentIndex = 0;                               //当前标志位对应的数字量序号
                string currentStatus = "";                          //当前标志位对应的数字量状态
                List<PValue> result = new List<PValue>();
                for (i = 0; i < spanconcat.Count; i++)
                {
                    //读取当前时间
                    currentDate = spanconcat[i].Datetime;
                    if (currentDate == spanconcat[spanconcat.Count - 1].Datetime) break;        //如果时间等于最后一个点的时间，直接跳出循环。这种方式，主要解决有多个点同时结束的情况。
                    //如果当前时间等于上一次时间，则把统一时间的状态变化依次进行处理，改变flagSeries
                    if (currentDate == lastDate)
                    {
                        //1、如果当前的时间等于上一次时间，则仅更新flagSeries
                        currentStatus = spanconcat[i].Flag.Substring(0, 1);
                        currentIndex = int.Parse(spanconcat[i].Flag.Substring(1, spanconcat[i].Flag.Length - 1));
                        if (currentStatus == "s")
                            flagSeries[currentIndex] = 1;
                        else
                            flagSeries[currentIndex] = 0;
                    }
                    else
                    {//如果当前时间不等于上一次时间，说明上一个时刻的所有状态变化处理完毕，flagSeries以代表上一个时刻最后状态

                        //2、如果当前的时间不等于上一次时间，先写入上次结果和结束时间，再先更新flagSeries，再创建新的一段值的起始时间
                        //2.1 写入上次结果                        
                        currentSum = flagSeries.Sum(); 
                        if (result.Count == 0)//第一个点    
                        {                                   
                            result.Add(new PValue(currentSum, lastDate, currentDate,0));    //写入第一个点，包括起始和截止时间                          
                        }
                        else //不是第一个点
                        {
                            //填写上上时刻点的结束时间
                            result[result.Count - 1].Value = currentSum; 
                            result[result.Count - 1].Endtime = currentDate;                 //不是第一个点，写入上一个数据的值和结束时间。         
                        }

                        //2.2 更新flagSeries
                        currentStatus = spanconcat[i].Flag.Substring(0, 1);
                        currentIndex = int.Parse(spanconcat[i].Flag.Substring(1, spanconcat[i].Flag.Length - 1));
                        if (currentStatus == "s")
                            flagSeries[currentIndex] = 1;
                        else
                            flagSeries[currentIndex] = 0;
                        currentSum = flagSeries.Sum();    //先求当前和

                        //2.3、写入新点的起始时间                        
                        result.Add(new PValue(0, currentDate, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                       
                        lastDate = currentDate;
                    }
                }//endfor
                
                //写结束点的末尾:结尾点状态改变不改变已经不重要               
                result[result.Count - 1].Value = currentSum;
                result[result.Count - 1].Endtime = spanconcat[spanconcat.Count - 1].Datetime;

                //组织计算结果
                List<PValue>[] results = new List<PValue>[1];
                results[0] = result;
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
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
         #endregion
    }
}
