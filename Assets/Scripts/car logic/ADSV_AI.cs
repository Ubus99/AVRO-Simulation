using System;
using Gameplay;
using Streets;
using UnityEngine;

namespace car_logic
{
    [RequireComponent(typeof(CarFollower))]
    public class ADSV_AI : BaseStateMachine<States>
    {
        [Header("Movement")]
        public CarFollower navMeshFollower;

        [Header("Cameras")]
        public Camera topDownCamera;

        public Camera povCamera;
        float _previousSpeed;

        public CarAI CarAI { private set; get; }

        void Awake()
        {
            navMeshFollower = GetComponent<CarFollower>();
            CarAI = GetComponent<CarAI>();
        }

        void Start()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager) && CarAI)
                gameManager.RegisterCar(this);
        }

        void Update()
        {
            topDownCamera.transform.LookAt(transform.position, Vector3.forward);
        }

        void FixedUpdate()
        {
            switch (State)
            {
                case States.Initializing:
                    BaseUpdate();
                    PrintState();
                    _previousSpeed = navMeshFollower.GetTargetSpeed();
                    State = States.NoCommand;
                    break;
                case States.NoCommand:
                    BaseUpdate();
                    PrintState();
                    navMeshFollower.SetTargetSpeed(0);
                    State = States.Driving;
                    break;
                case States.Driving:
                    BaseUpdate();
                    PrintState();
                    navMeshFollower.SetTargetSpeed(_previousSpeed);
                    break;
                case States.ErrorDetected:
                    BaseUpdate();
                    DoErrorDetected();
                    break;
                case States.WaitingForAid:
                    BaseUpdate();
                    PrintState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void DoErrorDetected()
        {
            if (StateChanged)
            {
                PrintEntryState();
                _previousSpeed = navMeshFollower.GetTargetSpeed();
                navMeshFollower.SetTargetSpeed(0);
            }
            BaseUpdate();
            PrintState();
        }
    }

    public enum States
    {
        Initializing,
        NoCommand,
        Driving,
        ErrorDetected,
        WaitingForAid
    }
}
