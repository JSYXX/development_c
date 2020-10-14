using MySql.Data.MySqlClient;
using PCCommon.NewCaculateCommand;
using PSLCalcu.Module.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.DAL
{
    public static class AlgorithmDAL
    {
        public static bool updateMPVBase(int id, MPVBaseMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid",id),
                new MySqlParameter("PVBMinV", newClass.PVBMin),
                new MySqlParameter("PVBMinTimeV", newClass.PVBMinTime),
                new MySqlParameter("PVBAvgV", newClass.PVBAvg),
                new MySqlParameter("PVBMaxV", newClass.PVBMax),
                new MySqlParameter("PVBMaxTimeV", newClass.PVBMaxTime),
                new MySqlParameter("PVBDMaxV", newClass.PVBDMax),
                new MySqlParameter("PVBSumV", newClass.PVBSum),
                new MySqlParameter("PVBSumkbV", newClass.PVBSumkb),
                new MySqlParameter("PVBLinekV", newClass.PVBLinek),
                new MySqlParameter("PVBLinebV", newClass.PVBLineb),
                new MySqlParameter("PVBSumPNRV", newClass.PVBSumPNR),
                new MySqlParameter("PVBAbsSumV", newClass.PVBAbsSum),
                new MySqlParameter("PVBStdevV", newClass.PVBStdev),
                new MySqlParameter("PVBVolatilityV", newClass.PVBVolatility),
                new MySqlParameter("PVBSDMaxV", newClass.PVBSDMax),
                new MySqlParameter("PVBSDMaxRV", newClass.PVBSDMaxR),
                new MySqlParameter("PVBDN1NumV", newClass.PVBDN1Num),
                new MySqlParameter("PVBDN2NumV", newClass.PVBDN2Num),
                new MySqlParameter("PVBDN3NumV", newClass.PVBDN3Num),
                new MySqlParameter("PVBTNumV", newClass.PVBTNum),
                new MySqlParameter("PVBVMaxV", newClass.PVBVMax),
                new MySqlParameter("PVBVMinV", newClass.PVBVMin),
                new MySqlParameter("PVBVAvgV", newClass.PVBVAvg),
                new MySqlParameter("PVBStbTRV", newClass.PVBStbTR),
                new MySqlParameter("PVBNoStbTRV", newClass.PVBNoStbTR),
                new MySqlParameter("PVBStbTSLV", newClass.PVBStbTSL),
                new MySqlParameter("PVBStbTSLRV", newClass.PVBStbTSLR),
                new MySqlParameter("PVBNoStbTSLV", newClass.PVBNoStbTSL),
                new MySqlParameter("PVBNoStbTSLRV", newClass.PVBNoStbTSLR),
                new MySqlParameter("PVBUpTSLV", newClass.PVBUpTSL),
                new MySqlParameter("PVBUpTSLRV", newClass.PVBUpTSLR),
                new MySqlParameter("PVBDownTSLV", newClass.PVBDownTSL),
                new MySqlParameter("PVBDownTSLRV", newClass.PVBDownTSLR),
                new MySqlParameter("PVBPNumV", newClass.PVBPNum),
                new MySqlParameter("PVBQltRV", newClass.PVBQltR),
                new MySqlParameter("PVBQaV", newClass.PVBQa),
                new MySqlParameter("PVBQbV", newClass.PVBQb),
                new MySqlParameter("PVBQcV", newClass.PVBQc),
                new MySqlParameter("PVBStbTSLPVV", newClass.PVBStbTSLPV)};
                isok = Helper.MysqlHelper.ModifySingleSql("updateMPVBase", CommandType.StoredProcedure, paramses, ref errmsg);

                int aid = getAID(type);
                isok = deleteTime(id);
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBSDMaxTime }, aid, id, "PVBSDMaxTime", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBStbTSLT }, aid, id, "PVBStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBNoStbTSLT }, aid, id, "PVBNoStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBUpTSLT }, aid, id, "PVBUpTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBDownTSLT }, aid, id, "PVBDownTSLT", "MPVBase");
                isok = insertTimeValue(newClass.PVBStbT, aid, id, "PVBStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBNoStbT, aid, id, "PVBNoStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBSDMaxTimeG, aid, id, "PVBSDMaxTimeG", "MPVBase");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool updateLongMPVBase(int id, MPVBaseMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid",id),
                new MySqlParameter("PVBMinV", newClass.PVBMin),
                new MySqlParameter("PVBMinTimeV", newClass.PVBMinTime),
                new MySqlParameter("PVBAvgV", newClass.PVBAvg),
                new MySqlParameter("PVBMaxV", newClass.PVBMax),
                new MySqlParameter("PVBMaxTimeV", newClass.PVBMaxTime),
                new MySqlParameter("PVBDMaxV", newClass.PVBDMax),
                new MySqlParameter("PVBSumV", newClass.PVBSum),
                new MySqlParameter("PVBSumkbV", newClass.PVBSumkb),
                new MySqlParameter("PVBLinekV", newClass.PVBLinek),
                new MySqlParameter("PVBLinebV", newClass.PVBLineb),
                new MySqlParameter("PVBSumPNRV", newClass.PVBSumPNR),
                new MySqlParameter("PVBAbsSumV", newClass.PVBAbsSum),
                new MySqlParameter("PVBStdevV", newClass.PVBStdev),
                new MySqlParameter("PVBVolatilityV", newClass.PVBVolatility),
                new MySqlParameter("PVBSDMaxV", newClass.PVBSDMax),
                new MySqlParameter("PVBSDMaxRV", newClass.PVBSDMaxR),
                new MySqlParameter("PVBDN1NumV", newClass.PVBDN1Num),
                new MySqlParameter("PVBDN2NumV", newClass.PVBDN2Num),
                new MySqlParameter("PVBDN3NumV", newClass.PVBDN3Num),
                new MySqlParameter("PVBTNumV", newClass.PVBTNum),
                new MySqlParameter("PVBVMaxV", newClass.PVBVMax),
                new MySqlParameter("PVBVMinV", newClass.PVBVMin),
                new MySqlParameter("PVBVAvgV", newClass.PVBVAvg),
                new MySqlParameter("PVBStbTRV", newClass.PVBStbTR),
                new MySqlParameter("PVBNoStbTRV", newClass.PVBNoStbTR),
                new MySqlParameter("PVBStbTSLV", newClass.PVBStbTSL),
                new MySqlParameter("PVBStbTSLRV", newClass.PVBStbTSLR),
                new MySqlParameter("PVBNoStbTSLV", newClass.PVBNoStbTSL),
                new MySqlParameter("PVBNoStbTSLRV", newClass.PVBNoStbTSLR),
                new MySqlParameter("PVBUpTSLV", newClass.PVBUpTSL),
                new MySqlParameter("PVBUpTSLRV", newClass.PVBUpTSLR),
                new MySqlParameter("PVBDownTSLV", newClass.PVBDownTSL),
                new MySqlParameter("PVBDownTSLRV", newClass.PVBDownTSLR),
                new MySqlParameter("PVBPNumV", newClass.PVBPNum),
                new MySqlParameter("PVBQltRV", newClass.PVBQltR),
                new MySqlParameter("PVBQaV", newClass.PVBQa),
                new MySqlParameter("PVBQbV", newClass.PVBQb),
                new MySqlParameter("PVBQcV", newClass.PVBQc),
                new MySqlParameter("PVBStbTSLPVV", newClass.PVBStbTSLPV)};
                isok = Helper.MysqlHelper.ModifySingleSql("updateMPVBase", CommandType.StoredProcedure, paramses, ref errmsg);

                int aid = getAID(type);
                isok = deleteTime(id);
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBSDMaxTime }, aid, id, "PVBSDMaxTime", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBStbTSLT }, aid, id, "PVBStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBNoStbTSLT }, aid, id, "PVBNoStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBUpTSLT }, aid, id, "PVBUpTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBDownTSLT }, aid, id, "PVBDownTSLT", "MPVBase");
                isok = insertTimeValue(newClass.PVBStbT, aid, id, "PVBStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBNoStbT, aid, id, "PVBNoStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBSDMaxTimeG, aid, id, "PVBSDMaxTimeG", "MPVBase");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool insertLongMPVBase(MPVBaseMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("PVBMinV", newClass.PVBMin),
                new MySqlParameter("PVBMinTimeV", newClass.PVBMinTime),
                new MySqlParameter("PVBAvgV", newClass.PVBAvg),
                new MySqlParameter("PVBMaxV", newClass.PVBMax),
                new MySqlParameter("PVBMaxTimeV", newClass.PVBMaxTime),
                new MySqlParameter("PVBDMaxV", newClass.PVBDMax),
                new MySqlParameter("PVBSumV", newClass.PVBSum),
                new MySqlParameter("PVBSumkbV", newClass.PVBSumkb),
                new MySqlParameter("PVBLinekV", newClass.PVBLinek),
                new MySqlParameter("PVBLinebV", newClass.PVBLineb),
                new MySqlParameter("PVBSumPNRV", newClass.PVBSumPNR),
                new MySqlParameter("PVBAbsSumV", newClass.PVBAbsSum),
                new MySqlParameter("PVBStdevV", newClass.PVBStdev),
                new MySqlParameter("PVBVolatilityV", newClass.PVBVolatility),
                new MySqlParameter("PVBSDMaxV", newClass.PVBSDMax),
                new MySqlParameter("PVBSDMaxRV", newClass.PVBSDMaxR),
                new MySqlParameter("PVBDN1NumV", newClass.PVBDN1Num),
                new MySqlParameter("PVBDN2NumV", newClass.PVBDN2Num),
                new MySqlParameter("PVBDN3NumV", newClass.PVBDN3Num),
                new MySqlParameter("PVBTNumV", newClass.PVBTNum),
                new MySqlParameter("PVBVMaxV", newClass.PVBVMax),
                new MySqlParameter("PVBVMinV", newClass.PVBVMin),
                new MySqlParameter("PVBVAvgV", newClass.PVBVAvg),
                new MySqlParameter("PVBStbTRV", newClass.PVBStbTR),
                new MySqlParameter("PVBNoStbTRV", newClass.PVBNoStbTR),
                new MySqlParameter("PVBStbTSLV", newClass.PVBStbTSL),
                new MySqlParameter("PVBStbTSLRV", newClass.PVBStbTSLR),
                new MySqlParameter("PVBNoStbTSLV", newClass.PVBNoStbTSL),
                new MySqlParameter("PVBNoStbTSLRV", newClass.PVBNoStbTSLR),
                new MySqlParameter("PVBUpTSLV", newClass.PVBUpTSL),
                new MySqlParameter("PVBUpTSLRV", newClass.PVBUpTSLR),
                new MySqlParameter("PVBDownTSLV", newClass.PVBDownTSL),
                new MySqlParameter("PVBDownTSLRV", newClass.PVBDownTSLR),
                new MySqlParameter("PVBPNumV", newClass.PVBPNum),
                new MySqlParameter("PVBQltRV", newClass.PVBQltR),
                new MySqlParameter("PVBQaV", newClass.PVBQa),
                new MySqlParameter("PVBQbV", newClass.PVBQb),
                new MySqlParameter("PVBQcV", newClass.PVBQc),
                new MySqlParameter("PVBStbTSLPVV", newClass.PVBStbTSLPV),
                new MySqlParameter("typeV", type),
                new MySqlParameter("yearV", year),
                new MySqlParameter("monthV", month),
                new MySqlParameter("dayV", day),
                new MySqlParameter("hourV", hour)};
                DataTable dt = Helper.MysqlHelper.getDataTableOfSQL("insertMPVBase", CommandType.StoredProcedure, paramses, ref errmsg);
                int pid = Convert.ToInt32(dt.Rows[0]["lastid"].ToString());
                int aid = getAID(type);
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBSDMaxTime }, aid, pid, "PVBSDMaxTime", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBStbTSLT }, aid, pid, "PVBStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBNoStbTSLT }, aid, pid, "PVBNoStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBUpTSLT }, aid, pid, "PVBUpTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBDownTSLT }, aid, pid, "PVBDownTSLT", "MPVBase");
                isok = insertTimeValue(newClass.PVBStbT, aid, pid, "PVBStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBNoStbT, aid, pid, "PVBNoStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBSDMaxTimeG, aid, pid, "PVBSDMaxTimeG", "MPVBase");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool insertMPVBase(MPVBaseMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("PVBMinV", newClass.PVBMin),
                new MySqlParameter("PVBMinTimeV", newClass.PVBMinTime),
                new MySqlParameter("PVBAvgV", newClass.PVBAvg),
                new MySqlParameter("PVBMaxV", newClass.PVBMax),
                new MySqlParameter("PVBMaxTimeV", newClass.PVBMaxTime),
                new MySqlParameter("PVBDMaxV", newClass.PVBDMax),
                new MySqlParameter("PVBSumV", newClass.PVBSum),
                new MySqlParameter("PVBSumkbV", newClass.PVBSumkb),
                new MySqlParameter("PVBLinekV", newClass.PVBLinek),
                new MySqlParameter("PVBLinebV", newClass.PVBLineb),
                new MySqlParameter("PVBSumPNRV", newClass.PVBSumPNR),
                new MySqlParameter("PVBAbsSumV", newClass.PVBAbsSum),
                new MySqlParameter("PVBStdevV", newClass.PVBStdev),
                new MySqlParameter("PVBVolatilityV", newClass.PVBVolatility),
                new MySqlParameter("PVBSDMaxV", newClass.PVBSDMax),
                new MySqlParameter("PVBSDMaxRV", newClass.PVBSDMaxR),
                new MySqlParameter("PVBDN1NumV", newClass.PVBDN1Num),
                new MySqlParameter("PVBDN2NumV", newClass.PVBDN2Num),
                new MySqlParameter("PVBDN3NumV", newClass.PVBDN3Num),
                new MySqlParameter("PVBTNumV", newClass.PVBTNum),
                new MySqlParameter("PVBVMaxV", newClass.PVBVMax),
                new MySqlParameter("PVBVMinV", newClass.PVBVMin),
                new MySqlParameter("PVBVAvgV", newClass.PVBVAvg),
                new MySqlParameter("PVBStbTRV", newClass.PVBStbTR),
                new MySqlParameter("PVBNoStbTRV", newClass.PVBNoStbTR),
                new MySqlParameter("PVBStbTSLV", newClass.PVBStbTSL),
                new MySqlParameter("PVBStbTSLRV", newClass.PVBStbTSLR),
                new MySqlParameter("PVBNoStbTSLV", newClass.PVBNoStbTSL),
                new MySqlParameter("PVBNoStbTSLRV", newClass.PVBNoStbTSLR),
                new MySqlParameter("PVBUpTSLV", newClass.PVBUpTSL),
                new MySqlParameter("PVBUpTSLRV", newClass.PVBUpTSLR),
                new MySqlParameter("PVBDownTSLV", newClass.PVBDownTSL),
                new MySqlParameter("PVBDownTSLRV", newClass.PVBDownTSLR),
                new MySqlParameter("PVBPNumV", newClass.PVBPNum),
                new MySqlParameter("PVBQltRV", newClass.PVBQltR),
                new MySqlParameter("PVBQaV", newClass.PVBQa),
                new MySqlParameter("PVBQbV", newClass.PVBQb),
                new MySqlParameter("PVBQcV", newClass.PVBQc),
                new MySqlParameter("PVBStbTSLPVV", newClass.PVBStbTSLPV),
                new MySqlParameter("typeV", type),
                new MySqlParameter("yearV", year),
                new MySqlParameter("monthV", month),
                new MySqlParameter("dayV", day),
                new MySqlParameter("hourV", hour)};
                DataTable dt = MysqlHelper.getDataTableOfSQL("insertMPVBase", CommandType.StoredProcedure, paramses, ref errmsg);
                int pid = Convert.ToInt32(dt.Rows[0]["lastid"].ToString());
                int aid = getAID(type);
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBSDMaxTime }, aid, pid, "PVBSDMaxTime", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBStbTSLT }, aid, pid, "PVBStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBNoStbTSLT }, aid, pid, "PVBNoStbTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBUpTSLT }, aid, pid, "PVBUpTSLT", "MPVBase");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.PVBDownTSLT }, aid, pid, "PVBDownTSLT", "MPVBase");
                isok = insertTimeValue(newClass.PVBStbT, aid, pid, "PVBStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBNoStbT, aid, pid, "PVBNoStbT", "MPVBase");
                isok = insertTimeValue(newClass.PVBSDMaxTimeG, aid, pid, "PVBSDMaxTimeG", "MPVBase");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static bool insertTimeValue(List<D22STimeClass> value, int aid, int pid, string columnName, string caculateName)
        {
            try
            {
                bool isok = false;
                string errmsg = string.Empty;
                if (value != null)
                {
                    foreach (D22STimeClass item in value)
                    {
                        if (item != null)
                        {
                            MySqlParameter[] paramses = {
                                new MySqlParameter("sDate", item.startDate),
                                new MySqlParameter("eDate", item.endDate),
                                new MySqlParameter("aid", aid),
                                new MySqlParameter("pid", pid),
                                new MySqlParameter("cid", columnName),
                                new MySqlParameter("cName", caculateName)
                            };
                            isok = MysqlHelper.ModifySingleSql("insertTimeValue", CommandType.StoredProcedure, paramses, ref errmsg);
                        }
                        else
                        {
                            isok = true;
                        }
                    }
                }
                else
                {
                    isok = true;
                }

                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static int getColumnID(string columnName)
        {
            try
            {
                int id = 0;
                string errmsg = string.Empty;
                MySqlParameter[] paramses = {
                new MySqlParameter("cName", columnName) };
                DataTable dt = MysqlHelper.getDataTableOfSQL("getColumnID", CommandType.StoredProcedure, paramses, ref errmsg);
                if (dt != null && dt.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool insertMDevLimit(MDevLimitMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                     new MySqlParameter("DevHHNV", newClass.DevHHN),
                     new MySqlParameter("DevHHTV", newClass.DevHHT),
                     new MySqlParameter("DevHHRV", newClass.DevHHR),
                     new MySqlParameter("DevHHTMaxV", newClass.DevHHTMax),
                     new MySqlParameter("DevHHAV", newClass.DevHHA),
                     new MySqlParameter("DevHHETV", newClass.DevHHET),
                     new MySqlParameter("DevHNV", newClass.DevHN),
                     new MySqlParameter("DevHTV", newClass.DevHT),
                     new MySqlParameter("DevHRV", newClass.DevHR),
                     new MySqlParameter("DevHTMaxV", newClass.DevHTMax),
                     new MySqlParameter("DevHAV", newClass.DevHA),
                     new MySqlParameter("DevHETV", newClass.DevHET),
                     new MySqlParameter("DevRPNV", newClass.DevRPN),
                     new MySqlParameter("DevRPTV", newClass.DevRPT),
                     new MySqlParameter("DevRPRV", newClass.DevRPR),
                     new MySqlParameter("DevRPTMaxV", newClass.DevRPTMax),
                     new MySqlParameter("DevRPAV", newClass.DevRPA),
                     new MySqlParameter("DevRPETV", newClass.DevRPET),
                     new MySqlParameter("Dev0PNV", newClass.Dev0PN),
                     new MySqlParameter("Dev0PTV", newClass.Dev0PT),
                     new MySqlParameter("Dev0PRV", newClass.Dev0PR),
                     new MySqlParameter("Dev0PTMaxV", newClass.Dev0PTMax),
                     new MySqlParameter("Dev0PAV", newClass.Dev0PA),
                     new MySqlParameter("Dev0PETV", newClass.Dev0PET),
                     new MySqlParameter("Dev0NNV", newClass.Dev0NN),
                     new MySqlParameter("Dev0NTV", newClass.Dev0NT),
                     new MySqlParameter("Dev0NRV", newClass.Dev0NR),
                     new MySqlParameter("Dev0NTMaxV", newClass.Dev0NTMax),
                     new MySqlParameter("Dev0NAV", newClass.Dev0NA),
                     new MySqlParameter("Dev0NETV", newClass.Dev0NET),
                     new MySqlParameter("DevRNNV", newClass.DevRNN),
                     new MySqlParameter("DevRNTV", newClass.DevRNT),
                     new MySqlParameter("DevRNRV", newClass.DevRNR),
                     new MySqlParameter("DevRNTMaxV", newClass.DevRNTMax),
                     new MySqlParameter("DevRNAV", newClass.DevRNA),
                     new MySqlParameter("DevRNETV", newClass.DevRNET),
                     new MySqlParameter("DevLNV", newClass.DevLN),
                     new MySqlParameter("DevLTV", newClass.DevLT),
                     new MySqlParameter("DevLRV", newClass.DevLR),
                     new MySqlParameter("DevLTMaxV", newClass.DevLTMax),
                     new MySqlParameter("DevLAV", newClass.DevLA),
                     new MySqlParameter("DevLETV", newClass.DevLET),
                     new MySqlParameter("DevLLNV", newClass.DevLLN),
                     new MySqlParameter("DevLLTV", newClass.DevLLT),
                     new MySqlParameter("DevLLRV", newClass.DevLLR),
                     new MySqlParameter("DevLLTMaxV", newClass.DevLLTMax),
                     new MySqlParameter("DevLLAV", newClass.DevLLA),
                     new MySqlParameter("DevLLETV", newClass.DevLLET),
                     new MySqlParameter("Dev0HTV", newClass.Dev0HT),
                     new MySqlParameter("Dev0HTRV", newClass.Dev0HTR),
                     new MySqlParameter("Dev0HHTV", newClass.Dev0HHT),
                     new MySqlParameter("Dev0HHTRV", newClass.Dev0HHTR),
                     new MySqlParameter("Dev0LV", newClass.Dev0L),
                     new MySqlParameter("Dev0LRV", newClass.Dev0LR),
                     new MySqlParameter("Dev0LLTV", newClass.Dev0LLT),
                     new MySqlParameter("Dev0LLTRV", newClass.Dev0LLTR),
                     new MySqlParameter("DevHHLLTV", newClass.DevHHLLT),
                     new MySqlParameter("DevHHLLTRV", newClass.DevHHLLTR),
                     new MySqlParameter("DevHLHHLLTV", newClass.DevHLHHLLT),
                     new MySqlParameter("DevHLHHLLRV", newClass.DevHLHHLLR),
                     new MySqlParameter("DevRPRMHLTV", newClass.DevRPRMHLT),
                     new MySqlParameter("DevRPRMHLTRV", newClass.DevRPRMHLTR),
                     new MySqlParameter("Dev0RPRMTV", newClass.Dev0RPRMT),
                     new MySqlParameter("Dev0RPRMTRV", newClass.Dev0RPRMTR),
                     new MySqlParameter("Dev0RPRMTMaxV", newClass.Dev0RPRMTMax),
                     new MySqlParameter("DevHLTV", newClass.DevHLT),
                     new MySqlParameter("DevHLTRV", newClass.DevHLTR),
                     new MySqlParameter("DevHLTMaxV", newClass.DevHLTMax),
                     new MySqlParameter("DevPTV", newClass.DevPT),
                     new MySqlParameter("DevPTRV", newClass.DevPTR),
                     new MySqlParameter("DevPTRTMaxV", newClass.DevPTRTMax),
                     new MySqlParameter("DevNTV", newClass.DevNT),
                     new MySqlParameter("DevNTRV", newClass.DevNTR),
                     new MySqlParameter("DevNTRTMaxV", newClass.DevNTRTMax),
                     new MySqlParameter("typeV", type),
                     new MySqlParameter("yearV", year),
                     new MySqlParameter("monthV", month),
                     new MySqlParameter("dayV", day),
                     new MySqlParameter("hourV", hour)};
                DataTable dt = MysqlHelper.getDataTableOfSQL("insertMDevLimit", CommandType.StoredProcedure, paramses, ref errmsg);
                int pid = Convert.ToInt32(dt.Rows[0]["lastid"].ToString());
                int aid = getAID(type);
                isok = insertTimeValue(newClass.DevHHTime, aid, pid, "DevHHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHHTMaxTime }, aid, pid, "DevHHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevHTime, aid, pid, "DevHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHTMaxTime }, aid, pid, "DevHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRPTime, aid, pid, "DevRPTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRPTMaxTime }, aid, pid, "DevRPTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0PTime, aid, pid, "Dev0PTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0PTMaxTime }, aid, pid, "Dev0PTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0NTime, aid, pid, "Dev0NTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0NTMaxTime }, aid, pid, "Dev0NTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRNTime, aid, pid, "DevRNTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRNTMaxTime }, aid, pid, "DevRNTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLTime, aid, pid, "DevLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLTMaxTime }, aid, pid, "DevLTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLLTime, aid, pid, "DevLLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLLTMaxTime }, aid, pid, "DevLLTMaxTime", "MDevLimit");

                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0RPRMTMaxTime }, aid, pid, "Dev0RPRMTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHLTMaxTime }, aid, pid, "DevHLTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevPTRTMaxTime }, aid, pid, "DevPTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevNTRTMaxTime }, aid, pid, "DevNTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(newClass.Dev0RPRMTTime, aid, pid, "Dev0RPRMTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevHLTTime, aid, pid, "DevHLTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevPTTime, aid, pid, "DevPTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevNTTime, aid, pid, "DevNTTime", "MDevLimit");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool updateMDevLimit(int id, MDevLimitMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                     new MySqlParameter("pid", id),
                     new MySqlParameter("DevHHNV", newClass.DevHHN),
                     new MySqlParameter("DevHHTV", newClass.DevHHT),
                     new MySqlParameter("DevHHRV", newClass.DevHHR),
                     new MySqlParameter("DevHHTMaxV", newClass.DevHHTMax),
                     new MySqlParameter("DevHHAV", newClass.DevHHA),
                     new MySqlParameter("DevHHETV", newClass.DevHHET),
                     new MySqlParameter("DevHNV", newClass.DevHN),
                     new MySqlParameter("DevHTV", newClass.DevHT),
                     new MySqlParameter("DevHRV", newClass.DevHR),
                     new MySqlParameter("DevHTMaxV", newClass.DevHTMax),
                     new MySqlParameter("DevHAV", newClass.DevHA),
                     new MySqlParameter("DevHETV", newClass.DevHET),
                     new MySqlParameter("DevRPNV", newClass.DevRPN),
                     new MySqlParameter("DevRPTV", newClass.DevRPT),
                     new MySqlParameter("DevRPRV", newClass.DevRPR),
                     new MySqlParameter("DevRPTMaxV", newClass.DevRPTMax),
                     new MySqlParameter("DevRPAV", newClass.DevRPA),
                     new MySqlParameter("DevRPETV", newClass.DevRPET),
                     new MySqlParameter("Dev0PNV", newClass.Dev0PN),
                     new MySqlParameter("Dev0PTV", newClass.Dev0PT),
                     new MySqlParameter("Dev0PRV", newClass.Dev0PR),
                     new MySqlParameter("Dev0PTMaxV", newClass.Dev0PTMax),
                     new MySqlParameter("Dev0PAV", newClass.Dev0PA),
                     new MySqlParameter("Dev0PETV", newClass.Dev0PET),
                     new MySqlParameter("Dev0NNV", newClass.Dev0NN),
                     new MySqlParameter("Dev0NTV", newClass.Dev0NT),
                     new MySqlParameter("Dev0NRV", newClass.Dev0NR),
                     new MySqlParameter("Dev0NTMaxV", newClass.Dev0NTMax),
                     new MySqlParameter("Dev0NAV", newClass.Dev0NA),
                     new MySqlParameter("Dev0NETV", newClass.Dev0NET),
                     new MySqlParameter("DevRNNV", newClass.DevRNN),
                     new MySqlParameter("DevRNTV", newClass.DevRNT),
                     new MySqlParameter("DevRNRV", newClass.DevRNR),
                     new MySqlParameter("DevRNTMaxV", newClass.DevRNTMax),
                     new MySqlParameter("DevRNAV", newClass.DevRNA),
                     new MySqlParameter("DevRNETV", newClass.DevRNET),
                     new MySqlParameter("DevLNV", newClass.DevLN),
                     new MySqlParameter("DevLTV", newClass.DevLT),
                     new MySqlParameter("DevLRV", newClass.DevLR),
                     new MySqlParameter("DevLTMaxV", newClass.DevLTMax),
                     new MySqlParameter("DevLAV", newClass.DevLA),
                     new MySqlParameter("DevLETV", newClass.DevLET),
                     new MySqlParameter("DevLLNV", newClass.DevLLN),
                     new MySqlParameter("DevLLTV", newClass.DevLLT),
                     new MySqlParameter("DevLLRV", newClass.DevLLR),
                     new MySqlParameter("DevLLTMaxV", newClass.DevLLTMax),
                     new MySqlParameter("DevLLAV", newClass.DevLLA),
                     new MySqlParameter("DevLLETV", newClass.DevLLET),
                     new MySqlParameter("Dev0HTV", newClass.Dev0HT),
                     new MySqlParameter("Dev0HTRV", newClass.Dev0HTR),
                     new MySqlParameter("Dev0HHTV", newClass.Dev0HHT),
                     new MySqlParameter("Dev0HHTRV", newClass.Dev0HHTR),
                     new MySqlParameter("Dev0LV", newClass.Dev0L),
                     new MySqlParameter("Dev0LRV", newClass.Dev0LR),
                     new MySqlParameter("Dev0LLTV", newClass.Dev0LLT),
                     new MySqlParameter("Dev0LLTRV", newClass.Dev0LLTR),
                     new MySqlParameter("DevHHLLTV", newClass.DevHHLLT),
                     new MySqlParameter("DevHHLLTRV", newClass.DevHHLLTR),
                     new MySqlParameter("DevHLHHLLTV", newClass.DevHLHHLLT),
                     new MySqlParameter("DevHLHHLLRV", newClass.DevHLHHLLR),
                     new MySqlParameter("DevRPRMHLTV", newClass.DevRPRMHLT),
                     new MySqlParameter("DevRPRMHLTRV", newClass.DevRPRMHLTR),
                     new MySqlParameter("Dev0RPRMTV", newClass.Dev0RPRMT),
                     new MySqlParameter("Dev0RPRMTRV", newClass.Dev0RPRMTR),
                     new MySqlParameter("Dev0RPRMTMaxV", newClass.Dev0RPRMTMax),
                     new MySqlParameter("DevHLTV", newClass.DevHLT),
                     new MySqlParameter("DevHLTRV", newClass.DevHLTR),
                     new MySqlParameter("DevHLTMaxV", newClass.DevHLTMax),
                     new MySqlParameter("DevPTV", newClass.DevPT),
                     new MySqlParameter("DevPTRV", newClass.DevPTR),
                     new MySqlParameter("DevPTRTMaxV", newClass.DevPTRTMax),
                     new MySqlParameter("DevNTV", newClass.DevNT),
                     new MySqlParameter("DevNTRV", newClass.DevNTR),
                     new MySqlParameter("DevNTRTMaxV", newClass.DevNTRTMax)};
                isok = MysqlHelper.ModifySingleSql("updateMDevLimit", CommandType.StoredProcedure, paramses, ref errmsg);
                int aid = getAID(type);
                isok = deleteTime(id);
                isok = insertTimeValue(newClass.DevHHTime, aid, id, "DevHHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHHTMaxTime }, aid, id, "DevHHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevHTime, aid, id, "DevHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHTMaxTime }, aid, id, "DevHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRPTime, aid, id, "DevRPTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRPTMaxTime }, aid, id, "DevRPTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0PTime, aid, id, "Dev0PTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0PTMaxTime }, aid, id, "Dev0PTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0NTime, aid, id, "Dev0NTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0NTMaxTime }, aid, id, "Dev0NTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRNTime, aid, id, "DevRNTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRNTMaxTime }, aid, id, "DevRNTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLTime, aid, id, "DevLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLTMaxTime }, aid, id, "DevLTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLLTime, aid, id, "DevLLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLLTMaxTime }, aid, id, "DevLLTMaxTime", "MDevLimit");

                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0RPRMTMaxTime }, aid, id, "Dev0RPRMTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHLTMaxTime }, aid, id, "DevHLTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevPTRTMaxTime }, aid, id, "DevPTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevNTRTMaxTime }, aid, id, "DevNTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(newClass.Dev0RPRMTTime, aid, id, "Dev0RPRMTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevHLTTime, aid, id, "DevHLTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevPTTime, aid, id, "DevPTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevNTTime, aid, id, "DevNTTime", "MDevLimit");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool updateLongMDevLimit(int id, MDevLimitMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid", id),
                     new MySqlParameter("DevHHNV", newClass.DevHHN),
                     new MySqlParameter("DevHHTV", newClass.DevHHT),
                     new MySqlParameter("DevHHRV", newClass.DevHHR),
                     new MySqlParameter("DevHHTMaxV", newClass.DevHHTMax),
                     new MySqlParameter("DevHHAV", newClass.DevHHA),
                     new MySqlParameter("DevHHETV", newClass.DevHHET),
                     new MySqlParameter("DevHNV", newClass.DevHN),
                     new MySqlParameter("DevHTV", newClass.DevHT),
                     new MySqlParameter("DevHRV", newClass.DevHR),
                     new MySqlParameter("DevHTMaxV", newClass.DevHTMax),
                     new MySqlParameter("DevHAV", newClass.DevHA),
                     new MySqlParameter("DevHETV", newClass.DevHET),
                     new MySqlParameter("DevRPNV", newClass.DevRPN),
                     new MySqlParameter("DevRPTV", newClass.DevRPT),
                     new MySqlParameter("DevRPRV", newClass.DevRPR),
                     new MySqlParameter("DevRPTMaxV", newClass.DevRPTMax),
                     new MySqlParameter("DevRPAV", newClass.DevRPA),
                     new MySqlParameter("DevRPETV", newClass.DevRPET),
                     new MySqlParameter("Dev0PNV", newClass.Dev0PN),
                     new MySqlParameter("Dev0PTV", newClass.Dev0PT),
                     new MySqlParameter("Dev0PRV", newClass.Dev0PR),
                     new MySqlParameter("Dev0PTMaxV", newClass.Dev0PTMax),
                     new MySqlParameter("Dev0PAV", newClass.Dev0PA),
                     new MySqlParameter("Dev0PETV", newClass.Dev0PET),
                     new MySqlParameter("Dev0NNV", newClass.Dev0NN),
                     new MySqlParameter("Dev0NTV", newClass.Dev0NT),
                     new MySqlParameter("Dev0NRV", newClass.Dev0NR),
                     new MySqlParameter("Dev0NTMaxV", newClass.Dev0NTMax),
                     new MySqlParameter("Dev0NAV", newClass.Dev0NA),
                     new MySqlParameter("Dev0NETV", newClass.Dev0NET),
                     new MySqlParameter("DevRNNV", newClass.DevRNN),
                     new MySqlParameter("DevRNTV", newClass.DevRNT),
                     new MySqlParameter("DevRNRV", newClass.DevRNR),
                     new MySqlParameter("DevRNTMaxV", newClass.DevRNTMax),
                     new MySqlParameter("DevRNAV", newClass.DevRNA),
                     new MySqlParameter("DevRNETV", newClass.DevRNET),
                     new MySqlParameter("DevLNV", newClass.DevLN),
                     new MySqlParameter("DevLTV", newClass.DevLT),
                     new MySqlParameter("DevLRV", newClass.DevLR),
                     new MySqlParameter("DevLTMaxV", newClass.DevLTMax),
                     new MySqlParameter("DevLAV", newClass.DevLA),
                     new MySqlParameter("DevLETV", newClass.DevLET),
                     new MySqlParameter("DevLLNV", newClass.DevLLN),
                     new MySqlParameter("DevLLTV", newClass.DevLLT),
                     new MySqlParameter("DevLLRV", newClass.DevLLR),
                     new MySqlParameter("DevLLTMaxV", newClass.DevLLTMax),
                     new MySqlParameter("DevLLAV", newClass.DevLLA),
                     new MySqlParameter("DevLLETV", newClass.DevLLET),
                     new MySqlParameter("Dev0HTV", newClass.Dev0HT),
                     new MySqlParameter("Dev0HTRV", newClass.Dev0HTR),
                     new MySqlParameter("Dev0HHTV", newClass.Dev0HHT),
                     new MySqlParameter("Dev0HHTRV", newClass.Dev0HHTR),
                     new MySqlParameter("Dev0LV", newClass.Dev0L),
                     new MySqlParameter("Dev0LRV", newClass.Dev0LR),
                     new MySqlParameter("Dev0LLTV", newClass.Dev0LLT),
                     new MySqlParameter("Dev0LLTRV", newClass.Dev0LLTR),
                     new MySqlParameter("DevHHLLTV", newClass.DevHHLLT),
                     new MySqlParameter("DevHHLLTRV", newClass.DevHHLLTR),
                     new MySqlParameter("DevHLHHLLTV", newClass.DevHLHHLLT),
                     new MySqlParameter("DevHLHHLLRV", newClass.DevHLHHLLR),
                     new MySqlParameter("DevRPRMHLTV", newClass.DevRPRMHLT),
                     new MySqlParameter("DevRPRMHLTRV", newClass.DevRPRMHLTR),
                     new MySqlParameter("Dev0RPRMTV", newClass.Dev0RPRMT),
                     new MySqlParameter("Dev0RPRMTRV", newClass.Dev0RPRMTR),
                     new MySqlParameter("Dev0RPRMTMaxV", newClass.Dev0RPRMTMax),
                     new MySqlParameter("DevHLTV", newClass.DevHLT),
                     new MySqlParameter("DevHLTRV", newClass.DevHLTR),
                     new MySqlParameter("DevHLTMaxV", newClass.DevHLTMax),
                     new MySqlParameter("DevPTV", newClass.DevPT),
                     new MySqlParameter("DevPTRV", newClass.DevPTR),
                     new MySqlParameter("DevPTRTMaxV", newClass.DevPTRTMax),
                     new MySqlParameter("DevNTV", newClass.DevNT),
                     new MySqlParameter("DevNTRV", newClass.DevNTR),
                     new MySqlParameter("DevNTRTMaxV", newClass.DevNTRTMax)};
                isok = MysqlHelper.ModifySingleSql("updateMDevLimit", CommandType.StoredProcedure, paramses, ref errmsg);
                int aid = getAID(type);
                isok = deleteTime(id);
                isok = insertTimeValue(newClass.DevHHTime, aid, id, "DevHHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHHTMaxTime }, aid, id, "DevHHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevHTime, aid, id, "DevHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHTMaxTime }, aid, id, "DevHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRPTime, aid, id, "DevRPTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRPTMaxTime }, aid, id, "DevRPTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0PTime, aid, id, "Dev0PTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0PTMaxTime }, aid, id, "Dev0PTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0NTime, aid, id, "Dev0NTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0NTMaxTime }, aid, id, "Dev0NTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRNTime, aid, id, "DevRNTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRNTMaxTime }, aid, id, "DevRNTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLTime, aid, id, "DevLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLTMaxTime }, aid, id, "DevLTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLLTime, aid, id, "DevLLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLLTMaxTime }, aid, id, "DevLLTMaxTime", "MDevLimit");

                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0RPRMTMaxTime }, aid, id, "Dev0RPRMTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHLTMaxTime }, aid, id, "DevHLTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevPTRTMaxTime }, aid, id, "DevPTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevNTRTMaxTime }, aid, id, "DevNTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(newClass.Dev0RPRMTTime, aid, id, "Dev0RPRMTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevHLTTime, aid, id, "DevHLTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevPTTime, aid, id, "DevPTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevNTTime, aid, id, "DevNTTime", "MDevLimit");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool insertLongMDevLimit(MDevLimitMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("DevHHNV", newClass.DevHHN),
                     new MySqlParameter("DevHHTV", newClass.DevHHT),
                     new MySqlParameter("DevHHRV", newClass.DevHHR),
                     new MySqlParameter("DevHHTMaxV", newClass.DevHHTMax),
                     new MySqlParameter("DevHHAV", newClass.DevHHA),
                     new MySqlParameter("DevHHETV", newClass.DevHHET),
                     new MySqlParameter("DevHNV", newClass.DevHN),
                     new MySqlParameter("DevHTV", newClass.DevHT),
                     new MySqlParameter("DevHRV", newClass.DevHR),
                     new MySqlParameter("DevHTMaxV", newClass.DevHTMax),
                     new MySqlParameter("DevHAV", newClass.DevHA),
                     new MySqlParameter("DevHETV", newClass.DevHET),
                     new MySqlParameter("DevRPNV", newClass.DevRPN),
                     new MySqlParameter("DevRPTV", newClass.DevRPT),
                     new MySqlParameter("DevRPRV", newClass.DevRPR),
                     new MySqlParameter("DevRPTMaxV", newClass.DevRPTMax),
                     new MySqlParameter("DevRPAV", newClass.DevRPA),
                     new MySqlParameter("DevRPETV", newClass.DevRPET),
                     new MySqlParameter("Dev0PNV", newClass.Dev0PN),
                     new MySqlParameter("Dev0PTV", newClass.Dev0PT),
                     new MySqlParameter("Dev0PRV", newClass.Dev0PR),
                     new MySqlParameter("Dev0PTMaxV", newClass.Dev0PTMax),
                     new MySqlParameter("Dev0PAV", newClass.Dev0PA),
                     new MySqlParameter("Dev0PETV", newClass.Dev0PET),
                     new MySqlParameter("Dev0NNV", newClass.Dev0NN),
                     new MySqlParameter("Dev0NTV", newClass.Dev0NT),
                     new MySqlParameter("Dev0NRV", newClass.Dev0NR),
                     new MySqlParameter("Dev0NTMaxV", newClass.Dev0NTMax),
                     new MySqlParameter("Dev0NAV", newClass.Dev0NA),
                     new MySqlParameter("Dev0NETV", newClass.Dev0NET),
                     new MySqlParameter("DevRNNV", newClass.DevRNN),
                     new MySqlParameter("DevRNTV", newClass.DevRNT),
                     new MySqlParameter("DevRNRV", newClass.DevRNR),
                     new MySqlParameter("DevRNTMaxV", newClass.DevRNTMax),
                     new MySqlParameter("DevRNAV", newClass.DevRNA),
                     new MySqlParameter("DevRNETV", newClass.DevRNET),
                     new MySqlParameter("DevLNV", newClass.DevLN),
                     new MySqlParameter("DevLTV", newClass.DevLT),
                     new MySqlParameter("DevLRV", newClass.DevLR),
                     new MySqlParameter("DevLTMaxV", newClass.DevLTMax),
                     new MySqlParameter("DevLAV", newClass.DevLA),
                     new MySqlParameter("DevLETV", newClass.DevLET),
                     new MySqlParameter("DevLLNV", newClass.DevLLN),
                     new MySqlParameter("DevLLTV", newClass.DevLLT),
                     new MySqlParameter("DevLLRV", newClass.DevLLR),
                     new MySqlParameter("DevLLTMaxV", newClass.DevLLTMax),
                     new MySqlParameter("DevLLAV", newClass.DevLLA),
                     new MySqlParameter("DevLLETV", newClass.DevLLET),
                     new MySqlParameter("Dev0HTV", newClass.Dev0HT),
                     new MySqlParameter("Dev0HTRV", newClass.Dev0HTR),
                     new MySqlParameter("Dev0HHTV", newClass.Dev0HHT),
                     new MySqlParameter("Dev0HHTRV", newClass.Dev0HHTR),
                     new MySqlParameter("Dev0LV", newClass.Dev0L),
                     new MySqlParameter("Dev0LRV", newClass.Dev0LR),
                     new MySqlParameter("Dev0LLTV", newClass.Dev0LLT),
                     new MySqlParameter("Dev0LLTRV", newClass.Dev0LLTR),
                     new MySqlParameter("DevHHLLTV", newClass.DevHHLLT),
                     new MySqlParameter("DevHHLLTRV", newClass.DevHHLLTR),
                     new MySqlParameter("DevHLHHLLTV", newClass.DevHLHHLLT),
                     new MySqlParameter("DevHLHHLLRV", newClass.DevHLHHLLR),
                     new MySqlParameter("DevRPRMHLTV", newClass.DevRPRMHLT),
                     new MySqlParameter("DevRPRMHLTRV", newClass.DevRPRMHLTR),
                     new MySqlParameter("Dev0RPRMTV", newClass.Dev0RPRMT),
                     new MySqlParameter("Dev0RPRMTRV", newClass.Dev0RPRMTR),
                     new MySqlParameter("Dev0RPRMTMaxV", newClass.Dev0RPRMTMax),
                     new MySqlParameter("DevHLTV", newClass.DevHLT),
                     new MySqlParameter("DevHLTRV", newClass.DevHLTR),
                     new MySqlParameter("DevHLTMaxV", newClass.DevHLTMax),
                     new MySqlParameter("DevPTV", newClass.DevPT),
                     new MySqlParameter("DevPTRV", newClass.DevPTR),
                     new MySqlParameter("DevPTRTMaxV", newClass.DevPTRTMax),
                     new MySqlParameter("DevNTV", newClass.DevNT),
                     new MySqlParameter("DevNTRV", newClass.DevNTR),
                     new MySqlParameter("DevNTRTMaxV", newClass.DevNTRTMax),
                     new MySqlParameter("typeV", type),
                     new MySqlParameter("yearV", year),
                     new MySqlParameter("monthV", month),
                     new MySqlParameter("dayV", day),
                     new MySqlParameter("hourV", hour)};
                DataTable dt = MysqlHelper.getDataTableOfSQL("insertMDevLimit", CommandType.StoredProcedure, paramses, ref errmsg);
                int pid = Convert.ToInt32(dt.Rows[0]["lastid"].ToString());
                int aid = getAID(type);
                isok = insertTimeValue(newClass.DevHHTime, aid, pid, "DevHHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHHTMaxTime }, aid, pid, "DevHHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevHTime, aid, pid, "DevHTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHTMaxTime }, aid, pid, "DevHTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRPTime, aid, pid, "DevRPTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRPTMaxTime }, aid, pid, "DevRPTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0PTime, aid, pid, "Dev0PTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0PTMaxTime }, aid, pid, "Dev0PTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.Dev0NTime, aid, pid, "Dev0NTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0NTMaxTime }, aid, pid, "Dev0NTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevRNTime, aid, pid, "DevRNTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevRNTMaxTime }, aid, pid, "DevRNTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLTime, aid, pid, "DevLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLTMaxTime }, aid, pid, "DevLTMaxTime", "MDevLimit");

                isok = insertTimeValue(newClass.DevLLTime, aid, pid, "DevLLTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevLLTMaxTime }, aid, pid, "DevLLTMaxTime", "MDevLimit");

                isok = insertTimeValue(new List<D22STimeClass> { newClass.Dev0RPRMTMaxTime }, aid, pid, "Dev0RPRMTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevHLTMaxTime }, aid, pid, "DevHLTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevPTRTMaxTime }, aid, pid, "DevPTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(new List<D22STimeClass> { newClass.DevNTRTMaxTime }, aid, pid, "DevNTRTMaxTime", "MDevLimit");
                isok = insertTimeValue(newClass.Dev0RPRMTTime, aid, pid, "Dev0RPRMTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevHLTTime, aid, pid, "DevHLTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevPTTime, aid, pid, "DevPTTime", "MDevLimit");
                isok = insertTimeValue(newClass.DevNTTime, aid, pid, "DevNTTime", "MDevLimit");
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool insertMDeviationS(MDeviationSOutClass newClass, string type, string year, string month, string day, string hour, int invalidflag)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("spV", newClass.sp.ToString()),
                new MySqlParameter("errV", newClass.err.ToString()),
                new MySqlParameter("errrateV", newClass.errrate.ToString()),
                new MySqlParameter("scoreV", newClass.score.ToString()),
                new MySqlParameter("wscoreV", newClass.wscore.ToString()),
                new MySqlParameter("typeV", type),
                new MySqlParameter("yearV", year),
                new MySqlParameter("monthV", month),
                new MySqlParameter("dayV", day),
                new MySqlParameter("hourV", hour),
                new MySqlParameter("invalidflagV", invalidflag)};
                DataTable dt = MysqlHelper.getDataTableOfSQL("insertMDeviationS", CommandType.StoredProcedure, paramses, ref errmsg);
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool updateMDeviationS(int id, MDeviationSOutClass newClass, string type, string year, string month, string day, string hour, int invalidflag)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid",id),
                new MySqlParameter("spV", newClass.sp.ToString()),
                new MySqlParameter("errV", newClass.err.ToString()),
                new MySqlParameter("errrateV", newClass.errrate.ToString()),
                new MySqlParameter("scoreV", newClass.score.ToString()),
                new MySqlParameter("wscoreV", newClass.wscore.ToString()),
                new MySqlParameter("invalidflagV", invalidflag)};
                isok = Helper.MysqlHelper.ModifySingleSql("updateMDeviationS", CommandType.StoredProcedure, paramses, ref errmsg);
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool insertMDeviation2DS(MDeviationSOutClass newClass, string type, string year, string month, string day, string hour, int invalidflag)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("spV", newClass.sp.ToString()),
                new MySqlParameter("errV", newClass.err.ToString()),
                new MySqlParameter("errrateV", newClass.errrate.ToString()),
                new MySqlParameter("scoreV", newClass.score.ToString()),
                new MySqlParameter("wscoreV", newClass.wscore.ToString()),
                new MySqlParameter("typeV", type),
                new MySqlParameter("yearV", year),
                new MySqlParameter("monthV", month),
                new MySqlParameter("dayV", day),
                new MySqlParameter("hourV", hour),
                new MySqlParameter("invalidflagV", invalidflag)};
                DataTable dt = MysqlHelper.getDataTableOfSQL("insertMDeviation2DS", CommandType.StoredProcedure, paramses, ref errmsg);
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool updateMDeviation2DS(int id, MDeviationSOutClass newClass, string type, string year, string month, string day, string hour, int invalidflag)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid",id),
                new MySqlParameter("spV", newClass.sp.ToString()),
                new MySqlParameter("errV", newClass.err.ToString()),
                new MySqlParameter("errrateV", newClass.errrate.ToString()),
                new MySqlParameter("scoreV", newClass.score.ToString()),
                new MySqlParameter("wscoreV", newClass.wscore.ToString()),
                new MySqlParameter("invalidflagV", invalidflag)};
                isok = Helper.MysqlHelper.ModifySingleSql("updateMDeviation2DS", CommandType.StoredProcedure, paramses, ref errmsg);
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool insertMAddMul(MAddMulOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("addMulSumV", newClass.addMulSum.ToString()),
                new MySqlParameter("addMulAbsSumV", newClass.addMulAbsSum.ToString()),
                new MySqlParameter("addMulMulV", newClass.addMulMul.ToString()),
                new MySqlParameter("addMulAbsMulV", newClass.addMulAbsMul.ToString()),
                new MySqlParameter("addMulAvgV", newClass.addMulAvg.ToString()),
                new MySqlParameter("addMulAbsAvgV", newClass.addMulAbsAvg.ToString()),
                new MySqlParameter("typeV", type),
                new MySqlParameter("yearV", year),
                new MySqlParameter("monthV", month),
                new MySqlParameter("dayV", day),
                new MySqlParameter("hourV", hour)};
                isok = MysqlHelper.ModifySingleSql("insertMAddMul", CommandType.StoredProcedure, paramses, ref errmsg);
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool updateMAddMul(int id, MAddMulOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid",id),
                new MySqlParameter("addMulSumV", newClass.addMulSum.ToString()),
                new MySqlParameter("addMulAbsSumV", newClass.addMulAbsSum.ToString()),
                new MySqlParameter("addMulMulV", newClass.addMulMul.ToString()),
                new MySqlParameter("addMulAbsMulV", newClass.addMulAbsMul.ToString()),
                new MySqlParameter("addMulAvgV", newClass.addMulAvg.ToString()),
                new MySqlParameter("addMulAbsAvgV", newClass.addMulAbsAvg.ToString())};
                isok = Helper.MysqlHelper.ModifySingleSql("updateMAddMul", CommandType.StoredProcedure, paramses, ref errmsg);
                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static bool deleteTime(int pid)
        {
            try
            {
                try
                {
                    bool isok = false;
                    string errmsg = string.Empty;
                    MySqlParameter[] paramses = { new MySqlParameter("pid", pid) };
                    isok = Helper.MysqlHelper.ModifySingleSql("deleteTimeValue", CommandType.StoredProcedure, paramses, ref errmsg);
                    return isok;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static bool updateMPVBasePlusSft(MPVBasePlusSftClass newClass)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("pid",newClass.id),
                new MySqlParameter("PVBMinV", newClass.PVBMin),
                new MySqlParameter("PVBMinTimeV", newClass.PVBMinTime),
                new MySqlParameter("PVBAvgV", newClass.PVBAvg),
                new MySqlParameter("PVBMaxV", newClass.PVBMax),
                new MySqlParameter("PVBMaxTimeV", newClass.PVBMaxTime),
                new MySqlParameter("PVBSumV", newClass.PVBSum),
                new MySqlParameter("PVBSumkbV", newClass.PVBSumkb),
                new MySqlParameter("PVBAbsSumV", newClass.PVBAbsSum),
                new MySqlParameter("PVBStbTRV", newClass.PVBStbTR),
                new MySqlParameter("PVBNoStbTRV", newClass.PVBNoStbTR),
                new MySqlParameter("UpdateTimeV", newClass.UpdateTime),
                new MySqlParameter("EffectiveCountV", newClass.EffectiveCount),
                new MySqlParameter("PVBSDMaxV", newClass.PVBSDMax),
                new MySqlParameter("PVBSDMaxTimeV", newClass.PVBSDMaxTime),
                new MySqlParameter("PVBDN1NumV", newClass.PVBDN1Num),
                new MySqlParameter("PVBDN2NumV", newClass.PVBDN2Num),
                new MySqlParameter("PVBDN3NumV", newClass.PVBDN3Num),
                new MySqlParameter("PVBTNumV", newClass.PVBTNum),
                new MySqlParameter("PVBSDSingleV", newClass.PVBSDSingle),
                new MySqlParameter("PVBSDSingleTimeV", newClass.PVBSDSingleTime),
                new MySqlParameter("PVBSDSingleTypeV", newClass.PVBSDSingleType)};
                isok = Helper.MysqlHelper.ModifySingleSql("updateMPVBasePlusSft", CommandType.StoredProcedure, paramses, ref errmsg);

                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool insertMPVBasePlusSft(MPVBasePlusSftClass newClass)
        {
            try
            {
                string errmsg = string.Empty;
                bool isok = false;
                MySqlParameter[] paramses = {
                new MySqlParameter("PVBMinV", newClass.PVBMin),
                new MySqlParameter("PVBMinTimeV", newClass.PVBMinTime),
                new MySqlParameter("PVBAvgV", newClass.PVBAvg),
                new MySqlParameter("PVBMaxV", newClass.PVBMax),
                new MySqlParameter("PVBMaxTimeV", newClass.PVBMaxTime),
                new MySqlParameter("PVBSumV", newClass.PVBSum),
                new MySqlParameter("PVBSumkbV", newClass.PVBSumkb),
                new MySqlParameter("PVBAbsSumV", newClass.PVBAbsSum),
                new MySqlParameter("PVBStbTRV", newClass.PVBStbTR),
                new MySqlParameter("PVBNoStbTRV", newClass.PVBNoStbTR),
                new MySqlParameter("UpdateTimeV", newClass.UpdateTime),
                new MySqlParameter("EffectiveCountV", newClass.EffectiveCount),
                new MySqlParameter("dutyTimeV", newClass.dutyTime),
                new MySqlParameter("typeV", newClass.type),
                new MySqlParameter("PVBSDMaxV", newClass.PVBSDMax),
                new MySqlParameter("PVBSDMaxTimeV", newClass.PVBSDMaxTime),
                new MySqlParameter("PVBDN1NumV", newClass.PVBDN1Num),
                new MySqlParameter("PVBDN2NumV", newClass.PVBDN2Num),
                new MySqlParameter("PVBDN3NumV", newClass.PVBDN3Num),
                new MySqlParameter("PVBTNumV", newClass.PVBTNum),
                new MySqlParameter("PVBSDSingleV", newClass.PVBSDSingle),
                new MySqlParameter("PVBSDSingleTimeV", newClass.PVBSDSingleTime),
                new MySqlParameter("PVBSDSingleTypeV", newClass.PVBSDSingleType)};
                isok = Helper.MysqlHelper.ModifySingleSql("insertMPVBasePlusSft", CommandType.StoredProcedure, paramses, ref errmsg);

                return isok;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable isHaveData(string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                string errmsg = string.Empty;
                MySqlParameter[] parames = { };
                dt = MysqlHelper.getDataTableOfSQL(sql, CommandType.Text, parames, ref errmsg);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static int getAID(string type)
        {
            try
            {
                return Convert.ToInt32(type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable getData(string sqlStr)
        {
            try
            {
                DataTable dt = new DataTable();
                string errmsg = string.Empty;
                MySqlParameter[] parames = { };
                dt = MysqlHelper.getDataTableOfSQL(sqlStr, CommandType.Text, parames, ref errmsg);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
