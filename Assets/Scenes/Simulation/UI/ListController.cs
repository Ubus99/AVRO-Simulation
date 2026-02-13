using System;
using System.Collections.Generic;
using System.Linq;
using Scenes.Simulation.UI.ListItem;
using UI;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI
{
    public class ListController<T> : IDisposable where T : IListItemData
    {
        readonly VisualTreeAsset _itemTemplate;
        readonly List<T> _listData = new();
        readonly ListView _listView;

        public ListController(VisualElement root, VisualTreeAsset itemTemplate, string name)
        {
            _itemTemplate = itemTemplate;
            _listView = root.Q<ListView>(name);

            // ensure we don't double-subscribe if InitializeList is called more than once
            _listView.selectionChanged -= OnItemSelected;
            _listView.selectionChanged += OnItemSelected;

            ConfigureListView();
            RefreshListView();
        }

        public int count
        {
            get { return _listData.Count; }
        }

        // cleanup helper to avoid leaking selection subscription (call when appropriate)
        public void Dispose()
        {
            if (_listView != null)
            {
                _listView.selectionChanged -= OnItemSelected;
            }
        }

        public void RegisterNavigation(NavigationMoveEvent.Direction direction, VisualElement element)
        {
            _listView.RegisterCallback<
                NavigationMoveEvent,
                (NavigationMoveEvent.Direction direction,
                VisualElement element)>(
            GUIUtils.SwitchFocusTo,
            (direction, element));
        }

        public void RegisterNavigation<TOther>(
            NavigationMoveEvent.Direction direction,
            ListController<TOther> element)
            where TOther : IListItemData
        {
            RegisterNavigation(direction, element._listView);
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
            };

            _listView.fixedItemHeight = 80;
            _listView.itemsSource = _listData;
        }

        // PUBLIC API: update the list contents later on
        public void UpdateList(IEnumerable<T> newItems)
        {
            if (newItems == null) throw new ArgumentNullException(nameof(newItems));

            _listData.Clear();
            _listData.AddRange(newItems);
            RefreshListView();
        }

        public void AddItem(T item)
        {
            _listData.Add(item);
            RefreshListView();
        }

        public bool RemoveItem(T item)
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
            if (_listData.FirstOrDefault() is { } item)
                _listView.fixedItemHeight = item.approximateHeight;
            _listView.itemsSource = _listData;
            _listView.RefreshItems();
        }

        public void SelectItem(int idx)
        {
            _listView.SetSelection(idx);
        }

        public void SelectItem(T item)
        {
            SelectItem(_listData.IndexOf(item));
        }

        public void ClearSelection()
        {
            _listView.ClearSelection();
        }

        public T GetSelectedItem()
        {
            if (_listView.selectedItem is T data) return data;
            //else
            throw new InvalidCastException();
        }

        void OnItemSelected(IEnumerable<object> enumerable)
        {
            var selectedItem = (IListItemData)_listView.selectedItem;
            if (selectedItem == null) return;
            ItemSelectedEvent?.Invoke(selectedItem);
        }
    }
}
