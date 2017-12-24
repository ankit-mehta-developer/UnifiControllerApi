using System;

namespace UnifiControllerCommunicator.Helpers
{
    public static class StringExtensions
    {
        public static long? ToNullableLong(this string longText)
        {
            long val;
            if(long.TryParse(longText, out val))
            {
                return val;
            }
            return null;
        }

        public static decimal? ToNullableDecimal(this string decimalText)
        {
            decimal val;
            if (decimal.TryParse(decimalText, out val))
            {
                return val;
            }
            return null;
        }

        public static string ToNullIfNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text) ? null : text;
        }

        public static string ToNullIfNullOrWhitespace(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }

        public static DateTime? ToDateTimeFromUnixTicks(this long? unixTicks)
        {
            if(unixTicks == null)
            {
                return null;
            }
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            return new DateTime(unixTicks.Value * TimeSpan.TicksPerSecond + epochTicks, DateTimeKind.Utc).ToUniversalTime();
        }



        public static string ToLocalTimeText(this DateTime? dateTime)
        {
            if(dateTime == null)
            {
                return null;
            }
            return dateTime.Value.ToLocalTime().ToString(DateTimeFormat);
        }


        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static TEnum? ToNullableEnum<TEnum>(this string text) where TEnum : struct, IConvertible
        {
            if(text == null)
            {
                return null;
            }
            TEnum val;
            if(Enum.TryParse(text, false, out val))
            {
                return val;
            }
            return null;
        }
    }
}
