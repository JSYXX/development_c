using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.Helper;
using PSLCalcu.Module.NewCaculate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class EquSigma : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "EquSigma";
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
        private string _algorithms = "EquSigma";
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


        private int _outputNumber = 1;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "equSigma;" +
                                      "equSigmaMaxP;" +
                                      "equSigmaMinP"
;




        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "全部点每分钟数据";



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
                string[] paras = calcuinfo.fparas.Split(';');
                mode = paras[0];
                double equSigma = 0;
                int equSigmaMaxP = 0;
                int equSigmaMinP = 0;
                ArrayList equSigmaList = new ArrayList();
                if (mode == "S")
                {
                    int l = 1;
                    List<CurveClass> MessageInList = new List<CurveClass>();
                    int point = 1;
                    foreach (List<PValue> item in inputs)
                    {
                        List<CurveClass> childList = new List<CurveClass>();
                        int childL = 1;
                        foreach (PValue childItem in item)
                        {
                            CurveClass cClass = new CurveClass();
                            cClass.x = l;
                            cClass.y = childItem.Value;
                            MessageInList.Add(cClass);
                            CurveClass childClass = new CurveClass();
                            childClass.x = childL++;
                            childClass.y = childItem.Value;
                            childList.Add(childClass);
                            l++;
                        }
                        equSigmaList.Add(new DictionaryEntry(point++, AlgorithmHelper.StandardDeviationSolve(childList)));
                    }
                    equSigma = AlgorithmHelper.StandardDeviationSolve(MessageInList);
                    equSigmaMaxP = Convert.ToInt32(((DictionaryEntry)equSigmaList[0]).Key);
                    equSigmaMinP = Convert.ToInt32(((DictionaryEntry)equSigmaList[0]).Key);
                    double equSigmaMax = Convert.ToDouble(((DictionaryEntry)equSigmaList[0]).Value);
                    double equSigmaMin = Convert.ToDouble(((DictionaryEntry)equSigmaList[0]).Value);
                    foreach (var item in equSigmaList)
                    {
                        if (equSigmaMax < Convert.ToDouble(((DictionaryEntry)item).Value))
                        {
                            equSigmaMax = Convert.ToDouble(((DictionaryEntry)item).Value);
                            equSigmaMaxP = Convert.ToInt32(((DictionaryEntry)item).Key);
                        }
                        if (equSigmaMin > Convert.ToDouble(((DictionaryEntry)item).Value))
                        {
                            equSigmaMin = Convert.ToDouble(((DictionaryEntry)item).Value);
                            equSigmaMinP = Convert.ToInt32(((DictionaryEntry)item).Key);
                        }
                    }

                }
                else
                {
                    double maxSigma = inputs[0][0].Value;
                    List<int> equSigmaMaxPList = new List<int>();
                    List<int> equSigmaMinPList = new List<int>();
                    foreach (List<PValue> item in inputs)
                    {
                        equSigmaMaxPList.Add(Convert.ToInt32(item[1].Value));
                        equSigmaMinPList.Add(Convert.ToInt32(item[2].Value));
                        foreach (PValue childItem in item)
                        {
                            if (maxSigma < childItem.Value)
                            {
                                maxSigma = childItem.Value;
                            }
                        }
                    }
                    equSigma = maxSigma * 0.75;
                    equSigmaMaxP= equSigmaMaxPList.GroupBy(x => x).OrderBy(y => y.Count()).First().Key;
                    equSigmaMinP = equSigmaMinPList.GroupBy(x => x).OrderBy(y => y.Count()).First().Key;
                }
                results[0].Add(new PValue(equSigma, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(equSigmaMaxP, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[0].Add(new PValue(equSigmaMinP, calcuinfo.fstarttime, calcuinfo.fendtime, 0));

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
    }
}
