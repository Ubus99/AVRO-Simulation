using Gameplay;
using Gameplay.Missions;
using UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(MissionSo))]
    [CanEditMultipleObjects]
    public class MissionInspector : UnityEditor.Editor
    {
        static void SubStateSection(VisualElement root, SerializedProperty correctStateProperty,
            SerializedProperty listProperty)
        {
            root.Add(new ListView
            {
                showFoldoutHeader = true,
                headerTitle = "Sub-States",
                showAddRemoveFooter = true,
                reorderable = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,

                // how many elements
                itemsSource = new int[listProperty.arraySize],

                // create row
                makeItem = () => new PropertyField(),

                // bind row
                bindItem = (e, i) => { ((PropertyField)e).BindProperty(listProperty.GetArrayElementAtIndex(i)); }
            });
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var mapImageProperty = serializedObject.FindProperty("map");
            var routeImageProperty = serializedObject.FindProperty("route");
            var correctStateProperty = serializedObject.FindProperty("correctSubState");
            var subStateListProperty = serializedObject.FindProperty("subStates");

            var reloadButton = new Button
            {
                text = "Reload"
            };
            reloadButton.clicked += () => { (target as MissionSo)?.SyncLists(); };
            root.Add(reloadButton);

            var generateButton = new Button
            {
                text = "Try Generate"
            };
            generateButton.clicked += () =>
            {
                var mission = target as MissionSo;
                mission?.GenerateStates();
                mission?.SyncLists();
            };
            root.Add(generateButton);

            GUIUtils.AssignableImageSection(root, mapImageProperty);
            GUIUtils.AssignableImageSection(root, routeImageProperty);
            SubStateSection(root, correctStateProperty, subStateListProperty);

            root.Bind(serializedObject);

            return root;
        }
    }
}
