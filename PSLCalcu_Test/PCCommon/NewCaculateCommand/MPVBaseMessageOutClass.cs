using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCCommon.NewCaculateCommand
{
    public class MPVBaseMessageOutClass
    {
        public int id { get; set; }
        public double PVBMin { get; set; }
        public string PVBMinTime { get; set; }
        public double PVBAvg { get; set; }
        public double PVBMax { get; set; }
        public string PVBMaxTime { get; set; }
        public double PVBDMax { get; set; }
        public double PVBSum { get; set; }
        public double PVBSumkb { get; set; }
        public double PVBLinek { get; set; }
        public double PVBLineb { get; set; }
        public double PVBSumPNR { get; set; }
        public double PVBAbsSum { get; set; }
        public double PVBStdev { get; set; }
        public double PVBVolatility { get; set; }
        public double PVBSDMax { get; set; }
        public D22STimeClass PVBSDMaxTime { get; set; }
        public double PVBSDMaxR { get; set; }
        public int PVBDN1Num { get; set; }
        public int PVBDN2Num { get; set; }
        public int PVBDN3Num { get; set; }
        public int PVBTNum { get; set; }
        public double PVBVMax { get; set; }
        public double PVBVMin { get; set; }
        public double PVBVAvg { get; set; }
        public double PVBStbTR { get; set; }
        public double PVBNoStbTR { get; set; }
        public double PVBStbTSL { get; set; }
        public double PVBStbTSLR { get; set; }
        public double PVBNoStbTSL { get; set; }
        public double PVBNoStbTSLR { get; set; }
        public double PVBUpTSL { get; set; }
        public double PVBUpTSLR { get; set; }
        public double PVBDownTSL { get; set; }
        public double PVBDownTSLR { get; set; }
        public double PVBPNum { get; set; }
        public double PVBQltR { get; set; }
        public string PVBStatus { get; set; }

        public double PVBQa { get; set; }
        public double PVBQb { get; set; }
        public double PVBQc { get; set; }

        public List<D22STimeClass> PVBStbT { get; set; }
        public List<D22STimeClass> PVBNoStbT { get; set; }

        public double PVBStbTSLPV { get; set; }

        public D22STimeClass PVBStbTSLT { get; set; }
        public D22STimeClass PVBNoStbTSLT { get; set; }

        public D22STimeClass PVBUpTSLT { get; set; }
        public D22STimeClass PVBDownTSLT { get; set; }

        public List<D22STimeClass> PVBSDMaxTimeG { get; set; }
    }
}
