using System;
using Utils.Lucide;

namespace UI
{
    public interface IListable
    {
        public ListElementData ElementData();
    }

    public abstract class ListElementData : IEquatable<ListElementData>
    {
        public bool selectable { get; protected set; }

        public GlyphData leftIcon { get; protected set; }

        public GlyphData rightIcon { get; protected set; }

        public string titleText { get; protected set; }

        public string labelText { get; protected set; }

        public bool Equals(ListElementData other)
        {
            return selectable == other.selectable &&
                   Equals(leftIcon, other.leftIcon) &&
                   Equals(rightIcon, other.rightIcon) &&
                   titleText == other.titleText &&
                   labelText == other.labelText;
        }

        public override bool Equals(object obj)
        {
            return obj is ListElementData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(selectable, leftIcon, rightIcon, titleText, labelText);
        }
    }
}
