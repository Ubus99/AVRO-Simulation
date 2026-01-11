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
            if (leftButton) leftButton.gameObject.SetActive(itemData.leftLoader);
            if (rightButton) rightButton.gameObject.SetActive(itemData.rightLoader);
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

            if (data.leftLoader)
            {
                leftButton.gameObject.SetActive(true);
                leftButton.glyph.unicodeString = data.leftLoader.glyph.unicodeString;
                leftButton.Refresh();
            }
            else
            {
                leftButton.gameObject.SetActive(false);
            }

            if (data.rightLoader)
            {
                rightButton.gameObject.SetActive(true);
                rightButton.glyph.unicodeString = data.rightLoader.glyph.unicodeString;
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

            [FormerlySerializedAs("leftIcon")]
            public LucideLoader leftLoader;

            [FormerlySerializedAs("rightIcon")]
            public LucideLoader rightLoader;

            public string titleText;

            public string labelText;
        }
    }
}
