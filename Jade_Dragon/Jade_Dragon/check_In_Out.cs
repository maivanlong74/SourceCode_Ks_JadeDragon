using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jade_Dragon.common
{
    public class check_In_Out
    {
        public static int check(DateTime checkInDate, DateTime checkOutDate)
        {
            TimeSpan ts = checkOutDate - checkInDate;
            return (int)Math.Ceiling(ts.TotalDays);
        }

    }
}