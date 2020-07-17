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
    public partial class HistoryHelp : Form
    {
        public HistoryHelp()
        {
            InitializeComponent();
        }

        private void HistoryHelp_Load(object sender, EventArgs e)
        {
            this.lb_HelpInfo.Text="关于历史计算时间段的说明："+Environment.NewLine+
                                "——只有计算周期为秒、分、时、日的计算项才能使用历史计算功能。" + Environment.NewLine +
                                "——实际的并发计算时间段，是界面设定的“年月日”加计算项的计算起始时间中的“时分秒”。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'日'的项，'时分秒'为日周期的具体起始时刻。" + Environment.NewLine +
                                "——比如，某个以“日”为周期的计算项当前的下次计算起始时刻是2018-08-02 2:02:02" + Environment.NewLine +
                                "——如果并发计算时间段选为2018-01-01到2018-04-01 。" + Environment.NewLine +
                                "——则实际的并发计算时间段为2018-01-01 2:02:02到2018-04-01 2:02:02  。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'小时'的项，'分秒'为小时周期的具体起始时刻。" + Environment.NewLine +
                                "——比如，某个以“小时”为周期的计算项当前的下次计算起始是2018-08-02 2:15:00" + Environment.NewLine +
                                "——如果并发计算时间段选为2018-01-01到2018-04-01 。" + Environment.NewLine +
                                "——则实际的并发计算时间段为2018-01-01 0:15:00到2018-04-01 0:15:00  。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'分'的项，'秒'为分周期的具体起始时刻。" + Environment.NewLine +
                                "——比如，某个以“分”为周期的计算项当前的下次计算起始是2018-08-02 2:15:05" + Environment.NewLine +
                                "——如果并发计算时间段选为2018-01-01到2018-04-01 。" + Environment.NewLine +
                                "——则实际的并发计算时间段为2018-01-01 0:00:05到2018-04-01 0:00:05  。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——并发计算要删除的数据的起始时间和截止时间，要重新计算的数据的起始时间和截止时间均以以上规则为准。";
        }
    }
}
