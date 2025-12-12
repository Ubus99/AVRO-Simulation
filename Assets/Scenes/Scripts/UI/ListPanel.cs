using TMPro;
using UnityEngine;

namespace Scenes.Scripts.UI
{
    [ExecuteAlways]
    public class ListPanel : MonoBehaviour
    {
        public string title;
        public bool leftIcon;
        public bool rightIcon;

        public TextMeshProUGUI titleText;
        public GameObject buttonLeft;
        public GameObject buttonRight;
        //public ListItem itemPrefab;

        void OnValidate()
        {
            if (titleText) titleText.text = title;
            if (buttonLeft) buttonLeft.SetActive(leftIcon);
            if (buttonRight) buttonRight.SetActive(rightIcon);
        }
    }
}
