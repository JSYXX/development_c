using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu
{
    public class ShiftItem
    {
        public int id { get; set; }
        public DateTime shiftstarttime { get; set; }
        public DateTime shiftendtime { get; set; }
        public int shiftworktime { get; set; }
        public int shiftindex { get; set; }
        public string shiftgroupname { get; set; }
        public string shiftmonitorname { get; set; }
        public string shiftmonitorcomm { get; set; }
        public string shiftvicemonitorname { get; set; }
        public string shiftvicemonitorcomm { get; set; }
    }
}
