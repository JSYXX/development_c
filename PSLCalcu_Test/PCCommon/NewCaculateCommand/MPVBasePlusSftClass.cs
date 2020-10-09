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
    }
}
