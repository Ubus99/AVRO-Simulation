using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    [ExecuteInEditMode]
    public class LucidePicker : MonoBehaviour
    {
        static readonly char[] Trim = { '&', '#' };

        public string iconID;
        public Color textColor = Color.black;
        Button _button;

        TextMeshProUGUI _text;

        void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
        }

        void OnValidate()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            if (!_text)
            {
                Debug.LogWarning($"unable to find TextMeshProUGUI on {gameObject.name}");
                return;
            }
            if (!int.TryParse(iconID.TrimStart(Trim), out var sanitizedIconID)) return;
            sanitizedIconID += 0; //offset
            var unicode = sanitizedIconID.ToString("X");
            _text.text = $"\\u{unicode}";
            _text.color = textColor;

            //update color
            _button = GetComponent<Button>();
            if (!_button) return;
            if (!_button.interactable)
            {
                _text.color = new Color(
                textColor.r,
                textColor.g,
                textColor.b,
                0.75f);
            }
        }
    }
}
