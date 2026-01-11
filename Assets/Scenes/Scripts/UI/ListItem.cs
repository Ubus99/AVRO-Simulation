using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Scenes.Scripts.UI
{
    public class ListItem : EditorBehavior
    {
        [Header("References")]
        [SerializeField]
        LucidePicker leftButton;

        [SerializeField]
        LucidePicker rightButton;

        [SerializeField]
        TextMeshProUGUI title;

        [SerializeField]
        TextMeshProUGUI label;

        [Header("Options")]
        [SerializeField]
        ElementData itemData;

        //privates
        Button _button;

        void Awake()
        {
            RefreshComponents();
        }

        protected override void DelayedOnValidate()
        {
            RefreshComponents();

            _button.interactable = itemData.selectable;
            if (leftButton) leftButton.gameObject.SetActive(itemData.leftIcon);
            if (rightButton) rightButton.gameObject.SetActive(itemData.rightIcon);
            SetData(itemData);
        }

        protected override void RefreshComponents()
        {
            _button = GetComponent<Button>();
        }

        public void ToggleSelectable()
        {
            itemData.selectable = !itemData.selectable;
        }

        public void SetData(ElementData data)
        {
            itemData = data;

            if (title) title.text = itemData.titleText;

            if (label)
            {
                label.gameObject.SetActive(itemData.labelText != "");
                label.text = itemData.labelText;
            }

            if (data.leftIcon)
            {
                leftButton.gameObject.SetActive(true);
                leftButton.unicodeString = data.leftIcon.unicodeString;
                leftButton.Refresh();
            }
            else
            {
                leftButton.gameObject.SetActive(false);
            }

            if (data.rightIcon)
            {
                rightButton.gameObject.SetActive(true);
                rightButton.unicodeString = data.rightIcon.unicodeString;
                rightButton.Refresh();
            }
            else
            {
                rightButton.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public struct ElementData
        {
            public bool selectable;

            public LucidePicker leftIcon;

            public LucidePicker rightIcon;

            public string titleText;

            public string labelText;
        }
    }
}
