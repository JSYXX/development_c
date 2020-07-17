using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// 壁温分析设备偏差超限统计算法-越限时长统计   
    /// ——该算法有两种思路，一种是用设备各点已经统计出的在各个超限区间中的span，来进行统计。有点是非常准确。缺点是中间要存储的数据量非常大，每个点要先算出span，数据量太大，算法太过于复杂。
    /// ——为了避免上面出现的问题：1、存储每一个点在各超限区间的span造成数据量大。2、计算每个点的超限时间段，算法上的复杂程度。该算法必须找到一个可以接受的化简算法。
    /// ——第二个思路，就是直接读取各个点每分钟的偏差值，将偏差值视为阶梯形来进行判断和计算。
    /// ——这样算法会简单很多，但算法比较粗糙。但是由于我们这里首先是通过设备点的总体状态来反应设备，点本身取得是50%、100%的一个大概值。因此，实际要求并不是特别的精确。
    ///     
    /// ——算法周期仅在小时和日周期上进行计算。
    /// 
    /// 小时周期上，MMTAEquDevORSpan直接用一个设备所有点每分钟的偏差来计算。
    /// ——如果一个设备有100个点，通过按设备方式读取偏差结果，一个小时是100*60*4=24000点。
    /// 
    /// 天周期上，直接用小时计算的结果相加
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
    public class MMTAEquDevORSpan : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquDevORSpan";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备超限统计算法-超限时长统计";
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
        private string _inputDescsCN = "设备点的分钟均值";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAEquDevORSpan";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
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
        private int _outputNumber = 52;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EquHHT1;" +               //1               
                                    "EquHHTR1;" +
                                    "EquHHT50;" +
                                    "EquHHTR50;" +
                                    "EquHHT100;" +
                                    "EquHHTR100;" +
                                    "EquHT1;" +                  //7
                                    "EquHTR1;" +                                                     
                                    "EquHT50;" +
                                    "EquHTR50;" +
                                    "EquHT100;" +
                                    "EquHTR100;" +              //12
                                    "EquLT1;" +                 //13
                                    "EquLTR1;" +                                                    
                                    "EquLT50;" +
                                    "EquLTR50;" +
                                    "EquLT100;" +
                                    "EquLTR100;" +              //18
                                    "EquLLT1;" +                //19
                                    "EquLLTR1;" +                                                     
                                    "EquLLT50;" +
                                    "EquLLTR50;" +
                                    "EquLLT100;" +
                                    "EquLLTR100;" +             //24
                                    "EquRRT1;" +                //25
                                    "EquRRTR1;" +                                                 
                                    "EquRRT50;" +
                                    "EquRRTR50;" +
                                    "EquRRT100;" +
                                    "EquRRTR100;" +
                                    "EquRRT100Max;"+            //31
                                    "EquRRTR100Max;"+           //32
                                    "EquHLT1;" +                //33
                                    "EquHLTR1;" +                                                   
                                    "EquHLT50;" +
                                    "EquHLTR50;" +
                                    "EquHLT100;" +
                                    "EquHLTR100;" +             //38
                                    "EquHHLLT1;" +              //39
                                    "EquHHLLTR1;" +                                                    
                                    "EquHHLLT50;" +
                                    "EquHHLLTR50;" +
                                    "EquHHLLT100;" +
                                    "EquHHLLTR100;" +           //44
                                    "EquPT50;"+                 //45
                                    "EquPTR50;"+
                                    "EquPT100;"+
                                    "EquPTR100;"+
                                    "EquNT50;" +
                                    "EquNTTR50;" +
                                    "EquNT100;" +
                                    "EquNTR100"                 //52
                                    ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "单点越HH时长;" +
                                        "单点越HH时长占比;" +
                                        "50%点越HH时长;" +
                                        "50%点越HH时长占比;" +
                                        "100%点越HH时长;" +
                                        "100%点越HH时长占比;" +

                                        "单点越H时长(包含越HH);" +
                                        "单点越H时长占比(包含越HH);" +
                                        "50%点越H时长(包含越HH);" +
                                        "50%点越H时长占比(包含越HH);" +
                                        "100%点越H时长(包含越HH);" +
                                        "100%点越H时长占比(包含越HH);" +

                                        "单点越L时长(包含越LL);" +
                                        "单点越L时长占比(包含越LL);" +
                                        "50%点越L时长(包含越LL);" +
                                        "50%点越L时长占比(包含越LL);" +
                                        "100%点越L时长(包含越LL);" +
                                        "100%点越L时长占比(包含越LL);" +

                                        "单点越LL时长;" +
                                        "单点越LL时长占比;" +
                                        "50%点越LL时长;" +
                                        "50%点越LL时长占比;" +
                                        "100%点越LL时长;" +
                                        "100%点越LL时长占比;" +

                                        "单点在-R到R时长;" +
                                        "单点在-R到R时长占比;" +
                                        "50%点在-R到R时长;" +
                                        "50%点在-R到R时长占比;" +
                                        "100%点在-R到R时长;" +
                                        "100%点在-R到R时长占比;" +
                                        "100%点在-R到R的最长时间及时刻;"+
                                        "100%点在-R到R的最长时间占比;" +

                                        "单点在H到L时长;" +
                                        "单点在H到L时长占比;" +
                                        "50%点在H到L时长;" +
                                        "50%点在H到L时长占比;" +
                                        "100%点在H到L时长;" +
                                        "100%点在H到L时长占比;" +

                                        "单点在HH到LL时长;" +
                                        "单点在HH到LL时长占比;" +
                                        "50%点在HH到LL时长;" +
                                        "50%点在HH到LL时长占比;" +
                                        "100%点在HH到LL时长;" +
                                        "100%点在HH到LL时长占比;" +

                                        "50%点为正的时间;"+
                                        "50%点为正的时间占比;"+
                                        "100%点为正的时间;"+
                                        "100%点为正的时间占比;"+

                                        "50%点为负的时间;"+
                                        "50%点为负的时间占比;"+
                                        "100%点为负的时间;"+
                                        "100%点为负的时间占比"
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
            List<PValue>[] results = new List<PValue>[52];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }          


            try
            {
                int MDeviation2DSpan = 4;   //本算法是直接读取MDeviation2DSpan算法所有计算结果。但是仅用分钟偏差值。

                //数据量分析
                //——MMTAAnaDevORSpan读取一个设备所有的分钟过滤值计算结果，只有一个值。一个设备100个温度点，那么也就100*60=6000个点。readmulti读取速度不会太慢。

                //0、输入：输入是同一设备上所有点的分钟过滤值计算结果，如该设备包含100个。List<PValue>[]是长度为100的数组，每个数组的List<Pvalue>长度为60.         
                
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.                
                if (inputs == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //——输入的是该设备上所有温度测点每分钟偏差计算的四个值。按小时计算时，一个标签61个值（包含截止时刻值）。按天计算时，一个标签包含1441个值（包含截止时刻值）。
                //——对于设备来说，在统计这些点时，实际上是无差别的，仅仅统计宏观的点数。因此无需考虑每个点是否为空，也不需要对此进行记录
                //——本计算仅使用偏差计算结果中的偏差值。
                List<PValue> inputall = new List<PValue>();         //将同一设备下所有点的偏差值，集中到一个list，并用状态位代表所属点序号
                            List<double> inputallvalue = new List<double>();        //测试，用于观察所有值
                for (i = 0; i < inputs.Length; i++)
                {
                    if (i % MDeviation2DSpan == 1)      //MDeviation2DSpan得四个计算结果期望值(多值);偏差(多值);偏差比(多值);得分(多值)，这里仅用偏差值
                    {
                        //每一个List<PValue> inputs[i]是一个点60分钟内，每分钟的偏差值，再加一个截止值
                        //如果inputs[i]为空，或者长度小于2（正常情况至少包含一个均值和一个截止值），则直接跳过。
                        if (inputs[i] != null && inputs[i].Count >= 2)
                        {                            
                            for (int j = 0; j < inputs[i].Count - 1; j++)   //本算法将点按照梯形对待，不取最后的截止时刻的值
                            {
                                if (inputs[i][j] != null && inputs[i][j].Status == 0)
                                {
                                    //将所有标签点的所有状态为0（正常值）值加入到inputall中，并将该值所属点的序号保存在状态位中（状态位不为0的值已经被过滤）                           
                                    inputall.Add(new PValue(inputs[i][j].Value, inputs[i][j].Timestamp, inputs[i][j].Endtime, i / MDeviation2DSpan));        
                                    inputallvalue.Add(inputs[i][j].Value);          //仅用于测试和观察
                                }
                            }
                        }
                    }

                }
                if(inputall.Count==0)
                {
                    _warningFlag = true;
                    _warningInfo = "当前时间段过滤非正常状态点后，没有有效数据！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //计算
                 
                //1.1、对输入参数paras进行解析，输入参数一共16个值，包括三高三低的设定值和三高三低的死区值。如“20,30,40,50,60,70,80,2,2,2,2,2,2”                
                double[] LimitRange = new double[7];  //界限数组，用于查找当前value值处于的值域段。界限数组包括LLL、LL、L、N、H、HH、HHH七个值，以及MinValue和MaxValue
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
                    return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //1.2、准备各设备点所属区域数组
                //——0区域代表MinValue<x<=LLL,  1区域代表LLL<x<=LL,  2区域代表LL<x<=L,  3区域代表L<x<=N,  4区域代表N<x<=H,5区域代表H<x<=HH, 6区域代表HH<x<HHH,  7区域代表HHH<x<=MaxValue
                int[] PointsLimitRange = new int[(int)(inputs.Length / MDeviation2DSpan)];            //用来存放每一个点当前坐在的区域
                for (i = 0; i < PointsLimitRange.Length; i++)
                {
                    PointsLimitRange[i] = -1;
                }                

                //1.3、对点进行排序，按时间顺序排列
                inputall = inputall.OrderBy(m => m.Timestamp).ToList();     //升序，时间从早到晚

                //2、循环判断，求出每一次变化后，各范围内的点的数量
                List<PValue> LimitSpanHHT = new List<PValue>();            //用来存放在HH区间包含点的数量变化情况
                List<PValue> LimitSpanHT = new List<PValue>();             //用来存放在H区间包含点的数量变化情况
                List<PValue> LimitSpanLT = new List<PValue>();             //用来存放在L区间包含点的数量变化情况
                List<PValue> LimitSpanLLT = new List<PValue>();            //用来存放在LL区间包含点的数量变化情况
                List<PValue> LimitSpanRRT = new List<PValue>();            //用来存放在RR区间包含点的数量变化情况
                List<PValue> LimitSpanHLT = new List<PValue>();            //用来存放在HL区间包含点的数量变化情况
                List<PValue> LimitSpanHHLLT = new List<PValue>();          //用来存放在HHLL区间包含点的数量变化情况
                List<PValue> LimitSpanPT = new List<PValue>();             //用来存放在L区间包含点的数量变化情况
                List<PValue> LimitSpanNT = new List<PValue>();             //用来存放在L区间包含点的数量变化情况

                DateTime lastDate = calcuinfo.fstarttime;                   //上一个点的时间
                DateTime currentDate = calcuinfo.fstarttime;                //当前点的时间
                double[] SortArray = new double[8];                         //当前数据点和7个限值点的排序
                int currentRange = -1;                                      //当前点的区域
                int currentPoint = -1;                                      //当前点是哪个标签点
                int totalpoint=0;                                           //每次一个新的点判断后，统计当前一共有多少个点位于当前新点所在区域

                for(i=0;i<inputall.Count ;i++)
                {
                    LimitRange.CopyTo(SortArray, 0);                                    //把界限数组考入  
                    SortArray[7] = inputall[i].Value;                                   //把当前点放入排序对象最后一个元素。
                    Array.Sort(SortArray);                                              //排序
                    currentRange = Array.IndexOf(SortArray, inputall[i].Value);         //当前点的值的所在的区域，这个相当于在每个区间（L,H]内找
                    currentPoint = (int)inputall[i].Status;                             //当前点是哪个标签的点

                    PointsLimitRange[currentPoint] = currentRange;                      //更新当前点当前所在区域 

                    totalpoint=PointsLimitRange.Count(n => n == currentRange);          //所有点，当前所在区域等于currentRange的总数量

                    LimitSpanHHT.Add(new PValue(PointsLimitRange.Count(n => n == 7), inputall[i].Timestamp, inputall[i].Timestamp, 0));                 //HH，是所有位于7区（0~7）的点
                    LimitSpanHT.Add(new PValue(PointsLimitRange.Count(n => (n == 7 || n == 6)), inputall[i].Timestamp, inputall[i].Timestamp, 0));      //H，是所有位于6区和7区（0~7）的点
                    LimitSpanLT.Add(new PValue(PointsLimitRange.Count(n => (n == 0 || n == 1)), inputall[i].Timestamp, inputall[i].Timestamp, 0));      //L,是所有位于0区和1区（0~7）的点
                    LimitSpanLLT.Add(new PValue(PointsLimitRange.Count(n => n == 0), inputall[i].Timestamp, inputall[i].Timestamp, 0));                 //LL,是所有位于0区（0~7）的点
                    LimitSpanRRT.Add(new PValue(PointsLimitRange.Count(n => (n == 3 || n == 4)), inputall[i].Timestamp, inputall[i].Timestamp, 0));     //RR,是所有位于3区和4区（0~7）的点
                    LimitSpanHLT.Add(new PValue(PointsLimitRange.Count(n => (n == 2 || n == 3 || n == 4 || n == 5)), inputall[i].Timestamp, inputall[i].Timestamp, 0));     //HL,是所有位于2、3区和4、5区（0~7）的点
                    LimitSpanHHLLT.Add(new PValue(PointsLimitRange.Count(n => (n == 1 || n == 2 || n == 3 || n == 4 || n == 5 || n == 6)), inputall[i].Timestamp, inputall[i].Timestamp, 0));     //HHLL,是所有位于1-6的点
                    LimitSpanPT.Add(new PValue(PointsLimitRange.Count(n => (n == 4 || n == 5 || n == 6 || n == 7)), inputall[i].Timestamp, inputall[i].Timestamp, 0));      //PT，所有正数的点
                    LimitSpanNT.Add(new PValue(PointsLimitRange.Count(n => (n == 0 || n == 1 || n == 2 || n == 3)), inputall[i].Timestamp, inputall[i].Timestamp, 0));      //NT，所有负数的点
                }
               
                //3、对计算完毕的LimitSpanXXX进行处理
                //——LimitSpanXXX的特点是，在每一个整分钟点，都有很多的重复值
                //——需要对这些重复的值进行合并。
                //HH               
                currentDate = LimitSpanHHT[LimitSpanHHT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanHHT[LimitSpanHHT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanHHT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanHHT[j].Timestamp == currentDate)
                    {
                        LimitSpanHHT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanHHT[j].Endtime = LimitSpanHHT[j + 1].Timestamp;
                        currentDate = LimitSpanHHT[j].Timestamp;
                    }
                }
                //H
                currentDate = LimitSpanHT[LimitSpanHT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanHT[LimitSpanHT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanHT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanHT[j].Timestamp == currentDate)
                    {
                        LimitSpanHT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanHT[j].Endtime = LimitSpanHT[j + 1].Timestamp;
                        currentDate = LimitSpanHT[j].Timestamp;
                    }
                }
                //L
                currentDate = LimitSpanLT[LimitSpanLT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanLT[LimitSpanLT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanLT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanLT[j].Timestamp == currentDate)
                    {
                        LimitSpanLT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanLT[j].Endtime = LimitSpanLT[j + 1].Timestamp;
                        currentDate = LimitSpanLT[j].Timestamp;
                    }
                }
                //LL
                currentDate = LimitSpanLLT[LimitSpanLLT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanLLT[LimitSpanLLT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanLLT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanLLT[j].Timestamp == currentDate)
                    {
                        LimitSpanLLT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanLLT[j].Endtime = LimitSpanLLT[j + 1].Timestamp;
                        currentDate = LimitSpanLLT[j].Timestamp;
                    }
                }
                //RR
                currentDate = LimitSpanRRT[LimitSpanRRT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanRRT[LimitSpanRRT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanRRT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanRRT[j].Timestamp == currentDate)
                    {
                        LimitSpanRRT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanRRT[j].Endtime = LimitSpanRRT[j + 1].Timestamp;
                        currentDate = LimitSpanRRT[j].Timestamp;
                    }
                }
                //HL
                currentDate = LimitSpanHLT[LimitSpanHLT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanHLT[LimitSpanHLT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanHLT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanHLT[j].Timestamp == currentDate)
                    {
                        LimitSpanHLT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanHLT[j].Endtime = LimitSpanHLT[j + 1].Timestamp;
                        currentDate = LimitSpanHLT[j].Timestamp;
                    }
                }
                //HHLL
                currentDate = LimitSpanHHLLT[LimitSpanHHLLT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanHHLLT[LimitSpanHHLLT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanHHLLT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanHHLLT[j].Timestamp == currentDate)
                    {
                        LimitSpanHHLLT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanHHLLT[j].Endtime = LimitSpanHHLLT[j + 1].Timestamp;
                        currentDate = LimitSpanHHLLT[j].Timestamp;
                    }
                }
                //PT
                currentDate = LimitSpanPT[LimitSpanPT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanPT[LimitSpanPT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanPT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanPT[j].Timestamp == currentDate)
                    {
                        LimitSpanPT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanPT[j].Endtime = LimitSpanPT[j + 1].Timestamp;
                        currentDate = LimitSpanPT[j].Timestamp;
                    }
                }
                //NT
                currentDate = LimitSpanNT[LimitSpanNT.Count - 1].Timestamp;           //数组最后一个数据的时间
                LimitSpanNT[LimitSpanNT.Count - 1].Endtime = calcuinfo.fendtime;      //给数组最后一个数据添加截止时刻
                for (int j = LimitSpanNT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanNT[j].Timestamp == currentDate)
                    {
                        LimitSpanNT.RemoveAt(j);
                    }
                    else
                    {
                        LimitSpanNT[j].Endtime = LimitSpanNT[j + 1].Timestamp;
                        currentDate = LimitSpanNT[j].Timestamp;
                    }
                }
                //4、分别计算每种情况的时长
                //定义计算结果
                double EquHHT1=0;
                double EquHHT50=0;
                double EquHHT100=0;

                double EquHT1=0;
                double EquHT50=0;
                double EquHT100=0;
                
                double EquLT1=0;
                double EquLT50=0;
                double EquLT100=0;

                double EquLLT1=0;
                double EquLLT50=0;
                double EquLLT100=0;

                double EquRRT1=0;
                double EquRRT50=0;
                double EquRRT100=0;
                PValue EquRRT100Max = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); 
               
                double EquHLT1=0;
                double EquHLT50=0;
                double EquHLT100=0;
                
                double EquHHLLT1=0;
                double EquHHLLT50=0;
                double EquHHLLT100=0;
                
                double EquPT50=0;
                double EquPT100=0;
                double EquNT50=0;
                double EquNT100=0;  

                double Percent1OfPoint = 1;
                double Percnet50OfPoint = (inputs.Length/ MDeviation2DSpan) * 0.5;
                double Percent100OfPoint = inputs.Length / MDeviation2DSpan;
               
                //越HH
                for (i = 0; i < LimitSpanHHT.Count; i++)
                {
                    if (LimitSpanHHT[i].Value >= Percent1OfPoint)
                        EquHHT1 += LimitSpanHHT[i].Timespan;
                    if (LimitSpanHHT[i].Value >= Percnet50OfPoint)
                        EquHHT50 += LimitSpanHHT[i].Timespan;
                    if (LimitSpanHHT[i].Value >= Percent100OfPoint)
                        EquHHT100 += LimitSpanHHT[i].Timespan;
                }
                //越H
                for (i = 0; i < LimitSpanHT.Count; i++)
                {
                    if (LimitSpanHT[i].Value >= Percent1OfPoint)
                        EquHT1 += LimitSpanHT[i].Timespan;
                    if (LimitSpanHT[i].Value >= Percnet50OfPoint)
                        EquHT50 += LimitSpanHT[i].Timespan;
                    if (LimitSpanHT[i].Value >= Percent100OfPoint)
                        EquHT100 += LimitSpanHT[i].Timespan;
                }
               
                
                //越L
                for (i = 0; i < LimitSpanLT.Count; i++)
                {
                    if (LimitSpanLT[i].Value >= Percent1OfPoint)
                        EquLT1 += LimitSpanLT[i].Timespan;
                    if (LimitSpanLT[i].Value >= Percnet50OfPoint)
                        EquLT50 += LimitSpanLT[i].Timespan;
                    if (LimitSpanLT[i].Value >= Percent100OfPoint)
                        EquLT100 += LimitSpanLT[i].Timespan;
                }
                //越LL
                for (i = 0; i < LimitSpanLLT.Count; i++)
                {
                    if (LimitSpanLLT[i].Value >= Percent1OfPoint)
                        EquLLT1 += LimitSpanLLT[i].Timespan;
                    if (LimitSpanLLT[i].Value >= Percnet50OfPoint)
                        EquLLT50 += LimitSpanLLT[i].Timespan;
                    if (LimitSpanLLT[i].Value >= Percent100OfPoint)
                        EquLLT100 += LimitSpanLLT[i].Timespan;
                }
               
                
                //-R到R
                for (i = 0; i < LimitSpanRRT.Count; i++)
                {
                    if (LimitSpanRRT[i].Value >= Percent1OfPoint)
                        EquRRT1 += LimitSpanRRT[i].Timespan;
                    if (LimitSpanRRT[i].Value >= Percnet50OfPoint)
                        EquRRT50 += LimitSpanRRT[i].Timespan;
                    if (LimitSpanRRT[i].Value >= Percent100OfPoint)
                        EquRRT100 += LimitSpanRRT[i].Timespan;
                }

                //全部点在-R到R的最长时间段
                //先把相同的值都合并
                        //测试用
                        /*
                        LimitSpanRRT[1].Value = Percent100OfPoint;
                        LimitSpanRRT[2].Value = Percent100OfPoint;
                        LimitSpanRRT[21].Value = Percent100OfPoint;
                        LimitSpanRRT[22].Value = Percent100OfPoint;
                        LimitSpanRRT[23].Value = Percent100OfPoint;
                        */
                double prevalue = LimitSpanRRT[LimitSpanRRT.Count - 1].Value;
                for (int j = LimitSpanRRT.Count - 2; j >= 0; j--)
                {
                    if (LimitSpanRRT[j].Value == prevalue)
                    {
                        LimitSpanRRT[j + 1].Timestamp = LimitSpanRRT[j].Timestamp;
                        LimitSpanRRT.RemoveAt(j);
                    }
                    else
                    {
                        prevalue = LimitSpanRRT[j].Value;
                    }
                }
                //找出所有点在rr的时间段                    
                List<PValue> totalspan = LimitSpanRRT.Where(p => p.Value == Percent100OfPoint).ToList();        //从LimitSpanRRT中找出所有value值等于总点数的时间段
                if (totalspan.Count>0)
                { 
                    EquRRT100Max = totalspan.First(n => n.Timespan == totalspan.Max(m => m.Timespan));              //从时间段中找出最大时间段。
                    EquRRT100Max.Value = EquRRT100Max.Timespan;
                }
                
                //H到L
                for (i = 0; i < LimitSpanHLT.Count; i++)
                {
                    if (LimitSpanHLT[i].Value >= Percent1OfPoint)
                        EquHLT1 += LimitSpanHLT[i].Timespan;
                    if (LimitSpanHLT[i].Value >= Percnet50OfPoint)
                        EquHLT50 += LimitSpanHLT[i].Timespan;
                    if (LimitSpanHLT[i].Value >= Percent100OfPoint)
                        EquHLT100 += LimitSpanHLT[i].Timespan;
                }             

                //HH到LL：就是2、3、4、5、6、7这几个区                
                for (i = 0; i < LimitSpanHHLLT.Count; i++)
                {
                    if (LimitSpanHHLLT[i].Value >= Percent1OfPoint)
                        EquHHLLT1 += LimitSpanHHLLT[i].Timespan;
                    if (LimitSpanHHLLT[i].Value >= Percnet50OfPoint)
                        EquHHLLT50 += LimitSpanHHLLT[i].Timespan;
                    if (LimitSpanHHLLT[i].Value >= Percent100OfPoint)
                        EquHHLLT100 += LimitSpanHHLLT[i].Timespan;
                }

                //PT                              
                for (i = 0; i < LimitSpanPT.Count; i++)
                {                    
                    if (LimitSpanPT[i].Value >= Percnet50OfPoint)
                        EquPT50 += LimitSpanPT[i].Timespan;
                    if (LimitSpanPT[i].Value >= Percent100OfPoint)
                        EquPT100 += LimitSpanPT[i].Timespan;
                }

                //NT                              
                for (i = 0; i < LimitSpanNT.Count; i++)
                {
                    if (LimitSpanNT[i].Value >= Percnet50OfPoint)
                        EquNT50 += LimitSpanNT[i].Timespan;
                    if (LimitSpanNT[i].Value >= Percent100OfPoint)
                        EquNT100 += LimitSpanNT[i].Timespan;
                }

                //将所有点加入一个大的数组然后按时间排队（时间可以有重复）
                

                //组织计算结果
                double calcuspan = calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalMilliseconds;

                results[0] = new List<PValue>();
                results[0].Add(new PValue(EquHHT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[1] = new List<PValue>();
                results[1].Add(new PValue(EquHHT1 *100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[2] = new List<PValue>();
                results[2].Add(new PValue(EquHHT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[3] = new List<PValue>();
                results[3].Add(new PValue(EquHHT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[4] = new List<PValue>();
                results[4].Add(new PValue(EquHHT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[5] = new List<PValue>();
                results[5].Add(new PValue(EquHHT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[6] = new List<PValue>();
                results[6].Add(new PValue(EquHT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[7] = new List<PValue>();
                results[7].Add(new PValue(EquHT1 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[8] = new List<PValue>();
                results[8].Add(new PValue(EquHT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[9] = new List<PValue>();
                results[9].Add(new PValue(EquHT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[10] = new List<PValue>();
                results[10].Add(new PValue(EquHT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[11] = new List<PValue>();
                results[11].Add(new PValue(EquHT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[12] = new List<PValue>();
                results[12].Add(new PValue(EquLT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[13] = new List<PValue>();
                results[13].Add(new PValue(EquLT1 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[14] = new List<PValue>();
                results[14].Add(new PValue(EquLT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[15] = new List<PValue>();
                results[15].Add(new PValue(EquLT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[16] = new List<PValue>();
                results[16].Add(new PValue(EquLT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[17] = new List<PValue>();
                results[17].Add(new PValue(EquLT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[18] = new List<PValue>();
                results[18].Add(new PValue(EquLLT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[19] = new List<PValue>();
                results[19].Add(new PValue(EquLLT1 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[20] = new List<PValue>();
                results[20].Add(new PValue(EquLLT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[21] = new List<PValue>();
                results[21].Add(new PValue(EquLLT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[22] = new List<PValue>();
                results[22].Add(new PValue(EquLLT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[23] = new List<PValue>();
                results[23].Add(new PValue(EquLLT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[24] = new List<PValue>();
                results[24].Add(new PValue(EquRRT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[25] = new List<PValue>();
                results[25].Add(new PValue(EquRRT1 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[26] = new List<PValue>();
                results[26].Add(new PValue(EquRRT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[27] = new List<PValue>();
                results[27].Add(new PValue(EquRRT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[28] = new List<PValue>();
                results[28].Add(new PValue(EquRRT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[29] = new List<PValue>();
                results[29].Add(new PValue(EquRRT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[30] = new List<PValue>();
                results[30].Add(EquRRT100Max);

                results[31] = new List<PValue>();
                results[31].Add(new PValue(EquRRT100Max.Value * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[32] = new List<PValue>();
                results[32].Add(new PValue(EquHLT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[33] = new List<PValue>();
                results[33].Add(new PValue(EquHLT1 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[34] = new List<PValue>();
                results[34].Add(new PValue(EquHLT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[35] = new List<PValue>();
                results[35].Add(new PValue(EquHLT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[36] = new List<PValue>();
                results[36].Add(new PValue(EquHLT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[37] = new List<PValue>();
                results[37].Add(new PValue(EquHLT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[38] = new List<PValue>();
                results[38].Add(new PValue(EquHHLLT1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[39] = new List<PValue>();
                results[39].Add(new PValue(EquHHLLT1 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[40] = new List<PValue>();
                results[40].Add(new PValue(EquHHLLT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[41] = new List<PValue>();
                results[41].Add(new PValue(EquHHLLT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[42] = new List<PValue>();
                results[42].Add(new PValue(EquHHLLT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[43] = new List<PValue>();
                results[43].Add(new PValue(EquHHLLT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[44] = new List<PValue>();
                results[44].Add(new PValue(EquPT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[45] = new List<PValue>();
                results[45].Add(new PValue(EquPT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[46] = new List<PValue>();
                results[46].Add(new PValue(EquPT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[47] = new List<PValue>();
                results[47].Add(new PValue(EquPT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[48] = new List<PValue>();
                results[48].Add(new PValue(EquNT50, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[49] = new List<PValue>();
                results[49].Add(new PValue(EquNT50 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[50] = new List<PValue>();
                results[50].Add(new PValue(EquNT100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[51] = new List<PValue>();
                results[51].Add(new PValue(EquNT100 * 100 / calcuspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));





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
