using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon;
using Config;                           //使用log
using System.Text.RegularExpressions;   //使用正则表达式


namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备整体特征值计算   
    /// 
    /// ——MMTAUnitEVSOR
    /// ——算法周期仅在小时周期上进行计算。   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///    
    ///		2018.07.30 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.08.31</date>
    /// </author> 
    /// </summary>
    public class MMTAUnitEVSOR : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：设备整体温度统计值得分及排序 ;

        private string _moduleName = "MMTAUnitEVSOR";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备整体温度统计值得分及排序";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 0;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "设备整体各种统计值";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAUnitEVSOR";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY" +
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY" +
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"
                                         ;
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "30;5";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "设备总数;考察指标个数";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){1}$");      //第一个数字描述读取计算模块的种类数量，后面的数字匹配至少1-n次

        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 150;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "E01_Score;" + "E01_ScoreO;" + "E01_ScoreR;" +
                                      "E02_Score;" + "E02_ScoreO;" + "E02_ScoreR;" +
                                      "E03_Score;" + "E03_ScoreO;" + "E03_ScoreR;" +
                                      "E04_Score;" + "E04_ScoreO;" + "E04_ScoreR;" +
                                      "E05_Score;" + "E05_ScoreO;" + "E05_ScoreR;" +
                                      "E06_Score;" + "E06_ScoreO;" + "E06_ScoreR;" +
                                      "E07_Score;" + "E07_ScoreO;" + "E07_ScoreR;" +
                                      "E08_Score;" + "E08_ScoreO;" + "E08_ScoreR;" +
                                      "E09_Score;" + "E09_ScoreO;" + "E09_ScoreR;" +
                                      "E10_Score;" + "E10_ScoreO;" + "E10_ScoreR;" +
                                      "E11_Score;" + "E11_ScoreO;" + "E11_ScoreR;" +
                                      "E12_Score;" + "E12_ScoreO;" + "E12_ScoreR;" +
                                      "E13_Score;" + "E13_ScoreO;" + "E13_ScoreR;" +
                                      "E14_Score;" + "E14_ScoreO;" + "E14_ScoreR;" +
                                      "E15_Score;" + "E15_ScoreO;" + "E15_ScoreR;" +
                                      "E16_Score;" + "E16_ScoreO;" + "E16_ScoreR;" +
                                      "E17_Score;" + "E17_ScoreO;" + "E17_ScoreR;" +
                                      "E18_Score;" + "E18_ScoreO;" + "E18_ScoreR;" +
                                      "E19_Score;" + "E19_ScoreO;" + "E19_ScoreR;" +
                                      "E20_Score;" + "E20_ScoreO;" + "E20_ScoreR;" +
                                      "E21_Score;" + "E21_ScoreO;" + "E21_ScoreR;" +
                                      "E22_Score;" + "E22_ScoreO;" + "E22_ScoreR;" +
                                      "E23_Score;" + "E23_ScoreO;" + "E23_ScoreR;" +
                                      "E24_Score;" + "E24_ScoreO;" + "E24_ScoreR;" +
                                      "E25_Score;" + "E25_ScoreO;" + "E25_ScoreR;" +
                                      "E26_Score;" + "E26_ScoreO;" + "E26_ScoreR;" +
                                      "E27_Score;" + "E27_ScoreO;" + "E27_ScoreR;" +
                                      "E28_Score;" + "E28_ScoreO;" + "E28_ScoreR;" +
                                      "E29_Score;" + "E29_ScoreO;" + "E29_ScoreR;" +
                                      "E30_Score;" + "E30_ScoreO;" + "E30_ScoreR;" +
                                      "E31_Score;" + "E31_ScoreO;" + "E31_ScoreR;" +
                                      "E32_Score;" + "E32_ScoreO;" + "E32_ScoreR;" +
                                      "E33_Score;" + "E33_ScoreO;" + "E33_ScoreR;" +
                                      "E34_Score;" + "E34_ScoreO;" + "E34_ScoreR;" +
                                      "E35_Score;" + "E35_ScoreO;" + "E35_ScoreR;" +
                                      "E36_Score;" + "E36_ScoreO;" + "E36_ScoreR;" +
                                      "E37_Score;" + "E37_ScoreO;" + "E37_ScoreR;" +
                                      "E38_Score;" + "E38_ScoreO;" + "E38_ScoreR;" +
                                      "E39_Score;" + "E39_ScoreO;" + "E39_ScoreR;" +
                                      "E40_Score;" + "E40_ScoreO;" + "E40_ScoreR;" +
                                      "E41_Score;" + "E41_ScoreO;" + "E41_ScoreR;" +
                                      "E42_Score;" + "E42_ScoreO;" + "E42_ScoreR;" +
                                      "E43_Score;" + "E43_ScoreO;" + "E43_ScoreR;" +
                                      "E44_Score;" + "E44_ScoreO;" + "E44_ScoreR;" +
                                      "E45_Score;" + "E45_ScoreO;" + "E45_ScoreR;" +
                                      "E46_Score;" + "E46_ScoreO;" + "E46_ScoreR;" +
                                      "E47_Score;" + "E47_ScoreO;" + "E47_ScoreR;" +
                                      "E48_Score;" + "E48_ScoreO;" + "E48_ScoreR;" +
                                      "E49_Score;" + "E49_ScoreO;" + "E49_ScoreR;" +
                                      "E50_Score;" + "E50_ScoreO;" + "E50_ScoreR";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "第1号设备排名均值均值;第1号设备均值排名;第1号设备排名折算;" +
                                        "第2号设备排名均值均值;第2号设备均值排名;第2号设备排名折算;" +
                                        "第3号设备排名均值均值;第3号设备均值排名;第3号设备排名折算;" +
                                        "第4号设备排名均值均值;第4号设备均值排名;第4号设备排名折算;" +
                                        "第5号设备排名均值均值;第5号设备均值排名;第5号设备排名折算;" +
                                        "第6号设备排名均值均值;第6号设备均值排名;第6号设备排名折算;" +
                                        "第7号设备排名均值均值;第7号设备均值排名;第7号设备排名折算;" +
                                        "第8号设备排名均值均值;第8号设备均值排名;第8号设备排名折算;" +
                                        "第9号设备排名均值均值;第9号设备均值排名;第9号设备排名折算;" +
                                        "第10号设备排名均值均值;第10号设备均值排名;第10号设备排名折算;" +
                                        "第11号设备排名均值均值;第11号设备均值排名;第11号设备排名折算;" +
                                        "第12号设备排名均值均值;第12号设备均值排名;第12号设备排名折算;" +
                                        "第13号设备排名均值均值;第13号设备均值排名;第13号设备排名折算;" +
                                        "第14号设备排名均值均值;第14号设备均值排名;第14号设备排名折算;" +
                                        "第15号设备排名均值均值;第15号设备均值排名;第15号设备排名折算;" +
                                        "第16号设备排名均值均值;第16号设备均值排名;第16号设备排名折算;" +
                                        "第17号设备排名均值均值;第17号设备均值排名;第17号设备排名折算;" +
                                        "第18号设备排名均值均值;第18号设备均值排名;第18号设备排名折算;" +
                                        "第19号设备排名均值均值;第19号设备均值排名;第19号设备排名折算;" +
                                        "第20号设备排名均值均值;第20号设备均值排名;第20号设备排名折算;" +
                                        "第21号设备排名均值均值;第21号设备均值排名;第21号设备排名折算;" +
                                        "第22号设备排名均值均值;第22号设备均值排名;第22号设备排名折算;" +
                                        "第23号设备排名均值均值;第23号设备均值排名;第23号设备排名折算;" +
                                        "第24号设备排名均值均值;第24号设备均值排名;第24号设备排名折算;" +
                                        "第25号设备排名均值均值;第25号设备均值排名;第25号设备排名折算;" +
                                        "第26号设备排名均值均值;第26号设备均值排名;第26号设备排名折算;" +
                                        "第27号设备排名均值均值;第27号设备均值排名;第27号设备排名折算;" +
                                        "第28号设备排名均值均值;第28号设备均值排名;第28号设备排名折算;" +
                                        "第29号设备排名均值均值;第29号设备均值排名;第29号设备排名折算;" +
                                        "第30号设备排名均值均值;第30号设备均值排名;第30号设备排名折算;" +
                                        "第31号设备排名均值均值;第31号设备均值排名;第31号设备排名折算;" +
                                        "第32号设备排名均值均值;第32号设备均值排名;第32号设备排名折算;" +
                                        "第33号设备排名均值均值;第33号设备均值排名;第33号设备排名折算;" +
                                        "第34号设备排名均值均值;第34号设备均值排名;第34号设备排名折算;" +
                                        "第35号设备排名均值均值;第35号设备均值排名;第35号设备排名折算;" +
                                        "第36号设备排名均值均值;第36号设备均值排名;第36号设备排名折算;" +
                                        "第37号设备排名均值均值;第37号设备均值排名;第37号设备排名折算;" +
                                        "第38号设备排名均值均值;第38号设备均值排名;第38号设备排名折算;" +
                                        "第39号设备排名均值均值;第39号设备均值排名;第39号设备排名折算;" +
                                        "第40号设备排名均值均值;第40号设备均值排名;第40号设备排名折算;" +
                                        "第41号设备排名均值均值;第41号设备均值排名;第41号设备排名折算;" +
                                        "第42号设备排名均值均值;第42号设备均值排名;第42号设备排名折算;" +
                                        "第43号设备排名均值均值;第43号设备均值排名;第43号设备排名折算;" +
                                        "第44号设备排名均值均值;第44号设备均值排名;第44号设备排名折算;" +
                                        "第45号设备排名均值均值;第45号设备均值排名;第45号设备排名折算;" +
                                        "第46号设备排名均值均值;第46号设备均值排名;第46号设备排名折算;" +
                                        "第47号设备排名均值均值;第47号设备均值排名;第47号设备排名折算;" +
                                        "第48号设备排名均值均值;第48号设备均值排名;第48号设备排名折算;" +
                                        "第49号设备排名均值均值;第49号设备均值排名;第49号设备排名折算;" +
                                        "第50号设备排名均值均值;第50号设备均值排名;第50号设备排名折算"
                                        ;
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
        //3个输入参数标签顺序：考核标签、参考标签1（期望曲线X轴对应标签）、参考标签2（期望曲线Y轴对应标签）
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

        #region 计算模块
        /// <summary>
        /// 计算模块算法实现:依据期望曲线求期望值、偏差、偏差比，在依据记分曲线求得分
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>计算结果分别为OPX;OPXE;OPXErate;OPXW</returns>

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
            List<PValue>[] results = new List<PValue>[150];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                const int MAXNUMBER_IN_Unit = 50;                     //允许的最大设备数
                //获得参数
                string[] paras = calcuinfo.fparas.Split(';');
                int EquNumberInUnit = int.Parse(paras[0]);             //参数第一个值是总设备数
                int ParaNumber = int.Parse(paras[1]);                         //每个设备参与考察的指标数量                

                if (EquNumberInUnit > MAXNUMBER_IN_Unit)
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("计算参数中指定的设备数超过最大允许设备数{0}，请检查计算参数配置！", MAXNUMBER_IN_Unit);
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0、输入:输入数据为各设备特征值小时统计结果。
                //——
                int totalInputNumber = ParaNumber * EquNumberInUnit; ;   //根据参数计算出的需要的输入数据的总数量 

                //0.1、输入处理：输入长度。当输入为空或者长度为0时，给出标志位为StatusConst.InputIsNull的计算结果.                
                if (inputs == null || inputs.Length == 0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                if (inputs.Length != totalInputNumber)  //只要输入与参数计算的总数不一致，根本无法判断究竟是多了那个点，少了那个点。此时就需要报错，检查计算组态配置问题，看看参数配置的是否有问题
                {
                    _errorFlag = true;
                    _errorInfo = "输入数据的数量与需要的数量不符，请检查输入标签数量和计算项参数中设备数量及参与评价的设备参数数量！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0.2、取出的是每个设备多个参数的小时值。
                //——每个参数，在所有设备中进行排序。
                //——计算每个设备各参数的排名均值
                //——计算这些均值的排名
                //——对排名进行折算
                bool[] UnitEquFlag = new bool[EquNumberInUnit];
                double[] UnitEquScore = new double[EquNumberInUnit];
                double[] UnitEquScoreO = new double[EquNumberInUnit];
                double[] UnitEquScoreR = new double[EquNumberInUnit];

                //组织计算结果
                long status = 0;
                for (i = 0; i < EquNumberInUnit; i++) //最大可以接受50个点，但按实际点数给个点计算结果赋值
                {
                    //status = (long)StatusConst.InvalidPoint;
                    results[i * 3] = new List<PValue>();
                    results[i * 3].Add(new PValue(UnitEquScore[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));

                    results[i * 3 + 1] = new List<PValue>();
                    results[i * 3 + 1].Add(new PValue(UnitEquScoreO[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));

                    results[i * 3 + 2] = new List<PValue>();
                    results[i * 3 + 2].Add(new PValue(UnitEquScoreR[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));
                }
                //超出实际点的计算结果，按初始化时的值返回。不会写入计算结果。
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
        #endregion
    }
}
