using System;
using Scenes.Simulation.UI.ListItem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace Gameplay
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

#if UNITY_EDITOR
        void OnValidate()
        {
            if (mainTexture)
            {
                EditorApplication.delayCall += () =>
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), mainTexture.name);

                isCorrect = mainTexture.name.EndsWith("C");
            }
        }
#endif

        public VectorImage leftImage
        {
            get { return null; }
        }

        public VectorImage rightImage
        {
            get { return null; }
        }

        public string mainText
        {
            get { return actionName.ToString().ToSentenceCase(); }
        }

        public string supportText
        {
            get { return actionDescription.ToString().ToSentenceCase(); }
        }

        public int approximateHeight
        {
            get { return 80; }
        }

        public bool Equals(IListItemData other)
        {
            return other != null &&
                   mainText == other.mainText &&
                   supportText == other.supportText &&
                   leftImage == other.leftImage &&
                   rightImage == other.rightImage;
        }

        public bool Equals(MissionSubState other)
        {
            return Equals(mainTexture, other.mainTexture) && actionName == other.actionName &&
                   actionDescription == other.actionDescription;
        }

        public override bool Equals(object obj)
        {
            return obj is MissionSubState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(mainTexture, (int)actionName, (int)actionDescription);
        }
    }
}
