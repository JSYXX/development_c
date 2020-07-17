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
    public partial class DeleteConfig : Form
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(DataSearch));       //全局log

        private ToolTip tooltip;

        public List<PSLCalcuItem> PslCalcuItems { get; set; }               //概化计算(公式)配置对象集
        public Dictionary<string, System.UInt32> TagName2Id { get; set; }   //标签名称id字典       
        public int startYearTable;                                          //起始时间
        public int endYearTable;                                            //结束时间

        //全局变量
        uint[] tagids;                                                      //标签名称数组
        List<uint> calcuIndexs;                                             //计算项数组
        List<uint> alltagidsInCalcu;                                        //计算项数组包含的id
        List<uint> alltagidsInTags;                                         //标签中包含的id
        

        public DeleteConfig()
        {
            InitializeComponent();
        }

        private void bt_delelt4calcu_Click(object sender, EventArgs e)
        {
            string password = Input.InputBox.ShowInputBox("请输入操作密码", string.Empty);
            if (password != "newkind")
            {
                MessageBox.Show("密码不正确！");
                return;
            }
            //获取所有序号
            try
            {
                calcuIndexs = new List<uint>();
                string[] StrCalcuIndex = tb_calcuIndexs.Text.Split(';');
                for (int i = 0; i < StrCalcuIndex.Length; i++)
                {
                    if (StrCalcuIndex[i].Contains("-"))
                    {
                        string[] limitStr = StrCalcuIndex[i].Split('-');
                        int lower = int.Parse(limitStr[0]);
                        int upper = int.Parse(limitStr[1]);
                        for (int j = lower; j <= upper; j++)
                            calcuIndexs.Add((uint)j);
                    }
                    else
                    {
                        calcuIndexs.Add(uint.Parse(StrCalcuIndex[i]));
                    }
                }
            }
            catch
            {
                MessageBox.Show("计算项序号填写错误，请检查！正确的填写方式如：1;3;4-9");
                return;
            }
            //根据序号获取所有标签
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

            bool procflag = false;
            //删除所有标签对应的数据
            DateTime startdate = new DateTime(startYearTable, 1, 1);
            DateTime enddate = new DateTime(endYearTable, 12, 31, 23, 59, 59);
            procflag=deleteDatas(tagids, startdate, enddate);
            //删除所有标签
            if (procflag)
                deleteTags(tagids);
            else
                return;
            //删除所有计算项
            if (procflag)
                deleteCalcuItems(calcuIndexs.ToArray());
            else
                return;
          
        }

        //删除数据子程序
        private bool deleteDatas(uint[] tagids, DateTime startdate, DateTime enddate)
        {
            try
            {
                int psldeletecount = PSLDataDAO.Delete(tagids, startdate, enddate);
                MessageBox.Show("计算项对应的计算标签的数据已经删除完毕...");
                return true;
            }
            catch
            {
                string messageinfo = String.Format("删除标签数据时发生错误！错误信息见log文件。为了保证数据一致性，请重算数据，确保数据没有丢失！");
                MessageBox.Show(messageinfo);
                return false;
            }
        }
        //删除标签子程序
        private bool deleteTags(uint[] tagids)
        {
            try
            {
                PSLTagNameIdMapDAO.DeleteTags(tagids);
                WebTagNameIdMapDAO.DeleteTags(tagids);
                MessageBox.Show("计算项对应的计算标签已经删除完毕...");
                return true;
            }
            catch
            {
                string messageinfo = String.Format("删除标签时发生错误！错误信息见log文件。为了保证数据一致性，请检查两个标签id映射表！");
                MessageBox.Show(messageinfo);
                return false;
            }
        
        }
        //删除计算项子程序
        private bool deleteCalcuItems(uint[] calcuIndexs)
        {

            try
            {
                PSLCalcuConfigDAO.DeleteConfigItems(calcuIndexs);
                MessageBox.Show("选定计算项已经删除完毕，请关闭计算引擎后重新启动...");
                return true;
            }
            catch
            {
                string messageinfo = String.Format("删除计算配置项时发生错误！错误信息见log文件。为了保证数据一致性，请检查计算配置表！");
                MessageBox.Show(messageinfo);
                return false;
            }
        }
    }
}
