using PCCommon;
using PCCommon.NewCaculateCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class NewMDeviation2DS : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：二维偏差及得分计算(单值计算);

        private string _moduleName = "NewMDeviation2DS";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "二维偏差及得分计算(单值计算)。当参考值超出偏差曲线范围时，计算结果状态位为11，表示当次计算结果无效。";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 3;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "求偏差的标签;求期望值的参考标签1、求期望值的参考标签2";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "OPXCurveID;ScoreCurveID";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "D;1;1;1;0";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "偏差得分曲线读取方式(D/S);期望曲线ID;记分曲线ID";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([DS]){1}(;[+]?\d+){2}(;[+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 5;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "DevSP;DevErr;DevErrR;DevScore;DevScoreA";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "期望值(单值);偏差(单值);偏差比(单值);得分(单值)";
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
            List<PValue>[] results = new List<PValue>[5];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                //0、输入

                //0.1、输入处理：输入长度。当输入为空时，给出标志位为StatusConst.InputIsNull的计算结果.
                if (inputs == null || inputs.Length < 3)
                {
                    _errorFlag = true;
                    _errorInfo = "二维偏差及得分计算要求必须有且仅有三个有效输入。当前输入为空或输入数量不足。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[0] == null || inputs[1] == null || inputs[2] == null)
                {
                    _errorFlag = true;
                    _errorInfo = "二维偏差及得分计算要求三个输入均不为空。当前输入有空数据。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //0.2、输入处理：截止时刻值。该算法，截止时刻点不需要参与计算，要删除。
                //本算法做特别处理
                //if (input.Count > 1) input.RemoveAt(input.Count - 1);
                //0.3、输入处理：标志位。该算法考虑标志位不为0的情况，先过滤这些点。
                //********如果计算偏差前，参考量和被考核量，在计算的分钟周期内，分钟值为状态位无效的值。
                //********这里会读入这个值，并在下面过滤。如果被过滤，则下面的参考量和被考核量长度判断会报错。并返回不正常状态的计算结果。
                for (i = 0; i < inputs.Length; i++)
                {
                    for (int j = inputs[i].Count - 1; j >= 0; j--)
                    {
                        if (inputs[i][j].Status != 0) inputs[i].RemoveAt(j);
                    }
                }

                //0.4、输入处理：过滤后结果。
                //本算法做特别处理
                //——如果去除了截止时刻点，过滤后长度小于1（计算要求至少有一个有效数据），则直接返回全0
                //——如果没取除截止时刻点，过滤后长度小于2（计算要求至少有一个有效数据和一个截止时刻值）
                //if (input.Count < 1) return results;  

                //1、检查并整理数据              
                //——这里是计算每分钟的偏差。
                //——输入量是被评变量每分钟滤波值、参考变量每分钟滤波值
                if (inputs[0] == null || inputs[0].Count < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "二维偏差及得分计算，被测评量正常状态的数据为空";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[1] == null || inputs[1].Count < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "二维偏差及得分计算，第一个参考量正常状态的数据为空";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[2] == null || inputs[2].Count < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "二维偏差及得分计算，第二个参考量正常状态的数据为空";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //1.2、直接读取各变量第一个数据，不需要处理截止时刻值 

                PValue pvdata;
                PValue refdataX;
                PValue refdataY;

                pvdata = inputs[0][0];          //不使用截止值，被测评量
                refdataX = inputs[1][0];        //不使用截止值，参考量1
                refdataY = inputs[2][0];        //不使用截止值，参考量2

                StatusConst invalidflag = StatusConst.Normal;   //参考指标值是否超越偏差曲线x范围，当次指标考核计算是否有效

                //2、根据参数获取期望曲线、记分曲线
                string[] para = calcuinfo.fparas.Split(new char[] { ';' });
                string readMethod = para[0];        //第一个参数是获取曲线的方式，D为动态，S为静态。动态方式，每次计算均读取CSV。静态方式，只读取一次CSV。
                int opxid = int.Parse(para[1]);     //第二个参数是期望曲线序号
                int scoreid = int.Parse(para[2]);   //第三个参数是得分曲线序号
                double k = double.Parse(para[3]);
                double b = double.Parse(para[4]);
                Curve2D opxCurve;                   //偏差曲线是二维
                Curve1D scoreCurve;                 //得分曲线是一维
                string strTemp = CurveConfig.Instance.OPXCurvesReadStatus;  //这行这条语句是为了下面在使用CurveConfig.GetXXXCurve时保证Instance已被初始化。20181109调试发现该问题。

                try
                {
                    if (readMethod.ToUpper() == "S")
                    {   //静态读取，对于分钟计算来说，只有第一次才读取偏差曲线和得分曲线
                        opxCurve = CurveConfig.GetOPXCurve2D(opxid, calcuinfo.fstarttime);
                        scoreCurve = CurveConfig.GetScoreCurve(scoreid, calcuinfo.fstarttime);
                    }
                    else
                    {   //——20181031，因动态方式读取曲线，速度太慢，不在采用。
                        //——转而采用在静态方式下读取带生效时间的配置曲线。这样在增加配置曲线时，仅需要暂停计算引擎后重启，即可载入新的配置曲线，并继续进行计算。
                        //动态读取，对于分钟计算来说，每次计算都要读取偏差曲线和得分曲线
                        //opxCurve = CurveConfig.ReadOPXCurves2D(calcuinfo.fstarttime).Find(delegate(Curve2D a) { return a.Index == opxid; });
                        //scoreCurve = CurveConfig.ReadScoreCurves(calcuinfo.fstarttime).Find(delegate(Curve1D a) { return a.Index == scoreid; });
                        _errorFlag = true;
                        _errorInfo = "参数配置为D，动态读取配置曲线功能已经取消，请使用静态读取方式。";
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                }
                catch (Exception ex)
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("读取期望和得分曲线时错误！错误信息：{0}", ex.ToString());
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //判读所需要的曲线是否正确读取，如果没有正确读取一定是返回null，此时要给出错误log
                if (opxCurve == null)
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("期望曲线为空，对应的期望曲线：{0}-{1}获取错误！", opxid, calcuinfo.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (scoreCurve == null)
                {
                    _errorFlag = true;
                    _errorInfo = String.Format("得分曲线为空，对应的得分曲线：{0}-{1}获取错误！", scoreid, calcuinfo.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //3、计算偏差                
                PValue sp, err, errrate, score, wscore;       //期望值、偏差、偏差比、得分

                sp = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                err = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                errrate = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                score = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);
                wscore = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);        //加权得分
                //计算期望值，查期望曲线。当参考量超限时，有两种处理方法
                //处理方式一：不做处理，返回null，对应时间段没有任何计算结果
                /*
                if (refdataX.Value < opxCurve.Xvalue[0] ||
                    refdataX.Value > opxCurve.Xvalue[opxCurve.Xvalue.Length - 1] ||
                    refdataY.Value < opxCurve.Yvalue[0] ||
                    refdataY.Value > opxCurve.Yvalue[opxCurve.Yvalue.Length - 1]
                    )
                {
                    //如果参考标签的值，在期望曲线参考值有效范围外，则当前时刻点，考核无效，不返回任何考核值
                    return null;
                }
                */
                //处理方式二:任意一个参考值超限时，按边界值计算偏差，同时所有计算结果状态位置为StatusConst.OutOfValid4Bias
                //要注意，防止偏差曲线的x不是从小到大。
                //参考值X（对应表的第一行配置）
                if (refdataX.Value < opxCurve.XLowerLimit)
                {
                    refdataX.Value = opxCurve.XLowerLimit;
                    invalidflag = StatusConst.OutOfValid4Bias;
                }
                else if (refdataX.Value > opxCurve.XUpperLimit)
                {
                    refdataX.Value = opxCurve.XUpperLimit;
                    invalidflag = StatusConst.OutOfValid4Bias;
                }
                //参考值Y（对应表的第一列配置）
                if (refdataY.Value < opxCurve.YLowerLimit)
                {
                    refdataY.Value = opxCurve.YLowerLimit;
                    invalidflag = StatusConst.OutOfValid4Bias;
                }
                else if (refdataY.Value > opxCurve.YUpperLimit)
                {
                    refdataY.Value = opxCurve.YUpperLimit;
                    invalidflag = StatusConst.OutOfValid4Bias;
                }
                //期望值：查期望曲线 - 2d表格
                sp.Value = opxCurve.Fx(refdataX.Value, refdataY.Value); //虽然一律要算期望值，但是超限后按边界算期望，所有计算结果标志位都会被置位

                //偏差=实际值-期望值
                err.Value = pvdata.Value - sp.Value;
                //偏差比=偏差/期望值
                if (sp.Value == 0)
                    errrate.Value = 0;
                else
                    errrate.Value = err.Value / sp.Value * 100;

                //得分：查记分曲线
                //——偏差超过限度时，直接返回得分边界值。但是得分不像偏差曲线的x越界，要置invalidflag
                score.Value = scoreCurve.Fx(err.Value);
                wscore.Value = k * score.Value + b;
                //4、录入数据库       
                MDeviationSOutClass MessageIN = new MDeviationSOutClass();
                MessageIN.sp = sp.Value;
                MessageIN.err = err.Value;
                MessageIN.errrate = errrate.Value;
                MessageIN.score = score.Value;
                MessageIN.wscore = wscore.Value;
                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                string hour = string.Empty;
                year = calcuinfo.fstarttime.Year.ToString();
                month = calcuinfo.fstarttime.Month.ToString();
                day = calcuinfo.fstarttime.Day.ToString();
                hour = calcuinfo.fstarttime.Hour.ToString();
                bool isok = BLL.AlgorithmBLL.insertMDeviation2DS(MessageIN, calcuinfo.fsourtagids[0].ToString(), year, month, day, hour, Convert.ToInt32(invalidflag));
                if (isok)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MDeviationS数据录入数据库是失败";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
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
