// IconAtlasRegistry.cs

using System.Collections.Generic;
using UnityEngine;

namespace UI.Icons
{
    public static class IconAtlasRegistry
    {
        static readonly Dictionary<string, IconAtlas> Atlases = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            foreach (var atlas in Resources.LoadAll<IconAtlas>(""))
                Register(atlas);
        }

        public static void Register(IconAtlas atlas)
        {
            if (atlas) Atlases[atlas.name] = atlas;
        }

        public static void Unregister(IconAtlas atlas)
        {
            Atlases.Remove(atlas.name);
        }

        public static IconAtlas Get(string atlasName)
        {
            if (Atlases.Count == 0) Init();

            return Atlases[atlasName];
        }
    }
}
