using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon;                         //使用PValue

namespace PSLCalcu.Module
{
    /// <summary>
    /// 通过一维偏差期望曲线计算偏差，再根据记分曲线计算得分。一次计算一个周期内的多个时间点偏差。
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。该算法仅使用每一个标签的第一个数据
    ///     2018.04.02 版本：1.1 截止时刻值问题已处理。
    ///		2018.02.08 版本：1.0 xiangmin 创建。    
    /// <author>
    ///		<name>xiangmin</name>
    ///		<date>2018.02.08</date>
    /// </author> 
    /// </summary>
    public class MDeviationSpan : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：一维偏差及得分计算(单值计算);

        private string _moduleName = "MDeviationSpan";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "时间段一维偏差及得分计算(多值计算)。当参考值超出偏差曲线范围时，计算结果状态位为11，表示当次计算结果无效。";
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
            List<PValue>[] results = new List<PValue>[4];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {

                //0、输入
                //时间段偏差算法与单点偏差算法，唯一的不同点在于，单点偏差算法一次仅算一个偏差值。时间段偏差算法，一次要写多个偏差值。


                //0.1、输入处理：输入长度。输入必须还有一个被考核量，一个参考量。当输入为空或者输入数量不足，给出标志位为StatusConst.InputIsNull的计算结果.
                //——在该算法下，正常情况不应该出现inputs整体为空的情况，出现整体为空，说明前置的求每分钟均值的算法有误。这种情况必须给出错误
                if (inputs == null || inputs.Length < 2 || inputs[0] == null || inputs[0].Count == 0 || inputs[1] == null || inputs[1].Count == 0  )          //一些情况下，进来就有null，这样下面的的取出状态为会出错
                {
                    _errorFlag = true;
                    _errorInfo = "时间段一维偏差算法输入错误。输入为空或输入数量不足，或者被考核量、参考量数据长度为0。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //0.2、输入处理：输入的是要参与考核的被考核量与参考量的分钟数据，长度必须相同
                if (inputs[0].Count != inputs[1].Count)
                {
                    _errorFlag = true;
                    _errorInfo = "时间段一维偏差算法输入错误。被考核量和参考量数据长度不相同。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }

                //准备结果，只要输入数据没有整体错误，则必须有计算结果
                int spanNumber = inputs[0].Count-1;     //不算截止数据
                double intervalseconds = (double)(calcuinfo.fendtime.Subtract(calcuinfo.fstarttime).TotalSeconds / spanNumber); 
                List<PValue>[] tempresults = new List<PValue>[4];
                tempresults[0]=new List<PValue>();  //期望
                tempresults[1]=new List<PValue>();  //偏差
                tempresults[2]=new List<PValue>();  //偏差比
                tempresults[3]=new List<PValue>();  //得分
                
                //在输入数量正确，且分钟数量的数量相同的情况下，循环计算每分钟的偏差值
                for (int j = 0; j < inputs[0].Count-1; j++)   //不计算截止数据
                {
                    //0.3 初始化当前分钟的计算结果
                    for (i = 0; i < results.Length; i++)
                    {
                        tempresults[i].Add(new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j+1) * intervalseconds), (long)StatusConst.InputIsNull));
                    }
                    //0.4、输入处理：检查标志位。
                    //——这里是计算每分钟的偏差。
                    //——输入量是被评变量每分钟滤波值、参考变量每分钟滤波值 
                    if (inputs[0][j] == null || inputs[1][j] == null)
                    {
                        //正常状态下，不应该出现分钟过滤值在某个点没有值得情况，这种情况说明分钟过滤算法有误，应该检查
                        //在计算引擎中，即使计算错误标志为true，但仍会写计算结果
                        string nullobject = "";
                        if (inputs[0][j] == null) nullobject += "被考核量";
                        if (inputs[1][j] == null) nullobject += "、参考量";

                        _errorFlag = true;
                        _errorInfo += String.Format("时间段一维偏差及得分计算，{0}分钟数据为空。请检查！" + Environment.NewLine, nullobject);
                        //return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo);  //这里不直接结束，而是继续循环
                        continue;
                    }
                    if (inputs[0][j].Status != 0 || inputs[1][j].Status != 0)
                    {
                        string statusnotzero = "";
                        if (inputs[0][j].Status != 0) statusnotzero += "被考核量";
                        if (inputs[1][j].Status != 0) statusnotzero += "、参考量";
                        //正常状态下，有可能出现分钟过滤在某个时刻点状态值非0，记录报警，并计算下一个点
                        _warningFlag = true;
                        _warningInfo += String.Format("时间段一维偏差及得分计算，{0}分钟数据状态位异常。{1}到{2}无有效数据!" + Environment.NewLine,
                                                        statusnotzero,
                                                        inputs[0][j].Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                                                        inputs[0][j].Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                                                      );
                        continue;                        
                    }

                    //1.2 直接取各变量分钟值
                    PValue pvdata;
                    PValue refdata;

                    pvdata = inputs[0][j];      //
                    refdata = inputs[1][j];     //

                    StatusConst invalidflag = StatusConst.Normal;   //参考指标值是否超越偏差曲线x范围，当次指标考核计算是否有效

                    //2、根据参数获取期望曲线、记分曲线
                    string[] para = calcuinfo.fparas.Split(new char[] { ';' });
                    string readMethod = para[0];        //第一个参数是获取曲线的方式，D为动态，S为静态。动态方式，每次计算均读取CSV。静态方式，只读取一次CSV。
                    int opxid = int.Parse(para[1]);     //第二个参数是期望曲线序号
                    int scoreid = int.Parse(para[2]);   //第三个参数是得分曲线序号

                    Curve1D opxCurve;
                    Curve1D scoreCurve;
                    string strTemp = CurveConfig.Instance.OPXCurvesReadStatus;  //这行这条语句是为了下面在使用CurveConfig.GetXXXCurve时保证Instance已被初始化。20181109调试发现该问题。

                    try
                    {
                        if (readMethod.ToUpper() == "S")
                        {
                            //第一个参数为S时，静态读取期望曲线和积分曲线表
                            opxCurve = CurveConfig.GetOPXCurve(opxid, calcuinfo.fstarttime);
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
                        _errorInfo += String.Format("读取期望和得分曲线时错误！错误信息：{0}", ex.ToString())+Environment.NewLine;
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
                    PValue sp, err, errrate, score;           //期望值、偏差、偏差比、得分

                    sp = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);           //期望值
                    err = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);          //偏差
                    errrate = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);      //偏差比
                    score = new PValue(0, calcuinfo.fstarttime.AddSeconds(j * intervalseconds), calcuinfo.fstarttime.AddSeconds((j + 1) * intervalseconds), 0);        //得分

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

