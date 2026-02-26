using System;
using UnityEngine.UIElements;

namespace UI.ListItem
{
    public interface IListItemData : IEquatable<IListItemData>
    {
        public VectorImage LeftImage { get; }
        public VectorImage RightImage { get; }
        public bool RightIconInteractable { get; }
        public string RightButtonLabel { get; }
        public string MainText { get; }
        public string SupportText { get; }
        public int ApproximateHeight { get; }

        public new bool Equals(IListItemData other)
        {
            return Equals(LeftImage, other.LeftImage)
                   && Equals(RightImage, other.RightImage)
                   && RightIconInteractable == other.RightIconInteractable
                   && ApproximateHeight == other.ApproximateHeight;
        }
    }
}
