using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 快速分布算法202段   
    /// 
    /// ——快速分布算法202段按照几度一个档，统计一天内温度分钟均值的200个档位的精细分布情况。分钟均值历史曲线按照阶梯型对待。而不是按照线性对待。
    /// ——可以通过参数设定中间档位的值。比如设定中间档位为499-500。则实际统计400-600温度范围内，每1度的分布情况。
    /// ——设定中间档位为498-500，则实际统计300-700温度范围内的情况，每2度为一个档位。
    /// ——1、数据量是60*24=1440点。计算结果，一度一个档。典型的如0-800度，即801个计算标签和计算结果。 
    /// 
    /// 特别注明：从自己的分析看，这样的统计并没有什么意义。因为是把原数据看做梯形。因此源数据实际上即代表了这种精细分布结果。
    ///          要想从前端来看精细分布，直接读取源数据（一天1440点，一个月43200点），经过简单循环，即可得出分布。速度也不会慢。
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     
    ///		2018.08.15 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.08.15</date>
    /// </author> 
    /// </summary>
    public class MFDistribute202 : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MFDistribute202";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "快速分布算法202段";
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
        private string _algorithms = "MFDistribute202";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"+      //一行50个Y
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"+
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"+
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"+
                                         "YY";                                                      //最后再加一个
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "500;502";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "第101段的下限（也是整体量程中点）;第101段上限";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){0,1}(;){1}([+-]?\d+(\.\d+)?){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 202;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "FD202S0;"+
                                      "FD202S01;FD202S02;FD202S03;FD202S04;FD202S05;FD202S06;FD202S07;FD202S08;FD202S09;FD202S10;" +
                                      "FD202S11;FD202S12;FD202S13;FD202S14;FD202S15;FD202S16;FD202S17;FD202S18;FD202S19;FD202S20;" +
                                      "FD202S21;FD202S22;FD202S23;FD202S24;FD202S25;FD202S26;FD202S27;FD202S28;FD202S29;FD202S30;" +
                                      "FD202S31;FD202S32;FD202S33;FD202S34;FD202S35;FD202S36;FD202S37;FD202S38;FD202S39;FD202S40;" +
                                      "FD202S41;FD202S42;FD202S43;FD202S44;FD202S45;FD202S46;FD202S47;FD202S48;FD202S49;FD202S50;" +
                                      "FD202S51;FD202S52;FD202S53;FD202S54;FD202S55;FD202S56;FD202S57;FD202S58;FD202S59;FD202S60;" +
                                      "FD202S61;FD202S62;FD202S63;FD202S64;FD202S65;FD202S66;FD202S67;FD202S68;FD202S69;FD202S70;" +
                                      "FD202S71;FD202S72;FD202S73;FD202S74;FD202S75;FD202S76;FD202S77;FD202S78;FD202S79;FD202S80;" +
                                      "FD202S81;FD202S82;FD202S83;FD202S84;FD202S85;FD202S86;FD202S87;FD202S88;FD202S89;FD202S90;" +
                                      "FD202S91;FD202S92;FD202S93;FD202S94;FD202S95;FD202S96;FD202S97;FD202S98;FD202S99;FD202S100;" +
                                      "FD202S101;FD202S102;FD202S103;FD202S104;FD202S105;FD202S106;FD202S107;FD202S108;FD202S109;FD202S110;" +
                                      "FD202S111;FD202S112;FD202S113;FD202S114;FD202S115;FD202S116;FD202S117;FD202S118;FD202S119;FD202S120;" +
                                      "FD202S121;FD202S122;FD202S123;FD202S124;FD202S125;FD202S126;FD202S127;FD202S128;FD202S129;FD202S130;" +
                                      "FD202S131;FD202S132;FD202S133;FD202S134;FD202S135;FD202S136;FD202S137;FD202S138;FD202S139;FD202S140;" +
                                      "FD202S141;FD202S142;FD202S143;FD202S144;FD202S145;FD202S146;FD202S147;FD202S148;FD202S149;FD202S150;" +
                                      "FD202S151;FD202S152;FD202S153;FD202S154;FD202S155;FD202S156;FD202S157;FD202S158;FD202S159;FD202S160;" +
                                      "FD202S161;FD202S162;FD202S163;FD202S164;FD202S165;FD202S166;FD202S167;FD202S168;FD202S169;FD202S170;" +
                                      "FD202S171;FD202S172;FD202S173;FD202S174;FD202S175;FD202S176;FD202S177;FD202S178;FD202S179;FD202S180;" +
                                      "FD202S181;FD202S182;FD202S183;FD202S184;FD202S185;FD202S186;FD202S187;FD202S188;FD202S189;FD202S190;" +
                                      "FD202S191;FD202S192;FD202S193;FD202S194;FD202S195;FD202S196;FD202S197;FD202S198;FD202S199;FD202S200;" +
                                      "FD202S201";
                                     
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "202段分布第0段;"+
                                        "202段分布第1段;202段分布第2段;202段分布第3段;202段分布第4段;202段分布第5段;202段分布第6段;202段分布第7段;202段分布第8段;202段分布第9段;202段分布第10段;" +
                                        "202段分布第11段;202段分布第12段;202段分布第13段;202段分布第14段;202段分布第15段;202段分布第16段;202段分布第17段;202段分布第18段;202段分布第19段;202段分布第20段;" +
                                        "202段分布第21段;202段分布第22段;202段分布第23段;202段分布第24段;202段分布第25段;202段分布第26段;202段分布第27段;202段分布第28段;202段分布第29段;202段分布第30段;" +
                                        "202段分布第31段;202段分布第32段;202段分布第33段;202段分布第34段;202段分布第35段;202段分布第36段;202段分布第37段;202段分布第38段;202段分布第39段;202段分布第40段;" +
                                        "202段分布第41段;202段分布第42段;202段分布第43段;202段分布第44段;202段分布第45段;202段分布第46段;202段分布第47段;202段分布第48段;202段分布第49段;202段分布第50段;" +
                                        "202段分布第51段;202段分布第52段;202段分布第53段;202段分布第54段;202段分布第55段;202段分布第56段;202段分布第57段;202段分布第58段;202段分布第59段;202段分布第60段;" +
                                        "202段分布第61段;202段分布第62段;202段分布第63段;202段分布第64段;202段分布第65段;202段分布第66段;202段分布第67段;202段分布第68段;202段分布第69段;202段分布第70段;" +
                                        "202段分布第71段;202段分布第72段;202段分布第73段;202段分布第74段;202段分布第75段;202段分布第76段;202段分布第77段;202段分布第78段;202段分布第79段;202段分布第80段;" +
                                        "202段分布第81段;202段分布第82段;202段分布第83段;202段分布第84段;202段分布第85段;202段分布第86段;202段分布第87段;202段分布第88段;202段分布第89段;202段分布第90段;" +
                                        "202段分布第91段;202段分布第92段;202段分布第93段;202段分布第94段;202段分布第95段;202段分布第96段;202段分布第97段;202段分布第98段;202段分布第99段;202段分布第100段;" +
                                        "202段分布第101段;202段分布第102段;202段分布第103段;202段分布第104段;202段分布第105段;202段分布第106段;202段分布第107段;202段分布第108段;202段分布第109段;202段分布第110段;" +
                                        "202段分布第111段;202段分布第112段;202段分布第113段;202段分布第114段;202段分布第115段;202段分布第116段;202段分布第117段;202段分布第118段;202段分布第119段;202段分布第120段;" +
                                        "202段分布第121段;202段分布第122段;202段分布第123段;202段分布第124段;202段分布第125段;202段分布第126段;202段分布第127段;202段分布第128段;202段分布第129段;202段分布第130段;" +
                                        "202段分布第131段;202段分布第132段;202段分布第133段;202段分布第134段;202段分布第135段;202段分布第136段;202段分布第137段;202段分布第138段;202段分布第139段;202段分布第140段;" +
                                        "202段分布第141段;202段分布第142段;202段分布第143段;202段分布第144段;202段分布第145段;202段分布第146段;202段分布第147段;202段分布第148段;202段分布第149段;202段分布第150段;" +
                                        "202段分布第151段;202段分布第152段;202段分布第153段;202段分布第154段;202段分布第155段;202段分布第156段;202段分布第157段;202段分布第158段;202段分布第159段;202段分布第160段;" +
                                        "202段分布第161段;202段分布第162段;202段分布第163段;202段分布第164段;202段分布第165段;202段分布第166段;202段分布第167段;202段分布第168段;202段分布第169段;202段分布第170段;" +
                                        "202段分布第171段;202段分布第172段;202段分布第173段;202段分布第174段;202段分布第175段;202段分布第176段;202段分布第177段;202段分布第178段;202段分布第179段;202段分布第180段;" +
                                        "202段分布第181段;202段分布第182段;202段分布第183段;202段分布第184段;202段分布第185段;202段分布第186段;202段分布第187段;202段分布第188段;202段分布第189段;202段分布第190段;" +
                                        "202段分布第191段;202段分布第192段;202段分布第193段;202段分布第194段;202段分布第195段;202段分布第196段;202段分布第197段;202段分布第198段;202段分布第199段;202段分布第200段;" +
                                        "202段分布第201段;";
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
        /// 计算模块算法实现:求实时值落在限定条件范围内的时间段span
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志。本算法无效</param>
        /// <param name="calcuinfo.fparas">限定条件，下限在前，上限在后，用分号分隔。如“25;70”，“25;”,";70"</param>       
        /// <returns>实时值落在限定条件范围内的时间段序列</returns>

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
            List<PValue>[] results = new List<PValue>[202];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {

                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，所有的输出项为0.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法采用阶梯型算法，截止时刻点不参与计算。
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = input.Count - 1; i >= 0; i--)
                {
                    if (input[i].Status != 0) input.RemoveAt(i);
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回全0
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //0、对输入参数paras进行解析，输入参数一共2个值，代表分布统计的区间。如“0,100”
                //string[] paras = calcuinfo.fparas.Split(',');
                string[] paras = Regex.Split(calcuinfo.fparas, ";|；");
                double sectionMin = double.Parse(paras[0]);             //中间区段的下限
                double sectionMax = double.Parse(paras[1]);             //中间区段的上限,sectionMax是实际量程的重点

                if (sectionMin == sectionMax)
                {
                    _errorFlag = true;
                    _errorInfo = "分布计算分布统计区间上下限设定数值不能相同！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); //如果两者相等，则有问题，直接返回null
                }

                if (sectionMax < sectionMin)       //如果大小顺序不对，则对调
                {
                    double temp = sectionMax;
                    sectionMax = sectionMin;
                    sectionMin = temp;
                }

                double rangeMin = sectionMin - (sectionMax - sectionMin) * 100;
                double rangeMax = sectionMin + (sectionMax - sectionMin) * 100;
                //1、准备边界标识数组                
                double Section = sectionMax - sectionMin;               

                //2、准备存储变量
                double[] section = new double[202];    //一共202个统计段

                //3、对数据进行遍历
                int currentSection = 0;
                for (int iPoint = 0; iPoint < input.Count; iPoint++)
                {

                    currentSection = (int)Math.Ceiling((input[iPoint].Value - rangeMin) / Section);
                    if (currentSection < 0) currentSection = 0;
                    if (currentSection > 201) currentSection = 201;
                    section[currentSection] = section[currentSection] + input[iPoint].Timespan;//将时间累加到对应的区间
                }

                //4、组织计算结果
                results = new List<PValue>[202];
                double sum = 0;     //用来验证总和是不是100%
                double totalmilliseconds=calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalMilliseconds;
                for (i = 0; i < 202; i++)
                {
                    results[i] = new List<PValue>();
                    results[i].Add(new PValue(section[i] * 100 / totalmilliseconds, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最后每一段分布存储的是时长百分比
                    //sum += section[i] * 100 / calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalMilliseconds;
                }
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); ;
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
            }//end try    
        }//end calcu
        #endregion
    


    }
}
