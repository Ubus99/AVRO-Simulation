using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scenes.Simulation.UI.CarView
{
    public class ContentController
    {
        static readonly StyleEnum<DisplayStyle> Hidden = new(DisplayStyle.None);
        static readonly StyleEnum<DisplayStyle> Visible = new(DisplayStyle.Flex);

        readonly VisualElement _emptyView;
        readonly Image _mainImage;
        readonly Image _mapImage;
        readonly VisualElement _newView;
        readonly VisualElement _pov;
        VisualElement _selfRoot;

        public ContentController(VisualElement visualElement)
        {
            _selfRoot = visualElement;

            _mapImage = visualElement.Q<Image>("mapView");
            _mainImage = visualElement.Q<Image>("mainImage");

            _pov = visualElement.Q("pov-panel");
            _emptyView = visualElement.Q("empty-panel");
            _newView = visualElement.Q("new-panel");
        }

        public void LoadData(MissionSo mission)
        {
            if (!mission) return;
            if (!mission.options[0]) return;
            
            _mainImage.image = mission.options[0].mainTexture;
            _mapImage.image = mission.GetRouteTexture();
        }

        public void SwitchView(CarViewController.View view)
        {
            switch (view)
            {
                case CarViewController.View.EmptyView:
                    _pov.style.display = Hidden;
                    _newView.style.display = Hidden;
                    _emptyView.style.display = Visible;
                    break;
                case CarViewController.View.NewView:
                    _pov.style.display = Hidden;
                    _newView.style.display = Visible;
                    _emptyView.style.display = Hidden;
                    break;
                case CarViewController.View.PovView:
                    _pov.style.display = Visible;
                    _newView.style.display = Hidden;
                    _emptyView.style.display = Hidden;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(view), view, null);
            }
        }

        public void SetMainImage(Texture2D image)
        {
            _mainImage.image = image;
        }
    }
}
