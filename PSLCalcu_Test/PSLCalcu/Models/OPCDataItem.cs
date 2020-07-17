using System;
namespace PSLCalcu
{
    public class OPCDataItem
    {
        public uint opctagid { get; set; }
        public long opctagstarttime { get; set; }
        public long opctagendtime { get; set; }
        public double opctagvalue { get; set; }
        public long opctagstatus { get; set; }

    }
}
