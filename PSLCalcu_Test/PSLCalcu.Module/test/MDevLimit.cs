using PCCommon;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Model;
using PSLCalcu.Module.Limit;


namespace PSLCalcu.Module
{
    class MDevLimit : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MDevLimit";
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
        private string _algorithms = "MDevLimit";
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
        private string _outputDescs =   "DevHLHHLLT;" +
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
         #region 计算模块算法
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
                mode= paras[7];
                if (paras.Length == 8)
                    mode = paras[7];   //如果设定了第8个参数，计算模式用第个参数值。S表示短周期，L表示长周期                
                else
                    mode = "S";
                if (mode == "S")
                {
                 #region 短周期计算


                    //将input 装化成 List<MPVBaseMessageInBadClass>
                    List<MPVBaseMessageInBadClass> valueList = new List<MPVBaseMessageInBadClass>();
                    for (int j = 0; j < input.Count; j++) {
                        MPVBaseMessageInBadClass v = new MPVBaseMessageInBadClass();
                        v.seq = j;
                        string time = input[j].Timestamp.ToShortTimeString();
                        string time2 = input[j].Timestamp.ToString();
                        v.valueDate = time2;
                        v.valueAmount = input[j].Value.ToString();
                        valueList.Add(v);
                    }

                    MDevLimitMessageOutBadClass res = Limits.shortMDevLimit(valueList, LimitHH, LimitH, LimitRP, LimitOO, LimitRN, LimitL, LimitLL);
                    ///DevHLHHLLT	越限段，在 HH ＞ X ≥ H，L ≥ X ＞ LL 之间总时间长
                    if (null != res.DevHLHHLLT)
                    {
                        results[0] = new List<PValue>();
                        PValue DevHLHHLLT = new PValue(Double.Parse(res.DevHLHHLLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[0].Add(DevHLHHLLT);
                    }
                    ///DevHLHHLLR	在越限段总时间占比
                    if (null != res.DevHLHHLLR)
                    {
                        results[1] = new List<PValue>();
                        PValue DevHLHHLLR = new PValue(Double.Parse(res.DevHLHHLLR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[1].Add(DevHLHHLLR);
                    }
                    ///DevHHN	越 HH 次数，X ≥ HH
                    if (null != res.DevHHN) {
                        results[2] = new List<PValue>();
                        PValue DevHHN = new PValue(Double.Parse(res.DevHHN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[2].Add(DevHHN);
                    }
                    ///DevHHTime	越 HH 时刻 - 进、出 HH 时刻，≥ HH 算进
                    if (null != res.DevHHTime) {
                        results[3] = new List<PValue>();
                        results[4] = new List<PValue>();
                        //results[5] = new List<PValue>();
                        int count = res.DevHHTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevHHTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevHHTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[3].Add(DevHHTime);

                            DateTime endTime = Convert.ToDateTime(res.DevHHTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevHHTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[4].Add(DevHHTimeEnd);

                            //PValue DevHHTimeVC = new PValue(res.DevHHTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                           // results[5].Add(DevHHTimeVC);
                        }
                    }
                    ///DevHHT	越 HH 总时长，N 周期
                    if (null != res.DevHHT) {
                        results[5] = new List<PValue>();
                        PValue DevHHT = new PValue(Double.Parse(res.DevHHT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[5].Add(DevHHT);
                    }
                    ///DevHHR	越 HH 时间占比，%
                    if (null != res.DevHHR) {
                        results[6] = new List<PValue>();
                        PValue DevHHR = new PValue(Double.Parse(res.DevHHR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[6].Add(DevHHR);
                    }
                    ///DevHHTMax	单次越 HH 最长时间
                    if (null != res.DevHHTMax) {
                        results[7] = new List<PValue>();
                        //DateTime time = Convert.ToDateTime(res.DevHHTMax);
                        //double value = time.Ticks;
                        PValue DevHHTMax = new PValue(Double.Parse(res.DevHHTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[7].Add(DevHHTMax);
                    }
                    ///DevHHTMaxTime	单次越 HH 最长起点时刻 - 进、出 HH 时刻
                    if (null != res.DevHHTMaxTime) {
                        results[8] = new List<PValue>();
                        results[9] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevHHTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevHHTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[8].Add(DevHHTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevHHTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevHHTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[9].Add(DevHHTMaxTimeEnd);
                    }
                    ///DevHHA	越 HH 面积，对 X ≥ HH 的点，求和 (点值 - HH)
                    if (null != res.DevHHA) {
                        results[10] = new List<PValue>();
                        PValue DevHHA = new PValue(Double.Parse(res.DevHHA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[10].Add(DevHHA);
                    }
                    ///DevHHET	等效 DIVHHA / DIVHHT
                    if (null != res.DevHHET) {
                        results[11] = new List<PValue>();
                        PValue DevHHET = new PValue(Double.Parse(res.DevHHET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[11].Add(DevHHET);
                    }
                    ///DevHN	越 H 次数，HH ＞ X ≥ H
                    if (null != res.DevHN) {
                        results[12] = new List<PValue>();
                        PValue DevHN = new PValue(Double.Parse(res.DevHN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[12].Add(DevHN);
                    }
                    ///DevHTime	越 H 时刻 - 进、出 H 时刻，≥ H 算进
                    if (null != res.DevHTime) {
                        results[13] = new List<PValue>();
                        results[14] = new List<PValue>();
                        //results[16] = new List<PValue>();
                        int count = res.DevHTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime time = Convert.ToDateTime(res.DevHTime[j].startDate);
                            double value = time.Ticks;
                            PValue DevHTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[13].Add(DevHTime);

                            DateTime endTime = Convert.ToDateTime(res.DevHTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevHTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[14].Add(DevHTimeEnd);

                            //PValue DevHTimeVC = new PValue(res.DevHTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[16].Add(DevHTimeVC);
                        }
                    }
                    ///DevHT	越 H 总时长，N 周期
                    if (null != res.DevHT && res.DevHT.Contains("无穷大") == false) {
                        results[15] = new List<PValue>();
                        PValue DevHT = new PValue(Double.Parse(res.DevHT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[15].Add(DevHT);
                    }
                    ///DevHR	越 H 时间占比，%
                    if (null != res.DevHR) {
                        results[16] = new List<PValue>();
                        PValue DevHR = new PValue(Double.Parse(res.DevHR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[16].Add(DevHR);
                    }
                    ///DevHTMax	单次越 H 最长时间
                    if (null != res.DevHTMax) {
                        results[17] = new List<PValue>();
                        PValue DevHTMax = new PValue(Double.Parse(res.DevHTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[17].Add(DevHTMax);
                    }
                    ///DevHTMaxTime	单次越 H 最长起点时刻 - 进、出 H 时刻
                    if (null != res.DevHTMaxTime) {
                        results[18] = new List<PValue>();
                        results[19] = new List<PValue>();
                        DateTime time = Convert.ToDateTime(res.DevHTMaxTime.startDate);
                        double value = time.Ticks;
                        PValue DevHTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[18].Add(DevHTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevHTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevHTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[19].Add(DevHTMaxTimeEnd);
                    }
                    ///DevHA	越 H 面积，对 ，HH ＞ X ≥ H 的点，求和 (点值 - H)
                    if (null != res.DevHA) {
                        results[20] = new List<PValue>();
                        PValue DevHA = new PValue(Double.Parse(res.DevHA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[20].Add(DevHA);
                    }
                    ///DevHET	等效 DIVHA / DIVHT
                    if (null != res.DevHET) {
                        results[21] = new List<PValue>();
                        PValue DevHET = new PValue(Double.Parse(res.DevHET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[21].Add(DevHET);
                    }
                    ///DevRPN	越 R+ 次数，H ＞ X ≥ R+
                    if (null != res.DevRPN) {
                        results[22] = new List<PValue>();
                        PValue DevRPN = new PValue(Double.Parse(res.DevRPN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[22].Add(DevRPN);
                    }
                    ///DevRPTime	越 R+ 时刻 - 进、出 R+ 时刻，≥ R+ 算进
                    if (null != res.DevRPTime) {
                        results[23] = new List<PValue>();
                        results[24] = new List<PValue>();
                        //results[27] = new List<PValue>();
                        int count = res.DevRPTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevRPTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevRPTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[23].Add(DevRPTime);

                            DateTime endTime = Convert.ToDateTime(res.DevRPTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevRPTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[24].Add(DevRPTimeEnd);

                            //PValue DevRPTimeVC = new PValue(res.DevRPTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[27].Add(DevRPTimeVC);
                        }
                    }
                    ///DevRPT	越 R+ 总时长，N 周期
                    if (null != res.DevRPT) {
                        results[25] = new List<PValue>();
                        PValue DevRPT = new PValue(Double.Parse(res.DevRPT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[25].Add(DevRPT);
                    }
                    ///DevRPR	越 R+ 时间占比，%
                    if (null != res.DevRPR) {
                        results[26] = new List<PValue>();
                        PValue DevRPR = new PValue(Double.Parse(res.DevRPR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[26].Add(DevRPR);
                    }
                    ///DevRPTMax	单次越 R+ 最长时间
                    if (null != res.DevRPTMax) {
                        results[27] = new List<PValue>();
                        PValue DevRPTMax = new PValue(Double.Parse(res.DevRPTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[27].Add(DevRPTMax);
                    }
                    ///DevRPTMaxTime	单次越 R+ 最长起点时刻 - 进、出 R+ 时刻
                    if (null != res.DevRPTMaxTime) {
                        results[28] = new List<PValue>();
                        results[29] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevRPTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevRPTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[28].Add(DevRPTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevRPTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevRPTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[29].Add(DevRPTMaxTimeEnd);
                    }
                    ///DevRPA	越 R+ 面积，对 H ＞ X ≥ R+ 的点，求和 (点值 - R+)
                    if (null != res.DevRPA) {
                        results[30] = new List<PValue>();
                        PValue DevRPA = new PValue(Double.Parse(res.DevRPA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[30].Add(DevRPA);
                    }
                    ///DevRPET	等效 DIVRPA / DIVRPT
                    if (null != res.DevRPET) {
                        results[31] = new List<PValue>();
                        PValue DevRPET = new PValue(Double.Parse(res.DevRPET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[31].Add(DevRPET);
                    }
                    ///Dev0PN	越 0P 次数，R+ ＞ X ≥ 0
                    if (null != res.Dev0PN)
                    {
                        results[32] = new List<PValue>();
                        PValue Dev0PN = new PValue(Double.Parse(res.Dev0PN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[32].Add(Dev0PN);
                    }
                    ///Dev0PTime	越 0P 时刻 - 进、出 0 时刻，≥ 0 算进
                    if (null != res.Dev0PTime) {
                        results[33] = new List<PValue>();
                        results[34] = new List<PValue>();
                        //results[38] = new List<PValue>();
                        int count = res.Dev0PTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.Dev0PTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue Dev0PTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[33].Add(Dev0PTime);

                            DateTime endTime = Convert.ToDateTime(res.Dev0PTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue Dev0PTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[34].Add(Dev0PTimeEnd);

                            //PValue Dev0PTimeVC = new PValue(res.Dev0PTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[38].Add(Dev0PTimeVC);
                        }
                    }
                    ///Dev0PT	越 0P 总时长，N 周期
                    if (null != res.Dev0PT) {
                        results[35] = new List<PValue>();
                        PValue Dev0PT = new PValue(Double.Parse(res.Dev0PT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[35].Add(Dev0PT);
                    }
                    ///Dev0PR	越 0P 时间占比，%
                    if (null != res.Dev0PR) {
                        results[36] = new List<PValue>();
                        PValue Dev0PR = new PValue(Double.Parse(res.Dev0PR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[36].Add(Dev0PR);
                    }
                    ///Dev0PTMax	单次越 0P 最长时间
                    if (null != res.Dev0PTMax) {
                        results[37] = new List<PValue>();
                        PValue Dev0PTMax = new PValue(Double.Parse(res.Dev0PTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[37].Add(Dev0PTMax);
                    }
                    ///Dev0PTMaxTime	单次越 0P 最长起点时刻 - 进、出 0 时刻，≥ 0 算进
                    if (null != res.Dev0PTMaxTime) {
                        results[38] = new List<PValue>();
                        results[39] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.Dev0PTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue Dev0PTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[38].Add(Dev0PTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.Dev0PTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue Dev0PTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[39].Add(Dev0PTMaxTimeEnd);
                    }
                    ///Dev0PA	越 0P 面积，对 R+ ＞ X ≥ 0 的点，求和 (点值 - 0)
                    if (null != res.Dev0PA) {
                        results[40] = new List<PValue>();
                        PValue Dev0PA = new PValue(Double.Parse(res.Dev0PA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[40].Add(Dev0PA);
                    }
                    ///Dev0PET	等效 DIV0PA / DIV0PT
                    if (null != res.Dev0PET) {
                        results[41] = new List<PValue>();
                        PValue Dev0PET = new PValue(Double.Parse(res.Dev0PET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[41].Add(Dev0PET);
                    }
                    ///Dev0NN	越 0N 次数，0 ＞ X ＞ R-
                    if (null != res.Dev0NN) {
                        results[42] = new List<PValue>();
                        PValue Dev0NN = new PValue(Double.Parse(res.Dev0NN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[42].Add(Dev0NN);
                    }
                    ///Dev0NTime	越 0N 时刻 - 进、出 0 时刻，＜ 0 算进
                    if (null != res.Dev0NTime) {
                        results[43] = new List<PValue>();
                        results[44] = new List<PValue>();
                        //results[49] = new List<PValue>();
                        int count = res.Dev0NTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.Dev0NTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue Dev0NTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[43].Add(Dev0NTime);

                            DateTime endTime = Convert.ToDateTime(res.Dev0NTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue Dev0NTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[44].Add(Dev0NTimeEnd);

                            //PValue Dev0NTimeVC = new PValue(res.Dev0NTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[49].Add(Dev0NTimeVC);
                        }
                    }
                    ///Dev0NT	越 0N 总时长，N 周期
                    if (null != res.Dev0NT) {
                        results[45] = new List<PValue>();
                        PValue Dev0NT = new PValue(Double.Parse(res.Dev0NT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[45].Add(Dev0NT);
                    }
                    ///Dev0NR	越 0N 时间占比，%
                    if (null != res.Dev0NR) {
                        results[46] = new List<PValue>();
                        PValue Dev0NR = new PValue(Double.Parse(res.Dev0NR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[46].Add(Dev0NR);
                    }
                    ///Dev0NTMax	单次越 0N 最长时间
                    if (null != res.Dev0NTMax) {
                        results[47] = new List<PValue>();
                        PValue Dev0NTMax = new PValue(Double.Parse(res.Dev0NTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[47].Add(Dev0NTMax);
                    }
                    ///Dev0NTMaxTime	单次越 0N 最长起点时刻 - 进、出 0 时刻，＜ 0 算进
                    if (null != res.Dev0NTMaxTime) {
                        results[48] = new List<PValue>();
                        results[49] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.Dev0NTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue Dev0NTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[48].Add(Dev0NTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.Dev0NTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue Dev0NTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[49].Add(Dev0NTMaxTimeEnd);
                    }
                    ///Dev0NA	越 0N 面积，对 0 ＞ X ＞  R- 的点，求和 -(点值 - 0)
                    if (null != res.Dev0NA) {
                        results[50] = new List<PValue>();
                        PValue Dev0NA = new PValue(Double.Parse(res.Dev0NA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[50].Add(Dev0NA);
                    }
                    ///Dev0NET	等效 DIV0NA / DIV0NT
                    if (null != res.Dev0NET) {
                        results[51] = new List<PValue>();
                        PValue Dev0NET = new PValue(Double.Parse(res.Dev0NET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[51].Add(Dev0NET);
                    }
                    ///DevRNN	越 RN 次数，R- ≥ X＞  L
                    if (null != res.DevRNN) {
                        results[52] = new List<PValue>();
                        PValue DevRNN = new PValue(Double.Parse(res.DevRNN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[52].Add(DevRNN);
                    }
                    ///DevRNTime	越 RN 时刻 - 进、出 R- 时刻，＜ R- 算进
                    if (null != res.DevRNTime) {
                        results[53] = new List<PValue>();
                        results[54] = new List<PValue>();
                        //results[60] = new List<PValue>();
                        int count = res.DevRNTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevRNTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevRNTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[53].Add(DevRNTime);

                            DateTime endTime = Convert.ToDateTime(res.DevRNTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevRNTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[54].Add(DevRNTimeEnd);

                            //PValue DevRNTimeVC = new PValue(res.DevRNTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[60].Add(DevRNTimeVC);
                        }
                    }
                    ///DevRNT	越 RN 总时长，N 周期
                    if (null != res.DevRNT) {
                        results[55] = new List<PValue>();
                        PValue DevRNT = new PValue(Double.Parse(res.DevRNT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[55].Add(DevRNT);
                    }
                    ///DevRNR	越 RN 时间占比，%
                    if (null != res.DevRNR) {
                        results[56] = new List<PValue>();
                        PValue DevRNR = new PValue(Double.Parse(res.DevRNR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[56].Add(DevRNR);
                    }
                    ///DevRNTMax	单次越 RN 最长时间
                    if (null != res.DevRNTMax) {
                        results[57] = new List<PValue>();
                        PValue DevRNTMax = new PValue(Double.Parse(res.DevRNTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[57].Add(DevRNTMax);
                    }
                    ///DevRNTMaxTime	单次越 RN 最长起点时刻 - 进、出 R- 时刻
                    if (null != res.DevRNTMaxTime) {
                        results[58] = new List<PValue>();
                        results[59] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevRNTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevRNTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[58].Add(DevRNTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevRNTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevRNTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[59].Add(DevRNTMaxTimeEnd);
                    }
                    ///DevRNA	越 RN 面积，对R- ≥ X ＞  L 的点，求和 -(点值 - R-)
                    if (null != res.DevRNA) {
                        results[60] = new List<PValue>();
                        PValue DevRNA = new PValue(Double.Parse(res.DevRNA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[60].Add(DevRNA);
                    }
                    ///DevRNET	等效 DIVRNA / DIVRNT
                    if (null != res.DevRNET) {
                        results[61] = new List<PValue>();
                        PValue DevRNET = new PValue(Double.Parse(res.DevRNET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[61].Add(DevRNET);
                    }
                    ///DevLN	越 L 次数，L ≥ X ＞  LL
                    if (null != res.DevLN) {
                        results[62] = new List<PValue>();
                        PValue DevLN = new PValue(Double.Parse(res.DevLN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[62].Add(DevLN);
                    }
                    ///DevLTime	越 L 时刻 - 进、出 L 时刻，＜ L 算进
                    if (null != res.DevLTime) {
                        results[63] = new List<PValue>();
                        results[64] = new List<PValue>();
                        //results[71] = new List<PValue>();
                        int count = res.DevLTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevLTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevLTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[63].Add(DevLTime);

                            DateTime endTime = Convert.ToDateTime(res.DevLTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevLTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[64].Add(DevLTimeEnd);

                            //PValue DevLTimeVC = new PValue(res.DevLTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[71].Add(DevLTimeVC);
                        }
                    }
                    ///DevLT	越 L 总时长，N 周期
                    if (null != res.DevLT) {
                        results[65] = new List<PValue>();
                        PValue DevLT = new PValue(Double.Parse(res.DevLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[65].Add(DevLT);
                    }
                    ///DevLR	越 L 时间占比，%
                    if (null != res.DevLR) {
                        results[66] = new List<PValue>();
                        PValue DevLR = new PValue(Double.Parse(res.DevLR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[66].Add(DevLR);
                    }
                    ///DevLTMax	单次越 L 最长时间
                    if (null != res.DevLTMax) {
                        results[67] = new List<PValue>();
                        PValue DevLTMax = new PValue(Double.Parse(res.DevLTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[67].Add(DevLTMax);
                    }
                    ///DevLTMaxTime	单次越 L 最长起点时刻 - 进、出 L 时刻
                    if (null != res.DevLTMaxTime) {
                        results[68] = new List<PValue>();
                        results[69] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevLTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevLTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[68].Add(DevLTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevLTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevLTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[69].Add(DevLTMaxTimeEnd);
                    }
                    ///DevLA	越 L 面积，对 L ≥ X ＞  LL 的点，求和 -(点值 - L)
                    if (null != res.DevLA) {
                        results[70] = new List<PValue>();
                        PValue DevLA = new PValue(Double.Parse(res.DevLA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[70].Add(DevLA);
                    }
                    ///DevLET	等效 DIVLA / DIVLT
                    if (null != res.DevLET) {
                        results[71] = new List<PValue>();
                        PValue DevLET = new PValue(Double.Parse(res.DevLET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[71].Add(DevLET);
                    }
                    ///DevLLN	越 LL 次数，LL ≥ X
                    if (null != res.DevLLN) {
                        results[72] = new List<PValue>();
                        PValue DevLLN = new PValue(Double.Parse(res.DevLLN), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[72].Add(DevLLN);
                    }
                    ///DevLLTime	越 LL 时刻 - 进、出 LL 时刻，＜ LL 算进
                    if (null != res.DevLLTime) {
                        results[73] = new List<PValue>();
                        results[74] = new List<PValue>();
                        //results[82] = new List<PValue>();
                        int count = res.DevLLTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevLLTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevLLTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[73].Add(DevLLTime);

                            DateTime endTime = Convert.ToDateTime(res.DevLLTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevLLTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[74].Add(DevLLTimeEnd);

                            //PValue DevLLTimeVC = new PValue(res.DevLLTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[82].Add(DevLLTimeVC);
                        }
                    }
                    ///DevLLT	越 LL 总时长，N 周期
                    if (null != res.DevLLT) {
                        results[75] = new List<PValue>();
                        PValue DevLLT = new PValue(Double.Parse(res.DevLLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[75].Add(DevLLT);
                    }
                    ///DevLLR	越 LL 时间占比，%
                    if (null != res.DevLLR) {
                        results[76] = new List<PValue>();
                        PValue DevLLR = new PValue(Double.Parse(res.DevLLR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[76].Add(DevLLR);
                    }
                    ///DevLLTMax	单次越 LL 最长时间
                    if (null != res.DevLLTMax) {
                        results[77] = new List<PValue>();
                        PValue DevLLTMax = new PValue(Double.Parse(res.DevLLTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[77].Add(DevLLTMax);
                    }
                    ///DevLLTMaxTime	单次越 LL 最长起点时刻 - 进、出 LL 时刻
                    if (null != res.DevLLTMaxTime) {
                        results[78] = new List<PValue>();
                        results[79] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevLLTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevLLTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[78].Add(DevLLTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevLLTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevLLTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[79].Add(DevLLTMaxTimeEnd);
                    }
                    ///DevLLA	越 LL 面积，对 LL ≥ X 的点，求和 -(点值 - LL)
                    if (null != res.DevLLA) {
                        results[80] = new List<PValue>();
                        PValue DevLLA = new PValue(Double.Parse(res.DevLLA), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[80].Add(DevLLA);
                    }
                    ///DevLLET	等效 DIVLLA / DIVLLT
                    if (null != res.DevLLET) {
                        results[81] = new List<PValue>();
                        PValue DevLLET = new PValue(Double.Parse(res.DevLLET), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[81].Add(DevLLET);
                    }
                    ///Dev0HT	正常，正段，在 0 - H 之内时间长，H ＞ X ≥ 0
                    if (null != res.Dev0HT) {
                        results[82] = new List<PValue>();
                        PValue Dev0HT = new PValue(Double.Parse(res.Dev0HT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[82].Add(Dev0HT);
                    }
                    ///Dev0HTR	在正常，正段之内时间占比
                    if (null != res.Dev0HTR) {
                        results[83] = new List<PValue>();
                        PValue Dev0HTR = new PValue(Double.Parse(res.Dev0HTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[83].Add(Dev0HTR);
                    }
                    ///Dev0HHT	非超限，且正段，在 0 - HH 之内时间长，HH ＞ X ≥ 0
                    if (null != res.Dev0HHT) {
                        results[84] = new List<PValue>();
                        PValue Dev0HHT = new PValue(Double.Parse(res.Dev0HHT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[84].Add(Dev0HHT);
                    }
                    ///Dev0HHTR	在非超限，且正段之内时间占比
                    if (null != res.Dev0HHTR) {
                        results[85] = new List<PValue>();
                        PValue Dev0HHTR = new PValue(Double.Parse(res.Dev0HHTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[85].Add(Dev0HHTR);
                    }
                    ///Dev0L	正常，负段，在 0 - L 之内时间长，0 ＞ X ＞ L
                    if (null != res.Dev0L) {
                        results[86] = new List<PValue>();
                        PValue Dev0L = new PValue(Double.Parse(res.Dev0L), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[86].Add(Dev0L);
                    }
                    ///Dev0LR	在正常，负段之内时间占比
                    if (null != res.Dev0LR) {
                        results[87] = new List<PValue>();
                        PValue Dev0LR = new PValue(Double.Parse(res.Dev0LR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[87].Add(Dev0LR);
                    }
                    ///Dev0LLT	非超限，且负段，在 0 - LL 之内时间长，0 ＞ X ＞ LL
                    if (null != res.Dev0LLT) {
                        results[88] = new List<PValue>();
                        PValue Dev0LLT = new PValue(Double.Parse(res.Dev0LLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[88].Add(Dev0LLT);
                    }
                    ///Dev0LLTR	在非超限，且负段之内时间占比
                    if (null != res.Dev0LLTR) {
                        results[89] = new List<PValue>();
                        PValue Dev0LLTR = new PValue(Double.Parse(res.Dev0LLTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[89].Add(Dev0LLTR);
                    }
                    ///DevHHLLT	超限段。在 HH ≤ X，X ≤ LL 之间总时间长
                    if (null != res.DevHHLLT) {
                        results[90] = new List<PValue>();
                        PValue DevHHLLT = new PValue(Double.Parse(res.DevHHLLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[90].Add(DevHHLLT);
                    }
                    ///DevHHLLTR	在超限段总时间占比
                    if (null != res.DevHHLLTR) {
                        results[91] = new List<PValue>();
                        PValue DevHHLLTR = new PValue(Double.Parse(res.DevHHLLTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[91].Add(DevHHLLTR);
                    }
                    ///DevRPRMHLT	正常，非优秀段，在 H ＞ X ≥ R+，R- ≥ X ＞ L 之间总时间长
                    if (null != res.DevRPRMHLT) {
                        results[92] = new List<PValue>();
                        PValue DevRPRMHLT = new PValue(Double.Parse(res.DevRPRMHLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[92].Add(DevRPRMHLT);
                    }
                    ///DevRPRMHLTR	在正常，非优秀段总时间占比
                    if (null != res.DevRPRMHLTR) {
                        results[93] = new List<PValue>();
                        PValue DevRPRMHLTR = new PValue(Double.Parse(res.DevRPRMHLTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[93].Add(DevRPRMHLTR);
                    }
                    ///Dev0RPRMTTime	优秀段时刻，在 R+ ＞ X ＞ R- 时刻 - 进、出 R± 时刻
                    if (null != res.Dev0RPRMTTime) {
                        results[94] = new List<PValue>();
                        results[95] = new List<PValue>();
                        //results[104] = new List<PValue>();
                        int count = res.Dev0RPRMTTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.Dev0RPRMTTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue Dev0RPRMTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[94].Add(Dev0RPRMTTime);

                            DateTime endTime = Convert.ToDateTime(res.Dev0RPRMTTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue Dev0RPRMTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[95].Add(Dev0RPRMTTimeEnd);

                            //PValue Dev0RPRMTTimeVC = new PValue(res.Dev0RPRMTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[104].Add(Dev0RPRMTTimeVC);
                        }
                    }
                    ///Dev0RPRMT	优秀段，在 R+ ＞ X ＞ R- 之间总时间长
                    if (null != res.Dev0RPRMT) {
                        results[96] = new List<PValue>();
                        PValue Dev0RPRMT = new PValue(Double.Parse(res.Dev0RPRMT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[96].Add(Dev0RPRMT);
                    }
                    ///Dev0RPRMTR	在优秀段总时间占比
                    if (null != res.Dev0RPRMTR) {
                        results[97] = new List<PValue>();
                        PValue Dev0RPRMTR = new PValue(Double.Parse(res.Dev0RPRMTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[97].Add(Dev0RPRMTR);
                    }
                    ///Dev0RPRMTMax	单次在优秀段最长时间，R+ ＞ X ＞ R-
                    if (null != res.Dev0RPRMTMax) {
                        results[98] = new List<PValue>();
                        PValue Dev0RPRMTMax = new PValue(Double.Parse(res.Dev0RPRMTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[98].Add(Dev0RPRMTMax);
                    }
                    ///Dev0RPRMTMaxTime	单次在优秀段最长起点时刻 - 进、出 R± 时刻
                    if (null != res.Dev0RPRMTMaxTime) {
                        results[99] = new List<PValue>();
                        results[100] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.Dev0RPRMTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue Dev0RPRMTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[99].Add(Dev0RPRMTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.Dev0RPRMTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue Dev0RPRMTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[100].Add(Dev0RPRMTMaxTimeEnd);
                    }
                    ///DevHLTTime	在正常段时刻，在 H ＞ X ＞ L 时刻 - 进、出 H 到 L 时刻
                    if (null != res.DevHLTTime) {
                        results[101] = new List<PValue>();
                        results[102] = new List<PValue>();
                        //results[112] = new List<PValue>();
                        int count = res.DevHLTTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevHLTTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevHLTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[101].Add(DevHLTTime);

                            DateTime endTime = Convert.ToDateTime(res.DevHLTTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevHLTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[102].Add(DevHLTTimeEnd);

                            //PValue DevHLTTimeVC = new PValue(res.DevHLTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[112].Add(DevHLTTimeVC);
                        }
                    }
                    ///DevHLT	正常段，在 H ＞ X ＞ L 之间总时间长
                    if (null != res.DevHLT) {
                        results[103] = new List<PValue>();
                        PValue DevHLT = new PValue(Double.Parse(res.DevHLT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[103].Add(DevHLT);
                    }
                    ///DevHLTR	在正常段总时间占比
                    if (null != res.DevHLTR) {
                        results[104] = new List<PValue>();
                        PValue DevHLTR = new PValue(Double.Parse(res.DevHLTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[104].Add(DevHLTR);
                    }
                    ///DevHLTMax	单次在正常段最长时间
                    if (null != res.DevHLTMax) {
                        results[105] = new List<PValue>();
                        PValue DevHLTMax = new PValue(Double.Parse(res.DevHLTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[105].Add(DevHLTMax);
                    }
                    ///DevHLTMaxTime	单次在正常段最长时间起点时刻 - 单次最长，进、出 H 到 L 时刻
                    if (null != res.DevHLTMaxTime) {
                        results[106] = new List<PValue>();
                        results[107] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevHLTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevHLTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[106].Add(DevHLTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevHLTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevHLTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[107].Add(DevHLTMaxTimeEnd);
                    }
                    ///DevPTTime	正向时刻，在 X ≥ 0 时刻 - 进、出 X ≥ 0 时刻，X ≥ 0 算进
                    if (null != res.DevPTTime) {
                        results[108] = new List<PValue>();
                        results[109] = new List<PValue>();
                        //results[120] = new List<PValue>();
                        int count = res.DevPTTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevPTTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevPTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[108].Add(DevPTTime);

                            DateTime endTime = Convert.ToDateTime(res.DevPTTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevPTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[109].Add(DevPTTimeEnd);

                            //PValue DevPTTimeVC = new PValue(res.DevPTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[120].Add(DevPTTimeVC);
                        }
                    }
                    ///DevPT	正向段，X ≥ 0 时间长
                    if (null != res.DevPT) {
                        results[110] = new List<PValue>();
                        PValue DevPT = new PValue(Double.Parse(res.DevPT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[110].Add(DevPT);
                    }
                    ///DevPTR	正向段，X ≥ 0 时间占比
                    if (null != res.DevPTR) {
                        results[111] = new List<PValue>();
                        PValue DevPTR = new PValue(Double.Parse(res.DevPTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[111].Add(DevPTR);
                    }
                    ///DevPTRTMax	单次正向段最长持续时间
                    if (null != res.DevPTRTMax) {
                        results[112] = new List<PValue>();
                        PValue DevPTRTMax = new PValue(Double.Parse(res.DevPTRTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[112].Add(DevPTRTMax);
                    }
                    ///DevPTRTMaxTime	单次正向段最长持续时间起点时刻 - 单次最长，进、出 x ＜ 0 时刻
                    if (null != res.DevPTRTMaxTime) {
                        results[113] = new List<PValue>();
                        results[114] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevPTRTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevPTRTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[113].Add(DevPTRTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevPTRTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevPTRTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[114].Add(DevPTRTMaxTimeEnd);
                    }
                    ///DevNTTime	负向时刻，在 X ＜ O 时刻 - 进、出 X ＜ O 时刻，X ＜ O 算进
                    if (null != res.DevNTTime) {
                        results[115] = new List<PValue>();
                        results[116] = new List<PValue>();
                        //results[128] = new List<PValue>();
                        int count = res.DevNTTime.Count;
                        for (int j = 0; j < count; j++) {
                            DateTime startTime = Convert.ToDateTime(res.DevNTTime[j].startDate);
                            double value = startTime.Ticks;
                            PValue DevNTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[115].Add(DevNTTime);

                            DateTime endTime = Convert.ToDateTime(res.DevNTTime[j].endDate);
                            double endValue = endTime.Ticks;
                            PValue DevNTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            results[116].Add(DevNTTimeEnd);

                            //PValue DevNTTimeVC = new PValue(res.DevNTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                            //results[128].Add(DevNTTimeVC);
                        }
                    }
                    ///DevNT	负向段，X ＜ 0 时间长
                    if (null != res.DevNT) {
                        results[117] = new List<PValue>();
                        PValue DevNT = new PValue(Double.Parse(res.DevNT), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[117].Add(DevNT);
                    }
                    ///DevNTR	负向段，X ＜ 0 时间占比
                    if (null != res.DevNTR) {
                        results[118] = new List<PValue>();
                        PValue DevNTR = new PValue(Double.Parse(res.DevNTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[118].Add(DevNTR);
                    }
                    ///DevNTRTMax	单次负向段最长持续时间
                    if (null != res.DevNTRTMax) {
                        results[119] = new List<PValue>();
                        PValue DevNTRTMax = new PValue(Double.Parse(res.DevNTRTMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[119].Add(DevNTRTMax);
                    }
                    ///DevNTRTMaxTime	单次负向段最长持续时间起点时刻 - 单次最长，进、出 x ＜ 0 时刻
                    if (null != res.DevNTRTMaxTime) {
                        results[120] = new List<PValue>();
                        results[121] = new List<PValue>();
                        DateTime startTime = Convert.ToDateTime(res.DevNTRTMaxTime.startDate);
                        double value = startTime.Ticks;
                        PValue DevNTRTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[120].Add(DevNTRTMaxTime);
                        DateTime endTime = Convert.ToDateTime(res.DevNTRTMaxTime.endDate);
                        double endValue = endTime.Ticks;
                        PValue DevNTRTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[121].Add(DevNTRTMaxTimeEnd);
                    }

                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                 #endregion
                }
                else
                {
                #region 长周期计算
                //长周期算法
                //长周期算法数据是在短周期算法得出的数据结果上进行再次处理 天、月、年 等时间数据运算

                 List<MDevLimitMessageOutClass> longlist = new List<MDevLimitMessageOutClass>();
                 
                 for (int j = 0; j < input.Count; j++) {
                     MDevLimitMessageOutClass v = new MDevLimitMessageOutClass();
                     v.id = j;

                     if (null != inputs[0][j].Value) { v.DevHLHHLLT = inputs[0][j].Value; }

                     if (null != inputs[1][j].Value) { v.DevHLHHLLR = inputs[1][j].Value; }

                     if (null != inputs[2][j].Value) { v.DevHHN     = inputs[2][j].Value; }

                     if (null != inputs[3][j].Value) {
                         List<D22STimeClass> DevHHTime = new List<D22STimeClass>();
                         v.DevHHTime = DevHHTime;

                         int DevHHTimeCount = inputs[3].Count;
                         for (int TimeStart = 0; TimeStart < DevHHTimeCount; TimeStart++) {
                             if (inputs[3][TimeStart].Timestamp == input[j].Timestamp && inputs[3][TimeStart].Endtime == input[j].Endtime && inputs[3][TimeStart].Status == 0) {
                                 D22STimeClass DevHHTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[3][TimeStart].Value;
                                 DateTime DevHHTimeStart = new DateTime(StartTime);
                                 DevHHTimes.startDate = DevHHTimeStart.ToString();

                                 long EndTime = (long)inputs[4][TimeStart].Value;
                                 DateTime DevHHTimeEnd = new DateTime(EndTime);
                                 DevHHTimes.endDate = DevHHTimeEnd.ToString();

                                 //DevHHTimes.valueCount = inputs[5][TimeStart].Value;

                                 DevHHTime.Add(DevHHTimes);
                             }
                         }
                     }
                     if (null != inputs[5][j].Value) { v.DevHHT   = inputs[5][j].Value; }

                     if (null != inputs[6][j].Value) { v.DevHHR   = inputs[6][j].Value; }

                     if (null != inputs[7][j].Value) { v.DevHHTMax= inputs[7][j].Value; }

                     if (null != inputs[8][j].Value) {
                         D22STimeClass DevHHTMaxTimes = new D22STimeClass();
                         v.DevHHTMaxTime = DevHHTMaxTimes;

                         long StartTime = (long)inputs[8][j].Value;
                         DateTime HHTMaxTime = new DateTime(StartTime);
                         DevHHTMaxTimes.startDate = HHTMaxTime.ToString();

                         long EndTime = (long)inputs[9][j].Value;
                         DateTime DevHHTMaxTimeEnd = new DateTime(EndTime);
                         DevHHTMaxTimes.endDate = DevHHTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[10][j].Value) { v.DevHHA = inputs[10][j].Value; }

                     if (null != inputs[11][j].Value) { v.DevHHET = inputs[11][j].Value; }

                     if (null != inputs[12][j].Value) { v.DevHN = inputs[12][j].Value; }

                     if (null != inputs[13][j].Value) {
                         List<D22STimeClass> DevHTime = new List<D22STimeClass>();
                         v.DevHTime = DevHTime;
                         int DevHTimeCount = inputs[13].Count;

                         for (int TimeStart = 0; TimeStart < DevHTimeCount; TimeStart++) {
                             if (inputs[13][TimeStart].Timestamp == input[j].Timestamp && inputs[13][TimeStart].Endtime == input[j].Endtime && inputs[13][TimeStart].Status == 0) {
                                 D22STimeClass DevHTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[13][TimeStart].Value;
                                 DateTime DevHTimeStart = new DateTime(StartTime);
                                 DevHTimes.startDate = DevHTimeStart.ToString();

                                 long EndTime = (long)inputs[14][TimeStart].Value;
                                 DateTime DevHTimeEnd = new DateTime(EndTime);
                                 DevHTimes.endDate = DevHTimeEnd.ToString();

                                 //DevHTimes.valueCount = inputs[16][TimeStart].Value;

                                 DevHTime.Add(DevHTimes);
                             }
                         }
                     }
                     if (null != inputs[15][j].Value) { v.DevHT = inputs[15][j].Value; }

                     if (null != inputs[16][j].Value) { v.DevHR   = inputs[16][j].Value; }

                     if (null != inputs[17][j].Value) { v.DevHTMax= inputs[17][j].Value; }

                     if (null != inputs[18][j].Value) {
                         D22STimeClass DevHTMaxTimes = new D22STimeClass();
                         v.DevHTMaxTime = DevHTMaxTimes;

                         long StartTime = (long)inputs[18][j].Value;
                         DateTime HTMaxTime = new DateTime(StartTime);
                         DevHTMaxTimes.startDate = HTMaxTime.ToString();

                         long EndTime = (long)inputs[19][j].Value;
                         DateTime DevHTMaxTimeEnd = new DateTime(EndTime);
                         DevHTMaxTimes.endDate = DevHTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[20][j].Value) { v.DevHA  = inputs[20][j].Value; }

                     if (null != inputs[21][j].Value) { v.DevHET = inputs[21][j].Value; }

                     if (null != inputs[22][j].Value) { v.DevRPN = inputs[22][j].Value; }

                     if (null != inputs[23][j].Value) {
                         List<D22STimeClass> DevRPTime = new List<D22STimeClass>();
                         v.DevRPTime = DevRPTime;

                         int DevRPTimeCount = inputs[23].Count;
                         for (int TimeStart = 0; TimeStart < DevRPTimeCount; TimeStart++) {
                             if (inputs[23][TimeStart].Timestamp == input[j].Timestamp && inputs[23][TimeStart].Endtime == input[j].Endtime && inputs[23][TimeStart].Status == 0) {
                                 D22STimeClass DevRPTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[23][TimeStart].Value;
                                 DateTime DevRPTimeStart = new DateTime(StartTime);
                                 DevRPTimes.startDate = DevRPTimeStart.ToString();

                                 long EndTime = (long)inputs[24][TimeStart].Value;
                                 DateTime DevRPTimeEnd = new DateTime(EndTime);
                                 DevRPTimes.endDate = DevRPTimeEnd.ToString();

                                 //DevRPTimes.valueCount = inputs[27][TimeStart].Value;

                                 DevRPTime.Add(DevRPTimes);
                             }
                         }
                     }
                     if (null != inputs[25][j].Value) { v.DevRPT   = inputs[25][j].Value; }

                     if (null != inputs[26][j].Value) { v.DevRPR   = inputs[26][j].Value; }

                     if (null != inputs[27][j].Value) { v.DevRPTMax= inputs[27][j].Value; }

                     if (null != inputs[28][j].Value) {
                         D22STimeClass DevRPTMaxTimes = new D22STimeClass();
                         v.DevRPTMaxTime = DevRPTMaxTimes;

                         long StartTime = (long)inputs[28][j].Value;
                         DateTime DevRPTMaxTimeStart = new DateTime(StartTime);
                         DevRPTMaxTimes.startDate = DevRPTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[29][j].Value;
                         DateTime DevRPTMaxTimeEnd = new DateTime(EndTime);
                         DevRPTMaxTimes.endDate = DevRPTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[30][j].Value) { v.DevRPA  = inputs[30][j].Value; }

                     if (null != inputs[31][j].Value) { v.DevRPET = inputs[31][j].Value; }

                     if (null != inputs[32][j].Value) { v.Dev0PN  = inputs[32][j].Value; }

                     if (null != inputs[33][j].Value) {
                         List<D22STimeClass> Dev0PTime = new List<D22STimeClass>();
                         v.Dev0PTime = Dev0PTime;

                         int Dev0PTimeCount = inputs[33].Count;
                         for (int TimeStart = 0; TimeStart < Dev0PTimeCount; TimeStart++) {
                             if (inputs[33][TimeStart].Timestamp == input[j].Timestamp && inputs[33][TimeStart].Endtime == input[j].Endtime && inputs[33][TimeStart].Status == 0) {
                                 D22STimeClass Dev0PTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[33][TimeStart].Value;
                                 DateTime Dev0PTimeStart = new DateTime(StartTime);
                                 Dev0PTimes.startDate = Dev0PTimeStart.ToString();

                                 long EndTime = (long)inputs[34][TimeStart].Value;
                                 DateTime Dev0PTimeEnd = new DateTime(EndTime);
                                 Dev0PTimes.endDate = Dev0PTimeEnd.ToString();

                                 //Dev0PTimes.valueCount = inputs[38][TimeStart].Value;

                                 Dev0PTime.Add(Dev0PTimes);
                             }
                         }
                     }
                     if (null != inputs[35][j].Value) { v.Dev0PT   = inputs[35][j].Value; }

                     if (null != inputs[36][j].Value) { v.Dev0PR = inputs[36][j].Value; }

                     if (null != inputs[37][j].Value) { v.Dev0PTMax = inputs[37][j].Value; }

                     if (null != inputs[38][j].Value) {
                         D22STimeClass Dev0PTMaxTimes = new D22STimeClass();
                         v.Dev0PTMaxTime = Dev0PTMaxTimes;

                         long StartTime = (long)inputs[38][j].Value;
                         DateTime Dev0PTMaxTimeStart = new DateTime(StartTime);
                         Dev0PTMaxTimes.startDate = Dev0PTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[39][j].Value;
                         DateTime Dev0PTMaxTimeEnd = new DateTime(EndTime);
                         Dev0PTMaxTimes.endDate = Dev0PTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[40][j].Value) { v.Dev0PA  = inputs[40][j].Value; }

                     if (null != inputs[41][j].Value) { v.Dev0PET = inputs[41][j].Value; }

                     if (null != inputs[42][j].Value) { v.Dev0NN  = inputs[42][j].Value; }

                     if (null != inputs[43][j].Value) {
                         List<D22STimeClass> Dev0NTime = new List<D22STimeClass>();
                         v.Dev0NTime = Dev0NTime;
                         int Dev0NTimeCount = inputs[43].Count;
                         for (int TimeStart = 0; TimeStart < Dev0NTimeCount; TimeStart++) {
                             if (inputs[43][TimeStart].Timestamp == input[j].Timestamp && inputs[43][TimeStart].Endtime == input[j].Endtime && inputs[43][TimeStart].Status == 0) {
                                 D22STimeClass Dev0NTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[43][TimeStart].Value;
                                 DateTime Dev0NTimeStart = new DateTime(StartTime);
                                 Dev0NTimes.startDate = Dev0NTimeStart.ToString();

                                 long EndTime = (long)inputs[44][TimeStart].Value;
                                 DateTime Dev0NTimeEnd = new DateTime(EndTime);
                                 Dev0NTimes.endDate = Dev0NTimeEnd.ToString();

                                 //Dev0NTimes.valueCount = inputs[49][TimeStart].Value;

                                 Dev0NTime.Add(Dev0NTimes);
                             }
                         }
                     }
                     if (null != inputs[45][j].Value) { v.Dev0NT   = inputs[45][j].Value; }

                     if (null != inputs[46][j].Value) { v.Dev0NR   = inputs[46][j].Value; }

                     if (null != inputs[47][j].Value) { v.Dev0NTMax= inputs[47][j].Value; }

                     if (null != inputs[48][j].Value) {
                         D22STimeClass Dev0NTMaxTimes = new D22STimeClass();
                         v.Dev0NTMaxTime = Dev0NTMaxTimes;

                         long StartTime = (long)inputs[48][j].Value;
                         DateTime Dev0NTMaxTimeStart = new DateTime(StartTime);
                         Dev0NTMaxTimes.startDate = Dev0NTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[49][j].Value;
                         DateTime Dev0NTMaxTimeEnd = new DateTime(EndTime);
                         Dev0NTMaxTimes.endDate = Dev0NTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[50][j].Value) { v.Dev0NA  = inputs[50][j].Value; }

                     if (null != inputs[51][j].Value) { v.Dev0NET = inputs[51][j].Value; }

                     if (null != inputs[52][j].Value) { v.DevRNN  = inputs[52][j].Value; }

                     if (null != inputs[53][j].Value) {
                         List<D22STimeClass> DevRNTime = new List<D22STimeClass>();
                         v.DevRNTime = DevRNTime;

                         int DevRNTimeCount = inputs[53].Count;
                         for (int TimeStart = 0; TimeStart < DevRNTimeCount; TimeStart++) {
                             if (inputs[53][TimeStart].Timestamp == input[j].Timestamp && inputs[53][TimeStart].Endtime == input[j].Endtime && inputs[53][TimeStart].Status == 0) {
                                 D22STimeClass DevRNTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[53][TimeStart].Value;
                                 DateTime DevRNTimeStart = new DateTime(StartTime);
                                 DevRNTimes.startDate = DevRNTimeStart.ToString();

                                 long EndTime = (long)inputs[54][TimeStart].Value;
                                 DateTime DevRNTimeEnd = new DateTime(EndTime);
                                 DevRNTimes.endDate = DevRNTimeEnd.ToString();

                                 //DevRNTimes.valueCount = inputs[60][TimeStart].Value;

                                 DevRNTime.Add(DevRNTimes);
                             }
                         }
                     }
                     if (null != inputs[55][j].Value) { v.DevRNT   = inputs[55][j].Value; }

                     if (null != inputs[56][j].Value) { v.DevRNR   = inputs[56][j].Value; }

                     if (null != inputs[57][j].Value) { v.DevRNTMax= inputs[57][j].Value; }

                     if (null != inputs[58][j].Value)
                     {
                         D22STimeClass DevRNTMaxTimes = new D22STimeClass();
                         v.DevRNTMaxTime = DevRNTMaxTimes;

                         long StartTime = (long)inputs[58][j].Value;
                         DateTime DevRNTMaxTimeStart = new DateTime(StartTime);
                         DevRNTMaxTimes.startDate = DevRNTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[59][j].Value;
                         DateTime DevRNTMaxTimeEnd = new DateTime(EndTime);
                         DevRNTMaxTimes.endDate = DevRNTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[60][j].Value) { v.DevRNA = inputs[60][j].Value; }

                     if (null != inputs[61][j].Value) { v.DevRNET = inputs[61][j].Value; }

                     if (null != inputs[62][j].Value) { v.DevLN   = inputs[62][j].Value; }

                     if (null != inputs[63][j].Value) {
                         List<D22STimeClass> DevLTime = new List<D22STimeClass>();
                         v.DevLTime = DevLTime;

                         int DevLTimeCount = inputs[63].Count;
                         for (int TimeStart = 0; TimeStart < DevLTimeCount; TimeStart++) {
                             if (inputs[63][TimeStart].Timestamp == input[j].Timestamp && inputs[63][TimeStart].Endtime == input[j].Endtime && inputs[63][TimeStart].Status == 0) {
                                 D22STimeClass DevLTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[63][TimeStart].Value;
                                 DateTime DevLTimeStart = new DateTime(StartTime);
                                 DevLTimes.startDate = DevLTimeStart.ToString();

                                 long EndTime = (long)inputs[64][TimeStart].Value;
                                 DateTime DevLTimeEnd = new DateTime(EndTime);
                                 DevLTimes.endDate = DevLTimeEnd.ToString();

                                 //DevLTimes.valueCount = inputs[71][TimeStart].Value;

                                 DevLTime.Add(DevLTimes);
                             }
                         }
                     }
                     if (null != inputs[65][j].Value) { v.DevLT   = inputs[65][j].Value; }

                     if (null != inputs[66][j].Value) { v.DevLR   = inputs[66][j].Value; }

                     if (null != inputs[67][j].Value) { v.DevLTMax= inputs[67][j].Value; }

                     if (null != inputs[68][j].Value) {
                         D22STimeClass DevLTMaxTimes = new D22STimeClass();
                         v.DevLTMaxTime = DevLTMaxTimes;

                         long StartTime = (long)inputs[68][j].Value;
                         DateTime DevLTMaxTimeStart = new DateTime(StartTime);
                         DevLTMaxTimes.startDate = DevLTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[69][j].Value;
                         DateTime DevLTMaxTimeEnd = new DateTime(EndTime);
                         DevLTMaxTimes.endDate = DevLTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[70][j].Value) { v.DevLA  = inputs[70][j].Value; }

                     if (null != inputs[71][j].Value) { v.DevLET = inputs[71][j].Value; }

                     if (null != inputs[72][j].Value) { v.DevLLN = inputs[72][j].Value; }

                     if (null != inputs[73][j].Value) {
                         List<D22STimeClass> DevLLTime = new List<D22STimeClass>();
                         v.DevLLTime = DevLLTime;

                         int DevLLTimeCount = inputs[73].Count;
                         for (int TimeStart = 0; TimeStart < DevLLTimeCount; TimeStart++) {
                             if (inputs[73][TimeStart].Timestamp == input[j].Timestamp && inputs[73][TimeStart].Endtime == input[j].Endtime && inputs[73][TimeStart].Status == 0) {
                                 D22STimeClass DevLLTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[73][TimeStart].Value;
                                 DateTime DevLLTimeStart = new DateTime(StartTime);
                                 DevLLTimes.startDate = DevLLTimeStart.ToString();

                                 long EndTime = (long)inputs[74][TimeStart].Value;
                                 DateTime DevLLTimeEnd = new DateTime(EndTime);
                                 DevLLTimes.endDate = DevLLTimeEnd.ToString();

                                 //DevLLTimes.valueCount = inputs[82][TimeStart].Value;

                                 DevLLTime.Add(DevLLTimes);
                             }
                         }
                     }
                     if (null != inputs[75][j].Value) { v.DevLLT   = inputs[75][j].Value; }

                     if (null != inputs[76][j].Value) { v.DevLLR   = inputs[76][j].Value; }

                     if (null != inputs[77][j].Value) { v.DevLLTMax= inputs[77][j].Value; }

                     if (null != inputs[78][j].Value) {
                         D22STimeClass DevLLTMaxTimes = new D22STimeClass();
                         v.DevLLTMaxTime = DevLLTMaxTimes;

                         long StartTime = (long)inputs[78][j].Value;
                         DateTime DevLLTMaxTimeStart = new DateTime(StartTime);
                         DevLLTMaxTimes.startDate = DevLLTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[79][j].Value;
                         DateTime DevLLTMaxTimeEnd = new DateTime(EndTime);
                         DevLLTMaxTimes.endDate = DevLLTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[80][j].Value) { v.DevLLA   = inputs[80][j].Value; }

                     if (null != inputs[81][j].Value) { v.DevLLET  = inputs[81][j].Value; }

                     if (null != inputs[82][j].Value) { v.Dev0HT   = inputs[82][j].Value; }

                     if (null != inputs[83][j].Value) { v.Dev0HTR  = inputs[83][j].Value; }

                     if (null != inputs[84][j].Value) { v.Dev0HHT  = inputs[84][j].Value; }

                     if (null != inputs[85][j].Value) { v.Dev0HHTR = inputs[85][j].Value; }

                     if (null != inputs[86][j].Value) { v.Dev0L    = inputs[86][j].Value; }

                     if (null != inputs[87][j].Value) { v.Dev0LR   = inputs[87][j].Value; }

                     if (null != inputs[88][j].Value) { v.Dev0LLT  = inputs[88][j].Value; }

                     if (null != inputs[89][j].Value) { v.Dev0LLTR = inputs[89][j].Value; }

                     if (null != inputs[90][j].Value) { v.DevHHLLT   = inputs[90][j].Value; }

                     if (null != inputs[91][j].Value) { v.DevHHLLTR  = inputs[91][j].Value; }

                     if (null != inputs[92][j].Value) { v.DevRPRMHLT = inputs[92][j].Value; }

                     if (null != inputs[93][j].Value) { v.DevRPRMHLTR= inputs[93][j].Value; }

                     if (null != inputs[94][j].Value)
                     {
                         List<D22STimeClass> Dev0RPRMTTime = new List<D22STimeClass>();
                         v.Dev0RPRMTTime = Dev0RPRMTTime;

                         int Dev0RPRMTTimeCount = inputs[94].Count;
                         for (int TimeStart = 0; TimeStart < Dev0RPRMTTimeCount; TimeStart++) {
                             if (inputs[94][TimeStart].Timestamp == input[j].Timestamp && inputs[94][TimeStart].Endtime == input[j].Endtime && inputs[94][TimeStart].Status == 0) {
                                 D22STimeClass Dev0RPRMTTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[94][TimeStart].Value;
                                 DateTime Dev0RPRMTTimeStart = new DateTime(StartTime);
                                 Dev0RPRMTTimes.startDate = Dev0RPRMTTimeStart.ToString();

                                 long EndTime = (long)inputs[95][TimeStart].Value;
                                 DateTime Dev0RPRMTTimeEnd = new DateTime(EndTime);
                                 Dev0RPRMTTimes.endDate = Dev0RPRMTTimeEnd.ToString();

                                 //Dev0RPRMTTimes.valueCount = inputs[104][TimeStart].Value;

                                 Dev0RPRMTTime.Add(Dev0RPRMTTimes);
                             }
                         }
                     }
                     if (null != inputs[96][j].Value) { v.Dev0RPRMT = inputs[96][j].Value; }

                     if (null != inputs[97][j].Value) { v.Dev0RPRMTR = inputs[97][j].Value; }

                     if (null != inputs[98][j].Value) { v.Dev0RPRMTMax = inputs[98][j].Value; }

                     if (null != inputs[99][j].Value) {
                         D22STimeClass Dev0RPRMTMaxTimes = new D22STimeClass();
                         v.Dev0RPRMTMaxTime = Dev0RPRMTMaxTimes;

                         long StartTime = (long)inputs[99][j].Value;
                         DateTime Dev0RPRMTMaxTimeStart = new DateTime(StartTime);
                         Dev0RPRMTMaxTimes.startDate = Dev0RPRMTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[100][j].Value;
                         DateTime Dev0RPRMTMaxTimeEnd = new DateTime(EndTime);
                         Dev0RPRMTMaxTimes.endDate = Dev0RPRMTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[101][j].Value) {
                         List<D22STimeClass> DevHLTTime = new List<D22STimeClass>();
                         v.DevHLTTime = DevHLTTime;

                         int DevHLTTimeCount = inputs[101].Count;
                         for (int TimeStart = 0; TimeStart < DevHLTTimeCount; TimeStart++) {
                             if (inputs[101][TimeStart].Timestamp == input[j].Timestamp && inputs[101][TimeStart].Endtime == input[j].Endtime && inputs[101][TimeStart].Status == 0) {
                                 D22STimeClass DevHLTTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[101][TimeStart].Value;
                                 DateTime DevHLTTimeStart = new DateTime(StartTime);
                                 DevHLTTimes.startDate = DevHLTTimeStart.ToString();

                                 long EndTime = (long)inputs[102][TimeStart].Value;
                                 DateTime DevHLTTimeEnd = new DateTime(EndTime);
                                 DevHLTTimes.endDate = DevHLTTimeEnd.ToString();

                                 //DevHLTTimes.valueCount = inputs[112][TimeStart].Value;

                                 DevHLTTime.Add(DevHLTTimes);
                             }
                         }
                     }
                     if (null != inputs[103][j].Value) { v.DevHLT   = inputs[103][j].Value; }

                     if (null != inputs[104][j].Value) { v.DevHLTR  = inputs[104][j].Value; }

                     if (null != inputs[105][j].Value) { v.DevHLTMax= inputs[105][j].Value; }

                     if (null != inputs[106][j].Value) {
                         D22STimeClass DevHLTMaxTimes = new D22STimeClass();
                         v.DevHLTMaxTime = DevHLTMaxTimes;

                         long StartTime = (long)inputs[106][j].Value;
                         DateTime DevHLTMaxTimeStart = new DateTime(StartTime);
                         DevHLTMaxTimes.startDate = DevHLTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[107][j].Value;
                         DateTime DevHLTMaxTimeEnd = new DateTime(EndTime);
                         DevHLTMaxTimes.endDate = DevHLTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[108][j].Value) {
                         List<D22STimeClass> DevPTTime = new List<D22STimeClass>();
                         v.DevPTTime = DevPTTime;

                         int DevPTTimeCount = inputs[108].Count;
                         for (int TimeStart = 0; TimeStart < DevPTTimeCount; TimeStart++) {
                             if (inputs[108][TimeStart].Timestamp == input[j].Timestamp && inputs[108][TimeStart].Endtime == input[j].Endtime && inputs[108][TimeStart].Status == 0) {
                                 D22STimeClass DevPTTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[108][TimeStart].Value;
                                 DateTime DevPTTimeStart = new DateTime(StartTime);
                                 DevPTTimes.startDate = DevPTTimeStart.ToString();

                                 long EndTime = (long)inputs[109][TimeStart].Value;
                                 DateTime DevPTTimeEnd = new DateTime(EndTime);
                                 DevPTTimes.endDate = DevPTTimeEnd.ToString();

                                 //DevPTTimes.valueCount = inputs[120][TimeStart].Value;

                                 DevPTTime.Add(DevPTTimes);
                             }
                         }
                     }
                     if (null != inputs[110][j].Value) { v.DevPT     = inputs[110][j].Value; }

                     if (null != inputs[111][j].Value) { v.DevPTR    = inputs[111][j].Value; }

                     if (null != inputs[112][j].Value) { v.DevPTRTMax= inputs[112][j].Value; }

                     if (null != inputs[113][j].Value) {
                         D22STimeClass DevPTRTMaxTimes = new D22STimeClass();
                         v.DevPTRTMaxTime = DevPTRTMaxTimes;

                         long StartTime = (long)inputs[113][j].Value;
                         DateTime DevPTRTMaxTimeStart = new DateTime(StartTime);
                         DevPTRTMaxTimes.startDate = DevPTRTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[114][j].Value;
                         DateTime DevPTRTMaxTimeEnd = new DateTime(EndTime);
                         DevPTRTMaxTimes.endDate = DevPTRTMaxTimeEnd.ToString();
                     }
                     if (null != inputs[115][j].Value) {
                         List<D22STimeClass> DevNTTime = new List<D22STimeClass>();
                         v.DevNTTime = DevNTTime;

                         int DevNTTimeCount = inputs[115].Count;
                         for (int TimeStart = 0; TimeStart < DevNTTimeCount; TimeStart++) {
                             if (inputs[115][TimeStart].Timestamp == input[j].Timestamp && inputs[115][TimeStart].Endtime == input[j].Endtime && inputs[115][TimeStart].Status == 0) {
                                 D22STimeClass DevNTTimes = new D22STimeClass();
                                 long StartTime = (long)inputs[115][TimeStart].Value;
                                 DateTime DevPTTimeStart = new DateTime(StartTime);
                                 DevNTTimes.startDate = DevPTTimeStart.ToString();

                                 long EndTime = (long)inputs[116][TimeStart].Value;
                                 DateTime DevPTTimeEnd = new DateTime(EndTime);
                                 DevNTTimes.endDate = DevPTTimeEnd.ToString();

                                 //DevNTTimes.valueCount = inputs[128][TimeStart].Value;

                                 DevNTTime.Add(DevNTTimes);
                             }
                         }
                     }
                     if (null != inputs[117][j].Value) { v.DevNT     = inputs[117][j].Value; }

                     if (null != inputs[118][j].Value) { v.DevNTR    = inputs[118][j].Value; }

                     if (null != inputs[119][j].Value) { v.DevNTRTMax= inputs[119][j].Value; }

                     if (null != inputs[120][j].Value) {
                         D22STimeClass DevNTRTMaxTimes = new D22STimeClass();
                         v.DevNTRTMaxTime = DevNTRTMaxTimes;

                         long StartTime = (long)inputs[120][j].Value;
                         DateTime DevNTRTMaxTimeStart = new DateTime(StartTime);
                         DevNTRTMaxTimes.startDate = DevNTRTMaxTimeStart.ToString();

                         long EndTime = (long)inputs[121][j].Value;
                         DateTime DevNTRTMaxTimeEnd = new DateTime(EndTime);
                         DevNTRTMaxTimes.endDate = DevNTRTMaxTimeEnd.ToString();
                     }

                     longlist.Add(v);
                 }
                 #endregion
                 #region 长周期算法--数据组织输出
                 //调用长周期算法数据是把小时数据结果进行处理 天、月级别时间数据运算
                 MDevLimitMessageOutClass res = Limits.longMDevLimit(longlist, LimitHH, LimitH, LimitRP, LimitOO, LimitRN, LimitL, LimitLL);
                 //组织结算结果
                 if (null != res.DevHLHHLLT) {
                     results[0] = new List<PValue>();
                     PValue DevHLHHLLT = new PValue(res.DevHLHHLLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                     results[0].Add(DevHLHHLLT);
                 }
                 if (null != res.DevHLHHLLR) {
                     results[1] = new List<PValue>();
                     PValue DevHLHHLLR = new PValue(res.DevHLHHLLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                     results[1].Add(DevHLHHLLR);
                 }
                 if (null != res.DevHHN) {
                    results[2] = new List<PValue>();
                    PValue DevHHN = new PValue(res.DevHHN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[2].Add(DevHHN);
                 } 
                if (null != res.DevHHTime) {
                    results[3] = new List<PValue>();
                    results[4] = new List<PValue>();
                    //results[5] = new List<PValue>();
                    int count = res.DevHHTime.Count;
                    for (int j = 0; j < count; j++) {
                        DateTime startTime = Convert.ToDateTime(res.DevHHTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevHHTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[3].Add(DevHHTime);

                        DateTime endTime = Convert.ToDateTime(res.DevHHTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevHHTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[4].Add(DevHHTimeEnd);

                        //PValue DevHHTimeVC = new PValue(res.DevHHTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[5].Add(DevHHTimeVC);
                    }
                }      
                
                if (null != res.DevHHT){
                    results[5] = new List<PValue>();
                    PValue DevHHT = new PValue(res.DevHHT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[5].Add(DevHHT);
                }
                if (null != res.DevHHR){
                    results[6] = new List<PValue>();
                    PValue DevHHR = new PValue(res.DevHHR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[6].Add(DevHHR);
                }
                if (null != res.DevHHTMax){
                    results[7] = new List<PValue>();
                    //DateTime time = Convert.ToDateTime(res.DevHHTMax);
                    //double value = time.Ticks;
                    PValue DevHHTMax = new PValue(res.DevHHTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[7].Add(DevHHTMax);
                }
                if (null != res.DevHHTMaxTime){
                    results[8] = new List<PValue>();
                    results[9] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevHHTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevHHTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[8].Add(DevHHTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevHHTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevHHTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[9].Add(DevHHTMaxTimeEnd);
                }
                if (null != res.DevHHA){
                    results[10] = new List<PValue>();
                    PValue DevHHA = new PValue(res.DevHHA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[10].Add(DevHHA);
                }
                if (null != res.DevHHET){
                    results[11] = new List<PValue>();
                    PValue DevHHET = new PValue(res.DevHHET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[11].Add(DevHHET);
                }
                if (null != res.DevHN){
                    results[12] = new List<PValue>();
                    PValue DevHN = new PValue(res.DevHN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[12].Add(DevHN);
                }
                if (null != res.DevHTime){
                    results[13] = new List<PValue>();
                    results[14] = new List<PValue>();
                    //results[16] = new List<PValue>();
                    int count = res.DevHTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime time = Convert.ToDateTime(res.DevHTime[j].startDate);
                        double value = time.Ticks;
                        PValue DevHTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[13].Add(DevHTime);

                        DateTime endTime = Convert.ToDateTime(res.DevHTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevHTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[14].Add(DevHTimeEnd);

                        //PValue DevHTimeVC = new PValue(res.DevHTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[16].Add(DevHTimeVC);
                    }
                }
                if (null != res.DevHT){
                    results[15] = new List<PValue>();
                    PValue DevHT = new PValue(res.DevHT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[15].Add(DevHT);
                }
                if (null != res.DevHR){
                    results[16] = new List<PValue>();
                    PValue DevHR = new PValue(res.DevHR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[16].Add(DevHR);
                }
                if (null != res.DevHTMax){
                    results[17] = new List<PValue>();
                    PValue DevHTMax = new PValue(res.DevHTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[17].Add(DevHTMax);
                }
                if (null != res.DevHTMaxTime){
                    results[18] = new List<PValue>();
                    results[19] = new List<PValue>();
                    DateTime time = Convert.ToDateTime(res.DevHTMaxTime.startDate);
                    double value = time.Ticks;
                    PValue DevHTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[18].Add(DevHTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevHTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevHTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[19].Add(DevHTMaxTimeEnd);
                }
                if (null != res.DevHA){
                    results[20] = new List<PValue>();
                    PValue DevHA = new PValue(res.DevHA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[20].Add(DevHA);
                }
                if (null != res.DevHET){
                    results[21] = new List<PValue>();
                    PValue DevHET = new PValue(res.DevHET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[21].Add(DevHET);
                }
                if (null != res.DevRPN){
                    results[22] = new List<PValue>();
                    PValue DevRPN = new PValue(res.DevRPN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[22].Add(DevRPN);
                }
                if (null != res.DevRPTime){
                    results[23] = new List<PValue>();
                    results[24] = new List<PValue>();
                    //results[27] = new List<PValue>();
                    int count = res.DevRPTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.DevRPTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevRPTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[23].Add(DevRPTime);

                        DateTime endTime = Convert.ToDateTime(res.DevRPTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevRPTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[24].Add(DevRPTimeEnd);

                        //PValue DevRPTimeVC = new PValue(res.DevRPTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[27].Add(DevRPTimeVC);
                    }
                }
                if (null != res.DevRPT){
                    results[25] = new List<PValue>();
                    PValue DevRPT = new PValue(res.DevRPT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[25].Add(DevRPT);
                }
                if (null != res.DevRPR){
                    results[26] = new List<PValue>();
                    PValue DevRPR = new PValue(res.DevRPR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[26].Add(DevRPR);
                }
                if (null != res.DevRPTMax){
                    results[27] = new List<PValue>();
                    PValue DevRPTMax = new PValue(res.DevRPTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[27].Add(DevRPTMax);
                }
                if (null != res.DevRPTMaxTime){
                    results[28] = new List<PValue>();
                    results[29] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevRPTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevRPTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[28].Add(DevRPTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevRPTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevRPTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[29].Add(DevRPTMaxTimeEnd);
                }
                if (null != res.DevRPA){
                    results[30] = new List<PValue>();
                    PValue DevRPA = new PValue(res.DevRPA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[30].Add(DevRPA);
                }
                if (null != res.DevRPET){
                    results[31] = new List<PValue>();
                    PValue DevRPET = new PValue(res.DevRPET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[31].Add(DevRPET);
                }
                if (null != res.Dev0PN){
                    results[32] = new List<PValue>();
                    PValue Dev0PN = new PValue(res.Dev0PN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[32].Add(Dev0PN);
                }
                if (null != res.Dev0PTime){
                    results[33] = new List<PValue>();
                    results[34] = new List<PValue>();
                    //results[38] = new List<PValue>();
                    int count = res.Dev0PTime.Count;
                    for (int j = 0; j < count; j++) {
                        DateTime startTime = Convert.ToDateTime(res.Dev0PTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue Dev0PTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[33].Add(Dev0PTime);

                        DateTime endTime = Convert.ToDateTime(res.Dev0PTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue Dev0PTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[34].Add(Dev0PTimeEnd);

                        //PValue Dev0PTimeVC = new PValue(res.Dev0PTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[38].Add(Dev0PTimeVC);
                    }
                }
                if (null != res.Dev0PT){
                    results[35] = new List<PValue>();
                    PValue Dev0PT = new PValue(res.Dev0PT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[35].Add(Dev0PT);
                }
                if (null != res.Dev0PR){
                    results[36] = new List<PValue>();
                    PValue Dev0PR = new PValue(res.Dev0PR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[36].Add(Dev0PR);
                }
                if (null != res.Dev0PTMax){
                    results[37] = new List<PValue>();
                    PValue Dev0PTMax = new PValue(res.Dev0PTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[37].Add(Dev0PTMax);
                }
                if (null != res.Dev0PTMaxTime){
                    results[38] = new List<PValue>();
                    results[39] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.Dev0PTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue Dev0PTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[38].Add(Dev0PTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.Dev0PTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue Dev0PTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[39].Add(Dev0PTMaxTimeEnd);
                }
                if (null != res.Dev0PA){
                    results[40] = new List<PValue>();
                    PValue Dev0PA = new PValue(res.Dev0PA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[40].Add(Dev0PA);
                }
                if (null != res.Dev0PET){
                    results[41] = new List<PValue>();
                    PValue Dev0PET = new PValue(res.Dev0PET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[41].Add(Dev0PET);
                }
                if (null != res.Dev0NN){
                    results[42] = new List<PValue>();
                    PValue Dev0NN = new PValue(res.Dev0NN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[42].Add(Dev0NN);
                }
                if (null != res.Dev0NTime){
                    results[43] = new List<PValue>();
                    results[44] = new List<PValue>();
                    //results[49] = new List<PValue>();
                    int count = res.Dev0NTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.Dev0NTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue Dev0NTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[43].Add(Dev0NTime);

                        DateTime endTime = Convert.ToDateTime(res.Dev0NTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue Dev0NTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[44].Add(Dev0NTimeEnd);

                        //PValue Dev0NTimeVC = new PValue(res.Dev0NTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[49].Add(Dev0NTimeVC);
                    }
                }
                if (null != res.Dev0NT){
                    results[45] = new List<PValue>();
                    PValue Dev0NT = new PValue(res.Dev0NT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[45].Add(Dev0NT);
                }
                if (null != res.Dev0NR){
                    results[46] = new List<PValue>();
                    PValue Dev0NR = new PValue(res.Dev0NR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[46].Add(Dev0NR);
                }
                if (null != res.Dev0NTMax){
                    results[47] = new List<PValue>();
                    PValue Dev0NTMax = new PValue(res.Dev0NTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[47].Add(Dev0NTMax);
                }
                if (null != res.Dev0NTMaxTime){
                    results[48] = new List<PValue>();
                    results[49] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.Dev0NTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue Dev0NTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[48].Add(Dev0NTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.Dev0NTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue Dev0NTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[49].Add(Dev0NTMaxTimeEnd);
                }
                if (null != res.Dev0NA){
                    results[50] = new List<PValue>();
                    PValue Dev0NA = new PValue(res.Dev0NA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[50].Add(Dev0NA);
                }
                if (null != res.Dev0NET){
                    results[51] = new List<PValue>(); 
                    PValue Dev0NET = new PValue(res.Dev0NET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[51].Add(Dev0NET);
                }
                if (null != res.DevRNN){
                    results[52] = new List<PValue>(); 
                    PValue DevRNN = new PValue(res.DevRNN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[52].Add(DevRNN);
                }
                if (null != res.DevRNTime){
                    results[53] = new List<PValue>();
                    results[54] = new List<PValue>();
                    //results[60] = new List<PValue>();
                    int count = res.DevRNTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.DevRNTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevRNTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[53].Add(DevRNTime);

                        DateTime endTime = Convert.ToDateTime(res.DevRNTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevRNTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[54].Add(DevRNTimeEnd);

                        //PValue DevRNTimeVC = new PValue(res.DevRNTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[60].Add(DevRNTimeVC);
                    }
                }
                if (null != res.DevRNT){
                    results[55] = new List<PValue>();
                    PValue DevRNT = new PValue(res.DevRNT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[55].Add(DevRNT);
                }
                if (null != res.DevRNR){
                    results[56] = new List<PValue>();
                    PValue DevRNR = new PValue(res.DevRNR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[56].Add(DevRNR);
                }
                if (null != res.DevRNTMax){
                    results[57] = new List<PValue>();
                    PValue DevRNTMax = new PValue(res.DevRNTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[57].Add(DevRNTMax);
                }
                if (null != res.DevRNTMaxTime){
                    results[58] = new List<PValue>();
                    results[59] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevRNTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevRNTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[58].Add(DevRNTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevRNTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevRNTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[59].Add(DevRNTMaxTimeEnd);
                }
                if (null != res.DevRNA){
                    results[60] = new List<PValue>();
                    PValue DevRNA = new PValue(res.DevRNA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[60].Add(DevRNA);
                }
                if (null != res.DevRNET){
                    results[61] = new List<PValue>();
                    PValue DevRNET = new PValue(res.DevRNET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[61].Add(DevRNET);
                }
                if (null != res.DevLN){
                    results[62] = new List<PValue>();
                    PValue DevLN = new PValue(res.DevLN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[62].Add(DevLN);
                }
                if (null != res.DevLTime){
                    results[63] = new List<PValue>();
                    results[64] = new List<PValue>();
                    //results[71] = new List<PValue>();
                    int count = res.DevLTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.DevLTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevLTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[63].Add(DevLTime);

                        DateTime endTime = Convert.ToDateTime(res.DevLTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevLTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[64].Add(DevLTimeEnd);

                        //PValue DevLTimeVC = new PValue(res.DevLTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[71].Add(DevLTimeVC);
                    }
                }
                if (null != res.DevLT){
                    results[65] = new List<PValue>(); 
                    PValue DevLT = new PValue(res.DevLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[65].Add(DevLT);
                }
                if (null != res.DevLR){
                    results[66] = new List<PValue>();
                    PValue DevLR = new PValue(res.DevLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[66].Add(DevLR);
                }
                if (null != res.DevLTMax){
                    results[67] = new List<PValue>(); 
                    PValue DevLTMax = new PValue(res.DevLTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[67].Add(DevLTMax);
                }
                if (null != res.DevLTMaxTime){
                    results[68] = new List<PValue>();
                    results[69] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevLTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevLTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[68].Add(DevLTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevLTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevLTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[69].Add(DevLTMaxTimeEnd);
                }
                if (null != res.DevLA){
                    results[70] = new List<PValue>();
                    PValue DevLA = new PValue(res.DevLA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[70].Add(DevLA);
                }
                if (null != res.DevLET){
                    results[71] = new List<PValue>();
                    PValue DevLET = new PValue(res.DevLET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[71].Add(DevLET);
                }
                if (null != res.DevLLN){
                    results[72] = new List<PValue>();
                    PValue DevLLN = new PValue(res.DevLLN, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[72].Add(DevLLN);
                }
                if (null != res.DevLLTime){
                    results[73] = new List<PValue>();
                    results[74] = new List<PValue>();
                    //results[82] = new List<PValue>();
                    int count = res.DevLLTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.DevLLTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevLLTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[73].Add(DevLLTime);

                        DateTime endTime = Convert.ToDateTime(res.DevLLTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevLLTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[74].Add(DevLLTimeEnd);

                        //PValue DevLLTimeVC = new PValue(res.DevLLTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[82].Add(DevLLTimeVC);
                    }
                }
                if (null != res.DevLLT){
                    results[75] = new List<PValue>();
                    PValue DevLLT = new PValue(res.DevLLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[75].Add(DevLLT);
                }
                if (null != res.DevLLR){
                    results[76] = new List<PValue>();
                    PValue DevLLR = new PValue(res.DevLLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[76].Add(DevLLR);
                }
                if (null != res.DevLLTMax){
                    results[77] = new List<PValue>();
                    PValue DevLLTMax = new PValue(res.DevLLTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[77].Add(DevLLTMax);
                }
                if (null != res.DevLLTMaxTime){
                    results[78] = new List<PValue>();
                    results[79] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevLLTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevLLTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[78].Add(DevLLTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevLLTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevLLTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[79].Add(DevLLTMaxTimeEnd);
                }
                if (null != res.DevLLA){
                    results[80] = new List<PValue>();
                    PValue DevLLA = new PValue(res.DevLLA, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[80].Add(DevLLA);
                }
                if (null != res.DevLLET){
                    results[81] = new List<PValue>();
                    PValue DevLLET = new PValue(res.DevLLET, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[81].Add(DevLLET);
                }
                if (null != res.Dev0HT){
                    results[82] = new List<PValue>();
                    PValue Dev0HT = new PValue(res.Dev0HT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[82].Add(Dev0HT);
                }
                if (null != res.Dev0HTR){
                    results[83] = new List<PValue>();
                    PValue Dev0HTR = new PValue(res.Dev0HTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[83].Add(Dev0HTR);
                }
                if (null != res.Dev0HHT){
                    results[84] = new List<PValue>();
                    PValue Dev0HHT = new PValue(res.Dev0HHT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[84].Add(Dev0HHT);
                }
                if (null != res.Dev0HHTR){
                    results[85] = new List<PValue>();
                    PValue Dev0HHTR = new PValue(res.Dev0HHTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[85].Add(Dev0HHTR);
                }
                if (null != res.Dev0L){
                    results[86] = new List<PValue>();
                    PValue Dev0L = new PValue(res.Dev0L, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[86].Add(Dev0L);
                }
                if (null != res.Dev0LR){
                    results[87] = new List<PValue>();
                    PValue Dev0LR = new PValue(res.Dev0LR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[87].Add(Dev0LR);
                }
                if (null != res.Dev0LLT){
                    results[88] = new List<PValue>();
                    PValue Dev0LLT = new PValue(res.Dev0LLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[88].Add(Dev0LLT);
                }
                if (null != res.Dev0LLTR){
                    results[89] = new List<PValue>();
                    PValue Dev0LLTR = new PValue(res.Dev0LLTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[89].Add(Dev0LLTR);
                }
                if (null != res.DevHHLLT){
                    results[90] = new List<PValue>();
                    PValue DevHHLLT = new PValue(res.DevHHLLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[90].Add(DevHHLLT);
                }
                if (null != res.DevHHLLTR){
                    results[91] = new List<PValue>(); 
                    PValue DevHHLLTR = new PValue(res.DevHHLLTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[91].Add(DevHHLLTR);
                }
                if (null != res.DevRPRMHLT){
                    results[92] = new List<PValue>(); 
                    PValue DevRPRMHLT = new PValue(res.DevRPRMHLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[92].Add(DevRPRMHLT);
                }
                if (null != res.DevRPRMHLTR){
                    results[93] = new List<PValue>(); 
                    PValue DevRPRMHLTR = new PValue(res.DevRPRMHLTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[93].Add(DevRPRMHLTR);
                }
                if (null != res.Dev0RPRMTTime){
                    results[94] = new List<PValue>();
                    results[95] = new List<PValue>();
                    //results[104] = new List<PValue>();
                    int count = res.Dev0RPRMTTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.Dev0RPRMTTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue Dev0RPRMTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[94].Add(Dev0RPRMTTime);

                        DateTime endTime = Convert.ToDateTime(res.Dev0RPRMTTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue Dev0RPRMTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[95].Add(Dev0RPRMTTimeEnd);

                        //PValue Dev0RPRMTTimeVC = new PValue(res.Dev0RPRMTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[104].Add(Dev0RPRMTTimeVC);
                    }
                }
                if (null != res.Dev0RPRMT){
                    results[96] = new List<PValue>();
                    PValue Dev0RPRMT = new PValue(res.Dev0RPRMT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[96].Add(Dev0RPRMT);
                }
                if (null != res.Dev0RPRMTR){
                    results[97] = new List<PValue>();
                    PValue Dev0RPRMTR = new PValue(res.Dev0RPRMTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[97].Add(Dev0RPRMTR);
                }
                if (null != res.Dev0RPRMTMax){
                    results[98] = new List<PValue>();
                    PValue Dev0RPRMTMax = new PValue(res.Dev0RPRMTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[98].Add(Dev0RPRMTMax);
                }
                if (null != res.Dev0RPRMTMaxTime){
                    results[99] = new List<PValue>();
                    results[100] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.Dev0RPRMTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue Dev0RPRMTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[99].Add(Dev0RPRMTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.Dev0RPRMTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue Dev0RPRMTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[100].Add(Dev0RPRMTMaxTimeEnd);
                }
                if (null != res.DevHLTTime){
                    results[101] = new List<PValue>();
                    results[102] = new List<PValue>();
                    //results[112] = new List<PValue>();
                    int count = res.DevHLTTime.Count;
                    for (int j = 0; j < count; j++){
                        DateTime startTime = Convert.ToDateTime(res.DevHLTTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevHLTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[101].Add(DevHLTTime);

                        DateTime endTime = Convert.ToDateTime(res.DevHLTTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevHLTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[102].Add(DevHLTTimeEnd);

                        //PValue DevHLTTimeVC = new PValue(res.DevHLTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[112].Add(DevHLTTimeVC);
                    }
                }
                if (null != res.DevHLT){
                    results[103] = new List<PValue>();
                    PValue DevHLT = new PValue(res.DevHLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[103].Add(DevHLT);
                }
                if (null != res.DevHLTR){
                    results[104] = new List<PValue>();
                    PValue DevHLTR = new PValue(res.DevHLTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[104].Add(DevHLTR);
                }
                if (null != res.DevHLTMax){
                    results[105] = new List<PValue>();
                    PValue DevHLTMax = new PValue(res.DevHLTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[105].Add(DevHLTMax);
                }
                if (null != res.DevHLTMaxTime){
                    results[106] = new List<PValue>();
                    results[107] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevHLTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevHLTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[106].Add(DevHLTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevHLTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevHLTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[107].Add(DevHLTMaxTimeEnd);
                }
                if (null != res.DevPTTime){
                    results[108] = new List<PValue>();
                    results[109] = new List<PValue>();
                    int count = res.DevPTTime.Count;
                    for (int j = 0; j < count; j++)
                    {
                        DateTime startTime = Convert.ToDateTime(res.DevPTTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevPTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[108].Add(DevPTTime);

                        DateTime endTime = Convert.ToDateTime(res.DevPTTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevPTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[109].Add(DevPTTimeEnd);

                        //PValue DevPTTimeVC = new PValue(res.DevPTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[120].Add(DevPTTimeVC);
                    }
                }
                if (null != res.DevPT){
                    results[110] = new List<PValue>();
                    PValue DevPT = new PValue(res.DevPT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[110].Add(DevPT);
                }
                if (null != res.DevPTR){
                    results[111] = new List<PValue>();
                    PValue DevPTR = new PValue(res.DevPTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[111].Add(DevPTR);
                }
                if (null != res.DevPTRTMax){
                    results[112] = new List<PValue>();
                    PValue DevPTRTMax = new PValue(res.DevPTRTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[112].Add(DevPTRTMax);
                }
                if (null != res.DevPTRTMaxTime){
                    results[113] = new List<PValue>();
                    results[114] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevPTRTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevPTRTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[113].Add(DevPTRTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevPTRTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevPTRTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[114].Add(DevPTRTMaxTimeEnd);
                }
                if (null != res.DevNTTime){
                    results[115] = new List<PValue>();
                    results[116] = new List<PValue>();
                    int count = res.DevNTTime.Count;
                    for (int j = 0; j < count; j++) {
                        DateTime startTime = Convert.ToDateTime(res.DevNTTime[j].startDate);
                        double value = startTime.Ticks;
                        PValue DevNTTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[115].Add(DevNTTime);

                        DateTime endTime = Convert.ToDateTime(res.DevNTTime[j].endDate);
                        double endValue = endTime.Ticks;
                        PValue DevNTTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        results[116].Add(DevNTTimeEnd);

                        //PValue DevNTTimeVC = new PValue(res.DevNTTime[j].valueCount, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        //results[128].Add(DevNTTimeVC);
                    }
                }
                if (null != res.DevNT){
                    results[117] = new List<PValue>();
                    PValue DevNT = new PValue(res.DevNT, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[117].Add(DevNT);
                }
                if (null != res.DevNTR){
                    results[118] = new List<PValue>();
                    PValue DevNTR = new PValue(res.DevNTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[118].Add(DevNTR);
                }
                if (null != res.DevNTRTMax){
                    results[119] = new List<PValue>();
                    PValue DevNTRTMax = new PValue(res.DevNTRTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[119].Add(DevNTRTMax);
                }
                if (null != res.DevNTRTMaxTime){
                    results[120] = new List<PValue>();
                    results[121] = new List<PValue>();
                    DateTime startTime = Convert.ToDateTime(res.DevNTRTMaxTime.startDate);
                    double value = startTime.Ticks;
                    PValue DevNTRTMaxTime = new PValue(value, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[120].Add(DevNTRTMaxTime);
                    DateTime endTime = Convert.ToDateTime(res.DevNTRTMaxTime.endDate);
                    double endValue = endTime.Ticks;
                    PValue DevNTRTMaxTimeEnd = new PValue(endValue, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                    results[121].Add(DevNTRTMaxTimeEnd);
                }
           }
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

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

 #endregion