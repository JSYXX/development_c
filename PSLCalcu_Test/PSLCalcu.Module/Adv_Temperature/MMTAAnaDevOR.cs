using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析偏差超限统计算法   
    /// 
    /// ——MMTAAnaDevOR直接用一个月的分钟数据计算
    /// ——1、数据量是4.5W左右，即3600*24*30=43200
    /// ——2、读取一个月的分钟数据，sql耗时4-5ms。
    /// ——3、从DateReader中取Pvalue格式的数据，需要200ms。
    /// ——4、整个取数据阶段需要350ms。
    /// ——5、计算需要35ms。
    /// ——6、写计算结果需要80ms。
    /// ——7、总体需要500ms左右。
    /// 结论，MLTempLimit最多直接用于1个月的计算。如果计算周期再长比如三个月。需要改用长周期的算法。按照rdbset来取数据。
    /// ——如果改为rdbset的方式读取，多个标签的数据读取方式需要改变
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。该算法是线性计算，需要截止时刻数据。
    ///		2017.03.21 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2017.03.21</date>
    /// </author> 
    /// </summary>
    public class MMTAAnaDevOR : BaseModule, IModule, IModuleExPara
    {

        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAAnaDevOR";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "温度超限统计算法";
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
        private string _algorithms = "MMTAAnaDevOR";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "20;30;40;50;60;70;80;2;2;2;2;2;2;50;50;10";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "LLL限值;LL限值;L限值;标准值;H限值;HH限值;HHH限值;LLL死区;LL死区;L死区;H死区;HH死区;HHH死区;速度稳定限值V1(/秒);速度稳定限值V2(/秒);最小时间段阈值(秒);——最小时间段阈值可选。不填写则默认取0分钟。填写时前面加分号与前面的参数分隔。";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){14}(;[+-]?\d+(\.\d+)?){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 74;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "TempHHN;" +      //0
                                    "TempHHSpan;" +
                                    "TempHHT;" +
                                    "TempHHR;" +
                                    "TempHHTMax;" +
                                    "TempHHA;" +        //5
                                    "TempHHAvg;" +
                                    "TempHN;" +
                                    "TempHSpan;" +
                                    "TempHT;" +
                                    "TempHR;" +         //10
                                    "TempHTMax;" +
                                    "TempHA;" +
                                    "TempHAvg;" +                                   
                                    "TempLN;" +
                                    "TempLSpan;" +      //15
                                    "TempLT;" +
                                    "TempLR;" +
                                    "TempLTMax;" +
                                    "TempLA;" +
                                    "TempLAvg;" +           //20                         
                                    "TempLLN;" +
                                    "TempLLSpan;" +
                                    "TempLLT;" +
                                    "TempLLR;" +
                                    "TempLLTMax;" +     //25
                                    "TempLLA;" +
                                    "TempLLAvg;" +  
                                    "TempRRT;"+
                                    "TempRRR;"+
                                    "TempRRTMax;"+      //30
                                    "Temp0HT;" +
                                    "Temp0HTR;" +
                                    "Temp0HTMax;" +                                    
                                    "Temp0LT;" +
                                    "Temp0LTR;" +       //35
                                    "Temp0LTMax;" +     
                                    "TempHLT;" +
                                    "TempHLTR;" +
                                    "TempHLTMax;" +
                                    "TempHHHLLLT;" +    //40
                                    "TempHHHLLLTR;" +                                   
                                    "TempGHHLLT;" +
                                    "TempGHHLLTR;" +
                                    "TempGHT;" +
                                    "TempGHTR;" +       //45
                                    "TempGHAvg;"+       
                                    "TempGLT;" +
                                    "TempGLTR;"+
                                    "TempGLAvg;"+
                                    "TempGHLT;" +        //50
                                    "TempGHLTR;" +      
                                    "TempLV1T;" +
                                    "TempLV1TR;" +
                                    "TempLV1TMax;" +
                                    "TempGV2T;" +       //55
                                    "TempGV2TR;" +      
                                    "TempGV2TMax;" +
                                    "TempPT;" +
                                    "TempPTR;"+
                                    "TempPA;" +         //60
                                    "TempNT;" +         
                                    "TempNTR;" +
                                    "TempNA;" +
                                    "TempPNR;"+
                                    "TEvaSR;" +         //65
                                    "TEvaHT;" +         
                                    "TEvaHHT;" +
                                    "TEvaHTF;" +
                                    "TEvaHHTF;" +
                                    "TEvaLT;" +         //70
                                    "TEvaLLT;" +        
                                    "TEvaLTF;" +
                                    "TEvaLLTF"                                    
                                    ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "越 HH 次数;" +                         //0  YNYNY（玉华项目典型设置）
                                        "越 HH 时刻;" +                         //1  N                     
                                        "越 HH 总时长(秒);" +                   //2  Y
                                        "越 HH 时间占比(%);" +                  //3  N
                                        "越 HH 单次最长时间及时刻(秒);" +        //4  Y
                                        "越 HH 面积(原单位*秒);" +              //5  YNYNY（玉华项目典型设置）
                                        "越 HH 平均值;" +                       //6  N
                                        "越 H 次数;" +                         //7   Y
                                        "越 H 时刻;" +                         //8   N
                                        "越 H 总时长(秒);" +                   //9   Y
                                        "越 H 时间占比(%);" +                  //10  NYYNY（玉华项目典型设置）
                                        "越 H 单次最长时间及时刻;" +            //11  Y
                                        "越 H 面积(原单位*秒);" +              //12  Y
                                        "越 H 平均值;" +                       //13  N
                                        "越 L 次数;" +                         //14  Y
                                        "越 L 时刻;" +                         //15  NYNYY（玉华项目典型设置）
                                        "越 L 总时长(秒);" +                   //16  Y
                                        "越 L 时间占比(%);" +                  //17  N
                                        "越 L 单次最长时间及时刻;" +            //18  Y
                                        "越 L 面积(原单位*秒);" +              //19  Y
                                        "越 L 平均值;" +                       //20  NYNYN（玉华项目典型设置）
                                        "越 LL 次数;" +                        //21  Y
                                        "越 LL 时刻;" +                        //22  N
                                        "越 LL 总时长(秒);" +                  //23  Y
                                        "越 LL 时间占比(%);" +                 //24  N
                                        "越 LL 单次最长时间及时刻;" +           //25 YYNYN（玉华项目典型设置）
                                        "越 LL 面积(原单位*秒);" +             //26  Y
                                        "越 LL 平均值;" +                     //27  N
                                        "-R 到 R 总时长(秒);" +                //28 Y
                                        "-R 到 R 时间占比(%);" +               //29 N
                                        "-R 到 R 单次最长时间及时刻;" +        //30 YNNYN（玉华项目典型设置）
                                        "0 到 H 总时长;" +                    //31 N
                                        "0 到 H 时间占比(%);" +               //32 N
                                        "0 到 H 单次最长时间及时刻;" +         //33 Y
                                        "0 到 L 总时长(秒);" +                //34 N
                                        "0 到 L 时间占比(%);" +                          //35  NYYNY（玉华项目典型设置）
                                        "0 到 L 单次最长时间及时刻;" +                    //36  Y
                                        "H 到 L 之间总时长(秒);" +                        //37  Y
                                        "H 到 L 之间时间占比(%);" +                       //38  N
                                        "H 到 L 之间单次最长时间及时刻;" +                 //39  Y
                                        "（H 到 HH）时间 +（L 到 LL）时间长(秒);" +        //40  NNYNY（玉华项目典型设置）
                                        "（H 到 HH）时间 +（L 到 LL）时间占比(%);" +        //41 N
                                        "（> HH 时间）+（< LL）时间长(秒);" +              //42  Y
                                        "（> HH 时间）+（< LL）时间占比(%);" +             //43  N
                                        "（> H 时间）+（< L）时间长(秒);" +                //44  Y
                                        "（> H 时间）+（< L）时间占比(%);" +               //45  NNNYN（玉华项目典型设置）
                                        " > H 总时长，即HHT+HT; " +                       //46   N
                                        " > H 总时长占比T; " +                            //47   N
                                        " > H 总平均值，（HA+HHA）/（HT+HHT）;" +          //48  Y
                                        " < L 总时长，即LLT+LT; " +                        //49  N
                                        " < L 总时长占比T; " +                             //50  NYYNY（玉华项目典型设置）
                                        " < L 总平均值，（LA+LLA）/（LT+LLT）;" +           //51  Y
                                        "速度变化绝对值小于 V1（稳定）时间长(秒);" +         //52  Y
                                        "稳定时间占比(%);" +                               //53   N
                                        "单次最长稳定时间及时刻;" +                         //54   Y
                                        "速度变化绝对值大于 V2（不稳定）时间长(秒);" +       //55  YNYNN（玉华项目典型设置）
                                        "不稳定时间占比(%);" +                              //56  N
                                        "单次最长不稳定时间及时刻;" +                        //57  Y
                                        "正数值时长(秒);" +                                 //58  N
                                        "正数值时间占比(%);" +                              //59  N
                                        "正数值面积(原数值*秒);" +                          //60    NNNNY（玉华项目典型设置）
                                        "负数值时长(秒);" +                                 //61  N  
                                        "负数值时间占比(%);" +                              //62  N
                                        "负数值面积(原数值*秒);" +                          //63  N                
                                        "正面积占比;" +                                     //64  Y
                                        "安全系数,H~L时间占比;" +                          //65  YYYYY（玉华项目典型设置）
                                        "越 H 限强度指数;" +                              //66   Y
                                        "越 HH 限强度指数;" +                             //67   Y
                                        "越 H 限力度指数;" +                              //68   Y
                                        "越 HH 限力度指数;" +                             //69   Y
                                        "越 L 限强度指数;" +                              //70 YYYY    （玉华项目典型设置）                //
                                        "越 LL 限强度指数;" +                             //71   Y
                                        "越 L 限力度指数;" +                              //72   Y
                                        "越 LL 限力度指数"                                //73   Y          
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

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，则应当有输出，大部分的输出项为0。个别输出项为空
            List<PValue>[] results = new List<PValue>[74];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }
            //其中越线时间段、极值时刻的相应项为空
            results[1] = null;      //越HH时间序列
            //results[4] = null;      //越HH最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            results[8] = null;      //越H时间序列
            //results[11] = null;     //越H最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            results[15] = null;     //越L时间序列
            //results[18] = null;     //越L最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            results[22] = null;     //越LL时间序列
            //results[25] = null;     //越LL最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            //results[30] = null;     //RR最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            //results[33] = null;     //0H最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            //results[36] = null;     //0L最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            //results[39] = null;     //LH最长时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            //results[54] = null;     //V1最长稳定时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空
            //results[57] = null;     //V2最长不稳定时间。最长时间不一定存在。比如当计算周期内没有到达该区间。最长时间为0。起止时刻与计算周期相同。其他算法还要基于该结果统计，因此不能为空

            try
            {

                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，所有的输出项为0.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); //如果输入为空，则主引擎已经报警
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法采用线性计算，截止时刻点要参与计算。不删除
                //if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = input.Count - 1; i >= 0; i--)
                {
                    if (input[i].Status != 0) input.RemoveAt(i);
                    //典型的，比如在指标考核中，当负荷降为0以后，所有的偏差的状态值均为1000。即超出偏差曲线范围。这个过程可能持续1天。
                    //那整个一天的每分钟偏差值，状态位均为1000。
                    //此时，如果对1天的偏差进行超限统计，则输入值均会被过滤掉，过滤完的inputs为null。
                    //应该直接返回头部定义的结果results，并且该result的状态位为StatusConst.InputIsNull
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回全0
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }



                //1、准备界限数组和死区值
                double[] LimitRange = new double[7];  //界限数组，用于查找当前value值处于的值域段。界限数组包括LLL、LL、L、N、H、HH、HHH七个值，以及MinValue和MaxValue
                double LLLDeadBand, LLDeadBand, LDeadBand, HDeadBand, HHDeadBand, HHHDeadBand;
                LLLDeadBand = 0; LLDeadBand = 0; LDeadBand = 0; HDeadBand = 0; HHDeadBand = 0; HHHDeadBand = 0;
                double threshold;

                //1.1、对输入参数paras进行解析，输入参数一共16个值，包括三高三低的设定值和三高三低的死区值。如“20,30,40,50,60,70,80,2,2,2,2,2,2”
                string[] paras = calcuinfo.fparas.Split(';');
                for (i = 0; i < 7; i++)
                {
                    LimitRange[i] = float.Parse(paras[i]);
                }

                //给定的LimitRange必须由小到到，如果参数顺序不正确，不能进行计算，给出错误提示。
                //——如果参数顺序不对，下面的计算，判断区域会出错。
                if (LimitRange[0] >= LimitRange[1] ||
                    LimitRange[1] >= LimitRange[2] ||
                    LimitRange[2] >= LimitRange[3] ||
                    LimitRange[3] >= LimitRange[4] ||
                    LimitRange[4] >= LimitRange[5] ||
                    LimitRange[5] >= LimitRange[6]
                    )
                {
                    _errorFlag = true;
                    _errorInfo = "计算参数不正确。三高三低限值以及标准值，顺序不正确";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //死区参数
                LLLDeadBand = float.Parse(paras[7]);
                LLDeadBand = float.Parse(paras[8]);
                LDeadBand = float.Parse(paras[9]);
                HDeadBand = float.Parse(paras[10]);
                HHDeadBand = float.Parse(paras[11]);
                HHHDeadBand = float.Parse(paras[12]);

                //速度参数
                double V1 = float.Parse(paras[13]);         //稳定速度上限
                double V2 = float.Parse(paras[14]);          //不稳定速度下限

                //阈值参数为可选，如果有，则设置，没有，则视为0
                if (paras.Length == 16 && paras[15] != "")
                {
                    threshold = float.Parse(paras[15]);
                }
                else
                {
                    threshold = 0;
                }

                //1.1 对各限值进行死区调整
                //超限统计，根据三低限值、标准值和三高限值，把值域分成8个限值区域，对应7个分割限值LLL、LL、L、N、H、HH、HHH
                //——0区域代表MinValue<x<=LLL,  1区域代表LLL<x<=LL,  2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 6区域代表HH<x<HHH,  7区域代表HHH<x<=MaxValue
                //——每个区域向外跳变时，都对应自己的死区调整后的7个分割限值，记为LimitRangeDeadBand[i]。
                //——比如LimitRangeDeadBand[0],处于LLL，那么就只有LLL的限值需要进行死区调整。如果是LimitRangeDeadBand[3]，则只有L需要进行死区调整
                //但是，由于有死区的设定，点进入不同区域时，对应的限值数组需要做调整.x区域的点对应LimitRangeDeadBand[x]。
                //位于不同限值区域的点，要使用不同的限值数组进行下一个点的限值区域判定。
                double[][] LimitRangeDeadBand = new double[8][];
                for (i = 0; i < 8; i++)                         //先用LimitRange去初始化LimitRangeDeadBand
                {
                    LimitRangeDeadBand[i] = new double[7];
                    for (int j = 0; j < 3; j++)
                    {   //在限值上不算越限。由于本算法采用的是排序法，找到点所在的区域。
                        //——当点位于H、HH、HHH的位置时，计算得到的点的区域刚好是想要的结果，即不算越限。
                        //——当点位于L、LL、LLL的位置时，计算得到的点的区域会被认为是算为越界。这不是想要的结果。为了使点在L、LL、LLL位置时不被认为是越界，需要给L、LL、LLL减掉一个极小值。这样在L、LL、LLL边界上的点会返回正确的结果
                        LimitRangeDeadBand[i][j] = LimitRange[j] - 0.00000001;
                    }
                    LimitRangeDeadBand[i][3] = LimitRange[3];
                    for (int j = 4; j < 7; j++)
                    {
                        LimitRangeDeadBand[i][j] = LimitRange[j];
                    }
                }
                //当实时值位于LLL区时，当前区域值为currentRange=0，在LLL区内，返回LL时，要求实时值要到LLL+LLLDeadBand之上，才返回有效。返回L时要求实时值到LL+LLDeadBand之上才有效，返回Normal时，要求返回L+LDeadBand才有效。依次类推
                //此时点位于区域值为0，对应的调整死区界限数组为 LimitRangeDeadBand[0]      
                LimitRangeDeadBand[0][0] = LimitRangeDeadBand[0][0] + LLLDeadBand;  //数值点位于0区 MinValue<x<=LLL时，调整后的LLL值
                LimitRangeDeadBand[0][1] = LimitRangeDeadBand[0][1] + LLDeadBand;   //数值点位于0区 MinValue<x<=LLL时，调整后的LL值
                LimitRangeDeadBand[0][2] = LimitRangeDeadBand[0][2] + LDeadBand;    //数值点位于0区 MinValue<x<=LLL时，调整后的L值
                LimitRangeDeadBand[0][4] = LimitRangeDeadBand[0][4] ;    //数值点位于0区 MinValue<x<=LLL时，调整后的H值
                LimitRangeDeadBand[0][5] = LimitRangeDeadBand[0][5] ;   //数值点位于0区 MinValue<x<=LLL时，调整后的HH值
                LimitRangeDeadBand[0][6] = LimitRangeDeadBand[0][6] ;  //数值点位于0区 MinValue<x<=LLL时，调整后的HHH值
                //当实时值位于LL区时，当前区域值为currentRange=1，在LL区内，返回L时要求实时值到LL+LLDeadBand之上才有效，...
                //此区域currentRange=1，对应的限值数组 LimitRangeDeadBand[1]           
                LimitRangeDeadBand[1][0] = LimitRangeDeadBand[1][0] ;  //数值点位于1区域 LLL<x<=LL时，调整后的LLL值
                LimitRangeDeadBand[1][1] = LimitRangeDeadBand[1][1] + LLDeadBand;   //数值点位于1区域 LLL<x<=LL时，调整后的LL值
                LimitRangeDeadBand[1][2] = LimitRangeDeadBand[1][2] + LDeadBand;    //数值点位于1区域 LLL<x<=LL时，调整后的L值
                LimitRangeDeadBand[1][4] = LimitRangeDeadBand[1][4] ;    //数值点位于1区域 LLL<x<=LL时，调整后的H值
                LimitRangeDeadBand[1][5] = LimitRangeDeadBand[1][5] ;   //数值点位于1区域 LLL<x<=LL时，调整后的HH值
                LimitRangeDeadBand[1][6] = LimitRangeDeadBand[1][6] ;  //数值点位于1区域 LLL<x<=LL时，调整后的HHH值
                //当实时值位于L区时，当前区域值为currentRange=2，在L区内，返回Normal时，要求返回L+LDeadBand才有效。...
                //此区域currentRange=2，对应的限值数组 LimitRangeDeadBand[2]    
                LimitRangeDeadBand[2][0] = LimitRangeDeadBand[2][0] ;  //数值点位于2区 LL<x<=L时，调整后的LLL值
                LimitRangeDeadBand[2][1] = LimitRangeDeadBand[2][1] ;   //数值点位于2区 LL<x<=L时，调整后的LL值
                LimitRangeDeadBand[2][2] = LimitRangeDeadBand[2][2] + LDeadBand;    //数值点位于2区 LL<x<=L时，调整后的L值
                LimitRangeDeadBand[2][4] = LimitRangeDeadBand[2][4] ;    //数值点位于2区 LL<x<=L时，调整后的H值
                LimitRangeDeadBand[2][5] = LimitRangeDeadBand[2][5] ;   //数值点位于2区 LL<x<=L时，调整后的HH值
                LimitRangeDeadBand[2][6] = LimitRangeDeadBand[2][6] ;  //数值点位于2区 LL<x<=L时，调整后的HHH值
                //当实时值位于L-N正常区间时，当前区域值为currentRange=3，对应的限值数组 LimitRangeDeadBand[3]
                LimitRangeDeadBand[3][0] = LimitRangeDeadBand[3][0] ;  //数值点位于3区 L<x<=N时，调整后的LLL值
                LimitRangeDeadBand[3][1] = LimitRangeDeadBand[3][1] ;   //数值点位于3区 L<x<=N时，调整后的LL值
                LimitRangeDeadBand[3][2] = LimitRangeDeadBand[3][2] ;    //数值点位于3区 L<x<=N时，调整后的L值
                LimitRangeDeadBand[3][4] = LimitRangeDeadBand[3][4] ;    //数值点位于3区 L<x<=N时，调整后的H值
                LimitRangeDeadBand[3][5] = LimitRangeDeadBand[3][5] ;   //数值点位于3区 L<x<=N时，调整后的HH值
                LimitRangeDeadBand[3][6] = LimitRangeDeadBand[3][6] ;  //数值点位于3区 L<x<=N时，调整后的HHH值
                //当实时值位于N-H区时，当前区域值为currentRange=4，  ，对应的限值数组 LimitRangeDeadBand[4]          
                LimitRangeDeadBand[4][0] = LimitRangeDeadBand[4][0] ;  //数值点位于4区 N<x<=H时，调整后的LLL值
                LimitRangeDeadBand[4][1] = LimitRangeDeadBand[4][1] ;   //数值点位于4区 N<x<=H时，调整后的LL值
                LimitRangeDeadBand[4][2] = LimitRangeDeadBand[4][2] ;    //数值点位于4区 N<x<=H时，调整后的L值
                LimitRangeDeadBand[4][4] = LimitRangeDeadBand[4][4] ;    //数值点位于4区 N<x<=H时，调整后的H值
                LimitRangeDeadBand[4][5] = LimitRangeDeadBand[4][5] ;   //数值点位于4区 N<x<=H时，调整后的HH值
                LimitRangeDeadBand[4][6] = LimitRangeDeadBand[4][6] ;  //数值点位于4区 N<x<=H时，调整后的HHH值
                //当实时值位于HH区时，当前区域值为currentRange=5，对应的限值数组 LimitRangeDeadBand[5]           
                LimitRangeDeadBand[5][0] = LimitRangeDeadBand[5][0] ;  //数值点位于5区 H<x<=HH时，调整后的LLL值
                LimitRangeDeadBand[5][1] = LimitRangeDeadBand[5][1] ;   //数值点位于5区 H<x<=HH时，调整后的LL值
                LimitRangeDeadBand[5][2] = LimitRangeDeadBand[5][2] ;    //数值点位于5区 H<x<=HH时，调整后的L值
                LimitRangeDeadBand[5][4] = LimitRangeDeadBand[5][4] - HDeadBand;    //数值点位于5区 H<x<=HH时，调整后的H值
                LimitRangeDeadBand[5][5] = LimitRangeDeadBand[5][5] ;   //数值点位于5区 H<x<=HH时，调整后的HH值
                LimitRangeDeadBand[5][6] = LimitRangeDeadBand[5][6] ;  //数值点位于5区 H<x<=HH时，调整后的HHH值
                //当实时值位于HH区时，当前区域值为currentRange=5，对应的限值数组 LimitRangeDeadBand[6]          
                LimitRangeDeadBand[6][0] = LimitRangeDeadBand[6][0] ;  //数值点位于6区 HH<x<HHH时，调整后的LLL值
                LimitRangeDeadBand[6][1] = LimitRangeDeadBand[6][1] ;   //数值点位于6区 HH<x<HHH时，调整后的LL值
                LimitRangeDeadBand[6][2] = LimitRangeDeadBand[6][2] ;    //数值点位于6区 HH<x<HHH时，调整后的L值
                LimitRangeDeadBand[6][4] = LimitRangeDeadBand[6][4] - HDeadBand;    //数值点位于6区 HH<x<HHH时，调整后的H值
                LimitRangeDeadBand[6][5] = LimitRangeDeadBand[6][5] - HHDeadBand;   //数值点位于6区 HH<x<HHH时，调整后的HH值
                LimitRangeDeadBand[6][6] = LimitRangeDeadBand[6][6] ;  //数值点位于6区 HH<x<HHH时，调整后的HHH值
                //当实时值位于HHH区时，当前区域值为currentRange=7，，对应的限值数组 LimitRangeDeadBand[7]       
                LimitRangeDeadBand[7][0] = LimitRangeDeadBand[7][0] ;  //数值点位于7区 HH<x<HHH时，调整后的LLL值
                LimitRangeDeadBand[7][1] = LimitRangeDeadBand[7][1] ;   //数值点位于7区 HH<x<HHH时，调整后的LL值
                LimitRangeDeadBand[7][2] = LimitRangeDeadBand[7][2] ;    //数值点位于7区 HH<x<HHH时，调整后的L值
                LimitRangeDeadBand[7][4] = LimitRangeDeadBand[7][4] - HDeadBand;    //数值点位于7区 HH<x<HHH时，调整后的H值
                LimitRangeDeadBand[7][5] = LimitRangeDeadBand[7][5] - HHDeadBand;   //数值点位于7区 HH<x<HHH时，调整后的HH值
                LimitRangeDeadBand[7][6] = LimitRangeDeadBand[7][6] - HHHDeadBand;  //数值点位于7区 HH<x<HHH时，调整后的HHH值

                for (int iLimitRangeDeadBand = 0; iLimitRangeDeadBand < LimitRangeDeadBand.Length; iLimitRangeDeadBand++)
                {
                    for (int jLimitRangeDeadBand = 0; jLimitRangeDeadBand < LimitRangeDeadBand[iLimitRangeDeadBand].Length - 1; jLimitRangeDeadBand++)
                    {
                        if (LimitRangeDeadBand[iLimitRangeDeadBand][jLimitRangeDeadBand] >= LimitRangeDeadBand[iLimitRangeDeadBand][jLimitRangeDeadBand + 1])
                        {
                            _errorFlag = true;
                            _errorInfo = "计算参数不正确。死区值大小不合适，导致调整后的三高三低发生交错";
                            return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                        }
                    }
                }
                //2、准备存储变量  
                //特别注意这个totalspan，比如取2018-01-01 00:00:00 到2018-01-01 00:00:00 的值
                //取到的Pvalue最后一个点是30,2018-01-01 00:00:59----2018-01-01 00:01:00，
                //由于在2018-01-01 00:01:00时刻并不知道准确的值,因此没有办法对最后一秒做线性的超限统计。所以所有span对最后一个值得时间段不做统计。TotalSpan也不包括最后一个时间段。
                //这种方法，目前不准确。尤其是最后一个点的实际有效时间比较长的情况下。具体请参考PSLcalcu下的文档“主要问题_实时数据库取数据与把数据看成折线的相关计算问题”

                double TotalSpan = input[input.Count - 1].Timestamp.Subtract(input[0].Timestamp).TotalMilliseconds / 1000; //（单位，用毫秒/1000，转换为的秒值，保持毫秒精度）   
                double[] LimitNumber = new double[8];                   //各限值区域的次数统计
                double[] LimitSpan = new double[8];                     //各限值区域的时长统计  （单位，用毫秒/1000，转换为的秒值，保持毫秒精度）   
                double[] LimitSpanRatio = new double[8];                //各限值区域的时长占比
                double[] LimitArea = new double[8];                     //各限值区域的面积统计   (单位，数值*秒，其中秒为毫秒/1000转换，保持毫秒精度)
                double[] Limit2XAxisArea = new double[8];               //各限值区域点与X轴构成的面积     (单位，数值*秒，其中秒为毫秒/1000转换，保持毫秒精度)
                double[] LimitAreaRatio = new double[8];   	            //各限值区域的面积占比              
                List<PValue>[] LimitSpanSeries = new List<PValue>[8];   //各限值区域的时间序列统计（Value值采用毫秒转换的秒值。）
                for (i = 0; i < 8; i++)
                {
                    LimitSpanSeries[i] = new List<PValue>();
                }
                PValue[] LimitSpanMax = new PValue[8];                  //各限值区域的时间序列的最长时间序列值及时刻。（value值是毫秒值/1000转换的秒值，保留毫秒精度）
                for (i = 0; i < 8; i++)
                {
                    LimitSpanMax[i] = null;
                }
                double LV1Span = 0;                                     //速度小于V1的时长统计   （单位，用毫秒/1000，转换为的秒值，保持毫秒精度）
                double LV1SpanRatio = 0;                                //速度小于V1的时长占比
                //特别注意，这个与LimitSpanMax不一样。LimitSpanMax不一定有值，因为曲线不一定走到对应范围内。LV1SpanMax一定有值
                PValue LV1SpanMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //速度小于V1的最长时间段长度及时刻 （value值是毫秒值/1000转换的秒值，保留毫秒精度）
                double GV2Span = 0;                                     //速度小于V1的时长统计   （单位，用毫秒/1000，转换为的秒值，保持毫秒精度）
                double GV2SpanRatio = 0;                                //速度小于V1的时长占比
                PValue GV2SpanMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //速度小于V1的最长时间段长度及时刻 （value值是毫秒值/1000转换的秒值，保留毫秒精度）                


                //3、对数据进行遍历
                int currentRange = 10;                      //当前点的限值区域
                int previousRange = 10;                     //上一个点的限值区域
                PValue currentPoint;                        //当前点Pvalue值                
                PValue previousPoint;                       //上一点PValue值
                PValue[] crossPoint = new PValue[7];        //与限值的焦点
                PValue[] transPoint;                        //currentPoint、previousPoint及限值焦点构成的直线
                double[] SortArray = new double[8];         //当前数据点和7个限值点的排序
                double currentSpan = 0;                     //单位是 毫秒/1000转换为的秒值，保持毫秒精度
                double currentArea = 0;                     //单位，数值*秒，其中秒为毫秒/1000转换，保持毫秒精度
                double currentArea2XAxis = 0;               //单位，数值*秒，其中秒为毫秒/1000转换，保持毫秒精度
                int currentVRange = -1;                       //当前速度区间。1代表V1区间，2代表V2
                int previousVRange = -1;                    //前一个速度区间。1代表V1区间，2代表V2
                List<PValue>[] VSpanSeries = new List<PValue>[3];   //各速度区间的时间序列
                for (i = 0; i < 3; i++)
                {
                    VSpanSeries[i] = new List<PValue>();
                }

                //循环计算
                for (int iPoint = 0; iPoint < input.Count; iPoint++)
                {

                    //3.1、判断点的位置是否有变化
                    //3.1.1、计算当前点的所在的限值区域
                    if (iPoint == 0)
                    {
                        //计算第一个点的当前区域。当previousStatus == 10时，是第一个点
                        //计算第一个点的当前区域，采用正常值区域3对应的界限数组来判断第一个点的区域。这样规定算法，是有微小误差的。
                        //误差可能：如果统计周期前面的点，是从[H,HH]落入到[H,H-HDeadBand]内，实际的统计可能会有误差。这个需要在仔细分析
                        LimitRange.CopyTo(SortArray, 0);                         //如果是第一个点   
                        SortArray[7] = input[iPoint].Value;                                 //把当前点放入排序对象最后一个元素。
                        Array.Sort(SortArray);
                        currentRange = Array.IndexOf(SortArray, input[iPoint].Value);       //找第一个点的值的所在的区域，这个相当于在每个区间（L,H]内找
                        previousRange = currentRange;                                       //假定第一个点的前一时刻点的previousRange与currentRange相同
                        currentPoint = input[iPoint];                                       //获取当前点
                        previousPoint = input[iPoint];                                      //获取上一个点
                        //results[currentRange].Add(new PValue(1, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));  
                        LimitSpanSeries[currentRange].Add(new PValue(0, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    }
                    else
                    {
                        //不是第一个点
                        LimitRangeDeadBand[previousRange].CopyTo(SortArray, 0);
                        SortArray[7] = input[iPoint].Value;
                        Array.Sort(SortArray);
                        currentRange = Array.IndexOf(SortArray, input[iPoint].Value);       //找当前点所在的区域，这个相当于在每个区间（min,max]内找
                        currentPoint = input[iPoint];                                       //获取当前点
                        previousPoint = input[iPoint - 1];                                  //获取上一个点
                    }

                    //3.1.2、跟当前点所在限值区域与上一个点所在限值区域情况，进行统计
                    if (currentRange == previousRange)
                    {
                        //如果当前点的限值区域没有变化
                        //——则统计时长和面积到对应的previousRange内
                        //——第一个点，currentRange == previousRange，一定走该分支                    
                        currentSpan = (currentPoint.Timestamp - previousPoint.Timestamp).TotalMilliseconds / 1000;
                        if (currentRange > 4)
                        {
                            //——0区域代表MinValue<x<=LLL,  1区域代表LLL<x<=LL,  2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 6区域代表HH<x<HHH,  7区域代表HHH<x<=MaxValue
                            currentArea = (Math.Abs(currentPoint.Value - LimitRange[currentRange - 1]) + Math.Abs(previousPoint.Value - LimitRange[currentRange - 1])) * currentSpan / 2;
                        }
                        else if (currentRange < 3)
                        {
                            //——0区域代表MinValue<x<=LLL,  1区域代表LLL<x<=LL,  2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 6区域代表HH<x<HHH,  7区域代表HHH<x<=MaxValue
                            currentArea = (Math.Abs(currentPoint.Value - LimitRange[currentRange]) + Math.Abs(previousPoint.Value - LimitRange[currentRange])) * currentSpan / 2;
                        }
                        else
                            currentArea = 0;
                        currentArea2XAxis = (Math.Abs(currentPoint.Value) + Math.Abs(previousPoint.Value)) * currentSpan / 2;
                        //累计时长
                        LimitSpan[previousRange] = LimitSpan[previousRange] + currentSpan; //对于第一个点来说，currentSpan为0
                        //累计面积
                        LimitArea[previousRange] = LimitArea[previousRange] + currentArea; //对于第一个点来说，currentSpan为0
                        //累积到x轴的面积
                        Limit2XAxisArea[previousRange] = Limit2XAxisArea[previousRange] + currentArea2XAxis;
                    }
                    else
                    {
                        //如果当前的点的限值区域有变化，则
                        //——1、则计算交叉点，并和previousPoint、currentPoint构成直线
                        //——2、据直线的端点和交叉点统计时长和面积
                        if (currentRange > previousRange)
                        {
                            //1、求出焦点，并把焦点和previousPoint、currentPoint构成数组
                            transPoint = new PValue[currentRange - previousRange + 2];  //previousPoint、currentPoint以及和限值的交点构成的直线，共currentRange - previousRange + 2个点
                            transPoint[0] = previousPoint;
                            transPoint[transPoint.Length - 1] = currentPoint;
                            for (i = previousRange; i < currentRange; i++)   //这个循环是求交点，是< currentRange
                            {
                                //两点式：x=((y-y0)*(x1-x0)/(y1-y0))+x0
                                DateTime transcendtimestamp = previousPoint.Timestamp.AddMilliseconds(((LimitRangeDeadBand[previousRange][i] - previousPoint.Value) * (currentPoint.Timestamp - previousPoint.Timestamp).TotalMilliseconds) / (currentPoint.Value - previousPoint.Value));
                                //以后可以采用新的PValue新方法
                                //PValue middlepv=PValue.Interpolate(previousPoint,currentPoint,LimitRange[i - 1]);

                                transPoint[i - previousRange + 1] = new PValue(LimitRange[i], transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0);//得到交点                                
                                LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Endtime = transcendtimestamp;                                                  //写上一个range的结束时间
                                LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Value = LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Timespan / 1000;        //写上一个range的毫秒值/1000，转换成秒值，保持毫秒精度
                                LimitSpanSeries[i + 1].Add(new PValue(1, transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));        //写当期range的开始时间

                                //if (LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Value > LimitSpanMax[i].Value)
                                //    LimitSpanMax[i] = new PValue(LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Timespan / 1000, LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Timestamp, calcuinfo.fendtime, 0); //找最大值

                            }
                            //2、根区域跨越的点求累计时长和累计面积
                            for (i = previousRange; i <= currentRange; i++) //这个循环是求每一段累加值，是<= currentRange
                            {
                                //——第一个点，currentRange == previousRange，一定走该分支                    
                                currentSpan = (transPoint[i - previousRange + 1].Timestamp - transPoint[i - previousRange].Timestamp).TotalMilliseconds / 1000;
                                if (i > 4)
                                {
                                    currentArea = (Math.Abs(transPoint[i - previousRange + 1].Value - LimitRange[i - 1]) + Math.Abs(transPoint[i - previousRange].Value - LimitRange[i - 1])) * currentSpan / 2;
                                }
                                else if (i < 3)
                                {
                                    currentArea = (Math.Abs(transPoint[i - previousRange + 1].Value - LimitRange[i]) + Math.Abs(transPoint[i - previousRange].Value - LimitRange[i])) * currentSpan / 2;
                                }
                                else
                                    currentArea = 0;

                                currentArea2XAxis = (Math.Abs(transPoint[i - previousRange + 1].Value) + Math.Abs(transPoint[i - previousRange].Value)) * currentSpan / 2;
                                //累计时长
                                LimitSpan[i] = LimitSpan[i] + currentSpan; //对于第一个点来说，currentSpan为0
                                //累计面积
                                LimitArea[i] = LimitArea[i] + currentArea; //对于第一个点来说，currentSpan为0
                                //累积到x轴的面积
                                Limit2XAxisArea[previousRange] = Limit2XAxisArea[previousRange] + currentArea2XAxis;
                            }
                        }
                        else   //(currentRange < previousRange)
                        {
                            //1、求出焦点，并把焦点和previousPoint、currentPoint构成数组
                            transPoint = new PValue[previousRange - currentRange + 2];//previousPoint、currentPoint以及和限值的交点构成的直线，共currentRange - previousRange + 2个点
                            transPoint[0] = previousPoint;
                            transPoint[transPoint.Length - 1] = currentPoint;
                            for (i = previousRange; i > currentRange; i--)  //这个循环是求交点，是> currentRange
                            {
                                //两点式：x=((y-y0)*(x1-x0)/(y1-y0))+x0
                                DateTime transcendtimestamp = previousPoint.Timestamp.AddMilliseconds(((LimitRangeDeadBand[previousRange][i - 1] - previousPoint.Value) * (currentPoint.Timestamp - previousPoint.Timestamp).TotalMilliseconds) / (currentPoint.Value - previousPoint.Value));
                                //以后可以采用新的PValue新方法
                                //PValue middlepv=PValue.Interpolate(previousPoint,currentPoint,LimitRange[i - 1]);

                                transPoint[previousRange - i + 1] = new PValue(LimitRange[i - 1], transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0);
                                LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Endtime = transcendtimestamp;                                                  //写上一个range的结束时间
                                LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Value = LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Timespan / 1000;        //写上一个range的毫秒值/1000,单位是秒，但是保持毫秒精度
                                LimitSpanSeries[i - 1].Add(new PValue(1, transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));        //写当期range的开始时间
                                //if (LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Value > LimitSpanMax[i].Value)
                                //    LimitSpanMax[i] = new PValue(LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Timespan / 1000, LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Timestamp, calcuinfo.fendtime, 0); //找最大值

                            }
                            //2、根区域跨越的点求累计时长和累计面积
                            for (i = previousRange; i >= currentRange; i--)//这个循环是求每一段累加值，是>= currentRange
                            {
                                //——第一个点，currentRange == previousRange，一定走该分支                    
                                currentSpan = (transPoint[previousRange - i + 1].Timestamp - transPoint[previousRange - i].Timestamp).TotalMilliseconds / 1000;
                                if (i > 4)
                                {
                                    currentArea = (Math.Abs(transPoint[previousRange - i + 1].Value - LimitRange[i - 1]) + Math.Abs(transPoint[previousRange - i].Value - LimitRange[i - 1])) * currentSpan / 2;
                                }
                                else if (i < 3)
                                {
                                    currentArea = (Math.Abs(transPoint[previousRange - i + 1].Value - LimitRange[i]) + Math.Abs(transPoint[previousRange - i].Value - LimitRange[i])) * currentSpan / 2;
                                }
                                else
                                    currentArea = 0;

                                currentArea2XAxis = (Math.Abs(transPoint[previousRange - i + 1].Value) + Math.Abs(transPoint[previousRange - i].Value)) * currentSpan / 2;
                                //累计时长
                                LimitSpan[i] = LimitSpan[i] + currentSpan; //对于第一个点来说，currentSpan为0
                                //累计面积
                                LimitArea[i] = LimitArea[i] + currentArea; //对于第一个点来说，currentSpan为0
                                //累积到x轴的面积
                                Limit2XAxisArea[previousRange] = Limit2XAxisArea[previousRange] + currentArea2XAxis;
                            }
                        }


                        //——寻找下降沿或者上升沿统计超限次数
                        ////——0区域代表MinValue<x<=LLL,  1区域代表LLL<x<=LL,  2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 6区域代表HH<x<HHH,  7区域代表HHH<x<=MaxValue                 
                        //在3H区域看上升沿，即实时值由低向高值变动
                        if (currentRange > 4 && currentRange > previousRange)
                        {
                            for (i = previousRange; i < currentRange; i++)
                            {
                                if (i >= 4)
                                {
                                    LimitNumber[i + 1] = LimitNumber[i + 1] + 1;
                                }

                            }

                        }
                        //在3L区域看上升沿，即实时值由低值向高值变动
                        else if (currentRange < 3 && currentRange < previousRange)
                        {
                            for (i = previousRange; i > currentRange; i--)
                            {
                                if (i <= 3)
                                {
                                    LimitNumber[i - 1] = LimitNumber[i - 1] + 1;
                                }

                            }
                        }
                        previousRange = currentRange;
                    }
                    //4.2.3 处理结束点
                    if (iPoint == input.Count - 1)
                    {
                        LimitSpanSeries[currentRange][LimitSpanSeries[currentRange].Count - 1].Endtime = currentPoint.Endtime;
                        //目前该算法不处理最后一个点起始值到结束值之间的时间段。这是该算法统计存在误差的原因之一。
                    }

                    //5.速度大小判断
                    double currentV;

                    if (iPoint < input.Count - 1)   //最后两个点的速度不处理
                    {
                        currentV = (input[iPoint + 1].Value - input[iPoint].Value) / (input[iPoint + 1].Timestamp.Subtract(input[iPoint].Timestamp).TotalSeconds); //数据变化速度是指，变化值/时间间隔秒数

                        //速度大小判断
                        if (Math.Abs(currentV) <= V1)  //如果当前点input[iPoint]到下一点input[iPoint+1]的速度小于V1，则当前点的时长计入V1
                        {
                            LV1Span = LV1Span + input[iPoint].Timespan / 1000;     //与其他时长统计一致，采用毫秒数/1000，单位是秒，但是保留毫秒精度
                            //速度区间判断                       
                            currentVRange = 0;

                        }
                        else if (Math.Abs(currentV) >= V2)
                        {
                            GV2Span = GV2Span + input[iPoint].Timespan / 1000;         //与其他时长统计一致，采用毫秒数
                            currentVRange = 2;

                        }
                        else
                            currentVRange = 1;

                        if (currentVRange != previousVRange)
                        {
                            if (previousVRange == -1)
                            {
                                VSpanSeries[currentVRange].Add(new PValue(0, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                                previousVRange = currentVRange;
                            }
                            else
                            {
                                VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Endtime = input[iPoint].Timestamp;
                                VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value = VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Timespan / 1000; //使用 毫秒/1000， 单位是秒，精确度毫秒
                                VSpanSeries[currentVRange].Add(new PValue(0, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));

                            }
                            //找最大时间段
                            if (previousVRange == 0 && VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value > LV1SpanMax.Value)
                            {
                                LV1SpanMax = new PValue(VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value, VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Timestamp, calcuinfo.fendtime, 0);
                            }
                            else if (previousVRange == 2 && VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value > GV2SpanMax.Value)
                            {
                                GV2SpanMax = new PValue(VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value, VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Timestamp, calcuinfo.fendtime, 0);
                            }

                            previousVRange = currentVRange;
                        }
                    }
                    else if (iPoint == input.Count - 1)
                    {
                        //倒数第二个点。
                        VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Endtime = input[iPoint].Timestamp;
                        VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value = VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Timespan / 1000;

                        //找最大时间段
                        if (previousVRange == 0 && VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value > LV1SpanMax.Value)
                        {
                            LV1SpanMax = new PValue(VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value, VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Timestamp, calcuinfo.fendtime, 0);
                        }
                        else if (previousVRange == 2 && VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value > GV2SpanMax.Value)
                        {
                            GV2SpanMax = new PValue(VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Value, VSpanSeries[previousVRange][VSpanSeries[previousVRange].Count - 1].Timestamp, calcuinfo.fendtime, 0);
                        }
                    }
                    //速度计算完成


                }//end for 结束所有点的超限判断

                //计算时间占比
                for (i = 0; i < 8; i++)
                {
                    LimitSpanRatio[i] = LimitSpan[i] / TotalSpan;
                }

                //计算最长时间及起点时刻
                for (i = 0; i < 8; i++) 
                {
                    //从LimitSpanSeries[i]中找Value最大的对象，付给LimitSpanMax[i]，如果找不到则为null。
                    if (LimitSpanSeries[i] != null && LimitSpanSeries[i].Count != 0)
                    {
                        LimitSpanMax[i] = LimitSpanSeries[i].First(n => n.Value == LimitSpanSeries[i].Max(m => m.Value));
                    }
                    else
                    {
                        //如果对应的i段，没有数据，则i段对应的时间序列LimitSpanSeries[i]为空，则对应i段的最大时间段也为null。
                        LimitSpanMax[i] = null;
                    }
                }

                //对找出的时间序列进行过滤，如果长度小于threshold，则去掉。
                //——特别注意，对个值域总时长span的统计，时长占比，以及时间序列最大值得统计，不去掉小于threshold的时间段
                //——只有对各值域时间序列的统计，才会根据threshold去掉时间段。
                //——一般建议threshold设置为0
                if (threshold > 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        for (int j = LimitSpanSeries[i].Count - 1; j >= 0; j--)
                        {
                            if (LimitSpanSeries[i][j].Timespan < threshold * 1000)  //Timespan是毫秒值，threshold设定是秒值
                            {
                                LimitSpanSeries[i].RemoveAt(j);
                            }
                        }

                    }
                }
                //



                //组织结果的时候，不再对results重新初始化
                //——0区域代表MinValue<x<=LLL,  1区域代表LLL<x<=LL,  2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 6区域代表HH<x<HHH,  7区域代表HHH<x<=MaxValue
                //组织计算结果:共返回58个统计结果
               

                //7区域代表HHH<x<=MaxValue，在偏差考核中，对应偏差考核的>HH
                results[0] = new List<PValue>();
                results[0].Add(new PValue(LimitNumber[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                //越 HH 次数;如果没有，就是LimitNumber的初始值0
                results[1] = new List<PValue>();
                results[1] = LimitSpanSeries[7];                                                                        //越 HH 时间序列;如果没有，就是LimitSpanSeries的初始值null
                results[2] = new List<PValue>();
                results[2].Add(new PValue(LimitSpan[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                  //越 HH 总时长。如果没有，就是LimitSpan的初始值0
                results[3] = new List<PValue>();
                results[3].Add(new PValue(LimitSpanRatio[7] * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));       //越 HH 时间占比 %。如果没有，就是LimitSpan的初始值0（由LimitSpan除以总时长）
                results[4] = new List<PValue>();                
                if (LimitSpanMax[7] != null)    //特别注意LimitSpanMax[7]本身有可能是null，如果null直接用results[4].add添加，会导致results[4].count=1，但是results[4][0]=null。这样写结果时会报错
                    results[4].Add(LimitSpanMax[7]);                                                                    //单次越 HH 最长时间及时刻
                else
                    results[4].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));             //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计   
                results[5] = new List<PValue>();
                results[5].Add(new PValue(LimitArea[7]/3600, calcuinfo.fstarttime, calcuinfo.fendtime, 0));              //越 HH 面积。如果没哟，就是LimitArea的初始值0.
                results[6] = new List<PValue>();
                if (LimitSpan[7] != 0)
                    results[6].Add(new PValue(LimitArea[7] / LimitSpan[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //越 HH 平均值
                else
                    results[6].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     
                
                //6区域代表HH<x<HHH，在偏差考核中，对应偏差考核的H
                results[7] = new List<PValue>();
                results[7].Add(new PValue(LimitNumber[6], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                //越 H 次数;
                results[8] = new List<PValue>();
                results[8] = LimitSpanSeries[6];                                                                        //越 H 时间序列
                results[9] = new List<PValue>();
                results[9].Add(new PValue(LimitSpan[6], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                  //越 H 总时长
                results[10] = new List<PValue>();
                results[10].Add(new PValue(LimitSpanRatio[6] * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //越 H 时间占比%
                results[11] = new List<PValue>();
                if (LimitSpanMax[6] != null)
                    results[11].Add(LimitSpanMax[6]);                                                                       //单次越 H 最长时间及时刻 
                else
                    results[11].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计
                results[12] = new List<PValue>();
                results[12].Add(new PValue(LimitArea[6]/3600, calcuinfo.fstarttime, calcuinfo.fendtime, 0));                 //越 H 面积
                results[13] = new List<PValue>();
                if (LimitSpan[6] != 0)
                    results[13].Add(new PValue(LimitArea[6] / LimitSpan[6], calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //越 H 平均值
                else
                    results[13].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //1区域代表LLL<x<=LL 在偏差考核中，对应偏差考核的L
                results[14] = new List<PValue>();
                results[14].Add(new PValue(LimitNumber[1], calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //越 L 次数;
                results[15] = new List<PValue>();
                results[15] = LimitSpanSeries[1];                                                                //越 L 时间序列
                results[16] = new List<PValue>();
                results[16].Add(new PValue(LimitSpan[1], calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //越 L 总时长
                results[17] = new List<PValue>();
                results[17].Add(new PValue(LimitSpanRatio[1] * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //越 L 时间占比%
                results[18] = new List<PValue>();
                if (LimitSpanMax[1] != null)
                    results[18].Add(LimitSpanMax[1]);                                                                //单次越 L 最长时间及时刻   
                else
                    results[18].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计
                results[19] = new List<PValue>();
                results[19].Add(new PValue(LimitArea[1]/3600, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //越 L 面积

                results[20] = new List<PValue>();
                if (LimitSpan[1] != 0)
                    results[20].Add(new PValue(LimitArea[1] / LimitSpan[1], calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //越 L 平均值
                else
                    results[20].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));



                //0区域代表MinValue<x<=LLL，在偏差考核中，对应偏差考核的LL
                results[21] = new List<PValue>();
                results[21].Add(new PValue(LimitNumber[0], calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //越 LL 次数;
                results[22] = new List<PValue>();
                results[22] = LimitSpanSeries[0];                                                                //越 LL 时间序列
                results[23] = new List<PValue>();
                results[23].Add(new PValue(LimitSpan[0], calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //越 LL 总时长
                results[24] = new List<PValue>();
                results[24].Add(new PValue(LimitSpanRatio[0] * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //越 LL 时间占比%
                results[25] = new List<PValue>();
                if (LimitSpanMax[0] != null)
                    results[25].Add(LimitSpanMax[0]);                                                                //单次越 LL 最长时间及时刻
                else
                    results[25].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计
                 
                results[26] = new List<PValue>();
                results[26].Add(new PValue(LimitArea[0]/3600, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //越 LL 面积
                results[27] = new List<PValue>();
                if (LimitSpan[0] != 0)
                    results[27].Add(new PValue(LimitArea[0] / LimitSpan[0], calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //越 LL 平均值
                else
                    results[27].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //L<x<H区域，即偏差考核中的-R~R区域
                PValue SpanMax;
                results[28] = new List<PValue>();
                results[28].Add(new PValue(LimitSpan[3] + LimitSpan[4], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //L 到 H 总时长
                results[29] = new List<PValue>();
                results[29].Add(new PValue((LimitSpan[3] + LimitSpan[4]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //L 到 H 时间占比%               
                /*原算法有误
                if (LimitSpanMax[3] == null && LimitSpanMax[4] == null)
                    SpanMax = null;
                else if (LimitSpanMax[4] == null)
                    SpanMax = LimitSpanMax[3];
                else if (LimitSpanMax[3] == null)
                    SpanMax = LimitSpanMax[4];
                else
                    SpanMax = LimitSpanMax[4].Value > LimitSpanMax[3].Value ? LimitSpanMax[4] : LimitSpanMax[3];
                results[30] = new List<PValue>();
                if (SpanMax != null)    //特别注意SpanMax本身有可能是null，如果null直接用results[].add添加，会导致results[].count=1，但是results[][0]=null。这样写结果时会报错
                    results[30].Add(SpanMax);                                                                                                           //L 到 H 单次最长时间及时刻
                else
                    results[30] = null; 
                */
                int[] focusIndex30 = new int[] { 3, 4 };
                results[30] = new List<PValue>();
                results[30] = GetSpanMax(LimitSpanSeries, focusIndex30,calcuinfo);      //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计

                //4区域代表N<x<=H,5区域代表H<x<=HH，在偏差考核中，对应偏差考核的0——H
                results[31] = new List<PValue>();
                results[31].Add(new PValue(LimitSpan[4] + LimitSpan[5], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //0 到 H 总时长
                results[32] = new List<PValue>();
                results[32].Add(new PValue((LimitSpan[4] + LimitSpan[5]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //0 到 H 时间占比%
                /*原算法有误
                if (LimitSpanMax[4] == null && LimitSpanMax[5] == null)
                    SpanMax = null;
                else if (LimitSpanMax[4] == null)
                    SpanMax = LimitSpanMax[5];
                else if (LimitSpanMax[5] == null)
                    SpanMax = LimitSpanMax[4];
                else
                    SpanMax = LimitSpanMax[4].Value > LimitSpanMax[5].Value ? LimitSpanMax[4] : LimitSpanMax[5];                
                results[33] = new List<PValue>();
                if (SpanMax != null)    //特别注意SpanMax本身有可能是null，如果null直接用results[].add添加，会导致results[].count=1，但是results[][0]=null。这样写结果时会报错
                    results[33].Add(SpanMax);                                                                                                           //0 到 H 单次最长时间及时刻
                else
                    results[33] = null;
                */
                int[] focusIndex33 = new int[] { 4, 5 };
                results[33] = new List<PValue>();
                results[33] = GetSpanMax(LimitSpanSeries, focusIndex33, calcuinfo);      //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计

                //2区域代表LL<x<=L,  3区域代表L<x<=N，在偏差考核中，对应偏差考核的0——L
                results[34] = new List<PValue>();
                results[34].Add(new PValue(LimitSpan[2] + LimitSpan[3], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //0 到 L 总时长
                results[35] = new List<PValue>();
                results[35].Add(new PValue((LimitSpan[2] + LimitSpan[3]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));           //0 到 L 时间占比%
                /*原算法有误
                if (LimitSpanMax[2] == null && LimitSpanMax[3] == null)
                    SpanMax = null;
                else if (LimitSpanMax[2] == null)
                    SpanMax = LimitSpanMax[3];
                else if (LimitSpanMax[3] == null)
                    SpanMax = LimitSpanMax[2];
                else
                    SpanMax = LimitSpanMax[2].Value > LimitSpanMax[3].Value ? LimitSpanMax[2] : LimitSpanMax[3];
                results[36] = new List<PValue>();
                if (SpanMax != null)    //特别注意SpanMax本身有可能是null，如果null直接用results[].add添加，会导致results[].count=1，但是results[][0]=null。这样写结果时会报错
                    results[36].Add(SpanMax);                                                                                                            //0 到 L 单次最长时间及时刻
                else
                    results[36] = null;
                 */
                int[] focusIndex36 = new int[] { 2, 3 };
                results[36] = new List<PValue>();
                results[36] = GetSpanMax(LimitSpanSeries, focusIndex36, calcuinfo);      //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计

                //2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 
                //在偏差考核中，对应偏差考核的L-H
                results[37] = new List<PValue>();
                results[37].Add(new PValue(LimitSpan[2] + LimitSpan[3] + LimitSpan[4] + LimitSpan[5], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                    //H 到 L 之间总时长
                results[38] = new List<PValue>();
                results[38].Add(new PValue((LimitSpan[2] + LimitSpan[3] + LimitSpan[4] + LimitSpan[5]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));  //H 到 L 之间时间占比%                
                /*原算法有误
                List<PValue> SpanMaxSeries = new List<PValue>();
                for (int j = 2; j <= 5;j++ )
                    if (LimitSpanMax[j] != null) SpanMaxSeries.Add(LimitSpanMax[j]);        //从2-5区，如果LimitSpanMax不为空，则加入到SpanMaxSeries
                if (SpanMaxSeries.Count != 0)
                    SpanMax = SpanMaxSeries.First(n => n.Value == SpanMaxSeries.Max(m => m.Value));    //从SpanMaxSeries找到最大的PValue
                else
                    SpanMax = null;
                results[39] = new List<PValue>();
                if (SpanMax != null)
                    results[39].Add(SpanMax);
                else
                    results[39] = null;
                */
                int[] focusIndex39 = new int[] { 2, 3, 4, 5 };
                results[39] = new List<PValue>();
                results[39] = GetSpanMax(LimitSpanSeries, focusIndex39, calcuinfo);      //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计

                //6区域代表HH<x<HHH,1区域代表LLL<x<=LL,在偏差考核中对应H<x<HH,LL<x<L
                results[40] = new List<PValue>();
                results[40].Add(new PValue(LimitSpan[1] + LimitSpan[6], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //（H 到 HH）时间 +（L 到 LL）时间长
                results[41] = new List<PValue>();
                results[41].Add(new PValue((LimitSpan[1] + LimitSpan[6]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //（H 到 HH）时间 +（L 到 LL）时间占比%;

                //7区域代表HHH<x<=MaxValue，0区域代表MinValue<x<=LLL,,在偏差考核中对应x>HH,x<LL
                results[42] = new List<PValue>();
                results[42].Add(new PValue(LimitSpan[0] + LimitSpan[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //（> HH 时间）+（< LL）时间长
                results[43] = new List<PValue>();
                results[43].Add(new PValue((LimitSpan[0] + LimitSpan[7]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //（> HH 时间）+（< LL）时间占比%;

                //,在偏差考核中对应x>H,x<L
                results[44] = new List<PValue>();
                results[44].Add(new PValue(LimitSpan[0] + LimitSpan[7] + LimitSpan[1] + LimitSpan[6], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                          //（> H 时间）+（< L）时间长
                results[45] = new List<PValue>();
                results[45].Add(new PValue((LimitSpan[0] + LimitSpan[7] + LimitSpan[1] + LimitSpan[6]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //（> H 时间）+（< L）时间占比;


                //6区域代表HH<x<HHH,7区域代表HHH<x<=MaxValue,在偏差考核中对应>H，
                results[46] = new List<PValue>();
                results[46].Add(new PValue(LimitSpan[6] + LimitSpan[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //（> H 时间）+（>HH）时间长
                results[47] = new List<PValue>();
                results[47].Add(new PValue((LimitSpan[6] + LimitSpan[7]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //（> H 时间）+（>HH）时间占比;
                results[48] = new List<PValue>();
                if (LimitSpan[6] + LimitSpan[7] != 0)                                                                                               //( > H 时间）+（>HH）总平均值
                    results[48].Add(new PValue((LimitArea[6] + LimitArea[7]) / (LimitSpan[6] + LimitSpan[7]), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                else
                    results[48].Add(new PValue(0,calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //0区域代表MinValue<x<=LLL,1区域代表LLL<x<=LL,在偏差考核中对应<L，
                results[49] = new List<PValue>();
                results[49].Add(new PValue(LimitSpan[0] + LimitSpan[1], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                              //（< L 时间）+（<LL）时间长
                results[50] = new List<PValue>();
                results[50].Add(new PValue((LimitSpan[0] + LimitSpan[1]) * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //（< L 时间）+（<LL）时间长
                results[51] = new List<PValue>();
                if (LimitSpan[0] + LimitSpan[1] != 0)                                                                                                //（< L 时间）+（<LL）总平均值
                    results[51].Add(new PValue((LimitArea[0] + LimitArea[1]) / (LimitSpan[0] + LimitSpan[1]), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                else
                    results[51].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //速度变化绝对值小于 V1
                results[52] = new List<PValue>();
                results[52].Add(new PValue(LV1Span, calcuinfo.fstarttime, calcuinfo.fendtime, 0));         //速度变化绝对值小于 V1（稳定）时间长
                results[53] = new List<PValue>();
                results[53].Add(new PValue(LV1Span * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));    //稳定时间占比%
                results[54] = new List<PValue>();
                if (LV1SpanMax != null)
                    results[54].Add(LV1SpanMax);                                                                //单次最长稳定时间及时刻
                else
                    results[54].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0)); 

                //速度变化绝对值大于 V2
                results[55] = new List<PValue>();
                results[55].Add(new PValue(GV2Span, calcuinfo.fstarttime, calcuinfo.fendtime, 0));          //速度变化绝对值大于 V2（不稳定）时间长
                results[56] = new List<PValue>();
                results[56].Add(new PValue(GV2Span * 100 / TotalSpan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //不稳定时间占比%
                results[57] = new List<PValue>();
                if (GV2SpanMax != null)
                    results[57].Add(GV2SpanMax);                                                                //单次最长不稳定时间及时刻
                else
                    results[57].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));


                //大于0的时长  
                results[58] = new List<PValue>();   //全部正时间占比就是4、5、6、7区域占比之和
                results[58].Add(new PValue(LimitSpan[4] + LimitSpan[5] + LimitSpan[6] + LimitSpan[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //大于0的占比
                results[59] = new List<PValue>();   //全部正时间占比就是4、5、6、7区域占比之和
                results[59].Add(new PValue((LimitSpanRatio[4] + LimitSpanRatio[5] + LimitSpanRatio[6] + LimitSpanRatio[7]) * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //大于 0的面积
                results[60] = new List<PValue>();   //全部正面积占比%
                results[60].Add(new PValue((Limit2XAxisArea[4] + Limit2XAxisArea[5] + Limit2XAxisArea[6] + Limit2XAxisArea[7])/3600, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //小于0的时长
                results[61] = new List<PValue>();   //全部负时间占比就是0、1、2、3区域占比之和
                results[61].Add(new PValue(LimitSpan[0] + LimitSpan[1] + LimitSpan[2] + LimitSpan[3], calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //小于0的占比
                results[62] = new List<PValue>();   //全部负时间占比就是0、1、2、3区域占比之和
                results[62].Add(new PValue((LimitSpanRatio[0] + LimitSpanRatio[1] + LimitSpanRatio[2] + LimitSpanRatio[3]) * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                //小于0的面积
                results[63] = new List<PValue>();   //全部负面积占比%
                results[63].Add(new PValue((Limit2XAxisArea[0] + Limit2XAxisArea[1] + Limit2XAxisArea[2] + Limit2XAxisArea[3])/3600, calcuinfo.fstarttime, calcuinfo.fendtime, 0));


                //正负面积
                results[64] = new List<PValue>();   //全部正面积占比%
                if (results[60][0].Value + results[63][0].Value != 0)
                {
                    results[64].Add(new PValue(results[60][0].Value * 100 / (results[60][0].Value+results[63][0].Value), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                else
                    results[64].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //安全系数（H~L之间的占比）
                results[65] = new List<PValue>();
                results[65].Add(new PValue(results[38][0].Value, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //越H限强度指数(包括HH)：越H和HH面积之和，除以两者时间之和
                results[66] = new List<PValue>();
                if ((LimitSpan[6] + LimitSpan[7]) == 0)
                {
                    results[66].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                else
                {
                    results[66].Add(new PValue((LimitArea[6] + LimitArea[7]) / (LimitSpan[6] + LimitSpan[7]), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }

                //越HH限强度指数：越HH面积，除以越HH时间(即为越HH部分平均值，与结果results[6]相同)
                results[67] = new List<PValue>();
                if ( LimitSpan[7] == 0)
                {
                    results[67].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                else
                {
                    results[67].Add(new PValue(LimitArea[7] /  LimitSpan[7], calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                
                //越H力度指数：越H强度次数（包括HH），乘以越H强度指数
                results[68] = new List<PValue>();
                results[68].Add(new PValue((LimitNumber[7] + LimitNumber[6]) * results[65][0].Value, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //越HH力度指数：越HH强度次数，乘以越HH强度指数
                results[69] = new List<PValue>();
                results[69].Add(new PValue(LimitNumber[7]  * results[66][0].Value, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //越L限强度指数(包括LL)：越L和LL面积之和，除以两者时间之和
                results[70] = new List<PValue>();
                if ((LimitSpan[0] + LimitSpan[1]) == 0)
                {
                    results[70].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                else
                {
                    results[70].Add(new PValue((LimitArea[0] + LimitArea[1]) / (LimitSpan[0] + LimitSpan[1]), calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }

                //越LL限强度指数：越LL面积，除以越LL时间(即为越LL部分平均值，与结果results[]相同)
                results[71] = new List<PValue>();
                if (LimitSpan[0] == 0)
                {
                    results[71].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }
                else
                {
                    results[71].Add(new PValue(LimitArea[0] / LimitSpan[0], calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }

                //越L力度指数：越L强度次数（包括LL），乘以越L强度指数
                results[72] = new List<PValue>();
                results[72].Add(new PValue((LimitNumber[0] + LimitNumber[1]) * results[70][0].Value, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                //越LL力度指数：越LL强度次数，乘以越LL强度指数
                results[73] = new List<PValue>();
                results[73].Add(new PValue(LimitNumber[0] * results[71][0].Value, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuinfo.fmodulename, calcuInfo.sourcetagname, calcuinfo.fstarttime.ToString(), calcuinfo.fendtime.ToString());
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
        
        #region 辅助函数
        //从LimitSpanSeries中，在感兴趣的组中，找出最大的span。注意收尾时间相同的要连接。
        private static List<PValue> GetSpanMax(List<PValue>[] LimitSpanSeries, int[] focusIndex,CalcuInfo calcuinfo)
        {
            List<PValue> results = new List<PValue>();
            List<PValue> totalspan = new List<PValue>();
            //把关注的span合并
            for (int i = 0; i < focusIndex.Length; i++)
            {
                if (LimitSpanSeries[focusIndex[i]] != null && LimitSpanSeries[focusIndex[i]].Count > 0)
                {
                    for (int j = 0; j < LimitSpanSeries[focusIndex[i]].Count; j++)
                    {
                        //特别注意这里不能直接把LimitSpanSeries[i][j]加入到totalspan。因为totalspan后面要删除，会影响原有的LimitSpanSeries。
                        totalspan.Add(new PValue(LimitSpanSeries[focusIndex[i]][j].Value, LimitSpanSeries[focusIndex[i]][j].Timestamp, LimitSpanSeries[focusIndex[i]][j].Endtime, LimitSpanSeries[focusIndex[i]][j].Status));
                    }
                }
            }

            if (totalspan == null || totalspan.Count == 0) 
            {
                results.Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));    //20181109 为空的时候，返回一个值为0，起止时刻为计算周期的pvalue值，以便后续算法统计
                return results;
            };
            //排序，然后把前后相连
            totalspan = totalspan.OrderBy(m => m.Timestamp).ToList();
            for (int i = totalspan.Count - 1; i > 0; i--)
            {
                if (totalspan[i].Timestamp == totalspan[i - 1].Endtime)
                {
                    totalspan[i].Timestamp = totalspan[i - 1].Timestamp;
                    totalspan[i].Value = totalspan[i - 1].Value + totalspan[i].Value;
                    totalspan.RemoveAt(i - 1);
                }
            }
            //找出最大的span
            PValue SpanMax = totalspan.First(n => n.Value == totalspan.Max(m => m.Value));    //从SpanMaxSeries找到最大的PValue

            //结果            
            results.Add(SpanMax);
            return results;
        }

        #endregion
    }
}
