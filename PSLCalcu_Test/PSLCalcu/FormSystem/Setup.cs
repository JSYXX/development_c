using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;   //使用正则表达式
using Config;

namespace PSLCalcu
{
    public partial class Setup : Form
    {

        public Setup()
        {
            try 
            {
                InitializeComponent();

                #region “数据表配置”页
                //"计算结果标签名自动生成“
                if (APPConfig.rdbtable_resulttagauto == "1")
                {
                    this.rb_TagAuto.Checked = true;
                    this.rb_TagManual.Checked = false;
                }
                else
                {
                    this.rb_TagAuto.Checked = false;
                    this.rb_TagManual.Checked = true;
                }
                if (APPConfig.rdbtable_resulttagincludeinterval == "1")
                {
                    this.rb_IncludeIntervalType.Checked = true;
                    this.rb_ExcludeIntervalType.Checked = false;
                }
                else
                {
                    this.rb_IncludeIntervalType.Checked = false;
                    this.rb_ExcludeIntervalType.Checked = true;
                }
                //“生成tag id映射表时，重新编id号
                if (APPConfig.rdbtable_tag2idalwaysreset == "1")
                {
                    this.cbTag2IdMapReset.Checked = true;
                }
                else
                {
                    this.cbTag2IdMapReset.Checked = false;
                }
                //“初始化时包含psldata表”
                if (APPConfig.rdbtable_iniTableIncludePsldata == "1")
                {
                    this.cbIniTableIncludePSLData.Checked = true;
                }
                else 
                {
                    this.cbIniTableIncludePSLData.Checked = false;
                }
                //初始化psldata起止年月
                this.tbStartYear.Text = APPConfig.psldata_startyear.ToString();
                this.tbEndYear.Text = APPConfig.psldata_endyear.ToString();
                #endregion
                
                #region “实时计算引擎”页
                //初始化“是否自动启动计算
                if (APPConfig.realcalcu_autorun == "1")
                {
                    this.cbAutoRun.Checked = true;
                }
                else 
                {
                    this.cbAutoRun.Checked = false;
                }
                //初始化“计算周期”
                this.tb_CalcuPeriod.Text = APPConfig.realcalcu_period.ToString();
                //初始化"保存计算配置对象周期"
                this.tb_WritePeriod.Text = APPConfig.realcalcu_periodwritepslcalcuitem.ToString();
                //初始化“统计计算信息”
                if (APPConfig.realcalcu_recordcalcutime == "1")
                {
                    this.cbCalcuTime.Checked = true;
                }
                else
                {
                    this.cbCalcuTime.Checked = false;
                }
                //统计计算信息，每多少条记录保存一次
                this.tb_saveNumber.Text = APPConfig.realcalcu_recordsavenumber.ToString();
                this.tbMaxReadRTDB.Text = APPConfig.realcalcu_maxreadrtdb.ToString();
                //初始化“计算引擎一次读取数据条数”
                
                #endregion

                #region "历史计算引擎"页
                this.tb_ParallelCalcuPeriod4RTD.Text = APPConfig.historycalcu_period4RTD.ToString();
                this.tb_ParallelCalcuPeriod4PSL.Text = APPConfig.historycalcu_period4PSL.ToString();
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("配置文件有误，请检查！");
            }
           
        }

        private void tb_CalcuPeriod_TextChanged(object sender, EventArgs e)
        {

        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            //"计算结果标签名自动生成“
            if (this.rb_TagAuto.Checked)
            {//这里注意SaveConfig()接口，第一个参数要写xpaht，并且写到要更新节点的父节点。
                APPConfig.SaveConfig("/config", "resulttagauto", "1");
                APPConfig.rdbtable_resulttagauto = "1";
            }
            else
            {
                APPConfig.SaveConfig("/config", "resulttagauto", "0");
                APPConfig.rdbtable_resulttagauto = "0";
            }
            //"计算结果标签名自动生成“
            if (this.rb_IncludeIntervalType.Checked)
            {//这里注意SaveConfig()接口，第一个参数要写xpaht，并且写到要更新节点的父节点。
                APPConfig.SaveConfig("/config", "resulttagincludeinterval", "1");
                APPConfig.rdbtable_resulttagincludeinterval = "1";
            }
            else
            {
                APPConfig.SaveConfig("/config", "resulttagincludeinterval", "0");
                APPConfig.rdbtable_resulttagincludeinterval = "0";
            }
            //“生成tag id映射表时，重新编id号
            if (this.rb_TagAuto.Checked)
            {
                APPConfig.SaveConfig("/config", "tag2idalwaysreset", "1");
                APPConfig.rdbtable_tag2idalwaysreset = "1";
            }
            else 
            {
                APPConfig.SaveConfig("/config", "tag2idalwaysreset", "0");
                APPConfig.rdbtable_tag2idalwaysreset = "0";
            }
            //“初始化时包含psldata表”
            if (this.cbIniTableIncludePSLData.Checked)
            {
                APPConfig.SaveConfig("/config", "rdbtable_iniTableIncludePsldata", "1");
                APPConfig.rdbtable_tag2idalwaysreset = "1";
            }
            else
            {
                APPConfig.SaveConfig("/config", "rdbtable_iniTableIncludePsldata", "0");
                APPConfig.rdbtable_tag2idalwaysreset = "0";
            }
            //psldata时间
            APPConfig.psldata_startyear = int.Parse(this.tbStartYear.Text);
            APPConfig.SaveConfig("/config", "startyear", int.Parse(this.tbStartYear.Text).ToString());
            APPConfig.psldata_endyear = int.Parse(this.tbEndYear.Text);
            APPConfig.SaveConfig("/config", "endyear", int.Parse(this.tbEndYear.Text).ToString());          
            
            
            this.Close();
            
        }

