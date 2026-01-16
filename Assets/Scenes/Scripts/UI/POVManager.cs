using System.Linq;
using car_logic;
using UI;
using UnityEngine;
using UnityEngine.UI;
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

        ADSV_AI _carInstance;

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

        public void LoadMission(ADSV_AI vehicle)
        {
            _carInstance = vehicle;
            videoFeed.SetCamera(_carInstance.povCamera);
            log.UpdateList(_carInstance.currentMission.history);
            actions.UpdateList(_carInstance.currentMission.alternativeRoutes.Select(ar => ar.GetData()));
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

        // footer actions
        public void StopCar()
        {

        }

        public void ContinueCar()
        {

        }

        public void EmergencyStopCar()
        {

        }

        public void ShowEditMenu(Button origin)
        {
        }
    }
}
