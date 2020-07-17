using System;
using System.Collections.Generic;
using System.Text;
using Config;   //使用log
using DBInterface.RDBInterface;         //使用关系数据库接口

namespace PSLCalcu
{
    
    public class PSLScoreWeightConfig
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLModulesDAO));       //全局log

        #region 公有变量
        public static bool ErrorFlag = false;                                                   //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        #endregion

        public static bool WriteOrUpdate(string tagname, string tagdesc, string weighttype, double weightvalue, DateTime validdate)
        { 
            string sqlStr = "";
            string tablename = "pslscoreweight";
            try 
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;select id from {0} where psltagname='{1}' and pslweighttype='{2}' and pslweightvaliddate='{3}'", tablename,tagname, weighttype, validdate.Ticks);
                object obj = dbhelper.ExecuteScalar(sqlStr);
                //如果找到了id就为记录的id号，如果找不到，id为0
                int id = Convert.ToInt32(obj);  //如果obj为null，id=0
                if (id == 0) //如果没有该点，则直接插入数据
                {
                    sqlStr = String.Format("use psldb;insert into {0}(psltagname,psltagdesc,pslweighttype,pslweightvalue,pslweightvaliddate) values ('{1}','{2}','{3}','{4}','{5}')",
                                            tablename, tagname, tagdesc, weighttype, weightvalue, validdate.Ticks);
                    dbhelper.ExecuteNonQuery(sqlStr);
                }
                else        //如果有该点，则更新数据
                {
                    sqlStr = String.Format("use psldb;update {0} set psltagname='{1}',psltagdesc='{2}',pslweighttype='{3}',pslweightvalue='{4}',pslweightvaliddate='{5}' where id='{6}'",
                                            tablename, tagname, tagdesc, weighttype, weightvalue, validdate.Ticks, id);
                    dbhelper.ExecuteNonQuery(sqlStr);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLScoreWeightConfig.WriteOrUpdate()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
    }
}
