using System;
using System.Collections.Generic;       //使用List<>
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PCCommon;                         //使用PValue
//using DBInterface.RTDBInterface;        //使用实时数据库接口。调用接口全部在DAO层。
//using DBInterface.RDBInterface;         //使用关系数据库接口。调用接口全部在DAO层。
using System.Reflection;                //使用反射
using PSLCalcu.Module;                  //使用计算模块
using Config;                           //使用配置模块
using System.Timers;                    //使用定时器
using System.Text.RegularExpressions;   //使用正则表达式
using System.IO;
using log4net;
using Globalspace;

//运行时，如果使用RTDbHelper或DbHelper，出现初始化方面的错误，则一般是xml配置文件有错。
//因为RTDbHelper和DbHelper的数据库类型，是由字符串转枚举，这个过程有可能失败，但是这里没有办法添加异常处理机制。后续看如何解决这个问题。

namespace PSLCalcu
{
    public partial class AppRunForm : Form
    {
        //app全局
        public string APPName = "PSLCalcu.exe";                                                     //APP名称        
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(AppRunForm));                     //全局log

        //计算引擎全局常量    

        //计算引擎全局变量
        private String CSVFileName = "";                                                            //概化计算配置表名称
        private bool IMPORTOK;                                                                      //概化计算配置表导入标志       
        private static List<PSLCalcuItem> PslCalcuItems = new List<PSLCalcuItem>();                        //概化计算(公式)配置对象集
        private static CurrentCalcuEngine currentCalcuEngine = new CurrentCalcuEngine();
        private List<string> ModuleNames = new List<string>();                                        //概化计算使用的公式名称集
        //private Dictionary<string, string> ModuleOutputDesc = new Dictionary<string, string>();   //概化计算使用的公式结算结果字典
        private bool resetTagIDMap;
        //标签id映射是否重置

        private static List<PSLCalcuItem> secondList;
        private static List<PSLCalcuItem> minList;
        private static List<PSLCalcuItem> hourList;
        private static List<PSLCalcuItem> dayList;
        private static List<PSLCalcuItem> monthList;
        private static List<PSLCalcuItem> yearList;


        private static Dictionary<int, List<PSLCalcuItem>> secondMap;
        private static Dictionary<int, List<PSLCalcuItem>> minMap;
        private static Dictionary<int, List<PSLCalcuItem>> hourMap;
        private static Dictionary<int, List<PSLCalcuItem>> dayMap;
        private static Dictionary<int, List<PSLCalcuItem>> monthMap;
        private static Dictionary<int, List<PSLCalcuItem>> yearMap;

        //计算引擎主线程对象
        private System.Timers.Timer _timer;                                         //概化计算主线程
        private static long errorCount;                                                    //概化计算主线程错误统计
        private static long warningCount;                                                  //概化计算主线程警告统计
        private static long beforeCalcount = 0;
        private static long calcount;                                                      //计算次数
        private List<TimeRecord> TimeRecords = new List<TimeRecord>();              //概化计算时间记录对象   
        private PSLCalcuItem previousItem = new PSLCalcuItem();                     //上一个计算对象

        //主界面               
        private static bool calcurunningflag = false;
        private static long calstatus = 0;
        private string[] IntervalType = { "second", "minute", "hour", "day", "week", "month", "year" };




        #region 主界面初始化与载入

        public AppRunForm()
        {


            InitializeComponent();
            //初始化窗口大小
            Rectangle myScreen = Screen.GetWorkingArea(this);           //获得屏幕实际分辨率（如1920*1036）
            if (myScreen.Height > this.Height)                          //如果当前屏幕纵向分辨率大于当前界面高度
            {
                this.Top = myScreen.Height / 5;                         //将当前界面顶部起始位设置为距顶部1/4处
                this.Height = myScreen.Height * 4 / 5;                   //将当前界面高度设为屏幕高度2/4
            }
            //根据配置文件决定是否显示debug菜单
            if (APPConfig.app_mode == "debug" && APPConfig.app_debug_password == "wenjiug")
            {
                this.DebugToolStripMenuItem.Visible = true;
            }
            else
            {
                this.DebugToolStripMenuItem.Visible = false;
            }

            //主界面控件初始化
            this.notifyIcon1.Visible = true;                                                //程序图标，右下角。？问题是为什么重复运行，会显示一堆  

            dtStartDate.Value = DateTime.Now;                                               //初始化“计算起始日期”
            dtStartTime.Value = DateTime.Now.Date.AddHours(DateTime.Now.Hour);              //初始化“计算起始时间”，以当前时间之前最近的整点为值
            tslb_CalcuConfig.Text = "";
            tslb_CalcuWarnings.Text = "";
            tslb_CalcuErrors.Text = "";
            this.cb_intervaltype.Items.Clear();
            this.cb_intervaltype.Items.AddRange(this.IntervalType);
            this.cb_intervaltype.SelectedIndex = 2;
            this.cb_moduletype.Items.Clear();
        }

        //主界面载入时
        private void AppRunForm_Load(object sender, EventArgs e)
        {
            //初始化的过程要读取关系型数据库，检查标签名的过程要读取实时数据库。
            //要先检查数据库的连接情况，否则读取过程会非常漫长
            //先尝试连接关系库
            //如报错“初始值设定项引发异常”，请检查DbHelper的数据库类型和连接字符串赋值是否正确。
            //注意，xml中定义的数据库类型必须与枚举型CurrentRTDbType中的描述一致，大小写敏感。
            //修改config项目中的AppConfig.xml，然后必须将该文档拷贝到..\lib\config下。
            //测试实时数据库
            string messageStr = "";
            if (RTDBDAO.connectTest())
            {
                //初始化时，只有不成功才显示信息
                //messageStr = String.Format("数据库:\n--{0}\n连接测试结果:{1}", RTDBDAO.rtdbConnStr, RTDBDAO.testStatus); //非调试状态成功时不显示
                //MessageBox.Show(messageStr, "实时数据库连接信息");
            }
            else
            {
                messageStr = String.Format("实时数据库连接失败：\n\r1、检查实时数据库服务是否正常；\n\r2、检查网络连接是否正常；\n\r3、详细错误信息请查看log文件。");
                MessageBox.Show(messageStr, "实时数据库连接失败");
            }
            //测试关系数据库
            if (IniTable.connectTest())
            {
                //初始化时，只有不成功才显示信息
                //messageStr = String.Format("数据库:\r\n--{0}\n连接测试结果:{1}", IniTable.rdbConnStr, IniTable.testStatus);
                // MessageBox.Show(messageStr, "关系数据库连接信息");
            }
            else
            {
                messageStr = String.Format("关系数据库连接失败，计算引擎启动将终止：\n\r1、检查关系数据库服务是否正常；\n\r2、检查是否开启远程访问权限；\n\r3、检查sys和psldb两个数据库是否存在。\n\r4、详细错误信息请查看log文件。");
                MessageBox.Show(messageStr, "关系数据库连接失败。");
                //启动时检测到关系数据库出错，要跳出，后面要从关系库读取数据。
                Application.ExitThread();
                Application.Exit();
                this.Close();
            }
            //初始化概化计算对象集、概化计算标签字典、listview控件
            if (!InitialCalcu()) return;

            //自动运行
            if (APPConfig.realcalcu_autorun == "1")
                this.tsbt_Start_Click(null, null);
        }
        #endregion

