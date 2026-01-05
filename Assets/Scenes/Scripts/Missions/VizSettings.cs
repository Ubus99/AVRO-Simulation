using UnityEngine;

namespace Scenes.Scripts.Missions
{
    [CreateAssetMenu(fileName = "Visualisation Setting", menuName = "Visualisation Setting", order = 0)]
    public class VizSettings : ScriptableObject
    {
        public Color inactiveColor;
        public Color errorColor;
        public Color activeColor;
    }
}