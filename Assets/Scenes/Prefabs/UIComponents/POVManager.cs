using System;
using System.Collections.Generic;
using car_logic;
using Gameplay;
using Scenes.Scripts.UI;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.Objects;

namespace Scenes.Prefabs.UIComponents
{
    public class POVManager : MonoBehaviour, ISubScreen
    {
        [FormerlySerializedAs("menu")]
        public ListPanel menuPanel;

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
            menuPanel.Hide();
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
            menuPanel.Hide();
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
            //position panel
            var corners = new Vector3[4];
            origin.GetWorldCorners(corners);
            var topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            menuPanel.transform.parent as RectTransform,
            topRight,
            null,
            out var localPoint);

            if (origin.TryGetComponent(out LayoutGroup layoutGroup))
            {
                // var padding = layoutGroup.padding;
                // topRight += new Vector2(padding.right, padding.top);
            }

            var offset = menuPanel.rectTransform.rect.size / 2;
            offset.y = -offset.y;
            menuPanel.rectTransform.localPosition = localPoint + offset;

            //load data
            //_currentMission.GetObstacleData();

            menuPanel.Show();
        }
    }
}
