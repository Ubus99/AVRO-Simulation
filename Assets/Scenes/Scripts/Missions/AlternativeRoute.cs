using Scenes.Scripts.UI;
using UnityEngine;
using UnityEngine.Splines;
using Utils;
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

        public SplineContainer Route { get; private set; }

        void Start()
        {
            GetDependencies();
        }

        protected override void HandleIsDirty()
        {

            GetDependencies();
            if (!_material || !vizSettings) return;
            _material.color = !selectable ? vizSettings.errorColor : vizSettings.inactiveColor;
        }

        protected override void RefreshComponents()
        {
            GetDependencies();
        }

        public ListItem.ElementData GetData()
        {
            return new ListItem.ElementData { titleText = name, labelText = informationText };
        }

        void GetDependencies()
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
            _material = _renderer.material = new Material(_renderer.sharedMaterial);
            Route = GetComponentInChildren<SplineContainer>();
        }
    }
}
