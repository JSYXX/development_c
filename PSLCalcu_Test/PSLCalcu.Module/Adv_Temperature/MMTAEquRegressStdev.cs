using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备模型分析——回归分析——系数均差   
    /// 
    /// ——MMTAEquRegressStdev是对算法MMTAEquRegression求出的系数再求一段时间内的均差
    /// ——算法仅在日周期上进行计算。   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///    
    ///		2018.07.30 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.07.30</date>
    /// </author> 
    /// </summary>
    public class MMTAEquRegressStdev : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquRegressStdev";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备模型回归分析系数均差";
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
        private string _algorithms = "MMTAEquRegressStdev";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }

        private int _outputNumber = 7;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EquRLKStd;" +                    //1               
                                    "EquRLBStd;" +
                                    "EquRLRStd;" +
                                    "EquRQAStd;" +
                                    "EquRQBStd;" +
                                    "EquRQCStd;" +
                                    "EquRQRStd"                         //7                  
                                    ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "壁温分析线性回归K标准差;" +
                                        "壁温分析线性回归B标准差;" +
                                        "壁温分析线性回归残差R标准差;" +
                                        "壁温分析二次多项式回归A标准差;" +
                                        "壁温分析二次多项式回归B标准差;" +
                                        "壁温分析二次多项式回归C标准差;" +
                                        "壁温分析二次多项式回归残差R标准差"
                                        ;
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
        /// 计算模块算法实现:求实时值落在限定条件范围内的时间段span
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志。本算法无效</param>
        /// <param name="calcuinfo.fparas">限定条件，下限在前，上限在后，用分号分隔。如“25;70”，“25;”,";70"</param>       
        /// <returns>实时值落在限定条件范围内的时间段序列</returns>

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

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，则应当有输出，大部分的输出项为0。个别输出项为空
            List<PValue>[] results = new List<PValue>[7];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //数据量分析
                //——MMTAEquRegression有7个计算结果，对每个计算结果求方差
               
                //0、输入：输入是同一设备上所有温度点的小时均值，或者天均值         
                List<PValue> input = new List<PValue>();
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.                
                if (inputs == null ||inputs.Length==0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }

                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //输入的是该设备上所有温度测点每小时基本统计值，每个统计值也就是每个测点，仅有两个值，一个是小时值，一个是截止时刻值

                //定义变量
                double sum = 0;
                double avg = 0;
                double vaildcount = 0;
                double[] RegressionParaStd = new double[7];
                double std = 0;

                //计算
                for (i = 0; i < inputs.Length; i++)
                {
                    
                    if(inputs[i]==null ||inputs[i].Count==0)
                    {
                        continue ;
                    }
                    vaildcount = 0;
                    sum = 0;
                    avg = 0;
                    std = 0;
                    //求和
                    for (int j = 0; j < inputs[i].Count - 1; j++)//不包含截止点
                    {
                        if (inputs[i][j].Status == 0)       
                        {
                            sum += inputs[i][j].Value;
                            vaildcount += 1;                //只要有任何一个有效数据，就置位
                        }
                    }
                    if (vaildcount != 0)                    //如果剔除状态不正常点，无有效数据，则跳下一个
                    {
                        //求均值
                        avg = sum / vaildcount;     //得到均值
                        //求方差和
                        for (int j = 0; j < inputs[i].Count - 1; j++)
                        {
                            if (inputs[i][j].Status == 0)
                            {
                                std += Math.Pow((inputs[i][j].Value - avg), 2);
                            }
                        }

                        std = Math.Sqrt(std / vaildcount);

                        results[i] = new List<PValue>();
                        results[i].Add(new PValue(std, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                    }
                }

                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuinfo.fmodulename, calcuInfo.sourcetagname, calcuinfo.fstarttime.ToString(), calcuinfo.fendtime.ToString());
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
