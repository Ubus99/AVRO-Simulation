using System;
using System.Collections.Generic;
using car_logic;
using Gameplay;
using UnityEngine;
using ZLinq;

namespace Scenes.Scripts.Missions
{
    public class PerceptionModificationMission : Mission
    {
        [Header("Mission Specifics")]
        public StreetEvent trigger;

        public AdsObstacle obstacle;

        void Update()
        {
        }

        void HandleStateChanged(States newState)
        {
            switch (newState)
            {
                case States.Initializing:
                    break;

                case States.NoCommand:
                    break;

                case States.Driving:
                    break;

                case States.ErrorDetected:
                    break;

                case States.WaitingForAid:
                    carInstance.SetTarget(trigger.transform.position + trigger.transform.forward * 2f);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        protected override void Setup()
        {
            trigger.parent = this;
            trigger.gameObject.SetActive(true);

            obstacle.gameObject.SetActive(true);

            carInstance.OnStateChangeEvent += HandleStateChanged;
            obstacle.OnStateChangedEvent += HandleObstacleChanged;
        }

        protected override void CleanUp()
        {
            trigger.gameObject.SetActive(false);
            obstacle.gameObject.SetActive(false);

            if (carInstance) carInstance.OnStateChangeEvent -= HandleStateChanged;
            obstacle.OnStateChangedEvent -= HandleObstacleChanged;
        }

        public override IEnumerable<ObstacleActionListElement> GetObstacleData()
        {
            return obstacle.GetAvailableStates()
                .Select(state => new ObstacleActionListElement(state, obstacle)).ToList();
        }

        void HandleObstacleChanged(AdsObstacle adsObstacle, AdsObstacle.State state)
        {
        }
    }
}
