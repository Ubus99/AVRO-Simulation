using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    public class VideoFeed : UIBehaviour, ICanvasElement, IPointerClickHandler
    {
        [SerializeField]
        Camera feedCamera;
        Canvas _canvas;
        bool _dirty;
        RectTransform _rectTransform;

        protected override void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (!_dirty) return;
            UpdateCamera();
            _dirty = false;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            _dirty = true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _dirty = true;
        }

        public void Rebuild(CanvasUpdate executing)
        {
        }

        public void LayoutComplete()
        {
            _dirty = true;
        }

        public void GraphicUpdateComplete()
        {
            _dirty = true;
        }

        void UpdateCamera()
        {
            if (!(feedCamera && _rectTransform && _canvas)) return;
            feedCamera.targetDisplay = _canvas.targetDisplay;

            var rect = ToScreenRect(_rectTransform, _canvas);

            feedCamera.pixelRect = rect;
            Debug.Log($"{name} updated {rect}");
        }

        static Rect ToScreenRect(RectTransform rectTransform, Canvas canvas, Camera cam = null)
        {
            if (canvas && canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                cam = canvas.worldCamera;
            }

            var worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            var min = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[0]);
            var max = min;

            for (var i = 1; i < 4; i++)
            {
                var sp = RectTransformUtility.WorldToScreenPoint(cam, worldCorners[i]);
                min = Vector2.Min(min, sp);
                max = Vector2.Max(max, sp);
            }

            var width = Mathf.Max(0f, max.x - min.x);
            var height = Mathf.Max(0f, max.y - min.y);

            return new Rect(min.x, min.y, width, height);
        }

        public void SetCamera(Camera cam)
        {
            if (feedCamera)
            {
                feedCamera.enabled = false;
            }
            feedCamera = cam;
            cam.enabled = true;
            _dirty = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}
