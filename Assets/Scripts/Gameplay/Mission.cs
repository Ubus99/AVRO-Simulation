using System;
using System.Collections.Generic;
using car_logic;
using Scenes.Scripts.Missions;
using UI;
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
        public AlternativeRouteHelper alternativeRoutes;
        public List<ElementData> history = new();

        protected bool Active;
        public ADSV_AI carInstance { protected set; get; }
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
            foreach (var sc in alternativeRoutes.routes)
                sc.gameObject.SetActive(true);
            OnActivated?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        void SpawnCar(bool startErrored)
        {
            carInstance = Instantiate(carPrefab, startPoint.position + Vector3.up, startPoint.rotation);
            carInstance.currentMission = this;
            if (startErrored)
                carInstance.state = States.ErrorDetected;
        }

        protected abstract void Setup();

        public void Deactivate()
        {
            CleanUp();
            if (carInstance) Destroy(carInstance.gameObject);
            foreach (var sc in alternativeRoutes.routes)
                sc.gameObject.SetActive(false);
            OnDeactivated?.Invoke(this, EventArgs.Empty);
            Active = false;
        }

        protected abstract void CleanUp();

        public void SelectRoute(AlternativeRoute route)
        {
            alternativeRoutes.SelectRoute(route);
        }
    }
}
