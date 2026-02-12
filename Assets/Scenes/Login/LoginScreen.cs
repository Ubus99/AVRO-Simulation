using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Enumerable = System.Linq.Enumerable;

namespace Scenes.Login
{
    public class LoginScreen : MonoBehaviour
    {
        IntegerField _idField;

        GameplayGlobals.Input[] _inputOptions;
        Button _loginButton;
        DropdownField _modeSelection;
        GameplayGlobals.Severity[] _severityOptions;

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            var root = uiDocument.rootVisualElement;

            _idField = root.Q<IntegerField>("IDField");

            _loginButton = root.Q<Button>("ConfirmButton");
            _loginButton.SetEnabled(false);
            _loginButton.clicked += SubmitForm;

            _modeSelection = root.Q<DropdownField>("ScenarioDropdown");
            _inputOptions = Enumerable.ToArray(EnumerateEnumValues<GameplayGlobals.Input>());
            _severityOptions = Enumerable.ToArray(EnumerateEnumValues<GameplayGlobals.Severity>());

            foreach (var input in _inputOptions)
            foreach (var severity in _severityOptions)
            {
                _modeSelection.choices.Add($"variant {_modeSelection.choices.Count.ToString()}");
            }

            _modeSelection.RegisterValueChangedCallback(evt => { _loginButton.SetEnabled(true); });
        }

        void OnDisable()
        {
            if (_loginButton != null) _loginButton!.clicked -= SubmitForm;
        }

        void SubmitForm()
        {
            var index = _modeSelection.index;
            var input = _inputOptions[index / _severityOptions.Length];
            var severity = _severityOptions[index % _severityOptions.Length];
            GameplayGlobals.setGameMode?.Invoke(_idField.value, input, severity);
            GameplayGlobals.switchSceneEvent?.Invoke(GameplayGlobals.Scenes.Simulation);
        }

        static IEnumerable<T> EnumerateEnumValues<T>()
        {
            return Enumerable.Cast<T>(Enum.GetValues(typeof(T)));
        }
    }
}
