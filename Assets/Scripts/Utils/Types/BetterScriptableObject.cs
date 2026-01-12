using System;
using UnityEngine;

namespace Utils.Types
{
    [ExecuteAlways]
    public abstract class BetterScriptableObject : ScriptableObject
    {
        void OnValidate()
        {
            HandleChanged();
            OnChanged?.Invoke();
        }

        protected abstract void HandleChanged();

        public event Action OnChanged;
    }
}
