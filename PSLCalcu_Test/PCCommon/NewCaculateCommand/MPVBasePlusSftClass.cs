using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCCommon.NewCaculateCommand
{
    public class MPVBasePlusSftClass
    {
        public long id { get; set; }
        public string PVBMin { get; set; }
        public string dutyTime { get; set; }
        public string type { get; set; }
        public string PVBMinTime { get; set; }
        public string PVBAvg { get; set; }
        public string PVBMax { get; set; }
        public string PVBMaxTime { get; set; }
        public string PVBSum { get; set; }
        public string PVBSumkb { get; set; }
        public string PVBAbsSum { get; set; }
        public string PVBStbTR { get; set; }
        public string PVBNoStbTR { get; set; }
        public string UpdateTime { get; set; }
        public string EffectiveCount { get; set; }

        public string PVBSDMax { get; set; }
        public string PVBSDMaxTime { get; set; }
        public string PVBDN1Num { get; set; }
        public string PVBDN2Num { get; set; }
        public string PVBDN3Num { get; set; }
        public string PVBTNum { get; set; }
        public string PVBSDSingle { get; set; }
        public string PVBSDSingleTime { get; set; }
        public string PVBSDSingleType { get; set; }
    }
}
