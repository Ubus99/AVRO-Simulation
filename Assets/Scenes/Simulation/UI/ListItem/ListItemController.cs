using System;
using UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Scenes.Simulation.UI.ListItem
{
    public class ListItemController
    {
        Image _leftImage;
        Label _mainText;
        Image _rightImage;
        VisualElement _selfRoot;
        Label _supportText;

        public void LoadVisualElement(VisualElement visualElement)
        {
            _selfRoot = visualElement;
            _leftImage = visualElement.Q<Image>("left-icon");
            _rightImage = visualElement.Q<Image>("right-icon");
            _mainText = visualElement.Q<Label>("main-text");
            _supportText = visualElement.Q<Label>("support-text");
        }

        public void SetData(IListItemData data)
        {
            UpdateIcon(_leftImage, data.leftImage, "hideable");
            UpdateIcon(_rightImage, data.rightImage, "hideable2");
            UpdateLabel(_mainText, data.mainText);
            UpdateLabel(_supportText, data.supportText);
        }

        void UpdateLabel(Label label, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                label.AddToClassList("hidden");
            }
            else
            {
                label.text = text;
                label.RemoveFromClassList("hidden");
            }
        }

        void UpdateIcon(Image target, VectorImage icon, string group = null)
        {
            var elements = _selfRoot.Query<VisualElement>(className: group).ToList();
            if (elements.Count == 0)
            {
                throw new ArgumentNullException();
            }

            GUIUtils.ToggleHidden(elements, !icon);
            target.vectorImage = icon;
        }
    }
}
