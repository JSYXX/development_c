using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 排序(20个以内的输入变量)
    /// ——对输入的一组标签(不大于20个)的值进行排序
    /// ——本算法支持排序的输入标签数量不能大于20个。
    /// ——支持的输出，即每个便签当期的序位，不能大于20个。
    /// ——如果实际参与排序的标签数量不足20个，则相应的有效输出也不足20。应该使用algorithmsflag参数来控制有效输出的保存。
    /// 版本：1.1
    ///    
    /// 修改纪录    
    /// 
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///     2018.01.09 arrow测试
    ///		2018.01.04 版本：1.0 arrow 创建。    
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2018.01.04</date>
    /// </author> 
    /// </summary>
    public class MIndex20:BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "Index20";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "排序(20个以内的输入变量)";
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
        private string _inputDescsCN = "最少1个，最多20个";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "Index20";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "A";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "升序/降序";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([AD]){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 20;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs =   "Index20Of1;Index20Of2;Index20Of3;Index20Of4;Index20Of5;Index20Of6;Index20Of7;Index20Of8;Index20Of9;Index20Of10;"+
                                        "Index20Of11;Index20Of12;Index20Of13;Index20Of14;Index20Of15;Index20Of16;Index20Of17;Index20Of18;Index20Of19;Index20Of20";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "标签1序位;标签2序位;标签3序位;标签4序位;标签5序位;标签6序位;标签7序位;标签8序位;标签9序位;标签10序位;"+
                                        "标签11序位;标签12序位;标签13序位;标签14序位;标签15序位;标签16序位;标签17序位;标签18序位;标签19序位;标签20序位";
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
        /// 计算模块算法实现:求限定时间内积分值，用梯形面积累加
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数:积分限值、积分复位方式、积分放大倍数</param>       
        /// <returns>计算结果分别为积分值、积分复位次数</returns>
        
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
                
                List<double> series = new List<double>();

                //检查输入数据，不能超过9个
                if (inputs.Length > 20)
                {
                    _errorFlag = true;
                    _errorInfo = "输入的标签个数不能超过9个";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //处理截止时刻数据，本算法不需要截止时刻值参与
                for (i = 0; i < inputs.Length; i++)
                { if (inputs[i].Count > 1) inputs[i].RemoveAt(inputs[i].Count - 1); }

                //1、读取数据
                for (i = 0; i < inputs.Length; i++)
                {
                    series.Add(inputs[i][0].Value);
                }

                //2、排序
                if (calcuinfo.fparas == "A")
                {
                    series.Sort();
                }
                else
                {
                    series.Sort((x, y) => -x.CompareTo(y));
                }

                //3、找出标签的序位
                int[] tagindex = new int[inputs.Length];
                for (i = 0; i < inputs.Length; i++)
                {
                    tagindex[i] = indexOf(series, inputs[i][0].Value)+1;
                }

                //4、组织结果
                results = new List<PValue>[20];
                                
                for (i = 0; i < inputs.Length; i++)
                {
                    results[i] = new List<PValue>();
                    results[i].Add(new PValue(tagindex[i], calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                }

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

        #region 辅助函数
        //插入排序法
        private static void Sort(int[] list)
        {
            for (int i = 1; i < list.Length; ++i)
            {
                int t = list[i];
                int j = i;
                while ((j > 0) && (list[j - 1] > t))
                {
                    list[j] = list[j - 1];
                    --j;
                }
                list[j] = t;
            }

        }
        //查找序号
        private static int indexOf(List<double> series, double x)
        {
            try
            {
                for (int i = 0; i < series.Count; i++)
                {
                    if (series[i] == x)
                    {
                        return i;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
    }
}
