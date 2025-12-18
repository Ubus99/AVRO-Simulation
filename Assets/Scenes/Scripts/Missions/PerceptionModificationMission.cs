using car_logic;
using Gameplay;
using UnityEngine;

namespace Scenes.Scripts.Missions
{
    public class PerceptionModificationMission : Mission
    {
        public StreetEvent trigger;
        public GameObject obstacle;

        void Update()
        {
            if (!CarInstance) return;
            if (CarInstance.state == States.WaitingForAid)
            {
                CarInstance.SetTarget(obstacle.transform.position);
            }
        }

        protected override void Setup()
        {
            trigger.parent = this;
            trigger.gameObject.SetActive(true);

            obstacle.gameObject.SetActive(true);
        }

        protected override void CleanUp()
        {
            trigger.gameObject.SetActive(false);
            obstacle.gameObject.SetActive(false);
        }
    }
}
