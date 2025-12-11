using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class VideoFeed : Graphic, IPointerClickHandler
    {
        [SerializeField]
        Camera feedCamera;

        public UnityEvent<IPlayerClickable, Vector2> onClick;
        public UnityEvent<Vector2> onMiss;
        Color _baseColor;

        Canvas _canvas;
        bool _dirty;
        RectTransform _rectTransform;

        protected override void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _baseColor = color;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            var ray = feedCamera.ScreenPointToRay(eventData.position);
            var hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                Debug.DrawLine(ray.origin, hits[0].point);

                if (!hits.Any(hit => hit.collider.TryGetComponent(out IPlayerClickable _)))
                {
                    onMiss?.Invoke(eventData.position);
                }
                else
                {
                    foreach (var hit in hits)
                    {
                        if (!hit.collider.TryGetComponent(out IPlayerClickable clickable))
                            continue;

                        onClick?.Invoke(clickable, eventData.position);
                    }
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            }

        }

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);
        }

        public override void LayoutComplete()
        {
            base.LayoutComplete();
            _dirty = true;
        }

        public override void GraphicUpdateComplete()
        {
            base.GraphicUpdateComplete();
            _dirty = true;
        }

        void UpdateCamera()
        {
            if (!(feedCamera && _rectTransform && _canvas)) return;
            feedCamera.targetDisplay = _canvas.targetDisplay;

            var rect = ToScreenRect(_rectTransform, _canvas);

            feedCamera.pixelRect = rect;
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
                feedCamera.gameObject.SetActive(false);
            }

            if (cam)
            {
                feedCamera = cam;
                cam.gameObject.SetActive(true);
                color = new Color(1, 1, 1, 0);
            }
            else
            {
                color = _baseColor;
            }

            _dirty = true;
        }
    }
}