                    //要注意，防止偏差曲线的x不是从小到大。(但是，目前硬性要求期望曲线和得分曲线配置，范围要从低到高。因此老版本上，没有这个判断)
                    double XMin = opxCurve.Xvalue[0] < opxCurve.Xvalue[opxCurve.Xvalue.Length - 1] ? opxCurve.Xvalue[0] : opxCurve.Xvalue[opxCurve.Xvalue.Length - 1];
                    double XMax = opxCurve.Xvalue[0] < opxCurve.Xvalue[opxCurve.Xvalue.Length - 1] ? opxCurve.Xvalue[opxCurve.Xvalue.Length - 1] : opxCurve.Xvalue[0];

                    if (refdata.Value < XMin)
                    {
                        //如果参考标签的值，在期望曲线参考值有效范围外，则偏差返回边界值。
                        sp.Value = opxCurve.Fx(XMin);
                        invalidflag = StatusConst.OutOfValid4Bias;
                    }
                    else if (refdata.Value > XMax)
                    {
                        sp.Value = opxCurve.Fx(XMax);
                        invalidflag = StatusConst.OutOfValid4Bias;
                    }
                    else
                    {
                        //期望值
                        sp.Value = opxCurve.Fx(refdata.Value);      //虽然一律要算期望值，但是超限后按边界算期望，所有计算结果标志位都会被置位
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
                    score.Value = scoreCurve.Fx(err.Value);

                    //4、组织输出                
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
                }//end for

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
