using System;
using System.Collections.Generic;
using System.Text;

namespace TooSimple.Poco.Extensions
{
    public static class DateTimeToCentralExtensions
    {
        public static DateTime DateToCentral(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }
    }
}
