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
    /// k百分比最大值最小值      
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
    public class MKMaxMin : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称K最大值最小值百分比KMaxMin、包含的算法Kmax；Kmin；Kpercent、输出项个数3、输出项名称Kmax；Kmin；Kpercent、输出项写入的数据表名称

        private string _moduleName = "KMaxMin ";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "k百分比最大值最小值 ";
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
        private string _algorithms = "Kmax;Kmin;Kpercent";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "10;10;0.3";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "第k个最大值;第k个最小值;k百分比";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex=new Regex(@"^(\+?[1-9][0-9]*;){2}(0\.{1}\d{0,})$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 3;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "KMax;KMin;KPercent";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "K最大值;k最小值;k百分比";
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

        #region sortPV类，为了实现排序
        class sortPV : IComparer<PValue>
        {

            public int Compare(PValue x, PValue y)
            {
                if (y.Value > x.Value)
                    return -1;
                else if (y.Value == x.Value)
                    return 0;
                else
                    return 1;
            }
        }
        #endregion

        #region 计算模块
        /// <summary>
        /// 计算模块算法实现:求限定时间K最大值最小值百分比KMaxMin
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>计算结果分别为Kmax；Kmin；Kpercent</returns>
      
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
                PValue LARGE = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue SMALL = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                PValue PERCENTILE = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                //Console.WriteLine("请输入计算所需的三个参数large，small，percent，各个参数以;分隔\n");
                //calcuinfo.fparas = Console.ReadLine();
                string[] para = calcuinfo.fparas.Split(new char[] { ',' });
                int large = int.Parse(para[0]);
                int small = int.Parse(para[1]);
                double percentile = double.Parse(para[2]);


                int len = input.Count;                
                List<PValue> pvalues = input;

                List<PValue>[] results = new List<PValue>[3];
                #endregion

                #region 对pvalues进行排序
                sortPV inputsort = new sortPV();
                pvalues.Sort(inputsort);
                #endregion

                LARGE = pvalues[len - large];   //求k最大值
                SMALL = pvalues[small - 1];       //求k最小值

                //求k百分比
                int len2 = 0;
                int len3 = 0;
                if (percentile * (len - 1) % 100 == 0)      //表示可以整除
                {
                    len2 = Convert.ToInt32(1 + percentile * (len - 1) / 100);
                    PERCENTILE = pvalues[len2 - 1];
                }
                else                            //不能整除时，采用线性插值计算
                {
                    len2 = Convert.ToInt32(1 + percentile * (len - 1) / 100);
                    len3 = Convert.ToInt32(Math.Truncate(1 + percentile * (len - 1) / 100));
                    PERCENTILE.Value = pvalues[len3 - 1].Value + (pvalues[len3].Value - pvalues[len3 - 1].Value) * (len2 - len3);
                }


                #region 组织输出
                //组织输出
                results[0] = new List<PValue>();
                results[1] = new List<PValue>();
                results[2] = new List<PValue>();

                results[0].Add(LARGE);
                results[1].Add(SMALL);
                results[2].Add(PERCENTILE);
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
