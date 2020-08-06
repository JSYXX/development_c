using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.AlgorithmModel
{
    public class MPVScoreEvaShortMessageOutClass
    {
        public int id { get; set; }
        public string SftScoreMin { get; set; }
        public string SftScoreMinT { get; set; }
        public string SftScoreMax { get; set; }
        public string SftScoreMaxT { get; set; }
        public string SftScoreAvg { get; set; }
        public string SftScoreAvgP { get; set; }
        public string SftScoreTotal { get; set; }
        public string SftScoreTotalP { get; set; }
        public string SftScoreHighT { get; set; }
        public string SftScoreHighTR { get; set; }
        public string SftScoreHighST { get; set; }
        public D22STimeClass SftScoreHighSTTime { get; set; }
        public string SftScoreLowT { get; set; }
        public string SftScoreLowTR { get; set; }
        public string SftScoreLowST { get; set; }
        public D22STimeClass SftScoreLowSTTime { get; set; }
        public string SftScoreEva { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double totalHour { get; set; }

    }
}
