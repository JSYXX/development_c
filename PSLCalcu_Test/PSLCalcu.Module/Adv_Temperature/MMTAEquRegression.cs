using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //使用log
using System.Linq;                      //对list进行操作
using Accord.Statistics.Models.Regression.Linear;

namespace PSLCalcu.Module
{
    /// <summary>
    /// 壁温分析设备模型分析——回归分析   
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
    public class MMTAEquRegression : BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MMTAEquRegression";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "设备超限统计算法";
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
        private string _algorithms = "MMTAEquRegression";
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
        private string _outputDescs = "EquRLK;" +                    //1               
                                    "EquRLB;" +
                                    "EquRLR;" +
                                    "EquRQA;" +
                                    "EquRQB;" +
                                    "EquRQC;" +
                                    "EquRQR"                         //7                  
                                    ;
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "壁温分析线性回归K;" +
                                        "壁温分析线性回归B;" +
                                        "壁温分析线性回归残差R;" +
                                        "壁温分析二次多项式回归A;" +
                                        "壁温分析二次多项式回归B;" +
                                        "壁温分析二次多项式回归C;" +
                                        "壁温分析二次多项式回归残差R"                                       
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
                //——MMTAAnaBase对偏差的计算结果，有19个值，一个设备100个温度点，那么也就1900个点。readmulti读取速度不会太慢。
                //——本计算仅用到19个计算结果中的均值，但是为了配置方便，仍然按MMTAAnaBase计算结果全部取得方式
                int MMTAAnaBaseOutputNumber = 19;   //本算法是直接读取MMTAAnaBase算法所有计算结果。MMTAAnaBase算法有19个计算结果。要靠次参数完成对计算结果的解析。
                int validDataNumber = 0;
                //0、输入：输入是同一设备上所有温度点的小时均值，或者天均值
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值。整体为空计算引擎已经记录报警或错误。
                }
                //0.2、输入处理：截止时刻值。该算法不需要截止时刻点参与计算。
                //输入的是该设备上所有温度测点每小时基本统计值，每个统计值也就是每个测点，仅有两个值，一个是小时值，一个是截止时刻值                
                string[] sourtagname = calcuinfo.sourcetagname.Split(';');              //本算法是做管子位置与温度的回归。管子的位置需要从标签名称中取
                List<List<PValue>> AvgInputsList = new List<List<PValue>>();            //将MMTAAnaBase算法结果中的均值取出来
                List<string> AvgtagnameList = new List<string>();                       //将均值对应的标签名称取出来
                for (i = 0; i < inputs.Length; i++)
                {
                    //按MMTAAnaBase算法,将均值（MMTAAnaBaseOutputNumber==1 ）取出
                    //对于设备来说，某些单点状态位异常，直接过滤而不进行报警。只有这一类计算的结果全部异常才报警。

                    if (i % MMTAAnaBaseOutputNumber == 1 && inputs[i] != null && inputs[i].Count >= 2 && inputs[i][0].Status == 0)
                    {
                        //找到所有有效均值的标签名
                        AvgtagnameList.Add(sourtagname[i]);

                        //找到数据
                        //if (AvgInputsList[i / MMTAAnaBaseOutputNumber] == null)
                            AvgInputsList.Add(new List<PValue>());
                        
                        //只取第一个值
                        AvgInputsList[validDataNumber].Add(new PValue(inputs[i][0].Value, inputs[i][0].Timestamp, inputs[i][0].Endtime, inputs[i][0].Status));

                        validDataNumber = validDataNumber + 1;
                    }    
                }//end for
                string[]  Avgtagname = AvgtagnameList.ToArray();            //有效点不同，标签数量不同，删除后面的空位。
                List<PValue>[] AvgInputs = AvgInputsList.ToArray();     //与标签点对应添加的有效数据
                if (validDataNumber < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "当前时间段过滤非正常状态点后，全部点的均值有效个数少于2个，无法进行回归计算！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
               
                //定义变量
                double MTAEquRLK=0;
                double MTAEquRLB=0;
                double MTAEquRLR=0;
                double MTAEquRQA=0;
                double MTAEquRQB=0;
                double MTAEquRQC=0;
                double MTAEquRQR=0;

                //计算
                //1、线性回归
                //参考 http://accord-framework.net/docs/html/T_Accord_Statistics_Models_Regression_Linear_SimpleLinearRegression.htm
                List<double> ArrayX=new List<double>();         //回归分析，自变量是管号
                List<double> ArrayY = new List<double>();       //回归分析，应变量是温度
                for(i=0;i<AvgInputs.Length;i++)
                {
                    double xposition=double.Parse(Avgtagname[i].Substring(7, 2));
                    for (int j = 0; j < AvgInputs[i].Count; j++)
                    {
                        ArrayX.Add(xposition);
                        ArrayY.Add(AvgInputs[i][j].Value);
                    }
                }

                /*
                //测试数据                
                ArrayX = new List<double> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                ArrayY = new List<double> { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
                //按上述测试数据，MTAEquRLK=2，MTAEquRLB=0，MTAEquRLRR=1              

                
                ArrayX = new double[10] { 1.2, 1.9, 3.1, 3.7, 5, 6, 7.1, 7.9, 9, 10 };
                ArrayY = new double[10] { 2.1, 4.2, 6.8, 8.5, 10, 12, 14.6, 14, 18, 20 };
                */
                //按上述测试数据，MTAEquRLK=1.911，MTAEquRLB=0.526，MTAEquRLRR=0.985

                double[] X = ArrayX.ToArray();
                double[] Y = ArrayY.ToArray(); ;

                OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
                SimpleLinearRegression regression = ols.Learn(X, Y);

                MTAEquRLK=regression.Slope;                                         //一次斜率
                MTAEquRLB=regression.Intercept;                                     //截距
                MTAEquRLR = regression.CoefficientOfDetermination(X, Y);            //R^2代表模型拟合程度

                //2、多项式回归
                //参考 http://accord-framework.net/docs/html/T_Accord_Statistics_Models_Regression_Linear_MultivariateLinearRegression.htm
                //MultiX =[X^2 ,X]
                double[][] MultiX = new double[X.Length][];
                for (i = 0; i < X.Length; i++)
                {
                    double x=X[i];
                    MultiX[i]=new double[]{x*x,x};
                }
                double[][] MultiY = new double[X.Length][];
                //MultiY =X'
                for (i = 0; i < X.Length; i++)
                {
                    double y = Y[i];
                    MultiY[i]=new double[]{y};
                }

                //测试数据
                /*
                MultiX = new double[5][];
                MultiX[0] = new double[2] { 1, 1};
                MultiX[1] = new double[2] { 4, 2};
                MultiX[2] = new double[2] { 9, 3};
                MultiX[3] = new double[2] { 16, 4};
                MultiX[4] = new double[2] { 25, 5};
                
                MultiY = new double[5][];
                MultiY[0] = new double[1] { 7 };
                MultiY[1] = new double[1] { 17 };
                MultiY[2] = new double[1] { 31 };
                MultiY[3] = new double[1] { 49 };
                MultiY[4] = new double[1] { 71 };
                */
                //按上述测试数据，MTAEquRQA=2，MTAEquRQB=4，MTAEquRQC=1，MTAEquRQR=1
                /*
                MultiX = new double[5][];
                MultiX[0] = new double[2] { 1, 1 };
                MultiX[1] = new double[2] { 4, 2 };
                MultiX[2] = new double[2] { 9, 3 };
                MultiX[3] = new double[2] { 16, 4 };
                MultiX[4] = new double[2] { 25, 5 };

                MultiY = new double[5][];
                MultiY[0] = new double[1] { 7.1 };
                MultiY[1] = new double[1] { 16.9 };
                MultiY[2] = new double[1] { 32 };
                MultiY[3] = new double[1] { 51 };
                MultiY[4] = new double[1] { 69 };
                */
                //按上述测试数据，MTAEquRQA=1.45，MTAEquRQB=7.08，MTAEquRQC=-2，MTAEquRQR=0.998

                OrdinaryLeastSquares olsMulti = new OrdinaryLeastSquares();
                MultivariateLinearRegression regressionMulti = olsMulti.Learn(MultiX,MultiY);

                double[][] weights=regressionMulti.Weights;
                MTAEquRQA=weights[0][0];                            //二次参数
                MTAEquRQB=weights[1][0];                            //一次参数
                MTAEquRQC = regressionMulti.Intercepts[0];          //截距
                MTAEquRQR=regressionMulti.CoefficientOfDetermination(MultiX,MultiY)[0];    //R^2代表模型拟合程度

                //组织计算结果
                results[0] = new List<PValue>();
                results[0].Add(new PValue(MTAEquRLK, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[1] = new List<PValue>();
                results[1].Add(new PValue(MTAEquRLB, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[2] = new List<PValue>();
                results[2].Add(new PValue(MTAEquRLR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[3] = new List<PValue>();
                results[3].Add(new PValue(MTAEquRQA, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[4] = new List<PValue>();
                results[4].Add(new PValue(MTAEquRQB, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[5] = new List<PValue>();
                results[5].Add(new PValue(MTAEquRQC, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

                results[6] = new List<PValue>();
                results[6].Add(new PValue(MTAEquRQR, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

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
