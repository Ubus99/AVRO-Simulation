using Scenes.Simulation.Scripts;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(MissionSo))]
    [CanEditMultipleObjects]
    public class MissionInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var mapImageProperty = serializedObject.FindProperty("map");
            var routeImageProperty = serializedObject.FindProperty("route");
            var subStateListProperty = serializedObject.FindProperty("subStates");

            var mapImageField = new ObjectField
            {
                objectType = typeof(Texture2D),
                allowSceneObjects = false
            };
            mapImageField.BindProperty(mapImageProperty);

            // draw map
            var mapImage = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = new StyleLength(Length.Percent(100)),
                    height = new StyleLength(StyleKeyword.Auto),
                    maxHeight = 256,
                    flexShrink = 0
                },
                image = mapImageProperty.objectReferenceValue as Texture2D
            };

            mapImageField.RegisterValueChangedCallback(_ =>
                mapImage.image = mapImageProperty.objectReferenceValue as Texture2D);

            // draw route
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
                image = routeImageProperty.objectReferenceValue as Texture2D
            };

            // When the container gets a layout width, calculate pixel height to preserve aspect ratio.
            root.RegisterCallback<GeometryChangedEvent>(evt =>
            {

                UpdateImage(mapImage);
                UpdateImage(routeImage);
                return;

                void UpdateImage(Image image)
                {
                    if (image.image is not Texture2D { width: > 0 } tex)
                        return;

                    var targetWidth = evt.newRect.width; // available width
                    var scaledHeight = targetWidth * ((float)tex.height / tex.width);
                    image.style.height = new StyleLength(new Length(scaledHeight, LengthUnit.Pixel));
                }
            });

            root.Add(mapImageField);
            root.Add(mapImage);
            root.Add(routeImage);

            root.Add(new ListView
            {
                showFoldoutHeader = true,
                headerTitle = "Substates",
                showAddRemoveFooter = false,
                reorderable = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,

                // how many elements
                itemsSource = new int[subStateListProperty.arraySize],

                // create row
                makeItem = () => new PropertyField(),

                // bind row
                bindItem = (e, i) =>
                {
                    ((PropertyField)e).BindProperty(subStateListProperty.GetArrayElementAtIndex(i));
                }
            });

            root.Bind(serializedObject);

            return root;
        }
    }
}
