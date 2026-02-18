using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
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

        [SerializeField]
        int missionsToComplete = 20;

        float _currentGameSpeed;

        float _lastMissionCreationTime;

        Logger _logger;

        MissionManager _missionManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _logger = Logger.instance;
            _logger.Init(GameplayGlobals.logName);

            _currentGameSpeed = gameSpeedEasy;

            GameplayGlobals.setGameMode += SetGameMode;
            GameplayGlobals.switchSceneEvent += SwitchScene;
            GameplayGlobals.restartEvent += OnGameRestart;

            _missionManager = new MissionManager(maxMissions);
        }

        void FixedUpdate()
        {
            if (_missionManager.missionsCompleted >= missionsToComplete)
            {
                GameplayGlobals.restartEvent?.Invoke();
            }
            else
            {
                UpdateMissionQueue();
            }
        }

        void OnDisable()
        {
            _logger.Dispose();
            _missionManager.Dispose();
        }

        void OnGameRestart()
        {
            GameplayGlobals.currentID = 0;
            _logger.Dispose();
            _logger.Init(GameplayGlobals.logName);
            GameplayGlobals.switchSceneEvent?.Invoke(GameplayGlobals.Scenes.Login);
        }

        void SwitchScene(GameplayGlobals.Scenes scene)
        {
            mode = scene;
            switch (scene)
            {
                case GameplayGlobals.Scenes.Login:
                    SceneManager.LoadScene("Login");
                    InputUtils.SwitchToScheme("Keyboard&Mouse");
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

            switch (inputMethod)
            {
                case GameplayGlobals.Input.Mouse:
                    InputUtils.SwitchToScheme("Keyboard&Mouse");
                    break;
                case GameplayGlobals.Input.Touch:
                    InputUtils.SwitchToScheme("Touch");
                    break;
                case GameplayGlobals.Input.Speech:
                    InputUtils.SwitchToScheme("Keyboard");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputMethod), inputMethod, null);
            }

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
