using System.Collections.Generic;
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

        protected override void HandleIsDirty()
        {
            if (!_primaryCamera || _secondaryCameras.Count == 0) return;

            foreach (var camera in _secondaryCameras)
            {
                camera.transform.localPosition = Vector3.zero;
                camera.transform.localRotation = Quaternion.identity;
                camera.targetDisplay = _primaryCamera.targetDisplay;
                
                if (camera.gameObject.TryGetComponent(out AudioListener audioListener))
                {
                    audioListener.enabled = false;
                }

                camera.orthographic = _primaryCamera.orthographic;
                if (camera.orthographic)
                {
                    camera.orthographicSize = _primaryCamera.orthographicSize;
                }
                else
                {
                    camera.fieldOfView = _primaryCamera.fieldOfView;
                }
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
