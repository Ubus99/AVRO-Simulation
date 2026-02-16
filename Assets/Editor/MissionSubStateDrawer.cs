using Gameplay;
using UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(MissionSubState))]
    public class MissionSubStateDrawer : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var imageProperty = serializedObject.FindProperty("mainTexture");
            var isCorrectProperty = serializedObject.FindProperty("isCorrect");
            var actionName = new PropertyField(serializedObject.FindProperty("actionName"));
            var actionDescription = new PropertyField(serializedObject.FindProperty("actionDescription"));

            var root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    alignItems = Align.Stretch
                }
            };

            var checkbox = new Toggle
            {
                text = "isCorrect"
            };
            checkbox.BindProperty(isCorrectProperty);
            root.Add(checkbox);

            GUIUtils.AssignableImageSection(root, imageProperty);

            root.Add(actionName);
            root.Add(actionDescription);

            root.Bind(serializedObject);

            return root;
        }
    }
}
