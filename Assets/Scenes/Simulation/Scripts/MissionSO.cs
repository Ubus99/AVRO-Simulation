using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.UIElements;
using ZLinq;

namespace Scenes.Simulation.Scripts
{
    [CreateAssetMenu]
    public class MissionSo : ScriptableObject
    {
        [SerializeField]
        Texture2D map;

        [SerializeField]
        Texture2D route;

        [SerializeField]
        List<MissionSubState> subStates = new();

        public List<ListItemData> options
        {
            get { return Enumerable.ToList(subStates.Select(mss => mss.ToListData()).ToList()); }
        }

        void OnValidate()
        {
            var loader = MissionDataLoader.instance;
            if (loader == null)
            {
                Debug.LogWarning("MissionDataLoader.instance is null");
                return;
            }
            var images = loader.textures.Where(kvp => kvp.Key.StartsWith(name))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            SyncLists(images);
        }

        void SyncLists(Dictionary<string, Texture2D> images)
        {
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (subStates == null) throw new ArgumentNullException(nameof(subStates));

            for (var i = subStates.Count - 1; i >= 0; i--)
            {
                var entry = subStates[i];
                var id = entry.id ?? string.Empty;

                if (images.TryGetValue(id, out var texture))
                {
                    // Set the struct's image by replacing the list element
                    entry.mainTexture = texture;
                    entry.id = name;
                    subStates[i] = entry;

                    // mark sprite consumed so we can add only remaining sprites later
                    images.Remove(id);
                }
                else
                {
                    // No matching sprite: remove this struct
                    subStates.RemoveAt(i);
                }
            }

            // Add new entries for any remaining sprites with no struct
            foreach (var kv in images)
            {
                if (kv.Key == name)
                {
                    route = kv.Value;
                }
                else
                {
                    subStates.Add(new MissionSubState { id = name, mainTexture = kv.Value });
                }
            }
        }

        [Serializable]
        public struct MissionSubState
        {
            public string id;

            public VectorImage leftIcon;
            public VectorImage rightIcon;

            public Texture2D mainTexture;
            public string actionName;
            public string actionDescription;

            public ListItemData ToListData()
            {
                return new ListItemData
                {
                    LeftImage = leftIcon,
                    RightImage = rightIcon,
                    MainText = "",
                    SupportText = ""
                };
            }
        }
    }
}
