using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Data;                      //使用IDataReader
using Config;                           //使用配置模块
using System.Windows.Forms;             //使用msgbox
using PCCommon;                         //使用PValue

namespace PSLCalcu
{
    /// <summary>
    /// PSLHistoryCalcuConfigDAO
    /// 用于存放历史数据计算配置信息。为了方便服用与管理，虽然历史数据计算配置需要的信息与实时计算略有不同，但：
    /// ——历史数据计算配置信息表采用与计算信息配置表完全相同的表结构。    
    /// ——复用同一个Model，即PSLCalcuItem。
    /// ——采用完全相同的DAO函数。
    /// 完成的功能：
    /// 1、负责概化计算组态信息表pslhistorycalcuconfig的读写
    /// 2、为了便于程序在pslcalcuitems和listview之间操作的便捷，尤其是为减小计算引擎主循环的开销，因避免通过遍历的方式查找双方对象对应关系。
    /// 3、为了达到第2条的目的，本程序以pslcalcuitems的fid作为与listview对应关系的依据。
    ///     ——在pslcalcuconfig表中，fid必须是从1开始的连续整数。
    ///     ——在数据表层面，由fid字段的主键、自增属性保证。
    ///     ——在数据表的操作层面，要保证仅对表进行整体读入和删除操作，而不做某一条记录的删除操作。
    ///     ——在数据使用方面，读取数据表后，首先要对fid进行检查，以保证fid是从1开始的连续整数。
    ///     ——在用配置信息生成pslcalcuitems前，要对fid进行排序，一保证fid号，与生成的listview的item序号对应。
    /// </summary>
    public class PSLHistoryCalcuConfigDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLHistoryCalcuConfigDAO));       //全局log
        //清空pslconfig数据
        public static bool ClearData()
        {
            DbHelper dbhelper = new DbHelper();
            string sqlStr = String.Format("use psldb;truncate table pslhistorycalcuconfig");
            dbhelper.ExecuteNonQuery(sqlStr);
            return true;
        }

        //读取pslhistoryconfig表的配置信息:1、检查fid是否是从0开始的连续整数。2、返回结果、按照fid进行排序
        //读取pslconfig表的配置信息:1、检查fid是否是从0开始的连续整数。2、返回结果、按照fid进行排序
        public static List<PSLCalcuItem> Read()
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                //从pslhistorycalcuconfig表读取
                string fields = "pslhistorycalcuconfig.fid,"
                    //源标签基本信息
                                        + "pslhistorycalcuconfig.sourcetagname,"
                                        + "pslhistorycalcuconfig.sourcetagdb,"
                                        + "pslhistorycalcuconfig.sourcetagdesc,"
                                        + "pslhistorycalcuconfig.sourcetagdim,"
                                        + "pslhistorycalcuconfig.sourcetagmrb,"
                                        + "pslhistorycalcuconfig.sourcetagmre,"
                    //计算公式配置信息 1、名称
                                        + "pslhistorycalcuconfig.fmodulename,"
                                        + "pslhistorycalcuconfig.fgroup,"
                                        + "pslhistorycalcuconfig.forder,"
                    // + "pslmodules.moduleclass,"                       //fclass在pslmodules表中，不在pslcalcuconfig中出现。用于反射调用。
                    //计算公式配置信息 2、算法
                                        + "pslhistorycalcuconfig.falgorithmsflag,"
                                        + "pslhistorycalcuconfig.fparas,"
                                        + "pslhistorycalcuconfig.fcondpslnames,"
                                        + "pslhistorycalcuconfig.fcondexpression,"
                    //计算公式配置信息 3、结果
                                        + "pslhistorycalcuconfig.foutputtable,"
                                        + "pslhistorycalcuconfig.foutputnumber,"                //foutputnumber在pslmodules表中，不在pslcalcuconfig中出现。用于计算引擎循环
                                        + "pslhistorycalcuconfig.foutputpsltagnames,"
                    //计算公式配置信息 4、周期
                                        + "pslhistorycalcuconfig.finterval,"
                                        + "pslhistorycalcuconfig.fintervaltype,"
                                        + "pslhistorycalcuconfig.fdelay,"
                    //计算时间
                                        + "pslhistorycalcuconfig.fstarttime"
                                        ;

                sqlStr = String.Format("use psldb;select {0} from pslhistorycalcuconfig ", fields);
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLCalcuItem> pslcalcuitems = IDataReader2PSLCalcuItems(reader);
                reader.Close();

                //按照fid升序排列
                pslcalcuitems.OrderBy(a => a.fid);

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
                return pslcalcuitems;
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("DAO层错误：------------------------------------------------------------------") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——PSLCalcuConfigDAO.ReadConfig()错误！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                logHelper.Error(ex.ToString());
                return null;
            }
        }

        //写pslhistoryconfig表接口
        public static bool Write(List<PSLCalcuItem> calcuitems) 
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                if (calcuitems.Count != 0)
                {
                    string values = "";
                    for (int i = 0; i < calcuitems.Count; i++) 
                    {
                        values = values + String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}'),",
                                 calcuitems[i].sourcetagname, calcuitems[i].sourcetagdb, calcuitems[i].sourcetagdesc, calcuitems[i].sourcetagdim, calcuitems[i].sourcetagmrb, calcuitems[i].sourcetagmre,    //源标签信息
                                 calcuitems[i].fmodulename, calcuitems[i].fgroup, calcuitems[i].forder,                                                 //计算公式信息    1、名称
                                 calcuitems[i].falgorithmsflag, calcuitems[i].fparas, calcuitems[i].fcondpslnames, calcuitems[i].fcondexpression,       //计算公式信息    2、算法
                                 calcuitems[i].foutputtable, calcuitems[i].foutputnumber, calcuitems[i].foutputpsltagnames,                             //计算公式信息    3、结果
                                 calcuitems[i].finterval, calcuitems[i].fintervaltype);                                                                 //计算公式信息    4、循环

                        //一次写入一百条记录，加快写入速度
                        if (i % 100 == 0 || i == calcuitems.Count - 1)            //i到整除10时，或到最后一行时，进行写入
                        {
                            
                            //数据库字段
                            string fileds = "sourcetagname,sourcetagdb,sourcetagdesc,sourcetagdim,sourcetagmrb,sourcetagmre," +
                                             "fmodulename,fgroup,forder," +
                                             "falgorithmsflag,fparas,fcondpslnames,fcondexpression," +
                                             "foutputtable,foutputnumber,foutputpsltagnames," +
                                             "finterval,fintervaltype";
                            values = values.Substring(0, values.Length - 1);
                            sqlStr = String.Format("use psldb;insert into pslhistorycalcuconfig({0}) values {1}", fileds, values);
                            dbhelper.ExecuteNonQuery(sqlStr);
                            //复位
                            values = "";
                        }
                    
                    
                    }

                   
                }
                return true;
            }
            catch (Exception ex)
            {
                string messageStr;
                //messageStr = String.Format("配置信息写入数据表pslcalcuconfig时出错，请检查\\LogFiles目录下的log文件！！");
                //MessageBox.Show(messageStr);

                messageStr = String.Format("DAO层错误：------------------------------------------------------------------") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——PSLCalcuConfigDAO.ImportConfig()错误！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("——sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                logHelper.Error(ex.ToString());
                return false;
            }  
        }
               
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
    }
}
