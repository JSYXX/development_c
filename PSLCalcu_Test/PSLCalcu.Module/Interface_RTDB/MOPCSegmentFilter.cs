using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using System.Linq;                      //使用list的orderby

//using System.Diagnostics;               //使用计时器进行测试

namespace PSLCalcu.Module
{
    /// <summary>
    /// (阶梯曲线)OPC数据（或者实时数据）按指定的秒数（具体由参数设置）分割再求均值
    /// 将从OPC数据表opcdataxxxx中读取的实时数据（或从实时数据库读取的数据），按分钟分割然后再求均值
    /// ——OPC数据与实时数据相同，均视为阶梯型曲线。起止时刻和截止时刻值，均用前值。
    /// ——本算法处理的数据是用OPCDAO.Read取得的OPC数据，或者用RTDBDAO.Read（）取得的实时数据，两种情况下，起止时刻点的数值均使用前值。
    /// ——本算法输入标签为要抽取数据的标签，参数标签为计算结果标签。
    /// ——本算法本计算周期的计算结果，要将最后一个结果的值覆盖到下一个周期：即要额外保存一个值，该值的起始时间为当前周期的下一个周期的起始时间，该值得结束周期为下一个周期的结束时间。
    /// ——按照上述得到的数据，按阶梯曲线求每个分割时间段上的均值
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2018.02.06 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.02.06</date>
    /// </author> 
    /// </summary>
    public class MOPCSegmentFilter : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MOPCSegmentFilter";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "(线性曲线)OPC（或实时）数据先分割再求带滤波均值。";
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
        private string _algorithms = "MOPCSegmentFilter";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }

        private string _algorithmsflag = "Y";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "60;0;1;0;L";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "分割数据间隔(秒);二次系数;一次系数；平移系数;用上下限进行过滤的标志;";
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        //正则表达式，用来检测计算的参数，是否符合要求
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){3}(;L){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 1;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "FOPCSegmentFilter";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "OPC(或实时)数据分割滤波均值";
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
        /// 计算模块算法实现:(阶梯曲线)OPC数据读取成分钟格式
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志。本算法无效</param>
        /// <param name="calcuinfo.fparas"></param>       
        /// <returns>OPC分钟数据</returns>       
        public static Results Calcu()
        {
            return Calcu(_inputData, _calcuInfo);
        }
        public static Results Calcu(List<PValue>[] inputs, CalcuInfo calcuinfo)
        {
            //var sw1 = Stopwatch.StartNew();
            
            //公用变量
            bool _errorFlag = false;
            string _errorInfo = "";
            bool _warningFlag = false;
            string _warningInfo = "";
            bool _fatalFlag = false;
            string _fatalInfo = "";

            int i;
            
            //0输出初始化:输入为空或者计算出错时，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[1];
            
            try
            {
                //本算法特殊，需要先处理参数
                string[] para = calcuinfo.fparas.Split(new char[] { ';' });
                int intervalseconds = int.Parse(para[0]);           //分割时长，单位秒
                double A = double.Parse(para[1]);
                double B = double.Parse(para[2]);
                double C = double.Parse(para[3]);
                string FilterWithLimit = "";
                if (para.Length > 4)
                    FilterWithLimit = para[4].ToString();           //是否用上下限进行过滤，L是过滤
               

                //数据分割
                int spanNumber = (int)(calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalSeconds / intervalseconds);

                //准备输出结果
                results[0] = new List<PValue>();
                for (i = 0; i < spanNumber; i++)
                {
                    results[0].Add(new PValue(0, calcuinfo.fstarttime.AddSeconds(i * intervalseconds), calcuinfo.fstarttime.AddSeconds((i + 1) * intervalseconds), (long)StatusConst.InputIsNull));
                }

                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，给出标志位为StatusConst.InputIsNull的计算结果.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    //如果输入进来的实时数据有空，计算引擎在外部会报错 
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法，截止时刻点不需要参与计算，要删除。
                //本算法在下面专门处理
                //if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                //本算法在下面专门处理
                //for (i = input.Count - 1; i >= 0; i--)
                //{
                //   if (input[i].Status != 0) input.RemoveAt(i);
                //}
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回全
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "源数据不满足要求，数据数量不够。源数据至少要包括一个起始时刻有效数据和一个截止时刻数据。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }               

                //对数据进行分割
                List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
                pvaluesSpan = SpanPValues4rtdbStep20190317(input, calcuinfo.fstarttime, calcuinfo.fendtime, intervalseconds);    //对数据进行分割

                //特别注意，正常情况下，每个分割段内必有数据。
                //——比如，取了一个小时的数据，分割成每分钟，那每分钟内就应该有数据，只不过状态值不一定是0。比如中间如果有一个数据状态位不为0，则在该数据有效的分割端内，数据状态位均不为0
                //——但是，在缺少起始时刻数据时，会造成前几个分割段内返回的是null。此时，计算结果不能没有，而必须有有效值，只不过状态位不为0。另外要给出报错！

                List<PValue> result = new List<PValue>();       //进行计算时，先暂时向result中添加，如果计算到最后不出错，才把result的值给results。如果出错还需要直接返回results
                //对分割数据进行计算
                for (int j = 0; j < spanNumber; j++)
                {
                    List<double> pvs = new List<double>();
                    //处理输入点
                    //——对于滤波算法，不关心结束时刻值，因此忽略数据input[input.Count-1]。
                    //——剔除坏质量点。
                    if (pvaluesSpan[j] == null || pvaluesSpan[j].Count ==0)
                    {
                        _errorFlag = true;
                        _errorInfo = _errorInfo + 
                                    Environment.NewLine + 
                                    String.Format("分段数据，经分割后，{0}到{1}的数据为空。正常情况下，分段数据不应该出现count<2的情况，请检查！这里一般是缺少起始时刻数据导致。", calcuinfo.fstarttime.AddSeconds(j * intervalseconds).ToString(), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds).ToString());
                        result.Add(new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), (long)StatusConst.InputIsNull));
                        continue;
                        //这里不返回，添加当前时间段的计算结果，置数据为空的标志，然后直接进行下面的点
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                    }
                    for (i = 0; i < pvaluesSpan[j].Count-1; i++)
                    {
                        if (pvaluesSpan[j][i]!=null && pvaluesSpan[j][i].Status == 0)
                        {
                            pvs.Add(pvaluesSpan[j][i].Value);
                        }
                    }

                    //添加后，有可能pvs为null 或者count=0。那么该时间段要报错
                    if (pvs == null || pvs.Count ==0)
                    {
                        //正常情况下，只要上面检查
                        _errorFlag = true;
                        _errorInfo = _errorInfo +
                                     Environment.NewLine +
                                     String.Format("分段数据，经坏质量过滤后，{0}到{1}的数据为空。正常情况下，分段数据不应该出现count<2的情况，请检查！", calcuinfo.fstarttime.AddSeconds(j * intervalseconds).ToString(), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds).ToString());
                        result.Add(new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), (long)StatusConst.InputIsNull));
                        continue;
                        //这里不返回，添加当前时间段的计算结果，置数据为空的标志，然后直接进行下面的点
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                    }
                    //排序：由小到大
                    pvs.Sort();
                    //过滤
                    if (pvs.Count > 20)
                    {
                        if (pvs.Count % 2 == 0)
                        {
                            int delNumber = (pvs.Count - 16) / 2;
                            pvs.RemoveRange(pvs.Count - delNumber, delNumber);
                            pvs.RemoveRange(0, delNumber);
                        }
                        else
                        {
                            int delNumber = (pvs.Count - 15) / 2;
                            pvs.RemoveRange(pvs.Count - delNumber, delNumber);
                            pvs.RemoveRange(0, delNumber);
                        }
                    }
                    else if (pvs.Count > 12)
                    {
                        pvs.RemoveRange(pvs.Count - 3, 3);
                        pvs.RemoveRange(0, 3);
                    }
                    else if (pvs.Count > 6)
                    {
                        pvs.RemoveRange(pvs.Count - 2, 2);
                        pvs.RemoveRange(0, 2);
                    }
                    else if (pvs.Count > 2)
                    {
                        pvs.RemoveRange(pvs.Count - 1, 1);
                        pvs.RemoveRange(0, 1);
                    }

                    //结果处理。在
                    if (pvs.Count == 0)
                    {
                        //不会出现这种情况
                        //result.Add(new PValue(0, calcuinfo.fstarttime.AddSeconds(i * intervalseconds), calcuinfo.fstarttime.AddSeconds((i + 1) * intervalseconds), (long)StatusConst.InputIsNull));         //返回故障数据
                    }
                    else
                    {
                        double sum = 0;
                        for (i = 0; i < pvs.Count; i++)
                        {
                            sum = sum + pvs[i];
                        }
                        double avg = sum / pvs.Count;                       //对剩下的值求平均
                        double filtervalue = A * avg * avg + B * avg + C;   //对平均值进行变换。注意一般情况是A=0；B=1；B=0；D=0，即filtervalue=ave

                        //如果参数中含有第四个变量，且变量值为L，则对计算结果用上下限进行过滤
                        if (FilterWithLimit == "L")
                        {
                            if (filtervalue < calcuinfo.sourcetagmrb)
                                result.Add(new PValue(filtervalue, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), (long)StatusConst.InputOverLimit));
                            else if (filtervalue > calcuinfo.sourcetagmre)
                                result.Add(new PValue(filtervalue, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), (long)StatusConst.InputOverLimit));
                            else
                                result.Add(new PValue(filtervalue, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0));
                        }
                        else//没有参数L，不过滤
                        {
                            result.Add(new PValue(filtervalue, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0));
                        }                        
                    }

                }//endfor

                //Debug.WriteLine("----ParallelReadRealData task " + i + " complete:" + sw1.Elapsed.ToString());

                //组织计算结果
                results = new List<PValue>[1];
                results[0] = result;
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuInfo.fmodulename, calcuInfo.sourcetagname,calcuinfo.fstarttime.ToString(),calcuinfo.fendtime.ToString());
                //LogHelper.Write(LogType.Error, moduleInfo);
                //计算引擎报错具体信息
                //string errInfo=string.Format("——具体报错信息：{0}。", ex.ToString());
                //LogHelper.Write(LogType.Error, errInfo);
                //返回null供计算引擎处理

                _fatalFlag = true;
                _fatalInfo = ex.ToString();
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }

        }//end module
        #endregion

        #region 辅助函数，此处辅助函数借用并发计算引擎辅助函数，如果该算法更新，请两处保持一致
        //计算标签数据分割，针对实时数据，将实时数据视为离散阶梯型，有截止时刻值。分割的结果也有截止时刻值。特别注意rtdb和opc均用这种方式分割。opc数据视为实时数据。
        private static List<PValue>[] SpanPValues4rtdbStep(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，只有两种结果，第一种至少有两个值，即至少有一个起始时刻有效数据点和一个截止时刻值。另一种结果就是null。
            //——不为空的情况下pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——不为空的情况下pvalue的各个数据点起始时间和截止时间一定前后相连！！！
            //——在具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐。如果不对齐，需要对数据进行插值。这个插值与实时数据库对起始时刻和截止时刻插值相同。

            //*************请使用主界面的“并行时间分割spanPValues4rtdbToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);      //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。

            //********仅用于测试*********
            //List<string> resultsStr = new List<string>();
            //**************************

            if (pvalues != null && pvalues.Count != 0)              //如果数据源整体为空，则直接走下面的分支，返回每个分段均为null的数据。否则每个分段必有数据
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());        //分隔点   
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点，即起始时候向后第一个间隔位置
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果。只要在读取数据的时候不出错。pvaluesSpan必然不会整体为空。可以所有的分段为空。

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                //每一个点分三种情况处理：
                //——第一种情况，当前点pvalues[ipoint]没有超过下一个分割位置
                //——第二种情况，当前点pvalues[ipoint]刚好在下一个分割位置
                //——第三种情况，当前点pvalues[ipoint]超过了下一个分割位置
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当前点的时刻小于下一个分隔时刻时，应该将该点计入该段
                        //——首先要判断该段是否开始
                        //——如果该段已经开始（即已经有点，count！=0），则修改上一个点的结束时刻，再添加当前点
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0) pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint].Timestamp;
                        //——如果该段未开始（还没有点，count=0），则说明当前点是开始点，直接添加。（开始点，由于做了初始化 pvaluesSpan[spanIndex] = new List<PValue>()，所以pvaluesSpan不再为空，而是count=0）
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 

                            //resultsStr.Add("End--:" + pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString());        //测试
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空。实际上这种情况不会出现
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)                    //最后一个点是原数据的截止时刻点，如果已经到截止时刻点，就不再添加新段
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //当前点超出下一个分割时刻位置，该情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                        {
                            //当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去找结束点。                            
                            //——首先，修改当前时间段最后一个有效数据的结束位置为当前分割位置。
                            //——然后，应该根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。                            
                            //——最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，并用上一个时刻点的值作为起始点值。
                            //——回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                            PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                            //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                            //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                            //else                                                                                                                
                            lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                            pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                            //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                            //准备新一时间段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, lastpvalue.Status));

                            //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }



                    }//结束三种情况分支

                }//结束for
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点                
                return pvaluesSpan;
            }
            else
            {
                //如果数据源整体为空，则直接返回每个分段均为null的数据
                return pvaluesSpan;         //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空           
            }
        }
        private static List<PValue>[] SpanPValues4rtdbStep20190317(List<PValue> pvalues, DateTime startdate, DateTime enddate, int intervalseconds)
        {
            //该pvalues有截止时刻点。结算结果每一个小段也有截止时刻值。

            //将实时数据分隔成 1days/intervalseconds段
            //——注意经rtdbhelper.GetRawValues()读出的数据，只有两种结果，第一种至少有两个值，即至少有一个起始时刻有效数据点和一个截止时刻值。另一种结果就是null。
            //——不为空的情况下pvalue的第一点起始时间，和最后一点的结束时间，会与入参startdate和enddate对齐。
            //——不为空的情况下pvalue的各个数据点起始时间和截止时间一定前后相连！！！
            //——在具体分割时，每一段内的数据点，也要和段的起始时间和结束时间对齐。如果不对齐，需要对数据进行插值。这个插值与实时数据库对起始时刻和截止时刻插值相同。

            //*************请使用主界面的“并行时间分割spanPValues4rtdbToolStripMenuItem_Click()”方法对此算法进行测试
            //编程要点：任何分支情况下，移动当前时间段spanIndex，和移动分割时间isolatedata，要同时进行

            int spanNumber = (int)(enddate.Subtract(startdate).TotalSeconds / intervalseconds);      //一旦起始时刻和截止时刻确定，间隔时间确。分割段数就是确定的。
            List<PValue>[] pvaluesSpan = new List<PValue>[spanNumber];
            //分段的结果也是确定的。不管有么有pvalues数据，或pvalues数据如何，pvaluesSpan都有固定的段数。
            //——初始化后每个pvaluesSpan[ipoint]均为null，如果执行pvaluesSpan[spanIndex] = new List<PValue>()后，pvaluesSpan[ipoint]不为空，而是count==0
            //——如果有数据落入到pvaluesSpan[ipoint]中，pvaluesSpan[ipoint]中就有时间段。
            //——如果没有数据落入到pvaluesSpan[ipoint]中，该段pvaluesSpan[ipoint]最终就为空。最终返回的分段数据，允许某些段确实为空。没有有效时间段。
            //——如果pvalues没有数据，则直接返回每段都为空的pvaluesSpan[ipoint]。

            //********仅用于测试*********
            //List<string> resultsStr = new List<string>();
            //**************************

            if (pvalues != null && pvalues.Count != 0)              //如果数据源整体为空，则直接走下面的分支，返回每个分段均为null的数据。否则每个分段必有数据
            {
                DateTime isolatedata = DateTime.Parse(startdate.ToString());        //分隔点   
                isolatedata = isolatedata.AddSeconds(intervalseconds);              //初始化为第一分割点，即起始时候向后第一个间隔位置
                int spanIndex = 0;                                                  //分段指针，第0段
                int ipoint = 0;                                                     //被分割数据指针

                //对第0段进行初始化，初始化后pvaluesSpan[spanIndex]就不再为null，而是count==0
                pvaluesSpan[spanIndex] = new List<PValue>();                        //对于实时数据的分段结果。只要在读取数据的时候不出错。pvaluesSpan必然不会整体为空。可以所有的分段为空。

                //循环处理从0到Count-1点
                for (ipoint = 0; ipoint < pvalues.Count; ipoint++)
                {
                //每一个点分三种情况处理：
                //——第一种情况，当前点pvalues[ipoint]没有超过下一个分割位置
                //——第二种情况，当前点pvalues[ipoint]刚好在下一个分割位置
                //——第三种情况，当前点pvalues[ipoint]超过了下一个分割位置
                currentPoint:
                    if (pvalues[ipoint].Timestamp < isolatedata)
                    {
                        //当前点的时刻小于下一个分隔时刻时，应该将该点计入该段
                        //——首先要判断该段是否开始
                        //——如果该段已经开始（即已经有点，count！=0），则修改上一个点的结束时刻，再添加当前点
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                            pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;
                        //——如果该段未开始（还没有点，count=0），则说明当前点是开始点，直接添加。（开始点，由于做了初始化 pvaluesSpan[spanIndex] = new List<PValue>()，所以pvaluesSpan不再为空，而是count=0）
                        pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));

                        //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试
                    }
                    else if (pvalues[ipoint].Timestamp == isolatedata)
                    {
                        //当前点恰好是边界点的情况，这种情况比较简单                            
                        //如果当前点是边界点，说明上一段最后一个点的结束时间正好在边界上。此时，只需添加当前段截止时刻点
                        //——解释，按照最新的标准，读取周期数据的最后一个数据，必须是Timestamp和endtime都为截止时刻的点。
                        //——比如，计算周期为2016-01-01 00:00 到2016-01-01 01:00，最后时刻的点必须为(Value值,2016-01-01 01:00：00,2016-01-01 01:00：00,1)。该点在阶梯计算中舍弃，在线性计算中有用
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)  //在找到边界的情况下，如果当前段已经有数，则添加结束，否则添加null
                        {
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));    //注意这里不能直接Add(pvalues[ipoint])，要修改endtime值 

                            //resultsStr.Add("End--:" + pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString());        //测试
                        }
                        else
                        {
                            pvaluesSpan[spanIndex] = null;      //在找到边界的情况下，如果当前段没有数据，则不应该添加结束，当前段应该为空。实际上这种情况不会出现
                        }
                        //创建新段。如果没有到最后一个点，则创建新段。否则就退出循环
                        if (ipoint != pvalues.Count - 1)                    //最后一个点是原数据的截止时刻点，如果已经到截止时刻点，就不再添加新段
                        {
                            spanIndex = spanIndex + 1;                      //创建新段
                            pvaluesSpan[spanIndex] = new List<PValue>();
                            pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Endtime, pvalues[ipoint].Status));       //给新段，添加开始点。用当前点加入下一段段初    

                            //resultsStr.Add(pvalues[ipoint].Timestamp.ToString("yyyy/MM/dd HH:mm:ss") + "--" + pvalues[ipoint].Value.ToString() + "--" + spanIndex.ToString());        //测试

                            isolatedata = isolatedata.AddSeconds(intervalseconds);
                        }
                    }
                    else
                    {
                        //pvalues[ipoint].Timestamp > isolatedata 
                        //当前点超出下一个分割时刻位置，该情况比较复杂
                        //——当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去超结束点
                        //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                        if (pvaluesSpan[spanIndex] != null && pvaluesSpan[spanIndex].Count != 0)
                        {
                            //当pvaluesSpan[spanIndex].Count不为0时，说明当前数据段已经有了起始点，这时应该去找结束点。                            
                            //——首先，修改当前时间段最后一个有效数据的结束位置：这里又分为两种情况：
                            //————如果上一个点结束位置大于isolatedata：
                            //——————在isolatedata结束上一个段最后一个数据。
                            //——————然后，根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。                            
                            //——————最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，并用上一个时刻点的值作为起始点值。
                            //——————回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            //————如果上一个点结束位置小于等于isolatedata：
                            //——————在pvalues[ipoint - 1].Endtime结束上一个段最后一个数据。
                            //——————然后，根据isolatedata时刻，用上一个实时点作为截止时刻点，即timestamp和enddate都一致的结束点。
                            //——————最后，上一段结束，意味着新一时间段开始。isolatedata自增，创建新时间段，但是不添加新点
                            //——————回到循环的当前点ipoint，继续看当前点与下一个分割时刻点的关系。
                            if (pvalues[ipoint - 1].Endtime > isolatedata)
                            {
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = isolatedata;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                                PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                                //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                                //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                                //else                                                                                                                
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                                pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                                //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                                //准备新一时间段数据
                                spanIndex = spanIndex + 1;
                                pvaluesSpan[spanIndex] = new List<PValue>();
                                pvaluesSpan[spanIndex].Add(new PValue(lastpvalue.Value, lastpvalue.Timestamp, pvalues[ipoint].Timestamp, lastpvalue.Status));

                                //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                                //调到新的边界时刻，对当前点再次进行判断
                                isolatedata = isolatedata.AddSeconds(intervalseconds);

                                //回到当前点
                                goto currentPoint;
                            }
                            else
                            {
                                //pvalues[ipoint - 1].Endtime < isolatedata
                                pvaluesSpan[spanIndex][pvaluesSpan[spanIndex].Count - 1].Endtime = pvalues[ipoint - 1].Endtime;                                 //首先，当前时间段最后一个有效数据，修正结束时刻
                                PValue lastpvalue;                                                                                              //然后，计算间隔位置插值
                                //if (InterploationTerminal)                                                                                          //在离散阶梯型曲线下，无论端点是否插值，均使用上一个点作为端点值
                                //    lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);                                  
                                //else                                                                                                                
                                lastpvalue = new PValue(pvalues[ipoint - 1].Value, isolatedata, isolatedata, pvalues[ipoint - 1].Status);

                                pvaluesSpan[spanIndex].Add(lastpvalue);     //给上当前时间段添加截止时刻值

                                //resultsStr.Add("End--" + isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString());        //测试
                                //准备新一时间段数据
                                spanIndex = spanIndex + 1;
                                pvaluesSpan[spanIndex] = new List<PValue>();
                                //pvaluesSpan[spanIndex].Add(new PValue(pvalues[ipoint].Value, pvalues[ipoint].Timestamp, pvalues[ipoint].Timestamp, pvalues[ipoint].Status));

                                //resultsStr.Add(isolatedata.ToString("yyyy/MM/dd HH:mm:ss") + "--" + lastpvalue.Value.ToString() + "--" + spanIndex.ToString());        //测试
                                //调到新的边界时刻，跳过当前点
                                isolatedata = isolatedata.AddSeconds(intervalseconds);

                                //回到当前点
                                goto currentPoint;
                            }

                        }
                        else
                        {
                            //——当pvaluesSpan[spanIndex].Count为0时，说明当前数据段还未找到起始点。这种情况通常发生在pvalues的第一个值的Timestamp就大于isolatedata的情况下。
                            //——直接到到下一个isolatedata。并且当前分段置为空。
                            pvaluesSpan[spanIndex] = null;
                            //准备新一段数据
                            spanIndex = spanIndex + 1;
                            pvaluesSpan[spanIndex] = new List<PValue>();

                            //调到新的边界时刻，对当前点再次进行判断
                            isolatedata = isolatedata.AddSeconds(intervalseconds);

                            //回到当前点
                            goto currentPoint;
                        }



                    }//结束三种情况分支

                }//结束for
                //添加截止时刻值，用原pvalues的截止时刻作为最后一个分段的截止时刻
                //实时数据（rtdb、opc），概化数据取数据接口，最后一个数据是timestamp为enddate时刻的数值，该PValue的value值用enddate时刻前一个值和后一个值插值而的，其timestamp和endtime均为entdate时刻
                //因为pvalues最后一个点为截止时刻值，会走分支pvalues[ipoint].Timestamp == isolatedata，在该分支中添加最后一段的点                
                return pvaluesSpan;
            }
            else
            {
                //如果数据源整体为空，则直接返回每个分段均为null的数据
                return pvaluesSpan;         //此时pvaluesSpan = new List<PValue>[spanNumber]; ，刚好每一段均为空           
            }
        }
        
        #endregion

    }
}
