using System;
using TMPro;
using UI;
using UnityEngine;
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
        ColorGuide colors;

        [SerializeField]
        ElementData itemData;

        //privates
        Button _button;
        int _lowerLeftPadding;

        void Start()
        {
            RefreshComponents();
        }

        protected override void HandleIsDirty()
        {
            RefreshComponents();
            Refresh();
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

        void Refresh()
        {
            if (title) title.text = itemData.titleText;

            if (label)
            {
                label.gameObject.SetActive(itemData.labelText != "");
                label.text = itemData.labelText;
            }

            lowerSection.padding.left = itemData.leftIcon ? _lowerLeftPadding : 0;

            UpdateIconButton(leftButton, itemData.leftIcon);
            UpdateIconButton(rightButton, itemData.rightIcon);

            _button.interactable = itemData.selectable;
            _button.onClick = itemData.onClicked;

        }

        void UpdateColors()
        {
        }

        public void SetData(ElementData data)
        {
            itemData = data;
            Dirty = true;
        }

        static void UpdateIconButton(LucideLoader button, GlyphData data)
        {
            if (data)
            {
                button.gameObject.SetActive(true);
                button.glyph = data;
                button.UpdateComponents();
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
