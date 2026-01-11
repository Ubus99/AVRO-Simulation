using System;
using System.Collections.Generic;
using car_logic;
using Scenes.Scripts.Missions;
using Scenes.Scripts.UI;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public abstract class Mission : MonoBehaviour
    {
        [Header("Prefabs")]
        public ADSV_AI carPrefab;

        [Header("Key Points")]
        public Transform startPoint;

        public Transform endPoint;
        public List<AlternativeRoute> alternativeRoutes = new();
        public List<ListItem.ElementData> history = new();

        protected bool Active;
        protected ADSV_AI CarInstance;
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
            SpawnCar(true);
            Setup();
            foreach (var sc in alternativeRoutes)
                sc.gameObject.SetActive(true);
            OnActivated?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        void SpawnCar(bool startErrored)
        {
            CarInstance = Instantiate(carPrefab, startPoint.position + Vector3.up, startPoint.rotation);
            CarInstance.currentMission = this;
            if (startErrored)
                CarInstance.state = States.ErrorDetected;
        }

        protected abstract void Setup();

        public void Deactivate()
        {
            CleanUp();
            if (CarInstance) Destroy(CarInstance.gameObject);
            foreach (var sc in alternativeRoutes)
                sc.gameObject.SetActive(false);
            OnDeactivated?.Invoke(this, EventArgs.Empty);
            Active = false;
        }

        protected abstract void CleanUp();
    }
}
