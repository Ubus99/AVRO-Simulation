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
            var container = videoFeed.rectTransform;

            if (!_menuInstance)
            {
                var go = Instantiate(menu);
                _menuInstance = go.GetComponent<RectTransform>();
                _menuInstance.gameObject.SetActive(true);
            }
            
            screenPos.y *= -1; // flip coordinate system
            GUIUtils.PlaceIn(_menuInstance, screenPos, container, false);
        }

        public void OnObstacleMissed(Vector2 pos)
        {
            if (!_menuInstance) return;

            menu.gameObject.SetActive(false);
            Destroy(_menuInstance.gameObject);
        }
    }
}
