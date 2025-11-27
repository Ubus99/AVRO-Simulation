using System;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public abstract class Mission : MonoBehaviour
    {
        protected bool Active;
        public bool Completed { get; protected set; }

        void Awake()
        {
            OnLoad();
        }

        public event EventHandler<Mission> OnCompleted;
        public event EventHandler OnActivated;
        public event EventHandler OnDeactivated;

        protected void OnLoad()
        {
            Deactivate();
        }

        public void Activate()
        {
            Setup();
            OnActivated?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        protected abstract void Setup();

        public void Deactivate()
        {
            CleanUp();
            OnDeactivated?.Invoke(this, EventArgs.Empty);
            Active = false;
        }

        protected abstract void CleanUp();
    }
}
