using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.Base_MultiInput
{
    public class MMangSumAvg
    {
        private static List<PValue> _inputData;
        public static List<PValue> inputData
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
        public static Results Calcu()
        {
            return Calcu(_inputData, _calcuInfo);
        }
        public static Results Calcu(List<PValue> inputs, CalcuInfo calcuinfo)
        {
            //公用变量
            bool _errorFlag = false;
            string _errorInfo = "";
            bool _warningFlag = false;
            string _warningInfo = "";
            bool _fatalFlag = false;
            string _fatalInfo = "";


            List<PValue>[] results = new List<PValue>[1];
            results[0] = new List<PValue>();
            results[0].Add(new PValue(0, calcuinfo.fstarttime, calcuinfo.fendtime, (long)StatusConst.InputIsNull));
            try
            {
                if (inputs == null || inputs.Count == 0)
                {
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);        //不报错，直接返回默认值
                }
                //处理参数
                double k;
                double b;
                List<double> kiList = new List<double>();
                string[] paras = calcuinfo.fparas.Split(';');
                k = float.Parse(paras[0]);
                b = float.Parse(paras[1]);
                //加权数处理
                for (int i = 2; i < paras.Count() - 1; i++)
                {
                    kiList.Add(Convert.ToDouble(paras[i]));
                }
                //判断权数与数据数量是否相同
                if (kiList.Count != inputs.Count)
                {
                    _warningFlag = true;  //报错
                    _warningInfo = "权数与数据数量不一致。";
                    return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
                }
                //声明加权和
                double SumAmount = 0;
                for (int i = 0; i < inputs.Count; i++)
                {
                    SumAmount += inputs[i].Value * kiList[i];
                }
                //声明返回参数
                List<PValue> result = new List<PValue>();
                double MANGSum, MANGAvg = 0;
                MANGSum = k * SumAmount + b;
                MANGAvg = k * SumAmount / (double)(inputs.Count) + b;
                result.Add(new PValue(MANGSum, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                result.Add(new PValue(MANGAvg, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), 0));
                results[0] = result;
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

            }
            catch (Exception ex)
            {
                _fatalFlag = true;
                _fatalInfo = ex.ToString();
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
    }
}
