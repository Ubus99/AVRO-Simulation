using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Scenes.Simulation.Scripts
{
    public class HumanInputObserver : MonoBehaviour
    {
        CSVLogger<HumanInputRecord> _csvLogger;

        void Start()
        {
            _csvLogger = new CSVLogger<HumanInputRecord>(GameplayGlobals.logName);
        }

        // Update is called once per frame
        void Update()
        {
            _csvLogger.Log(new HumanInputRecord
            {
                MousePosition = Pointer.current.position.ReadValue()
            });
        }

        void OnDisable()
        {
            _csvLogger.Dispose();
        }

        class HumanInputRecord : BaseRecord
        {
            public Vector2 MousePosition;
        }
    }
}
