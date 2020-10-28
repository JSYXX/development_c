using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.Helper
{
    public static class MathHelper
    {
        public static string returnAllStr(string str)
        {
            try
            {
                if (str != "0")
                {

                    string[] strlist = str.Split(new[] { "E+" }, StringSplitOptions.None);
                    long timeStr = 0;
                    timeStr = (long)(Convert.ToDouble(strlist[0]) * Math.Pow(10, Convert.ToDouble(strlist[1])));
                    return timeStr.ToString();
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string returnProportionStr(double molecule, int denominator)
        {
            try
            {
                string returnStr = string.Empty;
                if (molecule == 0)
                {
                    returnStr = "0";
                }
                else
                {
                    returnStr = (Math.Round(molecule / (double)denominator, 5) * 100).ToString();
                }
                return returnStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string returnProportionUpdateStr(double molecule, int denominator, string oldData)
        {
            try
            {
                string returnStr = string.Empty;
                double oData = Convert.ToDouble(oldData);
                if (molecule == 0 && oData == 0)
                {
                    returnStr = "0";
                }
                else
                {
                    returnStr = (Math.Round((oData / (double)100 * denominator + molecule) / (double)(denominator + 1), 5) * 100).ToString();
                }
                return returnStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
