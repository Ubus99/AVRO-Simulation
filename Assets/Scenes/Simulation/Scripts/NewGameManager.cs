using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenes.Simulation.Scripts
{
    public class NewGameManager : MonoBehaviour
    {
        readonly List<MissionSo> _missions = new();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _missions.AddRange(Resources.LoadAll<MissionSo>("MissionData/Bengt Scenarios/Missions"));
        }

        void Start()
        {
            GameplayEvents.changeMissionEvent.Invoke(GetRandomMission());
        }

        MissionSo GetRandomMission()
        {
            return _missions[Random.Range(0, _missions.Count)];
        }
    }
}
