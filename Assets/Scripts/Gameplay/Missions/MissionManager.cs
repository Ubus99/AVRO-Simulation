using System;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using Random = System.Random;

namespace Gameplay.Missions
{
    public class MissionManager : IDisposable
    {
        const string MissionPath = "MissionData/Missions";
        readonly List<MissionSo> _missions = new();
        readonly Random _random;

        CSVLogger<MissionSo.MissionRecord> _csvLogger = new(GameplayGlobals.ParticipantString,
        GameplayGlobals.GameModeString);

        MissionSo _currentMission;
        List<MissionSo> _shuffledMissions;

        public int MaxMissions;

        public MissionManager(int maxMissions, int? randomSeed = null)
        {
            MaxMissions = maxMissions;
            _random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

            _missions.AddRange(Resources.LoadAll<MissionSo>(MissionPath));
            ShuffleMissions(_missions);
            _shuffledMissions = new List<MissionSo>(_missions);

            GameplayGlobals.gameModeUpdatedEvent += OnSetGameMode;
            MissionEvents.switchMissionEvent += OnMissionChange;
            MissionEvents.missionSubmittedEvent += OnMissionSubmitted;
            GameplayGlobals.restartEvent += OnRestart;
        }

        public bool ExecuteMissionsOnlyOnce { get; set; }

        public int MissionsCompleted { get; private set; }

        List<MissionSo> Queue { get; } = new();

        public void Dispose()
        {
            _csvLogger.Dispose();
            GameplayGlobals.gameModeUpdatedEvent -= OnSetGameMode;
            MissionEvents.switchMissionEvent -= OnMissionChange;
            MissionEvents.missionSubmittedEvent -= OnMissionSubmitted;
            GameplayGlobals.restartEvent -= OnRestart;
        }

        void OnSetGameMode()
        {
            _csvLogger.Rename(GameplayGlobals.ParticipantString,
            GameplayGlobals.GameModeString);
        }

        void OnRestart()
        {
            _csvLogger.Dispose();
            _csvLogger =
                new CSVLogger<MissionSo.MissionRecord>(
                GameplayGlobals.ParticipantString,
                GameplayGlobals.GameModeString);

            Queue.Clear();
            ResetCompletedMissions();
        }

        void OnMissionChange(MissionSo mission)
        {
            _currentMission = Queue.Find(timedMission => mission == timedMission);
            _currentMission.Start();
        }

        public bool TryAddMission()
        {
            if (Queue.Count >= MaxMissions)
            {
                //Debug.Log("Mission is full");
                return false;
            }

            MissionSo nextMission;
            do
            {
                nextMission = GetRandomMission();
                if (!nextMission) return false;
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

        void ShuffleMissions<T>(IList<T> list)
        {
            _missions.OrderBy(so => so.name); // ensure shuffle remains deterministic
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = _random.Next(n + 1); // preinitialized random generator
                (list[k], list[n]) = (list[n], list[k]); // swap places
            }
        }

        MissionSo GetRandomMission()
        {
            if (!ExecuteMissionsOnlyOnce)
            {
                var randomMission = _shuffledMissions[_random.Next(_shuffledMissions.Count)];
                Assert.IsNotNull(randomMission);
                return randomMission;
            }

            if (_shuffledMissions.Count == 0)
            {
                Debug.LogWarning("All missions completed. Call ResetCompletedMissions() to allow repeats.");
                return null;
            }

            var mission = _shuffledMissions[0];
            _shuffledMissions.RemoveAt(0);
            Assert.IsNotNull(mission);
            return mission;
        }

        public void ResetCompletedMissions()
        {
            MissionsCompleted = 0;
            _shuffledMissions = new List<MissionSo>(_missions);
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
