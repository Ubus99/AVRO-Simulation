using System;
using UnityEngine;

namespace car_logic
{
    public abstract class BaseStateMachine<T> : MonoBehaviour where T : Enum
    {
        float _lastMsgMillis;
        T _state;
        protected bool stateChanged { get; private set; }
        T previousState { get; set; }

        public T state
        {
            protected set
            {
                if (!value.Equals(previousState))
                {
                    stateChanged = true;
                }
                previousState = _state;
                _state = value;
            }
            get { return _state; }
        }

        protected void BaseUpdate()
        {
            stateChanged = false;
        }

        protected void PrintState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !stateChanged)
                return;

            Debug.Log($"car in {state.ToString()} state");
            _lastMsgMillis = t;
        }

        protected void PrintEntryState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !stateChanged)
                return;

            Debug.Log($"car entering {state.ToString()} state");
            _lastMsgMillis = t;
        }

        protected void PrintExitState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !stateChanged)
                return;

            Debug.Log($"car exiting {state.ToString()} state");
            _lastMsgMillis = t;
        }
    }
}
