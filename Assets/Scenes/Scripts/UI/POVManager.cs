using System.Linq;
using car_logic;
using UI;
using UnityEngine;
using Utils.Objects;

namespace Scenes.Scripts.UI
{
    public class POVManager : MonoBehaviour, ISubScreen
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
            ServiceLocator.instance.TryRegister<POVManager>(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.instance.TryGet<OverviewManager>(out var overviewManager);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            videoFeed.Show();
        }

        public void Hide()
        {
            videoFeed.Hide();
            gameObject.SetActive(false);
        }

        public void LoadData(ADSV_AI vehicle)
        {
            videoFeed.SetCamera(vehicle.povCamera);
            log.UpdateList(vehicle.currentMission.history);
            actions.UpdateList(vehicle.currentMission.alternativeRoutes.Select(ar => ar.GetData()));
        }

        public void OnObstacleClicked(IPlayerClickable playerClickable, Vector2 screenPos)
        {
            var container = videoFeed.rectTransform;

        }

        public void OnObstacleMissed(Vector2 pos)
        {
            if (!_menuInstance) return;

            menu.gameObject.SetActive(false);
            Destroy(_menuInstance.gameObject);
        }
    }
}
