using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSLCalcu
{
    public partial class DateTransTool : Form
    {
        private DateTime startDatetime;
        public DateTransTool()
        {
            InitializeComponent();
        }
        private void DateTransTool_Load(object sender, EventArgs e)
        {
            this.dt_startdate.Value = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);                      //初始化计算时间段，开始时间：以结束时间向前一年为开始时间。
            this.dt_starttime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);     //初始化计算时间段，结束时间：以前当系统时间所在月份的第一条0:00:00位结束时间
        }
        private void btn_exportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                long ticks = long.Parse(this.tb_rawticks.Text);
                DateTime date = new DateTime(ticks);
                this.tb_resultdate.Text = date.ToString("yyyy/MM/dd hh:mm:ss");
            }
            catch (Exception ex)
            {
                this.tb_resultdate.Text = "清输入正确的tick值，tick为长整型！";
            }
        }

       

        private void dt_startdate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_startdate.Text, this.dt_starttime.Text);
            this.startDatetime = DateTime.Parse(datetimeStr);
        }

        private void dt_starttime_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_startdate.Text, this.dt_starttime.Text);
            this.startDatetime = DateTime.Parse(datetimeStr);
            
        }

        private void btn_transticks_Click(object sender, EventArgs e)
        {
            try
            {
                this.tb_resultticks.Text = this.startDatetime.Ticks.ToString();
            }
            catch (Exception ex)
            {
                this.tb_resultticks.Text = "清输入正确的date值！";
            }
        }

       
    }
}
