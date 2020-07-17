using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using PCCommon;                         //使用PValue
using System.Text.RegularExpressions;   //使用正则表达式
using DBInterface.RTDBInterface;        //使用实时数据库接口
using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Diagnostics;               //使用计时器
using System.Threading;                 //使用线程
using System.Threading.Tasks;           //使用任务
using System.Collections.Concurrent;    //使用并发集合
using Config;                           //使用Config配置
using System.Reflection;                //使用反射
using PSLCalcu.Module;                  //使用计算模块

namespace PSLCalcu
{
   
    public partial class HistoryCalcu : Form
    {
        private LogHelper logHelper = LogFactory.GetLogger(typeof(HistoryCalcu));       //全局log
        private int RTDB_PERIOD;                //实时数据并发计算的并发周期（包括RTDB和OPC）
        private int PSLDB_PEROID;               //概化数据并发计算周期

        private long errorCount;                //历史数据计算错误统计
        private bool errorFlag;                 //历史数据计算错误标志
        private long warningCount;              //历史数据计算警告统计
        private bool warningFlag;               //历史数据计算警告标志

        private bool flag_selectall;            //全选标志
        private bool flag_selectintervaltype;   //周期类型选择标志
        private bool flag_selectmodulename;     //计算类型选择标志

        private bool flag_selectallDown;            //全选标志——界面下部参与计算list
        private bool flag_selectintervaltypeDown;   //周期类型选择标志——界面下部参与计算list
        private bool flag_selectmodulenameDown;     //计算类型选择标志——界面下部参与计算list

        private ToolTip tooltip;

        private List<PSLCalcuItem> PslCalcuItems;                //计算配置对象集
        private List<PSLCalcuItem> FilterCalcuItems;             //过滤算法对象集
        private List<PSLCalcuItem> SelectedCalcuItems;           //选定重算对象集
        private List<PSLCalcuItem> CurrentCalcuItems;            //并发计算的对象集(可以在组织在一起进行一次并发计算的对象)
        private int startIndex, endIndex;                        //并发计算的对象集currentCalcuItems对应在选定重算对象集SelectedCalcuItems中的起始和截止位置
        private DateTime StartDateForHistoryCalcu;               //重算起始时间
        private DateTime EndDateForHistoryCalcu;                 //重算结束时间
        private Dictionary<string, System.UInt32> TagName2Id;    //概化计算标签名称id号字典

        private bool FlagHistoryCalcuRunning=false;              //历史计算正在进行标志

        public NotifyIcon notifyIcon1;                           //获得主界面的notifyIcon1
        public string[] IntervalType;

        #region 界面初始化
        public HistoryCalcu(List<PSLCalcuItem> pslcalcuitems)
        {
            InitializeComponent();
            
            //实例化ToolTip控件
            tooltip = new ToolTip();
            //设置提示框显示时间，默认5000，最大为32767，超过此数，将以默认5000显示           
            tooltip.AutoPopDelay = 30000;
            //是否以球状显示提示框            
            tooltip.IsBalloon = true;
            

            //全局变量初始化
            this.PslCalcuItems = pslcalcuitems;                  //通过入参，获得来自主界面的计算配置对象变量pslcalcuitems
            this.FilterCalcuItems = pslcalcuitems;               //过滤窗口的过滤计算配置对象变量，初始值为所有的pslcalcuitems
            this.SelectedCalcuItems = new List<PSLCalcuItem>();  //选定窗口的选定计算配置对象变量    
            
        }
        private void HistoryCalcu_Load(object sender, EventArgs e)
        {
            //上面的间隔类型
            this.cb_intervaltype.Items.Clear();
            this.cb_intervaltype.Items.AddRange(this.IntervalType);
            this.cb_intervaltype.SelectedIndex = 2;

            //下面的间隔类型
            this.cb_intervaltypeDown.Items.Clear();
            this.cb_intervaltypeDown.Items.AddRange(this.IntervalType);
            this.cb_intervaltypeDown.SelectedIndex = 2;

            //界面控件初始化                   
            this.cb_CheckTags.Checked = false;
            this.Initial_lv_filtercalcu(this.FilterCalcuItems);                                      //初始化过滤窗口，初始值为pslcalcuitems

            this.dtStartDate.Value = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);                 //初始化计算时间段，开始时间：以结束时间向前一年为开始时间。
            this.dtEndDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);     //初始化计算时间段，结束时间：以前当系统时间所在月份的第一条0:00:00位结束时间
            //测试时，用来直接设定数据的时间            
            //this.dtStartDate.Value = DateTime.Parse("2018-01-01");
            //this.dtEndDate.Value = DateTime.Parse("2018-04-01");

            //初始化历史数据计算时间段变量，主要是考虑，如果用户采用默认界面值，则这两个变量必须也跟随默认值
            this.StartDateForHistoryCalcu = this.dtStartDate.Value;     //初始化历史数据计算起始时间变量
            this.EndDateForHistoryCalcu = this.dtEndDate.Value;         //初始化历史数据计算结束时间变量

