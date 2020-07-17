using System;
using System.Collections;   //ʹ��hashtable
using System.Windows.Forms; //ʹ��messagebox

namespace Config
{
    /// <summary>
    /// Config ��ժҪ˵����
    /// Config�����ڴ������ͨ�����ú����ݿ����ã����������ݿ����ӡ�
    /// Config�������湦�ܺ�config�ڳ����еĵ�λ����Config���þ�̬����ʵ�֡�
    /// �����config�ļ��У�Config\\AppConfig.xml��û���ҵ���Ӧ�������ᷢ����SQLHelper�����ͳ�ʼֵ�趨����쳣�����⡱
    /// </summary>
    public static class APPConfig
    {   
        #region APPȫ�ֲ�������Ϊȫ�ֲ�����Ŀǰֱ����Config�ڶ��壬δ���뵽xml���õ���ȥ
        public static string DLLNAME_CALCUMODULE = "PSLCalcu.Module.dll";                   //����ģ������dll����
        public static string NAMESPACE_CALCUMODULE = "PSLCalcu.Module";                     //����ģ�����������ռ�����
        public const int CALCUMODULE_THRESHOLD = 0;                                         //����ģ��ʱ�������ֵ
        #endregion      

        public static string CONFIG_FILE = "\\APPConfig\\AppConfig.xml";     //xml config�ļ�λ�ü�����
        #region Common config items ͨ��������Ϣ
        //public static int IntervalCalculate { get; set; }                                 //������ʱ��
        public static int IntervalDisplay { get; set; }                                     //��ʾˢ�¼��ʱ��
        public static string app_mode { get; set; }                                         //app����ģʽ���û�ģʽuser������ģʽdebug���ò�����������log��������appconfig�н������á�����log4net.config�н�������
        public static string app_debug_password { get; set; }                               //APP����ģʽ������ģʽ���롣
        public static int log_validperiod { get; set; }                                     //log�ļ���Ч����
        #endregion

        #region realcalcu ʵʱ���������������
        public static string realcalcu_calcunode { get; set; }                              //ʵʱ����ڵ�š���ͬ�ļ���ڵ㣬ֻ���������ڵ����֮��Ӧ�ļ����
        public static string realcalcu_autorun { get; set; }                                //�Ƿ��Զ���������
        public static int realcalcu_period { get; set; }                                    //����ɨ������
        public static int realcalcu_periodwritepslcalcuitem { get; set; }                   //д�������ö����̣߳������ļ�����Сʱ
        public static string realcalcu_recordcalcutime { get; set; }                        //��������ģ������ʱ���ͳ�ƹ���
        public static int realcalcu_recordsavenumber { get; set; }                          //ʱ��ͳ�ƹ���ÿ��������¼д��һ��
        public static int realcalcu_maxreadrtdb { get; set; }                               //����������ӿڴ�ʵʱ���ݿ�һ�ζ�ȡ������������������ԣ�golden��pgim��Ϊ200000w����        
        #endregion
        
        #region historycalcu ��ʷ���������������
        public static int historycalcu_period4RTD { get; set; }                             //��ʷ�������棬ʵʱ���ݲ�������Ĳ������ڡ���λ����
        public static int historycalcu_period4PSL { get; set; }                             //��ʷ�������棬�Ż����ݲ�������Ĳ������ڡ���λ����
        #endregion

        #region RTDb config ʵʱ���ݿ�������Ϣ
        public static string rtdb_Type { get; set; }                                        //ʵʱ���ݿ����ƣ���CurrentRTDbTypeö�ٱ���       
        public static string rtdb_connString { get; set; }                                  //�����ַ���
        public static int rtdb_maxrepeatonerror { get; set; }                               //��ȡ����ʱ������ض�����
        public static int rtdb_waitsecondonerror { get; set; }                              //���������´��ض�ʱ�ĵȴ�����
        #endregion

        #region RDb config  ��ϵ���ݿ�������Ϣ
        public static string rdb_Type { get; set; }                                         //��ϵ���ݿ����ƣ���CurrentDbTypeö�ٱ���       
        public static string rdb_connString { get; set; }                                   //�����ַ���
        #endregion

        #region rdbtable ��ϵ�����ݱ�����
        public static string rdbtable_resulttagauto { get; set; }                           //�Ƿ��Զ����ɼ�������ǩ
        public static string rdbtable_resulttagincludeinterval { get; set; }                //�Զ����ɼ�������ǩʱ����������������
        public static string rdbtable_tag2idalwaysreset { get; set; }                       //��ǩidӳ����Ƿ�ÿ�ζ��������
        public static string rdbtable_iniTableIncludePsldata { get; set; }                  //��ʼ�����ݱ�ʱ����psldata��
        public static int rdbtable_constmaxnumber { get; set; }                             //������ǩid���ֵ���Ż���ǩid��ʼֵ
        #endregion

