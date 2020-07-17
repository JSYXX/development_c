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
    /// 偏度和峰度      
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
    public class MSkewness_Kurtosis : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称Skewness_Kurtosis、包含的算法、输出项个数13、输出项名称、输出项写入的数据表名称
        //包含的算法包含算法：4、代数累加值;代数累加复位次数；1、代数平均值；14、未校平方和；13、矫正平方和；二阶中心距；8、方差；9、标准差；12、变异系数；27、离散系数;三阶中心距；四阶中心距；17偏度；18峰度
        //输出个数：14
        //输出名称:results结果依次为:代数累加值Sum;代数累加复位次数SumResetT;代数平均值Avg;未校平方和SqrtSum;
        //                          矫正平方和CalibSqrtSum;二阶中心距2CenterDist;方差Varicance;标准差STD;变异系数VariationCoeff;离散系数Variation;
        //                          三阶中心距3CenterDist；四阶中心距4CenterDist；17偏度Skewness；18峰度kurtosis
        private string _moduleName = "Skewness_Kurtosis";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "偏度和峰度";//用于需要计算偏度和峰度的情况下,该计算仅算那些偏度和峰度需要的中间值
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
        //包含算法：4、代数累加值;代数累加复位次数；1、代数平均值；14、未校平方和；13、矫正平方和；二阶中心距；8、方差；9、标准差；12、变异系数；27、离散系数;三阶中心距；四阶中心距；17偏度；18峰度
        //其中，二阶中心距为矫正平方和/n
        private string _algorithms = "Sum;SumResetT;Avg;SqrtSum;CalibSqrtSum;2CenterDist;Varicance;STD;VariationCoeff;Variation;3CenterDist;4CenterDist;Skewness;Kurtosis";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "10000";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "代数累加复位上限值";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex=new Regex(@"^(\d+\.{0,1}\d{0,}){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 14;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "SKSum;SKSumResetT;SKAvg;SKSqrtSum;SKCalibSqrtSum;SK2CenterDist;SKVaricance;SKSTD;SKVariationCoeff;SKVariation;3CenterDist;4CenterDist;Skewness;Kurtosis";        
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "代数累加和;代数累加和复位次数;代数平均值;未校平方和;校正平方和;二阶中心距;方差;标准差;变异系数;离散系数;三阶中心距;四阶中心距;偏度;峰度";
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
        /// 计算模块算法实现:4、代数累加值;代数累加复位次数；1、代数平均值；14、未校平方和；13、矫正平方和；二阶中心距；8、方差；9、标准差；12、变异系数；27、离散系数
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara"></param>       
        /// <returns>计算结果分别为4、代数累加值;代数累加复位次数；1、代数平均值；14、未校平方和；13、矫正平方和；二阶中心距；8、方差；9、标准差；12、变异系数；27、离散系数</returns>
        
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
                PValue Sum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue SumResetT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Avg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue SqrtSum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue CalibSqrtSum = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue CenterDist_2 = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Varicance = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue STD = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue VariationCoeff = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Variation = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue CenterDist_3 = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue CenterDist_4 = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Skewness = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Kurtosis = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);

                List<PValue>[] results = new List<PValue>[14];
                int len = input.Count;
                //TimeSpan ts = calcuinfo.fendtime - calcuinfo.fstarttime;
                //Console.WriteLine("请输入计算代数累加复位次数所需参数 累加限值(按回车输入结束):\n");

                //calcuinfo.fparas = Console.ReadLine();
                string[] para = calcuinfo.fparas.Split(new char[] { ',' });

                #endregion


                #region 计算主体
                double Sumlimit = double.Parse(para[0]);
                //double k3 = double.Parse(para[1]);
                //double k4 = double.Parse(para[2]);


                #region  代数累加值，代数累加复位次数 未校正平方和
                for (i = 0; i < len; i++)
                {
                    #region 代数累加值，代数累加复位次数
                    if (Sum.Value < Sumlimit)
                        Sum.Value += input[i].Value;
                    else
                    {
                        SumResetT.Value++;
                        Sum.Value = Sum.Value - Sumlimit;
                    }
                    #endregion

                    #region  未校正平方和
                    SqrtSum.Value += System.Math.Pow(input[i].Value, 2);
                    #endregion
                }
                #endregion


                //代数累加均值
                Avg.Value = Sum.Value / len;



                for (i = 0; i < len; i++)
                {
                    #region  校正平方和
                    CalibSqrtSum.Value += System.Math.Pow((input[i].Value - Avg.Value), 2);
                    #endregion

                    //方差
                    Varicance.Value += System.Math.Pow((input[i].Value - Avg.Value), 2);
                    //3阶中心矩
                    CenterDist_3.Value += System.Math.Pow((input[i].Value - Avg.Value), 3);
                    //4阶中心矩
                    CenterDist_4.Value += System.Math.Pow((input[i].Value - Avg.Value), 4);
                    //偏度
                    Skewness.Value += System.Math.Pow((input[i].Value - Avg.Value), 3);

                }
                //方差
                Varicance.Value = Varicance.Value / len;
                //标准差
                STD.Value = System.Math.Sqrt(Varicance.Value);
                //变异系数
                VariationCoeff.Value = STD.Value / Avg.Value;
                //离散系数等于变异系数
                Variation.Value = STD.Value / Avg.Value;
                //二阶中心距为矫正平方和
                CenterDist_2 = CalibSqrtSum;
                //3阶中心矩
                CenterDist_3.Value = CenterDist_3.Value / len;
                //4阶中心矩
                CenterDist_4.Value = CenterDist_4.Value / len;
                //偏度
                Skewness.Value = len * Skewness.Value / ((len - 1) * (len - 2) * System.Math.Pow(STD.Value, 3));


                for (i = 0; i < len; i++)
                {
                    //峰度
                    Kurtosis.Value += System.Math.Pow((input[i].Value - Avg.Value) / STD.Value, 4);
                }
                //峰度
                Kurtosis.Value = (len * (len + 1) * Kurtosis.Value) / ((len - 1) * (len - 2) * (len - 3)) - ((len - 1) * (len - 1) * 3) / ((len - 2) * (len - 3));


                #endregion


                #region  组织输出
                //代数累加值
                results[0] = new List<PValue>();
                results[0].Add(Sum);

                //代数累加复位次数
                results[1] = new List<PValue>();
                results[1].Add(SumResetT);

                //代数累加均值
                results[2] = new List<PValue>();
                results[2].Add(Avg);

                //未校正平方和
                results[3] = new List<PValue>();
                results[3].Add(SqrtSum);

                //校正平方和
                results[4] = new List<PValue>();
                results[4].Add(CalibSqrtSum);

                //二阶中心距
                results[5] = new List<PValue>();
                results[5].Add(CenterDist_2);


                //方差
                results[6] = new List<PValue>();
                results[6].Add(Varicance);

                //标准差
                results[7] = new List<PValue>();
                results[7].Add(STD);

                //变异系数
                results[8] = new List<PValue>();
                results[8].Add(VariationCoeff);

                //离散系数
                results[9] = new List<PValue>();
                results[9].Add(Variation);

                //3阶中心距
                results[10] = new List<PValue>();
                results[10].Add(CenterDist_3);

                //4阶中心距
                results[11] = new List<PValue>();
                results[11].Add(CenterDist_4);

                //偏度
                results[12] = new List<PValue>();
                results[12].Add(Skewness);

                //峰度
                results[13] = new List<PValue>();
                results[13].Add(Kurtosis);

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
