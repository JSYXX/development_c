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
    /// 四分位数   
    /// 基于快速排序算法，给出
    /// 2、最大值；3、最小值；5、极差；
    /// 21、4分位Q1；4分位Q3；19、中位数；22、四分位极差；23、三均值；20、P分位数
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
    public class MFourQuantileRange : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "FourQuantileRange";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "四分位数";
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
        private string _algorithms = "Max;Min;Range;FQ1;FQ3;FQRange;Median;ThreeMean;PQ";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "0.5";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "P分位数P值";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^(1|0\.{0,1}\d{0,})$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 9;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Max;Min;Range;FQ1;FQ3;FQRange;Median;ThreeMean;PQ";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "最大值;最小值;极差;第一四分位数;第三四分位数;四分位距;中位数;三均值;P分位数";
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
        /// 计算模块算法实现:基于快速排序算法的最大值、最小值、极差值、4分位FQ1、4分位FQ3、中位数、四分位极差、三均值、P分位数
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">计算p分位数所需参数P</param>       
        /// <returns>计算结果分别为最大值、最小值、极差值、4分位FQ1、4分位FQ3、四分位极差、中位数、三均值、P分位数</returns>
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
                PValue Max = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Min = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Range = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue FQ1 = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue FQ3 = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue FQRange = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue Median = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue ThreeMean = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue PQ = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                List<PValue>[] results = new List<PValue>[9];
                int len = input.Count;                
                //Console.WriteLine("请输入计算P分位数时所需要的参数P（0<=P<=1）:\n");
                //calcuinfo.fparas = Console.ReadLine();    //20170406高峰修改

                #endregion

                //概化计算中的所有计算结果均为List<PValue>[]，当计算中出现任何已知可以判断的问题时，均用results=null来返回
                //计算引擎在处理结果时
                if (len <= 0) return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);    //如果长度为零，返回空数组，计算引擎对空数组做跳过处理，不向数据库写入任何数据

                //对input安装value值进行排序
                
                input.OrderBy(x=>x.Value);


                #region 计算下四分位数FQ1

                if (Convert.ToInt32(len * 0.25) == len * 0.25)
                {
                    Int32 d4 = Convert.ToInt32(len * 0.25);
                    FQ1.Value = 0.5 * (input[d4 - 1].Value + input[d4].Value);
                }

                else
                {
                    Int32 d4 = Convert.ToInt32(len * 0.25);
                    FQ1.Value = input[d4 - 1].Value;
                }

                #endregion

                #region 计算上四分位数FQ3

                if (Convert.ToInt32(len * 0.75) == len * 0.75)
                {
                    Int32 u4 = Convert.ToInt32(len * 0.75);
                    FQ3.Value = 0.5 * (input[u4 - 1].Value + input[u4].Value);
                }

                else
                {
                    Int32 u4 = Convert.ToInt32(len * 0.75);
                    FQ3.Value = input[u4 - 1].Value;

                }

                #endregion

                #region 四分位数极差
                FQRange.Value = FQ3.Value - FQ1.Value;
                #endregion

                #region 中位数
                if (len % 2 == 0) //判断长度是奇数还是偶数
                    Median.Value = 0.5 * (input[len / 2 - 1].Value + input[len / 2].Value);
                else
                    Median.Value = input[(len + 1) / 2 - 1].Value;
                #endregion

                #region 三均值
                ThreeMean.Value = 0.25 * (FQ3.Value + FQ1.Value) + 0.5 * Median.Value;
                #endregion

                #region P分位数
                double P = double.Parse(calcuinfo.fparas);
                if (P < 1)
                {
                    //判断 len*P是否为整数
                    if (Convert.ToInt32(len * P) == len * P)
                    {
                        Int32 Pq = Convert.ToInt32(len * P);
                        PQ.Value = 0.5 * (input[Pq - 1].Value + input[Pq].Value);
                    }

                    else
                    {
                        Int32 PQnum = Convert.ToInt32(len * P);
                        PQ.Value = input[PQnum - 1].Value;
                    }
                }
                else if (P == 1)
                    PQ.Value = input[len - 1].Value;

                #endregion

                #region 组织输出
                //最大值
                results[0] = new List<PValue>();
                results[0].Add(input[len - 1]);//已经对input进行sort排序，所以最大值为最后一个

                //最小值
                results[1] = new List<PValue>();
                results[1].Add(input[0]);//已经对input进行sort排序，所以最小值为第一个

                //极差值
                results[2] = new List<PValue>();
                Range.Value = input[len - 1].Value - input[0].Value;
                results[2].Add(Range);

                //下四分位数FQ1
                results[3] = new List<PValue>();
                results[3].Add(FQ1);

                //上四分位数FQ3
                results[4] = new List<PValue>();
                results[4].Add(FQ3);

                //四分位数极差FQRange
                results[5] = new List<PValue>();
                results[5].Add(FQRange);


                //中位数Median
                results[6] = new List<PValue>();
                results[6].Add(Median);

                //三均值ThreeMean
                results[7] = new List<PValue>();
                results[7].Add(ThreeMean);

                //P分位数PQ
                results[8] = new List<PValue>();
                results[8].Add(PQ);

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
