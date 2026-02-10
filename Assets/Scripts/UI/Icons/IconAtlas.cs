// IconAtlas.cs

using System.Collections.Generic;
using System.Linq;
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
        List<VectorImage> images = new();

        Dictionary<string, VectorImage> _cache;

        public VectorImage this[string key]
        {
            get
            {
                if (_cache == null) RebuildCache();
                _cache.TryGetValue(name, out var v);
                return v;
            }
        }

        void OnEnable()
        {
            RebuildCache();
            IconAtlasRegistry.Register(this);
        }

        void OnDisable()
        {
            IconAtlasRegistry.Unregister(this);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!string.IsNullOrEmpty(resourcesFolder))
                Build();
        }
#endif

        public void Build()
        {
            images.Clear();

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
                if (img) images.Add(img);
            }

            EditorUtility.SetDirty(this);

#else
            // Player: load via Resources
            images.AddRange(Resources.LoadAll<VectorImage>(resourcesFolder));
#endif

            RebuildCache();
        }

        void RebuildCache()
        {
            _cache = new Dictionary<string, VectorImage>();

            foreach (var img in images.Where(img => img))
                _cache[img.name] = img;
        }
    }
}
