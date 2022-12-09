using Core.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Helpers
{
    public static class DateTimeWithZone
    {

        private static readonly TimeZoneInfo timeZone;

        static DateTimeWithZone()
        {
            //I added web.config <add key="CurrentTimeZoneId" value="Central Europe Standard Time" />
            //You can add value directly into function.
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(CommonConstants.TimeZoneConfig);
        }


        public static DateTime LocalTime(this DateTime t)
        {
            return t;
            //return TimeZoneInfo.ConvertTime(t, timeZone);
        }

        public static DateTime ToSpecificTimeZone(this DateTime t)
        {
            return t;
            //return TimeZoneInfo.ConvertTime(t, timeZone);
        }
    }
}
