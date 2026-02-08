using Scenes.Simulation.Scripts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Editor
{
    [CustomEditor(typeof(MissionSo))]
    [CanEditMultipleObjects]
    public class MissionInspector : UnityEditor.Editor
    {
        static void SubStateSection(VisualElement root, SerializedProperty correctStateProperty,
            SerializedProperty listProperty)
        {
            var correctStateField = new PropertyField();
            correctStateField.BindProperty(correctStateProperty);
            root.Add(correctStateField);

            root.Add(new ListView
            {
                showFoldoutHeader = true,
                headerTitle = "Sub-States",
                showAddRemoveFooter = false,
                reorderable = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,

                // how many elements
                itemsSource = new int[listProperty.arraySize],

                // create row
                makeItem = () => new PropertyField(),

                // bind row
                bindItem = (e, i) => { ((PropertyField)e).BindProperty(listProperty.GetArrayElementAtIndex(i)); }
            });
        }

        static void UpdateImage(Image image, GeometryChangedEvent evt)
        {
            if (image.image is not Texture2D { width: > 0 } tex)
                return;

            var targetWidth = evt.newRect.width; // available width
            var scaledHeight = targetWidth * ((float)tex.height / tex.width);
            image.style.height = new StyleLength(new Length(scaledHeight, LengthUnit.Pixel));
        }

        static void AssignableImageSection(VisualElement root, SerializedProperty imageProperty)
        {
            var imageField = new ObjectField
            {
                objectType = typeof(Texture2D),
                allowSceneObjects = false
            };
            imageField.BindProperty(imageProperty);

            // draw map
            var image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = new StyleLength(Length.Percent(100)),
                    height = new StyleLength(StyleKeyword.Auto),
                    maxHeight = 256,
                    flexShrink = 0
                },
                image = imageProperty.objectReferenceValue as Texture2D
            };

            imageField.RegisterValueChangedCallback(_ =>
                image.image = imageProperty.objectReferenceValue as Texture2D);

            root.RegisterCallback<GeometryChangedEvent, Image>((evt, img) =>
                UpdateImage(img, evt),
            image);

            root.Add(imageField);
            root.Add(image);
        }

        static void ImageSection(VisualElement root, SerializedProperty imageProperty)
        {
            var routeImage = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = new StyleLength(Length.Percent(100)),
                    height = new StyleLength(StyleKeyword.Auto),
                    maxHeight = 128,
                    flexShrink = 0
                },
                image = imageProperty.objectReferenceValue as Texture2D
            };

            root.RegisterCallback<GeometryChangedEvent, Image>((evt, image) =>
                UpdateImage(image, evt),
            routeImage);

            root.Add(routeImage);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var mapImageProperty = serializedObject.FindProperty("map");
            var routeImageProperty = serializedObject.FindProperty("route");
            var correctStateProperty = serializedObject.FindProperty("correctSubState");
            var subStateListProperty = serializedObject.FindProperty("subStates");

            AssignableImageSection(root, mapImageProperty);
            AssignableImageSection(root, routeImageProperty);
            SubStateSection(root, correctStateProperty, subStateListProperty);

            root.Bind(serializedObject);

            return root;
        }
    }
}
