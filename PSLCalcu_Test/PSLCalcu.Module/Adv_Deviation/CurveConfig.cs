using System;
using System.Collections.Generic;
using System.IO;                //读取文件
using System.Windows.Forms;     //提出错窗口
using System.Linq;
using System.Collections;       //使用hastable

namespace PSLCalcu.Module
{
    //一维偏差曲线、一维得分曲线
    public class Curve1D
    {
        public string readstatus { get; set; }      //曲线读取状态。
        public int Index { get; set; }              //曲线的序号，可以不连续。比如可以1号机组以1开头，2号机组以2开头
        public DateTime validDate { get; set; }     //曲线生效时间
        public string Name { get; set; }            //曲线的名称，用来调试时，查看曲线是否正确
        public string Mode { get; set; }            //曲线的计算方式，Table：查表方法，Function：函数方法
        public double XLowerLimit { get; set; }     //自变量X考核范围下限
        public double XUpperLimit { get; set; }     //自变量X考核范围上限
        public double[] Xvalue { get; set; }        //查表方式，表的X
        public double[] Yvalue { get; set; }        //查表方式，表的Y
        public double A { get; set; }               //函数方法，Y=AX^2+BX+C
        public double B { get; set; }               //函数方法，Y=AX^2+BX+C
        public double C { get; set; }               //函数方法，Y=AX^2+BX+C

