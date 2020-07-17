using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备温度基本统计算法（小时、天）
    /// ——计算一个设备在小时及天级别的基本统计指标
    /// ——采用设备上所有点的小时结果   
    /// 修改纪录
    ///     
    ///		2018.4.14 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.04.14</date>
    /// </author> 
    /// </summary>
    public class MMTAEquBase : BaseModule, IModule
    {
       
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquBase";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "壁温分析设备温度基本统计算法（小时、天）";
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
        private string _inputDescsCN = "温度点的小时基本统计值（数量不确定）";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAEquBase";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }

        private int _outputNumber = 24;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EquMin;" +               //1
                                      "EquAvg;" +
                                      "EquMax;" +                                      
                                      "EquStdev;" +
                                      "EquStdevMax;" +   
                                      "EquVolatility;" +   
                                      "EquVolDMax;"+
                                      "EquDMax;" +
                                      "EquDMaxR;" +
                                      "EquDN1Num;" +
                                      "EquDN2Num;" +
                                      "EquDN3Num;" +            //12
                                      "EquTNum;" +
                                      "EquVMax;" +
                                      "EquVMin;" +
                                      "EquVAvg;" +             //16                        
                                      "EquMinPN;" +                                      
                                      "EquMedian;" +
                                      "EquMedianPN;" +                                      
                                      "EquMaxPN;" +            //20                          
                                      "EquFar;" +
                                      "EquDiv;" +
                                      "EquDevPN;" +
                                      "EquDevPNR" 
                                   
                                    ;

        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "设备点最小值及时刻;" +      //1
                                        "设备点平均值;" +
                                        "设备点最大值及时刻;" +                                        
                                        "设备点标准差;" +
                                        "设备点标准差最大值;" +    
                                        "设备点波动量(强度);" +
                                        "设备点波动力度;" +
                                        "设备点单次极差;" +
                                        "设备点单次极差率;" +
                                        "设备点极差大于N1次数;" +
                                        "设备点极差大于N2次数(包含N1);" +
                                        "设备点极差大于N3次数(包含N2、N1);" +      //12
                                        "设备点翻转次数;" +
                                        "设备点最大速度绝对值;" +
                                        "设备点最小速度绝对值;" +
                                        "设备点平均速度（速度代数和求平均）;" +     //16
                                        "设备点最小值发生的点位置编号（标签名中）;" +
                                        "设备点中位数;" +
                                        "设备点距离中位数最近的点位置编号（标签名中）;" +                                      
                                        "设备点最大值发生的点位置编号（标签名中）;" +           //20
                                        "设备点极值点距中位数距离（点位置编号的差）;" +         //21,Far 
                                        "设备点高低差;" +                                     //22,Div
                                        "设备点高低差之间的距离（点位置编号的差）;" +           //23,PN
                                        "设备点高低差距"
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
        /// 计算模块算法实现:
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
            List<PValue>[] results = new List<PValue>[24];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //本算法是直接读取MMTAAnaBase算法对模拟量点的所有计算结果。MMTAAnaBase算法有19个计算结果。要靠次参数完成对计算结果的解析。
                int MMTAAnaBaseOutputNumber = 19;   
                string[] sourtagname = calcuinfo.sourcetagname.Split(';');          //所有计算结果标签。要靠标签找位置
                List<string> mintagnames = new List<string>();
                List<string> maxtagnames = new List<string>();
                List<string> avgtagnmaes = new List<string>();
                //0、输入：输入是同一设备上所有点偏差的MMTAAnaBase小时计算结果，或者MMTAAnaBase天计算结果。每一个计算结果一个值。                
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.                
                if (inputs == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //输入的是该设备上所有温度测点每分钟偏差值按小时的基本统计值。
                //如果计算周期是小时，每个计算结果有且仅有两个值，一个是小时计算值，一个是截止时刻值.
                //如果计算周期是天，每个计算结果有25个值。其中24个是小时计算值，一个是截止时刻值.
                List<PValue>[] categoryInputs = new List<PValue>[MMTAAnaBaseOutputNumber];      //MMTAAnaBase算法有19个计算结果,将inputs按计算结果类别分成19类各自存放
                bool[] categoryNullFlags = new bool[MMTAAnaBaseOutputNumber];                   //categoryFlags是19种计算结果，每种结果是否包含null的标志位  
                bool[] errorStatusInDevice = new bool[MMTAAnaBaseOutputNumber];                 //errorStatusInDevice是19种计算结果，每种结果是否包含null的标志位
                
                                        List<double>[] categoryInputsvalue =new List<double>[MMTAAnaBaseOutputNumber];                        //仅用于测试，为了便于观察各类的值
                for (i = 0; i < inputs.Length; i++)
                {                    
                    //按MMTAAnaBase算法所有计算结果的顺序，分类整理好所有点。同一类计算结果放在一起。同时过滤数据不足的点和状态位不对的点
                    //对于设备来说，某些单点状态位异常，直接过滤而不进行报警。只有这一类计算的结果全部异常才报警。
                    if (inputs[i] != null && inputs[i].Count >= 2)
                    {
                        //分配标签，从找出最大值标签、平均值标签、最小值标签
                        if (i % MMTAAnaBaseOutputNumber == 0)
                            mintagnames.Add(sourtagname[i]);
                        else if (i % MMTAAnaBaseOutputNumber == 1)
                            avgtagnmaes.Add(sourtagname[i]);
                        else if (i % MMTAAnaBaseOutputNumber == 2)
                            maxtagnames.Add(sourtagname[i]);
                        
                        //分配数据
                        if (categoryInputs[i % MMTAAnaBaseOutputNumber]==null) categoryInputs[i % MMTAAnaBaseOutputNumber] = new List<PValue>();
                        if (categoryInputsvalue[i % MMTAAnaBaseOutputNumber]==null) categoryInputsvalue[i % MMTAAnaBaseOutputNumber] = new List<double>();

                        //特别注意，计算周期为hour，取的是MMTAAnaBase周期为hour的计算值，计算周期为day，取的是MMTAAnaBase周期为day的计算值。因此inputs下正常情况，应该有两个值，其中第一个是有效值。
                        //但是要注意两个特别的点，一个是inputs[0]最小值点，一个是inputs[2]最大值点。
                        //这两个点，在并行计算分割后，因为并行计算自动分割特性，或产生两个或三个值。
                        //如果，该hour周期内的最大值点，恰好在hour起始时间，则有两个值点。第一个是有效值点。
                        //如果，该hour周期内的最大值点，在中间，则并发计算组织的数据，在该hour内有三个值点。其中第一个值点，是并发计算补进去的点，时刻从hour起始时间开始，到真正的最大值点截止。其值为上一个最大值。这个值是不对的。
                        if (i % MMTAAnaBaseOutputNumber == 0 || i % MMTAAnaBaseOutputNumber == 2)
                        {
                            int rightDataPoint = inputs[i].Count - 2;   //如果是最大值和最小值，取倒数第二个点是正确点
                            if (inputs[i][rightDataPoint].Status == 0)
                            {
                                categoryInputs[i % MMTAAnaBaseOutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAAnaBaseOutputNumber].Add(inputs[i][rightDataPoint].Value); 
                            }
                            else
                            {
                                categoryInputs[i % MMTAAnaBaseOutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAAnaBaseOutputNumber].Add(inputs[i][rightDataPoint].Value); 
                                errorStatusInDevice[i % MMTAAnaBaseOutputNumber] = true;       //状态位错误标志。只要有一个状态位不正确，就不计算
                            }
                        }
                        else
                        {
                            int rightDataPoint = 0;   //如果不是最大值和最小值点，直接取第一个点
                            if (inputs[i][rightDataPoint].Status == 0)
                            {
                                categoryInputs[i % MMTAAnaBaseOutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAAnaBaseOutputNumber].Add(inputs[i][rightDataPoint].Value);
                            }
                            else
                            {
                                categoryInputs[i % MMTAAnaBaseOutputNumber].Add(new PValue(inputs[i][rightDataPoint].Value, inputs[i][rightDataPoint].Timestamp, inputs[i][rightDataPoint].Endtime, inputs[i][rightDataPoint].Status));
                                categoryInputsvalue[i % MMTAAnaBaseOutputNumber].Add(inputs[i][rightDataPoint].Value);
                                errorStatusInDevice[i % MMTAAnaBaseOutputNumber] = true;       //状态位错误标志。只要有一个状态位不正确，就不计算
                            }
                        }


                    }
                    else
                    {
                        //20181108之前，假定MMTAAnaBase算法所有结果存在
                        /*
                        if (inputs[i] == null || inputs[i].Count < 2)
                            errorPointInDevice = true;        
                        */
                        //20181108之后，考虑MMTAAnaBase算法并不是所有计算结果都会保存
                        
                        //分配标签，从找出最大值标签、平均值标签、最小值标签
                        if (i % MMTAAnaBaseOutputNumber == 0)
                            mintagnames.Add(sourtagname[i]);
                        else if (i % MMTAAnaBaseOutputNumber == 1)
                            avgtagnmaes.Add(sourtagname[i]);
                        else if (i % MMTAAnaBaseOutputNumber == 2)
                            maxtagnames.Add(sourtagname[i]);

                        //分配数据
                        if (categoryInputs[i % MMTAAnaBaseOutputNumber] == null) categoryInputs[i % MMTAAnaBaseOutputNumber] = new List<PValue>();
                        if (categoryInputsvalue[i % MMTAAnaBaseOutputNumber] == null) categoryInputsvalue[i % MMTAAnaBaseOutputNumber] = new List<double>();

                        categoryInputs[i % MMTAAnaBaseOutputNumber].Add(null);
                        categoryNullFlags[i % MMTAAnaBaseOutputNumber] = true;      //如果某一个分类中加入了null，则该分类的null标记为true
                    }

                }

                //20181108之后，考虑MMTAAnaBase算法并不是所有计算结果都会保存,所以有可能19类中存在某一类中全是null的可能性
                /*
                //如果经过过滤有，19类中有任意一项为空，即不含任何数据，则报警，并直接返回。因此本计算有些计算结果需要19类结果中的多个计算值。所以要求19个计算结果至少都有正常值
                for (i = 0; i < MMTAAnaBaseOutputNumber; i++)
                {
                    if (categoryInputs[i] == null || categoryInputs[i].Count == 0)
                    {
                        _warningFlag = true;
                        _warningInfo = "当前时间段过滤非正常状态点后，有部分标签点，没有有效数据！";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                    }
                }
                //在19个结果都有正常值得前提下，如果有点没有数据，则报警，但继续计算
                if (errorPointInDevice == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段过滤非正常状态点后，有部分标签点，没有有效数据！";
                }
                */

               
                //定义计算结果
                PValue EquMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);            //最低,最低值时刻
                PValue EquAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);              //平均
                PValue EquMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); 	        //最高,最高值时刻                
                PValue EquStdevS = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);           //标准差
                PValue EquStdevMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);         //标准差最大值
                PValue EquVolatility = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); 	    //波动量(强度)
                PValue EquVolDMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); ;          //波动力度               
                PValue EquAVMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);            //最大代数速度
                PValue EquAVMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);            //最小代数速度
                PValue EquVavg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);             //平均速度
                PValue EquDMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);            //单次极差,单次极差时刻。注意这个极差的定义与统计中的极差不同。这个极差是指曲线在每两次翻转之间单向曲线段变化值的最大值。
                PValue EquDMaxR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);            //极差比
                PValue EquDN1Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);           //极差大于 N1 次数
                PValue EquDN2Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);           //极差大于 N2 次数
                PValue EquDN3Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);           //极差大于 N3 次数
                PValue EquTNum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);             //翻转次数
                PValue EquVMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);             //实际上在第一个周期内，赋初始值
                PValue EquVMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);             //实际上在第一个周期内，赋初始值
                PValue EquVAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);                //平均速度。初始值为0。
                PValue EquMinPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquMedian = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquMedianPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquMaxPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquFar = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquDev = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquDevPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue EquDevPNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);



                double valuesum=0;
                double valueagv = 0;
                double[] valueall = new double[avgtagnmaes.Count];

                //**************特别注意,20181108在调试该算法时，并发计算时出现算法内部inputs未知原因的混乱**************************
                //*******在计算组件中不能使用这样的语句。在并发计算时，不能再内部改变函数的全局输入变量inputs
                //*******inputs = categoryInputs;会导致并发计算中，算法内部inputs出现混乱
                //inputs = categoryInputs;
                //*****************************************************************************************
                //0、最小值，从最小中找最小                
                if (calcuinfo.fsourtagflags[0] == false)
                {
                    //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                    EquMin = null;
                    EquMinPN = null;
                }
                else if (categoryNullFlags[0] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有最小值！";
                    EquMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                    EquMinPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                }
                else if (errorStatusInDevice[0] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点最小值状态位不正常！";
                    EquMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                    EquMinPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                }
                else
                {
                    EquMin = categoryInputs[0][0];
                    for (i = 0; i < categoryInputs[0].Count; i++)
                    {
                        if (categoryInputs[0][i].Value < EquMin.Value)
                        {
                            EquMin.Value = categoryInputs[0][i].Value;
                            EquMin.Timestamp = categoryInputs[0][i].Timestamp;
                            EquMin.Endtime = categoryInputs[0][i].Endtime;
                            //16、最小值对应的管号
                            EquMinPN = new PValue(double.Parse(mintagnames[i].Substring(7, 2)), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                        }
                    }
                }
                
                //1、平均，各点均值得平均
                if (calcuinfo.fsourtagflags[1] == false)
                {
                    //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                    EquAvg = null;                   
                }
                else if (categoryNullFlags[1] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有平均值！";
                    EquAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                    
                }
                else if (errorStatusInDevice[1] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点平均值状态位不正常！";
                    EquAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);                    
                }
                else
                {
                    valuesum = 0;
                    for (i = 0; i < categoryInputs[1].Count; i++)
                    {
                        valuesum += categoryInputs[1][i].Value;
                        valueall[i] = categoryInputs[1][i].Value;
                    }
                    EquAvg = new PValue(valuesum / categoryInputs[1].Count, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                }

                //2、最大值，从最大中找最大
                if (calcuinfo.fsourtagflags[2] == false)
                {
                    //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                    EquMax = null;
                    EquMaxPN = null;
                }
                else if (categoryNullFlags[2] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点没有最大值！";
                    EquMax=new PValue(0,calcuinfo.fstarttime,calcuinfo.fendtime,(long)StatusConst.NullInDevice);
                    EquMaxPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                   
                }
                else if (errorStatusInDevice[2] == true)
                {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点最大值状态位不正常！";                   
                    EquMax=new PValue(0,calcuinfo.fstarttime,calcuinfo.fendtime,(long)StatusConst.ErrorStatusInDevice);
                    EquMaxPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);      
                }
                else
                {
                    EquMax = categoryInputs[2][0];
                    for (i = 0; i < categoryInputs[2].Count; i++)
                    {
                        if (categoryInputs[2][i].Value > EquMax.Value)
                        {
                            EquMax.Value = categoryInputs[2][i].Value;
                            EquMax.Timestamp = categoryInputs[2][i].Timestamp;
                            EquMax.Endtime = categoryInputs[2][i].Endtime;
                            //19、
                            EquMaxPN =new PValue( double.Parse(maxtagnames[i].Substring(7, 2)), calcuinfo.fstarttime, calcuinfo.fendtime,0); 
                        }

                    }
                }
                Console.WriteLine("标志长度：" + calcuinfo.fsourtagflags.Count());
               //3、均差:经讨论，用每个点均差来求均差
               //4、均差最大值，用每个点的均差来找最大的一个
                if (calcuinfo.fsourtagflags[4] == false)
                {
                    //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                    EquStdevS = null;
                    EquStdevMax = null;
                }
                else if (categoryNullFlags[4] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有均差值！";
                   EquStdevS = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                   EquStdevMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);   
               }
               else if (errorStatusInDevice[4] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点均差值状态位不正常！";
                   EquStdevS = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                   EquStdevMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   valuesum = 0;
                   for (i = 0; i < categoryInputs[4].Count; i++)
                   {
                       valuesum += categoryInputs[4][i].Value;
                   }
                   valueagv = valuesum / categoryInputs[4].Count;
                   valuesum = 0;
                   for (i = 0; i < categoryInputs[4].Count; i++)
                   {
                       valuesum += Math.Pow((categoryInputs[4][i].Value - valueagv), 2);
                   }
                   EquStdevS = new PValue(Math.Sqrt(valuesum / categoryInputs[4].Count), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                   EquStdevMax = categoryInputs[4].First(n => n.Value == categoryInputs[4].Max(m => m.Value));
               }
                
                
                
               //5、波动，每个点波动相加求均值。
               if (calcuinfo.fsourtagflags[5] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquVolatility = null;                   
               }
               else if (categoryNullFlags[5] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有波动值！";
                   EquVolatility = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                      
               }
               else if (errorStatusInDevice[5] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点波动值状态位不正常！";
                   EquVolatility = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);                    
               }
               else
               {
                   valuesum = 0;
                   for (i = 0; i < categoryInputs[5].Count; i++)
                   {
                       valuesum += categoryInputs[5][i].Value;
                   }
                   EquVolatility = new PValue(valuesum / categoryInputs[5].Count, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
               }
               Console.WriteLine("标志长度：" + calcuinfo.fsourtagflags.Count());
               //6、波动力度
               if (calcuinfo.fsourtagflags[6] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquVolDMax = null;
               }
               else if (categoryNullFlags[6] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有波动力度值！";
                   EquVolDMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                      
               }
               else if (errorStatusInDevice[6] == true)
               {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点波动力度值状态位不正常！";
                    EquVolDMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquVolDMax = categoryInputs[6].First(n => n.Value == categoryInputs[6].Max(m => m.Value));
               }
               //7、单次极差：从点的极差中找最大
               //8、单次极差率
               if (calcuinfo.fsourtagflags[7] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquDMax = null;
                   EquDMaxR = null;
               }
               else if (categoryNullFlags[7] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有极差度值！";
                   EquDMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
                   EquDMaxR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);   
               }
               else if (errorStatusInDevice[7] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点极差值状态位不正常！";
                   EquDMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
                   EquDMaxR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquDMax = categoryInputs[7].First(n => n.Value == categoryInputs[7].Max(m => m.Value));
                   if (EquMax.Value != EquMin.Value)
                        EquDMaxR = new PValue(EquDMax.Value * 100 / (EquMax.Value - EquMin.Value), calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                   else
                       EquDMaxR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
               }
                
                

               //9、N1次数，取单点最大值
               if (calcuinfo.fsourtagflags[9] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquDN1Num = null;                   
               }
               else if (categoryNullFlags[9] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有N1值！";
                   EquDN1Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                    
               }
               else if (errorStatusInDevice[9] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点N1值状态位不正常！";
                   EquDN1Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);                    
               }
               else
               {
                   EquDN1Num = categoryInputs[9].First(n => n.Value == categoryInputs[9].Max(m => m.Value));
               }
               //10、N2次数
               if (calcuinfo.fsourtagflags[10] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquDN2Num = null;
               }
               else if (categoryNullFlags[10] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有N2值！";
                   EquDN2Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                    
               }
               else if (errorStatusInDevice[10] == true)
               {
                    _warningFlag = true;
                    _warningInfo += "当前时间段设备内有部分点N2值状态位不正常！";
                    EquDN2Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquDN2Num = categoryInputs[10].First(n => n.Value == categoryInputs[10].Max(m => m.Value));
               }
               //11、N3次数
               if (calcuinfo.fsourtagflags[11] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquDN3Num = null;
               }
               else if (categoryNullFlags[11] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有N3值！";
                   EquDN3Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                    
               }
               else if (errorStatusInDevice[11] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点N3值状态位不正常！";
                   EquDN3Num = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquDN3Num = categoryInputs[11].First(n => n.Value == categoryInputs[11].Max(m => m.Value));
               }
               //12、翻转次数
               if (calcuinfo.fsourtagflags[12] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquTNum = null;
               }
               else if (categoryNullFlags[12] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有翻转次数值！";
                   EquTNum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);                    
               }
               else if (errorStatusInDevice[12] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点翻转次数值状态位不正常！";
                   EquTNum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquTNum = categoryInputs[12].First(n => n.Value == categoryInputs[12].Max(m => m.Value));
               }
               //13、V最大：单点最大中找最大
               if (calcuinfo.fsourtagflags[13] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquVMax = null;
               }
               else if (categoryNullFlags[13] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有V最大数值！";
                   EquVMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
               }
               else if (errorStatusInDevice[13] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点V最大值状态位不正常！";
                   EquVMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquVMax = categoryInputs[13].First(n => n.Value == categoryInputs[13].Max(m => m.Value));
               }
               //14、V最小：单点最小中找最小
               if (calcuinfo.fsourtagflags[14] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquVMin = null;
               }
               else if (categoryNullFlags[14] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有V最小数值！";
                   EquVMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
               }
               else if (errorStatusInDevice[14] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点V最小值状态位不正常！";
                   EquVMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   EquVMin = categoryInputs[14].First(n => n.Value == categoryInputs[14].Min(m => m.Value));
               }
               //15、V平均，个点速度均值
               if (calcuinfo.fsourtagflags[15] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquVAvg = null;
               }
               else if (categoryNullFlags[15] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有V平均数值！";
                   EquVAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
               }
               else if (errorStatusInDevice[15] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点V平均值状态位不正常！";
                   EquVAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   valuesum = 0;
                   for (i = 0; i < categoryInputs[15].Count; i++)
                   {
                       valuesum += categoryInputs[15][i].Value;
                   }
                   EquVAvg = new PValue(valuesum / categoryInputs[15].Count, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
               }
                
               //17中位数：从设备所有点的小时偏差均值中找中位值。               
               if (calcuinfo.fsourtagflags[1] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquMedian = null;
               }
               else if (categoryNullFlags[1] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有平均数值！";
                   EquMedian = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
               }
               else if (errorStatusInDevice[1] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点平均值状态位不正常！";
                   EquMedian = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   Array.Sort(valueall);
                   int len = valueall.Length;
                   if (len % 2 == 0)
                   {
                       double douEquMedian = (valueall[len / 2 - 1] + valueall[len / 2]) / 2;       //比如，一共6个元素，如果下标从0开始，就是[2]+[3]
                       EquMedian = new PValue(douEquMedian, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                   }
                   else
                   {
                       double douEquMedian = valueall[(int)Math.Floor(len / 2.0)];                                 //比如，一共7个元素，就是第4个元素
                       EquMedian = new PValue(douEquMedian, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                   }
               }
               //18、中位数点号
               if (calcuinfo.fsourtagflags[1] == false)
               {
                   //如果计算这个点的源标签本身就没有存，则本计算也不进行。
                   EquMedianPN = null;
               }
               else if (categoryNullFlags[1] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点没有平均数值！";
                   EquMedianPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.NullInDevice);
               }
               else if (errorStatusInDevice[1] == true)
               {
                   _warningFlag = true;
                   _warningInfo += "当前时间段设备内有部分点平均值状态位不正常！";
                   EquMedianPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               else
               {
                   double minRangeMedian = Math.Abs(categoryInputs[1][0].Value - EquMedian.Value);    //距中位数距离最小值，用均值第一个值进行初始化
                   EquMedianPN.Value = double.Parse(avgtagnmaes[0].Substring(7, 2));
                   for (i = 0; i < categoryInputs[1].Count; i++)
                   {
                       if (categoryInputs[1][i].Value - EquMedian.Value < minRangeMedian)
                           EquMedianPN.Value = double.Parse(avgtagnmaes[i].Substring(7, 2));
                   }
               }
               //20、极值点距中位值点距离
               if (EquMaxPN.Status == 0 && EquMedianPN.Status == 0 && EquMinPN.Status == 0)
               {
                   double upper = Math.Abs(EquMaxPN.Value - EquMedianPN.Value);
                   double lower = Math.Abs(EquMedianPN.Value - EquMinPN.Value);
                   EquFar.Value = upper > lower ? upper : lower;
               }
               else
               {
                   EquFar = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }

               //21、高低差，最大值减最小值
               if (EquMax.Status == 0 && EquMin.Status == 0)
               {
                   EquDev.Value = EquMax.Value - EquMin.Value;
               }
               else
               {
                   EquDev = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               //22、高低点距离
               if (EquMaxPN.Status == 0 && EquMinPN.Status == 0)
               {
                   EquDevPN.Value = Math.Abs(EquMaxPN.Value - EquMinPN.Value);
               }
               else
               {
                   EquDevPN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.ErrorStatusInDevice);
               }
               //23、高低点差距
               if (EquDev.Status == 0)
               {
                   if (EquDevPN.Value != 0)
                       EquDevPNR.Value = EquDev.Value / EquDevPN.Value;
                   else
                       EquDevPNR.Value = 0;
               }
               else
               {
                   EquDevPNR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, EquDev.Status);
               }
               
                //

                //组织计算结果
                /*
                "EquMin;" +              //1
                "EquAvg;" +
                "EquMax;" +                                      
                "EquStdev;" +
                "EquStdevMax;" +   
                "EquVolatility;" +       //6   
                "EquVolDMax;"+
                "EquDMax;" +
                "EquDMaxR;" +
                "EquDN1Num;" +
                "EquDN2Num;" +           //11
                "EquDN3Num;" +
                "EquTNum;" +
                "EquVMax;" +
                "EquVMin;" +
                "EquVAvg;" +             //16                        
                "EquMinPN;" +                                      
                "EquMedian;" +
                "EquMedianPN;" +                                      
                "EquMaxPN;" +                                    
                "EquFar;" +              //21
                "EquDiv;" +
                "EquDevPN;" +
                "EquDevPNR" 
                 */
                
                results[0] = new List<PValue>();
                results[0].Add(EquMin);

                results[1] = new List<PValue>();
                results[1].Add(EquAvg);

                results[2] = new List<PValue>();
                results[2].Add(EquMax);

                results[3] = new List<PValue>();
                results[3].Add(EquStdevS);


                results[4] = new List<PValue>();
                results[4].Add(EquStdevMax);

                results[5] = new List<PValue>();
                results[5].Add(EquVolatility);

                results[6] = new List<PValue>();
                results[6].Add(EquVolDMax);

                results[7] = new List<PValue>();
                results[7].Add(EquDMax);

                results[8] = new List<PValue>();
                results[8].Add(EquDMaxR);

                results[9] = new List<PValue>();
                results[9].Add(EquDN1Num);

                results[10] = new List<PValue>();
                results[10].Add(EquDN2Num);

                results[11] = new List<PValue>();
                results[11].Add(EquDN3Num);

                results[12] = new List<PValue>();
                results[12].Add(EquTNum);

                results[13] = new List<PValue>();
                results[13].Add(EquVMax);

                results[14] = new List<PValue>();
                results[14].Add(EquVMin);

                results[15] = new List<PValue>();
                results[15].Add(EquVAvg);

                results[16] = new List<PValue>();
                results[16].Add(EquMinPN);

                results[17] = new List<PValue>();
                results[17].Add(EquMedian);

                results[18] = new List<PValue>();
                results[18].Add(EquMedianPN);

                results[19] = new List<PValue>();
                results[19].Add(EquMaxPN);

                results[20] = new List<PValue>();
                results[20].Add(EquFar);

                results[21] = new List<PValue>();
                results[21].Add(EquDev);

                results[22] = new List<PValue>();
                results[22].Add(EquDevPN);

                results[23] = new List<PValue>();
                results[23].Add(EquDevPNR);


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
    }
}
