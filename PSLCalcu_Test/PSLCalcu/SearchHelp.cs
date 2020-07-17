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
    public partial class SearchHelp : Form
    {
        public SearchHelp()
        {
            InitializeComponent();
        }

        private void SearchHelp_Load(object sender, EventArgs e)
        {
            this.lb_HelpInfo.Text = "关于修改计算项时刻的说明：" + Environment.NewLine +
                                "——修改的是计算项下一次计算起始的时刻。" + Environment.NewLine +
                                "——对于不同计算周期，设定时刻的有效部分不同。" + Environment.NewLine + 
                                "——计算项的下一次计算起始的时刻中比计算周期设定的周期单位更小的部分，一旦载入是不变的。。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'年'的项，设定时刻的有效部分为'年'，剩余'月日时分秒'无效。" + Environment.NewLine +
                                "——对于计算周期为'月'的项，设定时刻的有效部分为'年月'，剩余'日时分秒'无效。" + Environment.NewLine +                                
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'日'的项，设定时刻的有效部分为'年月日'，剩余'时分秒'无效。" + Environment.NewLine +
                                "——比如，某个以“日”为周期的计算项当前的下次计算起始时刻是2018-08-02 2:00:00" + Environment.NewLine +
                                "——设定时刻为2017-02-01 1:00:00 。" + Environment.NewLine +
                                "——则该计算项实际的计算起始时刻会被设定为2017-02-01 2:00:00 。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'小时'的项，设定时刻的有效部分为'年月日时'，剩余'分秒'无效。" + Environment.NewLine +
                                "——比如，某个以“小时”为周期的计算项当前的下次计算起始是2018-08-02 2:15:00" + Environment.NewLine +
                                "——设定时刻为2017-02-01 1:00:00 。" + Environment.NewLine +
                                "——则该计算项实际的计算起始时刻会被设定为2017-02-01 1:00:00  。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'分'的项，设定时刻的有效部分为'年月日时分'，剩余'秒'无效。" + Environment.NewLine +
                                "——比如，某个以“分”为周期的计算项当前的下次计算起始是2018-08-02 2:15:05" + Environment.NewLine +
                                "——设定时刻为2017-02-01 1:10:00 。" + Environment.NewLine +
                                "——则该计算项实际的计算起始时刻会被设定为2017-02-01 1:10:00 。" + Environment.NewLine +
                                "———————————————————————————————————————————" + Environment.NewLine +
                                "——对于计算周期为'秒'的项，设定时刻全部有效。";
        }

        private void lb_HelpInfo_Click(object sender, EventArgs e)
        {

        }
    }
}