        //构造函数
        public Curve1D() { }
        //一维期望、得分计算函数
        public double Fx(double X)
        {
            //在计算时，该算法即可能用来计算期望值，也可能用来计算得分值
            //——采用Table计算期望值或者得分值得时候，要先判断自变量X是否在TableX给定的范围内，如果不在范围内，自动给出边界得分。
            //      比如，X小于给定的X的范围的最小值，则得分Y自动给X最小值对应的得分值（或期望值）
            //      再如，X大于给定的X的范围的最大值，则得分Y自动给Y最大值对应的得分值（或期望值）
            //——采用Function计算期望值或者得分值得时候，也要在Table的位置给定X的最大最小值，已及对应的得分（或者期望值）。在使用Function公式进行计算时，也要先判断X是否在给定的范围内。如果不在范围内，自动给出边界得分。
            //      比如，X小于给定的X的范围的最小值，则得分Y自动给X最小值对应的得分值（或期望值）
            //      再如，X大于给定的X的范围的最大值，则得分Y自动给Y最大值对应的得分值（或期望值）
            //      X在范围内，才会使用公式进行计算。
            //      采用公式进行计算时，Table中给定的边界值与公式计算的边界值要注意一定不能有突变。
            //出现错误返回一个一般情况下不可能的期望值-10000
            try
            {            
                if (this.Mode == "TABLE")
                {
                    //Table模式
                    double Y = 0, x1, y1, x2, y2, m;
                    int i, xcount;
               
                    xcount = Xvalue.Length;
                    //从Xvalue中找X的位置
                    for (i = 0; i < xcount; i++)
                    {
                        x2 = Xvalue[i];
                        if (X <=x2)        //
                        {
                            if (i == 0)     //如果X在左边界外，直接返回Yvalue[0]
                            {
                                Y = Yvalue[0];
                                break;
                            }
                            else
                            {               //否则用两点式计算
                                y2 = Yvalue[i];
                                x1 = Xvalue[i - 1];
                                y1 = Yvalue[i - 1];
                                if (x2 == x1) Y = y2;
                                else
                                {
                                    //m = (y2 - y1) / (x2 - x1);
                                    //b = y1 - m * x1;
                                    //Y = m * X + b;
                                    m = (y2 - y1) / (x2 - x1);
                                    Y = m * (X - x1) + y1;
                                    break;
                                }
                            }
                        }
                    }
                    if (X >= Xvalue[xcount - 1])    //如果X在右边界外。
                        Y = Yvalue[xcount - 1];

                    return Y;               
               
                }
                else if (this.Mode == "FUNCTION")
                {
                    //公式计算法，如果自变量，超出范围，直接给出边界值。
                    if (X <= Xvalue[0]) return Yvalue[0];
                    if (X >= Xvalue[1]) return Yvalue[1];
                    //未超出边界值得情况下，用函数计算
                    return A * X * X + B * X + C;
                }
                else
                {
                    //出现问题，返回一个一般情况下不可能的期望值
                    return -100000; //期望值
                }
            }
            catch (Exception ex)
            {
                //出现问题，返回一个一般情况下不可能的期望值
                return -100000;
            }
            
        }//endFX
    }
    //二维期望曲线
    public class Curve2D
    {
        public string readstatus { get; set; }      //曲线读取状态。
        public int Index { get; set; }              //曲线的序号，可以不连续。比如可以1号机组以1开头，2号机组以2开头
        public DateTime validDate { get; set; }     //曲线生效时间
        public string Name { get; set; }            //曲线的名称，用来调试时，查看曲线是否正确
        public string Mode { get; set; }            //曲线的计算方式，Table：查表方法，Function：函数方法
        public double XLowerLimit { get; set; }     //自变量X考核范围下限
        public double XUpperLimit { get; set; }     //自变量X考核范围上限
        public double YLowerLimit { get; set; }     //自变量Y考核范围下限
        public double YUpperLimit { get; set; }     //自变量Y考核范围上限
        public double[] Xvalue { get; set; }        //查表方式，表的X
        public double[] Yvalue { get; set; }        //查表方式，表的Y
        public double[][] Ztable { get; set; }      //查表方式，表的Z
        public double A1 { get; set; }              //函数方法，Z=A1X^2+B1X+A2Y^2+B2Y+C
        public double B1 { get; set; }              //函数方法，Z=A1X^2+B1X+A2Y^2+B2Y+C
        public double A2 { get; set; }              //函数方法，Z=A1X^2+B1X+A2Y^2+B2Y+C
        public double B2 { get; set; }              //函数方法，Z=A1X^2+B1X+A2Y^2+B2Y+C
        public double C { get; set; }               //函数方法，Z=A1X^2+B1X+A2Y^2+B2Y+C
        //构造函数
        public Curve2D() { }
        //二维期望计算函数
        public double Fx(double X, double Y)
        {
            //在计算时，二维算法只用来计算期望值。所有得分计算，均为一维计算。
            //——采用Table计算期望值的时候，要先判断自变量X和Y是否在Table给定的范围内，如果不在范围内，自动给出边界得分。
            //      比如，X小于给定的X的范围的最小值，则得分Y自动给X最小值对应的得分值（或期望值）
            //      再如，X大于给定的X的范围的最大值，则得分Y自动给Y最大值对应的得分值（或期望值）
            //——采用Function计算期望值或者得分值得时候，也要在Table的位置给定X的最大最小值，已及对应的得分（或者期望值）。在使用Function公式进行计算时，也要先判断X是否在给定的范围内。如果不在范围内，自动给出边界得分。
            //      比如，X小于给定的X的范围的最小值，则得分Y自动给X最小值对应的得分值（或期望值）
            //      再如，X大于给定的X的范围的最大值，则得分Y自动给Y最大值对应的得分值（或期望值）
            //      X在范围内，才会使用公式进行计算。
            //      采用公式进行计算时，Table中给定的边界值与公式计算的边界值要注意一定不能有突变。
            //出现错误返回一个一般情况下不可能的期望值-10000
            try
            {
                if (Mode == "TABLE")
                {
                    double Z = 0, x1, y1, x2, y2, z11, z12, z21, z22;
                    double zx1, zx2, zy1, zy2;
                    int i, xcount, ycount;
                    
                    //自变量X，y所在位置
                    //——这个位置的含义是，x<Xvalue[i],x的位置就是i
                    //——x的位置是0，意思就是x处于（-无穷,Xvalue[0])的范围内。
                    //——x的位置是1，意思就是x处于[Xvalue[0],Xvalue[1]]的范围内。
                    int xindex = 0, yindex = 0;   
                    
                    //获取X所在位置
                    xcount = Xvalue.Length;
                    for (i = 0; i < xcount; i++)
                    {
                        x2 = Xvalue[i];
                        if (x2 >= X)
                        {
                            xindex = i;         
                            break;
                        }
                    }
                    if (X >= Xvalue[xcount - 1])
                        xindex = xcount;

                    //获取Y所在位置
                    ycount = Yvalue.Length;
                    for (i = 0; i < ycount; i++)
                    {
                        y2 = Yvalue[i];
                        if (y2 >= Y)
                        {
                            yindex = i;         
                            break;
                        }
                    }
                    if (Y >= Yvalue[ycount - 1])
                        yindex = ycount;

                    //根据X、Y所在位置判断特殊情况
                    //特例1：x<x0，y<y0,不在表格范围内，z=z[0][0]
                    if (xindex == 0 && yindex == 0)
                        Z = Ztable[0][0];
                    //特例2：x>xn，y>yn,不在表格范围内，z=z[n][n]
                    else if (xindex == xcount && yindex == ycount)
                        Z = Ztable[xcount - 1][ycount - 1];
                    //特例3：x<x0,不在表格范围内，y正常，z取0列，变为一维
                    else if (xindex == 0 && yindex > 0 && yindex < ycount)
                    {
                        x1 = Ztable[yindex - 1][0];
                        x2 = Ztable[yindex][0];
                        y1 = Yvalue[yindex - 1];
                        y2 = Yvalue[yindex];
                        Z = Fy1D(x1, x2, y1, y2, Y);
                    }
                    //特例4：x>xn,不在表格范围内，y正常，z取n列，变为一维
                    else if (xindex > (xcount - 1) && yindex > 0 && yindex < ycount)
                    {
                        x1 = Ztable[yindex - 1][xcount - 1];
                        x2 = Ztable[yindex][xcount - 1];
                        y1 = Yvalue[yindex - 1];
                        y2 = Yvalue[yindex];
                        Z = Fy1D(x1, x2, y1, y2, Y);
                    }
                    //特例5：x正常，y<y0，不在表格范围内，z取0行，变为一维
                    else if (yindex == 0 && xindex > 0 && xindex < xcount)
                    {
                        x1 = Xvalue[xindex - 1];
                        x2 = Xvalue[xindex];
                        y1 = Ztable[0][xindex - 1];
                        y2 = Ztable[0][xindex];
                        Z = Fx1D(x1, x2, y1, y2, X);
                    }
                    //特例6：x正常，y>yn，不在表格范围内，z取n行，变为一维
                    else if (yindex > (ycount - 1) && xindex > 0 && xindex < xcount)
                    {
                        x1 = Xvalue[xindex - 1];
                        x2 = Xvalue[xindex];
                        y1 = Ztable[ycount - 1][xindex - 1];
                        y2 = Ztable[ycount - 1][xindex];
                        Z = Fx1D(x1, x2, y1, y2, X);
                    }
                    //x正常，y正常，z正常计算
                    else
                    {
                        x1 = Xvalue[xindex - 1];
                        x2 = Xvalue[xindex];
                        y1 = Yvalue[yindex - 1];
                        y2 = Yvalue[yindex];
                        z11 = Ztable[yindex - 1][xindex - 1];
                        z12 = Ztable[yindex - 1][xindex];
                        z21 = Ztable[yindex][xindex - 1];
                        z22 = Ztable[yindex][xindex];
                        zx1 = Fx1D(x1, x2, z11, z12, X);
                        zx2 = Fx1D(x1, x2, z21, z22, X);
                        zy1 = Fx1D(y1, y2, z11, z21, Y);
                        zy2 = Fx1D(y1, y2, z12, z22, Y);        //zy2没有用。
                        Z = Fx1D(z11, z21, zx1, zx2, zy1);
                    }
                    return Z;
                }
                else if (Mode == "FUNCTION")
                {
                    int i, xcount, ycount;

                    xcount = Xvalue.Length;
                    ycount = Yvalue.Length;

                    //公式计算法，如果自变量，超出范围，按边界值来带入公式
                    if (X < Xvalue[0])
                        X = Xvalue[0];
                    else if (X > Xvalue[xcount - 1])
                        X = Xvalue[xcount - 1];

                    if (Y < Yvalue[0])
                        Y = Yvalue[0];
                    else if (Y > Yvalue[ycount - 1])
                        Y = Yvalue[ycount - 1];
                    return A1 * X * X + B1 * X + A2 * Y * Y + B2 * Y + C;
                }
                else
                {
                    return -100000;
                }
            }
            catch (Exception ex)
            {
                return -100000;
            }

            
        }

