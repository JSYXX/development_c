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
    public class PSLTagNameIdMapDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLTagNameIdMapDAO));               //全局log

        #region 公有变量
        public static bool ErrorFlag = false;                                                   //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        #endregion

        #region 公有方法
        //清空数据
        public static bool ClearData()
        {
            ErrorFlag = false;
            string sqlStr = "";
            //先判断是否存在psldb数据库
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;truncate table psltagnameidmap");
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
            ErrorFlag = false;
            string sqlStr = "";
            //先判断是否存在psldb数据库
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;alter table psltagnameidmap  auto_increment={0}", startint);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch
            {
                MessageBox.Show("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                return false;
            }
        }
        //检查标签唯一性
        public static bool CheckUnique(ConfigCSV configcsv)
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //0、将所有标签汇集到psltagname
                Dictionary<string, string> psltagnamesDic = new Dictionary<string, string>();
                Dictionary<string, int> psltagCalcuIndexDic = new Dictionary<string, int>();

                //1、先读取映射表中已有的标签
                sqlStr = String.Format("use psldb;select * from psltagnameidmap ");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLTagNameIdMapItem> psltagitems = IDataReader2PSLTagNameIdMapItem(reader);
                reader.Close();
                Dictionary<string, System.UInt32> psltagmap = new Dictionary<string, System.UInt32>();
                foreach (PSLTagNameIdMapItem psltagitem in psltagitems)
                {
                    //logHelper.Info("已经存在的名称：" + psltagitem.psltagname);
                    psltagnamesDic.Add(psltagitem.psltagname, "数据库计算配置表");
                    psltagCalcuIndexDic.Add(psltagitem.psltagname, psltagitem.pslcalcuconfigindex);
                }//此时，psltagmap包含所有已经存在的标签

                //2、向psltagnames添加配置的计算结果表标签。计算结果标签放在importdata[i][foutputpsltagprefIndex]中
                int sum = 0;
                int columnfoutputpsltagprefIndex = configcsv.foutputpsltagprefIndex;

                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] plstagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagprefIndex], ";|；");    //如果有;|；|，|,则表示多个标签
                    foreach (string psltag in plstagstemp)
                    {

                        if (psltagnamesDic.ContainsKey(psltag))
                        {

                            sum += 1;
                            string messageStr = String.Format("第{0}行的计算结果标签{1}，与{2}中的第{3}行计算的计算结果标签重名，请检查！", i - configcsv.firstDataRow + 1, psltag, psltagnamesDic[psltag], -configcsv.firstDataRow + psltagCalcuIndexDic[psltag]);
                            logHelper.Error(messageStr);
                        }
                        else
                        {

                            psltagnamesDic.Add(psltag.Trim(), "CSV计算配置表");
                            psltagCalcuIndexDic.Add(psltag.Trim(), i + 1);
                        }
                    }
                }//此时，psltagnames包含了所有要添加的标签

                //3、检查结果
                if (sum == 0)
                {
                    return true;
                }
                else
                {
                    string messageStr = String.Format("要添加的计算结果标签与已经存在的计算结果标签重名，请检查log文件！");
                    MessageBox.Show(messageStr);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.CheckUnique()错误：检查计算结果标签名称在标签id映射表中是否已经存在时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //检查源标签在计算配置表和标签id映射表中是否存在
        public static bool CheckSourcePSLName(ConfigCSV configcsv)
        {
            ErrorFlag = false;
            //——该检查必须在AutoGeneratePSLTags之后进行。该方法后，计算结果标签均放入foutputpsltagprefIndex中。位于计算条件的标签名称均应该在foutputpsltagprefIndex可以找到
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //1、读取当前配置信息中的计算结果标签名到outputtagnamelist
                int columnoutputplsnames = configcsv.foutputpsltagprefIndex;    //由程序生成的计算结果标签AutoGeneratePSLTags都在foutputpsltagprefIndex字段内。   
                //输出项名称用分号分隔，
                Dictionary<string, int> outputtagnamelist = new Dictionary<string, int>();
                //取出所有计算结果标签
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] outputtagsArray = Regex.Split(configcsv.importdata[i][columnoutputplsnames], ";|；");
                    for (int j = 0; j < outputtagsArray.Length; j++)
                    {
                        outputtagnamelist.Add(outputtagsArray[j], i);   //由于前面已经检查了计算结果标签是否重复，如果可以进行到这里，计算结果标签肯定不重复。这里直接添加即可。                   
                    }
                }

                //2、读取数据库中的计算结果标签名到outputtagnamelist                
                sqlStr = String.Format("use psldb;select * from psltagnameidmap ");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLTagNameIdMapItem> psltagitems = IDataReader2PSLTagNameIdMapItem(reader);
                reader.Close();

                foreach (PSLTagNameIdMapItem psltagitem in psltagitems)
                {
                    outputtagnamelist.Add(psltagitem.psltagname, 0);     //由于前面已经检查了计算结果标签和库中已经有的标签不重复。这里直接添加即可。
                }//此时，psltagmap包含所有已经存在的标签

                //3、依顺序检查源标签
                int sum = 0;
                int columnsourceplsnames = configcsv.sourcetagnameIndex;
                int columnsourcedbtype = configcsv.sourcetagdbIndex;
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {

                    if (configcsv.importdata[i][columnsourcedbtype] == "rdb" || configcsv.importdata[i][columnsourcedbtype] == "rdbset")
                    {
                        configcsv.importdata[i][columnsourceplsnames] = configcsv.importdata[i][columnsourceplsnames].ToUpper();    //将概化型源标签全部转为大写
                        string[] condpsltagsArray = Regex.Split(configcsv.importdata[i][columnsourceplsnames], ";|；");
                        for (int j = 0; j < condpsltagsArray.Length; j++)
                        {
                            if (condpsltagsArray[j] != "" && outputtagnamelist.ContainsKey(condpsltagsArray[j]) == false)
                            {
                                sum += 1;
                                string messageStr = String.Format("第{0}行的源标签{1}，在计算结果标签（已在库中的标签和要导入的配置表标签）中不存在，请检查！", i - configcsv.firstDataRow + 1, condpsltagsArray[j]);
                                logHelper.Error(messageStr);
                            }

                        }
                    }
                }

                if (sum != 0)
                {
                    string messageStr = String.Format("概化计算组态中概化类型的源标签中有不存在标签项，请检查log文件！");
                    MessageBox.Show(messageStr);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.CheckCondPSLName()错误：检查计算条件标签名称是否已经存在时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //检查条件标签在计算配置表和标签id映射表中是否存在
        public static bool CheckCondPSLName(ConfigCSV configcsv)
        {
            ErrorFlag = false;
            //——该检查必须在AutoGeneratePSLTags之后进行。该方法后，计算结果标签均放入foutputpsltagprefIndex中。位于计算条件的标签名称均应该在foutputpsltagprefIndex可以找到
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //1、读取当前配置信息中的计算结果标签名到outputtagnamelist
                int columnoutputplsnames = configcsv.foutputpsltagprefIndex;    //由程序生成的计算结果标签AutoGeneratePSLTags都在foutputpsltagprefIndex字段内。   
                //输出项名称用分号分隔，
                Dictionary<string, int> outputtagnamelist = new Dictionary<string, int>();
                //取出所有计算结果标签
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] outputtagsArray = Regex.Split(configcsv.importdata[i][columnoutputplsnames], ";|；");
                    for (int j = 0; j < outputtagsArray.Length; j++)
                    {
                        outputtagnamelist.Add(outputtagsArray[j], i);   //由于前面已经检查了计算结果标签是否重复，如果可以进行到这里，计算结果标签肯定不重复。这里直接添加即可。                   
                    }
                }

                //2、读取数据库中的计算结果标签名到outputtagnamelist                
                sqlStr = String.Format("use psldb;select * from psltagnameidmap ");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLTagNameIdMapItem> psltagitems = IDataReader2PSLTagNameIdMapItem(reader);
                reader.Close();
                /*
                foreach (var item in outputtagnamelist)

                {

                    Console.WriteLine(item.Key + item.Value);

                }
                 */
                foreach (PSLTagNameIdMapItem psltagitem in psltagitems)
                {
                    outputtagnamelist.Add(psltagitem.psltagname, 0);     //由于前面已经检查了计算结果标签和库中已经有的标签不重复。这里直接添加即可。
                }//此时，psltagmap包含所有已经存在的标签

                //3、依顺序检查条件标签
                int sum = 0;
                int columncondplsnames = configcsv.fcondpslnamesIndex;
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] condpsltagsArray = Regex.Split(configcsv.importdata[i][columncondplsnames].ToUpper(), ";|；");
                    for (int j = 0; j < condpsltagsArray.Length; j++)
                    {
                        if (condpsltagsArray[j] != "" && outputtagnamelist.ContainsKey(condpsltagsArray[j]) == false)
                        {
                            sum += 1;
                            string messageStr = String.Format("第{0}行的计算条件标签{1}，在计算结果标签（已在库中的标签和要导入的配置表标签）中不存在，请检查！", i - configcsv.firstDataRow + 1, condpsltagsArray[j]);
                            logHelper.Error(messageStr);
                        }

                    }
                }

                if (sum != 0)
                {
                    string messageStr = String.Format("概化计算组态文件计算条件标签中有不存在标签项，请检查log文件！");
                    MessageBox.Show(messageStr);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.CheckCondPSLName()错误：检查计算条件标签名称是否已经存在时出错---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        public static BackgroundWorker worker;      //给generateMap，用于更新进度条界面
        public static int MAX_NUMBER_CONSTTAG = APPConfig.rdbtable_constmaxnumber;
        //抽取概化标签名称和id映射表：reassign=1为重新建立标签序号映射；reassign=0为添加标签 
        public static bool generateMap(ConfigCSV configcsv, bool reassign)
        {
            ErrorFlag = false;
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
                    sqlStr = String.Format("use psldb;delete from psltagnameidmap;truncate table psltagnameidmap;alter table psltagnameidmap  auto_increment={0}", MAX_NUMBER_CONSTTAG);
                    dbhelper.ExecuteNonQuery(sqlStr);
                }

                //2、将所有标签汇集到psltagname,将所有计算结果描述汇集到psltagdesc
                List<string> consttagnames = new List<string>();                                //常数标签名
                Dictionary<string, string> consttagdesc = new Dictionary<string, string>();     //常数标签描述字典
                Dictionary<string, int> consttagcalcuindex = new Dictionary<string, int>();     //常数标签计算项序号字典
                Dictionary<string, string> consttaggroup = new Dictionary<string, string>();    //常数标签组名字典
                Dictionary<string, int> consttagorder = new Dictionary<string, int>();          //常数标签组序号字典
                Dictionary<string, int> consttagflag = new Dictionary<string, int>();           //常数标签保存标记字典

                List<string> psltagnames = new List<string>();                                  //概化标签名
                Dictionary<string, string> psltagdesc = new Dictionary<string, string>();       //概化标签名描述字典
                Dictionary<string, int> psltagcalcuindex = new Dictionary<string, int>();       //概化标签名计算项序号字典
                Dictionary<string, string> psltaggroup = new Dictionary<string, string>();      //概化标签组名字典
                Dictionary<string, int> psltagorder = new Dictionary<string, int>();            //概化标签组序号字典
                Dictionary<string, int> psltagflag = new Dictionary<string, int>();           //概化标签保存标记字典

                //2.1、先向psltagnames添加特定标签
                if (reassign)
                {
                    //添加特殊标签“当前时间段”
                    string specialtag = "CurrentSpan";
                    psltagnames.Add(specialtag.ToUpper());                                             //tagid=10000，tagname=CurrentSpan，表示当前计算周期，用于时间逻辑表达式要找当前周期时。
                    psltagdesc.Add(specialtag.ToUpper(), "当前计算时间段");
                    psltagcalcuindex.Add(specialtag.ToUpper(), 0);
                    psltaggroup.Add(specialtag.ToUpper(), "");
                    psltagorder.Add(specialtag.ToUpper(), 0);
                    psltagflag.Add(specialtag.ToUpper(), 1);       //该标签计算结果是否会被保存的标记位，0==false不会被保存，1==true会被保存
                }

                //2.2、再向consttagnames、psltagnames添加配置的计算结果表标签。计算结果标签放在importdata[i][foutputpsltagprefIndex]中
                int columncalcuitemindex = configcsv.calcuitemindex;                        //计算项序号
                int columnfoutputpsltagprefIndex = configcsv.foutputpsltagprefIndex;        //标签名
                int columnfoutputpsltagdescIndex = configcsv.foutputpsltagdescIndex;        //标签描述
                int columnsouredbIndex = configcsv.sourcetagdbIndex;                          //标签源类型
                int columnfmodulename = configcsv.fmodulenameIndex;                         //计算项名称
                int columnforderIndex = configcsv.forderIndex;                              //计算项组序号
                int columnfgroupIndex = configcsv.fgroupIndex;                              //计算项组号
                int columnfalgorithmsflag = configcsv.falgorithmsflagIndex;                 //计算结果是否保存标记

                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    //统计非常数标签
                    if (configcsv.importdata[i][columnfmodulename].Trim() != "MReadConst")
                    {
                        string[] psltagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagprefIndex], ";|；");       //如果有;|；|，|,则表示多个标签
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
                            int psltagsaveflag = 0;
                            if ((configcsv.importdata[i][columnfalgorithmsflag])[j] == 'Y')
                                psltagsaveflag = 1;
                            else
                                psltagsaveflag = 0;

                            psltagflag.Add(psltagstemp[j].Trim().ToUpper(), psltagsaveflag);
                        }
                    }
                    else
                    {
                        //统计常数标签
                        string[] consttagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagprefIndex], ";|；");       //如果有;|；|，|,则表示多个标签
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
                            consttagflag.Add(consttagstemp[j].Trim().ToUpper(), 1);
                        }
                    }
                }

                //3、重新分配id
                int writecount;
                string values;
                //分配常数标签，常数标签使用10000以下的id号，在检查常数配置标签的时候，要检查总数量不能大于10000
                //只有在重置标签的情况下，才写常数标签
                if (reassign)
                {
                    writecount = 0;
                    values = "";
                    int pointid4const = 1;  //常量标签直接给定psltagid

                    foreach (string contag in consttagnames)
                    {
                        writecount = writecount + 1;
                        values = values + String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'),",
                                                    pointid4const,                  //点id号
                                                    consttagflag[contag],           //点是否保存
                                                    contag,                         //点名
                                                    consttagcalcuindex[contag],     //点计算项序号
                                                    consttagdesc[contag],           //点描述
                                                    consttaggroup[contag],          //点组名
                                                    consttagorder[contag],          //点组内序号
                                                    ""                              //输出table名称
                                                    );    //组织字段的实际值字符串。
                        pointid4const++;
                        if (writecount % 100 == 0 || contag == consttagnames[consttagnames.Count - 1])//凑够100个标签，写入一次；或直到最后一个标签才写入一次。
                        {
                            //刷新ui                           
                            worker.ReportProgress(writecount * 100 / consttagnames.Count, String.Format("建立计算引擎内部常数标签名称和id映射，共{0}条，已导入第{1}条！", consttagnames.Count, writecount));
                            //批量写入数据
                            values = values.Substring(0, values.Length - 1);
                            sqlStr = String.Format("use psldb;insert into psltagnameidmap (psltagid,psltagsaveflag,psltagname,pslcalcuconfigindex,psltagdesc,psltaggroup,psltagorder,outputtablename) values {0}", values);
                            dbhelper.ExecuteNonQuery(sqlStr);
                            values = "";

                        }
                    }
                }
                //分配概化标签
                writecount = 0;
                values = "";
                foreach (string psltag in psltagnames)
                {
                    writecount = writecount + 1;
                    values = values + String.Format("('{0}','{1}','{2}','{3}','{4}','{5}','{6}'),",
                                                        psltagflag[psltag],           //点是否保存
                                                        psltag,                         //点名
                                                        psltagcalcuindex[psltag],       //点计算项序号
                                                        psltagdesc[psltag],             //点描述
                                                        psltaggroup[psltag],            //点组名
                                                        psltagorder[psltag],            //点组内序号
                                                        ""                              //输出table名称
                                                        );    //组织字段的实际值字符串。
                    if (writecount % 100 == 0 || psltag == psltagnames[psltagnames.Count - 1])//凑够100个标签，写入一次；或直到最后一个标签才写入一次。
                    {
                        //刷新ui                           
                        worker.ReportProgress(writecount * 100 / psltagnames.Count, String.Format("建立计算引擎内部概化标签名称和id映射，共{0}条，已导入第{1}条！", psltagnames.Count, writecount));
                        //批量写入数据
                        values = values.Substring(0, values.Length - 1);
                        sqlStr = String.Format("use psldb;insert into psltagnameidmap (psltagsaveflag,psltagname,pslcalcuconfigindex,psltagdesc,psltaggroup,psltagorder,outputtablename) values {0}", values);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        values = "";

                    }
                }
                //写入完毕
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
        //更新概化标签名称和id映射表中的中文描述
        public static bool UpdateDesc(ConfigCSV configcsv, bool reassign)
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();

                //1、将所有标签汇集到psltagname,将所有计算结果描述汇集到psltagdesc
                List<string> psltagnames = new List<string>();
                Dictionary<string, string> psltagdesc = new Dictionary<string, string>();

                //2、再向psltagnames添加配置的计算结果表标签。计算结果标签放在importdata[i][foutputpsltagprefIndex]中
                int columnfoutputpsltagprefIndex = configcsv.foutputpsltagprefIndex;
                int columnfoutputpsltagdescIndex = configcsv.foutputpsltagdescIndex;
                for (int i = configcsv.firstDataRow; i < configcsv.importdata.Length; i++)
                {
                    string[] psltagstemp = Regex.Split(configcsv.importdata[i][columnfoutputpsltagprefIndex], ";|；");       //如果有;|；|，|,则表示多个标签
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
                tagMap = PSLTagNameIdMapDAO.ReadMap();

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
                        sqlStr = String.Format("use psldb;update psltagnameidmap set psltagdesc = case psltagid {0} end where psltagid in {1}", valuesStr, idStr);
                        dbhelper.ExecuteNonQuery(sqlStr);
                        //刷新ui                           
                        worker.ReportProgress(writecount * 100 / psltagnames.Count, String.Format("更新标签名称中文描述，共{0}条，已导入第{1}条！", psltagnames.Count, writecount));
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
                sqlStr = String.Format("use psldb;update psltagnameidmap set psltagname = case psltagid {0} end where psltagid in ({1})", valuesStr, psltagidStr);
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
            ErrorFlag = false;
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
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadMap()错误：读取计算标签名称id映射表时出错---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        //读取概化标签id和是否保存映射表
        public static Dictionary<string, bool> ReadFlagMap()
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;select * from psltagnameidmap ");
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLTagNameIdMapItem> psltagitems = IDataReader2PSLTagNameIdMapItem(reader);
                reader.Close();

                Dictionary<string, bool> psltagidflagmap = new Dictionary<string, bool>();
                foreach (PSLTagNameIdMapItem psltagitem in psltagitems)
                {
                    bool saveflag = false;
                    if (psltagitem.psltagsaveflag == 1) saveflag = true;        //标签是否保存位，为1时表示保存
                    psltagidflagmap.Add(psltagitem.psltagname, saveflag);
                }

                return psltagidflagmap;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadMap()错误：读取计算标签名称id映射表时出错---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("sql语句：{0}", sqlStr);
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        public static List<string> ReadCaculateFunction()
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                List<string> caculateFunctions = new List<string>();
                string functions = System.Configuration.ConfigurationManager.AppSettings["NewCaculate"].ToString();
                caculateFunctions = functions.Split(';').ToList();
                return caculateFunctions;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadCaculateFunction()错误：读取新算法名称是出错---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        public static List<string> ReadNoCheckFunction()
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                List<string> caculateFunctions = new List<string>();
                string functions = System.Configuration.ConfigurationManager.AppSettings["NoCheckFunction"].ToString();
                caculateFunctions = functions.Split(';').ToList();
                return caculateFunctions;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadNoCheckFunction()错误：读取新算法名称是出错---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }
        
        public static List<string> ReadMPVBasePlusSft()
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                List<string> caculateFunctions = new List<string>();
                string functions = System.Configuration.ConfigurationManager.AppSettings["PlusSft"].ToString();
                caculateFunctions = functions.Split(';').ToList();
                return caculateFunctions;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadCaculateFunction()错误：读取MPVBasePlusSft名称是出错---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return null;
            }
        }

        public static List<string> ReadLongCaculateFunction()
        {
            ErrorFlag = false;
            string sqlStr = "";
            try
            {
                List<string> caculateFunctions = new List<string>();
                string functions = System.Configuration.ConfigurationManager.AppSettings["NewLongCaculate"].ToString();
                caculateFunctions = functions.Split(';').ToList();
                return caculateFunctions;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.ReadCaculateFunction()错误：读取新算法长周期名称是出错---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
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
                sqlStr = String.Format("use psldb;delete from psltagnameidmap where psltagid in ({0})", psltagidStr);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLTagNameIdMapDAO.DeleteTags()错误：删除计算标签名称id映射表时出错---------->") + Environment.NewLine;
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
