using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using Utils.Objects;

namespace Scenes.Simulation.Scripts
{
    public class HumanInputObserver : MonoBehaviour
    {
        const string PointerPositionKey = "PointerPosition";
        CSVLogger _csvLogger;

        void Start()
        {
            if (!ServiceLocator.instance.TryGet(out _csvLogger))
            {
                throw new NullReferenceException("Could not find service locator");
            }
            _csvLogger.RegistrationEvent += () => { _csvLogger.TryRegister(PointerPositionKey); };
        }

        // Update is called once per frame
        void Update()
        {
            _csvLogger.TryLog(PointerPositionKey, Pointer.current.position.ReadValue().ToString());
        }
    }
}
