using System;
using Scenes.Simulation.Scripts;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using Utils.Objects;

namespace Scenes.Simulation.UI
{
    public class CarView : MonoBehaviour
    {
        [SerializeField]
        VisualTreeAsset actionItemTemplate;

        ListController<MissionSo.MissionSubState> _actionListController;
        Button _confirmButton;
        CSVLogger _csvLogger;
        Image _mainImage;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _mainImage = root.Q<Image>("mainImage");

            _actionListController = new ListController<MissionSo.MissionSubState>(root, actionItemTemplate);

            _confirmButton = root.Q<Button>("confirm-button");
            _confirmButton.clicked += CompleteMission;
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV logger");
            }
            _csvLogger.RegistrationEvent += () => _csvLogger.TryRegister("SubState");

            GameplayEvents.changeMissionEvent += LoadMission;
        }

        void OnDisable()
        {
            if (_confirmButton != null) _confirmButton.clicked -= CompleteMission;

            GameplayEvents.changeMissionEvent -= LoadMission;
        }

        public event Action ReloadedEvent;

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
            var subState = obj as MissionSo.MissionSubState? ?? default;
            _csvLogger.TryLog("SubState", subState.actionName.ToString());
            _mainImage.image = subState.mainTexture;
        }
    }
}
