using System;
using car_logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Default.Scripts.UI
{
    public class CarTopView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Dependencies")]
        public RawImage image;

        public Outline outline;
        public TextMeshProUGUI text;
        public ADSV_AI ADS;

        [Header("State")]
        public bool selected;

        public Color selectedColor = Color.white;

        public Color defaultColor = Color.white;

        // Update is called once per frame
        void LateUpdate()
        {
            if (!outline) return;

            outline.effectColor = selected ? selectedColor : defaultColor;
            text.text = ADS ? ADS.GetState() : "";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            selected = !selected;
            switch (selected)
            {
                case true:
                    OnTwiceClicked?.Invoke(this, EventArgs.Empty);
                    break;
                case false:
                    OnClicked?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public event EventHandler OnClicked;
        public event EventHandler OnTwiceClicked;
    }
}
