using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils.Editor;

namespace UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    public class VideoFeed : Graphic, IPointerClickHandler, ISubScreen
    {
        [SerializeField]
        Camera feedCamera;

        public float sphereCastRadius = 0.2f;
        public UnityEvent<IPlayerClickable, Vector2> onClick;
        public UnityEvent<Vector2> onMiss;
        readonly RaycastHit[] _hits = new RaycastHit[32];

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

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _dirty = true;
        }
#endif

        public void OnPointerClick(PointerEventData eventData)
        {
            var ray = feedCamera.ScreenPointToRay(eventData.pressPosition);
            var size = Physics.SphereCastNonAlloc(ray, sphereCastRadius, _hits);
            if (size > 0)
            {
                Debug.DrawLine(ray.origin, _hits[0].point, Color.red, 1);

                IPlayerClickable target = null;
                for (var i = 0; i < size; i++)
                {
                    if (_hits[i].collider.TryGetComponent(out target)) break;
                }
                if (target != null)
                {
                    Debug.Log($"Hit Target at {eventData.pressPosition}");
                    onClick?.Invoke(target, eventData.pressPosition);
                }
                else
                {
                    Debug.Log($"No Target at {eventData.pressPosition}");
                    onMiss?.Invoke(eventData.pressPosition);
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            }

        }

        public void Show()
        {
            if (feedCamera)
            {
                feedCamera.enabled = true;
            }
            _dirty = true;
        }

        public void Hide()
        {
            if (feedCamera) feedCamera.enabled = false;
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

            if (feedCamera.gameObject.TryGetComponent(out CameraStackSynchronizer css))
            {
                css.ForceRefresh();
            }
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

            if (cam)
            {
                feedCamera = cam;
                cam.enabled = true;
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
