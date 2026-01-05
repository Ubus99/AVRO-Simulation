using Scenes.Scripts.UI;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes.Scripts.Missions
{
    [ExecuteInEditMode]
    public class AlternativeRoute : MonoBehaviour
    {
        [Header("Information")] //
        [SerializeField]
        private string informationText;

        [SerializeField] private bool selectable;

        [Header("Settings")] //
        public VizSettings vizSettings;

        // --------------------- private ------------------

        private Material _material;

        private MeshRenderer _renderer;

        public SplineContainer Route { get; private set; }

        private void Start()
        {
            GetDependencies();
        }

        private void OnValidate()
        {
            GetDependencies();
            if (!_material || !vizSettings) return;
            _material.color = !selectable ? vizSettings.errorColor : vizSettings.inactiveColor;
        }

        public ListItem.ElementData GetData()
        {
            return new ListItem.ElementData { titleText = name, labelText = informationText };
        }

        private void GetDependencies()
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
            _material = _renderer.material = new Material(_renderer.sharedMaterial);
            Route = GetComponentInChildren<SplineContainer>();
        }
    }
}