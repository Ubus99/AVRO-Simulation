using Scenes.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Scenes.Scripts.Missions
{
    public class ADSVObstacle : MonoBehaviour
    {
        public InputActionReference pointAction;
        public InputActionReference selectAction;
        POVManager _manager;

        InputAction _pointAction;
        InputAction _selectAction;

        void Start()
        {
            ServiceLocator.Instance.TryGet(out _manager);
            _pointAction = pointAction.action;
            _selectAction = selectAction.action;
        }

        void Update()
        {
            if (!(_selectAction.IsPressed() && _manager))
                return;
            var pos = _pointAction.ReadValue<Vector2>();
            _manager.OpenAt(pos);
        }
    }
}
