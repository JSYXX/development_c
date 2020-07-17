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
using ZedGraph;
using System.IO; 

namespace PSLCalcu
{
    //zedgraph，采用5.1.5版
    public partial class DataGraph : Form
    {
        public string[] tagnames{ get; set; }           //查询标签
        public DateTime startDatetime { get; set; }     //数据的起始时间
        public DateTime endDatetime { get; set; }      //数据的截止时间
        public List<PValue>[] searchData{ get; set; }   //查询结果
        public int startLeft { get; set; }              //窗口起始位置
        public int startTop { get; set; }               //窗口起始位置

        public int windowWidth { get; set; }                  //图形窗口宽度
        public int windowHeigh { get; set; }                  //图形窗口高度

        public DataGraph()
        {
            InitializeComponent();
        }

        private void DataGraph_Load(object sender, EventArgs e)
        {
            //设置鼠标悬停的处理程序
            //this.z1.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);

            //设定窗口位置
            this.Left = startLeft;
            this.Top = startTop;
            this.Width = this.windowWidth;
            this.Height = this.windowHeigh;
            
            //设置图形位置
            this.z1.Width = this.Width - 2 * (this.z1.Location.X-2);
            this.z1.Height = this.Height - 2 * (this.z1.Location.Y-4);
            
            
            this.z1.IsShowPointValues = true;
            //this.z1.GraphPane.Title = "查询结果";
            this.z1.GraphPane.XAxis.Title.Text="";                      //x轴标题
            this.z1.GraphPane.XAxis.Type = ZedGraph.AxisType.Date;      //x轴为时间类型
            this.z1.GraphPane.XAxis.Scale.Format = "MM-dd HH:mm";       //x轴刻度的格式MM-dd HH:mm:ss
            this.z1.GraphPane.XAxis.MajorGrid.IsVisible = true;         //辅助网格           
            
            
            this.z1.GraphPane.YAxis.Title.Text="";                      //y轴标题
            //this.z1.GraphPane.YAxis.ScaleFontSpec.Size = 12;
           
            

            //设置数据
            for (int i = 0; i < this.searchData.Length; i++)
            {
                if (searchData[i].Count>0)
                {
                    double[] x = new double[searchData[i].Count];          //x是时间tick值
                    double[] y = new double[searchData[i].Count];          //y是变量值
                    for (int j = 0; j < searchData[i].Count; j++)
                    {
                        x[j] = (double)new XDate(searchData[i][j].Timestamp);
                        y[j] = searchData[i][j].Value;
                    }
                    //添加曲线
                    z1.GraphPane.AddCurve(this.tagnames[i], x, y, Color.Red, SymbolType.None);                
                }                
            }

            //显示
            this.z1.AxisChange();
            this.z1.Invalidate();
        }
       
        //在zedgraph正确的版本下，x轴设置为时间的时候，tooltip已经可以正确的显示。
        //鼠标悬停节点事件
        //private string MyPointValueHandler(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        //{
        //    PointPair pt = curve[iPt];
        //    return "X=" + new DateTime((long)pt.X).ToString() + Environment.NewLine +"Y=" + pt.Y.ToString();
        //}

        //保存图像
        private void bt_Save_Click(object sender, EventArgs e)
        {
            string picPath = "D:\\DataTrendExport\\";
            string picfilename = tagnames[0].Replace("\\", "^") + this.startDatetime.ToString("_S_yyyy-MM-dd_HH-mm-ss") + this.endDatetime.ToString("_E_yyyy-MM-dd_HH-mm-ss") + "_PSL" + DateTime.Now.ToString("HHmmss") +".png";

            if (Directory.Exists(picPath) == false)//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(picPath);
            }

            this.z1.GetImage().Save(picPath+picfilename);   //如果路径或文件夹不存在，这里保存就会报错
        }
    }
}
