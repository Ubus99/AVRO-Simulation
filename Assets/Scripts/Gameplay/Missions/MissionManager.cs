using System;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using Enumerable = System.Linq.Enumerable;
using Random = UnityEngine.Random;

namespace Gameplay.Missions
{
    public class MissionManager : IDisposable
    {
        const string MissionPath = "MissionData/Bengt Scenarios/Missions";

        readonly int _maxMissions;
        readonly List<MissionSo> _missions = new();

        CSVLogger<MissionSo.MissionRecord> _csvLogger = new(GameplayGlobals.logName);
        MissionSo _currentMission;

        public MissionManager(int maxMissions)
        {
            _maxMissions = maxMissions;

            _missions.AddRange(Resources.LoadAll<MissionSo>(MissionPath));

            GameplayGlobals.setGameMode += (_, _, _) => _csvLogger.Rename(GameplayGlobals.logName);
            MissionEvents.switchMissionEvent += OnMissionChange;
            MissionEvents.missionSubmittedEvent += OnMissionSubmitted;
            GameplayGlobals.restartEvent += OnRestart;
        }

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
            MissionEvents.missionQueueUpdateEvent?.Invoke(Enumerable.ToList(Enumerable.Cast<MissionSo>(Queue)));
            return true;
        }

        MissionSo GetRandomMission()
        {
            var mission = _missions[Random.Range(0, _missions.Count)];
            Assert.IsNotNull(mission);
            return mission;
        }

        void OnMissionSubmitted(MissionSubState missionSubState)
        {
            _currentMission.Complete(missionSubState);
            MissionsCompleted++;

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
