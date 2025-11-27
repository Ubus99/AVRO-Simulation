using UnityEditor;
using UnityEngine;

namespace Dreamteck.Splines.Editor
{
    public class ColorModifierEditor : SplineSampleModifierEditor
    {
        readonly float addTime = 0f;

        public ColorModifierEditor(SplineUser user, SplineUserEditor editor) : base(user, editor, "_colorModifier")
        {
            title = "Color Modifiers";
        }

        public void ClearSelection()
        {
            selected = -1;
        }

        public override void DrawInspector()
        {
            base.DrawInspector();
            if (!isOpen) return;
            if (GUILayout.Button("Add New Color"))
            {
                AddKey(addTime - 0.1f, addTime + 0.1f);
                UpdateValues();
            }
        }

        protected override void KeyGUI(SerializedProperty key)
        {
            var color = key.FindPropertyRelative("color");
            var blendMode = key.FindPropertyRelative("blendMode");
            base.KeyGUI(key);
            EditorGUILayout.PropertyField(color);
            EditorGUILayout.PropertyField(blendMode);
        }
    }
}
