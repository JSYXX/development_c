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
    /// 众数      
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
    public class MModeVariation : BaseModule, IModule
    {
        #region 计算模块信息：模块名称众数异众比率ModeVariation、输出项个数2、输出项名称Mode，VariationRation、输出项写入的数据表名称plsdata

        private string _moduleName = "ModeVariation";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "众数";
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
        private string _algorithms = "Mode;VariationRation";
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
        private string _outputDescs = "Mode;VariationRation";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "众数;众异比率";
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
        /// 计算模块算法实现:求众数异众比率ModeVariation
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">计算所需参数k</param>       
        /// <returns>计算结果为众数Mode异众比率VariationRation</returns>
       
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
                PValue Mode = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue VariationRation = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                List<PValue>[] results = new List<PValue>[2];
                int len = input.Count;                

                #endregion


                //计算平均数
                PValue Avg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                for (i = 0; i < len; i++)
                {
                    Avg.Value = Avg.Value + input[i].Value;

                }
                Avg.Value = Avg.Value / len;


                #region 最大众数（新定义）,异众比率
                //建立一个“邻域”的宽度。每个点都有自己的邻域。邻域重合“最多”的点的均值，是众数
                //当众数出现多个，应当是输出众数里边最大最小之差最小的那个，若还有多个众数，输出最大的众数

                PValue BiggestMode = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //定义最大众数 BiggestMode为一个 PValue类
                PValue Variation_ratio = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //定义异众比率 Variation_ratio为一个 PValue类

                List<PValue> pvalues = input;
                //定义宽度为均值的1%
                double width = Avg.Value * 0.01;
                //index_before1为之前计算的满足区间范围的最小值的索引；index_before2为之前计算的满足区间范围的最大值索引；
                int index_before1 = 1, index_before2 = 1;
                int countbf = 1;//之前计算的满足区间范围的数据个数初始化
                double sumbf = pvalues[1].Value;//之前计算的满足区间范围的数据值求和初始化
                double nummax = 0;//邻域上限
                double nummin = 0;//邻域下限
                //    //index_after1为当前计算的满足区间范围的最小值的索引；index_after2为当前计算的满足区间范围的最大值索引；
                int index_after1 = 0, index_after2 = 0;

                #region 循环查找众数
                for (i = 0; i < len; i++)
                {
                    nummax = pvalues[i].Value + width;
                    nummin = pvalues[i].Value - width;

                    index_after1 = i; index_after2 = i;//初始化

                    int countaf = 1;//当前计算的满足区间范围的数据个数初始化
                    double sumaf = pvalues[i].Value;//当前计算的满足区间范围的数据值求和初始化

                    //    向前搜索
                    int j = i - 1;
                    if (j >= 0)
                    {
                        while (j >= 0 & pvalues[j].Value < nummax & pvalues[j].Value >= nummin)
                        {
                            index_after1 = j;
                            sumaf = sumaf + pvalues[j].Value;
                            countaf = countaf + 1;
                            j = j - 1;
                            if (j < 0)
                                break;
                        }
                    }

                    //  向后搜索
                    j = i + 1;
                    if (j < len)
                    {
                        while (j < len & pvalues[j].Value < nummax & pvalues[j].Value >= nummin)
                        {
                            index_after2 = j;
                            countaf = countaf + 1;
                            sumaf = sumaf + pvalues[j].Value;
                            j = j + 1;
                            if (j >= len)
                                break;

                        }
                    }
                    //   个数比较条件
                    if (countaf > countbf)
                    {

                        index_before1 = index_after1;
                        index_before2 = index_after2;
                        countbf = countaf;
                        sumbf = sumaf;

                    }
                    else
                        //极差比较条件
                        if (countaf == countbf & (pvalues[index_after2].Value - pvalues[index_after1].Value) < (pvalues[index_before2].Value - pvalues[index_before1].Value))
                        {
                            index_before1 = index_after1; index_before2 = index_after2;
                            countbf = countaf;
                            sumbf = sumaf;

                        }

                        else
                            //总和比较条件
                            if (countaf == countbf & (pvalues[index_after2].Value - pvalues[index_after1].Value) == (pvalues[index_before2].Value - pvalues[index_before1].Value) & sumaf > sumbf)
                            {
                                index_before1 = index_after1;
                                index_before2 = index_after2;
                                countbf = countaf;
                                sumbf = sumaf;
                            }

                }
                BiggestMode.Value = sumbf / countbf;
                double VR = (double)countbf / (double)len;
                Variation_ratio.Value = 1.0 - VR;
                #endregion


                #endregion

                #region 组织输出
                results[0] = new List<PValue>();
                results[0].Add(BiggestMode);
                results[1] = new List<PValue>();
                results[1].Add(Variation_ratio);

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
