using System;
using System.Linq;
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

        ListController _actionListController;

        Image _mainImage;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _mainImage = root.Q<Image>("mainImage");
            _actionListController = new ListController(root, actionItemTemplate);

            GameplayEvents.changeMissionEvent += LoadMission;
        }

        //expose UI update
        public event Action ReloadedEvent;

        void LoadMission(MissionSo mission)
        {
            _mainImage.image = mission.options[0].mainTexture;

            _actionListController.UpdateList(mission.options.Cast<IListItemData>());
            _actionListController.SelectItem(0);
            _actionListController.ItemSelectedEvent += UpdateImage;

            ReloadedEvent?.Invoke();
        }

        void UpdateImage(IListItemData obj)
        {
            _mainImage.image = ((MissionSo.MissionSubState)obj).mainTexture;
        }
    }
}
