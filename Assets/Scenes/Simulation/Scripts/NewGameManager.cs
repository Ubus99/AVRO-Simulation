using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Objects;
using Logger = Utils.Logger;
using Random = UnityEngine.Random;

namespace Scenes.Simulation.Scripts
{
    public class NewGameManager : MonoBehaviour
    {
        const string MissionNameKey = "missionName";
        const string MissionStateKey = "lastMissionState";
        const string MissionEventKey = "missionEvent";

        readonly List<MissionSo> _missions = new();

        readonly Queue<MissionSo> _missionsQueue = new();
        CSVLogger _csvLogger;
        MissionSo _currentMission;

        Logger _logger;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _logger = Logger.instance;
            _logger.Init();

            _missions.AddRange(Resources.LoadAll<MissionSo>("MissionData/Bengt Scenarios/Missions"));

            GameplayEvents.missionCompletedEvent += OnMissionCompleted;
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV Logger");
            }
            _csvLogger.RegistrationEvent += RegisterMessages;
            _csvLogger.RestartLogging(DateTime.Now.ToString("dd-MM-yyyy_HH-mm"));

            NextMission();
        }

        void RegisterMessages()
        {
            _csvLogger.TryRegister(MissionNameKey);
            _csvLogger.TryRegister(MissionStateKey);
            _csvLogger.TryRegister(MissionEventKey);
        }

        MissionSo GetRandomMission()
        {
            return _missions[Random.Range(0, _missions.Count)];
        }

        void OnMissionCompleted(MissionSubState missionSubState)
        {
            _csvLogger.TryLog(MissionStateKey, _currentMission.Complete(missionSubState) ? "success" : "failed");

            NextMission();
        }

        void NextMission()
        {
            _currentMission = _missionsQueue.Count == 0 ? GetRandomMission() : _missionsQueue.Dequeue();
            GameplayEvents.changeMissionEvent?.Invoke(_currentMission);
            _csvLogger.TryLog(MissionNameKey, _currentMission.name);
            _csvLogger.TryLog(MissionEventKey, "nextMission");
        }
    }
}
