using System;
using System.Collections.Generic;
using Gameplay;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Utils;
using Utils.Objects;

namespace Scenes.Simulation.UI.CarView
{
    public class CarViewManager : MonoBehaviour
    {
        const string MissionStateKey = "SubState";

        [SerializeField]
        VectorImage motivationalImage;

        [FormerlySerializedAs("actionItemTemplate")]
        [SerializeField]
        VisualTreeAsset itemTemplate;

        CarViewController _carViewController;
        CSVLogger _csvLogger;

        InputAction _jumpToAction;

        IList<MissionSo> _missions;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _carViewController = new CarViewController(root, itemTemplate);
            _carViewController.actionList.ItemSelectedEvent += OnSubStateSelected;
            _carViewController.carList.ItemSelectedEvent += data =>
            {
                GameplayGlobals.switchMissionEvent?.Invoke(data as MissionSo);
            };
            _carViewController.confirmButton.clicked += SubmitMission;

            _jumpToAction = InputSystem.actions.FindAction("JumpTo");
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV logger");
            }
            _csvLogger.RegistrationEvent += () => _csvLogger.TryRegister(MissionStateKey);

            GameplayGlobals.switchMissionEvent += OnSwitchToMission;
            GameplayGlobals.missionCompletedEvent += OnMissionCompleted;
            GameplayGlobals.missionQueueUpdateEvent += OnMissionQueueUpdate;

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

            GameplayGlobals.switchMissionEvent -= OnSwitchToMission;
            GameplayGlobals.missionCompletedEvent -= OnMissionCompleted;
            GameplayGlobals.missionQueueUpdateEvent -= OnMissionQueueUpdate;
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
            _missions = missions;
            _carViewController.UpdateMissionList(missions);

            if (_carViewController.state == CarViewController.View.EmptyView)
            {
                _carViewController.ShowMissionAvailable();
            }
        }

        void OnSwitchToMission(MissionSo mission)
        {
            if (!mission) return;

            _carViewController.ShowMission(mission);
            Debug.Log($"loaded mission {mission.name} onto Screen");
        }

        void SubmitMission()
        {
            GameplayGlobals.missionSubmittedEvent?.Invoke(
            _carViewController.actionList.GetSelectedItem()
            );
        }

        void OnMissionCompleted()
        {
            if (_missions.Count == 0)
            {
                _carViewController.ShowNoMissions();
            }
            else
            {
                _carViewController.ShowMissionAvailable();
            }
        }

        void OnSubStateSelected(IListItemData obj)
        {
            var subState = obj as MissionSubState;
            _carViewController.SwitchToSubState(subState);

            if (!subState) return;
            Debug.Log($"switching to sub-state: {subState.actionName}");
            _csvLogger.TryLog(MissionStateKey, subState.actionName.ToString());
        }
    }
}
