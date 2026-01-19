using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class ModalUI :
        MonoBehaviour,
        IPointerExitHandler,
        IPointerEnterHandler,
        ISubScreen
    {
        public float secondsShown = 5;
        bool _hovering;

        float _timestamp;

        void Update()
        {
            if (_hovering)
            {
                _timestamp = Time.realtimeSinceStartup;
            }
            else if (Time.realtimeSinceStartup - _timestamp > secondsShown ||
                     Pointer.current.press.wasPressedThisFrame) // pressed but outside
            {
                Hide();
            }
        }

        void OnEnable()
        {
            _timestamp = Time.realtimeSinceStartup;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovering = true;
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            _hovering = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
