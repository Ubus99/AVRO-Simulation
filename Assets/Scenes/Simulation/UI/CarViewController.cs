using System;
using System.Collections.Generic;
using Scenes.Simulation.Scripts;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Utils;
using Utils.Objects;

namespace Scenes.Simulation.UI
{
    public class CarViewController : MonoBehaviour
    {
        const string MissionStateKey = "SubState";

        [FormerlySerializedAs("actionItemTemplate")]
        [SerializeField]
        VisualTreeAsset itemTemplate;

        ListController<MissionSubState> _actionListController;
        ListController<MissionSo> _carListController;
        Button _confirmButton;
        CSVLogger _csvLogger;
        Image _mainImage;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _mainImage = root.Q<Image>("mainImage");

            _actionListController = new ListController<MissionSubState>(root, itemTemplate, "actions-list");
            _carListController = new ListController<MissionSo>(root, itemTemplate, "car-list");

            _confirmButton = root.Q<Button>("confirm-button");
            _confirmButton.clicked += CompleteMission;
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV logger");
            }
            _csvLogger.RegistrationEvent += () => _csvLogger.TryRegister(MissionStateKey);

            GameplayEvents.changeMissionEvent += LoadMission;
            GameplayEvents.missionQueueUpdateEvent += SetMissions;
        }

        void OnDisable()
        {
            if (_confirmButton != null) _confirmButton.clicked -= CompleteMission;

            GameplayEvents.changeMissionEvent -= LoadMission;
            GameplayEvents.missionQueueUpdateEvent -= SetMissions;
        }

        public event Action ReloadedEvent;

        void SetMissions(IEnumerable<MissionSo> missions)
        {
            _carListController.UpdateList(missions);
            _carListController.ItemSelectedEvent += data =>
            {
                GameplayEvents.changeMissionEvent?.Invoke(data as MissionSo);
            };
        }

        void LoadMission(MissionSo mission)
        {
            _mainImage.image = mission.options[0].mainTexture;

            _actionListController.UpdateList(mission.options);
            _actionListController.SelectItem(0);
            _actionListController.ItemSelectedEvent += SwitchSubStateView;

            ReloadedEvent?.Invoke();
        }

        void CompleteMission()
        {
            GameplayEvents.missionCompletedEvent?.Invoke(_actionListController.GetSelectedItem());
        }

        void SwitchSubStateView(IListItemData obj)
        {
            var subState = obj as MissionSubState? ?? default;
            if (subState.mainTexture)
            {
                _mainImage.image = subState.mainTexture;
            }

            _csvLogger.TryLog(MissionStateKey, subState.actionName.ToString());
        }
    }
}
