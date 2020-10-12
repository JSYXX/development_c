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
    public class NewMDeviationS : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：一维偏差及得分计算(单值计算);

        private string _moduleName = "NewMDeviationS";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "一维偏差及得分计算(单值计算)。当参考值超出偏差曲线范围时，计算结果状态位为11，表示当次计算结果无效。";
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
        private string _inputDescsCN = "求偏差的标签;求期望值的参考标签";
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
        private string _moduleParaExample = "D;1;1;1;0;S";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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
        private Regex _moduleParaRegex = new Regex(@"^([DS]){1}(;[+]?\d+){2}(;[+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){1}(;){0,1}([SD]){0,1}$");
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
        private string _outputDescsCN = "期望值(单值);偏差(单值);偏差比(单值);得分(单值);加权得分(单值)";
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
        //多输入参数标签顺序：考核标签、参考标签（期望曲线X轴对应标签）
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
                if (inputs == null || inputs.Length < 2 || inputs[0] == null || inputs[1] == null)          //一些情况下，进来就有null，这样下面的的取出状态为会出错
                {
                    _errorFlag = true;
                    _errorInfo = "输入有效数据数量不足。一维偏差及得分计算要求必须有且仅有两个有效输入。";
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
                    _warningInfo = "一维偏差及得分计算，被测评量经状态位过滤后数据为空！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[1] == null || inputs[1].Count < 2)
                {
                    _warningFlag = true;
                    _warningInfo = "一维偏差及得分计算，参考量经状态位过滤后数据为空！";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //1.2 直接取各变量第一个数值计算，不需要处理截止时刻值
                PValue pvdata;
                PValue refdata;

                pvdata = inputs[0][0];      //不使用截止值
                refdata = inputs[1][0];     //不使用截止值

                StatusConst invalidflag = StatusConst.Normal;   //参考指标值是否超越偏差曲线x范围，当次指标考核计算是否有效

                //2、根据参数获取期望曲线、记分曲线
                string calcuMode;   //计算方式选择
                string[] para = calcuinfo.fparas.Split(new char[] { ';' });
                string readMethod = para[0];        //第一个参数是获取曲线的方式，D为动态，S为静态。动态方式，每次计算均读取CSV。静态方式，只读取一次CSV。
                int opxid = int.Parse(para[1]);     //第二个参数是期望曲线序号
                int scoreid = int.Parse(para[2]);   //第三个参数是得分曲线序号
                double k = double.Parse(para[3]);
                double b = double.Parse(para[4]);
                if (para.Length == 6)
                {
                    calcuMode = para[5];            //如果设定了第5个参数，计算模式用二维计分。D表示二维计分，s表示一维计分 
                }
                else
                    calcuMode = "S";                //如果没设定第5个参数，计算模式为一维计分

                Curve1D opxCurve;    //期望值
                Curve1D scoreCurve;  //计分值
                Curve2D scoreCurve2D;

                string strTemp = CurveConfig.Instance.OPXCurvesReadStatus;  //这行这条语句是为了下面在使用CurveConfig.GetXXXCurve时保证Instance已被初始化。20181109调试发现该问题。
                string strTemp2D = CurveConfig.Instance.ScoreCurves2DReadStatus;

                try
                {
                    if (readMethod.ToUpper() == "S")
                    {
                        //第一个参数为S时，静态读取期望曲线和积分曲线表
                        opxCurve = CurveConfig.GetOPXCurve(opxid, calcuinfo.fstarttime);

                        scoreCurve2D = CurveConfig.GetScoreCurve2D(scoreid, calcuinfo.fstarttime);

                        scoreCurve = CurveConfig.GetScoreCurve(scoreid, calcuinfo.fstarttime);
                    }
                    else
                    {
                        //——20181031，因动态方式读取曲线，速度太慢，不在采用。
                        //——转而采用在静态方式下读取带生效时间的配置曲线。这样在增加配置曲线时，仅需要暂停计算引擎后重启，即可载入新的配置曲线，并继续进行计算。
                        //第一个参数为D时，动态读取期望曲线和积分曲线表
                        //opxCurve = CurveConfig.ReadOPXCurves(calcuinfo.fstarttime).Find(delegate(Curve1D a) { return a.Index == opxid; });
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
                if (calcuMode == "D")
                {
                    if (scoreCurve2D == null)
                    {
                        _errorFlag = true;
                        _errorInfo = String.Format("得分曲线为空，对应的得分曲线：{0}-{1}获取错误！", scoreid, calcuinfo.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                }
                if (calcuMode == "S")
                {
                    if (scoreCurve == null)
                    {
                        _errorFlag = true;
                        _errorInfo = String.Format("得分曲线为空，对应的得分曲线：{0}-{1}获取错误！", scoreid, calcuinfo.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                    }
                }



                //3、计算偏差
                PValue sp, err, errrate, score, wscore;           //期望值、偏差、偏差比、得分

                sp = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);           //期望值
                err = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);          //偏差
                errrate = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);      //偏差比
                score = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);        //得分
                wscore = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);        //加权得分


                //Debug.WriteLine("----opxCurve.Xvalue[0] " + opxCurve.Xvalue[0].ToString() + "~opxCurve.Xvalue[1]" + opxCurve.Xvalue[opxCurve.Xvalue.Length - 1].ToString());  
                //期望值：查期望曲线
                //处理方式一：参考点超限时，不做处理，对应时间段没有任何计算结果
                //——该方法的主要问题是，整分钟的偏差和得分计算接口，会有空。在那些超过偏差曲线范围外的时间段，没有分钟计算值。
                //——该方法导致的另外一个问题就是，由于缺少分钟值，并发计算在缺少分钟值的位置要去插值，这个插值行为是错误的。
                //——在缺少分钟值的情况下，实时计算，由于取数据时存在起始时间和截止时间的插值算法，如果恰好这些时间点没有值，也会进行插值，这个插值会造成错误。
                /*
                if (refdata.Value < opxCurve.Xvalue[0] || refdata.Value > opxCurve.Xvalue[opxCurve.Xvalue.Length - 1])
                {
                    return null;
                }
                else
                {
                    opx.Value = opxCurve.Fx(refdata.Value);
                }
                */

                //处理方式二：参考点超限时，按边界点计算偏差。同时在计算结果中置标志位为StatusConst.OutOfValid4Bias.
                //——四个计算结果均按边界值计算，置计算结果标志位为StatusConst.OutOfValid4Bias。
                //——但是，凡是对偏差和得分这些结果进行进一步计算的，这些算法需要考虑标志位。 
                //——由于不会缺少分钟值，无论是并发计算和实时计算都不会出现问题。


                if (refdata.Value < opxCurve.XLowerLimit)   //小于下限
                {
                    //如果参考标签的值，在期望曲线参考值有效范围外，则期望返回边界值。
                    sp.Value = opxCurve.Fx(opxCurve.XLowerLimit);   //期望值返回边界值
                    invalidflag = StatusConst.OutOfValid4Bias;       //至过程值超限标志
                }
                else if (refdata.Value > opxCurve.XUpperLimit) //大于上限
                {
                    sp.Value = opxCurve.Fx(opxCurve.XUpperLimit);
                    invalidflag = StatusConst.OutOfValid4Bias;
                }
                else
                {
                    sp.Value = opxCurve.Fx(refdata.Value);          //期望值按正常计算
                }

                //偏差=实际值-期望值
                err.Value = pvdata.Value - sp.Value;
                //偏差比=偏差/期望值
                if (sp.Value == 0)
                    errrate.Value = 0;
                else
                    errrate.Value = err.Value / sp.Value * 100;
                //得分：查记分曲线
                //——偏差超过限度时，直接返回得分边界值。但是得分不像偏差曲线的x越界，要置invalidflag
                if (calcuMode == "S")
                    score.Value = scoreCurve.Fx(err.Value);
                else
                {
                    //参考值Y（对应表的第一列配置）
                    if (refdata.Value < scoreCurve2D.YLowerLimit)
                    {
                        refdata.Value = scoreCurve2D.YLowerLimit;
                        invalidflag = StatusConst.OutOfValid4Bias;
                    }
                    else if (refdata.Value > scoreCurve2D.YUpperLimit)
                    {
                        refdata.Value = scoreCurve2D.YUpperLimit;
                        invalidflag = StatusConst.OutOfValid4Bias;
                    }
                    score.Value = scoreCurve2D.Fx(err.Value, refdata.Value); //虽然一律要算期望值，但是超限后按边界算期望，所有计算结果标志位都会被置位

                }

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
                bool isok = BLL.AlgorithmBLL.insertMDeviationS(MessageIN, calcuinfo.fsourtagids[0].ToString(), year, month, day, hour, Convert.ToInt32(invalidflag));
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
