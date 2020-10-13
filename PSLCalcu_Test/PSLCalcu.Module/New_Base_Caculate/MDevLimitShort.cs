using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.NewCaculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class MDevLimitShort : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MDevLimitShort";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "偏差型变量运行范围计算";
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
        private string _algorithms = "MDevLimitShort";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "1;2;1;2;1;2;1;L";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "LimitHH;LimitH;LimitRP;LimitOO;LimitRN;LimitL;LimitLL;";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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


        private int _outputNumber = 122;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "DevHLHHLLT;" +
                                        "DevHLHHLLR;" +
                                        "DevHHN;" +
                                        "DevHHTime;" +
                                        "DevHHTimeEnd;" +
                                        "DevHHT;" +
                                        "DevHHR;" +
                                        "DevHHTMax;" +
                                        "DevHHTMaxTime;" +
                                        "DevHHTMaxTimeEnd;" +
                                        "DevHHA;" +
                                        "DevHHET;" +
                                        "DevHN;" +
                                        "DevHTime;" +
                                        "DevHTimeEnd;" +
                                        "DevHT;" +
                                        "DevHR;" +
                                        "DevHTMax;" +
                                        "DevHTMaxTime;" +
                                        "DevHTMaxTimeEnd;" +
                                        "DevHA;" +
                                        "DevHET;" +
                                        "DevRPN;" +
                                        "DevRPTime;" +
                                        "DevRPTimeEnd;" +
                                        "DevRPT;" +
                                        "DevRPR;" +
                                        "DevRPTMax;" +
                                        "DevRPTMaxTime;" +
                                        "DevRPTMaxTimeEnd;" +
                                        "DevRPA;" +
                                        "DevRPET;" +
                                        "Dev0PN;" +
                                        "Dev0PTime;" +
                                        "Dev0PTimeEnd;" +
                                        "Dev0PT;" +
                                        "Dev0PR;" +
                                        "Dev0PTMax;" +
                                        "Dev0PTMaxTime;" +
                                        "Dev0PTMaxTimeEnd;" +
                                        "Dev0PA;" +
                                        "Dev0PET;" +
                                        "Dev0NN;" +
                                        "Dev0NTime;" +
                                        "Dev0NTimeEnd;" +
                                        "Dev0NT;" +
                                        "Dev0NR;" +
                                        "Dev0NTMax;" +
                                        "Dev0NTMaxTime;" +
                                        "Dev0NTMaxTimeEnd;" +
                                        "Dev0NA;" +
                                        "Dev0NET;" +
                                        "DevRNN;" +
                                        "DevRNTime;" +
                                        "DevRNTimeEnd;" +
                                        "DevRNT;" +
                                        "DevRNR;" +
                                        "DevRNTMax;" +
                                        "DevRNTMaxTime;" +
                                        "DevRNTMaxTimeEnd;" +
                                        "DevRNA;" +
                                        "DevRNET;" +
                                        "DevLN;" +
                                        "DevLTime;" +
                                        "DevLTimeEnd;" +
                                        "DevLT;" +
                                        "DevLR;" +
                                        "DevLTMax;" +
                                        "DevLTMaxTime;" +
                                        "DevLTMaxTimeEnd;" +
                                        "DevLA;" +
                                        "DevLET;" +
                                        "DevLLN;" +
                                        "DevLLTime;" +
                                        "DevLLTimeEnd;" +
                                        "DevLLT;" +
                                        "DevLLR;" +
                                        "DevLLTMax;" +
                                        "DevLLTMaxTime;" +
                                        "DevLLTMaxTimeEnd;" +
                                        "DevLLA;" +
                                        "DevLLET;" +
                                        "Dev0HT;" +
                                        "Dev0HTR;" +
                                        "Dev0HHT;" +
                                        "Dev0HHTR;" +
                                        "Dev0L;" +
                                        "Dev0LR;" +
                                        "Dev0LLT;" +
                                        "Dev0LLTR;" +
                                        "DevHHLLT;" +
                                        "DevHHLLTR;" +
                                        "DevRPRMHLT;" +
                                        "DevRPRMHLTR;" +
                                        "Dev0RPRMTTime;" +
                                        "Dev0RPRMTTimeEnd;" +
                                        "Dev0RPRMT;" +
                                        "Dev0RPRMTR;" +
                                        "Dev0RPRMTMax;" +
                                        "Dev0RPRMTMaxTime;" +
                                        "Dev0RPRMTMaxTimeEnd;" +
                                        "DevHLTTime;" +
                                        "DevHLTTimeEnd;" +
                                        "DevHLT;" +
                                        "DevHLTR;" +
                                        "DevHLTMax;" +
                                        "DevHLTMaxTime;" +
                                        "DevHLTMaxTimeEnd;" +
                                        "DevPTTime;" +
                                        "DevPTTimeEnd;" +
                                        "DevPT;" +
                                        "DevPTR;" +
                                        "DevPTRTMax;" +
                                        "DevPTRTMaxTime;" +
                                        "DevPTRTMaxTimeEnd;" +
                                        "DevNTTime;" +
                                        "DevNTTimeEnd;" +
                                        "DevNT;" +
                                        "DevNTR;" +
                                        "DevNTRTMax;" +
                                        "DevNTRTMaxTime;" +
                                        "DevNTRTMaxTimeEnd";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "越限段，在 HH ＞ x ≥ H，L ≥ x ＞ LL 之间总时间长;" +
                                        "在越限段总时间占比;" +
                                        "越 HH 次数，x ≥ HH;" +
                                        "越 HH 时刻 - 进 HH 时刻，≥ HH 算进;" +
                                        "越 HH 时刻 - 出 HH 时刻;" +
                                        "越 HH 总时长，n 周期;" +
                                        "越 HH 时间占比，%;" +
                                        "单次越 HH 最长时间;" +
                                        "单次越 HH 最长起点时刻 - 进 HH 时刻;" +
                                        "单次越 HH 最长起点时刻 - 出 HH 时刻;" +
                                        "越 HH 面积，对 x ≥ HH 的点，求和 (点值 - HH);" +
                                        "等效 DivHHA / DivHHT;" +
                                        "越 H 次数，HH ＞ x ≥ H;" +
                                        "越 H 时刻 - 进、出 H 时刻，≥ H 算进;" +
                                        "越 H 时刻 - 出 H 时刻;" +
                                        "越 H 总时长，n 周期;" +
                                        "越 H 时间占比，%;" +
                                        "单次越 H 最长时间;" +
                                        "单次越 H 最长起点时刻 - 进 H 时刻;" +
                                        "单次越 H 最长起点时刻 - 出 H 时刻;" +
                                        "越 H 面积，对 ，HH ＞ x ≥ H 的点，求和 (点值 - H);" +
                                        "等效 DivHA / DivHT;" +
                                        "越 R+ 次数，H ＞ x ≥ R+;" +
                                        "越 R+ 时刻 - 进、出 R+ 时刻，≥ R+ 算进;" +
                                        "越 R+ 时刻 - 出 R+ 时刻;" +
                                        "越 R+ 总时长，n 周期;" +
                                        "越 R+ 时间占比，%;" +
                                        "单次越 R+ 最长时间;" +
                                        "单次越 R+ 最长起点时刻 - 进 R+ 时刻;" +
                                        "单次越 R+ 最长起点时刻 - 出 R+ 时刻;" +
                                        "越 R+ 面积，对 H ＞ x ≥ R+ 的点，求和 (点值 - R+);" +
                                        "等效 DivRPA / DivRPT;" +
                                        "越 0P 次数，R+ ＞ x ≥ 0;" +
                                        "越 0P 时刻 - 进 0 时刻，≥ 0 算进;" +
                                        "越 0P 时刻 - 出 0 时刻;" +
                                        "越 0P 总时长，n 周期;" +
                                        "越 0P 时间占比，%;" +
                                        "单次越 0P 最长时间;" +
                                        "单次越 0P 最长起点时刻 - 进 0 时刻，≥ 0 算进;" +
                                        "单次越 0P 最长起点时刻 - 出 0 时刻;" +
                                        "越 0P 面积，对 R+ ＞ x ≥ 0 的点，求和 (点值 - 0);" +
                                        "等效 Div0PA / Div0PT;" +
                                        "越 0N 次数，0 ＞ x ＞ R-;" +
                                        "越 0N 时刻 - 进 0 时刻，＜ 0 算进;" +
                                        "越 0N 时刻 - 出 0 时刻;" +
                                        "越 0N 总时长，n 周期;" +
                                        "越 0N 时间占比，%;" +
                                        "单次越 0N 最长时间;" +
                                        "单次越 0N 最长起点时刻 - 进 0 时刻，＜ 0 算进;" +
                                        "单次越 0N 最长起点时刻 - 出 0 时刻;" +
                                        "越 0N 面积，对 0 ＞ x ＞  R- 的点，求和 -(点值 - 0);" +
                                        "等效 Div0NA / Div0NT;" +
                                        "越 RN 次数，R- ≥ x＞  L;" +
                                        "越 RN 时刻 - 进、出 R- 时刻，＜ R- 算进;" +
                                        "越 RN 时刻 - 进、出 R- 时刻;" +
                                        "越 RN 总时长，n 周期;" +
                                        "越 RN 时间占比，%;" +
                                        "单次越 RN 最长时间;" +
                                        "单次越 RN 最长起点时刻 - 进 R- 时刻;" +
                                        "单次越 RN 最长起点时刻 - 出 R- 时刻;" +
                                        "越 RN 面积，对R- ≥ x ＞  L 的点，求和 -(点值 - R-);" +
                                        "等效 DivRNA / DivRNT;" +
                                        "越 L 次数，L ≥ x ＞  LL;" +
                                        "越 L 时刻 - 进 L 时刻，＜ L 算进;" +
                                        "越 L 时刻 - 出 L 时刻;" +
                                        "越 L 总时长，n 周期;" +
                                        "越 L 时间占比，%;" +
                                        "单次越 L 最长时间;" +
                                        "单次越 L 最长起点时刻 - 进 L 时刻;" +
                                        "单次越 L 最长起点时刻 - 出 L 时刻;" +
                                        "越 L 面积，对 L ≥ x ＞  LL 的点，求和 -(点值 - L);" +
                                        "等效 DivLA / DivLT;" +
                                        "越 LL 次数，LL ≥ x;" +
                                        "越 LL 时刻 - 进 LL 时刻，＜ LL 算进;" +
                                        "越 LL 时刻 - 出 LL 时刻;" +
                                        "越 LL 总时长，n 周期;" +
                                        "越 LL 时间占比，%;" +
                                        "单次越 LL 最长时间;" +
                                        "单次越 LL 最长起点时刻 - 进 LL 时刻;" +
                                        "单次越 LL 最长起点时刻 - 出 LL 时刻;" +
                                        "越 LL 面积，对 LL ≥ x 的点，求和 -(点值 - LL);" +
                                        "等效 DivLLA / DivLLT;" +
                                        "正常，正段，在 0 - H 之内时间长，H ＞ x ≥ 0;" +
                                        "在正常，正段之内时间占比;" +
                                        "非超限，且正段，在 0 - HH 之内时间长，HH ＞ x ≥ 0;" +
                                        "在非超限，且正段之内时间占比;" +
                                        "正常，负段，在 0 - L 之内时间长，0 ＞ x ＞ L;" +
                                        "在正常，负段之内时间占比;" +
                                        "非超限，且负段，在 0 - LL 之内时间长，0 ＞ x ＞ LL;" +
                                        "在非超限，且负段之内时间占比;" +
                                        "超限段。在 HH ≤ x，x ≤ LL 之间总时间长;" +
                                        "在超限段总时间占比;" +
                                        "正常，非优秀段，在 H ＞ x ≥ R+，R- ≥ x ＞ L 之间总时间长;" +
                                        "在正常，非优秀段总时间占比;" +
                                        "优秀段时刻，在 R+ ＞ x ＞ R- 时刻 - 进 R± 时刻;" +
                                        "优秀段时刻，在 R+ ＞ x ＞ R- 时刻 - 出 R± 时刻;" +
                                        "优秀段，在 R+ ＞ x ＞ R- 之间总时间长;" +
                                        "在优秀段总时间占比;" +
                                        "单次在优秀段最长时间，R+ ＞ x ＞ R-;" +
                                        "单次在优秀段最长起点时刻 - 进 R± 时刻;" +
                                        "单次在优秀段最长起点时刻 - 出 R± 时刻;" +
                                        "在正常段时刻，在 H ＞ x ＞ L 时刻 - 进 H 到 L 时刻;" +
                                        "在正常段时刻，在 H ＞ x ＞ L 时刻 - 出 H 到 L 时刻;" +
                                        "正常段，在 H ＞ x ＞ L 之间总时间长;" +
                                        "在正常段总时间占比;" +
                                        "单次在正常段最长时间;" +
                                        "单次在正常段最长时间起点时刻 - 单次最长，进 进 H 到 L 时刻;" +
                                        "单次在正常段最长时间起点时刻 - 单次最长，出 进 H 到 L 时刻;" +
                                        "正向时刻，在 x ≥ 0 时刻 - 进、出 X ≥ 0 时刻，X ≥ 0 算进;" +
                                        "正向时刻，在 x ≥ 0 时刻 - 出 X ≥ 0 时刻;" +
                                        "正向段，x ≥ 0 时间长;" +
                                        "正向段，x ≥ 0 时间占比;" +
                                        "单次正向段最长持续时间;" +
                                        "单次正向段最长持续时间起点时刻 - 单次最长，进 x ＜ 0 时刻;" +
                                        "单次正向段最长持续时间起点时刻 - 单次最长，出 x ＜ 0 时刻;" +
                                        "正向时刻求反，且数据正常 - TimeStart;" +
                                        "正向时刻求反，且数据正常 - TimeEnd;" +
                                        "负向段，x < 0 时间长;" +
                                        "负向段，x < 0 时间占比;" +
                                        "单次负向段最长持续时间;" +
                                        "单次负向段最长持续时间起点时刻 - 单次最长，进 x ＜ 0 时刻;" +
                                        "单次负向段最长持续时间起点时刻 - 单次最长，出 x ＜ 0 时刻";

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
        public static Results Calcu()
        {
            return Calcu(_inputData, _calcuInfo);
        }

        public static Results Calcu(List<PValue>[] inputs, CalcuInfo calcuinfo)
        {
            bool _errorFlag = false;
            string _errorInfo = "";
            bool _warningFlag = false;
            string _warningInfo = "";
            bool _fatalFlag = false;
            string _fatalInfo = "";

            int i;

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[122];   //最大值（快速）;最小值（快速）;极差（快速）;算术和（快速）;算术平均值（快速）
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

                double LimitHH, LimitH, LimitRP, LimitOO, LimitRN, LimitL, LimitLL;
                string mode;
                string[] paras = calcuinfo.fparas.Split(';');

                LimitHH = int.Parse(paras[0]);
                LimitH = int.Parse(paras[1]);
                LimitRP = double.Parse(paras[2]);
                LimitOO = double.Parse(paras[3]);
                LimitRN = double.Parse(paras[4]);
                LimitL = double.Parse(paras[5]);
                LimitLL = double.Parse(paras[6]);
                string type = calcuinfo.fsourtagids[0].ToString();
                #region 短周期计算


                //将input 装化成 List<MPVBaseMessageInBadClass>
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

                MDevLimitMessageOutBadClass res = MDevLimitCaculate.shortMDevLimit(valueList, LimitHH, LimitH, LimitRP, LimitOO, LimitRN, LimitL, LimitLL);
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
                bool isok = BLL.AlgorithmBLL.insertMDevLimit(res, type, year, month, day, hour);
                if (isok)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MDevLimit短周期数据录入数据库是失败";
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
