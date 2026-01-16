using System;
using Utils.Lucide;

namespace UI
{
    public interface IListable
    {
        public ElementData ElementData();
    }

    [Serializable]
    public struct ElementData : IEquatable<ElementData>
    {
        public bool selectable;

        public GlyphData leftIcon;

        public GlyphData rightIcon;

        public string titleText;

        public string labelText;

        public bool Equals(ElementData other)
        {
            return selectable == other.selectable &&
                   Equals(leftIcon, other.leftIcon) &&
                   Equals(rightIcon, other.rightIcon) &&
                   titleText == other.titleText &&
                   labelText == other.labelText;
        }

        public override bool Equals(object obj)
        {
            return obj is ElementData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(selectable, leftIcon, rightIcon, titleText, labelText);
        }
    }
}
