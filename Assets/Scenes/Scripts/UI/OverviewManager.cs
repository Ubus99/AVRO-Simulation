using System;
using System.Collections.Generic;
using car_logic;
using UI;
using UnityEngine;
using Utils;

namespace Scenes.Scripts.UI
{
    [RequireComponent(typeof(DynamicGrid))]
    public class OverviewManager : EditorBehavior
    {
        public delegate void FocusChangeDelegate(ADSV_AI activeVehicle);

        public CarTopView imagePrefab;
        public ADSV_AI selectedVehicle;

        readonly Dictionary<Texture, ADSV_AI> _renderTextures = new();
        readonly List<ADSV_AI> _trackedVehicles = new();
        readonly List<CarTopView> _views = new();
        DynamicGrid _gridLayout;

        void Awake()
        {
            RefreshComponents();
            ObjectManagementUtility.KillAllChildren(transform);
            ServiceLocator.Instance.TryRegister<OverviewManager>(this);
        }

        void OnEnable()
        {
            _gridLayout.OnLayoutChanged += MarkDirty;
        }

        void OnDisable()
        {
            _gridLayout.OnLayoutChanged -= MarkDirty;
            foreach (var v in _views)
            {
                v.OnClicked -= HandleViewClicked;
            }
        }

        void OnRectTransformDimensionsChange()
        {
            MarkDirty();
        }

        protected override void HandleIsDirty()
        {

            RebuildVideoFeed();
        }

        protected override void RefreshComponents()
        {
            _gridLayout = gameObject.GetComponent<DynamicGrid>();
            _dirty = true;
        }

        public event FocusChangeDelegate OnFocusChange;

        void MarkDirty()
        {
            _dirty = true;
        }

        public void RegisterVehicle(IEnumerable<ADSV_AI> cameras)
        {
            _trackedVehicles.AddRange(cameras);
            _dirty = true;
        }

        public void RegisterVehicle(ADSV_AI vehicle)
        {
            _trackedVehicles.Add(vehicle);
            _dirty = true;
        }

        public void DeregisterVehicle(ADSV_AI vehicle)
        {
            _trackedVehicles.Remove(vehicle);
            _dirty = true;
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
                var cellSize = _gridLayout.CellSize;
                var t = new RenderTexture(cellSize.x, cellSize.y, 16, RenderTextureFormat.ARGB32)
                {
                    name = v.name + "_texture",
                    antiAliasing = 4
                };
                v.topDownCamera.targetTexture = t;

                var view = Instantiate(imagePrefab, transform);
                view.transform.SetParent(transform);
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
            foreach (var ctw in _views)
            {
                ctw.selected = ctw == (CarTopView)sender;
                if (ctw.selected)
                {
                    selectedVehicle = ctw.ADS;
                }
            }
            OnFocusChange?.Invoke(selectedVehicle);
        }
    }
}
