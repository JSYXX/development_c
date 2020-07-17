using PCCommon;
using PSLCalcu.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test test = new Test();
            //test.testgetMedian();
            CalcuInfo info = new CalcuInfo();
            info.fparas = "2;4;6;0.1;10;3;6;S";
            List<PValue>[] inputs = new List<PValue>[1];
            //导入数据
            List<PValue> input = new List<PValue>();
            List<double> numList = new List<double>();
           LogHelper logHelper = LogFactory.GetLogger(typeof(Program));                     //全局log
            numList.Add(52.54159969);
            numList.Add(52.50509746);
            numList.Add(59.69764237);
            numList.Add(61.972912);
            numList.Add(65.80854953);
            numList.Add(71.98123385);
            numList.Add(83.47505666);
            numList.Add(79.37749864);
            numList.Add(85.37504792);
            numList.Add(93.22199874);
            numList.Add(97.76079334);
            numList.Add(98.64779962);
            numList.Add(99.07363096);
            numList.Add(102.8669049);
            numList.Add(104.1181729);
            numList.Add(98.06359504);
            numList.Add(98.09457107);
            numList.Add(96.97447223);
            numList.Add(94.49955851);
            numList.Add(93.26695724);
            numList.Add(88.42282597);
            numList.Add(94.02568941);
            numList.Add(88.74491661);
            numList.Add(83.63876096);
            numList.Add(84.37002094);
            numList.Add(79.35094495);
            numList.Add(74.10857393);
            numList.Add(65.95335822);
            numList.Add(64.41271746);
            numList.Add(50.6698147);
            numList.Add(48.77187187);
            numList.Add(40.25859368);
            numList.Add(42.75983415);
            numList.Add(31.17076165);
            numList.Add(31.87343334);
            numList.Add(29.08899387);
            numList.Add(24.32362435);
            numList.Add(16.16540545);
            numList.Add(11.31496909);
            numList.Add(14.1287861);
            numList.Add(8.725459637);
            numList.Add(4.900400962);
            numList.Add(2.218089445);
            numList.Add(-2.58869618);
            numList.Add(5.067132415);
            numList.Add(-0.776601167);
            numList.Add(4.194782234);
            numList.Add(-1.940590725);
            numList.Add(5.701939192);
            numList.Add(1.25557323);
            numList.Add(10.31278802);
            numList.Add(11.23061252);
            numList.Add(8.041819406);
            numList.Add(11.93481781);
            numList.Add(23.21176214);
            numList.Add(24.10317795);
            numList.Add(32.40917041);
            numList.Add(32.31477281);
            numList.Add(34.89431382);
            numList.Add(41.13020111);
            numList.Add(33.13020111);
            List<string> timelist = new List<string>();
            timelist.Add("2019-01-01 00:00:00");
            timelist.Add("2019-01-01 00:01:00");
            timelist.Add("2019-01-01 00:02:00");
            timelist.Add("2019-01-01 00:03:00");
            timelist.Add("2019-01-01 00:04:00");
            timelist.Add("2019-01-01 00:05:00");
            timelist.Add("2019-01-01 00:06:00");
            timelist.Add("2019-01-01 00:07:00");
            timelist.Add("2019-01-01 00:08:00");
            timelist.Add("2019-01-01 00:09:00");
            timelist.Add("2019-01-01 00:10:00");
            timelist.Add("2019-01-01 00:11:00");
            timelist.Add("2019-01-01 00:12:00");
            timelist.Add("2019-01-01 00:13:00");
            timelist.Add("2019-01-01 00:14:00");
            timelist.Add("2019-01-01 00:15:00");
            timelist.Add("2019-01-01 00:16:00");
            timelist.Add("2019-01-01 00:17:00");
            timelist.Add("2019-01-01 00:18:00");
            timelist.Add("2019-01-01 00:19:00");
            timelist.Add("2019-01-01 00:20:00");
            timelist.Add("2019-01-01 00:21:00");
            timelist.Add("2019-01-01 00:22:00");
            timelist.Add("2019-01-01 00:23:00");
            timelist.Add("2019-01-01 00:24:00");
            timelist.Add("2019-01-01 00:25:00");
            timelist.Add("2019-01-01 00:26:00");
            timelist.Add("2019-01-01 00:27:00");
            timelist.Add("2019-01-01 00:28:00");
            timelist.Add("2019-01-01 00:29:00");
            timelist.Add("2019-01-01 00:30:00");
            timelist.Add("2019-01-01 00:31:00");
            timelist.Add("2019-01-01 00:32:00");
            timelist.Add("2019-01-01 00:33:00");
            timelist.Add("2019-01-01 00:34:00");
            timelist.Add("2019-01-01 00:35:00");
            timelist.Add("2019-01-01 00:36:00");
            timelist.Add("2019-01-01 00:37:00");
            timelist.Add("2019-01-01 00:38:00");
            timelist.Add("2019-01-01 00:39:00");
            timelist.Add("2019-01-01 00:40:00");
            timelist.Add("2019-01-01 00:41:00");
            timelist.Add("2019-01-01 00:42:00");
            timelist.Add("2019-01-01 00:43:00");
            timelist.Add("2019-01-01 00:44:00");
            timelist.Add("2019-01-01 00:45:00");
            timelist.Add("2019-01-01 00:46:00");
            timelist.Add("2019-01-01 00:47:00");
            timelist.Add("2019-01-01 00:48:00");
            timelist.Add("2019-01-01 00:49:00");
            timelist.Add("2019-01-01 00:50:00");
            timelist.Add("2019-01-01 00:51:00");
            timelist.Add("2019-01-01 00:52:00");
            timelist.Add("2019-01-01 00:53:00");
            timelist.Add("2019-01-01 00:54:00");
            timelist.Add("2019-01-01 00:55:00");
            timelist.Add("2019-01-01 00:56:00");
            timelist.Add("2019-01-01 00:57:00");
            timelist.Add("2019-01-01 00:58:00");
            timelist.Add("2019-01-01 00:59:00");
            timelist.Add("2019-01-02 00:00:00");
            timelist.Add("2019-01-02 00:01:00");
            DateTime time = Convert.ToDateTime("2019-01-01 00:00:00");
            for (int i = 0; i < numList.Count; i++)
            {
                PValue p = new PValue();
                p.Value = numList[i];
                p.Timestamp = Convert.ToDateTime(timelist[i]);
                p.Endtime = Convert.ToDateTime(timelist[i + 1]);
                p.Status = 0;
                input.Add(p);

            }
            inputs[0] = input;
            MPVBase.inputData=inputs;
            MPVBase.calcuInfo=info;
            Results res=MPVBase.Calcu();
           
            List<PValue>[] reslists= res.results;
            List<PValue> reslist = reslists[0];
            for (int i = 0; i < reslists.Length; i++)
            {
                Console.WriteLine("res"+i+":   " + reslists[i][0].Value);
                logHelper.Info(reslists[i][0].Value);
            }
            Console.Read();
            
        }
    }
}
