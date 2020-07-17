using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备上单点温度特征值指标  
    /// 
    /// ——MMTAEquDORDay用MMTAEquDOR和MMTAEVSOR的计算结果进行综合计算
    /// ——算法周期仅在天时周期上进行计算。   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///    
    ///		2018.07.30 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.07.30</date>
    /// </author> 
    /// </summary>
    public class MMTAEquDORDay : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MMTAEquDORDay";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备上单点温度统计值特征值综合排名";
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
        private string _inputDescsCN = "设备上所有温度点MMTAEquDOR和MMTAEVSOR的计算结果";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAEquDORDay";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY" +                                         
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"
                                         ;
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "15;155;150";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "设备含有的点数;第1个算法的计算结果数量;第二个算法的计算结果数量";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){1,}$");      //第一个数字描述读取计算模块的种类数量，后面的数字匹配至少1-n次

        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 100;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "P01_ScoreDay;" + "P01_ScoreDayO;" +
                                      "P02_ScoreDay;" + "P02_ScoreDayO;" +
                                      "P03_ScoreDay;" + "P03_ScoreDayO;" +
                                      "P04_ScoreDay;" + "P04_ScoreDayO;" + 
                                      "P05_ScoreDay;" + "P05_ScoreDayO;" + 
                                      "P06_ScoreDay;" + "P06_ScoreDayO;" + 
                                      "P07_ScoreDay;" + "P07_ScoreDayO;" + 
                                      "P08_ScoreDay;" + "P08_ScoreDayO;" + 
                                      "P09_ScoreDay;" + "P09_ScoreDayO;" + 
                                      "P10_ScoreDay;" + "P10_ScoreDayO;" + 
                                      "P11_ScoreDay;" + "P11_ScoreDayO;" + 
                                      "P12_ScoreDay;" + "P12_ScoreDayO;" + 
                                      "P13_ScoreDay;" + "P13_ScoreDayO;" + 
                                      "P14_ScoreDay;" + "P14_ScoreDayO;" + 
                                      "P15_ScoreDay;" + "P15_ScoreDayO;" + 
                                      "P16_ScoreDay;" + "P16_ScoreDayO;" + 
                                      "P17_ScoreDay;" + "P17_ScoreDayO;" + 
                                      "P18_ScoreDay;" + "P18_ScoreDayO;" + 
                                      "P19_ScoreDay;" + "P19_ScoreDayO;" + 
                                      "P20_ScoreDay;" + "P20_ScoreDayO;" + 
                                      "P21_ScoreDay;" + "P21_ScoreDayO;" + 
                                      "P22_ScoreDay;" + "P22_ScoreDayO;" + 
                                      "P23_ScoreDay;" + "P23_ScoreDayO;" +
                                      "P24_ScoreDay;" + "P24_ScoreDayO;" + 
                                      "P25_ScoreDay;" + "P25_ScoreDayO;" + 
                                      "P26_ScoreDay;" + "P26_ScoreDayO;" + 
                                      "P27_ScoreDay;" + "P27_ScoreDayO;" + 
                                      "P28_ScoreDay;" + "P28_ScoreDayO;" + 
                                      "P29_ScoreDay;" + "P29_ScoreDayO;" +
                                      "P30_ScoreDay;" + "P30_ScoreDayO;" + 
                                      "P31_ScoreDay;" + "P31_ScoreDayO;" + 
                                      "P32_ScoreDay;" + "P32_ScoreDayO;" + 
                                      "P33_ScoreDay;" + "P33_ScoreDayO;" + 
                                      "P34_ScoreDay;" + "P34_ScoreDayO;" + 
                                      "P35_ScoreDay;" + "P35_ScoreDayO;" + 
                                      "P36_ScoreDay;" + "P36_ScoreDayO;" + 
                                      "P37_ScoreDay;" + "P37_ScoreDayO;" + 
                                      "P38_ScoreDay;" + "P38_ScoreDayO;" + 
                                      "P39_ScoreDay;" + "P39_ScoreDayO;" + 
                                      "P40_ScoreDay;" + "P40_ScoreDayO;" + 
                                      "P41_ScoreDay;" + "P41_ScoreDayO;" +
                                      "P42_ScoreDay;" + "P42_ScoreDayO;" + 
                                      "P43_ScoreDay;" + "P43_ScoreDayO;" + 
                                      "P44_ScoreDay;" + "P44_ScoreDayO;" + 
                                      "P45_ScoreDay;" + "P45_ScoreDayO;" + 
                                      "P46_ScoreDay;" + "P46_ScoreDayO;" + 
                                      "P47_ScoreDay;" + "P47_ScoreDayO;" + 
                                      "P48_ScoreDay;" + "P48_ScoreDayO;" + 
                                      "P49_ScoreDay;" + "P49_ScoreDayO;" + 
                                      "P50_ScoreDay;" + "P50_ScoreDayO" 
                                      ;



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "第1点各项指标排名得分日均值;第1点日均值的排名;" +
                                        "第2点各项指标排名得分日均值;第2点日均值的排名;" +
                                        "第3点各项指标排名得分日均值;第3点日均值的排名;" +
                                        "第4点各项指标排名得分日均值;第4点日均值的排名;" +
                                        "第5点各项指标排名得分日均值;第5点日均值的排名;" +
                                        "第6点各项指标排名得分日均值;第6点日均值的排名;" +
                                        "第7点各项指标排名得分日均值;第7点日均值的排名;" +
                                        "第8点各项指标排名得分日均值;第8点日均值的排名;" +
                                        "第9点各项指标排名得分日均值;第9点日均值的排名;" +
                                        "第10点各项指标排名得分日均值;第10点日均值的排名;" +
                                        "第11点各项指标排名得分日均值;第11点日均值的排名;" +
                                        "第12点各项指标排名得分日均值;第12点日均值的排名;" +
                                        "第13点各项指标排名得分日均值;第13点日均值的排名;" +
                                        "第14点各项指标排名得分日均值;第14点日均值的排名;" +
                                        "第15点各项指标排名得分日均值;第15点日均值的排名;" +
                                        "第16点各项指标排名得分日均值;第16点日均值的排名;" +
                                        "第17点各项指标排名得分日均值;第17点日均值的排名;" +
                                        "第18点各项指标排名得分日均值;第18点日均值的排名;" +
                                        "第19点各项指标排名得分日均值;第19点日均值的排名;" +
                                        "第20点各项指标排名得分日均值;第20点日均值的排名;" +
                                        "第21点各项指标排名得分日均值;第21点日均值的排名;" +
                                        "第22点各项指标排名得分日均值;第22点日均值的排名;" +
                                        "第23点各项指标排名得分日均值;第23点日均值的排名;" +
                                        "第24点各项指标排名得分日均值;第24点日均值的排名;" +
                                        "第25点各项指标排名得分日均值;第25点日均值的排名;" +
                                        "第26点各项指标排名得分日均值;第26点日均值的排名;" +
                                        "第27点各项指标排名得分日均值;第27点日均值的排名;" +
                                        "第28点各项指标排名得分日均值;第28点日均值的排名;" +
                                        "第29点各项指标排名得分日均值;第29点日均值的排名;" +
                                        "第30点各项指标排名得分日均值;第30点日均值的排名;" +
                                        "第31点各项指标排名得分日均值;第31点日均值的排名;" +
                                        "第32点各项指标排名得分日均值;第32点日均值的排名;" +
                                        "第33点各项指标排名得分日均值;第33点日均值的排名;" +
                                        "第34点各项指标排名得分日均值;第34点日均值的排名;" +
                                        "第35点各项指标排名得分日均值;第35点日均值的排名;" +
                                        "第36点各项指标排名得分日均值;第36点日均值的排名;" +
                                        "第37点各项指标排名得分日均值;第37点日均值的排名;" +
                                        "第38点各项指标排名得分日均值;第38点日均值的排名;" +
                                        "第39点各项指标排名得分日均值;第39点日均值的排名;" +
                                        "第40点各项指标排名得分日均值;第40点日均值的排名;" +
                                        "第41点各项指标排名得分日均值;第41点日均值的排名;" +
                                        "第42点各项指标排名得分日均值;第42点日均值的排名;" +
                                        "第43点各项指标排名得分日均值;第43点日均值的排名;" +
                                        "第44点各项指标排名得分日均值;第44点日均值的排名;" +
                                        "第45点各项指标排名得分日均值;第45点日均值的排名;" +
                                        "第46点各项指标排名得分日均值;第46点日均值的排名;" +
                                        "第47点各项指标排名得分日均值;第47点日均值的排名;" +
                                        "第48点各项指标排名得分日均值;第48点日均值的排名;" +
                                        "第49点各项指标排名得分日均值;第49点日均值的排名;" +
                                        "第50点各项指标排名得分日均值;第50点日均值的排名"
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
            List<PValue>[] results = new List<PValue>[100];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //说明：ReadMulti接口，对于所要读取的标签，如果在指定的计算周期内找不到值，则该标签对应的List<PValue>不为null，而是count=0
                const int MAXNUMBER_IN_DEVICE = 50;                     //一个设备中允许设备测点的最大值
                //获得参数
                string[] paras = calcuinfo.fparas.Split(';');
                int PointNumberInEqu = int.Parse(paras[0]);             //参数第一个值是设备包含的点数                
                int calcuModuleNumber = paras.Length - 1;               //参数中后面是要输入的每个算法的计算结果数量
                int[] ModuleOuptNumber = new int[calcuModuleNumber];    //各算法计算结果数量               
                for (i = 0; i < paras.Length - 1; i++)
                {
                    ModuleOuptNumber[i] = int.Parse(paras[1 + i]);                    
                }
                if (PointNumberInEqu > MAXNUMBER_IN_DEVICE)
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("计算参数中指定的设备点数超过最大允许点数{0}，请检查计算参数配置！", MAXNUMBER_IN_DEVICE);
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); 
                }
                //先截取结算结果
                results = results.Take(PointNumberInEqu * 2).ToArray();

                //0、输入:输入数据为设备上所有单点一个小时内的统计计算结果。可能是多个算法的结算结果。
                //——把每种计算结果所有点放在一起排名。 无效值则排名为-1。
                int totalInputNumber = 0;   //根据参数计算出的需要的输入数据的总数量                
                for (i = 0; i < ModuleOuptNumber.Length; i++)
                {
                    totalInputNumber = totalInputNumber + ModuleOuptNumber[i];  //由于ReadMulti的特点，无论tagid有没有值，都会返回一个count=0的List<PValue>
                }
                
                //0.1、输入处理：输入长度。当输入为空或者长度为0时，给出标志位为StatusConst.InputIsNull的计算结果.                
                if (inputs == null || inputs.Length == 0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                if (inputs.Length != totalInputNumber)  //只要输入与参数计算的总数不一致，根本无法判断究竟是多了那个点，少了那个点。此时就需要报错，检查计算组态配置问题，看看参数配置的是否有问题
                {
                    _errorFlag = true;
                    _errorInfo = "输入数据的数量与需要的数量不符，请检查输入标签数量和计算项参数中设备所含点数及算法结果标签标志位数量设定是否相符！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0.2、取出的是MMTAEquDOR和MMTAEquEVSOR两种算法的小时计算结果，即每个计算结果2个值(包含截止时刻值)
                //——先分别取出两个算法中各点的排名
                //——再把每个点对应的MMTAEquDOR和MMTAEquEVSOR排名做平均
                //——最后对这个结果做排名

                double[] AvgMMTAEquDOR = new double[PointNumberInEqu];
                bool[] flagMMTAEquDOR = new bool[PointNumberInEqu];
                int countMMTAEquDOR=0;
                //先取MMTAEquDOR排名。MMTAEquDOR计算结果从6-155是1-50点的计算结果。
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    AvgMMTAEquDOR[i] = 0;                    
                   
                    if (inputs[4 + i * 3 + 3][0].Status == 0)   //取排名
                    {
                        AvgMMTAEquDOR[i] = AvgMMTAEquDOR[i] + inputs[4 + i * 3 + 3][0].Value;
                        flagMMTAEquDOR[i] = true;                        
                    }
                    else
                    {
                        AvgMMTAEquDOR[i] = -1;
                        flagMMTAEquDOR[i] = false;
                    }                   
                }
                //再取MMTAEquEVSOR。MMTAEquEVSOR计算结果是从156-305
                double[] AvgMMTAEquEVSOR = new double[PointNumberInEqu];
                bool[] flagMMTAEquEVSOR = new bool[PointNumberInEqu];
                int countMMTAEquEVSOR = 0;
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    AvgMMTAEquEVSOR[i] = 0;
                    countMMTAEquEVSOR = 0;

                    if (inputs[154 + i * 3 + 2][0].Status == 0) //取排名
                    {
                        AvgMMTAEquEVSOR[i] = AvgMMTAEquEVSOR[i] + inputs[154 + i * 3 + 2][0].Value;
                        flagMMTAEquEVSOR[i] = true;                        
                    }
                    else
                    {
                        AvgMMTAEquEVSOR[i] = -1;
                        flagMMTAEquEVSOR[i] = false;
                    }                    
                }
                //求均值
                double[] AvgEquDORDay = new double[PointNumberInEqu];
                bool[] flagEquDORDay = new bool[PointNumberInEqu];
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    if (flagMMTAEquEVSOR[i] == true && flagMMTAEquDOR[i] == true)
                    {
                        AvgEquDORDay[i] = (AvgMMTAEquDOR[i] + AvgMMTAEquEVSOR[i]) / 2;
                        flagEquDORDay[i] = true;
                    }
                    else
                    {
                        AvgEquDORDay[i] = -1;
                        flagEquDORDay[i] = false;
                    }                    
                }
                //求均值排名
                List<double> value4sort = new List<double>();
                double[] AvgEquDORDayO = new double[PointNumberInEqu];
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    if (flagEquDORDay[i]) value4sort.Add(AvgEquDORDay[i]);
                    else AvgEquDORDayO[i] = -1;                
                }
                double[] value = value4sort.ToArray();
                Array.Sort(value);
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    if (flagEquDORDay[i]) AvgEquDORDayO[i] = Array.IndexOf(value, AvgEquDORDay[i])+1;
                }
                //组织计算结果
                long status = 0;
                for (i = 0; i < PointNumberInEqu; i++) //最大可以接受50个点，但按实际点数给个点计算结果赋值
                {
                    status = (long)StatusConst.Normal;
                    if (flagEquDORDay[i] == false) status = (long)StatusConst.InvalidPoint;
                    
                    results[i * 2] = new List<PValue>();
                    results[i * 2].Add(new PValue(AvgEquDORDay[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));

                    results[i * 2 + 1] = new List<PValue>();
                    results[i * 2 + 1].Add(new PValue(AvgEquDORDayO[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));                    
                }
                //只返回与点数相符的计算结果项。计算引擎已经修改为按照实际返回结果来写数据。
                results = results.Take(PointNumberInEqu * 2).ToArray();
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
