using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue

namespace PSLCalcu.Module
{
    /// <summary>
    /// 数字量置位（复位）时间段 
    /// 给定统计条件，即统计1还是统计0，求出数字量为1或者为0的时间段
    /// ——关于时间段span的算法，一律都和时间序列的先后有关，因此要求input参数，pvalue值是按照时间先后顺序排列的。
    /// ——如果input的pvalue值没有按照时间排序，则会造成结果的错误。这一点要特别注意。
    /// ——限定条件形式为"1,60,5",1表示统计数字量为1时的时间段。60表示过滤时间段最小阈值，单位为秒，必须为正。5表示延迟时间，单位为秒，可正可负。   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2018.01.03 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.01.03</date>
    /// </author> 
    /// </summary>
    public class MDigitalSpan : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "DigitalSpan";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "数字量置位（复位）时间段";
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
        private string _algorithms = "DigitalSpan";
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
        private string _moduleParaExample = "1;60;300";
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "统计置位或复位;最小时间段阈值(秒);延时(秒)。——典型示例:1;60;300";
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
        //——([01]){1}，第一个参数是表示选择统计置位时间段还是复位时间段，必须是1或者0
        //——(,){1}([+]?\d+){1}，第二个参数是最小时间段过滤，是一个以秒为单位的正整数或者0。
        //——(,){1}([+-]?\d+){1}，第三个参数是迟延时间，是一个以秒为单位的正数或者负数，或者0.
        private Regex _moduleParaRegex = new Regex(@"^([01]){1}(;){1}([+]?\d+){1}(;){1}([+-]?\d+){1}$");
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
        private string _outputDescs = "DigitalSpan";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "数字量置位的时间段序列";
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
        /// 计算模块算法实现:对时间段span求延时时间段
        /// </summary>
        /// <param name="input">时间段序列</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志。本算法无效</param>
        /// <param name="calcuinfo.fparas">延时秒数。</param>       
        /// <returns>对时间段序列进行延时后的时间段序列</returns>
        
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
                List<PValue> input = inputs[0];

                //处理截止时刻数据
                if (input.Count > 1) input.RemoveAt(input.Count - 1);

                List<PValue> digitalSpan = new List<PValue>();

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
                double delay;
                if (paras.Length >= 3 && paras[2] != "")
                {
                    delay = float.Parse(paras[2]);          //取第三个参数，迟延时间，单位秒
                }
                else
                {
                    delay = 0;  //如果没有给定threshold值，则取默认值0秒钟
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
                        spans[currentRange].Add(new PValue(1, input[iPoint].Timestamp.AddSeconds(delay), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    }
                    //4.2.2、处理中间点
                    if (currentRange == previousRange)
                    {
                        //如果当前点的位置没有变化,则什么也不做                        
                    }
                    else
                    {
                        //如果当前点的位置有变化

                        spans[previousRange][spans[previousRange].Count - 1].Endtime = currentPoint.Timestamp.AddSeconds(delay);  //该交点的时间计算，精确到毫秒
                        spans[currentRange].Add(new PValue(1, currentPoint.Timestamp.AddSeconds(delay), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                        previousRange = currentRange;
                    }
                    //4.2.3 处理结束点
                    if (iPoint == input.Count - 1)
                    {
                        spans[currentRange][spans[currentRange].Count - 1].Endtime = currentPoint.Endtime.AddSeconds(delay);
                    }

                }//end for

                //对找出的时间序列进行过滤，如果长度小于threshold，则去掉

                for (i = spans[set].Count - 1; i >= 0; i--)
                {
                    if (threshold > 0)
                    {
                        if (spans[set][i].Timespan < threshold * 1000)      //注意pvalue中的Timespan记录的是毫秒值
                        {
                            spans[set].RemoveAt(i);
                        }
                    }
                }
                //results[0]对应小于下限的时间段，results[1]对应于下限和上限之间的时间段，results[2]对应于大于上限的时间段
                List<PValue>[] results = new List<PValue>[1];
                results[0]= spans[set] ;
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

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
