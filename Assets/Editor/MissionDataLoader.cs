using System;
using System.Collections.Generic;
using Gameplay;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Editor
{
    public class MissionDataLoader : ScriptableSingleton<MissionDataLoader>
    {
        [SerializeField]
        SerializedDictionary<string, Texture2D> _textures = new();

        [SerializeField]
        List<MissionSo> missions = new();

        public SerializedDictionary<string, Texture2D> textures
        {
            get { return _textures; }
        }

        void OnDisable()
        {
            Save(true);
        }

        void OnValidate()
        {
            var texturesList = Resources.LoadAll<Texture2D>("MissionData/Bengt Scenarios");
            var missionStrings = new List<string>();

            _textures.Clear();
            foreach (var tex2D in texturesList)
            {
                if (!TryParseMissionName(tex2D.name, out var addr)) // not a mission element
                    continue;

                _textures.Add(tex2D.name, tex2D);

                var missionName = $"{addr[0]}_{addr[1]}";
                if (!missionStrings.Contains(missionName))
                {
                    missionStrings.Add(missionName);
                }
            }
        }

        static bool TryParseMissionName(string fullName, out string[] path)
        {
            var addr = fullName.Split("_");
            if (addr[0].StartsWith("W-")) // is a mission element
            {
                path = addr;
                return true;
            }
            path = null;
            return false;
        }


        public void CreateMissionTemplates()
        {
            if (textures == null) throw new ArgumentNullException(nameof(textures));

#if UNITY_EDITOR

            // Expected mission IDs derived from textures (W-X_Y)
            var expectedMissionIds = new HashSet<string>();

            foreach (var tex in textures.Keys)
            {
                if (TryParseMissionName(tex, out var addr))
                    expectedMissionIds.Add($"{addr[0]}_{addr[1]}");
            }

            // Load ALL MissionSO assets on disk
            var missionGuids = AssetDatabase.FindAssets("t:MissionSo");
            var existingMissions = new Dictionary<string, MissionSo>();

            foreach (var guid in missionGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mso = AssetDatabase.LoadAssetAtPath<MissionSo>(path);
                if (mso != null)
                    existingMissions[mso.name] = mso;
            }

            missions.Clear();

            // Delete invalid missions
            foreach (var kv in existingMissions)
            {
                if (!expectedMissionIds.Contains(kv.Key))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(kv.Value));
                }
                else
                {
                    missions.Add(kv.Value);
                }
            }

            // Create missing missions
            const string folder = "Assets/Resources/MissionData/Bengt Scenarios/Missions";
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets/Resources/Bengt Scenarios/MissionData", "Missions");

            foreach (var id in expectedMissionIds)
            {
                if (existingMissions.ContainsKey(id))
                    continue;

                var mso = CreateInstance<MissionSo>();
                mso.name = id;

                AssetDatabase.CreateAsset(mso, $"{folder}/{id}.asset");
                missions.Add(mso);
            }

            missions.Sort((mso1, mso2) => string.Compare(mso1.name, mso2.name, StringComparison.Ordinal));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

#endif
        }
    }
}
