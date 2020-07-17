using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                     //使用PValue
using Config;                       //使用log
using System.Linq;                  //list的select要使用

namespace PSLCalcu.Module
{
    /// <summary>
    /// 求多各输入变量中的最小值、时间、及其对应的变量序号
    /// ——输入是多个性质相同的变量的系列值。比如瓦温1到瓦温10,各自24个小时的实时值。比如机组1到机组4平均负荷，各自一个月每天的平均值。
    /// ——在所有变量的所有值中找出最小值及时间，最小值对应的变量序号
    /// 
    /// 版本：1.1
    ///    
    /// 修改纪录    
    /// 
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///     2018.01.09 arrow测试
    ///		2018.02.04 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.02.04</date>
    /// </author> 
    /// </summary>
    public class MFindMinInfo : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "FindMinInfo";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "求多各输入变量中的最小值、时间、及其对应的变量序号";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 0;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "最少两个，最多不限";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "FindMinInfo";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }        
        private int _outputNumber = 2;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "FMinValue;FMinIndex";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "最小值;最小值对应的标签序号(从0开始)";
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
        /// 计算模块算法实现:求多个输入中，所有值得最小值,对应的时间，对应变量的序号
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数:各值次开始时间</param>       
        /// <returns>最小值,对应的时间，对应变量的序号</returns>
       
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
            List<PValue>[] results = new List<PValue>[2];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //疑问，如果输入的值，在同一个点内部或者不同的点之间有相同的值，怎么处理
                //吴总答复：只找第一个点
                //本算法的输入值一般为概化数据，数量不会很大
                //2018.1.5与吴总沟通，由于值次表比较复杂，不再在算法中找值次。而是放到外部去找。
               

                //检查input，最少为两个输入
                //检查输入
                if (inputs.Length < 2)
                {
                    _errorFlag = true;
                    _errorInfo = "输入标签至少需要两个。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //处理截止时刻数据
                for (i = 0; i < inputs.Length; i++)
                { if (inputs[i].Count > 1) inputs[i].RemoveAt(inputs[i].Count - 1); }

                //1、准备所有值的统一数组series，值到变量序号的字典，值到时间的字典。
                Dictionary<double, int> valueIndex = new Dictionary<double, int>();
                Dictionary<double, DateTime> valueDate = new Dictionary<double, DateTime>();
                List<double> series = new List<double>();
                for (i = 0; i < inputs.Length; i++)
                {
                    for (int j = 0; j < inputs[i].Count; j++)
                    {
                        series.Add(inputs[i][j].Value);
                        if (!valueIndex.Keys.Contains(inputs[i][j].Value)) valueIndex.Add(inputs[i][j].Value, i);                      //一个value值，只能被添加一次。如果给出的数据中有重复的值，只有第一个会被添加
                        if (!valueDate.Keys.Contains(inputs[i][j].Value)) valueDate.Add(inputs[i][j].Value, inputs[i][j].Timestamp);   //一个value值，只能被添加一次。如果给出的数据中有重复的值，只有第一个会被添加
                        //基于以上添加方式，最大值恰好有多个相同的值时，只有第一个会被找到
                    }
                }
                //2、查找最小值
                var min = series.OrderBy(n => n).Take(1);       //升序排列，找第一个，即找最小
                var minlist = min.ToList<double>();

                //3、根据最小值确定时间、序号
                double minValue = minlist[0];
                double minIndex = valueIndex[minValue];
                DateTime minDate = valueDate[minValue];

                //组织输出
                results = new List<PValue>[2];
                results[0] = new List<PValue>();    //最大值，及时间
                results[1] = new List<PValue>();    //最大值对应序号 

                results[0].Add(new PValue(minValue, minDate, calcuinfo.fendtime, 0));
                results[1].Add(new PValue(minIndex, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

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
