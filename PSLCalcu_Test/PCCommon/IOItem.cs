using System;
using System.Collections.Generic;

namespace PCCommon
{
    public class IOItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Dimension { get; set; }
        public string Type { get; set; }
        public int Order { get; set; }
        public PValue PV { get; set; }
        public List<PValue> PV_Array { get; set; }
        public string SourceTag { get; set; }
        //public PValue PV_Out { get; set; }
        //public List<PValue> PV_Out_Array { get; set; }

        public IOItem(string name, string description, string dimension, string type, int order, double defaultvalue)
        {
            this.Name = name;
            this.Description = description;
            this.Dimension = dimension;
            this.Type = type;
            this.Order = order;
            this.PV = new PValue(defaultvalue, DateTime.Now, 0);
            this.PV_Array = new List<PValue>();
            //this.PV_Out = new PValue(0, DateTime.Now, 0);
            //this.PV_Out_Array = new List<PValue>();
        }
        public IOItem(IOItem other)
        {
            this.Name = other.Name;
            this.Description = other.Description;
            this.Dimension = other.Dimension;
            this.Type = other.Type;
            this.Order = other.Order;
            this.PV = other.PV;
            this.PV_Array = other.PV_Array;
            //this.PV_Out = other.PV_Out;
            //this.PV_Out_Array = other.PV_Out_Array;
        }
    }
}
