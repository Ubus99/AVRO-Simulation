#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UI.Icons
{
    public class IconAtlasPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] imported,
            string[] deleted,
            string[] moved,
            string[] movedFrom)
        {
            var atlases = Resources.LoadAll<IconAtlas>("");

            foreach (var atlas in atlases)
            {
                if (string.IsNullOrEmpty(atlas.resourcesFolder))
                    continue;

                var watch = $"Assets/Resources/{atlas.resourcesFolder}";

                var touched =
                    imported.Any(p => p.StartsWith(watch)) ||
                    deleted.Any(p => p.StartsWith(watch)) ||
                    moved.Any(p => p.StartsWith(watch));

                if (touched)
                    atlas.Build();
            }
        }
    }
}
#endif
