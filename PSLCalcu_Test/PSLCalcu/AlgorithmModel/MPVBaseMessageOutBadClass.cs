using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.AlgorithmModel
{
    public class MPVBaseMessageOutBadClass
    {
        public string PVBMin { get; set; }
        public string PVBMinTime { get; set; }
        public string PVBAvg { get; set; }
        public string PVBMax { get; set; }
        public string PVBMaxTime { get; set; }
        public string PVBDMax { get; set; }
        public string PVBSum { get; set; }
        public string PVBSumkb { get; set; }
        public string PVBLinek { get; set; }
        public string PVBLineb { get; set; }
        public string PVBSumPNR { get; set; }
        public string PVBAbsSum { get; set; }
        public string PVBStdev { get; set; }
        public string PVBVolatility { get; set; }
        public string PVBSDMax { get; set; }
        public D22STimeClass PVBSDMaxTime { get; set; }
        public string PVBSDMaxR { get; set; }
        public string PVBDN1Num { get; set; }
        public string PVBDN2Num { get; set; }
        public string PVBDN3Num { get; set; }
        public string PVBTNum { get; set; }
        public string PVBVMax { get; set; }
        public string PVBVMin { get; set; }
        public string PVBVAvg { get; set; }
        public string PVBStbTR { get; set; }
        public string PVBNoStbTR { get; set; }
        public string PVBStbTSL { get; set; }
        public string PVBStbTSLR { get; set; }
        public string PVBNoStbTSL { get; set; }
        public string PVBNoStbTSLR { get; set; }
        public string PVBUpTSL { get; set; }
        public string PVBUpTSLR { get; set; }
        public string PVBDownTSL { get; set; }
        public string PVBDownTSLR { get; set; }
        public string PVBPNum { get; set; }
        public string PVBQltR { get; set; }
        public string PVBStatus { get; set; }
        public string PVBQa { get; set; }
        public string PVBQb { get; set; }
        public string PVBQc { get; set; }

        public List<D22STimeClass> PVBStbT { get; set; }
        public List<D22STimeClass> PVBNoStbT { get; set; }

        public string PVBStbTSLPV { get; set; }

        public D22STimeClass PVBStbTSLT { get; set; }
        public D22STimeClass PVBNoStbTSLT { get; set; }

        public D22STimeClass PVBUpTSLT { get; set; }
        public D22STimeClass PVBDownTSLT { get; set; }

        public List<D22STimeClass> PVBSDMaxTimeG { get; set; }
    }
}
