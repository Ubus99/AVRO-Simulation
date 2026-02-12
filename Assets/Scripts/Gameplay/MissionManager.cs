using System;
using System.Collections.Generic;
using Scenes.Simulation.Scripts;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class MissionManager : IDisposable
    {
        const string MissionStateKey = "lastMissionState";
        const string MissionNameKey = "missionName";
        const string MissionEventKey = "missionEvent";

        readonly CSVLogger _csvLogger;

        readonly int _maxMissions;
        readonly List<MissionSo> _missions = new();
        MissionSo _currentMission;

        public MissionManager(CSVLogger csvLogger, int maxMissions)
        {
            _maxMissions = maxMissions;

            _missions.AddRange(Resources.LoadAll<MissionSo>("MissionData/Bengt Scenarios/Missions"));

            _csvLogger = csvLogger;
            _csvLogger.RegistrationEvent += RegisterMessages;

            GameplayGlobals.switchMissionEvent += OnMissionChange;
            GameplayGlobals.missionSubmittedEvent += OnMissionSubmitted;
        }

        public List<MissionSo> queue { get; } = new();

        public void Dispose()
        {
            GameplayGlobals.switchMissionEvent -= OnMissionChange;
            GameplayGlobals.missionSubmittedEvent -= OnMissionSubmitted;
        }

        void RegisterMessages()
        {
            _csvLogger.TryRegister(MissionNameKey);
            _csvLogger.TryRegister(MissionStateKey);
            _csvLogger.TryRegister(MissionEventKey);
        }

        void OnMissionChange(MissionSo mission)
        {
            _currentMission = mission;
        }

        public bool TryAddMission()
        {
            if (queue.Count >= _maxMissions) return false;

            queue.Add(GetRandomMission());
            GameplayGlobals.missionQueueUpdateEvent?.Invoke(queue);
            return true;
        }

        public bool TryRemoveMission(MissionSo mission)
        {
            if (queue.Count <= 0) return false;
            if (!queue.Contains(mission)) return false;

            queue.Remove(mission);
            GameplayGlobals.missionQueueUpdateEvent?.Invoke(queue);
            return true;
        }

        MissionSo GetRandomMission()
        {
            return _missions[Random.Range(0, _missions.Count)];
        }

        void OnMissionSubmitted(MissionSubState missionSubState)
        {
            _csvLogger.TryLog(MissionStateKey, _currentMission.Complete(missionSubState) ? "success" : "failed");

            TryRemoveMission(_currentMission);
            _currentMission = null;
            GameplayGlobals.missionCompletedEvent?.Invoke();
        }

        public void SetNextOrRandom()
        {
            if (queue.Count == 0)
                TryAddMission();

            _currentMission = queue[0];

            GameplayGlobals.switchMissionEvent?.Invoke(_currentMission);
            _csvLogger.TryLog(MissionNameKey, _currentMission.name);
            _csvLogger.TryLog(MissionEventKey, "nextMission");
        }
    }
}
