using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Data;
using System.Windows.Forms;             //使用messagebox
using Config;                           //使用log
using System.Text.RegularExpressions;   //使用正则表达式
using System.ComponentModel;            //使用BackgroundWorker
namespace PSLCalcu
{
    public class WebTagNameIdMapDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(WebTagNameIdMapDAO));               //全局log

        #region 公有变量
        public static bool ErrorFlag = false;                                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        public static string ErrorInfo = "";  
        #endregion

        #region 公有方法
        //清空数据
        public static bool ClearData()
        {
            string sqlStr = "";
            //先判断是否存在psldb数据库
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;truncate table webtagnameidmap");
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch
            {
                MessageBox.Show("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                return false;
            }
        }
        //设定id自增的起始位置
        public static bool SetAutoIncrement(int startint)
        {
            string sqlStr = "";
            //先判断是否存在psldb数据库
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;alter table webtagnameidmap  auto_increment={0}", startint);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch
            {
                MessageBox.Show("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                return false;
            }
        }
        //检查别名唯一性
        public static bool CheckUnique(ConfigCSV configcsv)
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //0、将所有标签汇集到psltagname
                Dictionary<string, string> psltagnamesDic = new Dictionary<string, string>();
                Dictionary<string, int> psltagCalcuIndexDic = new Dictionary<string, int>();

                //1、先读取映射表中已有的标签
                sqlStr = String.Format("use psldb;select * from webtagnameidmap ");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLTagNameIdMapItem> psltagitems = IDataReader2PSLTagNameIdMapItem(reader);
                reader.Close();
                Dictionary<string, System.UInt32> psltagmap = new Dictionary<string, System.UInt32>();
                foreach (PSLTagNameIdMapItem psltagitem in psltagitems)
                {
                    psltagnamesDic.Add(psltagitem.psltagname, "数据库计算配置表");
                    psltagCalcuIndexDic.Add(psltagitem.psltagname, psltagitem.pslcalcuconfigindex);
                }//此时，psltagmap包含所有映射表中已经存在的标签

                //2、向psltagnames添加配置的计算结果表标签。计算结果标签放在importdata[i][foutputpsltagprefIndex]中
                int sum = 0;
                int columnfoutputpsltagaliasIndex = configcsv.foutputpsltagaliasIndex;
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] plstagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagaliasIndex], ";|；");    //如果有;|；|，|,则表示多个标签
                    foreach (string psltag in plstagstemp)
                    {
                        if (psltagnamesDic.ContainsKey(psltag))
                        {
                            sum += 1;
                            string messageStr = String.Format("第{0}行的计算结果标签{1}，与{2}中的第{3}行计算的计算结果别名重名，请检查！", i - configcsv.firstDataRow + 1, psltag, psltagnamesDic[psltag], psltagCalcuIndexDic[psltag]);
                            logHelper.Error(messageStr);
                        }
                        else
                        {
                            psltagnamesDic.Add(psltag.Trim(), "CSV计算配置表");
                            psltagCalcuIndexDic.Add(psltag.Trim(), i + 1);
                        }
                    }
                }//此时，psltagnames包含了所有要添加的标签别名

                //3、检查结果
                if (sum == 0)
                {
                    return true;
                }
                else
                {
                    string messageStr = String.Format("要添加的计算结果标签别名与已经存在的计算结果标签别名重名，请检查log文件！");
                    MessageBox.Show(messageStr);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层WebTagNameIdMapDAO.CheckUnique()错误：检查计算结果标签别名在标签id映射表中是否已经存在时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }

        public static int MAX_NUMBER_CONSTTAG=APPConfig.rdbtable_constmaxnumber;
        public static BackgroundWorker worker;      //给generateMap，用于更新进度条界面
        //抽取概化标签别名和id映射表：reassign=1为重新建立标签序号映射；reassign=0为添加标签 
        public static bool generateMap(ConfigCSV configcsv, bool reassign)
        {
            //用配置文件中的psltagnames字段的最后一段（以“.”分段，如果是pgim，则是以“^”分段）填写的名称作为计算结果的标签名
            //该接口在读取概化计算配置表后，紧跟着执行
            //1、reassign为true，则全部重新分配id
            //2、reassing为false，则仅向后添加
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //reassign=1，则全部重新分配id.
                //reassign="0",则不重新对于没添加的tag，向后使用新id

                //根据psltagname生成映射表
                //去掉名称重复的标签名:在ConfigCSV.checkPSLNameUnique()中已经检查过计算结果标签唯一性。这里可以保证计算结果标签都是唯一的。                
                //string[] trimpslnames = psltagnames.Distinct().ToArray();


                //1、如果reassign=1、清空表数据
                if (reassign)
                {
                    sqlStr = String.Format("use psldb;delete from webtagnameidmap;truncate table webtagnameidmap;alter table webtagnameidmap  auto_increment={0}", MAX_NUMBER_CONSTTAG);
                    dbhelper.ExecuteNonQuery(sqlStr);
                }

                //2、将所有标签汇集到psltagname,将所有计算结果描述汇集到psltagdesc
                List<string> consttagnames = new List<string>();                                //常数标签名
                Dictionary<string, string> consttagdesc = new Dictionary<string, string>();     //常数标签描述字典
                Dictionary<string, int> consttagcalcuindex = new Dictionary<string, int>();     //常数标签计算项序号字典
                Dictionary<string, string> consttaggroup = new Dictionary<string, string>();    //常数标签组名字典
                Dictionary<string, int> consttagorder = new Dictionary<string, int>();          //常数标签组序号字典

                List<string> psltagnames = new List<string>();                                  //概化标签名
                Dictionary<string, string> psltagdesc = new Dictionary<string, string>();       //概化标签名描述字典
                Dictionary<string, int> psltagcalcuindex = new Dictionary<string, int>();       //概化标签名计算项序号字典
                Dictionary<string, string> psltaggroup = new Dictionary<string, string>();    //常数标签组名字典
                Dictionary<string, int> psltagorder = new Dictionary<string, int>();          //常数标签组序号字典


                //2.1、先向psltagnames添加特定标签
                if (reassign)
                {

                    psltagnames.Add("CurrentSpan");  //tagid=100，tagname=CurrentSpan，表示当前计算周期，用于时间逻辑表达式要找当前周期时。
                    psltagdesc.Add("CurrentSpan", "当前计算时间段");
                    psltagcalcuindex.Add("CurrentSpan", 0);
                    psltaggroup.Add("CurrentSpan", "");
                    psltagorder.Add("CurrentSpan", 0);

                }

                //2.2、再向consttagnames、psltagnames添加配置的计算结果表标签。计算结果标签放在importdata[i][foutputpsltagprefIndex]中
                int columncalcuitemindex = configcsv.calcuitemindex;                        //计算项序号
                int columnfoutputpsltagaliasIndex = configcsv.foutputpsltagaliasIndex;      //标签别名，注意web端使用别名项
                int columnfoutputpsltagdescIndex = configcsv.foutputpsltagdescIndex;        //标签描述
                int columnsouredbIndex = configcsv.sourcetagdbIndex;                        //标签源类型
                int columnfmodulename = configcsv.fmodulenameIndex;                         //计算项名称
                int columnforderIndex = configcsv.forderIndex;
                int columnfgroupIndex = configcsv.fgroupIndex;

                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    //统计非常数标签
                    if (configcsv.importdata[i][columnfmodulename].Trim() != "MReadConst")
                    {
                        string[] psltagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagaliasIndex], ";|；");       //如果有;|；|，|,则表示多个标签
                        string[] psltagsdesctemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagdescIndex], ";|；");   //如果有;|；|，|,则表示多个标签
                        
                        for (int j = 0; j < psltagstemp.Length; j++)
                        {
                            //描述如果数量与输入不一致，不影响程序运行，会得到空描述
                            psltagnames.Add(psltagstemp[j].Trim().ToUpper());
                            try
                            {
                                psltagcalcuindex.Add(psltagstemp[j].Trim().ToUpper(), int.Parse(configcsv.importdata[i][columncalcuitemindex]));   //csv的第三行数据，对应这里i=2，实际应该是第一行数据。所以index为i-1
                                if (j < psltagsdesctemp.Length)                                 //描述的数量如果和标签数据不符，则给描述填写空值。
                                    psltagdesc.Add(psltagstemp[j].Trim().ToUpper(), psltagsdesctemp[j].Trim());
                                else
                                    psltagdesc.Add(psltagstemp[j].Trim().ToUpper(), "");                                

                            }
                            catch
                            {
                                psltagcalcuindex.Add(psltagstemp[j].Trim().ToUpper(), int.Parse(configcsv.importdata[i][columncalcuitemindex]));   //csv的第三行数据，对应这里i=2，实际应该是第一行数据。所以index为i-1
                                psltagdesc.Add(psltagstemp[j].Trim().ToUpper(), "");

                            }
                            psltaggroup.Add(psltagstemp[j].Trim().ToUpper(), configcsv.importdata[i][columnfgroupIndex]);
                            psltagorder.Add(psltagstemp[j].Trim().ToUpper(), int.Parse(configcsv.importdata[i][columnforderIndex]));
                        }
                    }
                    else
                    {
                       //统计常数标签
                        string[] consttagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagaliasIndex], ";|；");       //如果有;|；|，|,则表示多个标签
                        string[] consttagdesctemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagdescIndex], ";|；");   //如果有;|；|，|,则表示多个标签
                        for (int j = 0; j < consttagstemp.Length; j++)
                        {
                            //描述如果数量与输入不一致，不影响程序运行，会得到空描述
                            consttagnames.Add(consttagstemp[j].Trim().ToUpper());
                            try
                            {
                                consttagcalcuindex.Add(consttagstemp[j].Trim().ToUpper(), int.Parse(configcsv.importdata[i][columncalcuitemindex]));   //csv的第三行数据，对应这里i=2，实际应该是第一行数据。所以index为i-1
                                if (j < consttagdesctemp.Length)                                 //描述的数量如果和标签数据不符，则给描述填写空值。
                                    consttagdesc.Add(consttagstemp[j].Trim().ToUpper(), consttagdesctemp[j].Trim());
                                else
                                    consttagdesc.Add(consttagstemp[j].Trim().ToUpper(), "");

                            }
                            catch
                            {
                                consttagcalcuindex.Add(consttagstemp[j].Trim().ToUpper(), int.Parse(configcsv.importdata[i][columncalcuitemindex]));   //csv的第三行数据，对应这里i=2，实际应该是第一行数据。所以index为i-1
                                consttagdesc.Add(consttagstemp[j].Trim().ToUpper(), "");

                            }
                            consttaggroup.Add(consttagstemp[j].Trim().ToUpper(), configcsv.importdata[i][columnfgroupIndex]);
                            consttagorder.Add(consttagstemp[j].Trim().ToUpper(), int.Parse(configcsv.importdata[i][columnforderIndex]));
                        }
                    }
                }

                //3、重新分配id
                int writecount;
                string values;
                //分配常数标签别名，常数标签使用10000以下的id号，在检查常数配置标签的时候，要检查总数量不能大于10000
                //只有在重置标签的情况下，才写常数标签
                if (reassign)
                {
                    writecount = 0;
                    values = "";
                    int pointid4const = 1;

                    foreach (string contag in consttagnames)
                    {
                        writecount = writecount + 1;
                        values = values + String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}'),", pointid4const, contag, consttagcalcuindex[contag], consttagdesc[contag], consttaggroup[contag], consttagorder[contag], "");    //组织字段的实际值字符串。
                        pointid4const++;
                        if (writecount % 100 == 0 || contag == consttagnames[consttagnames.Count - 1])//凑够100个标签，写入一次；或直到最后一个标签才写入一次。
                        {
                            //刷新ui                           
                            worker.ReportProgress(writecount * 100 / consttagnames.Count, String.Format("建立计算引擎内部常数标签名称和id映射，共{0}条，已导入第{1}条！", consttagnames.Count, writecount));
                            //批量写入数据
                            values = values.Substring(0, values.Length - 1);
                            sqlStr = String.Format("use psldb;insert into webtagnameidmap (psltagid,psltagname,pslcalcuconfigindex,psltagdesc,psltaggroup,psltagorder,outputtablename) values {0}", values);
                            dbhelper.ExecuteNonQuery(sqlStr);
                            values = "";

                        }
                    }
                }
                //分配概化标签别名
                writecount = 0;
                values = "";
                foreach (string psltag in psltagnames)
                {
                    writecount = writecount + 1;
                    values = values + String.Format("('{0}','{1}','{2}','{3}','{4}','{5}'),", psltag, psltagcalcuindex[psltag], psltagdesc[psltag], psltaggroup[psltag], psltagorder[psltag], "");    //组织字段的实际值字符串。
                    if (writecount % 100 == 0 || psltag == psltagnames[psltagnames.Count - 1])//凑够100个标签，写入一次；或直到最后一个标签才写入一次。
                    {
                        //刷新ui                           
                        worker.ReportProgress(writecount * 100 / psltagnames.Count, String.Format("建立web端概化标签别名和id映射，共{0}条，已导入第{1}条！", psltagnames.Count, writecount));
                        //批量写入数据
                        values = values.Substring(0, values.Length - 1);
                        sqlStr = String.Format("use psldb;insert into webtagnameidmap (psltagname,pslcalcuconfigindex,psltagdesc,psltaggroup,psltagorder,outputtablename) values {0}", values);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        values = "";

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层WebTagNameIdMapDAO.generateMap()错误：抽取计算标签别名id映射表时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //更新概化标签名称和id映射表中的中文描述
        public static bool UpdateDesc(ConfigCSV configcsv, bool reassign)
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //1、将所有标签汇集到psltagname,将所有计算结果描述汇集到psltagdesc
                List<string> psltagnames = new List<string>();
                Dictionary<string, string> psltagdesc = new Dictionary<string, string>();

                //2、再向psltagnames添加配置的计算结果表标签。计算结果标签放在importdata[i][foutputpsltagprefIndex]中
                int columnfoutputpsltagaliasIndex = configcsv.foutputpsltagaliasIndex;
                int columnfoutputpsltagdescIndex = configcsv.foutputpsltagdescIndex;
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] psltagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagaliasIndex], ";|；");       //如果有;|；|，|,则表示多个标签
                    string[] psltagsdesctemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagdescIndex], ";|；");   //如果有;|；|，|,则表示多个标签
                    for (int j = 0; j < psltagstemp.Length; j++)
                    {
                        //描述如果数量与输入不一致，不影响程序运行，会得到空描述
                        psltagnames.Add(psltagstemp[j].Trim().ToUpper());
                        try
                        {
                            psltagdesc.Add(psltagstemp[j].Trim().ToUpper(), psltagsdesctemp[j].Trim());
                        }
                        catch
                        {
                            psltagdesc.Add(psltagstemp[j].Trim().ToUpper(), "");
                        }
                    }
                }
                //4、读取Map
                Dictionary<string, System.UInt32> tagMap = new Dictionary<string, System.UInt32>();
                tagMap = WebTagNameIdMapDAO.ReadMap();

                //3、更新desc
                int writecount = 0;
                string valuesStr = "";
                string idStr = "";
                foreach (string psltag in psltagnames)
                {
                    writecount = writecount + 1;
                    valuesStr = valuesStr + String.Format(" when {0} then '{1}'", tagMap[psltag], psltagdesc[psltag]);    //组织字段的实际值字符串。
                    idStr = idStr + "," + tagMap[psltag].ToString();

                    if (writecount % 100 == 0 || psltag == psltagnames[psltagnames.Count - 1])//凑够100个标签，写入一次；或直到最后一个标签才写入一次。
                    {
                        idStr = "(" + idStr.Substring(1, idStr.Length - 1) + ")";

                        //批量写入数据                       
                        sqlStr = String.Format("use psldb;update webtagnameidmap set psltagdesc = case psltagid {0} end where psltagid in {1}", valuesStr, idStr);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        //刷新ui                           
                        worker.ReportProgress(writecount * 100 / psltagnames.Count, String.Format("更新标签别名中文描述，共{0}条，已导入第{1}条！", psltagnames.Count, writecount));
                        //重置
                        valuesStr = "";
                        idStr = "";
                    }

                }
                return true;


            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.generateMap()错误：抽取计算标签名称id映射表时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //更新概化标签名称和id映射表中的标签名称：前提id不变
        public static bool UpdateTagname(string[] tagids, string[] tagnames)
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                string valuesStr = "";
                for (int i = 0; i < tagids.Length; i++)
                {
                    valuesStr = valuesStr + String.Format(" when {0} then '{1}'", tagids[i], tagnames[i]);    //组织字段的实际值字符串。
                }
                string psltagidStr = String.Join(",", tagids);
                /*语句实例
                use psldb;
                update psltagnameidmap 
                set psltagname = case psltagid 
                when "10143" then "030100CONDBACKP_000000_MANGAVG_1MIN"
                when "10144" then "030100CONDBACKP_000000_MANGAVG_1MIN"
                end
                where psltagid in(10143,10144)
                 * 
                */

                //批量写入数据                       
                sqlStr = String.Format("use psldb;update webtagnameidmap set psltagname = case psltagid {0} end where psltagid in ({1})", valuesStr, psltagidStr);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.generateMap()错误：抽取计算标签名称id映射表时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //读取概化标签名称和id映射表
        public static Dictionary<string, System.UInt32> ReadMap()
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;select * from psltagnameidmap ");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLTagNameIdMapItem> psltagitems = IDataReader2PSLTagNameIdMapItem(reader);
                reader.Close();

                Dictionary<string, System.UInt32> psltagmap = new Dictionary<string, System.UInt32>();
                foreach (PSLTagNameIdMapItem psltagitem in psltagitems)
                {
                    psltagmap.Add(psltagitem.psltagname, psltagitem.psltagid);
                }

                return psltagmap;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadMap()错误：读取计算标签名称id映射表时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        //删除概化标签
        public static bool DeleteTags(uint[] tagids)
        {
            /*
                Delete * from tablename where tagid in('value1','value2'...)
            */
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //获取要删除的字符串id
                string psltagidStr = "";
                for (int i = 0; i < tagids.Length; i++)
                {
                    psltagidStr = psltagidStr + "'" + tagids[i].ToString() + "'" + ",";
                }
                psltagidStr = psltagidStr.Substring(0, psltagidStr.Length - 1);

                //批量删除数据                       
                sqlStr = String.Format("use psldb;delete from webtagnameidmap where psltagid in ({0})", psltagidStr);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层WebTagNameIdMapDAO.DeleteTags()错误：删除计算标签名称id映射表时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
       
        #endregion

        #region 辅助函数
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集
        private static List<PSLTagNameIdMapItem> IDataReader2PSLTagNameIdMapItem(IDataReader reader)
        {
            List<PSLTagNameIdMapItem> items = new List<PSLTagNameIdMapItem>();
            while (reader.Read())
            {
                PSLTagNameIdMapItem item = new PSLTagNameIdMapItem();
                DAOSupport.ReaderToObject(reader, item);
                items.Add(item);
            }
            return items;
        }
        #endregion
    }
}
