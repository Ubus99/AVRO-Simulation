using System;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor.Data
{
    [Serializable]
    internal class ElementData
    {
        [SerializeField]
        bool _isListToggleActive;

        public ElementData(ElementSettings elementSettings)
        {
            Settings = elementSettings;
        }

        public ElementSettings Settings { get; }

        public bool ShowAsList
        {
            get { return Settings.HasListDrawerToggle && IsListToggleActive; }
        }

        public bool IsListToggleActive
        {
            get { return _isListToggleActive; }
            set { _isListToggleActive = value; }
        }

        public DisplayType EffectiveDisplayType
        {
            get { return ShowAsList ? DisplayType.List : Settings.DisplayType; }
        }
    }
}
