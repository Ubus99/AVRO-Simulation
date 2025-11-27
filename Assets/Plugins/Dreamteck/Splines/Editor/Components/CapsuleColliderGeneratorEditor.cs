using UnityEditor;

namespace Dreamteck.Splines.Editor
{
    [CustomEditor(typeof(CapsuleColliderGenerator), true)]
    [CanEditMultipleObjects]
    public class CapsuleColliderGeneratorEditor : SplineUserEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        protected override void BodyGUI()
        {
            base.BodyGUI();
            var generator = (CapsuleColliderGenerator)target;
            var directionProperty = serializedObject.FindProperty("_direction");
            var heightProperty = serializedObject.FindProperty("_height");
            var radiusProperty = serializedObject.FindProperty("_radius");
            var overlapCapsProperty = serializedObject.FindProperty("_overlapCaps");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(directionProperty);
            var direction = (CapsuleColliderGenerator.CapsuleColliderZDirection)directionProperty.intValue;
            if (direction == CapsuleColliderGenerator.CapsuleColliderZDirection.Z)
            {
                EditorGUILayout.PropertyField(radiusProperty);
                EditorGUILayout.PropertyField(overlapCapsProperty);
            }
            else
            {
                EditorGUILayout.PropertyField(heightProperty);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

        }
    }
}
