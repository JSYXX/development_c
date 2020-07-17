﻿using System;
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
    /// 快速最大最小极差值算术和平均值   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2017.03.17 版本：1.0 wujunbo 创建。    
    /// <author>
    ///		<name>wujunbo</name>
    ///		<date>2017.03.17</date>
    /// </author> 
    /// </summary>
    public class MFastMaxMinRange : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "FastMaxMinRange";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "快速最大最小极差值算术和平均值";
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
        private string _algorithms = "FMax;FMin;FRange;FSum;FAvg";
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
        private string _outputDescs = "FMax;FMin;FRange;FSum;FAvg";   
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "最大值（快速）;最小值（快速）;极差（快速）;算术和（快速）;算术平均值（快速）";
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
        /// 计算模块算法实现:求限定时间内最大最小和极差值
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>计算结果分别为最大值、最小值、极差值</returns>       
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
            List<PValue>[] results = new List<PValue>[5];   //最大值（快速）;最小值（快速）;极差（快速）;算术和（快速）;算术平均值（快速）
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null)
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                else
                    input = inputs[0];
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。 
                if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                for (i = input.Count - 1; i >= 0; i--)
                {
                    if (input[i].Status != 0) input.RemoveAt(i);
                }
                //0.4、输入处理：过滤后结果。
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回null
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                if (input.Count < 1)
                {
                    _warningFlag = true;
                    _warningInfo = "对应时间段内的源数据状态位全部异常。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }  
                
                //初始化
                PValue Max = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Min = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Range = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Sum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Avg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
             
                int len = input.Count;
                TimeSpan ts = calcuinfo.fendtime - calcuinfo.fstarttime;  

                //最大值的初始化
                Max.Value = input[0].Value;
                Max.Timestamp = input[0].Timestamp;

                //最小值的初始化
                Min.Value = input[0].Value;
                Min.Timestamp = input[0].Timestamp;

                //最大差值的初始化
                Range.Value = 0;

                //和初始化
                Sum.Value = 0;
                
                //均值初始化
                Avg.Value = 0;

                //计算
                for (i = 0; i < len; i++)
                {
                    #region 寻找最大值
                    //寻找最大值
                    if (input[i].Value > Max.Value)
                    {
                        Max.Value = input[i].Value;
                        Max.Timestamp = input[i].Timestamp;
                    }
                    #endregion

                    #region 寻找最小值
                    //寻找最小值
                    if (input[i].Value < Min.Value)
                    {
                        Min.Value = input[i].Value;
                        Min.Timestamp = input[i].Timestamp;
                    }
                    #endregion

                    #region 计算确定时间段的极差
                    //   功能：计算确定时间段的极差，定义为时间段内最大值与最小值的差
                    //   重要中间变量：
                    //   编程笔记：计算结果保存在Range.Value

                    Range.Value = Max.Value - Min.Value;
                    #endregion

                    Sum.Value = Sum.Value + input[i].Value;

                }
                Avg.Value = Sum.Value / input.Count;

                //组织输出结果
                //最大值
                results[0] = new List<PValue>();
                results[0].Add(Max);

                //最小值
                results[1] = new List<PValue>();
                results[1].Add(Min);

                //极差值
                results[2] = new List<PValue>();
                results[2].Add(Range);

                //和
                results[3] = new List<PValue>();
                results[3].Add(Sum);

                //均值
                results[4] = new List<PValue>();
                results[4].Add(Avg);

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
