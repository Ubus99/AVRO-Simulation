using System;
using car_logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.Prefabs.UIComponents
{
    public class CarTopView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Dependencies")]
        public RawImage image;

        public RenderTexture renderTexture;

        public Outline outline;
        public TextMeshProUGUI text;
        public ADSV_AI ADS;

        readonly Rect _lastSize = new();

        void Start()
        {
            Rebuild();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!outline) return;

            text.text = ADS ? ADS.GetState() : "";
        }

        void OnRectTransformDimensionsChange()
        {
            Rebuild();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Rebuild()
        {
            var rectTransform = image.rectTransform;
            if (_lastSize == rectTransform.rect) return;

            var width = Mathf.FloorToInt(rectTransform.rect.width);
            var height = Mathf.FloorToInt(rectTransform.rect.height);

            Resize(renderTexture, width, height);
            image.texture = renderTexture;
        }

        static void Resize(RenderTexture renderTexture, int width, int height)
        {
            if (!renderTexture) return;

            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
        }

        public event EventHandler OnClicked;
        public event EventHandler OnTwiceClicked;
    }
}
