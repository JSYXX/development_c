using System;
using System.Collections.Generic;
namespace PSLCalcu
{
    public class OPCTagIdMap
    {
        //public int opctagid { get; set; }                         //
        public System.UInt32 opctagindex { get; set; }              //数据库中类型为mediumint unsigned，这里必须是 System.UInt32。否则数据读不出来
        public System.UInt32 opcserverindex { get; set; }           //数据库中类型为mediumint unsigned，这里必须是 System.UInt32。否则数据读不出来
        public string opctagname { get; set; }
        public string opctagdesc { get; set; }
        public string opctagdatatype { get; set; }
        public string opcdbtagname { get; set; }

    }
}
