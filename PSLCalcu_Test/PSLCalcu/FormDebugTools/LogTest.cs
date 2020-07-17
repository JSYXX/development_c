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
    public partial class LogTest : Form
    {
        private LogHelper logHelper = LogFactory.GetLogger(typeof(LogTest));                     //全局log
        bool stopflag = false;

        public LogTest()
        {
            InitializeComponent();
        }

        private void btn_exportCSV_Click(object sender, EventArgs e)
        {
            if (this.btn_exportCSV.Text == "开始写入")
            {
                btn_exportCSV.Text = "停止";

                for (int i = 0; i < 500000; i++)
                {
                    if (this.cb_Debug.Checked) logHelper.Debug("logger info +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    if (this.cb_Info.Checked) logHelper.Info("logger info +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    if (this.cb_Warn.Checked) logHelper.Warn("logger info +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    if (this.cb_error.Checked) logHelper.Error("debug message++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    if (this.cb_fatal.Checked) logHelper.Fatal("logger info +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    if (stopflag) break;
                }

            }
            else
            {
                btn_exportCSV.Text = "开始写入";
                stopflag = true;
            }
        }
    }
}
