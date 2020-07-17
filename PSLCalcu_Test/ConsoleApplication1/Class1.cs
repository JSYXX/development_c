using DBInterface.RDBInterface;
using DBInterface.RTDBInterface;
using PCCommon;
using PSLCalcu;
using PSLCalcu.Module;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConsoleApplication1
{
    class Test
    {
        public void testMethod()
        {
            Console.WriteLine("start test================================");
            DbHelper helper = new DbHelper();
            helper.ConnTest();
            String sqlStr = "use psldb;select * from psldata201911";
            //var sqlTimer = Stopwatch.StartNew();          //用于测试读取一个月分钟数据的sql执行所耗时长。一个月分钟数据45000条左右，在公司服务测试结果是耗时4ms-5ms            
            IDataReader reader = helper.ExecuteReader(sqlStr);
            List<PValue> items = new List<PValue>();
            while (reader.Read())
            {
                PValue item = new PValue();
                //DAOSupport.ReaderToObject(reader, item);   //20180711,经测试，在读取1个月的分钟概化数据时，返回数据在45000条，使用该 ReaderToObject转换程序，IDataReader2PSLDataItems整体耗时300ms。效率较低。
                //DAOSupport.ReaderToObject是一个通用格式的转换程序。在单条数据转换时，会去循环读取。在对效率要求比较高的psldataXXXX数据库的数据进行转换时，需要使用专用的转换程序，以便保证效率。
                //在psldata表的读写中，由于数据量大，要提高读写效率，需要注意不要使用反射
                //关于反射效率的问题，在这个地址的帖子中，有所讨论。https://bbs.csdn.net/topics/391910616
                //关于IDataReader的速度，在这个地址的帖子中，有所讨论。https://blog.csdn.net/lilong_herry/article/details/79993907
                try { item.Timestamp = new DateTime(Convert.ToInt64(reader["tagstarttime"])); }
                catch { };
                try { item.Endtime = new DateTime(Convert.ToInt64(reader["tagendtime"])); }
                catch { };
                try { item.Value = Convert.ToDouble(reader["tagvalue"]); }
                catch { item.Value = 0; };
                try { item.Status = Convert.ToInt64(reader["tagstatus"]); }
                catch { item.Status = 0; };
                items.Add(item);
               
            }
            Console.WriteLine(items.Count);
            Console.Read();
        }
       
        public void rtdbTestTomGetData() {
            DateTime start = new DateTime(2015, 5, 1, 0, 0, 0);
            DateTime end = new DateTime(2015, 5, 1, 0, 2, 0);
            //TOMHelper helper = new TOMHelper();
           // String info=helper.Logon();
           // bool status=helper.isLogOn;
            //Console.WriteLine("连接信息" + info + "----状态：" + status);
            DBInterface.RTDBInterface.LongTOM.RTDbHelper helper = new DBInterface.RTDBInterface.LongTOM.RTDbHelper();
            List<PValue> pvalues=helper.GetRawValues("test.tag01", start, end);
            List<PValue> pvalues2 = helper.GetRawValues("test.tag02", start, end);
            List<PValue> pvalues3 = helper.GetRawValues("test.tag03", start, end);
            List<PValue> pvalues4 = helper.GetRawValues("test.tag04", start, end);
            List<PValue> pvalues5 = helper.GetRawValues("test.tag05", start, end);
            List<PValue> pvalues6 = helper.GetRawValues("test.tag06", start, end);
           
            foreach(PValue p in pvalues){
                Console.WriteLine(p.Timestamp+" "+p.Value);
            }

            
            foreach (PValue p in pvalues2)
            {
                Console.WriteLine(p.Timestamp + " " + p.Value);
            }
           
            foreach (PValue p in pvalues3)
            {
                Console.WriteLine(p.Timestamp + " " + p.Value);
            }
           
            foreach (PValue p in pvalues4)
            {
                Console.WriteLine(p.Timestamp + " " + p.Value);
            }
          
            foreach (PValue p in pvalues5)
            {
                Console.WriteLine(p.Timestamp + " " + p.Value);
            }
           
            foreach (PValue p in pvalues6)
            {
                Console.WriteLine(p.Timestamp + " " + p.Value);
            }
        }
        public void MethodTest() {
            DbHelper helper = new DbHelper();
            helper.ConnTest();
            String sqlStr = "use psldb;select * from psldata201911 where tagid=10052";// where psltagid=10052";
            IDataReader reader = helper.ExecuteReader(sqlStr);
            List<PValue> items = new List<PValue>();
            while (reader.Read())
            {
                PValue item = new PValue();
                try { item.Tagid = Convert.ToInt32(reader["tagid"]); }
                catch { };
                try { item.Timestamp = new DateTime(Convert.ToInt64(reader["tagstarttime"])); }
                catch { };
                try { item.Endtime = new DateTime(Convert.ToInt64(reader["tagendtime"])); }
                catch { };
                try { item.Value = Convert.ToDouble(reader["tagvalue"]); }
                catch { item.Value = 0; };
                try { item.Status = Convert.ToInt64(reader["tagstatus"]); }
                catch { item.Status = 0; };
                Console.WriteLine("value:" + item.Value);
                items.Add(item);

            }
            Console.WriteLine(items.Count);
            // MHarMean
            PValue Max=BaseCalcu.getMax(items);
            PValue Min = BaseCalcu.getMin(items);
            Console.WriteLine("最大值：" + Max.Value);
            Console.WriteLine("最小值：" + Min.Value);
            Console.WriteLine("最大值标签:" + Max.Tagid);
            Console.Read();
            /*

            Console.WriteLine("start test================================");
            DbHelper helper = new DbHelper();
            helper.ConnTest();
            String sqlStr = "use psldb;select * from psldata201911";
            //var sqlTimer = Stopwatch.StartNew();          //用于测试读取一个月分钟数据的sql执行所耗时长。一个月分钟数据45000条左右，在公司服务测试结果是耗时4ms-5ms            
            IDataReader reader = helper.ExecuteReader(sqlStr);
            List<PValue> items = new List<PValue>();
            while (reader.Read())
            {
                PValue item = new PValue();
                try { item.Timestamp = new DateTime(Convert.ToInt64(reader["tagstarttime"])); }
                catch { };
                try { item.Endtime = new DateTime(Convert.ToInt64(reader["tagendtime"])); }
                catch { };
                try { item.Value = Convert.ToDouble(reader["tagvalue"]); }
                catch { item.Value = 0; };
                try { item.Status = Convert.ToInt64(reader["tagstatus"]); }
                catch { item.Status = 0; };
                items.Add(item);

            }
            Console.WriteLine(items.Count);
            Console.Read();
             */
        }

        //psldao 测试
        public void psldaoMethod(){
            DateTime time1 = new DateTime(2019, 11, 10, 00, 00, 00);
            DateTime time2 = new DateTime(2019, 11, 10, 01, 00, 00);
            //10001
            List<PValue> value = PSLDataDAO.Read(10448, time1, time2);
            Console.WriteLine("数据数量：" + value.Count);
            Console.Read();
        }
        public void testgetTurnN() {
            List<PValue> list = new List<PValue>();
            PValue p1 = new PValue();
            p1.Value = 5;
            PValue p2 = new PValue();
            p2.Value = 1;
            PValue p3 = new PValue();
            p3.Value = 4;
            PValue p4 = new PValue();
            p4.Value = 6;
            PValue p5 = new PValue();
            p5.Value = 7;
            list.Add(p1);
            list.Add(p2);
            list.Add(p3);
            list.Add(p4);
            list.Add(p5);
            PValue res=BaseCalcu.getTurnN(list);
            Console.WriteLine("翻转次数："+res.Value);
            Console.Read();
        }
        public void testgetTopDown() {
            int a = 6;
            Console.WriteLine(5% 2);
            List<PValue> list = new List<PValue>();
            PValue p1 = new PValue();
            PValue p2 = new PValue();
            PValue p3 = new PValue();
            PValue p4 = new PValue();
            PValue p5 = new PValue();
            PValue p6 = new PValue();
            PValue p7 = new PValue();
            PValue p8 = new PValue();
            PValue p9 = new PValue();
            PValue p10 = new PValue();
            p1.Value = 0;
            p2.Value = 5;
            p3.Value = 2;
            p4.Value = 4;
            p5.Value = 2;
            p6.Value = 1;
            p7.Value = 3;
            p8.Value = 3;
            p9.Value = 1;
            p10.Value = 0;
            list.Add(p1);
            list.Add(p2);
            list.Add(p3);
            list.Add(p4);
            list.Add(p5);
            list.Add(p6);
            list.Add(p7);
            list.Add(p8);
            list.Add(p9);
            list.Add(p10);
            List<PValue> res = BaseCalcu.getTopDown(list);
            Console.WriteLine("最凸点；" + res[0].Value + "  点号：" + res[1].Value + " 最凹点：" + res[2].Value + " 点号：" + res[3].Value);
            Console.Read();
        }
        //getMedian
        public void testgetMedian()
        {
           
            List<PValue> list = new List<PValue>();
            PValue p1 = new PValue();
            PValue p2 = new PValue();
            PValue p3 = new PValue();
            PValue p4 = new PValue();
            PValue p5 = new PValue();
            PValue p6 = new PValue();
            PValue p7 = new PValue();
            PValue p8 = new PValue();
            PValue p9 = new PValue();
            PValue p10 = new PValue();
            p1.Value = 0;
            p2.Value = 5;
            p3.Value = 2;
            p4.Value = 4;
            p5.Value = 2;
            p6.Value = 1;
            p7.Value = 3;
            p8.Value = 3;
            p9.Value = 1;
            p10.Value = 0;
            list.Add(p1);
            list.Add(p2);
            list.Add(p3);
            list.Add(p4);
            list.Add(p5);
            list.Add(p6);
            list.Add(p7);
            list.Add(p8);
            list.Add(p9);
            list.Add(p10);
            List<PValue> res = BaseCalcu.getMedian(list);
            Console.WriteLine("中位值；" + res[0].Value + "  点号：" + res[1].Value);
            Console.Read();
        }
       
    }
}
