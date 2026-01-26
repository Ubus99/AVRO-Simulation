using System;
using System.Collections.Generic;
using Scenes.Prefabs.UIComponents;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.Objects;

namespace Scenes.Scripts.Missions
{
    public class AdsObstacle : MonoBehaviour
    {
        public enum State
        {
            Safe,
            DifficultTerrain,
            Blocking
        }

        [SerializeField]
        List<State> availableStates = new();

        POVManager _manager;

        InputAction _pointAction;
        InputAction _selectAction;

        State _state = State.Blocking;

        public Action<AdsObstacle, State> OnStateChangedEvent;

        void Start()
        {
            ServiceLocator.instance.TryGet(out _manager);
        }

        public void SetState(State state)
        {
            _state = state;
            OnStateChangedEvent?.Invoke(this, _state);
        }

        public List<State> GetAvailableStates()
        {
            return availableStates;
        }
    }
}
