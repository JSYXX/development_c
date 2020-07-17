using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备基本统计指标实时值（分钟）
    /// ——用于计算一个设备上的所有点的分钟均值，在当前分钟的统计指标，用来做设备整体的实时曲线。
    /// ——该算法使用设备内个点的分钟均值，采用D:X:Y读点法，一次性读取所有点的分钟均值。
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
    public class MMTAEquRTD : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquRTD";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备实时（分钟）温度指标";
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
        private string _inputDescsCN = "温度点的分钟均值（数量不确定）";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAEquRTD";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }

        private int _outputNumber = 12;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EquMinReal;" +
                                    "EquMinPNReal;" +
                                    "EquAvgReal;" +
                                    "EquMedianReal;" +
                                    "EquMedianPNReal;" +
                                    "EquMaxReal;" +
                                    "EquMaxPNReal;" +
                                    "EquFarReal;" +
                                    "EquStdevSReal;" +
                                    "EquDevReal;" +
                                    "EquDevPNReal;" +
                                    "EquDevPNRReal"
                                    ;

        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "设备点最低值及时刻;" +
                                        "设备点最低值发生的点位置编号（标签名中）;" +
                                        "设备点均值;" +
                                        "设备点中位数;" +
                                        "设备点距离中位数最近的点位置编号（标签名中）;" +
                                        "设备点最高值及时刻;" +
                                        "设备点最高值发生的点位置编号（标签名中）;" +
                                        "设备点极值点距中位数距离（点位置编号的差）;" +
                                        "设备点分钟值的标准差;" +
                                        "设备点高低差;" +
                                        "设备点高低差之间的距离（点位置编号的差）;" +
                                        "设备点高低差距"
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

            int i,j;

            int DataNumber = (int)calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalMinutes + 1;       //每一个设备点的数量量，应该为从起始时间到截止时间的分钟数（加一个截止时刻值）  

            //0输出初始化           
            List<PValue>[] results = new List<PValue>[12];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                for (j = 0; j < DataNumber - 1; j++)                  //DataNumber，每个计算结果只需要0~59，共60个计算结果
                    results[i].Add(new PValue(0, calcuinfo.fstarttime.AddMinutes(j), calcuinfo.fstarttime.AddMinutes(j+1), (long)StatusConst.InputIsNull)); //给一小时内的60分钟，每分钟都准备好计算结果
            }

            try
            {
                //0、输入：  输入值是某一个设备上所有温度测点一个小时内 每分钟过滤均值，
                //——也就是每个测点，有61个值，前60个是分钟均值，最后徐一个是截止时刻值            
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.  该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，则应当有输出，大部分的输出项为0。个别输出项为空。 
                if (inputs == null || inputs.Length == 0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                int PointNumberInEqu = inputs.Length;                                                       //如果inputs不为空或长度不为零，则设备内点数量为inupts.length  

                //0.2、输入处理：数据长度必须相同。每一个输入必须都包含相同数量DataNumber个数据点                   
                for(i=0;i<PointNumberInEqu;i++)
                {
                    if (inputs[i].Count != DataNumber)
                    {
                        _errorFlag = true;
                        _errorInfo = String.Format("当前时间段第{0}点所包含的数据不足{1}个.正常情况下，计算周期内应该有{1}个数据点（包含状态非正常点）。", i, DataNumber); //这种情况下，即记录报警。但同时又有计算结果
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                    }
                }

                //准备存放计算结果的变量
                List<PValue>[] resultsTemp = new List<PValue>[12];
                for (i = 0; i < 12; i++)
                {
                    resultsTemp[i] = new List<PValue>();
                }
                //0.3、如果设备内所有点包含数据相同，并等于起止时间的分钟数，则一次计算每分钟情况
                string[] tagnames = calcuinfo.sourcetagname.Split(';');             //为了从管号中，获取位置信息。
                for (i = 0; i < DataNumber-1; i++)                                  //按第二维，时间进行取数，只计算前面的时间段，跳过截止时刻
                {
                    //设备中有无效点标志位
                    bool errorPointInDevice = false;
                    //设备中有状态位非0点标志位
                    bool errorStatusInDevice = false;

                    //0.3、输入处理：标志位。过滤标志位。
                    List<string> validnames = new List<string>();                   //有效数据的标签名
                    List<PValue> validinputs = new List<PValue>();                  //有效数据
                    List<double> validinputsvalue =new List<double>();              //便于观察

                    for (j = 0; j < PointNumberInEqu; j++)                          //每一分钟，读取所有设备点在该分钟的值
                    {                       
                        //对于空数据或坏质量点数据进行剔除
                        if (inputs[j][i] == null )       //如果该点数据为空，且状态不为0
                        {
                            errorPointInDevice = true;
                            continue;
                        }
                        else if (inputs[j][i].Status != 0)
                        {
                            errorStatusInDevice = true;
                            continue;
                        }
                        else
                        {
                            validinputs.Add(inputs[j][i]);
                            validnames.Add(tagnames[j]);
                            validinputsvalue.Add(inputs[j][i].Value);   //仅用于测试方便
                        }   
                    }
                    //如果没有有效数据，就添加报警,给非正常状态结算结果，并直接计算下一个
                    if (validinputs == null || validinputs.Count == 0)
                    {
                        for (j = 0; j < 12; j++)
                        {
                            resultsTemp[j].Add(new PValue(0, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), (long)StatusConst.InputIsNull));
                        }
                        _warningFlag = true;
                        _warningInfo += String.Format("当前时间段{0}-{1}过滤非正常状态点后，所有标签点，均没有有效数据！" + Environment.NewLine, calcuinfo.fstarttime.AddMinutes(i).ToString(), calcuinfo.fstarttime.AddMinutes(i + 1).ToString()); //这种情况下，即记录报警。但同时又有计算结果
                        continue;
                    }
                    //如果仅是有个别点有问题，报警，但继续计算
                    if (errorPointInDevice == true)
                    {
                        _warningFlag = true;
                        _warningInfo += String.Format("在{0}到{1}中，设备点中有个别点数据为空！", calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1)) + Environment.NewLine;
                        //这种情况下，即记录报警,但同时又有计算结果.                        
                    }
                    //如果仅是个别点状态有问题，报警，但继续计算
                    if (errorStatusInDevice == true)
                    {
                        _warningFlag = true;
                        _warningInfo += String.Format("在{0}到{1}中，设备点中有个别点数据状态位异常！", calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1)) + Environment.NewLine;
                        //这种情况下，即记录报警,但同时又有计算结果.
                    }
                   
                    //定义计算结果
                    PValue TempMin = new PValue(validinputs[0].Value, validinputs[0].Timestamp, validinputs[0].Endtime, 0);           //最低,最低值时刻
                    double TempMinPN = 0;           //发生最低值得点编号
                    double TempAvg = 0;             //平均
                    double Median = 0;              //中位数
                    double MedianPN = 0;            //发生中位数的点编号
                    PValue TempMax = new PValue(validinputs[0].Value, validinputs[0].Timestamp, validinputs[0].Endtime, 0);  	        //最高,最高值时刻
                    double TempMaxPN = 0; 	        //发生最高值得点编号
                    double TempFar = 0;             //极值点距离中位数距离
                    double TempStdevS = 0;          //标准差

                    double TempDev = 0;             //高低差
                    double TempDevPN = 0;           //高低差距离
                    double TempDevPNR = 0;          //高低差距
                    
                    double valuesum = 0;                //本次值与下一个值得和
                    double totalspan = 0;               //总时间长度

                    //通过循环找出最大最小值
                    for (j = 0; j < validinputs.Count; j++)
                    {
                        valuesum += validinputs[j].Value;
                        totalspan += validinputs[j].Timespan;

                        //最小值、最小值时刻。和截止时刻值无关。
                        if (validinputs[j].Value < TempMin.Value)
                        {
                            TempMin.Value = validinputs[j].Value;
                            TempMin.Timestamp = validinputs[j].Timestamp;
                            TempMin.Endtime = validinputs[j].Endtime;
                            try { TempMinPN = double.Parse(validnames[j].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", validnames[j]);
                            }

                        }
                        //最大值、最大值时刻。和截止时刻值无关。
                        if (validinputs[j].Value > TempMax.Value)
                        {
                            TempMax.Value = validinputs[j].Value;
                            TempMax.Timestamp = validinputs[j].Timestamp;
                            TempMax.Endtime = validinputs[j].Endtime;
                            try { TempMaxPN = double.Parse(validnames[j].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", validnames[j]);
                            }
                        }
                    }

                    //均值
                    TempAvg = valuesum / validinputs.Count;

                    //中位数
                    double[] valueall = validinputsvalue.ToArray();
                    Array.Sort(valueall);
                    int len = valueall.Length;
                    if (len % 2 == 0)
                        Median = (valueall[len / 2 - 1] + valueall[len / 2]) / 2;           //比如，一共6个元素，如果下标从0开始，就是[2]+[3]
                    else
                        Median = valueall[(int)Math.Floor(len / 2.0)];                      //比如，一共7个元素，就是第4个元素

                    //找到中位数以后，通过循环找距离中位值最近的值的点的点号。到中位数距离，从第一个点开始，依次判断。
                    double minRangeMedian = Math.Abs(validinputs[0].Value - Median);        //距中位数距离最小值，先从第一个点找起
                    try { MedianPN = double.Parse(validnames[0].Substring(7, 2)); }         //距中位数距离最小值对应的位置（从标签名中获取）。先从第一个点找起
                    catch
                    {
                        _errorFlag = true;
                        _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", validnames[0]);
                    }
                    valuesum = 0;
                    for (j = 0; j < validinputs.Count; j++)
                    {
                        valuesum += Math.Pow((validinputs[j].Value - TempAvg), 2);          //用于求标准差
                        if (Math.Abs(validinputs[j].Value - Median) < minRangeMedian)
                        {
                            minRangeMedian = validinputs[j].Value - Median;
                            try { MedianPN = double.Parse(validnames[j].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", validnames[j]);
                            }
                        }
                    }
                    //标准差
                    TempStdevS = Math.Sqrt(valuesum / validinputs.Count);      //最后标准差还要开方

                    //极值点距离中位数距离
                    double upperFar = Math.Abs(TempMaxPN - MedianPN);   //最大点距离中位数距离
                    double lowerFar = Math.Abs(TempMinPN - MedianPN);   //最小点距离中位数距离
                    TempFar = upperFar > lowerFar ? upperFar : lowerFar;    //找里面比较大的一个

                    //最大值最小值距离
                    TempDev = Math.Abs(TempMax.Value - TempMin.Value);  //最大值最小值差绝对值
                    TempDevPN = Math.Abs(TempMaxPN - TempMinPN);        //最大值最小值距离
                    if (TempDevPN != 0)
                        TempDevPNR = TempDev / TempDevPN;
                    else
                        TempDevPNR = 0;

                    //组织计算结果

                    resultsTemp[0].Add(TempMin);

                    resultsTemp[1].Add(new PValue(TempMinPN, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[2].Add(new PValue(TempAvg, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[3].Add(new PValue(Median, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[4].Add(new PValue(MedianPN, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[5].Add(TempMax);

                    resultsTemp[6].Add(new PValue(TempMaxPN, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[7].Add(new PValue(TempFar, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[8].Add(new PValue(TempStdevS, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[9].Add(new PValue(TempDev, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[10].Add(new PValue(TempDevPN, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));

                    resultsTemp[11].Add(new PValue(TempDevPNR, calcuinfo.fstarttime.AddMinutes(i), calcuinfo.fstarttime.AddMinutes(i + 1), 0));
                }
                return new Results(resultsTemp, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

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
