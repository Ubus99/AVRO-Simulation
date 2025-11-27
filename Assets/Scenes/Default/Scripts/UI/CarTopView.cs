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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

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
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnClicked;
    }
}
