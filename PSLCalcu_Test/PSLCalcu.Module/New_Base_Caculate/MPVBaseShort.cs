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
    public class MPVBaseShort : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "NewMPVBaseShort";
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
        private string _algorithms = "NewMPVBaseShort";
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
                //tagId
                string type = calcuinfo.fsourtagids[0].ToString();

                #region 短周期计算

                //短周期算法
                //将输入input(分钟数据） 转化成 List<MPVBaseMessageInBadClass>
                List<MPVBaseMessageInBadClass> valueList = new List<MPVBaseMessageInBadClass>();
                valueList = Helper.AlgorithmHelper.transformValue(input);
                #endregion

                #region 短周期算法输出
                //
                //调用短周期算法数据是把每分钟的数据结果进行处理 小时级别时间数据运算
                MPVBaseMessageOutBadClass res = MPVBaseCaculate.shortMPVBase(valueList, N1, N2, N3, k, b, stbl, nostbl);
                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                string hour = string.Empty;
                DateTime dt = Convert.ToDateTime(valueList[valueList.Count - 1].valueDate);
                year = dt.Year.ToString();
                month = dt.Month.ToString();
                day = dt.Day.ToString();
                hour = dt.Hour.ToString();
                //计算结果存入数据库
                bool isok = BLL.AlgorithmBLL.insertMPVBase(res, type, year, month, day, hour);
                if (isok)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MPVBase短周期数据录入数据库是失败";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }


                #endregion






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


}
