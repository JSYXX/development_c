using Model;
using PSLCalcu.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class AlgorithmHelper
    {
        /// <summary>
        /// 一元线性回归
        /// </summary>
        /// <param name="_plist">坐标值集合</param>
        /// <param name="returnK">斜率</param>
        /// <param name="returnB">截距</param>
        public static void LinearRegressionSolve(List<CurveClass> _plist, ref double returnK, ref double returnB)
        {
            try
            {
                double k = 0, b = 0;
                double sumX = 0, sumY = 0;
                double avgX = 0, avgY = 0;
                foreach (var v in _plist)
                {
                    sumX += v.x;
                    sumY += v.y;
                }
                avgX = sumX / (_plist.Count + 0.0);
                avgY = sumY / (_plist.Count + 0.0);

                //sumA=(x-avgX)(y-avgY)的和 sumB=(x-avgX)平方
                double sumA = 0, sumB = 0;
                foreach (var v in _plist)
                {
                    sumA += (v.x - avgX) * (v.y - avgY);
                    sumB += (v.x - avgX) * (v.x - avgX);
                }
                k = sumA / (sumB + 0.0);
                b = avgY - k * avgX;
                returnK = Math.Round(k, 3);
                returnB = Math.Round(b, 3);

            }
            catch (Exception ex)
            {
                throw new Exception("线性回归算法出错");
            }
        }
        /// <summary>
        /// 标准差
        /// </summary>
        /// <param name="_plist">坐标值集合</param>
        /// <returns>标准差</returns>
        public static double StandardDeviationSolve(List<CurveClass> _plist)
        {
            double avgValue = 0;
            double sumValue = 0;
            double variance = 0;
            foreach (CurveClass item in _plist)
            {
                sumValue += item.y;
            }
            avgValue = sumValue / (_plist.Count + 0.0);
            foreach (CurveClass item in _plist)
            {
                variance += Math.Pow((item.y - avgValue), 2.0);
            }
            return Math.Round(Math.Sqrt(variance / (_plist.Count() + 0.0)), 3);
        }
        public static double VarianceSolve(List<CurveClass> _plist)
        {
            double avgValue = 0;
            double sumValue = 0;
            double variance = 0;
            foreach (CurveClass item in _plist)
            {
                sumValue += item.y;
            }
            avgValue = sumValue / (_plist.Count + 0.0);
            foreach (CurveClass item in _plist)
            {
                variance += Math.Pow((item.y - avgValue), 2.0);
            }
            return Math.Round(variance, 3);
        }
        /// <summary>
        /// 单次极差集合
        /// </summary>
        /// <param name="valueList">值类集合</param>
        /// <returns>返回单次极差的集合</returns>

        public static MDevLimitClass GetLimit(List<D22STimeClass> valueList,int totalEffectiveCount)
        {
            MDevLimitClass Limit = new MDevLimitClass();
            if (valueList.Count > 0)
            {
                double DevHHN = 0;
                double DevHHTMax = valueList[0].valueCount;
                D22STimeClass DevHHTMaxTime = valueList[0];
                double DevHHA = 0;
                foreach (D22STimeClass item in valueList)
                {
                    DevHHN += item.valueCount;
                    if (DevHHTMax < item.valueCount)
                    {
                        DevHHTMax = item.valueCount;
                        DevHHTMaxTime = item;
                    }
                    DevHHA += item.valueAmount;
                }
                Limit.DevN = valueList.Count.ToString();//越 HH 次数，x ≥ HH
                Limit.DevTime = valueList;//越 HH 时刻
                Limit.DevT = DevHHN.ToString();//越 HH 总时长，n 周期
                Limit.DevR = (Math.Round((DevHHN / (double)(totalEffectiveCount)), 5) * 100).ToString();//越 HH 时间占比，%
                Limit.DevTMax = DevHHTMax.ToString();
                Limit.DevTMaxTime = DevHHTMaxTime;
                Limit.DevA = DevHHA.ToString();
                Limit.DevET = Math.Round(DevHHA / DevHHN, 3).ToString(); ;
            }
            return Limit;
        }
        public static MDevLimitClass GetLimit1(List<D22STimeClass> valueList)
        {
            MDevLimitClass Limit = new MDevLimitClass();
            if (valueList.Count > 0)
            {
                double DevHHTMax = valueList[0].valueCount;
                D22STimeClass DevHHTMaxTime = valueList[0];
                foreach (D22STimeClass item in valueList)
                {
                    if (DevHHTMax < item.valueCount)
                    {
                        DevHHTMax = item.valueCount;
                        DevHHTMaxTime = item;
                    }
                }
                Limit.DevTime = valueList;
                Limit.DevTMax = DevHHTMax.ToString();
                Limit.DevTMaxTime = DevHHTMaxTime;
            }
            return Limit;
        }

        public static List<D22STimeClass> GetD22STimeList(List<MPVBaseMessageInBadClass> valueList, double maxValue, double minValue, int type)
        {
            try
            {
                List<D22STimeClass> returnClass = new List<D22STimeClass>();
                string sDate = string.Empty;
                string preDate = string.Empty;
                double vCount = 0;
                int status = 0;
                int i = 1;
                if (type == 0)
                {
                    foreach (MPVBaseMessageInBadClass item in valueList)
                    {
                        if (i < valueList.Count)
                        {
                            //初始状态或者坏点
                            if (status == 0)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue)
                                    {
                                        vCount += 1;
                                        sDate = item.valueDate;
                                        preDate = item.valueDate;
                                        status = 1;
                                    }
                                    else
                                    {
                                        status = 2;
                                    }
                                }
                            }
                            //在阈值内
                            else if (status == 1)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = vCount;
                                    returnClass.Add(newClass);
                                    sDate = string.Empty;
                                    preDate = string.Empty;
                                    vCount = 0;
                                    status = 0;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue)
                                    {
                                        vCount += 1;
                                        preDate = item.valueDate;
                                    }
                                    else
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount;
                                        returnClass.Add(newClass);
                                        sDate = string.Empty;
                                        preDate = string.Empty;
                                        vCount = 0;
                                        status = 2;
                                    }
                                }
                            }
                            //在阈值外
                            else if (status == 2)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    status = 0;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue)
                                    {
                                        vCount += 1;
                                        sDate = item.valueDate;
                                        preDate = item.valueDate;
                                        status = 1;
                                    }
                                    else
                                    {
                                        status = 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (status == 0)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = item.valueDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = 1;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                            //在阈值内
                            else if (status == 1)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = vCount;
                                    returnClass.Add(newClass);
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount + 1;
                                        returnClass.Add(newClass);
                                    }
                                    else
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                            //在阈值外
                            else if (status == 2)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = item.valueDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = 1;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                        }
                        i++;
                    }
                }
                else if (type == 1)
                {
                    foreach (MPVBaseMessageInBadClass item in valueList)
                    {
                        if (i < valueList.Count)
                        {
                            if (status == 0)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue && (Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        vCount += 1;
                                        sDate = item.valueDate;
                                        preDate = item.valueDate;
                                        status = 1;
                                    }
                                    else
                                    {
                                        status = 2;
                                    }
                                }
                            }
                            else if (status == 1)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = vCount;
                                    returnClass.Add(newClass);
                                    sDate = string.Empty;
                                    preDate = string.Empty;
                                    vCount = 0;
                                    status = 0;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue && (Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        vCount += 1;
                                        preDate = item.valueDate;
                                    }
                                    else
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount;
                                        returnClass.Add(newClass);
                                        sDate = string.Empty;
                                        preDate = string.Empty;
                                        vCount = 0;
                                        status = 2;
                                    }
                                }
                            }
                            else if (status == 2)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    status = 0;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue && (Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        vCount += 1;
                                        sDate = item.valueDate;
                                        preDate = item.valueDate;
                                        status = 1;
                                    }
                                    else
                                    {
                                        status = 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (status == 0)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue && (Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = item.valueDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = 1;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                            //在阈值内
                            else if (status == 1)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = vCount;
                                    returnClass.Add(newClass);
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue && (Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount + 1;
                                        returnClass.Add(newClass);
                                    }
                                    else
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                            //在阈值外
                            else if (status == 2)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) < maxValue && (Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = item.valueDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = 1;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                        }
                        i++;
                    }
                }
                else if (type == 2)
                {
                    foreach (MPVBaseMessageInBadClass item in valueList)
                    {
                        if (i < valueList.Count)
                        {
                            if (status == 0)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        vCount += 1;
                                        sDate = item.valueDate;
                                        preDate = item.valueDate;
                                        status = 1;
                                    }
                                    else
                                    {
                                        status = 2;
                                    }
                                }
                            }
                            else if (status == 1)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = vCount;
                                    returnClass.Add(newClass);
                                    sDate = string.Empty;
                                    preDate = string.Empty;
                                    vCount = 0;
                                    status = 0;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        vCount += 1;
                                        preDate = item.valueDate;
                                    }
                                    else
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount;
                                        returnClass.Add(newClass);
                                        sDate = string.Empty;
                                        preDate = string.Empty;
                                        vCount = 0;
                                        status = 2;
                                    }
                                }
                            }
                            else if (status == 2)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    status = 0;
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        vCount += 1;
                                        sDate = item.valueDate;
                                        preDate = item.valueDate;
                                        status = 1;
                                    }
                                    else
                                    {
                                        status = 2;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (status == 0)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = item.valueDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = 1;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                            //在阈值内
                            else if (status == 1)
                            {
                                if (item.valueAmount.Equals("-"))
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = vCount;
                                    returnClass.Add(newClass);
                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount + 1;
                                        returnClass.Add(newClass);
                                    }
                                    else
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = vCount;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                            //在阈值外
                            else if (status == 2)
                            {
                                if (item.valueAmount.Equals("-"))
                                {

                                }
                                else
                                {
                                    if ((Convert.ToInt32(item.valueAmount)) >= minValue)
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = item.valueDate;
                                        newClass.endDate = item.valueDate;
                                        newClass.valueCount = 1;
                                        returnClass.Add(newClass);
                                    }
                                }
                            }
                        }
                        i++;
                    }
                }
                return returnClass;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 判断点的值是否在一个区域的跨时段合并
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static List<D22STimeClass> GetD22STimeList(D22STimeClass d1, D22STimeClass d2)
        {
            try
            {
                List<D22STimeClass> newList = new List<D22STimeClass>();
                if (d1.valueType != d2.valueType)
                {
                    newList.Add(d1);
                    newList.Add(d2);
                }
                else
                {
                    if (Convert.ToDateTime(d1.endDate).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss") == Convert.ToDateTime(d2.startDate).ToString("yyyy-MM-dd HH:mm:ss"))
                    {
                        D22STimeClass d3 = new D22STimeClass();
                        d3.startDate = d1.startDate;
                        d3.endDate = d2.endDate;
                        d3.valueType = d1.valueType;
                        d3.valueCount = d1.valueCount + d2.valueCount + 1;
                        newList.Add(d3);
                    }
                    else
                    {
                        newList.Add(d1);
                        newList.Add(d2);
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 稳定值跨时段合并
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static List<D22STimeClass> GetD22STimeListStb(D22STimeClass d1, D22STimeClass d2, double stbL, double noStbL)
        {
            try
            {
                List<D22STimeClass> newList = new List<D22STimeClass>();
                if (Convert.ToDateTime(d1.endDate).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss") == Convert.ToDateTime(d2.startDate).ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    string valueType = string.Empty;
                    if (Math.Abs(d2.startValue - d1.endValue) <= stbL)
                    {
                        valueType = "Stb";
                    }
                    else if (Math.Abs(d2.startValue - d1.endValue) > noStbL)
                    {
                        valueType = "noStb";
                    }
                    if (string.IsNullOrWhiteSpace(valueType))
                    {
                        newList.Add(d1);
                        newList.Add(d2);
                    }
                    else if (d1.valueType == valueType && valueType == d2.valueType)
                    {
                        if (valueType == d2.valueType)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueType = d1.valueType;
                            d3.valueAmount = d1.valueAmount + d2.valueAmount + Math.Abs(d2.startValue - d1.endValue);
                            d3.valueCount = d1.valueCount + d2.valueCount + 1;
                            newList.Add(d3);
                        }
                        else
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueType = d1.valueType;
                            d3.valueAmount = d1.valueAmount + Math.Abs(d2.startValue - d1.endValue);
                            d3.valueCount = d1.valueCount + 1;
                            newList.Add(d3);
                            newList.Add(d2);
                        }
                    }
                    else if (d1.valueType != valueType)
                    {
                        if (valueType == d2.valueType)
                        {
                            newList.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueType = d2.valueType;
                            d3.valueAmount = d2.valueAmount + Math.Abs(d2.startValue - d1.endValue);
                            d3.valueCount = d1.valueCount + d2.valueCount + 1;
                            newList.Add(d3);
                        }
                        else
                        {
                            newList.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueType = valueType;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueCount = 1;
                            newList.Add(d3);
                            newList.Add(d2);
                        }
                    }

                }
                else
                {
                    newList.Add(d1);
                    newList.Add(d2);
                }
                return newList;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 极差跨时段合并
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static List<D22STimeClass> GetD22STimeListSD(D22STimeClass d1, D22STimeClass d2)
        {
            List<D22STimeClass> returnClass = new List<D22STimeClass>();
            if (Convert.ToDateTime(d1.endDate).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss") == Convert.ToDateTime(d2.startDate).ToString("yyyy-MM-dd HH:mm:ss"))
            {
                if (d1.valueType == "up")
                {
                    if (d2.valueType == "up")
                    {
                        if (d1.endValue < d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.startValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                        }
                        else if (d1.endValue == d2.startValue) 
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                    }
                    else if (d2.valueType == "down")
                    {
                        if (d1.endValue < d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.startValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.endValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                        }
                    }
                    else 
                    {
                        if (d1.endValue < d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.startValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                            returnClass.Add(d2);//新增
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.endValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                    }
                }
                else if (d1.valueType == "down")
                {
                    if (d2.valueType == "down")
                    {
                        if (d1.endValue > d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.startValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                            returnClass.Add(d2);//新增
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                    }
                    else if (d2.valueType == "up")
                    {
                        if (d1.endValue > d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.startValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.endValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                        }
                    }
                    else
                    {
                        if (d1.endValue < d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                            returnClass.Add(d2);//新增
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.endValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                        }
                        else
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.startValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                    }
                }
                else 
                {
                    if (d2.valueType == "down")
                    {
                        if (d1.endValue > d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.endValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.startValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                            returnClass.Add(d2);//新增
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                    }
                    else if (d2.valueType == "up")
                    {
                        if (d1.endValue > d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.startValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.endValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                        }
                    }
                    else
                    {
                        if (d1.endValue < d2.startValue)
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "up";
                            returnClass.Add(d3);
                            returnClass.Add(d2);//新增
                        }
                        else if (d1.endValue == d2.startValue)
                        {
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.startDate;
                            d3.startValue = d1.startValue;
                            d3.endDate = d2.endDate;
                            d3.endValue = d2.endValue;
                            d3.valueAmount = Math.Abs(d2.endValue - d1.startValue);
                            d3.valueType = "translation";
                            returnClass.Add(d3);
                        }
                        else
                        {
                            returnClass.Add(d1);
                            D22STimeClass d3 = new D22STimeClass();
                            d3.startDate = d1.endDate;
                            d3.startValue = d1.endValue;
                            d3.endDate = d2.startDate;
                            d3.endValue = d2.startValue;
                            d3.valueAmount = Math.Abs(d2.startValue - d1.endValue);
                            d3.valueType = "down";
                            returnClass.Add(d3);
                            returnClass.Add(d2);
                        }
                    }
                }
            }
            else
            {
                returnClass.Add(d1);
                returnClass.Add(d2);
            }
            return returnClass;
        }

        public static List<List<MPVBaseMessageInClass>> getEffectValueList(List<MPVBaseMessageInBadClass> valueList, ref List<MPVBaseMessageInClass> effectValueList, ref bool isAllBad)
        {
            List<List<MPVBaseMessageInClass>> returnClass = new List<List<MPVBaseMessageInClass>>();
            int status = 0;
            bool isBad = true;
            List<MPVBaseMessageInClass> newlist = new List<MPVBaseMessageInClass>();
            List<MPVBaseMessageInClass> refList = new List<MPVBaseMessageInClass>();
            foreach (MPVBaseMessageInBadClass item in valueList)
            {
                if (item.valueAmount.Equals("-"))
                {
                    if (newlist.Count() > 0)
                    {
                        returnClass.Add(newlist);
                    }
                    newlist = new List<MPVBaseMessageInClass>();
                }
                else
                {
                    isBad = false;
                    MPVBaseMessageInClass newClass = new MPVBaseMessageInClass();
                    newClass.seq = item.seq;
                    newClass.valueDate = item.valueDate;
                    newClass.valueAmount = Convert.ToDouble(item.valueAmount);
                    newlist.Add(newClass);
                    refList.Add(newClass);
                }
            }
            returnClass.Add(newlist);
            isAllBad = isBad;
            effectValueList = refList;
            return returnClass;


        }

        public static D22STimeClass compareFunction(List<D22STimeClass> valueList, ref double reValue)
        {
            try
            {
                D22STimeClass newClass = valueList[0];
                double newValue = valueList[0].valueCount;
                foreach (D22STimeClass item in valueList)
                {
                    DateTime endDate = Convert.ToDateTime(item.endDate);
                    DateTime startDate = Convert.ToDateTime(item.startDate);
                    TimeSpan min = endDate.Subtract(startDate);
                    item.valueCount = min.TotalMinutes+1;
                    

                    if (newValue < item.valueCount)
                    {
                        newValue = item.valueCount;
                        newClass = item;
                    }
                }
                reValue = newValue;
                return newClass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int getTNum(List<MPVBaseMessageInClass> valueList)
        {
            int TNum = 0;
            int status = 0;
            for (int i = 1; i < valueList.Count; i++)
            {
                if (valueList[i].valueAmount > valueList[i - 1].valueAmount)
                {
                    if (status == 2)
                    {
                        TNum++;
                    }
                    status = 1;
                }
                else if (valueList[i].valueAmount < valueList[i - 1].valueAmount)
                {
                    if (status == 1)
                    {
                        TNum++;
                    }
                    status = 2;
                }
            }
            return TNum;
        }
        public static int getMultiTNum(List<MultiEffectMessageInClass> valueList)
        {
            int TNum = 0;
            int status = 0;
            for (int i = 1; i < valueList.Count; i++)
            {
                if (valueList[i].valueAmount > valueList[i - 1].valueAmount)
                {
                    if (status == 2)
                    {
                        TNum++;
                    }
                    status = 1;
                }
                else if (valueList[i].valueAmount < valueList[i - 1].valueAmount)
                {
                    if (status == 1)
                    {
                        TNum++;
                    }
                    status = 2;
                }
            }
            return TNum;
        }
        public static List<List<MultiEffectMessageInClass>> getMultiEffectValueList(List<MultiMessageInClass> valueList, ref List<MultiEffectMessageInClass> effectValueList, ref double effectCount)
        {
            List<List<MultiEffectMessageInClass>> returnClass = new List<List<MultiEffectMessageInClass>>();
            List<MultiEffectMessageInClass> newlist = new List<MultiEffectMessageInClass>();
            List<MultiEffectMessageInClass> refList = new List<MultiEffectMessageInClass>();
            double eCount = 0;
            foreach (MultiMessageInClass item in valueList)
            {
                if (item.valueAmount.Equals("-"))
                {
                    if (newlist.Count() > 0)
                    {
                        returnClass.Add(newlist);
                    }
                    newlist = new List<MultiEffectMessageInClass>();
                }
                else
                {
                    eCount++;
                    MultiEffectMessageInClass newClass = new MultiEffectMessageInClass();
                    newClass.seq = item.seq;
                    newClass.lIndex = item.lIndex;
                    newClass.valueAmount = Convert.ToDouble(item.valueAmount);
                    newlist.Add(newClass);
                    refList.Add(newClass);
                }
            }
            returnClass.Add(newlist);
            effectCount = eCount;
            effectValueList = refList;
            return returnClass;


        }

        public static List<MultiEffectMessageInClass> filter(List<MultiEffectMessageInClass> valueList, double FHigh, double FLow)
        {
            try
            {
                List<MultiEffectMessageInClass> returnClass = new List<MultiEffectMessageInClass>();
                foreach (MultiEffectMessageInClass item in valueList)
                {
                    returnClass.Add(item);
                }
                int countMax = (int)Math.Round((valueList.Count * (FHigh / 100)), 0);
                int countMin = (int)Math.Round((valueList.Count * (FLow / 100)), 0);
                returnClass = (from a in returnClass
                               orderby a.valueAmount descending
                               select a).ToList();
                List<int> removeList = new List<int>();

                for (int l = 0; l < countMax; l++)
                {
                    if (!removeList.Contains(valueList[l].seq))
                    {
                        removeList.Add(valueList[l].seq);
                    }
                }
                returnClass = (from a in returnClass
                               orderby a.valueAmount ascending
                               select a).ToList();
                for (int l = 0; l < countMin; l++)
                {
                    if (!removeList.Contains(valueList[l].seq))
                    {
                        removeList.Add(valueList[l].seq);
                    }
                }

                foreach (int item in removeList)
                {
                    returnClass.RemoveAll(x => x.seq == item);
                }
                return returnClass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string OrderMPVBase(List<MPVBaseMultiMessageInClass> ValueList, string type)
        {
            try
            {
                List<MPVBaseMultiMessageInClass> orderList = new List<MPVBaseMultiMessageInClass>();
                string orderStr = string.Empty;
                #region
                switch (type)
                {
                    case "PVBMin":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBMin != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBMin ascending select a).ToList();
                        }
                        break;
                    case "PVBMinTime":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBMinTime != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBMinTime ascending select a).ToList();
                        }
                        break;
                    case "PVBAvg":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBAvg != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBAvg ascending select a).ToList();
                        }
                        break;
                    case "PVBMax":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBMax != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBMax ascending select a).ToList();
                        }
                        break;
                    case "PVBMaxTime":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBMaxTime != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBMaxTime ascending select a).ToList();
                        }
                        break;
                    case "PVBDMax":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBDMax != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBDMax ascending select a).ToList();
                        }
                        break;
                    case "PVBSum":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBSum != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBSum ascending select a).ToList();
                        }
                        break;
                    case "PVBSumkb":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBSumkb != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBSumkb ascending select a).ToList();
                        }
                        break;
                    case "PVBLinek":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBLinek != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBLinek ascending select a).ToList();
                        }
                        break;
                    case "PVBLineb":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBLineb != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBLineb ascending select a).ToList();
                        }
                        break;
                    case "PVBSumPNR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBSumPNR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBSumPNR ascending select a).ToList();
                        }
                        break;
                    case "PVBAbsSum":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBAbsSum != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBAbsSum ascending select a).ToList();
                        }
                        break;
                    case "PVBStdev":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBStdev != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBStdev ascending select a).ToList();
                        }
                        break;
                    case "PVBVolatility":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBVolatility != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBVolatility ascending select a).ToList();
                        }
                        break;
                    case "PVBSDMax":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBSDMax != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBSDMax ascending select a).ToList();
                        }
                        break;
                    case "PVBSDMaxR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBSDMaxR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBSDMaxR ascending select a).ToList();
                        }
                        break;
                    case "PVBDN1Num":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBDN1Num != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBDN1Num ascending select a).ToList();
                        }
                        break;
                    case "PVBDN2Num":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBDN2Num != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBDN2Num ascending select a).ToList();
                        }
                        break;
                    case "PVBDN3Num":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBDN3Num != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBDN3Num ascending select a).ToList();
                        }
                        break;
                    case "PVBTNum":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBTNum != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBTNum ascending select a).ToList();
                        }
                        break;
                    case "PVBVMax":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBVMax != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBVMax ascending select a).ToList();
                        }
                        break;
                    case "PVBVMin":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBVMin != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBVMin ascending select a).ToList();
                        }
                        break;
                    case "PVBVAvg":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBVAvg != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBVAvg ascending select a).ToList();
                        }
                        break;
                    case "PVBStbTR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBStbTR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBStbTR ascending select a).ToList();
                        }
                        break;
                    case "PVBNoStbTR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBNoStbTR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBNoStbTR ascending select a).ToList();
                        }
                        break;
                    case "PVBStbTSL":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBStbTSL != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBStbTSL ascending select a).ToList();
                        }
                        break;
                    case "PVBStbTSLR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBStbTSLR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBStbTSLR ascending select a).ToList();
                        }
                        break;
                    case "PVBNoStbTSL":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBNoStbTSL != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBNoStbTSL ascending select a).ToList();
                        }
                        break;
                    case "PVBNoStbTSLR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBNoStbTSLR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBNoStbTSLR ascending select a).ToList();
                        }
                        break;
                    case "PVBUpTSL":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBUpTSL != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBUpTSL ascending select a).ToList();
                        }
                        break;
                    case "PVBUpTSLR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBUpTSLR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBUpTSLR ascending select a).ToList();
                        }
                        break;
                    case "PVBDownTSL":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBDownTSL != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBDownTSL ascending select a).ToList();
                        }
                        break;
                    case "PVBDownTSLR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBDownTSLR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBDownTSLR ascending select a).ToList();
                        }
                        break;
                    case "PVBPNum":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBPNum != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBPNum ascending select a).ToList();
                        }
                        break;
                    case "PVBQltR":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBQltR != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBQltR ascending select a).ToList();
                        }
                        break;
                    case "PVBStatus":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBStatus != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBStatus ascending select a).ToList();
                        }
                        break;
                    case "PVBQa":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBQa != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBQa ascending select a).ToList();
                        }
                        break;
                    case "PVBQb":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBQb != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBQb ascending select a).ToList();
                        }
                        break;
                    case "PVBQc":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBQc != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBQc ascending select a).ToList();
                        }
                        break;
                    case "PVBStbTSLPV":
                        foreach (MPVBaseMultiMessageInClass item in ValueList)
                        {
                            if (item.valueClass.PVBStbTSLPV != "-")
                            {
                                orderList.Add(item);
                            }
                        }
                        if (orderList.Count > 0)
                        {
                            orderList = (from a in orderList orderby a.valueClass.PVBStbTSLPV ascending select a).ToList();
                        }
                        break;
                    default:
                        break;
                }
                #endregion

                if (orderList.Count > 0)
                {
                    foreach (MPVBaseMultiMessageInClass item in orderList)
                    {
                        orderStr += item.lIndex.ToString() + ";";
                    }
                    orderStr = orderStr.Substring(0, orderStr.Length - 1);
                }
                else
                {
                    orderStr = "-";
                }
                return orderStr;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static string OrderMDevLimit(List<MDevLimitMultiMessageInClass> ValueList, string type)
        {
            List<MDevLimitMultiMessageInClass> orderList = new List<MDevLimitMultiMessageInClass>();
            string orderStr = string.Empty;
            switch (type)
            {
                case "DevHHNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHN ascending select a).ToList();
                    }
                    break;
                case "DevHHTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHT ascending select a).ToList();
                    }
                    break;
                case "DevHHROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHR ascending select a).ToList();
                    }
                    break;
                case "DevHHTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHTMax ascending select a).ToList();
                    }
                    break;
                case "DevHHAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHA ascending select a).ToList();
                    }
                    break;
                case "DevHHETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHET ascending select a).ToList();
                    }
                    break;


                case "DevHNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHN ascending select a).ToList();
                    }
                    break;
                case "DevHTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHT ascending select a).ToList();
                    }
                    break;
                case "DevHROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHR ascending select a).ToList();
                    }
                    break;
                case "DevHTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHTMax ascending select a).ToList();
                    }
                    break;
                case "DevHAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHA ascending select a).ToList();
                    }
                    break;
                case "DevHETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHET ascending select a).ToList();
                    }
                    break;


                case "DevRPNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPN ascending select a).ToList();
                    }
                    break;
                case "DevRPTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPT ascending select a).ToList();
                    }
                    break;
                case "DevRPROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPR ascending select a).ToList();
                    }
                    break;
                case "DevRPTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPT ascending select a).ToList();
                    }
                    break;
                case "DevRPAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPA ascending select a).ToList();
                    }
                    break;
                case "DevRPETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPET ascending select a).ToList();
                    }
                    break;


                case "Dev0PNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0PN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0PN ascending select a).ToList();
                    }
                    break;
                case "Dev0PTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0PT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0PT ascending select a).ToList();
                    }
                    break;
                case "Dev0PROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0PR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0PR ascending select a).ToList();
                    }
                    break;
                case "Dev0PTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0PTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0PTMax ascending select a).ToList();
                    }
                    break;
                case "Dev0PAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0PA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0PA ascending select a).ToList();
                    }
                    break;
                case "Dev0PETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0PET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0PET ascending select a).ToList();
                    }
                    break;


                case "Dev0NNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0NN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0NN ascending select a).ToList();
                    }
                    break;
                case "Dev0NTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0NT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0NT ascending select a).ToList();
                    }
                    break;
                case "Dev0NROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0NR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0NR ascending select a).ToList();
                    }
                    break;
                case "Dev0NTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0NTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0NTMax ascending select a).ToList();
                    }
                    break;
                case "Dev0NAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0NA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0NA ascending select a).ToList();
                    }
                    break;
                case "Dev0NETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0NET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0NET ascending select a).ToList();
                    }
                    break;


                case "DevRNNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRNN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRNN ascending select a).ToList();
                    }
                    break;
                case "DevRNTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRNT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRNT ascending select a).ToList();
                    }
                    break;
                case "DevRNROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRNR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRNR ascending select a).ToList();
                    }
                    break;
                case "DevRNTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRNTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRNTMax ascending select a).ToList();
                    }
                    break;
                case "DevRNAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRNA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRNA ascending select a).ToList();
                    }
                    break;
                case "DevRNETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRNET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRNET ascending select a).ToList();
                    }
                    break;



                case "DevLNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLN ascending select a).ToList();
                    }
                    break;
                case "DevLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLT ascending select a).ToList();
                    }
                    break;
                case "DevLROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLR ascending select a).ToList();
                    }
                    break;
                case "DevLTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLTMax ascending select a).ToList();
                    }
                    break;
                case "DevLAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLA ascending select a).ToList();
                    }
                    break;
                case "DevLETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLET ascending select a).ToList();
                    }
                    break;



                case "DevLLNOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLLN != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLLN ascending select a).ToList();
                    }
                    break;
                case "DevLLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLLT ascending select a).ToList();
                    }
                    break;
                case "DevLLROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLLR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLLR ascending select a).ToList();
                    }
                    break;
                case "DevLLTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLLTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLLTMax ascending select a).ToList();
                    }
                    break;
                case "DevLLAOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLLA != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLLA ascending select a).ToList();
                    }
                    break;
                case "DevLLETOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevLLET != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevLLET ascending select a).ToList();
                    }
                    break;


                case "Dev0HTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0HT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0HT ascending select a).ToList();
                    }
                    break;
                case "Dev0HTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0HTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0HTR ascending select a).ToList();
                    }
                    break;
                case "Dev0HHTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0HHT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0HHT ascending select a).ToList();
                    }
                    break;
                case "Dev0HHTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0HHTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0HHTR ascending select a).ToList();
                    }
                    break;
                case "Dev0LOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0L != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0L ascending select a).ToList();
                    }
                    break;
                case "Dev0LROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0LR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0LR ascending select a).ToList();
                    }
                    break;
                case "Dev0LLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0LLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0LLT ascending select a).ToList();
                    }
                    break;
                case "Dev0LLTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0LLTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0LLTR ascending select a).ToList();
                    }
                    break;
                case "DevHHLLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHLLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHLLT ascending select a).ToList();
                    }
                    break;
                case "DevHHLLTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHHLLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHHLLT ascending select a).ToList();
                    }
                    break;
                case "DevHLHHLLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHLHHLLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHLHHLLT ascending select a).ToList();
                    }
                    break;
                case "DevHLHHLLROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHLHHLLR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHLHHLLR ascending select a).ToList();
                    }
                    break;
                case "DevRPRMHLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPRMHLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPRMHLT ascending select a).ToList();
                    }
                    break;
                case "DevRPRMHLTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevRPRMHLTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevRPRMHLTR ascending select a).ToList();
                    }
                    break;
                case "Dev0RPRMTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0RPRMT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0RPRMT ascending select a).ToList();
                    }
                    break;
                case "Dev0RPRMTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0RPRMTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0RPRMTR ascending select a).ToList();
                    }
                    break;
                case "Dev0RPRMTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.Dev0RPRMTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.Dev0RPRMTMax ascending select a).ToList();
                    }
                    break;
                case "DevHLTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHLT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHLT ascending select a).ToList();
                    }
                    break;
                case "DevHLTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHLTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHLTR ascending select a).ToList();
                    }
                    break;
                case "DevHLTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevHLTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevHLTMax ascending select a).ToList();
                    }
                    break;
                case "DevPTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevPT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevPT ascending select a).ToList();
                    }
                    break;
                case "DevPTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevPTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevPTR ascending select a).ToList();
                    }
                    break;
                case "DevPTRTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevPTRTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevPTRTMax ascending select a).ToList();
                    }
                    break;
                case "DevNTOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevNT != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevNT ascending select a).ToList();
                    }
                    break;
                case "DevNTROrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevNTR != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevNTR ascending select a).ToList();
                    }
                    break;
                case "DevNTRTMaxOrder":
                    foreach (MDevLimitMultiMessageInClass item in ValueList)
                    {
                        if (item.valueClass.DevNTRTMax != "-")
                        {
                            orderList.Add(item);
                        }
                    }
                    if (orderList.Count > 0)
                    {
                        orderList = (from a in orderList orderby a.valueClass.DevNTRTMax ascending select a).ToList();
                    }
                    break;
                default:
                    break;
            }
            if (orderList.Count > 0)
            {
                foreach (MDevLimitMultiMessageInClass item in orderList)
                {
                    orderStr += item.lIndex.ToString() + ";";
                }
                orderStr = orderStr.Substring(0, orderStr.Length - 1);
            }
            else
            {
                orderStr = "-";
            }
            return orderStr;
        }
    }
}
