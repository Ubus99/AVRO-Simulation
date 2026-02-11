using System;
using UnityEngine;
using Utils;
using Utils.Objects;
using Logger = Utils.Logger;
using Random = UnityEngine.Random;

namespace Scenes.Simulation.Scripts
{
    public class NewGameManager : MonoBehaviour
    {

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
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV Logger");
            }
            
            _missionManager = new MissionManager(_csvLogger, maxMissions);
            _csvLogger.RestartLogging(DateTime.Now.ToString("dd-MM-yyyy_HH-mm"));

            _missionManager.SetNextOrRandom();
        }

        void FixedUpdate()
        {
            if (!(Time.timeSinceLevelLoad - _lastMissionCreationTime > gameSpeed) ||
                _missionManager.queue.Count >= maxMissions)
                return;

            _lastMissionCreationTime = Time.timeSinceLevelLoad;
            _missionManager.TryAddMission();
        }
    }
}
