using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using Utils.Objects;

namespace Scenes.Prefabs.UIComponents
{
    [ExecuteAlways]
    public class ListPanel : MonoBehaviour, ISubScreen
    {
        public string title;
        public bool leftIcon;
        public bool rightIcon;

        [Header("References")] //
        [SerializeField]
        TextMeshProUGUI titleText;

        [SerializeField]
        GameObject buttonLeft;

        [SerializeField]
        GameObject buttonRight;

        [SerializeField]
        ListItem itemPrefab;

        [SerializeField]
        GameObject body;

        public RectTransform rectTransform
        {
            get { return transform as RectTransform; }
        }

        void OnValidate()
        {
            if (titleText) titleText.text = title;
            if (buttonLeft) buttonLeft.SetActive(leftIcon);
            if (buttonRight) buttonRight.SetActive(rightIcon);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public event Action<RectTransform, ElementData> OnItemSelected;

        public void UpdateList(IEnumerable<ElementData> data)
        {
            Debug.Log($"Updating List {title} in {name}");
            ObjectManagementUtility.KillAllChildren(body.transform);

            foreach (var ed in data)
            {
                var li = Instantiate(itemPrefab, body.transform);
                li.name = $"Item_{ed.titleText}";
                li.GetButton().onClick.AddListener(() => OnItemSelected?.Invoke(li.transform as RectTransform, ed));
                var go = li.gameObject;

                li.SetData(ed);
            }
        }
    }
}
