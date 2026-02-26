using System;
using System.Collections.Generic;
using System.IO;
using Logging;
using UI.Icons;
using UI.ListItem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Gameplay.Missions
{
    [CreateAssetMenu(menuName = "missions/Mission")]
    public class MissionSo : ScriptableObject, IListItemData
    {
        [SerializeField]
        Texture2D route;

        [SerializeField]
        List<MissionSubState> subStates = new();

        [SerializeField]
        Texture2D map;

        IconAtlas _icons;

        public MissionRecord record { get; } = new();

        public List<MissionSubState> options
        {
            get { return subStates; }
        }

        void OnEnable()
        {
            _icons = IconAtlasRegistry.Get("lucide");
            if (!_icons)
            {
                throw new NullReferenceException("Icon Database not found");
            }
        }


        public Texture2D GetRouteTexture()
        {
            return route;
        }

        public void Start()
        {
            record.TimeStart = Time.timeSinceLevelLoad;
        }

        public void Load()
        {
            MainText = $"Car #{Mathf.FloorToInt(Random.Range(1000, 9999))}";
            record.TimeLoaded = Time.timeSinceLevelLoad;
        }

        public bool Complete(MissionSubState missionSubState)
        {
            record.TimeEnd = Time.timeSinceLevelLoad;
            record.missionName = name;
            record.correct = missionSubState.isCorrect;

            return missionSubState.isCorrect;
        }

        public class MissionRecord : BaseRecord
        {
            public float TimeEnd;
            public float TimeLoaded;
            public float TimeStart;

            public string missionName { get; set; }
            public int numberCompleted { get; set; }
            public bool correct { get; set; }

            public float timeToComplete
            {
                get { return TimeEnd - TimeStart; }
            }

            public float timeToStart
            {
                get { return TimeStart - TimeLoaded; }
            }

            public float totalTime
            {
                get { return timeToComplete + timeToStart; }
            }
        }

        #region IListItemData

        public VectorImage LeftImage
        {
            get { return null; }
        }

        public VectorImage RightImage
        {
            get { return _icons["chevron-right"]; }
        }

        public bool RightIconInteractable { get; } = false;
        public Action ButtonAction { get; } = null;

        public string RightButtonLabel { get; } = null;

        public string MainText { get; set; }

        public string SupportText { get; } = null;

        public int ApproximateHeight
        {
            get { return 50; }
        }

        public bool Equals(IListItemData other)
        {
            return base.Equals(other);
        }

        #endregion

#if UNITY_EDITOR
        string ownPath
        {
            get
            {
                var filePath = AssetDatabase.GetAssetPath(this);
                var path = Path.GetDirectoryName(filePath);
                return path?.Replace(@"Assets\Resources\", string.Empty);
            }
        }

        void OnValidate()
        {
            _icons = IconAtlasRegistry.Get("lucide");
        }

        public void SyncLists()
        {
            var states = Resources.LoadAll<MissionSubState>(ownPath).ToList();

            if (!states.Any()) return;
            subStates.Clear();
            subStates.AddRange(states);
            subStates.OrderBy(state => state.actionName);

            EditorUtility.SetDirty(this);
        }

        public void GenerateStates()
        {
            var states = Resources.LoadAll<MissionSubState>(ownPath).ToList();
            var images = Resources.LoadAll<Texture2D>(ownPath).ToList();

            foreach (var image in images)
            {
                if (!image.name.EndsWith("F") && !image.name.EndsWith("C")) continue;
                if (states.Any(state => state.name == image.name)) continue;

                var ss = CreateInstance<MissionSubState>();
                ss.mainTexture = image;

                if (image.name.EndsWith("0_F") || image.name.EndsWith("0_C"))
                {
                    ss.actionName = MissionSubState.AdsAction.Stop;
                    ss.actionDescription = MissionSubState.OddChange.NoValidPaths;
                }

                var assetPath = Path.Combine(@"Assets\Resources", ownPath, $"{image.name}.asset").Replace("\\", "/");
                AssetDatabase.CreateAsset(ss, assetPath);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}
