using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace car_logic
{
    public abstract class BaseStateMachine<T> : MonoBehaviour where T : Enum
    {
        float _lastMsgMillis;
        T _state;
        protected bool StateChanged { get; private set; }
        T PreviousState { get; set; }

        protected void BaseUpdate()
        {
            StateChanged = false;
        }

        protected T State
        {
            set
            {
                if (!value.Equals(PreviousState))
                {
                    StateChanged = true;
                }
                PreviousState = _state;
                _state = value;
            }
            get { return _state; }
        }

        protected void PrintState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !StateChanged)
                return;

            Debug.Log($"car in {State.ToString()} state");
            _lastMsgMillis = t;
        }

        protected void PrintEntryState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !StateChanged)
                return;

            Debug.Log($"car entering {State.ToString()} state");
            _lastMsgMillis = t;
        }

        protected void PrintExitState()
        {
            var t = Time.realtimeSinceStartup;
            if (t - _lastMsgMillis < 1 && !StateChanged)
                return;

            Debug.Log($"car exiting {State.ToString()} state");
            _lastMsgMillis = t;
        }
    }
}
