using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.AlgorithmModel
{
    public class MPVTypRangeMessageInClass
    {
        public List<List<MPVBaseMessageInBadClass>> minuValueList { get; set; }
        public List<List<MPVBaseMessageInBadClass>> hourMaxValueList { get; set; }
        public List<List<MPVBaseMessageInBadClass>> hourMinValueList { get; set; }
        public List<List<MPVBaseMessageInBadClass>> hourAvgValueList { get; set; }
    }
}
