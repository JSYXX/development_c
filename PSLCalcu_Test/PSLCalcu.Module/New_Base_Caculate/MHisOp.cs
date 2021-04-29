using PCCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class MHisOp : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MHisOp";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "将变量当前值、逻辑状态与历史数据联带计算";
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
        private string _algorithms = "MHisOp";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "1;2;1;2;1;2;1;S";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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


        private int _outputNumber = 67;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "";

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
        /// <summary>
        /// 根据周期和时间计算出该周期的起始结束时间
        /// </summary>
        /// <param name="P"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private static List<DateTime> getDateRegion(string P, DateTime date)
        {

            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                List<DateTime> returnList = new List<DateTime>();
                switch (P)
                {
                    case "D":
                        startDate = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " 00:00");
                        endDate = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " 23:59");
                        break;
                    case "W":
                        int dayOfWeek = (int)(date.DayOfWeek);
                        if (dayOfWeek == 0)
                        {
                            dayOfWeek = 7;
                        }
                        startDate = Convert.ToDateTime(date.AddDays(-dayOfWeek + 1).ToString("yyyy-MM-dd") + " 00:00");
                        endDate = Convert.ToDateTime(date.AddDays(7 - dayOfWeek).ToString("yyyy-MM-dd") + " 23:59");
                        break;
                    case "M":
                        startDate = Convert.ToDateTime(date.ToString("yyyy-MM") + "-01 00:00");
                        endDate = Convert.ToDateTime((Convert.ToDateTime(date.AddMonths(1).ToString("yyyy-MM") + "-01").AddDays(-1)).ToString("yyyy-MM-dd") + " 23:59");
                        break;
                    case "S":
                        int month = date.Month;
                        int startMonth = 0;
                        int endMonth = 0;
                        startMonth = month - (month % 3) + 1;
                        endMonth = startMonth + 3;
                        startDate = Convert.ToDateTime(date.ToString("yyyy") + "-" + (startMonth < 10 ? "0" + startMonth.ToString() : startMonth.ToString()) + "-01 00:00");
                        endDate = endMonth == 12 ? Convert.ToDateTime(date.ToString("yyyy") + "-12-31 23:59") : Convert.ToDateTime((Convert.ToDateTime(date.ToString("yyyy") + "-" + (endMonth < 10 ? "0" + endMonth.ToString() : endMonth.ToString()) + "-01").AddDays(-1)).ToString("yyyy-MM-dd") + " 23:59");
                        break;
                    case "Y":
                        startDate = Convert.ToDateTime(date.ToString("yyyy") + "-01-01 00:00");
                        endDate = Convert.ToDateTime((Convert.ToDateTime(date.AddYears(1).ToString("yyyy") + "-01-01").AddDays(-1)).ToString("yyyy-MM-dd") + " 23:59");
                        break;
                    default:
                        break;
                }
                returnList.Add(startDate);
                returnList.Add(endDate);
                return returnList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
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



            List<PValue>[] results = new List<PValue>[1];
            try
            {
                string[] paras = calcuinfo.fparas.Split(';');
                List<PValue> intValues = new List<PValue>();
                foreach (List<PValue> item in inputs)
                {
                    intValues.Add(item[0]);
                }
                for (int i = 0; i < intValues.Count - 1; i++)
                {
                    double V, M, k, b, A, R1, R2, J1, J2, L;
                    string P = string.Empty;
                    V = int.Parse(paras[0]);
                    M = int.Parse(paras[1]);
                    P = paras[2];
                    k = double.Parse(paras[3]);
                    b = double.Parse(paras[4]);
                    A = int.Parse(paras[5]);
                    R1 = double.Parse(paras[6]);
                    R2 = double.Parse(paras[7]);
                    J1 = int.Parse(paras[8]);
                    J2 = int.Parse(paras[9]);
                    L = int.Parse(paras[10]);
                    string type = calcuinfo.fsourtagids[i].ToString();

                    double newValue = (A == 0 ? intValues[i].Value : Math.Abs(intValues[i].Value)) * k + b;
                    double x = 0;
                    if (V == 0)
                    {
                        bool j1Result = false;
                        bool j2Result = false;
                        switch (J1.ToString())
                        {
                            case "0":
                                if (newValue > R1)
                                {
                                    j1Result = true;
                                }
                                break;
                            case "1":
                                if (newValue >= R1)
                                {
                                    j1Result = true;
                                }
                                break;
                            case "2":
                                if (newValue < R1)
                                {
                                    j1Result = true;
                                }
                                break;
                            case "3":
                                if (newValue <= R1)
                                {
                                    j1Result = true;
                                }
                                break;
                            default:
                                break;
                        }
                        switch (J2.ToString())
                        {
                            case "0":
                                if (newValue > R2)
                                {
                                    j2Result = true;
                                }
                                break;
                            case "1":
                                if (newValue >= R2)
                                {
                                    j2Result = true;
                                }
                                break;
                            case "2":
                                if (newValue < R2)
                                {
                                    j2Result = true;
                                }
                                break;
                            case "3":
                                if (newValue <= R2)
                                {
                                    j2Result = true;
                                }
                                break;
                            default:
                                break;
                        }

                        switch (L.ToString())
                        {
                            case "0":
                                if (j1Result && j2Result)
                                {
                                    x = 1;
                                }
                                else
                                {
                                    x = 0;
                                }
                                break;
                            case "1":
                                if (j1Result || j2Result)
                                {
                                    x = 1;
                                }
                                else
                                {
                                    x = 0;
                                }
                                break;
                            case "2":
                                if (!j1Result && !j2Result)
                                {
                                    x = 1;
                                }
                                else
                                {
                                    x = 0;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        x = newValue;
                    }

                    string tableNo = string.Empty;
                    List<DateTime> seDate = getDateRegion(P, intValues[i].Timestamp);

                    DataTable dt = BLL.AlgorithmBLL.getMHisOpHistory("MHisOp" + type + P, seDate);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        double newX = 0;
                        if (M == 0)
                        {
                            newX = Convert.ToDouble(dt.Rows[0]["tagvalue"].ToString()) + x;
                        }
                        else if (M == 1)
                        {
                            TimeSpan d1 = intValues[i].Timestamp.Subtract(seDate[0]);
                            int mins = d1.Minutes;
                            newX = Math.Round((Convert.ToDouble(dt.Rows[0]["tagvalue"].ToString()) * mins + x) / (mins + 1), 2);
                        }
                        bool isok = BLL.AlgorithmBLL.UpdateMHisOp("MHisOp" + type + P, newX, seDate);
                        if (!isok)
                        {
                            _errorInfo += "MHisOp" + type + P + " 时间区间：" + seDate[0].ToString("yyyy-MM-dd HH:mm") + "--" + seDate[1].ToString("yyyy-MM-dd HH:mm") + "  时间点：" + intValues[i].Timestamp.ToString("yyyy-MM-dd HH:mm") + "更新报错";
                        }
                    }
                    else
                    {
                        bool isok = BLL.AlgorithmBLL.InsertMHisOp("MHisOp" + type + P, x, seDate);
                        if (!isok)
                        {
                            _errorInfo += "MHisOp" + type + P + " 时间区间：" + seDate[0].ToString("yyyy-MM-dd HH:mm") + "--" + seDate[1].ToString("yyyy-MM-dd HH:mm") + "  时间点：" + intValues[i].Timestamp.ToString("yyyy-MM-dd HH:mm") + "录入报错";
                        }
                    }
                }




                if (string.IsNullOrWhiteSpace(_errorInfo))
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                else
                {
                    _fatalFlag = true;
                    _fatalInfo = "MHisOp算法出错";
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
