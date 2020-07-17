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
    /// 22段快速分布计算   
    /// ——对指定区域“min,max”进行分段统计，把[min,max]中平均分为20段。把（-∞，min]作为一段，把[max,+∞)做为一段，共22段。
    /// ——参数形式“0,100”。
    /// ——快速分布计算不统计各分段内的时间系列。
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2017.04.12 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2017.04.12</date>
    /// </author> 
    /// </summary>
    public class MFDistribute22 : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MFDistribute22";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "快速分布算法22段";
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
        private string _algorithms = "MFDistribute22";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "0;100";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "分段统计区间下限;分段统计区间上限";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){0,1}(;){1}([+-]?\d+(\.\d+)?){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 22;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "D22S0;D22S1;D22S2;D22S3;D22S4;D22S5;D22S6;D22S7;" +
                                      "D22S8;D22S9;D22S10;D22S11;D22S12;D22S13;D22S14;" +
                                      "D22S15;D22S16;D22S17;D22S18;D22S19;D22S20;D22S21"
                                      ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "22段分布第0段;22段分布第1段;22段分布第2段;22段分布第3段;22段分布第4段;" +
                                        "22段分布第5段;22段分布第6段;22段分布第7段;22段分布第8段;" +
                                        "22段分布第9段;22段分布第10段;22段分布第11段;22段分布第12段;" +
                                        "22段分布第13段;22段分布第14段;22段分布第15段;22段分布第16段;" +
                                        "22段分布第17段;22段分布第18段;22段分布第19段;22段分布第20段;" +
                                        "22段分布第21段";
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

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[22];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {

                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，所有的输出项为0.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法采用阶梯型算法，截止时刻点不参与计算。
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = input.Count - 1; i >= 0; i--)
                {
                    if (input[i].Status != 0) input.RemoveAt(i);
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回全0
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }          

                //0、对输入参数paras进行解析，输入参数一共2个值，代表分布统计的区间。如“0,100”
                //string[] paras = calcuinfo.fparas.Split(',');
                string[] paras = Regex.Split(calcuinfo.fparas, ";|；"); 
                double rangeMin = double.Parse(paras[0]);
                double rangeMax = double.Parse(paras[1]);
                if (rangeMax < rangeMin)       //如果大小顺序不对，则对调
                {
                    double temp = rangeMax;
                    rangeMax = rangeMin;
                    rangeMin = temp;
                }
                if (rangeMin == rangeMax) return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); //如果两者相等，则有问题，直接返回null

                //1、准备边界标识数组
                double[] LimitRange = new double[22 - 1];  //界限数组，用于查找当前value值处于的值域段。
                double rangeSection = (rangeMax - rangeMin) / (22 - 2);
                for (i = 0; i < (22 - 2); i++)
                {
                    LimitRange[i] = rangeMin + rangeSection * i;
                }
                LimitRange[22 - 2] = rangeMax;                //最后一个值直接用rangeMax，以避免误差

                //2、准备存储变量
                double[] section = new double[22];    //一共22个统计段，21个分隔点

                //3、对数据进行遍历
                double[] SortArray = new double[22];    //一共22个统计段，21个分隔点,加当前排序点，一共22个排序点
                int currentSection;
                for (int iPoint = 0; iPoint < input.Count; iPoint++)
                {
                    LimitRange.CopyTo(SortArray, 0);
                    SortArray[22 - 1] = input[iPoint].Value;              //对每一个数据用排序发确定其所在的区间
                    Array.Sort(SortArray);
                    currentSection = Array.IndexOf(SortArray, input[iPoint].Value); //这个排序划分区间，等价于(min,max]。

                    section[currentSection] = section[currentSection] + input[iPoint].Timespan;//将时间累加到对应的区间
                }

                //4、组织计算结果
                results = new List<PValue>[22];
                for (i = 0; i < 22; i++)
                {
                    results[i] = new List<PValue>();
                    results[i].Add(new PValue(section[i], calcuInfo.fstarttime, calcuinfo.fendtime, 0));
                }
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); ;
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
            }//end try    
        }//end calcu
        #endregion
    }
}
