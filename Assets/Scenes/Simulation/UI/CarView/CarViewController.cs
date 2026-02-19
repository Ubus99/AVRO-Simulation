using System.Collections.Generic;
using Gameplay;
using Scenes.Simulation.Scripts;
using UI;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI.CarView
{
    public class CarViewController
    {
        public enum View
        {
            EmptyView,
            NewView,
            PovView
        }

        readonly ContentController _contentController;
        readonly FocusController _focusController;
        readonly VisualElement _selfRoot;

        public CarViewController(VisualElement root, VisualTreeAsset itemTemplate)
        {
            _selfRoot = root;
            _focusController = root.focusController;

            actionList = new ListController<MissionSubState>(root, itemTemplate, "actions-list");
            carList = new ListController<MissionSo>(root, itemTemplate, "car-list");

            //actionList.RegisterNavigation(NavigationMoveEvent.Direction.Left, carList);
            //actionList.RegisterNavigation(NavigationMoveEvent.Direction.Right, actionList);

            _contentController = new ContentController(root);
            confirmButton = root.Q<Button>("confirm-button");
        }

        public View state { get; private set; } = View.EmptyView;

        public Button confirmButton { get; }
        public ListController<MissionSubState> actionList { get; }
        public ListController<MissionSo> carList { get; }

        public void UpdateMissionList(IList<MissionSo> missions)
        {
            carList.UpdateList(missions);
        }

        public void ShowMission(MissionSo mission)
        {
            _contentController.LoadData(mission);
            _contentController.SwitchView(View.PovView);

            //select, just in case not already selected
            carList.SelectItem(mission);

            // update available options
            actionList.UpdateList(mission.options);
            actionList.SelectItem(0);

            confirmButton.SetEnabled(true);

            state = View.PovView;
        }

        public void ShowNoMissions()
        {
            _contentController.SwitchView(View.EmptyView);
            carList.ClearSelection();
            actionList.Clear();
            confirmButton.SetEnabled(false);

            state = View.EmptyView;
        }

        public void ShowMissionAvailable()
        {
            _contentController.SwitchView(View.NewView);
            carList.ClearSelection();
            actionList.Clear();
            confirmButton.SetEnabled(false);

            state = View.NewView;
        }

        public void SwitchToSubState(MissionSubState subState)
        {
            if (!subState.mainTexture) return;

            _contentController.SetMainImage(subState.mainTexture);
        }

        public void JumpToItem(int idx)
        {
            var e = _focusController.focusedElement as VisualElement;
            if (e is ListView list)
            {
                list.SetSelection(idx);
            }
        }
    }
}
