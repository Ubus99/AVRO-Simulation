using System;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace UI.ListItem
{
    public class ListItemController
    {
        Action _buttonAction;
        Image _leftIcon;
        Label _mainText;
        Image _rightIcon;
        Button _rightIconButton;
        VisualElement _selfRoot;
        Label _supportText;

        public void LoadVisualElement(VisualElement visualElement)
        {
            _selfRoot = visualElement;

            _leftIcon = visualElement.Q<Image>("left-icon");
            _rightIcon = visualElement.Q<Image>("right-icon");

            _rightIconButton = visualElement.Q<Button>("right-icon-button");

            _mainText = visualElement.Q<Label>("main-text");
            _supportText = visualElement.Q<Label>("support-text");
        }

        public void SetData(IListItemData data)
        {
            UpdateIcon(_leftIcon, data.LeftImage, true, "hideable");
            UpdateIcon(_rightIcon, data.RightImage, !data.RightIconInteractable);

            UpdateIconButton(_rightIconButton,
            data.RightButtonLabel,
            data.RightImage,
            data.ButtonAction,
            data.RightIconInteractable);

            UpdateLabel(_mainText, data.MainText);
            UpdateLabel(_supportText, data.SupportText);
        }

        public void SetSelected(bool selected)
        {
            _rightIconButton.SetEnabled(selected);
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

        void UpdateIcon(Image target, VectorImage icon, bool enabled, string group = null)
        {
            var elements = group != null
                ? _selfRoot.Query<VisualElement>(className: group).ToList()
                : target.Query<VisualElement>().ToList();
            if (elements.Count == 0)
            {
                throw new ArgumentNullException();
            }

            GUIUtils.ToggleHidden(elements, !icon || !enabled);
            target.vectorImage = icon;
        }

        void UpdateIconButton(Button target, string label, VectorImage icon, Action action, bool enabled,
            string group = null)
        {
            var elements = group != null
                ? _selfRoot.Query<VisualElement>(className: group).ToList()
                : target.Query<VisualElement>().ToList();
            if (elements.Count == 0)
            {
                throw new ArgumentNullException();
            }

            GUIUtils.ToggleHidden(elements, !icon || !enabled);

            if (_buttonAction != null)
                target.clicked -= _buttonAction;

            _buttonAction = action;
            target.clicked += _buttonAction;

            target.iconImage = Background.FromVectorImage(icon);
            target.text = label;
        }
    }
}
