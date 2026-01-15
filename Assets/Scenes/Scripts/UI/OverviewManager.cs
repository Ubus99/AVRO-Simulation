using System;
using System.Collections.Generic;
using car_logic;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Utils.Editor;
using Utils.Objects;
using Utils.Types;

namespace Scenes.Scripts.UI
{
    public class OverviewManager : EditorBehavior, ISubScreen
    {
        public CarTopView imagePrefab;
        public ADSV_AI selectedVehicle;
        public GameObject body;
        public UnityEvent<ADSV_AI> onViewSelected;

        readonly Dictionary<Texture, ADSV_AI> _renderTextures = new();
        readonly List<ADSV_AI> _trackedVehicles = new();
        readonly List<CarTopView> _views = new();
        DynamicGrid _gridLayout;

        void Awake()
        {
            RefreshComponents();
            if (Application.isPlaying && body)
            {
                ObjectManagementUtility.KillAllChildren(body.transform);
            }
            ServiceLocator.instance.TryRegister<OverviewManager>(this);
        }

        void OnEnable()
        {
            _gridLayout.OnLayoutChanged += MarkDirty;
            foreach (var view in _views)
            {
                view.OnClicked += HandleViewClicked;
            }
        }

        void OnDisable()
        {
            _gridLayout.OnLayoutChanged -= MarkDirty;
            foreach (var view in _views)
            {
                view.OnClicked -= HandleViewClicked;
            }
        }

        void OnRectTransformDimensionsChange()
        {
            MarkDirty();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        protected override void HandleIsDirty()
        {
            RebuildVideoFeed();
        }

        protected override void RefreshComponents()
        {
            _gridLayout = body.GetComponent<DynamicGrid>();
            Dirty = true;
        }

        void MarkDirty()
        {
            Dirty = true;
        }

        public void RegisterVehicle(IEnumerable<ADSV_AI> cameras)
        {
            _trackedVehicles.AddRange(cameras);
            Dirty = true;
        }

        public void RegisterVehicle(ADSV_AI vehicle)
        {
            _trackedVehicles.Add(vehicle);
            Dirty = true;
        }

        public void DeregisterVehicle(ADSV_AI vehicle)
        {
            _trackedVehicles.Remove(vehicle);
            Dirty = true;
        }

        void RebuildVideoFeed()
        {
            _renderTextures.Clear();
            foreach (var rawImage in _views)
            {
                DestroyImmediate(rawImage.gameObject);
            }
            _views.Clear();

            foreach (var v in _trackedVehicles)
            {
                var cellSize = _gridLayout.cellSize;
                var t = new RenderTexture(cellSize.x, cellSize.y, 16, RenderTextureFormat.ARGB32)
                {
                    name = v.name + "_texture",
                    antiAliasing = 4
                };
                v.topDownCamera.targetTexture = t;
                if (v.topDownCamera.gameObject.TryGetComponent(out CameraStackSynchronizer css))
                {
                    css.ForceRefresh();
                }

                var view = Instantiate(imagePrefab, transform);
                view.transform.SetParent(body.transform);
                view.transform.localScale = Vector3.one;
                view.image.texture = t;
                view.OnClicked += HandleViewClicked;
                view.ADS = v;
                _views.Add(view);
                _renderTextures.Add(t, v);
            }
        }

        void HandleViewClicked(object sender, EventArgs e)
        {
            foreach (var view in _views)
            {
                selectedVehicle = view == (CarTopView)sender ? view.ADS : null;
            }
            onViewSelected?.Invoke(selectedVehicle);
        }
    }
}
