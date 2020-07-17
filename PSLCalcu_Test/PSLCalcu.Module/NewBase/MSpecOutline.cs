using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    /// <summary>
    /// 此算法为多输入算法
    /// </summary>
    public class MSpecOutline : BaseModule, IModule, IModuleExPara
    {
        static LogHelper logHelper = LogFactory.GetLogger(typeof(MSpecOutline));                     //全局log
        private String _moduleName = "MSpecOutline"; //计算模块名称
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc="计算各项指标轮廓线"; //计算模块描述
        public string moduleDesc
        {
            get {
                return _moduleDesc;
            }
        }
        private int _inputNumber;//输入参数数量
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private String _inputDescsCN;
        public string inputDescsCN
        {
            get { return _inputDescsCN; }
        }
        //算法计算结果
        //平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差);和;绝对值和;最大值最小值点号差;最大差除以点号差;最凸出值;最凸出点号;最凹陷值;最凹陷点号;翻转次数;≥高限的点数;≥高限的点数占总点数之比%;＜低限的点数;＜低限的点数占总点数之比%;中位值;中位值所在点号;线性模型斜率K;线性模型截距B;线性模型有效性R;二次项系数 A;一次项系数 B;常数项 C;二次模型有效性R
        private string _algorithms = "SpecAvg;SpecMin;SpecMinN;SpecMax;SpecMaxN;SpecStdev;SpecDmax;SpecSum;SpecSumABS;SpecDmaxDN;SpecDmaxRSpecTop;SpecTopN;SpecDown;SpecDownN;SpecTurnN;SpecHighN;SpecHighR;SpecLowN;SpecLowR;SpecMedian;SpecMedianN;SpecLinearK;SpecLinearB;SpecLinearR;SpecQquadraticA;SpecQquadraticB;SpecQquadraticC;SpecQquadraticR";
        public string algorithms
        {
            get { return _algorithms; }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get { return _algorithmsflag; }
        }
        private int _outputNumber=29; //输出数量
        public int outputNumber
        {
            get { return _outputNumber; }
        }
        private string _outputDescs = "SpecAvg;SpecMin;SpecMinN;SpecMax;SpecMaxN;SpecStdev;SpecDmax;SpecSum;SpecSumABS;SpecDmaxDN;SpecDmaxR;SpecTop;SpecTopN;SpecDown;SpecDownN;SpecTurnN;SpecHighN;SpecHighR;SpecLowN;SpecLowR;SpecMedian;SpecMedianN;SpecLinearK;SpecLinearB;SpecLinearR;SpecQquadraticA;SpecQquadraticB;SpecQquadraticC;SpecQquadraticR";
        public string outputDescs
        {
            get { return _outputDescs; }
        }
        private string _outputDescsCN = "平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差);和;绝对值和;最大值最小值点号差;最大差除以点号差;最凸出值;最凸出点号;最凹陷值;最凹陷点号;翻转次数;≥高限的点数;≥高限的点数占总点数之比%;＜低限的点数;＜低限的点数占总点数之比%;中位值;中位值所在点号;线性模型斜率K;线性模型截距B;线性模型有效性R;二次项系数 A;一次项系数 B;常数项 C;二次模型有效性R";
        public string outputDescsCN
        {
            get { return _outputDescsCN; }
        }
        private string _outputTable = "plsdata";
        public string outputTable
        {
            get { return _outputTable; }
        }
        private bool _outputPermitNULL = false;
        public bool outputPermitNULL
        {
            get
            {
                return _outputPermitNULL;
            }
        }
        #region 输入参数
        private string _moduleParaExample = "20;30";    // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "high;low";    //高限 低限 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
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
        /// 计算模块算法实现:平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差);和;绝对值和
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara"></param>       
        /// <returns>计算结果分别为平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差)</returns>
        /// 

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
            List<PValue>[] results = new List<PValue>[29];   //平均值;最小值;最小值所在的点号;最大值;最大值所在的点号;最均差;最大差(最大值与最小值得差);和;绝对值和
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入
                List<PValue> input ;
                //0.1、输入处理：输入长度。当输入为空时，则输出项也为空.
                if (inputs == null || inputs.Length == 0 || inputs[0] == null){
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }else{
                    input = new List<PValue>();
                    //应当是每个输入只有一个值
                    for (int j = 0; j < inputs.Length; j++) {
                        input.Add(inputs[j][0]);
                    }

                }

                //参数处理
                double high;
                double low;
                string[] paras = calcuinfo.fparas.Split(';');
                high = float.Parse(paras[0]);
                low = float.Parse(paras[1]);

                //组织点号
                Dictionary<uint, int> pointNumDic = new Dictionary<uint, int>();
                for (int j = 0; j < calcuinfo.fsourtagids.Length; j++) {
                    pointNumDic.Add(calcuinfo.fsourtagids[j], (j + 1));
                }
                #region 计算结果初始化 SpecAvg;SpecMin;SpecMinN;SpecMax;SpecMaxN;SpecStdev;SpecDmax
                PValue SpecAvg = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //平均值; 1
                PValue SpecMin = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //最小值; 2
                PValue SpecMinN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //最小值所在的点号;3
                PValue SpecMax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //最大值;4
                PValue SpecMaxN = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //最大值所在的点号;5
                PValue SpecStdev = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//均差;6
                PValue SpecDmax = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //最大差(最大值与最小值得差)7
                PValue SpecSum= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//和;8
                PValue SpecSumABS= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//绝对值和;9
                PValue SpecDmaxDN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//最大值最小值点号差;10
                PValue SpecDmaxR= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//最大差除以点号差;11
                PValue SpecTop= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//最凸出值;12
                PValue SpecTopN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//最凸出点号;13
                PValue SpecDown= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//最凹陷值;14
                PValue SpecDownN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//最凹陷点号;15
                PValue SpecTurnN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //翻转次数;16
                PValue SpecHighN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //≥高限的点数;17
                PValue SpecHighR= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//≥高限的点数占总点数之比%;18
                PValue SpecLowN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//＜低限的点数;19
                PValue SpecLowR= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//＜低限的点数占总点数之比%;20
                PValue SpecMedian= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//中位值;21
                PValue SpecMedianN= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//中位值所在点号;22
                PValue SpecLinearK= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//线性模型斜率K;23
                PValue SpecLinearB= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);  //线性模型截距B;24
                PValue SpecLinearR= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0); //线性模型有效性R;25
                PValue SpecQquadraticA= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//二次项系数 A;26
                PValue SpecQquadraticB= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//一次项系数 B;27
                PValue SpecQquadraticC= new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//常数项 C;28
                PValue SpecQquadraticR = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//二次模型有效性R29

                results = new List<PValue>[29];
                int len = input.Count;
                if (len<2) {
                    logHelper.Info("数据长度不正常：" + calcuinfo.fsourtagids[0]);
                }
                //TimeSpan ts = calcuinfo.fendtime - calcuinfo.fstarttime;
                //Console.WriteLine("请输入计算代数累加复位次数所需参数累加限值（按回车键结束输入）:\n");
                //calcuinfo.fparas = Console.ReadLine();
                string[] para = calcuinfo.fparas.Split(new char[] { ',' });
                #endregion


                #region 计算主体
                //最大值
                PValue avg = BaseCalcu.getAvg(input);  //平均值 
                List<PValue> minMaxList = BaseCalcu.getMaxMin(input);
                PValue std = BaseCalcu.getStd(input);  //均差
                SpecAvg.Value = avg.Value;
                SpecMin.Value = minMaxList[2].Value;
                SpecMinN.Value = minMaxList[3].Value;
                SpecMax.Value = minMaxList[0].Value;
                SpecMaxN.Value = minMaxList[1].Value;
                SpecStdev.Value = std.Value;             //均差;6
                SpecDmax.Value = SpecMax.Value - SpecMin.Value;  //最大差(最大值与最小值得差)7
                PValue sum = BaseCalcu.getSum(input);
                SpecSum.Value = sum.Value;       //和;8
                PValue absSum=BaseCalcu.getAbsSum(input);
                SpecSumABS.Value = absSum.Value; //绝对值和;9
                SpecDmaxDN.Value = SpecMaxN.Value - SpecMinN.Value;//最大值最小值点号差;10
                SpecDmaxR.Value = SpecDmax.Value / SpecDmaxDN.Value;//最大差除以点号差;11
                List<PValue> topdown = BaseCalcu.getTopDown(input);
                SpecTop.Value = topdown[0].Value;//最凸出值;12
                SpecTopN.Value = topdown[1].Value;//最凸出点号;13
                SpecDown.Value = topdown[2].Value;//最凹陷值;14
                SpecDownN.Value = topdown[3].Value;//最凹陷点号;15
                SpecTurnN.Value = BaseCalcu.getTurnN(input).Value;//翻转次数;16
                List<PValue> highlowlist = BaseCalcu.getHighLow(input,high,low);
                SpecHighN.Value = highlowlist[0].Value;//≥高限的点数;17
                SpecHighR.Value = highlowlist[2].Value;//≥高限的点数占总点数之比%;18
                SpecLowN.Value = highlowlist[1].Value;//＜低限的点数;19
                SpecLowR.Value = highlowlist[3].Value;//＜低限的点数占总点数之比%;20
                List<PValue> medain = BaseCalcu.getMedian(input);
                SpecMedian.Value = medain[0].Value;//中位值;21
                SpecMedianN.Value = medain[1].Value;//中位值所在点号;22
                List<PValue> models = BaseCalcu.getModel(input);
                SpecLinearK.Value = models[0].Value;//线性模型斜率K;23
                SpecLinearB.Value = models[1].Value;//线性模型截距B;24
                SpecLinearR.Value = models[2].Value;//线性模型有效性R;25
                SpecQquadraticA.Value = models[3].Value;//二次项系数 A;26
                SpecQquadraticB.Value = models[4].Value;//一次项系数 B;27
                SpecQquadraticC.Value = models[5].Value;//常数项 C;28
                SpecQquadraticR.Value = models[6].Value;//二次模型有效性R29
                #endregion
                #region  组织输出
                results[0] = new List<PValue>();
                results[0].Add(SpecAvg);
                results[1] = new List<PValue>();
                results[1].Add(SpecMin);
                results[2] = new List<PValue>();
                results[2].Add(SpecMinN);
                results[3] = new List<PValue>();
                results[3].Add(SpecMax);
                results[4] = new List<PValue>();
                results[4].Add(SpecMaxN);
                results[5] = new List<PValue>();
                results[5].Add(SpecStdev);
                results[6] = new List<PValue>();
                results[6].Add(SpecDmax);
                results[7] = new List<PValue>();
                results[7].Add(SpecSum);
                results[8] = new List<PValue>();
                results[8].Add(SpecSumABS);
                results[9] = new List<PValue>();
                results[9].Add(SpecDmaxDN);
                results[10] = new List<PValue>();
                results[10].Add(SpecDmaxR);
                results[11] = new List<PValue>();
                results[11].Add(SpecTop);
                results[12] = new List<PValue>();
                results[12].Add(SpecTopN);
                results[13] = new List<PValue>();
                results[13].Add(SpecDown);
                results[14] = new List<PValue>();
                results[14].Add(SpecDownN);
                results[15] = new List<PValue>();
                results[15].Add(SpecTurnN);
                results[16] = new List<PValue>();
                results[16].Add(SpecHighN);
                results[17] = new List<PValue>();
                results[17].Add(SpecHighR);
                results[18] = new List<PValue>();
                results[18].Add(SpecLowN);
                results[19] = new List<PValue>();
                results[19].Add(SpecLowR);
                results[20] = new List<PValue>();
                results[20].Add(SpecMedian);
                results[21] = new List<PValue>();
                results[21].Add(SpecMedianN);
                results[22] = new List<PValue>();
                results[22].Add(SpecLinearK);
                results[23] = new List<PValue>();
                results[23].Add(SpecLinearB);
                results[24] = new List<PValue>();
                results[24].Add(SpecLinearR);
                results[25] = new List<PValue>();
                results[25].Add(SpecQquadraticA);
                results[26] = new List<PValue>();
                results[26].Add(SpecQquadraticB);
                results[27] = new List<PValue>();
                results[27].Add(SpecQquadraticC);
                results[28] = new List<PValue>();
                results[28].Add(SpecQquadraticR);
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
