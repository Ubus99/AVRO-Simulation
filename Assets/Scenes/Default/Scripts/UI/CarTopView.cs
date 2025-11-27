using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Scripts.UI
{
    public class CarTopView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Dependencies")]
        public RawImage image;

        public Outline outline;

        [Header("State")]
        public bool selected;

        public Color selectedColor = Color.white;

        public Color defaultColor = Color.white;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!outline) return;

            outline.effectColor = selected ? selectedColor : defaultColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            selected = !selected;
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnClicked;
    }
}
