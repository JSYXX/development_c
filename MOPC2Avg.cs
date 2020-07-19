using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon;                         //使用PValue
using System.Text.RegularExpressions;   //使用正则表达式

namespace PSLCalcu.Module
{
    /// <summary>
    /// (阶梯曲线)OPC数据读取固定间隔数据求均值
    /// 将OPC数据表opcdataxxxx中的实时数据，每分钟读取一次，并求均值    
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///		2019.07.17 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2019.07.17</date>
    /// </author> 
    /// </summary>
    public class MOPC2WAvg : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MOPC2WAvg";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "OPC（或实时）数据求加权均值";
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
        private string _algorithms = "OPC2WAvg";
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
        private string _moduleParaExample = "0;1;0;L";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "二次系数;一次系数；平移系数;用上下限进行过滤的标志";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){2}(;L){0,1}$");
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
        private string _outputDescs = "FOPC2WAvg";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "OPC数据加权均值";
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
        /// 计算模块算法实现:(阶梯曲线)OPC数据滤波
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志。本算法无效</param>
        /// <param name="calcuinfo.fparas"></param>       
        /// <returns>OPC滤波数据</returns>       
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


            //0输出初始化:输入为空或者计算出错时，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[1];
            results[0] = new List<PValue>();
            results[0].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));

            try
            {

                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，给出标志位为StatusConst.InputIsNull的计算结果.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法，截止时刻点不需要参与计算，要删除。                
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。                
                for (i = input.Count - 1; i >= 0; i--)
                {
                    if (input[i].Status != 0)
                    {
                        input.RemoveAt(i);
                    }
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回全0
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "源数据不满足要求，源数据至少要包括一个起始时刻有效数据和一个截止时刻数据。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //处理参数
                string[] para = calcuinfo.fparas.Split(new char[] { ';' });
                double A = double.Parse(para[0]);
                double B = double.Parse(para[1]);
                double C = double.Parse(para[2]);
                string FilterWithLimit = "";
                if (para.Length > 3) FilterWithLimit = para[3].ToString();

                //求加权平均值
                double WSum = 0;//加权累加值
                double WTotalSpan = 0;
                double WAvg = 0;//加权平均值
                for (i = 0; i < input.Count; i++)
                {
                    WSum = WSum + input[i].Value * input[i].Timespan;
                    WTotalSpan = WTotalSpan + input[i].Timespan;
                }

                WAvg = WSum / WTotalSpan;



                //组织计算结果
                results[0] = new List<PValue>();               
              
                //对均值进行线性变换
                double filtervalue = A * WAvg * WAvg + B * WAvg + C;  //对平均值进行变换。注意一般情况是A=0；B=1；B=0；D=0，即filtervalue=ave

                //如果参数中含有第四个变量，且变量值为L，则对计算结果用上下限进行过滤
                if (FilterWithLimit == "L")
                {
                    if (filtervalue < calcuinfo.sourcetagmrb)
                        results[0].Add(new PValue(filtervalue, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputOverLimit));
                    else if (filtervalue > calcuinfo.sourcetagmre)
                        results[0].Add(new PValue(filtervalue, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputOverLimit));
                    else
                        results[0].Add(new PValue(filtervalue, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                else//没有参数L，不过滤
                {
                    results[0].Add(new PValue(filtervalue, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }



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
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }

        }//end module
        #endregion


    }
}
