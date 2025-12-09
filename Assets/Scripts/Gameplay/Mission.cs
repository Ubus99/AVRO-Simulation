using System;
using car_logic;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public abstract class Mission : MonoBehaviour
    {
        public ADSV_AI car;
        protected bool Active;
        public bool completed { get; protected set; }

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
        
        public void SetCar(ADSV_AI car)
        {
            this.car = car;
        }
    }
}
