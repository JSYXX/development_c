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
    public class MReadStatus : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MReadStatus";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "基础数据计算";
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
        private string _algorithms = "MReadStatus";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "Y";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "L";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "Mode";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex = new Regex(@"^([+-]?\d+(\.\d+)?){1}(;[+-]?\d+(\.\d+)?){6}(;){0,1}([SL]){0,1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }


        private int _outputNumber = 32;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Si";






        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "变量的 i 状态。给几个输入参数就有几个输出";



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
        //多输入参数标签顺序：
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
        #endregion  #region 计算模块算法
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
            List<PValue>[] results = new List<PValue>[1];
            for (i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
            }

            try
            {
                //读取参数

                string mode;
                //tagId
                string type = calcuinfo.fsourtagids[0].ToString();
                string[] paras = calcuinfo.fparas.Split(';');
                mode = paras[0];
                List<PValue> input = new List<PValue>();
                input = inputs[0];
                string[] sectionList = mode.Split(';');
                List<DoubleTimeListClass> returnList = new List<DoubleTimeListClass>();
                foreach (string item in sectionList)
                {
                    DoubleTimeListClass newClass = new DoubleTimeListClass();
                    newClass.timeList = new List<DoubleTimeClass>();
                    newClass.areaStr = item;
                    bool firstCompare = false;
                    bool endCompare = false;
                    double firstDouble = 0;
                    double endDouble = 0;
                    if (item.Substring(0, 1) == "[")
                    {
                        firstCompare = true;
                    }
                    if (item.Substring(item.Length - 1, 1) == "]")
                    {
                        endCompare = true;
                    }
                    string[] doubleList = item.Substring(1, item.Length - 2).Split(',');
                    firstDouble = Convert.ToDouble(doubleList[0].Trim());
                    endDouble = Convert.ToDouble(doubleList[1].Trim());
                    if (firstCompare && endCompare)
                    {
                        double startTime = 0;
                        double endTime = 0;
                        int status = 0;
                        for (int l = 0; l < input.Count; l++)
                        {
                            if (status == 0)
                            {
                                if (input[l].Value >= firstDouble && input[l].Value <= endDouble)
                                {
                                    if (l == 0)
                                    {
                                        startTime = input[l].Timespan;
                                    }
                                    else
                                    {
                                        double totalValue = Math.Abs(input[l].Value - input[l - 1].Value);
                                        double changeValue = Math.Abs(firstDouble - input[l - 1].Value);
                                        double changeTime = input[l].Timespan - input[l - 1].Timespan;
                                        startTime = input[l - 1].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0);
                                    }
                                    endTime = input[l].Timespan;
                                    status = 1;
                                }
                            }
                            else
                            {
                                if (input[l].Value >= firstDouble && input[l].Value <= endDouble)
                                {
                                    endTime = input[l].Timespan;
                                }
                                else
                                {
                                    if (l != input.Count - 1)
                                    {
                                        double totalValue = Math.Abs(input[l + 1].Value - input[l].Value);
                                        double changeValue = Math.Abs(endDouble - input[l].Value);
                                        double changeTime = input[l + 1].Timespan - input[l].Timespan;
                                        startTime = input[l].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0);
                                    }
                                    newClass.timeList.Add(new DoubleTimeClass(startTime, endTime));
                                    startTime = 0;
                                    endTime = 0;
                                    status = 0;
                                }
                            }
                        }
                    }
                    else if (!firstCompare && endCompare)
                    {
                        double startTime = 0;
                        double endTime = 0;
                        int status = 0;
                        for (int l = 0; l < input.Count; l++)
                        {
                            if (status == 0)
                            {
                                if (input[l].Value > firstDouble && input[l].Value <= endDouble)
                                {
                                    if (l == 0)
                                    {
                                        startTime = input[l].Timespan;
                                    }
                                    else
                                    {
                                        double totalValue = Math.Abs(input[l].Value - input[l - 1].Value);
                                        double changeValue = Math.Abs(firstDouble - input[l - 1].Value);
                                        double changeTime = input[l].Timespan - input[l - 1].Timespan;
                                        startTime = input[l - 1].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0) + 1000;
                                    }
                                    endTime = input[l].Timespan;
                                    status = 1;
                                }
                            }
                            else
                            {
                                if (input[l].Value > firstDouble && input[l].Value <= endDouble)
                                {
                                    endTime = input[l].Timespan;
                                }
                                else
                                {
                                    if (l != input.Count - 1)
                                    {
                                        double totalValue = Math.Abs(input[l + 1].Value - input[l].Value);
                                        double changeValue = Math.Abs(endDouble - input[l].Value);
                                        double changeTime = input[l + 1].Timespan - input[l].Timespan;
                                        startTime = input[l].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0);
                                    }
                                    newClass.timeList.Add(new DoubleTimeClass(startTime, endTime));
                                    startTime = 0;
                                    endTime = 0;
                                    status = 0;
                                }
                            }
                        }
                    }
                    else if (firstCompare && !endCompare)
                    {
                        double startTime = 0;
                        double endTime = 0;
                        int status = 0;
                        for (int l = 0; l < input.Count; l++)
                        {
                            if (status == 0)
                            {
                                if (input[l].Value >= firstDouble && input[l].Value < endDouble)
                                {
                                    if (l == 0)
                                    {
                                        startTime = input[l].Timespan;
                                    }
                                    else
                                    {
                                        double totalValue = Math.Abs(input[l].Value - input[l - 1].Value);
                                        double changeValue = Math.Abs(firstDouble - input[l - 1].Value);
                                        double changeTime = input[l].Timespan - input[l - 1].Timespan;
                                        startTime = input[l - 1].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0);
                                    }
                                    endTime = input[l].Timespan;
                                    status = 1;
                                }
                            }
                            else
                            {
                                if (input[l].Value >= firstDouble && input[l].Value < endDouble)
                                {
                                    endTime = input[l].Timespan;
                                }
                                else
                                {
                                    if (l != input.Count - 1)
                                    {
                                        double totalValue = Math.Abs(input[l + 1].Value - input[l].Value);
                                        double changeValue = Math.Abs(endDouble - input[l].Value);
                                        double changeTime = input[l + 1].Timespan - input[l].Timespan;
                                        startTime = input[l].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0) - 1000;
                                    }
                                    newClass.timeList.Add(new DoubleTimeClass(startTime, endTime));
                                    startTime = 0;
                                    endTime = 0;
                                    status = 0;
                                }
                            }
                        }
                    }
                    else if (!firstCompare && !endCompare)
                    {
                        double startTime = 0;
                        double endTime = 0;
                        int status = 0;
                        for (int l = 0; l < input.Count; l++)
                        {
                            if (status == 0)
                            {
                                if (input[l].Value > firstDouble && input[l].Value < endDouble)
                                {
                                    if (l == 0)
                                    {
                                        startTime = input[l].Timespan;
                                    }
                                    else
                                    {
                                        double totalValue = Math.Abs(input[l].Value - input[l - 1].Value);
                                        double changeValue = Math.Abs(firstDouble - input[l - 1].Value);
                                        double changeTime = input[l].Timespan - input[l - 1].Timespan;
                                        startTime = input[l - 1].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0) + 1000;
                                    }
                                    endTime = input[l].Timespan;
                                    status = 1;
                                }
                            }
                            else
                            {
                                if (input[l].Value > firstDouble && input[l].Value < endDouble)
                                {
                                    endTime = input[l].Timespan;
                                }
                                else
                                {
                                    if (l != input.Count - 1)
                                    {
                                        double totalValue = Math.Abs(input[l + 1].Value - input[l].Value);
                                        double changeValue = Math.Abs(endDouble - input[l].Value);
                                        double changeTime = input[l + 1].Timespan - input[l].Timespan;
                                        startTime = input[l].Timespan + Math.Round((changeTime * (changeValue / totalValue)), 0) - 1000;
                                    }
                                    newClass.timeList.Add(new DoubleTimeClass(startTime, endTime));
                                    startTime = 0;
                                    endTime = 0;
                                    status = 0;
                                }
                            }
                        }
                    }

                    returnList.Add(newClass);
                }

                string year = string.Empty;
                string month = string.Empty;
                string day = string.Empty;
                string hour = string.Empty;
                DateTime dt = Convert.ToDateTime(input[input.Count - 1].Timestamp);
                year = dt.Year.ToString();
                month = dt.Month.ToString();
                day = dt.Day.ToString();
                hour = dt.Hour.ToString();

                bool isok = BLL.AlgorithmBLL.insertMReadStatus(returnList, type, year, month, day, hour);
                if (isok)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MPVBase长周期数据录入数据库是失败";
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
    }
}
