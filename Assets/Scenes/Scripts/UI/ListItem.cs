using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Scripts.UI
{
    public class ListItem : MonoBehaviour
    {
        [Header("References")] public GameObject leftButton;

        [SerializeField] private GameObject rightButton;

        [SerializeField] private TextMeshProUGUI title;

        [SerializeField] private TextMeshProUGUI label;

        [Header("Options")] public bool selectable = true;

        [SerializeField] private bool showLeftButton;

        [SerializeField] private bool showRightButton;

        [SerializeField] private bool showLabel;

        [SerializeField] private ElementData itemData;

        //privates
        private Button _button;

        private void Awake()
        {
            RefreshComponents();
        }

        private void OnValidate()
        {
            RefreshComponents();

            _button.interactable = selectable;
            if (leftButton) leftButton.SetActive(showLeftButton);
            if (rightButton) rightButton.SetActive(showRightButton);
            SetData(itemData);
        }

        private void RefreshComponents()
        {
            _button = GetComponent<Button>();
        }

        public void ToggleSelectable()
        {
            selectable = !selectable;
        }

        public void SetData(ElementData data)
        {
            itemData = data;
            if (title) title.text = itemData.titleText;

            if (!label) return;

            label.gameObject.SetActive(showLabel);
            label.text = itemData.labelText;
        }

        [Serializable]
        public struct ElementData
        {
            public string titleText;

            public string labelText;
        }
    }
}