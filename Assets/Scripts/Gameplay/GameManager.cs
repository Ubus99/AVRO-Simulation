using System.Collections.Generic;
using car_logic;
using Scenes.Scripts.UI;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI")]
        public OverviewManager overviewManager;

        [Header("GameCameras")]
        public Camera mapCam;

        public Camera uiCamera;

        [Header("Missions")]
        public List<Mission> missions = new();

        readonly List<ADSV_AI> _activeCarList = new();

        protected void Awake()
        {
            var objs = FindObjectsByType<GameManager>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.InstanceID
            );

            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Instance.TryRegister<GameManager>(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log($"displays connected: {Display.displays.Length}");
            for (var i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }

            SetupMap();
            SetupUICamera();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void SetupMap()
        {
            mapCam.targetDisplay = (int)Screens.Map;
            foreach (var c in mapCam.GetComponentsInChildren<Camera>())
            {
                c.targetDisplay = (int)Screens.Map;
            }
        }

        void SetupUICamera()
        {
            overviewManager.GetComponentInParent<Canvas>().targetDisplay = (int)Screens.Vehicles;
            uiCamera.targetDisplay = (int)Screens.Vehicles;
        }

        public void RegisterCar(ADSV_AI carAI)
        {
            _activeCarList.Add(carAI);

            var cam = carAI.povCamera;
            cam.targetDisplay = (int)Screens.Closeup;

            overviewManager.RegisterVehicle(carAI);
        }

        enum Screens
        {
            Map = 0,
            Vehicles = 1,
            Closeup = 2,
            Controls = 3
        }
    }
}