        private double Fx1D(double x1, double x2, double y1, double y2, double x)
        {
            double y = 0;
            if (x1 == x2)
                y = y2;
            else
                y = (x - x1) / (x2 - x1) * (y2 - y1) + y1;
            return y;
        }
        private double Fy1D(double x1, double x2, double y1, double y2, double y)
        {
            double x;
            if (y1 == y2)
                x = x2;
            else
                x = (y - y1) / (y2 - y1) * (x2 - x1) + x1;
            return x;
        }
    }

    public class CurveConfig
    {
        //偏差曲线所在文件夹
        const string ConfigPath = "CurveConfig";           
        //期望曲线和记分曲线都是一维曲线，因此，每个文件中包含多条曲线
        const string DEV1DCURVE_FILE = "BiasCurve_1D";      //一维偏差曲线前缀
        const string DEV2DCURVE_FILE = "BiasCurve_2D";      //二维偏差曲线前缀
        const string SCORECURVE_FILE = "ScoreCurve";        //一维计分曲线前缀               

        //注意二维曲线存放在统一目录CurveConfig下，一个文件是一条曲线，因此要遍历目录
        private string _filePath;
        private string _fileNamePre;
        private string _readstatus;

        //期望曲线和得分曲线的静态对象
        private static CurveConfig _instance;
        public static CurveConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CurveConfig();
                return _instance;
            }//get
        }
        public List<PSLCalcu.Module.Curve1D> OPXCurves { get; set; }        //CurveConfig的静态实例Instance下的期望曲线列表
        public List<PSLCalcu.Module.Curve2D> OPXCurves2D { get; set; }      //CurveConfig的静态实例Instance下的二维期望曲线列表
        public List<PSLCalcu.Module.Curve1D> ScoreCurves { get; set; }      //CurveConfig的静态实例Instance下的得分曲线列表 

        //读取状态
        public string OPXCurvesReadStatus = "";             //用于记录Load1DConfig中的执行情况
        public string OPXCurves2DReadStatus = "";           //用于记录Load2DConfig中的执行情况
        public string ScoreCurvesReadStatus = "";           //用于记录Load1DConfig中的执行情况

        //**************用静态方式获得偏差曲线和得分曲线**********************************
        //——在每次运行计算引擎，第一次使用Curveconfig时进行初始化。之后就不再读取数据
        //——在算法中的调用方式为Curve1D opxCurve = CurveConfig.Instance.OPXCurves.Find(delegate(Curve1D a) { return a.Index == opxid; });
        public CurveConfig()
        {
            //读取一维偏差期望曲线
            _filePath = Environment.CurrentDirectory + "\\" + ConfigPath + "\\";
            _fileNamePre = DEV1DCURVE_FILE;
            List<Curve1D> curvesOPX = new List<Curve1D>();

            _readstatus = Load1DConfig(_filePath, _fileNamePre, DateTime.Now, ref curvesOPX);

            this.OPXCurvesReadStatus = String.Format("一维期望曲线读取状态：{0}", _readstatus);
            this.OPXCurves = curvesOPX;
            
            //读取二维偏差期望曲线
            _filePath = Environment.CurrentDirectory + "\\" + ConfigPath + "\\";
            _fileNamePre = DEV2DCURVE_FILE;
            List<Curve2D> curvesOPX2D = new List<Curve2D>(); ;

            _readstatus = Load2DConfig(_filePath, _fileNamePre, DateTime.Now, ref curvesOPX2D);
            this.OPXCurves2DReadStatus = String.Format("二维期望曲线读取状态：{0}", _readstatus);
            this.OPXCurves2D = curvesOPX2D;
            
            //读取得分期望曲线
            _filePath = Environment.CurrentDirectory + "\\" + ConfigPath + "\\" ;
            _fileNamePre = SCORECURVE_FILE;
            List<Curve1D> curvescore = new List<Curve1D>(); ;

            _readstatus = Load1DConfig(_filePath, _fileNamePre, DateTime.Now, ref curvescore);
            this.ScoreCurvesReadStatus = String.Format("得分曲线读取状态：{0}", _readstatus);
            this.ScoreCurves = curvescore;
        }

        //***************静态方式下，获取相应Index和有效起始时间的曲线
        public static Curve1D GetOPXCurve(int Index, DateTime calcuDatetime)
        {
            try
            {
                Curve1D opxcurve = new Curve1D();
                List<Curve1D> opxcurves = _instance.OPXCurves.FindAll(delegate(Curve1D a) { return a.Index == Index && a.validDate <= calcuDatetime; });    //注意，这里找小于等于的曲线配置
                if (opxcurves != null && opxcurves.Count != 0)
                {
                    opxcurve=opxcurves.OrderByDescending(a => a.validDate).First();
                    //opxcurve = opxcurves[opxcurves.Count - 1];
                    return opxcurve;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static Curve2D GetOPXCurve2D(int Index, DateTime calcuDatetime)
        {
            try
            {
                Curve2D opxcurve2d = new Curve2D();
                List<Curve2D> opxcurves2d = _instance.OPXCurves2D.FindAll(delegate(Curve2D a) { return a.Index == Index && a.validDate <= calcuDatetime; });
                if (opxcurves2d != null && opxcurves2d.Count != 0)
                {
                    opxcurve2d = opxcurves2d.OrderByDescending(a => a.validDate).First();
                    //opxcurve2d = opxcurves2d[opxcurves2d.Count - 1];                    
                    return opxcurve2d;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static Curve1D GetScoreCurve(int Index, DateTime calcuDatetime)
        {
            try
            {
                Curve1D scorecurve = new Curve1D();
                List<Curve1D> scorecurves = _instance.ScoreCurves.FindAll(delegate(Curve1D a) { return a.Index == Index && a.validDate <= calcuDatetime; });
                if (scorecurves != null && scorecurves.Count != 0)
                {
                    scorecurve = scorecurves.OrderByDescending(a => a.validDate).First();
                    //scorecurve = scorecurves[scorecurves.Count - 1];
                    return scorecurve;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        //***************用动态方式获得偏差曲线和得分曲线************************************
        //——在算法每次被调用时，都从CSV读取一次配置文件
        //——在算法中的调用方式为Curve1D opxCurve = CurveConfig.ReadOPXCurves.Find(delegate(Curve1D a) { return a.Index == opxid; });
        //——20181031，因动态方式读取曲线，速度太慢，不在采用。
        //——转而采用在静态方式下读取带生效时间的配置曲线。这样在增加配置曲线时，仅需要暂停计算引擎后重启，即可载入新的配置曲线，并继续进行计算。
        /*
        public static List<PSLCalcu.Module.Curve1D> ReadOPXCurves(DateTime readdate)
        {
            //读取一维偏差期望曲线
            string filePath = Environment.CurrentDirectory + "\\" + ConfigPath + "\\";
            string fileNamePre = DEV1DCURVE_FILE;
            List<Curve1D> curvesOPX = new List<Curve1D>();
            Load1DConfig(filePath, fileNamePre, readdate, ref curvesOPX);           
            
            return curvesOPX;
        }
        public static List<PSLCalcu.Module.Curve2D> ReadOPXCurves2D(DateTime readdate)
        {
            //读取二维偏差期望曲线
            string fileName = Environment.CurrentDirectory + "\\" + ConfigPath + "\\";
            string fileNamePre = DEV2DCURVE_FILE;
            List<Curve2D> curvesOPX2D = new List<Curve2D>();
            Load2DConfig(fileName, fileNamePre, readdate, ref curvesOPX2D);
            return curvesOPX2D;
        }
        public static List<PSLCalcu.Module.Curve1D> ReadScoreCurves(DateTime readdate)
        {            
            //读取一维得分期望曲线
            string filePath = Environment.CurrentDirectory + "\\" + ConfigPath + "\\" ;
            string fileNamePre = SCORECURVE_FILE;
            List<Curve1D> curvesscore = new List<Curve1D>();
            Load1DConfig(filePath, fileNamePre, readdate, ref curvesscore);
            return curvesscore;
        }
        */
   
        #region 静态加载和动态加载两种方式的公用载入方法
        //加载一维曲线文件，包括一维偏差曲线和得分曲线        
        private static string Load1DConfig(string path,string filepre, DateTime readdate,ref List<PSLCalcu.Module.Curve1D> curves)
        {
            string readStatus="";
            
            int i, j;
            
            //首先寻找合适的文件
            FileSystemInfo info;
            info = new DirectoryInfo(path);                         //查询路径文件夹是否存在
            if (!info.Exists)                                       //路径文件夹不存在，返回空的curve
            {
                curves = null;
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}不存在！", path);
                return readStatus;
            }
            else
            {
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}存在！",path);
            }
            DirectoryInfo dir = info as DirectoryInfo;              //如果路径文件夹存在，则获取文件夹对象
            if (dir == null)                                        //如果路径文件夹对象为null，则返回空的curve
            {
                curves=null;
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}中没有任何文件！",path);
                return readStatus; 
            }


            readStatus = readStatus + Environment.NewLine + String.Format("在文件夹{0}中寻找符合条件的配置文件！",path);
            FileSystemInfo[] files = dir.GetFileSystemInfos();      //获取路径文件夹下所有文件

            List<string> filenames=new List<string>();              //记录所有符合条件的文件名
            bool findflag=false;                                    //找到标记
            for (i = 0; i < files.Length; i++)                      //对文件夹下所有文件进行遍历，找到文件名称中符合条件的文件名称
            {
                try
                {
                    readStatus = readStatus + Environment.NewLine + String.Format("——解析文件名{0}！", files[i].Name);
                    if (filepre.ToUpper() == "BIASCURVE_1D")
                    {
                        string[] fields = new string[3];
                        //文件名称必须由3部分构成，如BiasCurve_1D_2018-05-06_0200.csv。files[0]是曲线类别，files[1]曲线维度，files[2]曲线有效时间
                        fields = files[i].Name.Split('_');
                        string curvetype = fields[0].ToUpper();       //曲线类型
                        string curvedime = fields[1].ToUpper();       //曲线维度
                        DateTime filedate = DateTime.Parse(fields[2] + " " + fields[3].Substring(0, 2) + ":" + fields[3].Substring(2, 2));    //曲线时间
                        string filetype = fields[3].Split('.')[1];
                        if (filetype.ToUpper() == "CSV" &&                                              //必须是csv文件
                            (curvetype.ToUpper() + "_" + curvedime.ToUpper()) == filepre.ToUpper()      //必须是filepre文件，即BIASCURVE_1D
                            )
                        //filedate < readdate)                                                          //必须是曲线时间小于当前读取时间。20181101去掉该条件
                        {
                            filenames.Add(files[i].Name);
                            findflag = true;                            
                        }
                    }
                    else if (filepre.ToUpper() == "SCORECURVE")
                    {
                        string[] fields = new string[3];
                        //文件名称必须由3部分构成，如BiasCurve_1D_2018-05-06_0200.csv
                        fields = files[i].Name.Split('_');
                        string curvetype = fields[0].ToUpper();             //曲线类型
                        //string curvedime = fields[1].ToUpper();           //曲线维度
                        DateTime filedate = DateTime.Parse(fields[1] + " " + fields[2].Substring(0, 2) + ":" + fields[2].Substring(2, 2));    //曲线时间
                        string filetype = fields[2].Split('.')[1];
                        if (filetype.ToUpper() == "CSV" &&                  //必须是csv文件
                            curvetype.ToUpper() == filepre.ToUpper()        //必须是filepre文件，即SCORECURVE                          
                            )
                        //filedate < readdate)                          //时间必须小于当前读取时间.20181101去掉该条件
                        {
                            filenames.Add(files[i].Name);
                            findflag=true;  
                        }
                    }
                }
                catch (Exception ex)
                {
                    readStatus = readStatus + Environment.NewLine + String.Format("————文件名称解析错误！");
                    continue;   //文件格式有误，直接下一个文件夹
                }

            }

            if (findflag == false)
            {
                curves = null;
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}中没有找到文件名前缀和有效时间均符合条件的曲线配置文件！",path);
                return readStatus;
            }
            else
            {
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}中已经找到文件名前缀和有效时间均符合条件的曲线配置文件！", path);
            }

            //从找到的文件中读取数据
            bool readflag = false;
            for (int ifiles = 0; ifiles < filenames.Count; ifiles++)
            {
                string filename = path + filenames[ifiles];

                string[] fields = filenames[ifiles].Split('_');
                DateTime filedate = new DateTime();
                if(fields[0].Trim().ToUpper()+"_"+fields[1].Trim().ToUpper()=="BIASCURVE_1D")     //这里选出的一定是BIASCURVE_1D为前缀的文件
                {
                    filedate= DateTime.Parse(fields[2] + " " + fields[3].Substring(0, 2) + ":" + fields[3].Substring(2, 2));    //曲线时间
                }
                else if (fields[0].Trim().ToUpper() == "SCORECURVE")
                {
                    filedate = DateTime.Parse(fields[1] + " " + fields[2].Substring(0, 2) + ":" + fields[2].Substring(2, 2));    //曲线时间
                }
                //开始解析文件，读取Curve1D对象
                //——无论动态读取还是静态读取，任何时候发生错误，将返回null。
                //——偏差算法中，在使用前先判断是否为null。如果为空，偏差算法会报曲线错误。

                PSLCalcu.Module.Curve1D cur;
                readStatus = readStatus + Environment.NewLine + String.Format("解析配置文件{0}中的数据！", filename);

                string[][] filedata = CsvFileReaderForModule.Read(filename);
                
                for (i = 0; i < filedata.Length; i++)
                {
                    try
                    {
                        if (filedata[i][0].Substring(0, 1) == "[")      //数据是逐行扫描的，当遇到改行第一个单元格是以'['开头，则表明这是一个曲线配置的起始位置
                        {
                            cur = new PSLCalcu.Module.Curve1D();
                            cur.Name = filedata[i][0].Substring(1, filedata[i][0].Length - 2);      //第1行，第一个单元格是[曲线名称]
                            cur.Index = int.Parse(filedata[i][1]);                                  //第1行，第二个单元个是曲线序号
                            cur.validDate = filedate;  //曲线有效时间
                            //Console.WriteLine("content:"+filedata[i][2].Trim());
                            cur.XLowerLimit = double.Parse(filedata[i][2].Trim());                  //第1行，第三个单元格是自变量X的下限
                            cur.XUpperLimit = double.Parse(filedata[i][3].Trim());                  //第1行，第四个单元格是自变量X的上限
                            cur.Mode = filedata[i][4].Trim().ToUpper();                             //第1行，第五个单元格是曲线配置模式。是数据表型Table，还是函数型Function


                            if (cur.Mode != "FUNCTION" && cur.Mode != "TABLE")
                            {
                                readStatus = readStatus + Environment.NewLine + String.Format("——配置模式参数不正确！模式参数必须是'FUNCTION'或'TABLE'");
                                //curves.Add(null); //参数不正确不能加入null，否则后面读取曲线时，curves中有空对象，会报错
                                i = i + 2;
                                continue;
                            }
                            else
                            {
                                readStatus = readStatus + Environment.NewLine + String.Format("————配置模式为{0}", cur.Mode);
                            }
                            //读Table模式下，表的X和Y
                            //——无论Table模式，或者Function模式，都要读Table。
                            //——Table模式，需要用Table来判断有效边界，并计算
                            //——Function模式，需要用Table来判断边界。
                            cur.Xvalue = new double[filedata[i + 1].Length];
                            cur.Yvalue = new double[filedata[i + 1].Length];
                            for (j = 0; j < filedata[i + 1].Length; j++)
                            {
                                if (filedata[i + 1][j].Trim() != "" && filedata[i + 2][j].Trim() != "")
                                {
                                    cur.Xvalue[j] = double.Parse(filedata[i + 1][j].Trim());
                                    cur.Yvalue[j] = double.Parse(filedata[i + 2][j].Trim());
                                }
                                else
                                {
                                    //遇到空字符，停止读取，重新截取数组长度。注意这里必须截取。偏差计算和得分计算，都要使用边界数据。
                                    cur.Xvalue = cur.Xvalue.Take(j).ToArray();
                                    cur.Yvalue = cur.Yvalue.Take(j).ToArray();
                                }
                            }
                            //修正上下限
                            //修正上下限
                            //要注意，防止偏差曲线的x不是从小到大。(但是，目前硬性要求期望曲线和得分曲线配置，范围要从低到高。因此老版本上，没有这个判断)
                            double xmin = cur.Xvalue[0] < cur.Xvalue[cur.Xvalue.Length - 1] ? cur.Xvalue[0] : cur.Xvalue[cur.Xvalue.Length - 1];
                            double xmax = cur.Xvalue[cur.Xvalue.Length - 1] > cur.Xvalue[0] ? cur.Xvalue[cur.Xvalue.Length - 1] : cur.Xvalue[0];
                            if (cur.XLowerLimit < xmin) cur.XLowerLimit = xmin;           //必须保证有效考核范围[XLowerLimit,XUpperLimit]范围要小于或等于cur.Xvalue范围。
                            if (cur.XUpperLimit > xmax) cur.XUpperLimit = xmax;

                            readStatus = readStatus + Environment.NewLine + String.Format("——X和Y读取完毕");
                            //读Function模式下，A，B，C
                            if (cur.Mode == "FUNCTION")
                            {
                                cur.A = double.Parse(filedata[i][5].Trim());   //第1行，第6个单元格是Funtion模式下的拟合参数A
                                cur.B = double.Parse(filedata[i][6].Trim());   //第1行，第7个单元格是Funtion模式下的拟合参数B
                                cur.C = double.Parse(filedata[i][7].Trim());   //第1行，第8个单元格是Funtion模式下的拟合参数B
                            }
                            readStatus = readStatus + Environment.NewLine + String.Format("——参数ABC读取完毕");
                            curves.Add(cur);
                            readStatus = readStatus + Environment.NewLine + String.Format("——添加曲线{0}", cur.Name);

                            readflag = true;
                            i = i + 2;
                        }

                    }
                    catch (Exception ex)
                    {
                        //String msgStr = String.Format("初始化一维偏差/得分曲线时出错！请检查");
                        //MessageBox.Show(msgStr);
                        //ErrorFlag = true;
                        //ErrorInfo = ex.ToString();
                        //return null;        //无论动态读取还是静态读取，任何时候发生错误，将返回null。
                        //偏差算法中，在使用前先判断是否为null。如果为空，偏差算法会报曲线错误。
                        readStatus = readStatus + Environment.NewLine + String.Format("——文件数据解析错误！详细信息：{0}", ex.ToString());
                        //curves.Add(null);//参数不正确不能加入null，否则后面读取曲线时，curves中有空对象，会报错
                    }
                }//end for 遍历数据
            }//end for 遍历文件
            if (readflag)
            {
                readStatus = readStatus+String.Format("配置曲线读取完毕！");
                return readStatus;
            }
            else
            {
                readStatus = readStatus + Environment.NewLine + String.Format("在文件中没有读取到任何有效的曲线配置数据！");
                return readStatus;
            }
        }
        //加载二维曲线，就是二维偏差曲线
        private static string Load2DConfig(string path, string filepre, DateTime readdate,ref List<PSLCalcu.Module.Curve2D> curves)
        {
            string readStatus="";
            int i, j, k;            

            //首先寻找合适的文件
            FileSystemInfo info;
            info = new DirectoryInfo(path);                         //查询路径文件夹是否存在
            if (!info.Exists)                                       //路径文件夹不存在，返回空的curve
            {
                curves = null;
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}不存在！", path);
                return readStatus;
            }
            else
            {                
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}存在！", path);                
            }
            DirectoryInfo dir = info as DirectoryInfo;              //如果路径文件夹存在，则获取文件夹对象
            if (dir == null)                                        //如果路径文件夹对象为空，则返回空的curve
            {
                curves = null;
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹中没有任何文件！");
                return readStatus; 
            }


            List<string> filenames = new List<string>();              //记录所有符合条件的文件名
            readStatus = readStatus + Environment.NewLine + String.Format("在文件夹{0}中寻找符合条件的配置文件！",path);
            FileSystemInfo[] files = dir.GetFileSystemInfos();      //获取路径文件夹下所有文件
            
            bool findflag = false;

            for (i = 0; i < files.Length; i++)
            {
                try
                {
                    readStatus = readStatus + Environment.NewLine + String.Format("——解析文件名{0}！", files[i].Name);
                    string[] fields = new string[6];
                    //文件名称必须由6部分构成，如BiasCurve_2D_CurveName_2_2018-01-01_0200.csv
                    fields = files[i].Name.Split('_');                    
                    string curvetype = fields[0].ToUpper();       //曲线类型
                    string curvedime = fields[1].ToUpper();       //曲线维度
                    string curvename = fields[2];                 //曲线名称
                    int curveindex = int.Parse(fields[3]);        //曲线序号
                    DateTime filedate = DateTime.Parse(fields[4] + " " + fields[5].Substring(0, 2) + ":" + fields[5].Substring(2, 2));    //曲线时间
                    string filetype = fields[5].Split('.')[1].ToUpper();  //文件类型

                    if (filetype == "CSV" &&                                        //必须是CSV文件
                        (curvetype + "_" + curvedime) == filepre.ToUpper()          //文件名前缀必须是filepre，这里是BiasCurve_2D
                        )
                        //filedate < readdate)                                      //必须比当前读取时间早
                    {
                        filenames.Add(files[i].Name);
                        findflag =true;
                    }
                }
                catch (Exception ex)
                {
                    readStatus = readStatus + Environment.NewLine + String.Format("————文件名称解析错误！");
                    continue;   //文件格式有误，直接下一个文件夹
                }

            }

            if (findflag == false)
            {
                curves = null;
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}中没有找到文件名前缀和有效时间均符合条件的曲线配置文件！",path);
                return readStatus;
            }
            else
            {
                readStatus = readStatus + Environment.NewLine + String.Format("路径文件夹{0}已经找到文件名前缀和有效时间均符合条件的曲线配置文件！",path);
            }

            //从找到的文件中读取数据
            bool readflag = false;
            for (int ifiles = 0; ifiles < filenames.Count; ifiles++)
            {
                string filename = path + filenames[ifiles];

                string[] fields = filenames[ifiles].Split('_');
                DateTime filedate = DateTime.Parse(fields[4] + " " + fields[5].Substring(0, 2) + ":" + fields[5].Substring(2, 2));    //曲线时间。//文件名称必须由6部分构成，如BiasCurve_2D_CurveName_2_2018-01-01_0200.csv
                //开始解析文件，读取Curve2D对象
                //——假设files是已经选取好的所有2D文件
                //——无论动态读取还是静态读取，任何时候发生错误，将返回null。
                //——偏差算法中，在使用前先判断是否为null。如果为空，偏差算法会报曲线错误。

                PSLCalcu.Module.Curve2D cur = new PSLCalcu.Module.Curve2D();               
                string[][] filedata;

                try
                {
                    filedata = CsvFileReaderForModule.Read(filename);

                    readStatus = readStatus + Environment.NewLine + String.Format("——解析配置文件{0}的数据！", filename);  
                  
                    cur.Name = filedata[0][0].Substring(1, filedata[0][0].Length - 2);      //第1行，第一个单元格是[曲线名称]
                    cur.Index = int.Parse(filedata[0][1]);                                  //第1行，第二个单元个是曲线序号
                    cur.validDate = filedate;                                               //有效时间
                    cur.XLowerLimit = double.Parse(filedata[0][2].Trim());                  //第1行，第三个单元格是自变量X的下限
                    cur.XUpperLimit = double.Parse(filedata[0][3].Trim());                  //第1行，第四个单元格是自变量X的上限
                    cur.YLowerLimit = double.Parse(filedata[0][4].Trim());                  //第1行，第五个单元格是自变量Y的下限
                    cur.YUpperLimit = double.Parse(filedata[0][5].Trim());                  //第1行，第六个单元格是自变量Y的上限
                    cur.Mode = filedata[0][6].Trim().ToUpper();                             //第1行，第七个单元格是曲线模式

                    if (cur.Mode != "FUNCTION" && cur.Mode != "TABLE")
                    {
                        readStatus = readStatus + Environment.NewLine + String.Format("————配置模式参数不正确！模式参数必须是'FUNCTION'或'TABLE'");
                        //curves.Add(null);//参数不正确不能加入null，否则后面读取曲线时，curves中有空对象，会报错
                        continue;
                    }
                    else
                    {
                        readStatus = readStatus + Environment.NewLine + String.Format("————配置模式为{0}", cur.Mode);
                    }
                    int _rowcount = filedata.Length;
                    int _colcount = filedata[1].Length;            //第1行，是X
                    //读Table模式下，表的X和Y
                    //填充Xvalue
                    cur.Xvalue = new double[_colcount - 1];
                    for (i = 1; i < _colcount; i++)
                    {
                        if (filedata[1][i] != "")
                        {
                            cur.Xvalue[i - 1] = double.Parse(filedata[1][i]);
                        }
                        else
                        {
                            //遇到空字符，停止读取，重新截取数组长度。注意这里必须截取。偏差计算和得分计算，都要使用边界数据。
                            cur.Xvalue = cur.Xvalue.Take(i - 1).ToArray();
                        }
                    }
                    readStatus = readStatus + Environment.NewLine + String.Format("————X读取完毕");
                    //填充Yvalue                                 //第0列，是Y
                    cur.Yvalue = new double[_rowcount - 2];
                    cur.Ztable = new double[_rowcount - 2][];
                    for (i = 2; i < _rowcount; i++)
                    {
                        if (filedata[i][0] != "")
                        {
                            cur.Yvalue[i - 2] = double.Parse(filedata[i][0]);
                            cur.Ztable[i - 2] = new double[cur.Xvalue.Length];
                        }
                    }
                    readStatus = readStatus + Environment.NewLine + String.Format("————“Y读取完毕");
                    //填充Ztable                     
                    for (i = 2; i < _rowcount; i++)
                    {
                        for (j = 1; j < _colcount; j++)
                        {
                            if (filedata[i][j] != "")
                            {
                                cur.Ztable[i - 2][j - 1] = double.Parse(filedata[i][j]);
                            }
                        }
                    }
                    readStatus = readStatus + Environment.NewLine + String.Format("————Z读取完毕");

                    //修正上下限
                    //要注意，防止偏差曲线的x不是从小到大。(但是，目前硬性要求期望曲线和得分曲线配置，范围要从低到高。因此老版本上，没有这个判断)
                    double xmin = cur.Xvalue[0] < cur.Xvalue[cur.Xvalue.Length - 1] ? cur.Xvalue[0] : cur.Xvalue[cur.Xvalue.Length - 1];
                    double xmax = cur.Xvalue[cur.Xvalue.Length - 1] > cur.Xvalue[0] ? cur.Xvalue[cur.Xvalue.Length - 1] : cur.Xvalue[0];
                    if (cur.XLowerLimit < xmin) cur.XLowerLimit = xmin;           //必须保证有效考核范围[XLowerLimit,XUpperLimit]范围要小于或等于cur.Xvalue范围。
                    if (cur.XUpperLimit > xmax) cur.XUpperLimit = xmax;
                    double ymin = cur.Yvalue[0] < cur.Yvalue[cur.Yvalue.Length - 1] ? cur.Yvalue[0] : cur.Yvalue[cur.Yvalue.Length - 1];
                    double ymax = cur.Yvalue[cur.Yvalue.Length - 1] > cur.Yvalue[0] ? cur.Yvalue[cur.Yvalue.Length - 1] : cur.Yvalue[0];
                    if (cur.YLowerLimit < ymin) cur.YLowerLimit = ymin;           //必须保证有效考核范围[XLowerLimit,XUpperLimit]范围要小于或等于cur.Xvalue范围。
                    if (cur.YUpperLimit > ymax) cur.YUpperLimit = ymax;
                    //读Function模式下，A，B，C，从第0行，第1列开始
                    if (cur.Mode == "FUNCTION")
                    {
                        cur.A1 = double.Parse(filedata[0][7].Trim());   //第0行，第8个单元格是Funtion模式下的拟合参数A
                        cur.B1 = double.Parse(filedata[0][8].Trim());   //第0行，第9个单元格是Funtion模式下的拟合参数B
                        cur.A2 = double.Parse(filedata[0][9].Trim());   //第0行，第10个单元格是Funtion模式下的拟合参数A
                        cur.B2 = double.Parse(filedata[0][10].Trim());   //第0行，第11个单元格是Funtion模式下的拟合参数B
                        cur.C = double.Parse(filedata[0][11].Trim());    //第0行，第12个单元格是Funtion模式下的拟合参数C
                    }
                    readStatus = readStatus + Environment.NewLine + String.Format("————参数A1B1A2B2C读取完毕");
                    curves.Add(cur);
                    readStatus = readStatus + Environment.NewLine + String.Format("——添加曲线{0}", cur.Name);
                    readflag = true;
                }
                catch (Exception ex)
                {
                    //String msgStr = String.Format("初始化二维偏差曲线时出错！请检查...");
                    //MessageBox.Show(msgStr);
                    //ErrorFlag = true;
                    //ErrorInfo = ex.ToString();
                    //return null;            //无论动态读取还是静态读取，任何时候发生错误，将返回null。
                    //偏差算法中，在使用前先判断是否为null。如果为空，偏差算法会报曲线错误。
                    readStatus = readStatus + Environment.NewLine + String.Format("——文件数据解析错误！详细信息：{0}", ex.ToString());
                    //curves.Add(null);//参数不正确不能加入null，否则后面读取曲线时，curves中有空对象，会报错
                    continue;
                }
              
            }//end for 遍历文件

            if (curves.Count != 0 && readflag==true)
            {
                readStatus = readStatus+String.Format("配置曲线读取完毕！"); //如果一切读取顺利，则readStatus前面的记录均不记录。直接赋予最后的值。
                return readStatus;
            }
            else
            {               
                curves = null;
                readStatus = readStatus + Environment.NewLine + readStatus + String.Format("在配置文件内没有读取到任何有效的曲线配置数据！");
                return readStatus;
            }

        }
        #endregion 
    }
}
