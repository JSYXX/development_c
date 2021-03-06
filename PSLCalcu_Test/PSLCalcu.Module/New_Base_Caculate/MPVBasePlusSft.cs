﻿using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.BLL;
using PSLCalcu.Module.Helper;
using PSLCalcu.Module.NewCaculate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class MPVBasePlusSft : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MPVBasePlusSft";
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
        private string _algorithms = "MPVBasePlusSft";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYY";
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


        private int _outputNumber = 16;
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
                                    "PVBSum;" +
                                    "PVBSumkb;" +
                                    "PVBAbsSum;" +
                                    "PVBStbTR;" +
                                    "PVBNoStbTR;" +
                                    "PVBSDMax;" +
                                    "PVBSDMaxTime;" +
                                    "PVBDN1Num;" +
                                    "PVBDN2Num;" +
                                    "PVBDN3Num;" +
                                    "PVBTNum";




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
                                         "和;" +
                                         "上面的和乘 k 加 b;" +
                                         "绝对值和;" +
                                         "稳定时间占比，稳定：|Δxi| ≤ StbL;" +
                                         "不稳定时间占比，稳定：|Δxi| ＞ NoStbL;" +
                                         "单次极差最大值。Max(|Δxi|)。极差：指 x 所经历的全部峰 - 谷差的绝对值。计算新的 | Δx |，与原 TΔx 比较;" +
                                         "单次极差最大值，发生时刻;" +
                                         "极差大于 N1 次数。若 x > N1，Tx = Tx + 1;" +
                                         "极差大于 N2 次数，包含 N1。若 x > N2，Tx = Tx + 1;" +
                                         "极差大于 N3 次数，包含 N2、N1，若 x > N3，Tx = Tx + 1;" +
                                         "翻转次数。标准：xi-1 < xi >xi+1，或 xi-1 > xi <xi+1";

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
                MPVBasePlusSftClass mpvMessageInClass = new MPVBasePlusSftClass();
                string[] paras = calcuinfo.fparas.Split(';');

                N1 = int.Parse(paras[0]);
                N2 = int.Parse(paras[1]);
                N3 = double.Parse(paras[2]);
                k = double.Parse(paras[3]);
                b = double.Parse(paras[4]);
                stbl = double.Parse(paras[5]);
                nostbl = double.Parse(paras[6]);
                //tagId
                string type = calcuInfo.fsourtagids[0].ToString();

                List<PValue> input = new List<PValue>();
                input = inputs[0];
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
                List<string> dutyList = new List<string>();
                for (int l = 7; l < paras.Length; l += 2)
                {
                    dutyList.Add(paras[l] + ":" + paras[l + 1]);
                }
                string dutyEndTime = string.Empty;
                string dutyTime = AlgorithmBLL.getDutyConst(input[0].Timestamp, dutyList, ref dutyEndTime);
                DataSet ds = BLL.AlgorithmBLL.getSftData("psl_mpvbaseplussft", type, dutyTime);

                mpvMessageInClass.type = type;
                bool isNewAdd = false;
                if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                {
                    isNewAdd = true;
                    mpvMessageInClass.dutyTime = input[input.Count() - 1].Endtime.ToString("yyyy-MM-dd HH:mm");
                    mpvMessageInClass.PVBMin = input[input.Count() - 1].Value.ToString();
                    mpvMessageInClass.PVBMinTime = input[input.Count() - 1].Endtime.ToString();
                    mpvMessageInClass.PVBAvg = input[input.Count() - 1].Value.ToString();
                    mpvMessageInClass.PVBMax = input[input.Count() - 1].Value.ToString();
                    mpvMessageInClass.PVBMaxTime = input[input.Count() - 1].Endtime.ToString();
                    mpvMessageInClass.PVBSum = input[input.Count() - 1].Value.ToString();
                    mpvMessageInClass.PVBSumkb = (input[input.Count() - 1].Value * k + b).ToString();
                    mpvMessageInClass.PVBAbsSum = Math.Abs(input[input.Count() - 1].Value).ToString();
                    double Xi = Math.Abs(input[input.Count() - 1].Value - input[input.Count() - 2].Value);
                    if (Xi <= stbl)
                    {
                        mpvMessageInClass.PVBStbTR = "1";
                        mpvMessageInClass.PVBNoStbTR = "0";
                    }
                    else if (Xi > nostbl)
                    {
                        mpvMessageInClass.PVBStbTR = "0";
                        mpvMessageInClass.PVBNoStbTR = "1";
                    }
                    else
                    {
                        mpvMessageInClass.PVBStbTR = "0";
                        mpvMessageInClass.PVBNoStbTR = "0";
                    }
                    mpvMessageInClass.PVBSDMax = Xi.ToString();
                    mpvMessageInClass.PVBSDSingle = Xi.ToString();
                    if (input[input.Count() - 1].Value - input[input.Count() - 2].Value < 0)
                    {
                        mpvMessageInClass.PVBSDSingleType = "2";
                    }
                    else if (input[input.Count() - 1].Value - input[input.Count() - 2].Value > 0)
                    {
                        mpvMessageInClass.PVBSDSingleType = "1";
                    }
                    else
                    {
                        mpvMessageInClass.PVBSDSingleType = "0";
                    }
                    mpvMessageInClass.PVBDN1Num = "0";
                    mpvMessageInClass.PVBDN2Num = "0";
                    mpvMessageInClass.PVBDN3Num = "0";
                    mpvMessageInClass.PVBTNum = "0";
                    mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");
                    mpvMessageInClass.UpdateTime = input[input.Count() - 1].Endtime.ToString("yyyy-MM-dd HH:mm");
                    mpvMessageInClass.EffectiveCount = "2";
                }
                else
                {
                    MPVBasePlusSftClass mpvClass = new MPVBasePlusSftClass();
                    mpvClass.PVBMin = ds.Tables[0].Rows[0]["PVBMin"].ToString();
                    mpvClass.PVBMinTime = ds.Tables[0].Rows[0]["PVBMinTime"].ToString();
                    mpvClass.PVBAvg = ds.Tables[0].Rows[0]["PVBAvg"].ToString();
                    mpvClass.PVBMax = ds.Tables[0].Rows[0]["PVBMax"].ToString();
                    mpvClass.PVBMaxTime = ds.Tables[0].Rows[0]["PVBMaxTime"].ToString();
                    mpvClass.PVBSum = ds.Tables[0].Rows[0]["PVBSum"].ToString();
                    mpvClass.PVBSumkb = ds.Tables[0].Rows[0]["PVBSumkb"].ToString();
                    mpvClass.PVBAbsSum = ds.Tables[0].Rows[0]["PVBAbsSum"].ToString();
                    mpvClass.PVBStbTR = ds.Tables[0].Rows[0]["PVBStbTR"].ToString();
                    mpvClass.PVBNoStbTR = ds.Tables[0].Rows[0]["PVBNoStbTR"].ToString();
                    mpvClass.UpdateTime = ds.Tables[0].Rows[0]["UpdateTime"].ToString();
                    mpvClass.EffectiveCount = ds.Tables[0].Rows[0]["EffectiveCount"].ToString();
                    mpvClass.PVBSDMax = ds.Tables[0].Rows[0]["PVBSDMax"].ToString();
                    mpvClass.PVBSDMaxTime = ds.Tables[0].Rows[0]["PVBSDMaxTime"].ToString();
                    mpvClass.PVBDN1Num = ds.Tables[0].Rows[0]["PVBDN1Num"].ToString();
                    mpvClass.PVBDN2Num = ds.Tables[0].Rows[0]["PVBDN2Num"].ToString();
                    mpvClass.PVBDN3Num = ds.Tables[0].Rows[0]["PVBDN3Num"].ToString();
                    mpvClass.PVBTNum = ds.Tables[0].Rows[0]["PVBTNum"].ToString();
                    mpvClass.PVBSDSingle = ds.Tables[0].Rows[0]["PVBSDSingle"].ToString();
                    mpvClass.PVBSDSingleTime = ds.Tables[0].Rows[0]["PVBSDSingleTime"].ToString();
                    mpvClass.PVBSDSingleType = ds.Tables[0].Rows[0]["PVBSDSingleType"].ToString();

                    if (Convert.ToDouble(mpvClass.PVBMin) > input[input.Count() - 1].Value)
                    {
                        mpvMessageInClass.PVBMin = input[input.Count() - 1].Value.ToString();
                        mpvMessageInClass.PVBMinTime = input[input.Count() - 1].Endtime.ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        mpvMessageInClass.PVBMin = mpvClass.PVBMin;
                        mpvMessageInClass.PVBMinTime = mpvClass.PVBMinTime;
                    }
                    mpvMessageInClass.PVBAvg = Math.Round((Convert.ToDouble(mpvClass.PVBAvg) * Convert.ToDouble(mpvClass.EffectiveCount)) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                    if (Convert.ToDouble(mpvClass.PVBMax) < input[input.Count() - 1].Value)
                    {
                        mpvMessageInClass.PVBMax = input[input.Count() - 1].Value.ToString();
                        mpvMessageInClass.PVBMaxTime = input[input.Count() - 1].Endtime.ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        mpvMessageInClass.PVBMax = mpvClass.PVBMax;
                        mpvMessageInClass.PVBMaxTime = mpvClass.PVBMaxTime;
                    }
                    mpvMessageInClass.PVBSum = (Convert.ToDouble(mpvClass.PVBSum) + input[input.Count() - 1].Value).ToString();
                    mpvMessageInClass.PVBSumkb = (Convert.ToDouble(mpvMessageInClass.PVBSum) * k + b).ToString();
                    mpvMessageInClass.PVBAbsSum = (Convert.ToDouble(mpvClass.PVBAbsSum) + Math.Abs(input[input.Count() - 1].Value)).ToString();
                    double Xi = Math.Abs(input[input.Count() - 1].Value - input[input.Count() - 2].Value);
                    if (Xi <= stbl)
                    {
                        mpvMessageInClass.PVBStbTR = Math.Round((Convert.ToDouble(mpvClass.EffectiveCount) * Convert.ToDouble(mpvClass.PVBStbTR) + (double)1) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                        mpvMessageInClass.PVBNoStbTR = Math.Round((Convert.ToDouble(mpvClass.EffectiveCount) * Convert.ToDouble(mpvClass.PVBNoStbTR)) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                    }
                    else if (Xi > nostbl)
                    {
                        mpvMessageInClass.PVBStbTR = Math.Round((Convert.ToDouble(mpvClass.EffectiveCount) * Convert.ToDouble(mpvClass.PVBStbTR)) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                        mpvMessageInClass.PVBNoStbTR = Math.Round((Convert.ToDouble(mpvClass.EffectiveCount) * Convert.ToDouble(mpvClass.PVBNoStbTR) + (double)1) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                    }
                    else
                    {
                        mpvMessageInClass.PVBStbTR = Math.Round((Convert.ToDouble(mpvClass.EffectiveCount) * Convert.ToDouble(mpvClass.PVBStbTR)) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                        mpvMessageInClass.PVBNoStbTR = Math.Round((Convert.ToDouble(mpvClass.EffectiveCount) * Convert.ToDouble(mpvClass.PVBNoStbTR)) / (Convert.ToDouble(mpvClass.EffectiveCount) + (double)1), 3).ToString();
                    }

                    if (input[input.Count() - 1].Value - input[input.Count() - 2].Value < 0)
                    {
                        if (mpvClass.PVBSDSingleType == "1")
                        {
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N1)
                            {
                                mpvMessageInClass.PVBDN1Num = (Convert.ToInt32(mpvClass.PVBDN1Num) + 1).ToString();
                            }
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N2)
                            {
                                mpvMessageInClass.PVBDN2Num = (Convert.ToInt32(mpvClass.PVBDN2Num) + 1).ToString();
                            }
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N3)
                            {
                                mpvMessageInClass.PVBDN3Num = (Convert.ToInt32(mpvClass.PVBDN3Num) + 1).ToString();
                            }
                            mpvMessageInClass.PVBTNum = (Convert.ToInt32(mpvClass.PVBTNum) + 1).ToString();
                            mpvMessageInClass.PVBSDSingle = Xi.ToString();
                            mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");
                            if (Convert.ToDouble(mpvMessageInClass.PVBSDSingle) > Convert.ToDouble(mpvMessageInClass.PVBSDMax))
                            {
                                mpvMessageInClass.PVBSDMax = Xi.ToString();
                                mpvMessageInClass.PVBSDMaxTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");
                            }
                            else
                            {
                                mpvMessageInClass.PVBSDMax = mpvClass.PVBSDMax;
                                mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                            }
                        }
                        else if (mpvClass.PVBSDSingleType == "2")
                        {
                            mpvMessageInClass.PVBSDSingle = (Convert.ToDouble(mpvClass.PVBSDSingle) + Xi).ToString();
                            mpvMessageInClass.PVBSDSingleTime = mpvClass.PVBSDSingleTime;
                            if (mpvClass.PVBSDSingleTime == mpvClass.PVBSDMaxTime)
                            {
                                mpvMessageInClass.PVBSDMax = (Convert.ToDouble(mpvClass.PVBSDSingle) + Xi).ToString();
                                mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                            }
                            else
                            {
                                if (Convert.ToDouble(mpvMessageInClass.PVBSDSingle) > Convert.ToDouble(mpvMessageInClass.PVBSDMax))
                                {
                                    mpvMessageInClass.PVBSDMax = (Convert.ToDouble(mpvClass.PVBSDSingle) + Xi).ToString();
                                    mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDSingleTime;
                                }
                                else
                                {
                                    mpvMessageInClass.PVBSDMax = mpvClass.PVBSDMax;
                                    mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                                }
                            }
                            mpvMessageInClass.PVBDN1Num = mpvClass.PVBDN1Num;
                            mpvMessageInClass.PVBDN2Num = mpvClass.PVBDN2Num;
                            mpvMessageInClass.PVBDN3Num = mpvClass.PVBDN3Num;
                            mpvMessageInClass.PVBTNum = mpvClass.PVBTNum;
                        }
                        else
                        {
                            mpvMessageInClass.PVBDN1Num = mpvClass.PVBDN1Num;
                            mpvMessageInClass.PVBDN2Num = mpvClass.PVBDN2Num;
                            mpvMessageInClass.PVBDN3Num = mpvClass.PVBDN3Num;
                            mpvMessageInClass.PVBTNum = mpvClass.PVBTNum;

                            mpvMessageInClass.PVBSDSingle = Xi.ToString();
                            mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");

                            mpvMessageInClass.PVBSDMax = mpvMessageInClass.PVBSDSingle;
                            mpvMessageInClass.PVBSDMaxTime = mpvMessageInClass.PVBSDSingleTime;

                        }
                        mpvMessageInClass.PVBSDSingleType = "2";
                    }
                    else if (input[input.Count() - 1].Value - input[input.Count() - 2].Value > 0)
                    {
                        if (mpvClass.PVBSDSingleType == "1")
                        {
                            mpvMessageInClass.PVBSDSingle = (Convert.ToDouble(mpvClass.PVBSDSingle) + Xi).ToString();
                            mpvMessageInClass.PVBSDSingleTime = mpvClass.PVBSDSingleTime;
                            if (mpvClass.PVBSDSingleTime == mpvClass.PVBSDMaxTime)
                            {
                                mpvMessageInClass.PVBSDMax = (Convert.ToDouble(mpvClass.PVBSDSingle) + Xi).ToString();
                                mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                            }
                            else
                            {
                                if (Convert.ToDouble(mpvMessageInClass.PVBSDSingle) > Convert.ToDouble(mpvMessageInClass.PVBSDMax))
                                {
                                    mpvMessageInClass.PVBSDMax = (Convert.ToDouble(mpvClass.PVBSDSingle) + Xi).ToString();
                                    mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDSingleTime;
                                }
                                else
                                {
                                    mpvMessageInClass.PVBSDMax = mpvClass.PVBSDMax;
                                    mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                                }
                            }
                            mpvMessageInClass.PVBDN1Num = mpvClass.PVBDN1Num;
                            mpvMessageInClass.PVBDN2Num = mpvClass.PVBDN2Num;
                            mpvMessageInClass.PVBDN3Num = mpvClass.PVBDN3Num;
                            mpvMessageInClass.PVBTNum = mpvClass.PVBTNum;
                        }
                        else if (mpvClass.PVBSDSingleType == "2")
                        {
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N1)
                            {
                                mpvMessageInClass.PVBDN1Num = (Convert.ToInt32(mpvClass.PVBDN1Num) + 1).ToString();
                            }
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N2)
                            {
                                mpvMessageInClass.PVBDN2Num = (Convert.ToInt32(mpvClass.PVBDN2Num) + 1).ToString();
                            }
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N3)
                            {
                                mpvMessageInClass.PVBDN3Num = (Convert.ToInt32(mpvClass.PVBDN3Num) + 1).ToString();
                            }
                            mpvMessageInClass.PVBTNum = (Convert.ToInt32(mpvClass.PVBTNum) + 1).ToString();
                            mpvMessageInClass.PVBSDSingle = Xi.ToString();
                            mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");
                            if (Convert.ToDouble(mpvMessageInClass.PVBSDSingle) > Convert.ToDouble(mpvMessageInClass.PVBSDMax))
                            {
                                mpvMessageInClass.PVBSDMax = Xi.ToString();
                                mpvMessageInClass.PVBSDMaxTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");
                            }
                            else
                            {
                                mpvMessageInClass.PVBSDMax = mpvClass.PVBSDMax;
                                mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                            }
                        }
                        else
                        {
                            mpvMessageInClass.PVBDN1Num = mpvClass.PVBDN1Num;
                            mpvMessageInClass.PVBDN2Num = mpvClass.PVBDN2Num;
                            mpvMessageInClass.PVBDN3Num = mpvClass.PVBDN3Num;
                            mpvMessageInClass.PVBTNum = mpvClass.PVBTNum;

                            mpvMessageInClass.PVBSDSingle = Xi.ToString();
                            mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");

                            mpvMessageInClass.PVBSDMax = mpvMessageInClass.PVBSDSingle;
                            mpvMessageInClass.PVBSDMaxTime = mpvMessageInClass.PVBSDSingleTime;
                        }
                        mpvMessageInClass.PVBSDSingleType = "1";
                    }
                    else
                    {
                        if (mpvClass.PVBSDSingleType == "0")
                        {
                            mpvMessageInClass.PVBDN1Num = mpvClass.PVBDN1Num;
                            mpvMessageInClass.PVBDN2Num = mpvClass.PVBDN2Num;
                            mpvMessageInClass.PVBDN3Num = mpvClass.PVBDN3Num;
                            mpvMessageInClass.PVBTNum = mpvClass.PVBTNum;

                            mpvMessageInClass.PVBSDSingle = "0";
                            mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");

                            mpvMessageInClass.PVBSDMax = mpvClass.PVBSDMax;
                            mpvMessageInClass.PVBSDMaxTime = mpvClass.PVBSDMaxTime;
                        }
                        else
                        {
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N1)
                            {
                                mpvMessageInClass.PVBDN1Num = (Convert.ToInt32(mpvClass.PVBDN1Num) + 1).ToString();
                            }
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N2)
                            {
                                mpvMessageInClass.PVBDN2Num = (Convert.ToInt32(mpvClass.PVBDN2Num) + 1).ToString();
                            }
                            if (Convert.ToDouble(mpvClass.PVBSDSingle) > N3)
                            {
                                mpvMessageInClass.PVBDN3Num = (Convert.ToInt32(mpvClass.PVBDN3Num) + 1).ToString();
                            }
                            mpvMessageInClass.PVBTNum = (Convert.ToInt32(mpvClass.PVBTNum) + 1).ToString();
                            mpvMessageInClass.PVBSDSingle = "0";
                            mpvMessageInClass.PVBSDSingleTime = input[input.Count() - 2].Endtime.ToString("yyyy-MM-dd HH:mm");

                            mpvMessageInClass.PVBSDMax = mpvClass.PVBSDMax;
                        }
                        mpvMessageInClass.PVBSDSingleType = "0";
                    }

                    mpvMessageInClass.UpdateTime = input[1].Endtime.ToString();
                    mpvMessageInClass.EffectiveCount = (Convert.ToInt32(mpvClass.EffectiveCount) + 1).ToString();
                }
                //初始化
                //results[0].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBMin), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[1].Add(new PValue(UniverHelper.ConvertDateTimeTolong(mpvMessageInClass.dutyTime), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[2].Add(new PValue(Convert.ToDouble(mpvMessageInClass.type), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[3].Add(new PValue(UniverHelper.ConvertDateTimeTolong(mpvMessageInClass.PVBMinTime), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[4].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBAvg), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[5].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[6].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBMaxTime), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[7].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSum), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[8].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSumkb), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[9].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBAbsSum), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[10].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBStbTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[11].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBNoStbTR), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[12].Add(new PValue(UniverHelper.ConvertDateTimeTolong(mpvMessageInClass.UpdateTime), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[13].Add(new PValue(Convert.ToDouble(mpvMessageInClass.EffectiveCount), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[14].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSDMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[15].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSDMaxTime), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[16].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBDN1Num), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[17].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBDN2Num), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[18].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBDN3Num), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[19].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBTNum), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[20].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSDSingle), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[21].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSDSingleTime), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //results[22].Add(new PValue(Convert.ToDouble(mpvMessageInClass.PVBSDSingleType), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                //计算结果存入数据库
                bool isok = BLL.AlgorithmBLL.insertMPVBasePlusSft(mpvMessageInClass, isNewAdd);
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
