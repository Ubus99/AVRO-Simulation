using Scenes.Scripts.UI;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Scenes.Scripts.Missions
{
    public class ADSV_Obstacle : MonoBehaviour, IPlayerClickable
    {
        POVManager _manager;

        InputAction _pointAction;
        InputAction _selectAction;

        void Start()
        {
            ServiceLocator.Instance.TryGet(out _manager);
        }

        public void ClickOn(object source, Vector2 position)
        {
        }
    }
}
