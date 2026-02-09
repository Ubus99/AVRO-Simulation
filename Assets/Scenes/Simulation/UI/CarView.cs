using System;
using Scenes.Simulation.Scripts;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI
{
    public class CarView : MonoBehaviour
    {
        [SerializeField]
        VisualTreeAsset actionItemTemplate;

        ListController<MissionSo.MissionSubState> _actionListController;
        Button _confirmButton;
        Image _mainImage;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;


            _mainImage = root.Q<Image>("mainImage");

            _actionListController = new ListController<MissionSo.MissionSubState>(root, actionItemTemplate);

            _confirmButton = root.Q<Button>("confirm-button");
            _confirmButton.clicked += CompleteMission;


            GameplayEvents.changeMissionEvent += LoadMission;
        }

        void OnDisable()
        {
            _confirmButton.clicked -= CompleteMission;

            GameplayEvents.changeMissionEvent -= LoadMission;
        }

        public event Action ReloadedEvent;

        void LoadMission(MissionSo mission)
        {
            _mainImage.image = mission.options[0].mainTexture;

            _actionListController.UpdateList(mission.options);
            _actionListController.SelectItem(0);
            _actionListController.ItemSelectedEvent += UpdateImage;

            ReloadedEvent?.Invoke();
        }

        void CompleteMission()
        {
            GameplayEvents.missionCompletedEvent?.Invoke(_actionListController.GetSelectedItem());
        }

        void UpdateImage(IListItemData obj)
        {
            _mainImage.image = ((MissionSo.MissionSubState)obj).mainTexture;
        }
    }
}
