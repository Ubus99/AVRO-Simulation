using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Lucide;

namespace Editor.Lucide
{
    [CustomEditor(typeof(GlyphData))]
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

            var glyph = root.Q<Label>("glyphLabel"); //for display only
            var input = root.Q<IntegerField>("IDField");

            if (iconFont) glyph.style.unityFont = iconFont;
            UpdateGlyph(glyph);

            input.RegisterValueChangedCallback(evt => UpdateGlyph(glyph));

            Undo.undoRedoPerformed += () => UpdateGlyph(glyph);

            return root;
        }

        void UpdateGlyph(Label glyph)
        {
            var iconIDProp = serializedObject.FindProperty("iconID");
            var unicodeProp = serializedObject.FindProperty("unicodeString");

            var sanitizedIconID = iconIDProp.intValue + 0; //offset
            var unicode = unicodeProp.stringValue = sanitizedIconID.ToString("X");

            serializedObject.ApplyModifiedProperties();

            glyph.text = unicode == "" ? "�" : GlyphData.UnicodeToChar(sanitizedIconID);
        }
    }
}
