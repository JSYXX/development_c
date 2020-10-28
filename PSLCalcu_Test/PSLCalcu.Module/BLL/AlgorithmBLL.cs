using PCCommon.NewCaculateCommand;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.BLL
{
    public static class AlgorithmBLL
    {
        public static DataSet getShortData(string tableName, string tagid, string year, string month, string day)
        {
            try
            {
                DataSet dt = getSData(tableName, tagid, year, month, day);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet getSftData(string tableName, string tagid, DateTime nowDate)
        {
            try
            {
                DataSet ds = new DataSet();
                string endtime = string.Empty;
                string dutyStr = getOldDutyConst(nowDate, ref endtime);
                string sqlstr = "select * from " + tableName + " where `tagId`=" + tagid + " and `dutytime`=" + dutyStr + ";";
                DataTable dt = DAL.AlgorithmDAL.getData(sqlstr);
                //string sqlChildStr = "select * from psldata" + nowDate.ToString("yyyyMM") + " where `tagid`=" + tagid + " and tagstarttime<=" + nowDate.ToString("yyyy-MM-dd HH:mm") + " limit 2";
                //DataTable dtTime = DAL.AlgorithmDAL.getData(sqlChildStr);
                ds.Tables.Add(dt);
                //ds.Tables.Add(dtTime);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getDutyTime(DateTime nowDate, ref string EndTime)
        {
            try
            {
                string dutyStr = getOldDutyConst(nowDate, ref EndTime);
                return dutyStr;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static DataTable getMPVBasePlusSftOriOldData(string dutyTime, uint[] foutputpsltagids)
        {
            try
            {
                string tableName = "psldb.psldata" + Convert.ToDateTime(dutyTime).ToString("yyyyMM");
                string ids = string.Empty;
                foreach (uint item in foutputpsltagids)
                {
                    ids += item.ToString() + ",";
                }
                ids = ids.Substring(0, ids.Length - 1);
                string sqlstr = "select * from " + tableName + " where `tagId` in (" + ids + ") and `tagstarttime`=" + Convert.ToDateTime(dutyTime).Ticks + ";";
                DataTable dt = DAL.AlgorithmDAL.getData(sqlstr);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        /// <returns></returns>
        public static bool compareDate(string st1, string st2)
        {
            try
            {
                bool isbig = false;
                if (st1 == "00:00")
                {
                    isbig = true;
                }
                else if (st2 == "00:00")
                {

                }
                else
                {
                    DateTime dt1 = Convert.ToDateTime(st1);
                    DateTime dt2 = Convert.ToDateTime(st2);
                    if (DateTime.Compare(dt1, dt2) >= 0)
                    {
                        isbig = true;
                    }
                    else
                    {
                        isbig = false;
                    }
                }

                return isbig;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getOldDutyConst(DateTime nowDate, ref string endTime)
        {
            try
            {
                //string dutyStr = string.Empty;
                string sqlStr = "select * from psldb.psldata" + nowDate.ToString("yyyyMM") + " where `tagid`=10001 and `tagstarttime` <= " + nowDate.Ticks.ToString() + " and `tagendtime` >=" + nowDate.Ticks.ToString() + ";";
                DataTable dutyTime = DAL.AlgorithmDAL.getData(sqlStr);
                if (dutyTime == null || dutyTime.Rows.Count == 0)
                {
                    throw new Exception("值次信息缺失。");
                }
                string dutyNow = (new DateTime(long.Parse(dutyTime.Rows[0]["tagstarttime"].ToString()))).ToString("yyyy-MM-dd HH:mm");
                endTime = (new DateTime(long.Parse(dutyTime.Rows[0]["tagendtime"].ToString()))).ToString("yyyy-MM-dd HH:mm");
                return dutyNow;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string getDutyConst(DateTime nowDate)
        {
            try
            {
                string dutyStr = string.Empty;
                string sqlStr = "select * from psldb.psl_dutyconst;";
                DataTable dutyTime = DAL.AlgorithmDAL.getData(sqlStr);
                string dutyNow = nowDate.ToString("HH:mm");
                for (int i = 0; i < dutyTime.Rows.Count; i++)
                {
                    string dt1 = nowDate.ToString("yyyy-MM-dd") + " " + dutyTime.Rows[i]["dutyTimeStart"].ToString();
                    string dt2 = nowDate.ToString("yyyy-MM-dd") + " " + dutyTime.Rows[i]["dutyTimeEnd"].ToString();
                    DateTime t1 = Convert.ToDateTime(dt1);
                    DateTime t2 = Convert.ToDateTime(dt2);
                    if (DateTime.Compare(t1, t2) >= 0)
                    {
                        t2 = t2.AddDays(1);
                    }
                    if (DateTime.Compare(nowDate, t1) >= 0 && DateTime.Compare(nowDate, t2) <= 0)
                    {
                        dutyStr = dt1;
                        break;
                    }
                }
                return dutyStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataSet getSData(string tableName, string tagid, string year, string month, string day)
        {
            try
            {
                DataSet ds = new DataSet();
                string sqlstr = "select * from " + tableName + " where `tagId`=" + tagid + " and `yearvalue`=\"" + year + "\"";
                if (!string.IsNullOrWhiteSpace(month))
                {
                    sqlstr += " and `monthvalue`=\"" + month + "\"";
                }
                if (!string.IsNullOrWhiteSpace(day))
                {
                    sqlstr += " and `dayvalue`=\"" + day + "\"";
                }
                sqlstr += ";";
                DataTable dt = DAL.AlgorithmDAL.getData(sqlstr);
                string pidList = string.Empty;
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        pidList += item["id"].ToString() + ",";
                    }
                    pidList = pidList.Substring(0, pidList.Length - 1);
                    string sqlChildStr = "select a.*,(select b.`columnName` from psl_columndata as b where a.AlgorithmId=b.id) as columnName from psl_timedata as a where a.`parentid` in (" + pidList + ")";
                    DataTable dtTime = DAL.AlgorithmDAL.getData(sqlChildStr);
                    ds.Tables.Add(dt);
                    ds.Tables.Add(dtTime);
                }
                return ds;
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
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psldb.psl_mpvbase");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateMPVBase(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertMPVBase(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool insertMPVBasePlusSft(MPVBasePlusSftClass newClass, bool isNew)
        {
            try
            {
                bool isok = false;
                if (isNew)
                {
                    isok = DAL.AlgorithmDAL.insertMPVBasePlusSft(newClass);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.updateMPVBasePlusSft(newClass);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool insertLongMPVBase(MPVBaseMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mpvbase");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateLongMPVBase(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertLongMPVBase(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DataTable isHaveData(string year, string month, string day, string hour, string type, string tableName)
        {
            try
            {
                DataTable dt = new DataTable();
                if (string.IsNullOrWhiteSpace(year))
                {
                    //isok = false;
                }
                else
                {
                    string sqlStr = "select * from psldb." + tableName + " where `tagId`=" + type + " and `yearvalue`=\"" + year + "\"";
                    if (string.IsNullOrWhiteSpace(month))
                    {
                        sqlStr += " and `monthvalue`=\"\" and `dayvalue`=\"\" and `hourvalue`=\"\"";
                    }
                    else
                    {
                        sqlStr += " and `monthvalue`=\"" + month + "\"";
                        if (string.IsNullOrWhiteSpace(day))
                        {
                            sqlStr += " and `dayvalue`=\"\" and `hourvalue`=\"\"";
                        }
                        else
                        {
                            sqlStr += " and `dayvalue`=\"" + day + "\"";
                            if (string.IsNullOrWhiteSpace(hour))
                            {
                                sqlStr += " and `hourvalue`=\"\"";
                            }
                            else
                            {
                                sqlStr += " and `hourvalue`=\"" + hour + "\"";
                            }
                        }
                    }
                    sqlStr += ";";
                    dt = DAL.AlgorithmDAL.isHaveData(sqlStr);
                }
                return dt;
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
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mdevlimit");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateLongMDevLimit(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertLongMDevLimit(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool insertMDevLimit(MDevLimitMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mdevlimit");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateMDevLimit(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertMDevLimit(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool insertMDeviationS(MDeviationSOutClass newClass, string type, string year, string month, string day, string hour, int invalidflag)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mdeviations");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateMDeviationS(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour, invalidflag);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertMDeviationS(newClass, type, year, month, day, hour, invalidflag);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool insertMDeviation2DS(MDeviationSOutClass newClass, string type, string year, string month, string day, string hour, int invalidflag)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mdeviation2ds");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateMDeviation2DS(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour, invalidflag);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertMDeviation2DS(newClass, type, year, month, day, hour, invalidflag);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool insertMAddMul(MAddMulOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_maddmul");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.updateMAddMul(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.insertMAddMul(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
