using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using Config;   //使用log

namespace PSLCalcu.Module
{
    /// <summary>
    /// 积分   
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///     2018.04.02 版本  1.2 arrow 修改。添加输入数据中截止时刻数据的处理。
    ///		2017.03.17 版本：1.0 wujunbo 创建。    
    /// <author>
    ///		<name>wujunbo</name>
    ///		<date>2017.03.17</date>
    /// </author> 
    /// </summary>
    public class MIntegral : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "Integral";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "积分";
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
        private string _algorithms = "Integral";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "YY";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "10000;1;2";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "积分复位限值;积分复位方式(0或1);积分放大倍数";// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        private Regex _moduleParaRegex=new Regex(@"^(\d+\.{0,1}\d{0,}){1};(0|1){1}(;\d+\.{0,1}\d{0,}){1}$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 2;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Integral;IntegralResetN";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "积分值;积分复位次数";
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
        /// 计算模块算法实现:求限定时间内积分值，用梯形面积累加
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数:积分限值、积分复位方式、积分放大倍数</param>       
        /// <returns>计算结果分别为积分值、积分复位次数</returns>
       
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

            try 
            {
                if (null == inputs) {
                    _fatalInfo = "计算函数：MIntegral----输入为空------";
                }
                List<PValue> input = inputs[0];

                //处理截止时刻数据
                if (input.Count > 1) input.RemoveAt(input.Count - 1);

                //初始化
                PValue Integral = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//积分
                PValue IntegralResetT = new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, 0);//积分复位次数
                //Console.WriteLine("请输入积分限值、积分复位方式（0或1）、积分放大倍数，顺序输入各参数，“;”分隔各个参数\n");
                //calcuinfo.fparas = Console.ReadLine();
                string[] para = calcuinfo.fparas.Split(';');
                double Integrallimit = double.Parse(para[0]);
                int InteResetway = int.Parse(para[1]);
                double Integralgaint = double.Parse(para[2]);


                //输出为积分值与积分复位次数
                List<PValue>[] results = new List<PValue>[2];

                int len = input.Count;                
                

                //概化计算中的所有计算结果均为List<PValue>[]，当计算中出现任何已知可以判断的问题时，均用results=null来返回
                //计算引擎在处理结果时
                if (len <= 0) return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);    //如果长度为零，返回空数组，计算引擎对空数组做跳过处理，不向数据库写入任何数据
                for (i = 0; i < len - 1; i++)
                {
                    if (Integral.Value < Integrallimit)
                    {
                        TimeSpan tspan = input[i + 1].Timestamp - input[i].Timestamp;
                        Integral.Value += (input[i].Value + input[i + 1].Value) * tspan.TotalSeconds / 2;
                    }
                    else
                    {
                        //根据积分复位方式来对积分进行复位
                        if (InteResetway == 0)
                            Integral.Value = Integral.Value - Integrallimit;
                        if (InteResetway == 1)
                            Integral.Value = 0;

                        IntegralResetT.Value++;

                    }

                }

                //所得积分值再乘以积分放大倍数
                Integral.Value = Integral.Value * Integralgaint;

                //组织计算结果
                //计算模块结果寄存器：
                //results[0]，积分值，List<PValue>类型，仅一个值PValue值；
                //results[1]，积分复位次数，List<PValue>类型，仅一个值PValue值；
                results[0] = new List<PValue>();
                results[0].Add(Integral);

                results[1] = new List<PValue>();
                results[1].Add(IntegralResetT);

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
                _fatalInfo =_fatalInfo + ex.ToString();
               
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }    
        }


        #endregion
    }
}
