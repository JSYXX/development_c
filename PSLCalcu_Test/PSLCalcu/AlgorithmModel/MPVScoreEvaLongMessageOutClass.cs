using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.AlgorithmModel
{
    public class MPVScoreEvaLongMessageOutClass
    {
        public string SftMScoreMin { get; set; }
        public string SftMScoreMinT { get; set; }
        public string SftMScoreMax { get; set; }
        public string SftMScoreMaxT { get; set; }
        public string SftMScoreAvg { get; set; }
        public string SftMScoreAvgP { get; set; }
        public string SftMScoreTotal { get; set; }
        public string SftMScoreTotalP { get; set; }
        public string SftMScoreHighT { get; set; }
        public string SftMScoreHighTR { get; set; }
        public string SftMScoreHighST { get; set; }
        public D22STimeClass SftMScoreHighSTTime { get; set; }
        public string SftMScoreLowT { get; set; }
        public string SftMScoreLowTR { get; set; }
        public string SftMScoreLowST { get; set; }
        public D22STimeClass SftMScoreLowSTTime { get; set; }
        public string SftMScoreSSHigh { get; set; }
        public D22STimeClass SftMScoreSSHighTime { get; set; }
        public string SftMScoreSSLow { get; set; }
        public D22STimeClass SftMScoreSSLowTime { get; set; }
        public string SftMScoreSSAvg { get; set; }
        public string SftMScoreEvaExlN { get; set; }
        public string SftMScoreEvaGoodN { get; set; }
        public string SftMScoreEvaOKN { get; set; }
        public string SftMScoreEvaOtrN { get; set; }
        public string SftMWorkT { get; set; }
        public string SftMWorkN { get; set; }

        public List<MPVScoreEvaShortMessageOutClass> SftScoreAvgOrd { get; set; }
        public List<MPVScoreEvaShortMessageOutClass> SftScoreTotalOrd { get; set; }
        public List<MPVScoreEvaShortMessageOutClass> SftScoreHighTROrd { get; set; }
        public List<MPVScoreEvaShortMessageOutClass> SftScoreHighSTOrd { get; set; }
        public List<MPVScoreEvaShortMessageOutClass> SftScoreLowTROrd { get; set; }
        public List<MPVScoreEvaShortMessageOutClass> SftScoreLowSTOrd { get; set; }
        public List<MPVScoreEvaShortMessageOutClass> SftMScoreTotalPOrd { get; set; }

    }
}
