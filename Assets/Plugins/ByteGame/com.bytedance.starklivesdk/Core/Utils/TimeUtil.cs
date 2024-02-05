using System;

namespace StarkLive
{
    public class TimeUtil
    {
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static int GetUtcSeconds()
        {
            return DateTimeToSeconds(DateTime.Now);
        }

        public static long GetUtcMilliSeconds()
        {
            return DateTimeToMilliSeconds(DateTime.Now);
        }

        public static int DateTimeToSeconds(DateTime t)
        {
            return (int) (t.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        public static long DateTimeToMilliSeconds(DateTime t)
        {
            return (long) (t.ToUniversalTime() - UnixEpoch).TotalMilliseconds;
        }

        public static DateTime MilliSecondsToDateTime(long milliSeconds)
        {
            return UnixEpoch.AddMilliseconds(milliSeconds).ToLocalTime();
        }

        public static DateTime SecondsToDateTime(long seconds)
        {
            return UnixEpoch.AddSeconds(seconds).ToLocalTime();
        }
    }
}