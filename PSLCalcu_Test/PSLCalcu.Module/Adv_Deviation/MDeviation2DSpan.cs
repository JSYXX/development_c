using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue
using Config;                           //小时周期算分钟数据，需要判断常数标签

namespace PSLCalcu.Module
{
    /// <summary>
    /// 通过二维偏差期望曲线计算偏差，再根据记分曲线计算得分
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。该算法仅使用每一个标签的第一个数据
    ///		2018.02.08 版本：1.0 xiangmin 创建。    
    /// <author>
    ///		<name>xiangmin</name>
    ///		<date>2018.02.08</date>
    /// </author> 
    /// </summary>
    public class MDeviation2DSpan : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：二维偏差及得分计算(单值计算);

        private string _moduleName = "MDeviation2DSpan";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "时间段二维偏差及得分计算(多值计算)。当参考值超出偏差曲线范围时，计算结果状态位为11，表示当次计算结果无效。";
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
        private string _algorithmsflag = "YYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "D;1;1";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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
        private Regex _moduleParaRegex = new Regex(@"^([DS]){1};([+]?\d+){1};([+]?\d+){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 4;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "DevSP;DevErr;DevErrR;DevScore";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "期望值(多值);偏差(多值);偏差比(多值);得分(多值)";
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
            List<PValue>[] results = new List<PValue>[4];
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
                    _errorInfo = "时间段二维偏差及得分算法输入错误。计算要求必须有且仅有三个有效输入。当前输入整体为空或输入数量不足。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[0] == null || inputs[0].Count == 0 )
                {
                    _errorFlag = true;
                    _errorInfo = "时间段二维偏差及得分计算输入错误。被考核量输入数据为空。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[1] == null || inputs[1].Count == 0 )
                {
                    _errorFlag = true;
                    _errorInfo = "时间段二维偏差及得分计算输入错误。参考量1输入数据为空。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                if (inputs[2] == null || inputs[2].Count == 0)
                {
                    _errorFlag = true;
                    _errorInfo = "时间段二维偏差及得分计算输入错误。参考量2输入数据为空。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //0.2、在判断各个数据长度是否相等之前，对常数标签按照被考核量进行扩充。因为常数标签仅返回一个值。
                for (i = 1; i < inputs.Length; i++)                                         //第0个量是被考核指标，不可能是常数标签
                {
                    if (calcuinfo.fsourtagids[i] < APPConfig.rdbtable_constmaxnumber)
                    {
                        double constValue = inputs[i][0].Value;                             //常数标签仅返回一个值
                        List<PValue> constList = new List<PValue>();                        //以被考核量为标准，重新构建常数标签
                        for (int j = 0; j < inputs[0].Count; j++)   
                        {
                            constList.Add(new PValue(constValue, inputs[0][j].Timestamp, inputs[0][j].Endtime, inputs[0][j].Status));
                        }
                        inputs[i] = constList;
                    }
                }
                
                //0.2、输入处理：输入的是要参与考核的被考核量与参考量的分钟数据，长度必须相同。
                //——因为被考核量和考核量，都是经过filter计算的，无论实时数据有什么问题，filter计算后都应该产生计算结果（只不过状态位不同）。
                //——因此正常状态下，被考核量与考核量的长度应该相同，如果不相同，则说明filter计算一定有问题，需要检查。因此这里应该报错误。而不是报警告。
                if (inputs[0].Count != inputs[1].Count || inputs[0].Count != inputs[2].Count || inputs[1].Count != inputs[2].Count)
                {
                    _errorFlag = true;
                    _errorInfo = "时间段一维偏差算法输入错误。被考核量和参考量数据长度不相同。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //准备结果，只要输入数据没有整体错误，则必须有计算结果
                int spanNumber = inputs[0].Count - 1;               //不算截止数据
                double intervalseconds = (double)(calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalSeconds / spanNumber); 
                List<PValue>[] tempresults = new List<PValue>[4];
                tempresults[0] = new List<PValue>();                //期望
                tempresults[1] = new List<PValue>();                //偏差
                tempresults[2] = new List<PValue>();                //偏差比
                tempresults[3] = new List<PValue>();                //得分

                //在输入数量正确，且分钟数量的数量相同的情况下，循环计算每分钟的偏差值
                for (int j = 0; j < inputs[0].Count-1; j++) //不计算截止数据
                {

                    //0.3 初始化当前分钟的计算结果
                    for (i = 0; i < results.Length; i++)
                    {
                        tempresults[i].Add(new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), (long)StatusConst.InputIsNull));
                    }

                    //0.4、输入处理：检查标志位。
                    //——这里是计算每分钟的偏差。
                    //——输入量是被评变量每分钟滤波值、参考变量每分钟滤波值 
                    if (inputs[0][j] == null || inputs[1][j] == null || inputs[2][j] == null)
                    {
                        //正常状态下，不应该出现分钟过滤值在某个点没有值得情况，这种情况说明分钟过滤算法有误，应该检查
                        //在计算引擎中，即使计算错误标志为true，但仍会写计算结果
                        string nullobject = "";
                        if (inputs[0][j] == null) nullobject += "|被考核量|";
                        if (inputs[1][j] == null) nullobject += "|参考量1|";
                        if (inputs[2][j] == null) nullobject += "|参考量2|";

                        _errorFlag = true;
                        _errorInfo += String.Format("时间段二维偏差及得分计算，{0}从{1}到{2}的分钟数据为空。请检查！" + Environment.NewLine, 
                                                    nullobject,
                                                    inputs[0][j].Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                                                    inputs[0][j].Endtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                        );
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);  //这里不直接结束，而是继续循环，计算下一分钟的偏差值
                        continue;
                    }
                    else if (inputs[0][j].Status != 0 || inputs[1][j].Status != 0 || inputs[2][j].Status != 0)
                    {
                        string statusnotzero = "";
                        if (inputs[0][j].Status != 0) statusnotzero += "|被考核量|";
                        if (inputs[1][j].Status != 0) statusnotzero += "|参考量1|";
                        if (inputs[2][j].Status != 0) statusnotzero += "|参考量2|";
                        //正常状态下，有可能出现分钟过滤在某个时刻点状态值非0，记录报警，并计算下一个点
                        _warningFlag = true;
                        _warningInfo += String.Format("时间段二维偏差及得分计算，{0}的分钟数据状态位异常。从{1}到{2}无有效数据。请检查！" + Environment.NewLine,
                                                        statusnotzero,
                                                        inputs[0][j].Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                                                        inputs[0][j].Endtime.ToString("yyyy-MM-dd HH:mm:ss")
                                                      );
                        continue;
                    }

                    //1.2、直接取各变量分钟值

                    PValue pvdata;
                    PValue refdataX;
                    PValue refdataY;

                    pvdata = inputs[0][j];          //不使用截止值，被测评量
                    refdataX = inputs[1][j];        //不使用截止值，参考量1
                    refdataY = inputs[2][j];        //不使用截止值，参考量2

                    StatusConst invalidflag = StatusConst.Normal;   //参考指标值是否超越偏差曲线x范围，当次指标考核计算是否有效

                    //2、根据参数获取期望曲线、记分曲线
                    string[] para = calcuinfo.fparas.Split(new char[] { ';' });
                    string readMethod = para[0];        //第一个参数是获取曲线的方式，D为动态，S为静态。动态方式，每次计算均读取CSV。静态方式，只读取一次CSV。
                    int opxid = int.Parse(para[1]);     //第二个参数是期望曲线序号
                    int scoreid = int.Parse(para[2]);   //第三个参数是得分曲线序号

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
                        {
                            //——20181031，因动态方式读取曲线，速度太慢，不在采用。
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
                        _errorInfo += String.Format("读取期望和得分曲线时错误，请检查曲线配置文件！错误信息：{0}", ex.ToString());
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                        continue;
                    }
                    //判读所需要的曲线是否正确读取，如果没有正确读取一定是返回null，此时要给出错误log
                    if (opxCurve == null)
                    {
                        _errorFlag = true;
                        _errorInfo += String.Format("期望曲线为空，对应的期望曲线：{0}-{1}获取错误！", opxid, calcuinfo.fstarttime.ToString("yyyy-MM-dd HH:mm:ss")) + Environment.NewLine;
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                        continue;
                    }
                    if (scoreCurve == null)
                    {
                        _errorFlag = true;
                        _errorInfo += String.Format("得分曲线为空，对应的得分曲线：{0}-{1}获取错误！", scoreid, calcuinfo.fstarttime.ToString("yyyy-MM-dd HH:mm:ss")) + Environment.NewLine;
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);
                        continue;
                    }

                    //3、计算偏差                
                    PValue sp, err, errrate, score;       //期望值、偏差、偏差比、得分

                    sp = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);
                    err = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);
                    errrate = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);
                    score = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);

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

                    //4、组织输出
                    results = new List<PValue>[4];
                    for (i = 0; i < 4; i++)
                    {
                        results[i] = new List<PValue>();
                    }
                    tempresults[0][j] = sp;
                    tempresults[1][j] = err;
                    tempresults[2][j] = errrate;
                    tempresults[3][j] = score;

                    //如果越界标识不为0，还需要给每个结果的状态添加越界的标志。
                    if (invalidflag != 0)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            tempresults[i][j].Status = (long)invalidflag;
                        }
                    }
                }
                return new Results(tempresults, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
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