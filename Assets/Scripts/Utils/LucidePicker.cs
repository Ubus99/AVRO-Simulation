using TMPro;
using UnityEngine;

namespace Utils
{
    [ExecuteInEditMode]
    public class LucidePicker : MonoBehaviour
    {
        static readonly char[] Trim = { '&', '#' };
        public string iconID;
        TextMeshProUGUI _text;

        void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        void OnValidate()
        {
            if (!_text)
            {
                Debug.LogWarning("unable to find TextMeshProUGUI on button");
                return;
            }
            if (!int.TryParse(iconID.TrimStart(Trim), out var sanitizedIconID)) return;
            sanitizedIconID += 0; //offset
            var unicode = sanitizedIconID.ToString("X");
            _text.text = $"\\u{unicode}";
        }
    }
}
