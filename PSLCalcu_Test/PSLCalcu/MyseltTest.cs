using Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu
{
    public partial class AppRunForm
    {  /*
        public AppRunForm()
        {


            InitializeComponent();
            //初始化窗口大小
            Rectangle myScreen = System.Windows.Forms.Screen.GetWorkingArea(this);           //获得屏幕实际分辨率（如1920*1036）
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
       */
    }
}
