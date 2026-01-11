using System;
using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Editor.Lucide
{
    [CustomEditor(typeof(LucidePicker))]
    public class LucideInspector : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorUxml;
        public Font iconFont;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            if (!inspectorUxml)
                return root;

            VisualElement uxmlContent = inspectorUxml.CloneTree();
            root.Add(uxmlContent);

            var input = root.Q<TextField>("unicodeInput");
            var glyph = root.Q<Label>("glyphLabel");

            // Optional: bind TextField to a string SerializedProperty named "unicodeString" if it exists
            var prop = serializedObject.FindProperty("unicodeString");
            if (prop is { propertyType: SerializedPropertyType.String })
            {
                input.BindProperty(prop);
            }

            // initial set (in case serialized value already present)
            UpdateGlyph(glyph, input.value);

            // update on user edit
            input.RegisterValueChangedCallback(evt =>
            {
                UpdateGlyph(glyph, evt.newValue);
                if (prop is not { propertyType: SerializedPropertyType.String })
                    return;

                // ensure serialized property kept in sync (if not using BindProperty)
                prop.stringValue = evt.newValue;
                serializedObject.ApplyModifiedProperties();
            });

            // refresh on Undo/Redo
            Undo.undoRedoPerformed += () => UpdateGlyph(glyph, input.value);

            // optionally assign a font that contains the glyph
            if (iconFont) glyph.style.unityFont = iconFont;

            return root;
        }

        static void UpdateGlyph(Label glyph, string source)
        {
            if (TryParseCodepoint(source, out var codepoint) && codepoint is >= 0 and <= 0x10FFFF)
                glyph.text = char.ConvertFromUtf32(codepoint);
            else
                glyph.text = "�"; // or empty string
        }


        static bool TryParseCodepoint(string s, out int code)
        {
            code = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();

            if (s.StartsWith("U+", StringComparison.OrdinalIgnoreCase)) s = s[2..];
            if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) s = s[2..];

            // try hex first
            return int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out code) ||
                   // try decimal
                   int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out code);

        }
    }
}
