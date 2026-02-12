using System;
using System.Collections.Generic;
using Gameplay;
using Scenes.Simulation.Scripts;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Utils;
using Utils.Objects;

namespace Scenes.Simulation.UI.CarView
{
    public class CarViewController : MonoBehaviour
    {
        const string MissionStateKey = "SubState";

        [SerializeField]
        VectorImage motivationalImage;

        [FormerlySerializedAs("actionItemTemplate")]
        [SerializeField]
        VisualTreeAsset itemTemplate;

        ListController<MissionSubState> _actionListController;
        ListController<MissionSo> _carListController;
        Button _confirmButton;
        ContentController _contentController;
        CSVLogger _csvLogger;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _actionListController = new ListController<MissionSubState>(root, itemTemplate, "actions-list");
            _actionListController.ItemSelectedEvent += SwitchSubStateView;

            _carListController = new ListController<MissionSo>(root, itemTemplate, "car-list");
            _carListController.ItemSelectedEvent += data =>
            {
                GameplayGlobals.missionSelectedEvent?.Invoke(data as MissionSo);
            };

            _contentController = new ContentController(root);

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

            GameplayGlobals.missionSelectedEvent += LoadMission;
            GameplayGlobals.missionQueueUpdateEvent += SetMissions;

            LoadMission(null);
        }

        void OnDisable()
        {
            if (_confirmButton != null) _confirmButton.clicked -= CompleteMission;

            GameplayGlobals.missionSelectedEvent -= LoadMission;
            GameplayGlobals.missionQueueUpdateEvent -= SetMissions;
        }

        public event Action ReloadedEvent;

        void SetMissions(IEnumerable<MissionSo> missions)
        {
            _carListController.UpdateList(missions);
            _carListController.ClearSelection();
        }

        void LoadMission(MissionSo mission)
        {
            _contentController.LoadData(mission);
            _confirmButton.SetEnabled(mission);

            if (mission)
            {
                _carListController.SelectItem(mission);

                _actionListController.UpdateList(mission.options);
                _actionListController.SelectItem(0);

                Debug.Log($"loaded mission {mission.name} onto Screen");
            }
            else
            {
                _actionListController.Clear();

                Debug.Log("loaded motivational message onto Screen");
            }
            
            ReloadedEvent?.Invoke();
        }

        void CompleteMission()
        {
            GameplayGlobals.missionSubmittedEvent?.Invoke(_actionListController.GetSelectedItem());
        }

        void SwitchSubStateView(IListItemData obj)
        {
            var subState = obj as MissionSubState? ?? default;
            if (subState.mainTexture)
            {
                _contentController.SetMainImage(subState.mainTexture);
            }

            Debug.Log($"switching to sub-state: {subState}");
            _csvLogger.TryLog(MissionStateKey, subState.actionName.ToString());
        }
    }
}
