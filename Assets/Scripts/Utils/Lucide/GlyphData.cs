using System.Globalization;
using UnityEngine;

namespace Utils.Lucide
{
    [CreateAssetMenu(fileName = "glyph", menuName = "GlyphPrefab", order = 0)]
    public class GlyphData : ScriptableObject
    {
        public int iconID;
        public string unicodeString;

        public static string UnicodeToChar(string hex)
        {
            var code = int.Parse(hex, NumberStyles.HexNumber);
            return UnicodeToChar(code);
        }

        public static string UnicodeToChar(int code)
        {
            var unicodeString = char.ConvertFromUtf32(code);
            return unicodeString;
        }
    }
}
