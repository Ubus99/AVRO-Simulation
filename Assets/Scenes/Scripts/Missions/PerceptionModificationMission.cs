using System;
using System.Collections.Generic;
using car_logic;
using Gameplay;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes.Scripts.Missions
{
    public class PerceptionModificationMission : Mission
    {
        [Header("Mission Specifics")]
        public StreetEvent trigger;
        public GameObject obstacle;

        private void Update()
        {
        }

        private void HandleStateChanged(States newState)
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
                    CarInstance.SetTarget(trigger.transform.position + trigger.transform.forward * 2f);
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

            CarInstance.OnStateChangeEvent += HandleStateChanged;
        }

        protected override void CleanUp()
        {
            trigger.gameObject.SetActive(false);
            obstacle.gameObject.SetActive(false);

            if (CarInstance) CarInstance.OnStateChangeEvent -= HandleStateChanged;
        }
    }
}