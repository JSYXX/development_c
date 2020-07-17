using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue


namespace PSLCalcu.Module
{
    /// <summary>
    /// 数字量置位统计算法   
    /// ——统计数字量在给定时间内置位的次数、置位的时长、置位时长占比、置位时长最大值、置位时长最小值。
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///     2018.01.09 arrow测试
    ///		2018.01.08 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.01.08</date>
    /// </author> 
    /// </summary>
    public class MDigitalSetStats : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "DigitalSetStats";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "数字量置位统计算法：统计数字量在给定时间内置位的次数、置位的时长、置位时长占比、置位时长最大值、置位时长最小值";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 1;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "被统计量";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "DigitalSetStatistics";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        
        private int _outputNumber = 5;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "SetNumber;SetSpan;SetSpanRatio;SetSpanMax;SetSpanMin";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "置位次数;置位时长;置位时长占比;置位时长最大值;置位时长最小值";
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
        /// 计算模块算法实现:求实时值落在限定条件范围内的时间段span
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志。本算法无效</param>
        /// <param name="calcuinfo.fparas">限定条件，下限在前，上限在后，用分号分隔。如“25;70”，“25;”,";70"</param>       
        /// <returns>实时值落在限定条件范围内的时间段序列</returns>
       
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


            List<PValue>[] results = new List<PValue>[5];

            try
            {
                
                List<PValue> input = inputs[0];


                //处理截止时刻数据
                if (input.Count > 1) input.RemoveAt(input.Count - 1);

                //1、取参数
                string[] paras = calcuinfo.fparas.Split(new char[1] { ',' });
                int set = int.Parse(paras[0]);      //取第一个参数，按置位统计，还是按复位统计
                double threshold;
                if (paras.Length >= 2 && paras[1] != "")
                {
                    threshold = float.Parse(paras[1]);  //取第二个参数，过滤时间段最小阈值，单位秒
                }
                else
                {
                    threshold = 0;  //如果没有给定threshold值，则取默认值0秒钟
                }
                
                //2、对数据进行遍历
                int currentRange = 10;      //当前点是否位于定义区间内
                int previousRange = 10;     //上一个点是否位于定义区间内
                PValue currentPoint;       //当前点
                PValue previousPoint;      //上一点   
                List<PValue>[] spans = new List<PValue>[2]; //0或1值域段的时间段序列
                spans[0] = new List<PValue>();  //用于存放复位时间段
                spans[1] = new List<PValue>();  //用于存放置位时间段  

                //3、遍历所有点
                for (int iPoint = 0; iPoint < input.Count; iPoint++)
                {
                    //3.0、选择当前点和上一个点
                    if (iPoint == 0)
                    {
                        currentPoint = input[iPoint];
                        previousPoint = input[iPoint];
                    }
                    else
                    {
                        currentPoint = input[iPoint];
                        previousPoint = input[iPoint - 1];
                    }

                    //3.1、判断当前点是否在条件区间内
                    //对于两点的区间判断，采用下面更为高效的判断方式
                    if (currentPoint.Value == 1)
                    {
                        currentRange = 1;
                    }
                    else
                    {
                        currentRange = 0;
                    }

                    //3.2、判断点的位置（是否位于条件区间内）是否有变化
                    //3.2.1、处理第一个点。当previousStatus == 10时，是第一个点
                    if (previousRange == 10)
                    {
                        previousRange = currentRange;
                        spans[currentRange].Add(new PValue(1, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    }
                    //4.2.2、处理中间点
                    else if (currentRange == previousRange)
                    {
                        //如果当前点的位置没有变化,则什么也不做  
                    }
                    else
                    {
                        //如果当前点的位置有变化

                        spans[previousRange][spans[previousRange].Count - 1].Endtime = currentPoint.Timestamp;  //该交点的时间计算，精确到毫秒
                        spans[currentRange].Add(new PValue(1, currentPoint.Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0)); 
                        previousRange = currentRange;
                    }
                    //4.2.3 处理结束点
                    if (iPoint == input.Count - 1)
                    {

                        spans[currentRange][spans[currentRange].Count - 1].Endtime = currentPoint.Endtime;                        
                    }

                }//end for

                //对找出的时间序列进行过滤，如果长度小于threshold，则去掉
                double totalspan = 0;
                double max = spans[set][0].Value;
                double min = spans[set][0].Value;
                for (i = spans[set].Count - 1; i >= 0; i--)
                {
                    if (spans[set][i].Value > max) max = spans[set][i].Value;
                    if (spans[set][i].Value < min) min = spans[set][i].Value;
                    if (threshold > 0 && spans[set][i].Timespan < threshold * 1000)      //注意pvalue中的Timespan记录的是毫秒值
                    {
                        spans[set].RemoveAt(i);
                    }
                    else
                    {
                        totalspan = totalspan + spans[set][i].Timespan;
                    }
                    
                                      
                }
                //Timespan是毫秒，totalspan是毫秒
                double spanratio=totalspan/(calcuinfo.fendtime-calcuInfo.fstarttime).TotalMilliseconds;
                
                //组织计算解结果
                
                //组织计算结果:共返回3个统计结果
                //置位次数;置位时长;置位时长占比
                results[0] = new List<PValue>();
                results[0].Add(new PValue(spans[set].Count, calcuInfo.fstarttime, calcuinfo.fendtime, 0));  //次数
                results[1] = new List<PValue>();
                results[1].Add(new PValue(totalspan, calcuInfo.fstarttime, calcuinfo.fendtime, 0));         //时长
                results[2] = new List<PValue>();
                results[2].Add(new PValue(spanratio, calcuInfo.fstarttime, calcuinfo.fendtime, 0));         //占比
                results[3] = new List<PValue>();
                results[3].Add(new PValue(max, calcuInfo.fstarttime, calcuinfo.fendtime, 0));               //最大
                results[4] = new List<PValue>();
                results[4].Add(new PValue(min, calcuInfo.fstarttime, calcuinfo.fendtime, 0));               //最小


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
