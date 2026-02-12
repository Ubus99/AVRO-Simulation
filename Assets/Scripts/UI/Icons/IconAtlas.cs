// IconAtlas.cs

using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Icons
{
    [CreateAssetMenu(menuName = "UI/Icon Atlas")]
    public class IconAtlas : ScriptableObject
    {
        [Tooltip("Resources-relative folder path (example: Icons/Gameplay)")]
        public string resourcesFolder;

        [SerializeField]
        SerializedDictionary<string, VectorImage> cache = new();

        bool _isRebuilt;

        public VectorImage this[string key]
        {
            get
            {
                // ensure has been build
                if (cache.Count == 0 && !_isRebuilt)
                {
                    Build();
                    _isRebuilt = true;
                }

                cache.TryGetValue(key, out var vi);
                return vi;
            }
        }

        void OnEnable()
        {
            IconAtlasRegistry.Register(this);
        }

        void OnDisable()
        {
            IconAtlasRegistry.Unregister(this);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (string.IsNullOrEmpty(resourcesFolder)) return;

            Build();
            IconAtlasRegistry.Register(this);
        }
#endif

        public void Build()
        {
            cache.Clear();

#if UNITY_EDITOR
            // Editor: load via AssetDatabase for reliability
            var assets = AssetDatabase.FindAssets("t:VectorImage",
            new[]
            {
                $"Assets/Resources/{resourcesFolder}"
            });

            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var img = AssetDatabase.LoadAssetAtPath<VectorImage>(path);
                cache.Add(img.name, img);
            }

            EditorUtility.SetDirty(this);

#else
            // Player: load via Resources
            foreach(var img in Resources.LoadAll<VectorImage>(resourcesFolder))
                cache.Add(name, img);
#endif
        }
    }
}
