using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using System.Linq;                      //使用list的orderby

namespace PSLCalcu.Module.Condition
{
    /// <summary>
    /// (阶梯曲线)限定条件时间段大于阈值有效  
    /// 给定单一标签的限定条件[Min,Max]，求出标签实时值在满足限定条件范围内的时间段，如果这些时间段长度有大于最小阈值的时间段，则整个时间段有效。
    /// ——关于时间段span的算法，一律都和时间序列的先后有关，因此要求input参数，pvalue值是按照时间先后顺序排列的。
    /// ——如果input的pvalue值没有按照时间排序，则会造成结果的错误。这一点要特别注意。
    /// ——如果限定条件形式为"[20,70],0,0",则lowerLimit=20，upperLimit=70,其含义是要找某标签实时值在20~70范围内的时间段。
    /// ——如果限定条件形式为"[20,],0,0",则lowerLimit=20，upperLimit=MaxValue，其含义是要找某标签实时值大于20的时间段。
    /// ——如果限定条件形式为"[,70],0,0",则lowerLimit=MinValue，upperLimit=70，其含义是要找某标签实时值小于70的时间段。
    /// ——如果限定条件形式为"(20,70）,0,0",圆括号表示开区间,上下限可以分别选择开区间或者闭区间。
    /// ——如果限定条件形式为"(20,70),0,0",第三个参数是要过滤的小时间段阈值，单位为秒。表示在符合条件[Min,Max]的时间系列内，过滤掉那些小于5分钟的时间段。第四个参数是迟延时间，单位为秒。
    /// 
    /// 关于计算精度的说明
    /// ——本计算将实时数据视为阶梯曲线，而非折线。       
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2017.03.20 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2017.03.20</date>
    /// </author> 
    /// </summary>
    class MFCondSpanValid:BaseModule, IModule,IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MCondSpanValid";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "(阶梯曲线)限定条件时间段大于阈值则有效";
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
        private string _algorithms = "MCondSpanValid";
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
        private string _moduleParaExample = "(20;50);600";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "(限定条件值域下限;限定条件值域上限）;最小时间段阈值(秒)——典型示例:[20;50];0;0||(20;50);0;0||[20;50);60;10||(20;50];5;-100||[20;]0;0||(;50);0||——最小时间段阈值和延时都必须填写";
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        //正则表达式，用来检测计算的参数，是否符合要求
        //——@表示不转义字符串。@后的“”内是正则表达式内容
        //——正则表达式字符串以^开头，以$结尾。这部分可以通过正则表达式来测试想要的效果
        //——(\[|\(){1}：左侧必须是一个[或者一个(，至少出现一次
        //——[+-]?出现正号或者负号0次或者一次；\d+匹配多位数字；(\.\d+)匹配多位小数，([+-]?\d+(\.\d+)?){0,1}一个或正或负多位整数或者多位浮点数，出现0次或者1次。
        //(,){1}，表示区间中间的逗号必须出现一次。
        //([+-]?\d+){1},表示区间时间段最小阈值，必须为正，必须出现一次。单位是秒
        //([+-]?\d+){1},表示区间后面的延时参数，可正可负，必须出现一次。单位是秒

