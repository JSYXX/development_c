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
        public static DataSet getSftData(string tableName, string tagid, string duty)
        {
            try
            {
                DataSet ds = new DataSet();
                string endtime = string.Empty;
                string sqlstr = "select * from " + tableName + " where `tagId`=" + tagid + " and `dutytime`=" + duty + ";";
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
                string sqlstr = "select * from " + tableName + " where `tagId` in (" + ids + ") and `tagstarttime`=" + Convert.ToDateTime(dutyTime).Ticks + " order by tagId asc;";
                DataTable dt = DAL.AlgorithmDAL.getData(sqlstr);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool deleteOldData(string dutyTime, uint[] foutputpsltagids)
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
                string sqlstr = "delete from " + tableName + " where `tagId` in (" + ids + ") and `tagstarttime`=" + Convert.ToDateTime(dutyTime).Ticks + ";";
                bool isok = DAL.AlgorithmDAL.excuSqlStr(sqlstr);
                return isok;
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
                string sqlStr = "select * from psldb.psldata" + nowDate.ToString("yyyyMM") + " where `tagid`=10001 and `tagstarttime` <= " + nowDate.Ticks.ToString() + " and `tagendtime` >" + nowDate.Ticks.ToString() + ";";
                DataTable dutyTime = DAL.AlgorithmDAL.getData(sqlStr);
                string sqlStr1 = "select * from psldb.psldata" + nowDate.AddMonths(-1).ToString("yyyyMM") + " where `tagid`=10001 and `tagstarttime` <= " + nowDate.Ticks.ToString() + " and `tagendtime` >" + nowDate.Ticks.ToString() + ";";
                DataTable dutyTime1 = DAL.AlgorithmDAL.getData(sqlStr1);
                if (dutyTime1 != null && dutyTime1.Rows.Count > 0)
                {
                    if (dutyTime == null)
                    {
                        dutyTime = new DataTable();
                    }
                    foreach (DataRow item in dutyTime1.Rows)
                    {
                        DataRow dr = dutyTime.NewRow();
                        dr.ItemArray = item.ItemArray;
                        dutyTime.Rows.Add(dr);
                    }
                }
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
        public static string getDutyConst(DateTime nowDate, List<string> dutyTime, ref string dutyEndTime)
        {
            try
            {
                string dutyStr = string.Empty;
                //string sqlStr = "select * from psldb.psl_dutyconst;";
                //DataTable dutyTime = DAL.AlgorithmDAL.getData(sqlStr);
                string dutyNow = nowDate.ToString("HH:mm");
                string dateStr = string.Empty;
                if (DateTime.Compare(nowDate, Convert.ToDateTime(nowDate.ToString("yyyy-MM-dd") + " " + dutyTime[0])) < 0)
                {
                    dateStr = nowDate.AddDays(-1).ToString("yyyy-MM-dd");
                }
                else
                {
                    dateStr = nowDate.ToString("yyyy-MM-dd");
                }
                for (int i = 0; i < dutyTime.Count; i++)
                {
                    string dt1 = string.Empty;
                    string dt2 = string.Empty;
                    if (i < dutyTime.Count - 1)
                    {
                        dt1 = dateStr + " " + dutyTime[i];
                        dt2 = dateStr + " " + dutyTime[i + 1];
                    }
                    else
                    {
                        dt1 = dateStr + " " + dutyTime[i];
                        dt2 = dateStr + " " + dutyTime[0];
                    }
                    DateTime t1 = Convert.ToDateTime(dt1);
                    DateTime t2 = Convert.ToDateTime(dt2);
                    if (DateTime.Compare(t1, t2) >= 0)
                    {
                        t2 = t2.AddDays(1);
                    }
                    if (DateTime.Compare(nowDate, t1) >= 0 && DateTime.Compare(nowDate, t2) < 0)
                    {
                        dutyStr = dt1;
                        dutyEndTime = t2.ToString("yyyy-MM-dd HH:mm");
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
        public static bool insertMDevLimitSft(MDevLimitShtClass newClass, bool isNew)
        {
            try
            {
                bool isok = false;
                if (isNew)
                {
                    isok = DAL.AlgorithmDAL.insertMDevLimitSft(newClass);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.updateMDevLimitSft(newClass);
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

        public static bool insertMReadStatus(List<DoubleTimeListClass> newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                isok = DAL.AlgorithmDAL.insertMReadStatus(newClass, type, year, month, day, hour);
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
        public static bool InsertMultipleRegressionAlgorithm(MultipleRegressionAlgorithmOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psldb.psl_multipleregression");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = DAL.AlgorithmDAL.UpdateMultipleRegressionAlgorithm(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = DAL.AlgorithmDAL.InsertMultipleRegressionAlgorithm(newClass, type, year, month, day, hour);
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
