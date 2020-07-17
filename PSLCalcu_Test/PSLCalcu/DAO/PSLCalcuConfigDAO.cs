using System;
using System.Collections.Generic;
//using System.Threading;
using System.Linq;                      //使用linq进行查询
using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Data;                      //使用IDataReader
using System.Windows.Forms;             //使用messagebox
using Config;                           //使用配置模块
using System.ComponentModel;            //使用BackgroundWorker
namespace PSLCalcu
{
    /// <summary>
    /// PSLCalcuConfigDAO
    /// 1、负责概化计算组态信息表pslcalcuconfig的读写
    /// 2、为了便于程序在pslcalcuitems和listview之间操作的便捷，尤其是为减小计算引擎主循环的开销，因避免通过遍历的方式查找双方对象对应关系。
    /// 3、为了达到第2条的目的，本程序以pslcalcuitems的fid作为与listview对应关系的依据。
    ///     ——在pslcalcuconfig表中，fid必须是从1开始的连续整数。
    ///     ——在数据表层面，由fid字段的主键、自增属性保证。
    ///     ——在数据表的操作层面，要保证仅对表进行整体读入和删除操作，而不做某一条记录的删除操作。
    ///     ——在数据使用方面，读取数据表后，首先要对fid进行检查，以保证fid是从1开始的连续整数。
    ///     ——在用配置信息生成pslcalcuitems前，要对fid进行排序，一保证fid号，与生成的listview的item序号对应。
    /// </summary>
    public class PSLCalcuConfigDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLCalcuConfigDAO));       //全局log

        #region 公有变量
        public static bool ErrorFlag = false;                                                       //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        #endregion

        #region 公有方法
        //清空pslconfig数据
        public static bool ClearData()
        {
            DbHelper dbhelper = new DbHelper();
            string sqlStr = String.Format("use psldb;truncate table pslcalcuconfig");
            dbhelper.ExecuteNonQuery(sqlStr);
            return true;
        }
        //导入计算配置信息(将计算配置信息由configcsv导入到数据库)        
        public static BackgroundWorker worker;
        public static bool ImportFromCSV(ConfigCSV configcsv)   //由于要使用wo
        {
            ErrorFlag = false; 
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                if (configcsv.importdata.Length != 0)
                {
                    string values = "";                   

                    for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                    {
                        //刷新ui
                        //worker.ReportProgress(i * 100 / configcsv.importdata.Length, String.Format("导入计算配置信息，共{0}条，导入第{1}条！", configcsv.importdata.Length,i));
                        //源标签信息
                        string sourcetagname = configcsv.importdata[i][configcsv.sourcetagnameIndex].Trim();
                        string sourcetagdb = configcsv.importdata[i][configcsv.sourcetagdbIndex].Trim();
                        string sourcetagdesc = configcsv.importdata[i][configcsv.sourcetagdescIndex].Trim();
                        string sourcetagdim = configcsv.importdata[i][configcsv.sourcetagdimIndex].Trim();
                        string sourcetagmrb = configcsv.importdata[i][configcsv.sourcetagmrbIndex].Trim();
                        string sourcetagmre = configcsv.importdata[i][configcsv.sourcetagmreIndex].Trim();
                        //计算公式信息    1、名称
                        string fmodulename = configcsv.importdata[i][configcsv.fmodulenameIndex].Trim();
                        string fnode = configcsv.importdata[i][configcsv.fnodeIndex].Trim();
                        string fgroup = configcsv.importdata[i][configcsv.fgroupIndex].Trim();
                        string forder = configcsv.importdata[i][configcsv.forderIndex].Trim();
                        //计算公式信息    2、算法              
                        string falgorithmsflag = configcsv.importdata[i][configcsv.falgorithmsflagIndex].Trim();
                        string fparas = configcsv.importdata[i][configcsv.fparasIndex].Trim();
                        string fcondpslnames = configcsv.importdata[i][configcsv.fcondpslnamesIndex].Trim();
                        string fcondexpression = configcsv.importdata[i][configcsv.fcondexpressionIndex].Trim();
                        //计算公式信息    3、结果
                        string foutputtable = configcsv.importdata[i][configcsv.foutputtableIndex].Trim();
                        string foutputnumber = configcsv.importdata[i][configcsv.foutputnumberIndex].Trim();
                        string foutputpsltagnames = configcsv.importdata[i][configcsv.foutputpsltagprefIndex].Trim();
                       
                        //计算公式信息    4、周期
                        string finterval = configcsv.importdata[i][configcsv.fintervalIndex].Trim();
                        string fintervaltype = configcsv.importdata[i][configcsv.fintervaltypeIndex].Trim();
                        //计算延时信息
                        string fdelay = configcsv.importdata[i][configcsv.fdelayIndex].Trim();
                        //计算起始时间
                        string fstartdate = configcsv.importdata[i][configcsv.fstartdateIndex].Trim();

                        values = values + String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}'),",
                                 sourcetagname, sourcetagdb, sourcetagdesc, sourcetagdim, sourcetagmrb, sourcetagmre,    //源标签信息
                                 fmodulename, fnode, fgroup, forder,                                                    //计算公式信息    1、名称
                                 falgorithmsflag, fparas, fcondpslnames, fcondexpression,                               //计算公式信息    2、算法
                                 foutputtable, foutputnumber, foutputpsltagnames,                                       //计算公式信息    3、结果
                                 finterval, fintervaltype, fdelay,fstartdate);                                          //计算公式信息    4、循环

                        //一次写入一百条记录，加快写入速度
                        if (i % 100 == 0 || i == configcsv.importdata.Length - 1)            //i到整除10时，或到最后一行时，进行写入
                        {
                            //刷新ui
                            worker.ReportProgress(i * 100 / configcsv.importdata.Length, String.Format("导入计算配置信息，共{0}条，导入第{1}条！", configcsv.importdata.Length, i));
                            //数据库字段
                            string fileds = "sourcetagname,sourcetagdb,sourcetagdesc,sourcetagdim,sourcetagmrb,sourcetagmre," +
                                             "fmodulename,fnode,fgroup,forder," +
                                             "falgorithmsflag,fparas,fcondpslnames,fcondexpression," +
                                             "foutputtable,foutputnumber,foutputpsltagnames," +
                                             "finterval,fintervaltype,fdelay,fstarttime";
                            values = values.Substring(0, values.Length - 1);
                            sqlStr = String.Format("use psldb;insert into pslcalcuconfig({0}) values {1}", fileds, values);
                            dbhelper.ExecuteNonQuery(sqlStr);
                            //复位
                            values = "";
                        }
                        /*
                        //数据库字段
                        string fileds = "sourcetagname,sourcetagdb,sourcetagdesc,sourcetagdim,sourcetagmrb,sourcetagmre," +
                                         "fmodulename,fgroup,forder," +
                                         "falgorithmsflag,fparas,fcondpslnames,fcondexpression," +
                                         "foutputtable,foutputnumber,foutputpsltagnames," +
                                         "finterval,fintervaltype";

                        //概化计算引擎关系库默认database是psldb时，可以不用use psldb
                        //概化计算引擎关系库默认database不是psldb时，必须使用use psldb
                        //下面语句，fileds是数据库对应字段。，右侧是上面变量取出的csv数值
                        sqlStr = String.Format("use psldb;insert into pslcalcuconfig({0}) values ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
                            fileds,     //数据库字段
                            sourcetagname, sourcetagdb, sourcetagdesc, sourcetagdim, sourcetagmrb, sourcetagmre,    //源标签信息
                            fmodulename, fgroup, forder,                                //计算公式信息    1、名称
                            falgorithmsflag, fparas, fcondpslnames, fcondexpression,     //计算公式信息    2、算法
                            foutputtable, foutputnumber, foutputpsltagnames,             //计算公式信息    3、结果
                            finterval, fintervaltype);                                  //计算公式信息    4、循环

                        DbHelper.ExecuteNonQuery(sqlStr);
                        */

                    }//end for
                    //}//end if
                }//end if
                return true;

            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.ImportFromCSV()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //用csv更新pslconfig表的一般信息：根据唯一的fid进行更新
        public static bool UpdateGeneralInfoFromCSV(ConfigCSV configcsv)
        {
            ErrorFlag = false; 
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                if (configcsv.importdata.Length != 0)
                {

                    string values = "";

                    for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                    {
                        //刷新ui
                        //worker.ReportProgress(i * 100 / configcsv.importdata.Length, String.Format("导入计算配置信息，共{0}条，导入第{1}条！", configcsv.importdata.Length,i));
                        string fid = configcsv.importdata[i][0];
                        //源标签信息
                        //string sourcetagname = configcsv.importdata[i][configcsv.sourcetagnameIndex].Trim();      //涉及到大小写变换、标签检查、PGIM规则替换。不能直接修改，只能初始化。
                        //string sourcetagdb = configcsv.importdata[i][configcsv.sourcetagdbIndex].Trim();          //涉及到根据类型，对sourcetagname进行重填。不能直接修改，只能初始化。
                        string sourcetagdesc = configcsv.importdata[i][configcsv.sourcetagdescIndex].Trim();        //仅描述性，不参与计算，可以直接修改
                        string sourcetagdim = configcsv.importdata[i][configcsv.sourcetagdimIndex].Trim();          //仅描述性，不参与计算，可以直接修改
                        string sourcetagmrb = configcsv.importdata[i][configcsv.sourcetagmrbIndex].Trim();          //仅描述性，不参与计算，可以直接修改
                        string sourcetagmre = configcsv.importdata[i][configcsv.sourcetagmreIndex].Trim();          //仅描述性，不参与计算，可以直接修改
                        
                        //计算公式信息    1、名称
                        //string fmodulename = configcsv.importdata[i][configcsv.fmodulenameIndex].Trim();          //计算公式，不能变，涉及标签id
                        //string fgroup = configcsv.importdata[i][configcsv.fgroupIndex].Trim();                      //暂时不用
                        //string forder = configcsv.importdata[i][configcsv.forderIndex].Trim();                      //暂时不用
                        
                        //计算公式信息    2、算法              
                        string falgorithmsflag = configcsv.importdata[i][configcsv.falgorithmsflagIndex].Trim();    //指定哪些结果保存。不参与计算，可以直接修改
                        string fparas = configcsv.importdata[i][configcsv.fparasIndex].Trim();                      //计算参数。参与计算。可以直接修改
                        //string fcondpslnames = configcsv.importdata[i][configcsv.fcondpslnamesIndex].Trim();      //计算条件。涉及到标签检查。不可直接修改
                        string fcondexpression = configcsv.importdata[i][configcsv.fcondexpressionIndex].Trim();    //计算条件表达式。计算条件如何逻辑运算。可以直接修改
                        
                        //计算公式信息    3、结果
                        //string foutputtable = configcsv.importdata[i][configcsv.foutputtableIndex].Trim();          //暂时不用
                        //string foutputnumber = configcsv.importdata[i][configcsv.foutputnumberIndex].Trim();        //与计算相关，固定值
                        //string foutputpsltagnames = configcsv.importdata[i][configcsv.foutputpsltagprefIndex].Trim(); //涉及到id不能修改。  
                        
                        //计算公式信息    4、周期
                        //string finterval = configcsv.importdata[i][configcsv.fintervalIndex].Trim();                   //不可以修改，影响计算结果标签
                        //string fintervaltype = configcsv.importdata[i][configcsv.fintervaltypeIndex].Trim();           //不可以修改，影响计算结果标签
                        //计算延时信息
                        string fdelay = configcsv.importdata[i][configcsv.fdelayIndex].Trim();                          //可以修改
                        //计算起始时间
                        //string fstartdate = configcsv.importdata[i][configcsv.fstartdateIndex].Trim();                  //2018.6.27 修改信息里不包含起始时间

                        values = values + String.Format("sourcetagdesc='{0}',sourcetagdim='{1}',sourcetagmrb='{2}',sourcetagmre='{3}',"+
                                                        "falgorithmsflag='{4}',fparas='{5}',fcondexpression='{6}',"+
                                                        "fdelay='{7}'",
                                  sourcetagdesc, sourcetagdim, sourcetagmrb, sourcetagmre,      //源标签信息                                 
                                 falgorithmsflag, fparas, fcondexpression,                      //计算公式信息    2、算法                                
                                 fdelay);                                                       //计算公式信息    4、循环

                        sqlStr = String.Format("use psldb;update pslcalcuconfig set {0} where fid={1}", values,fid);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        
                        //更新UI
                        worker.ReportProgress(i * 100 / configcsv.importdata.Length, String.Format("更新计算配置信息，共{0}条，导入第{1}条！", configcsv.importdata.Length, i));
                        //复位
                        values = "";
                                       
                       

                    }//end for

                }//end if
                return true;

            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.UpdateFromCSV()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //更新标签名称：根据唯一的fid进行更新
        public static bool UpdateTagnameInfo(uint fid,string fieldtype, string tagname)
        {
            ErrorFlag = false; 
            string sqlStr = "";
            string fieldname="";
            try
            {
                DbHelper dbhelper = new DbHelper();

                if (fieldtype != "" && tagname != "")
                 {
                     switch (fieldtype)
                     {
                         case "源标签":
                             fieldname = "sourcetagname";
                             break;
                         case "条件标签":
                             fieldname = "fcondpslnames";
                             break;
                         case "结果标签":
                             fieldname = "foutputpsltagnames";
                             break;
                     }
                     string values = String.Format("{0}='{1}'", fieldname, tagname);
                     sqlStr = String.Format("use psldb;update pslcalcuconfig set {0} where fid={1}", values, fid);
                     dbhelper.ExecuteNonQuery(sqlStr);
                     return true;
                 }
                 return false;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.UpdateTagnameInfo()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        
        }
        //读取pslconfig表的配置信息:1、检查fid是否是从0开始的连续整数。2、返回结果、按照fid进行排序
        public static List<PSLCalcuItem> ReadConfig()
        {
            ErrorFlag = false; 
            string sqlStr = "";

            //读取计算组件信息
            List<PSLModule> modulesinfo;
            //从算法信息数据表中读取算法信息
            try
            {
                modulesinfo = PSLModulesDAO.ReadData();
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLModulesDAO.ReadData()错误：---------->") + Environment.NewLine;
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
            //读取计算配置信息
            try
            {
                DbHelper dbhelper = new DbHelper();
                //从pscalcuconfig表读取
                string fields = "pslcalcuconfig.fid,"
                    //源标签基本信息
                                        + "pslcalcuconfig.sourcetagname,"
                                        + "pslcalcuconfig.sourcetagdb,"
                                        + "pslcalcuconfig.sourcetagdesc,"
                                        + "pslcalcuconfig.sourcetagdim,"
                                        + "pslcalcuconfig.sourcetagmrb,"
                                        + "pslcalcuconfig.sourcetagmre,"
                    //计算公式配置信息 1、名称
                                        + "pslcalcuconfig.fmodulename,"
                                        + "pslcalcuconfig.fnode,"
                                        + "pslcalcuconfig.fgroup,"
                                        + "pslcalcuconfig.forder,"
                    // + "pslmodules.moduleclass,"                       //fclass在pslmodules表中，不在pslcalcuconfig中出现。用于反射调用。
                    //计算公式配置信息 2、算法
                                        + "pslcalcuconfig.falgorithmsflag,"
                                        + "pslcalcuconfig.fparas,"
                                        + "pslcalcuconfig.fcondpslnames,"
                                        + "pslcalcuconfig.fcondexpression,"
                    //计算公式配置信息 3、结果
                                        + "pslcalcuconfig.foutputtable,"
                                        + "pslcalcuconfig.foutputnumber,"                //foutputnumber在pslmodules表中，不在pslcalcuconfig中出现。用于计算引擎循环
                                        + "pslcalcuconfig.foutputpsltagnames,"
                    //计算公式配置信息 4、周期
                                        + "pslcalcuconfig.finterval,"
                                        + "pslcalcuconfig.fintervaltype,"
                                        + "pslcalcuconfig.fdelay,"
                    //计算时间
                                        + "pslcalcuconfig.fstarttime"
                                        ;

                sqlStr = String.Format("use psldb;select {0} from pslcalcuconfig ORDER BY `fid` ASC", fields);
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLCalcuItem> pslcalcuitems = IDataReader2PSLCalcuItems(reader);
                reader.Close();

                //添加结算结果是否允许空值的属性值
                foreach (PSLCalcuItem item in pslcalcuitems)
                {
                    PSLModule module = modulesinfo.Where(a => a.modulename == item.fmodulename).ToList()[0];
                    item.foutputpermitnull = module.moduleoutputpermitnull;
                }

                //按照fid升序排列
                pslcalcuitems.OrderBy(a => a.fid);
                
                //过滤属于本结算节点的计算项
                pslcalcuitems=pslcalcuitems.Where(a => a.fnode == APPConfig.realcalcu_calcunode).ToList();     //将计算项中计算节点号与当前计算节点一致的过滤出来
                
                //过滤后给属于本节点的计算项添加序号Index
                for (int i = 0; i < pslcalcuitems.Count; i++)
                {
                    pslcalcuitems[i].index = i;
                }
                /*20180927，添加分布式计算功能，必须放弃这一个限制。页面的刷新采用pslcalcuitems新添加字段index来完成
                //检查fid是否是从0开始的连续整数
                //如果此检测通过，则能够保证pslcalcuitems.fid和listview的对应关系。
                for (int i = 0; i < pslcalcuitems.Count; i++)
                {
                    if (pslcalcuitems[i].fid != i + 1)
                    {
                        MessageBox.Show("导入的计算配置信息有误，fid号不是从0开始的连续整数！");
                        return new List<PSLCalcuItem>();
                    }
                }
                */
                return pslcalcuitems;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.ReadConfig()错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        //读取pslconfig表sourcetagname、fcondpslnames、foutputpsltagnames三个字段包含tagname的计算项
        public static List<PSLCalcuItem> ReadConfigContainTagname(string tagname)
        {
            ErrorFlag = false; 
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                //从pscalcuconfig表读取
                sqlStr = String.Format("use psldb;select * from pslcalcuconfig where pslcalcuconfig.sourcetagname like \"%{0}%\" or pslcalcuconfig.fcondpslnames like \"%{0}%\" or pslcalcuconfig.foutputpsltagnames like \"%{0}%\"", tagname);
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLCalcuItem> pslcalcuitems = IDataReader2PSLCalcuItems(reader);
                reader.Close();

                //按照fid升序排列
                pslcalcuitems.OrderBy(a => a.fid);

                if (pslcalcuitems == null || pslcalcuitems.Count == 0)
                    return null;
                else
                    return pslcalcuitems;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.ReadConfigContainTagname()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        //更新pslconfig表的starttime信息:一次更新一行
        public static bool UpdateStartTime(int fid, DateTime fstarttime)
        {
            ErrorFlag = false; 
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = string.Format("use psldb;update pslcalcuconfig set fstarttime='{0}' where fid='{1}'", fstarttime, fid);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.UpdateStartTime()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //更新pslconfig表的starttime信息：批量更新
        public static bool UpdateStartTimeBatch(List<PSLCalcuItem> PslCalcuItems)
        {
            /* 批量update语句语法
                UPDATE table_name 
                SET field_name = CASE other_field 
                WHEN 1 THEN 'value'
                WHEN 2 THEN 'value'
                WHEN 3 THEN 'value'
                END
                WHERE id IN (1,2,3) 
            */
            ErrorFlag = false; 
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                string fidStr = "";
                string fstartStrSwitch = "";
                for (int i = 0; i < PslCalcuItems.Count; i++)
                {
                    fidStr = fidStr + String.Format("{0},", PslCalcuItems[i].fid);
                    fstartStrSwitch = fstartStrSwitch + String.Format("when {0} then '{1}' ", PslCalcuItems[i].fid, PslCalcuItems[i].fstarttime);
                    //批量更新计算信息表，一次更新50条。
                    if (i % 50 == 0 || i == PslCalcuItems.Count - 1)
                    {
                        //刷新ui
                        worker.ReportProgress(i * 100 / PslCalcuItems.Count, String.Format("更新计算信息配置表'计算起始时间'，共{0}条，导入第{1}条！", PslCalcuItems.Count, i));
                        //批量update信息    
                        fidStr = String.Format("({0})", fidStr.Substring(0, fidStr.Length - 1));
                        sqlStr = string.Format("use psldb;update pslcalcuconfig set fstarttime= case fid {0} end where fid in {1};", fstartStrSwitch, fidStr);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        fidStr = "";
                        fstartStrSwitch = "";
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.UpdateStartTime()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //更新pslconfig表的starttime信息：批量更新,不包含界面刷新
        public static bool UpdateStartTimeBatchWithoutUpdate(List<PSLCalcuItem> PslCalcuItems)
        {
            /* 批量update语句语法
                UPDATE table_name 
                SET field_name = CASE other_field 
                WHEN 1 THEN 'value'
                WHEN 2 THEN 'value'
                WHEN 3 THEN 'value'
                END
                WHERE id IN (1,2,3) 
            */
            ErrorFlag = false; 
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                string fidStr = "";
                string fstartStrSwitch = "";
                for (int i = 0; i < PslCalcuItems.Count; i++)
                {
                    fidStr = fidStr + String.Format("{0},", PslCalcuItems[i].fid);
                    fstartStrSwitch = fstartStrSwitch + String.Format("when {0} then '{1}' ", PslCalcuItems[i].fid, PslCalcuItems[i].fstarttime);
                    //批量更新计算信息表，一次更新50条。
                    if (i % 50 == 0 || i == PslCalcuItems.Count - 1)
                    {
                        //批量update信息    
                        fidStr = String.Format("({0})", fidStr.Substring(0, fidStr.Length - 1));
                        sqlStr = string.Format("use psldb;update pslcalcuconfig set fstarttime= case fid {0} end where fid in {1};", fstartStrSwitch, fidStr);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        fidStr = "";
                        fstartStrSwitch = "";
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.UpdateStartTime()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //删除配置项
        public static bool DeleteConfigItems(uint[] PslCalcuItemIndexs)
        {
            /* 批量update语句语法
                UPDATE table_name 
                SET field_name = CASE other_field 
                WHEN 1 THEN 'value'
                WHEN 2 THEN 'value'
                WHEN 3 THEN 'value'
                END
                WHERE id IN (1,2,3) 
            */
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //获取要删除的计算项序列号串
                string PslCalcuItemidStr = "";
                for (int i = 0; i < PslCalcuItemIndexs.Length; i++)
                {
                    PslCalcuItemidStr = PslCalcuItemidStr + "'" + PslCalcuItemIndexs[i].ToString() + "'" + ",";
                }
                PslCalcuItemidStr = PslCalcuItemidStr.Substring(0, PslCalcuItemidStr.Length - 1);

                //批量删除数据                       
                sqlStr = String.Format("use psldb;delete from pslcalcuconfig where fid in ({0})", PslCalcuItemidStr);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLCalcuConfigDAO.DeleteConfigItems()错误：---------->") + Environment.NewLine;
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
        private static List<PSLCalcuItem> IDataReader2PSLCalcuItems(IDataReader reader)
        {
            List<PSLCalcuItem> items = new List<PSLCalcuItem>();
            while (reader.Read())
            {
                PSLCalcuItem item = new PSLCalcuItem();
                DAOSupport.ReaderToObject(reader, item);
                items.Add(item);
            }
            return items;
        }
        #endregion

    }
}
