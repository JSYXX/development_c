﻿using PCCommon.NewCaculateCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.NewCaculate
{
    public class EquBaseCaculate
    {
        public static EquBaseClass shortEquBase(List<MetalTemperatureClass> valueList)
        {
            try
            {
                EquBaseClass returnClass = new EquBaseClass();
                double equMin = Convert.ToDouble(valueList[0].Min);
                string equMinN = valueList[0].MinN;
                string equMinT = valueList[0].effectiveDateTime;
                double equMax = Convert.ToDouble(valueList[0].Max);
                string equMaxN = valueList[0].MaxN;
                string equMaxT = valueList[0].effectiveDateTime;
                double equAvgSum = 0;
                List<string> equAvgNList = new List<string>();
                double equBulge = Convert.ToDouble(valueList[0].Bulge);
                string equBulgeN = valueList[0].BulgeN;
                double equCave = Convert.ToDouble(valueList[0].Cave);
                string equCaveN = valueList[0].CaveN;
                double equHHG = 0;
                double equHG = 0;
                double equHHHB = 0;
                double equHRPB = 0;
                double equRP0B = 0;
                double equRM0B = 0;
                double equRMLB = 0;
                double equLLLB = 0;
                double equLL = 0;
                double equLLL = 0;
                double equRPRMB = 0;
                double equHLB = 0;
                double equHHHLLLB = 0;
                double equHHLLGL = 0;
                int equHHRMaxP = valueList[0].point;
                int equLLRMaxP = valueList[0].point;
                int equHHLLRMaxP = valueList[0].point;
                double equHHRMax = Convert.ToDouble(valueList[0].HHG);
                double equLLRMax = Convert.ToDouble(valueList[0].LLL);
                double equHHLLRMax = Convert.ToDouble(valueList[0].HHLLGL);
                int equSDMaxP= valueList[0].point;
                double equSDMax= Convert.ToDouble(valueList[0].dX);
                foreach (MetalTemperatureClass item in valueList)
                {
                    if (equMin > Convert.ToDouble(item.Min))
                    {
                        equMin = Convert.ToDouble(item.Min);
                        equMinN = item.MinN;
                        equMinT = item.effectiveDateTime;
                    }
                    if (equMax < Convert.ToDouble(item.Max))
                    {
                        equMax = Convert.ToDouble(item.Max);
                        equMaxN = item.MaxN;
                        equMaxT = item.effectiveDateTime;
                    }
                    equAvgSum += Convert.ToDouble(item.Avg);
                    equAvgNList.Add(item.AvgN);
                    if (equBulge < Convert.ToDouble(item.Bulge))
                    {
                        equBulge = Convert.ToDouble(item.Bulge);
                        equBulgeN = item.BulgeN;
                    }
                    if (equCave > Convert.ToDouble(item.Cave))
                    {
                        equCave = Convert.ToDouble(item.Cave);
                        equCaveN = item.CaveN;
                    }
                    equHHG += Convert.ToDouble(item.HHG);
                    equHG += Convert.ToDouble(item.HG);
                    equHHHB += Convert.ToDouble(item.HHHB);
                    equHRPB += Convert.ToDouble(item.HRPB);
                    equRP0B += Convert.ToDouble(item.RP0B);
                    equRM0B += Convert.ToDouble(item.RM0B);
                    equRMLB += Convert.ToDouble(item.RMLB);
                    equLLLB += Convert.ToDouble(item.LLLB);
                    equLL += Convert.ToDouble(item.LL);
                    equLLL += Convert.ToDouble(item.LLL);
                    equRPRMB += Convert.ToDouble(item.RPRMB);
                    equHLB += Convert.ToDouble(item.HLB);
                    equHHHLLLB += Convert.ToDouble(item.HHHLLLB);
                    equHHLLGL += Convert.ToDouble(item.HHLLGL);
                    if (equHHRMax < Convert.ToDouble(item.HHG))
                    {
                        equHHRMax = Convert.ToDouble(item.HHG);
                        equHHRMaxP = item.point;
                    }
                    if (equLLRMax < Convert.ToDouble(item.LLL))
                    {
                        equLLRMax = Convert.ToDouble(item.LLL);
                        equLLRMaxP = item.point;
                    }
                    if (equHHLLRMax < Convert.ToDouble(item.HHLLGL))
                    {
                        equHHLLRMax = Convert.ToDouble(item.HHLLGL);
                        equHHLLRMaxP = item.point;
                    }
                    if (equSDMax < Convert.ToDouble(item.dX))
                    {
                        equSDMax = Convert.ToDouble(item.dX);
                        equSDMaxP = item.point;
                    }

                }
                returnClass.equMin = equMin.ToString();
                returnClass.equMinP = equMinN;
                returnClass.equMinT = equMinT;
                returnClass.equMax = equMax.ToString();
                returnClass.equMaxP = equMaxN;
                returnClass.equMaxT = equMaxT;
                returnClass.equAvg = Math.Round(equAvgSum / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equAvgCP = equAvgNList.GroupBy(x => x).OrderBy(y => y.Count()).First().Key;
                returnClass.equAvgFP = equAvgNList.GroupBy(x => x).OrderByDescending(y => y.Count()).First().Key;
                returnClass.equdX = (equMax - equMin).ToString();
                returnClass.equSDMaxP = equSDMaxP.ToString();
                returnClass.equdXR = ((equMax - equMin) / (Math.Abs(Convert.ToDouble(equMaxN) - Convert.ToDouble(equMinN)) + 1)).ToString();
                returnClass.equBulge = equBulge.ToString();
                returnClass.equBulgeP = equBulgeN;
                returnClass.equCave = equCave.ToString();
                returnClass.equCaveP = equCaveN;
                returnClass.equHHG = Math.Round(equHHG / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHG = Math.Round(equHG / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHHB = Math.Round(equHHHB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHRPB = Math.Round(equHRPB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRP0B = Math.Round(equRP0B / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRM0B = Math.Round(equRM0B / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRMLB = Math.Round(equRMLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equLLLB = Math.Round(equLLLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equLL = Math.Round(equLL / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equLLL = Math.Round(equLLL / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRPRMB = Math.Round(equRPRMB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHLB = Math.Round(equHLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHHLLLB = Math.Round(equHHHLLLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHLLGL = Math.Round(equHHLLGL / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHRMaxP = equHHRMaxP.ToString();
                returnClass.equLLRMaxP = equLLRMaxP.ToString();
                returnClass.equHHLLRMaxP = equHHLLRMaxP.ToString();
                return returnClass;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static EquBaseClass longEquBase(List<EquBaseClass> valueList)
        {
            try
            {
                EquBaseClass returnClass = new EquBaseClass();
                double equMin = Convert.ToDouble(valueList[0].equMin);
                string equMinN = valueList[0].equMinP;
                string equMinT = valueList[0].equMinT;
                double equMax = Convert.ToDouble(valueList[0].equMax);
                string equMaxN = valueList[0].equMaxP;
                string equMaxT = valueList[0].equMaxT;
                double equAvgSum = 0;
                List<string> equAvgNList = new List<string>();
                double equBulge = Convert.ToDouble(valueList[0].equBulge);
                string equBulgeN = valueList[0].equBulgeP;
                double equCave = Convert.ToDouble(valueList[0].equCave);
                string equCaveN = valueList[0].equCaveP;
                double equHHG = 0;
                double equHG = 0;
                double equHHHB = 0;
                double equHRPB = 0;
                double equRP0B = 0;
                double equRM0B = 0;
                double equRMLB = 0;
                double equLLLB = 0;
                double equLL = 0;
                double equLLL = 0;
                double equRPRMB = 0;
                double equHLB = 0;
                double equHHHLLLB = 0;
                double equHHLLGL = 0;

                foreach (EquBaseClass item in valueList)
                {
                    if (equMin > Convert.ToDouble(item.equMin))
                    {
                        equMin = Convert.ToDouble(item.equMin);
                        equMinN = item.equMinP;
                        equMinT = item.equMinT;
                    }
                    if (equMax < Convert.ToDouble(item.equMax))
                    {
                        equMax = Convert.ToDouble(item.equMax);
                        equMaxN = item.equMaxP;
                        equMaxT = item.equMaxT;
                    }
                    equAvgSum += Convert.ToDouble(item.equAvg);
                    if (equBulge < Convert.ToDouble(item.equBulge))
                    {
                        equBulge = Convert.ToDouble(item.equBulge);
                        equBulgeN = item.equBulgeP;
                    }
                    if (equCave > Convert.ToDouble(item.equCave))
                    {
                        equCave = Convert.ToDouble(item.equCave);
                        equCaveN = item.equCaveP;
                    }
                    equHHG += Convert.ToDouble(item.equHHG);
                    equHG += Convert.ToDouble(item.equHG);
                    equHHHB += Convert.ToDouble(item.equHHHB);
                    equHRPB += Convert.ToDouble(item.equHRPB);
                    equRP0B += Convert.ToDouble(item.equRP0B);
                    equRM0B += Convert.ToDouble(item.equRM0B);
                    equRMLB += Convert.ToDouble(item.equRMLB);
                    equLLLB += Convert.ToDouble(item.equLLLB);
                    equLL += Convert.ToDouble(item.equLL);
                    equLLL += Convert.ToDouble(item.equLLL);
                    equRPRMB += Convert.ToDouble(item.equRPRMB);
                    equHLB += Convert.ToDouble(item.equHLB);
                    equHHHLLLB += Convert.ToDouble(item.equHHHLLLB);
                    equHHLLGL += Convert.ToDouble(item.equHHLLGL);

                }
                returnClass.equMin = equMin.ToString();
                returnClass.equMinP = equMinN;
                returnClass.equMinT = equMinT;
                returnClass.equMax = equMax.ToString();
                returnClass.equMaxP = equMaxN;
                returnClass.equMaxT = equMaxT;
                returnClass.equAvg = Math.Round(equAvgSum / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equAvgCP = valueList.GroupBy(x => x.equAvgCP).OrderBy(y => y.Count()).First().Key;
                returnClass.equAvgFP = valueList.GroupBy(x => x.equAvgFP).OrderBy(y => y.Count()).First().Key;
                returnClass.equdX = (equMax - equMin).ToString();
                returnClass.equSDMaxP = valueList.GroupBy(x => x.equSDMaxP).OrderBy(y => y.Count()).First().Key;
                returnClass.equBulge = equBulge.ToString();
                returnClass.equBulgeP = equBulgeN;
                returnClass.equCave = equCave.ToString();
                returnClass.equCaveP = equCaveN;
                returnClass.equHHG = Math.Round(equHHG / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHG = Math.Round(equHG / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHHB = Math.Round(equHHHB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHRPB = Math.Round(equHRPB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRP0B = Math.Round(equRP0B / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRM0B = Math.Round(equRM0B / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRMLB = Math.Round(equRMLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equLLLB = Math.Round(equLLLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equLL = Math.Round(equLL / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equLLL = Math.Round(equLLL / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equRPRMB = Math.Round(equRPRMB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHLB = Math.Round(equHLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHHLLLB = Math.Round(equHHHLLLB / Convert.ToDouble(valueList.Count), 3).ToString();
                returnClass.equHHLLGL = Math.Round(equHHLLGL / Convert.ToDouble(valueList.Count), 3).ToString();
                return returnClass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
