using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utils.Lucide;
using Utils.Types;

namespace Scenes.Prefabs.UIComponents
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
        Button button;

        [SerializeField]
        TextMeshProUGUI label;

        [Header("Options")]
        [SerializeField]
        bool selectable;

        [SerializeField]
        string titleText;

        [SerializeField]
        string labelText;

        [SerializeField]
        GlyphData rightIcon;

        [SerializeField]
        GlyphData leftIcon;

        [SerializeField]
        ColorGuide colors;

        // private
        int _lowerLeftPadding;

        void Start()
        {
            RefreshComponents();
        }

        public bool Equals(IListElement other)
        {
            throw new NotImplementedException();
        }

        public Button GetButton()
        {
            return button;
        }

        protected override void HandleIsDirty()
        {
            RefreshComponents();
            Refresh();
        }

        protected override void RefreshComponents()
        {
            button = GetComponent<Button>();
            _lowerLeftPadding = lowerSection.padding.left;
        }

        public void ToggleSelectable()
        {
            selectable = !selectable;
        }

        void Refresh()
        {
            if (title) title.text = titleText;

            if (label)
            {
                label.gameObject.SetActive(labelText != "");
                label.text = labelText;
            }

            lowerSection.padding.left = leftIcon ? _lowerLeftPadding : 0;

            UpdateIconButton(leftButton, leftIcon);
            UpdateIconButton(rightButton, rightIcon);

            button.interactable = selectable;

            // LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public void SetData(IListElement data)
        {
            selectable = data.selectable;
            titleText = data.titleText;
            labelText = data.labelText;
            leftIcon = data.leftIcon;
            rightIcon = data.rightIcon;

            Refresh();
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
    }
}
