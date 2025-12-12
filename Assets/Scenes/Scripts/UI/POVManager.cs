using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Scenes.Scripts.UI
{
    public class POVManager : MonoBehaviour
    {
        public GameObject menu;
        public VideoFeed videoFeed;

        RectTransform _menuInstance;

        void Awake()
        {
            ServiceLocator.Instance.TryRegister<POVManager>(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.Instance.TryGet<OverviewManager>(out var overviewManager);
            overviewManager.OnFocusChange += vehicle => AssignCamera(vehicle.povCamera);
        }

        void AssignCamera(Camera cam)
        {
            videoFeed.SetCamera(cam);
        }

        public void OnObstacleClicked(IPlayerClickable playerClickable, Vector2 screenPos)
        {
            var panel = videoFeed.rectTransform;

            if (!_menuInstance)
            {
                var go = Instantiate(menu, videoFeed.transform, false);
                _menuInstance = go.GetComponent<RectTransform>();
                _menuInstance.gameObject.SetActive(true);
            }

            var canvas = videoFeed.canvas;
            Camera cam = null;
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            // parent already set by Instantiate(..., videoFeed.transform, false) — ensure layout is up-to-date
            LayoutRebuilder.ForceRebuildLayoutImmediate(_menuInstance);

            // convert screen -> panel-local (returns point relative to panel.pivot)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, screenPos, cam, out var localPoint);

            // clamp to panel so popup stays fully inside
            const bool clampToPanel = true;
            if (clampToPanel)
            {
                var panelSize = panel.rect.size;
                var popupSize = _menuInstance.rect.size;

                var min = -panelSize * panel.pivot + Vector2.Scale(popupSize, _menuInstance.pivot);
                var max = panelSize * (Vector2.one - panel.pivot) -
                          Vector2.Scale(popupSize, Vector2.one - _menuInstance.pivot);

                localPoint.x = Mathf.Clamp(localPoint.x, min.x, max.x);
                localPoint.y = Mathf.Clamp(localPoint.y, min.y, max.y);
            }

            // apply position: anchoredPosition if fixed anchors, otherwise localPosition for stretched anchors
            if (_menuInstance.anchorMin == _menuInstance.anchorMax)
            {
                _menuInstance.anchoredPosition = localPoint;
            }
            else
            {
                _menuInstance.localPosition = new Vector3(localPoint.x, localPoint.y, _menuInstance.localPosition.z);
            }
        }

        public void OnObstacleMissed(Vector2 pos)
        {
            if (!_menuInstance) return;

            menu.gameObject.SetActive(false);
            Destroy(_menuInstance.gameObject);
        }
    }
}
