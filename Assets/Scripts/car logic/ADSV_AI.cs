using System;
using Gameplay;
using Streets;
using UnityEngine;
using Utils;

namespace car_logic
{
    [RequireComponent(typeof(CarSplineFollower))]
    public class ADSV_AI : BaseStateMachine<States>
    {
        [Header("Movement")]
        public CarSplineFollower navigationProvider;

        [Header("Cameras")]
        public Camera topDownCamera;

        public Camera povCamera;

        bool _errorFlag;
        float _previousSpeed;

        public CarAI carAI { private set; get; }

        void Awake()
        {
            navigationProvider = GetComponent<CarSplineFollower>();
            carAI = GetComponent<CarAI>();
        }

        void Start()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager) && carAI)
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
                    _previousSpeed = navigationProvider.GetTargetSpeed();
                    State = States.NoCommand;
                    break;
                case States.NoCommand:
                    BaseUpdate();
                    PrintState();
                    navigationProvider.SetTargetSpeed(0);
                    State = States.Driving;
                    break;
                case States.Driving:
                    BaseUpdate();
                    PrintState();
                    navigationProvider.SetTargetSpeed(_previousSpeed);
                    DetectErrors();
                    break;
                case States.ErrorDetected:
                    DoErrorDetected();
                    break;
                case States.WaitingForAid:
                    DoWaitingForAid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public string GetState()
        {
            return State.ToString();
        }

        public void TriggerError()
        {
            _errorFlag = true;
        }

        void DetectErrors()
        {
            if (_errorFlag)
            {
                State = States.ErrorDetected;
                Debug.Log($"{gameObject.name}: Error detected");
            }
        }

        void DoErrorDetected()
        {
            if (StateChanged)
            {
                PrintEntryState();
                _previousSpeed = navigationProvider.GetTargetSpeed();
                navigationProvider.SetTargetSpeed(0);
            }
            BaseUpdate();
            PrintState();

            State = States.WaitingForAid;
        }

        void DoWaitingForAid()
        {
            if (StateChanged)
            {
                PrintEntryState();
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
