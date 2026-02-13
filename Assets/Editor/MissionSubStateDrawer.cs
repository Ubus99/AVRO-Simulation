using Scenes.Simulation.Scripts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomPropertyDrawer(typeof(MissionSubState))]
    public class MissionSubStateDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var imageProperty = property.FindPropertyRelative("mainTexture");
            var isCorrectProperty = property.FindPropertyRelative("isCorrect");

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

            AddImage(imageProperty, root);

            var actionName = new PropertyField(property.FindPropertyRelative("actionName"));
            var actionDescription = new PropertyField(property.FindPropertyRelative("actionDescription"));

            root.Add(actionName);
            root.Add(actionDescription);

            root.Bind(property.serializedObject);

            return root;
        }

        static void AddImage(SerializedProperty imageProperty, VisualElement root)
        {

            var image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = new StyleLength(Length.Percent(100)),
                    height = new StyleLength(StyleKeyword.Auto),
                    flexShrink = 0
                },
                image = (Texture2D)imageProperty.objectReferenceValue
            };

            // When the container gets a layout width, calculate pixel height to preserve aspect ratio.
            root.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (image.image is not Texture2D { width: > 0 } tex)
                    return;

                var targetWidth = evt.newRect.width; // available width
                var scaledHeight = targetWidth * ((float)tex.height / tex.width);
                image.style.height = new StyleLength(new Length(scaledHeight, LengthUnit.Pixel));
            });

            root.Add(new Label(((Texture2D)imageProperty.objectReferenceValue).name));
            root.Add(image);
        }
    }
}
