using UI;
using UnityEngine;
using UnityEngine.Splines;
using Utils.Lucide;
using Utils.Types;

namespace Scenes.Scripts.Missions
{
    [ExecuteInEditMode]
    public class AlternativeRoute : EditorBehavior
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

        public IListElement ElementData()
        {
            var data = new AlternativeRouteListElement(name, informationText, selectable);
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

        public class AlternativeRouteListElement : IListElement
        {
            public AlternativeRouteListElement(string titleText, string labelText, bool selectable)
            {
                this.labelText = labelText;
                this.selectable = selectable;
                this.titleText = titleText;
            }

            public bool selectable { get; }

            public GlyphData leftIcon
            {
                get { return null; }
            }

            public GlyphData rightIcon
            {
                get { return null; }
            }

            public string titleText { get; }
            public string labelText { get; }
        }
    }
}
