using System;
using System.Collections.Generic;
using PCCommon;                         //使用PValue

namespace PSLCalcu.Module
{
    /// <summary>
    /// 对MCondSpan2计算结果在更长时间段内精确统计。
    /// ——MCondSpan2算法，是给定一个模拟量值，求出一段时间内，符合参数定义区间[a,b]的时间序列，并统计序列的总时长、总次数、最大值、最小值。
    ///   但是这个算法由于是面对实时数据，面对的数据周期仅能是小时或者天。不能直接用于周、月、年的计算。
    ///   同时对于周、月、年的总时长和次数计算，也不能直接用MCondSpan2的计算结果累加。这样不准确。尤其是次数。如果时段跨天，次数统计会发生错误。
    /// ——MCondSpanLong算法，是针对MCondSpan2的补充。利用MCondSpan2的时间序列结果，统计更长时间周期内的时间序列总时长和总次数。
    ///   这里主要是要把MCondSpan2在更长时间段内，那些上一个截止时间和下一个起始时间相同的时间段，按一个时间段来统计。
    /// 
    /// 可能存在的问题：
    /// ——在MLCondSpan2和MFCondSpan2中，如果加入了对无效数据段的处理能力，则MCondSpanLong算法无需考虑这一个问题。
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2018.01.13 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.01.13</date>
    /// </author> 
    /// </summary>
    public class MCondSpanLong:BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MCondSpanLong";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "对MCondSpan2计算结果在更长时间段内精确统计";
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
        private string _algorithms = "MCondSpanLong";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private int _outputNumber = 5;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "CSpanLN;CSpanLT;CSpanLMax;CSpanLMin;CspanLAvg";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "限定条件时间段总次数;限定条件时间段总时长(毫秒);限定条件时间段最大值(毫秒);限定条件时间段最小值(毫秒);限定条件时间段平均值(毫秒)";
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
        /// 计算模块算法实现:对MCondSpan2计算结果条件时间序列在更长时间段内的总时长和次数进行精确统计。
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>xian</returns>
      
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
            List<PValue>[] results = new List<PValue>[5];      //"限定条件时间段总次数;限定条件时间段总时长;限定条件时间段最大值;限定条件时间段最小值;限定条件时间段平均值"
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
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo); //如果输入为空，则主引擎已经报警
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法采用线性计算，截止时刻点要参与计算。不删除
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
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
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //1、将输入的时间序列组合成一个列表spanconcat
                //——在数字量变化的收尾添加si和ei的标志。                
                List<SpanSeries> spanconcat = new List<SpanSeries>();
                
                for (i = input.Count-1; i > 0 ; i--)
                {
                    if (input[i].Timestamp == input[i - 1].Endtime)
                    {                       
                        input[i].Timestamp = input[i - 1].Timestamp;
                        input[i].Value = input[i - 1].Value + input[i].Value;
                        input.RemoveAt(i - 1);
                    }
                    
                }
               
                double totalspan = 0;
                double max = input[0].Value;
                DateTime maxdate = input[0].Timestamp;
                double min = input[0].Value;
                DateTime mindate = input[0].Timestamp;
                for (i = 0; i < input.Count; i++) 
                {
                    if (max < input[i].Value)
                    {
                        max = input[i].Value;
                        maxdate = input[i].Timestamp;
                    }
                    if (min > input[i].Value)
                    {
                        min = input[i].Value;
                        mindate = input[i].Timestamp;
                    }
                    totalspan = totalspan + input[i].Value;
                } 

                //总次数调整，通常总次数就是合并后的时间段数量
                //但是应考虑特别情况，就是整个时间段序列第一个时间段，有可能是周期切分时，由上一个周期延续而来。
                //采用的判断方法是，如果该时间段的时间戳是0：00：00 000，则认为该时间段是切割而来。
                //如果该时间段的时间戳不是0：00：00 000，则认为该时间段是独立时间段
                //例如对天荒坪项目的PO抽水时间段统计，所有抽水时间段都是跨午夜的。
                //无论是按天统计，还是按月统计，总数都需要减掉时间戳恰好为0：00：00 000的第一个时间段
                double totalnumber = 0;
                if (input.Count != 0 &&
                    input[0].Timestamp.Hour == calcuinfo.fstarttime.Hour &&
                    input[0].Timestamp.Minute == calcuinfo.fstarttime.Minute &&
                    input[0].Timestamp.Second == calcuinfo.fstarttime.Second &&
                    input[0].Timestamp.Millisecond == calcuinfo.fstarttime.Millisecond
                    )
                {
                    totalnumber = input.Count - 1;
                }
                else
                {
                    totalnumber = input.Count;
                }
                //2、组织计算解结果
                results = new List<PValue>[5];
                //组织计算结果:共返回3个统计结果
                //置位次数;置位时长;置位时长占比
                results[0] = new List<PValue>();
                results[0].Add(new PValue(totalnumber, calcuinfo.fstarttime, calcuinfo.fendtime, 0));         //总次数
                results[1] = new List<PValue>();
                results[1].Add(new PValue(totalspan, calcuinfo.fstarttime, calcuinfo.fendtime, 0));     //总时长
                results[2] = new List<PValue>();
                results[2].Add(new PValue(max, maxdate, calcuinfo.fendtime, 0));     //最大值
                results[3] = new List<PValue>();
                results[3].Add(new PValue(min, mindate, calcuinfo.fendtime, 0));     //最小值
                results[4] = new List<PValue>();
                results[4].Add(new PValue(totalspan / input.Count, mindate, calcuinfo.fendtime, 0));     //平均值

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
    }
}
