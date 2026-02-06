using UnityEngine;
using Utils.Types;

namespace Scenes.Scripts.Missions
{
    [CreateAssetMenu(fileName = "Visualisation Setting", menuName = "custom/AIVisualisationSetting", order = 0)]
    [ExecuteAlways]
    public class VizSettings : BetterScriptableObject
    {
        public Color inactiveColor;
        public Color errorColor;
        public Color activeColor;

        protected override void HandleChanged()
        {
        }
    }
}
