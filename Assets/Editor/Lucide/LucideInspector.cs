using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Lucide;

namespace Editor.Lucide
{
    [CustomEditor(typeof(GlyphData))]
    public class LucideInspector : UnityEditor.Editor
    {
        static readonly char[] Trim = { '&', '#' };
        public VisualTreeAsset inspectorUxml;
        public Font iconFont;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            if (!inspectorUxml)
                return root;

            VisualElement uxmlContent = inspectorUxml.CloneTree();
            root.Add(uxmlContent);

            var glyph = root.Q<Label>("glyphLabel");
            var input = root.Q<TextField>("unicodeInput");

            // initial set (in case serialized value already present)
            UpdateGlyph(glyph);

            input.RegisterValueChangedCallback(evt => UpdateGlyph(glyph));

            // refresh on Undo/Redo
            Undo.undoRedoPerformed += () => UpdateGlyph(glyph);

            // optionally assign a font that contains the glyph
            if (iconFont) glyph.style.unityFont = iconFont;

            return root;
        }

        void UpdateGlyph(Label glyph)
        {
            var iconID = serializedObject.FindProperty("iconID").stringValue;

            if (!int.TryParse(iconID.TrimStart(Trim), out var sanitizedIconID)) return;
            sanitizedIconID += 0; //offset

            var unicode = sanitizedIconID.ToString("X");
            serializedObject.FindProperty("unicodeString").stringValue = unicode;

            glyph.text = unicode == "" ? "�" : GlyphData.UnicodeToChar(unicode);
        }
    }
}
