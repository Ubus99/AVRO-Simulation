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
                DelayedOnValidate();
                _dirty = false;
            }
        }

        void OnValidate()
        {
            _dirty = true;
        }

        protected abstract void DelayedOnValidate();
        protected abstract void RefreshComponents();
    }
}
