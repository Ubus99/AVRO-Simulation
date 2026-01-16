using UI;
using UnityEngine;
using UnityEngine.Splines;
using Utils.Types;

namespace Scenes.Scripts.Missions
{
    [ExecuteInEditMode]
    public class AlternativeRoute : EditorBehavior, IListable
    {
        [Header("Information")] //
        [SerializeField]
        string informationText;

        [SerializeField]
        bool selectable;

        [Header("Settings")] //
        public VizSettings vizSettings;

        // --------------------- private ------------------

        Material _material;

        MeshRenderer _renderer;

        public AlternativeRouteHelper parent { get; set; }

        public SplineContainer route { get; private set; }

        void Start()
        {
            RefreshComponents();
            if (vizSettings) vizSettings.OnChanged += HandleIsDirty;
        }

        void OnDisable()
        {

            if (vizSettings) vizSettings.OnChanged -= HandleIsDirty;
        }

        public ElementData ElementData()
        {
            var data = new ElementData
            {
                titleText = name,
                labelText = informationText,
                selectable = selectable
            };
            return data;
        }

        protected override void HandleIsDirty()
        {
            RefreshComponents();
            if (!_material || !vizSettings) return;
            _material.color = !selectable ? vizSettings.errorColor : vizSettings.inactiveColor;
        }

        protected override void RefreshComponents()
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
            _material = _renderer.material = new Material(_renderer.sharedMaterial);
            route = GetComponentInChildren<SplineContainer>();
        }

        public void SetSelected(bool active)
        {
            if (!selectable) return;
            _material.color = active ? vizSettings.activeColor : vizSettings.inactiveColor;
        }
    }
}
