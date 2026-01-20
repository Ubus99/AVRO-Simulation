using System;
using Utils.Lucide;

namespace UI
{
    public interface IListElement : IEquatable<IListElement>
    {
        public bool selectable { get; }

        public GlyphData leftIcon { get; }

        public GlyphData rightIcon { get; }

        public string titleText { get; }

        public string labelText { get; }

        bool IEquatable<IListElement>.Equals(IListElement other)
        {
            if (other == null) return false;
            return selectable == other.selectable &&
                   leftIcon == other.leftIcon &&
                   rightIcon == other.rightIcon &&
                   titleText == other.titleText &&
                   labelText == other.labelText;
        }
    }
}
