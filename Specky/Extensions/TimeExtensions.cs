using System;

namespace Specky.Extensions
{
    public static class TimeExtensions
    {
        /// <summary>
        /// Returns a boolean value indicating if the DateTime object is past the current time.
        /// </summary>
        /// <param name="dateTime">The DateTime object compared against.</param>
        /// <param name="differenceInTime">The TImeSpan used to compare the difference in time.</param>
        /// <returns>true if the DateTime is past the current time, including the difference in time.</returns>
        public static bool IsNowPast(this DateTime dateTime, TimeSpan differenceInTime)
        {
            return dateTime.Add(differenceInTime) < DateTime.Now;
        }

        /// <summary>
        /// Returns a boolean value indicating if the DateTime object is past the current time.
        /// </summary>
        /// <param name="dateTime">The DateTime object compared against.</param>
        /// <param name="differenceInTimeMS">The value in milliseconds used to compare the difference in time.</param>
        /// <returns>true if the DateTime is past the current time, including the difference in time.</returns>
        public static bool IsNowPast(this DateTime dateTime, int differenceInTimeMS)
        {
            return dateTime.AddMilliseconds(differenceInTimeMS) < DateTime.Now;
        }
    }
}
