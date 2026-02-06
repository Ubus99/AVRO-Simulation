using UnityEditor;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TYPE))]
    public class MissionSubStateDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            
            return base.CreatePropertyGUI(property);
        }
    }
}
