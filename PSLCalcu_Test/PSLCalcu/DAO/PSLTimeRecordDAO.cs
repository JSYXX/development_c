using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBInterface.RDBInterface;         //使用关系数据库接口
using Config;   //使用log
using System.Data;                      //使用IDataReader

namespace PSLCalcu
{
    public class PSLTimeRecordDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLTimeRecordDAO));       //全局log
        public static bool Write(List<TimeRecord> timerecords) 
        {
            string sqlStr = "";            
            int i = 0;
            double readspan = 0;
            double relectionspan = 0;
            double filterspan = 0;
            double calcuspan = 0;
            double writespan = 0;
            double updatespan = 0;
            double totalspan = 0;
            try
            {
                DbHelper dbhelper = new DbHelper();
                foreach ( TimeRecord timerecord in timerecords)
                {
                    if ((timerecord.EndCurrent - timerecord.BeforeReadData).TotalSeconds > 0)
                    {   //当前记录项必须最后一个时刻大于第一个时刻，才说明该次计算正确的走完
                        readspan = (int)(timerecord.BeforeFilter - timerecord.BeforeReadData).TotalMilliseconds;            //读取源数据
                        filterspan = (int)(timerecord.BeforeReflection - timerecord.BeforeFilter).TotalMilliseconds;        //读取条件数据并过滤
                        relectionspan = (int)(timerecord.BeforeCalcu - timerecord.BeforeReflection).TotalMilliseconds;      //反射
                        calcuspan = (int)(timerecord.BeforeWriteData - timerecord.BeforeCalcu).TotalMilliseconds;           //计算
                        writespan = (int)(timerecord.BeforeUpdateCalcuInfo - timerecord.BeforeWriteData).TotalMilliseconds; //写结果
                        updatespan = (int)(timerecord.EndCurrent - timerecord.BeforeUpdateCalcuInfo).TotalMilliseconds;     //更新界面
                        totalspan = (int)(timerecord.EndCurrent - timerecord.BeforeReadData).TotalMilliseconds;             //总时间
                    }

                    string fileds = "modulename,beforereaddata,beforefilter,beforereflection,beforecalcu,beforewritedata,beforeupdatecalcuInfo,endcurrent," +
                                    "beforereaddatams,beforefilterms,beforereflectionms,beforecalcums,beforewritedatams,beforeupdatecalcuInfoms,endcurrentms," +
                                    "readspan,filterspan,reflectionspan,calcuspan,writespan,updatespan,totalspan";
                    sqlStr = String.Format("use psldb;insert into psltimerecord({0}) values ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')", fileds,
                        timerecord.ModuleName, timerecord.BeforeReadData, timerecord.BeforeFilter, timerecord.BeforeReflection, timerecord.BeforeCalcu, timerecord.BeforeWriteData, timerecord.BeforeUpdateCalcuInfo, timerecord.EndCurrent,
                        timerecord.BeforeReadData.Millisecond, timerecord.BeforeFilter.Millisecond, timerecord.BeforeReflection.Millisecond,timerecord.BeforeCalcu.Millisecond, timerecord.BeforeWriteData.Millisecond, timerecord.BeforeUpdateCalcuInfo.Millisecond, timerecord.EndCurrent.Millisecond,
                        readspan, filterspan, relectionspan, calcuspan, writespan, updatespan, totalspan);
                    dbhelper.ExecuteNonQuery(sqlStr);
                    i++;
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("DAO层错误：------------------------------------------------------------------") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——PSLTimeRecordDAO.Write()错误！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                logHelper.Error(ex.ToString());
                return false;
            }
        }
        public static  string[][] Read() 
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                string fields =  "psltimerecord.modulename,"
                                 + "psltimerecord.beforereaddata,"
                                 + "psltimerecord.beforereaddatams,"
                                 + "psltimerecord.beforereflection,"
                                 + "psltimerecord.beforereflectionms,"
                                 + "psltimerecord.beforefilter,"
                                 + "psltimerecord.beforefilterms,"
                                 + "psltimerecord.beforecalcu,"
                                 + "psltimerecord.beforecalcums,"
                                 + "psltimerecord.beforewritedata,"
                                 + "psltimerecord.beforewritedatams,"
                                 + "psltimerecord.beforeupdatecalcuInfo,"
                                 + "psltimerecord.beforeupdatecalcuInfoms,"
                                 + "psltimerecord.endcurrent,"
                                 + "psltimerecord.endcurrentms,"
                                 + "psltimerecord.readspan,"
                                 + "psltimerecord.reflectionspan,"
                                 + "psltimerecord.filterspan,"
                                 + "psltimerecord.calcuspan,"
                                 + "psltimerecord.writespan,"
                                 + "psltimerecord.updatespan,"
                                 + "psltimerecord.totalspan"
                                 ;

                sqlStr = String.Format("use psldb;select {0} from psltimerecord ", fields);
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                string[][] results = IDataReader2string(reader);
                reader.Close();

                return results;
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("DAO层错误：------------------------------------------------------------------") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——PSLModulesDAO.ReadData()错误！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                logHelper.Error(ex.ToString());
                return null;
            }
        }

        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集
         private static string[][] IDataReader2string(IDataReader reader)
        {
            List<PSLTimeRecord> items = new List<PSLTimeRecord>();
            while (reader.Read())
            {
                PSLTimeRecord item = new PSLTimeRecord();
                DAOSupport.ReaderToObject(reader, item);
                items.Add(item);
            }

            string[][] results=new string[items.Count+1][];
            results[0] = new string[22] { "算法名称", "起始时间", "起始时间ms", "反射前", "反射前ms", "过滤前", "过滤前ms", "计算前", "计算前ms", "写结果", "写结果前", "更新信息前", "更新信息前ms", "结束时间", "结束时间ms", "读时间", "反射时间", "过滤时间", "计算时间", "写结果时间", "更新界面时间", "总时间" };
            for(int i=0;i<items.Count;i++)
            {
                results[i + 1]=new string[22];
                results[i + 1][0]=  items[i].modulename;
                results[i + 1][1] = items[i].beforereaddata.ToString();
                results[i + 1][2] = items[i].beforereaddatams.ToString();
                results[i + 1][3] = items[i].beforereflection.ToString();
                results[i + 1][4] = items[i].beforereflectionms.ToString();
                results[i + 1][5] = items[i].beforefilter.ToString();
                results[i + 1][6] = items[i].beforefilterms.ToString();
                results[i + 1][7] = items[i].beforecalcu.ToString();
                results[i + 1][8] = items[i].beforecalcums.ToString();
                results[i + 1][9] = items[i].beforewritedata.ToString();
                results[i + 1][10] = items[i].beforewritedatams.ToString();
                results[i + 1][11] = items[i].beforeupdatecalcuInfo.ToString();
                results[i + 1][12] = items[i].beforeupdatecalcuInfoms.ToString();
                results[i + 1][13] = items[i].endcurrent.ToString();
                results[i + 1][14] = items[i].endcurrentms.ToString();
                results[i + 1][15] = items[i].readspan.ToString();
                results[i + 1][16] = items[i].reflectionspan.ToString();
                results[i + 1][17] = items[i].filterspan.ToString();
                results[i + 1][18] = items[i].calcuspan.ToString();
                results[i + 1][19] = items[i].writespan.ToString();
                results[i + 1][20] = items[i].updatespan.ToString();
                results[i + 1][21] = items[i].totalspan.ToString();                
            }
            
            return results;
            
        }
    }

}
