using Gameplay;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MissionDataLoader))]
    public class MissionDataLoaderInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Create Mission Templates"))
            {
                foreach (var t in targets)
                {
                    var comp = (MissionDataLoader)t;
                    Undo.RecordObject(comp, "Run Action");
                    comp.CreateMissionTemplates();
                    EditorUtility.SetDirty(comp);
                }
            }
        }
    }
}
