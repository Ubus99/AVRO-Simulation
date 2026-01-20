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
        ColorGuide colors;

        [SerializeField]
        ListElementData itemData;

        int _lowerLeftPadding;

        void Start()
        {
            RefreshComponents();
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

            button.interactable = itemData.selectable;
        }

        void UpdateColors()
        {
        }

        public void SetData(ListElementData data)
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
    }
}
