using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TooSimple.Poco.Extensions
{
    public static class DateTimeToCentralExtensions
    {
        public static DateTime DateToCentral(this DateTime dateTime)
        {
            TimeZoneInfo timezoneId;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                timezoneId = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                timezoneId = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                timezoneId = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
            }
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }
    }
}
