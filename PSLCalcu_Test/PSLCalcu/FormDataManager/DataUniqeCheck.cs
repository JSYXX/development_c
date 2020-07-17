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
using System.IO;

namespace PSLCalcu
{
    public partial class DataUniqeCheck : Form
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(LogHelper));       //全局log

        private ToolTip tooltip;

        public List<PSLCalcuItem> PslCalcuItems { get; set; }               //概化计算(公式)配置对象集
        public Dictionary<string, System.UInt32> TagName2Id { get; set; }   //标签名称id字典             

        //全局变量
        uint[] tagids;                                  //标签名称数组
        List<int> calcuIndexs;                          //计算项数组
        List<uint> alltagidsInCalcu;                    //计算项数组包含的id
        List<uint> alltagidsInTags;                     //标签中包含的id
        DateTime startDatetime;                         //起始时间
        DateTime endDatetime;                           //结束时间

        int count = 0;                                  //重复记录计数

        public DataUniqeCheck()
        {
            //实例化ToolTip控件
            tooltip = new ToolTip();
            //设置提示框显示时间，默认5000，最大为32767，超过此数，将以默认5000显示           
            tooltip.AutoPopDelay = 30000;
            //是否以球状显示提示框            
            tooltip.IsBalloon = true;

            InitializeComponent();
        }

        private void DataUniqeCheck_Load(object sender, EventArgs e)
        {
            this.dt_startdate.Value = DateTime.Now.AddMonths(-1).AddDays(-DateTime.Now.Day + 1);
            this.dt_starttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

            this.dt_enddate.Value = DateTime.Now.AddMonths(-0).AddDays(-DateTime.Now.Day + 1);
            this.dt_endtime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
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
        //按计算项序号检查
        private void bt_check4calcu_Click(object sender, EventArgs e)
        {           
            try
            {
                calcuIndexs = new List<int>();
                string[] StrCalcuIndex = tb_calcuIndexs.Text.Split(';');
                for (int i = 0; i < StrCalcuIndex.Length; i++)
                {
                    if (StrCalcuIndex[i].Contains("-"))
                    {
                        string[] limitStr = StrCalcuIndex[i].Split('-');
                        int lower = int.Parse(limitStr[0]);
                        int upper = int.Parse(limitStr[1]);
                        for (int j = lower; j <= upper; j++)
                            calcuIndexs.Add(j);
                    }
                    else
                    {
                        calcuIndexs.Add(int.Parse(StrCalcuIndex[i]));
                    }
                }
            }
            catch
            {
                MessageBox.Show("计算项序号填写错误，请检查！正确的填写方式如：1;3;4-9");
                return;
            }

            try
            {
                alltagidsInCalcu = new List<uint>();
                for (int i = 0; i < calcuIndexs.Count; i++)
                {
                    PSLCalcuItem calcuitem = this.PslCalcuItems.First(p => p.fid == calcuIndexs[i]);
                    for (int j = 0; j < calcuitem.foutputpsltagids.Length; j++)
                    {
                        alltagidsInCalcu.Add(calcuitem.foutputpsltagids[j]);
                    }
                }

            }
            catch (Exception ex)
            {
                string messageinfo = String.Format("抽取标签id时发生错误：" + Environment.NewLine +
                                                   "——详细错误信息："+ex.ToString()
                                                  );
                MessageBox.Show(messageinfo);
                return;
            }
            tagids = alltagidsInCalcu.ToArray();
            checkData();     //本程序用到的三个变量uint[] tagids, DateTime startdate, DateTime enddate，均为全局变量
        }
        //按标签名称检查
        private void bt_check4tagnames_Click(object sender, EventArgs e)
        {           
            try
            {
                alltagidsInTags = new List<uint>();
                string[] tagnames = tb_tagnames.Text.Split(';');
                for (int i = 0; i < tagnames.Length; i++)
                {
                    alltagidsInTags.Add(this.TagName2Id[tagnames[i]]);
                }

            }
            catch
            {
                MessageBox.Show("标签名称填写错误，请检查！");
                return;               
            }
            tagids = alltagidsInTags.ToArray();
            checkData();     //本程序用到的三个变量uint[] tagids, DateTime startdate, DateTime enddate，均为全局变量
        }
        //按标签id
        private void bt_check4tagids_Click(object sender, EventArgs e)
        {
            try
            {
                alltagidsInTags = new List<uint>();
                string[] tagidsStr = tb_tagids.Text.Split(';');
                for (int i = 0; i < tagidsStr.Length; i++)
                {
                    alltagidsInTags.Add(uint.Parse(tagidsStr[i]));
                }
            }
            catch
            {
                MessageBox.Show("标签名称填写错误，请检查！");
                return; 
            }
            tagids = alltagidsInTags.ToArray();
            checkData();     //本程序用到的三个变量uint[] tagids, DateTime startdate, DateTime enddate，均为全局变量
        }


        //给检查子程序准备的进度条窗体、后台进程、以及后台进程控制窗体程序
        ImportProgress importProgress = new ImportProgress();                   //进度条窗体
        protected BackgroundWorker worker = new BackgroundWorker();             //后台线程
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)  //后台线程，进度条窗体进度控制函数：后台线程对象操作状态改变时（状态改变消息），更新界面
        {
            importProgress.ProgressText = e.UserState.ToString();       //更新label
            importProgress.ProgressValue = e.ProgressPercentage;        //更新进度条            
        }
        //检查子程序
        private void checkData()
        {
            

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
            
           
        }//end checkdata

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //本程序用到的三个变量uint[] tagids, DateTime startdate, DateTime enddate，均为全局变量
            List<PValue> results = new List<PValue>();
            string messageStr = "" + Environment.NewLine;                 //记录错误
            count = 0;                          //记录数量
            try
            {


                for (int i = 0; i < tagids.Length; i++)
                {
                    //刷新ui                           
                    worker.ReportProgress((i+1) * 100 / tagids.Length, String.Format("检查psldata表时间戳重复的记录，共{0}个tag，正在检查第{1}个tag！", tagids.Length, i+1));
                    //
                    results = PSLDataDAO.UniqeCheck(tagids[i], this.startDatetime, this.endDatetime);
                    //                            
                    if (results == null || results.Count == 0) continue;
                    for (int j = 0; j < results.Count; j++)
                    {
                        messageStr += String.Format("标签id-{0}在{1}时刻有重复数据。" + Environment.NewLine, tagids[i], results[j].Timestamp.ToString("yyyy-MM-dd hh:mm:ss:fff"));
                        count += 1;
                    }
                    
                }
                //更新界面结果
                //this.lb_checkResult.Text = String.Format("共查询到{0}条时间重复的记录！", count);
                //写log
                logHelper.Error(messageStr);
                            
                return;
            }
            catch (Exception ex)
            {
                string messageinfo = String.Format("检查数据时发生错误！错误信息见log文件。");
                MessageBox.Show(messageinfo);
                return;
            }
        }//end  worker_DoWork
        //关闭后台进程
        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)   //进度条窗体进度控制：后台线程完成时（操作完成消息），关闭进度条窗体
        {
            //关闭进度界面
            importProgress.Close();
            //结束提示
            string messageinfo = "";    
            if (count != 0)
            {
                
                messageinfo = String.Format("已查询完毕，共查到{0}条重复记录。详细检查结果在log\\Error.log中!", count);
            }
            else
            {
                messageinfo = String.Format("已查询完毕，没有重复记录!");
            }

            MessageBox.Show(messageinfo);

            //执行完毕后，要移除注册的事件
            worker.DoWork -= new DoWorkEventHandler(worker_DoWork);                                     //执行完毕，解除注册的事件
            worker.ProgressChanged -= new ProgressChangedEventHandler(worker_ProgressChanged);          //执行完毕，解除注册的事件
            worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted); //执行完毕，解除注册的事件 
        }

        
    }
}
