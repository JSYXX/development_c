using MathNet.Numerics;
using PCCommon.NewCaculateCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.NewCaculate
{
    public class MultipleRegressionAlgorithmCaculate
    {
        /// <summary>
        /// 多元回归算法
        /// </summary>
        /// <param name="n">自变量个数</param>
        /// <param name="u">归一化标识，1做归一化处理，0不做归一化处理</param>
        /// <param name="vib">过滤，去最大百分比滤波处理数组</param>
        /// <param name="vis">过滤，去最小百分比滤波处理数组</param>
        /// <param name="xij">自变量二维数组</param>
        /// <param name="Y">因变量数组</param>
        /// <param name="XF">预处理方式</param>
        /// <param name="k">返回过滤后的有效数据</param>
        /// <param name="p">返回过滤后的有效数据占比</param>
        /// <param name="mList">返回系数数组</param>
        ///// <param name="b">返回截距</param>
        /// <param name="errmsg">返回错误信息</param>
        /// <returns>错误编码S</returns>
        public int Regression(int u, double[] vib, double[] vis, double[][] xij, double[] Y, List<XFClass> XF, ref int k, ref double p, ref double[] mList, ref double[] resultList, ref string errmsg)
        {
            int S = 0;
            try
            {
                //mList = Fit.MultiDim(xij, Y, true, MathNet.Numerics.LinearRegression.DirectRegressionMethod.Svd);
                #region 数据初始判断
                //if (n != xij[0].Count())
                //{
                //    errmsg = "自变量二维数组列数与自变量个数n不符。";
                //    return 5;
                //}
                if (vib.Count() != xij[0].Count() + 1)
                {
                    errmsg = "自变量二维数组列数与最大百分比滤波数量不符。";
                    return 5;
                }
                if (vis.Count() != xij[0].Count() + 1)
                {
                    errmsg = "自变量二维数组列数与最小百分比滤波数量不符。";
                    return 5;
                }
                //if (XF.GetLength(0) != xij[0].Count())
                //{
                //    errmsg = "自变量二维数组列数与预处理个数不符。";
                //    return 5;
                //}
                if (Y.Count() != xij.GetLength(0))
                {
                    errmsg = "自变量二维数组行数与因变量数组个数不符。";
                    return 4;
                }
                if (xij.GetLength(0) < XF.Count)
                {
                    errmsg = "自变量二维数组行数小于自变量个数。";
                    return 1;
                }
                int getXCount = xij[0].Count();
                #endregion
                #region 将二维数组变化为List
                List<IndependentVariableClass> newXijList = new List<IndependentVariableClass>();
                newXijList = getNewList(xij, Y);
                #endregion
                #region 滤波过滤
                List<int> removeList = getRemoveIdList(newXijList, vib, vis);
                foreach (int item in removeList)
                {
                    newXijList.RemoveAll(x => x.id == item);
                }
                if (newXijList.Count < XF.Count)
                {
                    errmsg = "滤波后自变量二维数组行数小于自变量个数。";
                    return 2;
                }
                #endregion
                #region 预处理
                foreach (IndependentVariableClass item in newXijList)
                {
                    while (item.valueList.Count < XF.Count)
                    {
                        item.valueList.Add(0.0);
                    }
                }
                foreach (IndependentVariableClass item in newXijList)
                {
                    for (int i = 0; i < item.valueList.Count; i++)
                    {
                        switch (XF[i].A)
                        {
                            case 0:
                                break;
                            case 1:
                                item.valueList[i] = Preprocessing1(item.valueList[i], XF[i].B);
                                break;
                            case 2:
                                item.valueList[i] = Preprocessing2(item.valueList[i], XF[i].B);
                                break;
                            case 3:
                                item.valueList[i] = Preprocessing3(item.valueList[i], XF[i].B);
                                break;
                            case 4:
                                item.valueList[i] = Preprocessing4(item.valueList[i]);
                                break;
                            case 5:
                                item.valueList[i] = Preprocessing5(item.valueList, item.y, XF[i].B, getXCount);
                                break;
                            //case 6:
                            //    item.valueList[i] = Preprocessing6(item.valueList[i], XF[i][1]);
                            //    break;
                            default:
                                break;
                        }
                    }
                }

                #endregion
                #region 归一化处理
                if (u == 1)
                {
                    newXijList = Normalization(newXijList, XF.Count);
                }
                #endregion
                k = newXijList.Count;//有效数据行数
                p = Math.Round((Convert.ToDouble(newXijList.Count) / Convert.ToDouble(xij.GetLength(0))), 4);//有效数据占比
                double[][] newX = new double[newXijList.Count][];
                double[] newY = new double[newXijList.Count];
                for (int i = 0; i < newXijList.Count; i++)
                {
                    double[] xList = new double[newXijList[i].valueList.Count];
                    for (int l = 0; l < newXijList[i].valueList.Count; l++)
                    {
                        xList[l] = newXijList[i].valueList[l];
                    }
                    newX[i] = xList;
                    newY[i] = newXijList[i].y;
                }
                mList = Fit.MultiDim(newX, newY, true, MathNet.Numerics.LinearRegression.DirectRegressionMethod.NormalEquations);//系数数组

                double df = k - XF.Count - 1;
                List<double> yyList = new List<double>();
                foreach (IndependentVariableClass item in newXijList)
                {
                    double yy = mList[0];
                    for (int i = 0; i < item.valueList.Count; i++)
                    {
                        yy += item.valueList[i] * mList[i + 1];
                    }
                    yyList.Add(yy);
                }
                double avgY = newXijList.Average(t => t.y);
                double SSR = 0;
                for (int i = 0; i < yyList.Count; i++)
                {
                    SSR += Math.Pow(avgY - yyList[i], 2.0);
                }
                double SSE = 0;
                for (int i = 0; i < newXijList.Count; i++)
                {
                    SSE += Math.Pow(newXijList[i].y - yyList[i], 2.0);
                }
                double SST = SSR + SSE;
                double F = Math.Round(((SSR / XF.Count + 0.0) / (SSE / df)), 3);
                double R2 = Math.Round((SSR / SST), 3);
                double sey = Math.Round(Math.Sqrt(SSE / df), 3);
                resultList = new double[6];
                resultList[0] = R2;
                resultList[1] = sey;
                resultList[2] = F;
                resultList[3] = df;
                resultList[4] = SSR;
                resultList[5] = SSE;

                return S;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                S = 3;
            }
            return S;
        }
        /// <summary>
        /// 将自变量二位数组和因变量转化为List，以便数据处理
        /// </summary>
        /// <param name="xij"></param>
        /// <returns></returns>
        private List<IndependentVariableClass> getNewList(double[][] xij, double[] Y)
        {
            try
            {
                List<IndependentVariableClass> newList = new List<IndependentVariableClass>();
                if (xij != null)
                {
                    int i = 1;
                    foreach (double[] item in xij)
                    {
                        IndependentVariableClass iv = new IndependentVariableClass();
                        iv.id = i;
                        List<double> vList = new List<double>();
                        foreach (double nextItem in item)
                        {
                            vList.Add(nextItem);
                        }
                        iv.valueList = vList;
                        iv.y = Y[i - 1];
                        newList.Add(iv);
                        i++;
                    }
                }
                return newList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取滤波过滤掉的ID
        /// </summary>
        /// <param name="newXijList"></param>
        /// <param name="vib"></param>
        /// <param name="vis"></param>
        /// <returns></returns>
        private List<int> getRemoveIdList(List<IndependentVariableClass> newXijList, double[] vib, double[] vis)
        {
            try
            {
                List<int> removeIdList = new List<int>();
                List<int> maxIdList = getRemoveMaxIdList(newXijList, vib);
                List<int> minIdList = getRemoveMinIdList(newXijList, vis);
                foreach (int item in maxIdList)
                {
                    if (!removeIdList.Contains(item))
                    {
                        removeIdList.Add(item);
                    }
                }
                foreach (int item in minIdList)
                {
                    if (!removeIdList.Contains(item))
                    {
                        removeIdList.Add(item);
                    }
                }
                return removeIdList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 最大百分比过滤
        /// </summary>
        /// <param name="newXijList"></param>
        /// <param name="vib"></param>
        /// <returns></returns>
        private List<int> getRemoveMaxIdList(List<IndependentVariableClass> newXijList, double[] vib)
        {
            try
            {
                List<int> maxList = new List<int>();
                for (int i = 0; i < vib.Count() - 1; i++)
                {
                    if (vib[i] > 0)
                    {
                        newXijList = (from a in newXijList
                                      orderby a.valueList[i] descending
                                      select a).ToList();
                        int countDou = (int)Math.Round((newXijList.Count * (vib[i] / 100)), 0);
                        for (int l = 0; l < countDou; l++)
                        {
                            if (!maxList.Contains(newXijList[l].id))
                            {
                                maxList.Add(newXijList[l].id);
                            }
                        }
                    }
                }
                newXijList = (from a in newXijList
                              orderby a.y descending
                              select a).ToList();
                int countDou2 = (int)Math.Round((newXijList.Count * (vib[vib.Count() - 1] / 100)), 0);
                for (int l = 0; l < countDou2; l++)
                {
                    if (!maxList.Contains(newXijList[l].id))
                    {
                        maxList.Add(newXijList[l].id);
                    }
                }
                return maxList;
            }
            catch (Exception ex)
            {
                throw new Exception("最大百分比过滤错误");
            }
        }
        /// <summary>
        /// 最小百分比过滤
        /// </summary>
        /// <param name="newXijList"></param>
        /// <param name="vis"></param>
        /// <returns></returns>
        private List<int> getRemoveMinIdList(List<IndependentVariableClass> newXijList, double[] vis)
        {
            try
            {
                List<int> minList = new List<int>();
                for (int i = 0; i < vis.Count() - 1; i++)
                {
                    if (vis[i] > 0)
                    {
                        newXijList = (from a in newXijList
                                      orderby a.valueList[i] ascending
                                      select a).ToList();
                        int countDou = (int)Math.Round((newXijList.Count * (vis[i] / 100)), 0);
                        for (int l = 0; l < countDou; l++)
                        {
                            if (!minList.Contains(newXijList[l].id))
                            {
                                minList.Add(newXijList[l].id);
                            }
                        }
                    }
                }
                newXijList = (from a in newXijList
                              orderby a.y ascending
                              select a).ToList();
                int countDou2 = (int)Math.Round((newXijList.Count * (vis[vis.Count() - 1] / 100)), 0);
                for (int l = 0; l < countDou2; l++)
                {
                    if (!minList.Contains(newXijList[l].id))
                    {
                        minList.Add(newXijList[l].id);
                    }
                }
                return minList;

            }
            catch (Exception ex)
            {
                throw new Exception("最小百分比过滤错误");
            }
        }
        #region 预处理
        /// <summary>
        /// B = [k，b] 对 x 做线性迁移，x = k * 输入变量 + b
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Preprocessing1(double preValue, List<double> b)
        {
            try
            {
                return Math.Round(b[0] * preValue + b[1], 3);
            }
            catch (Exception ex)
            {
                throw new Exception("预处理1错误");
            }
        }
        /// <summary>
        /// B = 幂次，对 x 做幂运算，幂指数为 B。若 x 不满足运算要求，记标记，退出计算
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Preprocessing2(double preValue, List<double> b)
        {
            try
            {
                return Math.Round(Math.Pow(preValue, b[0]), 3);
            }
            catch (Exception ex)
            {
                throw new Exception("预处理2错误");
            }
        }
        /// <summary>
        /// 对数运算。B = 0，自然对数运算，B = 其他，做 B 为底的对数运算。
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Preprocessing3(double preValue, List<double> b)
        {
            try
            {
                double returnNumber = 0;
                if (b[0] == 0)
                {
                    returnNumber = Math.Round(Math.Log(preValue), 3);
                }
                else
                {
                    Math.Round(Math.Log(b[0], preValue), 3);
                }
                return returnNumber;
            }
            catch (Exception ex)
            {
                throw new Exception("预处理3错误");
            }
        }
        /// <summary>
        /// 绝对值
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Preprocessing4(double preValue)
        {
            try
            {
                return Math.Abs(preValue); ;
            }
            catch (Exception ex)
            {
                throw new Exception("自然对数预处理错误");
            }
        }
        /// <summary>
        /// B为已知自变量编号，根据已知自变量以及自变量编号的乘积得出新的自变量
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Preprocessing5(List<double> preValue, double y, List<double> b, int maxCount)
        {
            try
            {
                double returnNumber = 0;
                for (int i = 0; i < b.Count; i++)
                {
                    if (i == 0)
                    {
                        returnNumber = preValue[(int)b[i]];
                    }
                    else if (((int)b[i]) < maxCount)
                    {
                        returnNumber *= preValue[(int)b[i]];
                    }
                    else
                    {
                        returnNumber *= y;
                    }
                }
                return returnNumber;
            }
            catch (Exception ex)
            {
                throw new Exception("对数预处理错误");
            }
        }
        /// <summary>
        /// 绝对值处理
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private double Preprocessing6(double preValue, double b)
        {
            try
            {
                return Math.Abs(preValue);
            }
            catch (Exception ex)
            {
                throw new Exception("绝对值预处理错误");
            }
        }
        #endregion
        /// <summary>
        /// 归一化处理
        /// </summary>
        /// <param name="insertList"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private List<IndependentVariableClass> Normalization(List<IndependentVariableClass> insertList, int n)
        {
            try
            {
                List<IndependentVariableClass> newList = new List<IndependentVariableClass>();
                newList = insertList;
                for (int i = 0; i < n; i++)
                {
                    double maxXValue = newList[0].valueList[i];
                    double minXValue = newList[0].valueList[i];

                    foreach (IndependentVariableClass item in newList)
                    {
                        if (maxXValue < item.valueList[i])
                        {
                            maxXValue = item.valueList[i];
                        }
                        if (minXValue > item.valueList[i])
                        {
                            minXValue = item.valueList[i];
                        }
                    }
                    foreach (IndependentVariableClass item in newList)
                    {
                        item.valueList[i] = Math.Round((item.valueList[i] - minXValue) / (maxXValue - minXValue), 4);
                    }
                }
                double maxYValue = newList[0].y;
                double minYValue = newList[0].y;

                foreach (IndependentVariableClass item in newList)
                {
                    if (maxYValue < item.y)
                    {
                        maxYValue = item.y;
                    }
                    if (minYValue > item.y)
                    {
                        minYValue = item.y;
                    }
                }
                foreach (IndependentVariableClass item in newList)
                {
                    item.y = Math.Round((item.y - minYValue) / (maxYValue - minYValue), 4);
                }
                return newList;
            }
            catch (Exception ex)
            {
                throw new Exception("归一化处理错误");
            }
        }


    }
}
