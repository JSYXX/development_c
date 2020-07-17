using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu
{
    public class PSLModule
    {
        public int id { get; set; }                        
        public string modulename { get; set; }
        public string moduledesc { get; set; }
        public string moduleclass { get; set; }
        public string modulealgorithms { get; set; }
        public string modulealgorithmsflag { get; set; }
        public string moduleparaexample { get; set; }
        public string moduleparadesc { get; set; }
        public string moduleoutputtable { get; set; }
        public int moduleoutputnumber { get; set; }
        public string moduleoutputtype { get; set; }
        public string moduleoutputdescs { get; set; }
        public string moduleoutputdescscn { get; set; }
        public bool moduleoutputpermitnull { get; set; }

    }
}
