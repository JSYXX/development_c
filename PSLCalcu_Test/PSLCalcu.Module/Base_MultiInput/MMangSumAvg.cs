using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class MMangSumAvg : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMangSumAvg";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "多个模拟量求和、求平均，得到一个新的模拟量";
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
        private string _inputDescsCN = "至少2个；最多不限";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMangSumAvg";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }

        private int _outputNumber = 2;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "MANGSum;MANGAvg";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "模拟量求和;求平均";
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
        private string _moduleParaExample = "20;30;20;30";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "k;b;k1;k2";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){0,1}(;){1}([+-]?\d+(\.\d+)?){0,1}(;){1}([+]?\d+){1}(;){1}([+-]?\d+){0,1}$");
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

            List<PValue>[] results = new List<PValue>[2];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }
            try
            {
                //0、输入
                //List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                //0.2、输入处理：截止时刻值。该算法，截止时刻点不需要参与计算，要删除。
                //本算法做特别处理
                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i].Count > 1) inputs[i].RemoveAt(inputs[i].Count - 1);
                }

                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                //********如果计算偏差前，参考量和被考核量，在计算的分钟周期内，分钟值为状态位无效的值。
                //********这里会读入这个值，并在下面过滤。如果被过滤，则下面的参考量和被考核量长度判断会报错。并返回不正常状态的计算结果。
                for (i = 0; i < inputs.Length; i++)
                {
                    for (int j = inputs[i].Count - 1; j >= 0; j--)
                    {
                        if (inputs[i][j].Status != 0) inputs[i].RemoveAt(j);
                    }
                }
                //处理参数
                double k;
                double b;
                List<double> kiList = new List<double>();
                string[] paras = calcuinfo.fparas.Split(';');
                k = float.Parse(paras[0]);
                b = float.Parse(paras[1]);
                //加权数处理
                for (int j = 2; j < paras.Count(); j++)
                {
                    kiList.Add(Convert.ToDouble(paras[j]));
                }
                //判断权数与数据数量是否相同

                if (kiList.Count != inputs.Length)
                {
                    _warningFlag = true;  //报错
                    _warningInfo = "权数与数据数量不一致。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //声明加权和
                double SumAmount = 0;
                for (int x = 0; x < inputs.Length; x++)
                {
                    SumAmount += inputs[x][0].Value * kiList[x];
                }
                //声明返回参数
                List<PValue> result = new List<PValue>();
                double MANGSum, MANGAvg = 0;
                MANGSum = k * SumAmount + b;
                MANGAvg = k * SumAmount / (double)(inputs.Length) + b;
                results[0] = new List<PValue>();
                results[0].Add(new PValue(MANGSum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[1] = new List<PValue>();
                results[1].Add(new PValue(MANGAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

            }
            catch (Exception ex)
            {
                _fatalFlag = true;
                _fatalInfo = ex.ToString();
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
        #endregion
    }
}
