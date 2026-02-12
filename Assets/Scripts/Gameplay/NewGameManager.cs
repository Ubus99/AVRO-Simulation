using System;
using Scenes.Simulation.Scripts;
using UnityEngine;
using Utils;
using Utils.Objects;
using Logger = Utils.Logger;

namespace Gameplay
{
    public class NewGameManager : MonoBehaviour
    {
        [SerializeField]
        bool simulationStarted;

        [SerializeField]
        float gameSpeed = 5;

        [SerializeField]
        int maxMissions = 10;

        CSVLogger _csvLogger;

        float _lastMissionCreationTime;

        Logger _logger;

        MissionManager _missionManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _logger = Logger.instance;
            _logger.Init();

            GameplayEvents.startSimulationEvent += StartSimulationEvent;
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV Logger");
            }

            _missionManager = new MissionManager(_csvLogger, maxMissions);
            _csvLogger.RestartLogging(DateTime.Now.ToString("dd-MM-yyyy_HH-mm"));
        }

        void FixedUpdate()
        {
            UpdateMissionQueue();
        }

        void UpdateMissionQueue()
        {
            if (!simulationStarted) return;

            var timeSinceLastMissionCreation = Time.timeSinceLevelLoad - _lastMissionCreationTime;
            if (!(timeSinceLastMissionCreation > gameSpeed)) return;

            if (_missionManager.TryAddMission())
            {
                //only update if mission was indeed added
                _lastMissionCreationTime = Time.timeSinceLevelLoad;
            }
        }

        void StartSimulationEvent(int id, GameplayEvents.Input inputMethod, GameplayEvents.Severity severity)
        {
            simulationStarted = true;
            _missionManager.SetNextOrRandom();
        }
    }
}
