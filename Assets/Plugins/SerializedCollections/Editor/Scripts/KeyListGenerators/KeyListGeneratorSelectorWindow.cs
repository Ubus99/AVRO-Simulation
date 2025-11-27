using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    public class KeyListGeneratorSelectorWindow : EditorWindow
    {
        [SerializeField]
        int _selectedIndex;

        [SerializeField]
        ModificationType _modificationType;

        readonly Dictionary<Type, KeyListGenerator> _keysGenerators = new();
        string _detailsText;
        UnityEditor.Editor _editor;

        KeyListGenerator _generator;
        List<KeyListGeneratorData> _generatorsData;
        Type _targetType;
        int _undoStart;

        void OnEnable()
        {
            var document = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/Plugins/SerializedCollections/Editor/Assets/KeysGeneratorSelectorWindow.uxml");
            var element = document.CloneTree();
            element.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            rootVisualElement.Add(element);
        }

        void OnDestroy()
        {
            Undo.undoRedoPerformed -= HandleUndoCallback;
            Undo.RevertAllDownToGroup(_undoStart);
            foreach (var keyGenerator in _keysGenerators)
                DestroyImmediate(keyGenerator.Value);
        }

        public event Action<KeyListGenerator, ModificationType> OnApply;

        public void Initialize(IEnumerable<KeyListGeneratorData> generatorsData, Type type)
        {
            _targetType = type;
            _selectedIndex = 0;
            _modificationType = ModificationType.Add;
            _undoStart = Undo.GetCurrentGroup();
            _generatorsData = new List<KeyListGeneratorData>(generatorsData);
            SetGeneratorIndex(0);
            Undo.undoRedoPerformed += HandleUndoCallback;

            rootVisualElement.Q<Button>(className: "sc-close-button").clicked += Close;

            rootVisualElement.Q<RadioButton>(name = "add-modification").userData = ModificationType.Add;
            rootVisualElement.Q<RadioButton>(name = "remove-modification").userData = ModificationType.Remove;
            rootVisualElement.Q<RadioButton>(name = "confine-modification").userData = ModificationType.Confine;

            var modificationToggles = rootVisualElement.Query<RadioButton>(className: "sc-modification-toggle");
            modificationToggles.ForEach(InitializeModificationToggle);

            rootVisualElement.Q<IMGUIContainer>(name = "imgui-inspector").onGUIHandler = EditorGUIHandler;
            rootVisualElement.Q<Button>(name = "apply-button").clicked += ApplyButtonClicked;

            var generatorsContent = rootVisualElement.Q<ScrollView>(name = "generators-content");
            var radioButtonGroup = new RadioButtonGroup();
            radioButtonGroup.name = "generators-group";
            radioButtonGroup.AddToClassList("sc-radio-button-group");
            generatorsContent.Add(radioButtonGroup);

            for (var i = 0; i < _generatorsData.Count; i++)
            {
                var generatorData = _generatorsData[i];

                var radioButton = new RadioButton(generatorData.Name);
                radioButton.value = i == 0;
                radioButton.AddToClassList("sc-text-toggle");
                radioButton.AddToClassList("sc-generator-toggle");
                radioButton.userData = i;
                radioButton.RegisterValueChangedCallback(OnGeneratorClicked);
                radioButtonGroup.Add(radioButton);
            }
        }

        void ApplyButtonClicked()
        {
            OnApply?.Invoke(_editor.target as KeyListGenerator, _modificationType);
            OnApply = null;
            Close();
        }

        void EditorGUIHandler()
        {
            EditorGUI.BeginChangeCheck();
            _editor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                UpdateDetailsText();
            }
        }

        void InitializeModificationToggle(RadioButton obj)
        {
            if ((ModificationType)obj.userData == _modificationType)
                obj.value = true;
            obj.RegisterValueChangedCallback(OnModificationToggleClicked);
        }

        void OnModificationToggleClicked(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
                return;

            var modificationType = (ModificationType)((VisualElement)evt.target).userData;
            _modificationType = modificationType;
        }

        void UpdateDetailsText()
        {
            var enumerable = _generator.GetKeys(_targetType);
            var count = 0;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                count++;
                if (count > 100)
                {
                    _detailsText = "over 100 Elements";
                    return;
                }
            }
            _detailsText = $"{count} Elements";

            rootVisualElement.Q<Label>(name = "generated-count-label").text = _detailsText;
        }

        void OnGeneratorClicked(ChangeEvent<bool> evt)
        {
            if (!evt.newValue)
                return;

            SetGeneratorIndex((int)(evt.target as VisualElement).userData);
        }

        void HandleUndoCallback()
        {
            UpdateGeneratorAndEditorIfNeeded();
            Repaint();
        }

        void SetGeneratorIndex(int index)
        {
            Undo.RecordObject(this, "Change Window");
            _selectedIndex = index;
            UpdateGeneratorAndEditorIfNeeded();
        }

        void UpdateGeneratorAndEditorIfNeeded()
        {
            var targetType = _generatorsData[_selectedIndex].GeneratorType;
            if (_generator != null && _generator.GetType() == targetType)
                return;

            _generator = GetOrCreateKeysGenerator(targetType);
            if (_editor != null)
                DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(_generator);

            UpdateDetailsText();
        }

        KeyListGenerator GetOrCreateKeysGenerator(Type type)
        {
            if (!_keysGenerators.ContainsKey(type))
            {
                var so = (KeyListGenerator)CreateInstance(type);
                so.hideFlags = HideFlags.DontSave;
                _keysGenerators.Add(type, so);
            }
            return _keysGenerators[type];
        }
    }
}
