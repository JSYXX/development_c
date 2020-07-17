using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 超限统计算法   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。本算法按线性处理输入数据，截止时刻数据需要被算法使用
    ///		2017.03.21 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2017.03.21</date>
    /// </author> 
    /// </summary>
    public class MLimitStatistics : BaseModule, IModule, IModuleExPara
    {
        
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MLimitStatistics";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "三高三低超限统计";
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
        private string _algorithms = "MLimitStatistics";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYNNNNNNNNNN";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "20;30;40;60;70;80;2;2;2;2;2;2;10";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "LLL限值;LL限值;L限值;H限值;HH限值;HHH限值;LLL死区;LL死区;L死区;H死区;HH死区;HHH死区;最小时间段阈值(分钟);——最小时间段阈值可选。不填写则默认取0分钟。填写时前面加分号与前面的参数分隔。";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){11}(;[+-]?\d+(\.\d+)?){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 28;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs =   "LLLNumber;LLLSpan;LLLArea;"+
                                        "LLNumber;LLSpan;LLArea;"+
                                        "LNumber;LSpan;LArea;"+
                                        "HNumber;Hspan;HArea;"+
                                        "HHNumber;HHSpan;HHArea;"+
                                        "HHHNumber;HHHSpan;HHHArea;"+
                                        "LLAreaRatio;LAreaRatio;HAreaRatio;HHAreaRatio;"+
                                        "LLLSeries;LLSeries;LSeries;HSeries;HHLSeries;HHHSeries"
                                        ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "超LLL次数;超LLL时长;超LLL面积;" +
                                        "超LL次数;超LL时长;超LL面积;" +
                                        "超L次数;超L时长;超L面积;" +
                                        "超H次数;超H时长;超H面积;" +
                                        "超HH次数;超HH时长;超HH面积;" +
                                        "超HHH次数r;超HHH时长;超HHH面积;" +
                                        "超LL面积比率;超L面积比率;超H面积比率;超HH面积比率"+
                                        "超LLL时间序列;超LL时间序列;超L时间序列;超H时间序列;超HH时间序列;超HHH时间序列"
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

            List<PValue>[] results = new List<PValue>[28];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuInfo.fstarttime, calcuinfo.fendtime, 0));
            }
            results[22] = null;   //LLLSeries
            results[23] = null;   //LLSeries
            results[24] = null;   //LSeries
            results[25] = null;   //HSeries
            results[26] = null;   //HHSeries
            results[27] = null;   //HHHSeries

            try
            {                

                //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，则0-21项应当有输出，所有的输出项为0.23-27项应当为空
                

                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，所有的输出项为0.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); 
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法采用线性计算，截止时刻点要参与计算。不删除
                //if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = input.Count - 1; i >= 0; i--)
                {
                    input.RemoveAt(i);
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 2) return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);   

                //1、准备界限数组和死区值
                double[] LimitRange = new double[6];  //界限数组，用于查找当前value值处于的值域段。界限数组包括八个值LLL、LL、L、H、HH、HHH六个值，MinValue和MaxValue
                double LLLDeadBand, LLDeadBand, LDeadBand, HDeadBand, HHDeadBand, HHHDeadBand;
                LLLDeadBand = 0; LLDeadBand = 0; LDeadBand = 0; HDeadBand = 0; HHDeadBand = 0; HHHDeadBand = 0;
                double threshold ;
                
                //1.1、对输入参数paras进行解析，输入参数一共12个值，包括三高三低的设定值和三高三低的死区值。如“20,50,60,80,85,90,2,2,2,2,2,2”
                string[] paras = calcuinfo.fparas.Split(',');
                for (i = 0; i <6; i++) 
                {
                    LimitRange[i] = float.Parse(paras[i]);
                }           
               
                //对LimitRange进行排序，保证正确性。
                Array.Sort(LimitRange);

                LLLDeadBand = float.Parse(paras[6]);
                LLDeadBand = float.Parse(paras[7]);
                LDeadBand = float.Parse(paras[8]);
                HDeadBand = float.Parse(paras[9]);
                HHDeadBand = float.Parse(paras[10]);
                HHHDeadBand = float.Parse(paras[11]);

                if (paras.Length == 13 && paras[12] != "")
                {
                    threshold = float.Parse(paras[12]);
                }
                else
                {
                    threshold = 0;
                }
            
                //1.1 对各限值进行死区调整
                //超限统计，根据三低限值和三高限值，把值域分成7个限值区域，
                //——0区域代表MinValue<x<=LLL, 1区域代表LLL<x<=LL, 2区域代表LL<x<=L,   3区域代表L<x<=H,    4区域代表H<x<=HH,   5区域代表HH<x<HHH,  6区域代表HHH<x<=MaxValue
                //但是，由于有死区的设定，点进入不同区域时，对应的限值数组需要做调整.x区域的点对应LimitRangeDeadBand[x]。
                //位于不同限值区域的点，要使用不同的限值数组进行下一个点的限值区域判定。
                double[][] LimitRangeDeadBand = new double[7][];
                for (i = 0; i < 7; i++)                         //先用LimitRange去初始化LimitRangeDeadBand
                {
                    LimitRangeDeadBand[i] = new double[6];
                    for (int j = 0; j < 3; j++)
                    {   //在限值上不算越限。由于本算法采用的是排序法，找到点所在的区域。
                        //——当点位于H、HH、HHH的位置时，计算得到的点的区域刚好是想要的结果，即不算越限。
                        //——当点位于L、LL、LLL的位置时，计算得到的点的区域会被认为是算为越界。这不是想要的结果。为了使点在L、LL、LLL位置时不被认为是越界，需要给L、LL、LLL减掉一个极小值。这样在L、LL、LLL边界上的点会返回正确的结果
                        LimitRangeDeadBand[i][j] = LimitRange[j]-0.00000001;
                    }
                    for (int j =3; j < 6; j++)
                    {
                        LimitRangeDeadBand[i][j] = LimitRange[j];
                    }
                }
                //当实时值位于LLL区时，当前区域值为currentRange=0，在LLL区内，返回LL时，要求实时值要到LLL+LLLDeadBand之上，才返回有效。返回L时要求实时值到LL+LLDeadBand之上才有效，返回Normal时，要求返回L+LDeadBand才有效。
                //此区域区域值为0，对应的调整死区界限数组为 LimitRangeDeadBand[0]，要对LLL、LL、L值做调整          
                LimitRangeDeadBand[0][0] = LimitRangeDeadBand[0][0] + LLLDeadBand;
                LimitRangeDeadBand[0][1] = LimitRangeDeadBand[0][1] + LLDeadBand;
                LimitRangeDeadBand[0][2] = LimitRangeDeadBand[0][2] + LDeadBand;
                //当实时值位于LL区时，当前区域值为currentRange=1，在LL区内，返回L时要求实时值到LL+LLDeadBand之上才有效，返回Normal时，要求返回L+LDeadBand才有效。
                //此区域currentRange=1，对应的限值数组 LimitRangeDeadBand[1]，要对LL、L值做调整            
                LimitRangeDeadBand[1][1] = LimitRangeDeadBand[1][1] + LLDeadBand;
                LimitRangeDeadBand[1][2] = LimitRangeDeadBand[1][2] + LDeadBand;
                //当实时值位于L区时，当前区域值为currentRange=2，在L区内，返回Normal时，要求返回L+LDeadBand才有效。
                //此区域currentRange=2，对应的限值数组 LimitRangeDeadBand[2]，要对L值做调整            
                LimitRangeDeadBand[2][2] = LimitRangeDeadBand[2][2] + LDeadBand;
                //当实时值位于正常区间时，当前区域值为currentRange=3，所有区域均不调整
                
                //当实时值位于H区时，当前区域值为currentRange=4，在H区内，返回Normal时，要求返回H-HDeadBand才有效。            
                LimitRangeDeadBand[4][3] = LimitRangeDeadBand[4][3] - HDeadBand;
                //当实时值位于HH区时，当前区域值为currentRange=5，在HH区内，返回H时，要求返回HH-HHDeadBand才有效。返回Normal时，要求返回H-HDeadBand才有效。           
                LimitRangeDeadBand[5][3] = LimitRangeDeadBand[5][3] - HDeadBand;
                LimitRangeDeadBand[5][4] = LimitRangeDeadBand[5][4] - HHDeadBand;
                //当实时值位于HHH区时，当前区域值为currentRange=6，在HHH区内，返回HH时，要求返回HHH-HHHDeadBand才有效。返回H时，要求返回HH-HHDeadBand才有效。返回Normal时，要求返回H-HDeadBand才有效。            
                LimitRangeDeadBand[6][3] = LimitRangeDeadBand[6][3] - HHHDeadBand;
                LimitRangeDeadBand[6][4] = LimitRangeDeadBand[6][4] - HHHDeadBand;
                LimitRangeDeadBand[6][5] = LimitRangeDeadBand[6][5] - HHHDeadBand;


                //2、准备存储变量
                double[] LimitNumber =new double[7];        //各限值区域的次数统计
                double[] LimitSpan = new double[7];         //各限值区域的时长统计            
                double[] LimitArea = new double[7];         //各限值区域的面积统计           
                double[] LimitAreaRatio = new double[7];   	//各限值区域的面积统计
                List<PValue>[] LimitSpanSeries = new List<PValue>[7];//各限值区域的时间序列统计
                for (i = 0; i < 7; i++) 
                {
                    LimitSpanSeries[i] = new List<PValue>();
                }
                
                //3、对数据进行遍历
                int currentRange = 10;                      //当前点的限值区域
                int previousRange = 10;                     //上一个点的限值区域
                PValue currentPoint;                        //当前点Pvalue值                
                PValue previousPoint;                       //上一点PValue值
                PValue[] crossPoint=new PValue[6];          //与限值的焦点
                PValue[] transPoint;                        //currentPoint、previousPoint及限值焦点构成的直线
                double[] SortArray = new double[7];
                double currentSpan = 0;
                double currentArea = 0;
                
                
                for (int iPoint = 0; iPoint < input.Count; iPoint++)
                {
                    //3.1、判断点的位置是否有变化
                    //3.1.1、计算当前点的所在的限值区域
                    if (iPoint == 0)
                    {   
                        //计算第一个点的当前区域。当previousStatus == 10时，是第一个点
                        //计算第一个点的当前区域，采用正常值区域3对应的界限数组来判断第一个点的区域。这样规定算法，是有微小误差的。
                        //误差可能：如果统计周期前面的点，是从[H,HH]落入到[H,H-HDeadBand]内，实际的统计可能会有误差。这个需要在仔细分析
                        LimitRangeDeadBand[3].CopyTo(SortArray, 0);                         //如果是第一个点   
                        SortArray[6] = input[iPoint].Value;
                        Array.Sort(SortArray);
                        currentRange = Array.IndexOf(SortArray, input[iPoint].Value);       //找第一个点的所在的区域，这个相当于在每个区间（L,H]内找
                        previousRange = currentRange;                                       //假定第一个点的前一时刻点的previousRange与currentRange相同
                        currentPoint = input[iPoint];                                       //获取当前点
                        previousPoint = input[iPoint];                                      //获取上一个点
                        //results[currentRange].Add(new PValue(1, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));  
                        LimitSpanSeries[currentRange].Add(new PValue(1, input[iPoint].Timestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                    }
                    else
                    {
                        //不是第一个点
                        LimitRangeDeadBand[previousRange].CopyTo(SortArray, 0);             
                        SortArray[6] = input[iPoint].Value;
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
                        currentSpan = (currentPoint.Timestamp - previousPoint.Timestamp).TotalMilliseconds/1000;
                        if (currentRange > 3)
                        {
                            currentArea = (Math.Abs(currentPoint.Value - LimitRange[currentRange - 1]) + Math.Abs(previousPoint.Value - LimitRange[currentRange - 1])) * currentSpan / 2;
                        }
                        else if (currentRange < 3)
                        {
                            currentArea = (Math.Abs(currentPoint.Value - LimitRange[currentRange]) + Math.Abs(previousPoint.Value - LimitRange[currentRange])) * currentSpan / 2;
                        }
                        else currentArea = 0;
                        //累计时长
                        LimitSpan[previousRange] = LimitSpan[previousRange] + currentSpan; //对于第一个点来说，currentSpan为0
                        //累计面积
                        LimitArea[previousRange] = LimitArea[previousRange] + currentArea; //对于第一个点来说，currentSpan为0
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
                            transPoint[transPoint.Length-1] = currentPoint;
                            for (i = previousRange;i < currentRange;i++ )   //这个循环是求交点，是< currentRange
                            {
                                //两点式：x=((y-y0)*(x1-x0)/(y1-y0))+x0
                                DateTime transcendtimestamp = previousPoint.Timestamp.AddMilliseconds(((LimitRange[i] - previousPoint.Value) * (currentPoint.Timestamp - previousPoint.Timestamp).TotalMilliseconds) / (currentPoint.Value - previousPoint.Value));
                                
                                transPoint[i-previousRange+1] = new PValue(LimitRange[i], transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0);//得到交点                                
                                LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Endtime = transcendtimestamp;                                  //写上一个range的结束时间
                                LimitSpanSeries[i+1].Add(new PValue(1, transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));      //写当期range的开始时间
                            }
                            //2、根区域跨越的点求累计时长和累计面积
                            for (i = previousRange; i <= currentRange; i++) //这个循环是求每一段累加值，是<= currentRange
                            {
                                //——第一个点，currentRange == previousRange，一定走该分支                    
                                currentSpan = (transPoint[i - previousRange + 1].Timestamp - transPoint[i - previousRange].Timestamp).TotalMilliseconds/1000;
                                if (i > 3)
                                {
                                    currentArea = (Math.Abs(transPoint[i - previousRange + 1].Value - LimitRange[i - 1]) + Math.Abs(transPoint[i - previousRange].Value - LimitRange[i - 1])) * currentSpan / 2;
                                }
                                else if(i<3)
                                {
                                    currentArea = (Math.Abs(transPoint[i - previousRange + 1].Value - LimitRange[i]) + Math.Abs(transPoint[i - previousRange].Value - LimitRange[i])) * currentSpan / 2;
                                }
                                else currentArea = 0;
                                //累计时长
                                LimitSpan[i] = LimitSpan[i] + currentSpan; //对于第一个点来说，currentSpan为0
                                //累计面积
                                LimitArea[i] = LimitArea[i] + currentArea; //对于第一个点来说，currentSpan为0
                            }
                        }
                        else
                        {
                            //1、求出焦点，并把焦点和previousPoint、currentPoint构成数组
                            transPoint = new PValue[previousRange - currentRange + 2];//previousPoint、currentPoint以及和限值的交点构成的直线，共currentRange - previousRange + 2个点
                            transPoint[0] = previousPoint;
                            transPoint[transPoint.Length - 1] = currentPoint;
                            for (i = previousRange; i > currentRange; i--)  //这个循环是求交点，是> currentRange
                            {
                                //两点式：x=((y-y0)*(x1-x0)/(y1-y0))+x0
                                DateTime transcendtimestamp = previousPoint.Timestamp.AddMilliseconds(((LimitRange[i-1] - previousPoint.Value) * (currentPoint.Timestamp - previousPoint.Timestamp).TotalMilliseconds) / (currentPoint.Value - previousPoint.Value));
                                
                                transPoint[previousRange - i + 1] = new PValue(LimitRange[i - 1], transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0);
                                LimitSpanSeries[i][LimitSpanSeries[i].Count - 1].Endtime = transcendtimestamp;                                  //写上一个range的结束时间
                                LimitSpanSeries[i - 1].Add(new PValue(1, transcendtimestamp, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));       //写当期range的开始时间
                            }
                            //2、根区域跨越的点求累计时长和累计面积
                            for (i = previousRange; i >= currentRange; i--)//这个循环是求每一段累加值，是>= currentRange
                            {
                                //——第一个点，currentRange == previousRange，一定走该分支                    
                                currentSpan = (transPoint[previousRange-i +1].Timestamp - transPoint[ previousRange-i].Timestamp).TotalSeconds;
                                if (i > 3)
                                {
                                    currentArea = (Math.Abs(transPoint[previousRange - i + 1].Value - LimitRange[i - 1]) + Math.Abs(transPoint[previousRange - i].Value - LimitRange[i - 1])) * currentSpan / 2;
                                }
                                else if(i<3)
                                {
                                    currentArea = (Math.Abs(transPoint[previousRange - i + 1].Value - LimitRange[i]) + Math.Abs(transPoint[previousRange - i].Value - LimitRange[i])) * currentSpan / 2;
                                }
                                else currentArea = 0;
                                
                                //累计时长
                                LimitSpan[i] = LimitSpan[i] + currentSpan; //对于第一个点来说，currentSpan为0
                                //累计面积
                                LimitArea[i] = LimitArea[i] + currentArea; //对于第一个点来说，currentSpan为0
                            }   
                        }


                        //——寻找下降沿或者上升沿统计超限次数
                        //值域分成7个限值区域：0，1，2，3，4，5，6                    
                        //在3H区域看上升沿，即实时值由低向高值变动
                        if (currentRange > 3 && currentRange > previousRange)
                        {
                            for (i = previousRange; i < currentRange; i++) 
                            {
                                if (i >= 3)
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

                }//end for
                
                //计算占比
                if (LimitSpan[1] == 0 || (LimitRange[1] - LimitRange[0]) == 0)
                {
                    LimitAreaRatio[1] = 0;
                }
                else 
                {
                    LimitAreaRatio[1] = LimitArea[1] / (LimitSpan[1] * (LimitRange[1] - LimitRange[0]));//LL面积占比
                }
                if (LimitSpan[2] == 0 || (LimitRange[2] - LimitRange[1]) == 0)
                {
                    LimitAreaRatio[2] = 0;
                }
                else
                {
                    LimitAreaRatio[2] = LimitArea[2] / (LimitSpan[2] * (LimitRange[2] - LimitRange[1]));//L面积占比
                } 
                if (LimitSpan[4] == 0 || (LimitRange[4] - LimitRange[3]) == 0)
                {
                    LimitAreaRatio[4] = 0;
                }
                else
                {
                    LimitAreaRatio[4] = LimitArea[4] / (LimitSpan[4] * (LimitRange[4] - LimitRange[3]));//H面积占比
                } 
                if (LimitSpan[5] == 0 || (LimitRange[5] - LimitRange[4]) == 0)
                {
                    LimitAreaRatio[5] = 0;
                }
                else
                {
                    LimitAreaRatio[5] = LimitArea[5] / (LimitSpan[5] * (LimitRange[5] - LimitRange[4]));//HH面积占比
                }                
              
                
                
                //对找出的时间序列进行过滤，如果长度小于threshold，则去掉
                if (threshold > 0)
                {
                    for (i = 0; i < 7; i++)
                    {
                        for (int j = LimitSpanSeries[i].Count - 1; j >= 0; j--)
                        {
                            if (LimitSpanSeries[i][j].Timespan < threshold * 60)
                            {
                                LimitSpanSeries[i].RemoveAt(j);
                            }
                        }

                    }
                }


                
                //组织计算结果:共返回24个统计结果
                //LLLNumber;LLLSpan;LLLArea;
                //LLNumber;LLSpan;LLArea;;
                //LNumber;LSpan;LArea;;
                //HNumber;HSpan;HArea;;;
                //HHNumber;HHSpan;HHArea;;
                //HHHNumber;HHHSpan;HHHArea;
                //LLAreaRatio,LAreaRatio,HAreaRatio,HHAreaRatio
                
                
                results[0][0]=new PValue(LimitNumber[0], calcuInfo.fstarttime, calcuinfo.fendtime, 0);    //LLLNumber
               
                results[1][0]=new PValue(LimitSpan[0], calcuInfo.fstarttime, calcuinfo.fendtime, 0);      //LLLSpan
               
                results[2][0]=new PValue(LimitArea[0], calcuInfo.fstarttime, calcuinfo.fendtime, 0);      //LLLArea
                
                results[3][0]=new PValue(LimitNumber[1], calcuInfo.fstarttime, calcuinfo.fendtime, 0);    //LLNumber
                
                results[4][0]=new PValue(LimitSpan[1], calcuInfo.fstarttime, calcuinfo.fendtime, 0);      //LLSpan       
                
                results[5][0]=new PValue(LimitArea[1], calcuInfo.fstarttime, calcuinfo.fendtime, 0);      //LLArea
                
                results[6][0]=new PValue(LimitNumber[2], calcuInfo.fstarttime, calcuinfo.fendtime, 0);    //LNumber
                
                results[7][0]=new PValue(LimitSpan[2], calcuInfo.fstarttime, calcuinfo.fendtime, 0);      //LSpan 
               
                results[8][0]=new PValue(LimitArea[2], calcuInfo.fstarttime, calcuinfo.fendtime, 0);      //LArea
                
                results[9][0]=new PValue(LimitNumber[4], calcuInfo.fstarttime, calcuinfo.fendtime, 0);    //HNumber 
                
                results[10][0]=new PValue(LimitSpan[4], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HSpan         
                
                results[11][0]=new PValue(LimitArea[4], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HArea 
                
                results[12][0]=new PValue(LimitNumber[5], calcuInfo.fstarttime, calcuinfo.fendtime, 0);   //HHNumber
                
                results[13][0]=new PValue(LimitSpan[5], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HHSpan        
                
                results[14][0]=new PValue(LimitArea[5], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HHArea 
               
                results[15][0]=new PValue(LimitNumber[6], calcuInfo.fstarttime, calcuinfo.fendtime, 0);   //HHHNumber 
                
                results[16][0]=new PValue(LimitSpan[6], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HHHSpan          
               
                results[17][0]=new PValue(LimitArea[6], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HHHArea 
                
                results[18][0]=new PValue(LimitAreaRatio[1], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //LLAreaRatio
               
                results[19][0]=new PValue(LimitAreaRatio[2], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //LAreaRatio
                
                results[20][0]=new PValue(LimitAreaRatio[4], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HAreaRatio
                
                results[21][0]=new PValue(LimitAreaRatio[5], calcuInfo.fstarttime, calcuinfo.fendtime, 0);     //HHAreaRatio
                 
                results[22] = LimitSpanSeries[0];   //LLLSeries
                
                results[23] = LimitSpanSeries[1];   //LLSeries
               
                results[24] = LimitSpanSeries[2];   //LSeries
                
                results[25] = LimitSpanSeries[4];   //HSeries
                
                results[26] = LimitSpanSeries[5];   //HHSeries
               
                results[27] = LimitSpanSeries[6];   //HHHSeries

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
            }    
        }
        #endregion

    }
}
