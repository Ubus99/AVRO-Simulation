using UnityEngine;
using Utils.Types;

namespace UI
{
    [CreateAssetMenu(fileName = "ColorGuide", menuName = "custom/ColorGuide", order = 0)]
    public class ColorGuide : BetterScriptableObject
    {
        [Header("BasicColors")]
        public Color textColor = Color.black;

        [Header("Hierarchy")]
        public Color layer1Color = Color.white;

        public Color layer2Color = Color.white;

        public Color layer3Color = Color.white;

        [Header("Interactive")]
        public Color selectedColor = Color.white;

        public Color hoverColor = Color.white;
        
        public Color disabledColor = Color.white;

        protected override void HandleChanged()
        {
        }
    }
}
