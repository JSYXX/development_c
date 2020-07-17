using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Config;
using PCCommon;
using PSLCalcu.Module;                  //使用计算模块
using System.Diagnostics;               //使用计时器
using System.Reflection;                //使用反射

namespace PSLCalcu
{
    public partial class CheckCurve : Form
    {
        int SelectCurveIndex = 0;       //计算选定的曲线
        DateTime startDatetime;         //计算选定的时间
        string modulename;              //计算选定的组件


        public CheckCurve()
        {
            InitializeComponent();
        }
        private void CheckCurve_Load(object sender, EventArgs e)
        {
            cb_select.Items.Clear();
            cb_select.Items.Add("一维期望曲线");          //对应SelectedIndex=0
            cb_select.Items.Add("二维期望曲线");          //对应SelectedIndex=1
            cb_select.Items.Add("得分曲线");             //对应SelectedIndex=2
            cb_select.SelectedIndex = 0;                //默认

        }
        //载入不同页面        
        private void tc_Curve_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            switch (this.tc_Curve.SelectedIndex)
            {
                case 0:
                    //*******配置曲线读取情况****界面初始化
                    this.cb_select.Items.Clear();
                    this.cb_select.Items.Add("一维期望曲线");          //对应SelectedIndex=0
                    this.cb_select.Items.Add("二维期望曲线");          //对应SelectedIndex=1
                    this.cb_select.Items.Add("得分曲线");             //对应SelectedIndex=2
                    this.cb_select.SelectedIndex = 0;                //默认
                    break;
                case 1:
                    //*******偏差计算****界面初始化
                    cb_biascurve.Items.Clear();
                    //添加一维期望曲线
                    List<string> Dic=new List<string>();
                    if (CurveConfig.Instance.OPXCurves != null && CurveConfig.Instance.OPXCurves.Count != 0)
                    {
                        if (CurveConfig.Instance.OPXCurves.FindAll(x => { return x == null; }).Count != CurveConfig.Instance.OPXCurves.Count) //如果曲线对象集不为空，但曲线对象均为null也不行
                        {
                            for (int i = 0; i < CurveConfig.Instance.OPXCurves.Count; i++)
                            {
                                string strTemp=String.Format("一维期望曲线,{0},{1}",CurveConfig.Instance.OPXCurves[i].Index,CurveConfig.Instance.OPXCurves[i].Name);
                                //考虑到同样的Index和Name可能存在不同有效时刻的曲线配置，因此先看看以Index和Name字符串有么有重复
                                if(!Dic.Contains(strTemp))
                                {
                                    Dic.Add(strTemp);
                                    this.cb_biascurve.Items.Add(strTemp);
                                }
                                
                                
                            }
                        }
                    }
                    //添加二维期望曲线
                    Dic = new List<string>();
                    if (CurveConfig.Instance.OPXCurves2D != null && CurveConfig.Instance.OPXCurves2D.Count != 0)
                    {
                        if (CurveConfig.Instance.OPXCurves2D.FindAll(x => { return x == null; }).Count != CurveConfig.Instance.OPXCurves2D.Count)   //如果曲线对象集不为空，但曲线对象均为null也不行
                        {
                            for (int i = 0; i < CurveConfig.Instance.OPXCurves2D.Count; i++)
                            {
                                string strTemp = String.Format("二维期望曲线,{0},{1}", CurveConfig.Instance.OPXCurves2D[i].Index, CurveConfig.Instance.OPXCurves2D[i].Name);
                                //考虑到同样的Index和Name可能存在不同有效时刻的曲线配置，因此先看看以Index和Name字符串有么有重复
                                if (!Dic.Contains(strTemp))
                                {
                                    Dic.Add(strTemp);
                                    this.cb_biascurve.Items.Add(strTemp);
                                }
                            }
                        }
                    }
                    //时间赋初值
                    startDatetime = DateTime.Now;
                    this.dt_startdate.Value = DateTime.Now.AddMonths(0).AddDays(0);
                    this.dt_starttime.Value = DateTime.Now.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                    break;

            }
        }

