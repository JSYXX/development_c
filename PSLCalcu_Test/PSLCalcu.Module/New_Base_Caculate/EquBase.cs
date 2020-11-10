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
    public class EquBase : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "EquBase";
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
        private string _algorithms = "EquBase";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "L";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "Mode";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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


        private int _outputNumber = 27;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "equMin;" +
                                    "equMinN;" +
                                    "equMinT;" +
                                    "equMax;" +
                                    "equMaxN;" +
                                    "equMaxT;" +
                                    "equAvg;" +
                                    "equAvgN;" +
                                    "equdX;" +
                                    "equBulge;" +
                                    "equBulgeN;" +
                                    "equCave;" +
                                    "equCaveN;" +
                                    "equHHG;" +
                                    "equHG;" +
                                    "equHHHB;" +
                                    "equHRPB;" +
                                    "equRP0B;" +
                                    "equRM0B;" +
                                    "equRMLB;" +
                                    "equLLLB;" +
                                    "equLL;" +
                                    "equLLL;" +
                                    "equRPRMB;" +
                                    "equHLB;" +
                                    "equHHHLLLB;" +
                                    "equHHLLGL"
;




        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "MultiPVBase_Min 在 1 小时内的最小值;" +
                                        "上面最小值所在点号;" +
                                        "上面最小值发生时间;" +
                                        "MultiPVBase_Max 在 1 小时内的最大值;" +
                                        "上面最大值所在点号;" +
                                        "上面最大值发生时间;" +
                                        "MultiPVBase_Avg 在 1 小时内的平均值;" +
                                        "MultiPVBase_AvgN 在 1 小时内出现次数最多的点号;" +
                                        "上面的最大值 - 最小值;" +
                                        "MultiPVBase_Bulge 在 1 小时内的最大值;" +
                                        "上面最大值所在点号;" +
                                        "MultiPVBase_Cave 在 1 小时内的最小值;" +
                                        "上面最小值所在点号;" +
                                        "左边 MultiPVBase_HHG 平均值;" +
                                        "左边 MultiPVBase_HG 平均值;" +
                                        "左边 MultiPVBase_HHHB 平均值;" +
                                        "左边 MultiPVBase_HRPB 平均值;" +
                                        "左边 MultiPVBase_RP0B 平均值;" +
                                        "左边 MultiPVBase_RM0B 平均值;" +
                                        "左边 MultiPVBase_RMLB 平均值;" +
                                        "左边 MultiPVBase_LLLB 平均值;" +
                                        "左边 MultiPVBase_LL 平均值;" +
                                        "左边 MultiPVBase_LLL 平均值;" +
                                        "左边 MultiPVBase_RPRMB 平均值;" +
                                        "左边 MultiPVBase_HLB 平均值;" +
                                        "左边 MultiPVBase_HHHLLLB 平均值;" +
                                        "左边 MultiPVBase_HHLLGL 平均值";



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
            List<PValue>[] results = new List<PValue>[27];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
            }

            try
            {
                //读取参数

                string mode;
                string[] paras = calcuinfo.fparas.Split(';');

                mode = paras[0];




                //0.1、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                EquBaseClass newClass = new EquBaseClass();
                if (mode == "S")
                {
                    List<MetalTemperatureClass> valueList = new List<MetalTemperatureClass>();
                    foreach (List<PValue> item in inputs)
                    {
                        MetalTemperatureClass inClass = new MetalTemperatureClass();
                        inClass.Min = item[0].Value.ToString();
                        inClass.MinN = item[1].Value.ToString();
                        inClass.Max = item[2].Value.ToString();
                        inClass.MaxN = item[3].Value.ToString();
                        inClass.Avg = item[4].Value.ToString();
                        inClass.AvgN = item[5].Value.ToString();
                        inClass.dX = item[6].Value.ToString();
                        inClass.dXNR = item[7].Value.ToString();
                        inClass.dMaxB = item[8].Value.ToString();
                        inClass.dMaxBN = item[9].Value.ToString();
                        inClass.sigma = item[10].Value.ToString();
                        inClass.lk = item[11].Value.ToString();
                        inClass.lb = item[12].Value.ToString();
                        inClass.lr = item[13].Value.ToString();
                        inClass.qa = item[14].Value.ToString();
                        inClass.qb = item[15].Value.ToString();
                        inClass.qc = item[16].Value.ToString();
                        inClass.qr = item[17].Value.ToString();
                        inClass.Bulge = item[18].Value.ToString();
                        inClass.BulgeN = item[19].Value.ToString();
                        inClass.Cave = item[20].Value.ToString();
                        inClass.CaveN = item[21].Value.ToString();
                        inClass.HHG = item[22].Value.ToString();
                        inClass.HHHB = item[23].Value.ToString();
                        inClass.HRPB = item[24].Value.ToString();
                        inClass.RP0B = item[25].Value.ToString();
                        inClass.RM0B = item[26].Value.ToString();
                        inClass.RMLB = item[27].Value.ToString();
                        inClass.LLLB = item[28].Value.ToString();
                        inClass.LLL = item[29].Value.ToString();
                        inClass.RPRMB = item[30].Value.ToString();
                        inClass.HLB = item[31].Value.ToString();
                        inClass.HHHLLLB = item[32].Value.ToString();
                        inClass.HHLLGL = item[33].Value.ToString();
                        inClass.HG = item[34].Value.ToString();
                        inClass.LL = item[35].Value.ToString();
                        inClass.effectiveDateTime = item[36].Value.ToString();
                        valueList.Add(inClass);
                    }
                    newClass = EquBaseCaculate.shortEquBase(valueList);
                }
                else
                {
                    List<EquBaseClass> valueList = new List<EquBaseClass>();
                    foreach (List<PValue> item in inputs)
                    {
                        EquBaseClass inClass = new EquBaseClass();
                        inClass.equMin = item[0].Value.ToString();
                        inClass.equMinN = item[1].Value.ToString();
                        inClass.equMinT = item[2].Value.ToString();
                        inClass.equMax = item[3].Value.ToString();
                        inClass.equMaxN = item[4].Value.ToString();
                        inClass.equMaxT = item[5].Value.ToString();
                        inClass.equAvg = item[6].Value.ToString();
                        inClass.equAvgN = item[7].Value.ToString();
                        inClass.equdX = item[8].Value.ToString();
                        inClass.equBulge = item[9].Value.ToString();
                        inClass.equBulgeN = item[10].Value.ToString();
                        inClass.equCave = item[11].Value.ToString();
                        inClass.equCaveN = item[12].Value.ToString();
                        inClass.equHHG = item[13].Value.ToString();
                        inClass.equHG = item[14].Value.ToString();
                        inClass.equHHHB = item[15].Value.ToString();
                        inClass.equHRPB = item[16].Value.ToString();
                        inClass.equRP0B = item[17].Value.ToString();
                        inClass.equRM0B = item[18].Value.ToString();
                        inClass.equRMLB = item[19].Value.ToString();
                        inClass.equLLLB = item[20].Value.ToString();
                        inClass.equLL = item[21].Value.ToString();
                        inClass.equLLL = item[22].Value.ToString();
                        inClass.equRPRMB = item[23].Value.ToString();
                        inClass.equHLB = item[24].Value.ToString();
                        inClass.equHHHLLLB = item[25].Value.ToString();
                        inClass.equHHLLGL = item[26].Value.ToString();
                        valueList.Add(inClass);
                    }
                    newClass = EquBaseCaculate.longEquBase(valueList);
                }
                results[0].Add(new PValue(Convert.ToDouble(newClass.equMin), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equMinN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equMinT), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equMax), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equMaxN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equMaxT), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equAvg), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equAvgN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equdX), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equBulge), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equBulgeN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equCave), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equCaveN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHHG), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHG), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHHHB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHRPB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equRP0B), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equRM0B), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equRMLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equLLLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equLL), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equLLL), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equRPRMB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHHHLLLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(Convert.ToDouble(newClass.equHHLLGL), calcuinfo.fstarttime, calcuinfo.fendtime, 0));

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
}
