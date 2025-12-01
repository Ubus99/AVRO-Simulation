using System;
using System.Collections.Generic;
using System.Linq;
using car_logic;
using Scenes.Default.Scripts.UI;
using UI;
using UnityEngine;
using Utils;

namespace Scenes.Scripts.UI
{
    [RequireComponent(typeof(DynamicGrid))]
    public class OverviewManager : MonoBehaviour
    {
        public CarTopView imagePrefab;
        readonly List<RenderTexture> _renderTextures = new();
        readonly List<ADSV_AI> _trackedVehicles = new();
        readonly List<CarTopView> _views = new();
        bool _dirty;
        DynamicGrid _gridLayout;

        void Awake()
        {
            _gridLayout = gameObject.GetComponent<DynamicGrid>();
            _dirty = true;
            ServiceLocator.Instance.TryRegister<OverviewManager>(this);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!_dirty)
                return;

            RebuildVideoFeed();
            _dirty = false;
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

        void OnValidate()
        {
            MarkDirty();
        }

        void MarkDirty()
        {
            _dirty = true;
        }

        public void RegisterVehicle(IEnumerable<ADSV_AI> cameras)
        {
            _trackedVehicles.AddRange(cameras);
            _dirty = true;
        }

        public void RegisterVehicle(ADSV_AI cam)
        {
            _trackedVehicles.Add(cam);
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
                _renderTextures.Add(t);
            }
        }

        void HandleViewClicked(object sender, EventArgs e)
        {
            foreach (var ctw in _views.Where(ctw => ctw != (CarTopView)sender))
            {
                ctw.selected = false;
            }
        }
    }
}
