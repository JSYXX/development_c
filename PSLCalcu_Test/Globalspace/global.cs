using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globalspace
{
    public static class global
    {
        private static List<string> _caculateFunction = new List<string>();

        public static List<string> caculateFunction
        {
            get
            {
                return _caculateFunction;
            }

            set
            {
                _caculateFunction = value;
            }
        }
    }
}
