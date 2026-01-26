using System.Collections.Generic;
using car_logic;
using Gameplay;
using Scenes.Prefabs.UIComponents;
using UnityEngine;
using Utils.Objects;
using ZLinq;
using Logger = Utils.Logger;

[assembly: ZLinqDropIn(null, DropInGenerateTypes.Collection)]

namespace Scenes.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("UI")]
        public ScreenManager screenManager;

        [Header("GameCameras")]
        public Camera mapCam;

        public Camera uiCamera;

        [Header("Missions")]
        public List<MissionController> missionControllers = new();

        public int concurrentMissions = 1;

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

            if (Application.isPlaying)
            {
                Logger.instance.Init();
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.instance.TryRegister<GameManager>(this);
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
            if (missionControllers.Count > 0)
            {
                var activeMissions = missionControllers.Count(controller => controller.inProgress);
                for (var i = 0; i < concurrentMissions - activeMissions; i++)
                {
                    missionControllers.FirstOrDefault(controller => !controller.inProgress, null)?.TryActivateMission();
                }
            }
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
            screenManager.GetComponentInParent<Canvas>().targetDisplay = (int)Screens.Vehicles;
            uiCamera.targetDisplay = (int)Screens.Vehicles;
        }

        public void RegisterCar(ADSV_AI carAI)
        {
            var cam = carAI.povCamera;
            cam.targetDisplay = (int)Screens.Vehicles;

            screenManager.overviewManager.RegisterVehicle(carAI);
        }

        public void DeregisterCar(ADSV_AI carAI)
        {
            screenManager.overviewManager.DeregisterVehicle(carAI);
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
