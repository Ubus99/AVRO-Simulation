using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/DifficultySetting")]
    public class DifficultySo : ScriptableObject
    {
        public int gameSpeed = 5;
        public int maxMissions = 5;
        public int missionsToComplete = 20;
    }
}
