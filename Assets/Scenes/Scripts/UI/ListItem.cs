using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Utils.Lucide;

namespace Scenes.Scripts.UI
{
    public class ListItem : EditorBehavior
    {
        [Header("References")]
        [SerializeField]
        LucideLoader leftButton;

        [SerializeField]
        LucideLoader rightButton;

        [SerializeField]
        TextMeshProUGUI title;

        [SerializeField]
        LayoutGroup lowerSection;

        [SerializeField]
        TextMeshProUGUI label;

        [Header("Options")]
        [SerializeField]
        ElementData itemData;

        //privates
        Button _button;
        int _lowerLeftPadding;

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
            _lowerLeftPadding = lowerSection.padding.left;
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
                lowerSection.padding.left = _lowerLeftPadding;
                leftButton.gameObject.SetActive(true);
                leftButton.glyph.unicodeString = data.leftIcon.unicodeString;
                leftButton.Refresh();
            }
            else
            {
                lowerSection.padding.left = 0;
                leftButton.gameObject.SetActive(false);
            }

            if (data.rightIcon)
            {
                rightButton.gameObject.SetActive(true);
                rightButton.glyph.unicodeString = data.rightIcon.unicodeString;
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

            [FormerlySerializedAs("leftLoader")]
            public GlyphData leftIcon;

            [FormerlySerializedAs("rightLoader")]
            public GlyphData rightIcon;

            public string titleText;

            public string labelText;
        }
    }
}
