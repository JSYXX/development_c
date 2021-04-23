using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    public class StateRelation : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MDevLimitSft";
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
        private string _algorithms = "MDevLimitSft";
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
        private string _moduleParaExample = "A,B,pa,pb;A,B,pa,pb;A,B,pa,pb;";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "A,B,pa,pb;A,B,pa,pb;A,B,pa,pb;";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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


        private int _outputNumber = 17;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Y0;" +
                                    "Y1;" +
                                    "Y2;" +
                                    "Y3";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "x1，x2，……，同时满足条件，y0 = 1，否则为 0;" +
                                        "x1，x2，……，任意 1 项满足条件，y1 = 1，否则为 0;" +
                                        "x1，x2，……，任意 2 项满足条件，y2 = 1，否则为 0;" +
                                        "x1，x2，……，任意 3 项满足条件，y3 = 1，否则为 0";

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

            //int i;

            //0输出初始化：该算法如果没有有效输入值（inputs为null）或者输入值得有效值为null，给出的计算结果。值为0，计算标志位为StatusConst.InputIsNull
            List<PValue>[] results = new List<PValue>[4];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new List<PValue>();
                results[i].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            }

            try
            {
                string[] paras = calcuinfo.fparas.Split(';');
                List<int> resultList = new List<int>();
                for (int i = 0; i < inputs.Count() - 1; i++)
                {
                    int resultA = 1;
                    int resultB = 1;
                    double amount = inputs[i][0].Value;
                    string[] childParas = paras[i].Split(',');
                    double A = Convert.ToDouble(childParas[0]);
                    double B = Convert.ToDouble(childParas[1]);
                    int pa = Convert.ToInt32(childParas[2]);
                    int pb = Convert.ToInt32(childParas[3]);
                    int pr = Convert.ToInt32(childParas[4]);
                    if (pa == 0)
                    {
                        if (amount > A)
                        {

                        }
                        else
                        {
                            resultA = 0;
                        }
                    }
                    else if (pa == 1)
                    {
                        if (amount >= A)
                        {

                        }
                        else
                        {
                            resultA = 0;
                        }
                    }
                    else if (pa == 2)
                    {
                        if (amount < A)
                        {

                        }
                        else
                        {
                            resultA = 0;
                        }
                    }
                    else if (pa == 3)
                    {
                        if (amount <= A)
                        {

                        }
                        else
                        {
                            resultA = 0;
                        }
                    }


                    if (pb == 0)
                    {
                        if (amount > B)
                        {

                        }
                        else
                        {
                            resultB = 0;
                        }
                    }
                    else if (pb == 1)
                    {
                        if (amount >= B)
                        {

                        }
                        else
                        {
                            resultB = 0;
                        }
                    }
                    else if (pb == 2)
                    {
                        if (amount < B)
                        {

                        }
                        else
                        {
                            resultB = 0;
                        }
                    }
                    else if (pb == 3)
                    {
                        if (amount <= B)
                        {

                        }
                        else
                        {
                            resultB = 0;
                        }
                    }
                    int resultAmount = 0;
                    if (pr == 0)
                    {
                        if (resultA == 1 && resultB == 1)
                        {
                            resultAmount = 1;
                        }
                        else
                        {
                            resultAmount = 0;
                        }
                    }
                    else if (pr == 1)
                    {
                        if (resultA == 0 && resultB == 0)
                        {
                            resultAmount = 0;
                        }
                        else
                        {
                            resultAmount = 1;
                        }
                    }
                    resultList.Add(resultAmount);
                }
                int rightCount = 0;
                int wrongCount = 0;
                foreach (int item in resultList)
                {
                    if (item == 1)
                    {
                        rightCount++;
                    }
                    else
                    {
                        wrongCount++;
                    }
                }

                double y0 = 0;
                double y1 = 0;
                double y2 = 0;
                double y3 = 0;
                if (wrongCount == 0)
                {
                    y0 = 1;
                }
                else
                {
                    if (rightCount == 1)
                    {
                        y1 = 1;
                    }
                    else if (rightCount == 2)
                    {
                        y2 = 1;
                    }
                    else if (rightCount >= 3)
                    {
                        y3 = 1;
                    }
                }
                results[0].Add(new PValue(y0, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[1].Add(new PValue(y1, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[2].Add(new PValue(y2, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                results[3].Add(new PValue(y3, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
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
