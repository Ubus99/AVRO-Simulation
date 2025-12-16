using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class MissionController : MonoBehaviour
    {
        [SerializeField]
        List<Mission> missions;

        int _activeMission = -1;

        public bool inProgress { get; private set; }

        public bool TryActivateMission()
        {
            if (inProgress) return false;

            _activeMission++;
            missions[_activeMission].Activate();
            inProgress = true;
            return true;
        }
    }
}
