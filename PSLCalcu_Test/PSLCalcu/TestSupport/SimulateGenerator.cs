using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu
{
    class SimulateGenerator
    {
        //仿真数据信号发生器


        /// <summary>
        /// 三角函数发生器
        /// </summary>
        /// <param name="period">函数周期，</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="current">当前时间</param>
        /// <returns>生成值</returns>
        /// 
        public static double sinSimu(double period, int min, int max, double current)
        {
            double range = max - min;
            return range * (Math.Sin((2 * Math.PI / period) * current) + 1) / 2 + min;
        }
        /// <summary>
        /// 随机数发生器
        /// </summary>
        /// <param name="period">函数周期，</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="current">当前时间</param>
        /// <returns>生成值</returns>
        public static double randomSimu(double period, int min, int max, double current)
        {
            int iSeed = Convert.ToInt32(current);
            Random ran = new Random(iSeed);
            return ran.Next(min, max);
        }
        /// <summary>
        /// 方波发生器
        /// </summary>
        /// <param name="period">函数周期，</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="current">当前时间</param>
        /// <returns>生成值</returns>
        public static double squareSimu(double period, int min, int max, double current)
        {
            return (current % period) > (period / 2) ? max : min;

        }
        /// <summary>
        /// 锯齿波发生器
        /// </summary>
        /// <param name="period">函数周期，</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="current">当前时间</param>
        /// <returns>生成值</returns>
        public static double sawtoothSimu(double period, int min, int max, double current)
        {
            return (current % period) * ((max - min) / period) + min;

        }
    }
}
