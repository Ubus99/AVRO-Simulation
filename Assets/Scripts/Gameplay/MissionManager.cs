using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Logging;
using Assert = UnityEngine.Assertions.Assert;
using Enumerable = System.Linq.Enumerable;
using Random = UnityEngine.Random;

namespace Gameplay
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
            GameplayGlobals.switchMissionEvent += OnMissionChange;
            GameplayGlobals.missionSubmittedEvent += OnMissionSubmitted;
            GameplayGlobals.restartEvent += OnRestart;
        }

        public int missionsCompleted { get; set; }

        List<MissionSo> queue { get; } = new();

        public void Dispose()
        {
            _csvLogger.Dispose();
            GameplayGlobals.switchMissionEvent -= OnMissionChange;
            GameplayGlobals.missionSubmittedEvent -= OnMissionSubmitted;
            GameplayGlobals.restartEvent -= OnRestart;
        }

        void OnRestart()
        {
            _csvLogger.Dispose();
            _csvLogger = new CSVLogger<MissionSo.MissionRecord>(GameplayGlobals.logName);
        }

        void OnMissionChange(MissionSo mission)
        {
            _currentMission = queue.Find(timedMission => mission == timedMission);
            _currentMission.Start();
        }

        public bool TryAddMission()
        {
            if (queue.Count >= _maxMissions)
            {
                Debug.Log("Mission is full");
                return false;
            }

            MissionSo nextMission;
            do
            {
                nextMission = GetRandomMission();
            } while (queue.Contains(nextMission));

            nextMission.Load();
            queue.Add(nextMission);

            Debug.Log($"Mission added: {nextMission.name}");
            GameplayGlobals.missionQueueUpdateEvent?.Invoke(Enumerable.ToList(Enumerable.Cast<MissionSo>(queue)));
            return true;
        }

        public bool TryRemoveMission(MissionSo mission)
        {
            if (queue.Count <= 0) return false;
            if (!queue.Contains(mission)) return false;

            queue.Remove(mission);
            GameplayGlobals.missionQueueUpdateEvent?.Invoke(Enumerable.ToList(Enumerable.Cast<MissionSo>(queue)));
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
            missionsCompleted++;
            
            Debug.Log(
            $"mission {_currentMission.name} submitted. " +
            $"tts: {_currentMission.record.timeToStart}, " +
            $"ttc: {_currentMission.record.timeToComplete}, " +
            $"tt: {_currentMission.record.totalTime}, " +
            $"correct: {missionSubState.isCorrect}"
            );

            _csvLogger.Log(_currentMission.record);

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
        }
    }
}
