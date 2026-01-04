using System;
using car_logic;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public abstract class Mission : MonoBehaviour
    {
        public Transform startPoint;
        public ADSV_AI carPrefab;

        protected bool Active;
        protected ADSV_AI CarInstance;
        public bool completed { get; protected set; }

        private void Awake()
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
            SpawnCar(true);
            Setup();
            OnActivated?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        private void SpawnCar(bool startErrored)
        {
            CarInstance = Instantiate(carPrefab, startPoint.position + Vector3.up, startPoint.rotation);
            if (startErrored)
                CarInstance.state = States.ErrorDetected;
        }

        protected abstract void Setup();

        public void Deactivate()
        {
            CleanUp();
            if (CarInstance) Destroy(CarInstance.gameObject);
            OnDeactivated?.Invoke(this, EventArgs.Empty);
            Active = false;
        }

        protected abstract void CleanUp();
    }
}