        //**************************************配置曲线读取情况*****************************************
        private void cb_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatereadstatus();
        }
       
        private void updatereadstatus()
        {
            string strTemp = "";
            switch (this.cb_select.SelectedIndex)
            {
                    
                case 0:
                    strTemp=CurveConfig.Instance.OPXCurvesReadStatus;
                    if(CurveConfig.Instance.OPXCurves!=null && CurveConfig.Instance.OPXCurves.Count!=0)     
                    {
                        if (CurveConfig.Instance.OPXCurves.FindAll(x => { return x == null; }).Count != CurveConfig.Instance.OPXCurves.Count) //如果曲线对象集不为空，但曲线对象均为null也不行
                        {
                            for (int i = 0; i < CurveConfig.Instance.OPXCurves.Count; i++)
                            {
                                Curve1D bias_1D = CurveConfig.Instance.OPXCurves[i];
                                strTemp = strTemp + Environment.NewLine + String.Format("一维偏差曲线第{0}条，名称是：{1}" + Environment.NewLine + "X数组是：{2}" + Environment.NewLine + "Y数组是：{3}",
                                    i + 1,
                                    bias_1D.Name,
                                    String.Join(",", bias_1D.Xvalue),
                                    String.Join(",", bias_1D.Yvalue)
                                    );
                            }
                        }
                    }
                    this.tb_readstatus.Text = strTemp;
                    break;
                case 1:
                    strTemp=CurveConfig.Instance.OPXCurves2DReadStatus;
                    
                    if(CurveConfig.Instance.OPXCurves2D!=null && CurveConfig.Instance.OPXCurves2D.Count!=0)
                    {
                        if (CurveConfig.Instance.OPXCurves2D.FindAll(x => { return x == null; }).Count != CurveConfig.Instance.OPXCurves2D.Count)   //如果曲线对象集不为空，但曲线对象均为null也不行
                        {
                            for (int i = 0; i < CurveConfig.Instance.OPXCurves2D.Count; i++)
                            {
                                Curve2D bias_2D = CurveConfig.Instance.OPXCurves2D[i];
                                strTemp = strTemp + Environment.NewLine + String.Format("二维偏差曲线第{0}条，名称是：{1}" + Environment.NewLine + "X数组是：{2}" + Environment.NewLine + "Y数组是：{3}",
                                    i + 1,
                                    bias_2D.Name,
                                    String.Join(",", bias_2D.Xvalue),
                                    String.Join(",", bias_2D.Yvalue)
                                    );
                            }
                        }
                    }
                    this.tb_readstatus.Text = strTemp;
                    break;
                case 2:
                    strTemp = CurveConfig.Instance.ScoreCurvesReadStatus;
                    if (CurveConfig.Instance.ScoreCurves != null && CurveConfig.Instance.ScoreCurves.Count != 0)
                    {
                        if (CurveConfig.Instance.ScoreCurves.FindAll(x => { return x == null; }).Count != CurveConfig.Instance.ScoreCurves.Count)   //如果曲线对象集不为空，但曲线对象均为null也不行
                        {
                            for (int i = 0; i < CurveConfig.Instance.ScoreCurves.Count; i++)
                            {
                                Curve1D score = CurveConfig.Instance.ScoreCurves[i];
                                strTemp = strTemp + Environment.NewLine + String.Format("得分曲线第{0}条，名称是：{1}" + Environment.NewLine + "X数组是：{2}" + Environment.NewLine + "Y数组是：{3}",
                                    i,
                                    score.Name,
                                    String.Join(",", score.Xvalue),
                                    String.Join(",", score.Yvalue)
                                    );
                            }
                        }
                    }
                    this.tb_readstatus.Text = strTemp;
                    break;
                default:
                    break;
                    
            }
        }
        //**************************************偏差试算*****************************************
        //根据选择，显示曲线信息
        private void cb_biascurve_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] curvesinfo = cb_biascurve.Text.Split(',');
            //全局变量获取曲线id号
            this.SelectCurveIndex = int.Parse(curvesinfo[1]);            //全局变量获取曲线id号
            
            //获取曲线信息
            
            string curverStr = "";
            if (curvesinfo[0] == "一维期望曲线")
            {
                List<Curve1D> curvers=CurveConfig.Instance.OPXCurves.FindAll(x => { return x.Index==SelectCurveIndex; });
                for(int i=0;i<curvers.Count;i++)
                {
                    curverStr = curverStr + Environment.NewLine + Environment.NewLine + String.Format("一维期望曲线名称:{0}", curvers[i].Name);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线有效时间:{0}", curvers[i].validDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线配置模式:{0}", curvers[i].Mode);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量X有效范围:[{0},{1}]", curvers[i].XLowerLimit, curvers[i].XUpperLimit);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量X序列:{0}", String.Join(",",curvers[i].Xvalue));
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线应变量Y序列:{0}", String.Join(",", curvers[i].Yvalue));
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线Function系数:A-{0}，B-{1}，C-{2}", curvers[i].A, curvers[i].B, curvers[i].C);
                }
                //this.tb_biascurveinfo.Text=curverStr;
                this.tb_input1.Enabled = true;
                this.tb_input2.Enabled = false;
                //当前选定的计算组件。注意有多个计算组件
                this.modulename = "MCurveAssessS";
            }
            else if (curvesinfo[0] == "二维期望曲线")
            { 
                List<Curve2D> curvers=CurveConfig.Instance.OPXCurves2D.FindAll(x => { return x.Index==SelectCurveIndex; });
                for(int i=0;i<curvers.Count;i++)
                {
                    curverStr = curverStr + Environment.NewLine + Environment.NewLine + String.Format("二维曲线名称:{0}", curvers[i].Name);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线有效时间:{0}", curvers[i].validDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线配置模式:{0}", curvers[i].Mode);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量X有效范围:[{0},{1}]", curvers[i].XLowerLimit, curvers[i].XUpperLimit);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量X序列:{0}", String.Join(",",curvers[i].Xvalue));
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量Y有效范围:[{0},{1}]", curvers[i].YLowerLimit, curvers[i].YUpperLimit);
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量Y序列:{0}", String.Join(",", curvers[i].Yvalue));
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线应变力Z矩阵:");
                    curverStr = curverStr + Environment.NewLine + String.Format("-,{0}",String.Join(",",curvers[i].Xvalue));
                    for(int j=0;j<curvers[i].Ztable.Length;j++)
                    {
                        curverStr = curverStr + Environment.NewLine + String.Format("{0},{1}",curvers[i].Yvalue[j], String.Join(",",curvers[i].Ztable[j]));
                    }
                    curverStr = curverStr + Environment.NewLine + String.Format("曲线Function系数:A1-{0}，B1-{1}，A2-{2}，B2-{3}，C-{4}", 
                                                                                curvers[i].A1, 
                                                                                curvers[i].B1, 
                                                                                curvers[i].A2,
                                                                                curvers[i].B2,
                                                                                curvers[i].C);
                }
                this.tb_biascurveinfo.Text = curverStr;
                this.tb_input1.Enabled = true;
                this.tb_input2.Enabled = true;
                //当前选定的计算组件。注意有多个计算组件
                this.modulename = "MCurveAssess2DS";
            }
            //显示对应序号的得分曲线
            List<Curve1D> scores = CurveConfig.Instance.ScoreCurves.FindAll(x => { return x.Index == SelectCurveIndex; });
            for (int i = 0; i < scores.Count; i++)
            {
                curverStr = curverStr + Environment.NewLine + Environment.NewLine + String.Format("得分曲线名称:{0}", scores[i].Name);
                curverStr = curverStr + Environment.NewLine + String.Format("曲线有效时间:{0}", scores[i].validDate.ToString("yyyy-MM-dd HH:mm:ss"));
                curverStr = curverStr + Environment.NewLine + String.Format("曲线配置模式:{0}", scores[i].Mode);
                curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量X有效范围:[{0},{1}]", scores[i].XLowerLimit, scores[i].XUpperLimit);
                curverStr = curverStr + Environment.NewLine + String.Format("曲线自变量X序列:{0}", String.Join(",", scores[i].Xvalue));
                curverStr = curverStr + Environment.NewLine + String.Format("曲线应变量Y序列:{0}", String.Join(",", scores[i].Yvalue));
                curverStr = curverStr + Environment.NewLine + String.Format("曲线Function系数:A-{0}，B-{1}，C-{2}", scores[i].A, scores[i].B, scores[i].C);
            }
            
            //更新节目信息
            this.tb_biascurveinfo.Text = curverStr;
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
        //开始计算
        private void bt_calcu_Click(object sender, EventArgs e)
        {
            //提取数据
            double x = 0;
            double y = 0;
            double output = 0; ;
            try
            {
            
                if(this.tb_input1.Enabled) x=double.Parse(this.tb_input1.Text);
                if(this.tb_input2.Enabled) y=double.Parse(this.tb_input2.Text);
                output = double.Parse(this.tb_output.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("输入有误请检查！");
                return;
            }
            //注明计算模块名
            this.lb_modulename.Text = "当前计算组件："+this.modulename;
            //注明找到的曲线的有效时间
            Curve1D curve1d;
            Curve2D curve2d;
            if(!this.tb_input2.Enabled)
            {
                //this.tb_input2.Enabled=false，说明是一维期望曲线
                curve1d= CurveConfig.GetOPXCurve(this.SelectCurveIndex, this.startDatetime);
                this.lb_vailddate.Text="选定曲线有效时间："+curve1d.validDate.ToString();
            }
            else
            {
                //this.tb_input2.Enabled=true，说明是二维期望曲线
                curve2d= CurveConfig.GetOPXCurve2D(this.SelectCurveIndex, this.startDatetime);
                this.lb_vailddate.Text = "选定曲线有效时间：" + curve2d.validDate.ToString();
            }

            //准备参数
            string calcupara = "S;" + this.SelectCurveIndex.ToString() + ";" + this.SelectCurveIndex.ToString();

            //计算
            //1、准备数据
            List<PValue>[] inputs;
            if (this.tb_input2.Enabled)
            {
                inputs = new List<PValue>[3];
                inputs[0] = new List<PValue>();
                inputs[0].Add(new PValue(output, this.startDatetime, this.startDatetime.AddMinutes(1), 0));
                inputs[0].Add(new PValue(output, this.startDatetime.AddMinutes(1), this.startDatetime.AddMinutes(1), 0));
                inputs[1] = new List<PValue>();
                inputs[1].Add(new PValue(x, this.startDatetime, this.startDatetime.AddMinutes(1), 0));
                inputs[1].Add(new PValue(x, this.startDatetime.AddMinutes(1), this.startDatetime.AddMinutes(1), 0));
                inputs[2] = new List<PValue>();
                inputs[2].Add(new PValue(y, this.startDatetime, this.startDatetime.AddMinutes(1), 0));
                inputs[2].Add(new PValue(y, this.startDatetime.AddMinutes(1), this.startDatetime.AddMinutes(1), 0));               
            }
            else
            {
                inputs = new List<PValue>[3];
                inputs[0] = new List<PValue>();
                inputs[0].Add(new PValue(output, this.startDatetime, this.startDatetime.AddMinutes(1), 0));
                inputs[0].Add(new PValue(output, this.startDatetime.AddMinutes(1), this.startDatetime.AddMinutes(1), 0));
                inputs[1] = new List<PValue>();
                inputs[1].Add(new PValue(x, this.startDatetime, this.startDatetime.AddMinutes(1), 0));
                inputs[1].Add(new PValue(x, this.startDatetime.AddMinutes(1), this.startDatetime.AddMinutes(1), 0));
                inputs = inputs.Take(2).ToArray();
               
            }

            //2、计算
            var swTimer = Stopwatch.StartNew();     //开始计算计时
            //2.1 计算：获取计算对象（反射法）
            Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);                                   //获得PSLCalcu.exe
            Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + this.modulename);             //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”
            PropertyInfo inputData = calcuclass.GetProperty("inputData");                                           //获得算法类的静态参数inputData
            PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");                                           //获得算法类的静态参数calcuInfo                    
            MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { });                                       //获得算法类的Calcu()方法。注意，Calcu方法有重载，这里需要指明获得哪个具体对象，否则会报错
            PropertyInfo ErrorFlag = calcuclass.GetProperty("errorFlag");                                           //获得算法类的静态参数ErrorFlag，bool类型
            PropertyInfo ErrorInfo = calcuclass.GetProperty("errorInfo");

            //2.3 计算：主算法
            //1、这里需要判断Calcu是否为空，如果为空，要写log
            inputData.SetValue(null, inputs);            //将输入数据给入算法
            System.UInt32[] tagids = new System.UInt32[] { (System.UInt32)(APPConfig.rdbtable_constmaxnumber + 1), (System.UInt32)(APPConfig.rdbtable_constmaxnumber + 2) };
            string tagnames = "";
            bool[] tagflags = new bool[inputs.Length];
            for (int i = 0; i < tagflags.Length; i++) { tagflags[i] = true; }
            calcuInfo.SetValue(null, new CalcuInfo(tagnames, tagids, tagflags,this.modulename, calcupara, this.startDatetime, this.startDatetime.AddMinutes(1), 0, 100));       //将当前计算信息给入算法
            Results Results = (Results)Calcu.Invoke(null, null);

            string realSpan = swTimer.Elapsed.ToString();   //计算计时结束
            if (Results.warningFlag)
            {
                MessageBox.Show("计算警告：" + Results.warningInfo);
                return;
            }
            if (Results.errorFlag)
            {
                MessageBox.Show("计算错误：" + Results.errorInfo);
                return;
            }

            //展示计算结果
            this.tb_sp.Text = Results.results[0][0].Value.ToString();
            this.tb_spstatus.Text = Results.results[0][0].Status.ToString();
            this.tb_error.Text = Results.results[1][0].Value.ToString();
            this.tb_errorstatus.Text = Results.results[1][0].Status.ToString();
            this.tb_score.Text = Results.results[3][0].Value.ToString();
            this.tb_scorestatus.Text = Results.results[3][0].Status.ToString();
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb_calcuIndexs_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tb_readstatus_TextChanged(object sender, EventArgs e)
        {

        }

       
       

       
      

      

       
    }
}
