using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Scenes.Simulation.UI.ListItem;
using UI.Icons;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scenes.Simulation.Scripts
{
    [CreateAssetMenu(menuName = "missions/Mission")]
    public class MissionSo : ScriptableObject, IListItemData
    {
        static readonly MissionSubState EmergencyState = new()
        {
            actionName = MissionSubState.AdsAction.Stop,
            actionDescription = MissionSubState.OddChange.NoValidPaths
        };

        [SerializeField]
        Texture2D route;

        [SerializeField]
        int correctSubState;

        [SerializeField]
        List<MissionSubState> subStates = new();

        [SerializeField]
        Texture2D map;

        IconAtlas _icons;

        public List<MissionSubState> options
        {
            get { return subStates; }
        }

        void OnValidate()
        {
            var loader = MissionDataLoader.instance;
            if (loader == null)
            {
                Debug.LogWarning("MissionDataLoader.instance is null");
                return;
            }
            var images = loader.textures
                .Where(kvp => kvp.Key.StartsWith(name)) // belongs to the same mission
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            SyncLists(images);
            _icons = IconAtlasRegistry.Get("lucide");
            if (!_icons)
            {
                throw new NullReferenceException("Icon Database not found");
            }
        }

        public bool Equals(IListItemData other)
        {
            throw new NotImplementedException();
        }

        public VectorImage leftImage
        {
            get { return null; }
        }

        public VectorImage rightImage
        {
            get { return _icons["chevron-right"]; }
        }

        public string mainText
        {
            get { return name; }
        }

        public string supportText { get; } = null;

        public int approximateHeight
        {
            get { return 64; }
        }

        public Texture2D GetRouteTexture()
        {
            return route;
        }

        void SyncLists(Dictionary<string, Texture2D> images)
        {
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (subStates == null) throw new ArgumentNullException(nameof(subStates));

            if (images.Count == 0)
                subStates.Clear();

            // iterate all exising states
            for (var i = subStates.Count - 1; i >= 0; i--)
            {
                var ss = subStates[i];

                // no matching image
                if (images.All(kvp => kvp.Value != ss.mainTexture))
                {
                    subStates.RemoveAt(i);
                }
            }

            for (var i = images.Count - 1; i >= 0; i--)
            {
                var kvp = images.ElementAt(i);

                if (subStates.Any(state => state.mainTexture == kvp.Value))
                {
                    images.Remove(kvp.Key);
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
                    subStates.Add(new MissionSubState { mainTexture = kv.Value });
                }
            }

            if (!subStates.Contains(EmergencyState))
            {
                subStates.Add(EmergencyState);
            }

            // update old notation
            if (correctSubState == -1)
            {
                correctSubState = subStates.Count - 1;
            }
        }

        public bool Complete(MissionSubState missionSubState)
        {
            var idx = subStates.IndexOf(missionSubState);
            if (idx == correctSubState)
            {
                Debug.Log($"Mission {name} Completed successfully");
                return true;
            }
            Debug.Log($"Mission {name} Completed unsuccessfully");
            return false;
        }
    }
}
