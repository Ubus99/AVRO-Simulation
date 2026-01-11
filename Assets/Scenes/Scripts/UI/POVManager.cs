using System.Linq;
using car_logic;
using UI;
using UnityEngine;
using Utils;

namespace Scenes.Scripts.UI
{
    public class POVManager : MonoBehaviour
    {
        public GameObject menu;
        public VideoFeed videoFeed;
        public ListPanel log;
        public ListPanel actions;
        public ListPanel layers;

        Canvas _canvas;

        RectTransform _menuInstance;

        void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            ServiceLocator.Instance.TryRegister<POVManager>(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.Instance.TryGet<OverviewManager>(out var overviewManager);
            overviewManager.OnFocusChange += HandleFocusChange;
        }

        void HandleFocusChange(ADSV_AI vehicle)
        {
            videoFeed.SetCamera(vehicle.povCamera);
            actions.UpdateList(vehicle.currentMission.alternativeRoutes.Select(ar => ar.GetData()));
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


            GUIUtils.PlaceAt(_menuInstance, screenPos, _canvas, container);
        }

        public void OnObstacleMissed(Vector2 pos)
        {
            if (!_menuInstance) return;

            menu.gameObject.SetActive(false);
            Destroy(_menuInstance.gameObject);
        }
    }
}
