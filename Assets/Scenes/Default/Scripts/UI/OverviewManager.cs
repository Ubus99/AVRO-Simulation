using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Scenes.Scripts.UI
{
    [RequireComponent(typeof(DynamicGrid))]
    public class OverviewManager : MonoBehaviour
    {
        public CarTopView imagePrefab;
        readonly List<Camera> _cameras = new();
        readonly List<RenderTexture> _renderTextures = new();
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

        public void AddCameras(IEnumerable<Camera> cameras)
        {
            _cameras.AddRange(cameras);
            _dirty = true;
        }

        public void AddCamera(Camera cam)
        {
            _cameras.Add(cam);
            _dirty = true;
        }

        public void AddCameras(IEnumerable<CinemachineBrain> brains)
        {
            foreach (var brain in brains)
            {
                _cameras.Add(brain.OutputCamera);
            }
            _dirty = true;
        }

        public void AddCamera(CinemachineBrain brain)
        {
            _cameras.Add(brain.OutputCamera);
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

            foreach (var c in _cameras)
            {
                var cellSize = _gridLayout.CellSize;
                var t = new RenderTexture(cellSize.x, cellSize.y, 16, RenderTextureFormat.ARGB32)
                {
                    name = c.name + "_texture",
                    antiAliasing = 4
                };
                c.targetTexture = t;

                var view = Instantiate(imagePrefab, transform);
                view.transform.SetParent(transform);
                view.transform.localScale = Vector3.one;
                view.image.texture = t;
                view.OnClicked += HandleViewClicked;
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
