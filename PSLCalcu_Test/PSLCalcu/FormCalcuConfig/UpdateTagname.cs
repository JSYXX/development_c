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
    public partial class UpdateTagname : Form
    {
        string currentTagname = "";         //被替换的原标签名
        string afterTagname="";             //替换后的标签名
        Dictionary<string, uint> tagname2idMap = new Dictionary<string, uint>();        //标签名id映射

        public UpdateTagname()
        {
            InitializeComponent();
        }
        private void tb_currentname_TextChanged(object sender, EventArgs e)
        {
            currentTagname = this.tb_currentname.Text.Trim().ToUpper();
        }
        private void tb_aftername_TextChanged(object sender, EventArgs e)
        {
            afterTagname = this.tb_aftername.Text.Trim().ToUpper();
        }
        //改变选中状态
        private void cb_SelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.lv_searchresults.Items.Count; i++)
            {
                this.lv_searchresults.Items[i].Checked = this.cb_SelectAll.Checked;
            }
        }
        //搜索含有原标签名的计算项
        private void bt_search_Click(object sender, EventArgs e)
        {
            int i,j;
            //清空lv
            this.lv_searchresults.Items.Clear();

            if(currentTagname=="")
            {
                MessageBox.Show("请输原标签名称");
                return;
            }
            if (afterTagname == "")
            {
                MessageBox.Show("请输入修改后的标签名称");
                return;
            }
            List<PSLCalcuItem> pslcalcuitems=PSLCalcuConfigDAO.ReadConfigContainTagname(currentTagname);

            if(pslcalcuitems==null ||pslcalcuitems.Count==0)
            {
                MessageBox.Show("没有找到要替换的标签名称");
                return;
            }
            
            //将要替换的标签名称显示出来
            tagname2idMap = PSLTagNameIdMapDAO.ReadMap();

            string[] tagnames;
            uint[] tagids;
            int tagcount;
            for (i = 0; i < pslcalcuitems.Count; i++)
            {
                PSLCalcuItem pslcalcuitem = pslcalcuitems[i];
                string tagnamesStr = "";
                tagcount = 0;
                //判断处理源标签名称 
                tagnamesStr = pslcalcuitem.sourcetagname;
                if (tagnamesStr.Contains(currentTagname))
                {
                    //计数器
                    tagcount++;
                    //标签id号
                    tagnames = tagnamesStr.Split(';');
                    tagids = new uint[tagnames.Length];
                    for (j = 0; j < tagnames.Length; j++)
                    {
                        tagids[j] = tagname2idMap[tagnames[j]];
                    }
                    //添加lv
                    ListViewItem lvitem = this.lv_searchresults.Items.Add((pslcalcuitem.fid).ToString(), (pslcalcuitem.fid).ToString(), -1);        //第一列，添加计算项序号
                    lvitem.Checked = cb_SelectAll.Checked;                    
                    lvitem.SubItems.Add(tagcount.ToString());                               //第2列，是该计算项中含有currentTagname的项的序号
                    lvitem.SubItems.Add("源标签");                                          //第3列，是含有currentTagname的字段类型
                    lvitem.SubItems.Add(tagnamesStr);                                       //第4列，是该计算项中含有currentTagname的字段内容
                    lvitem.SubItems.Add(String.Join(";", tagids));                          //第5列，是该计算项中含有currentTagname的字段内容对应的id号
                    lvitem.SubItems.Add(tagnamesStr.Replace(currentTagname, afterTagname)); //第6列，是替换后的内容
                }
                //判断处理条件标签名称 
                tagnamesStr = pslcalcuitem.fcondpslnames;
                if (tagnamesStr.Contains(currentTagname))
                {
                    //计数器
                    tagcount++;
                    //标签id号
                    tagnames = tagnamesStr.Split(';');
                    tagids = new uint[tagnames.Length];
                    for (j = 0; j < tagnames.Length; j++)
                    {
                        tagids[j] = tagname2idMap[tagnames[j]];
                    }
                    //添加lv
                    ListViewItem lvitem = this.lv_searchresults.Items.Add((pslcalcuitem.fid).ToString(), (pslcalcuitem.fid).ToString(), -1);        //第一列，添加计算项序号
                    lvitem.Checked = cb_SelectAll.Checked;
                    lvitem.SubItems.Add(tagcount.ToString());                               //第2列，是该计算项中含有currentTagname的项的序号
                    lvitem.SubItems.Add("条件标签");                                         //第3列，是含有currentTagname的字段类型
                    lvitem.SubItems.Add(tagnamesStr);                                       //第4列，是该计算项中含有currentTagname的字段内容
                    lvitem.SubItems.Add(String.Join(";", tagids));                          //第5列，是该计算项中含有currentTagname的字段内容对应的id号
                    lvitem.SubItems.Add(tagnamesStr.Replace(currentTagname, afterTagname)); //第6列，是替换后的内容
                }
                //判断处理结果标签名称 
                tagnamesStr = pslcalcuitem.foutputpsltagnames;
                if (tagnamesStr.Contains(currentTagname))
                {
                    //计数器
                    tagcount++;
                    //标签id号
                    tagnames = tagnamesStr.Split(';');
                    tagids = new uint[tagnames.Length];
                    for (j = 0; j < tagnames.Length; j++)
                    {
                        tagids[j] = tagname2idMap[tagnames[j]];
                    }
                    //添加lv
                    ListViewItem lvitem = this.lv_searchresults.Items.Add((pslcalcuitem.fid).ToString(), (pslcalcuitem.fid).ToString(), -1);        //第一列，添加计算项序号
                    lvitem.Checked = cb_SelectAll.Checked;                    
                    lvitem.SubItems.Add(tagcount.ToString());                               //第2列，是该计算项中含有currentTagname的项的序号
                    lvitem.SubItems.Add("结果标签");                                         //第3列，是含有currentTagname的字段类型
                    lvitem.SubItems.Add(tagnamesStr);                                       //第4列，是该计算项中含有currentTagname的字段内容
                    lvitem.SubItems.Add(String.Join(";", tagids));                          //第5列，是该计算项中含有currentTagname的字段内容对应的id号
                    lvitem.SubItems.Add(tagnamesStr.Replace(currentTagname, afterTagname)); //第6列，是替换后的内容
                }
            }
            int temp = 10;

            

        }
        //修改标签名称
        private void bt_dorevise_Click(object sender, EventArgs e)
        {
            //将勾选的标签名称替换掉
            //——在计算配置表中进行替换
            //——在标签id映射中进行替换
            //遍历对象
            try
            {
                for (int i = 0; i < this.lv_searchresults.Items.Count; i++)
                {
                    if (this.lv_searchresults.Items[i].Checked)
                    {
                        uint fid = uint.Parse(this.lv_searchresults.Items[i].SubItems[0].Text);
                        string fieldtype = this.lv_searchresults.Items[i].SubItems[2].Text;
                        string[] tagids = this.lv_searchresults.Items[i].SubItems[4].Text.Split(';');
                        string aftertagname = this.lv_searchresults.Items[i].SubItems[5].Text;
                        string[] tagnames = this.lv_searchresults.Items[i].SubItems[5].Text.Split(';');
                        //更新配置表
                        PSLCalcuConfigDAO.UpdateTagnameInfo(fid, fieldtype, aftertagname);
                        //更新id映射表
                        PSLTagNameIdMapDAO.UpdateTagname(tagids, tagnames);
                        WebTagNameIdMapDAO.UpdateTagname(tagids, tagnames);

                        //
                        this.lv_searchresults.Items[i].Checked = false;

                    }
                }

                MessageBox.Show("更新完毕，请检查数据库‘计算配置表’、‘标签id映射表’、‘别名id映射表’。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改标签名错误，请检查！");
            }

        }

        private void lv_searchresults_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       

       
      

        
    }
}
