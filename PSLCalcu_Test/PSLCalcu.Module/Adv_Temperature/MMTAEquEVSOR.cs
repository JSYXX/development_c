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
    /// 壁温分析设备上单点温度特征值计算   
    /// 
    /// ——
    /// ——算法周期仅在日周期上进行计算。   
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
    public class MMTAEquEVSOR : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：设备上单点温度统计值得分及排序 ;

        private string _moduleName = "MMTAEquEVSOR";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备上单点温度统计值得分及排序";
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
        private string _inputDescsCN = "设备上所有温度点各种统计值";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAEquEVSOR";
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
        private string _moduleParaExample = "15;AAAAAAAAAAAAAAAADDAADDAADAAAAAAAA;NNNNYNNNNYNYYNNNNNN;NYNNYNYNYNNYNYNYNNYNYNYNNYNYYNNNNNNNNYNNYNYNNNNNNNNNYNYYNYNNNNNNNYYYYYYYYY";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "设备含有的点数;考察指标的升降序;第1个算法的计算结果数量;第二个算法的计算结果数量";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        //“第1个算法的计算结果数量,第二个算法的计算结果数量”，用NY表示，同时也代表这些结果中哪些参与排名计算。“考察指标的升降序"用来指定参与排名的这些指标的升降序。也就是第2个参数AD数量应该与3、4参数中Y的数量一致。
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;([AD]){1,}){1}(;([NY]){1,}){1,}$");      //第一个数字描述读取计算模块的种类数量，后面的数字匹配至少1-n次

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
        private string _outputDescs = "P01_Score;" + "P01_ScoreO;" + "P01_ScoreR;" +
                                      "P02_Score;" + "P02_ScoreO;" + "P02_ScoreR;" +
                                      "P03_Score;" + "P03_ScoreO;" + "P03_ScoreR;" +
                                      "P04_Score;" + "P04_ScoreO;" + "P04_ScoreR;" +
                                      "P05_Score;" + "P05_ScoreO;" + "P05_ScoreR;" +
                                      "P06_Score;" + "P06_ScoreO;" + "P06_ScoreR;" +
                                      "P07_Score;" + "P07_ScoreO;" + "P07_ScoreR;" +
                                      "P08_Score;" + "P08_ScoreO;" + "P08_ScoreR;" +
                                      "P09_Score;" + "P09_ScoreO;" + "P09_ScoreR;" +
                                      "P10_Score;" + "P10_ScoreO;" + "P10_ScoreR;" +
                                      "P11_Score;" + "P11_ScoreO;" + "P11_ScoreR;" +
                                      "P12_Score;" + "P12_ScoreO;" + "P12_ScoreR;" +
                                      "P13_Score;" + "P13_ScoreO;" + "P13_ScoreR;" +
                                      "P14_Score;" + "P14_ScoreO;" + "P14_ScoreR;" +
                                      "P15_Score;" + "P15_ScoreO;" + "P15_ScoreR;" +
                                      "P16_Score;" + "P16_ScoreO;" + "P16_ScoreR;" +
                                      "P17_Score;" + "P17_ScoreO;" + "P17_ScoreR;" +
                                      "P18_Score;" + "P18_ScoreO;" + "P18_ScoreR;" +
                                      "P19_Score;" + "P19_ScoreO;" + "P19_ScoreR;" +
                                      "P20_Score;" + "P20_ScoreO;" + "P20_ScoreR;" +
                                      "P21_Score;" + "P21_ScoreO;" + "P21_ScoreR;" +
                                      "P22_Score;" + "P22_ScoreO;" + "P22_ScoreR;" +
                                      "P23_Score;" + "P23_ScoreO;" + "P23_ScoreR;" +
                                      "P24_Score;" + "P24_ScoreO;" + "P24_ScoreR;" +
                                      "P25_Score;" + "P25_ScoreO;" + "P25_ScoreR;" +
                                      "P26_Score;" + "P26_ScoreO;" + "P26_ScoreR;" +
                                      "P27_Score;" + "P27_ScoreO;" + "P27_ScoreR;" +
                                      "P28_Score;" + "P28_ScoreO;" + "P28_ScoreR;" +
                                      "P29_Score;" + "P29_ScoreO;" + "P29_ScoreR;" +
                                      "P30_Score;" + "P30_ScoreO;" + "P30_ScoreR;" +
                                      "P31_Score;" + "P31_ScoreO;" + "P31_ScoreR;" +
                                      "P32_Score;" + "P32_ScoreO;" + "P32_ScoreR;" +
                                      "P33_Score;" + "P33_ScoreO;" + "P33_ScoreR;" +
                                      "P34_Score;" + "P34_ScoreO;" + "P34_ScoreR;" +
                                      "P35_Score;" + "P35_ScoreO;" + "P35_ScoreR;" +
                                      "P36_Score;" + "P36_ScoreO;" + "P36_ScoreR;" +
                                      "P37_Score;" + "P37_ScoreO;" + "P37_ScoreR;" +
                                      "P38_Score;" + "P38_ScoreO;" + "P38_ScoreR;" +
                                      "P39_Score;" + "P39_ScoreO;" + "P39_ScoreR;" +
                                      "P40_Score;" + "P40_ScoreO;" + "P40_ScoreR;" +
                                      "P41_Score;" + "P41_ScoreO;" + "P41_ScoreR;" +
                                      "P42_Score;" + "P42_ScoreO;" + "P42_ScoreR;" +
                                      "P43_Score;" + "P43_ScoreO;" + "P43_ScoreR;" +
                                      "P44_Score;" + "P44_ScoreO;" + "P44_ScoreR;" +
                                      "P45_Score;" + "P45_ScoreO;" + "P45_ScoreR;" +
                                      "P46_Score;" + "P46_ScoreO;" + "P46_ScoreR;" +
                                      "P47_Score;" + "P47_ScoreO;" + "P47_ScoreR;" +
                                      "P48_Score;" + "P48_ScoreO;" + "P48_ScoreR;" +
                                      "P49_Score;" + "P49_ScoreO;" + "P49_ScoreR;" +
                                      "P50_Score;" + "P50_ScoreO;" + "P50_ScoreR";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "第1点各项指标排名得分均值;第1点排名;第1点得分折算值;" +
                                        "第2点各项指标排名得分均值;第2点排名;第2点得分折算值;" +
                                        "第3点各项指标排名得分均值;第3点排名;第3点得分折算值;" +
                                        "第4点各项指标排名得分均值;第4点排名;第4点得分折算值;" +
                                        "第5点各项指标排名得分均值;第5点排名;第5点得分折算值;" +
                                        "第6点各项指标排名得分均值;第6点排名;第6点得分折算值;" +
                                        "第7点各项指标排名得分均值;第7点排名;第7点得分折算值;" +
                                        "第8点各项指标排名得分均值;第8点排名;第8点得分折算值;" +
                                        "第9点各项指标排名得分均值;第9点排名;第9点得分折算值;" +
                                        "第10点各项指标排名得分均值;第10点排名;第10点得分折算值;" +
                                        "第11点各项指标排名得分均值;第11点排名;第11点得分折算值;" +
                                        "第12点各项指标排名得分均值;第12点排名;第12点得分折算值;" +
                                        "第13点各项指标排名得分均值;第13点排名;第13点得分折算值;" +
                                        "第14点各项指标排名得分均值;第14点排名;第14点得分折算值;" +
                                        "第15点各项指标排名得分均值;第15点排名;第15点得分折算值;" +
                                        "第16点各项指标排名得分均值;第16点排名;第16点得分折算值;" +
                                        "第17点各项指标排名得分均值;第17点排名;第17点得分折算值;" +
                                        "第18点各项指标排名得分均值;第18点排名;第18点得分折算值;" +
                                        "第19点各项指标排名得分均值;第19点排名;第19点得分折算值;" +
                                        "第20点各项指标排名得分均值;第20点排名;第20点得分折算值;" +
                                        "第21点各项指标排名得分均值;第21点排名;第21点得分折算值;" +
                                        "第22点各项指标排名得分均值;第22点排名;第22点得分折算值;" +
                                        "第23点各项指标排名得分均值;第23点排名;第23点得分折算值;" +
                                        "第24点各项指标排名得分均值;第24点排名;第24点得分折算值;" +
                                        "第25点各项指标排名得分均值;第25点排名;第25点得分折算值;" +
                                        "第26点各项指标排名得分均值;第26点排名;第26点得分折算值;" +
                                        "第27点各项指标排名得分均值;第27点排名;第27点得分折算值;" +
                                        "第28点各项指标排名得分均值;第28点排名;第28点得分折算值;" +
                                        "第29点各项指标排名得分均值;第29点排名;第29点得分折算值;" +
                                        "第30点各项指标排名得分均值;第30点排名;第30点得分折算值;" +
                                        "第31点各项指标排名得分均值;第31点排名;第31点得分折算值;" +
                                        "第32点各项指标排名得分均值;第32点排名;第32点得分折算值;" +
                                        "第33点各项指标排名得分均值;第33点排名;第33点得分折算值;" +
                                        "第34点各项指标排名得分均值;第34点排名;第34点得分折算值;" +
                                        "第35点各项指标排名得分均值;第35点排名;第35点得分折算值;" +
                                        "第36点各项指标排名得分均值;第36点排名;第36点得分折算值;" +
                                        "第37点各项指标排名得分均值;第37点排名;第37点得分折算值;" +
                                        "第38点各项指标排名得分均值;第38点排名;第38点得分折算值;" +
                                        "第39点各项指标排名得分均值;第39点排名;第39点得分折算值;" +
                                        "第40点各项指标排名得分均值;第40点排名;第40点得分折算值;" +
                                        "第41点各项指标排名得分均值;第41点排名;第41点得分折算值;" +
                                        "第42点各项指标排名得分均值;第42点排名;第42点得分折算值;" +
                                        "第43点各项指标排名得分均值;第43点排名;第43点得分折算值;" +
                                        "第44点各项指标排名得分均值;第44点排名;第44点得分折算值;" +
                                        "第45点各项指标排名得分均值;第45点排名;第45点得分折算值;" +
                                        "第46点各项指标排名得分均值;第46点排名;第46点得分折算值;" +
                                        "第47点各项指标排名得分均值;第47点排名;第47点得分折算值;" +
                                        "第48点各项指标排名得分均值;第48点排名;第48点得分折算值;" +
                                        "第49点各项指标排名得分均值;第49点排名;第49点得分折算值;" +
                                        "第50点各项指标排名得分均值;第50点排名;第50点得分折算值"
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
                const int   MAXNUMBER_IN_DEVICE=50;                     //一个设备中允许设备测点的最大值
                //获得参数
                string[] paras = calcuinfo.fparas.Split(';');
                int PointNumberInEqu = int.Parse(paras[0]);             //参数第一个值是设备包含的点数
                string sortFlag = paras[1];                             //对所取指标进行升降序排列标志
                int calcuModuleNumber = paras.Length-2;                 //参数中后面NY字符的个数（用；分割）是计算模件种类的数量
                int[] ModuleOuptNumber = new int[calcuModuleNumber];    //每个NY字符串长度是计算模件输出的个数
                string[] ModuleOuptFlag = new string[calcuModuleNumber];//每个NY字符串
                for (i = 0; i < paras.Length - 2; i++)
                {
                    ModuleOuptNumber[i] = paras[2 + i].Length;
                    ModuleOuptFlag[i] = paras[2 + i];
                }
                if (PointNumberInEqu > MAXNUMBER_IN_DEVICE)
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("计算参数中指定的设备点数超过最大允许点数{0}，请检查计算参数配置！", MAXNUMBER_IN_DEVICE);
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0、输入:输入数据为设备上所有单点一个小时内的统计计算结果。可能是多个算法的结算结果。
                //——把每种计算结果所有点放在一起排名。 无效值则排名为-1。
                int totalInputNumber = 0;   //根据参数计算出的需要的输入数据的总数量                
                for (i = 0; i < ModuleOuptNumber.Length; i++)
                {
                    totalInputNumber = totalInputNumber + ModuleOuptNumber[i] * PointNumberInEqu;                    
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
                //0.2、取出的是每个点各种基本算法下的统计参数的小时值，即每个参数两个值，一个值为小时值，一个值为截止值。
                //——数据是按照不同的计算结果组织
                //——每类计算结果中每个点的小时值都有自己的标志位

                //根据YN字符串计算实际需要参与得分和排序的参数数量
                int NumberInCalcu = 0;
                for (i = 0; i < calcuModuleNumber; i++)
                {
                    for (int j = 0; j < ModuleOuptFlag[i].Length; j++)
                        if (ModuleOuptFlag[i][j] == 'Y') NumberInCalcu = NumberInCalcu + 1;
                }
                if (NumberInCalcu != sortFlag.Length)
                {
                    _errorFlag = true;
                    _errorInfo = "参数设置中参数升降序标志数量与计算结果标志位Y的数量必须相等，请检查参数配置！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0.3、重新组织数据
                //——第一维是要计算的参数（可能来源于多个计算结果，按照YN逻辑字符串来取，凡是对应位为Y的计算结果，就是要用到的，统一放到一个大数组中）。NumberInCalcu个
                //——第二维是同一个参数下设备上的不同点。PointNumberInEqu个
                bool[][] validflag = new bool[NumberInCalcu][];         //每个参数（NumberInCalcu个）每个设备点的有效性标记
                double[][] calcuValue = new double[NumberInCalcu][];    //每个参数（NumberInCalcu个）每个设备点的值
                for (i = 0; i < NumberInCalcu; i++)
                {
                    validflag[i] = new bool[PointNumberInEqu];          //每个参数中存放设备各点值（PointNumberInEqu个）
                    calcuValue[i] = new double[PointNumberInEqu];       //每个参数中存放设备各点值（PointNumberInEqu个）
                }
               
                int fromColumn = 0;
                int toColumn = 0;
                int paraStartRow=0;                
                for (i = 0; i < calcuModuleNumber; i++)             //依次读取每个计算模件
                {                    
                    for (int j = 0; j < PointNumberInEqu; j++)      //每个计算模件中，依次读取每个设备点
                    {
                        toColumn = 0;                               //计算结果序号
                        for(int k=0;k<ModuleOuptNumber[i];k++)      //依次读取每个设备点每一类结算结果
                        {
                            if (ModuleOuptFlag[i][k] == 'Y')        //该计算模件对应计算结果的标志位.ModuleOuptFlag[i]是字符串，ModuleOuptFlag[i][k]去字符串中字符
                            {
                                if (inputs[fromColumn]!=null && inputs[fromColumn].Count!=0 && inputs[fromColumn][0].Status == 0)
                                {
                                    calcuValue[paraStartRow + toColumn][j] = inputs[fromColumn][0].Value;   //第一维是结算结果序号，第二维是设备内不同点
                                    validflag[paraStartRow + toColumn][j] = true;
                                    toColumn = toColumn + 1;
                                }
                                else
                                {   
                                    //如果对应的点为null或状态不对，则该点给出最小值。并且标志位为false
                                    calcuValue[paraStartRow + toColumn][j] = float.MinValue;
                                    validflag[paraStartRow + toColumn][j] = false;
                                    toColumn = toColumn + 1;
                                }
                            }
                            fromColumn = fromColumn + 1;    //读下一个inputs值
                        }
                    }
                    //下一个计算模件的参数起始位置
                    paraStartRow = paraStartRow + toColumn;
                    
                }
                //转化好的calcuValue，第一维是参数中指定的两个或多个模件中要参与排名的那些结算结果。第二维是一个设备中不同的计算点。
                //定义计算结果
                double[] EquPointScore = new double[PointNumberInEqu];          //各点各项指标排名名次平均值。以Y的数量确定的要参与排序计算的参数数量。
                double[] EquPointScoreO = new double[PointNumberInEqu];         //上面的平均值的排名
                double[] EquPointScoreR = new double[PointNumberInEqu];         //排名折算到1-100

                //计算
                
                //1、按参数计算各点在同一参数上的排名
                //——待排序的数值calcuValue，第一维i是不同的参数，第二维j是一个参数下，所有的设备该参数值
                //——排序结果数组EquPointScore4para，第一维是设备点数。第二维是该点对应的不同参数。与calcuValue相反
                double[][] EquPointScore4para = new double[PointNumberInEqu][];     //某个参数中各点的排名，第一维是点，第二维是该点在不同参数上的排名
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    EquPointScore4para[i] = new double[NumberInCalcu];              //某一个点中每个参数在所有点的排名
                }
                List<double> value4sort;                         //用于排序的数组
                for(i=0;i<NumberInCalcu;i++)                                        //按参数来计算。calcuvalue的i。           
                {
                    value4sort = new List<double>();
                    for (int j = 0; j < PointNumberInEqu; j++)                      //依次取某个参数所有设备点的值，calcuvalue的j
                    {
                        if (validflag[i][j]) value4sort.Add(calcuValue[i][j]);      //value4sort仅包含有效点
                        else  EquPointScore4para[j][i] =-1;                         //无效点的排序直接置为-1
                    }
                    if (value4sort == null || value4sort.Count == 0) continue;      //如果该参数下，各点均无效，则所有点排名均为-1，直接进行下一个参数排名
                    double[] valuesort = value4sort.ToArray();
                    
                    //升序排列
                    Array.Sort(valuesort);
                    //降序排列
                    if (sortFlag[i] == 'D')  Array.Reverse(valuesort);

                    //查找顺序
                    for (int j = 0; j < PointNumberInEqu; j++)                      //参数i中第j个点，在整个参数中的排序结果
                    {
                        if (validflag[i][j]) EquPointScore4para[j][i] = Array.IndexOf(valuesort, calcuValue[i][j]) + 1; //获取有效点排名。这里注意EquPointScore4para第一维是点，第二维是参数
                    }
                }
                //2、按点计算该点各参数排名均值
                double valuesum = 0;
                for (i = 0; i < PointNumberInEqu; i++)              //EquPointScore4para第一维是设备点
                {
                    valuesum = 0;
                    for (int j = 0; j < NumberInCalcu; j++)         //EquPointScore4para第二维是设备点的不同参数
                    {
                        if (validflag[j][i]) valuesum = valuesum + EquPointScore4para[i][j];        //计算均值时只找有效点。注意validflag第一维是不同参数。第二维是不同设备点。
                    }
                    EquPointScore[i] = valuesum / NumberInCalcu;
                }

                //3、计算均值的排名
                value4sort = new List<double>();
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    if (EquPointScore[i] != -1) value4sort.Add(EquPointScore[i]);
                    
                }
                double[] valueArray = value4sort.ToArray();

                //升序排列
                Array.Sort(valueArray);
                for (i = 0; i < PointNumberInEqu; i++)
                {
                    if (EquPointScore[i] != -1)
                    {
                        EquPointScoreO[i] = Array.IndexOf(valueArray, EquPointScore[i])+1;
                        EquPointScoreR[i] = 1 + (EquPointScoreO[i] - 1) * 99 / (PointNumberInEqu - 1);
                    }
                    else
                    {
                        EquPointScoreO[i] = -1;
                        EquPointScoreR[i] = -1;
                    }
                }


                //组织计算结果
                long status = 0;
                for (i = 0; i < PointNumberInEqu; i++) //最大可以接受50个点，但按实际点数给个点计算结果赋值
                {
                    //status = (long)StatusConst.InvalidPoint;
                    results[i * 3] = new List<PValue>();
                    results[i * 3].Add(new PValue(EquPointScore[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));

                    results[i * 3 + 1] = new List<PValue>();
                    results[i * 3 + 1].Add(new PValue(EquPointScoreO[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));

                    results[i * 3 + 2] = new List<PValue>();
                    results[i * 3 + 2].Add(new PValue(EquPointScoreR[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));
                }
                
                //只返回与点数相符的计算结果项。计算引擎已经修改为按照实际返回结果来写数据。
                results = results.Take(PointNumberInEqu * 3).ToArray();

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
