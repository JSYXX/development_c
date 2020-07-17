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
    /// 方差和标准差及变异系数      
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
    public class MVar_STD : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称Var_STD、包含的算法、输出项个数10、输出项名称、输出项写入的数据表名称
        //包含的算法包含算法：4、代数累加值;代数累加复位次数；1、代数平均值；14、未校平方和；13、矫正平方和；二阶中心距；8、方差；9、标准差；12、变异系数；27、离散系数
        //输出个数：10
        //输出名称:results结果依次为代数累加值Sum;代数累加复位次数SumResetT;代数平均值Avg;未校平方和SqrtSum;
        //矫正平方和CalibSqrtSum;二阶中心距2CenterDist;方差Varicance;标准差STD;变异系数VariationCoeff;离散系数Variation;
        //
        private string _moduleName = "Var_STD";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "方差和标准差及变异系数";//方差和标准差及变异系数，用于不计算偏度和峰度的情况下
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
        //包含算法：4、代数累加值;代数累加复位次数；1、代数平均值；14、未校平方和；13、矫正平方和；二阶中心距；8、方差；9、标准差；12、变异系数；27、离散系数
        //其中，二阶中心距为矫正平方和/n
        private string _algorithms = "Sum;SumResetT;Avg;SqrtSum;CalibSqrtSum;2CenterDist;Varicance;STD;VariationCoeff;Variation";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "100000000";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "代数累加复位上限";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^(\d+\.{0,1}\d{0,}){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 10;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Sum;SumResetT;Avg;SqrtSum;CalibSqrtSum;2CenterDist;Varicance;STD;VariationCoeff;Variation";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "代数累加和;代数累加和复位次数;代数平均值;未校平方和;校正平方和;二阶中心距;方差;标准差;变异系数;离散系数";
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

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[10];   //最大值（快速）;最小值（快速）;极差（快速）;算术和（快速）;算术平均值（快速）
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

                results = new List<PValue>[10];
                int len = input.Count;
                //TimeSpan ts = calcuinfo.fendtime - calcuinfo.fstarttime;
                //Console.WriteLine("请输入计算代数累加复位次数所需参数累加限值（按回车键结束输入）:\n");
                //calcuinfo.fparas = Console.ReadLine();
                string[] para = calcuinfo.fparas.Split(new char[] { ',' });

                #endregion


                #region 计算主体
                double Sumlimit = double.Parse(para[0]);


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
                //代数累加均值
                Avg.Value = Sum.Value / len;

                for (i = 0; i < len; i++)
                {
                    #region  校正平方和
                CalibSqrtSum.Value += System.Math.Pow((input[i].Value - Avg.Value), 2);
                #endregion

                    //方差
                    Varicance.Value += System.Math.Pow((input[i].Value - Avg.Value), 2);
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
