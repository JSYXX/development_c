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
    /// 速度信息统计值      
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
    public class MVeloctiy : BaseModule, IModule
    {
        #region 计算模块信息：模块名称速度信息统计Veloctiy、包含的算法正向最大速度FowardMaxV；负向最大速度NegativeMaxV；最大代数速度ArithmeticMaxV；最小代数速度ArithmeticMin；平均速度MeanV、输出项个数5、输出项名称FowardMaxV；NegativeMaxV；ArithmeticMaxV；ArithmeticMin；MeanV、输出项写入的数据表名称plsdata
        //模块名称速度信息统计Veloctiy
        //包含的算法正向最大速度FowardMaxV；负向最大速度NegativeMaxV；最大代数速度ArithmeticMaxV；最小代数速度ArithmeticMin；平均速度MeanV
        //输出项名称FowardMaxV；NegativeMaxV；ArithmeticMaxV；ArithmeticMin；MeanV、
        //输出项个数5
        //输出项写入的数据表名称plsdata
        private string _moduleName = "Veloctiy";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "速度信息统计值";
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
        private string _algorithms = "FowardMaxV;NegativeMaxV;ArithmeticMaxV;ArithmeticMin;MeanV";
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
        private string _outputDescs = "FowardMaxV;NegativeMaxV;ArithmeticMaxV;ArithmeticMin;MeanV";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "正向最大速度;负向最大速度;最大代数速度;最小代数速度;平均速度";
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
        /// 计算模块算法实现:模块名称速度信息统计Veloctiy、包含的算法正向最大速度FowardMaxV；负向最大速度NegativeMaxV；最大代数速度ArithmeticMaxV；最小代数速度ArithmeticMin；平均速度MeanV
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">计算所需参数</param>       
        /// <returns>计算结果分别为正向最大速度FowardMaxV；负向最大速度NegativeMaxV；最大代数速度ArithmeticMaxV；最小代数速度ArithmeticMin；平均速度MeanV</returns>
        /// 
       
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
                PValue Veloresult = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);       //暂存计算变量
                PValue Vel_avg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Vel_Forwardmax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Vel_Backwardmax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Vel_Max = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Vel_Min = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                List<PValue>[] results = new List<PValue>[5];
                List<PValue> Veloresults = new List<PValue>();
                int len = input.Count;
                //TimeSpan ts = calcuinfo.fendtime - calcuinfo.fstarttime;


                #endregion

                TimeSpan t = input[1].Timestamp - input[0].Timestamp;
                Vel_Min.Value = Math.Abs((input[1].Value - input[0].Value) / t.TotalSeconds);
                #region 计算模块
                for (i = 1; i < len; i++)
                {
                    TimeSpan T = input[i].Timestamp - input[i - 1].Timestamp;

                    //计算速度
                    Veloresult.Value = (input[i].Value - input[i - 1].Value) / T.TotalSeconds;
                    Veloresult.Timestamp = input[i].Timestamp;

                    //更新最大正向速度
                    if (Veloresult.Value > 0)
                    {
                        if (Vel_Forwardmax.Value < Veloresult.Value)
                        {
                            Vel_Forwardmax.Value = Veloresult.Value;
                        }
                    }
                    //更新最大负向速度
                    if (Veloresult.Value < 0)
                    {
                        if (Vel_Backwardmax.Value > Veloresult.Value)
                        {
                            Vel_Backwardmax.Value = Veloresult.Value;
                        }
                    }


                    //速度平均值
                    Vel_avg.Value = Vel_avg.Value + Math.Abs(Veloresult.Value) * Convert.ToInt16(T.TotalSeconds);

                    //更新代数最小速度
                    if (Vel_Min.Value > Math.Abs(Veloresult.Value))
                        Vel_Min.Value = Math.Abs(Veloresult.Value);
                    //Veloresults.Add(Veloresult);
                }

                Vel_avg.Value = Vel_avg.Value / (len - 1);
                //最大代数速度
                if (Math.Abs(Vel_Backwardmax.Value) > Vel_Forwardmax.Value)
                    Vel_Max.Value = Math.Abs(Vel_Backwardmax.Value);
                else
                    Vel_Max.Value = Vel_Forwardmax.Value;



                //int len1 = Veloresults.Count;

                //Vel_Forwardmax.Value = Veloresults[0].Value;
                //Vel_Backwardmax.Value = Veloresults[0].Value;
                //Vel_Max.Value = Veloresults[0].Value;
                //Vel_Min.Value = Veloresults[0].Value;

                //for (int i = 0; i < len1; i++)
                //{
                //    Vel_avg.Value += Veloresults[i].Value;

                //    if (Veloresults[i].Value > Vel_Max.Value)   //更新最大代数速度
                //        Vel_Max.Value = Veloresults[i].Value;

                //    if (Veloresults[i].Value < Vel_Min.Value)   //更新最小代数速度
                //    {
                //        Vel_Min.Value = Veloresults[i].Value;
                //    }

                //    if ((Veloresults[i].Value > 0) && (Veloresults[i].Value > Vel_Max.Value))   //更新最大正向代数速度
                //        Vel_Forwardmax.Value = Veloresults[i].Value;

                //    if ((Veloresults[i].Value < 0) && (Veloresults[i].Value < Vel_Min.Value))   //更新最大负向代数速度
                //        Vel_Backwardmax.Value = Veloresults[i].Value;
                //}

                //if (Vel_Forwardmax.Value < 0)
                //    Vel_Forwardmax.Value = -9999999999;
                //if (Vel_Backwardmax.Value > 0)
                //    Vel_Backwardmax.Value = 9999999999;

                //Vel_avg.Value = Vel_avg.Value / len1;
                #endregion

                #region 组织输出
                //正向最大速度
                results[0] = new List<PValue>();
                results[0].Add(Vel_Forwardmax);

                //负向最大速度
                results[1] = new List<PValue>();
                results[1].Add(Vel_Backwardmax);

                //最大代数速度
                results[2] = new List<PValue>();

                results[2].Add(Vel_Max);

                //最小代数速度
                results[3] = new List<PValue>();
                results[3].Add(Vel_Min);

                //平均速度
                results[4] = new List<PValue>();
                results[4].Add(Vel_avg);

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