        private Regex _moduleParaRegex = new Regex(@"^(\[|\(){1}([+-]?\d+(\.\d+)?){0,1}(;){1}([+-]?\d+(\.\d+)?){0,1}(\)|]){1}(;){1}([+]?\d+){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
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
        private string _outputDescs = "FCSpanValid";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "符合限定条件的时间段(毫秒)";
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
        private bool _outputPermitNULL = true;
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
        /// <param name="calcuinfo.fparas">限定条件，下限在前，上限在后，用分号分隔。</param>       
        /// <returns>实时值落在限定条件范围内的时间段序列</returns>       
        public static Results Calcu()
        {
            return Calcu(_inputData, _calcuInfo);
        }
        public static Results Calcu(List<PValue>[] inputs, CalcuInfo calcuinfo)
        {
            //本计算需要获得限定条件上下限
            //如果限定条件形式为"[20,70]",则lowerLimit=20，upperLimit=70,其含义是要找某标签实时值在20~70范围内的时间段
            //如果限定条件形式为"(20,70)",则lowerLimit=20，upperLimit=70,其含义是要找某标签实时值在20~70范围内的时间段，不包含边界值
            //如果限定条件形式为"[20,]",则lowerLimit=20，upperLimit=MaxValue，其含义是要找某标签实时值大于等于20的时间段
            //如果限定条件形式为"(20,)",则lowerLimit=20，upperLimit=MaxValue，其含义是要找某标签实时值大于20的时间段
            //如果限定条件形式为"[,70]",则lowerLimit=MinValue，upperLimit=70，其含义是要找某标签实时值小于等于70的时间段
            //如果限定条件形式为"(,70)",则lowerLimit=MinValue，upperLimit=70，其含义是要找某标签实时值小于70的时间段
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
                List<PValue> input = inputs[0];

                //处理截止时刻数据
                if (input.Count > 1) input.RemoveAt(input.Count - 1);

                //1、准备界限数组\和区间边界标识
                double[] LimitRange = new double[4];      //界限数组，用于查找当前value值处于的值域段
                string CompareStr = "";                   //比较符     
                int len = calcuinfo.fparas.Length;
                string[] paras = Regex.Split(calcuinfo.fparas, ";|；");      //要测试一下，split分割完仅有三个元素是否可以                
                CompareStr = String.Format("{0}{1}", paras[0].Substring(0, 1), paras[1].Substring(paras[1].Length - 1, 1));
                paras[0] = paras[0].Substring(1, paras[0].Length - 1);          //范围的下边界“(20”中把20取出
                paras[1] = paras[1].Substring(0, paras[1].Length - 1);          //范围的上边界“50）”中把50取出                
                LimitRange[0] = float.MinValue;
                LimitRange[3] = float.MaxValue;
                if (paras[0].Trim() == "")
                {
                    LimitRange[1] = float.MinValue;
                }
                else
                {
                    LimitRange[1] = float.Parse(paras[0]);
                }

                if (paras[1].Trim() == "")
                {
                    LimitRange[2] = float.MaxValue;
                }
                else
                {
                    LimitRange[2] = float.Parse(paras[1]);
                }
                //如果输入时，上下限填写反了，则对调
                if (LimitRange[1] > LimitRange[2])
                {
                    double temp = LimitRange[1];
                    LimitRange[1] = LimitRange[2];
                    LimitRange[2] = temp;

                }
                double threshold;
                if (paras.Length >= 3 && paras[2] != "")
                {
                    threshold = float.Parse(paras[2]);
                }
                else
                {
                    threshold = 0;  //如果没有给定threshold值，则取默认值0秒钟
                }
                double delay;
                if (paras.Length >= 4 && paras[3] != "")
                {
                    delay = float.Parse(paras[3]);
                }
                else
                {
                    delay = 0;  //如果没有给定threshold值，则取默认值0秒钟
                }

                //2、准备符合条件的时间段存储变量

                //3、对数据进行遍历
                int currentRange = 10;      //当前点是否位于定义区间内
                int previousRange = 10;     //上一个点是否位于定义区间内
                PValue currentPoint;       //当前点
                PValue previousPoint;      //上一点                
                List<PValue>[] spans = new List<PValue>[3]; //三个值域段的时间段序列
                spans[0] = new List<PValue>();
                spans[1] = new List<PValue>();
                spans[2] = new List<PValue>();

                //4.遍历所有点
                for (int iPoint = 0; iPoint < input.Count; iPoint++)
                {
                    //4.0、选择当前点和上一个点
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

                    //4.1、判断当前点是否在条件区间内
                    //对于两点的区间判断，采用下面更为高效的判断方式
                    switch (CompareStr)
                    {
                        case "()":
                            if (input[iPoint].Value >= LimitRange[2])
                                currentRange = 2;
                            else if (input[iPoint].Value > LimitRange[1] && input[iPoint].Value < LimitRange[2])
                                currentRange = 1;
                            else
                                currentRange = 0;
                            break;
                        case "[]":
                            if (input[iPoint].Value > LimitRange[2])
                                currentRange = 2;
                            else if (input[iPoint].Value >= LimitRange[1] && input[iPoint].Value <= LimitRange[2])
                                currentRange = 1;
                            else
                                currentRange = 0;
                            break;
                        case "(]":
                            if (input[iPoint].Value > LimitRange[2])
                                currentRange = 2;
                            else if (input[iPoint].Value > LimitRange[1] && input[iPoint].Value <= LimitRange[2])
                                currentRange = 1;
                            else
                                currentRange = 0;
                            break;
                        case "[)":
                            if (input[iPoint].Value >= LimitRange[2])
                                currentRange = 2;
                            else if (input[iPoint].Value >= LimitRange[1] && input[iPoint].Value < LimitRange[2])
                                currentRange = 1;
                            else
                                currentRange = 0;
                            break;
                        default://如果以上情况均不是，则按()情况来做。
                            if (input[iPoint].Value >= LimitRange[2])
                                currentRange = 2;
                            else if (input[iPoint].Value > LimitRange[1] && input[iPoint].Value < LimitRange[2])
                                currentRange = 1;
                            else
                                currentRange = 0;
                            break;

                    }//end switch  

                    //4.2、判断点的位置（是否位于条件区间内）是否有变化
                    //4.2.1、处理第一个点。当previousStatus == 10时，是第一个点
                    if (previousRange == 10)
                    {
                        previousRange = currentRange;
                        spans[currentRange].Add(new PValue(1, input[iPoint].Timestamp.AddSeconds(delay), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    }
                    else
                    {
                        //4.2.2、处理中间点
                        if (currentRange == previousRange)
                        {
                            //如果当前点的位置没有变化,则看当前点的开始时间和上一个点的截止时间是否一样
                            //如果不一样，说明时间不连续，需要记录该点
                            if (currentPoint.Timestamp != previousPoint.Endtime)
                            {
                                spans[previousRange][spans[previousRange].Count - 1].Endtime = previousPoint.Endtime.AddSeconds(delay);
                                spans[previousRange].Add(new PValue(1, currentPoint.Timestamp.AddSeconds(delay), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                            }
                            //如果一样则什么也不做
                        }
                        else
                        {
                            //写上一个区域的结束值
                            spans[previousRange][spans[previousRange].Count - 1].Endtime = previousPoint.Endtime.AddSeconds(delay);  //该交点的时间计算，精确到毫秒
                            //写下一个区域的开始值
                            spans[currentRange].Add(new PValue(1, currentPoint.Timestamp.AddSeconds(delay), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));

                            previousRange = currentRange;

                        }
                    }
                    //4.2.3 处理结束点
                    if (iPoint == input.Count - 1)
                    {
                        spans[currentRange][spans[currentRange].Count - 1].Endtime = currentPoint.Endtime.AddSeconds(delay);
                    }

                }//end for

                //5、对找出的时间序列进行判断，如果时间段中有时间大于最小阈值，则整个时间段有效
                bool  spansvalid = false;
                for (i = spans[1].Count - 1; i >= 0; i--)
                {
                    if (threshold > 0)
                    {
                        if (spans[1][i].Timespan >= threshold * 1000)      //注意pvalue中的Timespan记录的是毫秒值
                        {
                            spansvalid = true;
                            break;
                        }
                    }                   
                }

               

                //7、组织计算结果
                //results[0]对应小于下限的时间段，results[1]对应于下限和上限之间的时间段，results[2]对应于大于上限的时间段
                List<PValue> spanseries = new List<PValue>();
                if (spansvalid)
                {
                    spanseries.Add(new PValue(1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                    List<PValue>[] results= new List<PValue>[1] { spanseries};
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    //计算结果不允许为null，MFCondSpanValid算法例外，当MFCondSpanValid算法为空，主程序不报警
                    return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                
                
            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuInfo.fmodulename, calcuInfo.sourcetagname,calcuinfo.fstarttime.ToString(),calcuinfo.fendtime.ToString());
                //LogHelper.Write(LogType.Error, moduleInfo);
                //计算引擎报错具体信息
                //string errInfo=string.Format("——具体报错信息：{0}。", ex.ToString());
                //LogHelper.Write(LogType.Error, errInfo);
                //返回null供计算引擎处理

                _fatalFlag = true;
                _fatalInfo = ex.ToString();
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }

        }//end module
        #endregion
    }
}
