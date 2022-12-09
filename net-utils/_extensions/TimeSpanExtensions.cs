using System;

namespace agaertner.NetUtils
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Returns a string representing the specified time span in short form.
        /// </summary>
        /// <param name="t">Target time span.</param>
        /// <returns>Short representation of the time span.</returns>
        public static string ToShortForm(this TimeSpan t)
        {
            return (t.Hours > 0 ? $"{t.Hours}:" : string.Empty) + t.ToString(t.Minutes > 9 ? "mm\\:ss" : "m\\:ss");
        }
    }
}
