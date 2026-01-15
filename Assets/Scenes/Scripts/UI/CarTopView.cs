using System;
using car_logic;
using TMPro;
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
        public TextMeshProUGUI text;
        public ADSV_AI ADS;

        // Update is called once per frame
        void LateUpdate()
        {
            if (!outline) return;

            text.text = ADS ? ADS.GetState() : "";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnClicked;
        public event EventHandler OnTwiceClicked;
    }
}
