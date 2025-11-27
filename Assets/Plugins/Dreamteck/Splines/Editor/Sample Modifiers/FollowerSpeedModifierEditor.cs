using UnityEditor;
using UnityEngine;

namespace Dreamteck.Splines.Editor
{
    public class FollowerSpeedModifierEditor : SplineSampleModifierEditor
    {
        readonly float addTime = 0f;
        public bool allowSelection = true;

        public FollowerSpeedModifierEditor(SplineUser user, SplineUserEditor editor) : base(user,
        editor,
        "_speedModifier")
        {
            title = "Speed Modifiers";
        }

        public void ClearSelection()
        {
            selected = -1;
        }

        public override void DrawInspector()
        {
            base.DrawInspector();
            if (!isOpen) return;
            if (GUILayout.Button("Add Speed Region"))
            {
                AddKey(addTime - 0.1f, addTime + 0.1f);
                UpdateValues();
            }
        }

        protected override void KeyGUI(SerializedProperty key)
        {
            var speed = key.FindPropertyRelative("speed");
            var mode = key.FindPropertyRelative("mode");
            base.KeyGUI(key);
            EditorGUILayout.PropertyField(mode);
            var text = (mode.intValue == (int)FollowerSpeedModifier.SpeedKey.Mode.Add ? "Add" : "Multiply") + " Speed";
            EditorGUILayout.PropertyField(speed, new GUIContent(text));
        }
    }
}
