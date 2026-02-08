using System;
using System.Collections.Generic;
using Scenes.Simulation.UI.ListItem;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI
{
    public class ListController : IDisposable
    {
        readonly VisualTreeAsset _itemTemplate;
        readonly List<IListItemData> _listData = new();
        readonly ListView _listView;

        public ListController(VisualElement root, VisualTreeAsset itemTemplate)
        {
            _itemTemplate = itemTemplate;
            _listView = root.Q<ListView>("actions-list");

            // ensure we don't double-subscribe if InitializeList is called more than once
            _listView.selectionChanged -= OnItemSelected;
            _listView.selectionChanged += OnItemSelected;

            ConfigureListView();
            RefreshListView();
        }

        // cleanup helper to avoid leaking selection subscription (call when appropriate)
        public void Dispose()
        {
            if (_listView != null)
            {
                _listView.selectionChanged -= OnItemSelected;
            }
        }

        public event Action<IListItemData> ItemSelectedEvent;

        void ConfigureListView()
        {
            _listView.makeItem = () =>
            {
                var instance = _itemTemplate.Instantiate();
                var instanceLogic = new ListItemController();
                instance.userData = instanceLogic;
                instanceLogic.LoadVisualElement(instance);
                return instance;
            };

            _listView.bindItem = (element, i) =>
            {
                if (i >= 0 && i < _listData.Count)
                    (element.userData as ListItemController)?.SetData(_listData[i]);
                else
                    (element.userData as ListItemController)?.SetData(null);
            };

            _listView.fixedItemHeight = 80;
            _listView.itemsSource = _listData;
        }

        // PUBLIC API: update the list contents later on
        public void UpdateList(IEnumerable<IListItemData> newItems)
        {
            if (newItems == null) throw new ArgumentNullException(nameof(newItems));

            _listData.Clear();
            _listData.AddRange(newItems);
            RefreshListView();
        }

        public void AddItem(IListItemData item)
        {
            _listData.Add(item);
            RefreshListView();
        }

        public bool RemoveItem(IListItemData item)
        {
            var removed = _listData.Remove(item);
            if (removed) RefreshListView();
            return removed;
        }

        public void Clear()
        {
            _listData.Clear();
            RefreshListView();
        }

        void RefreshListView()
        {
            if (_listView == null) return;

            // reassign itemsSource to ensure the ListView picks up changed Count,
            // then force a visual refresh of visible items.
            _listView.itemsSource = _listData;
            _listView.RefreshItems();
        }

        public void SelectItem(int idx)
        {
            _listView.SetSelection(idx);
        }

        public IListItemData GetSelectedItem()
        {
            return (IListItemData)_listView.selectedItem;
        }

        void OnItemSelected(IEnumerable<object> enumerable)
        {
            var selectedItem = (IListItemData)_listView.selectedItem;
            ItemSelectedEvent?.Invoke(selectedItem);
        }
    }
}
