﻿using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.BLL;
using PSLCalcu.Module.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class MDevLimitSft : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MDevLimitSft";
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
        private string _algorithms = "MDevLimitSft";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYY";
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
        private string _moduleParaDesc = "LimitHH;LimitH;LimitRP;LimitOO;LimitRN;LimitL;LimitLL";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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


        private int _outputNumber = 17;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "DevHLHHLLR;" +
                                    "DevHHR;" +
                                    "DevHR;" +
                                    "DevRPR;" +
                                    "Dev0PR;" +
                                    "Dev0NR;" +
                                    "DevRNR;" +
                                    "DevLR;" +
                                    "DevLLR;" +
                                    "Dev0HTR;" +
                                    "Dev0LR;" +
                                    "DevHHLLTR;" +
                                    "DevRPRMHLTR;" +
                                    "Dev0RPRMTR;" +
                                    "DevHLTR;" +
                                    "UpdateTime;" +
                                    "EffectiveCount";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "在越限段总时间占比 %。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 HH 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 H 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 R+ 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 0P 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 0N 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 RN 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 L 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "越 LL 时间占比，%。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "在正常，正段之内时间占比 %。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "在正常，负段之内时间占比 %。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "在超限段总时间占比。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "在正常，非优秀段总时间占比 %。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "在优秀段总时间占比。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                        "在正常段（含优秀）总时间占比。判断新值 x 是否满足在越限段的条件。若满足：Tx = (Tx / 100 * T + 1) / (T + 1) * 100。T：到上次采样的时间长;" +
                                         "更新时间;" +
                                         "有效数据个数";

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
            List<PValue>[] results = new List<PValue>[16];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //读取参数

                double N1, N2, N3, k, b, stbl, nostbl;
                string mode;
                MDevLimitShtClass mDevMessageInClass = new MDevLimitShtClass();
                double LimitHH, LimitH, LimitRP, LimitOO, LimitRN, LimitL, LimitLL;
                string[] paras = calcuinfo.fparas.Split(';');

                LimitHH = int.Parse(paras[0]);
                LimitH = int.Parse(paras[1]);
                LimitRP = double.Parse(paras[2]);
                LimitOO = double.Parse(paras[3]);
                LimitRN = double.Parse(paras[4]);
                LimitL = double.Parse(paras[5]);
                LimitLL = double.Parse(paras[6]);
                List<PValue> input = new List<PValue>();
                input = inputs[0];
                //tagId
                string type = calcuInfo.fsourtagids[0].ToString();
                List<string> dutyList = new List<string>();
                for (int l = 7; l < paras.Length; l += 2)
                {
                    dutyList.Add(paras[l] + ":" + paras[l + 1]);
                }




                //0.1、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。 
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.2、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                string dutyEndTime = string.Empty;
                string dutyTime = AlgorithmBLL.getDutyConst(input[0].Timestamp, dutyList, ref dutyEndTime);
                DataSet ds = BLL.AlgorithmBLL.getSftData("psl_mdevlimitsft", type, dutyTime);
                for (int l = 0; l < ds.Tables[1].Rows.Count; l++)
                {
                    PValue newClass = new PValue();

                }
                mDevMessageInClass.type = type;
                bool isNewAdd = false;
                if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                {
                    isNewAdd = true;
                    mDevMessageInClass.dutyTime = dutyTime;
                    double x = Convert.ToDouble(input[input.Count() - 1].Value.ToString());
                    int DevHLHHLLT = 0;
                    if ((LimitHH > x && x >= LimitH) || (LimitL >= x && x > LimitLL))
                    {
                        DevHLHHLLT++;
                    }
                    int DevHHT = 0;
                    if (x > LimitHH)
                    {
                        DevHHT++;
                    }
                    int DevHT = 0;
                    if (LimitHH > x && x >= LimitH)
                    {
                        DevHT++;
                    }
                    int DevRPT = 0;
                    if (LimitH > x && x >= LimitRP)
                    {
                        DevRPT++;
                    }
                    int Dev0PT = 0;
                    if (LimitRP > x && x >= LimitOO)
                    {
                        Dev0PT++;
                    }
                    int Dev0NT = 0;
                    if (LimitOO > x && x >= LimitRN)
                    {
                        Dev0NT++;
                    }
                    int DevRNT = 0;
                    if (LimitRN > x && x >= LimitL)
                    {
                        DevRNT++;
                    }
                    int DevLT = 0;
                    if (LimitL > x && x >= LimitLL)
                    {
                        DevLT++;
                    }
                    int DevLLT = 0;
                    if (LimitLL > x)
                    {
                        DevLLT++;
                    }
                    int Dev0HT = 0;
                    if (LimitH > x && x >= LimitOO)
                    {
                        Dev0HT++;
                    }
                    int Dev0L = 0;
                    if (LimitOO > x && x > LimitL)
                    {
                        Dev0L++;
                    }
                    int DevHHLLT = 0;
                    if (x <= LimitLL || x >= LimitHH)
                    {
                        DevHHLLT++;
                    }
                    int DevRPRMHLT = 0;
                    if ((LimitH > x && x >= LimitRP) || (LimitRN >= x && x > LimitL))
                    {
                        DevRPRMHLT++;
                    }
                    int Dev0RPRMT = 0;
                    if (LimitRP > x && x > LimitRN)
                    {
                        Dev0RPRMT++;
                    }
                    int DevHLT = 0;
                    if (LimitH > x && x > LimitL)
                    {
                        DevHLT++;
                    }
                    mDevMessageInClass.DevHLHHLLR = MathHelper.returnProportionStr(DevHLHHLLT, 1);
                    mDevMessageInClass.DevHHR = MathHelper.returnProportionStr(DevHHT, 1);
                    mDevMessageInClass.DevHR = MathHelper.returnProportionStr(DevHT, 1);
                    mDevMessageInClass.DevRPR = MathHelper.returnProportionStr(DevRPT, 1);
                    mDevMessageInClass.Dev0PR = MathHelper.returnProportionStr(Dev0PT, 1);
                    mDevMessageInClass.Dev0NR = MathHelper.returnProportionStr(Dev0NT, 1);
                    mDevMessageInClass.DevRNR = MathHelper.returnProportionStr(DevRNT, 1);
                    mDevMessageInClass.DevLR = MathHelper.returnProportionStr(DevLT, 1);
                    mDevMessageInClass.DevLLR = MathHelper.returnProportionStr(DevLLT, 1);
                    mDevMessageInClass.Dev0HTR = MathHelper.returnProportionStr(Dev0HT, 1);
                    mDevMessageInClass.Dev0LR = MathHelper.returnProportionStr(Dev0L, 1);
                    mDevMessageInClass.DevHHLLTR = MathHelper.returnProportionStr(DevHHLLT, 1);
                    mDevMessageInClass.DevRPRMHLTR = MathHelper.returnProportionStr(DevRPRMHLT, 1);
                    mDevMessageInClass.Dev0RPRMTR = MathHelper.returnProportionStr(Dev0RPRMT, 1);
                    mDevMessageInClass.DevHLTR = MathHelper.returnProportionStr(DevHLT, 1);
                    mDevMessageInClass.UpdateTime = input[0].Timestamp.ToString("yyyy-MM-dd HH:mm");
                    mDevMessageInClass.EffectiveCount = "1";
                }
                else
                {
                    MDevLimitShtClass mDevClass = new MDevLimitShtClass();
                    mDevClass.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    mDevClass.DevHLHHLLR = ds.Tables[0].Rows[0]["DevHLHHLLR"].ToString();
                    mDevClass.DevHHR = ds.Tables[0].Rows[0]["DevHHR"].ToString();
                    mDevClass.DevHR = ds.Tables[0].Rows[0]["DevHR"].ToString();
                    mDevClass.DevRPR = ds.Tables[0].Rows[0]["DevRPR"].ToString();
                    mDevClass.Dev0PR = ds.Tables[0].Rows[0]["Dev0PR"].ToString();
                    mDevClass.Dev0NR = ds.Tables[0].Rows[0]["Dev0NR"].ToString();
                    mDevClass.DevRNR = ds.Tables[0].Rows[0]["DevRNR"].ToString();
                    mDevClass.DevLR = ds.Tables[0].Rows[0]["DevLR"].ToString();
                    mDevClass.DevLLR = ds.Tables[0].Rows[0]["DevLLR"].ToString();
                    mDevClass.Dev0HTR = ds.Tables[0].Rows[0]["Dev0HTR"].ToString();
                    mDevClass.Dev0LR = ds.Tables[0].Rows[0]["Dev0LR"].ToString();
                    mDevClass.DevHHLLTR = ds.Tables[0].Rows[0]["DevHHLLTR"].ToString();
                    mDevClass.DevRPRMHLTR = ds.Tables[0].Rows[0]["DevRPRMHLTR"].ToString();
                    mDevClass.Dev0RPRMTR = ds.Tables[0].Rows[0]["Dev0RPRMTR"].ToString();
                    mDevClass.DevHLTR = ds.Tables[0].Rows[0]["DevHLTR"].ToString();
                    mDevClass.UpdateTime = ds.Tables[0].Rows[0]["UpdateTime"].ToString();
                    mDevClass.EffectiveCount = ds.Tables[0].Rows[0]["EffectiveCount"].ToString();


                    double x = Convert.ToDouble(input[input.Count() - 1].Value.ToString());
                    int DevHLHHLLT = 0;
                    if ((LimitHH > x && x >= LimitH) || (LimitL >= x && x > LimitLL))
                    {
                        DevHLHHLLT++;
                    }
                    int DevHHT = 0;
                    if (x > LimitHH)
                    {
                        DevHHT++;
                    }
                    int DevHT = 0;
                    if (LimitHH > x && x >= LimitH)
                    {
                        DevHT++;
                    }
                    int DevRPT = 0;
                    if (LimitH > x && x >= LimitRP)
                    {
                        DevRPT++;
                    }
                    int Dev0PT = 0;
                    if (LimitRP > x && x >= LimitOO)
                    {
                        Dev0PT++;
                    }
                    int Dev0NT = 0;
                    if (LimitOO > x && x >= LimitRN)
                    {
                        Dev0NT++;
                    }
                    int DevRNT = 0;
                    if (LimitRN > x && x >= LimitL)
                    {
                        DevRNT++;
                    }
                    int DevLT = 0;
                    if (LimitL > x && x >= LimitLL)
                    {
                        DevLT++;
                    }
                    int DevLLT = 0;
                    if (LimitLL > x)
                    {
                        DevLLT++;
                    }
                    int Dev0HT = 0;
                    if (LimitH > x && x >= LimitOO)
                    {
                        Dev0HT++;
                    }
                    int Dev0L = 0;
                    if (LimitOO > x && x > LimitL)
                    {
                        Dev0L++;
                    }
                    int DevHHLLT = 0;
                    if (x <= LimitLL || x >= LimitHH)
                    {
                        DevHHLLT++;
                    }
                    int DevRPRMHLT = 0;
                    if ((LimitH > x && x >= LimitRP) || (LimitRN >= x && x > LimitL))
                    {
                        DevRPRMHLT++;
                    }
                    int Dev0RPRMT = 0;
                    if (LimitRP > x && x > LimitRN)
                    {
                        Dev0RPRMT++;
                    }
                    int DevHLT = 0;
                    if (LimitH > x && x > LimitL)
                    {
                        DevHLT++;
                    }
                    int effectiveCount = Convert.ToInt32(mDevClass.EffectiveCount);
                    mDevMessageInClass.DevHLHHLLR = MathHelper.returnProportionUpdateStr(DevHLHHLLT, effectiveCount, mDevClass.DevHLHHLLR);
                    mDevMessageInClass.DevHHR = MathHelper.returnProportionUpdateStr(DevHHT, effectiveCount, mDevClass.DevHHR);
                    mDevMessageInClass.DevHR = MathHelper.returnProportionUpdateStr(DevHT, effectiveCount, mDevClass.DevHR);
                    mDevMessageInClass.DevRPR = MathHelper.returnProportionUpdateStr(DevRPT, effectiveCount, mDevClass.DevRPR);
                    mDevMessageInClass.Dev0PR = MathHelper.returnProportionUpdateStr(Dev0PT, effectiveCount, mDevClass.Dev0PR);
                    mDevMessageInClass.Dev0NR = MathHelper.returnProportionUpdateStr(Dev0NT, effectiveCount, mDevClass.Dev0NR);
                    mDevMessageInClass.DevRNR = MathHelper.returnProportionUpdateStr(DevRNT, effectiveCount, mDevClass.DevRNR);
                    mDevMessageInClass.DevLR = MathHelper.returnProportionUpdateStr(DevLT, effectiveCount, mDevClass.DevLR);
                    mDevMessageInClass.DevLLR = MathHelper.returnProportionUpdateStr(DevLLT, effectiveCount, mDevClass.DevLLR);
                    mDevMessageInClass.Dev0HTR = MathHelper.returnProportionUpdateStr(Dev0HT, effectiveCount, mDevClass.Dev0HTR);
                    mDevMessageInClass.Dev0LR = MathHelper.returnProportionUpdateStr(Dev0L, effectiveCount, mDevClass.Dev0LR);
                    mDevMessageInClass.DevHHLLTR = MathHelper.returnProportionUpdateStr(DevHHLLT, effectiveCount, mDevClass.DevHHLLTR);
                    mDevMessageInClass.DevRPRMHLTR = MathHelper.returnProportionUpdateStr(DevRPRMHLT, effectiveCount, mDevClass.DevRPRMHLTR);
                    mDevMessageInClass.Dev0RPRMTR = MathHelper.returnProportionUpdateStr(Dev0RPRMT, effectiveCount, mDevClass.Dev0RPRMTR);
                    mDevMessageInClass.DevHLTR = MathHelper.returnProportionUpdateStr(DevHLT, effectiveCount, mDevClass.DevHLTR);
                    mDevMessageInClass.UpdateTime = input[0].Timestamp.ToString("yyyy-MM-dd HH:mm");
                    mDevMessageInClass.EffectiveCount = (effectiveCount + 1).ToString();
                    mDevMessageInClass.id = mDevClass.id;
                }
                bool isok = BLL.AlgorithmBLL.insertMDevLimitSft(mDevMessageInClass, isNewAdd);
                if (isok)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MPVBasePlusSft录入数据失败";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

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
