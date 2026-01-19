using Scenes.Prefabs.UIComponents;
using Scenes.Scripts.UI;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using Utils.Objects;

namespace Scenes.Scripts.Missions
{
    public class ADSV_Obstacle : MonoBehaviour, IPlayerClickable
    {
        POVManager _manager;

        InputAction _pointAction;
        InputAction _selectAction;

        void Start()
        {
            ServiceLocator.instance.TryGet(out _manager);
        }

        public void ClickOn(object source, Vector2 position)
        {
        }
    }
}