        #region psldata ��ϵ�����ݱ�psldata����
        public static int psldata_startyear { get; set; }                                   //psldatayyyyMM���ݱ����ʼ���
        public static int psldata_endyear { get; set; }                                     //psldatayyyyMM���ݱ�Ľ�ֹ���
        public static int psldata_intervalmonth { get; set; }                               //psldata���ݱ����·�
        #endregion

        private static string _fileName;

        static APPConfig()
        {
            try
            {
                XmlFileHelper xmlfilehelper = new XmlFileHelper();

                //_fileName = gcsl.AppG.GetAppPath() + CONFIG_FILE;                     //����ʹ��gcsl��װ����Ϊϵͳ����
                _fileName =System.Environment.CurrentDirectory+ CONFIG_FILE;
                //Console.WriteLine(_fileName);
                //ͨ��������Ϣ
                //IntervalCalculate = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "interval_calculate").Value) * 1000;
                IntervalDisplay = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "interval_calculate").Value) * 1000;
                app_mode = xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "appmode").Value;
                app_debug_password = xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "password").Value;
                try { log_validperiod = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/common", "log_validperiod").Value); }
                catch { log_validperiod = 60; }

                //ʵʱ���������������
                realcalcu_calcunode = xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "calcunode").Value;
                realcalcu_period = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "period").Value);
                realcalcu_periodwritepslcalcuitem = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "periodwritepslcalcuitem").Value);    //�����ļ�����Сʱ
                realcalcu_autorun = xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "autorun").Value;
                realcalcu_maxreadrtdb = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "maxreadrtdb").Value);
                realcalcu_recordcalcutime = xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "recordcalcutime").Value;
                realcalcu_recordsavenumber = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/realcalcu", "recordsavenumber").Value);
                
                //��ʷ���������������
                historycalcu_period4RTD = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/historycalcu", "historycalcu_period4RTD").Value);
                historycalcu_period4PSL = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/historycalcu", "historycalcu_period4PSL").Value);
                
                //ʵʱ���ݿ�������Ϣ            
                rtdb_Type = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "type").Value;
                rtdb_connString = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "connstring").Value;
                rtdb_maxrepeatonerror = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "maxrepeatonerror").Value);
                rtdb_waitsecondonerror = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/rtdb", "waitsecondonerror").Value);

                //��ϵ���ݿ�������Ϣ
                rdb_Type = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdb", "type").Value;
                rdb_connString = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdb", "connstring").Value;
                //Console.WriteLine(rdb_connString);
                
                //��ϵ�����ݱ�����
                rdbtable_resulttagauto = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "resulttagauto").Value;
                rdbtable_resulttagincludeinterval = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "resulttagincludeinterval").Value;
                rdbtable_tag2idalwaysreset = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "tag2idalwaysreset").Value;
                rdbtable_iniTableIncludePsldata = xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "iniTableIncludePsldata").Value;
                rdbtable_constmaxnumber = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/rdbtable", "constmaxnumber").Value);

                //��ϵ��psldata���ݱ�����
                psldata_startyear = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/psldata", "startyear").Value);
                psldata_endyear = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/psldata", "endyear").Value);
                psldata_intervalmonth = int.Parse(xmlfilehelper.GetXmlAttribute(_fileName, "/config/psldata", "intervalmonth").Value);
            }
            catch(Exception e) 
            {
                string messageStr = String.Format(e.Message+"{0}��", CONFIG_FILE);
                MessageBox.Show(messageStr, "�����������xml�ļ�");
            }
            
        }

        public  static void SaveConfig(string node, string property, string value) 
        {
            //����˵��  node:�ڵ�  property������  value��ֵ
            //ʾ��
            //xml��д�뷽�������£�д��config/calcu ������auto
            //Hashtable autorun = new Hashtable();
            //autorun.Add("autorun", "auto1");
            //xmlfilehelper.UpdateNode(_fileName, "/config", autorun, autorun);

            //ע�⣬������Ҫ����xml�ļ��в�����<!--ע���У���������

            XmlFileHelper xmlfilehelper = new XmlFileHelper();
            Hashtable nodeproperty = new Hashtable();
            nodeproperty.Add(property,value);
            xmlfilehelper.UpdateNode(_fileName, node, nodeproperty, nodeproperty);

        }
        
    }
}
