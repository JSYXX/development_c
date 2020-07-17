using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class MPVBase : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MPVBase";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "模拟量基础计算";
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
        private string _algorithms = "MPVBase";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "20;30;40;1;2;50;70;S";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "极差N1限值;极差N2限值;极差N3限值;求和乘积系数k;求和偏置系数b;稳定限幅;不稳定限幅;计算周期标志S/L。选择长周期L时，源数据类型必须选择rdbset。";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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
        private int _outputNumber = 30;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "PVMin;" +
                                    "PVAvg;" +
                                    "PVMax;" +
                                    "PVDiv;" +
                                    "PVSum;" +
                                    "PVAbsSum;" +
                                    "PVStdev;" +
                                    "PVVolatility;" +
                                    "PVDMax;" +
                                    "PVDMaxR;" +
                                    "PVDN1Num;" +
                                    "PVDN2Num;" +
                                    "PVDN3Num;" +
                                    "PVTNum;" +
                                    "PVVMax;" +
                                    "PVVMin;" +
                                    "PVVAvg;" +
                                    "PVStbTR;" +
                                    "PVNoStbTR;" +
                                    "PVStbTSL;" +
                                    "PVStbTSLR;" +
                                    "PVNoStbTSL;" +
                                    "PVNoStbTSLR;" +
                                    "PVUpTSL;" +
                                    "PVUpTSLR;" +
                                    "PVDownTSL;" +
                                    "PVDownTSLR;" +
                                    "PVPNum;" +
                                    "PVQltR;" +
                                    "PVStatus";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "最低值及时刻;" +
                                        "平均;" +
                                        "最高值及时刻;" +
                                        "总极差;" +
                                        "和;" +
                                        "绝对值和;" +
                                        "标准差，分母是总点数，60;" +
                                        "波动。注意，从第 2 行算，除以 59;" +
                                        "单次极差及时刻;" +
                                        "单次极差比，绝对值;" +
                                        "极差大于 N1 次数;" +
                                        "极差大于 N2 次数，包含 N1;" +
                                        "极差大于 N3 次数，包含 N2、N1;" +
                                        "翻转次数;" +
                                        "最大速度，不要绝对值;" +
                                        "最小速度，不要绝对值;" +
                                        "平均速度;" +
                                        "稳定时间占比，MMTAAnaDevOR;" +
                                        "不稳定时间占比;" +
                                        "最长连续稳定时间;" +
                                        "最长连续稳定时间占比;" +
                                        "最长连续不稳定时间;" +
                                        "最长连续不稳定时间占比;" +
                                        "最长连续上升时间;" +
                                        "最长连续上升时间占比;" +
                                        "最长连续下降时间;" +
                                        "最长连续下降时间占比;" +
                                        "数据点数;" +
                                        "质量率 %;" +
                                        "计算故障标记;";
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
        #endregion
        #region 计算模块
        /// <summary>
        /// 计算模块算法实现:
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
            List<PValue>[] results = new List<PValue>[30];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //输入数据和参数检查
                double N1, N2, N3, k, b, StbL, NoStbL;  //极差超限值
                string calcuMode;   //计算方式选择

                string[] paras = Regex.Split(calcuinfo.fparas, ";|；");      //要测试一下，split分割完仅有三个元素是否可以    
                N1 = double.Parse(paras[0]);
                N2 = double.Parse(paras[1]);
                N3 = double.Parse(paras[2]);
                k = double.Parse(paras[3]);
                b = double.Parse(paras[4]);
                StbL = double.Parse(paras[5]);
                NoStbL = double.Parse(paras[6]);
                if (paras.Length == 8)
                    calcuMode = paras[7];   //如果设定了第4个参数，计算模式用第四个参数值。S表示短周期，L表示长周期                
                else
                    calcuMode = "S";        //如果没设定第4个参数，计算模式为短周期


                //定义计算结果
                PValue PVMin=null;                    //最低值及时刻  1             
                double PVAvg=0;                       //平均 2
                PValue PVMax=null;                    //最高值及时刻;3
                double PVDiv=0;                       //总极差;4
                double PVSum=0;                       //和;5
                double PVAbsSum=0;                    //绝对值和;6
                double PVStdev=0;                     //标准差，分母是总点数，60;7
                double PVVolatility=0;                //波动。注意，从第 2 行算，除以 59;8
                PValue PVDMax=null;                   //单次极差及时刻;9
                double PVDMaxR=0;                     //单次极差比，绝对值;10
                double PVDN1Num=0;                    //极差大于 N1 次数;11
                double PVDN2Num=0;                    //极差大于 N2 次数，包含 N1;12
                double PVDN3Num=0;                    //极差大于 N3 次数，包含 N2、N1;13
                double PVTNum=0;                      //翻转次数;14
                double PVVMax=0;                      //最大速度，不要绝对值;15
                double PVVMin=0;                      //最小速度，不要绝对值;16
                double PVVAvg=0;                      //平均速度;17
                double PVStbTR=0;                     //稳定时间占比，MMTAAnaDevOR;18
                double PVNoStbTR=0;                  //不稳定时间占比;19
                double PVStbTSL=0;                   //最长连续稳定时间;20
                double PVStbTSLR=0;                  //最长连续稳定时间占比;21
                double PVNoStbTSL=0;                 //最长连续不稳定时间;22
                double PVNoStbTSLR=0;                //最长连续不稳定时间占比;23
                double PVUpTSL=0;                    //最长连续上升时间;24
                double PVUpTSLR=0;                   //最长连续上升时间占比;25
                double PVDownTSL=0;                  //最长连续下降时间;26
                double PVDownTSLR=0;                 //最长连续下降时间占比;27
                double PVPNum=0;                     //数据点数;28
                double PVQltR=0;                     //质量率 %;29
                double PVStatus=0;                   //计算故障标记;"30

                //计算辅助内容
                double valuesum;                //本次值与下一个值得和
                double valuesub;                //下一个值和本次值差
                double valuesub_abs;            //下一个值和本次值差的绝对值
                double totalspan;               //总时间长度
                double divspeed;                //每个时间段速度
                PValue previousTurnPoint;       //翻转点
               
                

                //关于计算
                //——部分计算结果需要用到截止时刻的值。
                //——需要考虑输入数据之间时间不连贯
                if (calcuMode == "S")
                {
                    
                    #region 短周期计算
                    //0、输入检查。该算法考虑短周期计算和长周期计算两种情况
                    List<PValue> input = new List<PValue>();   //有一部分数据需要剔除坏质量点
                   
                    //0.1、输入处理：输入长度。当输入为空时，所有的输出项为空.
                    if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); //如果源数据为空，则不需要置报警为，计算引擎以记录报警
                    else
                        input = inputs[0];
                    //不对坏质量点进行处理
                    //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                    //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                    if (input.Count < 2)
                    {
                        _warningFlag = true;
                        _warningInfo = "对应时间段内的源数据状态位全部异常";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }


                    //2、初始化
                    PVMin = new PValue(input[0].Value, input[0].Timestamp, input[0].Endtime, 0);   //**1
                    PVMax = new PValue(input[0].Value, input[0].Timestamp, input[0].Endtime, 0);   //**3
                    PVAvg = 0;                 //平均值  **2 
                    PVDiv = 0;                 //高低差  **4
                    PVSum = 0;                 //和      **5
                    PVAbsSum = 0;              //绝对值和  **6
                    PVVolatility = 0;          //波动量为累计值，初始值为0.
                    totalspan = 0;             //总时间长度
                    divspeed = 0;
                    PVVMax = 0;                   //实际上在第一个周期内，赋初始值
                    PVVMin = 0;                   //实际上在第一个周期内，赋初始值
                    PVVAvg = 0;                    //平均速度。初始值为0。
                    PVDMax = new PValue(0, input[0].Timestamp, input[0].Endtime, 0);                //极差，从初始值为0
                    PVDMaxR = 0;
                    PVDN1Num = 0;                  //极差越线次数。初始值为0.
                    PVDN2Num = 0;                  //极差越线次数。初始值为0.
                    PVDN3Num = 0;                  //极差越线次数。初始值为0.
                    PVTNum = 0;                    //翻转次数，初始值为0.
                    PVQltR = 1;                    //好质量率，初始值100，全有效。目前DAO层接口过滤坏质量，暂时不计算。
                    PVStatus = 0;                  //故障标志。初始值为0，好质量。目前DAO层接口过滤坏质量，暂时不计算。
                    previousTurnPoint = new PValue(input[0].Value, input[0].Timestamp, input[0].Endtime, 0);    //翻转点
                   

                    /*
                   * *占比类统计  没有去除状态不正常的数据
                   */
                    PVStbTR = 0;                    //稳定时间占比，MMTAAnaDevOR;
                    PVNoStbTR = 0;                  //不稳定时间占比;
                    PVStbTSL = 0;                   //最长连续稳定时间;
                    PVStbTSLR = 0;                  //最长连续稳定时间占比;
                    PVNoStbTSL = 0;                 //最长连续不稳定时间;
                    PVNoStbTSLR = 0;                //最长连续不稳定时间占比;
                    PVUpTSL = 0;                    //最长连续上升时间;
                    PVUpTSLR = 0;                   //最长连续上升时间占比;
                    PVDownTSL = 0;                  //最长连续下降时间;
                    PVDownTSLR = 0;                 //最长连续下降时间占比;
                    //辅助计算数据
                    double wending = 0;
                    double nwending = 0;
                    double wcount = 0;
                    double nwcount = 0;
                    double up = 0;
                    double down = 0;
                    List<double> wenList = new List<double>();
                    List<double> nwenList = new List<double>();
                    List<double> upList = new List<double>();
                    List<double> downList = new List<double>();
                    List<PValue> absValue = new List<PValue>();
                    input.RemoveAt(input.Count - 1);  //移除最后一位
                    PVPNum = input.Count;      //不包括截止时刻点
                    PVQltR = 1;                    //好质量率，初始值100，全有效。目前DAO层接口过滤坏质量，暂时不计算。
                    PVStatus = 0;                  //故障标志。初始值为0，好质量。目前DAO层接口过滤坏质量，暂时不计算。
                    //从第0个数据，循环到最后一个时间段的数据input.Count.
                    //对于需要使用截止数据的结算，在循环中，直接使用input.Count-1，或者i+1
                    for (i = 0; i < input.Count; i++)
                    {
                        if (i < input.Count - 1) {
                            valuesub = input[i + 1].Value - input[i].Value;         //下一个值和本次值的差
                            valuesub_abs = Math.Abs(valuesub);                      //下一个值和本次值的差的绝对值，这里是为了避免后面每次都计算绝对值
                            divspeed = valuesub;       //每次的速度，可正可负。注意速度用变化量除以/总秒数
                            PVVolatility += valuesub_abs;
                            //最大代数速度\//最小代数速度
                            if (i == 0)
                            {
                                PVVMax = divspeed; //Math.Abs(divspeed);
                                PVVMin = divspeed; //Math.Abs(divspeed);
                            }
                            else
                            {
                                if (divspeed > PVVMax) PVVMax = divspeed;
                                if (divspeed < PVVMin) PVVMin = divspeed;
                            }
                            PVVAvg += Math.Abs(divspeed);
                        }
                        
                     
                       
                        PVSum = PVSum + input[i].Value;                         //和
                        PVAbsSum = PVAbsSum + Math.Abs(input[i].Value);         //绝对值求和
                        //最小值、最小值时刻。和截止时刻值无关。
                        if (input[i].Value < PVMin.Value)
                        {
                            PVMin.Value = input[i].Value;
                            PVMin.Timestamp = input[i].Timestamp;
                            PVMin.Endtime = input[i].Endtime;
                        }
                        //最大值、最大值时刻。和截止时刻值无关。
                        if (input[i].Value > PVMax.Value)
                        {
                            PVMax.Value = input[i].Value;
                            PVMax.Timestamp = input[i].Timestamp;
                            PVMax.Endtime = input[i].Endtime;
                        }
                        
                        if (i > 0) {
                            double d = Math.Abs(input[i].Value - input[i - 1].Value); //差的绝对值
                            double d1 = input[i].Value - input[i - 1].Value;
                            if (d < StbL && input[i].Status == 0)
                            {
                                wending = wending + 1;   //统计稳定时间
                                wcount = wcount + 1;
                            }
                            else
                            {
                                wenList.Add(wcount);     //连续稳定时间集合
                                wcount = 0;
                            }
                            if (d > NoStbL && input[i].Status == 0)
                            {
                                nwending = nwending + 1;  //统计不稳定时间
                                nwcount = nwcount + 1;
                            }
                            else
                            {
                                nwenList.Add(nwcount);    //连续不稳定时间集合
                                nwcount = 0;
                            }

                            if ((input[i].Value - input[i - 1].Value) > 0 && input[i].Status == 0)
                            {
                                up = up + 1;
                            }
                            else
                            {
                                upList.Add(up);
                                up = 0;
                            }
                            if ((input[i].Value - input[i - 1].Value) < 0 && input[i].Status == 0)
                            {
                                down = down + 1;
                            }
                            else
                            {
                                downList.Add(down);
                                down = 0;
                            }
                            if (d >= N3) { PVDN3Num++; PVDN2Num++; PVDN1Num++; }  //极差大于N3时，三个统计都需要加
                            else if (d >= N2) { PVDN2Num++; PVDN1Num++; }         //极差大于N2时，N2和N1需要加
                            else if (d >= N1) { PVDN1Num++; }
                            double pre;
                            if (i >= 1 && i < input.Count - 2)
                            {
                                pre = input[i].Value - input[i - 1].Value;
                                if (pre * (input[i + 1].Value - input[i].Value) < 0)
                                {
                                    PVTNum = PVTNum + 1;  //翻转次数
                                }
                            }
                            //单次极差
                            PValue p = new PValue();
                            p.Value = d1;
                            p.Timestamp = input[i].Timestamp;
                            p.Endtime = input[i].Endtime;
                            p.Status = input[i].Status;
                            absValue.Add(p);
                        }
                    }//end for
                    
                    PVAvg = PVSum / (input.Count);
                    PVSum = k * PVSum + b;
                    PVAbsSum = k * PVAbsSum + b;
                    PVVAvg = PVVAvg / (input.Count-1);
                    PVVolatility = PVVolatility / (input.Count-1);
                    //求得平均值后，再循环一遍求标准差，求标准差的时候，不涉及截止点。所以最后除的点数DivPNum，这个数量也不包括截止点。
                   
                    PVStdev=BaseCalcu.getStd(input).Value;
                    if (Math.Abs(absValue.Max().Value) > Math.Abs(absValue.Min().Value))
                    {
                        PVDMax = absValue.Max();
                    }
                    else {
                        PVDMax = absValue.Min();
                    }
                    PVDiv = PVMax.Value - PVMin.Value;
                    //极差比
                    if ((PVMax.Value - PVMin.Value) != 0)
                        PVDMaxR = Math.Abs(PVDMax.Value * 100 / PVDiv);
                    else
                        PVDMaxR = 0;
                    PVStbTR = wending / (input.Count) * 100;  //稳定时间占比
                    PVNoStbTR = nwending / (input.Count) * 100; //不稳定时间占比
                    PVStbTSL = wenList.Count>0?wenList.Max():0;                        //最长连续稳定时间
                    PVStbTSLR = PVStbTSL / (input.Count) * 100; //最长连续稳定时间占比
                    PVNoStbTSL = nwenList.Count>0?nwenList.Max():0;                     //最长连续不稳定时间
                    PVNoStbTSLR = PVNoStbTSL / (input.Count) * 100;  //最长连续不稳定时间占比
                    PVUpTSL = upList.Count>0?upList.Max():0;                        //最长连续上升时间
                    PVUpTSLR = PVUpTSL / (input.Count) * 100;     //最长连续上升时间占比
                    PVDownTSL = downList.Count>0?downList.Max():0;                    //最长连续下降时间
                    PVDownTSLR = PVDownTSL / (input.Count) * 100;  //最长连续下降时间占比
                    #endregion
                }
                else
                {
                    #region 长周期计算
                    //输入采用短周期的计算结果，顺序为
                    //最低值及时刻  1  //平均 2 //最高值及时刻;3 //总极差;4 //和;5
                    //绝对值和;6  //标准差，分母是总点数，60;7  //波动。注意，从第 2 行算，除以 59;8    
                    //单次极差及时刻;9   //单次极差比，绝对值;10   //极差大于 N1 次数;11       
                    //极差大于 N2 次数，包含 N1;12  //极差大于 N3 次数，包含 N2、N1;13  //翻转次数;14       
                    //最大速度，不要绝对值;15 //最小速度，不要绝对值;16  //平均速度;17         
                    //稳定时间占比，MMTAAnaDevOR;18   //不稳定时间占比;19   //最长连续稳定时间;20    
                    //最长连续稳定时间占比;21   //最长连续不稳定时间;22    //最长连续不稳定时间占比;23       
                    //最长连续上升时间;24      //最长连续上升时间占比;25   //最长连续下降时间;26     
                    //最长连续下降时间占比;27  //数据点数;28   //质量率 %;29   //计算故障标记;"30 
                    //——下面的长周期计算方法，每个指标各自计算，效率不一定高

                    if (inputs == null || inputs.Length == 0 || Array.IndexOf(inputs, null) != -1)
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    // 最小  最低值
                    inputs[0].RemoveAt(inputs[0].Count - 1);
                    PVMin = inputs[0].First(n => n.Value == inputs[0].Min(m => m.Value));
                    //平均，所用的短周期等间隔就没问题  2
                    valuesum = 0;
                    totalspan = 0;
                    for (i = 0; i < inputs[1].Count - 1; i++)
                    {
                        valuesum = valuesum + (inputs[1][i].Value + inputs[1][i + 1].Value) * inputs[1][i].Timespan / 2;
                        totalspan = totalspan + inputs[1][i].Timespan;
                    }
                    if (totalspan != 0)
                        PVAvg = valuesum / totalspan;
                    else
                        PVAvg = 0;
                    //最大，保留原时刻  最大值 3
                    inputs[2].RemoveAt(inputs[2].Count - 1);
                    PVMax = inputs[2].First(n => n.Value == inputs[2].Max(m => m.Value));
                    //最大  总极差4  
                    PVDiv = PVMax.Value - PVMin.Value;
                    //和    和5
                    PVSum = BaseCalcu.getSum(inputs[4]).Value;
                    //绝对值和  绝对值和6
                    PVAbsSum = BaseCalcu.getAbsSum(inputs[5]).Value;
                    //最大*0.75  标准差7
                    PVStdev = 0.75 * BaseCalcu.getMax(inputs[6]).Value;
                    //最大  波动8
                    PVVolatility = BaseCalcu.getMax(inputs[7]).Value;
                    //最大，保留原时刻  单次极差及时刻9
                    PVDMax = BaseCalcu.getMax(inputs[8]);
                    //按定义做：极差最大值 / (最大 - 最小） 极差比10
                    PVDMaxR = PVDMax.Value / (PVMax.Value - PVMin.Value) * 100;
                    //和  大于N1 11
                    PVDN1Num = BaseCalcu.getSum(inputs[10]).Value;
                    //和  大于N2 12
                    PVDN2Num = BaseCalcu.getSum(inputs[11]).Value;
                    //和  大于N3 13
                    PVDN3Num = BaseCalcu.getSum(inputs[12]).Value;
                    //和     翻转次数14
                    PVTNum = BaseCalcu.getSum(inputs[13]).Value;
                    //最大   最大速度15
                    PVVMax = BaseCalcu.getMax(inputs[14]).Value;
                    //最小   最小速度16
                    PVVMin = BaseCalcu.getMin(inputs[15]).Value;
                    //平均   平均速度17
                    PVVAvg = BaseCalcu.getAvg(inputs[16]).Value;
                    //平均   稳定时间占比18
                    PVStbTR = BaseCalcu.getAvg(inputs[17]).Value;
                    //平均   不稳定时间占比19
                    PVNoStbTR = BaseCalcu.getAvg(inputs[18]).Value;
                    //最大   连续最长稳定时间20
                    PVStbTSL = BaseCalcu.getMax(inputs[19]).Value;
                    //平均   连续最长稳定时间占比21
                    PVStbTSLR = BaseCalcu.getAvg(inputs[20]).Value;
                    //最大   连续最长不稳定时间  22
                    PVNoStbTSL = BaseCalcu.getMax(inputs[21]).Value;
                    //平均   连续最长不稳定时间占比23
                    PVNoStbTSLR = BaseCalcu.getAvg(inputs[22]).Value;
                    //最大   最大连续上升时间 24
                    PVUpTSL = BaseCalcu.getMax(inputs[23]).Value;
                    //平均   最大连续上升时间占比 25
                    PVUpTSLR = BaseCalcu.getAvg(inputs[24]).Value;
                    //最大   最大连续下降时间 26
                    PVDownTSL = BaseCalcu.getMax(inputs[25]).Value;
                    //平均   最大连续下降时间占比 27
                    PVDownTSLR = BaseCalcu.getAvg(inputs[26]).Value;
                    //和     数据点数 28
                    //平均   质量率
                    //按定义做  计算故障标记
                    //数据点数，用短周期求和。不需要截止时刻值。
                    inputs[15].RemoveAt(inputs[15].Count - 1);
                    PVPNum = 0;
                    for (i = 0; i < inputs[15].Count; i++)
                    {
                        PVPNum = PVPNum + inputs[15][i].Value;
                    }
                    //好质量率。暂不计算                   
                    PVQltR = 1;
                    //质量，0位好质量，1位坏质量。暂不计算
                    PVStatus = 0;
                    #endregion
                }
              
                //组织结算结果
                results[0] = new List<PValue>();
                results[0].Add(PVMin);                                                                 //最低值、最低值时刻
                results[1] = new List<PValue>();
                results[1].Add(new PValue(PVAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //平均
                results[2] = new List<PValue>();
                results[2].Add(PVMax);                                                                 //最高值、最高值时刻
                results[3] = new List<PValue>();
                results[3].Add(new PValue(PVDiv, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //高低差
                results[4] = new List<PValue>();
                results[4].Add(new PValue(PVSum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));       //和
                results[5] = new List<PValue>();
                results[5].Add(new PValue(PVAbsSum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));       //绝对值和
                results[6] = new List<PValue>();
                results[6].Add(new PValue(PVStdev, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //标准差
                results[7] = new List<PValue>();
                results[7].Add(new PValue(PVVolatility, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //波动
                results[8] = new List<PValue>();
                results[8].Add(PVDMax);     //单次极差及时刻
                results[9] = new List<PValue>();
                results[9].Add(new PValue(PVDMaxR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //单次极差比
                results[10] = new List<PValue>();
                results[10].Add(new PValue(PVDN1Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //极差大于 N1 次数
                results[11] = new List<PValue>();
                results[11].Add(new PValue(PVDN2Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //极差大于 N2 次数
                results[12] = new List<PValue>();
                results[12].Add(new PValue(PVDN3Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //极差大于 N3 次数                                                    
                results[13] = new List<PValue>();
                results[13].Add(new PValue(PVTNum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //翻转次数; 
                results[14] = new List<PValue>();
                results[14].Add(new PValue(PVVMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最大速度;  
                results[15] = new List<PValue>();
                results[15].Add(new PValue(PVVMin, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最小速度;  
                results[16] = new List<PValue>();
                results[16].Add(new PValue(PVVAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //平均速度; 
                results[17] = new List<PValue>();
                results[17].Add(new PValue(PVStbTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //稳定时间占比;
                results[18] = new List<PValue>();
                results[18].Add(new PValue(PVNoStbTR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //不稳定时间占比;
                results[19] = new List<PValue>();
                results[19].Add(new PValue(PVStbTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续稳定时间;
                results[20] = new List<PValue>();
                results[20].Add(new PValue(PVStbTSLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续稳定时间占比;
                results[21] = new List<PValue>();
                results[21].Add(new PValue(PVNoStbTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续不稳定时间;
                results[22] = new List<PValue>();
                results[22].Add(new PValue(PVNoStbTSLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续不稳定时间占比;
                results[23] = new List<PValue>();
                results[23].Add(new PValue(PVUpTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续上升时间;
                results[24] = new List<PValue>();
                results[24].Add(new PValue(PVUpTSLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续上升时间占比;
                results[25] = new List<PValue>();
                results[25].Add(new PValue(PVDownTSL, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续下降时间;
                results[26] = new List<PValue>();
                results[26].Add(new PValue(PVDownTSLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //最长连续下降时间占比;
                results[27] = new List<PValue>();
                results[27].Add(new PValue(PVPNum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //数据点数
                results[28] = new List<PValue>();
                results[28].Add(new PValue(PVQltR * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));  //质量率%
                results[29] = new List<PValue>();
                results[29].Add(new PValue(PVStatus, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //计算故障标记
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

        #region 辅助函数

        #endregion

    }
}
