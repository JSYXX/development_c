using System;
using System.Collections;   //使用hashtable
using System.Windows.Forms; //使用messagebox

namespace Config
{
    /// <summary>
    /// Config 的摘要说明。
    /// Config仅用于处理程序通用配置和数据库配置，不参与数据库连接。
    /// Config基于上面功能和config在程序中的地位，将Config类用静态类来实现。
    /// 如果在config文件中（Config\\AppConfig.xml）没有找到对应的项，程序会发生“SQLHelper的类型初始值设定项发生异常的问题”
    /// </summary>
    public static class APPConfig
    {   
        #region APP全局参数。作为全局参数，目前直接在Config内定义，未放入到xml配置当中去
        public static string DLLNAME_CALCUMODULE = "PSLCalcu.Module.dll";                   //计算模块所在dll名称
        public static string NAMESPACE_CALCUMODULE = "PSLCalcu.Module";                     //计算模块所在命名空间名称
        public const int CALCUMODULE_THRESHOLD = 0;                                         //计算模块时间过滤阈值
        #endregion      

        public static string CONFIG_FILE = "\\APPConfig\\AppConfig.xml";     //xml config文件位置及名称
        #region Common config items 通用配置信息
        //public static int IntervalCalculate { get; set; }                                 //计算间隔时间
        public static int IntervalDisplay { get; set; }                                     //显示刷新间隔时间
        public static string app_mode { get; set; }                                         //app运行模式，用户模式user，调试模式debug。该参数用来控制log。不再在appconfig中进行设置。改在log4net.config中进行设置
        public static string app_debug_password { get; set; }                               //APP运行模式，调试模式密码。
        public static int log_validperiod { get; set; }                                     //log文件有效周期
        #endregion

        #region realcalcu 实时计算引擎相关配置
        public static string realcalcu_calcunode { get; set; }                              //实时计算节点号。不同的计算节点，只载入计算项节点号与之对应的计算项。
        public static string realcalcu_autorun { get; set; }                                //是否自动启动计算
        public static int realcalcu_period { get; set; }                                    //计算扫描周期
        public static int realcalcu_periodwritepslcalcuitem { get; set; }                   //写计算配置对象线程，配置文件中是小时
        public static string realcalcu_recordcalcutime { get; set; }                        //开启计算模件计算时间的统计功能
        public static int realcalcu_recordsavenumber { get; set; }                          //时间统计功能每多少条记录写入一次
        public static int realcalcu_maxreadrtdb { get; set; }                               //计算引擎读接口从实时数据库一次读取的最大数据量。经测试，golden和pgim均为200000w条。        
        #endregion
        
        #region historycalcu 历史计算引擎相关配置
        public static int historycalcu_period4RTD { get; set; }                             //历史计算引擎，实时数据并发计算的并发周期。单位是秒
        public static int historycalcu_period4PSL { get; set; }                             //历史计算引擎，概化数据并发计算的并发周期。单位是月
        #endregion

        #region RTDb config 实时数据库配置信息
        public static string rtdb_Type { get; set; }                                        //实时数据库名称，见CurrentRTDbType枚举变量       
        public static string rtdb_connString { get; set; }                                  //连接字符串
        public static int rtdb_maxrepeatonerror { get; set; }                               //读取错误时的最大重读次数
        public static int rtdb_waitsecondonerror { get; set; }                              //发生错误到下次重读时的等待秒数
        #endregion

        #region RDb config  关系数据库配置信息
        public static string rdb_Type { get; set; }                                         //关系数据库名称，见CurrentDbType枚举变量       
        public static string rdb_connString { get; set; }                                   //连接字符串
        #endregion

        #region rdbtable 关系库数据表设置
        public static string rdbtable_resulttagauto { get; set; }                           //是否自动生成计算结果标签
        public static string rdbtable_resulttagincludeinterval { get; set; }                //自动生成计算结果标签时包含计算间隔和类型
        public static string rdbtable_tag2idalwaysreset { get; set; }                       //标签id映射表是否每次都清空重排
        public static string rdbtable_iniTableIncludePsldata { get; set; }                  //初始化数据表时包括psldata表
        public static int rdbtable_constmaxnumber { get; set; }                             //常数标签id最大值，概化标签id起始值
        #endregion

