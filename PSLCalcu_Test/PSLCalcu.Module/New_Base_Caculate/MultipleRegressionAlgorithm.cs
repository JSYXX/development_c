using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.NewCaculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module.New_Base_Caculate
{
    public class MultipleRegressionAlgorithm : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MultipleRegressionAlgorithm";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "多元线性回归";
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
        private string _algorithms = "MultipleRegressionAlgorithm";
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
        private string _moduleParaExample = "1,1;1,1;1:2*2*3,1:3*2*1,5:1*4*2;";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        //去最大百分比滤波处理数组;去最小百分比滤波处理数组;预处理方式
        private string _moduleParaDesc = "vib;vis;XF;";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){11}$");
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
        private string _outputDescs = "sey;" +
                                     "r2;" +
                                     "F;" +
                                     "df;" +
                                     "ssreg;" +
                                     "ssresid;" +
                                     "m1;" +
                                     "m2;" +
                                     "m3;" +
                                     "m4;" +
                                     "m5;" +
                                     "m6;" +
                                     "m7;" +
                                     "m8;" +
                                     "m9;" +
                                     "m10";

        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "y 估计值的标准误差" +
                                        "判定系数;" +
                                        "统计、观察值;" +
                                        "自由度;" +
                                        "回归平方和;" +
                                        "残差平方和;" +
                                        "X1 系数;" +
                                        "X2 系数;" +
                                        "X3 系数;" +
                                        "X4 系数;" +
                                        "X5 系数;" +
                                        "X6 系数;" +
                                        "X7 系数;" +
                                        "X8 系数;" +
                                        "X9 系数;" +
                                        "X10 系数;";

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
            List<PValue>[] results = new List<PValue>[7 + inputs.Length];   //平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差);和;绝对值和
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }
            try
            {
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }


                //1 参数处理

                string[] paras = calcuinfo.fparas.Split(';');



                string[] vibstr = paras[0].Split(',');
                double[] vib = new double[vibstr.Length];
                for (int i = 0; i < vibstr.Length - 1; i++)
                {
                    vib[i] = Convert.ToDouble(vibstr[i]);
                }

                string[] visstr = paras[1].Split(',');
                double[] vis = new double[visstr.Length];
                for (int i = 0; i < visstr.Length - 1; i++)
                {
                    vis[i] = Convert.ToDouble(visstr[i]);
                }

                List<XFClass> XF = new List<XFClass>();
                string[] XFStr = paras[2].Split(',');
                foreach (string item in XFStr)
                {
                    XFClass xfClass = new XFClass();
                    xfClass.A = Convert.ToInt32(item.Split(':')[0]);
                    List<double> bList = new List<double>();
                    foreach (string bItem in item.Split(':')[1].Split('*'))
                    {
                        bList.Add(Convert.ToDouble(bItem));
                    }
                    xfClass.B = bList;
                }

                double[] Y = new double[inputs[0].Count];
                double[][] X = new double[inputs[1].Count][];
                for (int i = 0; i < inputs[0].Count - 1; i++)
                {
                    Y[i] = inputs[0][i].Value;
                }
                for (int i = 1; i < inputs.Length - 1; i++)
                {
                    double[] xList = new double[inputs[i].Count];
                    for (int l = 0; l < inputs[i].Count - 1; l++)
                    {
                        xList[l] = inputs[i][l].Value;
                    }
                    X[i] = xList;
                }
                int k = 0;
                double p = 0;
                double[] mlist = new double[] { };
                double[] resultlist = new double[] { };
                string errmsg = string.Empty;
                MultipleRegressionAlgorithmCaculate mm = new MultipleRegressionAlgorithmCaculate();
                int s = mm.Regression(0, vib, vis, X, Y, XF, ref k, ref p, ref mlist, ref resultlist, ref errmsg);
                if (s != 0)
                {
                    _fatalFlag = true;
                    _errorInfo = errmsg;
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                PValue kValue = new PValue(k, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue pValue = new PValue(p, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                results[0] = new List<PValue>();
                results[0].Add(kValue);
                results[1] = new List<PValue>();
                results[1].Add(pValue);
                for (int i = 0; i < mlist.Length - 1; i++)
                {
                    PValue mValue = new PValue(mlist[i], calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[i + 2] = new List<PValue>();
                    results[i + 2].Add(mValue);
                }
                //分别对应R2; sey; F;df;SSR;SSE;
                for (int i = 0; i < resultlist.Length - 1; i++)
                {
                    PValue rValue = new PValue(resultlist[i], calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[i + 2 + mlist.Length] = new List<PValue>();
                    results[i + 2 + mlist.Length].Add(rValue);
                }
                //不够16位的0补齐
                if (results.Length < 16)
                {
                    int l = results.Length;
                    for (int i = 0; i < 16 - results.Length; i++)
                    {
                        PValue rValue = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[i + l] = new List<PValue>();
                        results[i + l].Add(rValue);
                    }
                }
                //计算结果初始化
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
