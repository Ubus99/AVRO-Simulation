using System;
using UI.Icons;
using UI.ListItem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Gameplay.Missions
{
    [CreateAssetMenu(menuName = "missions/SubState")]
    public class MissionSubState : ScriptableObject, IListItemData
    {
        public enum AdsAction
        {
            GoStraight = 0,
            GoLeft = 1,
            GoRight = 2,
            TurnLeft = 3,
            TurnRight = 4,
            PassToTheRight = 5,
            PassToTheLeft = 6,
            Stop = 7
        }

        public enum OddChange
        {
            None = 0,
            AllowUsingOppositeLane = 1,
            IgnoreSignage = 2,
            AcceptPlannedRoute = 3,
            Reroute = 4,
            WaitForObstacleToClear = 5,
            PrioritizeOriginalRoadSignage = 6,
            PrioritizeCurrentRoadSignage = 7,
            NoValidPaths = 8
        }

        public Texture2D mainTexture;
        public AdsAction actionName;
        public OddChange actionDescription;
        public bool isCorrect;
        IconAtlas _icons;

        void OnEnable()
        {
            _icons = IconAtlasRegistry.Get("lucide");
            if (!_icons)
            {
                throw new NullReferenceException("Icon Database not found");
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!mainTexture) return;

            EditorApplication.delayCall += () =>
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), mainTexture.name);

            isCorrect = mainTexture.name.EndsWith("C");
        }
#endif

        #region IListItemData

        public VectorImage LeftImage { get; } = null;

        public VectorImage RightImage
        {
            get { return _icons["chevron-right"]; }
        }

        public bool RightIconInteractable { get; } = true;

        public Action ButtonAction
        {
            get { return () => MissionEvents.missionSubmittedEvent?.Invoke(this); }
        }

        public string RightButtonLabel { get; } = "submit";

        public string MainText
        {
            get { return actionName.ToString().ToSentenceCase(); }
        }

        public string SupportText
        {
            get { return $"- {actionDescription.ToString().ToSentenceCase()}"; }
        }

        public int ApproximateHeight { get; } = 62;

        public bool Equals(IListItemData other)
        {
            return base.Equals(other);
        }

        #endregion
    }
}
