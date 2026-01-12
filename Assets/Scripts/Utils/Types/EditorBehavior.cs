using UnityEngine;

namespace Utils.Types
{
    public abstract class EditorBehavior : MonoBehaviour
    {
        protected bool Dirty;

        void Update()
        {
            if (Dirty)
            {
                HandleIsDirty();
                Dirty = false;
            }
        }

        void OnValidate()
        {
            Dirty = true;
        }

        protected abstract void HandleIsDirty();
        protected abstract void RefreshComponents();
    }
}
