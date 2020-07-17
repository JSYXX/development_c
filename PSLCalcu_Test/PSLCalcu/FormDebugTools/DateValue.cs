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

namespace PSLCalcu
{
    public partial class DateValue : Form
    {
        //全局变量
        DateTime startDatetime;                         //起始时间
        DateTime endDatetime;                           //结束时间
        DateTime insertDatetime;                        //插值时间

        public DateValue()
        {
            InitializeComponent();
        }

        private void DateValue_Load(object sender, EventArgs e)
        {
            this.dt_startdate.Value = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
            this.dt_starttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            this.dt_enddate.Value = DateTime.Now.AddMonths(-0).AddDays(-DateTime.Now.Day + 1);
            this.dt_endtime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            this.dt_insertdate.Value = DateTime.Now.AddMonths(-0).AddDays(-DateTime.Now.Day + 1);
            this.dt_inserttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
        }

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
        //插值日期
        private void dt_insertdate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_insertdate.Text, this.dt_inserttime.Text);
            this.insertDatetime = DateTime.Parse(datetimeStr);
        }
        //插值时间
        private void dt_inserttime_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} {1}", this.dt_insertdate.Text, this.dt_inserttime.Text);
            this.insertDatetime = DateTime.Parse(datetimeStr);
        }


        private void bt_export4calcu_Click(object sender, EventArgs e)
        {
            PValue beforepoint = new PValue(double.Parse(tb_startvalue.Text), startDatetime, 0);
            PValue afterpoint = new PValue(double.Parse(tb_endvalue.Text), endDatetime, 0);

            PValue insertpoint = PValue.Interpolate(beforepoint, afterpoint, insertDatetime);
            tb_insertvalue.Text = insertpoint.Value.ToString();

        }

       
    }
}