        private void 数据库及计算配置_Click(object sender, EventArgs e)
        {

        }
        //保存实时计算引擎配置
        private void btSaveCalcu_Click(object sender, EventArgs e)
        {
            //是否自动计算
            if (this.cbAutoRun.Checked)
            {
                APPConfig.realcalcu_autorun = "1";
                APPConfig.SaveConfig("/config", "autorun", "1");
            }
            else
            {
                APPConfig.realcalcu_autorun = "0";
                APPConfig.SaveConfig("/config", "autorun", "0");
            }
            //计算扫描周期
            if (this.tb_CalcuPeriod.Text != "")
            {
                APPConfig.SaveConfig("/config", "period", this.tb_CalcuPeriod.Text);
                APPConfig.realcalcu_period = int.Parse(this.tb_CalcuPeriod.Text);
            }
            //保存计算配置对象信息周期
            if (this.tb_WritePeriod.Text != "")
            {                
                APPConfig.SaveConfig("/config", "periodwritepslcalcuitem", this.tb_WritePeriod.Text);
                APPConfig.realcalcu_periodwritepslcalcuitem = int.Parse(this.tb_WritePeriod.Text);
            }
            //统计计算模件用时
            if (this.cbCalcuTime.Checked)
            {
                APPConfig.realcalcu_recordcalcutime = "1";
                APPConfig.SaveConfig("/config", "recordcalcutime", "1");
            }
            else
            {
                APPConfig.realcalcu_recordcalcutime = "0";
                APPConfig.SaveConfig("/config", "recordcalcutime", "0");
            }
            if (this.tb_saveNumber.Text != "")
            {
                APPConfig.realcalcu_recordsavenumber = int.Parse(this.tb_saveNumber.Text);
                APPConfig.SaveConfig("/config", "recordsavenumber", this.tb_saveNumber.Text);
            }
            //单次读取实时数据记录最大数量
            //该参数不允许在界面配置。

            this.Close();
        }
        //保存历史计算引擎配置
        private void bt_savehistorycalcu_Click(object sender, EventArgs e)
        {
            
            APPConfig.SaveConfig("/config", "historycalcu_period4RTD", APPConfig.historycalcu_period4RTD.ToString());
            
            APPConfig.SaveConfig("/config", "historycalcu_period4PSL", APPConfig.historycalcu_period4PSL.ToString());

            this.Close();
        }
        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void rb_ExcludeIntervalType_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cbTag2IdMapReset_CheckedChanged(object sender, EventArgs e)
        {

        }
        //检查“实时数据计算并发周期”
        private void tb_ParallelCalcuPeriod4RTD_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(this.tb_ParallelCalcuPeriod4RTD.Text, @"^[+-]?\d*[.]?\d*$"))
            {
                APPConfig.historycalcu_period4RTD = int.Parse(this.tb_ParallelCalcuPeriod4RTD.Text);
            }
            else
            {
                MessageBox.Show("实时数据计算并发周期，必须是整形，单位为秒");
            }
        }

        private void tb_ParallelCalcuPeriod4PSL_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(this.tb_ParallelCalcuPeriod4PSL.Text, @"^[+-]?\d*[.]?\d*$"))
            {
                APPConfig.historycalcu_period4PSL = int.Parse(this.tb_ParallelCalcuPeriod4PSL.Text);
            }
            else
            {
                MessageBox.Show("实时数据计算并发周期，必须是整形，单位为秒");
            }
        }

        private void cbIniTableIncludePSLData_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rb_TagManual_CheckedChanged(object sender, EventArgs e)
        {

        }

       
       
    }
}
