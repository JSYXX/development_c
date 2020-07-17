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
using Config;
using System.Reflection;                //使用反射
using PSLCalcu.Module;                  //使用计算模块
using System.Diagnostics;               //使用计时器
using System.Text.RegularExpressions;   //使用正则表达式

namespace PSLCalcu
{
    public partial class PSLCalcuModuleTest : Form
    {
        List<PSLModule> pslmodules = new List<PSLModule>();
        string pslmodulesname = "";
        string[] pslmodulesoutputdesc;
        string[] pslmodulesoutputdesccn;        
        string defaultpath="d:\\RealDataExport";
        string[] csvfilename;
        string calcupara = "";
        DateTime startdate;
        DateTime enddate;

        public PSLCalcuModuleTest()
        {
            InitializeComponent();
        }

        private void PSLCalcuModuleTest_Load(object sender, EventArgs e)
        {
            pslmodules = PSLModulesDAO.ReadData();                  //读取算法模块信息
            string[] pslmodulenames=new string[pslmodules.Count];   //获取算法模块名称列表             
            for(int i=0;i<pslmodules.Count;i++)
            {
                pslmodulenames[i]=pslmodules[i].modulename;
            }
            pslmodulenames = pslmodulenames.OrderBy(s => s).ToArray();               //按字母顺序排

            cb_calcumodulename.Items.Clear();                       //初始化算法名称下拉列表
            cb_calcumodulename.Items.AddRange(pslmodulenames);      //算法模块列表
            lb_csvdatafile.Text =defaultpath;                       //默认计算数据文件路径
        }
        //选择算法模块
        private void cb_calcumodulename_SelectedIndexChanged(object sender, EventArgs e)
        {
                    
            pslmodulesname = this.cb_calcumodulename.Text;
            PSLModule selectmodule=pslmodules.Where(a => (a.modulename == pslmodulesname)).First();
            pslmodulesoutputdesc = selectmodule.moduleoutputdescs.Split(';');
            pslmodulesoutputdesccn = selectmodule.moduleoutputdescscn.Split(';');

            tb_calcupara.Text = selectmodule.moduleparaexample;
        }
        //选择csv文件
        private void bt_csvdata_Click(object sender, EventArgs e)
        {                      
            //数据文件路径
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;//该值确定是否可以选择多个文件
            file.Title = "选择要导入的点表文件（CSV）";
            file.Filter = "所有文件(*.csv)|*.csv";

            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                csvfilename = new string[file.FileNames.Length];
                for (int i = 0; i < file.FileNames.Length; i++)
                {
                    csvfilename[i] = file.FileNames[i];
                }
            }
            else
            {
                return;
            }

            if (csvfilename.Length != 0) lb_csvdatafile.Text = csvfilename[0];
        }
        //计算参数
        private void lb_calcupara_TextChanged(object sender, EventArgs e)
        {
            calcupara = this.tb_calcupara.Text;
        }


