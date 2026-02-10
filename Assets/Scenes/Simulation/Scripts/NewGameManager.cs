using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Objects;
using Random = UnityEngine.Random;

namespace Scenes.Simulation.Scripts
{
    public class NewGameManager : MonoBehaviour
    {
        readonly List<MissionSo> _missions = new();

        readonly Queue<MissionSo> _missionsQueue = new();
        CSVLogger _csvLogger;
        MissionSo _currentMission;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _missions.AddRange(Resources.LoadAll<MissionSo>("MissionData/Bengt Scenarios/Missions"));
            GameplayEvents.missionCompletedEvent += OnMissionCompleted;
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV Logger");
            }
            _csvLogger.RegistrationEvent += () => _csvLogger.TryRegister("mission");
            _csvLogger.RestartLogging("test");

            NextMission();
        }

        MissionSo GetRandomMission()
        {
            return _missions[Random.Range(0, _missions.Count)];
        }

        void OnMissionCompleted(MissionSo.MissionSubState missionSubState)
        {
            _currentMission.Complete(missionSubState);

            NextMission();
        }

        void NextMission()
        {
            _currentMission = _missionsQueue.Count == 0 ? GetRandomMission() : _missionsQueue.Dequeue();
            GameplayEvents.changeMissionEvent?.Invoke(_currentMission);
            _csvLogger.TryLog("mission", _currentMission.name);
        }
    }
}
