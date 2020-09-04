using MathNet.Numerics;
using Model;
using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;

namespace PSLCalcu.Module
{
    public class MPVBasePlus : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MPVBasePlus";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "基础数据计算";
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
        private string _algorithms = "MPVBasePlus";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "1;2;1;2;1;2;1;S";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "极差N1限值;极差N2限值;极差N3限值;求和乘积系数k;求和偏置系数b;稳定限幅;不稳定限幅;计算周期标志S/L.选择长周期L时,源数据类型必须选择rdbset.";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){6}(;){0,1}([SL]){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }


        private int _outputNumber = 67;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "PVBMin;" +
                                    "PVBMinTime;" +
                                    "PVBAvg;" +
                                    "PVBMax;" +
                                    "PVBMaxTime;" +
                                    "PVBDMax;" +
                                    "PVBSum;" +
                                    "PVBSumkb;" +
                                    "PVBLinek;" +
                                    "PVBLineb;" +
                                    "PVBSumPNR;" +
                                    "PVBAbsSum;" +
                                    "PVBStdev;" +
                                    "PVBVolatility;" +
                                    "PVBSDMax;" +
                                    "PVBSDMaxTime;" +
                                    "PVBSDMaxTimeEnd;" +
                                    "PVBSDMaxTimeG;" +
                                    "PVBSDMaxTimeGEnd;" +
                                    "PVBSDMaxTimeGvC;" +
                                    "PVBSDMaxTimeGsA;" +
                                    "PVBSDMaxTimeGsV;" +
                                    "PVBSDMaxTimeGeV;" +
                                    "PVBSDMaxTimeGvT;" +
                                    "PVBSDMaxR;" +
                                    "PVBDN1Num;" +
                                    "PVBDN2Num;" +
                                    "PVBDN3Num;" +
                                    "PVBTNum;" +
                                    "PVBVMax;" +
                                    "PVBVMin;" +
                                    "PVBVAvg;" +
                                    "PVBStbT;" +
                                    "PVBStbTEnd;" +
                                    "PVBStbTvC;" +
                                    "PVBStbTvA;" +
                                    "PVBStbTsV;" +
                                    "PVBStbTeV;" +
                                    "PVBStbTvT;" +
                                    "PVBStbTR;" +
                                    "PVBNoStbT;" +
                                    "PVBNoStbTEnd;" +
                                    "PVBNoStbTvC;" +
                                    "PVBNoStbTvA;" +
                                    "PVBNoStbTsV;" +
                                    "PVBNoStbTeV;" +
                                    "PVBNoStbTvT;" +
                                    "PVBNoStbTR;" +
                                    "PVBStbTSLT;" +
                                    "PVBStbTSLTEnd;" +
                                    "PVBStbTSLPV;" +
                                    "PVBStbTSL;" +
                                    "PVBNoStbTSLT;" +
                                    "PVBNoStbTSLTEnd;" +
                                    "PVBNoStbTSL;" +
                                    "PVBUpTSLT;" +
                                    "PVBUpTSLTEnd;" +
                                    "PVBUpTSL;" +
                                    "PVBDownTSLT;" +
                                    "PVBDownTSLTEnd;" +
                                    "PVBDownTSL;" +
                                    "PVBPNum;" +
                                    "PVBQltR;" +
                                    "PVBStatus;" +
                                    "PVBQa;" +
                                    "PVBQb;" +
                                    "PVBQc";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "最小值;" +
                                         "最小值发生时刻;" +
                                         "平均;" +
                                         "最大值;" +
                                         "最大值发生时刻;" +
                                         "总极差。PVBMax - PVBMin;" +
                                         "和;" +
                                         "上面的和乘 k 加 b;" +
                                         "以直线拟合时的斜率 Regress_k;" +
                                         "以直线拟合时的截距 Regress_b;" +
                                         "正负比。正数和 / 绝对值(负数和) * 100%;" +
                                         "绝对值和;" +
                                         "标准差，分母是样本点总数;" +
                                         "波动。|Δxi| = | xi+1 - xi | 求和，除以段数，即，点数 - 1;" +
                                         "单次极差最大值。Max(|Δxi|)。极差：指 x 所经历的全部峰 - 谷差的绝对值;" +
                                         "单次极差最大值发生、退出时刻;" +
                                         "单次极差最大值，退出时刻;" +
                                         "极差时间组，所有波峰 - 谷的时间组;" +
                                         "极差时间组，所有- 谷的时间组;" +
                                         "极差时间组，所有- valueCount;" +
                                         "极差时间组，所有- valueAmount;" +
                                         "极差时间组，所有- startValue;" +
                                         "极差时间组，所有- endValue;" +
                                         "极差时间组，所有- valueType;" +
                                         "单次极差比，绝对值。PVBSDMax / (PVBMax -  PVBMin) x 100%;" +
                                         "极差大于 N1 次数;" +
                                         "极差大于 N2 次数，包含 N1;" +
                                         "极差大于 N3 次数，包含 N2、N1;" +
                                         "翻转次数。标准：xi-1 < xi >xi+1，或 xi-1 > xi <xi+1;" +
                                         "最大速度，max(Δxi);" +
                                         "最小速度，min(Δxi);" +
                                         "平均速度，Avg|Δxi|;" +
                                         "稳定时刻。进稳定区时刻，出稳定区时刻;" +
                                         "稳定时刻。出稳定区时刻;" +
                                         "稳定时刻。所有进出稳定区- valueCount;" +
                                         "稳定时刻。所有进出稳定区- valueAmount;" +
                                         "稳定时刻。所有进稳定区- startValue;" +
                                         "稳定时刻。所有出稳定区- endValue;" +
                                         "稳定时刻。所有进出稳定区- valueType;" +
                                         "稳定时间占比，稳定：|Δxi| ≤ StbL;" +
                                         "不稳定时刻。进不稳定区时刻，出不稳定区时刻;" +
                                         "不稳定时刻。出不稳定区时刻;" +
                                         "不稳定时刻。所有进出不稳定区- valueCount;" +
                                         "不稳定时刻。所有进出不稳定区- valueAmount;" +
                                         "不稳定时刻。所有进不稳定区- startValue;" +
                                         "不稳定时刻。所有出不稳定区- endValue;" +
                                         "不稳定时刻。所有进出不稳定区- valueType;" +
                                         "不稳定时间占比，稳定：|Δxi| ＞ NoStbL;" +
                                         "最长连续稳定时刻。进稳定区时刻，出稳定区时刻;" +
                                         "最长连续稳定时刻。出稳定区时刻;" +
                                         "最长连续稳定时变量平均值;" +
                                         "最长连续稳定时间长;" +
                                         "最长连续不稳定时刻。进不稳定区时刻，出不稳定区时刻;" +
                                         "最长连续不稳定时刻。出不稳定区时刻;" +
                                         "最长连续不稳定时间长;" +
                                         "最长连续上升时刻。进上升区时刻，出上升区时刻;" +
                                         "最长连续上升时刻。出上升区时刻;" +
                                         "最长连续上升时间。上升：Δxi ＞ 0;" +
                                         "最长连续下降时刻。进下降区时刻，出下降区时刻;" +
                                         "最长连续下降时刻。出下降区时刻;" +
                                         "最长连续下降时间。下降：Δxi ＜ 0;" +
                                         "数据点数。读进来的数据点数;" +
                                         "质量率 %。好质量点 / 总点数 x 100%;" +
                                         "计算故障标记;" +
                                         "以二次函数拟合时的 a。x = a t2 + b t + c;" +
                                         "以二次函数拟合时的 b;" +
                                         "以二次函数拟合时的 c";

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
        #endregion  #region 计算模块算法
        /// <summary>
        /// 计算模块算法实现:求限定时间内最大最小和极差值
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

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[67];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。 
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = input.Count - 1; i >= 0; i--)
                {
                    if (input[i].Status != 0) input.RemoveAt(i);
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //读取参数

                double N1, N2, N3, k, b, stbl, nostbl;
                string mode;
                string[] paras = calcuinfo.fparas.Split(';');

                N1 = int.Parse(paras[0]);
                N2 = int.Parse(paras[1]);
                N3 = double.Parse(paras[2]);
                k = double.Parse(paras[3]);
                b = double.Parse(paras[4]);
                stbl = double.Parse(paras[5]);
                nostbl = double.Parse(paras[6]);
                mode = paras[7];
                if (paras.Length == 8)
                    mode = paras[7];   //如果设定了第4个参数，计算模式用第四个参数值。S表示短周期，L表示长周期                
                else
                    mode = "S";

                if (mode == "S")
                {
                    #region 短周期计算

                    //短周期算法
                    //将输入input(分钟数据） 转化成 List<MPVBaseMessageInBadClass>
                    List<MPVBaseMessageInBadClass> valueList = new List<MPVBaseMessageInBadClass>();
                    for (int j = 0; j < input.Count; j++)
                    {
                        MPVBaseMessageInBadClass v = new MPVBaseMessageInBadClass();
                        v.seq = j;
                        string time = input[j].Timestamp.ToShortTimeString();
                        string time2 = input[j].Timestamp.ToString();
                        v.valueDate = time2;
                        v.valueAmount = input[j].Value.ToString();
                        valueList.Add(v);
                    }
                    #endregion

                    #region 短周期算法输出
                    //
                    //调用短周期算法数据是把每分钟的数据结果进行处理 小时级别时间数据运算
                    PSLCalcu.Module.MPVBaseMessageOutBadClass res = MPVBaseModule.shortMPVBase(valueList, N1, N2, N3, k, b, stbl, nostbl);
                    //组织结算结果
                    if (null != res.PVBMin)
                    {
                        results[0] = new List<PValue>();
                        PValue PVBMin = new PValue(Double.Parse(res.PVBMin), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[0].Add(PVBMin);
                    }
                    if (null != res.PVBMinTime)
                    {
                        results[1] = new List<PValue>();
                        DateTime time = Convert.ToDateTime(res.PVBMinTime);
                        double value = time.Ticks;
                        PValue PVBMinTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[1].Add(PVBMinTime);
                    }
                    if (null != res.PVBAvg)
                    {
                        results[2] = new List<PValue>();
                        PValue PVBAvg = new PValue(Double.Parse(res.PVBAvg), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[2].Add(PVBAvg);
                    }
                    if (null != res.PVBMax)
                    {
                        results[3] = new List<PValue>();
                        PValue PVBMax = new PValue(Double.Parse(res.PVBMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[3].Add(PVBMax);
                    }
                    if (null != res.PVBMaxTime)
                    {
                        results[4] = new List<PValue>();
                        DateTime time = Convert.ToDateTime(res.PVBMaxTime);
                        double value = time.Ticks;
                        PValue PVBMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[4].Add(PVBMaxTime);
                    }
                    if (null != res.PVBDMax)
                    {
                        results[5] = new List<PValue>();
                        PValue PVBDMax = new PValue(Double.Parse(res.PVBDMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[5].Add(PVBDMax);
                    }
                    if (null != res.PVBSum)
                    {
                        results[6] = new List<PValue>();
                        PValue PVBSum = new PValue(Double.Parse(res.PVBSum), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[6].Add(PVBSum);
                    }
                    if (null != res.PVBSumkb)
                    {
                        results[7] = new List<PValue>();
                        PValue PVBSumkb = new PValue(Double.Parse(res.PVBSumkb), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[7].Add(PVBSumkb);
                    }
                    if (null != res.PVBLinek)
                    {
                        results[8] = new List<PValue>();
                        PValue PVBLinek = new PValue(Double.Parse(res.PVBLinek), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[8].Add(PVBLinek);
                    }
                    if (null != res.PVBLineb)
                    {
                        results[9] = new List<PValue>();
                        PValue PVBLineb = new PValue(Double.Parse(res.PVBLineb), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[9].Add(PVBLineb);
                    }

                    if (null != res.PVBSumPNR && res.PVBSumPNR.Contains("无穷大") == false)
                    {
                        results[10] = new List<PValue>();
                        PValue PVBSumPNR = new PValue(Double.Parse(res.PVBSumPNR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[10].Add(PVBSumPNR);
                    }
                    if (null != res.PVBAbsSum)
                    {
                        results[11] = new List<PValue>();
                        PValue PVBAbsSum = new PValue(Double.Parse(res.PVBAbsSum), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[11].Add(PVBAbsSum);
                    }
                    if (null != res.PVBStdev)
                    {
                        results[12] = new List<PValue>();
                        PValue PVBStdev = new PValue(Double.Parse(res.PVBStdev), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[12].Add(PVBStdev);
                    }
                    if (null != res.PVBVolatility)
                    {
                        results[13] = new List<PValue>();
                        PValue PVBVolatility = new PValue(Double.Parse(res.PVBVolatility), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[13].Add(PVBVolatility);
                    }
                    if (null != res.PVBSDMax)
                    {
                        results[14] = new List<PValue>();
                        PValue PVBSDMax = new PValue(Double.Parse(res.PVBSDMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[14].Add(PVBSDMax);
                    }
                    if (null != res.PVBSDMaxTime)
                    {
                        results[15] = new List<PValue>();
                        results[16] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.PVBSDMaxTime.startDate);
                        double startValue = startTime.Ticks;
                        PValue PVBSDMaxTime = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[15].Add(PVBSDMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.PVBSDMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue PVBSDMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[16].Add(PVBSDMaxTimeEnd);
                    }
                    if (null != res.PVBSDMaxTimeG)
                    {
                        results[17] = new List<PValue>();
                        results[18] = new List<PValue>();
                        results[19] = new List<PValue>();
                        results[20] = new List<PValue>();
                        results[21] = new List<PValue>();
                        results[22] = new List<PValue>();
                        results[23] = new List<PValue>();
                        int count = res.PVBSDMaxTimeG.Count;
                        for (int j = 0; j < count; j++)
                        {
                            DateTime startTime = Convert.ToDateTime(res.PVBSDMaxTimeG[j].startDate);
                            double startValue = startTime.Ticks;
                            PValue PVBSDMaxTimeG = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[17].Add(PVBSDMaxTimeG);
                            DateTime endTime = Convert.ToDateTime(res.PVBSDMaxTimeG[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue PVBSDMaxTimeGEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[18].Add(PVBSDMaxTimeGEnd);
                            PValue PVBSDMaxTimeGvC = new PValue(res.PVBSDMaxTimeG[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[19].Add(PVBSDMaxTimeGvC);
                            PValue PVBSDMaxTimeGsA = new PValue(res.PVBSDMaxTimeG[j].valueAmount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[20].Add(PVBSDMaxTimeGsA);
                            PValue PVBSDMaxTimeGsV = new PValue(res.PVBSDMaxTimeG[j].startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[21].Add(PVBSDMaxTimeGsV);
                            PValue PVBSDMaxTimeGeV = new PValue(res.PVBSDMaxTimeG[j].endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[22].Add(PVBSDMaxTimeGeV);
                            if (res.PVBSDMaxTimeG[j].valueType.Contains("up"))
                            {
                                String s = "117";//ASCII码十进制值:u 
                                Double value = Double.Parse(s);
                                PValue PVBSDMaxTimeGvT = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[23].Add(PVBSDMaxTimeGvT);
                            }
                            if (res.PVBSDMaxTimeG[j].valueType.Contains("down"))
                            {
                                String s = "100";//ASCII码十进制值:d
                                Double value = Double.Parse(s);
                                PValue PVBSDMaxTimeGvT = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[23].Add(PVBSDMaxTimeGvT);
                            }
                            if (res.PVBSDMaxTimeG[j].valueType.Contains("translation"))
                            {
                                String s = "116";//ASCII码十进制值:t 
                                Double value = Double.Parse(s);
                                PValue PVBSDMaxTimeGvT = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[23].Add(PVBSDMaxTimeGvT);
                            }
                        }
                    }
                    if (null != res.PVBSDMaxR)
                    {
                        results[24] = new List<PValue>();
                        PValue PVBSDMaxR = new PValue(Double.Parse(res.PVBSDMaxR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[24].Add(PVBSDMaxR);
                    }
                    if (null != res.PVBDN1Num)
                    {
                        results[25] = new List<PValue>();
                        PValue PVBDN1Num = new PValue(Double.Parse(res.PVBDN1Num), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[25].Add(PVBDN1Num);
                    }
                    if (null != res.PVBDN2Num)
                    {
                        results[26] = new List<PValue>();
                        PValue PVBDN2Num = new PValue(Double.Parse(res.PVBDN2Num), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[26].Add(PVBDN2Num);
                    }
                    if (null != res.PVBDN3Num)
                    {
                        results[27] = new List<PValue>();
                        PValue PVBDN3Num = new PValue(Double.Parse(res.PVBDN3Num), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[27].Add(PVBDN3Num);
                    }
                    if (null != res.PVBTNum)
                    {
                        results[28] = new List<PValue>();
                        PValue PVBTNum = new PValue(Double.Parse(res.PVBTNum), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[28].Add(PVBTNum);
                    }
                    if (null != res.PVBVMax)
                    {
                        results[29] = new List<PValue>();
                        PValue PVBVMax = new PValue(Double.Parse(res.PVBVMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[29].Add(PVBVMax);
                    }
                    if (null != res.PVBVMin)
                    {
                        results[30] = new List<PValue>();
                        PValue PVBVMin = new PValue(Double.Parse(res.PVBVMin), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[30].Add(PVBVMin);
                    }
                    if (null != res.PVBVAvg)
                    {
                        results[31] = new List<PValue>();
                        PValue PVBVAvg = new PValue(Double.Parse(res.PVBVAvg), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[31].Add(PVBVAvg);
                    }
                    if (null != res.PVBStbT && res.PVBStbT.Count > 0)
                    {
                        results[32] = new List<PValue>();
                        results[33] = new List<PValue>();
                        results[34] = new List<PValue>();
                        results[35] = new List<PValue>();
                        results[36] = new List<PValue>();
                        results[37] = new List<PValue>();
                        results[38] = new List<PValue>();
                        int count = res.PVBStbT.Count;
                        for (int j = 0; j < count; j++)
                        {
                            DateTime startTime = Convert.ToDateTime(res.PVBStbT[j].startDate);
                            double startValue = startTime.Ticks;
                            PValue PVBStbT = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[32].Add(PVBStbT);
                            DateTime endTime = Convert.ToDateTime(res.PVBStbT[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue PVBStbTEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[33].Add(PVBStbTEnd);
                            PValue PVBStbTvC = new PValue(res.PVBStbT[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[34].Add(PVBStbTvC);
                            PValue PVBStbTvA = new PValue(res.PVBStbT[j].valueAmount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[35].Add(PVBStbTvA);
                            PValue PVBStbTsV = new PValue(res.PVBStbT[j].startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[36].Add(PVBStbTsV);
                            PValue PVBStbTeV = new PValue(res.PVBStbT[j].endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[37].Add(PVBStbTeV);
                            if (null != res.PVBStbT[j].valueType)
                            {
                                if (res.PVBStbT[j].valueType.Contains("Stb"))
                                {
                                    String s = "83";//ASCII码十进制值:83;控制字符:S 
                                    Double value = Double.Parse(s);
                                    PValue PVBStbTvT = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                    results[38].Add(PVBStbTvT);
                                }
                            }
                        }
                    }
                    if (null != res.PVBStbTR)
                    {
                        results[39] = new List<PValue>();
                        PValue PVBStbTR = new PValue(Double.Parse(res.PVBStbTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[39].Add(PVBStbTR);
                    }
                    if (null != res.PVBNoStbT && res.PVBNoStbT.Count > 0)
                    {
                        results[40] = new List<PValue>();
                        results[41] = new List<PValue>();
                        results[42] = new List<PValue>();
                        results[43] = new List<PValue>();
                        results[44] = new List<PValue>();
                        results[45] = new List<PValue>();
                        results[46] = new List<PValue>();
                        int count = res.PVBNoStbT.Count;
                        for (int j = 0; j < count; j++)
                        {
                            DateTime startTime = Convert.ToDateTime(res.PVBNoStbT[j].startDate);
                            double startValue = startTime.Ticks;
                            PValue PVBNoStbT = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[40].Add(PVBNoStbT);
                            DateTime endTime = Convert.ToDateTime(res.PVBNoStbT[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue PVBNoStbTEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[41].Add(PVBNoStbTEnd);
                            PValue PVBNoStbTvC = new PValue(res.PVBNoStbT[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[42].Add(PVBNoStbTvC);
                            PValue PVBNoStbTvA = new PValue(res.PVBNoStbT[j].valueAmount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[43].Add(PVBNoStbTvA);
                            PValue PVBNoStbTsV = new PValue(res.PVBNoStbT[j].startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[44].Add(PVBNoStbTsV);
                            PValue PVBNoStbTeV = new PValue(res.PVBNoStbT[j].endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[45].Add(PVBNoStbTeV);
                            if (res.PVBNoStbT[j].valueType.Contains("noStb"))
                            {
                                String s = "110";//ASCII码十进制值:100;控制字符:n 
                                Double value = Double.Parse(s);
                                PValue PVBNoStbTvT = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[46].Add(PVBNoStbTvT);
                            }
                        }
                    }
                    if (null != res.PVBNoStbTR)
                    {
                        results[47] = new List<PValue>();
                        PValue PVBNoStbTR = new PValue(Double.Parse(res.PVBNoStbTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[47].Add(PVBNoStbTR);
                    }
                    if (null != res.PVBStbTSLT)
                    {
                        results[48] = new List<PValue>();
                        results[49] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.PVBStbTSLT.startDate);
                        double startValue = startTime.Ticks;
                        PValue PVBStbTSLT = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[48].Add(PVBStbTSLT);
                        DateTime endTime = Convert.ToDateTime(res.PVBStbTSLT.endDate);  //"2020-02-01 00:00:00"
                        double endValue = endTime.Ticks;
                        PValue PVBStbTSLTEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[49].Add(PVBStbTSLTEnd);
                    }
                    if (null != res.PVBStbTSLPV)
                    {
                        results[50] = new List<PValue>();
                        PValue PVBStbTSLPV = new PValue(Double.Parse(res.PVBStbTSLPV), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[50].Add(PVBStbTSLPV);
                    }
                    if (null != res.PVBStbTSL)
                    {
                        results[51] = new List<PValue>();
                        PValue PVBStbTSL = new PValue(Double.Parse(res.PVBStbTSL), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[51].Add(PVBStbTSL);
                    }
                    if (null != res.PVBNoStbTSLT)
                    {
                        results[52] = new List<PValue>();
                        results[53] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.PVBNoStbTSLT.startDate);
                        double startValue = startTime.Ticks;
                        PValue PVBNoStbTSLT = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[52].Add(PVBNoStbTSLT);
                        DateTime endTime = Convert.ToDateTime(res.PVBNoStbTSLT.endDate);
                        double endValue = endTime.Ticks;
                        PValue PVBNoStbTSLTEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[53].Add(PVBNoStbTSLTEnd);
                    }
                    if (null != res.PVBNoStbTSL)
                    {
                        results[54] = new List<PValue>();
                        PValue PVBNoStbTSL = new PValue(Double.Parse(res.PVBNoStbTSL), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[54].Add(PVBNoStbTSL);
                    }
                    if (null != res.PVBUpTSLT.startDate)
                    {
                        results[55] = new List<PValue>();
                        results[56] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.PVBUpTSLT.startDate);
                        double startValue = startTime.Ticks;
                        PValue PVBUpTSLT = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[55].Add(PVBUpTSLT);
                        DateTime endTime = Convert.ToDateTime(res.PVBUpTSLT.endDate);
                        double endValue = endTime.Ticks;
                        PValue PVBUpTSLTEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[56].Add(PVBUpTSLTEnd);
                    }
                    if (null != res.PVBUpTSL)
                    {
                        results[57] = new List<PValue>();
                        PValue PVBUpTSL = new PValue(Double.Parse(res.PVBUpTSL), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[57].Add(PVBUpTSL);
                    }
                    if (null != res.PVBDownTSLT)
                    {
                        results[58] = new List<PValue>();
                        results[59] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.PVBDownTSLT.startDate);
                        double startValue = startTime.Ticks;
                        PValue PVBDownTSLT = new PValue(startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[58].Add(PVBDownTSLT);
                        DateTime endTime = Convert.ToDateTime(res.PVBDownTSLT.endDate);
                        double endValue = endTime.Ticks;
                        PValue PVBDownTSLTEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[59].Add(PVBDownTSLTEnd);
                    }
                    if (null != res.PVBDownTSL)
                    {
                        results[60] = new List<PValue>();
                        PValue PVBDownTSL = new PValue(Double.Parse(res.PVBDownTSL), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[60].Add(PVBDownTSL);
                    }
                    if (null != res.PVBPNum)
                    {
                        results[61] = new List<PValue>();
                        PValue PVBPNum = new PValue(Double.Parse(res.PVBPNum), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[61].Add(PVBPNum);
                    }
                    if (null != res.PVBQltR)
                    {
                        results[62] = new List<PValue>();
                        PValue PVBQltR = new PValue(Double.Parse(res.PVBQltR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[62].Add(PVBQltR);
                    }
                    if (null != res.PVBStatus)
                    {
                        results[63] = new List<PValue>();
                        PValue PVBStatus = new PValue(Double.Parse(res.PVBStatus), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[63].Add(PVBStatus);
                    }
                    if (null != res.PVBQa)
                    {
                        results[64] = new List<PValue>();
                        PValue PVBQa = new PValue(Double.Parse(res.PVBQa), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[64].Add(PVBQa);
                    }
                    if (null != res.PVBQb)
                    {
                        results[65] = new List<PValue>();
                        PValue PVBQb = new PValue(Double.Parse(res.PVBQb), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[65].Add(PVBQb);
                    }
                    if (null != res.PVBQc)
                    {
                        results[66] = new List<PValue>();
                        PValue PVBQc = new PValue(Double.Parse(res.PVBQc), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[66].Add(PVBQc);
                    }

                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                #endregion
                else
                {
                    #region 长周期计算
                    //长周期算法
                    //长周期算法数据是在短周期算法得出的数据结果上进行再次处理 天、月、年 等时间数据运算
                    List<MPVBaseMessageOutClass> longlist = new List<MPVBaseMessageOutClass>();
                    for (int j = 0; j < input.Count; j++)
                    {
                        MPVBaseMessageOutClass v = new MPVBaseMessageOutClass();
                        v.id = j;
                        if (null != inputs[0][j].Value) { v.PVBMin = inputs[0][j].Value; }

                        if (null != inputs[1][j].Value)
                        {
                            long PVBMinD = (long)inputs[1][j].Value;
                            DateTime date = new DateTime(PVBMinD);
                            v.PVBMinTime = date.ToString();
                        }

                        if (null != inputs[2][j].Value) { v.PVBAvg = inputs[2][j].Value; }

                        if (null != inputs[3][j].Value) { v.PVBMax = inputs[3][j].Value; }

                        if (null != inputs[4][j].Value)
                        {
                            long PVBMaxd = (long)inputs[4][j].Value;
                            DateTime PVBMaxdate = new DateTime(PVBMaxd);
                            v.PVBMaxTime = PVBMaxdate.ToString();
                        }

                        if (null != inputs[5][j].Value) { v.PVBDMax = inputs[5][j].Value; }

                        if (null != inputs[6][j].Value) { v.PVBSum = inputs[6][j].Value; }

                        if (null != inputs[7][j].Value) { v.PVBSumkb = inputs[7][j].Value; }

                        if (null != inputs[8][j].Value) { v.PVBLinek = inputs[8][j].Value; }

                        if (null != inputs[9][j].Value) { v.PVBLineb = inputs[9][j].Value; }

                        if (null != inputs[10][j].Value) { v.PVBSumPNR = inputs[10][j].Value; }

                        if (null != inputs[11][j].Value) { v.PVBAbsSum = inputs[11][j].Value; }

                        if (null != inputs[12][j].Value) { v.PVBStdev = inputs[12][j].Value; }

                        if (null != inputs[13][j].Value) { v.PVBVolatility = inputs[13][j].Value; }

                        if (null != inputs[14][j].Value) { v.PVBSDMax = inputs[14][j].Value; }

                        //  PVBSDMaxTime 单次极差最大值发生时刻、退出时刻
                        //List<D22STimeClass> PVBSDMaxTime=new List<D22STimeClass>();
                        if (null != inputs[15][j].Value)
                        {
                            D22STimeClass PVBSDMaxTime = new D22STimeClass();
                            v.PVBSDMaxTime = PVBSDMaxTime;

                            long PSDMaxdates = (long)inputs[15][j].Value;
                            DateTime PVBSDMaxdate = new DateTime(PSDMaxdates);
                            PVBSDMaxTime.startDate = PVBSDMaxdate.ToString();
                            long PSDMaxdateE = (long)inputs[16][j].Value;
                            DateTime PVBSDMaxdateE = new DateTime(PSDMaxdateE);
                            PVBSDMaxTime.endDate = PVBSDMaxdateE.ToString();
                        }

                        //  PVBSDMaxTimeG	时间	极差时间组，所有波峰 - 谷的时间组
                        if (null != inputs[17][j].Value)
                        {
                            List<D22STimeClass> PVBSDMaxTimeG = new List<D22STimeClass>();
                            v.PVBSDMaxTimeG = PVBSDMaxTimeG;

                            int MaxTimeGStartCount = inputs[17].Count;
                            for (int TimeStart = 0; TimeStart < MaxTimeGStartCount; TimeStart++)
                            {
                                if (inputs[17][TimeStart].Timestamp == input[j].Timestamp && inputs[17][TimeStart].Endtime == input[j].Endtime && inputs[17][TimeStart].Status == 0)
                                {
                                    D22STimeClass PVBSDMaxTimeGs = new D22STimeClass();
                                    long PSDMaxdateGs = (long)inputs[17][TimeStart].Value;
                                    DateTime MaxTimeGs = new DateTime(PSDMaxdateGs);
                                    PVBSDMaxTimeGs.startDate = MaxTimeGs.ToString();

                                    long PSDMaxdateGE = (long)inputs[18][TimeStart].Value;
                                    DateTime PVBSDMaxTimeGEnd = new DateTime(PSDMaxdateGE);
                                    PVBSDMaxTimeGs.endDate = PVBSDMaxTimeGEnd.ToString();

                                    PVBSDMaxTimeGs.valueCount = inputs[19][TimeStart].Value;

                                    PVBSDMaxTimeGs.valueAmount = inputs[20][TimeStart].Value;

                                    PVBSDMaxTimeGs.startValue = inputs[21][TimeStart].Value;

                                    PVBSDMaxTimeGs.endValue = inputs[22][TimeStart].Value;

                                    if (inputs[23][TimeStart].Value.ToString() == "117")
                                    {
                                        PVBSDMaxTimeGs.valueType = "up";
                                    } //ASCII码十进制值:117;控制字符:u 
                                    if (inputs[23][TimeStart].Value.ToString() == "100")
                                    {
                                        PVBSDMaxTimeGs.valueType = "down";
                                    }//ASCII码十进制值:100;控制字符:d 
                                    if (inputs[23][TimeStart].Value.ToString() == "116")
                                    {
                                        PVBSDMaxTimeGs.valueType = "translation";
                                    }//ASCII码十进制值:116;控制字符:t 

                                    PVBSDMaxTimeG.Add(PVBSDMaxTimeGs);
                                }

                            }
                        }
                        if (null != inputs[24][j].Value) { v.PVBSDMaxR = inputs[24][j].Value; }

                        if (null != inputs[25][j].Value) { v.PVBDN1Num = Convert.ToInt32(inputs[25][j].Value); }

                        if (null != inputs[26][j].Value) { v.PVBDN2Num = Convert.ToInt32(inputs[26][j].Value); }

                        if (null != inputs[27][j].Value) { v.PVBDN3Num = Convert.ToInt32(inputs[27][j].Value); }

                        if (null != inputs[28][j].Value) { v.PVBTNum = Convert.ToInt32(inputs[28][j].Value); }

                        if (null != inputs[29][j].Value) { v.PVBVMax = inputs[29][j].Value; }

                        if (null != inputs[30][j].Value) { v.PVBVMin = inputs[30][j].Value; }

                        if (null != inputs[31][j].Value) { v.PVBVAvg = inputs[31][j].Value; }

                        //  PVBStbT	时间:稳定时刻。。进稳定区时刻，出稳定区时刻
                        if (null != inputs[32][j].Value)
                        {
                            List<D22STimeClass> PVBStbT = new List<D22STimeClass>();
                            v.PVBStbT = PVBStbT;
                            int PVBStbTStartCount = inputs[32].Count;
                            for (int TimeStart = 0; TimeStart < PVBStbTStartCount; TimeStart++)
                            {
                                if (inputs[32][TimeStart].Timestamp == input[j].Timestamp && inputs[32][TimeStart].Endtime == input[j].Endtime && inputs[32][TimeStart].Status == 0)
                                {

                                    D22STimeClass PVBStbTs = new D22STimeClass();

                                    long PVBStbTdates = (long)inputs[32][TimeStart].Value;
                                    DateTime PVBStbTdate = new DateTime(PVBStbTdates);
                                    PVBStbTs.startDate = PVBStbTdate.ToString();

                                    long PVBStbTdateE = (long)inputs[33][TimeStart].Value;
                                    DateTime PVBStbTEnd = new DateTime(PVBStbTdateE);
                                    PVBStbTs.endDate = PVBStbTEnd.ToString();

                                    PVBStbTs.valueCount = inputs[34][TimeStart].Value;

                                    PVBStbTs.valueAmount = inputs[35][TimeStart].Value;

                                    PVBStbTs.startValue = inputs[36][TimeStart].Value;

                                    PVBStbTs.endValue = inputs[37][TimeStart].Value;
                                    //ASCII码十进制值:100;控制字符:d 
                                    if (null != inputs[38][j].Value && inputs[38][TimeStart].Value.ToString() == "83") { PVBStbTs.valueType = "Stb"; }
                                    else
                                    {
                                        PVBStbTs.valueType = "Err";
                                    }

                                    PVBStbT.Add(PVBStbTs);
                                }
                            }
                        }

                        //PVBStbTR	%:稳定时间占比，稳定：|Δxi| ≤ StbL
                        if (null != inputs[39][j].Value) { v.PVBStbTR = inputs[39][j].Value; }

                        //  PVBNoStbT	时间:不稳定时刻。进不稳定区时刻，出不稳定区时刻
                        if (null != inputs[40][j].Value)
                        {
                            List<D22STimeClass> PVBNoStbT = new List<D22STimeClass>();
                            v.PVBNoStbT = PVBNoStbT;
                            int PVBNoStbTStartCount = inputs[40].Count;
                            for (int TimeStart = 0; TimeStart < PVBNoStbTStartCount; TimeStart++)
                            {
                                if (inputs[40][TimeStart].Timestamp == input[j].Timestamp && inputs[40][TimeStart].Endtime == input[j].Endtime && inputs[40][TimeStart].Status == 0)
                                {
                                    D22STimeClass PVBNoStbTS = new D22STimeClass();
                                    long PVBNoStbTdates = (long)inputs[40][TimeStart].Value;
                                    DateTime PVBNoStbTdate = new DateTime(PVBNoStbTdates);
                                    PVBNoStbTS.startDate = PVBNoStbTdate.ToString();

                                    long PVBNoStbTdateE = (long)inputs[41][TimeStart].Value;
                                    DateTime PVBNoStbTEnd = new DateTime(PVBNoStbTdateE);
                                    PVBNoStbTS.endDate = PVBNoStbTEnd.ToString();

                                    PVBNoStbTS.valueCount = inputs[42][TimeStart].Value;

                                    PVBNoStbTS.valueAmount = inputs[43][TimeStart].Value;

                                    PVBNoStbTS.startValue = inputs[44][TimeStart].Value;

                                    PVBNoStbTS.endValue = inputs[45][TimeStart].Value;
                                    //ASII码十进制值:110;控制字符:n
                                    if (inputs[46][TimeStart].Value.ToString() == "110") { PVBNoStbTS.valueType = "noStb"; }

                                    PVBNoStbT.Add(PVBNoStbTS);
                                }
                            }
                        }

                        //  PVBNoStbTR  %:不稳定时间占比，稳定：|Δxi| ＞ NoStbL
                        if (null != inputs[47][j].Value) { v.PVBNoStbTR = inputs[47][j].Value; }

                        //  PVBStbTSLT 时间:最长连续稳定时刻。进稳定区时刻，出稳定区时刻
                        if (null != inputs[48][j].Value)
                        {
                            D22STimeClass PVBStbTSLT = new D22STimeClass();
                            v.PVBStbTSLT = PVBStbTSLT;
                            long PVBStbTSLTdates = (long)inputs[48][j].Value;
                            DateTime PVBStbTSLTdate = new DateTime(PVBStbTSLTdates);
                            PVBStbTSLT.startDate = PVBStbTSLTdate.ToString();

                            long PVBStbTSLTdateE = (long)inputs[49][j].Value;
                            DateTime PVBStbTSLTEnd = new DateTime(PVBStbTSLTdateE);
                            PVBStbTSLT.endDate = PVBStbTSLTEnd.ToString();
                        }

                        //PVBStbTSLPV 原单位:最长连续稳定时变量平均值
                        if (null != inputs[50][j].Value) { v.PVBStbTSLPV = inputs[50][j].Value; }

                        //  PVBStbTSL	时间:最长连续稳定时间长
                        if (null != inputs[51][j].Value) { v.PVBStbTSL = inputs[51][j].Value; }

                        //  PVBNoStbTSLT	时间:最长连续不稳定时刻。进不稳定区时刻，出不稳定区时刻
                        if (null != inputs[52][j].Value)
                        {
                            D22STimeClass PVBNoStbTSLT = new D22STimeClass();
                            long PVBNoStbTSLTdates = (long)inputs[52][j].Value;
                            DateTime PVBNoStbTSLTdate = new DateTime(PVBNoStbTSLTdates);
                            PVBNoStbTSLT.startDate = PVBNoStbTSLTdate.ToString();

                            long PVBNoStbTSLTdateE = (long)inputs[53][j].Value;
                            DateTime PVBNoStbTSLTEnd = new DateTime(PVBNoStbTSLTdateE);
                            PVBNoStbTSLT.endDate = PVBNoStbTSLTEnd.ToString();
                            v.PVBNoStbTSLT = PVBNoStbTSLT;
                        }

                        //  PVBNoStbTSL	时间:最长连续不稳定时间长
                        if (null != inputs[54][j].Value) { v.PVBNoStbTSL = inputs[54][j].Value; }

                        //  PVBUpTSLT	时间:最长连续上升时刻。进上升区时刻，出上升区时刻
                        if (null != inputs[55][j].Value)
                        {
                            D22STimeClass PVBUpTSLT = new D22STimeClass();
                            v.PVBUpTSLT = PVBUpTSLT;
                            long PVBUpTSLTdates = (long)inputs[55][j].Value;
                            DateTime PVBUpTSLTdate = new DateTime(PVBUpTSLTdates);
                            PVBUpTSLT.startDate = PVBUpTSLTdate.ToString();

                            long PVBUpTSLTdateE = (long)inputs[56][j].Value;
                            DateTime PVBUpTSLTEnd = new DateTime(PVBUpTSLTdateE);
                            PVBUpTSLT.endDate = PVBUpTSLTEnd.ToString();
                        }

                        //  PVBUpTSL	时间:最长连续上升时间。上升：Δxi ＞ 0
                        if (null != inputs[57][j].Value) { v.PVBUpTSL = inputs[57][j].Value; }

                        //  PVBDownTSLT	时间:最长连续下降时刻。进下降区时刻，出下降区时刻
                        if (null != inputs[58][j].Value)
                        {
                            D22STimeClass PVBDownTSLT = new D22STimeClass();
                            v.PVBDownTSLT = PVBDownTSLT;
                            long PVBDownTSLTdates = (long)inputs[58][j].Value;
                            DateTime PVBDownTSLTdate = new DateTime(PVBDownTSLTdates);
                            PVBDownTSLT.startDate = PVBDownTSLTdate.ToString();

                            long PVBDownTSLTdateE = (long)inputs[59][j].Value;
                            DateTime PVBDownTSLTEnd = new DateTime(PVBDownTSLTdateE);
                            PVBDownTSLT.endDate = PVBDownTSLTEnd.ToString();
                        }

                        //  PVBDownTSL	时间:最长连续下降时间。下降：Δxi ＜ 0
                        if (null != inputs[60][j].Value) { v.PVBDownTSL = inputs[60][j].Value; }

                        //  PVBPNum	数据点数。读进来的数据点数
                        if (null != inputs[61][j].Value) { v.PVBPNum = inputs[61][j].Value; }

                        //  PVBQltR	质量率 %。好质量点 / 总点数 x 100%
                        if (null != inputs[62][j].Value) { v.PVBQltR = inputs[62][j].Value; }

                        //  PVBStatus	NA	计算故障标记。不用单独标签，系统里有对上面每个标签都有状态标记
                        if (null != inputs[63][j].Value) { v.PVBStatus = inputs[63][j].Value.ToString(); }

                        //  PVBQa	NA	以二次函数拟合时的 a。x = a t2 + b t + c
                        if (null != inputs[64][j].Value) { v.PVBQa = inputs[64][j].Value; }

                        //  PVBQb	NA	以二次函数拟合时的 b
                        if (null != inputs[65][j].Value) { v.PVBQb = inputs[65][j].Value; }

                        //  PVBQc	NA	以二次函数拟合时的 c
                        if (null != inputs[66][j].Value) { v.PVBQc = inputs[66][j].Value; }

                        longlist.Add(v);
                    }
                    #endregion

                    #region 长周期算法输出
                    //调用长周期算法数据是把小时数据结果进行处理 天、月级别时间数据运算
                    MPVBaseMessageOutClass res = MPVBaseModule.longMPVBase(longlist, N1, N2, N3, k, b, stbl, nostbl);
                    //组织结算结果
                    if (null != res.PVBMin)
                    {
                        results[0] = new List<PValue>();
                        PValue PVBMin = new PValue(res.PVBMin, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[0].Add(PVBMin);
                    }
                    if (null != res.PVBMinTime)
                    {
                        results[1] = new List<PValue>();
                        DateTime time = Convert.ToDateTime(res.PVBMinTime);
                        double value = time.Ticks;
                        PValue PVBMinTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[1].Add(PVBMinTime);
                    }

                    if (null != res.PVBAvg)
                    {
                        results[2] = new List<PValue>();
                        PValue PVBAvg = new PValue(res.PVBAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[2].Add(PVBAvg);
                    }

                    if (null != res.PVBMax)
                    {
                        results[3] = new List<PValue>();
                        PValue PVBMax = new PValue(res.PVBMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[3].Add(PVBMax);
                    }

                    if (null != res.PVBMaxTime)
                    {
                        results[4] = new List<PValue>();
                        DateTime maxTime = Convert.ToDateTime(res.PVBMaxTime);
                        double maxValue = maxTime.Ticks;
                        PValue PVBMaxTime = new PValue(maxValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[4].Add(PVBMaxTime);
                    }

                    if (null != res.PVBDMax)
                    {
                        results[5] = new List<PValue>();
                        PValue PVBDMax = new PValue(res.PVBDMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[5].Add(PVBDMax);
                    }

                    if (null != res.PVBSum)
                    {
                        results[6] = new List<PValue>();
                        PValue PVBSum = new PValue(res.PVBSum, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[6].Add(PVBSum);
                    }

                    if (null != res.PVBSumkb)
                    {
                        results[7] = new List<PValue>();
                        PValue PVBSumkb = new PValue(res.PVBSumkb, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[7].Add(PVBSumkb);
                    }

                    if (null != res.PVBLinek)
                    {
                        results[8] = new List<PValue>();
                        PValue PVBLinek = new PValue(res.PVBLinek, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[8].Add(PVBLinek);
                    }

                    if (null != res.PVBLineb)
                    {
                        results[9] = new List<PValue>();
                        PValue PVBLineb = new PValue(res.PVBLineb, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[9].Add(PVBLineb);
                    }

                    if (null != res.PVBSumPNR)
                    {
                        results[10] = new List<PValue>();
                        PValue PVBSumPNR = new PValue(res.PVBSumPNR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[10].Add(PVBSumPNR);
                    }

                    if (null != res.PVBAbsSum)
                    {
                        results[11] = new List<PValue>();
                        PValue PVBAbsSum = new PValue(res.PVBAbsSum, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[11].Add(PVBAbsSum);
                    }

                    if (null != res.PVBStdev)
                    {
                        results[12] = new List<PValue>();
                        PValue PVBStdev = new PValue(res.PVBStdev, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[12].Add(PVBStdev);
                    }

                    if (null != res.PVBVolatility)
                    {
                        results[13] = new List<PValue>();
                        PValue PVBVolatility = new PValue(res.PVBVolatility, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[13].Add(PVBVolatility);
                    }

                    if (null != res.PVBSDMax)
                    {
                        results[14] = new List<PValue>();
                        PValue PVBSDMax = new PValue(res.PVBSDMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[14].Add(PVBSDMax);
                    }

                    if (null != res.PVBSDMaxTime)
                    {
                        results[15] = new List<PValue>();
                        results[16] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.PVBSDMaxTime.startDate);
                        //double MaxTimeSValue = startTime.Ticks;
                        PValue PPVBSDMaxTime = new PValue(startTime.Ticks, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[15].Add(PPVBSDMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.PVBSDMaxTime.endDate);
                        double MaxTimeEValue = endTime.Ticks;
                        PValue PVBSDMaxTimeEnd = new PValue(MaxTimeEValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[16].Add(PVBSDMaxTimeEnd);
                    }

                    if (null != res.PVBSDMaxTimeG && res.PVBSDMaxTimeG.Count > 0)
                    {
                        results[17] = new List<PValue>();
                        results[18] = new List<PValue>();
                        results[19] = new List<PValue>();
                        results[20] = new List<PValue>();
                        results[21] = new List<PValue>();
                        results[22] = new List<PValue>();
                        results[23] = new List<PValue>();
                        int count = res.PVBSDMaxTimeG.Count;
                        for (int j = 0; j < count; j++)
                        {
                            DateTime MaxstartTime = Convert.ToDateTime(res.PVBSDMaxTimeG[j].startDate);
                            double MaxstartValue = MaxstartTime.Ticks;
                            PValue PVBSDMaxTimeG = new PValue(MaxstartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[17].Add(PVBSDMaxTimeG);
                            DateTime MaxendTime = Convert.ToDateTime(res.PVBSDMaxTimeG[j].endDate);
                            double MaxendValue = MaxendTime.Ticks;
                            PValue PVBSDMaxTimeGEnd = new PValue(MaxendValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[18].Add(PVBSDMaxTimeGEnd);
                            PValue PVBSDMaxTimeGvC = new PValue(res.PVBSDMaxTimeG[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[19].Add(PVBSDMaxTimeGvC);
                            PValue PVBSDMaxTimeGsA = new PValue(res.PVBSDMaxTimeG[j].valueAmount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[20].Add(PVBSDMaxTimeGsA);
                            PValue PVBSDMaxTimeGsV = new PValue(res.PVBSDMaxTimeG[j].startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[21].Add(PVBSDMaxTimeGsV);
                            PValue PVBSDMaxTimeGeV = new PValue(res.PVBSDMaxTimeG[j].endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[22].Add(PVBSDMaxTimeGeV);
                            if (null != res.PVBSDMaxTimeG[j].valueType && res.PVBSDMaxTimeG[j].valueType.Contains("up"))
                            {
                                String s = "117";//ASCII码十进制值:117;控制字符:u 
                                Double typeValue = Double.Parse(s);
                                PValue PVBSDMaxTimeGvT = new PValue(typeValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[23].Add(PVBSDMaxTimeGvT);
                            }
                            if (null != res.PVBSDMaxTimeG[j].valueType && res.PVBSDMaxTimeG[j].valueType.Contains("down"))
                            {
                                String s = "100";//ASCII码十进制值:100;控制字符:d
                                Double typeValue = Double.Parse(s);
                                PValue PVBSDMaxTimeGvT = new PValue(typeValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[23].Add(PVBSDMaxTimeGvT);
                            }
                            if (null != res.PVBSDMaxTimeG[j].valueType && res.PVBSDMaxTimeG[j].valueType.Contains("translation"))
                            {
                                String s = "116";//ASCII码十进制值:116;控制字符:t
                                Double typeValue = Double.Parse(s);
                                PValue PVBSDMaxTimeGvT = new PValue(typeValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[23].Add(PVBSDMaxTimeGvT);
                            }
                        }
                    }

                    if (null != res.PVBSDMaxR)
                    {
                        results[24] = new List<PValue>();
                        PValue PVBSDMaxR = new PValue(res.PVBSDMaxR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[24].Add(PVBSDMaxR);
                    }

                    if (null != res.PVBDN1Num)
                    {
                        results[25] = new List<PValue>();
                        PValue PVBDN1Num = new PValue(res.PVBDN1Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[25].Add(PVBDN1Num);
                    }

                    if (null != res.PVBDN2Num)
                    {
                        results[26] = new List<PValue>();
                        PValue PVBDN2Num = new PValue(res.PVBDN2Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[26].Add(PVBDN2Num);
                    }

                    if (null != res.PVBDN3Num)
                    {
                        results[27] = new List<PValue>();
                        PValue PVBDN3Num = new PValue(res.PVBDN3Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[27].Add(PVBDN3Num);
                    }

                    if (null != res.PVBTNum)
                    {
                        results[28] = new List<PValue>();
                        PValue PVBTNum = new PValue(res.PVBTNum, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[28].Add(PVBTNum);
                    }

                    if (null != res.PVBVMax)
                    {
                        results[29] = new List<PValue>();
                        PValue PVBVMax = new PValue(res.PVBVMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[29].Add(PVBVMax);
                    }

                    if (null != res.PVBVMin)
                    {
                        results[30] = new List<PValue>();
                        PValue PVBVMin = new PValue(res.PVBVMin, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[30].Add(PVBVMin);
                    }

                    if (null != res.PVBVAvg)
                    {
                        results[31] = new List<PValue>();
                        PValue PVBVAvg = new PValue(res.PVBVAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[31].Add(PVBVAvg);
                    }

                    if (null != res.PVBStbT && res.PVBStbT.Count > 0)
                    {
                        results[32] = new List<PValue>();
                        results[33] = new List<PValue>();
                        results[34] = new List<PValue>();
                        results[35] = new List<PValue>();
                        results[36] = new List<PValue>();
                        results[37] = new List<PValue>();
                        results[38] = new List<PValue>();

                        int StbTcount = res.PVBStbT.Count;
                        for (int j = 0; j < StbTcount; j++)
                        {
                            DateTime StbTstartTime = Convert.ToDateTime(res.PVBStbT[j].startDate);
                            double StbTstartValue = StbTstartTime.Ticks;
                            PValue PVBStbT = new PValue(StbTstartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[32].Add(PVBStbT);
                            DateTime StbTendTime = Convert.ToDateTime(res.PVBStbT[j].endDate);
                            double StbTendValue = StbTendTime.Ticks;
                            PValue PVBStbTEnd = new PValue(StbTendValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[33].Add(PVBStbTEnd);
                            PValue PVBStbTvC = new PValue(res.PVBStbT[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[34].Add(PVBStbTvC);
                            PValue PVBStbTvA = new PValue(res.PVBStbT[j].valueAmount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[35].Add(PVBStbTvA);
                            PValue PVBStbTsV = new PValue(res.PVBStbT[j].startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[36].Add(PVBStbTsV);
                            PValue PVBStbTeV = new PValue(res.PVBStbT[j].endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[37].Add(PVBStbTeV);
                            if (null != res.PVBStbT[j].valueType && res.PVBStbT[j].valueType.Contains("Stb"))
                            {
                                String s = "83";//ASCII码十进制值:83;控制字符:S 
                                Double StbTvTvalue = Double.Parse(s);
                                PValue PVBStbTvT = new PValue(StbTvTvalue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[38].Add(PVBStbTvT);
                            }
                            else
                            {
                                String Err = "69";//ASCII码十进制值:69;控制字符:E 
                                Double StbTvTvalue = Double.Parse(Err);
                                PValue PVBStbTvT = new PValue(StbTvTvalue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[38].Add(PVBStbTvT);
                            }
                        }
                    }

                    if (null != res.PVBStbTR)
                    {
                        results[39] = new List<PValue>();
                        PValue PVBStbTR = new PValue(res.PVBStbTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[39].Add(PVBStbTR);
                    }

                    if (null != res.PVBNoStbT && res.PVBNoStbT.Count > 0)
                    {
                        results[40] = new List<PValue>();
                        results[41] = new List<PValue>();
                        results[42] = new List<PValue>();
                        results[43] = new List<PValue>();
                        results[44] = new List<PValue>();
                        results[45] = new List<PValue>();
                        results[46] = new List<PValue>();
                        int NoStbTcount = res.PVBNoStbT.Count;
                        for (int j = 0; j < NoStbTcount; j++)
                        {
                            DateTime NoStbTstartTime = Convert.ToDateTime(res.PVBNoStbT[j].startDate);
                            double NoStbTstartValue = NoStbTstartTime.Ticks;
                            PValue PVBNoStbT = new PValue(NoStbTstartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[40].Add(PVBNoStbT);
                            DateTime NoStbTendTime = Convert.ToDateTime(res.PVBNoStbT[j].endDate);
                            double NoStbTendValue = NoStbTendTime.Ticks;
                            PValue PVBNoStbTEnd = new PValue(NoStbTendValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[41].Add(PVBNoStbTEnd);
                            PValue PVBNoStbTvC = new PValue(res.PVBNoStbT[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[42].Add(PVBNoStbTvC);
                            PValue PVBNoStbTvA = new PValue(res.PVBNoStbT[j].valueAmount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[43].Add(PVBNoStbTvA);
                            PValue PVBNoStbTsV = new PValue(res.PVBNoStbT[j].startValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[44].Add(PVBNoStbTsV);
                            PValue PVBNoStbTeV = new PValue(res.PVBNoStbT[j].endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[45].Add(PVBNoStbTeV);
                            if (null != res.PVBNoStbT[j].valueType && res.PVBNoStbT[j].valueType.Contains("noStb"))
                            {
                                String s = "110";//ASCII码十进制值:110;控制字符:n 
                                Double NoStbTvTvalue = Double.Parse(s);
                                PValue PVBNoStbTvT = new PValue(NoStbTvTvalue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                                results[46].Add(PVBNoStbTvT);
                            }
                        }
                    }

                    if (null != res.PVBNoStbTR)
                    {
                        results[47] = new List<PValue>();
                        PValue PVBNoStbTR = new PValue(res.PVBNoStbTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[47].Add(PVBNoStbTR);
                    }

                    if (null != res.PVBStbTSLT)
                    {
                        results[48] = new List<PValue>();
                        results[49] = new List<PValue>();
                        DateTime StbTSLTstartTime = Convert.ToDateTime(res.PVBStbTSLT.startDate);
                        double StbTSLTstartValue = StbTSLTstartTime.Ticks;
                        PValue PPVBStbTSLT = new PValue(StbTSLTstartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[48].Add(PPVBStbTSLT);
                        DateTime StbTSLTendTime = Convert.ToDateTime(res.PVBStbTSLT.endDate);  //"2020-02-01 00:00:00"
                        double StbTSLTendValue = StbTSLTendTime.Ticks;
                        PValue PPVBStbTSLTEnd = new PValue(StbTSLTendValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[49].Add(PPVBStbTSLTEnd);
                    }

                    if (null != res.PVBStbTSLPV)
                    {
                        results[50] = new List<PValue>();
                        PValue PVBStbTSLPV = new PValue(res.PVBStbTSLPV, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[50].Add(PVBStbTSLPV);
                        results[51] = new List<PValue>();
                        PValue PVBStbTSL = new PValue(res.PVBStbTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[51].Add(PVBStbTSL);
                    }

                    if (null != res.PVBNoStbTSLT)
                    {
                        results[52] = new List<PValue>();
                        results[53] = new List<PValue>();
                        DateTime noStbStartTime = Convert.ToDateTime(res.PVBNoStbTSLT.startDate);
                        double noStbStartValue = noStbStartTime.Ticks;
                        PValue PPVBNoStbTSLT = new PValue(noStbStartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[52].Add(PPVBNoStbTSLT);
                        DateTime NoStbendTime = Convert.ToDateTime(res.PVBNoStbTSLT.endDate);
                        double noStbEndValue = NoStbendTime.Ticks;
                        PValue PPVBNoStbTSLTEnd = new PValue(noStbEndValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[53].Add(PPVBNoStbTSLTEnd);
                    }

                    if (null != res.PVBNoStbTSL)
                    {
                        results[54] = new List<PValue>();
                        PValue PVBNoStbTSL = new PValue(res.PVBNoStbTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[54].Add(PVBNoStbTSL);
                    }

                    if (null != res.PVBUpTSLT)
                    {
                        results[55] = new List<PValue>();
                        results[56] = new List<PValue>();
                        DateTime UpStartTime = Convert.ToDateTime(res.PVBUpTSLT.startDate);
                        double UpStartValue = UpStartTime.Ticks;
                        PValue PPVBUpTSLT = new PValue(UpStartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[55].Add(PPVBUpTSLT);
                        DateTime UpEndTime = Convert.ToDateTime(res.PVBUpTSLT.endDate);
                        double UpEndValue = UpEndTime.Ticks;
                        PValue PPVBUpTSLTEnd = new PValue(UpEndValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[56].Add(PPVBUpTSLTEnd);
                    }

                    if (null != res.PVBUpTSL)
                    {
                        results[57] = new List<PValue>();
                        PValue PVBUpTSL = new PValue(res.PVBUpTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[57].Add(PVBUpTSL);
                    }

                    if (null != res.PVBDownTSLT)
                    {
                        results[58] = new List<PValue>();
                        results[59] = new List<PValue>();
                        DateTime downStartTime = Convert.ToDateTime(res.PVBDownTSLT.startDate);
                        double downStartValue = downStartTime.Ticks;
                        PValue PPVBDownTSLT = new PValue(downStartValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[58].Add(PPVBDownTSLT);
                        DateTime downEndTime = Convert.ToDateTime(res.PVBDownTSLT.endDate);
                        double downEndValue = downEndTime.Ticks;
                        PValue PPVBDownTSLTEnd = new PValue(downEndValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[59].Add(PPVBDownTSLTEnd);
                    }

                    if (null != res.PVBDownTSL)
                    {
                        results[60] = new List<PValue>();
                        PValue PVBDownTSL = new PValue(res.PVBDownTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[60].Add(PVBDownTSL);
                    }

                    if (null != res.PVBPNum)
                    {
                        results[61] = new List<PValue>();
                        PValue PVBPNum = new PValue(res.PVBPNum, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[61].Add(PVBPNum);
                    }

                    if (null != res.PVBQltR)
                    {
                        results[62] = new List<PValue>();
                        PValue PVBQltR = new PValue(res.PVBQltR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[62].Add(PVBQltR);
                    }

                    if (null != res.PVBStatus)
                    {
                        results[63] = new List<PValue>();
                        PValue PVBStatus = new PValue(Double.Parse(res.PVBStatus), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[63].Add(PVBStatus);
                    }
                    if (null != res.PVBQa)
                    {
                        results[64] = new List<PValue>();
                        PValue PVBQa = new PValue(res.PVBQa, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[64].Add(PVBQa);
                    }

                    if (null != res.PVBQb)
                    {
                        results[65] = new List<PValue>();
                        PValue PVBQb = new PValue(res.PVBQb, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[65].Add(PVBQb);
                    }

                    if (null != res.PVBQc)
                    {
                        results[66] = new List<PValue>();
                        PValue PVBQc = new PValue(res.PVBQc, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[66].Add(PVBQc);
                    }
                    #endregion
                }


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

    }


    #region MPVBase算法程序
    public class MPVBaseModule
    {
        #region/// MPVBase长周期算法（天，月）
        /// </summary>
        /// <param name="valueList">每小时、每天的值,务必确保顺序正确</param>
        /// <param name="N1">单次极差 N1 限值</param>
        /// <param name="N2">单次极差 N2 限值，>N1</param>
        /// <param name="N3">单次极差 N3 限值，>N2</param>
        /// <param name="k">求和乘积系数</param>
        /// <param name="b">求和偏置系数</param>
        /// <param name="stbL">稳定限幅 xi ≤ StbL 为稳定</param>
        /// <param name="noStbL">不稳定限幅 xi ＞ NoStbL 为不稳定</param>
        /// <returns></returns>
        public static MPVBaseMessageOutClass longMPVBase(List<MPVBaseMessageOutClass> valueList, double N1, double N2, double N3, double k, double b, double stbL, double noStbL)
        {
            MPVBaseMessageOutClass returnClass = new MPVBaseMessageOutClass();
            if (valueList.Count > 0)
            {
                double minValue = valueList[0].PVBMin;
                double maxValue = valueList[0].PVBMax;
                string minPVBTime = valueList[0].PVBMinTime;
                string maxPVBTime = valueList[0].PVBMaxTime;
                double avgSumPVB = 0;
                double sumPVB = 0;
                double PVBSumkb = 0;
                double lineKSum = 0;
                double lineBSum = 0;
                double SumPNRPVB = 0;
                double AbsSumPVB = 0;
                double maxStdevPVB = valueList[0].PVBStdev;
                double maxPVBVolatility = 0;

                int sumPVBTNum = 0;
                double VMaxPVB = valueList[0].PVBVMax;
                double VMinPVB = valueList[0].PVBVMin;
                double sumPVBVAvg = 0;
                double sumPVBStbTR = 0;
                double sumPVBNoStbTR = 0;
                //double StbTSLPVB = valueList[0].PVBStbTSL;
                //double sumPVBStbTSLR = 0;
                //double NoStbTSLPVB = valueList[0].PVBNoStbTSL;
                //double sumPVBNoStbTSLR = 0;
                double UpTSLPVB = valueList[0].PVBUpTSL;
                double sumPVBUpTSLR = 0;
                double DownTSLPVB = valueList[0].PVBDownTSL;
                double sumPVBDownTSLR = 0;
                double sumPVBPNum = 0;
                double sumPVBQltR = 0;
                double sumPVBQa = 0;
                double sumPVBQb = 0;
                double sumPVBQc = 0;
                foreach (MPVBaseMessageOutClass item in valueList)
                {
                    sumPVBQa += item.PVBQa;
                    sumPVBQb += item.PVBQb;
                    sumPVBQc += item.PVBQc;
                    if (minValue > item.PVBMin)
                    {
                        minValue = item.PVBMin;
                        minPVBTime = item.PVBMinTime;
                    }
                    if (maxValue < item.PVBMax)
                    {
                        maxValue = item.PVBMax;
                        maxPVBTime = item.PVBMaxTime;
                    }
                    if (maxStdevPVB < item.PVBStdev)
                    {
                        maxStdevPVB = item.PVBStdev;
                    }

                    if (VMaxPVB < item.PVBVMax)
                    {
                        VMaxPVB = item.PVBVMax;
                    }
                    if (VMinPVB > item.PVBVMin)
                    {
                        VMinPVB = item.PVBVMin;
                    }
                    //if (StbTSLPVB < item.PVBStbTSL)
                    //{
                    //    StbTSLPVB = item.PVBStbTSL;
                    //}
                    //if (NoStbTSLPVB < item.PVBNoStbTSL)
                    //{
                    //    NoStbTSLPVB = item.PVBNoStbTSL;
                    //}
                    if (UpTSLPVB < item.PVBUpTSL)
                    {
                        UpTSLPVB = item.PVBUpTSL;
                    }
                    if (DownTSLPVB < item.PVBDownTSL)
                    {
                        DownTSLPVB = item.PVBDownTSL;
                    }
                    avgSumPVB += item.PVBAvg;
                    sumPVB += item.PVBSum;
                    PVBSumkb += item.PVBSumkb;
                    lineKSum += item.PVBLinek;
                    lineBSum += item.PVBLineb;
                    SumPNRPVB += item.PVBSumPNR;
                    AbsSumPVB += item.PVBAbsSum;
                    maxPVBVolatility += item.PVBVolatility;
                    sumPVBTNum += item.PVBTNum;
                    sumPVBVAvg += item.PVBVAvg;
                    sumPVBStbTR += item.PVBStbTR;
                    sumPVBNoStbTR += item.PVBNoStbTR;
                    //sumPVBStbTSLR += item.PVBStbTSLR;
                    //sumPVBNoStbTSLR += item.PVBNoStbTSLR;
                    sumPVBUpTSLR += item.PVBUpTSLR;
                    sumPVBDownTSLR += item.PVBDownTSLR;
                    sumPVBPNum += item.PVBPNum;
                    sumPVBQltR += item.PVBQltR;

                }

                List<D22STimeClass> PVBSDMaxList = new List<D22STimeClass>();
                PVBSDMaxList.AddRange(valueList[0].PVBSDMaxTimeG);
                List<D22STimeClass> PVBStbTList = new List<D22STimeClass>();
                PVBStbTList.AddRange(valueList[0].PVBStbT);
                List<D22STimeClass> PVBNoStbTList = new List<D22STimeClass>();
                PVBNoStbTList.AddRange(valueList[0].PVBNoStbT);
                for (int i = 1; i < valueList.Count(); i++)
                {
                    List<D22STimeClass> newSDlist = new List<D22STimeClass>();
                    if (PVBSDMaxList.Count != 0 && valueList[i].PVBSDMaxTimeG.Count != 0)
                    {
                        newSDlist = AlgorithmHelper.GetD22STimeListSD(PVBSDMaxList[PVBSDMaxList.Count() - 1], valueList[i].PVBSDMaxTimeG[0]);
                        PVBSDMaxList.RemoveAt(PVBSDMaxList.Count() - 1);
                    }
                    PVBSDMaxList.AddRange(newSDlist);
                    for (int l = 1; l < valueList[i].PVBSDMaxTimeG.Count(); l++)
                    {
                        PVBSDMaxList.Add(valueList[i].PVBSDMaxTimeG[l]);
                    }

                    List<D22STimeClass> newStblist = new List<D22STimeClass>();
                    if (PVBStbTList.Count != 0 && valueList[i].PVBStbT.Count != 0)
                    {
                        newStblist = AlgorithmHelper.GetD22STimeListStb(PVBStbTList[PVBStbTList.Count() - 1], valueList[i].PVBStbT[0], stbL, noStbL);
                        PVBStbTList.RemoveAt(PVBStbTList.Count() - 1);
                    }
                    PVBStbTList.AddRange(newStblist);
                    for (int l = 1; l < valueList[i].PVBStbT.Count(); l++)
                    {
                        PVBStbTList.Add(valueList[i].PVBStbT[l]);
                    }

                    List<D22STimeClass> newNoStblist = new List<D22STimeClass>();
                    if (PVBNoStbTList.Count != 0 && valueList[i].PVBNoStbT.Count != 0)
                    {
                        newNoStblist = AlgorithmHelper.GetD22STimeListStb(PVBNoStbTList[PVBNoStbTList.Count() - 1], valueList[i].PVBNoStbT[0], stbL, noStbL);
                        PVBNoStbTList.RemoveAt(PVBNoStbTList.Count() - 1);
                    }
                    PVBNoStbTList.AddRange(newNoStblist);
                    for (int l = 1; l < valueList[i].PVBNoStbT.Count(); l++)
                    {
                        PVBNoStbTList.Add(valueList[i].PVBNoStbT[l]);
                    }


                }

                double SDMaxPVB = PVBSDMaxList[0].valueAmount;
                int sumPVBDN1Num = 0;
                int sumPVBDN2Num = 0;
                int sumPVBDN3Num = 0;
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (SDMaxPVB < item.valueAmount)
                    {
                        SDMaxPVB = item.valueAmount;
                        returnClass.PVBSDMaxTime = item;
                    }
                    if (item.valueAmount > N1)
                    {
                        sumPVBDN1Num += 1;
                    }
                    if (item.valueAmount > N2)
                    {
                        sumPVBDN2Num += 1;
                    }
                    if (item.valueAmount > N3)
                    {
                        sumPVBDN3Num += 1;
                    }
                }

                returnClass.PVBMin = Math.Round(minValue, 3);
                returnClass.PVBMinTime = minPVBTime;
                returnClass.PVBAvg = Math.Round((avgSumPVB / (double)(valueList.Count())), 3);
                returnClass.PVBMax = Math.Round(maxValue, 3);
                returnClass.PVBMaxTime = maxPVBTime;
                returnClass.PVBDMax = Math.Round((maxValue - minValue), 3);
                returnClass.PVBSum = Math.Round(sumPVB, 3);
                returnClass.PVBSumkb = Math.Round(PVBSumkb, 3);
                returnClass.PVBLinek = Math.Round((lineKSum / (double)(valueList.Count())), 3);
                returnClass.PVBLineb = Math.Round((lineBSum / (double)(valueList.Count())), 3);
                returnClass.PVBQa = Math.Round((sumPVBQa / (double)(valueList.Count())), 3);
                returnClass.PVBQb = Math.Round((sumPVBQb / (double)(valueList.Count())), 3);
                returnClass.PVBQc = Math.Round((sumPVBQc / (double)(valueList.Count())), 3);
                returnClass.PVBSumPNR = Math.Round((SumPNRPVB / (double)(valueList.Count())), 3);
                returnClass.PVBAbsSum = Math.Round(AbsSumPVB, 3);
                returnClass.PVBStdev = Math.Round((maxStdevPVB * 0.75), 3);
                returnClass.PVBVolatility = Math.Round((maxPVBVolatility / (double)(valueList.Count())), 3);
                returnClass.PVBSDMaxTimeG = PVBSDMaxList;



                returnClass.PVBSDMax = Math.Round(SDMaxPVB, 3);
                returnClass.PVBSDMaxR = Math.Round((SDMaxPVB / (maxValue - minValue)), 5) * 100;
                returnClass.PVBDN1Num = sumPVBDN1Num;
                returnClass.PVBDN2Num = sumPVBDN2Num;
                returnClass.PVBDN3Num = sumPVBDN3Num;
                returnClass.PVBTNum = sumPVBTNum;
                returnClass.PVBVMax = Math.Round(VMaxPVB, 3);
                returnClass.PVBVMin = Math.Round(VMinPVB, 3);
                returnClass.PVBVAvg = Math.Round((sumPVBVAvg / (double)(valueList.Count())), 3);



                returnClass.PVBStbT = PVBStbTList;
                returnClass.PVBStbTR = Math.Round((sumPVBStbTR / (double)(valueList.Count())), 3);
                returnClass.PVBNoStbT = PVBNoStbTList;
                returnClass.PVBNoStbTR = Math.Round((sumPVBNoStbTR / (double)(valueList.Count())), 3);

                if (PVBStbTList.Count > 0)
                {
                    D22STimeClass PVBStbTSLT = new D22STimeClass();
                    double PVBStbTSL = PVBStbTList[0].valueCount;
                    double PVBStbTSLPV = PVBStbTList[0].valueAmount;
                    PVBStbTSLT = PVBStbTList[0];
                    foreach (D22STimeClass item in PVBStbTList)
                    {
                        if (PVBStbTSL < item.valueCount)
                        {
                            PVBStbTSL = item.valueCount;
                            PVBStbTSLPV = item.valueAmount;
                            PVBStbTSLT = item;
                        }
                    }
                    returnClass.PVBStbTSLT = PVBStbTSLT;
                    returnClass.PVBStbTSLPV = Math.Round(PVBStbTSLPV / PVBStbTSL, 3);
                    returnClass.PVBStbTSL = PVBStbTSL;
                }
                if (PVBNoStbTList.Count > 0)
                {
                    D22STimeClass PVBNoStbTSLT = new D22STimeClass();
                    double PVBNoStbTSL = 0;
                    PVBNoStbTSLT = PVBNoStbTList[0];
                    foreach (D22STimeClass item in PVBNoStbTList)
                    {
                        if (PVBNoStbTSL < item.valueCount)
                        {
                            PVBNoStbTSL = item.valueCount;
                            PVBNoStbTSLT = item;
                        }
                    }
                    returnClass.PVBNoStbTSLT = PVBNoStbTSLT;
                    returnClass.PVBNoStbTSL = PVBNoStbTSL;
                }
                //returnClass.PVBStbTSL = StbTSLPVB;
                //returnClass.PVBStbTSLR = Math.Round((sumPVBStbTSLR / (double)(valueList.Count())), 3);
                //returnClass.PVBNoStbTSL = NoStbTSLPVB;
                //returnClass.PVBNoStbTSLR = Math.Round((sumPVBNoStbTSLR / (double)(valueList.Count())), 3);

                D22STimeClass PVBUpTSLT = new D22STimeClass();
                D22STimeClass PVBDownTSLT = new D22STimeClass();
                double PVBUpTSL = 0;
                double PVBDownTSL = 0;
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (item.valueType.Equals("up"))
                    {
                        PVBUpTSL = item.valueCount;
                        break;
                    }
                }
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (item.valueType.Equals("down"))
                    {
                        PVBDownTSL = item.valueCount;
                        break;
                    }
                }
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (item.valueType.Equals("up"))
                    {
                        if (PVBUpTSL < item.valueCount)
                        {
                            PVBUpTSL = item.valueCount;
                            PVBUpTSLT = item;
                        }
                    }
                    else if (item.valueType.Equals("down"))
                    {
                        if (PVBDownTSL < item.valueCount)
                        {
                            PVBDownTSL = item.valueCount;
                            PVBDownTSLT = item;
                        }
                    }
                }
                returnClass.PVBUpTSLT = PVBUpTSLT;
                returnClass.PVBUpTSL = PVBUpTSL;
                returnClass.PVBDownTSLT = PVBDownTSLT;
                returnClass.PVBDownTSL = PVBDownTSL;

                //returnClass.PVBUpTSL = UpTSLPVB;
                //returnClass.PVBUpTSLR = Math.Round((sumPVBUpTSLR / (double)(valueList.Count())), 3);
                //returnClass.PVBDownTSL = DownTSLPVB;
                //returnClass.PVBDownTSLR = Math.Round((sumPVBDownTSLR / (double)(valueList.Count())), 3);
                returnClass.PVBPNum = sumPVBPNum;
                returnClass.PVBQltR = Math.Round((sumPVBQltR / (double)(valueList.Count())), 3);
            }
            return returnClass;
        }
        #endregion

        #region/// MPV短周期算法（小时）
        /// <summary>
        /// <param name="valueList">每分钟的值,务必确保顺序正确</param>
        /// <param name="N1">单次极差 N1 限值</param>
        /// <param name="N2">单次极差 N2 限值，>N1</param>
        /// <param name="N3">单次极差 N3 限值，>N2</param>
        /// <param name="k">求和乘积系数</param>
        /// <param name="b">求和偏置系数</param>
        /// <param name="stbL">稳定限幅 xi ≤ StbL 为稳定</param>
        /// <param name="noStbL">不稳定限幅 xi ＞ NoStbL 为不稳定</param>
        /// <returns></returns>
        public static MPVBaseMessageOutBadClass shortMPVBase(List<MPVBaseMessageInBadClass> valueList, double N1, double N2, double N3, double k, double b, double stbL, double noStbL)
        {
            try
            {
                if (N1 >= N2)
                {
                    new Exception("N1必须小于N2");
                }
                if (N2 >= N3)
                {
                    new Exception("N2必须小于N3");
                }
                MPVBaseMessageOutBadClass returnClass = new MPVBaseMessageOutBadClass();
                List<MPVBaseMessageInClass> effectValueList = new List<MPVBaseMessageInClass>();
                List<D22STimeClass> rangeList = new List<D22STimeClass>();
                string sDate = string.Empty;
                string preDate = string.Empty;
                double preValue = 0;
                double SDMaxValue = 0;
                double SDMaxAmount = 0;
                int SDStatus = 0;//0:初始点，坏点；1：上升点；2：下降点;3:延伸点
                int valueStatus = 0;//上一点状态 0：未判断；1：好点；2：坏点
                List<StartEndDateClass> effectDateRegion = new List<StartEndDateClass>();
                string eStartDate = string.Empty;
                for (int i = 0; i < valueList.Count; i++)
                {
                    #region 取有效值
                    if (!valueList[i].valueAmount.Equals("-"))
                    {
                        MPVBaseMessageInClass newValueClass = new MPVBaseMessageInClass();
                        newValueClass.seq = i + 1;
                        newValueClass.valueDate = valueList[i].valueDate;
                        newValueClass.valueAmount = Convert.ToDouble(valueList[i].valueAmount);
                        effectValueList.Add(newValueClass);
                    }
                    #endregion
                }
                if (effectValueList.Count() == 0)
                {

                }
                else
                {
                    for (int i = 0; i < valueList.Count; i++)
                    {
                        if (i == (valueList.Count - 1))
                        {

                        }
                        #region 取单次极差列表
                        if (SDStatus == 0)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {

                                if (valueStatus == 0 || valueStatus == 2)
                                {
                                    sDate = valueList[i].valueDate;
                                    preDate = valueList[i].valueDate;
                                    preValue = Convert.ToDouble(valueList[i].valueAmount);
                                    SDMaxAmount = 1;
                                    SDStatus = 0;
                                }
                                else
                                {
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) == Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        //sDate = valueList[i].valueDate;
                                        preDate = valueList[i].valueDate;
                                        preValue = Convert.ToDouble(valueList[i].valueAmount);
                                        SDMaxAmount = 1;
                                        SDStatus = 3;
                                    }
                                    else
                                    {
                                        preDate = valueList[i].valueDate;
                                        SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                        SDMaxAmount = 2;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            SDStatus = 1;
                                        }
                                        else if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            SDStatus = 2;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        else if (SDStatus == 1)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {
                                if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                {

                                    preDate = valueList[i].valueDate;
                                    SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount += 1;
                                    SDStatus = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = preDate;
                                        newClass.valueCount = SDMaxAmount;
                                        newClass.valueAmount = SDMaxValue;
                                        newClass.valueType = "up";
                                        rangeList.Add(newClass);
                                    }
                                }
                                else
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = SDMaxAmount;
                                    newClass.valueAmount = SDMaxValue;
                                    newClass.valueType = "up";
                                    rangeList.Add(newClass);

                                    sDate = preDate;
                                    preDate = valueList[i].valueDate;
                                    SDMaxValue = Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newLastClass = new D22STimeClass();
                                        newLastClass.startDate = sDate;
                                        newLastClass.endDate = preDate;
                                        newLastClass.valueCount = SDMaxAmount;
                                        newLastClass.valueAmount = SDMaxValue;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            newLastClass.valueType = "down";
                                        }
                                        else
                                        {
                                            newLastClass.valueType = "translation";
                                        }
                                        rangeList.Add(newLastClass);
                                    }
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        SDStatus = 2;
                                    }
                                    else
                                    {
                                        SDStatus = 3;
                                    }
                                }
                            }
                            else
                            {
                                D22STimeClass newClass = new D22STimeClass();
                                newClass.startDate = sDate;
                                newClass.endDate = preDate;
                                newClass.valueCount = SDMaxAmount;
                                newClass.valueAmount = SDMaxValue;
                                newClass.valueType = "up";
                                rangeList.Add(newClass);

                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        else if (SDStatus == 2)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {
                                if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                {

                                    preDate = valueList[i].valueDate;
                                    SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount += 1;
                                    SDStatus = 2;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = preDate;
                                        newClass.valueCount = SDMaxAmount;
                                        newClass.valueAmount = SDMaxValue;
                                        newClass.valueType = "down";
                                        rangeList.Add(newClass);
                                    }
                                }
                                else
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = SDMaxAmount;
                                    newClass.valueAmount = SDMaxValue;
                                    newClass.valueType = "down";
                                    rangeList.Add(newClass);

                                    sDate = preDate;
                                    preDate = valueList[i].valueDate;
                                    SDMaxValue = Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newLastClass = new D22STimeClass();
                                        newLastClass.startDate = sDate;
                                        newLastClass.endDate = preDate;
                                        newLastClass.valueCount = SDMaxAmount;
                                        newLastClass.valueAmount = SDMaxValue;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            newLastClass.valueType = "up";
                                        }
                                        else
                                        {
                                            newLastClass.valueType = "translation";
                                        }
                                        rangeList.Add(newLastClass);
                                    }
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        SDStatus = 1;
                                    }
                                    else
                                    {
                                        SDStatus = 3;
                                    }
                                }
                            }
                            else
                            {
                                D22STimeClass newClass = new D22STimeClass();
                                newClass.startDate = sDate;
                                newClass.endDate = preDate;
                                newClass.valueCount = SDMaxAmount;
                                newClass.valueAmount = SDMaxValue;
                                newClass.valueType = "down";
                                rangeList.Add(newClass);

                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        else if (SDStatus == 3)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {
                                if (Convert.ToDouble(valueList[i - 1].valueAmount) == Convert.ToDouble(valueList[i].valueAmount))
                                {

                                    preDate = valueList[i].valueDate;
                                    SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount += 1;
                                    SDStatus = 3;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = preDate;
                                        newClass.valueCount = SDMaxAmount;
                                        newClass.valueAmount = SDMaxValue;
                                        newClass.valueType = "down";
                                        rangeList.Add(newClass);
                                    }
                                }
                                else
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = SDMaxAmount;
                                    newClass.valueAmount = SDMaxValue;
                                    newClass.valueType = "translation";
                                    rangeList.Add(newClass);

                                    sDate = preDate;
                                    preDate = valueList[i].valueDate;
                                    SDMaxValue = Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newLastClass = new D22STimeClass();
                                        newLastClass.startDate = sDate;
                                        newLastClass.endDate = preDate;
                                        newLastClass.valueCount = SDMaxAmount;
                                        newLastClass.valueAmount = SDMaxValue;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            newLastClass.valueType = "up";
                                        }
                                        else
                                        {
                                            newLastClass.valueType = "down";
                                        }
                                        rangeList.Add(newLastClass);
                                    }
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        SDStatus = 1;
                                    }
                                    else
                                    {
                                        SDStatus = 2;
                                    }
                                }
                            }
                            else
                            {
                                D22STimeClass newClass = new D22STimeClass();
                                newClass.startDate = sDate;
                                newClass.endDate = preDate;
                                newClass.valueCount = SDMaxAmount;
                                newClass.valueAmount = SDMaxValue;
                                newClass.valueType = "translation";
                                rangeList.Add(newClass);

                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        #endregion
                        #region 取有效时间段
                        if (valueList[i].valueAmount.Equals("-"))
                        {
                            if (!string.IsNullOrWhiteSpace(eStartDate))
                            {
                                StartEndDateClass newDateClass = new StartEndDateClass();
                                newDateClass.startDate = eStartDate;
                                newDateClass.endDate = valueList[i - 1].valueDate;
                                effectDateRegion.Add(newDateClass);
                            }
                            eStartDate = string.Empty;
                        }
                        else
                        {
                            if (valueStatus == 2 || valueStatus == 0)
                            {
                                eStartDate = valueList[i].valueDate;
                            }
                            if (i == (valueList.Count - 1))
                            {
                                StartEndDateClass newDateClass = new StartEndDateClass();
                                newDateClass.startDate = eStartDate;
                                newDateClass.endDate = valueList[i].valueDate;
                                effectDateRegion.Add(newDateClass);
                            }
                        }
                        #endregion
                        #region 为是否好坏点状态赋值
                        if (!valueList[i].valueAmount.Equals("-"))
                        {
                            valueStatus = 1;
                        }
                        else
                        {
                            valueStatus = 2;
                        }
                        #endregion
                    }
                    double minValue = effectValueList[0].valueAmount;
                    double maxValue = effectValueList[0].valueAmount;
                    string PVBMaxTime = effectValueList[0].valueDate;
                    string PVBMinTime = effectValueList[0].valueDate;
                    double sumValue = 0;//总和
                    double sumPositive = 0;//正数和
                    double sumNegative = 0;//负数和
                    double sumAbsolute = 0;//绝对值和
                    List<CurveClass> xyList = new List<CurveClass>();
                    double[] xList = new double[valueList.Count];
                    double[] yList = new double[valueList.Count];
                    int lIndex = 0;
                    #region 使用有效数据集合算出一些没有持续属性的返回值
                    foreach (MPVBaseMessageInClass item in effectValueList)
                    {
                        xList[lIndex] = item.seq;
                        yList[lIndex] = item.valueAmount;
                        sumValue += item.valueAmount;
                        sumAbsolute += Math.Abs(item.valueAmount);
                        if (item.valueAmount > 0)
                        {
                            sumPositive += item.valueAmount;
                        }
                        else if (item.valueAmount < 0)
                        {
                            sumNegative += item.valueAmount;
                        }
                        if (item.valueAmount > maxValue)
                        {
                            maxValue = item.valueAmount;
                            PVBMaxTime = item.valueDate;//最大值发生时刻
                        }
                        if (item.valueAmount < minValue)
                        {
                            minValue = item.valueAmount;
                            PVBMinTime = item.valueDate;//最小值发生时刻
                        }
                        lIndex++;
                        CurveClass xy = new CurveClass();
                        xy.x = item.seq;
                        xy.y = item.valueAmount;
                        xyList.Add(xy);
                    }
                    returnClass.PVBMin = Math.Round(minValue, 3).ToString();//最小值
                    returnClass.PVBMax = Math.Round(maxValue, 3).ToString();//最大值
                    returnClass.PVBMaxTime = PVBMaxTime;
                    returnClass.PVBMinTime = PVBMinTime;
                    returnClass.PVBAvg = Math.Round((sumValue / (valueList.Count() + 0.0)), 3).ToString();//平均值
                    returnClass.PVBDMax = Math.Round((maxValue - minValue), 3).ToString();//总极差
                    returnClass.PVBSum = Math.Round(sumValue, 3).ToString();//和
                    returnClass.PVBSumkb = Math.Round(sumValue * k + b, 3).ToString();//上面的和乘 k 加 b
                    double returnK = 0;
                    double returnB = 0;
                    LinearRegressionSolve(xyList, ref returnK, ref returnB);
                    returnClass.PVBLinek = returnK.ToString();//以直线拟合时的斜率 Regress_k
                    returnClass.PVBLineb = returnB.ToString();//以直线拟合时的截距 Regress_b
                    returnClass.PVBSumPNR = ((Math.Round(sumPositive / (sumNegative * -1), 5)) * 100).ToString();//正负比。正数和 / 绝对值(负数和) * 100%
                    returnClass.PVBAbsSum = Math.Round(sumAbsolute, 3).ToString();//绝对值和
                    returnClass.PVBStdev = StandardDeviationSolve(xyList).ToString();//标准差

                    #endregion
                    double sdMax = rangeList[0].valueAmount;
                    int n1Num = 0;
                    int n2Num = 0;
                    int n3Num = 0;

                    int PVBTNum = 0;
                    #region 用极差分段集合得出数据
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueAmount > sdMax)
                        {
                            sdMax = item.valueAmount;
                            returnClass.PVBSDMaxTime = item;//单次极差发生时刻
                        }
                        if (item.valueAmount > N1)
                        {
                            n1Num += 1;
                        }
                        if (item.valueAmount > N2)
                        {
                            n2Num += 1;
                        }
                        if (item.valueAmount > N3)
                        {
                            n3Num += 1;
                        }

                    }
                    #endregion
                    #region 用有效时间段退出反转次数以及有效数据分段集合
                    List<List<MPVBaseMessageInClass>> effectValueListList = new List<List<MPVBaseMessageInClass>>();
                    int effectIndex = 0;
                    foreach (var dateItem in effectDateRegion)
                    {
                        int TT = 0;
                        foreach (D22STimeClass item in rangeList)
                        {
                            if (Convert.ToDateTime(dateItem.startDate) <= Convert.ToDateTime(item.startDate) && Convert.ToDateTime(dateItem.endDate) >= Convert.ToDateTime(item.endDate))
                            {
                                TT += 1;
                            }
                        }
                        PVBTNum = PVBTNum + (TT == 0 ? 0 : TT - 1);
                        List<MPVBaseMessageInClass> newList = new List<MPVBaseMessageInClass>();
                        bool isIn = false;
                        for (; effectIndex < valueList.Count; effectIndex++)
                        {
                            if (Convert.ToDateTime(dateItem.startDate) <= Convert.ToDateTime(valueList[effectIndex].valueDate) && Convert.ToDateTime(dateItem.endDate) >= Convert.ToDateTime(Convert.ToDateTime(valueList[effectIndex].valueDate)))
                            {
                                isIn = true;
                                MPVBaseMessageInClass newValueClass = new MPVBaseMessageInClass();
                                newValueClass.seq = effectIndex + 1;
                                newValueClass.valueDate = valueList[effectIndex].valueDate;
                                newValueClass.valueAmount = Convert.ToDouble(valueList[effectIndex].valueAmount);
                                newList.Add(newValueClass);
                            }
                            else
                            {
                                if (isIn)
                                {
                                    break;
                                }
                            }
                        }
                        if (newList.Count > 0)
                        {
                            effectValueListList.Add(newList);
                        }
                    }
                    #endregion
                    returnClass.PVBSDMax = Math.Round(sdMax, 3).ToString();//单次极差。Max(|Δxi|)。极差：指 x 所经历的全部峰 - 谷差的绝对值

                    returnClass.PVBSDMaxR = ((Math.Round((Convert.ToDouble(returnClass.PVBSDMax) / (Convert.ToDouble(returnClass.PVBMax) - Convert.ToDouble(returnClass.PVBMin))), 5)) * 100).ToString();//单次极差比，绝对值。PVSDMax / PVBDMax x 100%
                    returnClass.PVBDN1Num = n1Num.ToString();//极差大于 N1 次数
                    returnClass.PVBDN2Num = n2Num.ToString();//极差大于 N2 次数
                    returnClass.PVBDN3Num = n3Num.ToString();//极差大于 N3 次数
                    returnClass.PVBTNum = PVBTNum.ToString();//翻转次数

                    List<double> vMaxList = new List<double>();
                    List<double> vMinList = new List<double>();
                    double vSum = 0;//初始化总绝对值速度
                    double vCount = 0;
                    double stbTR = 0;//初始化稳定时间
                    double noStbTR = 0;//初始化不稳定时间

                    List<D22STimeClass> PVBStbT = new List<D22STimeClass>();//初始化连续稳定时间列表
                    List<D22STimeClass> PVBNoStbT = new List<D22STimeClass>();//初始化连续不稳定时间列表
                    foreach (List<MPVBaseMessageInClass> item in effectValueListList)
                    {
                        if (item.Count() > 1)
                        {
                            double vMax = 0;//初始化最大速度
                            double vMin = 0;//初始化最小速度
                            vCount += (item.Count() - 1);

                            int isStbTr = 0;//0初始值，1稳定，2不稳定
                            string stbSDate = string.Empty;
                            double initialStbTrTime = 0;//初始化连续时间（稳定/不稳定）
                            double initialStbTr = 0;
                            double SValue = 0;
                            for (int i = 1; i < item.Count(); i++)
                            {
                                if (vMax < item[i].valueAmount - item[i - 1].valueAmount)
                                {
                                    vMax = item[i].valueAmount - item[i - 1].valueAmount;
                                }
                                if (vMin > item[i].valueAmount - item[i - 1].valueAmount)
                                {
                                    vMin = item[i].valueAmount - item[i - 1].valueAmount;
                                }
                                vSum += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);

                                if (Math.Abs(item[i].valueAmount - item[i - 1].valueAmount) <= stbL)
                                {
                                    //求稳定时间
                                    stbTR += 1;
                                    if (i == item.Count() - 1)
                                    {
                                        if (isStbTr == 1)
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = stbSDate;
                                            Stb.startValue = SValue;
                                            Stb.endDate = item[i].valueDate;
                                            Stb.endValue = item[i].valueAmount;
                                            Stb.valueAmount = initialStbTr;
                                            Stb.valueCount = initialStbTrTime;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);
                                        }
                                        else if (isStbTr == 2)
                                        {
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = stbSDate;
                                            noStb.startValue = SValue;
                                            noStb.endDate = item[i - 1].valueDate;
                                            noStb.endValue = item[i - 1].valueAmount;
                                            noStb.valueAmount = initialStbTr;
                                            noStb.valueCount = initialStbTrTime;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);

                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = item[i - 1].valueDate;
                                            Stb.startValue = item[i - 1].valueAmount;
                                            Stb.endDate = item[i].valueDate;
                                            Stb.endValue = item[i].valueAmount;
                                            Stb.valueAmount = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            Stb.valueCount = 1;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);
                                        }
                                    }
                                    else
                                    {
                                        if (isStbTr == 0)
                                        {
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else if (isStbTr == 1)
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else
                                        {
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = stbSDate;
                                            noStb.endDate = item[i - 1].valueDate;
                                            noStb.startValue = SValue;
                                            noStb.endValue = item[i - 1].valueAmount;
                                            noStb.valueAmount = initialStbTr;
                                            noStb.valueCount = initialStbTrTime;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);

                                            initialStbTrTime = 1;
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTr = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        isStbTr = 1;
                                    }


                                }
                                else if (Math.Abs(item[i].valueAmount - item[i - 1].valueAmount) > noStbL)
                                {
                                    //求不稳定时间
                                    noStbTR += 1;

                                    if (i == item.Count() - 1)
                                    {
                                        if (isStbTr == 1)
                                        {
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = item[i - 1].valueDate;
                                            noStb.startValue = item[i - 1].valueAmount;
                                            noStb.endDate = item[i].valueDate;
                                            noStb.endValue = item[i].valueAmount;
                                            noStb.valueAmount = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            noStb.valueCount = 1;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);

                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = stbSDate;
                                            Stb.startValue = SValue;
                                            Stb.endDate = item[i - 1].valueDate;
                                            Stb.endValue = item[i - 1].valueAmount;
                                            Stb.valueAmount = initialStbTr;
                                            Stb.valueCount = initialStbTrTime;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);
                                        }
                                        else if (isStbTr == 2)
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = stbSDate;
                                            noStb.startValue = SValue;
                                            noStb.endDate = item[i].valueDate;
                                            noStb.endValue = item[i].valueAmount;
                                            noStb.valueAmount = initialStbTr;
                                            noStb.valueCount = initialStbTrTime;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);
                                        }
                                    }
                                    else
                                    {
                                        if (isStbTr == 0)
                                        {
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else if (isStbTr == 1)
                                        {
                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = stbSDate;
                                            Stb.startValue = SValue;
                                            Stb.endDate = item[i - 1].valueDate;
                                            Stb.endValue = item[i - 1].valueAmount;
                                            Stb.valueAmount = initialStbTr;
                                            Stb.valueCount = initialStbTrTime;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);

                                            initialStbTrTime = 1;
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTr = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        isStbTr = 2;
                                    }
                                }
                                else
                                {

                                    if (isStbTr == 1)
                                    {
                                        D22STimeClass Stb = new D22STimeClass();
                                        Stb.startDate = stbSDate;
                                        Stb.startValue = SValue;
                                        Stb.endDate = item[i - 1].valueDate;
                                        Stb.endValue = item[i - 1].valueAmount;
                                        Stb.valueAmount = initialStbTr;
                                        Stb.valueCount = initialStbTrTime;
                                        Stb.valueType = "Stb";
                                        PVBStbT.Add(Stb);
                                    }
                                    else if (isStbTr == 2)
                                    {
                                        D22STimeClass noStb = new D22STimeClass();
                                        noStb.startDate = stbSDate;
                                        noStb.startValue = SValue;
                                        noStb.endDate = item[i - 1].valueDate;
                                        noStb.endValue = item[i - 1].valueAmount;
                                        noStb.valueAmount = initialStbTr;
                                        noStb.valueCount = initialStbTrTime;
                                        noStb.valueType = "noStb";
                                        PVBNoStbT.Add(noStb);
                                    }
                                    stbSDate = item[i].valueDate;
                                    isStbTr = 0;
                                    SValue = 0;
                                    initialStbTrTime = 0;
                                    initialStbTr = 0;
                                }
                            }
                            vMaxList.Add(vMax);
                            vMinList.Add(vMin);


                        }

                    }

                    returnClass.PVBStbT = PVBStbT;
                    returnClass.PVBNoStbT = PVBNoStbT;



                    returnClass.PVBVMax = Math.Round(vMaxList.Max(), 3).ToString();//最大速度，max(Δxi)
                    returnClass.PVBVMin = Math.Round(vMinList.Min(), 3).ToString();//最小速度，min(Δxi)
                    returnClass.PVBVAvg = Math.Round((vSum / vCount), 3).ToString();//平均速度，Avg|Δxi|
                    returnClass.PVBVolatility = Math.Round((vSum / (double)(valueList.Count() - 1)), 3).ToString();//波动。|Δxi| = | xi+1 - xi | 求和，除以段数，即，点数 - 1
                    returnClass.PVBStbTR = ((Math.Round((stbTR / (double)(valueList.Count() - 1)), 5)) * 100).ToString();//稳定时间占比，稳定：|Δxi| ≤ StbL
                    returnClass.PVBNoStbTR = ((Math.Round((noStbTR / (double)(valueList.Count() - 1)), 5)) * 100).ToString();//不稳定时间占比，稳定：|Δxi| ＞ NoStbL

                    if (PVBStbT.Count > 0)
                    {
                        D22STimeClass PVBStbTSLT = new D22STimeClass();
                        PVBStbTSLT = PVBStbT[0];
                        double PVBStbTSL = PVBStbT[0].valueCount;
                        double PVBStbTSLPV = PVBStbT[0].valueAmount;
                        foreach (D22STimeClass item in PVBStbT)
                        {
                            if (PVBStbTSL < item.valueCount)
                            {
                                PVBStbTSL = item.valueCount;
                                PVBStbTSLPV = item.valueAmount;
                                PVBStbTSLT = item;
                            }
                        }
                        returnClass.PVBStbTSLT = PVBStbTSLT;
                        returnClass.PVBStbTSLPV = Math.Round(PVBStbTSLPV / PVBStbTSL, 3).ToString();
                        returnClass.PVBStbTSL = PVBStbTSL.ToString();
                    }
                    if (PVBNoStbT.Count > 0)
                    {
                        D22STimeClass PVBNoStbTSLT = new D22STimeClass();
                        double PVBNoStbTSL = 0;
                        foreach (D22STimeClass item in PVBNoStbT)
                        {
                            if (PVBNoStbTSL < item.valueCount)
                            {
                                PVBNoStbTSL = item.valueCount;
                                PVBNoStbTSLT = item;
                            }
                        }
                        returnClass.PVBNoStbTSLT = PVBNoStbTSLT;
                        returnClass.PVBNoStbTSL = PVBNoStbTSL.ToString();
                    }
                    D22STimeClass PVBUpTSLT = new D22STimeClass();
                    D22STimeClass PVBDownTSLT = new D22STimeClass();
                    double PVBUpTSL = 0;
                    double PVBDownTSL = 0;
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueType.Equals("up"))
                        {
                            PVBUpTSL = item.valueCount;
                            PVBUpTSLT = item;
                            break;
                        }
                    }
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueType.Equals("down"))
                        {
                            PVBDownTSL = item.valueCount;
                            PVBDownTSLT = item;
                            break;
                        }
                    }
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueType.Equals("up"))
                        {
                            if (PVBUpTSL < item.valueCount)
                            {
                                PVBUpTSL = item.valueCount;
                                PVBUpTSLT = item;
                            }
                        }
                        else if (item.valueType.Equals("down"))
                        {
                            if (PVBDownTSL < item.valueCount)
                            {
                                PVBDownTSL = item.valueCount;
                                PVBDownTSLT = item;
                            }
                        }
                    }
                    returnClass.PVBUpTSLT = PVBUpTSLT;
                    returnClass.PVBUpTSL = PVBUpTSL.ToString();
                    returnClass.PVBDownTSLT = PVBDownTSLT;
                    returnClass.PVBDownTSL = PVBDownTSL.ToString();
                    returnClass.PVBPNum = effectValueList.Count().ToString();
                    returnClass.PVBQltR = ((Math.Round((effectValueList.Count() / (double)(valueList.Count())), 5)) * 100).ToString();

                    double[] res = Fit.Polynomial(xList, yList, 2);
                    returnClass.PVBQa = Math.Round(res[2], 3).ToString();
                    returnClass.PVBQb = Math.Round(res[1], 3).ToString();
                    returnClass.PVBQc = Math.Round(res[0], 3).ToString();

                    foreach (D22STimeClass item in rangeList)
                    {
                        foreach (MPVBaseMessageInClass effItem in effectValueList)
                        {
                            if (item.startDate == effItem.valueDate)
                            {
                                item.startValue = effItem.valueAmount;
                            }
                            if (item.endDate == effItem.valueDate)
                            {
                                item.endValue = effItem.valueAmount;
                            }
                        }
                    }
                    returnClass.PVBSDMaxTimeG = rangeList;
                }
                return returnClass;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region/// 一元线性回归 LinearRegressionSolve
        /// <param name="_plist">坐标值集合</param>
        /// <param name="returnK">斜率</param>
        /// <param name="returnB">截距</param>
        public static void LinearRegressionSolve(List<CurveClass> _plist, ref double returnK, ref double returnB)
        {
            try
            {
                double k = 0, b = 0;
                double sumX = 0, sumY = 0;
                double avgX = 0, avgY = 0;
                foreach (var v in _plist)
                {
                    sumX += v.x;
                    sumY += v.y;
                }
                avgX = sumX / (_plist.Count + 0.0);
                avgY = sumY / (_plist.Count + 0.0);

                //sumA=(x-avgX)(y-avgY)的和 sumB=(x-avgX)平方
                double sumA = 0, sumB = 0;
                foreach (var v in _plist)
                {
                    sumA += (v.x - avgX) * (v.y - avgY);
                    sumB += (v.x - avgX) * (v.x - avgX);
                }
                k = sumA / (sumB + 0.0);
                b = avgY - k * avgX;
                returnK = Math.Round(k, 3);
                returnB = Math.Round(b, 3);

            }
            catch (Exception ex)
            {
                throw new Exception("线性回归算法出错");
            }
        }
        #endregion

        #region/// 标准差 StandardDeviationSolve
        /// <param name="_plist">坐标值集合</param>
        /// <returns>标准差</returns>
        public static double StandardDeviationSolve(List<CurveClass> _plist)
        {
            double avgValue = 0;
            double sumValue = 0;
            double variance = 0;
            foreach (CurveClass item in _plist)
            {
                sumValue += item.y;
            }
            avgValue = sumValue / (_plist.Count + 0.0);
            foreach (CurveClass item in _plist)
            {
                variance += Math.Pow((item.y - avgValue), 2.0);
            }
            return Math.Round(Math.Sqrt(variance / (_plist.Count() + 0.0)), 3);
        }
        #endregion

    }
    #endregion

    public class MPVBaseMessageOutBadClass
    {
        public string PVBMin { get; set; }
        public string PVBMinTime { get; set; }
        public string PVBAvg { get; set; }
        public string PVBMax { get; set; }
        public string PVBMaxTime { get; set; }
        public string PVBDMax { get; set; }
        public string PVBSum { get; set; }
        public string PVBSumkb { get; set; }
        public string PVBLinek { get; set; }
        public string PVBLineb { get; set; }
        public string PVBSumPNR { get; set; }
        public string PVBAbsSum { get; set; }
        public string PVBStdev { get; set; }
        public string PVBVolatility { get; set; }
        public string PVBSDMax { get; set; }
        public D22STimeClass PVBSDMaxTime { get; set; }
        public string PVBSDMaxR { get; set; }
        public string PVBDN1Num { get; set; }
        public string PVBDN2Num { get; set; }
        public string PVBDN3Num { get; set; }
        public string PVBTNum { get; set; }
        public string PVBVMax { get; set; }
        public string PVBVMin { get; set; }
        public string PVBVAvg { get; set; }
        public string PVBStbTR { get; set; }
        public string PVBNoStbTR { get; set; }
        public string PVBStbTSL { get; set; }
        public string PVBStbTSLR { get; set; }
        public string PVBNoStbTSL { get; set; }
        public string PVBNoStbTSLR { get; set; }
        public string PVBUpTSL { get; set; }
        public string PVBUpTSLR { get; set; }
        public string PVBDownTSL { get; set; }
        public string PVBDownTSLR { get; set; }
        public string PVBPNum { get; set; }
        public string PVBQltR { get; set; }
        public string PVBStatus { get; set; }
        public string PVBQa { get; set; }
        public string PVBQb { get; set; }
        public string PVBQc { get; set; }
        public string PVBStbTSLPV { get; set; }


        public List<D22STimeClass> PVBStbT { get; set; }
        public List<D22STimeClass> PVBNoStbT { get; set; }


        public D22STimeClass PVBStbTSLT { get; set; }
        public D22STimeClass PVBNoStbTSLT { get; set; }

        public D22STimeClass PVBUpTSLT { get; set; }
        public D22STimeClass PVBDownTSLT { get; set; }

        public List<D22STimeClass> PVBSDMaxTimeG { get; set; }
    }

    public class StartEndDateClass
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
    }

    public class CurveClass
    {
        private double _x;
        private double _y;

        public double x
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        public double y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }
        }
    }



    public class MPVBaseMessageOutClass
    {
        public int id { get; set; }
        public double PVBMin { get; set; }
        public string PVBMinTime { get; set; }
        public double PVBAvg { get; set; }
        public double PVBMax { get; set; }
        public string PVBMaxTime { get; set; }
        public double PVBDMax { get; set; }
        public double PVBSum { get; set; }
        public double PVBSumkb { get; set; }
        public double PVBLinek { get; set; }
        public double PVBLineb { get; set; }
        public double PVBSumPNR { get; set; }
        public double PVBAbsSum { get; set; }
        public double PVBStdev { get; set; }
        public double PVBVolatility { get; set; }
        public double PVBSDMax { get; set; }
        public D22STimeClass PVBSDMaxTime { get; set; }
        public double PVBSDMaxR { get; set; }
        public int PVBDN1Num { get; set; }
        public int PVBDN2Num { get; set; }
        public int PVBDN3Num { get; set; }
        public int PVBTNum { get; set; }
        public double PVBVMax { get; set; }
        public double PVBVMin { get; set; }
        public double PVBVAvg { get; set; }
        public double PVBStbTR { get; set; }
        public double PVBNoStbTR { get; set; }
        public double PVBStbTSL { get; set; }
        public double PVBStbTSLR { get; set; }
        public double PVBNoStbTSL { get; set; }
        public double PVBNoStbTSLR { get; set; }
        public double PVBUpTSL { get; set; }
        public double PVBUpTSLR { get; set; }
        public double PVBDownTSL { get; set; }
        public double PVBDownTSLR { get; set; }
        public double PVBPNum { get; set; }
        public double PVBQltR { get; set; }
        public string PVBStatus { get; set; }

        public double PVBQa { get; set; }
        public double PVBQb { get; set; }
        public double PVBQc { get; set; }

        public List<D22STimeClass> PVBStbT { get; set; }
        public List<D22STimeClass> PVBNoStbT { get; set; }

        public double PVBStbTSLPV { get; set; }

        public D22STimeClass PVBStbTSLT { get; set; }
        public D22STimeClass PVBNoStbTSLT { get; set; }

        public D22STimeClass PVBUpTSLT { get; set; }
        public D22STimeClass PVBDownTSLT { get; set; }

        public List<D22STimeClass> PVBSDMaxTimeG { get; set; }


    }
}