        //计算
        private void button1_Click(object sender, EventArgs e)
        {
            //1、读取数据
            List<PValue>[] inputs = new List<PValue>[csvfilename.Length];
            for(int i=0;i<csvfilename.Length;i++)
            {
                string[][] csvdata=CsvFileReader.Read(csvfilename[i]);
                //如果读取数据为null，不一定为失败
                //——在批量导出数据的时候，某些点可能就不存在数据，比如span类的点。此时，该类点的数据文件仍然生成，但内容为空。
                //——当这类点作为rdbset中的一个点被用作一个算法的输入数据时，需要保持null状态，作为inputs的一员被输入数据（尽管真正的计算中可能不用)，以保证算法可以进行。
                /*
                if (csvdata == null || csvdata.Length == 0)
                {
                    MessageBox.Show("读取数据失败！");
                    return;
                }
                 */
                if (csvdata == null || csvdata.Length == 0)
                {
                    inputs[i] = null;
                }
                else
                {
                    List<PValue> input = Array2PValue(csvdata);
                    inputs[i] = input;
                }
               
            }
            startdate=inputs[0][0].Timestamp;
            enddate = inputs[0][inputs[0].Count - 1].Endtime;

            //2、用正则表达式检查参数
            var swTimer = Stopwatch.StartNew();     //开始计算计时
            //2.1 计算：获取计算对象（反射法）
            Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);                                   //获得PSLCalcu.exe
            Type calcuclass = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + pslmodulesname);             //获得当前计算指定的算法类.注意，这里必须是“命名空间.类名”
            PropertyInfo inputData = calcuclass.GetProperty("inputData");                                           //获得算法类的静态参数inputData
            PropertyInfo calcuInfo = calcuclass.GetProperty("calcuInfo");                                           //获得算法类的静态参数calcuInfo                    
            MethodInfo Calcu = calcuclass.GetMethod("Calcu", new Type[] { });                                       //获得算法类的Calcu()方法。注意，Calcu方法有重载，这里需要指明获得哪个具体对象，否则会报错
            PropertyInfo ErrorFlag = calcuclass.GetProperty("errorFlag");                                           //获得算法类的静态参数ErrorFlag，bool类型
            PropertyInfo ErrorInfo = calcuclass.GetProperty("errorInfo");

            //2.3 计算：主算法
            //1、这里需要判断Calcu是否为空，如果为空，要写log
            inputData.SetValue(null, inputs);            //将输入数据给入算法
            System.UInt32[] tagids=new System.UInt32[]{(System.UInt32)(APPConfig.rdbtable_constmaxnumber+1),(System.UInt32)(APPConfig.rdbtable_constmaxnumber+2)};
            string tagnames = this.tb_sourctagnames.Text;
            bool[] tagflags=new bool[]{true,true};
            calcuInfo.SetValue(null, new CalcuInfo(tagnames, tagids, tagflags,pslmodulesname, calcupara, startdate, enddate, 0, 100));       //将当前计算信息给入算法
            Results Results = (Results)Calcu.Invoke(null, null);

            string realSpan = swTimer.Elapsed.ToString();   //计算计时结束
            
            if (Results.errorFlag)
            {
                MessageBox.Show("计算错误：" + Results.errorInfo);
                return;
            }
            //展示计算结果
            //清空listview的已有数据项
            this.lv_calcuresults.Items.Clear();
            //根据results添加
            List<PValue>[] results = Results.results;
            if (results == null || results.Count()==0)
            {
                MessageBox.Show("计算结果为空");
                return;
            }
            for (int i = 0; i < results.Length; i++)              //为listview添加 items.Count行数据
            {
                if (results[i] == null) continue;                   //有可能results部分计算结果为空
                for (int j = 0; j < results[i].Count; j++)
                {
                    PValue pv = results[i][j];
                    ListViewItem lvitem = this.lv_calcuresults.Items.Add((i + 1).ToString(), (i + 1).ToString(), -1);  //Items.Add可以参考definition说明，但是仍有问题。这里的add用法，使用指定key创建item。用的是第二项name创建
                    lvitem.SubItems.Add(j.ToString());                                      //序号
                    lvitem.SubItems.Add(pslmodulesoutputdesc[i]);                           //结算结果英文后缀
                   lvitem.SubItems.Add(pslmodulesoutputdesccn[i]);                         //结算结果中文描述                    
                    lvitem.SubItems.Add(results[i][j].Value.ToString(""));                  //计算结果数值
                    lvitem.SubItems.Add(results[i][j].Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                    lvitem.SubItems.Add(results[i][j].Endtime.ToString("yyyy-MM-dd HH:mm:ss"));
                    
                }
            }
            this.lb_calcuspan.Text = realSpan;

        }

        public List<PValue> Array2PValue(string[][] csvdata)
        {
            List<PValue> results = new List<PValue>();
            if (csvdata.Length > 0)
            {

                for (int i = 0; i < csvdata.Length-1; i++)
                {
                    results.Add(new PValue(double.Parse(csvdata[i][0]), DateTime.Parse(csvdata[i][1]), DateTime.Parse(csvdata[i + 1][1]), int.Parse(csvdata[i][2])));
                }
                
                //添加截止时刻值
                results.Add(new PValue(double.Parse(csvdata[csvdata.Length - 1][0]), DateTime.Parse(csvdata[csvdata.Length - 1][1]), DateTime.Parse(csvdata[csvdata.Length - 1][1]), int.Parse(csvdata[csvdata.Length - 1][2])));
                return results;              
            }
            else
            {
                return null;
            }
        }

        


    }
}
