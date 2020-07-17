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
    public class MMTAEquRTD_OLD : BaseModule, IModule
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

            int i;

            //设备中有有效点标志位
            bool validPointInDevice = false;
            //设备中有无效点标志位
            bool errorPointInDevice = false;
            //设备中有状态位非0点标志位
            bool errorStatusInDevice = false;

            //0输出初始化           
            List<PValue>[] results = new List<PValue>[12];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入：  输入值是某一个设备上所有温度测点一个小时内 每分钟过滤均值，
                //——也就是每个测点，有61个值，60个是分钟均值，一个是截止时刻值            
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.  该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，则应当有输出，大部分的输出项为0。个别输出项为空              
                            //inputs[1].RemoveAt(1);          //测试下面过滤是否正确
                            //inputs[2][0].Status = 10;       //测试下面过滤是否正确
                if (inputs == null ||inputs.Length==0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                int PointNumberInEqu = inputs.Length;                                                       //有多少个点，算多少个点。不追究实际有多少点。                
                                       
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。                
                for (i = inputs.Length-1; i >=0 ; i--)
                    if (inputs[i] != null && inputs[i].Count > 1)
                    {
                        validPointInDevice = true;                  //是要找到一个有效点，就置有效点标记
                        inputs[i].RemoveAt(inputs[i].Count - 1);    //删除该有效点的截止时刻
                    }
                    else
                    {
                        Array.Clear(inputs, i, 1);                  //如果点的数据长度不足2，则无效。去掉该点。去掉以后，该位置为一个null值。
                        
                        errorPointInDevice = true;                  //同时置标志位
                    }
                if (validPointInDevice == false)    //如果没有一个有效的数据点，则直接退出
                {
                    _warningFlag = true;
                    _warningInfo = "当前时间段的所有标签数据长度均未大于2，没有有效数据！"; //这种情况下，即记录报警。但同时又有计算结果
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //0.3、输入处理：标志位。过滤标志位。  
                string[] tagnames = calcuinfo.sourcetagname.Split(';');   //为了从管号中，获取位置信息。
                List<string> validnames = new List<string>();             //有效数据的标签名
                List<List<PValue>> validinputs = new List<List<PValue>>();
                for (i = inputs.Length-1; i >=0; i--)
                {
                    if (inputs[i] != null)
                    {
                        for (int j = inputs[i].Count - 1; j >= 0; j--)
                        {
                            //对于空数据或坏质量点数据进行剔除
                            if (inputs[i][j] == null || inputs[i][j].Status != 0)
                            {
                                inputs[i].RemoveAt(j);  //由于取分钟平均数，每个标签就有一个有效点，如果该有效点状态不正常，则该点无有效数据
                                errorStatusInDevice = true;
                            }
                        }
                        //对于剔除了坏质量点的数据在进行判断，
                        if (inputs[i] != null && inputs[i].Count != 0)
                        {
                            validinputs.Add(inputs[i]);
                            validnames.Add(tagnames[i]);
                        }
                    }
                }  
                //如果没有有效数据，就报警并退出
                if (validinputs == null || validinputs.Count == 0)
                {
                    _warningFlag = true;
                    _warningInfo = "当前时间段过滤非正常状态点后，所有标签点，均没有有效数据！"; //这种情况下，即记录报警。但同时又有计算结果
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //如果仅是有个别点有问题，报警，但继续计算
                if (errorPointInDevice == true)
                {
                    _warningFlag = true;
                    _warningInfo += "设备中有个别点数据无效！"; //这种情况下，即记录报警,但同时又有计算结果.
                    //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                }
                //如果仅是个别点状态有问题，报警，但继续计算
                if (errorStatusInDevice == true)
                {
                    _warningFlag = true;
                    _warningInfo += "设备中有个别点数据状态位异常！"; //这种情况下，即记录报警,但同时又有计算结果.
                    //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                }
                inputs = validinputs.ToArray();
                tagnames = validnames.ToArray();
                //定义计算结果
                PValue TempMin = new PValue(inputs[0][0].Value, inputs[0][0].Timestamp, inputs[0][0].Endtime, 0);           //最低,最低值时刻
                double TempMinPN = 0;           //发生最低值得点编号
                double TempAvg = 0;             //平均
                double Median = 0;              //中位数
                double MedianPN = 0;            //发生中位数的点编号
                PValue TempMax = new PValue(inputs[0][0].Value, inputs[0][0].Timestamp, inputs[0][0].Endtime, 0);  	        //最高,最高值时刻
                double TempMaxPN = 0; 	        //发生最高值得点编号
                double TempFar = 0;             //极值点距离中位数距离
                double TempStdevS = 0;          //标准差
               
                double TempDev = 0;             //高低差
                double TempDevPN = 0;           //高低差距离
                double TempDevPNR = 0;          //高低差距


                double[] valueall=new double[inputs.Length];
                double valuesum=0;                //本次值与下一个值得和
                double totalspan=0;               //总时间长度


                for (i = 0; i < inputs.Length ; i++)
                {
                    valueall[i] = inputs[i][0].Value;
                    valuesum += inputs[i][0].Value;
                    totalspan += inputs[i][0].Timespan;


                    //最小值、最小值时刻。和截止时刻值无关。
                    if (inputs[i][0].Value < TempMin.Value)
                    {
                        TempMin.Value = inputs[i][0].Value;
                        TempMin.Timestamp = inputs[i][0].Timestamp;
                        TempMin.Endtime = inputs[i][0].Endtime;
                        try { TempMinPN = double.Parse(tagnames[i].Substring(7, 2)); }
                        catch
                        {
                            _errorFlag = true;
                            _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", tagnames[i]);
                        }
                        
                    }
                    //最大值、最大值时刻。和截止时刻值无关。
                    if (inputs[i][0].Value > TempMax.Value)
                    {
                        TempMax.Value = inputs[i][0].Value;
                        TempMax.Timestamp = inputs[i][0].Timestamp;
                        TempMax.Endtime = inputs[i][0].Endtime;
                        try { TempMaxPN = double.Parse(tagnames[i].Substring(7, 2)); }
                        catch
                        {
                            _errorFlag = true;
                            _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", tagnames[i]);
                        }
                    }
                }

                //均值
                TempAvg = valuesum / inputs.Length;

                //中位数
                Array.Sort(valueall);
                int len=valueall.Length ;
                if (len % 2 == 0)
                    Median = (valueall[len / 2 - 1] + valueall[len / 2]) / 2;       //比如，一共6个元素，如果下标从0开始，就是[2]+[3]
                else
                    Median = valueall[(int)Math.Floor(len / 2.0) ];                                 //比如，一共7个元素，就是第4个元素

                //找到中位数以后，中位数点
                double minRangeMedian = Math.Abs(inputs[0][0].Value - Median);    //距中位数距离最小值
                try { MedianPN = double.Parse(tagnames[0].Substring(7, 2)); }     //距中位数距离最小值对应的位置（从标签名中获取）
                catch
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", tagnames[0]);
                }
                valuesum = 0;
                for (i = 0; i < inputs.Length; i++)
                {
                    valuesum += Math.Pow((inputs[i][0].Value - TempAvg), 2);     //用于求标准差
                    if (Math.Abs(inputs[i][0].Value - Median) < minRangeMedian)
                    {
                        minRangeMedian = inputs[i][0].Value - Median;
                        try { MedianPN = double.Parse(tagnames[i].Substring(7, 2)); }
                        catch
                        {
                            _errorFlag = true;
                            _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", tagnames[i]);
                        }
                    }
                }
                //标准差
                TempStdevS = Math.Sqrt(valuesum / inputs.Length);      //最后标准差还要开方

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
                results[0] = new List<PValue>();
                results[0].Add(TempMin);

                results[1] = new List<PValue>();
                results[1].Add(new PValue(TempMinPN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[2] = new List<PValue>();
                results[2].Add(new PValue(TempAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[3] = new List<PValue>();
                results[3].Add(new PValue(Median, calcuinfo.fstarttime, calcuinfo.fendtime, 0));


                results[4] = new List<PValue>();
                results[4].Add(new PValue(MedianPN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[5] = new List<PValue>();
                results[5].Add(TempMax);

                results[6] = new List<PValue>();
                results[6].Add(new PValue(TempMaxPN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[7] = new List<PValue>();
                results[7].Add(new PValue(TempFar, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[8] = new List<PValue>();
                results[8].Add(new PValue(TempStdevS, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[9] = new List<PValue>();
                results[9].Add(new PValue(TempDev, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[10] = new List<PValue>();
                results[10].Add(new PValue(TempDevPN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[11] = new List<PValue>();
                results[11].Add(new PValue(TempDevPNR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

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