        #region 图标按钮
        //“启动/暂停计算”按钮_
        public static void minCal()
        { //分钟线程执行方法
            if (null != minList && minList.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.Read(ref calstatus) == 1)  //计算状态为true时才执行
                    {
                        currentCalcuEngine.calcu(logHelper, minList, APPConfig.CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, ref beforeCalcount, ref calstatus);
                    }
                }
            }
        }
        public static void hourCal()
        {   //小时线程执行方法
            if (null != hourList && hourList.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.Read(ref calstatus) == 1)  //计算状态为true时才执行
                    {
                        bool flag = currentCalcuEngine.calcu(logHelper, hourList, APPConfig.CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, ref beforeCalcount, ref calstatus);
                        if (flag)
                        {
                            PSLCalcuItem min = hourList.Min();       //时间最小配置
                            if (null != minList && minList.Count > 0)
                            {
                                PSLCalcuItem minuteMin = minList.Min();  //分钟数据时间最小配置
                                //如果小于分钟最小配置  则进行计算  否则不进行计算
                                if (min.fnextstarttime > minuteMin.fnextstarttime)
                                {
                                    Thread.Sleep(60000);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void dayCal()
        {   //天线程执行方法
            if (null != dayList && dayList.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.Read(ref calstatus) == 1)  //计算状态为true时才执行
                    {
                        bool flag = currentCalcuEngine.calcu(logHelper, dayList, APPConfig.CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, ref beforeCalcount, ref calstatus);
                        if (flag)
                        {
                            PSLCalcuItem min = dayList.Min();       //时间最小配置
                            PSLCalcuItem hourMin = hourList.Min();  //小时计算截止时间最小的值
                            if (min.fnextstarttime > hourMin.fnextstarttime)
                            { //如果天的最小时间配置 大于小时的最小时间配置  说明小时计算还没有完成
                                Thread.Sleep(60000 * 60);  //线程休眠一小时
                            }
                        }
                    }
                }
            }
        }
        public static void monthCal()
        {   //月线程执行方法
            if (null != monthList && monthList.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.Read(ref calstatus) == 1)  //计算状态为true时才执行
                    {
                        bool flag = currentCalcuEngine.calcu(logHelper, monthList, APPConfig.CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, ref beforeCalcount, ref calstatus);
                        if (flag)
                        {
                            PSLCalcuItem min = monthList.Min(); //时间最小配置
                            PSLCalcuItem dayMin = dayList.Min(); //天时间最小配置
                            if (min.fnextstarttime > dayMin.fnextstarttime)
                            {
                                Thread.Sleep(60000 * 60);
                            }
                        }
                    }
                }
            }
        }
        public static void yearCal()
        {
            if (null != yearList && yearList.Count > 0)
            {
                while (true)
                {
                    if (Interlocked.Read(ref calstatus) == 1)  //计算状态为true时才执行
                    {
                        bool flag = currentCalcuEngine.calcu(logHelper, yearList, APPConfig.CALCUMODULE_THRESHOLD, ref errorCount, ref warningCount, ref calcount, ref beforeCalcount, ref calstatus);
                        if (flag)
                        {
                            PSLCalcuItem min = yearList.Min();      //时间最小配置
                            PSLCalcuItem monthMin = yearList.Min(); //月时间最小配置
                            if (min.fnextstarttime > monthMin.fnextstarttime)
                            {   //最小时间大于当前时间  线程休眠
                                Thread.Sleep(60000 * 60);
                            }
                        }
                    }
                }
            }
        }

        Thread minThread;   //分钟线程
        Thread hourThread;  //小时线程
        Thread dayThread;   //天线程
        Thread monthThread; //月线程
        Thread yearThread;  //年线程
        private void tsbt_Start_Click(object sender, EventArgs e)
        {
            if (tsbt_Start.Text.ToLower() == "start")
            {
                if (PslCalcuItems.Count > 0)
                {
                    tsbt_Start.Image = Properties.Resources.stop;
                    tsbt_Start.Text = "Stop";
                    tsbt_Quit.Enabled = false;
                    btn_SetStartDate.Enabled = false;
                    errorCount = 0;
                    warningCount = 0;
                    tslb_CalcuErrors.Text = string.Format("计算过程中发生{0}次错误!", errorCount.ToString());
                    tslb_CalcuWarnings.Text = string.Format("计算过程中发生{0}次警告!", warningCount.ToString());
                    calcurunningflag = true; //设置计算状态
                    Interlocked.Increment(ref calstatus);
                    this._timer = new System.Timers.Timer();
                    this._timer.Interval = 1000; //APPConfig.realcalcu_period;           //设置时间间隔
                    this._timer.Elapsed += new ElapsedEventHandler(updateView);     //
                    this._timer.AutoReset = true;                                //设置是执行一次（false）还是一直执行(true)，默认为true
                    this._timer.Start();
                    //此处创建一个线程进行计算
                    initalMap();//初始化计算map
                    minThread = new Thread(new ThreadStart(minCal));    //分钟线程
                    hourThread = new Thread(new ThreadStart(hourCal));  //小时线程
                    dayThread = new Thread(new ThreadStart(dayCal));      //天线程
                    monthThread = new Thread(new ThreadStart(monthCal));  //月线程
                    yearThread = new Thread(new ThreadStart(yearCal)); ;  //年线程
                    minThread.Start();
                    hourThread.Start();
                    dayThread.Start();
                    monthThread.Start();
                    yearThread.Start();
                }
                else
                {
                    MessageBox.Show("请先导入计算配置信息!");
                }
            }
            else
            {
                tsbt_Start.Image = Properties.Resources.start;    //更换按钮为start图片
                tsbt_Start.Text = "Start";
                tsbt_Quit.Enabled = true;
                btn_SetStartDate.Enabled = true;
                calcurunningflag = false;  //设置计算状态
                Interlocked.Decrement(ref calstatus);
                System.Timers.Timer stop = new System.Timers.Timer();        //定义定时器
                stop.Interval = 5000; //APPConfig.realcalcu_period;          //5秒钟以后执行
                stop.Elapsed += new ElapsedEventHandler(stopCalcu);          //
                stop.AutoReset = false;                                      //设置执行一次
                stop.Start();
                if (this._timer != null)
                {
                    this._timer.Dispose();                                                      //停止主线程。
                }

            }
        }
        //结束计算
        private void stopCalcu(object source, ElapsedEventArgs e)
        {
            minThread.Abort();
            hourThread.Abort();
            dayThread.Abort();
            monthThread.Abort();
            yearThread.Abort();
        }
        //全选************************************************************************************
        private bool flag_selectall;
        private bool flag_selectintervaltype;
        private bool flag_selectmodulename;
        //“全选”复选框单击的时候，另外几个选择项置false
        private void checkBox_SelectAll_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectall = true;                          //选中“全选”框时，马上将全选的标志位置1。因为此时，程序也同时会去调用“全选”的CheckedChanged。只有全选的标志位为true，这个程序内部才能执行

            flag_selectintervaltype = false;                //将“选间隔类型”标志位，“选算法”标志位置0.这样，在下面两个checked状态改变时，才不会去执行对应的改变程序。
            flag_selectmodulename = false;
            checkBox_SelectIntervalType.Checked = false;    //上面将标志位置0后，再来改变对象的check，对应的changed程序会触发，但是内部被标志位屏蔽。
            checkBox_SelectModule.Checked = false;
            //checkBox_SelectAll.Checked = !checkBox_SelectAll.Checked;
        }
        //“全选”复选框check改变的时候，
        private void checkBox_SelectAll_CheckedChanged(object sender, EventArgs e)
        {

            if (flag_selectall)
            {
                for (int i = 0; i < lV_DataList.Items.Count; i++)
                {
                    lV_DataList.Items[i].Checked = checkBox_SelectAll.Checked;
                }
            }
        }
        //按计算间隔类型选*****************************************************************************
        //“按间隔类型”复选框被单击的时候，另外几个选择项置false
        private void checkBox_SelectIntervalType_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectintervaltype = true;
            flag_selectall = false;
            flag_selectmodulename = false;
            checkBox_SelectAll.Checked = false;
            checkBox_SelectModule.Checked = false;
        }
        //“按间隔类型选”复选框check变化的时候
        private void checkBox_SelectIntervalType_CheckedChanged(object sender, EventArgs e)
        {

            if (flag_selectintervaltype == true)
            {

                for (int i = 0; i < lV_DataList.Items.Count; i++)
                {
                    string intervalStr = lV_DataList.Items[i].SubItems[17].Text;
                    if (intervalStr.Contains(this.cb_intervaltype.Text))
                    {
                        lV_DataList.Items[i].Checked = this.checkBox_SelectIntervalType.Checked;
                    }
                    else
                    {
                        lV_DataList.Items[i].Checked = false;
                    }
                }
            }
        }
        //“间隔类型选择”下拉框，选项变化。直接调用“按间隔类型选”复选框
        private void cb_intervaltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.checkBox_SelectIntervalType_CheckedChanged(sender, e);
        }
        //按计算组件类型选*****************************************************************************
        //"按算法"复选框单击
        private void checkBox_SelectModule_MouseDown(object sender, MouseEventArgs e)
        {
            flag_selectmodulename = true;
            flag_selectall = false;
            flag_selectintervaltype = false;
            checkBox_SelectAll.Checked = false;
            checkBox_SelectIntervalType.Checked = false;
            //checkBox_SelectModule.Checked = !checkBox_SelectModule.Checked ; 
            //checkBox_SelectModule.Checked = true;
        }
        //"按算法"复选框，check变化
        private void checkBox_SelectModule_CheckedChanged(object sender, EventArgs e)
        {

            if (flag_selectmodulename)
            {
                for (int i = 0; i < lV_DataList.Items.Count; i++)
                {
                    string moduleStr = lV_DataList.Items[i].SubItems[7].Text;
                    if (moduleStr == this.cb_moduletype.Text)
                    {
                        lV_DataList.Items[i].Checked = this.checkBox_SelectModule.Checked;
                    }
                    else
                    {
                        lV_DataList.Items[i].Checked = false;
                    }
                }
            }
        }
        //"计算组件类型"下拉框，选择变化
        private void cb_moduletype_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.checkBox_SelectModule_CheckedChanged(sender, e);
        }
        //重新starttime“修改”按钮        
        private void btn_SetStartDate_Click(object sender, EventArgs e)
        {
            //修改选中的计算配置对象PslCalcuItems，并更对应的界面信息。
            updateCalcu();
            /*
            //更新数据库信息。使用backgroundwork后台异步调用updatepslcalcuconfig();            
            //后台线程设置
            worker.DoWork += new DoWorkEventHandler(updatepslcalcuconfig);                              //设置后台线程运行函数
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);          //设置后台线程“变更”消息处理函数
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateComplete);            //设置后台线程“运行完成”消息处理函数
            //启动后台线程
            worker.WorkerReportsProgress = true;    //允许后台发送“变更”消息
            worker.RunWorkerAsync();                //后台线程异步启动
            //启动前台进度条界面
            importProgress.StartPosition = FormStartPosition.CenterParent;  //进度条界面位置
            importProgress.ShowDialog();                                    //模态方式启动进度条界面，后台线程work以消息的方式操作进度条界面
            */

        }
        //
        private void lV_DataList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //搜索帮助按钮
        private void bt_searchHelp_Click(object sender, EventArgs e)
        {
            SearchHelp help = new SearchHelp();
            help.ShowDialog();
        }

        //主界面“导出”按钮
        private void btn_exportCSV_Click(object sender, EventArgs e)
        {
            if (this.lV_DataList.Items.Count != 0)
            {
                string[][] writeData = new string[this.lV_DataList.Items.Count][];
                int irow = 0;
                foreach (ListViewItem item in this.lV_DataList.Items)
                {
                    writeData[irow] = new string[20];
                    writeData[irow][0] = item.SubItems[0].Text;
                    writeData[irow][1] = item.SubItems[1].Text;
                    writeData[irow][2] = item.SubItems[2].Text;
                    writeData[irow][3] = item.SubItems[3].Text;
                    writeData[irow][4] = item.SubItems[4].Text;
                    writeData[irow][5] = item.SubItems[5].Text;
                    writeData[irow][6] = item.SubItems[6].Text;
                    writeData[irow][7] = item.SubItems[7].Text;
                    writeData[irow][8] = item.SubItems[8].Text;
                    writeData[irow][9] = item.SubItems[9].Text;
                    writeData[irow][10] = item.SubItems[10].Text;
                    writeData[irow][11] = item.SubItems[11].Text;
                    writeData[irow][12] = item.SubItems[12].Text;
                    writeData[irow][13] = item.SubItems[13].Text;
                    writeData[irow][14] = item.SubItems[14].Text;
                    writeData[irow][15] = item.SubItems[15].Text;
                    writeData[irow][16] = item.SubItems[16].Text;
                    writeData[irow][17] = item.SubItems[17].Text.Substring(0, item.SubItems[17].Text.Length - 1);
                    writeData[irow][18] = item.SubItems[17].Text.Substring(item.SubItems[17].Text.Length - 1, 1);
                    writeData[irow][19] = item.SubItems[18].Text;
                    irow = irow + 1;
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "另存为";//对话框标题
                sfd.InitialDirectory = "d:\\";//对话框初始目录
                sfd.Filter = "CSV文件|*.csv";//对话框所有可以选择的文件类型
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CsvFileReader.Save(writeData, sfd.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("导出到CSV文件失败，请检查目标文件是否被占用！");
                    }
                }
                MessageBox.Show("仿真数据已经成功写入CSV文件！");
            }
            else
            {
                MessageBox.Show("还未配置计算信息！");
            }
        }
        //快速定位到x行，行数输入
        private void tb_Fastfixed_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果按下回车键
            if (e.KeyChar == (char)Keys.Enter)
            {
                //
                try
                {
                    int fixedIndx = int.Parse(this.tb_Fastfixed.Text);
                    this.lV_DataList.EnsureVisible(fixedIndx);//滚动到指定的行位置
                    this.lV_DataList.Items[fixedIndx].Focused = true;
                }
                catch
                {
                    MessageBox.Show("请输入正确的行号！");
                }
            }
        }
        //定位按钮
        private void bt_Fixed_Click(object sender, EventArgs e)
        {
            try
            {
                int fixedIndx = int.Parse(this.tb_Fastfixed.Text);
                this.lV_DataList.EnsureVisible(fixedIndx);//滚动到指定的行位置
                this.lV_DataList.Items[fixedIndx].Focused = true;
            }
            catch
            {
                MessageBox.Show("请输入正确的行号！");
            }
        }
        #endregion

        #region 菜单
        //设置setup菜单
        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!calcurunningflag)
            {
                Setup setupform = new Setup();
                setupform.ShowDialog();
            }
            else
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行参数设定！");
                MessageBox.Show(messageStr, "计算引擎参数设置");
            }

        }
        //实时数据库连接测试
        private void rTDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //如报错“初始值设定项引发异常”，请检查DbHelper的数据库类型和连接字符串赋值是否正确。
            //注意，xml中定义的数据库类型必须与枚举型CurrentRTDbType中的描述一致，大小写敏感。
            //修改config项目中的AppConfig.xml，然后必须将该文档拷贝到..\lib\config下。
            string messageStr = "";
            if (RTDBDAO.connectTest())
            {
                messageStr = String.Format("数据库:\n--{0}\n连接测试结果:{1}", RTDBDAO.rtdbConnStr, RTDBDAO.testStatus); //非调试状态成功时不显示
                MessageBox.Show(messageStr, "实时数据库连接信息");
            }
            else
            {
                messageStr = String.Format("{0}连接失败：" + Environment.NewLine +
                                                    "1、检查实时数据库服务是否正常。" + Environment.NewLine +
                                                    "2、检查网络连接是否正常。" + Environment.NewLine +
                                                    "3、检查PGIM是否有可用的客户端连接剩余。" + Environment.NewLine +
                                                    "请检查log文件！", RTDBDAO.rtdbConnStr);
                MessageBox.Show(messageStr, "实时数据库连接失败。");
            }

        }
        //关系数据库连接测试
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //如报错“初始值设定项引发异常”，请检查DbHelper的数据库类型和连接字符串赋值是否正确。
            //注意，xml中定义的数据库类型必须与枚举型CurrentDbType中的描述一致，大小写敏感。
            //修改config项目中的AppConfig.xml，然后必须将该文档拷贝到..\lib\config下。
            string messageStr = "";
            if (IniTable.connectTest())
            {
                messageStr = String.Format("数据库:\r\n--{0}\n连接测试结果:{1}", IniTable.rdbConnStr, IniTable.testStatus);
                MessageBox.Show(messageStr, "关系数据库连接信息");
            }
            else
            {
                messageStr = String.Format("{0}连接失败：" + Environment.NewLine +
                                                      "1、检查关系数据库服务是否正常。" + Environment.NewLine +
                                                      "2、检查是否开启远程访问权限。" + Environment.NewLine +
                                                      "3、检查psldb数据库是否存在。" + Environment.NewLine +
                                                      "请检查log文件！", IniTable.rdbConnStr);
                MessageBox.Show(messageStr, "关系数据库连接失败");
                return; //启动时检测到关系数据库出错，要跳出，后面要从关系库读取数据。
            }
        }
        //初始化数据表
        private void iniToolTable_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行参数设定！");
                MessageBox.Show(messageStr, "计算引擎参数设置");
                return;
            }
            Boolean runflag = false;
            runflag = IniTable.createDB_psldb();
            if (runflag) runflag = IniTable.createTable_pslmodules();                                       //创建计算模块信息表            
            if (runflag) runflag = IniTable.createTable_pslcalcuconfig();                                   //创建概化计算配置表
            if (runflag) runflag = IniTable.createTable_pslhistorycalcuconfig();                            //创建历史补算配置表
            if (runflag) runflag = IniTable.createTable_psltagnameidmap();                                  //创建概化标签名称id映射表
            if (runflag) runflag = PSLTagNameIdMapDAO.SetAutoIncrement(APPConfig.rdbtable_constmaxnumber);  //设定概化标签名称id映射表tagid自增起始值为10000,前10000个id用于常量标签
            if (runflag) runflag = IniTable.createTable_webtagnameidmap();                                  //创建web端标签名称id映射表
            if (runflag) runflag = WebTagNameIdMapDAO.SetAutoIncrement(APPConfig.rdbtable_constmaxnumber);  //设定概化标签名称id映射表tagid自增起始值为10000,前10000个id用于常量标签
            if (runflag) runflag = IniTable.createTable_psltimerecord();                                    //创建时间记录表
            if (runflag) runflag = IniTable.createTable_pslshift();                                         //创建值次信息表
            if (runflag) runflag = IniTable.createTable_pslemployeeforshift();                              //创建值班员工信息表
            if (runflag) runflag = IniTable.createTable_pslplanforshift();                                  //创建员工排班表
            if (runflag) runflag = IniTable.createTable_pslscoreweight();                                   //创建得分权重表
            if (runflag) runflag = IniTable.createTable_pslMpvbase();
            if (runflag) runflag = IniTable.createTable_pslalgorithm();
            if (runflag) runflag = IniTable.createTable_pslcolumndata();
            if (runflag) runflag = IniTable.createTable_pslm2analogdiv();
            if (runflag) runflag = IniTable.createTable_pslmdevlimit();
            if (runflag) runflag = IniTable.createTable_pslmdevlimitmulti();
            if (runflag) runflag = IniTable.createTable_pslmfdistribute22();
            if (runflag) runflag = IniTable.createTable_pslmmultical();
            if (runflag) runflag = IniTable.createTable_pslmmultipv();
            if (runflag) runflag = IniTable.createTable_pslmmultipvavgdistance();
            if (runflag) runflag = IniTable.createTable_pslmmultipvavgdistancedetail();
            if (runflag) runflag = IniTable.createTable_pslmpvbasemulti();
            if (runflag) runflag = IniTable.createTable_pslmpvoverrangeeva();
            if (runflag) runflag = IniTable.createTable_pslmpvscoreeva();
            if (runflag) runflag = IniTable.createTable_pslmpvtyperangedetail();
            if (runflag) runflag = IniTable.createTable_pslmpvtyprange();
            if (runflag) runflag = IniTable.createTable_pslmpvutnv();
            if (runflag) runflag = IniTable.createTable_pslmpvutnvdetail();
            if (runflag) runflag = IniTable.createTable_psltimedatal();
            string sqlFilePath = AppDomain.CurrentDomain.BaseDirectory + "procedures.sql";
            string MysqlConStr = System.Configuration.ConfigurationManager.AppSettings["Local"].ToString();
            if (runflag) runflag = IniTable.ExecuteSqlFile(sqlFilePath, MysqlConStr);
            if (runflag && APPConfig.rdbtable_iniTableIncludePsldata == "1")
            {
                if (APPConfig.psldata_startyear > 2000 && APPConfig.psldata_endyear > 2000 && APPConfig.psldata_endyear > APPConfig.psldata_startyear)
                {
                    IniTable.createTable_psldata(APPConfig.psldata_startyear, APPConfig.psldata_endyear, APPConfig.psldata_intervalmonth);           //创建概化数据表
                    PSLDataDAO.optiTable_psldata(APPConfig.psldata_startyear, APPConfig.psldata_endyear, APPConfig.psldata_intervalmonth);           //优化概化数据表
                }
                else
                {
                    MessageBox.Show("概化数据表psldata起始和截止年份设定错误，请检查配置文件！");
                }
            }
            if (runflag)
            {
                //初始化计算配置对象
                if (!InitialCalcu()) return;
                MessageBox.Show("初始化成功，请检查数据表！");
            }
            else
            {
                MessageBox.Show("初始化失败，请检查数据表！");
            }


        }
        //导入得分权重表
        private void importweightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i;
            //得分权重表的管理方法是，
            //——每次手动选择读取菜单时，仅读入时间在最后的一个得分权重文件。
            //——如果标签名，权重组别，生效时间一致，就更新权重值
            //——否则就插入新记录
            //——更多的权重管理方法，在web端实现。
            //1、值次文件遍历
            string csvFilePath = System.Environment.CurrentDirectory + "\\WeightConfig\\";
            string filepre = String.Format("WeightConfig").ToUpper();

            //首先寻找合适的文件
            FileSystemInfo info;
            info = new DirectoryInfo(csvFilePath);                  //查询路径文件夹是否存在
            if (!info.Exists)
            {
                MessageBox.Show("找不到得分权重配置文件夹WeightConfig！");
                return;
            }
            DirectoryInfo dir = info as DirectoryInfo;              //如果路径文件夹存在，则获取文件夹对象
            if (dir == null)
            {
                MessageBox.Show("找不到得分权重配置文件夹WeightConfig！");
                return;
            }
            FileSystemInfo[] files = dir.GetFileSystemInfos();      //获取路径文件夹下所有文件

            //首先寻找合适的文件
            DateTime maxdatetime = new DateTime();                     //记录文件最大时间
            for (i = 0; i < files.Length; i++)
            {
                try
                {
                    string[] fields = new string[3];
                    //文件名称必须由2部分构成，如WeightConfig_2018-01.csv
                    fields = files[i].Name.Split('_');
                    string weightname = fields[0].ToUpper();                                            //文件名称                   
                    DateTime filedate = DateTime.Parse(fields[1].Substring(0, 7) + "-01 00:00:00");     //文件时间
                    string filetype = fields[1].Split('.')[1];
                    if (weightname.ToUpper() == filepre &&
                        filetype.ToUpper() == "CSV"
                       )
                    {
                        if (filedate > maxdatetime)
                            maxdatetime = filedate;
                    }

                }
                catch (Exception ex)
                {
                    continue;   //文件格式有误，直接下一个文件夹
                }

            }
            if (maxdatetime <= DateTime.Parse("1970-01-01"))
            {
                MessageBox.Show("找不到合适的得分权重配置文件，请检查！");
                return;
            }
            //读取时间最大的文件的配置数据
            string filename = csvFilePath + filepre + "_" + maxdatetime.ToString("yyyy-MM") + ".csv";
            string[][] filedata = CsvFileReaderForModule.Read(filename);
            if (filedata == null || filedata.Length == 0)
            {
                MessageBox.Show("读取不到有效的得分权重配置信息，请检查！有可能是csv文件正在被占用！");
                return;
            }

            //检查标签是否在标签映射表中
            Dictionary<string, uint> TagName2Id = PSLTagNameIdMapDAO.ReadMap();

            List<string> errorlines = new List<string>();
            for (i = 1; i < filedata.Length; i++)
            {
                string tagname = filedata[i][1];
                if (tagname.Trim() != "" && !TagName2Id.ContainsKey(tagname.Trim().ToUpper())) errorlines.Add((i - 1).ToString());
            }

            if (errorlines.Count != 0)
            {
                MessageBox.Show("得分加权值配置表中，有得分标签在标签列表中不存在。" + Environment.NewLine + "不存在的标签所在行数：" + string.Join(",", errorlines.ToArray()));
                return;
            }

            for (i = 1; i < filedata.Length; i++)
            {
                try
                {
                    string tagname = filedata[i][1];
                    string tagdesc = filedata[i][2];
                    string weighttype = filedata[i][3];
                    double weightvalue = double.Parse(filedata[i][4]);
                    DateTime validdate = DateTime.Parse(DateTime.Parse(filedata[i][5]).ToString("yyyy-MM"));
                    PSLScoreWeightConfig.WriteOrUpdate(tagname, tagdesc, weighttype, weightvalue, validdate);
                }
                catch
                {
                    continue;
                }
            }

        }
        //抽取计算模块信息到pslmodules表
        private void extractModulesInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行参数设定！");
                MessageBox.Show(messageStr, "计算引擎参数设置");
                return;
            }
            bool flag = true;
            //每次抽取计算模块信息前，应该先清除原来所有记录
            flag = PSLModulesDAO.ClearData();
            if (!flag)
            {
                string messageStr = String.Format("清空模块信息表错误，检查log文件！");
                MessageBox.Show(messageStr);
                return;
            }
            //抽取计算模块信息到计算模块信息表
            flag = PSLModulesDAO.extractData();
            if (!flag)
            {
                string messageStr = String.Format("抽取模块信息表错误，检查log文件！");
                MessageBox.Show(messageStr);
                return;
            }

            if (flag)
            {
                //初始化计算配置对象
                if (!InitialCalcu()) return;
                MessageBox.Show("计算模块信息抽取完毕，请检查数据表pslmodules！");
            }
            else
            {
                MessageBox.Show("计算模块信息抽取错误，详细错误信息请检查log文件！");
            }

        }
        //算法信息导出到csv文件
        private void ExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<PSLModule> moduleinfo = new List<PSLModule>();
            //从数据表中读取数据
            try
            {
                moduleinfo = PSLModulesDAO.ReadData();
            }
            catch (Exception ex)
            {
                string errInfo = string.Format("算法信息读取错误。详细错误信息：", ex.ToString()) + Environment.NewLine;
                logHelper.Fatal(errInfo);
                string messageStr = String.Format("算法信息读取错误，详细错误信息请检查log文件！");
                MessageBox.Show(messageStr, "算法信息导出");
                return;

            }

            if (moduleinfo.Count != 0)
                moduleinfo = moduleinfo.OrderBy(n => n.modulename).ToList();
            //转换成csv的string[][]格式
            string[][] csvdata = new string[moduleinfo.Count][];

            for (int i = 0; i < moduleinfo.Count; i++)
            {
                csvdata[i] = new string[13];
                csvdata[i][0] = moduleinfo[i].id.ToString();
                csvdata[i][1] = moduleinfo[i].modulename;
                csvdata[i][2] = moduleinfo[i].moduledesc;
                csvdata[i][3] = moduleinfo[i].moduleclass;
                csvdata[i][4] = moduleinfo[i].modulealgorithms;
                csvdata[i][5] = moduleinfo[i].modulealgorithmsflag;

                csvdata[i][6] = moduleinfo[i].moduleparaexample;
                csvdata[i][7] = moduleinfo[i].moduleparadesc;
                csvdata[i][8] = moduleinfo[i].moduleoutputtable;
                csvdata[i][9] = moduleinfo[i].moduleoutputnumber.ToString();
                csvdata[i][10] = moduleinfo[i].moduleoutputtype;
                csvdata[i][11] = moduleinfo[i].moduleoutputdescs;
                csvdata[i][12] = moduleinfo[i].moduleoutputdescscn;
                //csvdata[i][13] = "";
            }

            //写入到csv文件
            try
            {
                string csvFilePath = System.Environment.CurrentDirectory + "\\CalcuConfig\\";
                string csvFileName = "ModuleInfo.csv";
                CsvFileReader.Save(csvdata, csvFilePath + csvFileName);
                string messageStr = String.Format("算法信息导出完毕，请检查CalcuConfig\\ModuleInfo.csv文件！");
                MessageBox.Show(messageStr, "算法信息导出");
                return;
            }
            catch (Exception ex)
            {
                string errInfo = string.Format("算法信息导出错误。详细错误信息：", ex.ToString());
                logHelper.Fatal(errInfo);
                string messageStr = String.Format("算法信息导出错误，详细错误信息请检查log文件！");
                MessageBox.Show(messageStr, "算法信息导出");
                return;
            }

        }
        //检查期望曲线和得分曲线
        private void checkCurveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //
            CheckCurve checkcurve = new CheckCurve();
            checkcurve.ShowDialog();
        }
        //打开索引
        private void OPenIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            DateTime startDate=DateTime.Parse(APPConfig.psldata_startyear.ToString() + "-1-1");
            DateTime endDate=DateTime.Parse(APPConfig.psldata_endyear.ToString() + "-12-30");
            bool flag = PSLDataDAO.OpenIndex(startDate, endDate);
            */
            if (false)
            {
                MessageBox.Show("所有PSLData数据表索引已经开启。");
            }
            else
            {
                MessageBox.Show(" PSLData数据表索引开启失败。");
            }
        }
        //关闭索引
        private void CloseIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            DateTime startDate = DateTime.Parse(APPConfig.psldata_startyear.ToString() + "-1-1");
            DateTime endDate = DateTime.Parse(APPConfig.psldata_endyear.ToString() + "-12-30");
            bool flag = PSLDataDAO.CloseIndex(startDate, endDate);
            */
            if (false)
            {
                MessageBox.Show("所有PSLData数据表索引已经关闭。");
            }
            else
            {
                MessageBox.Show(" PSLData数据表索引关闭失败。");
            }
        }

        //手动设置历史数据重算
        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!calcurunningflag)
            {
                HistoryCalcu historycalcu = new HistoryCalcu(PslCalcuItems);
                historycalcu.IntervalType = this.IntervalType;
                historycalcu.StartPosition = FormStartPosition.CenterScreen;
                historycalcu.notifyIcon1 = this.notifyIcon1;            //将主界面的notifyIcon1传给子界面，供子界面使用
                historycalcu.ShowDialog();
            }
            else
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行历史数据重算！");
                MessageBox.Show(messageStr, "历史数据重算");
            }
        }
        //自动设置历史数据重算
        private void AutoReCalcuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!calcurunningflag)
            {
                HistoryCalcuAuto historycalcuauto = new HistoryCalcuAuto(PslCalcuItems);
                historycalcuauto.StartPosition = FormStartPosition.CenterScreen;
                //historycalcuauto.notifyIcon1 = this.notifyIcon1;            //将主界面的notifyIcon1传给子界面，供子界面使用
                historycalcuauto.ShowDialog();
            }
            else
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行历史数据重算！");
                MessageBox.Show(messageStr, "历史数据重算");
            }
        }

        //从CSV文件导入或追加计算配置
        ImportProgress importProgress = new ImportProgress();                   //进度条窗体
        protected BackgroundWorker worker = new BackgroundWorker();             //后台线程
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)  //后台线程，进度条窗体进度控制函数：后台线程对象操作状态改变时（状态改变消息），更新界面
        {
            importProgress.ProgressText = e.UserState.ToString();       //更新label
            importProgress.ProgressValue = e.ProgressPercentage;        //更新进度条            
        }
        //抽取值次信息到概化数据库
        private void 检查并抽取值次信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadShiftInfo2PSLData readshift = new ReadShiftInfo2PSLData();
            readshift.readShiftItems = PslCalcuItems.Where(m => m.fmodulename == "MReadShift").ToList();
            readshift.ShowDialog();
        }
        //抽取常数标签信息到概化库
        private void ReadConstInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReadConstInfo2PSLData readconst = new ReadConstInfo2PSLData();
            readconst.readConstItems = PslCalcuItems.Where(m => m.fmodulename == "MReadConst").ToList();
            readconst.ShowDialog();
        }
        //1、从csv文件导入计算配置文件到pslcalcuconfig表，完全重建标签id映射
        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再导入计算配置组态信息！");
                MessageBox.Show(messageStr, "导入计算配置组态信息");
                return;
            }
            DialogResult dr;
            string StrMsg = "";

            StrMsg = "如果重新导入计算配置组态信息："
                    + Environment.NewLine
                    + "——原有的计算配置组态信息将完全丢失！"
                    + Environment.NewLine
                    + "——计算结果标签id会全部被重新分配，原有的计算结果也将全部删除！"
                    + Environment.NewLine
                    + "——常数读取项必须配置在第一张计算配置组态信息表中！"
                    + Environment.NewLine
                    + "确定要重新导入吗？";

            dr = MessageBox.Show(StrMsg, "导入计算配置组态信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.No || dr == DialogResult.Cancel)
            {
                return;
            }

            this.resetTagIDMap = true;

            //先清空当前listview
            lV_DataList.Items.Clear();
            //清空pslconfig表
            PSLCalcuConfigDAO.ClearData();
            //清空已存在的pslitem列表
            PslCalcuItems.Clear();
            //清空PSLTagIdMap表
            PSLTagNameIdMapDAO.ClearData();
            PSLTagNameIdMapDAO.SetAutoIncrement(APPConfig.rdbtable_constmaxnumber);       //设定id从100开始自增。注意，这个是预留出一个范围的id给特殊的值用。在PSLTagNameIdMapDAO.generateMap()会对这些变量进行初始化
            //清空WebTagidmap表
            WebTagNameIdMapDAO.ClearData();
            WebTagNameIdMapDAO.SetAutoIncrement(APPConfig.rdbtable_constmaxnumber);       //设定id从100开始自增。注意，这个是预留出一个范围的id给特殊的值用。在PSLTagNameIdMapDAO.generateMap()会对这些变量进行初始化
            //重建PSLData表
            if (APPConfig.psldata_startyear > 2000 && APPConfig.psldata_endyear > 2000 && APPConfig.psldata_endyear > APPConfig.psldata_startyear)
            {
                IniTable.createTable_psldata(APPConfig.psldata_startyear, APPConfig.psldata_endyear, APPConfig.psldata_intervalmonth);            //创建概化数据表
                PSLDataDAO.optiTable_psldata(APPConfig.psldata_startyear, APPConfig.psldata_endyear, APPConfig.psldata_intervalmonth);            //优化概化数据表
            }
            else
            {
                MessageBox.Show("概化数据表psldata起始和截止年份设定错误，请检查配置文件！");
            }

            //选择概化计算配置信息文件
            OpenFileDialog csvfile = new OpenFileDialog();
            csvfile.Multiselect = false;                             //允许同时选择多个文件
            csvfile.Title = "请选择组态配置文件";
            csvfile.InitialDirectory = "d:\\";                       //默认打开路径
            csvfile.Filter = "csv数据文件|*.csv|所有文件|*.*";        //对话框过滤条件：1、按csv过滤；2、按所有文件过滤
            csvfile.FilterIndex = 1;                                 //默认选择第一个过滤条件
            csvfile.RestoreDirectory = true;

            if (csvfile.ShowDialog() == DialogResult.OK)
            {
                APPConfig.rdbtable_tag2idalwaysreset = "0";              //修改全局变量，生成标签id映射时重新分配
                //由全局变量获取配置文件信息，共后台线程的主函数使用
                this.CSVFileName = csvfile.FileName;//全局变量
                //后台线程设置                
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);                                     //设置后台线程运行函数
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);          //设置后台线程“变更”消息处理函数
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted); //设置后台线程“运行完成”消息处理函数
                //启动后台线程
                worker.WorkerReportsProgress = true;    //允许后台发送“变更”消息
                worker.RunWorkerAsync();                //后台线程异步启动  开始执行一个后台操作。调用该方法后，将触发BackgroundWorker.DoWork事件，并以异步的方式执行DoWork事件中的代码
                //启动前台进度条界面
                importProgress.StartPosition = FormStartPosition.CenterParent;  //进度条界面位置
                importProgress.ShowDialog();                                    //模态方式启动进度条界面，后台线程work以消息的方式操作进度条界面                
            }
        }
        //2、从csv文件追加计算配置文件到pslcalcuconfig表，原标签id映射不变，在后面继续添加
        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再追加计算配置组态信息！");
                MessageBox.Show(messageStr, "追加计算配置组态信息");
                return;
            }
            DialogResult dr;
            string StrMsg = "";

            StrMsg = "如果追加计算配置组态信息："
                    + Environment.NewLine
                    + "——原有的计算配置组态信息将保留！"
                    + Environment.NewLine
                    + "——原有的计算结果标签id不变，原有的计算结果也全部保留！"
                    + Environment.NewLine
                    + "——新的计算结果标签id，在原有id后继续分配！"
                    + Environment.NewLine
                    + "——常数读取项必须配置在第一张计算配置组态信息表中！"
                    + Environment.NewLine
                    + "确定要追加吗？";

            dr = MessageBox.Show(StrMsg, "追加计算配置组态信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.No || dr == DialogResult.Cancel)
            {
                return;
            }

            this.resetTagIDMap = false;

            //与导入不同。这里           
            //——不清空pslconfig表
            //——不清空PSLTagIdMap表
            //——不清空WebTagIdMap表
            //——不重建PSLData表
            //选择概化计算配置信息文件
            OpenFileDialog csvfile = new OpenFileDialog();
            csvfile.Multiselect = false;                             //允许同时选择多个文件
            csvfile.Title = "请选择组态配置文件";
            csvfile.InitialDirectory = "d:\\";                       //默认打开路径
            csvfile.Filter = "csv数据文件|*.csv|所有文件|*.*";        //对话框过滤条件：1、按csv过滤；2、按所有文件过滤
            csvfile.FilterIndex = 1;                                 //默认选择第一个过滤条件
            csvfile.RestoreDirectory = true;

            if (csvfile.ShowDialog() == DialogResult.OK)
            {
                //先清空当前listview
                lV_DataList.Items.Clear();
                //修改重新分配标志置为0
                APPConfig.rdbtable_tag2idalwaysreset = "0";              //修改全局变量，生成标签id映射时重新分配标志置为0
                //由全局变量获取配置文件信息，共后台线程的主函数使用
                this.CSVFileName = csvfile.FileName;//全局变量
                //后台线程设置                
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);                                     //设置后台线程运行函数
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);          //设置后台线程“变更”消息处理函数
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted); //设置后台线程“运行完成”消息处理函数
                //启动后台线程
                worker.WorkerReportsProgress = true;    //允许后台发送“变更”消息
                worker.RunWorkerAsync();                //后台线程异步启动
                //启动前台进度条界面
                importProgress.StartPosition = FormStartPosition.CenterParent;  //进度条界面位置
                importProgress.ShowDialog();                                    //模态方式启动进度条界面，后台线程work以消息的方式操作进度条界面                
            }
        }

        //后台处理函数，检测并导入计算配置信息
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.IMPORTOK = true;
            //读取计算配置信息
            worker.ReportProgress(0, String.Format("读取计算配置信息！"));
            ConfigCSV configcsv = new ConfigCSV();
            configcsv.MAX_NUMBER_CONSTTAG = APPConfig.rdbtable_constmaxnumber;
            configcsv.readyPslCalcuItems = PslCalcuItems;                  //在追加新计算项时，有可能会用到已经存在的计算项
            configcsv.importdata = CsvFileReader.Read(this.CSVFileName);
            //检查读入的csv数据
            worker.ReportProgress(0, String.Format("检查配置信息！"));
            this.IMPORTOK = configcsv.CheckCSVData();

            //从配置文件读取常量标签到对应的计算项输出
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("读取常数标签配置表！"));  //ReportProgress  引发ProgressChanged事件
                this.IMPORTOK = configcsv.ReadAndReplaceConst();
            }

            //检查条件变量和条件表达式
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("检查条件变量和条件表达式！"));
                this.IMPORTOK = configcsv.checkfCond();
            }

            //csv数据，针对sourcetagname字段，pgim路径带\的处理。
            //——Pgim标签名称中含有\,由于转义字符的原因，在程序内部处理、数据库存储时，应该统一替换成^最为方便。仅在界面显示，和pgim接口调用时才替换成\。
            //——另外，由于概化程序主引擎，使用rtdbhelper时，并不区分pgim还是其他库，所以，在替换\时，还不能替换成“.”。否则在使用数据库接口时，pgim的“.“会与其他的点混淆。            
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("处理pgim路径字符！"));
                this.IMPORTOK = configcsv.PGIMPathCharChange();
            }

            //自动生成计算结果标签、别名、中文描述
            //不自动生成计算结果标签：采用configcsv中psltagnames字段中手动配置的标签名，作为计算结果的标签名称
            //自动生成计算结果标签：采用configcsv中的modulename对应计算模块的输出描述moduleoutputdescs来重新生成psltagnames字段。只有当psltagnames为空时，才发生替换
            //if (APPConfig.rdbtable_resulttagauto == "1" && IMPORTOK)
            if (this.IMPORTOK)               //2017.12.26 修改。不在按照APPConfig.rdbtable_resulttagauto来决定是否自动生成计算结果标签。一律自动生成
            {
                worker.ReportProgress(0, String.Format("自动生成标签！"));
                this.IMPORTOK = configcsv.AutoGeneratePSLTags();
            }

            //判断rdbset类型的源源标签是否正确。在自动生成标签后，计算结果描述字段内容，由于核心描述替换为真正的计算结果描述。此时将rdbset类型的源标签位置，替换为计算结果描述
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("替换rdbset类型源标签！"));
                this.IMPORTOK = configcsv.AutoReplaceSourceTags();
            }

            //检查计算结果标签是否唯一，和已经在标签映射表中的标签有没有重名。如果有重名，报错，如果没有重名则继续。
            //——特别注意这里，无论是导入计算配置（重新分配id），还是追加计算配置（不重新分配id）。这里都需要检查
            //——对于导入计算配置（重新分配id），有可能多个概化计算客户端同时运行，针对不同的实时库，但是针对相同的概化库。这要求两个概化库之间不能重名
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("检查计算配置表中计算结果标签与库中标签有无重名！"));
                this.IMPORTOK = PSLTagNameIdMapDAO.CheckUnique(configcsv);
            }

            //检查计算结果别名是否唯一
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("检查计算配置表中计算结果别名与库中标签有无重名！"));
                this.IMPORTOK = WebTagNameIdMapDAO.CheckUnique(configcsv);
            }

            //检查概化型源标签是否存在：源标签必须存在于即将导入的csv配置信息表中或者psltagidnamemap表中
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("检查计算配置表中概化类型的源标签是否存在！"));
                this.IMPORTOK = PSLTagNameIdMapDAO.CheckSourcePSLName(configcsv);
            }

            //检查条件标签名是否存在：计算条件标签名必须存在于即将导入的csv配置信息表或者psltagidnamemap表中
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("检查计算配置表中计算条件标签是否存在！"));
                this.IMPORTOK = PSLTagNameIdMapDAO.CheckCondPSLName(configcsv);
            }

            //写入pslcalcuconfig
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("导入计算配置信息..."));
                PSLCalcuConfigDAO.worker = worker;                              //将work传入PSLCalcuConfigDAO，供子程序刷新界面
                this.IMPORTOK = PSLCalcuConfigDAO.ImportFromCSV(configcsv);     //数据表PSLCalcuConfig导入数据 
            }

            //生成计算引擎内部概化标签名字和id映射表psltagidnamemap
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("生成计算引擎内部概化标签名称和id映射表..."));
                PSLTagNameIdMapDAO.worker = worker;                                                 //将work传入PSLTagNameIdMapDAO，供子程序刷新界面
                PSLTagNameIdMapDAO.MAX_NUMBER_CONSTTAG = APPConfig.rdbtable_constmaxnumber;         //常数标签最大数量
                this.IMPORTOK = PSLTagNameIdMapDAO.generateMap(configcsv, this.resetTagIDMap);      //读取刚生成的映射表，给后面填写原来的配置文件用。 
            }
            //生成web端概化标签别名和id映射表webtagidnamemap
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("生成web端概化标签别名和id映射表..."));
                WebTagNameIdMapDAO.worker = worker;                             //将work传入PSLTagNameIdMapDAO，供子程序刷新界面
                WebTagNameIdMapDAO.MAX_NUMBER_CONSTTAG = APPConfig.rdbtable_constmaxnumber;   //常数标签最大数量
                this.IMPORTOK = WebTagNameIdMapDAO.generateMap(configcsv, this.resetTagIDMap);
            }

            //将生成的计算结果标签及id号，写入到csv文件，文件名称比打开的文件多一个_resulttag后缀
            if (this.IMPORTOK)
            {
                //读取标签id映射
                Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();
                //写回csv文件
                //worker.ReportProgress(0, String.Format("回写csv文件..."));
                string csvfilename = this.CSVFileName.ToString();
                int pos = csvfilename.LastIndexOf('.');
                string newCSVFilename = csvfilename.Substring(0, pos) + "_resultTags.csv";      //文件名
                string[][] csvdataWithTagId = csvDataPlusTagId(configcsv, TagName2Id);  //在数据string[][]末尾，添加计算结果标签对应的id号字符串。
                CsvFileReader.Save(csvdataWithTagId, newCSVFilename);
            }
            else
            {
                MessageBox.Show("在数据库中更新计算配置信息过程中发生错误，请检查log文件！");
            }


        }
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)   //进度条窗体进度控制：后台线程完成时（操作完成消息），关闭进度条窗体
        {

            importProgress.Close();
            //执行完毕后，要移除注册的事件
            worker.DoWork -= new DoWorkEventHandler(worker_DoWork);                                     //执行完毕，解除注册的事件
            worker.ProgressChanged -= new ProgressChangedEventHandler(worker_ProgressChanged);          //执行完毕，解除注册的事件
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted); //执行完毕，解除注册的事件
            //初始化计算配置对象
            if (this.IMPORTOK)
            {
                InitialCalcu();     //这里不用再判断是否会成功。由于程序初始化时，必然执行InitialCalcu，不成功，则直接退出程序。可以执行到这里，则说明InitialCalcu可以运行成功。
                MessageBox.Show("计算配置导入完毕，请检查主界面listview中的计算配置信息！");
            }

        }

        //修改算法配置中的一般信息
        //——修改算法信息程序，与导入程序完全相同。仅在两处有差别：
        //——在写入pslcalcuconfig表时，这里是更新部分可更新字段
        //——在生成tagnameid映射表时，这里仅更新描述。
        private void rectifyToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再修改计算配置组态信息！");
                MessageBox.Show(messageStr, "修改计算配置组态信息");
                return;
            }
            DialogResult dr;
            string StrMsg = "";

            StrMsg = "根据CSV配置文件批量修改计算配置组态信息："
                    + Environment.NewLine
                    + "修改信息不涉及概化标签及id的改动与调整。不可修改的信息字段："
                    + Environment.NewLine
                    + "——数据源信息：计算序号、源数据类型、标签名称、计算公式、计算组、组序号、计算起始时间。"
                    + Environment.NewLine
                    + "——计算周期信息：计算间隔、间隔单位。这两项影响计算结果标签。"
                    + Environment.NewLine
                    + "——不处理常数标签和常数标签读值算法。"
                    + Environment.NewLine
                    + "可修改的信息字段："
                    + Environment.NewLine
                    + "——数据源信息：描述、工程单位、量程上限、量程下限。"
                    + Environment.NewLine
                    + "——算法信息：算法标志、计算参数、计算条件表达式。"
                    + Environment.NewLine
                    + "——计算结果信息：计算结果描述。"
                    + Environment.NewLine
                    + "——计算周期信息：计算时延。"
                    + Environment.NewLine
                    + "确定要批量修改吗？";

            dr = MessageBox.Show(StrMsg, "批量修改计算配置组态中的一般信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.No || dr == DialogResult.Cancel)
            {
                return;
            }

            //选择概化计算配置信息文件
            OpenFileDialog csvfile = new OpenFileDialog();
            csvfile.Multiselect = false;                             //允许同时选择多个文件
            csvfile.Title = "请选择组态配置文件";
            csvfile.InitialDirectory = "d:\\";                       //默认打开路径
            csvfile.Filter = "csv数据文件|*.csv|所有文件|*.*";        //对话框过滤条件：1、按csv过滤；2、按所有文件过滤
            csvfile.FilterIndex = 1;                                 //默认选择第一个过滤条件
            csvfile.RestoreDirectory = true;

            if (csvfile.ShowDialog() == DialogResult.OK)
            {
                //由全局变量获取配置文件信息，共后台线程的主函数使用
                this.CSVFileName = csvfile.FileName;//全局变量
                //后台线程设置                
                worker.DoWork += new DoWorkEventHandler(worker_updateConfig);                                       //设置后台线程运行函数
                worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);                  //设置后台线程“变更”消息处理函数
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_updateConfigCompleted);      //设置后台线程“运行完成”消息处理函数
                //启动后台线程
                worker.WorkerReportsProgress = true;    //允许后台发送“变更”消息
                worker.RunWorkerAsync();                //后台线程异步启动
                //启动前台进度条界面
                importProgress.StartPosition = FormStartPosition.CenterParent;  //进度条界面位置
                importProgress.ShowDialog();                                    //模态方式启动进度条界面，后台线程work以消息的方式操作进度条界面

            }
        }
        //更新配置信息表一般信息
        void worker_updateConfig(object sender, DoWorkEventArgs e)
        {
            //更新配置信息表的主要过程基本一致。只有两处不同：
            //——在写pslconfig表时，不一样。这里仅更新部分字段。而且有前提条件，必须，tagid、sourcetag、module一致，才可以更新
            //——在重写tagnameid映射表时，不一样。这里仅更新描述

            this.IMPORTOK = true;
            //读取计算配置信息
            worker.ReportProgress(0, String.Format("读取计算配置信息！"));
            ConfigCSV configcsv = new ConfigCSV();
            configcsv.importdata = CsvFileReader.Read(this.CSVFileName);        //要修改的信息，存在csv文件中
            configcsv.readyPslCalcuItems = PslCalcuItems;                       //被修改的对象，存在PslCalcuItems中

            //检查读入的csv数据
            worker.ReportProgress(0, String.Format("检查配置信息！"));
            this.IMPORTOK = configcsv.CheckCSVData();

            //注意：更新配置信息中不处理常数标签

            //检查条件变量和条件表达式
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("检查条件变量和条件表达式！"));
                this.IMPORTOK = configcsv.checkfCond();
            }

            //csv数据，针对sourcetagname字段，pgim路径带\的处理。
            //——Pgim标签名称中含有\,由于转义字符的原因，在程序内部处理、数据库存储时，应该统一替换成^最为方便。仅在界面显示，和pgim接口调用时才替换成\。
            //——另外，由于概化程序主引擎，使用rtdbhelper时，并不区分pgim还是其他库，所以，在替换\时，还不能替换成“.”。否则在使用数据库接口时，pgim的“.“会与其他的点混淆。            
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("处理pgim路径字符！"));
                this.IMPORTOK = configcsv.PGIMPathCharChange();
            }

            //是否自动生成计算结果标签
            //不自动生成计算结果标签：采用configcsv中psltagnames字段中手动配置的标签名，作为计算结果的标签名称
            //自动生成计算结果标签：采用configcsv中的modulename对应计算模块的输出描述moduleoutputdescs来重新生成psltagnames字段。只有当psltagnames为空时，才发生替换
            //if (APPConfig.rdbtable_resulttagauto == "1" && IMPORTOK)
            if (this.IMPORTOK)               //2017.12.26 修改。不在按照APPConfig.rdbtable_resulttagauto来决定是否自动生成计算结果标签。一律自动生成
            {
                worker.ReportProgress(0, String.Format("自动生成标签！"));
                this.IMPORTOK = configcsv.AutoGeneratePSLTags();
            }

            //在自动生成标签后，计算结果描述字段内容，由核心描述替换为真正的计算结果描述。此时将rdbset类型的源标签位置，替换为计算结果描述
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("替换rdbset类型源标签！"));
                this.IMPORTOK = configcsv.AutoReplaceSourceTags();
            }

            //更新pslcalcuconfig。****************这里与导入程序不同******************
            //——仅更新部分字段，并且有前提条件
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("更新计算配置信息，"));
                PSLCalcuConfigDAO.worker = worker;                              //将work传入PSLCalcuConfigDAO，供子程序刷新界面
                this.IMPORTOK = PSLCalcuConfigDAO.UpdateGeneralInfoFromCSV(configcsv);     //数据表PSLCalcuConfig导入数据 
            }

            //更新psltagidnamemap的中文描述。****************这里与导入程序不同******************
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("更改标签名称中文描述，"));
                PSLTagNameIdMapDAO.worker = worker;                     //将work传入PSLTagNameIdMapDAO，供子程序刷新界面
                PSLTagNameIdMapDAO.UpdateDesc(configcsv, this.resetTagIDMap);

            }
            //更新webtagidnamemap的中文描述。
            if (this.IMPORTOK)
            {
                worker.ReportProgress(0, String.Format("更改标签别名中文描述，"));
                WebTagNameIdMapDAO.worker = worker;                     //将work传入PSLTagNameIdMapDAO，供子程序刷新界面
                WebTagNameIdMapDAO.UpdateDesc(configcsv, this.resetTagIDMap);

            }
        }
        //更新完毕
        void worker_updateConfigCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            importProgress.Close();
            //执行完毕后，要移除注册的事件
            worker.DoWork -= new DoWorkEventHandler(worker_updateConfig);                                       //执行完毕，解除注册的事件
            worker.ProgressChanged -= new ProgressChangedEventHandler(worker_ProgressChanged);                  //执行完毕，解除注册的事件
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_updateConfigCompleted);      //执行完毕，解除注册的事件
            //初始化计算配置对象
            if (this.IMPORTOK)
            {
                InitialCalcu();     //这里不用再判断是否会成功。由于程序初始化时，必然执行InitialCalcu，不成功，则直接退出程序。可以执行到这里，则说明InitialCalcu可以运行成功。
                MessageBox.Show("计算配置更新完毕，请检查主界面listview中的计算配置信息！");
            }
        }
        //修改计算配置组态信息标签信息
        private void updatetagnameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateTagname updatetagname = new UpdateTagname();
            updatetagname.ShowDialog();
        }


        //删除计算配置项
        //——删除对应标签的所有数据（最耗时，不可恢复，放在最前面）
        //——删除对应的标签（快）
        //——删除对应的算法（快）
        private void deleteCalcuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行数据删除！");
                MessageBox.Show(messageStr, "数据删除");
                return;
            }

            //读取标签id映射
            Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();

            DeleteConfig deleteconfig = new DeleteConfig();
            deleteconfig.startYearTable = APPConfig.psldata_startyear;
            deleteconfig.endYearTable = APPConfig.psldata_endyear;
            deleteconfig.PslCalcuItems = PslCalcuItems;
            deleteconfig.TagName2Id = TagName2Id;
            deleteconfig.ShowDialog();
        }

        //检查实时标签有效性
        private void checkTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行参数设定！");
                MessageBox.Show(messageStr, "计算引擎参数设置");
                return;
            }
            if (PslCalcuItems.Count > 0)
            {
                //读取所变量信息
                List<DBInterface.RTDBInterface.SignalComm> rtdbtags = new List<DBInterface.RTDBInterface.SignalComm>();
                rtdbtags = RTDBDAO.ReadTagList();  //读取实时数据库所有变量
                if (RTDBDAO.ErrorFlag)
                {
                    MessageBox.Show("连接实时数据库或读取标签点时发生错误，请检查log文件！");
                    return;
                }

                //取出标签名
                List<string> tags = new List<string>();
                foreach (DBInterface.RTDBInterface.SignalComm signal in rtdbtags)
                {
                    tags.Add(signal.tagname.Replace("\\", "^"));
                }

                //检查计算配置中标签是否在tags中
                int count = 0;
                string messageStr = "";
                foreach (PSLCalcuItem item in PslCalcuItems)
                {
                    if (item.sourcetagdb == "rtdb")
                    {
                        string[] inputtags = Regex.Split(item.sourcetagname, ";|；");


                        for (int i = 0; i < inputtags.Length; i++)
                        {
                            if (!tags.Contains(inputtags[i]))
                            {
                                count = count + 1;
                                messageStr = String.Format("计算配置实时标签检查错误：第{0}条计算项的实时标签点{1}在实时数据库中不存在！", i + 1, inputtags[i]);
                                logHelper.Fatal(messageStr);
                            }
                        }
                    }

                }
                //
                if (count == 0)
                {
                    MessageBox.Show("配置信息表实时标签检查完毕，没有错误！");
                }
                else
                {
                    MessageBox.Show("配置信息表中有实时标签在实时数据库中不存在，请检查log文件！");
                }
            }
            else
            {
                MessageBox.Show("还未导入计算配置信息！");
            }
        }
        //计算结果查询
        private void ToolStripMenuItemSearchData_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行数据查询！");
                MessageBox.Show(messageStr, "数据查询");
                return;
            }

            //读取标签id映射
            Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();

            DataSearch searchForm = new DataSearch();
            searchForm.tagsdic = TagName2Id;
            searchForm.Show();
        }
        //计算结果删除
        private void ToolStripMenuItemDelData_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行数据删除！");
                MessageBox.Show(messageStr, "数据删除");
                return;
            }

            //读取标签id映射
            Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();

            DataDelete deleteForm = new DataDelete();
            deleteForm.TagName2Id = TagName2Id;
            deleteForm.PslCalcuItems = PslCalcuItems;
            deleteForm.Show();
        }
        //数据唯一性检查
        private void uniqeCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行数据唯一性检查！");
                MessageBox.Show(messageStr, "数据唯一性检查");
                return;
            }

            //读取标签id映射
            Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();

            DataUniqeCheck datacheck = new DataUniqeCheck();
            datacheck.TagName2Id = TagName2Id;
            datacheck.PslCalcuItems = PslCalcuItems;
            datacheck.Show();
        }

        //将时间记录导出到csv文件
        private void importTimerecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[][] csvData = PSLTimeRecordDAO.Read();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "另存为";//对话框标题
            sfd.InitialDirectory = "d:\\";//对话框初始目录
            sfd.Filter = "CSV文件|*.csv";//对话框所有可以选择的文件类型
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CsvFileReader.Save(csvData, sfd.FileName);
                    MessageBox.Show("时间数据已经成功写入CSV文件！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出到CSV文件失败，请检查目标文件是否被占用！");
                }
            }

        }
        //关闭退出
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logHelper.Info("计算引擎手动正常退出...");
            try
            {
                this._timer.Dispose();
                worker.Dispose();
            }
            catch { };

            Application.ExitThread();
            Application.Exit();
            this.Close();
        }
        //过滤框文件改变
        #endregion

        #region 窗体与托盘
        //主界面右上角“关闭”图标。注意，点击“关闭图标”，仅能最小化窗口，不能关闭程序。
        //要关闭程序只能通过菜单的“退出”选项
        private void AppRunForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
            this.notifyIcon1.Visible = true;
            this._timer.Stop();
        }
        //托盘图标双击
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
            this.Activate();
        }
        //主界面“退出”按钮_单击
        private void tsbt_Quit_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("Are you sure to quit?", "Message", MessageBoxButtons.OKCancel))
                return;
            logHelper.Fatal("Exiting");
            this.notifyIcon1.Visible = false;
            this.Close();
            this.Dispose();
            Application.Exit();
        }
        //主界面由隐藏或者最小化恢复时
        private void AppRunForm_Resize(object sender, EventArgs e)
        {
            //当窗口由最小化回复为正常时，对Listview进行一次即时更新
            if (this.WindowState == FormWindowState.Normal)
            {
                if (null != this._timer)
                {
                    this._timer.Start();
                }
                InitialListView();
            }
        }
        #endregion

        #region UI线程外更新UI
        //更新最近一次错误信息        
        delegate void dg_calcuError(string errorinfo);
        private void UpdateCalcuError(string errorinfo)
        {
            if (this.InvokeRequired)
            {
                dg_calcuError d = new dg_calcuError(UpdateCalcuError);
                this.Invoke(d, errorinfo);
            }
            else
            {
                this.tslb_CalcuErrors.Text = errorinfo;
            }
        }
        //更新最近一次警告信息
        delegate void dg_calcuWarning(string errorinfo);
        private void UpdateCalcuWarning(string errorinfo)
        {
            if (this.InvokeRequired)
            {
                dg_calcuWarning d = new dg_calcuWarning(UpdateCalcuWarning);
                this.Invoke(d, errorinfo);
            }
            else
            {
                this.tslb_CalcuWarnings.Text = errorinfo;
            }
        }
        //更新当前计算项信息
        delegate void dg_calcuIndex(string count);
        private void UpdateCalcuIndex(string count)
        {
            if (this.InvokeRequired)
            {
                dg_calcuIndex d = new dg_calcuIndex(UpdateCalcuIndex);
                this.Invoke(d, count);
            }
            else
            {
                this.tslb_CurrentIndex.Text = "当前计算速度" + (calcount - beforeCalcount) * 60 + "项/分钟";
                beforeCalcount = long.Parse(count);
            }
        }
        private void updateView(object source, ElapsedEventArgs e)
        {


            UpdateCalcuError(string.Format("计算过程中发生{0}次错误", errorCount));
            UpdateCalcuWarning(string.Format("计算过程中发生{0}次警告", warningCount));
            //InitialListView();
            UpdateListView();
            UpdateCalcuIndex(string.Format("{0}", calcount));
        }

        #endregion

        #region 初始化计算项
        //计算引擎每x00ms扫描一次，每次只计算一个fnextstarttime最近的计算对象
        //——采用线程锁定，如果当次计算未完成，则到定时周期，也不会启动第二次计算。
        //——DBHelper接口的静态方法不支持多线程同时调用，会出现混乱。
        //——计算配置有前后要求，有些算法要求采用前面算法的结果，必须前面算法完成后，才能启动。
        public void initalMap()
        {
            secondList = new List<PSLCalcuItem>();
            minList = new List<PSLCalcuItem>();
            hourList = new List<PSLCalcuItem>();
            dayList = new List<PSLCalcuItem>();
            monthList = new List<PSLCalcuItem>();
            yearList = new List<PSLCalcuItem>();
            for (int i = 0; i < PslCalcuItems.Count; i++)
            {
                switch (PslCalcuItems[i].fintervaltype.ToLower())
                {
                    case "s":
                    case "second":
                    case "seconds":
                        secondList.Add(PslCalcuItems[i]);
                        break;
                    case "min":
                    case "minute":
                    case "minutes":
                        minList.Add(PslCalcuItems[i]);
                        break;

                    case "h":
                    case "hour":
                    case "hours":
                        hourList.Add(PslCalcuItems[i]);
                        break;
                    case "d":
                    case "day":
                    case "days":
                        dayList.Add(PslCalcuItems[i]);
                        break;
                    case "m":
                    case "month":
                    case "months":
                        monthList.Add(PslCalcuItems[i]);
                        break;

                    case "y":
                    case "year":
                    case "years":
                        yearList.Add(PslCalcuItems[i]);
                        break;
                }
            }

            secondList = secondList.OrderBy(m => m.forder).ToList();  //根据计算先后顺序  进行排序
            minList = minList.OrderBy(m => m.forder).ToList();
            hourList = hourList.OrderBy(m => m.forder).ToList();
            dayList = dayList.OrderBy(m => m.forder).ToList();
            monthList = monthList.OrderBy(m => m.forder).ToList();
            yearList = yearList.OrderBy(m => m.forder).ToList();

            //根据计算优先级进行分组
            secondMap = new Dictionary<int, List<PSLCalcuItem>>();
            minMap = new Dictionary<int, List<PSLCalcuItem>>();
            hourMap = new Dictionary<int, List<PSLCalcuItem>>();
            dayMap = new Dictionary<int, List<PSLCalcuItem>>();
            monthMap = new Dictionary<int, List<PSLCalcuItem>>();
            yearMap = new Dictionary<int, List<PSLCalcuItem>>();
            for (int i = 0; i < secondList.Count; i++)
            {
                int key = int.Parse(secondList[i].forder);
                if (secondMap.ContainsKey(key))
                {
                    secondMap[key].Add(secondList[i]);
                }
                else
                {
                    secondMap[key] = new List<PSLCalcuItem>();
                    secondMap[key].Add(secondList[i]);
                }
            }
            for (int i = 0; i < minList.Count; i++)
            {
                int key = int.Parse(minList[i].forder);
                if (minMap.ContainsKey(key))
                {
                    minMap[key].Add(minList[i]);
                }
                else
                {
                    minMap[key] = new List<PSLCalcuItem>();
                    minMap[key].Add(minList[i]);
                }
            }
            for (int i = 0; i < hourList.Count; i++)
            {
                int key = int.Parse(hourList[i].forder);
                if (hourMap.ContainsKey(key))
                {
                    hourMap[key].Add(hourList[i]);
                }
                else
                {
                    hourMap[key] = new List<PSLCalcuItem>();
                    hourMap[key].Add(hourList[i]);
                }
            }
            for (int i = 0; i < dayList.Count; i++)
            {
                int key = int.Parse(dayList[i].forder);
                if (dayMap.ContainsKey(key))
                {
                    dayMap[key].Add(dayList[i]);
                }
                else
                {
                    dayMap[key] = new List<PSLCalcuItem>();
                    dayMap[key].Add(dayList[i]);
                }
            }
            for (int i = 0; i < monthList.Count; i++)
            {
                int key = int.Parse(monthList[i].forder);
                if (monthMap.ContainsKey(key))
                {
                    monthMap[key].Add(monthList[i]);
                }
                else
                {
                    monthMap[key] = new List<PSLCalcuItem>();
                    monthMap[key].Add(monthList[i]);
                }
            }
            for (int i = 0; i < yearList.Count; i++)
            {
                int key = int.Parse(yearList[i].forder);
                if (yearMap.ContainsKey(key))
                {
                    yearMap[key].Add(yearList[i]);
                }
                else
                {
                    yearMap[key] = new List<PSLCalcuItem>();
                    yearMap[key].Add(yearList[i]);
                }
            }
        }

        #endregion

        #region 辅助函数
        //初始化概化计算对象集、概化计算标签字典、listview控件
        private bool InitialCalcu()
        {
            try
            {
                //读取概化计算标签字典
                logHelper.Info("读取概化计算标签字典...");
                Dictionary<string, System.UInt32> TagName2Id = PSLTagNameIdMapDAO.ReadMap();   //标签名称跟id映射关系
                Dictionary<string, bool> TagName2Flag = PSLTagNameIdMapDAO.ReadFlagMap();

                //加载新版算法名
                global.caculateFunction = PSLTagNameIdMapDAO.ReadCaculateFunction();
                if (PSLTagNameIdMapDAO.ErrorFlag)
                {
                    string strMsg = String.Format("读取概化计算标签字典！" + Environment.NewLine +
                                               "——请手动检查数据库标签id映射表。" + Environment.NewLine +
                                               "——查看log文件。");
                    MessageBox.Show(strMsg);
                    return false;
                }
                if (TagName2Id.Count == 0)
                {
                    //读取的TagName2Id为空，不需要出错处理。因为TagName2Id有可能为空。配置数据还未导入。                   
                    //return false;
                }

                //读取概化计算配置对象集
                logHelper.Info("读取概化计算配置对象集...");
                //读取配置
                PslCalcuItems = PSLCalcuConfigDAO.ReadConfig();
                if (PSLCalcuConfigDAO.ErrorFlag)
                {
                    string strMsg = String.Format("读取概化计算配置对象集错误！" + Environment.NewLine +
                                               "——请手动检查数据库计算配置表" + Environment.NewLine +
                                               "——查看log文件。");
                    MessageBox.Show(strMsg);
                    return false;
                }
                //概化计算配置对象集对计算引擎进行准备、：转换算法标志为布尔、获取计算结果标签id、获取计算条件标签id
                logHelper.Info("准备计算引擎配置...");
                int sum = 0;
                int maxsum = 100;      //20181112，要给下面的初始化设置最大错误数量，否则这里因为PslCalcuItems数量有时会非常巨大，而产生错误
                foreach (PSLCalcuItem item in PslCalcuItems)
                {
                    //logHelper.Info("准备计算引擎配置项：" + item.fmodulename);     //调试测试
                    item.sourcetagname = item.sourcetagname.ToUpper();                  //转大写，防止直接修改数据库时，输入了小写的标签名
                    item.fcondpslnames = item.fcondpslnames.ToUpper();                  //转大写，防止直接修改数据库时，输入了小写的标签名
                    item.foutputpsltagnames = item.foutputpsltagnames.ToUpper();        //转大写，防止直接修改数据库时，输入了小写的标签名
                    if (!item.Prepare(ref TagName2Id, ref TagName2Flag)) sum = sum + 1;
                    if (sum > maxsum) break;        //当发生错误的配置项超过1000时，直接退出，否则程序会死掉
                };
                if (sum != 0)
                {
                    string strMsg = String.Format("计算配置项初始化错误！" + Environment.NewLine +
                                                "——在标签id映射表中找不到计算条件标签或计算结果标签！" + Environment.NewLine +
                                                "——通常该错误是手动修改计算配置项计算标签后，未更新映射表造成的" + Environment.NewLine +
                                                "——请手动检查标签id映射表" + Environment.NewLine +
                                                "点击‘确定’退出程序。");
                    MessageBox.Show(strMsg);
                    return false;
                }

                //概化计算配置对象集的计算时间初始化:起始时间、结束时间、下一次计算时间。如果起始时间为空，则用系统当前时间初始化starttime
                //注意，计算引擎的第一次计算，是第一个“下一次计算时间”。比如起始时间是12点，周期1小时，延迟5minutes。则第一次计算时间为13:05:00。
                logHelper.Info("准备计算时间...");
                string datetimeStr = string.Format("{0} {1}", this.dtStartDate.Text, this.dtStartTime.Text);
                foreach (PSLCalcuItem item in PslCalcuItems)
                {
                    //如果计算配置表中计算项的fstarttime为空，则用界面的starttime来赋值，如果不为空，则采用数据库中的值。
                    if (item.fstarttime == DateTime.MinValue) item.fstarttime = DateTime.Parse(datetimeStr);//根据界面时间框给starttime赋初值
                    item.IniDate();     //根据starttime初值给endtime和nextstarttime赋初值
                };

                //初始化算法名称集合
                logHelper.Info("初始化算法名称集合...");
                foreach (PSLCalcuItem item in PslCalcuItems) { this.ModuleNames.Add(item.fmodulename); }
                this.ModuleNames = this.ModuleNames.Distinct().ToList();
                this.cb_moduletype.Items.Clear();
                this.cb_moduletype.Items.AddRange(this.ModuleNames.ToArray());

                //主界面listview数据初始化 
                logHelper.Info("初始化界面listview...");
                InitialListView();
                //主界面信息栏信息初始化
                logHelper.Info("初始化界面信息栏...");
                tslb_CalcuConfig.Text = string.Format(" {0}号计算节点，共配置{1}项计算", APPConfig.realcalcu_calcunode, PslCalcuItems.Count);
                tslb_CalcuErrors.Text = string.Format("计算过程中发生{0}次错误!", errorCount.ToString());
                tslb_CalcuWarnings.Text = string.Format("计算过程中发生{0}次警告!", warningCount.ToString());

                return true;
            }
            catch (Exception ex)
            {
                //这里出错，主要是：
                //——PSLTagNameIdMapDAO.ReadMap()出错
                //—— PSLCalcuConfigDAO.ReadConfig()出错
                string messageStr = String.Format("界面初始化失败，请检查log文件！" + Environment.NewLine +
                                                  "——请检出数据库是否初始化，计算配置表和标签id映射表是否正常。" + Environment.NewLine +
                                                  "详细出错信息查看log文件。"
                                                    );
                MessageBox.Show(messageStr);
                logHelper.Fatal("界面初始化失败。详细信息：" + ex.ToString());
                return false;
            }

        }
        //更新概化计算对象集、listview控件
        //——由于仅更新了和周期相关的starttime、endtime、nextstarttim
        //——因此不会引起fid的变化,无需重新读取概化计算配置对象集PslCalcuItems            
        private void updateCalcu()
        {
            //概化计算配置对象集的计算时间更新
            string datetimeStr = string.Format("{0} {1}", this.dtStartDate.Text, this.dtStartTime.Text);
            DateTime setDateTime = DateTime.Parse(datetimeStr);
            for (int i = 0; i < lV_DataList.Items.Count; i++)
            {
                //UI提示
                //tslb_msg.Text = string.Format("{0} of {1} is is reviesed", i,PslCalcuItems.Count);
                //对listview的配置行进行遍历
                if (lV_DataList.Items[i].Checked)   //仅更新选中对象
                {
                    //20181212修改，不在采用fid来寻找。PslCalcuItems和lV_DataList.Items一一对应。
                    PSLCalcuItem checkeditem = PslCalcuItems[i];
                    int preMonth = checkeditem.fstarttime.Month;
                    int preDay = checkeditem.fstarttime.Day;
                    int preHour = checkeditem.fstarttime.Hour;
                    int preMinute = checkeditem.fstarttime.Minute;
                    int preSecond = checkeditem.fstarttime.Second;
                    //修改当前概化计算配置对象的starttime
                    if (checkeditem.fintervaltype == "y" || checkeditem.fintervaltype == "year" || checkeditem.fintervaltype == "years")
                    {//如果计算项的类型为年，则仅需要将当前计算时刻的年份，按照设定时间进行更改。月份、日、小时、分、秒还按照原有时间设定。
                        checkeditem.fstarttime = new DateTime(setDateTime.Year, preMonth, preDay, preHour, preMinute, preSecond);
                    }
                    else if (checkeditem.fintervaltype == "m" || checkeditem.fintervaltype == "month" || checkeditem.fintervaltype == "months")
                    {//如果计算项的类型为月，则仅需要将当前计算时刻的年份和月，按照设定时间进行更改。日、小时、分、秒还按照原有时间设定。
                        checkeditem.fstarttime = new DateTime(setDateTime.Year, setDateTime.Month, preDay, preHour, preMinute, preSecond);
                    }
                    else if (checkeditem.fintervaltype == "d" || checkeditem.fintervaltype == "day" || checkeditem.fintervaltype == "days")
                    {//如果计算项的类型为日，则仅需要将当前计算时刻的年份、月、日，按照设定时间进行更改。小时、分、秒还按照原有时间设定。
                        checkeditem.fstarttime = new DateTime(setDateTime.Year, setDateTime.Month, setDateTime.Day, preHour, preMinute, preSecond);
                    }
                    else if (checkeditem.fintervaltype == "h" || checkeditem.fintervaltype == "hour" || checkeditem.fintervaltype == "hours")
                    {//如果计算项的类型为小时，则需要将当前计算时刻的年份、月、日、小时，按照设定时间进行更改。分、秒还按照原有时间设定。
                        checkeditem.fstarttime = new DateTime(setDateTime.Year, setDateTime.Month, setDateTime.Day, setDateTime.Hour, preMinute, preSecond);
                    }
                    else if (checkeditem.fintervaltype == "min" || checkeditem.fintervaltype == "minute" || checkeditem.fintervaltype == "minuts")
                    {//如果计算项的类型为分，则需要将当前计算时刻的年份、月、日、小时、分，按照设定时间进行更改。秒还按照原有时间设定。
                        checkeditem.fstarttime = new DateTime(setDateTime.Year, setDateTime.Month, setDateTime.Day, setDateTime.Hour, setDateTime.Minute, preSecond);
                    }
                    else if (checkeditem.fintervaltype == "s" || checkeditem.fintervaltype == "second" || checkeditem.fintervaltype == "seconds")
                    {
                        checkeditem.fstarttime = setDateTime;
                    }

                    //根据starttime修改endtime和nextstarttime
                    checkeditem.IniDate();
                    PSLCalcuConfigDAO.UpdateStartTime(checkeditem.fid, checkeditem.fstarttime);
                    //将修改过的行置为非选中状态
                    lV_DataList.Items[i].Checked = false;
                }
            }
            //主界面listview对应行的更新 
            UpdateListViewRightNow();
            //主界面信息栏信息初始化
            tslb_CalcuConfig.Text = string.Format("--共{0}项计算配置", PslCalcuItems.Count);
        }
        private void updatepslcalcuconfig(object sender, DoWorkEventArgs e)
        {
            //将改变的信息，更新到pslcalcuconfig表starttime  
            worker.ReportProgress(0, String.Format("更新计算信息配置表'计算起始时间'！"));
            PSLCalcuConfigDAO.worker = worker;
            PSLCalcuConfigDAO.UpdateStartTimeBatch(PslCalcuItems); //更新pslconfig表中的starttime字段，为了记录当前计算到的时间。如果计算出错，后面会从这个位置开始计算                       
        }
        void UpdateComplete(object sender, RunWorkerCompletedEventArgs e)   //进度条窗体进度控制：后台线程完成时（操作完成消息），关闭进度条窗体
        {
            importProgress.Close();
            worker.DoWork -= new DoWorkEventHandler(updatepslcalcuconfig);                              //执行完毕，解除注册事件
            worker.ProgressChanged -= new ProgressChangedEventHandler(worker_ProgressChanged);          //执行完毕，解除注册事件
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(UpdateComplete);            //执行完毕，解除注册事件
            MessageBox.Show("更新完毕!");
        }
        //主界面listview数据初始化
        private void InitialListView()
        {
            //清空listview的已有数据项
            this.lV_DataList.Items.Clear();
            //根据pslconfigitems添加
            for (int i = 0; i < PslCalcuItems.Count; i++)              //为listview添加 items.Count行数据
            {
                PSLCalcuItem pslcalcuitem = PslCalcuItems[i];
                ListViewItem lvitem = this.lV_DataList.Items.Add((pslcalcuitem.fid).ToString(), (pslcalcuitem.fid).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                lvitem.Checked = checkBox_SelectAll.Checked;
                lvitem.SubItems.Add(pslcalcuitem.sourcetagname.Replace("^", @"\"));                 //添加listview的第1个字段Index。 Replace("^",@"\")是对PGIM的特殊处理    
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdb);                                      //添加listview的第3个字段Description
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdesc);                                    //添加listview的第4个字段dim
                lvitem.SubItems.Add(pslcalcuitem.sourcetagdim);                                     //
                lvitem.SubItems.Add(pslcalcuitem.sourcetagmrb.ToString());                          //
                lvitem.SubItems.Add(pslcalcuitem.sourcetagmre.ToString());                          //
                lvitem.SubItems.Add(pslcalcuitem.fmodulename);
                lvitem.SubItems.Add(pslcalcuitem.fgroup);
                lvitem.SubItems.Add(pslcalcuitem.forder.ToString());
                lvitem.SubItems.Add(pslcalcuitem.fclass);
                lvitem.SubItems.Add(pslcalcuitem.falgorithmsflag);
                lvitem.SubItems.Add(pslcalcuitem.fparas);
                lvitem.SubItems.Add(pslcalcuitem.fcondpslnames);
                lvitem.SubItems.Add(pslcalcuitem.foutputtable);
                lvitem.SubItems.Add(pslcalcuitem.foutputnumber.ToString());
                lvitem.SubItems.Add(pslcalcuitem.foutputpsltagnames.Replace("^", @"\"));            //Replace("^",@"\")是对PGIM的特殊处理
                lvitem.SubItems.Add(pslcalcuitem.resolution);
                lvitem.SubItems.Add(pslcalcuitem.fdelay.ToString());
                lvitem.SubItems.Add(pslcalcuitem.fstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
                lvitem.SubItems.Add(pslcalcuitem.fendtime.ToString("yyyy-MM-dd HH:mm:ss"));
                lvitem.SubItems.Add(pslcalcuitem.fnextstarttime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        //主界面listview数据刷新：本程序由主线程通过代理调用，更新listview某一行信息
        delegate void ListUpdate();                         //线程刷新UI代理
        private void UpdateListView()
        {

            if (this.lV_DataList.InvokeRequired)
            {
                ListUpdate d = new ListUpdate(UpdateListView);
                this.lV_DataList.Invoke(d);
            }
            else
            {
                DateTime cur = DateTime.Now; //获取系统时间
                DateTime start = Convert.ToDateTime("2015/01/01 00:00:00");
                long ticks = cur.Ticks - start.Ticks;
                if ((ticks / 10000) % 60000 > 50000)
                {//如果系统时间合适  则执行相应程序
                 //根据pslconfigitems添加
                    for (int i = 0; i < PslCalcuItems.Count; i++)              //为listview添加 items.Count行数据
                    {
                        PSLCalcuItem pslcalcuitem = PslCalcuItems[i];
                        this.lV_DataList.Items[pslcalcuitem.index].Selected = true;
                        this.lV_DataList.Items[pslcalcuitem.index].SubItems[19].Text = pslcalcuitem.fstarttime.ToString("yyyy-MM-dd HH:mm:ss");
                        this.lV_DataList.Items[pslcalcuitem.index].SubItems[20].Text = pslcalcuitem.fendtime.ToString("yyyy-MM-dd HH:mm:ss");
                        this.lV_DataList.Items[pslcalcuitem.index].SubItems[21].Text = pslcalcuitem.fnextstarttime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
        }

        private void UpdateListViewRightNow()
        {
            for (int i = 0; i < PslCalcuItems.Count; i++)              //为listview添加 items.Count行数据
            {
                PSLCalcuItem pslcalcuitem = PslCalcuItems[i];
                this.lV_DataList.Items[pslcalcuitem.index].Selected = true;
                this.lV_DataList.Items[pslcalcuitem.index].SubItems[19].Text = pslcalcuitem.fstarttime.ToString("yyyy-MM-dd HH:mm:ss");
                this.lV_DataList.Items[pslcalcuitem.index].SubItems[20].Text = pslcalcuitem.fendtime.ToString("yyyy-MM-dd HH:mm:ss");
                this.lV_DataList.Items[pslcalcuitem.index].SubItems[21].Text = pslcalcuitem.fnextstarttime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }


        //log自动管理程序
        private void logClear()
        {
            int circleDays = -APPConfig.log_validperiod;
            try
            {
                string logpath = System.Environment.CurrentDirectory + "\\log";
                DirectoryInfo logfolder = new DirectoryInfo(logpath);                             //该标签对应的文件夹对象
                FileInfo[] logfiles = logfolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);   //所有log文件
                for (int i = 0; i < logfiles.Length; i++)
                {
                    string[] logfilenames = logfiles[i].ToString().Split('.');                    //依次遍历每个log文件名，  
                    if (logfilenames.Length >= 3)
                    {
                        DateTime logfiledate = DateTime.Parse(logfilenames[2].Replace("_", "-"));
                        if (logfiledate < DateTime.Now.AddDays(circleDays))
                        {
                            File.Delete(logpath + "\\" + logfiles[i]);
                        }
                    }

                }
            }
            catch
            { }
        }
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
                this.notifyIcon1.Visible = true;
                string info = string.Format("内容【{0}】已经复制到剪贴板", strText);
                this.notifyIcon1.ShowBalloonTip(1500, "提示", info, ToolTipIcon.Info);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //在csv配置文件后面，追加tagid列。
        private string[][] csvDataPlusTagId(ConfigCSV configcsv, Dictionary<string, uint> tagname2id)
        {
            string[][] csvDataPlusTagId = new string[configcsv.importdata.Length][];
            for (int i = 0; i < configcsv.firstDataRow; i++)                            //拷贝标题行
            {
                csvDataPlusTagId[i] = new string[configcsv.importdata[i].Length + 1];
                Array.Copy(configcsv.importdata[i], 0, csvDataPlusTagId[i], 0, configcsv.importdata[i].Length);
                csvDataPlusTagId[i][configcsv.importdata[i].Length] = "";
            }
            for (int i = configcsv.firstDataRow; i < csvDataPlusTagId.Length; i++)      //拷贝数据行
            {
                csvDataPlusTagId[i] = new string[configcsv.importdata[i].Length + 1];
                string[] tagnames = configcsv.importdata[i][configcsv.foutputpsltagprefIndex].Split(';');
                uint[] tagids = new uint[tagnames.Length];
                for (int j = 0; j < tagnames.Length; j++)
                {
                    tagids[j] = tagname2id[tagnames[j]];
                }
                Array.Copy(configcsv.importdata[i], 0, csvDataPlusTagId[i], 0, configcsv.importdata[i].Length);
                csvDataPlusTagId[i][configcsv.importdata[i].Length] = String.Join(";", tagids);
            }

            return csvDataPlusTagId;
        }
        //延时秒
        public static bool Delay(int delayTime)
        {
            DateTime now = DateTime.Now;
            int s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.Seconds;
                Application.DoEvents();
            }
            while (s < delayTime);
            return true;
        }
        #endregion

        #region 测试项菜单进入密码
        private void DebugToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void DebugToolStripMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (APPConfig.app_mode == "debug" && APPConfig.app_debug_password == "wenjiug")
            {
                //正常显示菜单
            }
            else
            {
                //DebugLogon debuglogon = new DebugLogon();
                //debuglogon.ShowDialog();
                this.DebugToolStripMenuItem.HideDropDown();
            }
        }
        #endregion

        #region 调试模式相关
        //综合算法测试窗口
        private void 综合算法测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (calcurunningflag)
            {
                string messageStr = String.Format("计算引擎还在运行！请先停止计算引擎，再进行参数设定！");
                MessageBox.Show(messageStr, "计算引擎参数设置");
                return;
            }
            PSLCalcuModuleTest calcutest = new PSLCalcuModuleTest();
            calcutest.Show();

        }
        //根据时间插值窗口
        private void ToolsCalcuValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateValue datevalue = new DateValue();
            datevalue.Show();
        }
        //统计相同元素测试
        private void 统计相同元素个数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<int> temp = new List<int>() { 1, 1, 3, 3, 3, 3 };

            int number = temp.Count(n => n == 3);

            List<string> str = new List<string>();

        }
        //时间转换工具窗口
        private void 时间转换工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTransTool datetrans = new DateTransTool();
            datetrans.Show();
        }

        //计算模块测试菜单：这里选择菜单点击后执行哪一个测试程序
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //测试MMultiAnalogAvg
        private void mMultiAnalogAvgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[4];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = TestData_MMultiAnalogAvg.SimuData2()[i];
            }
            //input[4] = null;
            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:12:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            //List<PValue>[] testdata = TestData_MMultiAnalogAvg.SimuData1();

            //input[0] = testdata[2];



            //1.3、从csv数据文件读入数据

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
            MMultiAnalogAvg.inputData = input;
            MMultiAnalogAvg.calcuInfo = new CalcuInfo();
            MMultiAnalogAvg.calcuInfo.fmodulename = "MMultiAnalogAvg";
            MMultiAnalogAvg.calcuInfo.sourcetagname = "";
            MMultiAnalogAvg.calcuInfo.fparas = "";
            MMultiAnalogAvg.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MMultiAnalogAvg.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MCondSpan2.spanSeries = spanSeries;
                //MCondSpan2.Filter();
            }

            //4、进行计算
            Results Results = MMultiAnalogAvg.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }

        }
        //测试mFOPC2Minute
        private void mFOPC2MinuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[1];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:00:00";
            string endDate = "2016-01-01 13:00:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_MFOPC2Minute.SimuData();

            input[0] = testdata[2];



            //1.3、从csv数据文件读入数据

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
            MOPCSegmentFilter.inputData = input;
            MOPCSegmentFilter.calcuInfo = new CalcuInfo();
            MOPCSegmentFilter.calcuInfo.fmodulename = "MFOPC2Minute";
            MOPCSegmentFilter.calcuInfo.sourcetagname = "";
            MOPCSegmentFilter.calcuInfo.fparas = "";
            MOPCSegmentFilter.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MOPCSegmentFilter.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MCondSpan2.spanSeries = spanSeries;
                //MCondSpan2.Filter();
            }

            //4、进行计算
            Results results = MOPCSegmentFilter.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试Con的spanLong
        private void mCondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[3];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_MCondSpan.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[3];
            }


            //1.3、从csv数据文件读入数据

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
            MCondSpanLong.inputData = input;
            MCondSpanLong.calcuInfo = new CalcuInfo();
            MCondSpanLong.calcuInfo.fmodulename = "MCondSpanLong";
            MCondSpanLong.calcuInfo.sourcetagname = "";
            MCondSpanLong.calcuInfo.fparas = "";
            MCondSpanLong.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MCondSpanLong.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MCondSpan2.spanSeries = spanSeries;
                //MCondSpan2.Filter();
            }

            //4、进行计算
            Results results = MCondSpanLong.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试限定条件时间筛选MLCondSpan2
        private void mCondSpan2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue> input = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 14:00:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input = TestData_MCondSpan2.simuData1();

            //1.3、从csv数据文件读入数据

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
            MLCondSpan2.inputData = new List<PValue>[1] { input };
            MLCondSpan2.calcuInfo = new CalcuInfo();
            MLCondSpan2.calcuInfo.fmodulename = "MCondSpan2";
            MLCondSpan2.calcuInfo.sourcetagname = "testData";
            MLCondSpan2.calcuInfo.fparas = "[40,60,5]";
            MLCondSpan2.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MLCondSpan2.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MCondSpan2.spanSeries = spanSeries;
                //MCondSpan2.Filter();
            }

            //4、进行计算
            Results results = MLCondSpan2.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试限定条件时间筛选MFCondSpan2
        private void mFCondSpanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue> input = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 14:00:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input = TestData_MCondSpan2.simuData1();

            //1.3、从csv数据文件读入数据

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
            MFCondSpan2.inputData = new List<PValue>[1] { input };
            MFCondSpan2.calcuInfo = new CalcuInfo();
            MFCondSpan2.calcuInfo.fmodulename = "MCondSpan2";
            MFCondSpan2.calcuInfo.sourcetagname = "testData";
            MFCondSpan2.calcuInfo.fparas = "[40;60）;62;60";
            MFCondSpan2.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFCondSpan2.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MCondSpan2.spanSeries = spanSeries;
                //MCondSpan2.Filter();
            }

            //4、进行计算
            Results Results = MFCondSpan2.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }

        //测试超限统计MLimitStatistics
        //定义一个委托，用于委托信号发生函数
        delegate double generator(double period, int min, int max, double current);
        private void mToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue> input = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:10:09";
            int interval = 1;
            generator generator = SimulateGenerator.randomSimu;
            input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input = TestData_MLimitStat.simuData1();

            //1.3、从csv数据文件读入数据


            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MLimitStatistics.inputData = input;       //输入数据 
            //MLimitStatistics.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）
            MLimitStatistics.inputData = new List<PValue>[1] { input };
            MLimitStatistics.calcuInfo = new CalcuInfo();
            MLimitStatistics.calcuInfo.fparas = "20,30,40,60,70,80,2,2,2,2,2,2,0.09";
            MLimitStatistics.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MLimitStatistics.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLimitStatistics.spanSeries = spanSeries;
                //MLimitStatistics.Filter();
            }

            //4、进行计算
            Results Results = MLimitStatistics.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试分布统计算法MFDistribute22
        private void mFDistribute22ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue> input = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input = TestData_MFDistribute22.simuData1();

            //1.3、从csv数据文件读入数据
            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）
            MFDistribute22.inputData = new List<PValue>[1] { input };
            MFDistribute22.calcuInfo = new CalcuInfo();
            MFDistribute22.calcuInfo.fparas = "0,100";
            MFDistribute22.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFDistribute22.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MFDistribute22.spanSeries = spanSeries;
                //MFDistribute22.Filter();
            }

            //4、进行计算
            Results results = MFDistribute22.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试分布统计算法MFDistribute12
        private void mFDistribute12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue> input = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input = TestData_MFDistribute12.simuData1();

            //1.3、从csv数据文件读入数据
            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件            
            MFDistribute12.inputData = new List<PValue>[1] { input };
            MFDistribute12.calcuInfo = new CalcuInfo();
            MFDistribute12.calcuInfo.fparas = "0,100";
            MFDistribute12.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFDistribute12.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MFDistribute12.spanSeries = spanSeries;
                //MFDistribute12.Filter();
            }

            //4、进行计算
            Results results = MFDistribute12.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试分布统计算法MFDistribute22
        private void mLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue> input = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input = TestData_MLDistribute22.simuData1();

            //1.3、从csv数据文件读入数据
            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）
            MLDistribute22.inputData = new List<PValue>[1] { input };
            MLDistribute22.calcuInfo = new CalcuInfo();
            MLDistribute22.calcuInfo.fparas = "50,55";
            MLDistribute22.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MLDistribute22.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results Results = MLDistribute22.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试MDigitalSum
        private void mDigitalSumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue>[] input = new List<PValue>[3];
            input[0] = new List<PValue>();
            input[1] = new List<PValue>();
            input[2] = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input[0] = TestData_MDigitalSum.simuData1();
            input[1] = TestData_MDigitalSum.simuData2();
            input[2] = TestData_MDigitalSum.simuData3();

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MMultiDigitalSum.inputData = input;
            MMultiDigitalSum.calcuInfo = new CalcuInfo();
            MMultiDigitalSum.calcuInfo.fparas = "50,55";
            MMultiDigitalSum.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MMultiDigitalSum.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MMultiDigitalSum.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }

        }
        //测试MDigitalSelect
        private void mToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue>[] input = new List<PValue>[3];
            input[0] = new List<PValue>();
            input[1] = new List<PValue>();
            input[2] = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input[0] = TestData_MDigitalSum.simuData1();
            input[1] = TestData_MDigitalSum.simuData2();
            input[2] = TestData_MDigitalSum.simuData3();

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MMultiDigitalSelect.inputData = input;
            MMultiDigitalSelect.calcuInfo = new CalcuInfo();
            MMultiDigitalSelect.calcuInfo.fparas = "-10,5,20,80";
            MMultiDigitalSelect.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MMultiDigitalSelect.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MMultiDigitalSelect.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MdigitalSetStats
        private void mDigitalSetStatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue>[] input = new List<PValue>[1];
            input[0] = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input[0] = TestData_MDigitalSum.simuData1();

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MDigitalSetStats.inputData = input;
            MDigitalSetStats.calcuInfo = new CalcuInfo();
            MDigitalSetStats.calcuInfo.fparas = "1,600";
            MDigitalSetStats.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MDigitalSetStats.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results Results = MDigitalSetStats.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }

        }
        //MFindMaxInfo
        private void mToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue>[] input = new List<PValue>[3];
            input[0] = new List<PValue>();
            input[1] = new List<PValue>();
            input[2] = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input[0] = TestData_MLimitStat.simuData1();
            input[1] = TestData_MLimitStat.simuData2();
            input[2] = TestData_MLimitStat.simuData3();

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MFindMaxInfo.inputData = input;
            MFindMaxInfo.calcuInfo = new CalcuInfo();
            MFindMaxInfo.calcuInfo.fparas = "";
            MFindMaxInfo.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFindMaxInfo.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MFindMaxInfo.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        private void mFindMinInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式
            List<PValue>[] input = new List<PValue>[3];
            input[0] = new List<PValue>();
            input[1] = new List<PValue>();
            input[2] = new List<PValue>();

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input[0] = TestData_MLimitStat.simuData1();
            input[1] = TestData_MLimitStat.simuData2();
            input[2] = TestData_MLimitStat.simuData3();

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MFindMinInfo.inputData = input;
            MFindMinInfo.calcuInfo = new CalcuInfo();
            MFindMinInfo.calcuInfo.fparas = "";
            MFindMinInfo.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFindMinInfo.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MFindMinInfo.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }

        }
        //测试Index9
        private void mIndex9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[10];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_Index10.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MIndex9.inputData = input;
            MIndex9.calcuInfo = new CalcuInfo();
            MIndex9.calcuInfo.fparas = "";
            MIndex9.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MIndex9.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MIndex9.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试Index20
        private void mIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[20];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_Index10.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MIndex20.inputData = input;
            MIndex20.calcuInfo = new CalcuInfo();
            MIndex20.calcuInfo.fparas = "";
            MIndex20.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MIndex20.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MIndex20.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MFindMAx
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[20];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_Index10.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MFindMaxInfo.inputData = input;
            MFindMaxInfo.calcuInfo = new CalcuInfo();
            MFindMaxInfo.calcuInfo.fparas = "";
            MFindMaxInfo.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFindMaxInfo.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MFindMaxInfo.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MfindMin
        private void mFindMinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[20];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_Index10.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MFindMinInfo.inputData = input;
            MFindMinInfo.calcuInfo = new CalcuInfo();
            MFindMinInfo.calcuInfo.fparas = "";
            MFindMinInfo.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MFindMinInfo.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MFindMinInfo.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MNormalize
        private void mNormalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[1];
            input[0] = new List<PValue>();


            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            input[0] = TestData_MLimitStat.simuData1();

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MNormalize.inputData = input;
            MNormalize.calcuInfo = new CalcuInfo();
            MNormalize.calcuInfo.fparas = "";
            MNormalize.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MNormalize.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MNormalize.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MCondspn测试
        private void mToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[3];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_MCondSpan.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MCondSpanSum.inputData = input;
            MCondSpanSum.calcuInfo = new CalcuInfo();
            MCondSpanSum.calcuInfo.fparas = "";
            MCondSpanSum.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MCondSpanSum.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MCondSpanSum.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MMulitanalogSub测试
        private void mMultiAnalogSubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[2];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_MMultiAnalogSub.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            M2AnalogSub.inputData = input;
            M2AnalogSub.calcuInfo = new CalcuInfo();
            M2AnalogSub.calcuInfo.fparas = "";
            M2AnalogSub.calcuInfo.fstarttime = DateTime.Parse(startDate);
            M2AnalogSub.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = M2AnalogSub.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //MMulitanalogSum测试
        private void mMulitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[2];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_MMultiAnalogSub.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MMultiAnalogSum.inputData = input;
            MMultiAnalogSum.calcuInfo = new CalcuInfo();
            MMultiAnalogSum.calcuInfo.fparas = "";
            MMultiAnalogSum.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MMultiAnalogSum.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MMultiAnalogSum.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //测试MAnalogReadCurrent
        private void mNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1、给计算模块准备输入数据。准备方法可以从下面的三种中选取一种方式

            //Mindex9测试，可以通过改变下面input数组的下标值，来输入不同数量的数值对排序进行测试。入股输入数值超过9个，计算会报错。
            List<PValue>[] input = new List<PValue>[2];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new List<PValue>();
            }

            //1.1、用虚拟数据发生器批量产生数据
            string startDate = "2016-01-01 12:10:00";
            string endDate = "2016-01-01 12:11:00";
            //int interval = 1;
            //generator generator = SimulateGenerator.randomSimu;
            //input = simulateData(startDate, endDate, interval, generator);

            //1.2、用手动赋值法给定特殊数据            
            List<PValue>[] testdata = TestData_MAnalogReadCurrent.SimuData();
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = testdata[i];
            }

            //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件
            //MFDistribute22.inputData = input;       //输入数据 
            //MFDistribute22.calcuInfo = null;        //档次次计算信息（数据起始时间、数据截止时间、输出结果的数量、当次计算执行哪些算法“YYN”、当次计算需要参数）

            MAnalogReadCurrent.inputData = input;
            MAnalogReadCurrent.calcuInfo = new CalcuInfo();
            MAnalogReadCurrent.calcuInfo.fparas = "";
            MAnalogReadCurrent.calcuInfo.fstarttime = DateTime.Parse(startDate);
            MAnalogReadCurrent.calcuInfo.fendtime = DateTime.Parse(endDate);

            //3、如果有外部条件，先根据外部条件过滤
            if (false)
            {
                //List<PValue> spanSeries = new List<PValue>();
                //MLDistribute22.spanSeries = spanSeries;
                //MLDistribute22.Filter();
            }

            //4、进行计算
            Results results = MAnalogReadCurrent.Calcu();

            //5、检查计算结果
            for (int i = 0; i < 5; i++)
            {
                //将每个list<PValue>按标签名写入数据库，
                //PValue是否一条一条的写，每一条一个sql，还是整体写入？
                //写入的时候，是pslname和timestamp相同，就更新，不同就写入新记录
                //完成上面要求，应该怎么组织sql
            }
        }
        //生产模拟数据
        private static List<PValue> simulateData(string startDateStr, string endDateStr, int interval, generator generator)
        {
            DateTime startDate = DateTime.Parse(startDateStr);         //获得当前行的起始时间
            DateTime endDate = DateTime.Parse(endDateStr);           //获得当前行的截止时间

            List<PValue> results = new List<PValue>();
            long totalSeconds = (long)endDate.Subtract(startDate).TotalSeconds;
            long currentSeconds = 0;

            DateTime pvalueStartTime = startDate;
            DateTime pvalueEndTime = startDate.AddSeconds(interval);

            do
            {

                double value = generator(60, 0, 100, currentSeconds);

                results.Add(new PValue(value, pvalueStartTime, pvalueEndTime, 0));

                currentSeconds = currentSeconds + (long)interval;
                pvalueStartTime = pvalueStartTime.AddSeconds(interval);
                pvalueEndTime = pvalueEndTime.AddSeconds(interval);

            } while (currentSeconds <= totalSeconds);

            return results;
        }

        #endregion

        #region 测试_表达式解析测试函数
        private void testToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            testExpression();
        }
        private void testExpression()
        {
            //创建时间解析对象
            ICondEvaluatable exp;
            string CalcuStr = "({1}!({2}&{3}))";
            exp = new CondExpression(CalcuStr);
            //给入数据
            List<PValue>[] CalcuValues = new List<PValue>[3];
            CalcuValues[0] = TestData_MLimitStat.spanLogicData0();
            CalcuValues[1] = TestData_MLimitStat.spanLogicData1();
            CalcuValues[2] = TestData_MLimitStat.spanLogicData2();
            //解析并计算                  
            List<PValue> result = exp.Evaluate(CalcuValues);                      //根据解析结果进行计算

        }
        #endregion

        #region 测试_时间过滤函数测试
        private void testToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.testSpansFilter();
        }
        private void testSpansFilter()
        {
            //给入数据
            List<PValue> input = new List<PValue>();
            List<PValue>[] inputs = new List<PValue>[1];
            input = TestData_MLimitStat.spanFilterData0();
            inputs = new List<PValue>[1] { input };
            List<PValue> filterspan = new List<PValue>();
            filterspan = TestData_MLimitStat.spanFilterData1();
            SpanLogic.SpansFilter(ref inputs, filterspan);
        }
        //并行时间分割，实时数据分割
        private void spanPValues4rtdbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryParallelCalcuEngine his = new HistoryParallelCalcuEngine();
            List<PValue> input;
            DateTime startdate = DateTime.Parse("2016-01-01 12:00:00");
            DateTime endDate = DateTime.Parse("2016-01-01 13:00:00");
            List<PValue>[] results;

            //测试分钟的分割
            //——原始值和要划分的时间段，恰好完全重合。
            //——分割结果与原始值相同
            input = TestData_SpanPValue4RTDB.simuData1();
            results = null;
            results = his.SpanPValues4rtdbStep(input, startdate, endDate, 60);    //SpanPValues4rtdbStep是将实时数据视作离散阶梯型曲线来分割

            //——只有起始时刻值和截止时刻值。
            //——分割结果需要插值
            input = TestData_SpanPValue4RTDB.simuData2();
            results = null;
            results = his.SpanPValues4rtdbStep(input, startdate, endDate, 60);    //SpanPValues4rtdbStep是将实时数据视作离散阶梯型曲线来分割

            //——有起始时刻值和截止时刻值，
            //——01分钟的时间段内有多个值
            //——01分钟和02分钟，没有边界值
            //——分割结果需要插值
            input = TestData_SpanPValue4RTDB.simuData4();
            results = null;
            results = his.SpanPValues4rtdbStep(input, startdate, endDate, 60);    //SpanPValues4rtdbStep是将实时数据视作离散阶梯型曲线来分割

            int temp = 10;

        }
        //并行时间分割，概化数据分割
        private void 并行时间分割SpanPValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryParallelCalcuEngine his = new HistoryParallelCalcuEngine();
            List<PValue> input;
            DateTime startdate = DateTime.Parse("2016-01-01 12:00:00");
            DateTime endDate = DateTime.Parse("2016-01-01 13:00:00");
            List<PValue>[] results;

            //测试分钟的分割
            //——原始值和要划分的时间段，恰好完全重合。
            //——分割结果与原始值相同
            input = TestData_SpanPValue4RDB.simuData1();
            results = null;
            //results = his.SpanPValues4rdb(input, startdate, endDate, 60);

            //——原始值，在划分的时间间隔内，缺少部分小周期值。比如按分钟划分。每分钟内缺少一些秒周期值。
            //——分割结果需要插值
            input = TestData_SpanPValue4RDB.simuData2();
            results = null;
            results = his.SpanPValues4rdb(input, startdate, endDate, 60);
        }
        //并行时间分割，条件数据分割
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryParallelCalcuEngine his = new HistoryParallelCalcuEngine();

            List<PValue> input;
            DateTime startdate = DateTime.Parse("2016-01-01 12:00:00");
            DateTime endDate = DateTime.Parse("2016-01-01 13:00:00");
            List<PValue>[] results;

            //测试分钟的分割
            //——原始值和要划分的时间段，恰好完全重合。
            //——分割结果与原始值相同
            input = TestData_SpanPValue.simuData1();
            results = null;
            results = his.SpanPValues4SpanFilter(input, startdate, endDate, 60);

            //——原始值和要划分的时间段，恰好完全重合。但是缺少起始时刻的时间段。
            //——分割结果与原始值相同.缺少的时间段返回为空。
            input = TestData_SpanPValue.simuData2();
            results = null;
            results = his.SpanPValues4SpanFilter(input, startdate, endDate, 60);

            //——原始值和要划分的时间段，恰好完全重合。但是缺少起始时刻，中间时刻，截止时刻等若干段。
            //——分割结果与原始值相同.缺少的时间段返回为空。
            input = TestData_SpanPValue.simuData3();
            results = null;
            results = his.SpanPValues4SpanFilter(input, startdate, endDate, 60);

            //——原始值和要划分的时间段，恰好完全重合。
            //——第一个有效时间段从中间某个时刻开始，持续几个分割时间。起始时间和截止时间和分割时间均不重合
            //——中间有一个单独的有效时间段，起始时间重合，截止时间不重合
            //——中间有一个单独的有效时间段，起始时间不重合，截止时间重合
            //——分割结果与原始值相同.缺少的时间段返回为空。
            input = TestData_SpanPValue.simuData4();
            results = null;
            results = his.SpanPValues4SpanFilter(input, startdate, endDate, 60);

            int temp = 10;
        }
        #endregion

        #region OPC接口测试
        //OPC接口测试
        private void oPCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //测试
            long ticks = DateTime.Parse("2018-01-30 20:00:00").Ticks;
            ticks = DateTime.Parse("2018-03-10 20:00:00").Ticks;
            DateTime begin = new DateTime(636555312000000000);
            DateTime starttime = DateTime.Parse("2018-04-8 19:00:00");
            DateTime endtime = DateTime.Parse("2018-05-9 23:59:00");
            //DateTime starttime = new DateTime(636531950890000000);
            //DateTime endtime = new DateTime(636531960880000000);
            List<PValue> read = OPCDAO.Read("Random.Int1", starttime, endtime);
        }
        #endregion

        #region PSLData接口测试
        private void pSLDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uint[] calcuIndexs = new uint[3] { 1, 2, 7 };
            PSLCalcuConfigDAO.DeleteConfigItems(calcuIndexs);

        }
        #endregion

        #region pslTagnameIDMap接口测试
        private void pSLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uint[] tagids = new uint[3] { 1, 2, 5 };
            PSLTagNameIdMapDAO.DeleteTags(tagids);
        }
        #endregion

        #region webTagnameIDMap接口测试
        private void webTagnameIDMap接口测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uint[] tagids = new uint[3] { 1, 2, 5 };
            WebTagNameIdMapDAO.DeleteTags(tagids);
        }
        #endregion

        private void generaltestToolStripMenuItem_Click(object sender, EventArgs e)
        {

            List<PValue> temp = new List<PValue>();
            temp.Add(new PValue(55, DateTime.Parse("2018-01-02"), DateTime.Parse("2018-01-02"), 1));
            temp.Add(new PValue(59, DateTime.Parse("2018-01-05"), DateTime.Parse("2018-01-07"), 1));
            temp.Add(new PValue(72, DateTime.Parse("2018-01-08"), DateTime.Parse("2018-01-09"), 1));
            temp.Add(new PValue(33, DateTime.Parse("2018-01-09"), DateTime.Parse("2018-01-12"), 1));
            temp.Add(new PValue(44, DateTime.Parse("2018-01-21"), DateTime.Parse("2018-01-22"), 1));

            PValue max = temp.First(n => n.Value == temp.Max(m => m.Value));

            List<PValue> temp1 = new List<PValue>();

            temp1.Add(null);

            double[] LimitArea = new double[8];

            List<PValue> temp2 = temp.Union(temp1).ToList();
            max = temp2.First(n => n.Value == temp2.Max(m => m.Value));
            int tempint = 10;

        }

        private void log测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogTest logtest = new LogTest();
            logtest.Show();
        }

    }
}