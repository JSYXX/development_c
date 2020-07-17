using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCCommon;
using System.IO;
using System.Diagnostics;               //使用计时器

namespace PSLCalcu
{
    public partial class DataSearch : Form
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(DataSearch));       //全局log

        private ToolTip tooltip;

        //public 
        public Dictionary<string, System.UInt32> tagsdic { get; set; }           //标签字典。 

        //private  ，计算结果     全局变量
        System.UInt32 tagid = 101;                      //标签id
        List<System.UInt32> tagids = new List<System.UInt32>();//标签ids
        string tagname = "";                            //标签名称
        DateTime startDatetime;                         //起始时间
        DateTime endDatetime;                           //结束时间
        int pslmaxshowlimit = 5000;                     //最多显示数量        
        List<PValue> pslresults = new List<PValue>();   //概化数据查询结果
        List<PValue>[] pslresultsMulti;                 //概化数据查询结果
        int psldeletecount = 0;                         //概化数据删除统计

        //实时数据全局变量
        Dictionary<string, string> rtdbtagsdic = new Dictionary<string, string>();
        string realtagname = "";                        //标签名称
        DateTime realstartDatetime;                     //起始时间
        DateTime realendDatetime;                       //结束时间
        int maxshowlimit = 1000;                        //最多显示的数据行数                     
        List<PValue> realresults = new List<PValue>();  //实时数据查询结果
        
        //opc数据全局变量
        Dictionary<string, string> opctagsdic = new Dictionary<string, string>();
        string opctagname = "";                        //标签名称
        DateTime opcstartDatetime;                      //起始时间
        DateTime opcendDatetime;                        //结束时间
        int opcmaxshowlimit = 1000;                     //最多显示的数据行数                     
        List<PValue> opcresults = new List<PValue>();   //实时数据查询结果

        //初始化
        public DataSearch()
        {

            //实例化ToolTip控件
            tooltip = new ToolTip();
            //设置提示框显示时间，默认5000，最大为32767，超过此数，将以默认5000显示           
            tooltip.AutoPopDelay = 30000;
            //是否以球状显示提示框            
            tooltip.IsBalloon = true;

            InitializeComponent();
        }
        private void Search_Load(object sender, EventArgs e)
        {
            //*************************计算结果控件*******************************
            //this.tbTagName.Enabled = false;
            this.tb_tagid.Text = this.tagid.ToString();
            //this.lb_tagname.Text = "";

            this.rb_tagid.Checked = false;          //按id查询
            this.rb_tagname.Checked = true;         //按name 查询
            this.rb_endtime.Checked = true;         //按结束时间查询

            this.tb_pslmaxlimit.Text = this.pslmaxshowlimit.ToString();    //查询数量

            this.dt_startdate.Value = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
            this.dt_starttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            this.dt_enddate.Value = DateTime.Now.AddMonths(-0).AddDays(-DateTime.Now.Day+1);
            this.dt_endtime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            //根据默认的时间给起始时间和截止时间赋值
            string datetimeStr;
            datetimeStr = string.Format("{0} {1}", this.dt_startdate.Text, this.dt_starttime.Text);
            this.startDatetime = DateTime.Parse(datetimeStr);

            datetimeStr = string.Format("{0} {1}", this.dt_enddate.Text, this.dt_endtime.Text);
            this.endDatetime = DateTime.Parse(datetimeStr);           

            //tagname动态模糊查询           
            string[] data = new string[this.tagsdic.Keys.Count];
            this.tagsdic.Keys.CopyTo(data, 0);
            this.cb_tagname.Items.Clear();
            if (data.Length<100000) this.cb_tagname.Items.AddRange(data);     //将数据加载到combobox控件。此时如果data数据量非常大，则会非常的慢。所以只有到待选条数少于100000时，才加载
            this.cb_tagname.TextUpdate += (a, b) =>                           //当有输入信息时，通过输入的部分自动过滤。只有到待选条数少于100000时，才执行
            {
                if (data.Length < 100000)
                {
                var input = this.cb_tagname.Text.ToUpper();
                this.cb_tagname.Items.Clear();
                if (string.IsNullOrWhiteSpace(input)) this.cb_tagname.Items.AddRange(data);
                else
                {
                    var newList = data.Where(x => x.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) != -1).ToArray();
                    this.cb_tagname.Items.AddRange(newList);
                }
                this.cb_tagname.Select(this.cb_tagname.Text.Length, 0);
                this.cb_tagname.DroppedDown = true;
                //保持鼠标指针形状
                Cursor = Cursors.Default;
                }
            };

            //tagtype
            string[] tagtype = new string[2] { "rdb", "rdbset" };
            this.cb_tagtype.Items.Clear();
            this.cb_tagtype.Items.AddRange(tagtype);
            this.cb_tagtype.SelectedIndex = 0;

            //search按钮说明
            toolTipResult.SetToolTip(this.bt_Search, "搜索功能返回的数据包含时间戳等于截止时间得数据。"+Environment.NewLine+
                                                     "这点与计算引擎取计算数据的方式不同。" + Environment.NewLine +
                                                     "计算测试模块在使用搜索功能导出的CSV数据时，最后一行数据仅作为pvalue的endtime参数" + Environment.NewLine +
                                                     "所以搜索功能要取时间戳等于截止时间的数据，该数据使计算测试模块可以读取正确时间周期的pvalue值"
                                                     );
            toolTipResult.SetToolTip(this.bt_opcSearch, "搜索功能返回的数据包含时间戳等于截止时间得数据。" + Environment.NewLine +
                                                     "这点与计算引擎取计算数据的方式不同。" + Environment.NewLine +
                                                     "计算测试模块在使用搜索功能导出的CSV数据时，最后一行数据仅作为pvalue的endtime参数" + Environment.NewLine +
                                                     "所以搜索功能要取时间戳等于截止时间的数据，该数据使计算测试模块可以读取正确时间周期的pvalue值"
                                                     );
            toolTipResult.SetToolTip(this.bt_realSearch, "搜索功能返回的数据包含时间戳等于截止时间得数据。" + Environment.NewLine +
                                                     "这点与计算引擎取计算数据的方式不同。" + Environment.NewLine +
                                                     "计算测试模块在使用搜索功能导出的CSV数据时，最后一行数据仅作为pvalue的endtime参数" + Environment.NewLine +
                                                     "所以搜索功能要取时间戳等于截止时间的数据，该数据使计算测试模块可以读取正确时间周期的pvalue值"
                                                     );

        }//end load
        private void SelectChanged(object sender, EventArgs e)
        {
            string datetimeStr;
            switch (this.tab_search.SelectedIndex)
            {
                case 0:
                    //*************************计算结果控件*******************************
                    //this.tbTagName.Enabled = false;
                    this.tb_tagid.Text = this.tagid.ToString();
                    //this.lb_tagname.Text = "";

                    this.rb_tagid.Checked = false;       //按id查询
                    this.rb_tagname.Checked = true;    //按name 查询
                    this.rb_endtime.Checked = true;     //按结束时间查询
                    
                    this.tb_pslmaxlimit.Text =  this.pslmaxshowlimit.ToString();    //查询数量

                    
                    this.dt_startdate.Value = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
                    this.dt_starttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                    this.dt_enddate.Value = DateTime.Now.AddMonths(-0).AddDays(-DateTime.Now.Day+1);
                    this.dt_endtime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                    //根据默认的时间给起始时间和截止时间赋值

                    datetimeStr = string.Format("{0} {1}", this.dt_startdate.Text, this.dt_starttime.Text);
                    this.startDatetime = DateTime.Parse(datetimeStr);

                    datetimeStr = string.Format("{0} {1}", this.dt_enddate.Text, this.dt_endtime.Text);
                    this.endDatetime = DateTime.Parse(datetimeStr);

                    //tagname动态模糊查询
                    string[] data = new string[this.tagsdic.Keys.Count];
                    this.tagsdic.Keys.CopyTo(data, 0);
                    this.cb_tagname.Items.Clear();
                    if (data.Length < 100000) this.cb_tagname.Items.AddRange(data);
                    this.cb_tagname.TextUpdate += (a, b) =>
                    {
                        var input = this.cb_tagname.Text.ToUpper();
                        this.cb_tagname.Items.Clear();
                        if (string.IsNullOrWhiteSpace(input)) this.cb_tagname.Items.AddRange(data);
                        else
                        {
                            var newList = data.Where(x => x.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) != -1).ToArray();
                            this.cb_tagname.Items.AddRange(newList);
                        }
                        this.cb_tagname.Select(this.cb_tagname.Text.Length, 0);
                        this.cb_tagname.DroppedDown = true;
                        //保持鼠标指针形状
                        Cursor = Cursors.Default;
                    };
                    break;
                case 1:
                    //***********************实时数据控件****************************
                    this.rb_realtagname.Checked = true;
                    this.rb_realenddate.Checked = true;

                    this.dt_realstartdate.Value = DateTime.Now.AddMonths(0).AddDays(-DateTime.Now.Day + 1);
                    this.dt_realstarttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                    this.dt_realenddate.Value = DateTime.Now.AddMonths(0).AddDays(-DateTime.Now.Day + 2);
                    this.dt_realendtime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                    datetimeStr = string.Format("{0} {1}", this.dt_realstartdate.Text, this.dt_realstarttime.Text);
                    this.realstartDatetime = DateTime.Parse(datetimeStr);

                    datetimeStr = string.Format("{0} {1}", this.dt_realenddate.Text, this.dt_realendtime.Text);
                    this.realendDatetime = DateTime.Parse(datetimeStr);

                    this.tb_maxlimit.Text = this.maxshowlimit.ToString();

                    this.rb_sample.Checked = true;
                    this.rb_interval.Checked = false;
                    this.tb_interval.Text = "60";

                    this.lb_systime.Text = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
                    //tagname动态模糊查询
                    //读取所变量信息                    
                    List<DBInterface.RTDBInterface.SignalComm> rtdbtags = new List<DBInterface.RTDBInterface.SignalComm>();
                    rtdbtags = RTDBDAO.ReadTagList();  //读取实时数据库所有变量
                    if (RTDBDAO.ErrorFlag)
                    {
                        MessageBox.Show("读取实时库标签时发生错误，请检查数据库！");
                        return;
                    }
                    this.rtdbtagsdic.Clear();                    
                    foreach (DBInterface.RTDBInterface.SignalComm signal in rtdbtags)
                    {                       
                        string[] tagnamestr = signal.tagname.Split('\\');
                        //rtdbtagsdic.Add(tagnamestr[tagnamestr.Length - 1], signal.tagname.Replace("\\","^"));
                        this.rtdbtagsdic.Add(tagnamestr[tagnamestr.Length - 1], signal.tagname);
                    }                    
                    string[] rtdbdata = new string[this.rtdbtagsdic.Keys.Count];
                    this.rtdbtagsdic.Keys.CopyTo(rtdbdata, 0);
                    this.cb_realtagname.Items.Clear();
                    this.cb_realtagname.Items.AddRange(rtdbdata);                    
                    this.cb_realtagname.TextUpdate += (a, b) =>
                    {
                        var input = this.cb_realtagname.Text.ToUpper();
                        this.cb_realtagname.Items.Clear();
                        if (string.IsNullOrWhiteSpace(input)) this.cb_realtagname.Items.AddRange(rtdbdata);
                        else
                        {
                            var newList = rtdbdata.Where(x => x.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) != -1).ToArray();
                            this.cb_realtagname.Items.AddRange(newList);
                        }
                        this.cb_realtagname.Select(this.cb_realtagname.Text.Length, 0);
                        this.cb_realtagname.DroppedDown = true;
                        //保持鼠标指针形状
                        Cursor = Cursors.Default;
                    };
                    break;
                case 2:
                    //***********************opc数据控件****************************
                    this.rb_opctagname.Checked = true;
                    this.rb_opcenddate.Checked = true;

                    this.dt_opcstartdate.Value = DateTime.Now.AddMonths(0).AddDays(-DateTime.Now.Day + 1);
                    this.dt_opcstarttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                    this.dt_opcenddate.Value = DateTime.Now.AddMonths(0).AddDays(-DateTime.Now.Day + 2);
                    this.dt_opcendtime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

                    datetimeStr = string.Format("{0} {1}", this.dt_opcstartdate.Text, this.dt_opcstarttime.Text);
                    this.opcstartDatetime = DateTime.Parse(datetimeStr);

                    datetimeStr = string.Format("{0} {1}", this.dt_opcenddate.Text, this.dt_opcendtime.Text);
                    this.opcendDatetime = DateTime.Parse(datetimeStr);

                    this.tb_opcmaxlimit.Text = this.opcmaxshowlimit.ToString();

                    //tagname动态模糊查询
                    //读取所有变量信息                    
                    List<OPCTagIdMap> opctags = new List<OPCTagIdMap>();                    
                    opctags = OPCDAO.ReadTagIdMap();  //读取opc数据库所有变量
                    if (OPCDAO.ErrorFlag)
                    {
                        MessageBox.Show("读取OPC标签时发生错误，请检查数据库！");
                        return;
                    }                    
                    this.opctagsdic.Clear();
                    foreach (OPCTagIdMap tag in opctags)
                    {
                        this.opctagsdic.Add(tag.opcdbtagname, tag.opcdbtagname);
                    }
                    string[] opcdata = new string[this.opctagsdic.Keys.Count];
                    this.opctagsdic.Keys.CopyTo(opcdata, 0);
                    this.cb_opctagname.Items.Clear();
                    this.cb_opctagname.Items.AddRange(opcdata);                    
                    this.cb_opctagname.TextUpdate += (a, b) =>
                    {
                        var input = this.cb_opctagname.Text.ToUpper();
                        this.cb_opctagname.Items.Clear();
                        if (string.IsNullOrWhiteSpace(input)) this.cb_opctagname.Items.AddRange(opcdata);
                        else
                        {
                            var newList = opcdata.Where(x => x.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) != -1).ToArray();
                            this.cb_opctagname.Items.AddRange(newList);
                        }
                        this.cb_opctagname.Select(this.cb_opctagname.Text.Length, 0);
                        this.cb_opctagname.DroppedDown = true;
                        //保持鼠标指针形状
                        Cursor = Cursors.Default;
                    };
                    break;

            }
        }

        //***********************计算结果查询*******************************
        //起始日期
        private void dt_startdate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_startdate.Text, this.dt_starttime.Text);
            this.startDatetime = DateTime.Parse(datetimeStr);
        }
        //起始时间
        private void dt_starttime_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_startdate.Text, this.dt_starttime.Text);
            this.startDatetime = DateTime.Parse(datetimeStr);
        }
        //截止日期
        private void dt_enddate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_enddate.Text, this.dt_endtime.Text);
            this.endDatetime = DateTime.Parse(datetimeStr);
        }

        //截止时间
        private void dt_endtime_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_enddate.Text, this.dt_endtime.Text);
            this.endDatetime = DateTime.Parse(datetimeStr);
        }
        //tagid
        private void tb_tagid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x20) e.KeyChar = (char)0;  //禁止空格键  
            if ((e.KeyChar == 0x2D) && (((TextBox)sender).Text.Length == 0)) return;   //处理负数  
            if (e.KeyChar > 0x20)
            {
                try
                {
                    double.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符  
                }
            }  
        }
        private void tb_tagid_TextChanged(object sender, EventArgs e)
        {
            this.tagid = System.UInt32.Parse(this.tb_tagid.Text);
        }
        //读取数量
        private void tb_readnumber_TextChanged(object sender, EventArgs e)
        {           
            this.pslmaxshowlimit = int.Parse(this.tb_pslmaxlimit.Text);
        }
        //标签名称
        private void cb_tagname_TextChanged(object sender, EventArgs e)
        {
            this.tagname = this.cb_tagname.Text;
        }
        private void cb_tagname_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tagname = this.cb_tagname.Text;
        }
        //查询
        //单点查询：标签名在this.tagname中，读取结果在this.pslresults
        //多点查询：标签名在this.tagname中，读取结果在pslresultsMulti

        private void bt_Search_Click(object sender, EventArgs e)
        {
            try
            {
                string PSLSpan = "";
                this.lb_resultInfo.Text = String.Format("正在查询......");
                Application.DoEvents(); //等待界面更新
                //***************************************************************************单点查询接口PSLData.Read（）
                if (this.cb_tagtype.Text == "rdb")
                {
                    if (rb_tagname.Checked)
                    {
                        if (this.tagname == "")
                        {
                            MessageBox.Show("标签名称不能为空！");
                            return;
                        }
                        if (this.tagname.Contains(";"))
                        {
                            MessageBox.Show("标签名称中含有分号';'，如果要读取多个标签，请选择类型rdbset！");
                            return;
                        }

                    }
                    
                    this.lv_calcuresults.Items.Clear();
                    //按照起始时间和截止时间查询
                    if (rb_endtime.Checked)
                    {
                        if (rb_tagname.Checked)
                        {                           
                            this.tagid = this.tagsdic[this.tagname];                           
                        }
                        else
                        {
                            //tagid = tagid;
                        }
                        //按照起始时间和截止时间查询
                        var swPSLTimer = Stopwatch.StartNew();                           
                        this.pslresults = PSLDataDAO.Read(this.tagid, this.startDatetime, this.endDatetime);
                      
                        //results = TestData_MAnalogReadCurrent.SimuData()[1];
                        PSLSpan = swPSLTimer.Elapsed.ToString();
                    }
                    //结果处理
                    if (PSLDataDAO.ErrorFlag)
                    {
                        string messageinfo = String.Format("查询过程中出错，PSLDataDAO.Read请检查log文件！" + Environment.NewLine +
                                                           "——详细错误信息：" + PSLDataDAO.ErrorInfo
                                                            );
                        MessageBox.Show(messageinfo);
                        return;

                    }
                    if (this.pslresults == null || this.pslresults.Count == 0)
                    {
                        this.lb_resultInfo.Text = String.Format("共查询到{0}条记录！", 0);
                        string messageinfo = String.Format("查询结果为空，没有任何符合条件的数据！");
                        MessageBox.Show(messageinfo);
                        return;
                    }
                    else
                    {
                        long totalvalid = this.pslresults.Count(n => n.Status == 0);

                        this.lb_resultInfo.Text = String.Format("共查询到{0}条记录，其中状态位正常的有{1}条！", this.pslresults.Count, totalvalid);
                        this.lb_pslspan.Text = PSLSpan;

                        List<PValue> pslresultssub = new List<PValue>();

                        if (this.pslresults.Count > this.pslmaxshowlimit)
                        {
                            pslresultssub = this.pslresults.Take(this.pslmaxshowlimit).ToList<PValue>();
                        }
                        else
                        {
                            pslresultssub = this.pslresults;
                        }
                        this.updateLV(pslresultssub);
                    }
                }
                //***************************************************************************多点查询接口PSLData.ReadMulti（）
                else if (this.cb_tagtype.Text == "rdbset")
                { 
                    if (rb_tagname.Checked)
                    {
                        if (this.tagname == "")
                        {
                            MessageBox.Show("标签名称不能为空！");
                            return;
                        }

                    }
                    this.lv_calcuresults.Items.Clear();
                    //按照起始时间和截止时间查询
                    this.tagids = new List<System.UInt32>();
                    if (rb_endtime.Checked)
                    {
                        if (rb_tagname.Checked)
                        {
                            string[] tagnameStr = this.tagname.Trim().Split(';');
                            for (int i = 0; i < tagnameStr.Length; i++)
                            {
                                this.tagids.Add(this.tagsdic[tagnameStr[i]]);
                            }
                        }
                        else
                        {
                            //tagid = tagid;
                        }
                        //按照起始时间和截止时间查询
                        var swPSLTimer = Stopwatch.StartNew();
                        this.pslresultsMulti = PSLDataDAO.ReadMulti(this.tagids.ToArray(), this.startDatetime, this.endDatetime);
                        //results = TestData_MAnalogReadCurrent.SimuData()[1];
                        PSLSpan = swPSLTimer.Elapsed.ToString();
                    }
                    //结果处理
                    if (PSLDataDAO.ErrorFlag)
                    {
                        string messageinfo = String.Format("查询过程中出错，PSLDataDAO.ReadMulti请检查log文件！" + Environment.NewLine +
                                                           "——详细错误信息：" + PSLDataDAO.ErrorInfo
                                                            );
                        MessageBox.Show(messageinfo);
                        return;

                    }
                    if (this.pslresultsMulti == null || this.pslresultsMulti.Length == 0)
                    {
                        this.lb_resultInfo.Text = String.Format("共查询到{0}条记录！", 0);
                        string messageinfo = String.Format("查询结果为空，没有任何符合条件的数据！");
                        MessageBox.Show(messageinfo);
                        return;
                    }
                    else
                    {
                        //准计算结果
                        long totalvalid = 0;
                        
                        long totalcount = 0;
                        for (int i = 0; i < this.pslresultsMulti.Length; i++)
                        {
                            
                            if (this.pslresultsMulti[i] != null && this.pslresultsMulti[i].Count !=0)
                            {
                                totalcount += this.pslresultsMulti[i].Count;                            //取到的所有数据
                                totalvalid += this.pslresultsMulti[i].Count(n => n.Status == 0);        //取到的有效数据                                                              
                            }
                        }
                        //提示结果
                        if(totalcount == 0)
                        {
                            this.lb_resultInfo.Text = String.Format("共查询到{0}条记录！", 0);
                            string messageinfo = String.Format("查询结果为空，没有任何符合条件的数据！");
                            MessageBox.Show(messageinfo);
                            return;
                        }
                        //更新listview
                        for (int i = 0; i < this.pslresultsMulti.Length; i++)
                        {
                            List<PValue> pslresultssub = new List<PValue>();
                            //按界面长度截取
                            if (this.pslresultsMulti[i].Count > this.pslmaxshowlimit)
                            {
                                pslresultssub = this.pslresultsMulti[i].Take(this.pslmaxshowlimit).ToList<PValue>();
                            }
                            else
                            {
                                pslresultssub = this.pslresultsMulti[i];
                            }
                            //更新listview，更新listview时，根据this.tagid来显示id好
                            this.tagid = this.tagids[i];
                            if (i == 0) this.updateLV(pslresultssub);
                            else this.addLV(pslresultssub);
                        }                       

                        //更新UI
                        this.lb_resultInfo.Text = String.Format("共查询到{0}条记录，其中状态位正常的有{1}条！", totalcount, totalvalid);
                        this.lb_pslspan.Text = PSLSpan;

                    }
                
                }
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("查询计算结果时发生错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);

                string messageinfo = String.Format("查询过程中出错，请检查log文件！");
                MessageBox.Show(messageinfo);
            }

        }
        //删除
        private void bt_delete_Click(object sender, EventArgs e)
        {
            this.lb_resultInfo.Text = String.Format("正在删除......");
            Application.DoEvents(); //等待界面更新
            if (rb_tagname.Checked)
            {
                if (this.tagname == "")
                {
                    MessageBox.Show("标签名称不能为空！");
                    return;
                }

            }
            this.lv_calcuresults.Items.Clear();
            //按照起始时间和截止时间删除
            if (rb_endtime.Checked)
            {
                if (rb_tagname.Checked)
                {
                    this.tagid = this.tagsdic[this.tagname];
                }
                else
                {
                    //tagid = tagid;
                }
                System.UInt32[] tagids=new  System.UInt32[1]{this.tagid};
                this.psldeletecount = PSLDataDAO.Delete(tagids, this.startDatetime, this.endDatetime);
                //results = TestData_MAnalogReadCurrent.SimuData()[1];
            }
            if (PSLDataDAO.ErrorFlag)
            {
                string messageinfo = String.Format("删除过程中出错，PSLDataDAO.Read请检查log文件！" + Environment.NewLine +
                                                   "——详细错误信息：" + PSLDataDAO.ErrorInfo
                                                    );
                MessageBox.Show(messageinfo);
                return;
            }
            this.lb_resultInfo.Text = String.Format("共删除{0}条记录！", 0);            
            MessageBox.Show("数据已删除！");
        }

        //更新listview        
        private void updateLV(List<PValue> results)
        {
            //清空listview的已有数据项
            this.lv_calcuresults.Items.Clear();
            //根据results添加
            for (int i = 0; i < results.Count; i++)              //为listview添加 items.Count行数据
            {
                PValue pv = results[i];
                ListViewItem lvitem = this.lv_calcuresults.Items.Add((i + 1).ToString(), (i + 1).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.SubItems.Add(this.tagid.ToString());
                lvitem.SubItems.Add(results[i].Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Endtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Value.ToString(""));
                lvitem.SubItems.Add(results[i].Status.ToString(""));
            }
        }
        //添加listview        
        private void addLV(List<PValue> results)
        {            
            //根据results添加
            for (int i = 0; i < results.Count; i++)              //为listview添加 items.Count行数据
            {
                PValue pv = results[i];
                ListViewItem lvitem = this.lv_calcuresults.Items.Add((i + 1).ToString(), (i + 1).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.SubItems.Add(this.tagid.ToString());
                lvitem.SubItems.Add(results[i].Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Endtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Value.ToString(""));
                lvitem.SubItems.Add(results[i].Status.ToString(""));
            }
        }
        //显示数据曲线       
        private void bt_DataTrend_Click(object sender, EventArgs e)
        {
            //
            

            DataGraph datatrend = new DataGraph();
            datatrend.startDatetime = this.startDatetime;
            datatrend.endDatetime = this.endDatetime;
            datatrend.startTop = this.Top;
            datatrend.startLeft = this.Right;
            datatrend.windowWidth = 900;
            datatrend.windowHeigh = 700;

            datatrend.tagnames = this.tagname.Split(';');
            if (datatrend.tagnames.Length <= 1)
                datatrend.searchData = new List<PValue>[1] { this.pslresults };
            else
                datatrend.searchData = this.pslresultsMulti;
            
            datatrend.Show();
        }
        //概化数据，导出数据
        private void bt_pslexport_Click(object sender, EventArgs e)
        {
            try
            {
                string[] tagnamesStr = this.tagname.Trim().Split(';');
                for (int i = 0; i < tagnamesStr.Length;i++ )
                {
                    if (this.pslresults == null) return;
                    String filePath = "D:\\RealDataExport\\";
                    string filename = tagnamesStr[i].Replace("\\", "^") + this.startDatetime.ToString("_S_yyyy-MM-dd_HH-mm-ss") + this.endDatetime.ToString("_E_yyyy-MM-dd_HH-mm-ss") + "_PSL" + DateTime.Now.ToString("HHmmss") + ".csv";
                    //检查路径
                    if (Directory.Exists(filePath) == false)//如果不存在就创建file文件夹
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    //检查文件名称，因为标签名称中可能会含有非法字符。
                    //将所有非法字符替换成_
                    if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    {
                        foreach (char c in Path.GetInvalidFileNameChars())
                        {
                            filename = filename.Replace(c, '_');
                        }
                    }
                    //查看文件是否存在
                    if (File.Exists(filePath+filename) == true)
                    {
                        File.Delete(filePath + filename);
                    }

                    string[][] csvData;
                    if(this.cb_tagtype.Text == "rdb")                   //rdb类型，只能读取一个标签
                        csvData= PValue2Array(this.pslresults);
                    else
                        csvData = PValue2Array(this.pslresultsMulti[i]);
                    CsvFileReader.Save(csvData, filePath + filename);
                }
                string messageinfo = string.Format("概化数据导出完成！");
                MessageBox.Show(messageinfo);
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("导出计算数据时发生错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);

                string messageinfo = string.Format("导出计算数据过程中出错，请检查log文件！");
                MessageBox.Show(messageinfo);
            }
        }
        
        //***********************实时数据查询*******************************
        //起始日期
        private void dt_realstartdate_ValueChanged(object sender, EventArgs e)
        {
            string realdatetimeStr = string.Format("{0} {1}", this.dt_realstartdate.Text, this.dt_realstarttime.Text);
            this.realstartDatetime = DateTime.Parse(realdatetimeStr);
        }
        //起始时间
        private void dt_realstarttime_ValueChanged(object sender, EventArgs e)
        {
            string realdatetimeStr = string.Format("{0} {1}", this.dt_realstartdate.Text, this.dt_realstarttime.Text);
            this.realstartDatetime = DateTime.Parse(realdatetimeStr);
        }
        //截止日期
        private void dt_realenddate_ValueChanged(object sender, EventArgs e)
        {
            string realdatetimeStr = string.Format("{0} {1}", this.dt_realenddate.Text, this.dt_realendtime.Text);
            this.realendDatetime = DateTime.Parse(realdatetimeStr);
        }
        //截止时间
        private void dt_realendtime_ValueChanged(object sender, EventArgs e)
        {
            string realdatetimeStr = string.Format("{0} {1}", this.dt_realenddate.Text, this.dt_realendtime.Text);
            this.realendDatetime = DateTime.Parse(realdatetimeStr);
        }
        //标签名称
        //历史计算按钮   
        private void cb_realtagname_MouseHover(object sender, EventArgs e)
        {
            string MessageStr = "实时数据标签填写注意事项：" + Environment.NewLine +
                              "——对于没有获取标签列表功能的数据库，这里必须直接填写标签全路径名称。比如西安热工院数据库。" + Environment.NewLine +
                              "——对于可以获得标签列表的数据库，这里仅需填写标签名称，后台自动获取全路径名称。如PI、PGIM。" + Environment.NewLine +
                              "——标签可以在后面以逗号分隔添加读取标记，以便以不同的方式获取查询结果。" + Environment.NewLine +
                              "  例如：\"tag01,inner\"，读取标记inner，是获取不带起止时刻插值点的查询结果。"
                              ;
            tooltip.Show(MessageStr, cb_realtagname, cb_realtagname.Width / 2, -cb_realtagname.Height * 5);
        }
        private void cb_realtagname_MouseLeave(object sender, EventArgs e)
        {
            tooltip.Hide(cb_realtagname);
        }
       
        private void cb_realtagname_TextChanged(object sender, EventArgs e)
        {
            this.realtagname = "";
            try
            {
                if (this.rtdbtagsdic != null && this.rtdbtagsdic.Count!=0)
                {   
                    //对于可以获得标签列表功能的数据库，输入框输入的仅仅是标签名称，不带路径。
                    string[] tagnameStrs = this.cb_realtagname.Text.Split(',');     //this.cb_realtagname.Text中可能含有标记部分，比如",inner"或者其他
                    if (tagnameStrs.Length==1)
                        this.realtagname = this.rtdbtagsdic[tagnameStrs[0]];
                    else
                        this.realtagname = this.rtdbtagsdic[tagnameStrs[0]]+","+tagnameStrs[1];
                }
                else
                    //对于没有获取标签列表的数据库
                    this.realtagname = this.cb_realtagname.Text;
                
                //如果上述判断均有问题，则直接给realtagname赋值
                if (this.realtagname == "") this.realtagname = this.cb_realtagname.Text;
            }
            catch 
            {
                this.realtagname = this.cb_realtagname.Text;
            }
        }
        private void cb_realtagname_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.realtagname = "";
            try
            {
                if (this.rtdbtagsdic != null && this.rtdbtagsdic.Count != 0)
                {
                    //对于可以获得标签列表功能的数据库，输入框输入的仅仅是标签名称，不带路径。
                    string[] tagnameStrs = this.cb_realtagname.Text.Split(',');     //this.cb_realtagname.Text中可能含有标记部分，比如",inner"或者其他
                    if (tagnameStrs.Length == 1)
                        this.realtagname = this.rtdbtagsdic[tagnameStrs[0]];
                    else
                        this.realtagname = this.rtdbtagsdic[tagnameStrs[0]] + "," + tagnameStrs[1];
                }
                else
                    //对于没有获取标签列表的数据库
                    this.realtagname = this.cb_realtagname.Text;

                //如果上述判断均有问题，则直接给realtagname赋值
                if (this.realtagname == "") this.realtagname = this.cb_realtagname.Text;
            }
            catch 
            {
                this.realtagname = this.cb_realtagname.Text;
            }
        }
        //最多显示的数据行数
        private void tb_maxlimit_TextChanged(object sender, EventArgs e)
        {
            this.maxshowlimit = int.Parse(this.tb_maxlimit.Text);
        }
        //查询
        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                this.lb_realresultInfo.Text = String.Format("正在查询......");
                Application.DoEvents(); //等待界面更新
                if (this.realtagname == "")
                {
                    MessageBox.Show("标签名称不能为空！");
                    return;
                }

                this.lv_realpvalue.Items.Clear();
                
                //按照起始时间和截止时间查询
                var swRealTimer = Stopwatch.StartNew();
                if(this.rb_sample.Checked)
                    this.realresults = RTDBDAO.ReadData(this.realtagname, this.realstartDatetime, this.realendDatetime);
                else if(this.rb_interval.Checked)
                    this.realresults = RTDBDAO.ReadDataInterval(this.realtagname, this.realstartDatetime, this.realendDatetime,int.Parse(this.tb_interval.Text));
                //results = TestData_MAnalogReadCurrent.SimuData()[1];

                string realSpan = swRealTimer.Elapsed.ToString();
                if (RTDBDAO.ErrorFlag)
                {
                    string messageinfo = String.Format("查询过程出错，详细错误信息：{0}", RTDBDAO.ErrorInfo);
                    MessageBox.Show(messageinfo);
                    return;
                }

                if (this.realresults == null || this.realresults.Count == 0)
                {
                    string messageinfo = String.Format("查询结果为空，没有任何符合条件的数据！");
                    MessageBox.Show(messageinfo);
                    return;
                }
                else
                {
                    this.lb_systime.Text = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
                    this.lb_realresultInfo.Text = String.Format("共查询到{0}条记录！", this.realresults.Count);
                    this.lb_realspan.Text = realSpan;
                    List<PValue> resultssub = new List<PValue>();

                    if (this.realresults.Count > this.maxshowlimit)
                    {
                        resultssub = this.realresults.Take(this.maxshowlimit).ToList<PValue>();
                    }
                    else
                    {
                        resultssub = this.realresults;
                    }
                    this.updateLVreal(resultssub);
                   
                }
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("查询计算实时数据时发生错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);

                string messageinfo = string.Format("查询过程中出错，请检查log文件！");
                MessageBox.Show(messageinfo);
            }
        }
        //更新listview        
        private void updateLVreal(List<PValue> results)
        {
            //清空listview的已有数据项
            this.lv_realpvalue.Items.Clear();
            //根据results添加
            for (int i = 0; i < results.Count; i++)              //为listview添加 items.Count行数据
            {
                PValue pv = results[i];
                ListViewItem lvitem = this.lv_realpvalue.Items.Add((i + 1).ToString(), (i + 1).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.SubItems.Add(this.cb_realtagname.Text);
                lvitem.SubItems.Add(results[i].Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Endtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Value.ToString(""));
                lvitem.SubItems.Add(results[i].Status.ToString(""));
            }
        }
        //导出实时数据
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (this.realresults == null) return;
                string filePath = "D:\\RealDataExport\\";
                string filename;
                //标签名PGIM中的\特别处理
                if (this.rb_sample.Checked)
                    filename = this.realtagname.Replace("\\", "^") + this.realstartDatetime.ToString("_S_yyyy-MM-dd_HH-mm-ss") + this.realendDatetime.ToString("_E_yyyy-MM-dd_HH-mm-ss") + "_sample" + DateTime.Now.ToString("HHmmss") + ".csv";
                else if (this.rb_interval.Checked)
                    filename = this.realtagname.Replace("\\", "^") + this.realstartDatetime.ToString("_S_yyyy-MM-dd_HH-mm-ss") + this.realendDatetime.ToString("_E_yyyy-MM-dd_HH-mm-ss") + "_interval" + DateTime.Now.ToString("HHmmss") + ".csv";
                else
                    filename = "";
                //检查路径
                if (Directory.Exists(filePath) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(filePath);
                }
                //检查文件名称，因为标签名称中可能会含有非法字符。
                //将所有非法字符替换成_
                if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        filename = filename.Replace(c, '_');
                    }
                }
                if (File.Exists(filePath + filename) == true)
                {
                    File.Delete(filePath + filename);
                }

                string[][] csvData = PValue2Array(this.realresults);
                CsvFileReader.Save(csvData, filePath + filename);
                string messageinfo = string.Format("实时数据导出完成！");
                MessageBox.Show(messageinfo);
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("导出实时数据时发生错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);

                string messageinfo = string.Format("导出实时数据过程中出错，请检查log文件！");
                MessageBox.Show(messageinfo);
            }
        }

        //***********************opc数据查询*******************************
        //起始日期
        private void dt_opcstartdate_ValueChanged(object sender, EventArgs e)
        {
            string opcdatetimeStr = string.Format("{0} {1}", this.dt_opcstartdate.Text, this.dt_opcstarttime.Text);
            this.opcstartDatetime = DateTime.Parse(opcdatetimeStr);
        }
        //起始时间
        private void dt_opcstarttime_ValueChanged(object sender, EventArgs e)
        {
            string opcdatetimeStr = string.Format("{0} {1}", this.dt_opcstartdate.Text, this.dt_opcstarttime.Text);
            this.opcstartDatetime = DateTime.Parse(opcdatetimeStr);
        }
        //截止日期
        private void dt_opcenddate_ValueChanged(object sender, EventArgs e)
        {
            string opcdatetimeStr = string.Format("{0} {1}", this.dt_opcenddate.Text, this.dt_opcendtime.Text);
            this.opcendDatetime = DateTime.Parse(opcdatetimeStr);
        }
        //截止时间
        private void dt_opcendtime_ValueChanged(object sender, EventArgs e)
        {
            string opcdatetimeStr = string.Format("{0} {1}", this.dt_opcenddate.Text, this.dt_opcendtime.Text);
            this.opcendDatetime = DateTime.Parse(opcdatetimeStr);
        }
        //标签名称
        private void cb_opctagname_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.opctagname = this.opctagsdic[this.cb_opctagname.Text];
            }
            catch { }
        }
        private void cb_opctagname_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
            this.opctagname = this.opctagsdic[this.cb_opctagname.Text];
            }
            catch { }
        }
        //最多显示的数据行数
        private void tb_opcmaxlimit_TextChanged(object sender, EventArgs e)
        {
            this.opcmaxshowlimit = int.Parse(this.tb_opcmaxlimit.Text);
        }
        //查询
        private void OPCServer_Click(object sender, EventArgs e)
        {
            try
            {
                this.lb_opcresultInfo.Text = String.Format("正在查询......");
                Application.DoEvents(); //等待界面更新
                if (this.opctagname == "")
                {
                    MessageBox.Show("标签名称不能为空！");
                    return;
                }

                this.lv_opcpvalue.Items.Clear();
                //按照起始时间和截止时间查询

                this.opcresults = OPCDAO.Read(this.opctagname, this.opcstartDatetime, this.opcendDatetime);
                if (OPCDAO.ErrorFlag)
                {
                    MessageBox.Show("读取OPC数据时发生错误，请检查！");
                    return;
                }
                //results = TestData_MAnalogReadCurrent.SimuData()[1];
                if (this.opcresults == null || this.opcresults.Count==0)
                {
                    this.lb_opcresultInfo.Text = String.Format("共查询到{0}条记录！",0);
                    string messageinfo = String.Format("查询结果为空，没有任何符合条件的数据！");
                    MessageBox.Show(messageinfo);
                }
                else
                {
                    this.lb_opcresultInfo.Text = String.Format("共查询到{0}条记录！", this.opcresults.Count);
                    List<PValue> opcresultssub = new List<PValue>();

                    if (this.opcresults.Count > this.opcmaxshowlimit)
                    {
                        opcresultssub = this.opcresults.Take(this.opcmaxshowlimit).ToList<PValue>();
                    }
                    else
                    {
                        opcresultssub = this.opcresults;
                    }
                    this.updateLVopc(opcresultssub);

                }
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("查询OPC数据时发生错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);

                string messageinfo = string.Format("查询过程中出错，请检查log文件！");
                MessageBox.Show(messageinfo);
            }
        }
        //更新listview        
        private void updateLVopc(List<PValue> results)
        {
            //清空listview的已有数据项
            this.lv_opcpvalue.Items.Clear();
            //根据results添加
            for (int i = 0; i < results.Count; i++)              //为listview添加 items.Count行数据
            {
                PValue pv = results[i];
                ListViewItem lvitem = this.lv_opcpvalue.Items.Add((i + 1).ToString(), (i + 1).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.SubItems.Add(this.cb_opctagname.Text);
                lvitem.SubItems.Add(results[i].Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Endtime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lvitem.SubItems.Add(results[i].Value.ToString(""));
                lvitem.SubItems.Add(results[i].Status.ToString(""));
            }
        }
        //导出实时数据
        private void ExportOPC_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.opcresults == null) return;
                String filePath = "D:\\RealDataExport";
                string filename = filePath + "\\" + this.opctagname.Replace("\\", "^") + this.opcstartDatetime.ToString("_S_yyyy-MM-dd_HH-mm-ss") + this.opcendDatetime.ToString("_E_yyyy-MM-dd_HH-mm-ss") + ".csv";

                if (Directory.Exists(filePath) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(filePath);
                }
                //检查文件名称，因为标签名称中可能会含有非法字符。
                //将所有非法字符替换成_
                if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        filename = filename.Replace(c, '_');
                    }
                }
                if (File.Exists(filename) == true)
                {
                    File.Delete(filename);
                }

                string[][] csvData = PValue2Array(this.opcresults);
                CsvFileReader.Save(csvData, filename);
                string messageinfo = string.Format("OPC数据导出完成！");
                MessageBox.Show(messageinfo);
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("导出实时数据时发生错误：---------->");
                logHelper.Error(messageStr);
                messageStr = String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);

                string messageinfo = string.Format("导出实时数据过程中出错，请检查log文件！");
                MessageBox.Show(messageinfo);
            }
        }
        
        //辅助函数
        public static string[][] PValue2Array(List<PValue> pvalues)
        {
            //pvalues带截止时刻值
            string[][] results;
            if (pvalues.Count > 0)
            {
                results = new string[pvalues.Count][];       //pvalues包含截止时刻数据，转换的results也包含截止时刻值
                for (int i = 0; i < pvalues.Count; i++)
                {
                    results[i] = new string[4];
                    results[i][0] = pvalues[i].Value.ToString();
                    results[i][1] = pvalues[i].Timestamp.ToString();                    
                    results[i][2] = pvalues[i].Status.ToString();
                    results[i][3] = pvalues[i].Endtime.ToString();      //注意，endtime之所以要放到status后面，是为了保持和实时库导出的数据格式一致。
                }
                /*
                results[pvalues.Count] = new string[3];
                results[pvalues.Count][0] = pvalues[pvalues.Count-1].Value.ToString();
                results[pvalues.Count][1] = pvalues[pvalues.Count-1].Endtime.ToString();
                results[pvalues.Count][2] = pvalues[pvalues.Count-1].Status.ToString();
                */
                return results;
            }
            else
            {
                return null;
            }
        }

        private void lb_realspan_Click(object sender, EventArgs e)
        {

        }

       

       

       

       

       
       

       

       

       





    }

}
