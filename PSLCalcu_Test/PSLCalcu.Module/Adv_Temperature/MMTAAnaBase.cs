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
    /// 壁温分析模拟量基本统计算法
    ///
    ///    
    /// 修改纪录
    ///     
    ///		2018.4.14 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.04.14</date>
    /// </author> 
    /// </summary>
    public class MMTAAnaBase : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MMTAAnaBase";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "温度基本统计信息算法";
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
        private string _algorithms = "MMTAAnaBase";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "20;30;40;S";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "极差N1限值;极差N2限值;极差N3限值;计算周期标志S/L。选择长周期L时，源数据类型必须选择rdbset。";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){2}(;){0,1}([SL]){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 19;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "TempMin;" +
                                      "TempAvg;" +
                                      "TempMax;" +
                                      "TempDiv;" +
                                      "TempStdev;" +
                                      "TempVolatility;" +   
                                      "TempVolDMax;"+
                                      "TempDMax;" +
                                      "TempDMaxR;" +
                                      "TempDN1Num;" +
                                      "TempDN2Num;" +
                                      "TempDN3Num;" +
                                      "TempTNum;" +
                                      "TempVMax;" +
                                      "TempVMin;" +
                                      "TempVAvg;" +                                      
                                      "TempPNum;" +
                                      "TempQltR;" +
                                      "TempStatus";
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
                                        "高低差;" +
                                        "标准差;" +
                                        "波动（强度）;" +
                                        "波动力度;"+
                                        "单次极差及时刻;" +
                                        "单次极差比;" +
                                        "极差大于N1次数;" +
                                        "极差大于N2次数(包含N1);" +
                                        "极差大于N3次数(包含N2、N1);" +
                                        "翻转次数;" +
                                        "最大速度绝对值;" +
                                        "最小速度绝对值;" +
                                        "平均速度（速度代数和求平均）;" +                                         
                                        "数据点数;" +
                                        "质量率%;" +
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
            List<PValue>[] results = new List<PValue>[19];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //输入数据和参数检查
                double N1, N2, N3;  //极差超限值
                string calcuMode;   //计算方式选择

                string[] paras = Regex.Split(calcuinfo.fparas, ";|；");      //要测试一下，split分割完仅有三个元素是否可以    
                N1 = double.Parse(paras[0]);
                N2 = double.Parse(paras[1]);
                N3 = double.Parse(paras[2]);
                if (paras.Length == 4)
                    calcuMode = paras[3];   //如果设定了第4个参数，计算模式用第四个参数值。S表示短周期，L表示长周期                
                else
                    calcuMode = "S";        //如果没设定第4个参数，计算模式为短周期


                //定义计算结果
                PValue DivMin = null;           //最低,最低值时刻
                double DivAvg = 0;              //平均
                PValue DivMax = null; 	        //最高,最高值时刻
                double DivDiv = 0;              //高低差
                double DivStdevS = 0;           //标准差
                double DivVolatility = 0; 	    //波动
                double DivVolDMAx = 0;          //波动力度
                PValue DivFVMax = null;         //最大升速,最大升速时刻
                PValue DivNVMax = null;         //最大降速,最大降速发生时刻
                double DivAVMax = 0;            //最大代数速度
                double DivAVMin = 0;            //最小代数速度
                double DivVavg = 0;             //平均速度
                PValue DivDMax = null;          //单次极差,单次极差时刻。注意这个极差的定义与统计中的极差不同。这个极差是指曲线在每两次翻转之间单向曲线段变化值的最大值。
                double DivDMaxR = 0;            //极差比
                double DivDN1Num = 0;           //极差大于 N1 次数
                double DivDN2Num = 0;           //极差大于 N2 次数
                double DivDN3Num = 0;           //极差大于 N3 次数
                double DivTNum = 0;             //翻转次数
                double DivPNum = 0;             //数据点数
                double DivQltR = 0;             //质量率
                double DivStatus = 0;           //计算故障标记

                
                double valuesum;                //本次值与下一个值得和
                double valuesub;                //下一个值和本次值差
                double valuesub_abs;            //下一个值和本次值差的绝对值
                double totalspan;               //总时间长度
                double divspeed;                //每个时间段速度
                PValue previousTurnPoint;       //翻转点
                double previoussub;             //上一次非0差值
                double valuediff;               //曲线两次翻转之间的变化值。


                //关于计算
                //——部分计算结果需要用到截止时刻的值。
                //——需要考虑输入数据之间时间不连贯
                if (calcuMode == "S")
                {
                    #region 短周期计算

                    //0、输入检查。该算法考虑短周期计算和长周期计算两种情况
                    List<PValue> input = new List<PValue>();
                    //0.1、输入处理：输入长度。当输入为空时，所有的输出项为空.
                    if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); //如果源数据为空，则不需要置报警为，计算引擎以记录报警
                    else
                        input = inputs[0];
                    //0.2、输入处理：截止时刻值。该算法采用线性计算，截止时刻点要参与计算。不删除
                    //if (input.Count > 1) input.RemoveAt(input.Count - 1);
                    //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                    for (i = input.Count - 1; i >= 0; i--)
                    {
                        if (input[i].Status != 0) input.RemoveAt(i);
                    }
                    //0.4、输入处理：过滤后结果。
                    //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                    //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                    if (input.Count < 2)
                    {
                        _warningFlag = true;
                        _warningInfo = "对应时间段内的源数据状态位全部异常";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }

                    //2、初始化
                    DivMin = new PValue(input[0].Value, input[0].Timestamp, input[0].Endtime, 0);   //注意这里不能直接用DivMin=input[0]，引用类型
                    DivMax = new PValue(input[0].Value, input[0].Timestamp, input[0].Endtime, 0);
                    DivAvg = 0;                 //平均值。 输入数据小于两个，整体输入为null。否则必有平均值。
                    DivDiv = 0;                 //高低差。
                    totalspan = 0;              //总时间长度
                    DivVolatility = 0;          //波动量为累计值，初始值为0.
                    DivVolDMAx = 0;             //波动力度
                    divspeed = 0;
                    DivFVMax = new PValue(0, input[0].Timestamp, input[0].Endtime, 0);   //最大升速，初始值为0
                    DivNVMax = new PValue(0, input[0].Timestamp, input[0].Endtime, 0);   //最大升速，初始值为0
                    DivAVMax = 0;                   //实际上在第一个周期内，赋初始值
                    DivAVMin = 0;                   //实际上在第一个周期内，赋初始值
                    DivVavg = 0;                    //平均速度。初始值为0。
                    DivDMax = new PValue(0, input[0].Timestamp, input[0].Endtime, 0);                //极差，从初始值为0
                    DivDMaxR = 0;
                    DivDN1Num = 0;                  //极差越线次数。初始值为0.
                    DivDN2Num = 0;                  //极差越线次数。初始值为0.
                    DivDN3Num = 0;                  //极差越线次数。初始值为0.
                    DivTNum = 0;                    //翻转次数，初始值为0.
                    DivPNum = input.Count - 1;      //不包括截止时刻点
                    DivQltR = 1;                    //好质量率，初始值100，全有效。目前DAO层接口过滤坏质量，暂时不计算。
                    DivStatus = 0;                  //故障标志。初始值为0，好质量。目前DAO层接口过滤坏质量，暂时不计算。
                    previousTurnPoint = new PValue(input[0].Value, input[0].Timestamp, input[0].Endtime, 0);    //翻转点
                    previoussub = 0;
                    valuediff = 0;                  //曲线两次翻转之间的变化值

                    //从第0个数据，循环到最后一个时间段的数据input.Count-2.
                    //对于需要使用截止数据的结算，在循环中，直接使用input.Count-1，或者i+1
                    for (i = 0; i < input.Count - 1; i++)
                    {

                        valuesum = input[i].Value + input[i + 1].Value;         //本次值和下一个值的和
                        valuesub = input[i + 1].Value - input[i].Value;         //下一个值和本次值的差
                        valuesub_abs = Math.Abs(valuesub);                      //下一个值和本次值的差的绝对值，这里是为了避免后面每次都计算绝对值
                        totalspan = totalspan + input[i].Timespan;              //总时间长度，单位毫秒，应对数据不连续的情况
                        divspeed = valuesub / (input[i].Timespan / 1000);       //每次的速度，可正可负。注意速度用变化量除以/总秒数

                        //最小值、最小值时刻。和截止时刻值无关。
                        if (input[i].Value < DivMin.Value)
                        {
                            DivMin.Value = input[i].Value;
                            DivMin.Timestamp = input[i].Timestamp;
                            DivMin.Endtime = input[i].Endtime;
                        }
                        //最大值、最大值时刻。和截止时刻值无关。
                        if (input[i].Value > DivMax.Value)
                        {
                            DivMax.Value = input[i].Value;
                            DivMax.Timestamp = input[i].Timestamp;
                            DivMax.Endtime = input[i].Endtime;
                        }
                        //平均值的算法,考虑可以计算不等采样间隔的均值。采用曲线面积（梯形法计算）除以曲线经历时间的算法
                        //——先求面积，最后除时间段长度。
                        //——该算法和截止时刻值有关。
                        DivAvg = DivAvg + valuesum * input[i].Timespan / 2;                 //(上底+下底)*h/2
                        
                        //波动量。和截止时刻值有关。
                        DivVolatility += valuesub_abs * (1000 * 60 / input[i].Timespan);      //波动量，对(绝对值/分钟值)进行累计。

                        
                        //最大代数速度\//最小代数速度
                        if (i == 0)
                        {
                            DivAVMax = Math.Abs(divspeed);
                            DivAVMin = Math.Abs(divspeed);
                        }
                        else
                        {
                            if (Math.Abs(divspeed) > DivAVMax) DivAVMax = Math.Abs(divspeed);
                            if (Math.Abs(divspeed) < DivAVMin) DivAVMin = Math.Abs(divspeed);
                        }
                        //平均速度：
                        //——注意，实际意义上的平均速度，应该是波动量（所有变化量绝对值的累积）除以总时间。
                        //——这里的平均速度，暂时单纯是用每个时间段的速度的代数和求平均。吴鸿回复，暂时还未考虑清楚，先这么做
                        DivVavg += divspeed;

                        //翻转次数和截止时刻值有关。
                        //注意i从1到count-2。所以，计算中i-1和i+1都不会越界。
                        //曲线在每两次翻转之间的变化值绝对值为valuediff
                        //所有valuediff值之中最大的值为极差值
                        if (i > 0)
                        {
                            if ((input[i].Value - input[i - 1].Value) != 0)
                            {
                                //记录前一个不为0的差值
                                previoussub = input[i].Value - input[i - 1].Value;
                            }
                            //判断是否翻转.注意这种判断方式，主要是考虑中间如果有连续几个值相同的点。
                            //——上面记录了差值为0点之前的点
                            //——下面这个，当遇到差值为0的点时，乘积为0，不认为翻转。
                            if ((input[i + 1].Value - input[i].Value) * previoussub < 0)
                            {
                                //统计翻转次数
                                DivTNum += 1;
                                //每当翻转时，求翻转前的曲线段极差值
                                valuediff = Math.Abs(input[i].Value - previousTurnPoint.Value);
                                //找极差
                                if (valuediff > DivDMax.Value)
                                {
                                    DivDMax.Value = valuediff;
                                    DivDMax.Timestamp = previousTurnPoint.Timestamp;
                                    DivDMax.Endtime = input[i].Timestamp;
                                }
                                //极差大于 N1 次数
                                //极差大于 N2 次数
                                //极差大于 N3 次数
                                if (valuediff >= N3) { DivDN3Num++; DivDN2Num++;DivDN1Num++; }  //极差大于N3时，三个统计都需要加
                                else if (valuediff >= N2) { DivDN2Num++; DivDN1Num++; }         //极差大于N2时，N2和N1需要加
                                else if (valuediff >= N1) { DivDN1Num++; }
                                //极差N1<N2<N3，

                                //更新翻转点
                                previousTurnPoint.Value = input[i].Value;
                                previousTurnPoint.Timestamp = input[i].Timestamp;
                                previousTurnPoint.Endtime = input[i].Endtime;
                            }
                        }
                    }//end for

                    //高低差
                    DivDiv = DivMax.Value - DivMin.Value;
                    //平均值
                    DivAvg = DivAvg / totalspan;    //特别注意，求平均值不是用的点乘以span再除以总span。而是用点和轴构成的总面积除以总total求平均。因此平均值得面积使用了截止点，但是最终均值不涉及截止点。而是直接除以totalspan
                    //极差比
                    if ((DivMax.Value - DivMin.Value) != 0)
                        DivDMaxR = DivDMax.Value * 100 / (DivMax.Value - DivMin.Value);
                    else
                        DivDMaxR = 0;
                    //求得平均值后，再循环一遍求标准差，求标准差的时候，不涉及截止点。所以最后除的点数DivPNum，这个数量也不包括截止点。
                    valuesum = 0;
                    for (i = 0; i < input.Count - 1; i++)
                    {
                        valuesum += Math.Pow((input[i].Value - DivAvg), 2);
                    }
                    DivStdevS = Math.Sqrt(valuesum) / DivPNum;      //最后标准差还要开方

                    //波动力度
                    DivVolDMAx = 0;
                    double N1NumberReal = DivDN1Num;                    
                    double N2NumberReal = DivDN2Num - DivDN1Num;         //越限统计中，N2越限次数包含了越N2、N1次数
                    double N3NumberReal = DivDN3Num - DivDN2Num;         //越限统计中，N3越限次数包含了越N3、N2、N1次数
                    if (DivDN3Num!=0)
                        DivVolDMAx = (N1NumberReal * N1 + N2NumberReal * N2 + N3NumberReal * N3) / (DivDN3Num);
                    else
                        DivVolDMAx = 0;

                    #endregion
                }
                else
                {
                    #region 长周期计算
                    //输入采用短周期的计算结果，顺序为
                    //0、最低值及时刻;1、平均;2、最高值及时刻;3、高低差；4、标准差;5、波动;
                    //6、单次极差及时刻;7、单次极差比;8、极差大于 N1 次数;9、极差大于 N2 次数;10、极差大于 N3 次数;11、翻转次数
                    //12、最大速度;13、最小速度（绝对值）;14、平均速度;                    
                    //15、数据点数;16、质量率;17、计算故障标记;
                    //——下面的长周期计算方法，每个指标各自计算，效率不一定高。

                    //0、输入检查。长周期计算采用短周期结果。短周期计算结果，要么全部有值，要么全部没有值。
                    List<PValue> input = new List<PValue>();
                    //0.1、输入处理：输入长度。当输入为空时，所有的输出项为空。短周期计算结果，要么全部有值，要么全部没有值。只要有一个空值，则返回空值。
                    if (inputs == null || inputs.Length == 0 || Array.IndexOf(inputs, null) != -1)
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    //0.2、输入处理：截止时刻值。该算法采用线性计算，截止时刻点要参与计算。各点计算均不相同，长周期下大部分不需要截止时刻值参与
                    //input.RemoveAt(input.Count - 1);
                    //0.3、输入处理：标志位。长周期计算下，所有标志位一定为1
                    //for (i = input.Count - 1; i >= 0; i--)
                    //{
                    //    input.RemoveAt(i);
                    //}
                    //0.4、输入处理：过滤后结果。长周期计算，要么全部为null。要么你全部>2
                    //if (input.Count < 2)
                    //{
                    //    return null;
                    //}

                    //最低值、最低值时刻。从短周期最小值中找最小。不需要截止时刻值。
                    inputs[0].RemoveAt(inputs[0].Count - 1);
                    DivMin = inputs[0].First(n => n.Value == inputs[0].Min(m => m.Value));

                    //平均值，用短周期均值求均值，面积法，需要截止时刻值                    
                    valuesum = 0;
                    totalspan = 0;
                    for (i = 0; i < inputs[1].Count - 1; i++)
                    {
                        valuesum = valuesum + (inputs[1][i].Value + inputs[1][i + 1].Value) * inputs[1][i].Timespan / 2;
                        totalspan = totalspan + inputs[1][i].Timespan;
                    }
                    if (totalspan != 0)
                        DivAvg = valuesum / totalspan;
                    else
                        DivAvg = 0;

                    //最高值、最高值时刻。从长周期最大值中找最大。不需要截止时刻值。
                    inputs[2].RemoveAt(inputs[2].Count - 1);
                    DivMax = inputs[2].First(n => n.Value == inputs[2].Max(m => m.Value));

                    //高低差
                    DivDiv = DivMax.Value - DivMin.Value;

                    //标准差，用短周期标准差求近似标准差
                    inputs[4].RemoveAt(inputs[4].Count - 1);
                    PValue SMin = inputs[4].First(n => n.Value == inputs[4].Min(m => m.Value));
                    PValue SMax = inputs[4].First(n => n.Value == inputs[4].Max(m => m.Value));
                    DivStdevS = SMin.Value + 0.75 * (SMax.Value - SMin.Value);

                    //波动，用短周期波动求波动。不需要截止时刻值。
                    inputs[5].RemoveAt(inputs[5].Count - 1);
                    valuesum = 0;
                    for (i = 0; i < inputs[5].Count; i++)
                    {
                        valuesum = valuesum + inputs[5][i].Value;
                    }
                    if (inputs[5].Count != 0)
                        DivVolatility = valuesum / inputs[5].Count;
                    else
                        DivVolatility = 0;

                    //极差，从短周期最大代数中找最大。不需要截止时刻值。
                    inputs[6].RemoveAt(inputs[6].Count - 1);
                    DivDMax = inputs[6].First(n => n.Value == inputs[6].Max(m => m.Value));

                    //极差比
                    DivDMaxR = DivDMax.Value * 100 / (DivMax.Value - DivMin.Value);

                    //n1次数，用短周期求和。不需要截止时刻值。不精确
                    inputs[8].RemoveAt(inputs[8].Count - 1);
                    DivDN1Num = 0;
                    for (i = 0; i < inputs[8].Count; i++)
                    {
                        DivDN1Num = DivDN1Num + inputs[8][i].Value;
                    }

                    //n2次数，用短周期求和。不需要截止时刻值。不精确
                    inputs[9].RemoveAt(inputs[9].Count - 1);
                    DivDN2Num = 0;
                    for (i = 0; i < inputs[9].Count; i++)
                    {
                        DivDN2Num = DivDN2Num + inputs[9][i].Value;
                    }

                    //n3次数，用短周期求和。不需要截止时刻值。不精确
                    inputs[10].RemoveAt(inputs[10].Count - 1);
                    DivDN3Num = 0;
                    for (i = 0; i < inputs[10].Count; i++)
                    {
                        DivDN3Num = DivDN3Num + inputs[10][i].Value;
                    }

                    //翻转次数，用短周期求和。不需要截止时刻值。不精确
                    inputs[11].RemoveAt(inputs[11].Count - 1);
                    DivTNum = 0;
                    for (i = 0; i < inputs[11].Count; i++)
                    {
                        DivTNum = DivTNum + inputs[11][i].Value;
                    }
                    
                    //最大速度（绝对值）。从短周期最大代数中找最大。不需要截止时刻值。
                    inputs[12].RemoveAt(inputs[12].Count - 1);
                    DivAVMax = inputs[12].First(n => n.Value == inputs[12].Max(m => m.Value)).Value;

                    //最小速度（绝对值）。从短周期最小代数中找最大。不需要截止时刻值。
                    inputs[13].RemoveAt(inputs[13].Count - 1);
                    DivAVMin = inputs[13].First(n => n.Value == inputs[13].Min(m => m.Value)).Value;

                    //平均速度。用短周期平均求平均。不需要截止时刻值。
                    inputs[14].RemoveAt(inputs[14].Count - 1);
                    valuesum = 0;
                    for (i = 0; i < inputs[14].Count; i++)
                    {
                        valuesum = valuesum + inputs[14][i].Value;
                    }
                    if (inputs[14].Count != 0)
                        DivAvg = valuesum / inputs[14].Count;
                    else
                        DivAvg = 0;

                    
                    //数据点数，用短周期求和。不需要截止时刻值。
                    inputs[15].RemoveAt(inputs[15].Count - 1);
                    DivPNum = 0;
                    for (i = 0; i < inputs[15].Count; i++)
                    {
                        DivPNum = DivPNum + inputs[15][i].Value;
                    }

                    //好质量率。暂不计算                   
                    DivQltR = 1;

                    //质量，0位好质量，1位坏质量。暂不计算
                    DivStatus = 0;
                    #endregion
                }

                //组织结算结果
                results[0] = new List<PValue>();
                results[0].Add(DivMin);                                                                 //最低值、最低值时刻
                results[1] = new List<PValue>();
                results[1].Add(new PValue(DivAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //平均
                results[2] = new List<PValue>();
                results[2].Add(DivMax);                                                                 //最高值、最高值时刻
                results[3] = new List<PValue>();
                results[3].Add(new PValue(DivDiv, calcuinfo.fstarttime, calcuinfo.fendtime, 0));        //高低差
                results[4] = new List<PValue>();
                results[4].Add(new PValue(DivStdevS, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //标准差
                results[5] = new List<PValue>();
                results[5].Add(new PValue(DivVolatility, calcuinfo.fstarttime, calcuinfo.fendtime, 0)); //波动（强度）
                results[6] = new List<PValue>();
                results[6].Add(new PValue(DivVolDMAx, calcuinfo.fstarttime, calcuinfo.fendtime, 0));    //波动力度
                results[7] = new List<PValue>();
                results[7].Add(DivDMax);                                                                //单次极差、单次极差时刻
                results[8] = new List<PValue>();
                results[8].Add(new PValue(DivDMaxR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //单次极差比
                results[9] = new List<PValue>();
                results[9].Add(new PValue(DivDN1Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //极差大于 N1 次数
                results[10] = new List<PValue>();
                results[10].Add(new PValue(DivDN2Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //极差大于 N2 次数(包含大于N1次数)
                results[11] = new List<PValue>();
                results[11].Add(new PValue(DivDN3Num, calcuinfo.fstarttime, calcuinfo.fendtime, 0));    //极差大于 N3 次数（包含大于N1次数和N2次数）
                results[12] = new List<PValue>();
                results[12].Add(new PValue(DivTNum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //翻转次数
                results[13] = new List<PValue>();
                results[13].Add(new PValue(DivAVMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //最大速度（绝对值）
                results[14] = new List<PValue>();
                results[14].Add(new PValue(DivAVMin, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //最小速度（绝对值）
                results[15] = new List<PValue>();
                results[15].Add(new PValue(DivVavg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //平均速度                
                results[16] = new List<PValue>();
                results[16].Add(new PValue(DivPNum, calcuinfo.fstarttime, calcuinfo.fendtime, 0));      //数据点数
                results[17] = new List<PValue>();
                results[17].Add(new PValue(DivQltR * 100, calcuinfo.fstarttime, calcuinfo.fendtime, 0));  //质量率%
                results[18] = new List<PValue>();
                results[18].Add(new PValue(DivStatus, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //计算故障标记

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
