using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace agaertner.NetUtils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Inserts a whitespace before every uppercase character except the first.
        /// </summary>
        /// <param name="source">Target string.</param>
        /// <returns>A new string.</returns>
        public static string SplitAtUpperCase(this string source)
        {
            return Regex.Replace(source, "([A-Z]|[1-9])", " $1", RegexOptions.Compiled);
        }

        /// <summary>
        /// Retrieves a list of tuples of color and text parts from the string.
        /// </summary>
        /// <param name="input">Target string.</param>
        /// <returns>List of color and text tuples.</returns>
        public static IReadOnlyList<ValueTuple<Color, string>> FetchMarkupColoredText(this string input, Color? regularTextColor = null)
        {
            regularTextColor ??= Color.White;

            if (string.IsNullOrEmpty(input)) {
                return new List<ValueTuple<Color, string>> { new ValueTuple<Color, string>(regularTextColor.Value, input) };
            }

            var colorRegex = new Regex(@"<(c|color)=(#?((?'rgb'[a-fA-F0-9]{6})|(?'argb'[a-fA-F0-9]{8})))?>(?'text'.*?)<\s*\/\s*\1\s*>", RegexOptions.Multiline);

            var lines = new List<ValueTuple<Color, string>>();
            var startIndex = 0;
            foreach (Match m in colorRegex.Matches(input))
            {
                // Current match is not starting at the end of the last match which means there is non-captured text between.
                if (startIndex != m.Index)
                {
                    lines.Add(new ValueTuple<Color, string>(regularTextColor.Value, input.Substring(startIndex, m.Index - startIndex)));
                }
                startIndex = m.Index + m.Length;
                var color = Color.FromArgb(int.Parse(m.Groups["rgb"].Success ? "FF" + m.Groups["rgb"].Value : m.Groups["argb"].Value, NumberStyles.HexNumber));
                lines.Add(new ValueTuple<Color, string>(color, m.Groups["text"].Value));
            }

            // String does not end with the final match which means there is non-captured text remaining.
            if (startIndex != input.Length) {
                lines.Add(new ValueTuple<Color, string>(regularTextColor.Value, input.Substring(startIndex, input.Length - startIndex)));
            }

            return lines;
        }

        /// <summary>
        /// Removes all characters between &#60; and &#62; from a string.
        /// </summary>
        /// <param name="input">Target string.</param>
        /// <returns>String without mark up tags.</returns>
        public static string StripMarkupLazy(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Removes all mark up annotations from a string.
        /// </summary>
        /// <remarks>
        /// A tag will be removed if it has a matching closing tag.<br/>
        /// Example: <c>"&#60;span style="color:blue"&#62;abc&#60;/span&#62;" yields "abc"</c>
        /// </remarks>
        /// <param name="input">Target string.</param>
        /// <returns>String without mark up tags.</returns>
        public static string StripMarkup(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var matchingTags = new Regex("<\\s*([^ >]+)[^>]*>(?'text'.*?)<\\s*\\/\\s*\\1\\s*>", RegexOptions.Multiline);
            while (matchingTags.IsMatch(input))
            {
                var match = matchingTags.Match(input);
                var text = match.Groups["text"].Value;
                input = input.Replace(match.Value, text);
            }
            return input;
        }
    }
}
