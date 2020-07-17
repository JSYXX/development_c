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

    public partial class ReadShiftInfo2PSLData : Form
    {
        public List<PSLCalcuItem> readShiftItems;
        
        
        private DateTime StartDateForShifteFile;
        private DateTime EndDateForShifteFile;
        
        
        public ReadShiftInfo2PSLData()
        {
            InitializeComponent();
            string datetimeStr = string.Format("{0} 0:00:00", this.dtStartDate.Text);
            this.StartDateForShifteFile = this.dtStartDate.Value;     //初始化历史数据计算起始时间变量
            datetimeStr = string.Format("{0} 0:00:00", this.dtEndDate.Text);
            this.EndDateForShifteFile = this.dtEndDate.Value;         //初始化历史数据计算结束时间变量
        }

        private void dtStartDate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} 0:00:00", this.dtStartDate.Text);
            this.StartDateForShifteFile = DateTime.Parse(datetimeStr);  
        }

        private void dtEndDate_ValueChanged(object sender, EventArgs e)
        {
            string datetimeStr = string.Format("{0} 0:00:00", this.dtEndDate.Text);
            this.EndDateForShifteFile = DateTime.Parse(datetimeStr);      
        }

        private void bt_readshiftinfo_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (PSLCalcuItem item in readShiftItems)
                {

                    item.fstarttime = this.StartDateForShifteFile;
                    item.fendtime = item.GetEndTime();
                    do
                    {
                        //2、配置计算模块。1、选择要测试的计算模块。2、给入输入数据。3、给定输入参数calcuInfo.fparas。4、给定计算条件           
                        MReadShift.inputData = null;
                        MReadShift.calcuInfo = new CalcuInfo();
                        MReadShift.calcuInfo.fmodulename = "MReadShift";
                        MReadShift.calcuInfo.sourcetagname = "";
                        MReadShift.calcuInfo.fparas = item.fparas;
                        MReadShift.calcuInfo.fstarttime = item.fstarttime;
                        MReadShift.calcuInfo.fendtime = item.fendtime;

                        //计算
                        Results Results = MReadShift.Calcu();
                        List<PValue>[] results = Results.results;
                        //如果查询不到，需要返回一个整体不为空，但是对应元素为空的对象
                        if (results != null && results[0]!=null)
                        {
                            //删除结果对应时间内的信息
                            List<PValue> result = results[0].OrderBy(m => m.Timestamp).ToList();
                            //后面的值此表中的值次所指定的时间，如果与之前有重复，会先删除前面的表中对应时间段内的值此信息。
                            PSLDataDAO.Delete(item.foutputpsltagids, result[0].Timestamp, result[result.Count - 1].Endtime);  
                                
                            //写计算结果
                            PSLDataDAO.WriteOrUpdate(item.foutputpsltagids[0], result, item.fstarttime, item.fendtime);
                            if (PSLDataDAO.ErrorFlag)
                            {
                                MessageBox.Show("值次表数据写入错误！可能是值次设定的时间范围超出数据表时间范围，请检查...");
                            }
                        }

                        //下一次计算
                        item.fstarttime = item.fendtime;                                            //下一次计算结果对应时间段的起始时间。是当次计算结果对应的结束时间。
                        item.fendtime = item.GetEndTime();

                    } while (item.fstarttime < this.EndDateForShifteFile);
                }

                string strMsg = String.Format("值次信息读取完毕，请用数据查询功能检查值次对应的标签！");
                MessageBox.Show(strMsg);
                this.Close();
            }
            catch (Exception ex)
            {
                string strMsg = String.Format("值次信息读取错误，请检查!");
                MessageBox.Show(strMsg);
            }
        }


    }
}
