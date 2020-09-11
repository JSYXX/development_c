using MathNet.Numerics;
using PCCommon.NewCaculateCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.NewCaculate
{
    public class MPVBaseCaculate
    {
        #region/// MPVBase长周期算法（天，月）
        /// </summary>
        /// <param name="valueList">每小时、每天的值,务必确保顺序正确</param>
        /// <param name="N1">单次极差 N1 限值</param>
        /// <param name="N2">单次极差 N2 限值，>N1</param>
        /// <param name="N3">单次极差 N3 限值，>N2</param>
        /// <param name="k">求和乘积系数</param>
        /// <param name="b">求和偏置系数</param>
        /// <param name="stbL">稳定限幅 xi ≤ StbL 为稳定</param>
        /// <param name="noStbL">不稳定限幅 xi ＞ NoStbL 为不稳定</param>
        /// <returns></returns>
        public static MPVBaseMessageOutClass longMPVBase(List<MPVBaseMessageOutClass> valueList, double N1, double N2, double N3, double k, double b, double stbL, double noStbL)
        {
            MPVBaseMessageOutClass returnClass = new MPVBaseMessageOutClass();
            if (valueList.Count > 0)
            {
                double minValue = valueList[0].PVBMin;
                double maxValue = valueList[0].PVBMax;
                string minPVBTime = valueList[0].PVBMinTime;
                string maxPVBTime = valueList[0].PVBMaxTime;
                double avgSumPVB = 0;
                double sumPVB = 0;
                double PVBSumkb = 0;
                double lineKSum = 0;
                double lineBSum = 0;
                double SumPNRPVB = 0;
                double AbsSumPVB = 0;
                double maxStdevPVB = valueList[0].PVBStdev;
                double maxPVBVolatility = 0;

                int sumPVBTNum = 0;
                double VMaxPVB = valueList[0].PVBVMax;
                double VMinPVB = valueList[0].PVBVMin;
                double sumPVBVAvg = 0;
                double sumPVBStbTR = 0;
                double sumPVBNoStbTR = 0;
                //double StbTSLPVB = valueList[0].PVBStbTSL;
                //double sumPVBStbTSLR = 0;
                //double NoStbTSLPVB = valueList[0].PVBNoStbTSL;
                //double sumPVBNoStbTSLR = 0;
                double UpTSLPVB = valueList[0].PVBUpTSL;
                double sumPVBUpTSLR = 0;
                double DownTSLPVB = valueList[0].PVBDownTSL;
                double sumPVBDownTSLR = 0;
                double sumPVBPNum = 0;
                double sumPVBQltR = 0;
                double sumPVBQa = 0;
                double sumPVBQb = 0;
                double sumPVBQc = 0;
                foreach (MPVBaseMessageOutClass item in valueList)
                {
                    sumPVBQa += item.PVBQa;
                    sumPVBQb += item.PVBQb;
                    sumPVBQc += item.PVBQc;
                    if (minValue > item.PVBMin)
                    {
                        minValue = item.PVBMin;
                        minPVBTime = item.PVBMinTime;
                    }
                    if (maxValue < item.PVBMax)
                    {
                        maxValue = item.PVBMax;
                        maxPVBTime = item.PVBMaxTime;
                    }
                    if (maxStdevPVB < item.PVBStdev)
                    {
                        maxStdevPVB = item.PVBStdev;
                    }

                    if (VMaxPVB < item.PVBVMax)
                    {
                        VMaxPVB = item.PVBVMax;
                    }
                    if (VMinPVB > item.PVBVMin)
                    {
                        VMinPVB = item.PVBVMin;
                    }
                    //if (StbTSLPVB < item.PVBStbTSL)
                    //{
                    //    StbTSLPVB = item.PVBStbTSL;
                    //}
                    //if (NoStbTSLPVB < item.PVBNoStbTSL)
                    //{
                    //    NoStbTSLPVB = item.PVBNoStbTSL;
                    //}
                    if (UpTSLPVB < item.PVBUpTSL)
                    {
                        UpTSLPVB = item.PVBUpTSL;
                    }
                    if (DownTSLPVB < item.PVBDownTSL)
                    {
                        DownTSLPVB = item.PVBDownTSL;
                    }
                    avgSumPVB += item.PVBAvg;
                    sumPVB += item.PVBSum;
                    PVBSumkb += item.PVBSumkb;
                    lineKSum += item.PVBLinek;
                    lineBSum += item.PVBLineb;
                    SumPNRPVB += item.PVBSumPNR;
                    AbsSumPVB += item.PVBAbsSum;
                    maxPVBVolatility += item.PVBVolatility;
                    sumPVBTNum += item.PVBTNum;
                    sumPVBVAvg += item.PVBVAvg;
                    sumPVBStbTR += item.PVBStbTR;
                    sumPVBNoStbTR += item.PVBNoStbTR;
                    //sumPVBStbTSLR += item.PVBStbTSLR;
                    //sumPVBNoStbTSLR += item.PVBNoStbTSLR;
                    sumPVBUpTSLR += item.PVBUpTSLR;
                    sumPVBDownTSLR += item.PVBDownTSLR;
                    sumPVBPNum += item.PVBPNum;
                    sumPVBQltR += item.PVBQltR;

                }

                List<D22STimeClass> PVBSDMaxList = new List<D22STimeClass>();
                PVBSDMaxList.AddRange(valueList[0].PVBSDMaxTimeG);
                List<D22STimeClass> PVBStbTList = new List<D22STimeClass>();
                PVBStbTList.AddRange(valueList[0].PVBStbT);
                List<D22STimeClass> PVBNoStbTList = new List<D22STimeClass>();
                PVBNoStbTList.AddRange(valueList[0].PVBNoStbT);
                for (int i = 1; i < valueList.Count(); i++)
                {
                    List<D22STimeClass> newSDlist = new List<D22STimeClass>();
                    if (PVBSDMaxList.Count != 0 && valueList[i].PVBSDMaxTimeG.Count != 0)
                    {
                        newSDlist = Helper.AlgorithmHelper.GetD22STimeListSD(PVBSDMaxList[PVBSDMaxList.Count() - 1], valueList[i].PVBSDMaxTimeG[0]);
                        PVBSDMaxList.RemoveAt(PVBSDMaxList.Count() - 1);
                    }
                    PVBSDMaxList.AddRange(newSDlist);
                    for (int l = 1; l < valueList[i].PVBSDMaxTimeG.Count(); l++)
                    {
                        PVBSDMaxList.Add(valueList[i].PVBSDMaxTimeG[l]);
                    }

                    List<D22STimeClass> newStblist = new List<D22STimeClass>();
                    if (PVBStbTList.Count != 0 && valueList[i].PVBStbT.Count != 0)
                    {
                        newStblist = Helper.AlgorithmHelper.GetD22STimeListStb(PVBStbTList[PVBStbTList.Count() - 1], valueList[i].PVBStbT[0], stbL, noStbL);
                        PVBStbTList.RemoveAt(PVBStbTList.Count() - 1);
                    }
                    PVBStbTList.AddRange(newStblist);
                    for (int l = 1; l < valueList[i].PVBStbT.Count(); l++)
                    {
                        PVBStbTList.Add(valueList[i].PVBStbT[l]);
                    }

                    List<D22STimeClass> newNoStblist = new List<D22STimeClass>();
                    if (PVBNoStbTList.Count != 0 && valueList[i].PVBNoStbT.Count != 0)
                    {
                        newNoStblist = Helper.AlgorithmHelper.GetD22STimeListStb(PVBNoStbTList[PVBNoStbTList.Count() - 1], valueList[i].PVBNoStbT[0], stbL, noStbL);
                        PVBNoStbTList.RemoveAt(PVBNoStbTList.Count() - 1);
                    }
                    PVBNoStbTList.AddRange(newNoStblist);
                    for (int l = 1; l < valueList[i].PVBNoStbT.Count(); l++)
                    {
                        PVBNoStbTList.Add(valueList[i].PVBNoStbT[l]);
                    }


                }

                double SDMaxPVB = PVBSDMaxList[0].valueAmount;
                int sumPVBDN1Num = 0;
                int sumPVBDN2Num = 0;
                int sumPVBDN3Num = 0;
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (SDMaxPVB < item.valueAmount)
                    {
                        SDMaxPVB = item.valueAmount;
                        returnClass.PVBSDMaxTime = item;
                    }
                    if (item.valueAmount > N1)
                    {
                        sumPVBDN1Num += 1;
                    }
                    if (item.valueAmount > N2)
                    {
                        sumPVBDN2Num += 1;
                    }
                    if (item.valueAmount > N3)
                    {
                        sumPVBDN3Num += 1;
                    }
                }

                returnClass.PVBMin = Math.Round(minValue, 3);
                returnClass.PVBMinTime = minPVBTime;
                returnClass.PVBAvg = Math.Round((avgSumPVB / (double)(valueList.Count())), 3);
                returnClass.PVBMax = Math.Round(maxValue, 3);
                returnClass.PVBMaxTime = maxPVBTime;
                returnClass.PVBDMax = Math.Round((maxValue - minValue), 3);
                returnClass.PVBSum = Math.Round(sumPVB, 3);
                returnClass.PVBSumkb = Math.Round(PVBSumkb, 3);
                returnClass.PVBLinek = Math.Round((lineKSum / (double)(valueList.Count())), 3);
                returnClass.PVBLineb = Math.Round((lineBSum / (double)(valueList.Count())), 3);
                returnClass.PVBQa = Math.Round((sumPVBQa / (double)(valueList.Count())), 3);
                returnClass.PVBQb = Math.Round((sumPVBQb / (double)(valueList.Count())), 3);
                returnClass.PVBQc = Math.Round((sumPVBQc / (double)(valueList.Count())), 3);
                returnClass.PVBSumPNR = Math.Round((SumPNRPVB / (double)(valueList.Count())), 3);
                returnClass.PVBAbsSum = Math.Round(AbsSumPVB, 3);
                returnClass.PVBStdev = Math.Round((maxStdevPVB * 0.75), 3);
                returnClass.PVBVolatility = Math.Round((maxPVBVolatility / (double)(valueList.Count())), 3);
                returnClass.PVBSDMaxTimeG = PVBSDMaxList;



                returnClass.PVBSDMax = Math.Round(SDMaxPVB, 3);
                returnClass.PVBSDMaxR = Math.Round((SDMaxPVB / (maxValue - minValue)), 5) * 100;
                returnClass.PVBDN1Num = sumPVBDN1Num;
                returnClass.PVBDN2Num = sumPVBDN2Num;
                returnClass.PVBDN3Num = sumPVBDN3Num;
                returnClass.PVBTNum = sumPVBTNum;
                returnClass.PVBVMax = Math.Round(VMaxPVB, 3);
                returnClass.PVBVMin = Math.Round(VMinPVB, 3);
                returnClass.PVBVAvg = Math.Round((sumPVBVAvg / (double)(valueList.Count())), 3);



                returnClass.PVBStbT = PVBStbTList;
                returnClass.PVBStbTR = Math.Round((sumPVBStbTR / (double)(valueList.Count())), 3);
                returnClass.PVBNoStbT = PVBNoStbTList;
                returnClass.PVBNoStbTR = Math.Round((sumPVBNoStbTR / (double)(valueList.Count())), 3);

                if (PVBStbTList.Count > 0)
                {
                    D22STimeClass PVBStbTSLT = new D22STimeClass();
                    double PVBStbTSL = PVBStbTList[0].valueCount;
                    double PVBStbTSLPV = PVBStbTList[0].valueAmount;
                    PVBStbTSLT = PVBStbTList[0];
                    foreach (D22STimeClass item in PVBStbTList)
                    {
                        if (PVBStbTSL < item.valueCount)
                        {
                            PVBStbTSL = item.valueCount;
                            PVBStbTSLPV = item.valueAmount;
                            PVBStbTSLT = item;
                        }
                    }
                    returnClass.PVBStbTSLT = PVBStbTSLT;
                    returnClass.PVBStbTSLPV = Math.Round(PVBStbTSLPV / PVBStbTSL, 3);
                    returnClass.PVBStbTSL = PVBStbTSL;
                }
                if (PVBNoStbTList.Count > 0)
                {
                    D22STimeClass PVBNoStbTSLT = new D22STimeClass();
                    double PVBNoStbTSL = 0;
                    PVBNoStbTSLT = PVBNoStbTList[0];
                    foreach (D22STimeClass item in PVBNoStbTList)
                    {
                        if (PVBNoStbTSL < item.valueCount)
                        {
                            PVBNoStbTSL = item.valueCount;
                            PVBNoStbTSLT = item;
                        }
                    }
                    returnClass.PVBNoStbTSLT = PVBNoStbTSLT;
                    returnClass.PVBNoStbTSL = PVBNoStbTSL;
                }
                //returnClass.PVBStbTSL = StbTSLPVB;
                //returnClass.PVBStbTSLR = Math.Round((sumPVBStbTSLR / (double)(valueList.Count())), 3);
                //returnClass.PVBNoStbTSL = NoStbTSLPVB;
                //returnClass.PVBNoStbTSLR = Math.Round((sumPVBNoStbTSLR / (double)(valueList.Count())), 3);

                D22STimeClass PVBUpTSLT = new D22STimeClass();
                D22STimeClass PVBDownTSLT = new D22STimeClass();
                double PVBUpTSL = 0;
                double PVBDownTSL = 0;
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (item.valueType.Equals("up"))
                    {
                        PVBUpTSL = item.valueCount;
                        break;
                    }
                }
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (item.valueType.Equals("down"))
                    {
                        PVBDownTSL = item.valueCount;
                        break;
                    }
                }
                foreach (D22STimeClass item in PVBSDMaxList)
                {
                    if (item.valueType.Equals("up"))
                    {
                        if (PVBUpTSL < item.valueCount)
                        {
                            PVBUpTSL = item.valueCount;
                            PVBUpTSLT = item;
                        }
                    }
                    else if (item.valueType.Equals("down"))
                    {
                        if (PVBDownTSL < item.valueCount)
                        {
                            PVBDownTSL = item.valueCount;
                            PVBDownTSLT = item;
                        }
                    }
                }
                returnClass.PVBUpTSLT = PVBUpTSLT;
                returnClass.PVBUpTSL = PVBUpTSL;
                returnClass.PVBDownTSLT = PVBDownTSLT;
                returnClass.PVBDownTSL = PVBDownTSL;

                //returnClass.PVBUpTSL = UpTSLPVB;
                //returnClass.PVBUpTSLR = Math.Round((sumPVBUpTSLR / (double)(valueList.Count())), 3);
                //returnClass.PVBDownTSL = DownTSLPVB;
                //returnClass.PVBDownTSLR = Math.Round((sumPVBDownTSLR / (double)(valueList.Count())), 3);
                returnClass.PVBPNum = sumPVBPNum;
                returnClass.PVBQltR = Math.Round((sumPVBQltR / (double)(valueList.Count())), 3);
            }
            return returnClass;
        }
        #endregion

        #region/// MPV短周期算法（小时）
        /// <summary>
        /// <param name="valueList">每分钟的值,务必确保顺序正确</param>
        /// <param name="N1">单次极差 N1 限值</param>
        /// <param name="N2">单次极差 N2 限值，>N1</param>
        /// <param name="N3">单次极差 N3 限值，>N2</param>
        /// <param name="k">求和乘积系数</param>
        /// <param name="b">求和偏置系数</param>
        /// <param name="stbL">稳定限幅 xi ≤ StbL 为稳定</param>
        /// <param name="noStbL">不稳定限幅 xi ＞ NoStbL 为不稳定</param>
        /// <returns></returns>
        public static MPVBaseMessageOutBadClass shortMPVBase(List<MPVBaseMessageInBadClass> valueList, double N1, double N2, double N3, double k, double b, double stbL, double noStbL)
        {
            try
            {
                if (N1 >= N2)
                {
                    new Exception("N1必须小于N2");
                }
                if (N2 >= N3)
                {
                    new Exception("N2必须小于N3");
                }
                MPVBaseMessageOutBadClass returnClass = new MPVBaseMessageOutBadClass();
                List<MPVBaseMessageInClass> effectValueList = new List<MPVBaseMessageInClass>();
                List<D22STimeClass> rangeList = new List<D22STimeClass>();
                string sDate = string.Empty;
                string preDate = string.Empty;
                double preValue = 0;
                double SDMaxValue = 0;
                double SDMaxAmount = 0;
                int SDStatus = 0;//0:初始点，坏点；1：上升点；2：下降点;3:延伸点
                int valueStatus = 0;//上一点状态 0：未判断；1：好点；2：坏点
                List<StartEndDateClass> effectDateRegion = new List<StartEndDateClass>();
                string eStartDate = string.Empty;
                for (int i = 0; i < valueList.Count; i++)
                {
                    #region 取有效值
                    if (!valueList[i].valueAmount.Equals("-"))
                    {
                        MPVBaseMessageInClass newValueClass = new MPVBaseMessageInClass();
                        newValueClass.seq = i + 1;
                        newValueClass.valueDate = valueList[i].valueDate;
                        newValueClass.valueAmount = Convert.ToDouble(valueList[i].valueAmount);
                        effectValueList.Add(newValueClass);
                    }
                    #endregion
                }
                if (effectValueList.Count() == 0)
                {

                }
                else
                {
                    for (int i = 0; i < valueList.Count; i++)
                    {
                        if (i == (valueList.Count - 1))
                        {

                        }
                        #region 取单次极差列表
                        if (SDStatus == 0)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {

                                if (valueStatus == 0 || valueStatus == 2)
                                {
                                    sDate = valueList[i].valueDate;
                                    preDate = valueList[i].valueDate;
                                    preValue = Convert.ToDouble(valueList[i].valueAmount);
                                    SDMaxAmount = 1;
                                    SDStatus = 0;
                                }
                                else
                                {
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) == Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        //sDate = valueList[i].valueDate;
                                        preDate = valueList[i].valueDate;
                                        preValue = Convert.ToDouble(valueList[i].valueAmount);
                                        SDMaxAmount = 1;
                                        SDStatus = 3;
                                    }
                                    else
                                    {
                                        preDate = valueList[i].valueDate;
                                        SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                        SDMaxAmount = 2;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            SDStatus = 1;
                                        }
                                        else if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            SDStatus = 2;
                                        }
                                    }
                                }

                            }
                            else
                            {
                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        else if (SDStatus == 1)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {
                                if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                {

                                    preDate = valueList[i].valueDate;
                                    SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount += 1;
                                    SDStatus = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = preDate;
                                        newClass.valueCount = SDMaxAmount;
                                        newClass.valueAmount = SDMaxValue;
                                        newClass.valueType = "up";
                                        rangeList.Add(newClass);
                                    }
                                }
                                else
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = SDMaxAmount;
                                    newClass.valueAmount = SDMaxValue;
                                    newClass.valueType = "up";
                                    rangeList.Add(newClass);

                                    sDate = preDate;
                                    preDate = valueList[i].valueDate;
                                    SDMaxValue = Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newLastClass = new D22STimeClass();
                                        newLastClass.startDate = sDate;
                                        newLastClass.endDate = preDate;
                                        newLastClass.valueCount = SDMaxAmount;
                                        newLastClass.valueAmount = SDMaxValue;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            newLastClass.valueType = "down";
                                        }
                                        else
                                        {
                                            newLastClass.valueType = "translation";
                                        }
                                        rangeList.Add(newLastClass);
                                    }
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        SDStatus = 2;
                                    }
                                    else
                                    {
                                        SDStatus = 3;
                                    }
                                }
                            }
                            else
                            {
                                D22STimeClass newClass = new D22STimeClass();
                                newClass.startDate = sDate;
                                newClass.endDate = preDate;
                                newClass.valueCount = SDMaxAmount;
                                newClass.valueAmount = SDMaxValue;
                                newClass.valueType = "up";
                                rangeList.Add(newClass);

                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        else if (SDStatus == 2)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {
                                if (Convert.ToDouble(valueList[i - 1].valueAmount) > Convert.ToDouble(valueList[i].valueAmount))
                                {

                                    preDate = valueList[i].valueDate;
                                    SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount += 1;
                                    SDStatus = 2;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = preDate;
                                        newClass.valueCount = SDMaxAmount;
                                        newClass.valueAmount = SDMaxValue;
                                        newClass.valueType = "down";
                                        rangeList.Add(newClass);
                                    }
                                }
                                else
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = SDMaxAmount;
                                    newClass.valueAmount = SDMaxValue;
                                    newClass.valueType = "down";
                                    rangeList.Add(newClass);

                                    sDate = preDate;
                                    preDate = valueList[i].valueDate;
                                    SDMaxValue = Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newLastClass = new D22STimeClass();
                                        newLastClass.startDate = sDate;
                                        newLastClass.endDate = preDate;
                                        newLastClass.valueCount = SDMaxAmount;
                                        newLastClass.valueAmount = SDMaxValue;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            newLastClass.valueType = "up";
                                        }
                                        else
                                        {
                                            newLastClass.valueType = "translation";
                                        }
                                        rangeList.Add(newLastClass);
                                    }
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        SDStatus = 1;
                                    }
                                    else
                                    {
                                        SDStatus = 3;
                                    }
                                }
                            }
                            else
                            {
                                D22STimeClass newClass = new D22STimeClass();
                                newClass.startDate = sDate;
                                newClass.endDate = preDate;
                                newClass.valueCount = SDMaxAmount;
                                newClass.valueAmount = SDMaxValue;
                                newClass.valueType = "down";
                                rangeList.Add(newClass);

                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        else if (SDStatus == 3)
                        {
                            if (!valueList[i].valueAmount.Equals("-"))
                            {
                                if (Convert.ToDouble(valueList[i - 1].valueAmount) == Convert.ToDouble(valueList[i].valueAmount))
                                {

                                    preDate = valueList[i].valueDate;
                                    SDMaxValue += Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount += 1;
                                    SDStatus = 3;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newClass = new D22STimeClass();
                                        newClass.startDate = sDate;
                                        newClass.endDate = preDate;
                                        newClass.valueCount = SDMaxAmount;
                                        newClass.valueAmount = SDMaxValue;
                                        newClass.valueType = "down";
                                        rangeList.Add(newClass);
                                    }
                                }
                                else
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = sDate;
                                    newClass.endDate = preDate;
                                    newClass.valueCount = SDMaxAmount;
                                    newClass.valueAmount = SDMaxValue;
                                    newClass.valueType = "translation";
                                    rangeList.Add(newClass);

                                    sDate = preDate;
                                    preDate = valueList[i].valueDate;
                                    SDMaxValue = Math.Abs(Convert.ToDouble(valueList[i].valueAmount) - Convert.ToDouble(valueList[i - 1].valueAmount));
                                    SDMaxAmount = 1;
                                    if (i == (valueList.Count - 1))
                                    {
                                        D22STimeClass newLastClass = new D22STimeClass();
                                        newLastClass.startDate = sDate;
                                        newLastClass.endDate = preDate;
                                        newLastClass.valueCount = SDMaxAmount;
                                        newLastClass.valueAmount = SDMaxValue;
                                        if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                        {
                                            newLastClass.valueType = "up";
                                        }
                                        else
                                        {
                                            newLastClass.valueType = "down";
                                        }
                                        rangeList.Add(newLastClass);
                                    }
                                    if (Convert.ToDouble(valueList[i - 1].valueAmount) < Convert.ToDouble(valueList[i].valueAmount))
                                    {
                                        SDStatus = 1;
                                    }
                                    else
                                    {
                                        SDStatus = 2;
                                    }
                                }
                            }
                            else
                            {
                                D22STimeClass newClass = new D22STimeClass();
                                newClass.startDate = sDate;
                                newClass.endDate = preDate;
                                newClass.valueCount = SDMaxAmount;
                                newClass.valueAmount = SDMaxValue;
                                newClass.valueType = "translation";
                                rangeList.Add(newClass);

                                sDate = string.Empty;
                                preDate = string.Empty;
                                SDMaxValue = 0;
                                SDMaxAmount = 0;
                                SDStatus = 0;
                            }
                        }
                        #endregion
                        #region 取有效时间段
                        if (valueList[i].valueAmount.Equals("-"))
                        {
                            if (!string.IsNullOrWhiteSpace(eStartDate))
                            {
                                StartEndDateClass newDateClass = new StartEndDateClass();
                                newDateClass.startDate = eStartDate;
                                newDateClass.endDate = valueList[i - 1].valueDate;
                                effectDateRegion.Add(newDateClass);
                            }
                            eStartDate = string.Empty;
                        }
                        else
                        {
                            if (valueStatus == 2 || valueStatus == 0)
                            {
                                eStartDate = valueList[i].valueDate;
                            }
                            if (i == (valueList.Count - 1))
                            {
                                StartEndDateClass newDateClass = new StartEndDateClass();
                                newDateClass.startDate = eStartDate;
                                newDateClass.endDate = valueList[i].valueDate;
                                effectDateRegion.Add(newDateClass);
                            }
                        }
                        #endregion
                        #region 为是否好坏点状态赋值
                        if (!valueList[i].valueAmount.Equals("-"))
                        {
                            valueStatus = 1;
                        }
                        else
                        {
                            valueStatus = 2;
                        }
                        #endregion
                    }
                    double minValue = effectValueList[0].valueAmount;
                    double maxValue = effectValueList[0].valueAmount;
                    string PVBMaxTime = effectValueList[0].valueDate;
                    string PVBMinTime = effectValueList[0].valueDate;
                    double sumValue = 0;//总和
                    double sumPositive = 0;//正数和
                    double sumNegative = 0;//负数和
                    double sumAbsolute = 0;//绝对值和
                    List<CurveClass> xyList = new List<CurveClass>();
                    double[] xList = new double[valueList.Count];
                    double[] yList = new double[valueList.Count];
                    int lIndex = 0;
                    #region 使用有效数据集合算出一些没有持续属性的返回值
                    foreach (MPVBaseMessageInClass item in effectValueList)
                    {
                        xList[lIndex] = item.seq;
                        yList[lIndex] = item.valueAmount;
                        sumValue += item.valueAmount;
                        sumAbsolute += Math.Abs(item.valueAmount);
                        if (item.valueAmount > 0)
                        {
                            sumPositive += item.valueAmount;
                        }
                        else if (item.valueAmount < 0)
                        {
                            sumNegative += item.valueAmount;
                        }
                        if (item.valueAmount > maxValue)
                        {
                            maxValue = item.valueAmount;
                            PVBMaxTime = item.valueDate;//最大值发生时刻
                        }
                        if (item.valueAmount < minValue)
                        {
                            minValue = item.valueAmount;
                            PVBMinTime = item.valueDate;//最小值发生时刻
                        }
                        lIndex++;
                        CurveClass xy = new CurveClass();
                        xy.x = item.seq;
                        xy.y = item.valueAmount;
                        xyList.Add(xy);
                    }
                    returnClass.PVBMin = Math.Round(minValue, 3).ToString();//最小值
                    returnClass.PVBMax = Math.Round(maxValue, 3).ToString();//最大值
                    returnClass.PVBMaxTime = PVBMaxTime;
                    returnClass.PVBMinTime = PVBMinTime;
                    returnClass.PVBAvg = Math.Round((sumValue / (valueList.Count() + 0.0)), 3).ToString();//平均值
                    returnClass.PVBDMax = Math.Round((maxValue - minValue), 3).ToString();//总极差
                    returnClass.PVBSum = Math.Round(sumValue, 3).ToString();//和
                    returnClass.PVBSumkb = Math.Round(sumValue * k + b, 3).ToString();//上面的和乘 k 加 b
                    double returnK = 0;
                    double returnB = 0;
                    LinearRegressionSolve(xyList, ref returnK, ref returnB);
                    returnClass.PVBLinek = returnK.ToString();//以直线拟合时的斜率 Regress_k
                    returnClass.PVBLineb = returnB.ToString();//以直线拟合时的截距 Regress_b
                    returnClass.PVBSumPNR = ((Math.Round(sumPositive / (sumNegative * -1), 5)) * 100).ToString();//正负比。正数和 / 绝对值(负数和) * 100%
                    returnClass.PVBAbsSum = Math.Round(sumAbsolute, 3).ToString();//绝对值和
                    returnClass.PVBStdev = StandardDeviationSolve(xyList).ToString();//标准差

                    #endregion
                    double sdMax = rangeList[0].valueAmount;
                    int n1Num = 0;
                    int n2Num = 0;
                    int n3Num = 0;

                    int PVBTNum = 0;
                    #region 用极差分段集合得出数据
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueAmount > sdMax)
                        {
                            sdMax = item.valueAmount;
                            returnClass.PVBSDMaxTime = item;//单次极差发生时刻
                        }
                        if (item.valueAmount > N1)
                        {
                            n1Num += 1;
                        }
                        if (item.valueAmount > N2)
                        {
                            n2Num += 1;
                        }
                        if (item.valueAmount > N3)
                        {
                            n3Num += 1;
                        }

                    }
                    #endregion
                    #region 用有效时间段退出反转次数以及有效数据分段集合
                    List<List<MPVBaseMessageInClass>> effectValueListList = new List<List<MPVBaseMessageInClass>>();
                    int effectIndex = 0;
                    foreach (var dateItem in effectDateRegion)
                    {
                        int TT = 0;
                        foreach (D22STimeClass item in rangeList)
                        {
                            if (Convert.ToDateTime(dateItem.startDate) <= Convert.ToDateTime(item.startDate) && Convert.ToDateTime(dateItem.endDate) >= Convert.ToDateTime(item.endDate))
                            {
                                TT += 1;
                            }
                        }
                        PVBTNum = PVBTNum + (TT == 0 ? 0 : TT - 1);
                        List<MPVBaseMessageInClass> newList = new List<MPVBaseMessageInClass>();
                        bool isIn = false;
                        for (; effectIndex < valueList.Count; effectIndex++)
                        {
                            if (Convert.ToDateTime(dateItem.startDate) <= Convert.ToDateTime(valueList[effectIndex].valueDate) && Convert.ToDateTime(dateItem.endDate) >= Convert.ToDateTime(Convert.ToDateTime(valueList[effectIndex].valueDate)))
                            {
                                isIn = true;
                                MPVBaseMessageInClass newValueClass = new MPVBaseMessageInClass();
                                newValueClass.seq = effectIndex + 1;
                                newValueClass.valueDate = valueList[effectIndex].valueDate;
                                newValueClass.valueAmount = Convert.ToDouble(valueList[effectIndex].valueAmount);
                                newList.Add(newValueClass);
                            }
                            else
                            {
                                if (isIn)
                                {
                                    break;
                                }
                            }
                        }
                        if (newList.Count > 0)
                        {
                            effectValueListList.Add(newList);
                        }
                    }
                    #endregion
                    returnClass.PVBSDMax = Math.Round(sdMax, 3).ToString();//单次极差。Max(|Δxi|)。极差：指 x 所经历的全部峰 - 谷差的绝对值

                    returnClass.PVBSDMaxR = ((Math.Round((Convert.ToDouble(returnClass.PVBSDMax) / (Convert.ToDouble(returnClass.PVBMax) - Convert.ToDouble(returnClass.PVBMin))), 5)) * 100).ToString();//单次极差比，绝对值。PVSDMax / PVBDMax x 100%
                    returnClass.PVBDN1Num = n1Num.ToString();//极差大于 N1 次数
                    returnClass.PVBDN2Num = n2Num.ToString();//极差大于 N2 次数
                    returnClass.PVBDN3Num = n3Num.ToString();//极差大于 N3 次数
                    returnClass.PVBTNum = PVBTNum.ToString();//翻转次数

                    List<double> vMaxList = new List<double>();
                    List<double> vMinList = new List<double>();
                    double vSum = 0;//初始化总绝对值速度
                    double vCount = 0;
                    double stbTR = 0;//初始化稳定时间
                    double noStbTR = 0;//初始化不稳定时间

                    List<D22STimeClass> PVBStbT = new List<D22STimeClass>();//初始化连续稳定时间列表
                    List<D22STimeClass> PVBNoStbT = new List<D22STimeClass>();//初始化连续不稳定时间列表
                    foreach (List<MPVBaseMessageInClass> item in effectValueListList)
                    {
                        if (item.Count() > 1)
                        {
                            double vMax = 0;//初始化最大速度
                            double vMin = 0;//初始化最小速度
                            vCount += (item.Count() - 1);

                            int isStbTr = 0;//0初始值，1稳定，2不稳定
                            string stbSDate = string.Empty;
                            double initialStbTrTime = 0;//初始化连续时间（稳定/不稳定）
                            double initialStbTr = 0;
                            double SValue = 0;
                            for (int i = 1; i < item.Count(); i++)
                            {
                                if (vMax < item[i].valueAmount - item[i - 1].valueAmount)
                                {
                                    vMax = item[i].valueAmount - item[i - 1].valueAmount;
                                }
                                if (vMin > item[i].valueAmount - item[i - 1].valueAmount)
                                {
                                    vMin = item[i].valueAmount - item[i - 1].valueAmount;
                                }
                                vSum += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);

                                if (Math.Abs(item[i].valueAmount - item[i - 1].valueAmount) <= stbL)
                                {
                                    //求稳定时间
                                    stbTR += 1;
                                    if (i == item.Count() - 1)
                                    {
                                        if (isStbTr == 1)
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = stbSDate;
                                            Stb.startValue = SValue;
                                            Stb.endDate = item[i].valueDate;
                                            Stb.endValue = item[i].valueAmount;
                                            Stb.valueAmount = initialStbTr;
                                            Stb.valueCount = initialStbTrTime;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);
                                        }
                                        else if (isStbTr == 2)
                                        {
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = stbSDate;
                                            noStb.startValue = SValue;
                                            noStb.endDate = item[i - 1].valueDate;
                                            noStb.endValue = item[i - 1].valueAmount;
                                            noStb.valueAmount = initialStbTr;
                                            noStb.valueCount = initialStbTrTime;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);

                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = item[i - 1].valueDate;
                                            Stb.startValue = item[i - 1].valueAmount;
                                            Stb.endDate = item[i].valueDate;
                                            Stb.endValue = item[i].valueAmount;
                                            Stb.valueAmount = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            Stb.valueCount = 1;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);
                                        }
                                    }
                                    else
                                    {
                                        if (isStbTr == 0)
                                        {
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else if (isStbTr == 1)
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else
                                        {
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = stbSDate;
                                            noStb.endDate = item[i - 1].valueDate;
                                            noStb.startValue = SValue;
                                            noStb.endValue = item[i - 1].valueAmount;
                                            noStb.valueAmount = initialStbTr;
                                            noStb.valueCount = initialStbTrTime;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);

                                            initialStbTrTime = 1;
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTr = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        isStbTr = 1;
                                    }


                                }
                                else if (Math.Abs(item[i].valueAmount - item[i - 1].valueAmount) > noStbL)
                                {
                                    //求不稳定时间
                                    noStbTR += 1;

                                    if (i == item.Count() - 1)
                                    {
                                        if (isStbTr == 1)
                                        {
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = item[i - 1].valueDate;
                                            noStb.startValue = item[i - 1].valueAmount;
                                            noStb.endDate = item[i].valueDate;
                                            noStb.endValue = item[i].valueAmount;
                                            noStb.valueAmount = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            noStb.valueCount = 1;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);

                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = stbSDate;
                                            Stb.startValue = SValue;
                                            Stb.endDate = item[i - 1].valueDate;
                                            Stb.endValue = item[i - 1].valueAmount;
                                            Stb.valueAmount = initialStbTr;
                                            Stb.valueCount = initialStbTrTime;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);
                                        }
                                        else if (isStbTr == 2)
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                            D22STimeClass noStb = new D22STimeClass();
                                            noStb.startDate = stbSDate;
                                            noStb.startValue = SValue;
                                            noStb.endDate = item[i].valueDate;
                                            noStb.endValue = item[i].valueAmount;
                                            noStb.valueAmount = initialStbTr;
                                            noStb.valueCount = initialStbTrTime;
                                            noStb.valueType = "noStb";
                                            PVBNoStbT.Add(noStb);
                                        }
                                    }
                                    else
                                    {
                                        if (isStbTr == 0)
                                        {
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else if (isStbTr == 1)
                                        {
                                            D22STimeClass Stb = new D22STimeClass();
                                            Stb.startDate = stbSDate;
                                            Stb.startValue = SValue;
                                            Stb.endDate = item[i - 1].valueDate;
                                            Stb.endValue = item[i - 1].valueAmount;
                                            Stb.valueAmount = initialStbTr;
                                            Stb.valueCount = initialStbTrTime;
                                            Stb.valueType = "Stb";
                                            PVBStbT.Add(Stb);

                                            initialStbTrTime = 1;
                                            stbSDate = item[i - 1].valueDate;
                                            SValue = item[i - 1].valueAmount;
                                            initialStbTr = Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        else
                                        {
                                            initialStbTrTime += 1;
                                            initialStbTr += Math.Abs(item[i].valueAmount - item[i - 1].valueAmount);
                                        }
                                        isStbTr = 2;
                                    }
                                }
                                else
                                {

                                    if (isStbTr == 1)
                                    {
                                        D22STimeClass Stb = new D22STimeClass();
                                        Stb.startDate = stbSDate;
                                        Stb.startValue = SValue;
                                        Stb.endDate = item[i - 1].valueDate;
                                        Stb.endValue = item[i - 1].valueAmount;
                                        Stb.valueAmount = initialStbTr;
                                        Stb.valueCount = initialStbTrTime;
                                        Stb.valueType = "Stb";
                                        PVBStbT.Add(Stb);
                                    }
                                    else if (isStbTr == 2)
                                    {
                                        D22STimeClass noStb = new D22STimeClass();
                                        noStb.startDate = stbSDate;
                                        noStb.startValue = SValue;
                                        noStb.endDate = item[i - 1].valueDate;
                                        noStb.endValue = item[i - 1].valueAmount;
                                        noStb.valueAmount = initialStbTr;
                                        noStb.valueCount = initialStbTrTime;
                                        noStb.valueType = "noStb";
                                        PVBNoStbT.Add(noStb);
                                    }
                                    stbSDate = item[i].valueDate;
                                    isStbTr = 0;
                                    SValue = 0;
                                    initialStbTrTime = 0;
                                    initialStbTr = 0;
                                }
                            }
                            vMaxList.Add(vMax);
                            vMinList.Add(vMin);


                        }

                    }

                    returnClass.PVBStbT = PVBStbT;
                    returnClass.PVBNoStbT = PVBNoStbT;



                    returnClass.PVBVMax = Math.Round(vMaxList.Max(), 3).ToString();//最大速度，max(Δxi)
                    returnClass.PVBVMin = Math.Round(vMinList.Min(), 3).ToString();//最小速度，min(Δxi)
                    returnClass.PVBVAvg = Math.Round((vSum / vCount), 3).ToString();//平均速度，Avg|Δxi|
                    returnClass.PVBVolatility = Math.Round((vSum / (double)(valueList.Count() - 1)), 3).ToString();//波动。|Δxi| = | xi+1 - xi | 求和，除以段数，即，点数 - 1
                    returnClass.PVBStbTR = ((Math.Round((stbTR / (double)(valueList.Count() - 1)), 5)) * 100).ToString();//稳定时间占比，稳定：|Δxi| ≤ StbL
                    returnClass.PVBNoStbTR = ((Math.Round((noStbTR / (double)(valueList.Count() - 1)), 5)) * 100).ToString();//不稳定时间占比，稳定：|Δxi| ＞ NoStbL

                    if (PVBStbT.Count > 0)
                    {
                        D22STimeClass PVBStbTSLT = new D22STimeClass();
                        PVBStbTSLT = PVBStbT[0];
                        double PVBStbTSL = PVBStbT[0].valueCount;
                        double PVBStbTSLPV = PVBStbT[0].valueAmount;
                        foreach (D22STimeClass item in PVBStbT)
                        {
                            if (PVBStbTSL < item.valueCount)
                            {
                                PVBStbTSL = item.valueCount;
                                PVBStbTSLPV = item.valueAmount;
                                PVBStbTSLT = item;
                            }
                        }
                        returnClass.PVBStbTSLT = PVBStbTSLT;
                        returnClass.PVBStbTSLPV = Math.Round(PVBStbTSLPV / PVBStbTSL, 3).ToString();
                        returnClass.PVBStbTSL = PVBStbTSL.ToString();
                    }
                    if (PVBNoStbT.Count > 0)
                    {
                        D22STimeClass PVBNoStbTSLT = new D22STimeClass();
                        double PVBNoStbTSL = 0;
                        foreach (D22STimeClass item in PVBNoStbT)
                        {
                            if (PVBNoStbTSL < item.valueCount)
                            {
                                PVBNoStbTSL = item.valueCount;
                                PVBNoStbTSLT = item;
                            }
                        }
                        returnClass.PVBNoStbTSLT = PVBNoStbTSLT;
                        returnClass.PVBNoStbTSL = PVBNoStbTSL.ToString();
                    }
                    D22STimeClass PVBUpTSLT = new D22STimeClass();
                    D22STimeClass PVBDownTSLT = new D22STimeClass();
                    double PVBUpTSL = 0;
                    double PVBDownTSL = 0;
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueType.Equals("up"))
                        {
                            PVBUpTSL = item.valueCount;
                            PVBUpTSLT = item;
                            break;
                        }
                    }
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueType.Equals("down"))
                        {
                            PVBDownTSL = item.valueCount;
                            PVBDownTSLT = item;
                            break;
                        }
                    }
                    foreach (D22STimeClass item in rangeList)
                    {
                        if (item.valueType.Equals("up"))
                        {
                            if (PVBUpTSL < item.valueCount)
                            {
                                PVBUpTSL = item.valueCount;
                                PVBUpTSLT = item;
                            }
                        }
                        else if (item.valueType.Equals("down"))
                        {
                            if (PVBDownTSL < item.valueCount)
                            {
                                PVBDownTSL = item.valueCount;
                                PVBDownTSLT = item;
                            }
                        }
                    }
                    returnClass.PVBUpTSLT = PVBUpTSLT;
                    returnClass.PVBUpTSL = PVBUpTSL.ToString();
                    returnClass.PVBDownTSLT = PVBDownTSLT;
                    returnClass.PVBDownTSL = PVBDownTSL.ToString();
                    returnClass.PVBPNum = effectValueList.Count().ToString();
                    returnClass.PVBQltR = ((Math.Round((effectValueList.Count() / (double)(valueList.Count())), 5)) * 100).ToString();

                    double[] res = Fit.Polynomial(xList, yList, 2);
                    returnClass.PVBQa = Math.Round(res[2], 3).ToString();
                    returnClass.PVBQb = Math.Round(res[1], 3).ToString();
                    returnClass.PVBQc = Math.Round(res[0], 3).ToString();

                    foreach (D22STimeClass item in rangeList)
                    {
                        foreach (MPVBaseMessageInClass effItem in effectValueList)
                        {
                            if (item.startDate == effItem.valueDate)
                            {
                                item.startValue = effItem.valueAmount;
                            }
                            if (item.endDate == effItem.valueDate)
                            {
                                item.endValue = effItem.valueAmount;
                            }
                        }
                    }
                    returnClass.PVBSDMaxTimeG = rangeList;
                }
                return returnClass;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region/// 一元线性回归 LinearRegressionSolve
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
        #endregion

        #region/// 标准差 StandardDeviationSolve
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
        #endregion
    }
}
