using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Lucide
{
    [ExecuteInEditMode]
    public class LucideLoader : EditorBehavior
    {
        public GlyphData glyph;

        Button _button;
        TextMeshProUGUI _text;

        void Awake()
        {
            RefreshComponents();
        }

        protected override void DelayedOnValidate()
        {
            Refresh();
        }

        protected override void RefreshComponents()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
        }

        public void Refresh()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            if (!_text || !glyph)
            {
                Debug.LogWarning($"unable to find TextMeshProUGUI on {gameObject.name}");
                return;
            }

            _text.text = $"\\u{glyph.unicodeString}";
            _text.color = glyph.textColor;

            //update color
            _button = GetComponent<Button>();
            if (!_button) return;
            if (!_button.interactable)
            {
                _text.color = new Color(
                glyph.textColor.r,
                glyph.textColor.g,
                glyph.textColor.b,
                0.75f);
            }
        }
    }
}
