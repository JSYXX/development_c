using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module.New_Base_Caculate
{
    public class MultiDivision : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：

        private string _moduleName = "MultiDivision";
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
        private string _algorithms = "MultiDivision";
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
        private string _moduleParaExample = "1;2;1;2;1;2;1,0;1,0";   // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "K;B;#1;#2;#3;#4;kList;rList";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
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
        private string _outputDescs = "R";



        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "返回值R";

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

                double bigK, bigB;
                string flag1 = string.Empty;
                string flag2 = string.Empty;
                string flag3 = string.Empty;
                string flag4 = string.Empty;
                string[] paras = calcuinfo.fparas.Split(';');

                bigK = int.Parse(paras[0]);
                bigB = int.Parse(paras[1]);
                flag1 = paras[2];
                flag2 = paras[3];
                flag3 = paras[4];
                flag4 = paras[5];
                string[] smallKList = paras[6].Split(',');
                string[] smallRList = paras[7].Split(',');
                string type = calcuinfo.fsourtagids[0].ToString();

                if (smallKList.Count() + smallRList.Count() != inputs.Count())
                {
                    _warningFlag = true;
                    _warningInfo = "数据个数对应错误。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                List<double> xList = new List<double>();
                List<double> yList = new List<double>();
                for (int l = 0; l < smallKList.Count(); l++)
                {
                    xList.Add(inputs[l][0].Value);
                }
                for (int l = smallKList.Count(); l < inputs.Count(); l++)
                {
                    yList.Add(inputs[l][0].Value);
                }
                double R = 0;
                double sumX = 0;
                double sumY = 0;
                if (flag1 == "1")
                {
                    for (int l = 0; l < xList.Count; l++)
                    {
                        sumX += Math.Abs(xList[l] * Convert.ToDouble(smallKList[l]));
                    }
                }
                else
                {
                    for (int l = 0; l < xList.Count; l++)
                    {
                        sumX += xList[l] * Convert.ToDouble(smallKList[l]);
                    }
                }
                if (flag3 == "1")
                {
                    sumX = Math.Abs(sumX);
                }
                if (flag2 == "1")
                {
                    for (int l = 0; l < yList.Count; l++)
                    {
                        sumY += Math.Abs(yList[l] * Convert.ToDouble(smallRList[l]));
                    }
                }
                else
                {
                    for (int l = 0; l < yList.Count; l++)
                    {
                        sumY += yList[l] * Convert.ToDouble(smallRList[l]);
                    }
                }
                if (flag4 == "1")
                {
                    sumY = Math.Abs(sumY);
                }
                R = Math.Round((sumX / sumY) * bigK, 2) + bigB;
                //初始化
                results[0].Add(new PValue(R, calcuinfo.fstarttime, calcuinfo.fendtime, 0));
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                //计算结果存入数据库
                //bool isok = BLL.AlgorithmBLL.insertMPVBasePlusSft(mpvMessageInClass, isNewAdd);
                //if (isok)
                //{
                //    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                //}
                //else
                //{
                //    _fatalFlag = true;
                //    _fatalInfo = "MPVBasePlusSft录入数据失败";
                //    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                //}








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
