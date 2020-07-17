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
    /// 质量统计      
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
    public class MQuality : BaseModule, IModule
    {
        #region 计算模块信息：模块名称质量统计Quality、包含的算法、输出项个数5、输出项名称、输出项写入的数据表名称
        //输出分别为质量总点数Qua_TotalPoint;坏质量点数Qua_BadPoint;坏质量总时段Qua_Badtimespan;坏质量点数占比Qua_Badpointtratio;坏质量时段占比Qua_Badtimeratio
        private string _moduleName = "Quality";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "质量统计";
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
        private string _algorithms = "Qua_TotalPoint;Qua_BadPoint;Qua_Badtimespan;Qua_Badpointtratio;Qua_Badtimeratio";
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
        private string _outputDescs = "Qua_TotalPoint;Qua_BadPoint;Qua_Badtimespan;Qua_Badpointtratio;Qua_Badtimeratio";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "总点数;坏点数量;坏点时间;坏点数量比;坏点时间比";
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

        #region 计算模块
        /// <summary>
        /// 计算模块算法实现:求限定时间质量统计Quality
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara"></param>       
        /// <returns>输出分别为质量总点数Qua_TotalPoint;坏质量点数Qua_BadPoint;坏质量总时段Qua_Badtimespan;坏质量点数占比Qua_Badpointtratio;坏质量时段占比Qua_Badtimeratio</returns>
        
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
                List<PValue> input = inputs[0];

                //处理截止时刻数据
                if (input.Count > 1) input.RemoveAt(input.Count - 1);

                #region 初始化
                //质量总点数
                PValue Qua_TotalPoint = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                //坏质量点数
                PValue Qua_BadPoint = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                //坏质量总时段
                PValue Qua_Badtimespan = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                //坏质量点数占比
                PValue Qua_Badpointtratio = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                //坏质量时段占比
                PValue Qua_Badtimeratio = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);


                List<PValue>[] results = new List<PValue>[5];

                int len = input.Count;
                double ts = 0;
                #endregion


                Qua_TotalPoint.Value = len;
                for (i = 0; i < len; i++)
                {
                    ts = ts + input[i].Timespan;
                    Qua_Badtimeratio.Value = Qua_Badtimeratio.Value + input[i].Timespan;
                    if (input[i].Status == 0)
                    {
                        Qua_BadPoint.Value = Qua_BadPoint.Value + 1; //统计坏质量点数
                        Qua_Badtimespan.Value = input[i].Timespan + Qua_Badtimespan.Value;//坏质量时段

                    }
                }
                
                Qua_Badpointtratio.Value = Qua_BadPoint.Value / len;//坏质量个数占比
                Qua_Badtimeratio.Value = Qua_Badtimespan.Value / ts;  //坏质量时段占比

                #region 组织输出
                results[0] = new List<PValue>();
                results[0].Add(Qua_TotalPoint);

                results[1] = new List<PValue>();
                results[1].Add(Qua_BadPoint);

                results[2] = new List<PValue>();
                results[2].Add(Qua_Badtimespan);

                results[3] = new List<PValue>();
                results[3].Add(Qua_Badpointtratio);

                results[4] = new List<PValue>();
                results[4].Add(Qua_Badtimeratio);

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
