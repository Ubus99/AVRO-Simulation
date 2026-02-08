using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

namespace Scenes.Simulation.UI.ListItem
{
    public class ListItemController
    {
        Image _leftImage;
        Label _mainText;
        Image _rightImage;
        Label _supportText;

        public void LoadVisualElement(VisualElement visualElement)
        {
            _leftImage = visualElement.Q<Image>("left-image");
            _rightImage = visualElement.Q<Image>("right-image");
            _mainText = visualElement.Q<Label>("main-text");
            _supportText = visualElement.Q<Label>("support-text");
        }

        public void SetData(IListItemData data)
        {
            if (_leftImage != null)
                _leftImage.vectorImage = data.leftImage;
            if (_rightImage != null)
                _rightImage.vectorImage = data.rightImage;
            _mainText.text = data.mainText;
            _supportText.text = data.supportText;
        }
    }
}
