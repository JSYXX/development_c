using Accord.Statistics.Models.Regression.Linear;
using PCCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    /// <summary>
    /// 提供一些具体的计算函数
    /// </summary>
    public class BaseCalcu
    {
        static LogHelper logHelper = LogFactory.GetLogger(typeof(BaseCalcu));                     //全局log
        struct Point : IComparable<Point>
        {
            public int pointNum;
            public double value;
            public int CompareTo(Point other)
            {
                return this.value.CompareTo(other.value);
            }
        }
        /// <summary>
        /// 获取输入数据的最大值  最大值点号 最小值 最小值点号
        /// </summary>
        /// <returns></returns>
        public static List<PValue> getMaxMin(List<PValue> input) {
            List<Point> points = new List<Point>();
            List<PValue> list = new List<PValue>();
            int length = input.Count;
            for (int i = 0; i < length; i++)
            {
                Point p;
                p.pointNum = i + 1;
                p.value = input[i].Value;
                points.Add(p);
            }
            PValue max = new PValue();
            PValue min = new PValue();
            PValue maxN = new PValue();
            PValue minN = new PValue();
            max.Value = points.Max().value;
            maxN.Value = points.Max().pointNum;
            min.Value = points.Min().value;
            minN.Value = points.Min().pointNum;
            list.Add(max);
            list.Add(maxN);
            list.Add(min);
            list.Add(minN);
            return list;
        }
        /// <summary>
        /// 求输入数据的最大值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PValue getMax(List<PValue> input) {
            return input.Max();
        }
        /// <summary>
        /// 求输入数据的最小值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PValue getMin(List<PValue> input) {
            return input.Min();
        }
        /// <summary>
        /// 求输入数据的平均值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PValue getAvg(List<PValue> input) {
            PValue avg=new PValue();
            double sum = 0.0;
            for (int i = 0; i < input.Count; i++) {
                sum = sum + input[i].Value;
            }
            avg.Value = sum / input.Count;
            return avg;
        }
        /// <summary>
        /// 对输入数据求和
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PValue getSum(List<PValue> input) {
            PValue Sum = new PValue();
            double sum = 0.0;
            for (int i = 0; i < input.Count; i++)
            {
                sum = sum + input[i].Value;
            }
            Sum.Value = sum;
            return Sum;
        }
        /// <summary>
        /// 对输入数据的绝对值求和
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PValue getAbsSum(List<PValue> input)
        {
            PValue Sum = new PValue();
            double sum = 0.0;
            for (int i = 0; i < input.Count; i++)
            {
                sum = sum + Math.Abs(input[i].Value);
            }
            Sum.Value = sum;
            return Sum;
        }
        /// <summary>
        /// 求凹凸点  返回为list  其中有四个元素 最凸点 点号  最凹点 点号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<PValue> getTopDown(List<PValue> input)
        {
            List<PValue> vals = new List<PValue>();
            List<Point> points = new List<Point>();
            PValue top = new PValue();
            PValue topN = new PValue();
            PValue down = new PValue();
            PValue downN = new PValue();
            int length=input.Count;
            if (length > 1)
            {
                for (int i = 0; i < length; i++)
                {
                    //第一个
                    if (i == 0)
                    {
                        double val = input[0].Value - input[1].Value;
                        Point p1 = new Point();
                        p1.pointNum = (i + 1);
                        p1.value = val;
                        points.Add(p1);
                    }
                    else if (i == (length - 1))  //最后一个
                    {
                        double val = input[length - 1].Value - input[length - 2].Value;
                        Point p1 = new Point();
                        p1.pointNum = (i + 1);
                        p1.value = val;
                        points.Add(p1);
                    }
                    else
                    {
                        double val = ((input[i].Value - input[i - 1].Value) + (input[i].Value - input[i + 1].Value)) / 2;
                        Point p1 = new Point();
                        p1.pointNum = (i + 1);
                        p1.value = val;
                        points.Add(p1);
                    }
                }
                top.Value = points.Max().value;
                topN.Value = points.Max().pointNum;
                down.Value = points.Min().value;
                downN.Value = points.Min().pointNum;
            }
            else {
                top.Status = 100;
                topN.Status = 100;
                down.Status = 100;
                downN.Status = 100;
            }

           
            vals.Add(top);
            vals.Add(topN);
            vals.Add(down);
            vals.Add(downN);
            return vals;
        }
        /// <summary>
        /// 获取输入数据的翻转次数
        /// </summary>
        /// <returns></returns>
        public static PValue getTurnN(List<PValue> input) {
            PValue val = new PValue();
            double pre;
            double turnNum = 0;
            //两个后值减前值  相乘为负数  则为翻转一次
            for (int i = 0; i < input.Count; i++) {
                if (i >= 1 && i < input.Count-1)
                {
                    pre = input[i].Value - input[i - 1].Value;
                    if (pre * (input[i + 1].Value - input[i].Value) < 0) {
                        turnNum = turnNum + 1;
                    }
                }
            }
            val.Value = turnNum;
            return val;
        }
        /// <summary>
        /// 获取输入数据的越高限点数 越低限点数  越高限点数占比  越低限点数占比
        /// </summary>
        /// <param name="input"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static List<PValue> getHighLow(List<PValue> input,Double high,Double low)
        {
            double highNum=0;  //越高限点数
            double lowNum=0;   //越低限点数
            List<PValue> list = new List<PValue>();
            PValue highN = new PValue();
            PValue lowN = new PValue();
            PValue highR = new PValue();
            PValue lowR = new PValue();
            int length=input.Count;
            for (int i = 0; i < length; i++) {
                if (input[i].Value >= high)
                {
                    highNum = highNum + 1;
                }
                else {
                    lowNum = lowNum + 1;
                }
            }
            highN.Value = highNum;
            lowN.Value = lowNum;
            highR.Value = highNum / length;
            lowR.Value = lowNum / length;
            list.Add(highN);
            list.Add(lowN);
            list.Add(highR);
            list.Add(lowR);
            return list;
        }
        /// <summary>
        /// 获取输入数据的中位值  及 中位值点号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<PValue> getMedian(List<PValue> input) {
            
            List<PValue> list = new List<PValue>();
            PValue median = new PValue();
            PValue medianNum = new PValue();
            List<Point> points = new List<Point>();
            int length=input.Count;
            for (int i = 0; i < length; i++) {
                Point p;
                p.pointNum = i + 1;
                p.value = input[i].Value;
                points.Add(p);
            }
            points.Sort();
            if (length > 0) {
               
                if (length % 2 != 0)
                {
                    median.Value = points[length / 2].value;
                    medianNum.Value = points[length / 2].pointNum;
                }
                else
                {
                    median.Value = (points[length / 2-1].value + points[length / 2].value) / 2;
                    medianNum.Value = points[length / 2-1].pointNum;
                }
               
               
            }
            list.Add(median);
            list.Add(medianNum);
            return list;
        }
        /// <summary>
        /// 求出输入数据的一次模型  和  二次模型   输出顺序为 一次斜率k 截距b  模型拟合程度r  二次参数a 一次参数b  截距c  模型拟合程度r
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<PValue> getModel(List<PValue> input) {
            double MTAEquRLK = 0;  //k
            double MTAEquRLB = 0;  //b
            double MTAEquRLR = 0;  //r
            double MTAEquRQA = 0;  //a
            double MTAEquRQB = 0;  //b
            double MTAEquRQC = 0;  //c
            double MTAEquRQR = 0;  //r
            int length = input.Count;
            double[] X = new double[length];
            double[] Y = new double[length];
            for (int i = 0; i < length; i++) {
                X[i] = i + 1;
                Y[i] = input[i].Value;
            }
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
            SimpleLinearRegression regression = ols.Learn(X, Y);

            MTAEquRLK = regression.Slope;                                         //一次斜率
            MTAEquRLB = regression.Intercept;                                     //截距
            MTAEquRLR = regression.CoefficientOfDetermination(X, Y);            //R^2代表模型拟合程度

            double[][] MultiX = new double[X.Length][];
            for (int i = 0; i < X.Length; i++)
            {
                double x = X[i];
                MultiX[i] = new double[] { x * x, x };
            }
            double[][] MultiY = new double[X.Length][];
            //MultiY =X'
            for (int i = 0; i < X.Length; i++)
            {
                double y = Y[i];
                MultiY[i] = new double[] { y };
            }
            OrdinaryLeastSquares olsMulti = new OrdinaryLeastSquares();
            MultivariateLinearRegression regressionMulti = olsMulti.Learn(MultiX, MultiY);
            double[][] weights = regressionMulti.Weights;
            MTAEquRQA = weights[0][0];                            //二次参数
            MTAEquRQB = weights[1][0];                            //一次参数
            MTAEquRQC = regressionMulti.Intercepts[0];            //截距
            MTAEquRQR = regressionMulti.CoefficientOfDetermination(MultiX, MultiY)[0];    //R^2代表模型拟合程度
            PValue LK = new PValue(); LK.Value = MTAEquRLK;
            PValue LB = new PValue(); LB.Value = MTAEquRLB;
            PValue LR = new PValue(); LR.Value = MTAEquRLR;
            PValue QA = new PValue(); QA.Value = MTAEquRQA;
            PValue QB = new PValue(); QB.Value = MTAEquRQB;
            PValue QC = new PValue(); QC.Value = MTAEquRQC;
            PValue QR = new PValue(); QR.Value = MTAEquRQR;
            List<PValue> list = new List<PValue>();
            list.Add(LK);
            list.Add(LB);
            list.Add(LR);
            list.Add(QA);
            list.Add(QB);
            list.Add(QC);
            list.Add(QR);
            return list;
        }
        /// <summary>
        /// 求输入数据的均差
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static PValue getStd(List<PValue> input) { 
            PValue std=new PValue();
            double avg = getAvg(input).Value;
            //平方和
            double sum = 0;
            for (int i = 0; i < input.Count; i++)
            {
                sum = sum + Math.Pow(input[i].Value-avg, 2.0);
            }
            //开方
            double val = Math.Sqrt(sum / input.Count);
            std.Value = val;
            return std;
        }
    }
}
