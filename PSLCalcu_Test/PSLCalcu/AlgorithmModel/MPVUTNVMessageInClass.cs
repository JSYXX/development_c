using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.AlgorithmModel
{
    public class MPVUTNVMessageInClass
    {
        public List<MPVUTNVMessageInBadClass> monthValuesList { get; set; }
        public List<MPVUTNVMessageInBadClass> dayValuesList { get; set; }
        public List<MPVUTNVMessageInBadClass> hourValuesList { get; set; }
        public List<MPVUTNVMessageInBadClass> valuesList { get; set; }
        public int PVMode { get; set; }

    }
}
