using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作
using Accord.Statistics;                //使用Accord.Statistics命名空间下的Measures类的	Correlation方法进行分析
                                        //参考 http://accord-framework.net/docs/html/M_Accord_Statistics_Measures_Correlation.htm

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备模型分析——相关性分析   
    /// 
    /// ——MMTAEquRegression采用Math.NET的Numerics组件，完成回归计算
    /// ——算法仅在小时和日周期上进行计算。   
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
    public class MMTAEquCorrelation : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquCorrelation";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备模型分析——相关性分析";
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
        private string _algorithms = "MMTAEquCorrelation";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYY"+
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }        
        private int _outputNumber = 54;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EqucorMin;" +                               
                                    "EquCorMinN;" +
                                    "EquCorMax;" +
                                    "EquCorMaxN;" +
                                    "P01_CorTAvg;" +
                                    "P02_CorTAvg;" +
                                    "P03_CorTAvg;" +
                                    "P04_CorTAvg;" +
                                    "P05_CorTAvg;" +
                                    "P06_CorTAvg;" +
                                    "P07_CorTAvg;" +
                                    "P08_CorTAvg;" +
                                    "P09_CorTAvg;" +
                                    "P10_CorTAvg;" +
                                    "P11_CorTAvg;" +
                                    "P12_CorTAvg;" +
                                    "P13_CorTAvg;" +
                                    "P14_CorTAvg;" +
                                    "P15_CorTAvg;" +
                                    "P16_CorTAvg;" +
                                    "P17_CorTAvg;" +
                                    "P18_CorTAvg;" +
                                    "P19_CorTAvg;" +
                                    "P20_CorTAvg;" +
                                    "P21_CorTAvg;" +
                                    "P22_CorTAvg;" +
                                    "P23_CorTAvg;" +
                                    "P24_CorTAvg;" +
                                    "P25_CorTAvg;" +
                                    "P26_CorTAvg;" +
                                    "P27_CorTAvg;" +
                                    "P28_CorTAvg;" +
                                    "P29_CorTAvg;" +
                                    "P30_CorTAvg;" +
                                    "P31_CorTAvg;" +
                                    "P32_CorTAvg;" +
                                    "P33_CorTAvg;" +
                                    "P34_CorTAvg;" +
                                    "P35_CorTAvg;" +
                                    "P36_CorTAvg;" +
                                    "P37_CorTAvg;" +
                                    "P38_CorTAvg;" +
                                    "P39_CorTAvg;" +
                                    "P40_CorTAvg;" +
                                    "P41_CorTAvg;" +
                                    "P42_CorTAvg;" +
                                    "P43_CorTAvg;" +
                                    "P44_CorTAvg;" +
                                    "P45_CorTAvg;" +
                                    "P46_CorTAvg;" +
                                    "P47_CorTAvg;" +
                                    "P48_CorTAvg;" +
                                    "P49_CorTAvg;" +
                                    "P50_CorTAvg" 
                                    ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "相关系数均值的最小值;" +
                                        "最小值所在点;" +
                                        "相关系数均值的最大值;" +
                                        "最大值所在点;" +
                                        "第01点与其他点相关性均值;" +
                                        "第02点与其他点相关性均值;" +
                                        "第03点与其他点相关性均值;" +
                                        "第04点与其他点相关性均值;" +
                                        "第05点与其他点相关性均值;" +
                                        "第06点与其他点相关性均值;" +
                                        "第07点与其他点相关性均值;" +
                                        "第08点与其他点相关性均值;" +
                                        "第09点与其他点相关性均值;" +
                                        "第10点与其他点相关性均值;" +
                                        "第11点与其他点相关性均值;" +
                                        "第12点与其他点相关性均值;" +
                                        "第13点与其他点相关性均值;" +
                                        "第14点与其他点相关性均值;" +
                                        "第15点与其他点相关性均值;" +
                                        "第16点与其他点相关性均值;" +
                                        "第17点与其他点相关性均值;" +
                                        "第18点与其他点相关性均值;" +
                                        "第19点与其他点相关性均值;" +
                                        "第20点与其他点相关性均值;" +
                                        "第21点与其他点相关性均值;" +
                                        "第22点与其他点相关性均值;" +
                                        "第23点与其他点相关性均值;" +
                                        "第24点与其他点相关性均值;" +
                                        "第25点与其他点相关性均值;" +
                                        "第26点与其他点相关性均值;" +
                                        "第27点与其他点相关性均值;" +
                                        "第28点与其他点相关性均值;" +
                                        "第29点与其他点相关性均值;" +
                                        "第30点与其他点相关性均值;" +
                                        "第31点与其他点相关性均值;" +
                                        "第32点与其他点相关性均值;" +
                                        "第33点与其他点相关性均值;" +
                                        "第34点与其他点相关性均值;" +
                                        "第35点与其他点相关性均值;" +
                                        "第36点与其他点相关性均值;" +
                                        "第37点与其他点相关性均值;" +
                                        "第38点与其他点相关性均值;" +
                                        "第39点与其他点相关性均值;" +
                                        "第40点与其他点相关性均值;" +
                                        "第41点与其他点相关性均值;" +
                                        "第42点与其他点相关性均值;" +
                                        "第43点与其他点相关性均值;" +
                                        "第44点与其他点相关性均值;" +
                                        "第45点与其他点相关性均值;" +
                                        "第46点与其他点相关性均值;" +
                                        "第47点与其他点相关性均值;" +
                                        "第48点与其他点相关性均值;" +
                                        "第49点与其他点相关性均值;" +
                                        "第50点与其他点相关性均值"
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
            List<PValue>[] results = new List<PValue>[54];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }
            

            try
            {
                //数据量分析
                //——取一个设备所有点一个小时每分钟的温度过滤值，一个设备50个温度点，那么也就50*60=3000个点。readmulti读取速度不会太慢。                
               
                //0、输入：输入是同一设备上所有温度点的小时均值，或者天均值
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空
                if (inputs == null||inputs.Length==0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                int PointNumberInEqu = inputs.Length;                                                       //有多少个点，算多少个点。不追究实际有多少点。                
                results = results.Take(4 + PointNumberInEqu).ToArray();                                     //将结果截取为和实际点数情况一致

                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //输入数值是一个设备所有点一个小时每分钟的温度过滤值
                //因为需要对不同点之间做相关性分析，需要每个点的有效数值数量相同，均为60个。如果少于60个则无法做相关性分析。
                string[] sourtagnames = calcuinfo.sourcetagname.Split(';');                                  //本算法是做管子位置与温度的回归。管子的位置需要从标签名称中取
                if (PointNumberInEqu != sourtagnames.Length)
                {
                    _errorFlag = true;
                    _errorInfo = "源标签数量与输入数据数量不符，请检查！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //准备计算相关性的数据矩阵
                //——数据矩阵第一维i是不同时刻点，第二维是同一时刻上不同的点值
                double[][] inputsMatrix = new double[60][];
                bool[] noValidData = new bool[PointNumberInEqu];                //给每个点数据有效性，做标志位。如果某一个点有一个无效数据，则该点就不进行相关性比较
                for (i = 0; i < 60; i++)                          //相关性数据矩阵，第一维i是不同时刻点
                {
                    inputsMatrix[i] = new double[PointNumberInEqu];             //第二维是同一时刻上不同的点值 
                                                        
                }
                for (i = 0; i < PointNumberInEqu; i++)
                { 

                }
                
                //默认每个点都有效。当某个点在60分钟内有一个无效值，则认为无效
                for (i = 0; i < PointNumberInEqu; i++)                          //设备的不同点
                {
                    if (inputs[i].Count != 61)
                    {
                        _errorFlag = true;
                        _errorInfo = "每个标签点一小时必须有60个数据，请检查！";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                    for (int j = 0; j < 60; j++)                                //每个点不同时间值
                    {
                        if (inputs[i][j].Status == 0)
                        {
                            inputsMatrix[j][i] = inputs[i][j].Value;            //                    
                        }
                        else
                        {
                            inputsMatrix[j][i] = float.MinValue;                //状态值不对，则该点取最小值
                            noValidData[i] = true;                              //对应的点无效标志置位
                        }

                    }
                }
                
                /*
                //经测试，第一维i是不同时刻点，第二维是同一时刻上不同的点值
                double[][] exMatrix=new double[4][];
                exMatrix[0] = new double[5] {1, 2, 3, 2, 3};
                exMatrix[1] = new double[5] {2, 3, 5, 4, 5};
                exMatrix[2] = new double[5] {3,4.2,0.1,0,0};
                exMatrix[3] = new double[5] {5, 6, 0.1, 0, 0 };
                double[][] cor = Measures.Correlation(exMatrix);
                */


                //定义变量
                double[][] correlationMatrix;
                double[] CorTAvg = new double[PointNumberInEqu];
                double CorMin = 0;
                double CorMinN = 0;
                double CorMax = 0;
                double CorMaxN = 0;

                //得到相关矩阵
                correlationMatrix = Measures.Correlation(inputsMatrix);

                //从相关矩阵计算各设备点的相关系数平均值
                //相关矩阵两个为维度都代表点号
                int count=0;
                for (i = 0; i < correlationMatrix.Length; i++)
                {
                    for (int j = 0; j < correlationMatrix[i].Length; j++)
                    {
                        if (i != j && noValidData[j] == false)
                        {
                            CorTAvg[i] = CorTAvg[i] + correlationMatrix[i][j];
                            count = count + 1;
                        }
                        else
                        {
                            CorTAvg[i] = -2;        //有非正常状态的点，相关系数平均数置为一个不能的值，-2
                        }
                    }
                    if(count!=0)
                        CorTAvg[i] = CorTAvg[i] / count;
                    else
                        CorTAvg[i] = 0;
                }

                //找最小值和最大值
                //找最大最小和管号
                int firstIndex = Array.IndexOf(noValidData, false);
                if (firstIndex == -1)
                {
                    _warningFlag = true;
                    _warningInfo = "每个标签点在计算时段内均有无效数据，无法计算该时段内的相关性。请检查！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                CorMin = CorTAvg[firstIndex];
                CorMax = CorTAvg[firstIndex];
                try { CorMinN = double.Parse(sourtagnames[firstIndex].Substring(7, 2)); }
                catch { };
                try { CorMaxN = double.Parse(sourtagnames[firstIndex].Substring(7, 2)); }
                catch { };

                for (i = 0; i < PointNumberInEqu; i++)
                {
                    if (noValidData[i] == false)
                    {
                        if (CorTAvg[i] < CorMin)
                        {
                            CorMin = CorTAvg[i];
                            try { CorMinN = double.Parse(sourtagnames[i].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = _errorInfo + String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", sourtagnames[i]);
                            }
                        }
                        if (CorTAvg[i] > CorMax)
                        {
                            CorMax = CorTAvg[i];
                            try { CorMaxN = double.Parse(sourtagnames[i].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = _errorInfo +  String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", sourtagnames[i]);
                            }
                        }
                    }
                }
                
                
                //组织计算结果
                results[0] = new List<PValue>();
                results[0].Add(new PValue(CorMin, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[1] = new List<PValue>();
                results[1].Add(new PValue(CorMinN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[2] = new List<PValue>();
                results[2].Add(new PValue(CorMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[3] = new List<PValue>();
                results[3].Add(new PValue(CorMaxN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                for (i = 0; i < PointNumberInEqu;i++ )
                {
                    results[4 + i] = new List<PValue>();
                    results[4 + i].Add(new PValue(CorTAvg[i], calcuinfo.fstarttime, calcuinfo.fendtime, 0));                    
                }

                //只返回与点数相符的计算结果项。计算引擎已经修改为按照实际返回结果来写数据。
                //results = results.Take(4 + PointNumberInEqu).ToArray();

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
