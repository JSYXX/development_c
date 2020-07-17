using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PSLCalcu
{
    public class PSLTagNameIdMapItem
    {
        public System.UInt32 psltagid { get; set; }
        public int psltagsaveflag { get; set; }
        public string psltagname { get; set; }
        public int pslcalcuconfigindex { get; set; }
        public string psltagdesc { get; set; }
        public string outputtablename { get; set; }
    }
}

