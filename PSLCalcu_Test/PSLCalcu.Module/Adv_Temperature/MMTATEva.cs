using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 温度点综合评价算法   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     
    ///		2018.05.22 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.05.22</date>
    /// </author> 
    /// </summary>
    public class MMTATEva:BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTATEva";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "温度点综合评价算法";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 13;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "H~L时间长度;越H总平均值(包含HH);越HH平均值;越H次数;越HH次数;越L总平均值(包含LL);越LL平均值;越L次数;越LL次数;波动值;波动越N1次数;波动越N2次数;波动越N3次数";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTATEva";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
       
        private int _outputNumber = 11;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "TEvaSR;" +
                                    "TEvaHT;" +
                                    "TEvaHHT;" +
                                    "TEvaHTF;" +
                                    "TEvaHHTF;" +
                                    "TEvaLT;" +
                                    "TEvaLLT;" +
                                    "TEvaLTF;" +
                                    "TEvaLLTF;" +
                                    "TEvaVol;" +
                                    "TEvaDMax";
                                    
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "安全系数,H~L时间占比;" +
                                        "越 H 限强度指数;" +
                                        "越 HH 限强度指数;" +
                                        "越 H 限力度指数;" +
                                        "越 HH 限力度指数;" +
                                        "越 L 限强度指数;" +
                                        "越 LL 限强度指数;" +
                                        "越 L 限力度指数;" +
                                        "越 LL 限力度指数;" +
                                        "波动强度;" +
                                        "波动力度" ;
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
        /// 计算模块算法实现:
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
            List<PValue>[] results = new List<PValue>[11];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入                
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.                
                if (inputs == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                if (inputs.Length < 13)
                {
                    _errorFlag = true;
                    _errorInfo = "温度点综合评价算法要求必须有且仅有13个输入";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (Array.IndexOf(inputs, null) != -1)
                {
                    _errorFlag = true;
                    _errorInfo = "温度点综合评价算法13个源数据中有空数据";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。 
                for (i = 0; i < inputs.Length; i++)
                    if (inputs[i] != null && inputs[i].Count > 1) inputs[i].RemoveAt(inputs[i].Count - 1);

                //0.3、输入处理：标志位。过滤标志位。并将非空点取出来                
                for (i = 0; i < inputs.Length; i++)
                {                   
                        for (int j = inputs[i].Count - 1; j >= 0; j--)
                        {
                            if (inputs[i][j].Status != 0) inputs[i].RemoveAt(j);
                        }                   

                }
                
                //0.4、对于过滤完的点
                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] == null || inputs[i].Count() == 0)
                    {
                        _warningFlag = true;
                        _warningInfo = "温度点综合评价算法13个源数据经状态过滤后存在没有有效数据的点";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                }

                //输入数据和参数检查
                double N1, N2, N3;  //极差超限值                

                string[] paras = Regex.Split(calcuinfo.fparas, ";|；");      //要测试一下，split分割完仅有三个元素是否可以    
                N1 = double.Parse(paras[0]);
                N2 = double.Parse(paras[1]);
                N3 = double.Parse(paras[2]);

                //定义计算结果
                double TEvaSR=0;           //安全系数，H~L直接的时间占比
                for (i = 0; i < inputs[0].Count; i++)
                {
                    TEvaSR = TEvaSR + inputs[0][i].Value * inputs[0][i].Timespan;
                }
                TEvaSR = TEvaSR / calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalMilliseconds;

                double TEvaHT = 0;
                double TEvaHHT = 0;
                double TEvaHTF = 0;
                double TEvaHHTF = 0;
                double TEvaLT = 0;
                double TEvaLLT = 0;
                double TEvaLTF = 0;
                double TEvaLLTF = 0;
                

                //输入："0、H~L时间长度;
                //       1、越H总平均值(包含HH);2、越HH平均值;3、越H次数;4、越HH次数;
                //       5、越L总平均值(包含LL);6、越LL平均值;7、越L次数;8、越LL次数;
                //       9、波动值;10、波动越N1次数;11、波动越N2次数;12波动越N3次数"

                //越H限强度(包括HH)
                TEvaHT = 0;
                for (i = 0; i < inputs[1].Count; i++)
                {
                    TEvaHT = TEvaHT + inputs[1][i].Value;
                }
                TEvaHT = TEvaHT / inputs[1].Count;
                //越HH限强度
                TEvaHHT = 0;
                for (i = 0; i < inputs[2].Count; i++)
                {
                    TEvaHHT = TEvaHHT + inputs[2][i].Value;
                }
                TEvaHHT = TEvaHHT / inputs[2].Count;

                
                double GNumber = 0;
                //先累加HH总数
                for (i = 0; i < inputs[4].Count; i++)
                {
                    GNumber = GNumber + inputs[4][i].Value;
                }
                //越HH力度指数
                TEvaHHTF = GNumber * TEvaHHT;
                //在累加H总数
                for (i = 0; i < inputs[3].Count; i++)
                {
                    GNumber = GNumber + inputs[3][i].Value;
                }
                //越H力度指数（包括HH)
                TEvaHTF = GNumber * TEvaHT;
                
                

                //越L限强度(包括LL)
                TEvaLT = 0;
                for (i = 0; i < inputs[5].Count; i++)
                {
                    TEvaLT = TEvaLT + inputs[5][i].Value;
                }
                TEvaLT = TEvaLT / inputs[5].Count;
                //越LL限强度
                TEvaLLT = 0;
                for (i = 0; i < inputs[6].Count; i++)
                {
                    TEvaLLT = TEvaLLT + inputs[6][i].Value;
                }
                TEvaLLT = TEvaLLT / inputs[6].Count;

                GNumber = 0;
                //先累加LL总数
                for (i = 0; i < inputs[8].Count; i++)
                {
                    GNumber = GNumber + inputs[8][i].Value;
                }
                //越LL力度指数
                TEvaLLTF = GNumber * TEvaLLT;
                //在累加H总数
                for (i = 0; i < inputs[7].Count; i++)
                {
                    GNumber = GNumber + inputs[7][i].Value;
                }
                //越H力度指数（包括HH)
                TEvaLTF = GNumber * TEvaLT;
                
                //波动
                double TEvaVol = 0;
                for (i = 0; i < inputs[9].Count; i++)
                {
                    TEvaVol = TEvaVol + inputs[9][i].Value;
                }

                //波动力度
                double TEvaDMax = 0;
                double N1Number,N2Number,N3Number;
                N1Number = 0;
                for (i = 0; i < inputs[11].Count; i++)
                {
                    N1Number = N1Number + inputs[11][i].Value;      //越N1总次数
                }
                N2Number = 0;
                for (i = 0; i < inputs[12].Count; i++)
                {
                    N2Number = N2Number + inputs[12][i].Value;      //越N2总次数
                }
                N3Number = 0;
                for (i = 0; i < inputs[12].Count; i++)
                {
                    N3Number = N3Number + inputs[12][i].Value;      //越N3总次数
                }
                double N2NumberReal = N2Number - N1Number ;        //越限统计中，N2越限次数包含了越N2、N1次数
                double N3NumberReal = N3Number - N2Number;         //越限统计中，N3越限次数包含了越N3、N2、N1次数

                TEvaDMax = (N1Number * N1 + N2NumberReal * N2 + N3NumberReal * N3) / (N3Number);
                
                //组织计算结果
                results[0] = new List<PValue>();
                results[0].Add(new PValue(TEvaSR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[1] = new List<PValue>();
                results[1].Add(new PValue(TEvaHT, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[2] = new List<PValue>();
                results[2].Add(new PValue(TEvaHHT, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[3] = new List<PValue>();
                results[3].Add(new PValue(TEvaHTF, calcuinfo.fstarttime, calcuinfo.fendtime, 0));


                results[4] = new List<PValue>();
                results[4].Add(new PValue(TEvaHHTF, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[5] = new List<PValue>();
                results[5].Add(new PValue(TEvaLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[6] = new List<PValue>();
                results[6].Add(new PValue(TEvaLLT, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[7] = new List<PValue>();
                results[7].Add(new PValue(TEvaLTF, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[8] = new List<PValue>();
                results[8].Add(new PValue(TEvaLLTF, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[9] = new List<PValue>();
                results[9].Add(new PValue(TEvaVol, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[10] = new List<PValue>();
                results[10].Add(new PValue(TEvaDMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

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
