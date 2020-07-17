using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PSLCalcu.Module;      //CalcuInfo
using PCCommon;

namespace PSLCalcu
{
    public partial class ReadConstInfo2PSLData : Form
    {
        public List<PSLCalcuItem> readConstItems;

        private DateTime StartDateForConstFile;
        private DateTime EndDateForConstFile;

        public ReadConstInfo2PSLData()
        {
           
            InitializeComponent();
            string datetimeStr = string.Format("{0} 0:00:00", this.dtStartDate.Text);
            this.StartDateForConstFile = this.dtStartDate.Value;     //初始化历史数据计算起始时间变量
            datetimeStr = string.Format("{0} 0:00:00", this.dtEndDate.Text);
            this.EndDateForConstFile = this.dtEndDate.Value;         //初始化历史数据计算结束时间变量
        }

        private void dtStartDate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} 0:00:00", this.dtStartDate.Text);
            this.StartDateForConstFile = DateTime.Parse(datetimeStr);
        }

        private void dtEndDate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} 0:00:00", this.dtEndDate.Text);
            this.EndDateForConstFile = DateTime.Parse(datetimeStr);
        }
        private void bt_read_Click(object sender, EventArgs e)
        {
            try
            {
                
                foreach (PSLCalcuItem item in readConstItems)
                {

                    //先删除对应时间段内的所有常数标签信息
                    PSLDataDAO.Delete(item.foutputpsltagids, this.StartDateForConstFile, this.EndDateForConstFile);
                    item.fstarttime = this.StartDateForConstFile;
                    item.fendtime = item.GetEndTime();
                    do
                    {
                        //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
                        MReadConst.inputData = null;
                        MReadConst.calcuInfo = new CalcuInfo();
                        MReadConst.calcuInfo.fmodulename = "MReadConst";
                        MReadConst.calcuInfo.sourcetagname = "";
                        MReadConst.calcuInfo.fparas = item.fparas;
                        MReadConst.calcuInfo.fstarttime = item.fstarttime;
                        MReadConst.calcuInfo.fendtime = item.fendtime;

                        //计算
                        Results Results = MReadConst.Calcu();
                        List<PValue>[] results = Results.results;
                        //如果查询不到，需要返回一个整体不为空，但是对应元素为空的对象
                        if (results != null && Array.IndexOf(results, null) == -1)    //当results不为空，并且results中每一个变量都不为空时，才写结果
                        {
                            for (int i = 0; i < results.Length; i++)
                            {
                                //写计算结果，如果时刻点重复，则删除。
                                PSLDataDAO.WriteOrUpdate(item.foutputpsltagids[i], results[i], item.fstarttime, item.fendtime);
                            }
                        }

                        //下一次计算
                        item.fstarttime = item.fendtime;                                            //下一次计算结果对应时间段的起始时间。是当次计算结果对应的结束时间。
                        item.fendtime = item.GetEndTime();

                    } while (item.fstarttime < this.EndDateForConstFile);
                }

                string strMsg = String.Format("常数标签信息读取完毕，请用数据查询功能检查相应的常数标签！");
                MessageBox.Show(strMsg);
                this.Close();
            }
            catch (Exception ex)
            {
                string strMsg = String.Format("常数标签信息读取错误，请检查!");
                MessageBox.Show(strMsg);
            }
        }
    }
}
