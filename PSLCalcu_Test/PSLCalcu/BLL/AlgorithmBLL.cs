using PSLCalcu.AlgorithmModel;
using PSLCalcu.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.BLL
{
    public class AlgorithmBLL
    {
        AlgorithmDAL a_dal = new AlgorithmDAL();
        public bool insertMPVBase(MPVBaseMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mpvbase");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = a_dal.updateMPVBase(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = a_dal.insertMPVBase(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool insertLongMPVBase(MPVBaseMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mpvbase");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = a_dal.updateLongMPVBase(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = a_dal.insertLongMPVBase(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataTable isHaveData(string year, string month, string day, string hour, string type, string tableName)
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
                    string sqlStr = "select * from " + tableName + " where `tagId`=" + type + " `yearvalue`=\"" + year + "\"";
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
                    dt = a_dal.isHaveData(sqlStr);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool insertLongMDevLimit(MDevLimitMessageOutClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mdevlimit");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = a_dal.updateLongMDevLimit(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = a_dal.insertLongMDevLimit(newClass, type, year, month, day, hour);
                }
                return isok;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool insertMDevLimit(MDevLimitMessageOutBadClass newClass, string type, string year, string month, string day, string hour)
        {
            try
            {
                bool isok = false;
                DataTable dt = isHaveData(year, month, day, hour, type, "psl_mdevlimit");
                if (dt != null && dt.Rows.Count > 0)
                {
                    isok = a_dal.updateMDevLimit(Convert.ToInt32(dt.Rows[0]["id"].ToString()), newClass, type, year, month, day, hour);
                }
                else
                {
                    isok = a_dal.insertMDevLimit(newClass, type, year, month, day, hour);
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
