using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class DebugCharacterController : MonoBehaviour, DefaultInputActions.IPlayerActions
    {
        public float speed = 1;
        public Vector2 lookSpeed = Vector2.one;
        Camera _camera;
        CharacterController _characterController;
        Vector2 _mouseDelta;
        Vector3 _movementDirection;
        PlayerInput _playerInput;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _camera = GetComponentInChildren<Camera>();
        }

        void Update()
        {
            transform.Rotate(Vector3.up, _mouseDelta.x * lookSpeed.x * Time.deltaTime);
            _camera.transform.Rotate(Vector3.right, _mouseDelta.y * lookSpeed.y * Time.deltaTime);

            var globDir = transform.TransformDirection(_movementDirection);
            _characterController.SimpleMove(globDir * (speed * Time.deltaTime));
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var inp = context.ReadValue<Vector2>();
            _movementDirection = new Vector3(inp.x, 0, inp.y);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _mouseDelta = context.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}
