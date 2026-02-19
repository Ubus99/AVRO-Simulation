using System;
using UnityEngine.UIElements;

namespace UI.ListItem
{
    public interface IListItemData : IEquatable<IListItemData>
    {
        public VectorImage leftImage { get; }
        public VectorImage rightImage { get; }
        public string mainText { get; }
        public string supportText { get; }
        public int approximateHeight { get; }
    }
}
