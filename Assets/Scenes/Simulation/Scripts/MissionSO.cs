using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using ZLinq;

namespace Scenes.Simulation.Scripts
{
    [CreateAssetMenu]
    public class MissionSo : ScriptableObject
    {
        public enum AdsAction
        {
            GoStraight = 0,
            GoLeft = 1,
            GoRight = 2,
            TurnLeft = 3,
            TurnRight = 4,
            PassToTheRight = 5,
            PassToTheLeft = 6
        }

        public enum OddChange
        {
            None = 0,
            AllowUsingOppositeLane = 1,
            IgnoreSignage = 2,
            DeclarePlannedRouteValid = 3,
            Reroute = 4,
            WaitForObstacleToClear = 5,
            PrioritizeOriginalRoadSignage = 6,
            PrioritizeCurrentRoadSignage = 7,
            
        }

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
            var images = loader.textures
                .Where(kvp => kvp.Key.StartsWith(name)) // belongs to the same mission
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            SyncLists(images);
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
        }

        [Serializable]
        public struct MissionSubState
        {
            public Texture2D mainTexture;
            public AdsAction actionName;
            public OddChange actionDescription;

            public ListItemData ToListData()
            {
                return new ListItemData
                {
                    MainText = "",
                    SupportText = ""
                };
            }
        }
    }
}
