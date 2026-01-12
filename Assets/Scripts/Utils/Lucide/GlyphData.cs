using System.Globalization;
using UnityEngine;
using Utils.Types;

namespace Utils.Lucide
{
    [CreateAssetMenu(fileName = "glyph", menuName = "GlyphPrefab", order = 0)]
    public class GlyphData : BetterScriptableObject
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

        protected override void HandleChanged()
        {
        }
    }
}
