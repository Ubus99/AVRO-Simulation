using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils.Lucide;
using Utils.Types;

namespace Scenes.Scripts.UI
{
    [ExecuteInEditMode]
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

        protected override void HandleIsDirty()
        {
            RefreshComponents();
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

            lowerSection.padding.left = data.leftIcon ? _lowerLeftPadding : 0;

            UpdateIconButton(rightButton, data.rightIcon);
            UpdateIconButton(rightButton, data.rightIcon);

            _button.interactable = itemData.selectable;
            _button.onClick = data.onClicked;
        }

        static void UpdateIconButton(LucideLoader button, GlyphData data)
        {
            if (data)
            {
                button.gameObject.SetActive(true);
                button.glyph.unicodeString = data.unicodeString;
                button.Refresh();
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public struct ElementData
        {
            public bool selectable;

            public GlyphData leftIcon;

            public GlyphData rightIcon;

            public string titleText;

            public string labelText;

            public Button.ButtonClickedEvent onClicked;
        }
    }
}
