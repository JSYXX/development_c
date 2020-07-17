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
    public partial class DataDelete : Form
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(DataSearch));       //全局log

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
        
        //初始化
        public DataDelete()
        {
            //实例化ToolTip控件
            tooltip = new ToolTip();
            //设置提示框显示时间，默认5000，最大为32767，超过此数，将以默认5000显示           
            tooltip.AutoPopDelay = 30000;
            //是否以球状显示提示框            
            tooltip.IsBalloon = true;

            InitializeComponent();
        }
        
        
        private void DataDelete_Load(object sender, EventArgs e)
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

        //按计算项序号
        private void lb_calcuIndexs_MouseHover(object sender, EventArgs e)
        {
            string MessageStr = "计算项序号说明：" + Environment.NewLine +
                             "——计算项序号可以用分号;分割单个计算项，可以用中杠-表示连续计算项。" + Environment.NewLine +
                             "——比如，1;3;6表示删除第1、3、6项计算的所有计算结果。" + Environment.NewLine +
                             "——比如，1-7表示删除从第1项到第7项计算的所有计算结果。"
                             ;
            tooltip.Show(MessageStr, lb_calcuIndexs, lb_calcuIndexs.Width , 0);
        }
        private void tb_calcuIndexs_TextChanged(object sender, EventArgs e)
        {
            
           
        }
        //按标签名称
        private void lb_tagnames_MouseMove(object sender, MouseEventArgs e)
        {
            string MessageStr = "标签名称说明：" + Environment.NewLine +
                             "——标签名称可以用分号;分割单个计算项。"
                             ;
            tooltip.Show(MessageStr, lb_tagnames, lb_tagnames.Width , 0);
        }
        private void tb_tagnames_TextChanged(object sender, EventArgs e)
        {
           
        }      
        //按计算项删除       
        private void bt_delelt4calcu_Click(object sender, EventArgs e)
        {
            string password = Input.InputBox.ShowInputBox("请输入操作密码", string.Empty);
            if (password != "newkind")
            {
                MessageBox.Show("密码不正确！");
                return;
            }
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
                string messageinfo = String.Format("删除数据时发生错误，可能是计算序号无效或超范围！错误信息：" + Environment.NewLine +
                                                   " + PSLDataDAO.ErrorInfo"
                                                  );
                MessageBox.Show(messageinfo);
                return;
            }
            tagids = alltagidsInCalcu.ToArray();
            deleteData(tagids, startDatetime, endDatetime);
        }
        //按标签名称进行删除
        private void bt_delelt4tags_Click(object sender, EventArgs e)
        {
            string password = Input.InputBox.ShowInputBox("请输入操作密码", string.Empty);
            if (password != "newkind")
            {
                MessageBox.Show("密码不正确！");
                return;
            }
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
                string messageinfo = String.Format("删除数据时发生错误！错误信息：" + Environment.NewLine +
                                                   " + PSLDataDAO.ErrorInfo"
                                                  );
                MessageBox.Show(messageinfo);
                return;
            }
            tagids = alltagidsInTags.ToArray();
            deleteData(tagids, startDatetime, endDatetime);
        }        
        //删除子程序
        private void deleteData(uint[] tagids,DateTime startdate,DateTime enddate)
        {
            try
            {
                int psldeletecount = PSLDataDAO.Delete(tagids, this.startDatetime, this.endDatetime);
                this.lb_result.Text = String.Format("共删除{0}条记录！", psldeletecount);
                string messageinfo = String.Format("已删除完毕，共删除{0}条记录！", psldeletecount);
                MessageBox.Show(messageinfo);
            }
            catch
            {
                string messageinfo = String.Format("删除数据时发生错误！错误信息见log文件。");
                MessageBox.Show(messageinfo);
                return;
            }
        }
        //按计算项进行导出
        private void bt_export4calcu_Click(object sender, EventArgs e)
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
                string messageinfo = String.Format("导出数据时发生错误！错误信息：" + Environment.NewLine +
                                                   " + PSLDataDAO.ErrorInfo"
                                                  );
                MessageBox.Show(messageinfo);
                return;
            }
            tagids = alltagidsInCalcu.ToArray();
            exportData(tagids, TagName2Id, startDatetime, endDatetime);
        }
        //按标签项进行导出
        private void bt_export4tags_Click(object sender, EventArgs e)
        {
            try
            {
                alltagidsInTags = new List<uint>();
                string[] tagnames = tb_tagnames.Text.Split(';');
                for (int i = 0; i < tagnames.Length; i++)
                {
                    alltagidsInTags.Add(this.TagName2Id[tagnames[i]]);
                }
                tagids = alltagidsInTags.ToArray();
                exportData(tagids, TagName2Id, startDatetime, endDatetime);
            }
            catch
            {
                string messageinfo = String.Format("导出数据时发生错误！错误信息：" + Environment.NewLine +
                                                   " + PSLDataDAO.ErrorInfo"
                                                  );
                MessageBox.Show(messageinfo);
                return;
            }
        }
        //导出子程序       
        private void exportData(uint[] tagids,  Dictionary<string, System.UInt32> TagName2Id,DateTime startdate, DateTime enddate)
        {
            try
            {
                for (int i = 0; i < tagids.Length; i++)
                {
                    //读取数据
                    List<PValue> pslresults = PSLDataDAO.Read(tagids[i], this.startDatetime, this.endDatetime);
                    if (pslresults == null) continue;
                    //准备文件名称
                    String filePath = "D:\\RealDataExport";
                    string tagname = (string)TagName2Id.FirstOrDefault(q => q.Value == tagids[i]).Key;                   
                    string filename = filePath + "\\" + String.Format("{0:000}_",i)+tagname.Replace("\\", "^") + this.startDatetime.ToString("_S_yyyy-MM-dd_HH-mm-ss") + this.endDatetime.ToString("_E_yyyy-MM-dd_HH-mm-ss") + ".csv";

                    if (Directory.Exists(filePath) == false)//如果不存在就创建file文件夹
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    if (File.Exists(filename) == true)
                    {
                        File.Delete(filename);
                    }
                    string[][] csvData = PValue2Array(pslresults);

                    //有时候某些数据就找不到对应的数据，这里数据可能为null。遇到这种情况不能直接跳过。而应该形成文件，仅是在向文件写数据的过程中跳过
                    //if (csvData == null) continue;

                    CsvFileReader.Save(csvData, filename);

                    this.lb_result.Text = String.Format("正在导出第{0}个点！", i);
                    
                }
                this.lb_result.Text = String.Format("导出完毕，共导出{0}个点！", tagids.Length);
                string messageinfo = String.Format("已导出完毕，请检查导出数据。");
                MessageBox.Show(messageinfo);
                return;
            }
            catch
            {
                string messageinfo = String.Format("删除数据时发生错误！错误信息见log文件。");
                MessageBox.Show(messageinfo);
                return;
            }
        }

        //辅助函数
        public static string[][] PValue2Array(List<PValue> pvalues)
        {
            //pvalues带截止时刻值
            string[][] results;
            if (pvalues.Count > 0)
            {

                results = new string[pvalues.Count][];      //pvalues包含截止时刻数据，转换的results也包含截止时刻值
                for (int i = 0; i < pvalues.Count; i++)
                {
                    results[i] = new string[3];
                    results[i][0] = pvalues[i].Value.ToString();
                    results[i][1] = pvalues[i].Timestamp.ToString();
                    results[i][2] = pvalues[i].Status.ToString();
                }
                /*
                results[pvalues.Count] = new string[3];
                results[pvalues.Count][0] = pvalues[pvalues.Count - 1].Value.ToString();
                results[pvalues.Count][1] = pvalues[pvalues.Count - 1].Endtime.ToString();
                results[pvalues.Count][2] = pvalues[pvalues.Count - 1].Status.ToString();
                */
                return results;
            }
            else
            {
                return null;
            }
        } 
        
    }
}
