using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using Random = UnityEngine.Random;

namespace Gameplay.Missions
{
    public class MissionManager : IDisposable
    {
        const string MissionPath = "MissionData/Bengt Scenarios/Missions";

        readonly int _maxMissions;
        readonly List<MissionSo> _missions = new();
        List<MissionSo> _availableMissions;

        CSVLogger<MissionSo.MissionRecord> _csvLogger = new(GameplayGlobals.logName);
        MissionSo _currentMission;

        public MissionManager(int maxMissions, int? randomSeed = null)
        {
            _maxMissions = maxMissions;

            if (randomSeed.HasValue)
            {
                Random.InitState(randomSeed.Value);
            }

            _missions.AddRange(Resources.LoadAll<MissionSo>(MissionPath));
            _availableMissions = new List<MissionSo>(_missions);

            GameplayGlobals.setGameMode += (_, _, _) => _csvLogger.Rename(GameplayGlobals.logName);
            MissionEvents.switchMissionEvent += OnMissionChange;
            MissionEvents.missionSubmittedEvent += OnMissionSubmitted;
            GameplayGlobals.restartEvent += OnRestart;
        }

        public bool ExecuteMissionsOnlyOnce { get; set; }

        public int MissionsCompleted { get; set; }

        List<MissionSo> Queue { get; } = new();

        public void Dispose()
        {
            _csvLogger.Dispose();
            MissionEvents.switchMissionEvent -= OnMissionChange;
            MissionEvents.missionSubmittedEvent -= OnMissionSubmitted;
            GameplayGlobals.restartEvent -= OnRestart;
        }

        void OnRestart()
        {
            _csvLogger.Dispose();
            _csvLogger = new CSVLogger<MissionSo.MissionRecord>(GameplayGlobals.logName);
        }

        void OnMissionChange(MissionSo mission)
        {
            _currentMission = Queue.Find(timedMission => mission == timedMission);
            _currentMission.Start();
        }

        public bool TryAddMission()
        {
            if (Queue.Count >= _maxMissions)
            {
                //Debug.Log("Mission is full");
                return false;
            }

            MissionSo nextMission;
            do
            {
                nextMission = GetRandomMission();
                if (nextMission == null) return false;
            } while (Queue.Contains(nextMission));

            nextMission.Load();
            Queue.Add(nextMission);

            Debug.Log($"Mission added: {nextMission.name}");

            MissionEvents.missionQueuedEvent?.Invoke(nextMission);
            MissionEvents.missionQueueUpdateEvent?.Invoke(Queue);
            return true;
        }

        public bool TryRemoveMission(MissionSo mission)
        {
            if (Queue.Count <= 0) return false;
            if (!Queue.Contains(mission)) return false;

            Queue.Remove(mission);
            MissionEvents.missionQueueUpdateEvent?.Invoke(Queue.ToList());
            return true;
        }

        MissionSo GetRandomMission()
        {
            if (ExecuteMissionsOnlyOnce)
            {
                if (_availableMissions.Count == 0)
                {
                    Debug.LogWarning("All missions completed. Call ResetCompletedMissions() to allow repeats.");
                    return null;
                }
                var mission = _availableMissions[Random.Range(0, _availableMissions.Count)];
                Assert.IsNotNull(mission);
                return mission;
            }

            var randomMission = _missions[Random.Range(0, _missions.Count)];
            Assert.IsNotNull(randomMission);
            return randomMission;
        }

        public void ResetCompletedMissions()
        {
            MissionsCompleted = 0;
            _availableMissions = new List<MissionSo>(_missions);
        }

        void OnMissionSubmitted(MissionSubState missionSubState)
        {
            _currentMission.Complete(missionSubState);
            MissionsCompleted++;

            if (ExecuteMissionsOnlyOnce)
            {
                _availableMissions.Remove(_currentMission);
            }

            Debug.Log(
            $"mission {_currentMission.name} submitted. " +
            $"tts: {_currentMission.record.timeToStart}, " +
            $"ttc: {_currentMission.record.timeToComplete}, " +
            $"tt: {_currentMission.record.totalTime}, " +
            $"correct: {missionSubState.isCorrect}, " +
            $"completed: {MissionsCompleted}"
            );

            _currentMission.record.numberCompleted = MissionsCompleted;
            _csvLogger.Log(_currentMission.record);

            TryRemoveMission(_currentMission);

            MissionEvents.missionCompletedEvent?.Invoke(_currentMission);
            _currentMission = null;
        }

        public void SetNextOrRandom()
        {
            if (Queue.Count == 0)
                TryAddMission();

            _currentMission = Queue[0];

            MissionEvents.switchMissionEvent?.Invoke(_currentMission);
        }
    }
}
