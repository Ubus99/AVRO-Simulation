using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(RawImage))]
    public class VideoFeed : MonoBehaviour, IPointerClickHandler
    {
        Camera _camera;
        RawImage _image;
        Ray _ray;

        void Awake()
        {
            _image = GetComponent<RawImage>();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_ray.origin, _ray.direction * 10);
        }

        void OnRectTransformDimensionsChange()
        {
            UpdateFeed();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _image.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint)) return;

            var uv = localPoint / _image.rectTransform.rect.size + Vector2.one / 2;
            var ray = _camera.ViewportPointToRay(new Vector3(uv.x, uv.y, 0));
            _ray = ray;
            if (Physics.Raycast(ray, out var hit))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }

        public void UpdateFeed(Camera camera)
        {
            _camera = camera;
            UpdateFeed();
        }

        public void UpdateFeed()
        {
            var size = Vector2Int.RoundToInt(_image.rectTransform.rect.size);
            if (_camera)
            {
                var tex = new RenderTexture(size.x, size.y, 16, RenderTextureFormat.ARGB32);
                _image.texture = tex;
                _camera.targetTexture = tex;
            }
            else
            {
                _image.texture = null;
            }
        }
    }
}
