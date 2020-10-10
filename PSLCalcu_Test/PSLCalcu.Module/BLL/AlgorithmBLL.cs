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
        public static DataSet getSData(string tableName, string tagid, string year, string month, string day)
        {
            try
            {
                DataSet ds = new DataSet();
                string sqlstr = "select * from " + tableName + "where `tagId`=" + tagid + " and `yearvalue`=\"" + year + "\"";
                if (!string.IsNullOrWhiteSpace(month))
                {
                    sqlstr += " and `monthvalue`=\"" + month + "\"";
                }
                if (!string.IsNullOrWhiteSpace(day))
                {
                    sqlstr += " and `day`=\"" + day + "\"";
                }
                sqlstr += ";";
                DataTable dt = DAL.AlgorithmDAL.getData(sqlstr);
                string pidList = string.Empty;
                foreach (DataRow item in dt.Rows)
                {
                    pidList += item["id"].ToString() + ",";
                }
                pidList = pidList.Substring(0, pidList.Length - 1);
                string sqlChildStr = "select a.*,(select b.`columnName` from psl_columndata as b where a.AlgorithmId=b.id) as columnName from psl_timedata as a where a.`parentid` in (" + pidList + ")";
                DataTable dtTime = DAL.AlgorithmDAL.getData(sqlChildStr);
                ds.Tables.Add(dt);
                ds.Tables.Add(dtTime);
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
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mpvbase");
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
                    string sqlStr = "select * from " + tableName + " where `tagId`=" + type + "and `yearvalue`=\"" + year + "\"";
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
