using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.AlgorithmModel
{
    public class MMultiPVAvgDistanceMessageOutClass
    {
        public List<MPVBaseMessageInBadClass> MultiPVAvgDistance { get; set; }
        public string MultiPVAvgDistanceMin { get; set; }
        public string MultiPVAvgDistanceMinTime { get; set; }
        public string MultiPVAvgDistanceMax { get; set; }
        public string MultiPVAvgDistanceMaxTime { get; set; }
        public string MultiPVAvgDistanceAvg { get; set; }
        public string MultiPVAvgDistanceStdev { get; set; }
        public string MultiPVAvgDistanceLT { get; set; }
        public string MultiPVAvgDistanceLTR { get; set; }
        public string MultiPVAvgDistanceHT { get; set; }
        public string MultiPVAvgDistanceHTR { get; set; }

    }
}
