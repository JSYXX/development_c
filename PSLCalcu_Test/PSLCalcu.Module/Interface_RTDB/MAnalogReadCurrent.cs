using System;
using System.Collections.Generic;
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 读取模拟量的时刻值，保存到概化库   
    /// ——读模拟量某一限定时刻的值，由于计算引擎，只能是按照周期来读取实时值。
    /// ——因此，想取到某一时刻的值，则必须通过限定条件condpsl。
    /// ——比如，要读取一小时中温度最大值，对应的机组负荷值。
    /// ——应该先求出一小时温度最大值。该PValue的结果中，Value代表最大值，Timestamp代表最大值对应的时刻，endtime代表最大值对应时间段的结束时刻
    /// ——把一小时温度最大值，作为时间限定条件，给到MAnalogReadCurrent的condpsl中，
    /// ——把一小时的机组负荷值，作为输入标签。
    /// ——计算引擎就会根据温度最大值对应的时间段去过滤机组负荷，过滤后其起点时刻，就是最大值时刻。
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///     2018.01.09 arrow测试
    ///		2018.01.08 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.01.08</date>
    /// </author> 
    /// </summary>
    public class MAnalogReadCurrent : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MAnalogReadCurrent";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "读取模拟量的时刻值";
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
        private string _algorithms = "MAnalogReadCurrent";
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

        private int _outputNumber = 1;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "ANGCurrent";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "模拟量的时刻值";
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
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数:备选输出量</param>       
        /// <returns></returns>

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

            try
            {
                _errorFlag = false;
                _errorInfo = "";

                //该方法不需要去掉截止时刻值
               
                //1、比如要求最大值时刻对应的机组负荷，用最大值去过滤机组负荷，得到的机组负荷的第一个值，就是最大值时刻值
                PValue result = new PValue(inputs[0][0].Value ,inputs[0][0].Timestamp,calcuInfo.fendtime,0);
                                      

                //组织计算结果
                List<PValue>[] results = new List<PValue>[1];
                results[0] = new List<PValue>();
                results[0].Add(result);
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
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
        #endregion
    }
}
