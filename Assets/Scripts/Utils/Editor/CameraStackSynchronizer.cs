using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Types;

namespace Utils.Editor
{
    [ExecuteAlways]
    public class CameraStackSynchronizer : EditorBehavior
    {
        readonly List<Camera> _secondaryCameras = new();
        Camera _primaryCamera;

        void Awake()
        {
            RefreshComponents();
        }

        public void ForceRefresh()
        {
            Dirty = true;
        }

        protected override void HandleIsDirty()
        {
            if (!_primaryCamera || _secondaryCameras.Count == 0) return;

            foreach (var cam in _secondaryCameras.Where(cam => cam != _primaryCamera)) // with failsafe
            {
                cam.transform.localPosition = Vector3.zero;
                cam.transform.localRotation = Quaternion.identity;
                cam.transform.localScale = Vector3.one;

                cam.useOcclusionCulling = _primaryCamera.useOcclusionCulling;
                
                cam.targetTexture = _primaryCamera.targetTexture;
                cam.rect = _primaryCamera.rect;
                cam.targetDisplay = _primaryCamera.targetDisplay;

                if (cam.gameObject.TryGetComponent(out AudioListener audioListener))
                {
                    audioListener.enabled = false;
                }

                cam.orthographic = _primaryCamera.orthographic;
                cam.orthographicSize = _primaryCamera.orthographicSize;
                cam.fieldOfView = _primaryCamera.fieldOfView;
            }
        }

        protected override void RefreshComponents()
        {
            _primaryCamera = GetComponent<Camera>();

            _secondaryCameras.Clear();
            _secondaryCameras.AddRange(GetComponentsInChildren<Camera>());
        }
    }
}