            //分段的时刻点插值方式
            this.rb_step.Checked = true;                //按阶梯型取前值
            this.rb_insert.Checked = false;             //按折线型取插值
        }
        #endregion

        #region 按钮
        //全选按钮mousedown
        private void cb_SelectAllForFilter_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectall = true;                          //选中“全选”框时，马上将全选的标志位置1。因为此时，程序也同时会去调用“全选”的CheckedChanged。只有全选的标志位为true，这个程序内部才能执行

            flag_selectintervaltype = false;                //将“选间隔类型”标志位，“选算法”标志位置0.这样，在下面两个checked状态改变时，才不会去执行对应的改变程序。
            flag_selectmodulename = false;
            cb_SelectIntervalType.Checked = false;    //上面将标志位置0后，再来改变对象的check，对应的changed程序会触发，但是内部被标志位屏蔽。
            //cb_SelectModule.Checked = false;
        }
        //全选按钮：过滤计算项的全选
        private void checkBox_SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_selectall)
            {
                for (int i = 0; i < lv_FilterCalcuList.Items.Count; i++)
                {
                    lv_FilterCalcuList.Items[i].Checked = cb_SelectAllForFilter.Checked;
                }
            }
        }
        //按周期类型选择
        private void cb_SelectIntervalType_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectintervaltype = true;

            flag_selectall = false;
            flag_selectmodulename = false;
            cb_SelectAllForFilter.Checked = false;
            //cb_SelectModule.Checked = false;
        }
        private void cb_SelectIntervalType_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_selectintervaltype == true)
            {

                for (int i = 0; i < lv_FilterCalcuList.Items.Count; i++)
                {
                    string intervalStr = lv_FilterCalcuList.Items[i].SubItems[17].Text;
                    if (intervalStr.Contains(this.cb_intervaltype.Text))
                    {
                        lv_FilterCalcuList.Items[i].Checked = this.cb_SelectIntervalType.Checked; ;
                    }
                    else
                    {
                        lv_FilterCalcuList.Items[i].Checked = false;
                    }
                }
            }
        }
        //周期选择下拉菜单
        private void cb_intervaltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cb_SelectIntervalType_CheckedChanged(sender, e);
        }
        //搜索按钮：按tagname包含字母搜索
        private void bt_SearchTagName_Click(object sender, EventArgs e)
        {
            string filterStrTagName = tb_FliterTagName.Text.Trim();            
            string filterStrTagDesc = tb_FliterTagDesc.Text.Trim();

            var queryResults = from PSLCalcuItem item in this.PslCalcuItems
                           select item;

            if(filterStrTagName!=null && filterStrTagDesc!=null)
            {
                queryResults = from PSLCalcuItem item in this.PslCalcuItems
                               where item.sourcetagname.Contains(filterStrTagName) && item.sourcetagdesc.Contains(filterStrTagDesc)
                               select item;
            }
            else if(filterStrTagName!=null)
            {
                queryResults = from PSLCalcuItem item in this.PslCalcuItems
                               where item.sourcetagname.Contains(filterStrTagName) 
                               select item;
            }
            else if (filterStrTagDesc != null)
            {
                queryResults = from PSLCalcuItem item in this.PslCalcuItems
                               where item.sourcetagdesc.Contains(filterStrTagDesc)
                               select item;
            }
            else 
            {                
            }
            this.FilterCalcuItems = queryResults.ToList();
            this.Initial_lv_filtercalcu(this.FilterCalcuItems);                  //更新过滤窗口
        }
        //
        //搜索按钮：按tagid包含的id搜索（未完成）
        private void tb_SearchTagId_Click(object sender, EventArgs e)
        {
            //标签id号，可以一次输入多个，如1,3,5,
            //标签id好，也可以用-表示范围，如3-5
            //string tagIdsStr = tb_FilterTagId.Text;
            string tagIdsStr = "";
            List<int> tagIds = new List<int>();           //对tagIds初始化             

            string[] tagIdsArray = tagIdsStr.Split(',');    //对ui获得的字符串用，分隔

            try
            {
                for (int i = 0; i < tagIdsArray.Length; i++)
                {                                               //每一个id如果包含-，则循环处理
                    if (tagIdsArray[i].Contains('-'))
                    {
                        string[] range = tagIdsArray[i].Split('-');
                        for (int j = int.Parse(range[0]); j <= int.Parse(range[1]); j++)
                        {
                            tagIds.Add(j);
                        }
                    }
                    else
                    {                                           //每一个id如果不包含-，则直接取出
                        int tempa = int.Parse(tagIdsArray[i]);
                        tagIds.Add(int.Parse(tagIdsArray[i]));
                    }

                }
                HashSet<int> hs = new HashSet<int>(tagIds);     //去掉tagIds中的重复值
                tagIds = hs.ToList();

            }
            catch
            {
                MessageBox.Show("输入的标签id号格式不对，请重新输入！");
                //this.tb_FilterTagId.Text = "";
                tagIdsStr = "";
                return;
            }
            //目前PslCalcuItems中还没有sourcetagids。
            //等之后增加了sourcetagids属性，在完成此功能
            //参考 http://bbs.csdn.net/topics/390299448?page=1
            //var queryResults = from PSLCalcuItem item in PslCalcuItems
            //                   where item.sourcetagids.Contains(filterStr)
            //                   select item;
            //this.SelectedCalcuItems = queryResults.ToList();
            //Initial_lv_pslcalcu(this.SelectedCalcuItems);                  //更新界面

        }
        //全选按钮mousedown：选定参与历史计算listview的全选
        private void cb_SelectAllForSelected_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectallDown = true;                          //选中“全选”框时，马上将全选的标志位置1。因为此时，程序也同时会去调用“全选”的CheckedChanged。只有全选的标志位为true，这个程序内部才能执行

            flag_selectintervaltypeDown = false;                //将“选间隔类型”标志位，“选算法”标志位置0.这样，在下面两个checked状态改变时，才不会去执行对应的改变程序。
            flag_selectmodulenameDown = false;
            cb_SelectIntervalTypeDown.Checked = false;    //上面将标志位置0后，再来改变对象的check，对应的changed程序会触发，但是内部被标志位屏蔽。
            //cb_SelectModule.Checked = false;
        }
        //全选按钮：选定参与历史计算listview的全选
        private void cb_SelectAllForSelected_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_selectallDown)
            {
                for (int i = 0; i < lv_SelectedCalcuList.Items.Count; i++)
                {
                    lv_SelectedCalcuList.Items[i].Checked = cb_SelectAllForSelected.Checked;
                }
            }
        }
        //按周期类型选择按钮：选定参与历史计算listview的的类型选择
        private void cb_SelectIntervalTypeDown_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectintervaltypeDown = true;

            flag_selectallDown = false;
            flag_selectmodulenameDown = false;
            cb_SelectAllForSelected.Checked = false;
            //cb_SelectModule.Checked = false;
        }
        private void cb_SelectIntervalTypeDown_CheckedChanged(object sender, EventArgs e)
        {
            if (flag_selectintervaltypeDown == true)
            {

                for (int i = 0; i < lv_SelectedCalcuList.Items.Count; i++)
                {
                    string intervalStr = lv_SelectedCalcuList.Items[i].SubItems[17].Text;
                    if (intervalStr.Contains(this.cb_intervaltypeDown.Text))
                    {
                        lv_SelectedCalcuList.Items[i].Checked = this.cb_SelectIntervalTypeDown.Checked; ;
                    }
                    else
                    {
                        lv_SelectedCalcuList.Items[i].Checked = false;
                    }
                }
            }
        }
        //周期选择下拉菜单
        private void cb_intervaltypeDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cb_SelectIntervalTypeDown_CheckedChanged(sender, e);
        }
        //添加按钮
        private void bt_SelectedAdd_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lv_FilterCalcuList.Items.Count; i++)
            {
                if (lv_FilterCalcuList.Items[i].Checked) 
                {
                    this.SelectedCalcuItems.Add(this.FilterCalcuItems[i]);        //这里filterCalcuItems序号和lv_pslcalculist序号一一对应
                    lv_FilterCalcuList.Items[i].Checked = false;
                }
            }

            this.SortSelectedItems();    //在更新选定窗口前，按照历史计算主线程要求对SelectedItems进行排序。
            this.Initial_lv_selectedcalcu(this.SelectedCalcuItems);


        }
        //删除按钮:特别注意，删除时必须从后向前删除
        private void bt_SelectedDelete_Click(object sender, EventArgs e)
        {
            for (int i = lv_SelectedCalcuList.Items.Count-1; i >=0; i--)
            {
                if (lv_SelectedCalcuList.Items[i].Checked)
                {
                    this.SelectedCalcuItems.RemoveAt(i);        //这里filterCalcuItems序号和lv_pslcalculist序号一一对应
                }
            }
            this.SortSelectedItems();    //在更新选定窗口前，按照历史计算主线程要求对SelectedItems进行排序。
            this.Initial_lv_selectedcalcu(this.SelectedCalcuItems);
            this.cb_SelectAllForSelected.Checked = false;
        }
        //保存按钮
        private void bt_SeletecSave_Click(object sender, EventArgs e)
        {
            PSLHistoryCalcuConfigDAO.ClearData();
            bool flag = PSLHistoryCalcuConfigDAO.Write(this.SelectedCalcuItems);
            if (flag)
            {
                MessageBox.Show("历史计算配置信息保存完毕!");
            }
            else
            {
                MessageBox.Show("保存历史计算配置信息错误，请检查数据库连接是否正确!");
            }
        }
        //读取按钮
        private void bt_SeletedLoad_Click(object sender, EventArgs e)
        {

            this.SelectedCalcuItems = PSLHistoryCalcuConfigDAO.Read();       //读取选定的计算配置信息
            
            //概化计算配置对象集对计算引擎进行准备、：转换算法标志为布尔、获取计算结果标签id、获取计算条件标签id
            Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();
            Dictionary<string, bool> TagName2Flag = PSLTagNameIdMapDAO.ReadFlagMap();
            foreach (PSLCalcuItem item in this.SelectedCalcuItems) { item.Prepare(ref TagName2Id,ref TagName2Flag); };

            //this.SortSelectedItems();    //在更新选定窗口前，按照历史计算主线程要求对SelectedItems进行排序。
            this.Initial_lv_selectedcalcu(this.SelectedCalcuItems);               //初始化选定窗口
            
        }
        //起始时间改变
        private void dtStartDate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} 0:00:00", this.dtStartDate.Text);
            this.StartDateForHistoryCalcu = DateTime.Parse(datetimeStr);          
        }
        //结束时间改变
        private void dtEndDate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} 0:00:00", this.dtEndDate.Text);
            this.EndDateForHistoryCalcu = DateTime.Parse(datetimeStr);      
            
        }
        //检查实时标签按钮
        private void cb_CheckTags_CheckedChanged(object sender, EventArgs e)
        {
            
        }
        //帮助说明
        private void button1_Click(object sender, EventArgs e)
        {
            HistoryHelp help = new HistoryHelp();
            help.ShowDialog();

        }
        //历史计算按钮   
        private void bt_historyCalcu_MouseHover(object sender, EventArgs e)
        {
            string MessageStr = "历史数据计算注意事项：" + Environment.NewLine +
                                "——只有计算周期为秒、分、时、日的计算项才能使用历史计算功能。" + Environment.NewLine +
                                "——历史计算引擎会根据计算项的计算周期类型，自动的设置准确的起止时刻。" + Environment.NewLine +
                                "——历史计算引擎计算前，无须再对计算项当前的计算起始时刻进行手动调整。" + Environment.NewLine +
                                "——计算引擎设置历史计算起止时刻的规则，请点击帮助按钮查看。" 
                                ;
            tooltip.Show(MessageStr, bt_historyCalcu, bt_historyCalcu.Width/2, -bt_historyCalcu.Height *5);
        }
        private void bt_historyCalcu_MouseLeave(object sender, EventArgs e)
        {
            tooltip.Hide(bt_historyCalcu);
        }
        private void bt_historyCalcu_Click(object sender, EventArgs e)
        {
            //防止重复点击“开始历史数据计算”按钮
            //if (FlagHistoryCalcuRunning == false)
            if(true)
            {
                FlagHistoryCalcuRunning = true;
                //计算前检查点
                //——这里检查的是实时点，但是实际上要进行历史数据计算，不一定都从实时点开始，可能从OPC点开始计算
                //——暂时不检查
                if (cb_CheckTags.Checked)
                {
                    RTDbHelper rtdbhelper = new RTDbHelper();
                    string[] tagnames = { "", "", "" };
                    List<string> results = rtdbhelper.CheckTags(tagnames);
                }

                //计算前检查并发计算项是否符合要求
                //——计算周期大于等于一个月的，不能进行并发计算
                //——计算项

                if (!checkCalcuPeriod(this.SelectedCalcuItems)) return;  //——计算周期大于等于一个月的，不能进行并发计算
                if (!checkCalcuModule(this.SelectedCalcuItems)) return;  //——读取值此的算法不能用并行计算引擎。当重算时间段与值此表周期范围不一致时，会造成重复记录。
              

                //如果选中的计算不为0，则开始计算
                if (this.SelectedCalcuItems.Count != 0)
                {
                    //对选定窗口lv_SelectedCalcuList的样式进行设定，以便UpdateListView可以更改单元格样式
                    //后经测试，无须这两行，也可以改变行的颜色
                    //ListViewItem entryListItem = this.lv_SelectedCalcuList.Items.Add("Items");
                    //entryListItem.UseItemStyleForSubItems = false;                              
                    //设定lv_SelectedCalcuList单元格原样式无效，以便改变颜色

                    //界面信息初始化
                    this.lb_CalcuWarnings.Text = "计算过程中发生0次报警!";
                    this.lb_CalcuErrors.Text = "计算过程中发生0次错误!";

                    //UI线程运行历史计算主程序
                    //historyCalcu();            

                    //独立与UI线程之外运行历史计算主程序
                    Thread thread = new Thread(new ThreadStart(this.historyCalcu));
                    thread.Priority = ThreadPriority.AboveNormal;
                    thread.Start();
                }
                else
                {
                    MessageBox.Show("还未选定要进行历史计算的计算项！");
                }

                FlagHistoryCalcuRunning = false;
            }
        }
        #endregion      

        #region 历史数据计算主程序        
        //选定历史计算对象的排序：先根据源数据来源的数据库类型排，仅对rtdb类型的计算进行并发；再对计算条件排，没有计算条件的和有计算条件的要分开；再对计算间隔排序，相同计算间隔的一起并发计算。
        private void SortSelectedItems()
        {
            //对于SelectedItems的排列要求
            //——首先，按数据源属于实时库还是关系库排序。源数据属于实时库的计算一定排在源数据属于关系的计算之前：源数据是关系库数据的，要使用概化完的数据，这些数据一定来源于实时数据的计算结果。
            //——对于数据源均属于实时库的情况下，计算条件为空的计算一定排在计算条件不为空的计算之前：需要计算条件的，就是要时间筛选算法CondSpan2的时间序列结果。Condspan2不带计算条件（但是有计算参数），一定会先算。
            //——对于源数据属于实时库，按计算条件为空或不为空排序后，再按数据源名称和计算间隔排序。数据源名称相同的拍在一起，计算间隔相同的拍在一起。
            //——对于数据源均属于关系库的情况下，不再排序，也不适用优化算法，计算引擎对计算项一条一条的执行：因为对于概化数据进行在概化，计算量非常的小。一年365天，每天算一次，仅365次。
            
            
            //lambda表达式法 进行排序
            //spanconcat = spanconcat.OrderBy(m => m.Datetime).ThenBy(m => m.Flag).ToList(); 
            //使用专门的计算组态测试配置文件进行测试：组态模板_服务器测试用计算配置文件_20170721_测试selecteditems排序.xlsm
            //——旧版本，先按数据库类型排序，再按有没有条件排序
            //this.SelectedCalcuItems = this.SelectedCalcuItems.OrderBy(m => m.sourcetagdb).ThenBy(m => m.fcondpslnames).ThenBy(m => m.intervalseconds).ToList();  

            //——新版本的并发计算引擎，由于不对多项计算项同时进行并发，仅对单一计算项进行并发。
            //——因此，无需再对计算项对象进行排序。计算项对象的顺序，由csv组态文件来确定。
            //——计算引擎只是从前到后，对每一个计算项顺序的并发计算。
        }
        
        //历史数据计算并发处理       
        //外部循环，
        //——对每一个计算项依次进行并发计算。
        //异常处理：分三层
        //——第一层，for循环
        //——第二层，计算引擎内部，并在调用的外侧调用全局计数器
        //——第三层，DAO
        private void historyCalcu()
        {
            //先关闭所有表的索引:在2018年10月21日测试中。关闭索引后重启，会导致数据表崩溃
            //PSLDataDAO.CloseIndex(this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);

            //并发计算周期
            //——实时数据（包括实时数据库和OPC数据表）计算周期以天为单位，一般取1天。配置文件中以天为单位。程序中以秒为单位
            //——概化数据计算周期以天为单位。一般取30天。配置文件中以天为单位。程序中以秒为单位。
            this.RTDB_PERIOD = APPConfig.historycalcu_period4RTD*60*60*24;           //初始化  实时数据并发计算的并发周期（实时数据计算一般都是以每x分钟，每x小时计算的频率计算，并发周期一般是1d。大于等于1d的实时数据不并发）
            this.PSLDB_PEROID = APPConfig.historycalcu_period4PSL*60*60*24;          //初始化  概化数据并发计算的并发周期（概化数据计算一般都是以每x小时，每x天计算的频率计算，并发周期一般是1m。大于等于1m的概化计算不并发）

            //当前计算对象和指针
            this.CurrentCalcuItems = new List<PSLCalcuItem>();              //在选定的参与历史计算的配置对象集SelectedCalcuItems中，可以在组织在一起进行一次并发计算的并发计算对象集currentCalcuItems            
            this.startIndex = 0;                                            //当前并发计算在SelectedCalcuItems中的起始位置，用于listview更新
            this.endIndex = 0;                                              //当前并发计算在SelectedCalcuItems中的结束位置，用于listview更新

            //上次计算对象集、计算间隔初始化
            string previousSourceTagname = this.SelectedCalcuItems[0].sourcetagname;         //SelectedCalcuItems中上一个计算对象的原标签名称
            int previousIntervalSeconds = this.SelectedCalcuItems[0].intervalseconds;        //SelectedCalcuItems中上一个计算对象的计算间隔           
            
            //历史计算引擎
            HistoryParallelCalcuEngine historycalcu = new HistoryParallelCalcuEngine();     //初始化历史并发计算引擎
            historycalcu.ProcessUpdateAction += Update;                                     //历史并发计算引擎对象实例，注册观察者。使得历史并发计算的进度可能被观察者感知。这里观察者为listview
            historycalcu.WarningUpdateAction += UpdatelbWarnCount;
            historycalcu.ErrorUpdateAction += UpdatelbErrorCount;
            //对选定参与历史计算的配置项进行遍历
            //遍历分类方式，在上面SortSelectedItems()确定的排序方式下，总体按照rtdb和rdb来分类。
            //——对于rtdb：有可能的计算时间周期是1m、15m、30m、1h、2h、3h、4h、6h、8h、12h、24h、48h。
            //            如果原始数据是1s一个数据，一次取1天的数据是3600*24=86400项数据，占用15m内存。
            //            对于实时数据计算，原则上仅对计算周期不超过1天的计算进行多线程加速。并发计算的取数据周期RTDB_PERIOD原则是1天，或者2天。根据实际情况设定。当计算项计算周期大于这个并发计算周期的就不加速。
            //——对于opc:有可能的时间间隔是是1m、15m、30m、1h、2h、3h、4h、6h、8h、12h、24h、48h。同上
            //
            //——对于rdb：有可能的时间间隔是1m、15m、30m、1h、6h、8h、24h、36h、7d、10d、1m、2m。
            //           如果概化数据原始数据是1min一个数据，一次取1m月数据是是60*24*30=43200项，占用8m内存。
            //           比如对OPC分钟过滤值进行概化计算，间隔可能就是1min。 加速计算的数据周期是PSLDB_PEROID原则取1月。大于1个月的计算，由于计算数量急剧下降，所以不进行加速。
            //           比如对常见的概化结果再概化，原数据可能间隔是1h。 加速计算的数据周期是PSLDB_PEROID原则取1月。大于1个月的计算，由于计算数量急剧下降，所以不进行加速。            

            this.errorCount = 0;
            this.warningCount = 0;

            logHelper.Info("历史数据并发计算开始...");

            for (int i = 0; i < this.SelectedCalcuItems.Count; i++)
            {
                try
                {
                    #region 考虑多条源数据相同计算间隔相同的计算项，一起并发计算
                    /*
                    switch (this.SelectedCalcuItems[i].sourcetagdb)
                    {
                        
                        case "rtdb":                        
                            //对于实时数据的概化计算：
                            //——计算前提：1、要求intervalseconds必须小于等于PERIOD，并且intervalseconds可以被PERIOD整除。这个需要在检测程序中添加设定的检测。
                            //——计算：在满足计算前提的情况下，如果计算条件为空，则统计一次并发的计算；如果计算添加为空，则一次计算一条
                            if (this.SelectedCalcuItems[i].intervalseconds > this.RTDB_PERIOD)
                            {//如果出现计算间隔大于并发周期的情况，直接弹出窗口报错，然后退出程序，让使用者检查。
                                string MsgStr = String.Format("计算项{0}的计算周期interval设定为{1}，其值大于并发计算周期设置值{2}。计算引擎停止，请重新进行配置！！", i, this.SelectedCalcuItems[i].intervalseconds, this.RTDB_PERIOD);
                                MessageBox.Show(MsgStr);
                                return;
                            }
                            else if (this.RTDB_PERIOD % this.SelectedCalcuItems[i].intervalseconds != 0)
                            {//如果出现计算间隔不能被并发周期整除的情况，直接弹出窗口报错，然后退出程序，让使用者检查。
                                string MsgStr = String.Format("计算项{0}的计算周期interval设定为{1}，不能被计算并发周期设置值{2}整除。计算引擎停止，请重新进行配置！！", i, this.SelectedCalcuItems[i].intervalseconds, this.RTDB_PERIOD);
                                MessageBox.Show(MsgStr);
                                return;
                            }
                            if (this.SelectedCalcuItems[i].fcondpsltagids == null)
                            {
                                //下面这段代码用于多条源数据相同的计算配置，一起并发。但是由于每一个计算配置改为了多输入，因此下面的程序需要改动
                                //每一次并发计算，源数据相同、计算间隔相同的计算配置项，一起并发
                               
                                if (this.SelectedCalcuItems[i].sourcetagname == previousSourceTagname && this.SelectedCalcuItems[i].intervalseconds == previousIntervalSeconds)
                                {
                                    this.endIndex = i;
                                    this.CurrentCalcuItems.Add(this.SelectedCalcuItems[i]);
                                    continue;
                                }
                                else//在SelectedCalcuItems[i].sourcetagdb=="rtdb"且fcondpsltagids为空的情况下，当interval和上一次不同的时候进行计算
                                {
                                    UpdateListViewColor(this.startIndex, this.endIndex, "start");           //更新UI的listview，把要计算的行染色
                                    historycalcu.ParallelPeriod4RTD = this.RTDB_PERIOD;                     //历史并发计算引擎的实时数据并发周期
                                    historycalcu.MainHistoryParallelCalcu(this.CurrentCalcuItems, this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);
                                    UpdateListViewColor(this.startIndex, this.endIndex, "complete");      //更新UI的listview，把要计算的行染色
                                    this.CurrentCalcuItems.Clear();                                  //计算完成情况CurrentCalcuItems

                                    this.startIndex = i;
                                    this.endIndex = i;
                                    this.CurrentCalcuItems.Add(this.SelectedCalcuItems[i]);
                                }                                                                               

                            }
                            else//如果fcondpsltagids不为空，则每一个带condpsltagid的计算项，当前项单独走并发计算
                            {
                                if (this.CurrentCalcuItems.Count() != 0)
                                {
                                    UpdateListViewColor(this.startIndex, this.endIndex, "start");         //更新UI的listview，把要计算的行染色
                                    historycalcu.ParallelPeriod4RTD = this.RTDB_PERIOD;              //历史并发计算引擎的实时数据并发周期
                                    historycalcu.MainHistoryParallelCalcu(this.CurrentCalcuItems, this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);
                                    UpdateListViewColor(this.startIndex, this.endIndex, "complete");      //更新UI的listview，把要计算的行染色
                                    this.CurrentCalcuItems.Clear();                                  //计算完成情况CurrentCalcuItems
                                }
                                this.startIndex = i;
                                this.endIndex = i;
                                this.CurrentCalcuItems.Add(this.SelectedCalcuItems[i]);               //添加当前计算项到CurrentCalcuItems
                                UpdateListViewColor(this.startIndex, this.endIndex, "start");         //更新UI的listview，把要计算的行染色
                                historycalcu.ParallelPeriod4RTD = this.RTDB_PERIOD;              //历史并发计算引擎的实时数据并发周期
                                historycalcu.MainHistoryParallelCalcu(this.CurrentCalcuItems, this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);
                                UpdateListViewColor(this.startIndex, this.endIndex, "complete");      //更新UI的listview，把要计算的行染色
                                this.CurrentCalcuItems.Clear();                                  //计算完成情况CurrentCalcuItems
                                this.startIndex = i + 1;
                                this.endIndex = i + 1;
                            }
                            break;
                        case "rdb":
                            //对于概化数据的再概化计算：
                            //——要求1、要求intervalseconds必须小于等于PERIOD，但不要求整除。将结束时间调整到结束时间之前最近的可以整除的位置。
                            //——计算：每一条计算一次
                            int RDB_PERIOD = (this.EndDateForHistoryCalcu - this.StartDateForHistoryCalcu).Seconds;
                            if (RDB_PERIOD < this.SelectedCalcuItems[i].intervalseconds)
                            {
                                MessageBox.Show("计算项{0}的计算周期interval设定为{1}，其值大于并发计算周期设置值{2}。计算引擎停止，请重新进行配置！！");
                                return;
                            }
                            else
                            {
                                //将结束时间调整到结束时间之前最近的可以整除的位置。
                                DateTime EndDateForHistoryCalcuAdjust = this.StartDateForHistoryCalcu.AddSeconds((RDB_PERIOD / this.SelectedCalcuItems[i].intervalseconds) * this.SelectedCalcuItems[i].intervalseconds);
                                this.endIndex = i;
                                this.CurrentCalcuItems.Add(this.SelectedCalcuItems[i]);               //添加当前计算项到CurrentCalcuItems
                                UpdateListViewColor(this.startIndex, this.endIndex, "start");         //更新UI的listview，把要计算的行染色
                                historycalcu.ParallelPeriod4RTD = RDB_PERIOD;               //历史并发计算引擎的并发周期
                                historycalcu.MainHistoryParallelCalcu(this.CurrentCalcuItems, this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);
                                UpdateListViewColor(this.startIndex, this.endIndex, "complete");      //更新UI的listview，把要计算的行染色
                                this.CurrentCalcuItems.Clear();                                  //计算完成情况CurrentCalcuItems
                                this.startIndex = i + 1;
                                this.endIndex = i + 1;
                            }
                            break;
                        default:
                            break;
                    }//end switch   
                    */
                    #endregion 考虑多条源数据相同计算间隔相同的计算项，一起并发计算
                    
                    #region 每次仅对一条计算项进行并发计算。不考虑多条计算项一起并发情况。
                    this.startIndex = i;
                    this.endIndex = i;
                    this.CurrentCalcuItems.Add(this.SelectedCalcuItems[i]);
                    UpdateListViewColor(this.startIndex, this.endIndex, "start");           //更新UI的listview，把要计算的行染色
                    
                    #region 按照数据类型确定并发周期
                    /*
                    switch (this.SelectedCalcuItems[i].sourcetagdb)                         //对于不同的计算类型，并发周期的取值和判断方法不一样
                    {  
                        case "rtdb":
                        case "opc":
                            if (this.SelectedCalcuItems[i].intervalseconds > this.RTDB_PERIOD || this.RTDB_PERIOD % this.SelectedCalcuItems[i].intervalseconds != 0)
                            {   
                                //对于OPC和rtdb格式，如果计算间隔大于并发周期，或计算间隔不能被并发周期整除的情况，不进行并发计算   
                                //historycalcu.IsParallelCalcu = false;
                                //historycalcu.ParallelPeriod = this.SelectedCalcuItems[i].intervalseconds;       //历史计算引擎的取数周期和实际的计算周期相同
                                //调用实时计算引擎
                            }
                            else
                            {
                                //否则，进行并发计算   
                                historycalcu.IsParallelCalcu = true;
                                historycalcu.ParallelPeriod = this.RTDB_PERIOD;                                 //历史计算引擎的取数舟曲为参数“时数据并发周期”                                 
                            }
                            break;
                        
                        case "rdb":
                        case "rdbset":     
                            if (this.SelectedCalcuItems[i].intervalseconds > this.PSLDB_PEROID || this.RTDB_PERIOD % this.SelectedCalcuItems[i].intervalseconds != 0)
                            {
                                //对于rdb和rdbset格式如果计算间隔大于并发周期，或计算间隔不能被并发周期整除的情况，则采用普通计算
                                //historycalcu.IsParallelCalcu = false;
                                //historycalcu.ParallelPeriod = this.SelectedCalcuItems[i].intervalseconds;        //历史计算引擎的取数周期和实际的计算周期相同  
                                //调用实时计算引擎
                            }
                            else                            
                            {
                                //否则，进行并发计算     
                                historycalcu.IsParallelCalcu = true;
                                historycalcu.ParallelPeriod = this.PSLDB_PEROID;                                 //历史计算引擎的取数周期和为参数“概化数据并发周期”                                  
                            }
                            break;                            
                    }
                    */
                    #endregion
                    
                    #region 按照数据类型和计算间隔相结合确定并发周期
                    //——如果数据类型为rtdb，则由于实时数据量比较大（OPC等同于实时数据），同时受限于实时数据接口读取能力限制，一般一次并发读取的数据周期为1天。
                    //——否则，概化数据统一并发读取周期为30天。

                    //如果数据类型为rtdb或opc，或者计算周期小于等于1分钟（60秒），则按RTDB_PERIOD（一般为1天）并发
                    if (this.SelectedCalcuItems[i].sourcetagdb.Trim().ToUpper()=="RTDB" || this.SelectedCalcuItems[i].sourcetagdb.Trim().ToUpper()=="OPC" || this.SelectedCalcuItems[i].intervalseconds <= 60)      
                    {
                        historycalcu.IsParallelCalcu = true;
                        historycalcu.ParallelPeriod = this.RTDB_PERIOD;                                 //历史计算引擎的取数周期为参数“实时数据并发周期” ，通常为1天   
                    }
                    //20181111、添加。设备级别的rdbset类型，一个设备30个点，一个算法70个结果，rdbset可能一次取2000个标签。如果按月取数据太慢，造成连接中断。此时并行计算周期为1天
                    else if (this.SelectedCalcuItems[i].sourcetagdb.Trim().ToUpper() == "RDBSET")
                    {
                        if (this.SelectedCalcuItems[i].intervalseconds / (60 * 60 * 24) < this.RTDB_PERIOD)
                        {
                            historycalcu.IsParallelCalcu = true;
                            historycalcu.ParallelPeriod = this.RTDB_PERIOD;                                 //历史计算引擎的取数周期为参数“实时数据并发周期” ，通常为1天   
                        }
                        else
                        {
                            historycalcu.IsParallelCalcu = true;
                            historycalcu.ParallelPeriod = this.PSLDB_PEROID;                                 //历史计算引擎的取数周期为参数“实时数据并发周期” ，通常为1天
                        }
                    }
                    //如果数据类型不为rtdb，且计算周期大于1分钟（60秒），则按RTDB_PERIOD（一般为1天）并发
                    else
                    {
                        historycalcu.IsParallelCalcu = true;
                        historycalcu.ParallelPeriod = this.PSLDB_PEROID;                                 //历史计算引擎的取数周期和为参数“概化数据并发周期”  ，通常为30天
                    }
                    #endregion

                    historycalcu.errorCount = this.errorCount;                                           //获得上一次并发计算的总错误数量
                    historycalcu.warningCount = this.warningCount;                                       //获得上一次并发计算的总报警数量
                    historycalcu.MainHistoryParallelCalcu(this.CurrentCalcuItems, this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);  //开始并发计算
                    
                    //计算正常结束
                    logHelper.Info("第" + i.ToString() + "项历史计算项已经计算完毕。计算项序号:" + this.CurrentCalcuItems[0].fid);         //记录log
                    this.errorCount = historycalcu.errorCount;                                                                          //统计当项计算错误
                    this.warningCount = historycalcu.warningCount;                                                                      //统计当项计算报警
                    UpdateListViewColor(this.startIndex, this.endIndex, "complete");                                                    //更新UI的listview，把要计算的行染色
                    this.CurrentCalcuItems.Clear();                                                                                     //清空当项计算

                    #endregion

                    
                }
                catch (Exception ex)
                {
                    //如果计算中有未预知错误，先获得并发计算当前错误和警告次数
                    if (historycalcu.errorCount!=null) this.errorCount = historycalcu.errorCount;
                    if (historycalcu.warningCount != null) this.warningCount = historycalcu.warningCount;
                    //再累加
                    this.errorCount = this.errorCount + 1;
                    this.errorFlag = true;
                    //更新UI
                    this.UpdatelbErrorCount(this.errorCount, "");
                    //更新log
                    string errInfo;
                    errInfo = string.Format("历史数据计算引擎错误{0}：计算引擎主循环错误！", this.errorCount.ToString()) + Environment.NewLine;
                    //logHelper.Error(errInfo);
                    errInfo +=  string.Format("——计算模块的名称是：{0}-{1}，当前计算源标签是：{2}，计算起始时间是：{3}，计算结束时间是：{4}。",
                                                        this.CurrentCalcuItems[0].fid,
                                                        this.CurrentCalcuItems[0].fmodulename,
                                                        this.CurrentCalcuItems[0].sourcetagname,
                                                        this.StartDateForHistoryCalcu.ToString(),
                                                        this.EndDateForHistoryCalcu.ToString()
                                                        ) + Environment.NewLine;
                    //logHelper.Error(errInfo);
                    errInfo += string.Format("——详细错误信息：{0}", this.errorCount.ToString(), ex);
                    logHelper.Error(errInfo);
                }//end try
            }//endfor

            logHelper.Info("历史数据并发计算项已经计算完毕...");  
            //打凯所有psldata表的索引
            //PSLDataDAO.OpenIndex(this.StartDateForHistoryCalcu, this.EndDateForHistoryCalcu);

        }  
        #endregion

        #region 从线程外处理界面信息
        private void Update(int percent,string str)
        {
            string warningcount = str.Split(';')[0];
            string errorscount = str.Split(';')[1];
            UpdateListView(this.startIndex, endIndex, percent.ToString(), warningcount, errorscount);  //更新主界面listview
        }
        //历史计算界面listview窗口刷新：更新进度
        delegate void ListUpdate(int startindex, int endindex, string percent,string warning,string error);         //线程刷新UI代理：更新进度  
        private void UpdateListView(int startindex, int endindex, string percent, string warning, string error)
        {
            if (this.lv_SelectedCalcuList.InvokeRequired)
            {
                ListUpdate d = new ListUpdate(UpdateListView);
                this.lv_SelectedCalcuList.Invoke(d, startindex, endindex, percent,warning,error);
            }
            else
            {
                //特别注意，这里用currentItemd的fid，来找lV_DataList.Items，
                //这里以假定fid是从0开始的连续整数，因此fid与listview的item序号有对应关系。
                //fid和listview.item的关系是fid对应listview.item[fid-1]
                //这里的对应关系，极大的提高了计算主引擎刷新界面的效率。避免了在listview中的搜索过程。                
                //lV_DataList.Items[currentItem.fid - 1].BackColor = Color.LightSkyBlue;
                for (int i = startindex; i <= endindex; i++)
                {
                    lv_SelectedCalcuList.Items[i].SubItems[19].Text = percent+" %";         //更新进度信息
                    lv_SelectedCalcuList.Items[i].SubItems[20].Text = warning;              //更新报警总数信息
                    lv_SelectedCalcuList.Items[i].SubItems[21].Text = error;                //更新错误总数信息
                }
            }
        }
        //历史计算界面listview窗口刷新：更新颜色
        delegate void ListUpdateColor(int startindex, int endindex, string status);     //线程刷新UI代理：更新颜色
        private void UpdateListViewColor(int startindex, int endindex, string status)
        {
            if (this.lv_SelectedCalcuList.InvokeRequired)
            {
                ListUpdateColor d = new ListUpdateColor(UpdateListViewColor);
                this.lv_SelectedCalcuList.Invoke(d, startindex, endindex, status);
            }
            else
            {
                //特别注意，这里用currentItemd的fid，来找lV_DataList.Items，
                //这里以假定fid是从0开始的连续整数，因此fid与listview的item序号有对应关系。
                //fid和listview.item的关系是fid对应listview.item[fid-1]
                //这里的对应关系，极大的提高了计算主引擎刷新界面的效率。避免了在listview中的搜索过程。                
                //lV_DataList.Items[currentItem.fid - 1].BackColor = Color.LightSkyBlue;

                for (int i = startindex; i <= endindex; i++)
                {
                    switch (status)
                    {
                        case "start":
                            lv_SelectedCalcuList.Items[i].EnsureVisible();                  //显示区重新定位，这里相当于动态效果，一条条往上滚。直到最后一个显示出来为止。
                            lv_SelectedCalcuList.Items[i].BackColor = Color.MistyRose;      //要计算的染红色
                            Font font = new Font(Control.DefaultFont, FontStyle.Bold);
                            lv_SelectedCalcuList.Items[i].SubItems[19].Font = font;          //加粗百分比字体
                            break;
                        case "complete":
                            lv_SelectedCalcuList.Items[i].BackColor = Color.PaleGreen;       //计算完成的染绿色
                            lv_SelectedCalcuList.Items[i].SubItems[19].Text = "Complete";   //修改进度单元格内容
                            break;
                        default:
                            break;
                    }

                }
            }
        }
        
        //更新错误信息
        delegate void dg_lbErrorCount(long Count,string text);
        private void UpdatelbErrorCount(long Count, string text)
        {
            if (this.lb_CalcuErrors.InvokeRequired)
            {
                dg_lbErrorCount d = new dg_lbErrorCount(UpdatelbErrorCount);
                this.lb_CalcuErrors.Invoke(d, Count,text);
            }
            else
            {
                this.lb_CalcuErrors.Text = string.Format("计算过程中发生{0}次错误!", Count.ToString());
            }
        }
        //更新报警信息
        delegate void dg_lbWarnCount(long Count, string text);
        private void UpdatelbWarnCount(long Count, string text)
        {
            if (this.lb_CalcuErrors.InvokeRequired)
            {
                dg_lbWarnCount d = new dg_lbWarnCount(UpdatelbWarnCount);
                this.lb_CalcuWarnings.Invoke(d, Count,text);
            }
            else
            {
                this.lb_CalcuWarnings.Text = string.Format("计算过程中发生{0}次报警!", Count.ToString());
            }
        }
        #endregion
        
        #region 辅助函数
        //双击将单元格内容复制到剪切板
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            System.Windows.Forms.ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);
            string strText = lstcol.Text;
            try
            {
                Clipboard.SetDataObject(strText);
                notifyIcon1.Visible = true;
                string info = string.Format("内容【{0}】已经复制到剪贴板", strText);
                notifyIcon1.ShowBalloonTip(1500, "提示", info, ToolTipIcon.Info);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //筛选计算项list初始化
        private void Initial_lv_filtercalcu(List<PSLCalcuItem> calcuitems)
        {
            if (calcuitems.Count == 0) return;
            //清空listview的已有数据项
            lv_FilterCalcuList.Items.Clear();
            //根据pslconfigitems添加
            for (int i = 0; i < calcuitems.Count; i++)              //为listview添加 items.Count行数据
            {
                PSLCalcuItem pslcalcuitem = calcuitems[i];
                ListViewItem lvitem = lv_FilterCalcuList.Items.Add((pslcalcuitem.fid).ToString(), (pslcalcuitem.fid).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.Checked = cb_SelectAllForFilter.Checked;
                lvitem.SubItems.Add(pslcalcuitem.sourcetagname.Replace("^", @"\"));     //添加listview的第1个字段tagname。 Replace("^",@"\")是对PGIM的特殊处理               
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdb);                          //添加listview的第2个字段tagdb
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdesc);                        //添加listview的第3个字段tagdesc
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdim);                         //
                lvitem.SubItems.Add(pslcalcuitem.sourcetagmrb.ToString());              //
                lvitem.SubItems.Add(pslcalcuitem.sourcetagmre.ToString());              //
                lvitem.SubItems.Add(pslcalcuitem.fmodulename);
                lvitem.SubItems.Add(pslcalcuitem.fgroup);
                lvitem.SubItems.Add(pslcalcuitem.forder.ToString());
                lvitem.SubItems.Add(pslcalcuitem.fclass);
                lvitem.SubItems.Add(pslcalcuitem.falgorithmsflag);
                lvitem.SubItems.Add(pslcalcuitem.fparas);
                lvitem.SubItems.Add(pslcalcuitem.fcondpslnames);
                lvitem.SubItems.Add(pslcalcuitem.foutputtable);
                lvitem.SubItems.Add(pslcalcuitem.foutputnumber.ToString());
                lvitem.SubItems.Add(pslcalcuitem.foutputpsltagnames.Replace("^", @"\"));
                lvitem.SubItems.Add(pslcalcuitem.resolution);
                lvitem.SubItems.Add(pslcalcuitem.fdelay.ToString());
                lvitem.SubItems.Add(pslcalcuitem.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                lvitem.SubItems.Add(pslcalcuitem.fendtime.ToString("yyyy-MM-dd HH:mm:ss"));
                lvitem.SubItems.Add(pslcalcuitem.fnextstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                lvitem.SubItems.Add("0");       //报警数量
                lvitem.SubItems.Add("0");       //错误数量
            }
        }
        //选中计算项list初始化
        private void Initial_lv_selectedcalcu(List<PSLCalcuItem> calcuitems)
        {            
            //清空listview的已有数据项
            lv_SelectedCalcuList.Items.Clear();
            //根据pslconfigitems添加
            for (int i = 0; i < calcuitems.Count; i++)              //为listview添加 items.Count行数据
            {
                PSLCalcuItem pslcalcuitem = calcuitems[i];
                ListViewItem lvitem = lv_SelectedCalcuList.Items.Add((pslcalcuitem.fid).ToString(), (pslcalcuitem.fid).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.Checked = cb_SelectAllForFilter.Checked;
                lvitem.SubItems.Add(pslcalcuitem.sourcetagname.Replace("^", @"\")); //添加listview的第1个字段tagname         Replace("^",@"\")是对PGIM的特殊处理        
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdb);                      //添加listview的第2个字段tagdbtype
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdesc);                    //添加listview的第3个字段tagdesc
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdim);                     //
                lvitem.SubItems.Add(pslcalcuitem.sourcetagmrb.ToString());          //
                lvitem.SubItems.Add(pslcalcuitem.sourcetagmre.ToString());          //
                lvitem.SubItems.Add(pslcalcuitem.fmodulename);
                lvitem.SubItems.Add(pslcalcuitem.fgroup);
                lvitem.SubItems.Add(pslcalcuitem.forder.ToString());
                lvitem.SubItems.Add(pslcalcuitem.fclass);
                lvitem.SubItems.Add(pslcalcuitem.falgorithmsflag);
                lvitem.SubItems.Add(pslcalcuitem.fparas);
                lvitem.SubItems.Add(pslcalcuitem.fcondpslnames);
                lvitem.SubItems.Add(pslcalcuitem.foutputtable);
                lvitem.SubItems.Add(pslcalcuitem.foutputnumber.ToString());
                lvitem.SubItems.Add(pslcalcuitem.foutputpsltagnames.Replace("^", @"\"));
                lvitem.SubItems.Add(pslcalcuitem.resolution);
                lvitem.SubItems.Add(pslcalcuitem.fdelay.ToString());
                lvitem.SubItems.Add("0");                                       //计算进度
                lvitem.SubItems.Add("0");                                       //报警数量
                lvitem.SubItems.Add("0");                                       //错误数量
                //lvitem.SubItems.Add(pslcalcuitem.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                //lvitem.SubItems.Add(pslcalcuitem.fendtime.ToString("yyyy-MM-dd HH:mm:ss"));
                //lvitem.SubItems.Add(pslcalcuitem.fnextstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            label_selectedCount.Text = calcuitems.Count().ToString()+"条";       //更新数量
        }
        //检查计算周期是否符合要求
        private bool checkCalcuPeriod(List<PSLCalcuItem> calcuitems)
        {
            List<string> errorlines = new List<string>();
            int sum = 0;

            for (int i = 0; i < calcuitems.Count;i++ )
            {
                
                if (calcuitems[i].intervalseconds >= 60 * 60 * 24 * 28)     //如果计算项周期是以月、季、年来季，则不能进行并发计算。最小的月是28天。intervalseconds单位是秒
                {
                    errorlines.Add(calcuitems[i].fid.ToString());
                    sum = sum + 1;
                    continue;
                }
            }

            if (sum != 0)
            {
                string messageStr = String.Format("历史数据计算不支持计算周期大于等于1个月的计算项！" + Environment.NewLine +
                                                  "不符合条件的计算项为：" + String.Join(",", errorlines)
                                                    );
                MessageBox.Show(messageStr);
                return false;
            }
            else
                
            return true;
        }
        //检查计算计算项是否为MReadConst或MReadShift
        private bool checkCalcuModule(List<PSLCalcuItem> calcuitems)
        {
            List<string> errorlines = new List<string>();
            int sum = 0;
            for (int i = 0; i < calcuitems.Count; i++)
            {
                //读取值次的算法不能用并行计算引擎。
                //——比如，并发计算时间是2016.1.1---2016.3.1。
                //——计算引擎会在MReadShift计算开始时，一次性删除2016.1.1---2016.3.1的值此信息。
                //——而MReadShift，则是一次性读取值次表内所有信息。可能是2016.1.1---2017.1.1。这样，2016.3.1--2017.1.1的值次信息就是重复的。
                if (calcuitems[i].fmodulename == "MReadConst" ||                //
                    calcuitems[i].fmodulename == "MReadShift"                   //读取值次的算法不能用并行计算引擎。当重算时间段与值此表周期范围不一致时，会造成重复记录。
                    //calcuitems[i].fmodulename == "MCondSpanLong"||            //MCondspanLong算法不能进行并行计算。因为该算法的输入数据时间往往不连续。目前的并发计算不能正确分割。
                    //calcuitems[i].fmodulename == "MSpanDelay"                 //MSpanDelay算法不能进行并行计算。因为该算法的输入数据时间往往不连续。目前的并发计算不能正确分割。
                    )     //                
                {
                    errorlines.Add(calcuitems[i].fid.ToString()+":"+calcuitems[i].fmodulename);
                    sum = sum + 1;
                    continue;
                }
            }

            if (sum != 0)
            {
                string messageStr = String.Format("历史数据计算不支持以下计算公式：" + Environment.NewLine +
                                                  "不符合条件的计算项为：" + String.Join(",", errorlines)
                                                    );
                MessageBox.Show(messageStr);
                return false;
            }
            else

                return true;
        }

        #endregion

        private void tb_Fastfixed_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果按下回车键
            if (e.KeyChar == (char)Keys.Enter)
            {
                //
                try
                {
                    int fixedIndx = int.Parse(this.tb_Fastfixed.Text);
                    this.lv_FilterCalcuList.EnsureVisible(fixedIndx);//滚动到指定的行位置
                    this.lv_FilterCalcuList.Items[fixedIndx].Focused = true;
                }
                catch
                {
                    MessageBox.Show("请输入正确的行号！");
                }
            }
        }

        private void bt_Fixed_Click(object sender, EventArgs e)
        {
            try
            {
                int fixedIndx = int.Parse(this.tb_Fastfixed.Text);
                this.lv_FilterCalcuList.EnsureVisible(fixedIndx);//滚动到指定的行位置
                this.lv_FilterCalcuList.Items[fixedIndx].Focused = true;
            }
            catch
            {
                MessageBox.Show("请输入正确的行号！");
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        //按公式名称和计算周期选择计算公式
        private void button2_Click(object sender, EventArgs e)
        {
            if (flag_selectintervaltype == true)
            {

                for (int i = 0; i < lv_FilterCalcuList.Items.Count; i++)
                {
                    string intervalStr = lv_FilterCalcuList.Items[i].SubItems[17].Text;
                    String moduleName = ModuleBox.Text;
                    // && lv_FilterCalcuList.Items[i].SubItems[17].
                    if (intervalStr.Contains(this.cb_intervaltype.Text) && lv_FilterCalcuList.Items[i].SubItems[7].Text == moduleName)
                    {
                        lv_FilterCalcuList.Items[i].Checked = this.cb_SelectIntervalType.Checked; 
                    }
                    else
                    {
                        lv_FilterCalcuList.Items[i].Checked = false;
                    }
                }
            }
        }

        

      

      

      

      

       

       

       

        

       

       

        

        

       


        

        

        

        

       

        

        

        

       
    }
}
