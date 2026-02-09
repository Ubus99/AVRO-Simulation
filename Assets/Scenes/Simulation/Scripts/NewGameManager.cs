using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenes.Simulation.Scripts
{
    public class NewGameManager : MonoBehaviour
    {
        readonly List<MissionSo> _missions = new();

        readonly Queue<MissionSo> _missionsQueue = new();
        MissionSo _currentMission;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _missions.AddRange(Resources.LoadAll<MissionSo>("MissionData/Bengt Scenarios/Missions"));
            GameplayEvents.missionCompletedEvent += OnMissionCompleted;
        }

        void Start()
        {
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
            GameplayEvents.changeMissionEvent.Invoke(_currentMission);
        }
    }
}
