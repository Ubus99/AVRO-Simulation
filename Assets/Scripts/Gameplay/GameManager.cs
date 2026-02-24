using System;
using Gameplay.Missions;
using InputHelpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Logging.Logger;

namespace Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        GameplayGlobals.Scenes mode;

        [SerializeField]
        DifficultySo difficultyEasy;

        [SerializeField]
        DifficultySo difficultyHard;

        DifficultySo _currentDifficulty;

        float _lastMissionCreationTime;

        Logger _logger;

        MissionManager _missionManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _logger = Logger.instance;
            _logger.Init(GameplayGlobals.logName);

            _currentDifficulty = difficultyEasy;

            GameplayGlobals.setGameMode += SetGameMode;
            GameplayGlobals.switchSceneEvent += SwitchScene;
            GameplayGlobals.restartEvent += OnGameRestart;

            _missionManager = new MissionManager(_currentDifficulty.maxMissions, 0);
            _missionManager.ExecuteMissionsOnlyOnce = true;
        }

        void FixedUpdate()
        {
            if (_missionManager.MissionsCompleted >= _currentDifficulty.missionsToComplete)
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
                    _lastMissionCreationTime = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
            }
        }

        void UpdateMissionQueue()
        {
            if (mode != GameplayGlobals.Scenes.Simulation) return;

            var timeSinceLastMissionCreation = Time.timeSinceLevelLoad - _lastMissionCreationTime;
            if (!(timeSinceLastMissionCreation > _currentDifficulty.gameSpeed)) return;

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

            _currentDifficulty = severity switch
            {
                GameplayGlobals.Severity.Easy => difficultyEasy,
                GameplayGlobals.Severity.Hard => difficultyHard,
                _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
            };
            _missionManager.MaxMissions = _currentDifficulty.maxMissions;

            _logger.RenameLog(GameplayGlobals.logName);
        }
    }
}
