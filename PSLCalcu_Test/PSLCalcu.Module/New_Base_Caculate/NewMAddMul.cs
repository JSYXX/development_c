using PCCommon;
using PCCommon.NewCaculateCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class NewMAddMul : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "NewMAddMul";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "多个变量之间的运算";
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
        private string _algorithms = "MAddMul";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "1;2";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "K;b";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;)([+-]?\d+(\.\d+)?){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 6;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "AddMulSum;" +
                                    "AddMulAbsSum;" +
                                    "AddMulMul;" +
                                    "AddMulAbsMul;" +
                                    "AddMulAvg;" +
                                    "AddMulAbsAvg";

        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "和;" +
                                        "绝对值和;" +
                                        "积;" +
                                        "绝对值积;" +
                                        "平均;" +
                                        "绝对值平均";

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
        //多输入参数标签顺序：
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
        /// 计算模块算法实现:和、绝对值的和、积、绝对值的积、平均值、绝对值的平均值
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara"></param>       
        /// <returns>计算结果分别为平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差)</returns>
        /// 

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

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[6];   //平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差);和;绝对值和
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }
            try
            {
                //0、输入
                List<PValue> input;
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    input = new List<PValue>();
                    //应当是每个输入只有一个值
                    for (int j = 0; j < inputs.Length; j++)
                    {
                        input.Add(inputs[j][0]);
                    }

                }

                //1 参数处理
                double k;
                double b;
                string[] paras = calcuinfo.fparas.Split(';');
                k = float.Parse(paras[0]);
                b = float.Parse(paras[1]);

                //计算结果初始化
                PValue AddMulSum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //和; 1
                PValue AddMulAbsSum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //绝对值的和; 2
                PValue AddMulMul = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //积;3
                PValue AddMulAbsMul = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //绝对值的积;4
                PValue AddMulAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //平均值;5
                PValue AddMulAbsAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//绝对值的平均值;6

                double sum = 0, abssum = 0, mul = 1, absmul = 1, avg, absavg;

                for (int i = 0; i < input.Count; i++)
                {
                    sum = sum + input[i].Value;
                    abssum = abssum + Math.Abs(input[i].Value);
                    mul = mul * input[i].Value;
                    absmul = absmul * Math.Abs(input[i].Value);
                }
                avg = sum / input.Count;
                absavg = abssum / input.Count;

                //对计算结果赋值
                AddMulSum.Value = k * sum + b;
                AddMulAbsSum.Value = k * abssum + b;
                AddMulMul.Value = k * mul + b;
                AddMulAbsMul.Value = k * absmul + b;
                AddMulAvg.Value = k * avg + b;
                AddMulAbsAvg.Value = k * absavg + b;

                //录入数据库       
                MAddMulOutClass MessageIN = new MAddMulOutClass();
                MessageIN.addMulSum = AddMulSum.Value;
                MessageIN.addMulAbsSum = AddMulAbsSum.Value;
                MessageIN.addMulMul = AddMulMul.Value;
                MessageIN.addMulAbsMul = AddMulAbsMul.Value;
                MessageIN.addMulAvg = AddMulAvg.Value;
                MessageIN.addMulAbsAvg = AddMulAbsAvg.Value;
                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                string hour = string.Empty;
                year = calcuinfo.fstarttime.Year.ToString();
                month = calcuinfo.fstarttime.Month.ToString();
                day = calcuinfo.fstarttime.Day.ToString();
                hour = calcuinfo.fstarttime.Hour.ToString();
                bool isok = BLL.AlgorithmBLL.insertMAddMul(MessageIN, calcuinfo.fsourtagids[0].ToString(), year, month, day, hour);
                if (isok)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MDeviationS数据录入数据库是失败";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }



                results[0] = new List<PValue>();
                results[0].Add(AddMulSum);
                results[1] = new List<PValue>();
                results[1].Add(AddMulAbsSum);
                results[2] = new List<PValue>();
                results[2].Add(AddMulMul);
                results[3] = new List<PValue>();
                results[3].Add(AddMulAbsMul);
                results[4] = new List<PValue>();
                results[4].Add(AddMulAvg);
                results[5] = new List<PValue>();
                results[5].Add(AddMulAbsAvg);
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

            }
            catch (Exception ex)
            {
                _fatalFlag = true;
                _fatalInfo = ex.ToString();
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
        #endregion
    }
}
