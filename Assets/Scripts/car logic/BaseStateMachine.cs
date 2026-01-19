using System;
using UnityEngine;

namespace car_logic
{
    public abstract class BaseStateMachine<T> : MonoBehaviour where T : Enum
    {
        public delegate void OnStateChangeHandler(T previousState, T newState);

        [SerializeField] private T _state;
        private float _lastMsgMillis;
        protected bool stateChanged { get; private set; }
        private T previousState { get; set; }

        public T state
        {
            set
            {
                if (!value.Equals(previousState))
                {
                    stateChanged = true;
                    OnStateChangeEvent?.Invoke(value);
                }

                previousState = _state;
                _state = value;
            }
            get => _state;
        }

        public event Action<T> OnStateChangeEvent;

        protected void BaseUpdate()
        {
            stateChanged = false;
        }

        protected void PrintState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !stateChanged)
                return;

            Debug.Log($"{name} in {state.ToString()} state");
            _lastMsgMillis = t;
        }

        protected void PrintEntryState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !stateChanged)
                return;

            Debug.Log($"{name} entering {state.ToString()} state");
            _lastMsgMillis = t;
        }

        protected void PrintExitState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !stateChanged)
                return;

            Debug.Log($"{name} exiting {state.ToString()} state");
            _lastMsgMillis = t;
        }
    }
}