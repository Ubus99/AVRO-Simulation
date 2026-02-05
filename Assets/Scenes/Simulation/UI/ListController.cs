using System.Collections.Generic;
using Scenes.Simulation.Scripts;
using Scenes.Simulation.UI.ListItem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI
{
    public class ListController
    {
        readonly List<ListItemData> _listData = new();
        VisualTreeAsset _itemTemplate;
        ListView _listView;

        public void InitializeList(VisualElement root, VisualTreeAsset itemTemplate)
        {
            _itemTemplate = itemTemplate;

            _listView = root.Q<ListView>("actions-list");

            FillList();

            _listView.selectionChanged += OnItemSelected;
        }

        void FillList()
        {
            _listView.makeItem = () =>
            {
                var instance = _itemTemplate.Instantiate();
                var instanceLogic = new ListItemController();

                instance.userData = instanceLogic;
                instanceLogic.LoadVisualElement(instance);

                return instance;
            };

            _listView.bindItem = (element, i) => { (element.userData as ListItemController)?.SetData(_listData[i]); };

            _listView.fixedItemHeight = 80;

            _listView.itemsSource = _listData;
        }

        void OnItemSelected(IEnumerable<object> enumerable)
        {
            var selectedItem = (ListItemData)_listView.selectedItem;
            // todo
        }
    }
}
