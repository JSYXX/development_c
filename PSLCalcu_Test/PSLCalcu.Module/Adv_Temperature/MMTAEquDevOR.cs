using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备偏差超限统计算法   
    /// 
    /// ——MMTAEquDevOR直接用一个设备所有点每小时、日的超限算法结果来计算
    /// ——算法周期仅在小时和日周期上进行计算。   
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
    public class MMTAEquDevOR : BaseModule, IModule
    {

        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquDevOR";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备超限统计算法";
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
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
       
        private int _outputNumber = 34;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EquHHN;" +                    //1               
                                    "EquHHTMax;" +                                    
                                    "EquHN;" +                                    
                                    "EquHTMax;" +                                    
                                    "EquLN;" +                                    
                                    "EquLTMax;" +                                    
                                    "EquLLN;" +                                    
                                    "EquLLTMax;" +                  //8                                    
                                   
                                    "EquRRTMax;" +                  //9
                                    "Equ0HTMax;" +        
                                    "Equ0LTMax;" +                                   
                                    "EquHLTMax;" +                                    
                                    "EquLV1TMax;" +                                    
                                    "EquGV2TMax;" +                //14
                                   
                                    "EquEvaSRMin;" +                //15
                                    "EquEvaSRAvg;" +            
                                    "EquEvaSRMax;" +
                                    "EquEvaSRStdev;" +               //18

                                    "EquEvaHHT;" +                  //19
                                    "EquEvaHHTF;" +
                                    "EquEvaHT;" +                                                       
                                    "EquEvaHTF;" +
                                    "EquEvaLT;" +
                                    "EquEvaLTF;" +
                                    "EquEvaLLT;" +                                    
                                    "EquEvaLLTF;"+                  //26

                                    "EquHHTubeN;"+                      //27
                                    "EquHHTubeNR;"+
                                    "EquHTotalTubeN;"+
                                    "EquHTotalTubeNR;"+
                                    "EquLTotalTubeN;" +
                                    "EquLTotalTubeNR;" +
                                    "EquLLTubeN;"+
                                    "EquLLTubeNR"                       //34
                                   
                                    ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "越 HH 次数;" +                                        
                                        "越 HH 单次最长时间及时刻(秒);" +                                        
                                        "越 H 次数;" +                                        
                                        "越 H 单次最长时间及时刻;" +                                        
                                        "越 L 次数;" +                                        
                                        "越 L 单次最长时间及时刻;" +                                        
                                        "越 LL 次数;" +                                       
                                        "越 LL 单次最长时间及时刻;" + 

                                        "-R 到 R 单次最长时间及时刻;" +                                        
                                        "0 到 H 单次最长时间及时刻;" +                                        
                                        "0 到 L 单次最长时间及时刻;" +                                        
                                        "H 到 L 之间单次最长时间及时刻;" +                                       
                                        "单次最长稳定时间及时刻;" +                                        
                                        "单次最长不稳定时间及时刻;" +
                                       
                                        "安全系数最小值,H~L时间占比;" +
                                        "安全系数均值;"+
                                        "安全系数最大致;"+
                                        "安全系数均差;"+

                                        "越 H 限强度指数;" +
                                        "越 HH 限强度指数;" +
                                        "越 H 限力度指数;" +
                                        "越 HH 限力度指数;" +
                                        "越 L 限强度指数;" +
                                        "越 LL 限强度指数;" +
                                        "越 L 限力度指数;" +
                                        "越 LL 限力度指数;"+

                                        "越HH管数;"+
                                        "越HH管数占比;"+
                                        "越H管数（包括越HH）;"+
                                        "越H管数占比（包括越HH）;"+                                        
                                        "越L管数（包括越HH）;"+
                                        "越L管数占比（包括越HH）;"+
                                        "越LL管数;"+
                                        "越LL管数占比"
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
            List<PValue>[] results = new List<PValue>[34];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }
            //其中越线时间段、极值时刻的相应项为空           
            results[1] = null;      //越HH最长时间
            results[3] = null;      //越H最长时间
            results[5] = null;     //越L最长时间
            results[7] = null;     //越LL最长时间
            results[8] = null;     //RR最长时间
            results[9] = null;     //0H最长时间
            results[10] = null;     //0L最长时间
            results[11] = null;     //LH最长时间
            results[12] = null;     //V1最长稳定时间
            results[13] = null;     //V2最长不稳定时间
            results[14] = null;     //安全系数最小值
            results[16] = null;     //安全系数最大值

            try
            {
                int MMTAEquDevOROutputNumber = 74;   //本算法是直接读取MMTAAnaDevOR算法所有计算结果。要靠此参数完成对计算结果的解析。                
                //数据量分析
                //——MMTAAnaDevOR74个计算结果，大部分只有一个值，一个设备100个温度点，那么也就7400个点。readmulti读取速度不会太慢。
                //——如果计算周期为天，则直接采用MMTAAnaDevOR周期为天的计算结果
                
                //0、输入：输入是同一设备上所有点的MMTAAnaDevOR小时计算结果。如果按小时取，每一个计算结果一个值。如果按天取，每一个计算结果24个值。
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.               
                if (inputs == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }

                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //按小时取，输入的是该设备上所有温度测点每分钟均值，也就是每个测点，仅有两个值，一个是分钟均值，一个是截止时刻值
                List<PValue>[] categoryInputs = new List<PValue>[MMTAEquDevOROutputNumber];
                bool[] categoryNullFlags = new bool[MMTAEquDevOROutputNumber];                   //categoryFlags是74种计算结果，每种结果是否包含null的标志位  
                bool[] errorStatusInDevice = new bool[MMTAEquDevOROutputNumber];                 //errorStatusInDevice是74种计算结果，每种结果是否包含null的标志位
                                List<double>[] categoryInputsvalue = new List<double>[MMTAEquDevOROutputNumber];                        //仅用于测试，为了便于观察各类的值
                for (i = 0; i < inputs.Length; i++)
                {
                    //按MMTAEquDevOR算法所有计算结果的顺序，分类整理好所有点。同一类计算结果放在一起。
                    //对于设备来说，某些单点状态位异常，直接过滤而不进行报警。只有这一类计算的结果全部异常才报警。
                    if (inputs[i] != null && inputs[i].Count >= 2)
                    {
                        //分配数据
                        if (categoryInputs[i % MMTAEquDevOROutputNumber] == null) categoryInputs[i % MMTAEquDevOROutputNumber] = new List<PValue>();
                        if (categoryInputsvalue[i % MMTAEquDevOROutputNumber] == null) categoryInputsvalue[i % MMTAEquDevOROutputNumber] = new List<double>();

                        //特别注意，计算周期为hour，取的是MMTAEquDevOR周期为hour的计算值，计算周期为day，取的是MMTAEquDevOR周期为day的计算值。因此inputs下正常情况，应该有两个值，其中第一个是有效值。
                        //但是要注意一些特别的点，如单次最长时间及时刻。
                        //这个点，在并行计算分割后，因为并行计算自动分割特性，或产生两个或三个值。
                        //如果，该hour周期内的单次最长时间及时刻，恰好在hour起始时间，则有两个值点。第一个是有效值点。
                        //如果，该hour周期内的单次最长时间及时刻，在中间，则并发计算组织的数据，在该hour内有三个值点。其中第一个值点，是并发计算补进去的点，时刻从hour起始时间开始，到真正的最大值点截止。其值为上一个最大值。这个值是不对的。
                        if (i % MMTAEquDevOROutputNumber == 4 ||            //越HH最长时间
                            i % MMTAEquDevOROutputNumber == 11 ||           //越H最长时间
                            i % MMTAEquDevOROutputNumber == 25 ||             //越LL最长时间
                            i % MMTAEquDevOROutputNumber == 18 ||                //越L最长时间
                            i % MMTAEquDevOROutputNumber == 30  ||        //RR间单次最长
                            i % MMTAEquDevOROutputNumber == 33 ||
                            i % MMTAEquDevOROutputNumber == 36 ||
                            i % MMTAEquDevOROutputNumber == 39 ||
                            i % MMTAEquDevOROutputNumber == 54 ||       //速度小于V1最长时间
                            i % MMTAEquDevOROutputNumber == 57          //速度大于V2最长时间
                            )
                        {
                            int rightDataPoint = inputs[i].Count - 2;   //单次最长时间及时刻，取倒数第二个点是正确点
                            if (inputs[i][rightDataPoint].Status == 0)
                            {
                                categoryInputs[i % MMTAEquDevOROutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAEquDevOROutputNumber].Add(inputs[i][rightDataPoint].Value);
                            }
                            else
                            {
                                categoryInputs[i % MMTAEquDevOROutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAEquDevOROutputNumber].Add(inputs[i][rightDataPoint].Value);
                                errorStatusInDevice[i % MMTAEquDevOROutputNumber] = true;       //状态位错误标志。只要有一个状态位不正确，就不计算
                            }
                        }
                        else
                        {
                            int rightDataPoint = 0;   //    其他点，取第一个点
                            if (inputs[i][rightDataPoint].Status == 0)
                            {
                                categoryInputs[i % MMTAEquDevOROutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAEquDevOROutputNumber].Add(inputs[i][rightDataPoint].Value);
                            }
                            else
                            {
                                categoryInputs[i % MMTAEquDevOROutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAEquDevOROutputNumber].Add(inputs[i][rightDataPoint].Value);
                                errorStatusInDevice[i % MMTAEquDevOROutputNumber] = true;       //状态位错误标志。只要有一个状态位不正确，就不计算
                            }
                        }
                    }
                    else
                    {
                        //分配数据
                        if (categoryInputs[i % MMTAEquDevOROutputNumber] == null) categoryInputs[i % MMTAEquDevOROutputNumber] = new List<PValue>();
                        if (categoryInputsvalue[i % MMTAEquDevOROutputNumber] == null) categoryInputsvalue[i % MMTAEquDevOROutputNumber] = new List<double>();

                        categoryInputs[i % MMTAEquDevOROutputNumber].Add(null);
                        categoryNullFlags[i % MMTAEquDevOROutputNumber] = true;      //如果某一个分类中加入了null，则该分类的null标记为true
                    }

                }

                /* EquDevOR计算，并不需要用到所有的计算结果。因此不需要整体判断。仅在用到计算结果时进行判断
                //如果经过过滤有，74类中有任意一项为空，即不含任何数据，则报警，并直接返回。因此本计算有些计算结果需要19类结果中的多个计算值。所以要求19个计算结果至少都有正常值
                for (i = 0; i < MMTAEquDevOROutputNumber; i++)
                {
                    if (categoryInputs[i] == null || categoryInputs[i].Count == 0)
                    {
                        _warningFlag = true;
                        _warningInfo = "当前时间段过滤非正常状态点后，有部分标签点，没有有效数据！";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                    }
                }
                */      
                
                //定义计算结果
                PValue EquHHN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHHTMax=new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLLN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquRRTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue Equ0HTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue Equ0LTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLV1TMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquGV2TMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);

                PValue EquEvaSRMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaSRMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaSRAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaSRStdev = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaHT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaHHT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaHTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaHHTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaLT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaLLT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaLTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquEvaLLTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);

                PValue EquHHTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHHTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHTotalTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquHTotalTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLTotalTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLTotalTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLLTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull);
                PValue EquLLTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull); 

                double valuesum = 0;
                inputs = categoryInputs;
                
                //1、越HH次数，各测点越限次数总和
                if (calcuinfo.fsourtagflags[0] == false)
                {
                    //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                    EquHHN = null;
                    EquHHTubeN = null;
                    EquHHTubeNR = null;
                }
                else if (categoryNullFlags[0] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越HH次数！";
                    EquHHN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquHHTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquHHTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[0] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越HH次数状态位不正常！";
                    EquHHN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquHHTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquHHTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    valuesum = 0;
                    for (i = 0; i < inputs[0].Count; i++)
                    {
                        valuesum += inputs[0][i].Value;
                    }
                    EquHHN.Value = valuesum;
                    EquHHN.Status = 0;

                    //27、越HH管数
                    valuesum = 0;
                    for (i = 0; i < inputs[0].Count; i++)
                    {
                        if (inputs[0][i].Value > 0) valuesum += 1;
                    }
                    EquHHTubeN.Value = valuesum;
                    EquHHTubeN.Status = 0;

                    //28、越HH管数比例
                    if (inputs[0].Count != 0)
                    {
                        EquHHTubeNR.Value = EquHHTubeN.Value * 100 / inputs[0].Count;
                        EquHHTubeNR.Status = 0;
                    }
                    else
                    {
                        EquHHTubeNR.Value = 0;
                        EquHHTubeNR.Status = 0;
                    }
                }

                //2、越HH最长时间
                if (calcuinfo.fsourtagflags[4] == false)
                {
                    //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                    EquHHTMax = null;                   
                }
                else if (categoryNullFlags[4] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越HH最长时间！";
                    EquHHTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                    
                }
                else if (errorStatusInDevice[4] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越HH最长时间状态位不正常！";
                    EquHHTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquHHTMax = inputs[4].First(n => n.Value == inputs[4].Max(m => m.Value));
                }
                
                
                //3、越H次数，各测点越限次数总和
                if (calcuinfo.fsourtagflags[7] == false)
                {
                    //如果计算这个点的
                     EquHN = null;
                     EquHTotalTubeN = null;
                     EquHTotalTubeNR = null;
                   
                }
                else if (categoryNullFlags[7] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越H次数！";
                    EquHN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquHTotalTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquHTotalTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[7] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越H次数状态位不正常！";
                    EquHN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquHTotalTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquHTotalTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    valuesum = 0;
                    for (i = 0; i < inputs[7].Count; i++)
                    {
                        valuesum += inputs[7][i].Value;
                    }
                    EquHN.Value = valuesum;
                    EquHN.Status = 0;

                    //29、越H管数
                    valuesum = 0;
                    for (i = 0; i < inputs[7].Count; i++)
                    {
                        if (inputs[7][i].Value > 0) valuesum += 1;
                    }
                    EquHTotalTubeN.Value = valuesum + EquHHTubeN.Value;
                    EquHTotalTubeN.Status=0;

                    //30、越H管数比例
                    if (inputs[7].Count != 0)
                    {
                        EquHTotalTubeNR.Value = EquHTotalTubeN.Value * 100 / inputs[7].Count;
                        EquHTotalTubeNR.Status = 0;
                    }
                    else
                    {
                        EquHTotalTubeNR.Value = 0;
                        EquHTotalTubeNR.Status = 0;
                    }
                  
                }

                //4、越H最长时间
                if (calcuinfo.fsourtagflags[11] == false)
                {
                    //如果计算这个点的
                    EquHTMax = null;
                    
                }
                else if (categoryNullFlags[11] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越H最长时间！";
                    EquHTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[11] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越H最长时间状态位不正常！";
                    EquHTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquHTMax = inputs[11].First(n => n.Value == inputs[11].Max(m => m.Value));                    
                }
               
                //7、越LL次数，各测点越限次数总和                
                if (calcuinfo.fsourtagflags[21] == false)
                {
                    //如果计算这个点的
                    EquLLN = null;
                    EquLLTubeN = null;
                    EquLLTubeNR = null;

                }
                else if (categoryNullFlags[21] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越LL次数！";
                    EquLLN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquLLTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquLLTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[21] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越LL次数状态位不正常！";
                    EquLLN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquLLTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquLLTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    valuesum = 0;
                    for (i = 0; i < inputs[21].Count; i++)
                    {
                        valuesum += inputs[21][i].Value;
                    }
                    EquLLN.Value = valuesum;
                    EquLLN.Status = 0;

                    //31、越LL管数
                    valuesum = 0;
                    for (i = 0; i < inputs[21].Count; i++)
                    {
                        if (inputs[21][i].Value > 0) valuesum += 1;
                    }
                    EquLLTubeN.Value = valuesum;
                    EquLLTubeN.Status = 0;

                    //32、越LL管数比例
                    if (inputs[21].Count != 0)
                    {
                        EquLLTubeNR.Value = EquLLTubeN.Value * 100 / inputs[21].Count;
                        EquLLTubeNR.Status = 0;
                    }
                    else
                    {
                        EquLLTubeNR.Value = 0;
                        EquLLTubeNR.Status = 0;
                    }
                }
                
                //8、越LL最长时间
                if (calcuinfo.fsourtagflags[25] == false)
                {
                    //如果计算这个点的
                    EquLLTMax = null;                   

                }
                else if (categoryNullFlags[25] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越LL最长时间！";
                    EquLLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[25] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越LL最长时间状态位不正常！";
                    EquLLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquLLTMax = inputs[25].First(n => n.Value == inputs[25].Max(m => m.Value));
                }

                //5、越L次数，各测点越限次数总和(放在LL之后，是应为越L的管数包含LL的数量)
                if (calcuinfo.fsourtagflags[14] == false)
                {
                    //如果计算这个点的
                    EquLN = null;
                    EquLTotalTubeN = null;
                    EquLTotalTubeNR = null;                   
                }
                else if (categoryNullFlags[14] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越L次数！";
                    EquLN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquLTotalTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquLTotalTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[14] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越L次数状态位不正常！";
                    EquLN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquLTotalTubeN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquLTotalTubeNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    valuesum = 0;
                    for (i = 0; i < inputs[14].Count; i++)
                    {
                        valuesum += inputs[14][i].Value;
                    }
                    EquLN.Value = valuesum;
                    EquLN.Status = 0;

                    //33、越L管数
                    valuesum = 0;
                    for (i = 0; i < inputs[14].Count; i++)
                    {
                        if (inputs[14][i].Value > 0) valuesum += 1;
                    }
                    EquLTotalTubeN.Value  = valuesum + EquLLTubeN.Value;
                    EquLTotalTubeN.Status = 0;

                    //34、越H管数比例      
                    if (inputs[14].Count != 0)
                    {
                        EquLTotalTubeNR.Value = EquLTotalTubeN.Value * 100 / inputs[14].Count;
                        EquLTotalTubeNR.Status = 0;
                    }
                    else
                    {
                        EquLTotalTubeNR.Value = 0;
                        EquLTotalTubeNR.Status = 0;
                    }

                }
                //6、越L最长时间
                if (calcuinfo.fsourtagflags[18] == false)
                {
                    //如果计算这个点的
                    EquLTMax = null;                   
                }
                else if (categoryNullFlags[18] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越L最长时间！";
                    EquLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[18] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越L最长时间状态位不正常！";
                    EquLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquLTMax = inputs[18].First(n => n.Value == inputs[18].Max(m => m.Value));
                }

                
                //9、RR单次最长                
                if (calcuinfo.fsourtagflags[30] == false)
                {
                    //如果计算这个点的
                    EquRRTMax = null;
                }
                else if (categoryNullFlags[30] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越RR最长时间！";
                    EquRRTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[30] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越RR最长时间状态位不正常！";
                    EquRRTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquRRTMax = inputs[30].First(n => n.Value == inputs[30].Max(m => m.Value));
                }
                //10、0H单次最长
                if (calcuinfo.fsourtagflags[33] == false)
                {
                    //如果计算这个点的
                    Equ0HTMax = null;
                }
                else if (categoryNullFlags[33] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越0H最长时间！";
                    Equ0HTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[33] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越0H最长时间状态位不正常！";
                    Equ0HTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    Equ0HTMax = inputs[33].First(n => n.Value == inputs[33].Max(m => m.Value));
                }
                //11、0L单次最长
                if (calcuinfo.fsourtagflags[36] == false)
                {
                    //如果计算这个点的
                    Equ0LTMax = null;
                }
                else if (categoryNullFlags[36] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越0L最长时间！";
                    Equ0LTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[36] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越0L最长时间状态位不正常！";
                    Equ0LTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    Equ0LTMax = inputs[36].First(n => n.Value == inputs[36].Max(m => m.Value));
                }
                //12、HL单次最长
                if (calcuinfo.fsourtagflags[39] == false)
                {
                    //如果计算这个点的
                    EquHLTMax = null;
                }
                else if (categoryNullFlags[39] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越HL最长时间！";
                    EquHLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[39] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越HL最长时间状态位不正常！";
                    EquHLTMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquHLTMax = inputs[39].First(n => n.Value == inputs[39].Max(m => m.Value));
                }

                //13、速度小于V1最长时间
                if (calcuinfo.fsourtagflags[54] == false)
                {
                    //如果计算这个点的
                    EquLV1TMax = null;
                }
                else if (categoryNullFlags[54] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有速度小于V1最长时间的值！";
                    EquLV1TMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[54] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点速度小于V1最长时间状态位不正常！";
                    EquLV1TMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquLV1TMax = inputs[54].First(n => n.Value == inputs[54].Max(m => m.Value));
                }
                //14、速度大于V2最长时间
                if (calcuinfo.fsourtagflags[57] == false)
                {
                    //如果计算这个点的
                    EquGV2TMax = null;
                }
                else if (categoryNullFlags[57] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有速度大于V2最长时间的值！";
                    EquGV2TMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[57] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点速度大于V2最长时间状态位不正常！";
                    EquGV2TMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {  
                    EquGV2TMax = inputs[57].First(n => n.Value == inputs[57].Max(m => m.Value));
                }

                
                //15\
                if (calcuinfo.fsourtagflags[65] == false)
                {
                    //如果计算这个点的
                    EquEvaSRMin = null;
                    EquEvaSRMax = null;
                    EquEvaSRAvg = null;
                    EquEvaSRStdev = null;
                }
                else if (categoryNullFlags[65] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有安全系数的值！";
                    EquEvaSRMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquEvaSRMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquEvaSRAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquEvaSRStdev = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[65] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点安全系数状态位不正常！";
                    EquEvaSRMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquEvaSRMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquEvaSRAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquEvaSRStdev = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    //15、安全系数最小值
                    EquEvaSRMin = inputs[65].First(n => n.Value == inputs[65].Min(m => m.Value));
                    //16、安全系数最大值
                    EquEvaSRMax = inputs[65].First(n => n.Value == inputs[65].Max(m => m.Value));
                    //17、安全系数均值
                    valuesum = 0;
                    for (i = 0; i < inputs[65].Count; i++)
                    {
                        valuesum += inputs[65][i].Value;
                    }
                    if (inputs[65].Count != 0)
                        EquEvaSRAvg.Value = valuesum / inputs[65].Count;
                    else
                        EquEvaSRAvg.Value = 0;

                    EquEvaSRAvg.Status=0;

                    //18、安全系数方差
                    valuesum = 0;
                    for (i = 0; i < inputs[65].Count; i++)
                    {
                        valuesum += Math.Pow((inputs[65][i].Value - EquEvaSRAvg.Value), 2);
                    }
                    if (inputs[65].Count != 0)
                    {
                        EquEvaSRStdev.Value = Math.Sqrt(valuesum / inputs[65].Count);
                        EquEvaSRStdev.Status = 0;
                    }
                    else
                    {
                        EquEvaSRStdev.Value =0;
                        EquEvaSRStdev.Status = 0;
                    }
                }
               
                //19、越H强度最大值
                if (calcuinfo.fsourtagflags[66] == false)
                {
                    //如果计算这个点的
                    EquEvaHT = null;                  
                }
                else if (categoryNullFlags[66] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越H强度的值！";
                    EquEvaHT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                   
                }
                else if (errorStatusInDevice[66] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越H强度状态位不正常！";
                    EquEvaHT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);                   
                }
                else
                {
                    EquEvaHT = inputs[66].First(n => n.Value == inputs[66].Max(m => m.Value));
                }
                //20、越HH强度最大值
                if (calcuinfo.fsourtagflags[67] == false)
                {
                    //如果计算这个点的
                    EquEvaHHT = null;
                }
                else if (categoryNullFlags[67] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越HH强度的值！";
                    EquEvaHHT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[67] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越HH强度状态位不正常！";
                    EquEvaHHT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaHHT = inputs[67].First(n => n.Value == inputs[67].Max(m => m.Value));
                }
                //21、越H力度最大值
                if (calcuinfo.fsourtagflags[68] == false)
                {
                    //如果计算这个点的
                    EquEvaHTF = null;
                }
                else if (categoryNullFlags[68] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越H力度的值！";
                    EquEvaHTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[68] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越H力度状态位不正常！";
                    EquEvaHTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaHTF = inputs[68].First(n => n.Value == inputs[68].Max(m => m.Value));
                }
                //22、越HH力度最大值
                if (calcuinfo.fsourtagflags[69] == false)
                {
                    //如果计算这个点的
                    EquEvaHHTF = null;
                }
                else if (categoryNullFlags[69] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越HH力度的值！";
                    EquEvaHHTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[69] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越HH力度状态位不正常！";
                    EquEvaHHTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaHHTF = inputs[69].First(n => n.Value == inputs[69].Max(m => m.Value));
                }
                //23、越L强度最大值
                if (calcuinfo.fsourtagflags[70] == false)
                {
                    //如果计算这个点的
                    EquEvaLT = null;
                }
                else if (categoryNullFlags[70] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越L强度的值！";
                    EquEvaLT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[70] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越L强度状态位不正常！";
                    EquEvaLT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaLT = inputs[70].First(n => n.Value == inputs[70].Max(m => m.Value));
                }
                //24、越LL强度最大值
                if (calcuinfo.fsourtagflags[71] == false)
                {
                    //如果计算这个点的
                    EquEvaLLT = null;
                }
                else if (categoryNullFlags[71] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越LL强度的值！";
                    EquEvaLLT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[71] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越LL强度状态位不正常！";
                    EquEvaLLT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaLLT = inputs[71].First(n => n.Value == inputs[71].Max(m => m.Value));
                }
                //25、越L力度最大值
                if (calcuinfo.fsourtagflags[72] == false)
                {
                    //如果计算这个点的
                    EquEvaLTF = null;
                }
                else if (categoryNullFlags[72] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越L力度的值！";
                    EquEvaLTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[72] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越L力度状态位不正常！";
                    EquEvaLTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaLTF = inputs[72].First(n => n.Value == inputs[72].Max(m => m.Value));
                }
                //26、越LL力度最大值
                if (calcuinfo.fsourtagflags[73] == false)
                {
                    //如果计算这个点的
                    EquEvaLLTF = null;
                }
                else if (categoryNullFlags[73] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有越LL力度的值！";
                    EquEvaLLTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[73] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点越LL力度状态位不正常！";
                    EquEvaLLTF = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquEvaLLTF = inputs[73].First(n => n.Value == inputs[73].Max(m => m.Value));
                }
                
                //组织计算结果
                results[0] = new List<PValue>();
                results[0].Add(EquHHN);                

                results[1] = new List<PValue>();
                results[1].Add(EquHHTMax);                

                results[2] = new List<PValue>();
                results[2].Add(EquHN);                

                results[3] = new List<PValue>();
                results[3].Add(EquHTMax);               

                results[4] = new List<PValue>();
                results[4].Add(EquLN);                

                results[5] = new List<PValue>();
                results[5].Add(EquLTMax);                

                results[6] = new List<PValue>();
                results[6].Add(EquLLN);                

                results[7] = new List<PValue>();
                results[7].Add(EquLLTMax);                

                results[8] = new List<PValue>();
                results[8].Add(EquRRTMax);               

                results[9] = new List<PValue>();
                results[9].Add(Equ0HTMax);               

                results[10] = new List<PValue>();
                results[10].Add(Equ0LTMax);                

                results[11] = new List<PValue>();
                results[11].Add(EquHLTMax);                

                results[12] = new List<PValue>();
                results[12].Add(EquLV1TMax);                

                results[13] = new List<PValue>();
                results[13].Add(EquGV2TMax);                

                results[14] = new List<PValue>();
                results[14].Add(EquEvaSRMin);

                results[15] = new List<PValue>();
                results[15].Add(EquEvaSRAvg);  

                results[16] = new List<PValue>();
                results[16].Add(EquEvaSRMax);

                results[17] = new List<PValue>();
                results[17].Add(EquEvaSRStdev);  

                results[18] = new List<PValue>();
                results[18].Add(EquEvaHHT);                

                results[19] = new List<PValue>();
                results[19].Add(EquEvaHHTF);                

                results[20] = new List<PValue>();
                results[20].Add(EquEvaHT);               

                results[21] = new List<PValue>();
                results[21].Add(EquEvaHTF);               

                results[22] = new List<PValue>();
                results[22].Add(EquEvaLT);                

                results[23] = new List<PValue>();
                results[23].Add(EquEvaLTF);               

                results[24] = new List<PValue>();
                results[24].Add(EquEvaLLT);               

                results[25] = new List<PValue>();
                results[25].Add(EquEvaLLTF);

                results[26] = new List<PValue>();
                results[26].Add(EquHHTubeN);

                results[27] = new List<PValue>();
                results[27].Add(EquHHTubeNR);

                results[28] = new List<PValue>();
                results[28].Add(EquHTotalTubeN);

                results[29] = new List<PValue>();
                results[29].Add(EquHTotalTubeNR);

                results[30] = new List<PValue>();
                results[30].Add(EquLTotalTubeN);

                results[31] = new List<PValue>();
                results[31].Add(EquLTotalTubeNR);

                results[32] = new List<PValue>();
                results[32].Add(EquLLTubeN);

                results[33] = new List<PValue>();
                results[33].Add(EquLLTubeNR);



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
        private static List<PValue> GetSpanMax(List<PValue>[] LimitSpanSeries, int[] focusIndex)
        {
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

            if (totalspan == null || totalspan.Count == 0) return null;
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
            List<PValue> results = new List<PValue>();
            results.Add(SpanMax);
            return results;
        }

        #endregion
    }
}