        #region psldata 关系库数据表psldata设置
        public static int psldata_startyear { get; set; }                                   //psldatayyyyMM数据表的起始年份
        public static int psldata_endyear { get; set; }                                     //psldatayyyyMM数据表的截止年份
        public static int psldata_intervalmonth { get; set; }                               //psldata数据表间隔月份
        #endregion

        private static string _fileName;

        static APPConfig()
        {
            try
            {
                XmlFileHelper xmlfilehelper = new XmlFileHelper();

                //_fileName = gcsl.AppG.GetAppPath() + CONFIG_FILE;                     //不再使用gcsl封装，改为系统函数
                _fileName =System.Environment.CurrentDirectory+ CONFIG_FILE;
                //Console.WriteLine(_fileName);
                //通用配置信息
                //IntervalCalculate = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "interval_calculate").Value) * 1000;
                IntervalDisplay = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "interval_calculate").Value) * 1000;
                app_mode = xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "appmode").Value;
                app_debug_password = xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "password").Value;
                try { log_validperiod = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "log_validperiod").Value); }
                catch { log_validperiod = 60; }

                //实时计算引擎相关配置
                realcalcu_calcunode = xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "calcunode").Value;
                realcalcu_period = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "period").Value);
                realcalcu_periodwritepslcalcuitem = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "periodwritepslcalcuitem").Value);    //配置文件中是小时
                realcalcu_autorun = xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "autorun").Value;
                realcalcu_maxreadrtdb = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "maxreadrtdb").Value);
                realcalcu_recordcalcutime = xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "recordcalcutime").Value;
                realcalcu_recordsavenumber = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "recordsavenumber").Value);
                
                //历史计算引擎相关配置
                historycalcu_period4RTD = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/historycalcu", "historycalcu_period4RTD").Value);
                historycalcu_period4PSL = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/historycalcu", "historycalcu_period4PSL").Value);
                
                //实时数据库配置信息            
                rtdb_Type = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "type").Value;
                rtdb_connString = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "connstring").Value;
                rtdb_maxrepeatonerror = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "maxrepeatonerror").Value);
                rtdb_waitsecondonerror = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "waitsecondonerror").Value);

                //关系数据库配置信息
                rdb_Type = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdb", "type").Value;
                rdb_connString = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdb", "connstring").Value;
                //Console.WriteLine(rdb_connString);
                
                //关系库数据表设置
                rdbtable_resulttagauto = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "resulttagauto").Value;
                rdbtable_resulttagincludeinterval = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "resulttagincludeinterval").Value;
                rdbtable_tag2idalwaysreset = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "tag2idalwaysreset").Value;
                rdbtable_iniTableIncludePsldata = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "iniTableIncludePsldata").Value;
                rdbtable_constmaxnumber = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "constmaxnumber").Value);

                //关系库psldata数据表设置
                psldata_startyear = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/psldata", "startyear").Value);
                psldata_endyear = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/psldata", "endyear").Value);
                psldata_intervalmonth = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/psldata", "intervalmonth").Value);
            }
            catch(Exception e) 
            {
                string messageStr = String.Format(e.Message+"{0}！", CONFIG_FILE);
                MessageBox.Show(messageStr, "读入计算引擎xml文件");
            }
            
        }

        public  static void SaveConfig(string node, string property, string value) 
        {
            //参数说明  node:节点  property：属性  value：值
            //示例
            //xml的写入方法，如下：写入config/calcu 的属性auto
            //Hashtable autorun = new Hashtable();
            //autorun.Add("autorun", "auto1");
            //xmlfilehelper.UpdateNode(_fileName, "/config", autorun, autorun);

            //注意，本方法要求在xml文件中不能有<!--注释列，否则会出错。

            XmlFileHelper xmlfilehelper = new XmlFileHelper();
            Hashtable nodeproperty = new Hashtable();
            nodeproperty.Add(property,value);
            xmlfilehelper.UpdateNode(_fileName, node, nodeproperty, nodeproperty);

        }
        
    }
}
