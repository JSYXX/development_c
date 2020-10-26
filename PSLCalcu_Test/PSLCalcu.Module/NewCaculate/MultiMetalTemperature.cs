using MathNet.Numerics;
using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.NewCaculate
{
    public class MultiMetalTemperature
    {
        public List<MetalTemperatureClass> MultiMetalTemperatureCaculate(List<PValue>[] valueList)
        {
            try
            {
                List<MetalTemperatureClass> returnClass = new List<MetalTemperatureClass>();
                foreach (List<PValue> item in valueList)
                {
                    MetalTemperatureClass newClass = new MetalTemperatureClass();
                    int l = 1;
                    List<CurveClass> ccList = new List<CurveClass>();
                    foreach (PValue childItem in item)
                    {
                        CurveClass cc = new CurveClass();
                        cc.x = l;
                        cc.y = childItem.Value;
                        ccList.Add(cc);
                        l++;
                    }
                    double Min = ccList[0].y;
                    double MinN = 1;
                    double Max = ccList[0].y;
                    double MaxN = 1;
                    double Sum = 0;
                    foreach (CurveClass childItem in ccList)
                    {
                        if (Min > childItem.y)
                        {
                            Min = childItem.y;
                            MinN = childItem.x;
                        }
                        if (Max < childItem.y)
                        {
                            Max = childItem.y;
                            MaxN = childItem.x;
                        }
                        Sum += childItem.y;
                    }
                    newClass.Min = Min.ToString();
                    newClass.MinN = MinN.ToString();
                    newClass.Max = Max.ToString();
                    newClass.MaxN = MaxN.ToString();
                    double Avg = Math.Round(Sum / (double)(ccList.Count), 3);
                    newClass.Avg = Avg.ToString();
                    double xi = Math.Abs(Avg - ccList[0].y);
                    double AvgN = 1;

                    double[] xList = new double[ccList.Count];
                    double[] yList = new double[ccList.Count];
                    int n = 0;
                    foreach (CurveClass childItem in ccList)
                    {
                        xList[n] = childItem.x;
                        yList[n] = childItem.y;
                        if (xi > Math.Abs(Avg - childItem.y))
                        {
                            xi = Math.Abs(Avg - childItem.y);
                            AvgN = childItem.x;
                        }
                        n++;
                    }
                    double dMaxB = ccList[1].y - ccList[0].y;
                    double dMaxBN = 1;
                    for (int i = 1; i < ccList.Count - 1; i++)
                    {
                        if (dMaxB < ccList[i].y - ccList[i - 1].y)
                        {
                            dMaxB = ccList[i].y - ccList[i - 1].y;
                            dMaxBN = ccList[i - 1].x;
                        }
                    }
                    newClass.AvgN = AvgN.ToString();
                    newClass.dX = (Max - Min).ToString();
                    newClass.dXNR = Math.Round((MaxN - MinN) / (double)(ccList.Count), 3).ToString();
                    newClass.dMaxB = AvgN.ToString();
                    newClass.dMaxBN = AvgN.ToString();
                    newClass.sigma = AlgorithmHelper.StandardDeviationSolve(ccList).ToString();
                    double lk = 0;
                    double lb = 0;
                    AlgorithmHelper.LinearRegressionSolve(ccList, ref lk, ref lb);
                    newClass.lk = lk.ToString();
                    newClass.lb = lb.ToString();
                    double[] res = Fit.Polynomial(xList, yList, 2);
                    newClass.qa = Math.Round(res[2], 3).ToString();
                    newClass.qb = Math.Round(res[1], 3).ToString();
                    newClass.qc = Math.Round(res[0], 3).ToString();
                    returnClass.Add(newClass);
                }
                return returnClass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
