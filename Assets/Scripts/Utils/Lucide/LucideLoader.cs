using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Types;

namespace Utils.Lucide
{
    [ExecuteInEditMode]
    public class LucideLoader : EditorBehavior
    {
        public GlyphData glyph;
        public Color glyphColor = Color.black;

        Button _button;
        TextMeshProUGUI _text;

        void Awake()
        {
            RefreshComponents();
        }

        protected override void HandleIsDirty()
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
            _text.color = glyphColor;

            //update color
            _button = GetComponent<Button>();
            if (!_button) return;
            if (!_button.interactable)
            {
                _text.color = new Color(
                glyphColor.r,
                glyphColor.g,
                glyphColor.b,
                0.75f);
            }
        }
    }
}
