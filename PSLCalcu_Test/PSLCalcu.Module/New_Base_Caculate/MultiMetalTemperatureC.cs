using PCCommon;
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
    public class MultiMetalTemperatureC : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MultiMetalTemperatureC";
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
        private string _algorithms = "MultiMetalTemperatureC";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
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


        private int _outputNumber = 36;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Min;" +
                                        "MinN;" +
                                        "Max;" +
                                        "MaxN;" +
                                        "Avg;" +
                                        "AvgN;" +
                                        "dX;" +
                                        "dXNR;" +
                                        "dMaxB;" +
                                        "dMaxBN;" +
                                        "Bulge;" +
                                        "BulgeN;" +
                                        "Cave;" +
                                        "CaveN;" +
                                        "HHG;" +
                                        "HG;" +
                                        "HHHB;" +
                                        "HRPB;" +
                                        "RP0B;" +
                                        "RM0B;" +
                                        "RMLB;" +
                                        "LLLB;" +
                                        "LL;" +
                                        "LLL;" +
                                        "RPRMB;" +
                                        "HLB;" +
                                        "HHHLLLB;" +
                                        "HHLLGL;" +
                                        "sigma;" +
                                        "lk;" +
                                        "lb;" +
                                        "lr;" +
                                        "qa;" +
                                        "qb;" +
                                        "qc;" +
                                        "qr";




        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "最小;" +
                                        "最小点号;" +
                                        "最大;" +
                                        "最大点号;" +
                                        "平均值;" +
                                        "特征点号;" +
                                        "值极差;" +
                                        "号差比;" +
                                        "相邻最大差;" +
                                        "相邻最大差点号;" +
                                        "最凸值;" +
                                        "最凸点;" +
                                        "最凹值;" +
                                        "最凹点;" +
                                        "＞ HH 点数比;" +
                                        "＞ H 点数比;" +
                                        "HH - H 点数比;" +
                                        "H - R+ 点数比;" +
                                        "R+ - 0 点数比;" +
                                        "0 - R- 点数比;" +
                                        "R- - L 点数比;" +
                                        "L - LL 点数比;" +
                                        "＜ L 点数比;" +
                                        "＜ LL 点数比;" +
                                        "R- - R+ 点数比;" +
                                        "H - L 点数比;" +
                                        "HH - H，L - LL 点数比;" +
                                        "＞ HH，＜ LL 点数比;" +
                                        "标准差;" +
                                        "线性 k;" +
                                        "线性 b;" +
                                        "线性 r;" +
                                        "二次 a;" +
                                        "二次 b;" +
                                        "二次 c;" +
                                        "二次 r";


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
            List<PValue>[] results = new List<PValue>[36];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
            }

            try
            {
                //读取参数

                MDevLimitShtClass mDevMessageInClass = new MDevLimitShtClass();
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

                List<PValue> input = new List<PValue>();
                foreach (List<PValue> item in inputs)
                {
                    input.Add(item[0]);
                }


                //0.1、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                MetalTemperatureClass newClass = MultiMetalTemperature.MultiMetalTemperatureCaculate(input, LimitHH, LimitH, LimitRP, LimitOO, LimitRN, LimitL, LimitLL);
                results[0].Add(new PValue(Convert.ToDouble(newClass.Min), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[1].Add(new PValue(Convert.ToDateTime(newClass.MinN).Ticks, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[2].Add(new PValue(Convert.ToDouble(newClass.Max), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[3].Add(new PValue(Convert.ToDateTime(newClass.MaxN).Ticks, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[4].Add(new PValue(Convert.ToDouble(newClass.Avg), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[5].Add(new PValue(Convert.ToDouble(newClass.AvgN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[6].Add(new PValue(Convert.ToDouble(newClass.dX), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[7].Add(new PValue(Convert.ToDouble(newClass.dXNR), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[8].Add(new PValue(Convert.ToDouble(newClass.dMaxB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[9].Add(new PValue(Convert.ToDouble(newClass.dMaxBN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[10].Add(new PValue(Convert.ToDouble(newClass.sigma), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[11].Add(new PValue(Convert.ToDouble(newClass.lk), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[12].Add(new PValue(Convert.ToDouble(newClass.lb), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[13].Add(new PValue(Convert.ToDouble(newClass.lr), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[14].Add(new PValue(Convert.ToDouble(newClass.qa), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[15].Add(new PValue(Convert.ToDouble(newClass.qb), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[16].Add(new PValue(Convert.ToDouble(newClass.qc), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[17].Add(new PValue(Convert.ToDouble(newClass.qr), calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[18].Add(new PValue(Convert.ToDouble(newClass.Bulge), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[19].Add(new PValue(Convert.ToDouble(newClass.BulgeN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[20].Add(new PValue(Convert.ToDouble(newClass.Cave), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[21].Add(new PValue(Convert.ToDouble(newClass.CaveN), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[22].Add(new PValue(Convert.ToDouble(newClass.HHG), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[23].Add(new PValue(Convert.ToDouble(newClass.HHHB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[24].Add(new PValue(Convert.ToDouble(newClass.HRPB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[25].Add(new PValue(Convert.ToDouble(newClass.RP0B), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[26].Add(new PValue(Convert.ToDouble(newClass.RM0B), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[27].Add(new PValue(Convert.ToDouble(newClass.RMLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[28].Add(new PValue(Convert.ToDouble(newClass.LLLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[29].Add(new PValue(Convert.ToDouble(newClass.LLL), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[30].Add(new PValue(Convert.ToDouble(newClass.RPRMB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[31].Add(new PValue(Convert.ToDouble(newClass.HLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[32].Add(new PValue(Convert.ToDouble(newClass.HHHLLLB), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[33].Add(new PValue(Convert.ToDouble(newClass.HHLLGL), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[34].Add(new PValue(Convert.ToDouble(newClass.HG), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[35].Add(new PValue(Convert.ToDouble(newClass.LL), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
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
