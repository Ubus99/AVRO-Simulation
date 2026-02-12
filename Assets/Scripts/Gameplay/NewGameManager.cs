using System;
using Scenes.Simulation.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Utils.Objects;
using Logger = Utils.Logger;

namespace Gameplay
{
    public class NewGameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        GameplayGlobals.Scenes mode;

        [SerializeField]
        float gameSpeedEasy = 5;

        [SerializeField]
        float gameSpeedHard = 1;

        [SerializeField]
        int maxMissions = 10;

        CSVLogger _csvLogger;

        float _currentGameSpeed;

        float _lastMissionCreationTime;

        Logger _logger;

        MissionManager _missionManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _logger = Logger.instance;
            _logger.Init();

            _currentGameSpeed = gameSpeedEasy;
            
            GameplayGlobals.setGameMode += SetGameMode;
            GameplayGlobals.switchSceneEvent += SwitchScene;
        }

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new Exception("Could not find CSV Logger");
            }

            _missionManager = new MissionManager(_csvLogger, maxMissions);
            _csvLogger.RestartLogging(DateTime.Now.ToString("dd-MM-yyyy_HH-mm"));
        }

        void FixedUpdate()
        {
            UpdateMissionQueue();
        }

        void SwitchScene(GameplayGlobals.Scenes scene)
        {
            mode = scene;
            switch (scene)
            {
                case GameplayGlobals.Scenes.Login:
                    SceneManager.LoadScene("Login");
                    break;
                case GameplayGlobals.Scenes.Simulation:
                    SceneManager.LoadScene("Simulation");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
            }
        }

        void UpdateMissionQueue()
        {
            if (mode != GameplayGlobals.Scenes.Simulation) return;

            var timeSinceLastMissionCreation = Time.timeSinceLevelLoad - _lastMissionCreationTime;
            if (!(timeSinceLastMissionCreation > _currentGameSpeed)) return;

            if (_missionManager.TryAddMission())
            {
                //only update if mission was indeed added
                _lastMissionCreationTime = Time.timeSinceLevelLoad;
            }
        }

        void SetGameMode(int id, GameplayGlobals.Input inputMethod, GameplayGlobals.Severity severity)
        {
            GameplayGlobals.currentInput = inputMethod;
            GameplayGlobals.currentSeverity = severity;

            _currentGameSpeed = severity switch
            {
                GameplayGlobals.Severity.Easy => gameSpeedEasy,
                GameplayGlobals.Severity.Hard => gameSpeedHard,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
            _missionManager.SetNextOrRandom();
        }
    }
}
