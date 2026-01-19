using System;
using System.Collections.Generic;
using car_logic;
using Gameplay;
using Scenes.Prefabs.UIComponents;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Objects;

namespace Scenes.Scripts.UI
{
    public class POVManager : MonoBehaviour, ISubScreen
    {
        public GameObject menu;
        public VideoFeed videoFeed;

        [FormerlySerializedAs("log")]
        public ListPanel logPanel;

        public ListPanel actionsPanel;

        public ListPanel layers;

        readonly Dictionary<ElementData, Action> _actions = new();

        Canvas _canvas;

        ADSV_AI _carInstance;
        Mission _currentMission;

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
            Debug.Log($"Showing {name}");
            gameObject.SetActive(true);
            videoFeed.Show();
        }

        public void Hide()
        {
            Debug.Log($"Hiding {name}");
            videoFeed.Hide();
            gameObject.SetActive(false);
        }

        public void LoadMission(ADSV_AI vehicle)
        {
            _carInstance = vehicle;
            _currentMission = vehicle.currentMission;

            videoFeed.SetCamera(_carInstance.povCamera);
            logPanel.OnItemSelected += (rt, ed) => { ShowEditMenu(rt); };
            logPanel.UpdateList(_currentMission.history);

            _actions.Clear();
            foreach (var ar in _currentMission.alternativeRoutes.routes)
            {
                _actions.Add(ar.ElementData(), () => _currentMission.SelectRoute(ar));
            }
            actionsPanel.OnItemSelected += (rt, ed) => { _actions[ed]?.Invoke(); };
            actionsPanel.UpdateList(_actions.Keys);
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

        public void ShowEditMenu(RectTransform origin)
        {
        }
    }
}
