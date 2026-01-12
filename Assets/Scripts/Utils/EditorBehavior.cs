using UnityEngine;

namespace Utils
{
    public abstract class EditorBehavior : MonoBehaviour
    {
        protected bool _dirty;

        void Update()
        {
            if (_dirty)
            {
                HandleIsDirty();
                _dirty = false;
            }
        }

        void OnValidate()
        {
            _dirty = true;
        }

        protected abstract void HandleIsDirty();
        protected abstract void RefreshComponents();
    }
}
