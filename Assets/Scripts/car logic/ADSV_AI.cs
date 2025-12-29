using System;
using Gameplay;
using UnityEngine;
using Utils;

namespace car_logic
{
    [RequireComponent(typeof(AbstractFollower))]
    public class ADSV_AI : BaseStateMachine<States>
    {
        [Header("Movement")]
        [SerializeField]
        NavigationProvider navigationProvider;

        public Transform target;

        [Header("Cameras")]
        public Camera topDownCamera;

        public Camera povCamera;

        bool _errorFlag;
        float _previousSpeed;

        void Awake()
        {
            navigationProvider = GetComponent<NavigationProvider>();
        }

        void Start()
        {
            navigationProvider = GetComponent<NavigationProvider>();
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager))
                gameManager.RegisterCar(this);
        }

        void Update()
        {
            // hack
            topDownCamera.transform.LookAt(transform.position, Vector3.forward);
        }

        void FixedUpdate()
        {
            UpdateStateMachine();
        }

        void OnDestroy()
        {
            if (ServiceLocator.Instance.TryGet<GameManager>(out var gameManager))
                gameManager.DeregisterCar(this);
        }

        void UpdateStateMachine()
        {
            switch (state)
            {
                case States.Initializing:
                    BaseUpdate();
                    PrintState();
                    _previousSpeed = navigationProvider.GetTargetSpeed();
                    state = States.NoCommand;
                    break;
                case States.NoCommand:
                    BaseUpdate();
                    PrintState();
                    navigationProvider.SetTargetSpeed(0);
                    state = States.Driving;
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

        public void SetTarget(Vector3 position)
        {
            navigationProvider.SetTargetLocation(position);
        }

        public string GetState()
        {
            return state.ToString();
        }

        public void TriggerError()
        {
            _errorFlag = true;
        }

        void DetectErrors()
        {
            if (_errorFlag)
            {
                state = States.ErrorDetected;
                Debug.Log($"{gameObject.name}: Error detected");
            }
        }

        void DoErrorDetected()
        {
            if (stateChanged)
            {
                PrintEntryState();
                _previousSpeed = navigationProvider.GetTargetSpeed();
                navigationProvider.SetTargetSpeed(0);
            }
            BaseUpdate();
            PrintState();

            state = States.WaitingForAid;
        }

        void DoWaitingForAid()
        {
            if (stateChanged)
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
