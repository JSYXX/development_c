using PSLCalcu.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class ModuleTest
    {
        private BaseModule module;
        public BaseModule Module                      //工业数据实时值、计算值数据有效周期            
        {
            get
            {
                return module;
            }
            set                                       //2017.6.22，arrow修改，将Timespan改为只读属性，不能从外部设置该属性。
            {
                module = value;                    
            }
        }
    }
}
