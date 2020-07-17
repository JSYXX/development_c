using System;
using System.Collections.Generic;

namespace PCCommon
{
    public class PCModule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int OperationMode { get; set; }  //运行模式：0不运算，1运算，2不考虑输入输出为0，...
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public List<IOItem> ItemIn { get; set; }
        public List<IOItem> ItemOut { get; set; }
        public string BaseFile { get; set; }
        public string ClassName { get; set; }
        public string AliasName { get; set; }

        private object[] _modulevalues;
        public object[] ModuleValues
        {
            get
            {
                _modulevalues = new Object[2];
                List<PValue> pv_in = new List<PValue>();
                List<PValue> pv_out = new List<PValue>();
                foreach (IOItem item in this.ItemIn)
                {
                    pv_in.Add(item.PV);
                }
                foreach (IOItem item in this.ItemOut)
                {
                    pv_out.Add(item.PV);
                }
                _modulevalues[0] = pv_in;
                _modulevalues[1] = pv_out;
                return _modulevalues;
            }
            set
            {
                _modulevalues = value;
                for (int i = 0; i < this.ItemIn.Count; i++)
                {
                    this.ItemIn[i].PV = ((List<PValue>)_modulevalues[0])[i];
                }
                for (int i = 0; i < this.ItemOut.Count; i++)
                {
                    this.ItemOut[i].PV = ((List<PValue>)_modulevalues[1])[i];
                }
            }
        }
        
        public PCModule(string name, string description, int inputcount, int outputcount,string aliasname)
        {
            this.Name = name;
            this.Description = description;
            this.InputCount = inputcount;
            this.OutputCount = outputcount;
            this.AliasName = aliasname;
            this.OperationMode = 1;
            this.ItemIn = new List<IOItem>(inputcount);
            this.ItemOut = new List<IOItem>(outputcount);
        }
        public PCModule(PCModule other)
        {
            this.Name = other.Name;
            this.Description = other.Description;
            this.OperationMode = other.OperationMode;
            this.InputCount = other.InputCount;
            this.OutputCount = other.OutputCount;
            this.ItemIn = other.ItemIn;
            this.ItemOut = other.ItemOut;
            this.BaseFile = other.BaseFile;
            this.ClassName = other.ClassName;
            this.AliasName = other.AliasName;
        }

        private object[] GetValueArray()
        {
            ModuleValues = new Object[2];
            List<PValue> pv_in = new List<PValue>();
            List<PValue> pv_out = new List<PValue>();
            foreach (IOItem item in this.ItemIn)
            {
                pv_in.Add(item.PV);
            }
            foreach (IOItem item in this.ItemOut)
            {
                pv_out.Add(item.PV);
            }
            ModuleValues[0] = pv_in;
            ModuleValues[1] = pv_out;
            return ModuleValues;
        }
        private void SetValueArray()
        {
            for (int i = 0; i < this.ItemIn.Count; i++)
            {
                this.ItemIn[i].PV = ((List<PValue>)ModuleValues[0])[i];
            }
            for (int i = 0; i < this.ItemOut.Count; i++)
            {
                this.ItemOut[i].PV = ((List<PValue>)ModuleValues[1])[i];
            }
        }
    }
}
