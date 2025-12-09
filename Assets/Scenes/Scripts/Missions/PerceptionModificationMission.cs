using car_logic;
using Gameplay;
using Streets;
using UnityEngine;

namespace Scenes.Scripts.Missions
{
    public class PerceptionModificationMission : Mission
    {
        public StreetEvent trigger;
        public GameObject obstacle;

        void Start()
        {
            Activate();
        }

        void Update()
        {
            if (!car) return;
            if (car.state == States.WaitingForAid)
            {
                car.navigationProvider.SetTargetLocation(obstacle.transform.position);
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
