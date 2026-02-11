using Scenes.Simulation.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI.CarView
{
    public class ContentController
    {
        readonly VisualElement _emptyView;
        readonly Image _mainImage;
        readonly Image _mapImage;
        readonly VisualElement _pov;
        VisualElement _selfRoot;

        public ContentController(VisualElement visualElement)
        {
            _selfRoot = visualElement;
            
            _pov = visualElement.Q("pov-panel");
            _pov.style.visibility = StyleKeyword.Null;
            _pov.AddToClassList("hidden");
            
            _mapImage = visualElement.Q<Image>("mapView");
            _mainImage = visualElement.Q<Image>("mainImage");
            
            _emptyView = visualElement.Q("empty-panel");
            _emptyView.style.visibility = StyleKeyword.Null;
            _emptyView.AddToClassList("hidden");
            
        }

        public void LoadData(MissionSo mission)
        {
            _pov.EnableInClassList("hidden", !mission);
            _emptyView.EnableInClassList("hidden", mission);

            if (!mission) return;

            _mainImage.image = mission.options[0].mainTexture;
            _mapImage.image = mission.GetRouteTexture();
        }

        public void SetMainImage(Texture2D image)
        {
            _mainImage.image = image;
        }
    }
}
