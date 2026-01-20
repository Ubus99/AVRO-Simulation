using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using Utils.Lucide;
using Utils.Objects;

namespace UI
{
    [CreateAssetMenu(fileName = "IconAtlas", menuName = "custom/IconAtlas", order = 0)]
    public class IconAtlas : ScriptableSingleton<IconAtlas>
    {
        [SerializeField]
        List<GlyphData> glyphsList = new();

        public SerializedDictionary<string, GlyphData> glyphs { get; } = new();

        void Awake()
        {
            ServiceLocator.instance.TryRegister<IconAtlas>(this);
        }

        void OnValidate()
        {
            glyphsList = glyphsList.OrderBy(data => data.name).ToList();
            glyphs.Clear();
            foreach (var glyph in glyphsList)
            {
                glyphs.Add(glyph.name, glyph);
            }
        }
    }
}
