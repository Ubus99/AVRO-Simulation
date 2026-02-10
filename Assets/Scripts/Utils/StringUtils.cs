using System.Linq;
using System.Text.RegularExpressions;

namespace Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Converts camel case to sentence case. thank you, ChatGPT
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSentenceCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input ?? string.Empty;

            // normalize separators
            var s = input.Trim();
            s = Regex.Replace(s, @"[_\-]+", " ");

            // insert spaces between lower->Upper and acronym boundaries (e.g. "XMLParser" -> "XML Parser")
            s = Regex.Replace(s, @"([a-z0-9])([A-Z])", "$1 $2");
            s = Regex.Replace(s, @"([A-Z])([A-Z][a-z])", "$1 $2");

            // collapse multiple spaces
            s = Regex.Replace(s, @"\s+", " ").Trim();

            if (s.Length == 0) return s;

            // preserve all-uppercase tokens (acronyms), lowercase others
            var words = s.Split(' ')
                .Select(w => w.All(char.IsUpper) ? w : w.ToLowerInvariant());

            var sentence = string.Join(" ", words);
            return char.ToUpperInvariant(sentence[0]) + sentence[1..];
        }
    }
}
