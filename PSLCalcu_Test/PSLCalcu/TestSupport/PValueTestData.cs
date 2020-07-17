using System;
using System.Text;
using System.Collections.Generic;       //使用List<>
using PCCommon; //使用PValue


namespace PSLCalcu
{
    class TestData_MLimitStat
    {
        //20170322,用于测试超限统计
        //假设三低限为20，30，40，三高限为60，70，80，死区为2       
        public static List<PValue> simuData1() 
        {               
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(70, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(59, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(57, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(52, new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(71, new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(80, new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(78, new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results.Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(36, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), 0));
            results.Add(new PValue(44, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(34, new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(24, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(14, new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(21, new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(19, new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 42, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), 0));
            results.Add(new PValue(71, new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); 
            return results;
        }

        //20170322,用于测试超限统计,从N到HHH，再返回N
        //假设三低限为20，30，40，三高限为60，70，80，死区为2
        //本数据统计结果，应该为
        //Number:0,0,0,1,1,1
        //Span：0,0,0,1,2,2,1
        //Area:0,0,0,0,10,10,2.5        
        public static List<PValue> simuData2()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));   
            results.Add(new PValue(95, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));   
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));   
            return results;
        }
        //20170323,用于测试超限统计，从N到LLL，再返回N
        //假设三低限为20，30，40，三高限为60，70，80，死区为2
        //本数据统计结果，应该为
        //Number:1,1,1,0,0,0
        //Span：1,2,2,1,0,0,0,
        //Area:2.5,10,10,0,0,0        
        public static List<PValue> simuData3()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results.Add(new PValue(-15, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));
            results.Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));
            return results;
        }
        //20170323,用于测试超限统计，从N到HHH，再返回N，再到LLL，再返回N
        //假设三低限为20，30，40，三高限为60，70，80，死区为2
        //本数据统计结果，应该为
        //Number:1,1,1,0,1,1,1
        //Span：1,2,2,5,2,2,1,
        //Area:2.5,10,10,10,10,2.5        
        public static List<PValue> simuData4()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));
            results.Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), 0));
            results.Add(new PValue(15, new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results.Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), 0));
            return results;
        }
        //20170323,用于测试超限统计，从N到HHH，再直接到LLL，再返回N
        //假设三低限为20，30，40，三高限为60，70，80，死区为2
        //本数据统计结果，应该为
        //Number:1,1,1,0,1,1,1
        //Span：1,2,2,3,2,2,1,
        //Area:2.5,10,10,10,10,2.5        
        public static List<PValue> simuData5()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));            
            results.Add(new PValue(15, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), 0));
            results.Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), 0));
            return results;
        }
        //20170323,用于测试超限统计，从N到LLL，再直接到HHH，再返回N
        //假设三低限为20，30，40，三高限为60，70，80，死区为2
        //本数据统计结果，应该为
        //Number:1,1,1,0,1,1,1
        //Span：1,2,2,3,2,2,1,
        //Area:2.5,10,10,10,10,2.5        
        public static List<PValue> simuData6()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results.Add(new PValue(15, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), 0));
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), 0));
            return results;
        }


        

        //20170324,用于测试时间的与或非
        //本测试时一个总体时间段，用于not测试
        public static List<PValue> spanLogicData0()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10,15, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 21, 33, DateTimeKind.Local), 0));            
            return results;
        }
        //20170324,用于测试时间的与或非
        //本测试用于时间的与或非
        public static List<PValue> spanLogicData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 42, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 47, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 01, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 13, 22, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 13, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 13, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 21, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 44, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 59, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 15, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 15, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 56, DateTimeKind.Local), 0));
            return results;
        }
        public static List<PValue> spanLogicData2()
        //本测试用于时间的与或非
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 05, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 07, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 08, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 30, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 43, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 13, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 01, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 02, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 09, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 18, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 48, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 30, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 15, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 59, DateTimeKind.Local), 0));
            return results;
        }
        //20170324,用于测试时间的过滤
        //本测试用于时间的与或非
        public static List<PValue> spanFilterData0()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(22, new DateTime(2016, 1, 1, 12, 12, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(33, new DateTime(2016, 1, 1, 12, 12, 42, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 47, DateTimeKind.Local), 0));
            results.Add(new PValue(53, new DateTime(2016, 1, 1, 12, 12, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 01, DateTimeKind.Local), 0));
            results.Add(new PValue(56, new DateTime(2016, 1, 1, 12, 13, 22, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(54, new DateTime(2016, 1, 1, 12, 13, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(49, new DateTime(2016, 1, 1, 12, 13, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 21, DateTimeKind.Local), 0));
            results.Add(new PValue(43, new DateTime(2016, 1, 1, 12, 14, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(46, new DateTime(2016, 1, 1, 12, 14, 44, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 59, DateTimeKind.Local), 0));
            results.Add(new PValue(47, new DateTime(2016, 1, 1, 12, 15, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(22, new DateTime(2016, 1, 1, 12, 15, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 56, DateTimeKind.Local), 0));
            return results;
        }
        public static List<PValue> spanFilterData1()
        //本测试用于时间的与或非
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 05, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 07, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 08, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 30, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 12, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 43, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 13, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 01, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 02, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 09, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 18, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 48, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 30, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 15, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 59, DateTimeKind.Local), 0));
            return results;
        }
        
    }//end class
    class TestData_MFDistribute22
    {
        //20170322,用于测试超限统计
        //假设三低限为20，30，40，三高限为60，70，80，死区为2       
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(70, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(59, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(57, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(52, new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(71, new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(80, new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(78, new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results.Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(36, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), 0));
            results.Add(new PValue(44, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(34, new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(24, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(14, new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(21, new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(19, new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 42, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), 0));
            results.Add(new PValue(100, new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), 0));
            results.Add(new PValue(92, new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), 0));
            results.Add(new PValue(96, new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), 0));
            results.Add(new PValue(101, new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            return results;
        }
    }//end class
    class TestData_MFDistribute12
    {
        //20170322,用于测试超限统计
        //假设三低限为20，30，40，三高限为60，70，80，死区为2       
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(70, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(59, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(57, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(52, new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(71, new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(80, new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(78, new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results.Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(36, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), 0));
            results.Add(new PValue(44, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(34, new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(24, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(14, new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(21, new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(19, new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 42, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), 0));
            results.Add(new PValue(100, new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), 0));
            results.Add(new PValue(92, new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), 0));
            results.Add(new PValue(96, new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), 0));
            results.Add(new PValue(101, new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            return results;
        }
    }
    class TestData_MCondSpan2
    {
        //20170322,用于测试超限统计              
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(70, new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(59, new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(57, new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(52, new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(71, new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(80, new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 14, 00, 00, DateTimeKind.Local), 0)); //测试HH边界
            
            return results;
        }
    }//end class
    class TestData_MLDistribute22
    {
        //20170322,用于测试超限统计
        //假设三低限为20，30，40，三高限为60，70，80，死区为2       
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(70, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(59, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(63, new DateTime(2016, 1, 1, 12, 10, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(57, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(52, new DateTime(2016, 1, 1, 12, 10, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));
            results.Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(71, new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(80, new DateTime(2016, 1, 1, 12, 10, 12, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(78, new DateTime(2016, 1, 1, 12, 10, 13, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), 0));
            results.Add(new PValue(85, new DateTime(2016, 1, 1, 12, 10, 14, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(55, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 16, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results.Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 18, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(36, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), 0));
            results.Add(new PValue(44, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(34, new DateTime(2016, 1, 1, 12, 10, 24, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(24, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(14, new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(21, new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(19, new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 29, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 31, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 32, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 33, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 34, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 36, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 37, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 38, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 39, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 42, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 44, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 46, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 47, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 49, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 51, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 52, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 53, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 54, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), 0));
            results.Add(new PValue(100, new DateTime(2016, 1, 1, 12, 10, 56, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), 0));
            results.Add(new PValue(92, new DateTime(2016, 1, 1, 12, 10, 57, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), 0));
            results.Add(new PValue(96, new DateTime(2016, 1, 1, 12, 10, 58, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), 0));
            results.Add(new PValue(101, new DateTime(2016, 1, 1, 12, 10, 59, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            return results;
        }
    }//end class
    class TestData_MDigitalSum
    {
        //20170322,用于测试            
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 14, 00, 00, DateTimeKind.Local), 0)); //测试HH边界

            return results;
        }
        //20170322,用于测试            
        public static List<PValue> simuData2()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 14, 00, 00, DateTimeKind.Local), 0)); //测试HH边界

            return results;
        }
        //20170322,用于测试            
        public static List<PValue> simuData3()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 11, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 14, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 19, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 28, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 29, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 55, 6, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), 0));
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 10, 7, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 12, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 14, 9, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 14, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 13, 16, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 30, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 14, 00, 00, DateTimeKind.Local), 0)); //测试HH边界

            return results;
        }
    }
    //定义一个委托，用于委托信号发生函数
    delegate double generator(double period, int min, int max, double current);
    class TestData_RandomPValue
    {
        public static List<PValue>  Generate( string SimuType,int SimuPeriod,int SimuAMin,int SimuAMax, DateTime startDate, DateTime endDate,int intervalseconds) 
        {
            
            List<PValue> pvalues=new List<PValue>();
            DateTime currentDate = startDate;
            generator generator ;
            switch (SimuType)                                      //获得当前行的信号发生器
                    {
                        case "square":
                            generator = SimulateGenerator.squareSimu;
                            break;
                        case "random":
                            generator = SimulateGenerator.randomSimu;
                            break;
                        case "sin":
                            generator = SimulateGenerator.sinSimu;
                            break;
                        case "sawtooth":
                            generator = SimulateGenerator.sawtoothSimu;
                            break;
                        default:
                            generator = SimulateGenerator.sinSimu;
                            break;
                    }
            do
            {
                pvalues.Add(new PValue(
                    generator(SimuPeriod, SimuAMin, SimuAMax, currentDate.Subtract(startDate).TotalSeconds),
                    currentDate,
                    currentDate.AddSeconds(intervalseconds),
                    0
                    ));
                currentDate = currentDate.AddSeconds(intervalseconds);
            } while (currentDate < endDate);

            pvalues[pvalues.Count-1].Endtime = endDate;

            return pvalues;

        }
    }
    class TestData_Index10
    {
        public static List<PValue>[] SimuData()
        {
            List<PValue>[] results = new List<PValue>[25];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);
            Random ran = new Random(iSeed);
            
            for (int i = 0; i < results.Length; i++) 
            {   
                results[i] = new List<PValue>();
                for (int j = 0; j < 10; j++)
                {
                    results[i].Add(new PValue(ran.Next(1, 100), new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local).AddSeconds(j), new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local).AddSeconds(j+1), 0));
                }
            }
               return results;
        }

    }
    class TestData_MCondSpan
    {
        public static List<PValue>[] SimuData()
        {
            List<PValue>[] results = new List<PValue>[4];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);
            

            results[0] = new List<PValue>();
            results[0].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[1] = new List<PValue>();
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 11, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 26, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 27, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 28, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 48, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 10, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 34, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 47, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[2] = new List<PValue>();
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 42, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[3] = new List<PValue>();
            results[3].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[3].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[3].Add(new PValue(3, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[3].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));
                
            return results;
        }

    }
    class TestData_MFOPC2Minute
    {
        public static List<PValue>[] SimuData()
        {
            List<PValue>[] results = new List<PValue>[3];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);


            results[0] = new List<PValue>();
            results[0].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 00, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 1));
            results[0].Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(42, new DateTime(2016, 1, 1, 12, 14, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(43, new DateTime(2016, 1, 1, 12, 18, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(44, new DateTime(2016, 1, 1, 12, 30, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 40, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
           

            results[1] = new List<PValue>();
            results[1].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 00, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 1));
            results[1].Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));

            results[2] = new List<PValue>();
            results[2].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 00, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[2].Add(new PValue(41, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));

            return results;
        }

    }
    class TestData_MMultiAnalogSub
    {
        public static List<PValue>[] SimuData()
        {
            List<PValue>[] results = new List<PValue>[4];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);


            results[0] = new List<PValue>();
            results[0].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[0].Add(new PValue(20, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(33, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(27, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(96, new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), 0));
            results[0].Add(new PValue(66, new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(88, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(93, new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(22, new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(43, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[1] = new List<PValue>();
            results[1].Add(new PValue(25, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));
            results[1].Add(new PValue(28, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results[1].Add(new PValue(29, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results[1].Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(33, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0));
            results[1].Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[1].Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[1].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(67, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[1].Add(new PValue(69, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[2] = new List<PValue>();
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 42, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[3] = new List<PValue>();
            results[3].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[3].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[3].Add(new PValue(3, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[3].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            return results;
        }

    }
    class TestData_MMultiAnalogAvg
    {
        public static List<PValue>[] SimuData1()
        {
            List<PValue>[] results = new List<PValue>[6];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);


            results[0] = new List<PValue>();
            results[0].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), 0));
            results[0].Add(new PValue(3, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), 0));

            results[1] = new List<PValue>();
            results[1].Add(new PValue(25, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), 0));
            results[1].Add(new PValue(22, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), 0));

            results[2] = new List<PValue>();
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            results[2].Add(new PValue(41, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));

            results[3] = new List<PValue>();
            results[3].Add(new PValue(33, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 5));       //测试状态位为5
            results[3].Add(new PValue(36, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));

            results[4] = new List<PValue>();    //测试count=0

            results[5] = null;                  //测试为空

            return results;
        }
        public static List<PValue>[] SimuData2()
        {
            List<PValue>[] results = new List<PValue>[4];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);


            results[0] = new List<PValue>();
            results[0].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[0].Add(new PValue(20, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(65, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(33, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(27, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(96, new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[0].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), 0));
            results[0].Add(new PValue(66, new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[0].Add(new PValue(88, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), 0));
            results[0].Add(new PValue(93, new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), 0));
            results[0].Add(new PValue(22, new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[0].Add(new PValue(43, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[1] = new List<PValue>();
            results[1].Add(new PValue(25, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));
            results[1].Add(new PValue(28, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results[1].Add(new PValue(29, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results[1].Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(33, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0));
            results[1].Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[1].Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[1].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(67, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[1].Add(new PValue(69, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[2] = new List<PValue>();
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 4, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 17, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 19, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 22, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 23, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 41, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 41, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 42, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 43, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[2].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 48, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            results[3] = new List<PValue>();
            results[3].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), 0));
            results[3].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 45, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 0, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 5, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[3].Add(new PValue(3, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 25, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 35, DateTimeKind.Local), 0));
            results[3].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 11, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[3].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

            return results;
        }

    }
    class TestData_MAnalogReadCurrent
    {
        public static List<PValue>[] SimuData()
        {
            List<PValue>[] results = new List<PValue>[4];
            int iSeed = Convert.ToInt32(DateTime.Now.Millisecond);


            results[0] = new List<PValue>();
            results[0].Add(new PValue(1, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 10, DateTimeKind.Local), 0));
           
            results[1] = new List<PValue>();
            results[1].Add(new PValue(25, new DateTime(2016, 1, 1, 12, 10, 1, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), 0));
            results[1].Add(new PValue(28, new DateTime(2016, 1, 1, 12, 10, 2, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), 0));
            results[1].Add(new PValue(29, new DateTime(2016, 1, 1, 12, 10, 3, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), 0));
            results[1].Add(new PValue(30, new DateTime(2016, 1, 1, 12, 10, 8, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(33, new DateTime(2016, 1, 1, 12, 10, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), 0));
            results[1].Add(new PValue(38, new DateTime(2016, 1, 1, 12, 10, 21, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), 0));
            results[1].Add(new PValue(40, new DateTime(2016, 1, 1, 12, 10, 25, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), 0));
            results[1].Add(new PValue(45, new DateTime(2016, 1, 1, 12, 10, 35, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), 0));
            results[1].Add(new PValue(60, new DateTime(2016, 1, 1, 12, 10, 55, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), 0));
            results[1].Add(new PValue(67, new DateTime(2016, 1, 1, 12, 11, 15, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), 0));
            results[1].Add(new PValue(69, new DateTime(2016, 1, 1, 12, 11, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 49, DateTimeKind.Local), 0));

         
            return results;
        }
    }
    class TestData_SpanPValue
    {
        //20170322,用于测试条件分割             
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            return results;
        }
        public static List<PValue> simuData2()
        {
            List<PValue> results = new List<PValue>();
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            return results;
        }

        public static List<PValue> simuData3()
        {
            List<PValue> results = new List<PValue>();
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //测试第一个点处理
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            return results;
        }

        public static List<PValue> simuData4()
        {
            List<PValue> results = new List<PValue>();
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //测试第一个点处理
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(270000, new DateTime(2016, 1, 1, 12, 03, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 30, DateTimeKind.Local), 0));   //测试H连续
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(320000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 20, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(270000, new DateTime(2016, 1, 1, 12, 28, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), 0));
            //results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            return results;
        }
    }
    class TestData_SpanPValue4RTDB
    {
        //20170322,用于测试条件分割             
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));//截止时刻值
            return results;
        }

        public static List<PValue> simuData2()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));   //测试第一个点处理
           
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));//截止时刻值
            return results;
        }
        public static List<PValue> simuData3()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0));  
            results.Add(new PValue(60, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));   
            results.Add(new PValue(30, new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            return results;
        }

        //20170322,用于测试条件分割             
        public static List<PValue> simuData4()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(0, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 30, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 01, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 31, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(2, new DateTime(2016, 1, 1, 12, 01, 31, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 32, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(3, new DateTime(2016, 1, 1, 12, 01, 32, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 45, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(4, new DateTime(2016, 1, 1, 12, 01, 45, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 10, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(5, new DateTime(2016, 1, 1, 12, 02, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(6, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(7, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(8, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(9, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(9, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(10, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(100, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(20, new DateTime(2016, 1, 1, 12, 59, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));//截止时刻值
            return results;
        }
    }

    class TestData_SpanPValue4RDB
    {
        //20170322,用于测试条件分割             
        public static List<PValue> simuData1()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //测试第一个点处理
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //测试H边界,以及N连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //测试N越到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), 0));   //测试H连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 04, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), 0));   //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 05, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), 0));   //测试L到H的死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 06, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 59, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));//截止时刻值
            return results;
        }

        public static List<PValue> simuData2()
        {
            List<PValue> results = new List<PValue>();
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 12, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 00, 30, DateTimeKind.Local), 0));   //第1分钟分为2个点
            results.Add(new PValue(2, new DateTime(2016, 1, 1, 12, 00, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), 0));   //第1分钟分为2个点
            results.Add(new PValue(3, new DateTime(2016, 1, 1, 12, 01, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 10, DateTimeKind.Local), 0));   //第2分钟分为6个点
            results.Add(new PValue(4, new DateTime(2016, 1, 1, 12, 01, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 20, DateTimeKind.Local), 0));   //第2分钟分为6个点
            results.Add(new PValue(5, new DateTime(2016, 1, 1, 12, 01, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 30, DateTimeKind.Local), 0));   //第2分钟分为6个点
            results.Add(new PValue(6, new DateTime(2016, 1, 1, 12, 01, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 40, DateTimeKind.Local), 0));   //第2分钟分为6个点
            results.Add(new PValue(7, new DateTime(2016, 1, 1, 12, 01, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 01, 50, DateTimeKind.Local), 0));   //第2分钟分为6个点
            results.Add(new PValue(8, new DateTime(2016, 1, 1, 12, 01, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 00, DateTimeKind.Local), 0));   //第2分钟分为6个点
            results.Add(new PValue(9, new DateTime(2016, 1, 1, 12, 02, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 20, DateTimeKind.Local), 0));   //第3分钟缺起始点
            results.Add(new PValue(10, new DateTime(2016, 1, 1, 12, 02, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 30, DateTimeKind.Local), 0));   //第3分钟缺起始点
            results.Add(new PValue(11, new DateTime(2016, 1, 1, 12, 02, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 40, DateTimeKind.Local), 0));   //第3分钟缺起始点
            results.Add(new PValue(12, new DateTime(2016, 1, 1, 12, 02, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 02, 50, DateTimeKind.Local), 0));   //第3分钟缺起始点
            results.Add(new PValue(13, new DateTime(2016, 1, 1, 12, 02, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), 0));   //第3分钟缺起始点
            results.Add(new PValue(14, new DateTime(2016, 1, 1, 12, 03, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 10, DateTimeKind.Local), 0));   //第4分钟缺结束点
            results.Add(new PValue(15, new DateTime(2016, 1, 1, 12, 03, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 20, DateTimeKind.Local), 0));   //第4分钟缺结束点
            results.Add(new PValue(16, new DateTime(2016, 1, 1, 12, 03, 20, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 30, DateTimeKind.Local), 0));   //第4分钟缺结束点
            results.Add(new PValue(17, new DateTime(2016, 1, 1, 12, 03, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 40, DateTimeKind.Local), 0));   //第4分钟缺结束点
            results.Add(new PValue(18, new DateTime(2016, 1, 1, 12, 03, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 03, 50, DateTimeKind.Local), 0));   //第5、6分钟连续缺点
            results.Add(new PValue(19, new DateTime(2016, 1, 1, 12, 06, 10, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 20, DateTimeKind.Local), 0));   //第7分钟，缺收尾点，中间点
            results.Add(new PValue(20, new DateTime(2016, 1, 1, 12, 06, 30, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 40, DateTimeKind.Local), 0));   //第5、6分钟连续缺点
            results.Add(new PValue(21, new DateTime(2016, 1, 1, 12, 06, 40, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 06, 50, DateTimeKind.Local), 0));   //第5、6分钟连续缺点            
            results.Add(new PValue(22, new DateTime(2016, 1, 1, 12, 07, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(10, new DateTime(2016, 1, 1, 12, 08, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), 0));   //测试H到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 09, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 10, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), 0)); //测试N到H
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 11, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), 0)); //测试H到HH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 12, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), 0)); //测试HH边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 13, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 14, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), 0)); //测试HH到HHH
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 15, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), 0)); //测试HHH到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 16, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 17, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), 0)); //测试L边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 18, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 19, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), 0)); //测试L连续
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 20, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), 0)); //测试LL边界
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 21, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), 0)); //测试L到N死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 22, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 23, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), 0)); //测试L到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 24, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), 0)); //测试N到L
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 25, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), 0)); //测试L到LL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 26, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), 0)); //测试LL到LLL
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 27, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), 0)); //测试LLL死区
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 28, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 29, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), 0)); //测试LLL到N
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 30, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 31, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 32, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 33, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 34, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 35, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 36, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 37, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 38, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 39, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 40, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 41, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 42, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 43, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 44, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 45, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 46, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 47, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 48, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 49, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 50, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 51, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 52, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 53, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 54, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 55, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(60000, new DateTime(2016, 1, 1, 12, 56, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(100, new DateTime(2016, 1, 1, 12, 57, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(50, new DateTime(2016, 1, 1, 12, 58, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 12, 59, 50, DateTimeKind.Local), 0));
            results.Add(new PValue(20, new DateTime(2016, 1, 1, 12, 59, 50, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));
            results.Add(new PValue(1, new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), new DateTime(2016, 1, 1, 13, 00, 00, DateTimeKind.Local), 0));//截止时刻值
            return results;
        }
       
    }
}
