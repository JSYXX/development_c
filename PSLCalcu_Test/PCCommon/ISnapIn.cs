using System;
using System.Collections.Generic;

namespace PCCommon
{
    public interface ISnapIn
    {
        string Version { get; }
        List<PCModule> GetModuleInfo();
        //void GetClassName();
        //void GetMethods();
    }
}
