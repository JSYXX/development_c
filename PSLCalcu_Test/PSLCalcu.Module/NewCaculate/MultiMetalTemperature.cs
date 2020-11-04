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
        public List<MetalTemperatureClass> MultiMetalTemperatureCaculate(List<PValue>[] valueList, double LimitHH, double LimitH, double LimitRP, double LimitOO, double LimitRN, double LimitL, double LimitLL)
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
                    double HHG = 0;
                    double HHHB = 0;
                    double HRPB = 0;
                    double RP0B = 0;
                    double RM0B = 0;
                    double RMLB = 0;
                    double LLLB = 0;
                    double LLL = 0;
                    double RPRMB = 0;
                    double HLB = 0;
                    double HHHLLLB = 0;
                    double HHLLGL = 0;
                    double HG = 0;
                    double LL = 0;

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

                        if (childItem.y > LimitHH)
                        {
                            HHG++;
                        }
                        if (childItem.y > LimitH)
                        {
                            HG++;
                        }
                        if (childItem.y <= LimitHH && childItem.y > LimitH)
                        {
                            HHHB++;
                        }
                        if (childItem.y <= LimitH && childItem.y > LimitRP)
                        {
                            HRPB++;
                        }
                        if (childItem.y <= LimitRP && childItem.y > LimitOO)
                        {
                            RP0B++;
                        }
                        if (childItem.y <= LimitOO && childItem.y > LimitRN)
                        {
                            RM0B++;
                        }
                        if (childItem.y <= LimitRN && childItem.y > LimitL)
                        {
                            RMLB++;
                        }
                        if (childItem.y <= LimitL && childItem.y > LimitLL)
                        {
                            LLLB++;
                        }
                        if (childItem.y <= LimitL)
                        {
                            LL++;
                        }
                        if (childItem.y <= LimitLL)
                        {
                            LLL++;
                        }
                    }
                    RPRMB = RP0B + RM0B;
                    HLB = HRPB + RP0B + RM0B + RMLB;
                    HHHLLLB = HHHB + LLLB;
                    HHLLGL = HHG + LLL;
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
                    List<double> lY = new List<double>();
                    List<double> lX = new List<double>();
                    foreach (CurveClass lcItem in ccList)
                    {
                        lY.Add(lcItem.y);
                        lX.Add(lcItem.x);
                    }
                    List<double> lYTest = new List<double>();
                    foreach (double fdItem in lX)
                    {
                        lYTest.Add(lk * fdItem + lb);
                    }
                    double lR = GoodnessOfFit.RSquared(lY, lYTest);
                    newClass.lr = lR.ToString();
                    double[] res = Fit.Polynomial(xList, yList, 2);
                    newClass.qa = Math.Round(res[2], 3).ToString();
                    newClass.qb = Math.Round(res[1], 3).ToString();
                    newClass.qc = Math.Round(res[0], 3).ToString();
                    List<double> qYTest = new List<double>();
                    foreach (double qdItem in xList)
                    {
                        qYTest.Add(res[2] * Math.Pow(qdItem, 2) + res[1] * qdItem + res[0]);
                    }
                    double qR = GoodnessOfFit.RSquared(yList, qYTest);
                    newClass.qr = qR.ToString();
                    double Bulge = (ccList[1].y - ccList[0].y) * 2;
                    double BulgeN = 1;
                    double Cave = (ccList[1].y - ccList[0].y) * 2;
                    double CaveN = 1;
                    for (int i = 1; i < ccList.Count - 1; i++)
                    {
                        double b = 0;
                        if (i == ccList.Count - 1)
                        {
                            b = (ccList[i].y - ccList[i - 1].y) * 2;
                        }
                        else
                        {
                            b = ccList[i].y * 2 - ccList[i - 1].y - ccList[i + 1].y;
                        }
                        if (Bulge < b)
                        {
                            Bulge = b;
                            BulgeN = ccList[i].x;
                        }
                        if (Cave > b)
                        {
                            Cave = b;
                            CaveN = ccList[i].x;
                        }
                    }
                    newClass.Bulge = Bulge.ToString();
                    newClass.BulgeN = BulgeN.ToString();
                    newClass.Cave = Cave.ToString();
                    newClass.CaveN = CaveN.ToString();
                    newClass.HHG = Math.Round(HHG / (double)(ccList.Count), 3).ToString();
                    newClass.HHHB = Math.Round(HHHB / (double)(ccList.Count), 3).ToString();
                    newClass.HRPB = Math.Round(HRPB / (double)(ccList.Count), 3).ToString();
                    newClass.RP0B = Math.Round(RP0B / (double)(ccList.Count), 3).ToString();
                    newClass.RM0B = Math.Round(RM0B / (double)(ccList.Count), 3).ToString();
                    newClass.RMLB = Math.Round(RMLB / (double)(ccList.Count), 3).ToString();
                    newClass.LLLB = Math.Round(LLLB / (double)(ccList.Count), 3).ToString();
                    newClass.LLL = Math.Round(LLL / (double)(ccList.Count), 3).ToString();
                    newClass.RPRMB = Math.Round(RPRMB / (double)(ccList.Count), 3).ToString();
                    newClass.HLB = Math.Round(HLB / (double)(ccList.Count), 3).ToString();
                    newClass.HHHLLLB = Math.Round(HHHLLLB / (double)(ccList.Count), 3).ToString();
                    newClass.HHLLGL = Math.Round(HHLLGL / (double)(ccList.Count), 3).ToString();
                    newClass.HG = Math.Round(HG / (double)(ccList.Count), 3).ToString();
                    newClass.LL = Math.Round(LL / (double)(ccList.Count), 3).ToString();
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
