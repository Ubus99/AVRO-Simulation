using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Logger = Utils.Logging.Logger;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
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


        float _currentGameSpeed;

        float _lastMissionCreationTime;

        Logger _logger;

        MissionManager _missionManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _logger = Logger.instance;
            _logger.Init();

            _currentGameSpeed = gameSpeedEasy;


            GameplayGlobals.setGameMode += SetGameMode;
            GameplayGlobals.switchSceneEvent += SwitchScene;
            GameplayGlobals.restartEvent += OnGameRestart;

            _missionManager = new MissionManager(maxMissions);
        }

        void FixedUpdate()
        {
            UpdateMissionQueue();
        }

        void OnDisable()
        {
            _missionManager.Dispose();
        }

        void OnGameRestart()
        {
            GameplayGlobals.currentID = 0;
            _logger.Init();
            GameplayGlobals.switchSceneEvent?.Invoke(GameplayGlobals.Scenes.Login);
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
            GameplayGlobals.currentID = id;

            _currentGameSpeed = severity switch
            {
                GameplayGlobals.Severity.Easy => gameSpeedEasy,
                GameplayGlobals.Severity.Hard => gameSpeedHard,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };

            _logger.RenameLog(GameplayGlobals.logName);
        }
    }
}
