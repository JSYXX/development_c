using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 线性拟合分布统计算法22段   
    /// 22段分布统计，其中第11区段的值域范围为[min,max]，其他段的值域范围以第10段类推。
    /// 参数形式“0,100”。
    /// ——线性拟合分布计算不统计各分段内的时间系列。
    /// ——线性拟合分布计算主要的缺点：
    ///     1、由于每两个点之间要考虑是否与越限值像交，导致算法复杂，速度比较慢。
    ///     2、另外，时间都是以两点之间的情况来统计的。当遇到中间有坏质量点被剔除的时候，目前的算法，会认为坏质量前一个有效点直接变到坏质量后一个有效点。那么即使中间都为坏质量时间，最后统计时，这段时间仍然会被统计到某个区间内。
    ///     3、当所有的点均为坏质量点，统计结果返回均为0。
    ///     4、上面这些问题导致，该算法速度慢，且不准确。
    ///     快速分布（阶梯型）算法，不存在上述问题。
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。本算法按线性处理输入数据，截止时刻数据需要被算法使用
    ///		2017.04.12 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2017.04.12</date>
    /// </author> 
    /// </summary>
    public class MLDistribute22 : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称22段分布D22、包含的算法distribution、输出项个数22、输出项名称distribution,D22P1,D22P2,...,D22P22、输出项写入的数据表名称
        private string _moduleName = "MLDistribute22";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "线性拟合分布统计算法22段";
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
        private string _algorithms = "MLDistribute22";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYNNNNNNNNNNNNNNNNNNNNN";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "50;55";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "线性拟合分布统计第11段下限(也是整体量程中点);第11段上限。";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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
        private int _outputNumber = 43;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "LD22S0;LD22S1;LD22S2;LD22S3;LD22S4;LD22S5;LD22S6;LD22S7;" +
                                      "LD22S8;LD22S9;LD22S10;LD22S11;LD22S12;LD22S13;LD22S14;" +
                                      "LD22S15;LD22S16;LD22S17;LD22S18;LD22S19;LD22S20;LD22S21;" +
                                      "LD22SPAN0;LD22SPAN1;LD22SPAN2;LD22SPAN3;LD22SPAN4;LD22SPAN5;LD22SPAN6;LD22SPAN7;" +
                                      "LD22SPAN8;LD22SPAN9;LD22SPAN10;LD22SPAN11;LD22SPAN12;LD22SPAN13;LD22SPAN14;" +
                                      "LD22SPAN15;LD22SPAN16;LD22SPAN17;LD22SPAN18;LD22SPAN19;LD22SPAN20"
                                      ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "22段分布第0段;22段分布第1段;22段分布第2段;22段分布第3段;22段分布第4段;" +
                                        "22段分布第5段;22段分布第6段;22段分布第7段;22段分布第8段;" +
                                        "22段分布第9段;22段分布第10段;22段分布第11段;22段分布第12段;" +
                                        "22段分布第13段;22段分布第14段;22段分布第15段;22段分布第16段;" +
                                        "22段分布第17段;22段分布第18段;22段分布第19段;22段分布第20段;" +
                                        "22段分布第21段;"+
                                        "22段分布第0段时间段;22段分布第1段时间段;22段分布第2段时间段;22段分布第3段时间段;22段分布第4段时间段;" +
                                        "22段分布第5段时间段;22段分布第6段时间段;22段分布第7段时间段;22段分布第8段时间段;" +
                                        "22段分布第9段时间段;22段分布第10段时间段;22段分布第11段时间段;22段分布第12段时间段;" +
                                        "22段分布第13段时间段;22段分布第14段时间段;22段分布第15段时间段;22段分布第16段时间段;" +
                                        "22段分布第17段时间段;22段分布第18段时间段;22段分布第19段时间段;22段分布第20段时间段;" 
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

        #region 输出命名
        public static string NAME(int num)
        {
            string output_name = "distribution";
            string n1 = ";D" + num.ToString() + "P";
            for (int i = 1; i <= num - 1; i++)
            {
                output_name = output_name + n1 + i.ToString();
            }



            return output_name;
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

        #region 计算模块
        /// <summary>
        /// 计算模块算法实现:22段分布distribution
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数: a：第十二段区间的下限，b：第十二段区间的上限</param>       
        /// <returns>计算结果22段分布distribution的22个结果</returns>

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
            int Segment = 22;      //

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[Segment * 2 - 1];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();                
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入
                List<PValue> input =new List<PValue>();
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
                    if (input[i].Status != 0) input.RemoveAt(i);
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

               

                #region 初始化
               
                PValue[] Result = new PValue[Segment]; //返回每段的结果
                List<PValue>[] limittime = new List<PValue>[Segment - 1];
                for (i = 0; i < Segment - 1; i++)
                {
                    limittime[i] = new List<PValue>();
                }
                double[] limit = new double[Segment - 1];  //每一段节点的值

                //Console.WriteLine("请输入计算{0}段分布所需要的两个参数，第{1}段区间的下限和第{1}段区间的上限，两个参数以;分隔:\n", Segment, Segment / 2 + 1);
                //calcuinfo.fparas = Console.ReadLine();
                //string[] para = calcuinfo.fparas.Split(new char[] { ',' });
                string[] paras = Regex.Split(calcuinfo.fparas, ";|；"); 
                double a = double.Parse(paras[0]);
                double b = double.Parse(paras[1]);
                double interval = b - a;  //每一段的间隔

                for (i = 0; i < Segment; i++)
                {
                    Result[i] = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);

                    if (i < Segment - 1)
                    {
                        limit[i] = 0;
                    }

                }
                List<PValue> pvalues = input;
                double totalMilliSecondsForAll = 0;     //所有有效点总时长（毫秒）
                int len = input.Count;

                #endregion

                #region 根据a，b得出每一段节点的值limitt[Segment-1]
                //根据a，b得出每一段节点的值 ，Segment是分段数。如果是22段，则a,b对应的是第10个节点和第11个节点（节点从0开始计算）的值。
                limit[(Segment - 2) / 2] = a;
                limit[(Segment - 2) / 2 + 1] = b;
                for (i = 0; i < (Segment - 2) / 2; i++)
                {
                    limit[i] = a - (((Segment - 2) / 2) - i) * interval;
                }
                for (i = (Segment - 2) / 2 + 2; i <= Segment - 2; i++)
                {
                    limit[i] = b + (i - ((Segment - 2) / 2 + 1)) * interval;
                }

                #endregion

                #region 分布计算
                for (i = 0; i < len - 1; i++)
                {
                    totalMilliSecondsForAll += pvalues[i].Timespan;             //这里累计的是毫秒
                    #region 首先判断pvalues[i]和pvalues[i+1]分别位于第几段，记下段号

                    int Segment_i = 0; //pvalues[i]所在的段号
                    int Segment_i_1 = 0;//pvalues[i+1]所在的段号

                    for (int j = 0; j <= Segment - 2; j++)
                    {
                        if (j == 0)
                        {
                            if (pvalues[i].Value < limit[j])
                            {
                                Segment_i = j;

                            }

                            if (pvalues[i + 1].Value < limit[j])
                            {
                                Segment_i_1 = j;

                            }
                        }
                        if (j == Segment - 2)
                        {
                            if (pvalues[i].Value >= limit[j])
                            {
                                Segment_i = j + 1;

                            }

                            if (pvalues[i + 1].Value >= limit[j])
                            {
                                Segment_i_1 = j + 1;

                            }
                        }
                        if (j > 0 & j <= Segment - 2)
                        {
                            if (pvalues[i].Value >= limit[j - 1] & pvalues[i].Value < limit[j])
                            {
                                Segment_i = j;
                            }
                            if (pvalues[i + 1].Value >= limit[j - 1] & pvalues[i + 1].Value < limit[j])
                            {
                                Segment_i_1 = j;
                            }

                        }
                    }


                    #endregion

                    TimeSpan ts = pvalues[i + 1].Timestamp - pvalues[i].Timestamp;
                    TimeSpan T_interval = pvalues[i + 1].Timestamp - pvalues[i].Endtime;
                    double diff = Math.Abs(pvalues[i + 1].Value - pvalues[i].Value);

                    //判断pvalues[i]和pvalues[i+1]是否在同一段
                    if (Segment_i == Segment_i_1)
                    {
                        Result[Segment_i].Value = Result[Segment_i].Value + ts.TotalMilliseconds;
                    }


                    //判断pvalues[i]和pvalues[i+1]哪个在高段哪个在低段

                    #region  如果pvalues[i]在低段
                    //如果pvalues[i]在低段
                    if (Segment_i < Segment_i_1)
                    {
                        //利用相似三角形的原理来求得每一段的时间


                        if ((Segment_i_1 - Segment_i) == 1)
                        {
                            Result[Segment_i].Value = (limit[Segment_i] - pvalues[i].Value) / diff * ts.TotalMilliseconds + Result[Segment_i].Value;
                            Result[Segment_i_1].Value = (pvalues[i + 1].Value - limit[Segment_i_1 - 1]) / diff * ts.TotalMilliseconds + Result[Segment_i_1].Value;

                            DateTime T1 = pvalues[i].Endtime + TimeSpan.FromSeconds((limit[Segment_i] - pvalues[i].Value) / diff * T_interval.TotalMilliseconds);
                            PValue time = new PValue(0, T1, calcuinfo.fendtime, 1);
                            limittime[Segment_i].Add(time);


                        }

                        if ((Segment_i_1 - Segment_i) >= 2)
                        {
                            Result[Segment_i].Value = (limit[Segment_i] - pvalues[i].Value) / diff * ts.TotalMilliseconds + Result[Segment_i].Value;
                            Result[Segment_i_1].Value = (pvalues[i + 1].Value - limit[Segment_i_1 - 1]) / diff * ts.TotalMilliseconds + Result[Segment_i_1].Value;

                            DateTime T1 = pvalues[i].Endtime + TimeSpan.FromSeconds((limit[Segment_i] - pvalues[i].Value) / diff * T_interval.TotalMilliseconds);
                            PValue time1 = new PValue(0, T1, calcuinfo.fendtime, 1);
                            limittime[Segment_i].Add(time1);

                            DateTime T2 = pvalues[i + 1].Timestamp - TimeSpan.FromSeconds((pvalues[i + 1].Value - limit[Segment_i_1 - 1]) / diff * T_interval.TotalMilliseconds);
                            PValue time2 = new PValue(0, T2, calcuinfo.fendtime, 1);
                            limittime[Segment_i_1 - 1].Add(time2);


                            for (int k = Segment_i + 1; k < Segment_i_1; k++)
                            {
                                Result[k].Value = interval / diff * ts.TotalMilliseconds + Result[k].Value;

                                if (k < Segment_i_1 - 1)
                                {
                                    DateTime T = limittime[k - 1][limittime[k - 1].Count - 1].Timestamp + TimeSpan.FromSeconds(interval / diff * T_interval.TotalMilliseconds);
                                    PValue time = new PValue(0, T, calcuinfo.fendtime, 1);
                                    limittime[k].Add(time);
                                }


                            }
                        }

                    }
                    #endregion

                    #region  如果pvalues[i]在高段
                    //如果pvalues[i]在低段
                    if (Segment_i > Segment_i_1)
                    {
                        //利用相似三角形的原理来求得每一段的时间


                        if ((Segment_i - Segment_i_1) == 1)
                        {
                            Result[Segment_i].Value = (pvalues[i].Value - limit[Segment_i - 1]) / diff * ts.TotalMilliseconds + Result[Segment_i].Value;
                            Result[Segment_i_1].Value = (limit[Segment_i_1] - pvalues[i + 1].Value) / diff * ts.TotalMilliseconds + Result[Segment_i_1].Value;

                            DateTime T1 = pvalues[i].Endtime + TimeSpan.FromSeconds((pvalues[i].Value - limit[Segment_i - 1]) / diff * T_interval.TotalMilliseconds);
                            PValue time1 = new PValue(0, T1, calcuinfo.fendtime, 2);
                            limittime[Segment_i - 1].Add(time1);
                        }

                        if ((Segment_i - Segment_i_1) >= 2)
                        {
                            Result[Segment_i].Value = (pvalues[i].Value - limit[Segment_i - 1]) / diff * ts.TotalMilliseconds + Result[Segment_i].Value;
                            Result[Segment_i_1].Value = (limit[Segment_i_1] - pvalues[i + 1].Value) / diff * ts.TotalMilliseconds + Result[Segment_i_1].Value;

                            DateTime T1 = pvalues[i].Endtime + TimeSpan.FromSeconds((pvalues[i].Value - limit[Segment_i - 1]) / diff * T_interval.TotalMilliseconds);
                            PValue time1 = new PValue(0, T1, calcuinfo.fendtime, 2);
                            limittime[Segment_i - 1].Add(time1);


                            DateTime T2 = pvalues[i + 1].Timestamp - TimeSpan.FromSeconds((limit[Segment_i_1] - pvalues[i + 1].Value) / diff * T_interval.TotalMilliseconds);
                            PValue time2 = new PValue(0, T2, calcuinfo.fendtime, 2);
                            limittime[Segment_i_1].Add(time2);



                            for (int k = Segment_i_1 + 1; k < Segment_i; k++)
                            {
                                Result[k].Value = interval / diff * ts.TotalMilliseconds + Result[k].Value;
                                if (k < Segment_i - 1)
                                {
                                    DateTime T = limittime[k - 1][limittime[k - 1].Count - 1].Timestamp - TimeSpan.FromSeconds(interval / diff * T_interval.TotalMilliseconds);
                                    PValue time = new PValue(0, T, calcuinfo.fendtime, 2);
                                    limittime[k].Add(time);
                                }
                            }
                        }

                    }
                    #endregion
                    //Result[i].Value,记录各段时间统计
                    //limittime[i],记录各段时间序列
                    //
                }//end for
                #endregion
                                
                #region 组织输出                
                //22个分段，每个分段内的累加时长占比
                for (i = 0; i < Segment; i++)
                {
                    results[i] = new List<PValue>();
                    Result[i].Value = Result[i].Value * 100 / calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalMilliseconds;      //这里要除以总时间长度
                    results[i].Add(Result[i]);
                }
                //22个分段，21个分段点，信号穿越这些分段点的位置
                for (i = Segment; i < Segment * 2 - 1; i++)
                {
                    results[i] = new List<PValue>();
                    results[i] = limittime[i - Segment];

                }
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); 
                #endregion
            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuInfo.fmodulename, calcuInfo.sourcetagname, calcuinfo.fstarttime.ToString(), calcuinfo.fendtime.ToString());
                //LogHelper.Write(LogType.Error, moduleInfo);
                //计算引擎报错具体信息
                string errInfo = string.Format("——具体报错信息：{0}。", ex.ToString());
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
