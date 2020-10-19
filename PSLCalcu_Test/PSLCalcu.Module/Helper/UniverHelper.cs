using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module.Helper
{
    public static class UniverHelper
    {
        public static long ConvertDateTimeTolong(string datetime)
        {

            try
            {
                return Convert.ToDateTime(datetime).Ticks - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")).Ticks;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
