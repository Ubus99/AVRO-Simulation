using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

namespace Scenes.Scripts.UI
{
    [ExecuteAlways]
    public class ListPanel : MonoBehaviour
    {
        public string title;
        public bool leftIcon;
        public bool rightIcon;

        [Header("References")] //
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField] private GameObject buttonLeft;
        [SerializeField] private GameObject buttonRight;
        [SerializeField] private ListItem itemPrefab;
        [SerializeField] private GameObject body;

        private void OnValidate()
        {
            if (titleText) titleText.text = title;
            if (buttonLeft) buttonLeft.SetActive(leftIcon);
            if (buttonRight) buttonRight.SetActive(rightIcon);
        }

        public void UpdateList(IEnumerable<ListItem.ElementData> data)
        {
            ObjectManager.KillAllChildren(body.transform);

            foreach (var ed in data)
            {
                var li = Instantiate(itemPrefab, body.transform);
                var go = li.gameObject;

                li.SetData(ed);
            }
        }
    }
}