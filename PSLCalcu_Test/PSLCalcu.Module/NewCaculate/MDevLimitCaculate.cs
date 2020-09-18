using PCCommon;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PSLCalcu.Module.NewCaculate
{
    public class MDevLimitCaculate
    {
        /// <summary>
        /// MDevLimit短周期算法
        /// </summary>
        /// <param name="valueList">实际值</param>
        /// <param name="LimitHH">HH阈值</param>
        /// <param name="LimitH">H阈值</param>
        /// <param name="LimitRP">R+阈值</param>
        /// <param name="LimitRN">R-阈值</param>
        /// <param name="LimitL">L阈值</param>
        /// <param name="LimitLL">LL阈值</param>
        /// <returns></returns>
        public static MDevLimitMessageOutBadClass shortMDevLimit(List<MPVBaseMessageInBadClass> valueList, double LimitHH, double LimitH, double LimitRP, double LimitOO, double LimitRN, double LimitL, double LimitLL)
        {
            try
            {
                MDevLimitMessageOutBadClass returnClass = new MDevLimitMessageOutBadClass();
                List<MPVBaseMessageInClass> effectValueList = new List<MPVBaseMessageInClass>();
                bool isAllBad = true;
                List<List<MPVBaseMessageInClass>> effectValueListList = AlgorithmHelper.getEffectValueList(valueList, ref effectValueList, ref isAllBad);
                if (isAllBad)
                {

                }
                else
                {
                    List<D22STimeClass> DevHHList = new List<D22STimeClass>();
                    List<D22STimeClass> DevHList = new List<D22STimeClass>();
                    List<D22STimeClass> DevRPList = new List<D22STimeClass>();
                    List<D22STimeClass> Dev0PList = new List<D22STimeClass>();
                    List<D22STimeClass> Dev0NList = new List<D22STimeClass>();
                    List<D22STimeClass> DevRNList = new List<D22STimeClass>();
                    List<D22STimeClass> DevLList = new List<D22STimeClass>();
                    List<D22STimeClass> DevLLList = new List<D22STimeClass>();
                    List<D22STimeClass> Dev0RPRList = new List<D22STimeClass>();
                    List<D22STimeClass> DevHLList = new List<D22STimeClass>();
                    List<D22STimeClass> DevPTList = new List<D22STimeClass>();
                    List<D22STimeClass> DevNTList = new List<D22STimeClass>();
                    foreach (List<MPVBaseMessageInClass> item in effectValueListList)
                    {
                        #region 声明变量
                        #region HH
                        int HHstatus = 0;
                        string HHstartDate = string.Empty;
                        double HHsumAmount = 0;
                        double HHSumCount = 0;
                        #endregion
                        #region H
                        int Hstatus = 0;
                        string HstartDate = string.Empty;
                        double HsumAmount = 0;
                        double HSumCount = 0;
                        #endregion
                        #region RP
                        int RPstatus = 0;
                        string RPstartDate = string.Empty;
                        double RPsumAmount = 0;
                        double RPSumCount = 0;
                        #endregion
                        #region OP
                        int OPstatus = 0;
                        string OPstartDate = string.Empty;
                        double OPsumAmount = 0;
                        double OPSumCount = 0;
                        #endregion
                        #region ON
                        int ONstatus = 0;
                        string ONstartDate = string.Empty;
                        double ONsumAmount = 0;
                        double ONSumCount = 0;
                        #endregion
                        #region RN
                        int RNstatus = 0;
                        string RNstartDate = string.Empty;
                        double RNsumAmount = 0;
                        double RNSumCount = 0;
                        #endregion
                        #region L
                        int Lstatus = 0;
                        string LstartDate = string.Empty;
                        double LsumAmount = 0;
                        double LSumCount = 0;
                        #endregion
                        #region LL
                        int LLstatus = 0;
                        string LLstartDate = string.Empty;
                        double LLsumAmount = 0;
                        double LLSumCount = 0;
                        #endregion
                        #region EX
                        int EXstatus = 0;
                        string EXstartDate = string.Empty;
                        double EXSumCount = 0;
                        #endregion
                        #region NO
                        int NOstatus = 0;
                        string NOstartDate = string.Empty;
                        double NOSumCount = 0;
                        #endregion
                        #region PO
                        int POstatus = 0;
                        string POstartDate = string.Empty;
                        double POSumCount = 0;
                        #endregion
                        #region NE
                        int NEstatus = 0;
                        string NEstartDate = string.Empty;
                        double NESumCount = 0;
                        #endregion
                        #endregion
                        for (int i = 0; i < item.Count(); i++)
                        {
                            #region HH
                            if (item[i].valueAmount >= LimitHH)
                            {
                                if (HHstatus == 0)
                                {
                                    HHstartDate = item[i].valueDate;
                                }
                                HHsumAmount += item[i].valueAmount - LimitHH;
                                HHSumCount += 1;
                                HHstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = HHstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = HHsumAmount;
                                    newClass.valueCount = HHSumCount;
                                    newClass.valueType = "HH";
                                    DevHHList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (HHstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = HHstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = HHsumAmount;
                                    newClass.valueCount = HHSumCount;
                                    newClass.valueType = "HH";
                                    DevHHList.Add(newClass);
                                }
                                HHstatus = 0;
                                HHstartDate = string.Empty;
                                HHsumAmount = 0;
                                HHSumCount = 0;
                            }
                            #endregion
                            #region H
                            if (item[i].valueAmount >= LimitH && item[i].valueAmount < LimitHH)
                            {
                                if (Hstatus == 0)
                                {
                                    HstartDate = item[i].valueDate;
                                }
                                HsumAmount += item[i].valueAmount - LimitH;
                                HSumCount += 1;
                                Hstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = HstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = HsumAmount;
                                    newClass.valueCount = HSumCount;
                                    newClass.valueType = "H";
                                    DevHList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (Hstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = HstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = HsumAmount;
                                    newClass.valueCount = HSumCount;
                                    newClass.valueType = "H";
                                    DevHList.Add(newClass);
                                }
                                Hstatus = 0;
                                HstartDate = string.Empty;
                                HsumAmount = 0;
                                HSumCount = 0;
                            }
                            #endregion
                            #region RP
                            if (item[i].valueAmount >= LimitRP && item[i].valueAmount < LimitH)
                            {
                                if (RPstatus == 0)
                                {
                                    RPstartDate = item[i].valueDate;
                                }
                                RPsumAmount += item[i].valueAmount - LimitRP;
                                RPSumCount += 1;
                                RPstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = RPstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = RPsumAmount;
                                    newClass.valueCount = RPSumCount;
                                    newClass.valueType = "RP";
                                    DevRPList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (RPstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = RPstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = RPsumAmount;
                                    newClass.valueCount = RPSumCount;
                                    newClass.valueType = "RP";
                                    DevRPList.Add(newClass);
                                }
                                RPstatus = 0;
                                RPstartDate = string.Empty;
                                RPsumAmount = 0;
                                RPSumCount = 0;
                            }
                            #endregion
                            #region OP
                            if (item[i].valueAmount >= LimitOO && item[i].valueAmount < LimitRP)
                            {
                                if (OPstatus == 0)
                                {
                                    OPstartDate = item[i].valueDate;
                                }
                                OPsumAmount += item[i].valueAmount;
                                OPSumCount += 1;
                                OPstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = OPstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = OPsumAmount;
                                    newClass.valueCount = OPSumCount;
                                    newClass.valueType = "0P";
                                    Dev0PList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (OPstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = OPstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = OPsumAmount;
                                    newClass.valueCount = OPSumCount;
                                    newClass.valueType = "0P";
                                    Dev0PList.Add(newClass);
                                }
                                OPstatus = 0;
                                OPstartDate = string.Empty;
                                OPsumAmount = 0;
                                OPSumCount = 0;
                            }
                            #endregion
                            #region ON
                            if (item[i].valueAmount > LimitRN && item[i].valueAmount < LimitOO)
                            {
                                if (ONstatus == 0)
                                {
                                    ONstartDate = item[i].valueDate;
                                }
                                ONsumAmount += item[i].valueAmount * -1;
                                ONSumCount += 1;
                                ONstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = ONstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = ONsumAmount;
                                    newClass.valueCount = ONSumCount;
                                    newClass.valueType = "0N";
                                    Dev0NList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (ONstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = ONstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = ONsumAmount;
                                    newClass.valueCount = ONSumCount;
                                    newClass.valueType = "0N";
                                    Dev0NList.Add(newClass);
                                }
                                ONstatus = 0;
                                ONstartDate = string.Empty;
                                ONsumAmount = 0;
                                ONSumCount = 0;
                            }
                            #endregion
                            #region RN
                            if (item[i].valueAmount > LimitL && item[i].valueAmount <= LimitRN)
                            {
                                if (RNstatus == 0)
                                {
                                    RNstartDate = item[i].valueDate;
                                }
                                RNsumAmount += (item[i].valueAmount - LimitRN) * -1;
                                RNSumCount += 1;
                                RNstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = RNstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = RNsumAmount;
                                    newClass.valueCount = RNSumCount;
                                    newClass.valueType = "RN";
                                    DevRNList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (RNstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = RNstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = RNsumAmount;
                                    newClass.valueCount = RNSumCount;
                                    newClass.valueType = "RN";
                                    DevRNList.Add(newClass);
                                }
                                RNstatus = 0;
                                RNstartDate = string.Empty;
                                RNsumAmount = 0;
                                RNSumCount = 0;
                            }
                            #endregion
                            #region L
                            if (item[i].valueAmount > LimitLL && item[i].valueAmount <= LimitL)
                            {
                                if (Lstatus == 0)
                                {
                                    LstartDate = item[i].valueDate;
                                }
                                LsumAmount += (item[i].valueAmount - LimitL) * -1;
                                LSumCount += 1;
                                Lstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = LstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = LsumAmount;
                                    newClass.valueCount = LSumCount;
                                    newClass.valueType = "L";
                                    DevLList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (Lstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = LstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = LsumAmount;
                                    newClass.valueCount = LSumCount;
                                    newClass.valueType = "L";
                                    DevLList.Add(newClass);
                                }
                                Lstatus = 0;
                                LstartDate = string.Empty;
                                LsumAmount = 0;
                                LSumCount = 0;
                            }
                            #endregion
                            #region LL
                            if (item[i].valueAmount <= LimitLL)
                            {
                                if (LLstatus == 0)
                                {
                                    LLstartDate = item[i].valueDate;
                                }
                                LLsumAmount += (item[i].valueAmount - LimitLL) * -1;
                                LLSumCount += 1;
                                LLstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = LLstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueAmount = LLsumAmount;
                                    newClass.valueCount = LLSumCount;
                                    newClass.valueType = "LL";
                                    DevLLList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (LLstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = LLstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueAmount = LLsumAmount;
                                    newClass.valueCount = LLSumCount;
                                    newClass.valueType = "LL";
                                    DevLLList.Add(newClass);
                                }
                                LLstatus = 0;
                                LLstartDate = string.Empty;
                                LLsumAmount = 0;
                                LLSumCount = 0;
                            }
                            #endregion

                            #region EX
                            if (item[i].valueAmount > LimitRN && item[i].valueAmount < LimitRP)
                            {
                                if (EXstatus == 0)
                                {
                                    EXstartDate = item[i].valueDate;
                                }
                                EXSumCount += 1;
                                EXstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = EXstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueCount = EXSumCount;
                                    newClass.valueType = "0RPR";
                                    Dev0RPRList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (EXstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = EXstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueCount = EXSumCount;
                                    newClass.valueType = "0RPR";
                                    Dev0RPRList.Add(newClass);
                                }
                                EXstatus = 0;
                                EXstartDate = string.Empty;
                                EXSumCount = 0;
                            }
                            #endregion

                            #region NO
                            if (item[i].valueAmount > LimitL && item[i].valueAmount < LimitH)
                            {
                                if (NOstatus == 0)
                                {
                                    NOstartDate = item[i].valueDate;
                                }
                                NOSumCount += 1;
                                NOstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = NOstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueCount = NOSumCount;
                                    newClass.valueType = "HL";
                                    DevHLList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (NOstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = NOstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueCount = NOSumCount;
                                    newClass.valueType = "HL";
                                    DevHLList.Add(newClass);
                                }
                                NOstatus = 0;
                                NOstartDate = string.Empty;
                                NOSumCount = 0;
                            }
                            #endregion

                            #region PO
                            if (item[i].valueAmount >= LimitOO)
                            {
                                if (POstatus == 0)
                                {
                                    POstartDate = item[i].valueDate;
                                }
                                POSumCount += 1;
                                POstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = POstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueCount = POSumCount;
                                    newClass.valueType = "PT";
                                    DevPTList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (POstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = POstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueCount = POSumCount;
                                    newClass.valueType = "PT";
                                    DevPTList.Add(newClass);
                                }
                                POstatus = 0;
                                POstartDate = string.Empty;
                                POSumCount = 0;
                            }
                            #endregion
                            #region NE
                            if (item[i].valueAmount < LimitOO)
                            {
                                if (NEstatus == 0)
                                {
                                    NEstartDate = item[i].valueDate;
                                }
                                NESumCount += 1;
                                NEstatus = 1;
                                if (i == item.Count() - 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = NEstartDate;
                                    newClass.endDate = item[i].valueDate;
                                    newClass.valueCount = NESumCount;
                                    newClass.valueType = "NT";
                                    DevNTList.Add(newClass);
                                }
                            }
                            else
                            {
                                if (NEstatus == 1)
                                {
                                    D22STimeClass newClass = new D22STimeClass();
                                    newClass.startDate = NEstartDate;
                                    newClass.endDate = item[i - 1].valueDate;
                                    newClass.valueCount = NESumCount;
                                    newClass.valueType = "NT";
                                    DevNTList.Add(newClass);
                                }
                                NEstatus = 0;
                                NEstartDate = string.Empty;
                                NESumCount = 0;
                            }
                            #endregion
                        }
                    }

                    MDevLimitClass HHLimit = AlgorithmHelper.GetLimit(DevHHList, effectValueList.Count);
                    returnClass.DevHHN = HHLimit.DevN;      //越 HH 次数，x ≥ HH
                    returnClass.DevHHTime = HHLimit.DevTime;//越 HH 时刻
                    returnClass.DevHHT = HHLimit.DevT;      //越 HH 总时长，n 周期
                    returnClass.DevHHR = HHLimit.DevR;      //越 HH 时间占比，%
                    returnClass.DevHHTMax = HHLimit.DevTMax;
                    returnClass.DevHHTMaxTime = HHLimit.DevTMaxTime;
                    returnClass.DevHHA = HHLimit.DevA;
                    returnClass.DevHHET = HHLimit.DevET;
                    MDevLimitClass HLimit = AlgorithmHelper.GetLimit(DevHList, effectValueList.Count);
                    returnClass.DevHN = HLimit.DevN;        //越 H 次数，x ≥ HH
                    returnClass.DevHTime = HLimit.DevTime;  //越 H 时刻
                    returnClass.DevHT = HLimit.DevT;        //越 H 总时长，n 周期
                    returnClass.DevHR = HLimit.DevR;        //越 H 时间占比，%
                    returnClass.DevHTMax = HLimit.DevTMax;
                    returnClass.DevHTMaxTime = HLimit.DevTMaxTime;
                    returnClass.DevHA = HLimit.DevA;
                    returnClass.DevHET = HLimit.DevET;
                    MDevLimitClass RPLimit = AlgorithmHelper.GetLimit(DevRPList, effectValueList.Count);
                    returnClass.DevRPN = RPLimit.DevN;      //越 R+ 次数，x ≥ H
                    returnClass.DevRPTime = RPLimit.DevTime;//越 R+ 时刻
                    returnClass.DevRPT = RPLimit.DevT;      //越 R+ 总时长，n 周期
                    returnClass.DevRPR = RPLimit.DevR;      //越 R+ 时间占比，%
                    returnClass.DevRPTMax = RPLimit.DevTMax;
                    returnClass.DevRPTMaxTime = RPLimit.DevTMaxTime;
                    returnClass.DevRPA = RPLimit.DevA;
                    returnClass.DevRPET = RPLimit.DevET;
                    MDevLimitClass OPLimit = AlgorithmHelper.GetLimit(Dev0PList, effectValueList.Count);
                    returnClass.Dev0PN = OPLimit.DevN;      //越 OP 次数，x ≥ HH
                    returnClass.Dev0PTime = OPLimit.DevTime;//越 OP 时刻
                    returnClass.Dev0PT = OPLimit.DevT;      //越 OP 总时长，n 周期
                    returnClass.Dev0PR = OPLimit.DevR;      //越 OP 时间占比，%
                    returnClass.Dev0PTMax = OPLimit.DevTMax;
                    returnClass.Dev0PTMaxTime = OPLimit.DevTMaxTime;
                    returnClass.Dev0PA = OPLimit.DevA;
                    returnClass.Dev0PET = OPLimit.DevET;
                    MDevLimitClass ONLimit = AlgorithmHelper.GetLimit(Dev0NList, effectValueList.Count);
                    returnClass.Dev0NN = ONLimit.DevN;      //越 ON 次数，x ≥ HH
                    returnClass.Dev0NTime = ONLimit.DevTime;//越 ON 时刻
                    returnClass.Dev0NT = ONLimit.DevT;      //越 ON 总时长，n 周期
                    returnClass.Dev0NR = ONLimit.DevR;      //越 ON 时间占比，%
                    returnClass.Dev0NTMax = ONLimit.DevTMax;
                    returnClass.Dev0NTMaxTime = ONLimit.DevTMaxTime;
                    returnClass.Dev0NA = ONLimit.DevA;
                    returnClass.Dev0NET = ONLimit.DevET;
                    MDevLimitClass RNLimit = AlgorithmHelper.GetLimit(DevRNList, effectValueList.Count);
                    returnClass.DevRNN = RNLimit.DevN;      //越 R- 次数，x ≥ HH
                    returnClass.DevRNTime = RNLimit.DevTime;//越 R- 时刻
                    returnClass.DevRNT = RNLimit.DevT;      //越 R- 总时长，n 周期
                    returnClass.DevRNR = RNLimit.DevR;      //越 R- 时间占比，%
                    returnClass.DevRNTMax = RNLimit.DevTMax;
                    returnClass.DevRNTMaxTime = RNLimit.DevTMaxTime;
                    returnClass.DevRNA = RNLimit.DevA;
                    returnClass.DevRNET = RNLimit.DevET;
                    MDevLimitClass LNLimit = AlgorithmHelper.GetLimit(DevLList, effectValueList.Count);
                    returnClass.DevLN = LNLimit.DevN;        //越 L 次数，x ≥ 
                    returnClass.DevLTime = LNLimit.DevTime;  //越 L 时刻
                    returnClass.DevLT = LNLimit.DevT;        //越 L 总时长，n 周
                    returnClass.DevLR = LNLimit.DevR;        //越 L 时间占比，%
                    returnClass.DevLTMax = LNLimit.DevTMax;
                    returnClass.DevLTMaxTime = LNLimit.DevTMaxTime;
                    returnClass.DevLA = LNLimit.DevA;
                    returnClass.DevLET = LNLimit.DevET;
                    MDevLimitClass LLNLimit = AlgorithmHelper.GetLimit(DevLLList, effectValueList.Count);
                    returnClass.DevLLN = LLNLimit.DevN;      //越 LL 次数，x ≥ 
                    returnClass.DevLLTime = LLNLimit.DevTime;//越 LL 时刻
                    returnClass.DevLLT = LLNLimit.DevT;      //越 LL 总时长，n 周
                    returnClass.DevLLR = LLNLimit.DevR;      //越 LL 时间占比，%
                    returnClass.DevLLTMax = LLNLimit.DevTMax;
                    returnClass.DevLLTMaxTime = LLNLimit.DevTMaxTime;
                    returnClass.DevLLA = LLNLimit.DevA;
                    returnClass.DevLLET = LLNLimit.DevET;


                    returnClass.Dev0HT = (Convert.ToDouble(returnClass.DevRPT) + Convert.ToDouble(returnClass.Dev0PT)).ToString();
                    returnClass.Dev0HTR = (Math.Round(Convert.ToDouble(returnClass.Dev0HT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.Dev0HHT = (Convert.ToDouble(returnClass.DevHT) + Convert.ToDouble(returnClass.DevRPT) + Convert.ToDouble(returnClass.Dev0PT)).ToString();
                    returnClass.Dev0HHTR = (Math.Round(Convert.ToDouble(returnClass.Dev0HHT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.Dev0L = (Convert.ToDouble(returnClass.Dev0NT) + Convert.ToDouble(returnClass.DevRNT)).ToString();
                    returnClass.Dev0LR = (Math.Round(Convert.ToDouble(returnClass.Dev0L) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.Dev0LLT = (Convert.ToDouble(returnClass.DevLT) + Convert.ToDouble(returnClass.Dev0NT) + Convert.ToDouble(returnClass.DevRNT)).ToString();
                    returnClass.Dev0LLTR = (Math.Round(Convert.ToDouble(returnClass.Dev0LLT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.DevHHLLT = (Convert.ToDouble(returnClass.DevHHT) + Convert.ToDouble(returnClass.DevLLT)).ToString();
                    returnClass.DevHHLLTR = (Math.Round(Convert.ToDouble(returnClass.DevHHLLT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.DevHLHHLLT = (Convert.ToDouble(returnClass.DevHT) + Convert.ToDouble(returnClass.DevLT)).ToString();
                    returnClass.DevHLHHLLR = (Math.Round(Convert.ToDouble(returnClass.DevHLHHLLT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.DevRPRMHLT = (Convert.ToDouble(returnClass.DevRPT) + Convert.ToDouble(returnClass.DevRNT)).ToString();
                    returnClass.DevRPRMHLTR = (Math.Round(Convert.ToDouble(returnClass.DevRPRMHLT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.Dev0RPRMT = (Convert.ToDouble(returnClass.Dev0PT) + Convert.ToDouble(returnClass.Dev0NT)).ToString();
                    returnClass.Dev0RPRMTR = (Math.Round(Convert.ToDouble(returnClass.Dev0RPRMT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.DevHLT = (Convert.ToDouble(returnClass.DevRPT) + Convert.ToDouble(returnClass.Dev0PT) + Convert.ToDouble(returnClass.Dev0NT) + Convert.ToDouble(returnClass.DevRNT)).ToString();
                    returnClass.DevHLTR = (Math.Round(Convert.ToDouble(returnClass.DevHLT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.DevPT = (Convert.ToDouble(returnClass.DevHHT) + Convert.ToDouble(returnClass.DevHT) + Convert.ToDouble(returnClass.DevRPT) + Convert.ToDouble(returnClass.Dev0PT)).ToString();
                    returnClass.DevPTR = (Math.Round(Convert.ToDouble(returnClass.DevPT) / (double)valueList.Count(), 5) * 100).ToString();

                    returnClass.DevNT = (Convert.ToDouble(returnClass.DevLT) + Convert.ToDouble(returnClass.Dev0NT) + Convert.ToDouble(returnClass.DevRNT) + Convert.ToDouble(returnClass.DevLLT)).ToString();
                    returnClass.DevNTR = (Math.Round(Convert.ToDouble(returnClass.DevNT) / (double)valueList.Count(), 5) * 100).ToString();


                    MDevLimitClass ORPRLimit = AlgorithmHelper.GetLimit1(Dev0RPRList);
                    returnClass.Dev0RPRMTTime = ORPRLimit.DevTime;
                    returnClass.Dev0RPRMTMax = ORPRLimit.DevTMax;
                    returnClass.Dev0RPRMTMaxTime = ORPRLimit.DevTMaxTime;



                    MDevLimitClass HLLimit = AlgorithmHelper.GetLimit1(DevHLList);
                    returnClass.DevHLTTime = HLLimit.DevTime;
                    returnClass.DevHLTMax = HLLimit.DevTMax;
                    returnClass.DevHLTMaxTime = HLLimit.DevTMaxTime;



                    MDevLimitClass PLimit = AlgorithmHelper.GetLimit1(DevPTList);
                    returnClass.DevPTTime = PLimit.DevTime;
                    returnClass.DevPTRTMax = PLimit.DevTMax;
                    returnClass.DevPTRTMaxTime = PLimit.DevTMaxTime;



                    MDevLimitClass NLimit = AlgorithmHelper.GetLimit1(DevNTList);
                    returnClass.DevNTTime = NLimit.DevTime;
                    returnClass.DevNTRTMax = NLimit.DevTMax;
                    returnClass.DevNTRTMaxTime = NLimit.DevTMaxTime;
                }
                return returnClass;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static MDevLimitMessageOutClass longMDevLimit(List<MDevLimitMessageOutClass> valueList, double LimitHH, double LimitH, double LimitRP, double LimitOO, double LimitRN, double LimitL, double LimitLL)
        {
            MDevLimitMessageOutClass returnClass = new MDevLimitMessageOutClass();
            #region 声明参数
            double DevHHN = 0;
            double DevHHT = 0;
            double DevHHR = 0;
            double DevHHA = 0;
            double DevHN = 0;
            double DevHT = 0;
            double DevHR = 0;
            double DevHA = 0;
            double DevHET = 0;
            double DevRPN = 0;
            double DevRPT = 0;
            double DevRPR = 0;
            double DevRPA = 0;
            double Dev0PN = 0;
            double Dev0PT = 0;
            double Dev0PR = 0;
            double Dev0PA = 0;
            double Dev0NN = 0;
            double Dev0NT = 0;
            double Dev0NR = 0;
            double Dev0NA = 0;
            double DevRNN = 0;
            double DevRNT = 0;
            double DevRNR = 0;
            double DevRNA = 0;
            double DevLN = 0;
            double DevLT = 0;
            double DevLR = 0;
            double DevLA = 0;
            double DevLLN = 0;
            double DevLLT = 0;
            double DevLLR = 0;
            double DevLLA = 0;
            double Dev0HT = 0;
            double Dev0HTR = 0;
            double Dev0HHT = 0;
            double Dev0HHTR = 0;
            double Dev0L = 0;
            double Dev0LR = 0;
            double Dev0LLT = 0;
            double Dev0LLTR = 0;
            double DevHHLLT = 0;
            double DevHHLLTR = 0;
            double DevHLHHLLT = 0;
            double DevHLHHLLR = 0;
            double DevRPRMHLT = 0;
            double DevRPRMHLTR = 0;
            double Dev0RPRMT = 0;
            double Dev0RPRMTR = 0;
            double DevHLT = 0;
            double DevHLTR = 0;
            double DevPT = 0;
            double DevPTR = 0;
            double DevNT = 0;
            double DevNTR = 0;
            #endregion
            #region 声明时间list

            List<D22STimeClass> DevHHTime = new List<D22STimeClass>();
            List<D22STimeClass> DevHTime = new List<D22STimeClass>();
            List<D22STimeClass> DevRPTime = new List<D22STimeClass>();
            List<D22STimeClass> Dev0PTime = new List<D22STimeClass>();
            List<D22STimeClass> Dev0NTime = new List<D22STimeClass>();
            List<D22STimeClass> DevRNTime = new List<D22STimeClass>();
            List<D22STimeClass> DevLTime = new List<D22STimeClass>();
            List<D22STimeClass> DevLLTime = new List<D22STimeClass>();
            List<D22STimeClass> Dev0RPRMTTime = new List<D22STimeClass>();
            List<D22STimeClass> DevHLTTime = new List<D22STimeClass>();
            List<D22STimeClass> DevPTTime = new List<D22STimeClass>();
            List<D22STimeClass> DevNTTime = new List<D22STimeClass>();
            DevHHTime.AddRange(valueList[0].DevHHTime);
            DevHTime.AddRange(valueList[0].DevHTime);
            DevRPTime.AddRange(valueList[0].DevRPTime);
            Dev0PTime.AddRange(valueList[0].Dev0PTime);
            Dev0NTime.AddRange(valueList[0].Dev0NTime);
            DevRNTime.AddRange(valueList[0].DevRNTime);
            DevLTime.AddRange(valueList[0].DevLTime);
            DevLLTime.AddRange(valueList[0].DevLLTime);
            Dev0RPRMTTime.AddRange(valueList[0].Dev0RPRMTTime);
            DevHLTTime.AddRange(valueList[0].DevHLTTime);
            DevPTTime.AddRange(valueList[0].DevPTTime);
            DevNTTime.AddRange(valueList[0].DevNTTime);
            #endregion
            for (int i = 0; i < valueList.Count(); i++)
            {
                DevHHN += valueList[i].DevHHN;      //越 HH 次数，X ≥ HH
                DevHHT += valueList[i].DevHHT;      //越 HH 总时长，N 周期
                DevHHR += valueList[i].DevHHR;      //越 HH 时间占比，%
                DevHHA += valueList[i].DevHHA;      //越 HH 面积，对 X ≥ HH 的点，求和 (点值 - HH)
                DevHN += valueList[i].DevHN;        //越 H  次数，X ≥ H
                DevHT += valueList[i].DevHT;        //越 H  总时长，N 周期
                DevHR += valueList[i].DevHR;        //越 H  时间占比，%
                DevHA += valueList[i].DevHA;        //越 H  面积，对 X ≥ H 的点，求和 (点值 - H)
                DevRPN += valueList[i].DevRPN;      //越 R+ 次数，X ≥ R+
                DevRPT += valueList[i].DevRPT;      //越 R+ 总时长，N 周期
                DevRPR += valueList[i].DevRPR;      //越 R+ 时间占比，%
                DevRPA += valueList[i].DevRPA;      //越 R+ 面积，对 X ≥ H 的点，求和 (点值 - R+)
                Dev0PN += valueList[i].Dev0PN;      //越 OP 次数，X ≥ OP
                Dev0PT += valueList[i].Dev0PT;      //越 OP 总时长，N 周期
                Dev0PR += valueList[i].Dev0PR;      //越 OP 时间占比，%
                Dev0PA += valueList[i].Dev0PA;      //越 OP 面积，对 X ≥ H 的点，求和 (点值 - OP)
                Dev0NN += valueList[i].Dev0NN;      //越 ON 次数，X ≥ ON
                Dev0NT += valueList[i].Dev0NT;      //越 ON 总时长，N 周期
                Dev0NR += valueList[i].Dev0NR;      //越 ON 时间占比，%
                Dev0NA += valueList[i].Dev0NA;      //越 ON 面积，对 X ≥ H 的点，求和 (点值 - ON)
                DevRNN += valueList[i].DevRNN;      //越 R- 次数，X ≥ R-
                DevRNT += valueList[i].DevRNT;      //越 R- 总时长，N 周期
                DevRNR += valueList[i].DevRNR;      //越 R- 时间占比，%
                DevRNA += valueList[i].DevRNA;      //越 R- 面积，对 X ≥ H 的点，求和 (点值 - R-)
                DevLN += valueList[i].DevLN;        //越 L  次数，X ≥ L
                DevLT += valueList[i].DevLT;        //越 L  总时长，N 周期
                DevLR += valueList[i].DevLR;        //越 L  时间占比，%
                DevLA += valueList[i].DevLA;        //越 L  面积，对 X ≥ H 的点，求和 (点值 - L)
                DevLLN += valueList[i].DevLLN;      //越 LL 次数，X ≥ LL
                DevLLT += valueList[i].DevLLT;      //越 LL 总时长，N 周期
                DevLLR += valueList[i].DevLLR;      //越 LL 时间占比，%
                DevLLA += valueList[i].DevLLA;      //越 LL 面积，对 X ≥ H 的点，求和 (点值 - LL)
                Dev0HT += valueList[i].Dev0HT;      //正常，      正段，在 0 - H 之内时间长，H ＞ X ≥ 0
                Dev0HTR += valueList[i].Dev0HTR;    //在正常，    正段之内时间占比
                Dev0HHT += valueList[i].Dev0HHT;    //非超限，  且正段，在 0 - HH 之内时间长，HH ＞ X ≥ 0
                Dev0HHTR += valueList[i].Dev0HHTR;  //在非超限，且正段之内时间占比
                Dev0L += valueList[i].Dev0L;        //正常，      负段，在 0 - L 之内时间长，0 ＞ X ＞ L
                Dev0LR += valueList[i].Dev0LR;      //在正常，    负段之内时间占比
                Dev0LLT += valueList[i].Dev0LLT;    //非超限，  且负段，在 0 - LL 之内时间长，0 ＞ X ＞ LL
                Dev0LLTR += valueList[i].Dev0LLTR;  //在非超限，且负段之内时间占比
                DevHHLLT += valueList[i].DevHHLLT;  //超限段。在 HH ≤ X，X ≤ LL 之间总时间长
                DevHHLLTR += valueList[i].DevHHLLTR;//在超限段总时间占比
                DevHLHHLLT += valueList[i].DevHLHHLLT;  //越限段，在 HH ＞ X ≥ H，L ≥ X ＞ LL 之间总时间长
                DevHLHHLLR += valueList[i].DevHLHHLLR;  //在越限段总时间占比
                DevRPRMHLT += valueList[i].DevRPRMHLT;  //正常，   非优秀段，在 H ＞ X ≥ R+，R- ≥ X ＞ L 之间总时间长
                DevRPRMHLTR += valueList[i].DevRPRMHLTR;//在正常， 非优秀段总时间占比
                Dev0RPRMT += valueList[i].Dev0RPRMT;    //优秀段，在 R+ ＞ X ＞ R- 之间总时间长
                Dev0RPRMTR += valueList[i].Dev0RPRMTR;  //在优秀段总时间占比
                DevHLT += valueList[i].DevHLT;          //正常段，在 H ＞ X ＞ L 之间总时间长
                DevHLTR += valueList[i].DevHLTR;        //在正常段总时间占比
                DevPT += valueList[i].DevPT;            //正向段，X ≥ 0 时间长
                DevPTR += valueList[i].DevPTR;          //正向段，X ≥ 0 时间占比
                DevNT += valueList[i].DevNT;            //负向段，X < 0 时间长
                DevNTR += valueList[i].DevNTR;          //负向段，X < 0 时间占比
                if (i > 0)
                {
                    #region 跨时间段合并
                    #region
                    List<D22STimeClass> newHHlist = new List<D22STimeClass>();
                    if (DevHHTime.Count() != 0 && valueList[i].DevHHTime.Count != 0)
                    {
                        newHHlist = AlgorithmHelper.GetD22STimeList(DevHHTime[DevHHTime.Count() - 1], valueList[i].DevHHTime[0]);
                        DevHHTime.RemoveAt(DevHHTime.Count() - 1);
                    }
                    if (newHHlist.Count > 0)
                    {
                        DevHHTime.AddRange(newHHlist);
                        for (int l = 1; l < valueList[i].DevHHTime.Count(); l++)
                        {
                            DevHHTime.Add(valueList[i].DevHHTime[l]);
                        }
                    }
                    else
                    {
                        for (int l = 0; l < valueList[i].DevHHTime.Count(); l++)
                        {
                            DevHHTime.Add(valueList[i].DevHHTime[l]);
                        }
                    }
                    #endregion
                    #region
                    List<D22STimeClass> newHlist = new List<D22STimeClass>();
                    if (DevHTime.Count() != 0 && valueList[i].DevHTime.Count != 0)
                    {
                        newHlist = AlgorithmHelper.GetD22STimeList(DevHTime[DevHTime.Count() - 1], valueList[i].DevHTime[0]);
                        DevHTime.RemoveAt(DevHTime.Count() - 1);
                        DevHTime.AddRange(newHlist);
                        for (int l = 1; l < valueList[i].DevHTime.Count(); l++)
                        {
                            DevHTime.Add(valueList[i].DevHTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevHTime.Count != 0)
                        {
                            DevHTime.AddRange(valueList[i].DevHTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> newRPlist = new List<D22STimeClass>();
                    if (DevRPTime.Count() != 0 && valueList[i].DevRPTime.Count != 0)
                    {
                        newRPlist = AlgorithmHelper.GetD22STimeList(DevRPTime[DevRPTime.Count() - 1], valueList[i].DevRPTime[0]);
                        DevRPTime.RemoveAt(DevRPTime.Count() - 1);
                        DevRPTime.AddRange(newRPlist);
                        for (int l = 1; l < valueList[i].DevRPTime.Count(); l++)
                        {
                            DevRPTime.Add(valueList[i].DevRPTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevRPTime.Count != 0)
                        {
                            DevRPTime.AddRange(valueList[i].DevRPTime);
                        }
                    }



                    #endregion
                    #region
                    List<D22STimeClass> newOPlist = new List<D22STimeClass>();
                    if (Dev0PTime.Count() != 0 && valueList[i].Dev0PTime.Count != 0)
                    {
                        newOPlist = AlgorithmHelper.GetD22STimeList(Dev0PTime[Dev0PTime.Count() - 1], valueList[i].Dev0PTime[0]);
                        Dev0PTime.RemoveAt(Dev0PTime.Count() - 1);
                    }
                    else
                    {
                        if (valueList[i].Dev0PTime.Count != 0)
                        {
                            Dev0PTime.AddRange(valueList[i].Dev0PTime);
                        }
                    }
                    Dev0PTime.AddRange(newOPlist);
                    for (int l = 1; l < valueList[i].Dev0PTime.Count(); l++)
                    {
                        Dev0PTime.Add(valueList[i].Dev0PTime[l]);
                    }
                    #endregion
                    #region
                    List<D22STimeClass> new0Nlist = new List<D22STimeClass>();
                    if (Dev0NTime.Count() != 0 && valueList[i].Dev0NTime.Count != 0)
                    {
                        new0Nlist = AlgorithmHelper.GetD22STimeList(Dev0NTime[Dev0NTime.Count() - 1], valueList[i].Dev0NTime[0]);
                        Dev0NTime.RemoveAt(Dev0NTime.Count() - 1);
                        Dev0NTime.AddRange(new0Nlist);
                        for (int l = 1; l < valueList[i].Dev0NTime.Count(); l++)
                        {
                            Dev0NTime.Add(valueList[i].Dev0NTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].Dev0NTime.Count != 0)
                        {
                            Dev0NTime.AddRange(valueList[i].Dev0NTime);
                        }
                    }
                    #endregion
                    #region
                    List<D22STimeClass> newRNlist = new List<D22STimeClass>();
                    if (DevRNTime.Count() != 0 && valueList[i].DevRNTime.Count != 0)
                    {
                        newRNlist = AlgorithmHelper.GetD22STimeList(DevRNTime[DevRNTime.Count() - 1], valueList[i].DevRNTime[0]);
                        DevRNTime.RemoveAt(DevRNTime.Count() - 1);
                        DevRNTime.AddRange(newRNlist);
                        for (int l = 1; l < valueList[i].DevRNTime.Count(); l++)
                        {
                            DevRNTime.Add(valueList[i].DevRNTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevRNTime.Count != 0)
                        {
                            DevRNTime.AddRange(valueList[i].DevRNTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> newLlist = new List<D22STimeClass>();
                    if (DevLTime.Count() != 0 && valueList[i].DevLTime.Count != 0)
                    {
                        newLlist = AlgorithmHelper.GetD22STimeList(DevLTime[DevLTime.Count() - 1], valueList[i].DevLTime[0]);
                        DevLTime.RemoveAt(DevLTime.Count() - 1);
                        DevLTime.AddRange(newLlist);
                        for (int l = 1; l < valueList[i].DevLTime.Count(); l++)
                        {
                            DevLTime.Add(valueList[i].DevLTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevLTime.Count != 0)
                        {
                            DevLTime.AddRange(valueList[i].DevLTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> newLLlist = new List<D22STimeClass>();
                    if (DevLLTime.Count() != 0 && valueList[i].DevLLTime.Count != 0)
                    {
                        newLLlist = AlgorithmHelper.GetD22STimeList(DevLLTime[DevLLTime.Count() - 1], valueList[i].DevLLTime[0]);
                        DevLLTime.RemoveAt(DevLLTime.Count() - 1);
                        DevLLTime.AddRange(newLLlist);
                        for (int l = 1; l < valueList[i].DevLLTime.Count(); l++)
                        {
                            DevLLTime.Add(valueList[i].DevLLTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevLLTime.Count != 0)
                        {
                            DevLLTime.AddRange(valueList[i].DevLLTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> new0RPRlist = new List<D22STimeClass>();
                    if (Dev0RPRMTTime.Count() != 0 && valueList[i].Dev0RPRMTTime.Count != 0)
                    {
                        new0RPRlist = AlgorithmHelper.GetD22STimeList(Dev0RPRMTTime[Dev0RPRMTTime.Count() - 1], valueList[i].Dev0RPRMTTime[0]);
                        Dev0RPRMTTime.RemoveAt(Dev0RPRMTTime.Count() - 1);
                        Dev0RPRMTTime.AddRange(new0RPRlist);
                        for (int l = 1; l < valueList[i].Dev0RPRMTTime.Count(); l++)
                        {
                            Dev0RPRMTTime.Add(valueList[i].Dev0RPRMTTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].Dev0RPRMTTime.Count != 0)
                        {
                            Dev0RPRMTTime.AddRange(valueList[i].Dev0RPRMTTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> newHLlist = new List<D22STimeClass>();
                    if (DevHLTTime.Count() != 0 && valueList[i].DevHLTTime.Count != 0)
                    {
                        newHLlist = AlgorithmHelper.GetD22STimeList(DevHLTTime[DevHLTTime.Count() - 1], valueList[i].DevHLTTime[0]);
                        DevHLTTime.RemoveAt(newHLlist.Count() - 1);
                        DevHLTTime.AddRange(newHHlist);
                        for (int l = 1; l < valueList[i].DevHLTTime.Count(); l++)
                        {
                            DevHLTTime.Add(valueList[i].DevHLTTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevHLTTime.Count != 0)
                        {
                            DevHLTTime.AddRange(valueList[i].DevHLTTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> newPTlist = new List<D22STimeClass>();
                    if (DevPTTime.Count() != 0 && valueList[i].DevPTTime.Count != 0)
                    {
                        newPTlist = AlgorithmHelper.GetD22STimeList(DevPTTime[DevPTTime.Count() - 1], valueList[i].DevPTTime[0]);
                        DevPTTime.RemoveAt(DevPTTime.Count() - 1);
                        DevPTTime.AddRange(newPTlist);
                        for (int l = 1; l < valueList[i].DevPTTime.Count(); l++)
                        {
                            DevPTTime.Add(valueList[i].DevPTTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevPTTime.Count != 0)
                        {
                            DevPTTime.AddRange(valueList[i].DevPTTime);
                        }
                    }

                    #endregion
                    #region
                    List<D22STimeClass> newNTlist = new List<D22STimeClass>();
                    if (DevNTTime.Count() != 0 && valueList[i].DevNTTime.Count != 0)
                    {
                        newNTlist = AlgorithmHelper.GetD22STimeList(DevNTTime[DevNTTime.Count() - 1], valueList[i].DevNTTime[0]);
                        DevNTTime.RemoveAt(DevNTTime.Count() - 1);
                        DevNTTime.AddRange(newNTlist);
                        for (int l = 1; l < valueList[i].DevNTTime.Count(); l++)
                        {
                            DevNTTime.Add(valueList[i].DevNTTime[l]);
                        }
                    }
                    else
                    {
                        if (valueList[i].DevNTTime.Count != 0)
                        {
                            DevNTTime.AddRange(valueList[i].DevNTTime);
                        }
                    }

                    #endregion
                    #endregion
                }
            }
            double DevHHTMax = 0;
            double DevHTMax = 0;
            double DevRPTMax = 0;
            double Dev0PTMax = 0;
            double Dev0NTMax = 0;
            double DevRNTMax = 0;
            double DevLTMax = 0;
            double DevLLTMax = 0;
            double Dev0RPRMTMax = 0;
            double DevHLTMax = 0;
            double DevPTRTMax = 0;
            double DevNTRTMax = 0;

            if (DevHHTime.Count != 0)
            {
                D22STimeClass DevHHTMaxTime = AlgorithmHelper.compareFunction(DevHHTime, ref DevHHTMax);
                returnClass.DevHHN = DevHHTime.Count;
                returnClass.DevHHTime = DevHHTime;
                returnClass.DevHHT = DevHHT;
                returnClass.DevHHR = Math.Round(DevHHR / (double)(valueList.Count()), 3);
                returnClass.DevHHTMax = DevHHTMax;
                returnClass.DevHHTMaxTime = DevHHTMaxTime;
                returnClass.DevHHA = DevHHA;
                returnClass.DevHHET = Math.Round(DevHHA / DevHHT, 3);
            }
            if (DevHTime.Count != 0)
            {
                D22STimeClass DevHTMaxTime = AlgorithmHelper.compareFunction(DevHTime, ref DevHTMax);
                returnClass.DevHN = DevHTime.Count;
                returnClass.DevHTime = DevHTime;
                returnClass.DevHT = DevHT;
                returnClass.DevHR = Math.Round(DevHR / (double)(valueList.Count()), 3);
                returnClass.DevHTMax = DevHTMax;
                returnClass.DevHTMaxTime = DevHTMaxTime;
                returnClass.DevHA = DevHA;
                returnClass.DevHET = Math.Round(DevHET / DevHT, 3);
            }
            if (DevRPTime.Count != 0)
            {
                D22STimeClass DevRPTMaxTime = AlgorithmHelper.compareFunction(DevRPTime, ref DevRPTMax);
                returnClass.DevRPN = DevRPTime.Count;
                returnClass.DevRPTime = DevRPTime;
                returnClass.DevRPT = DevRPT;
                returnClass.DevRPR = Math.Round(DevRPR / (double)(valueList.Count()), 3); ;
                returnClass.DevRPTMax = DevRPTMax;
                returnClass.DevRPTMaxTime = DevRPTMaxTime;
                returnClass.DevRPA = DevRPA;
                returnClass.DevRPET = Math.Round(DevRPA / DevRPT, 3);
            }
            if (Dev0PTime.Count != 0)
            {
                D22STimeClass Dev0PTMaxTime = AlgorithmHelper.compareFunction(Dev0PTime, ref Dev0PTMax);
                returnClass.Dev0PN = Dev0PTime.Count;
                returnClass.Dev0PTime = Dev0PTime;
                returnClass.Dev0PT = Dev0PT;
                returnClass.Dev0PR = Math.Round(Dev0PR / (double)(valueList.Count()), 3);
                returnClass.Dev0PTMax = Dev0PTMax;
                returnClass.Dev0PTMaxTime = Dev0PTMaxTime;
                returnClass.Dev0PA = Dev0PA;
                returnClass.Dev0PET = Math.Round(Dev0PA / Dev0PT, 3);
            }

            if (Dev0NTime.Count != 0)
            {
                D22STimeClass Dev0NTMaxTime = AlgorithmHelper.compareFunction(Dev0NTime, ref Dev0NTMax);
                returnClass.Dev0NN = Dev0NTime.Count;
                returnClass.Dev0NTime = Dev0NTime;
                returnClass.Dev0NT = Dev0NT;
                returnClass.Dev0NR = Math.Round(Dev0NR / (double)(valueList.Count()), 3);
                returnClass.Dev0NTMax = Dev0NTMax;
                returnClass.Dev0NTMaxTime = Dev0NTMaxTime;
                returnClass.Dev0NA = Dev0NA;
                returnClass.Dev0NET = Math.Round(Dev0NA / Dev0NT, 3);
            }

            if (DevRNTime.Count != 0)
            {
                D22STimeClass DevRNTMaxTime = AlgorithmHelper.compareFunction(DevRNTime, ref DevRNTMax);
                returnClass.DevRNN = DevRNTime.Count;
                returnClass.DevRNTime = DevRNTime;
                returnClass.DevRNT = DevRNT;
                returnClass.DevRNR = Math.Round(DevRNR / (double)(valueList.Count()), 3);
                returnClass.DevRNTMax = DevRNTMax;
                returnClass.DevRNTMaxTime = DevRNTMaxTime;
                returnClass.DevRNA = DevRNA;
                returnClass.DevRNET = Math.Round(DevRNA / DevRNT, 3);
            }


            if (DevLTime.Count != 0)
            {
                D22STimeClass DevLTMaxTime = AlgorithmHelper.compareFunction(DevLTime, ref DevLTMax);
                returnClass.DevLN = DevLTime.Count;
                returnClass.DevLTime = DevLTime;
                returnClass.DevLT = DevLT;
                returnClass.DevLR = Math.Round(DevLR / (double)(valueList.Count()), 3);
                returnClass.DevLTMax = DevLTMax;
                returnClass.DevLTMaxTime = DevLTMaxTime;
                returnClass.DevLA = DevLA;
                returnClass.DevLET = Math.Round(DevLA / DevLT, 3);
            }

            if (DevLLTime.Count != 0)
            {
                D22STimeClass DevLLTMaxTime = AlgorithmHelper.compareFunction(DevLLTime, ref DevLLTMax);
                returnClass.DevLLN = DevLLTime.Count;
                returnClass.DevLLTime = DevLLTime;
                returnClass.DevLLT = DevLLT;
                returnClass.DevLLR = Math.Round(DevLLR / (double)(valueList.Count()), 3);
                returnClass.DevLLTMax = DevLLTMax;
                returnClass.DevLLTMaxTime = DevLLTMaxTime;
                returnClass.DevLLA = DevLLA;
                returnClass.DevLLET = Math.Round(DevLLA / DevLLT, 3);
            }

            returnClass.Dev0HT = Dev0HT;
            returnClass.Dev0HTR = Math.Round(Dev0HTR / (double)(valueList.Count()), 3);
            returnClass.Dev0HHT = Dev0HHT;
            returnClass.Dev0HHTR = Math.Round(Dev0HHTR / (double)(valueList.Count()), 3);
            returnClass.Dev0L = Dev0L;
            returnClass.Dev0LR = Math.Round(Dev0LR / (double)(valueList.Count()), 3);
            returnClass.Dev0LLT = Dev0LLT;
            returnClass.Dev0LLTR = Math.Round(Dev0LLTR / (double)(valueList.Count()), 3);
            returnClass.DevHHLLT = DevHHLLT;
            returnClass.DevHHLLTR = Math.Round(DevHHLLTR / (double)(valueList.Count()), 3);
            returnClass.DevHLHHLLT = DevHLHHLLT;
            returnClass.DevHLHHLLR = Math.Round(DevHLHHLLR / (double)(valueList.Count()), 3);
            returnClass.DevRPRMHLT = DevRPRMHLT;
            returnClass.DevRPRMHLTR = Math.Round(DevRPRMHLTR / (double)(valueList.Count()), 3);

            if (Dev0RPRMTTime.Count != 0)
            {
                D22STimeClass Dev0RPRMTMaxTime = AlgorithmHelper.compareFunction(Dev0RPRMTTime, ref Dev0RPRMTMax);
                returnClass.Dev0RPRMTTime = Dev0RPRMTTime;
                returnClass.Dev0RPRMT = DevHHN;
                returnClass.Dev0RPRMTR = Math.Round(DevRPRMHLTR / (double)(valueList.Count()), 3);
                returnClass.Dev0RPRMTMax = Dev0RPRMTMax;
                returnClass.Dev0RPRMTMaxTime = Dev0RPRMTMaxTime;
            }

            if (DevHLTTime.Count != 0)
            {
                D22STimeClass DevHLTMaxTime = AlgorithmHelper.compareFunction(DevHLTTime, ref DevHLTMax);
                returnClass.DevHLTTime = DevHLTTime;
                returnClass.DevHLT = DevHLT;
                returnClass.DevHLTR = Math.Round(DevHLTR / (double)(valueList.Count()), 3);
                returnClass.DevHLTMax = DevHLTMax;
                returnClass.DevHLTMaxTime = DevHLTMaxTime;
            }

            if (DevPTTime.Count != 0)
            {
                D22STimeClass DevPTRTMaxTime = AlgorithmHelper.compareFunction(DevPTTime, ref DevPTRTMax);
                returnClass.DevPTTime = DevPTTime;
                returnClass.DevPT = DevPT;
                returnClass.DevPTR = Math.Round(DevPTR / (double)(valueList.Count()), 3);
                returnClass.DevPTRTMax = DevPTRTMax;
                returnClass.DevPTRTMaxTime = DevPTRTMaxTime;
            }

            if (DevNTTime.Count != 0)
            {
                D22STimeClass DevNTRTMaxTime = AlgorithmHelper.compareFunction(DevNTTime, ref DevNTRTMax);
                returnClass.DevNTTime = DevNTTime;
                returnClass.DevNT = DevNT;
                returnClass.DevNTR = Math.Round(DevNTR / (double)(valueList.Count()), 3);
                returnClass.DevNTRTMax = DevNTRTMax;
                returnClass.DevNTRTMaxTime = DevNTRTMaxTime;
            }










            return returnClass;
        }
    }
}
