using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon;
using Config;                           //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备上单点温度特征值计算   
    /// 
    /// ——MMTAEquDOR在求出设备每分钟温度均值的基础上，看一小时内，设备内的某一点每分钟滤波值与设备均值的距离，在一个小时上的累积量。
    /// ——算法周期仅在小时周期上进行计算。   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///    
    ///		2018.07.30 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.08.31</date>
    /// </author> 
    /// </summary>
    public class MMTAEquTDOR: BaseModule, IModule
    {
        #region 计算模块信息：设备上单点温度特征值计算 ;

        private string _moduleName = "MMTAEquTDOR";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备上单点温度特征值计算-单点计算";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 2;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "设备上所有温度点分钟过滤值";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MMTAEquTDOR";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYY"+
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"+
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"+
                                         "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY"
                                         ;
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
       
        private int _outputNumber = 155;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "EquDistTAvg;" +
                                      "EquDistTMin;" +
                                      "EquDistTMinN;" +
                                      "EquDistTMax;" +
                                      "EquDistTMaxN;" +                                      
                                      "P01_DistT;" + "P01_DistTR;" + "P01_DistTO;" +
                                      "P02_DistT;" + "P02_DistTR;" + "P02_DistTO;" +
                                      "P03_DistT;" + "P03_DistTR;" + "P03_DistTO;" +
                                      "P04_DistT;" + "P04_DistTR;" + "P04_DistTO;" +
                                      "P05_DistT;" + "P05_DistTR;" + "P05_DistTO;" +
                                      "P06_DistT;" + "P06_DistTR;" + "P06_DistTO;" +
                                      "P07_DistT;" + "P07_DistTR;" + "P07_DistTO;" +
                                      "P08_DistT;" + "P08_DistTR;" + "P08_DistTO;" +
                                      "P09_DistT;" + "P09_DistTR;" + "P09_DistTO;" +
                                      "P10_DistT;" + "P10_DistTR;" + "P10_DistTO;" +
                                      "P11_DistT;" + "P11_DistTR;" + "P11_DistTO;" +
                                      "P12_DistT;" + "P12_DistTR;" + "P12_DistTO;" +
                                      "P13_DistT;" + "P13_DistTR;" + "P13_DistTO;" +
                                      "P14_DistT;" + "P14_DistTR;" + "P14_DistTO;" +
                                      "P15_DistT;" + "P15_DistTR;" + "P15_DistTO;" +
                                      "P16_DistT;" + "P16_DistTR;" + "P16_DistTO;" +
                                      "P17_DistT;" + "P17_DistTR;" + "P17_DistTO;" +
                                      "P18_DistT;" + "P18_DistTR;" + "P18_DistTO;" +
                                      "P19_DistT;" + "P19_DistTR;" + "P19_DistTO;" +
                                      "P20_DistT;" + "P20_DistTR;" + "P20_DistTO;" +
                                      "P21_DistT;" + "P21_DistTR;" + "P21_DistTO;" +
                                      "P22_DistT;" + "P22_DistTR;" + "P22_DistTO;" +
                                      "P23_DistT;" + "P23_DistTR;" + "P23_DistTO;" +
                                      "P24_DistT;" + "P24_DistTR;" + "P24_DistTO;" +
                                      "P25_DistT;" + "P25_DistTR;" + "P25_DistTO;" +
                                      "P26_DistT;" + "P26_DistTR;" + "P26_DistTO;" +
                                      "P27_DistT;" + "P27_DistTR;" + "P27_DistTO;" +
                                      "P28_DistT;" + "P28_DistTR;" + "P28_DistTO;" +
                                      "P29_DistT;" + "P29_DistTR;" + "P29_DistTO;" +
                                      "P30_DistT;" + "P30_DistTR;" + "P30_DistTO;" +
                                      "P31_DistT;" + "P31_DistTR;" + "P31_DistTO;" +
                                      "P32_DistT;" + "P32_DistTR;" + "P32_DistTO;" +
                                      "P33_DistT;" + "P33_DistTR;" + "P33_DistTO;" +
                                      "P34_DistT;" + "P34_DistTR;" + "P34_DistTO;" +
                                      "P35_DistT;" + "P35_DistTR;" + "P35_DistTO;" +
                                      "P36_DistT;" + "P36_DistTR;" + "P36_DistTO;" +
                                      "P37_DistT;" + "P37_DistTR;" + "P37_DistTO;" +
                                      "P38_DistT;" + "P38_DistTR;" + "P38_DistTO;" +
                                      "P39_DistT;" + "P39_DistTR;" + "P39_DistTO;" +
                                      "P40_DistT;" + "P40_DistTR;" + "P40_DistTO;" +
                                      "P41_DistT;" + "P41_DistTR;" + "P41_DistTO;" +
                                      "P42_DistT;" + "P42_DistTR;" + "P42_DistTO;" +
                                      "P43_DistT;" + "P43_DistTR;" + "P43_DistTO;" +
                                      "P44_DistT;" + "P44_DistTR;" + "P44_DistTO;" +
                                      "P45_DistT;" + "P45_DistTR;" + "P45_DistTO;" +
                                      "P46_DistT;" + "P46_DistTR;" + "P46_DistTO;" +
                                      "P47_DistT;" + "P47_DistTR;" + "P47_DistTO;" +
                                      "P48_DistT;" + "P48_DistTR;" + "P48_DistTO;" +
                                      "P49_DistT;" + "P49_DistTR;" + "P49_DistTO;" +
                                      "P50_DistT;" + "P50_DistTR;" + "P50_DistTO";
                                     
                                      
                                       
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "设备各点平均距离;设备各点的最小距离;最小距离所在的点号;设备各点的最大距离;最大距离所在的点号;"+
                                        "第1点距离;第1点距离比;第1点距离排名;" +
                                        "第2点距离;第2点距离比;第2点距离排名;" +
                                        "第3点距离;第3点距离比;第3点距离排名;" +
                                        "第4点距离;第4点距离比;第4点距离排名;" +
                                        "第5点距离;第5点距离比;第5点距离排名;" +
                                        "第6点距离;第6点距离比;第6点距离排名;" +
                                        "第7点距离;第7点距离比;第7点距离排名;" +
                                        "第8点距离;第8点距离比;第8点距离排名;" +
                                        "第9点距离;第9点距离比;第9点距离排名;" +
                                        "第10点距离;第10点距离比;第10点距离排名;" +
                                        "第11点距离;第11点距离比;第11点距离排名;" +
                                        "第12点距离;第12点距离比;第12点距离排名;" +
                                        "第13点距离;第13点距离比;第13点距离排名;" +
                                        "第14点距离;第14点距离比;第14点距离排名;" +
                                        "第15点距离;第15点距离比;第15点距离排名;" +
                                        "第16点距离;第16点距离比;第16点距离排名;" +
                                        "第17点距离;第17点距离比;第17点距离排名;" +
                                        "第18点距离;第18点距离比;第18点距离排名;" +
                                        "第19点距离;第19点距离比;第19点距离排名;" +
                                        "第20点距离;第20点距离比;第20点距离排名;" +
                                        "第21点距离;第21点距离比;第21点距离排名;" +
                                        "第22点距离;第22点距离比;第22点距离排名;" +
                                        "第23点距离;第23点距离比;第23点距离排名;" +
                                        "第24点距离;第24点距离比;第24点距离排名;" +
                                        "第25点距离;第25点距离比;第25点距离排名;" +
                                        "第26点距离;第26点距离比;第26点距离排名;" +
                                        "第27点距离;第27点距离比;第27点距离排名;" +
                                        "第28点距离;第28点距离比;第28点距离排名;" +
                                        "第29点距离;第29点距离比;第29点距离排名;" +
                                        "第30点距离;第30点距离比;第30点距离排名;" +
                                        "第31点距离;第31点距离比;第31点距离排名;" +
                                        "第32点距离;第32点距离比;第32点距离排名;" +
                                        "第33点距离;第33点距离比;第33点距离排名;" +
                                        "第34点距离;第34点距离比;第34点距离排名;" +
                                        "第35点距离;第35点距离比;第35点距离排名;" +
                                        "第36点距离;第36点距离比;第36点距离排名;" +
                                        "第37点距离;第37点距离比;第37点距离排名;" +
                                        "第38点距离;第38点距离比;第38点距离排名;" +
                                        "第39点距离;第39点距离比;第39点距离排名;" +
                                        "第40点距离;第40点距离比;第40点距离排名;" +
                                        "第41点距离;第41点距离比;第41点距离排名;" +
                                        "第42点距离;第42点距离比;第42点距离排名;" +
                                        "第43点距离;第43点距离比;第43点距离排名;" +
                                        "第44点距离;第44点距离比;第44点距离排名;" +
                                        "第45点距离;第45点距离比;第45点距离排名;" +
                                        "第46点距离;第46点距离比;第46点距离排名;" +
                                        "第47点距离;第47点距离比;第47点距离排名;" +
                                        "第48点距离;第48点距离比;第48点距离排名;" +
                                        "第49点距离;第49点距离比;第49点距离排名;" +
                                        "第50点距离;第50点距离比;第50点距离排名"                                 
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
        //3个输入参数标签顺序：考核标签、参考标签1（期望曲线X轴对应标签）、参考标签2（期望曲线Y轴对应标签）
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
        /// 计算模块算法实现:依据期望曲线求期望值、偏差、偏差比，在依据记分曲线求得分
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>计算结果分别为OPX;OPXE;OPXErate;OPXW</returns>

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
            List<PValue>[] results = new List<PValue>[155];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入:输入数据为设备上所有单点一个小时内的分钟过滤值。一次取一个小时周期，也就是每个测点，有61个值，60个是分钟均值，1个是截止时刻值 
                double[][] InputsOnMinute = new double[60][];               //计算时是将设备上所有点，按每分钟一次去比较和计算。因此需要将点重新按照1个小时中的每分钟来组织。一共60分钟的数据
                for (i = 0; i < InputsOnMinute.Length; i++)
                {
                    InputsOnMinute[i] = new double[inputs.Length];          //每分钟的数组中依次放置每个点的数据；
                }
                bool[] calcuInvalidFlag=new bool[inputs.Length];            //每个点数据是否正常的状态位。若某点60个分钟数据中，有一个不正常，则该点的状态位就置位。
                string[] tagnames = calcuinfo.sourcetagname.Split(';');     //各管子名称。为了从管号中获取位置信息。


                //inputs[6][0].Status = 10;     //用于测试状态不正常的时候

                //0.1、输入处理：输入长度。当输入为空或者长度为0时，给出标志位为StatusConst.InputIsNull的计算结果.
                if (inputs == null || inputs.Length == 0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                if (inputs.Length > 50)
                {
                    _errorFlag = true;
                    _errorInfo = "输入数据超过50个，该算法支持最大50个温度测点的输入数据！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);//不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //该算法下对源数据的特殊要求：
                //——对于一个温度点，如果进来的数据不足60个（加截止时刻值共61个），说明计算引擎有问题。正常情况下，无论数据怎么样，应该有60个数据。因此如果出现某个点数据不足60个，直接给出错误，并退出当前计算。
                //——对于一个温度点，如果60个数据中有一个状态位不正常，则该点所有值均用float.Min替代，并置对应的标志位。
                for (i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] != null && inputs[i].Count==61)     //特别注意这里要求每一个长度必须是61。因为计算是每分钟内的各点在一起计算，如果长度不统一，就没法在时间上对齐
                    {
                        for (int j = 0; j < inputs[i].Count-1; j++)   //将该点的60个分钟值，分别放进对应的分钟数组。不取截止数据
                        {
                            if (inputs[i][j].Status == 0)              //如果该点，分钟均值状态正常，则放入对应分钟数组realcalcuinputs[j]的对应位置
                                InputsOnMinute[j][i] = inputs[i][j].Value;
                            else
                            {                                           //如果该点，有一个分钟均值状态不正常，则该点60分钟均值均置为最小值
                                for (int k = 0; k < 60; k++)
                                {
                                    InputsOnMinute[k][i] = float.MinValue;
                                }
                                calcuInvalidFlag[i] = true;             //该点对应的状态标志位
                                break;
                            }
                        }
                    }
                    else
                    {
                        //如果发现有点数据为null或者不足61个，说明计算引擎一定有问题，直接报错退出，不再计算
                        _errorFlag = true;
                        _errorInfo = "源数据中有设备点一个小时的数据不足60个，请检查计算数据。";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                }

                //用于计算的数据realcalcuinputs  
                //——第一维对应着分钟数，共60分钟。如果某一个点少于60分种数据，直接退出
                //——第二维对应着设备上的所有点。如果某一个点有一个数据状态位不正常，该点所有值均置为浮点数最小值。

                //定义计算结果
                double EquDistTAvg = 0;             //设备个点均值
                double EquDistTMin = 0;             //设备个点的距均值的最小距离
                double EquDistTMinN = 0;            //最小距离所在点编号                
                double EquDistTMax = 0;             //设备个点的距均值的最大距离
                double EquDistTMaxN = 0;            //最大距离所在点编号

                double[] EquPointDistT=new double[inputs.Length];            //各点每分钟距中位值距离平方在一个小时上的累积平均
                double[] EquPointDistTR = new double[inputs.Length];         //各点距离比，各点距离EquPointDistT除设备平均距离EquDistTAvg
                double[] EquPointIndex = new double[inputs.Length];          //个点距离排名
                
                //计算中间变量
                double[] Median = new double[60];           //每分钟里的中位数
                double[][] PointDistT = new double[60][];   //每分钟里各点过滤值距中位值距离

                int ValidDataNumber = 0;                    //设备中有效的点的数量。某一个点只要在60分钟内某一分钟无效，则该点就视为无效
                for (i = 0; i < calcuInvalidFlag.Length; i++)
                {
                    if (calcuInvalidFlag[i] == false) ValidDataNumber += 1;
                }
                if (ValidDataNumber == 0)
                {
                    //如果发现有点数据为null或者不足61个，说明计算引擎一定有问题，直接报错退出，不再计算
                    _warningFlag = true;
                    _warningInfo = " 当前设备一个小时的数据中所有点均含有非正常状态点，请检查计算数据。";        //这种情况下推出计算
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                double valuesum = 0;        //求和变量
                List<double> valueall=new List<double>() ;         //排序数据
                double[] valueallArray;
                         
                
                //计算每分钟点的中位数，然后计算各点到中位数的距离
                for (i = 0; i < 60; i++)                 //依次计算每一分钟
                {                    
                    //找中位数
                    valueall = new List<double>();                 //找中位数的时候仅考虑有效数据   
                    for (int j = 0; j < inputs.Length; j++)
                    {
                        if (InputsOnMinute[i][j] != float.MinValue) valueall.Add(InputsOnMinute[i][j]);     //当realcalcuinputs[i][j]为最小浮点值，则不参与中位数计算
                    }
                    valueallArray = valueall.ToArray();
                    Array.Sort(valueallArray);
                    int len = ValidDataNumber;
                    if (len % 2 == 0)
                        Median[i] = (valueall[len / 2 - 1] + valueall[len / 2]) / 2;                    //比如，一共6个元素，如果下标从0开始，就是[2]+[3]
                    else
                        Median[i] = valueall[(int)Math.Floor(len / 2.0)];                               //比如，一共7个元素，就是第4个元素

                    //计算各点到中位数距离：计算中位数时要排除值为float.min的状态异常点，但计算距离时，则都计算。
                    PointDistT[i] = new double[inputs.Length];              //每分钟按实际点数来计算，包括非正常点也计算(float.MinValue是一个很小的负数！！！)
                    for (int j = 0; j < inputs.Length; j++)
                    {
                        PointDistT[i][j] = InputsOnMinute[i][j] - Median[i];                            //PointDistT第一维是分钟数，第二维是点序号
                    }
                }

                /* 以下理解错误
                //***计算设备整体参数***
                //计算设备小时特征值算法：1、先计算每个测点在60分钟上的中位数。2、再求这些中位数的均值
                double[] Median4Point = new double[inputs.Length];
                for (i = 0; i < inputs.Length; i++)         //依次计算每一个输入点(包括无效点)
                {
                    //找中位数
                    if (calcuInvalidFlag[i] == true)        //如果是无效点，直接给Median4Point[i]为最小浮点值
                    {
                        Median4Point[i] = float.MinValue;
                        continue;
                    }
                    else
                    {
                        valueall = new List<double>();
                        for (int j = 0; j < 60; j++)
                        {
                            valueall.Add(InputsOnMinute[j][i]);
                        }
                        valueallArray = valueall.ToArray();
                        Array.Sort(valueallArray);
                        int len = 60;
                        if (len % 2 == 0)
                            Median4Point[i] = (valueall[len / 2 - 1] + valueall[len / 2]) / 2;                    //比如，一共6个元素，如果下标从0开始，就是[2]+[3]
                        else
                            Median4Point[i] = valueall[(int)Math.Floor(len / 2.0)];                               //比如，一共7个元素，就是第4个元素
                    }
                }
                valuesum = 0;
                for (i = 0; i < Median4Point.Length; i++)
                {
                    if (Median4Point[i] != float.MinValue) valuesum = valuesum + Median4Point[i];                   //求均值时，仅求有效点的中位数均值。无效点不参加累积。求平均时，也不计入总数
                }
                EquDistTAvg = valuesum / ValidDataNumber;                                                           //求均值时，仅求有效点的中位数均值
                */

                //***计算设备上每个点参数***
                //计算每个点在60分钟内每分钟对中位数累积的距离、距离比、排名
                valueall = new List<double>();          //要计算排名的数组，只对有效点进行排名
                double valuesumDisT = 0;                //用来统计个点距离的总和，以便求平均距离
                for (i = 0; i < inputs.Length; i++)             //按每点分别累积
                {
                    //累积距离、距离比
                    if (calcuInvalidFlag[i] == false)           //如果该点有效
                    {
                        valuesum = 0;
                        for (int j = 0; j < 60; j++)                          //当前点，各分钟累积。
                            valuesum = valuesum + Math.Pow(PointDistT[j][i], 2);

                        //距离
                        EquPointDistT[i] = Math.Sqrt(valuesum / ValidDataNumber);               //每个点的距离计算。不将无效点计算在内
                        //个点距离总和
                        valuesumDisT = valuesumDisT + EquPointDistT[i];                        
                        //将累积距离放入排序数组
                        valueall.Add(EquPointDistT[i]);
                    }
                    else
                    {
                        EquPointDistT[i] = float.MinValue;                        
                    }
                    
                }
                //个点距离比、排名
                EquDistTAvg = valuesumDisT / ValidDataNumber;
                valueallArray = valueall.ToArray();
                Array.Sort(valueallArray);
                for (i = 0; i <inputs.Length; i++)
                {
                    if (calcuInvalidFlag[i] == false)
                    {
                        EquPointIndex[i] = Array.IndexOf(valueallArray, EquPointDistT[i]) + 1;     //有效点取正常排名
                        if (EquDistTAvg != 0) EquPointDistTR[i] = EquPointDistT[i] / EquDistTAvg;
                        else EquPointDistTR[i] = 0;
                    }
                    else
                    {
                        EquPointIndex[i] = -1;         //无效点排名取-1（正常情况排名不会出现-1）
                        EquPointDistTR[i] = float.MinValue;         //无效点距离比取0（正常情况距离比可能出现0）
                    }
                }

                //找最大最小和管号
                int firstIndex = Array.IndexOf(calcuInvalidFlag, false);
                EquDistTMin = EquPointDistT[firstIndex];
                EquDistTMax = EquPointDistT[firstIndex];
                try { EquDistTMinN = double.Parse(tagnames[firstIndex].Substring(7, 2)); }
                catch { };
                try { EquDistTMaxN = double.Parse(tagnames[firstIndex].Substring(7, 2)); }
                catch { };
                for (i = 0; i < inputs.Length; i++)
                {
                    if(calcuInvalidFlag[i]==false)
                    {                        
                        if (EquPointDistT[i] < EquDistTMin)
                        {
                            EquDistTMin = EquPointDistT[i];
                            try { EquDistTMinN = double.Parse(tagnames[i].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", tagnames[i]);
                            }
                        }
                        if (EquPointDistT[i] > EquDistTMax)
                        {
                            EquDistTMax = EquPointDistT[i];
                            try { EquDistTMaxN = double.Parse(tagnames[i].Substring(7, 2)); }
                            catch
                            {
                                _errorFlag = true;
                                _errorInfo = String.Format("计算标签{0}第7、8位应该是数字，表示温度所在管子的位置。请检查！", tagnames[i]);
                            }
                        }
                    }
                }

                //组织计算结果
                results[0] = new List<PValue>();
                results[0].Add(new PValue(EquDistTAvg, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[1] = new List<PValue>();
                results[1].Add(new PValue(EquDistTMin, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[2] = new List<PValue>();
                results[2].Add(new PValue(EquDistTMinN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[3] = new List<PValue>();
                results[3].Add(new PValue(EquDistTMax, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[4] = new List<PValue>();
                results[4].Add(new PValue(EquDistTMaxN, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                long status = 0;
                for (i = 0; i < inputs.Length; i++) //最大可以接受50个点，但按实际点数给个点计算结果赋值
                {
                    if (calcuInvalidFlag[i] == true) status = (long)StatusConst.InvalidPoint;
                    results[i * 3 + 5] = new List<PValue>();
                    results[i * 3 + 5].Add(new PValue(EquPointDistT[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));
                    
                    results[i * 3 + 1 + 5] = new List<PValue>();
                    results[i * 3 + 1 + 5].Add(new PValue(EquPointDistTR[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));

                    results[i * 3 + 2 + 5] = new List<PValue>();
                    results[i * 3 + 2 + 5].Add(new PValue(EquPointIndex[i], calcuinfo.fstarttime, calcuinfo.fendtime, status));
                }
                
                //只返回与点数相符的计算结果项。计算引擎已经修改为按照实际返回结果来写数据。
                results = results.Take(5 + inputs.Length*3).ToArray();

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
    }
}
