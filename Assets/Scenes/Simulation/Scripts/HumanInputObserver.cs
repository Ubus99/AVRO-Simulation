using System;
using Gameplay;
using Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using ZLinq;

namespace Scenes.Simulation.Scripts
{
    public class HumanInputObserver : MonoBehaviour, IDisposable
    {
        CSVLogger<HumanInputRecord> _csvLogger;

        InputAction _restartAction;

        void Start()
        {
            _restartAction = InputSystem.actions.FindAction("Global/Restart");
            _restartAction.performed += _ => { GameplayGlobals.restartEvent?.Invoke(); };

            _csvLogger = new CSVLogger<HumanInputRecord>(
            GameplayGlobals.ParticipantString,
            GameplayGlobals.GameModeString);

            GameplayGlobals.setGameMode += OnSetGameMode;
            GameplayGlobals.restartEvent += OnRestart;
        }

        void FixedUpdate()
        {
            var mousePos = Mouse.current.position.ReadValue();
            _csvLogger?.Log(new HumanInputRecord
            {
                mouseX = mousePos.x,
                mouseY = mousePos.y
            });
        }

        void OnDisable()
        {
            Dispose();
        }

        public void Dispose()
        {
            GameplayGlobals.restartEvent -= OnRestart;
            GameplayGlobals.setGameMode -= OnSetGameMode;
            _csvLogger?.Dispose();
        }

        void OnSetGameMode(int id, GameplayGlobals.Input input, GameplayGlobals.Severity severity)
        {
            _csvLogger.Rename(GameplayGlobals.ParticipantString,GameplayGlobals.GameModeString);
        }

        void OnRestart()
        {
            _csvLogger.Dispose();
            _csvLogger = new CSVLogger<HumanInputRecord>(
            GameplayGlobals.ParticipantString,
            GameplayGlobals.GameModeString);
        }

        class HumanInputRecord : BaseRecord
        {
            public float mouseX { get; set; }
            public float mouseY { get; set; }

            public string keyCodes
            {
                get
                {
                    return string.Join(",",
                    (from kc in Keyboard.current.allKeys where kc.isPressed select kc.name).ToList());
                }
            }
        }
    }
}
