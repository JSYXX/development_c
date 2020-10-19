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
    }
}
