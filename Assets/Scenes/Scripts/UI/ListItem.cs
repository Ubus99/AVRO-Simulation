using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Scripts.UI
{
    public class ListItem : MonoBehaviour
    {
        [Header("References")]
        public GameObject leftButton;

        public GameObject rightButton;

        public TextMeshProUGUI title;

        public TextMeshProUGUI label;

        [Header("Options")]
        public bool selectable = true;

        public bool showLeftButton;

        public bool showRightButton;

        public bool showLabel;

        public string titleText;

        public string labelText;

        //privates
        Button _button;

        void Awake()
        {
            RefreshComponents();
        }

        void OnValidate()
        {
            RefreshComponents();

            _button.interactable = selectable;
            if (leftButton) leftButton.SetActive(showLeftButton);
            if (rightButton) rightButton.SetActive(showRightButton);
            if (title) title.text = titleText;
            if (label)
            {
                label.gameObject.SetActive(showLabel);
                label.text = labelText;
            }
        }

        void RefreshComponents()
        {
            _button = GetComponent<Button>();
        }

        public void ToggleSelectable()
        {
            selectable = !selectable;
        }
    }
}
