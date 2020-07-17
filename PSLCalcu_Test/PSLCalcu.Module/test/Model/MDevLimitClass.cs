using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MDevLimitClass
    {
        public string DevN { get; set; }
        public List<D22STimeClass> DevTime { get; set; }
        public string DevT { get; set; }
        public string DevR { get; set; }
        public string DevTMax { get; set; }
        public D22STimeClass DevTMaxTime { get; set; }
        public string DevA { get; set; }
        public string DevET { get; set; }
    }
}
