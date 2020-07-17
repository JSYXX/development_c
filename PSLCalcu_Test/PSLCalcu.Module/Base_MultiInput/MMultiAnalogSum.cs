using System;
using System.Collections.Generic;
using System.Linq;                      //使用linq
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 多个模拟量值序列求加法，得到一个新的模拟量值序列   
    /// ——两个模拟量值序列，第一个模拟量加上第二个模拟量的值，得到的结果作为新的模拟量返回。
    /// ——输出是两个模拟量的和。
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    /// 
    ///     2019.11.12      zuoweiqiang     添加了计算参数k,b    
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///     2018.01.09 arrow测试
    ///		2018.01.08 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.01.08</date>
    /// </author> 
    /// </summary>
    public class MMultiAnalogSum : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMultiAnalogSum";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "多个模拟量的加法，得到一个新的模拟量";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 2;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "加数;加数";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MultiAnalogSum";
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
        private string _outputDescs = "MANGSum";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "模拟量相加结果";
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
        #region 输入参数
        private string _moduleParaExample = "20;30";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "k;b";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
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
        /// 计算模块算法实现:
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数:备选输出量</param>       
        /// <returns></returns>

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

            int i, j;
            
            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[1];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
               

                //1、检查输入
                if (inputs.Length < 2)
                {
                    _errorFlag = true;
                    _errorInfo = "输入标签必须是两个或者多个。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                //注意数字量求和，允许其中一个数字量没有数值（为空或者长度为0）。当出现这种情况时，只计算剩余其他数字量的和
                if (inputs == null || inputs.Length == 0 )
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                
                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] != null && inputs[i].Count != 0)
                    {
                        if (inputs[i].Count > 1)
                            inputs[i].RemoveAt(inputs[i].Count - 1);
                        if (inputs[i].Count < 1)
                        {
                            _warningFlag = true;
                            _warningInfo = "对应时间段内每个标签的源数据至少包含起始时刻数据和截止时刻值。";
                            return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                        }
                    }
                }                   
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = 0; i < inputs.Length; i++)
                {
                    for (j = inputs[i].Count - 1; j >= 0; j--)
                    {
                        if (inputs[i][j].Status != 0) inputs[i].RemoveAt(j);
                    }
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].Count < 1)
                    {
                        _warningFlag = true;
                        _warningInfo = "输入标签源数据全部状态位异常。";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                }

                //参数处理
                double k;
                double b;
                string[] paras = calcuinfo.fparas.Split(';');
                k = float.Parse(paras[0]);
                b = float.Parse(paras[1]);

                //2、合并数组
                List<SpanSeries> spanconcat = new List<SpanSeries>();
                for (i = 0; i < inputs.Length; i++)
                {
                    foreach (PValue pv in inputs[i])
                    {
                        spanconcat.Add(new SpanSeries(pv.Timestamp, i.ToString() + ";" + pv.Value.ToString(),"0"));
                    }

                }

                //3、排序
                spanconcat = spanconcat.OrderBy(m => m.Datetime).ToList();

                //4、
                double[] valueSeries = new double[inputs.Length];
                for (i = 0; i < inputs.Length; i++)
                {
                    valueSeries[i] = 0;
                }                 
                double currentSum = 0;

                DateTime currentDate = spanconcat[0].Datetime;          //当前计算时间
                DateTime lastDate = spanconcat[0].Datetime;             //上一次计算时间                             
                int currentIndex = 0;                                   //当前标志位对应的模拟量序号
                double currentValue = 0;                               //当前标志位对应的模拟量值

                List<PValue> result = new List<PValue>();
                result.Add(new PValue(currentSum, spanconcat[0].Datetime, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                for (i = 0; i < spanconcat.Count; i++)
                {
                    //读取当前时间
                    currentDate = spanconcat[i].Datetime;
                    //如果当前时间等于上一次时间，则把统一时间的状态变化依次进行处理，改变flagSeries
                    if (currentDate == lastDate)
                    {
                        //如果当前的时间等于上一次时间，则仅更新flagSeries
                        string[] flags = spanconcat[i].Flag.Split(';');
                        currentIndex = int.Parse(flags[0]);
                        currentValue = double.Parse(flags[1]);
                        valueSeries[currentIndex] = currentValue;
                    }
                    else
                    {//如果当前时间不等于上一次时间，说明上一个时刻的所有状态变化处理完毕，flagSeries以代表上一个时刻最后状态
                        //2.1 写入上次结果
                        //currentSum = valueSeries.Sum();
                        currentSum = k*valueSeries.Sum()+b;
                        //填写上上时刻点的结束时间
                        result[result.Count - 1].Value = currentSum;
                        result[result.Count - 1].Endtime = currentDate;                 //不是第一个点，写入上一个数据的值和结束时间。         


                        //2.2 更新flagSeries
                        string[] flags = spanconcat[i].Flag.Split(';');
                        currentIndex = int.Parse(flags[0]);
                        currentValue = double.Parse(flags[1]);
                        valueSeries[currentIndex] = currentValue;
                        //currentSum = valueSeries.Sum();
                        currentSum = k * valueSeries.Sum() + b;

                        //2.3、写入新点的起始时间                        
                        result.Add(new PValue(0, currentDate, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));

                        lastDate = currentDate;
                    }
                }

                //写结束点的末尾:结尾点状态改变不改变已经不重要 
                //currentSum = valueSeries.Sum();
                currentSum = k * valueSeries.Sum() + b;
                result[result.Count - 1].Value = currentSum;
                result[result.Count - 1].Endtime = inputs[0][inputs[0].Count - 1].Endtime;

                //组织计算结果
                results[0] = new List<PValue>();
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
    }//endclass
}
