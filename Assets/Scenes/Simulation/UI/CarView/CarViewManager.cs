using System.Collections.Generic;
using Gameplay;
using Gameplay.Missions;
using InputHelpers;
using Logging;
using UI.ListItem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI.CarView
{
    public class CarViewManager : MonoBehaviour
    {
        [SerializeField]
        VectorImage motivationalImage;

        [SerializeField]
        VisualTreeAsset itemTemplate;

        [SerializeField]
        AudioSource missionAudio;

        [SerializeField]
        AudioSource submitAudio;

        CarViewController _carViewController;
        CSVLogger<UIRecord> _csvLogger;

        IList<MissionSo> _missions = new List<MissionSo>();

        MissionSo _selectedMission;
        MissionSubState _selectedSubState;

        void Awake()
        {
            _csvLogger = new CSVLogger<UIRecord>(GameplayGlobals.logName);

            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _carViewController = new CarViewController(root, itemTemplate);
            _carViewController.actionList.ItemSelectedEvent += OnSubStateSelected;
            _carViewController.carList.ItemSelectedEvent += data =>
            {
                MissionEvents.switchMissionEvent?.Invoke(data as MissionSo);
            };
            _carViewController.confirmButton.clicked += SubmitMission;
        }

        void Start()
        {
            MissionEvents.switchMissionEvent += OnSwitchToMission;
            MissionEvents.missionCompletedEvent += OnMissionCompleted;
            MissionEvents.missionQueuedEvent += OnMissionQueued;
            MissionEvents.missionQueueUpdateEvent += OnMissionQueueUpdate;

            GameplayGlobals.restartEvent += OnRestart;

            _carViewController.ShowNoMissions();
        }

        void Update()
        {
            CheckJumpPerformed();
        }

        void OnDisable()
        {
            _carViewController.actionList!.ItemSelectedEvent -= OnSubStateSelected;
            _carViewController.confirmButton!.clicked -= SubmitMission;

            MissionEvents.switchMissionEvent -= OnSwitchToMission;
            MissionEvents.missionCompletedEvent -= OnMissionCompleted;
            MissionEvents.missionQueuedEvent -= OnMissionQueued;
            MissionEvents.missionQueueUpdateEvent -= OnMissionQueueUpdate;

            GameplayGlobals.restartEvent -= OnRestart;
        }

        void OnMissionQueued(MissionSo mission)
        {
            missionAudio.Stop();
            missionAudio.Play();
        }

        void OnRestart()
        {
            _csvLogger.Dispose();
            _csvLogger = new CSVLogger<UIRecord>(GameplayGlobals.logName);
        }

        void CheckJumpPerformed()
        {
            if (!Keyboard.current.ctrlKey.isPressed) return;
            var i = InputUtils.DigitPressed();
            if (i == -1) return;

            _carViewController.JumpToItem(i);

            Debug.Log($"Jump to Item {i}");
        }

        void OnMissionQueueUpdate(IList<MissionSo> missions)
        {
            // remove reference by duplication
            _missions = missions.ToList();
            _carViewController.UpdateMissionList(missions);

            if (_carViewController.state == CarViewController.View.EmptyView)
            {
                _carViewController.ShowMissionAvailable();
            }
        }

        void OnSwitchToMission(MissionSo mission)
        {
            if (!mission) return;

            _selectedMission = mission;
            _selectedSubState = mission.options[0];
            _carViewController.ShowMission(mission);

            _csvLogger.Log(new UIRecord
            {
                view = _selectedMission.name,
                viewState = _selectedSubState.actionName.ToString()
            });
            Debug.Log($"loaded mission {mission.name} onto Screen");
        }

        void SubmitMission()
        {
            MissionEvents.missionSubmittedEvent?.Invoke(
            _carViewController.actionList.GetSelectedItem()
            );
        }

        void OnMissionCompleted(MissionSo mission)
        {
            // bullshit gate, in case something on the backend goes wrong
            if (mission != _selectedMission) return;

            _selectedMission = null;
            _selectedSubState = null;
            if (_missions.Count == 0)
            {
                _carViewController.ShowNoMissions();
                _csvLogger.Log(new UIRecord
                {
                    view = "No Missions Screen",
                    viewState = ""
                });
            }
            else
            {
                _carViewController.ShowMissionAvailable();
                _csvLogger.Log(new UIRecord
                {
                    view = "New Missions Screen",
                    viewState = ""
                });
            }
        }

        void OnSubStateSelected(IListItemData obj)
        {
            _selectedSubState = obj as MissionSubState;
            _carViewController.SwitchToSubState(_selectedSubState);

            if (!_selectedSubState) return;
            _csvLogger.Log(new UIRecord
            {
                view = _selectedMission.name,
                viewState = _selectedSubState.actionName.ToString()
            });
            Debug.Log($"switching to sub-state: {_selectedSubState.actionName}");
        }

        class UIRecord : BaseRecord
        {
            public string view { get; set; }
            public string viewState { get; set; }
        }
    }
}
