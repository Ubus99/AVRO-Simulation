using UnityEngine;

namespace Dreamteck.Splines
{
    [ExecuteInEditMode]
    [AddComponentMenu("Dreamteck/Splines/Users/Spline Projector")]
    public class SplineProjector : SplineTracer
    {
        public enum Mode
        {
            Accurate,
            Cached
        }

        [SerializeField]
        [HideInInspector]
        Mode _mode = Mode.Cached;

        [SerializeField]
        [HideInInspector]
        bool _autoProject = true;

        [SerializeField]
        [HideInInspector]
        [Range(3, 8)]
        int _subdivide = 4;

        [SerializeField]
        [HideInInspector]
        Transform _projectTarget;


        [SerializeField]
        [HideInInspector]
        Transform applyTarget;

        [SerializeField]
        [HideInInspector]
        GameObject _targetObject;

        [SerializeField]
        [HideInInspector]
        public Vector2 _offset;

        [SerializeField]
        [HideInInspector]
        public Vector3 _rotationOffset = Vector3.zero;

        [SerializeField]
        [HideInInspector]
        Vector3 lastPosition = Vector3.zero;

        public Mode mode
        {
            get { return _mode; }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                    Rebuild();
                }
            }
        }

        public bool autoProject
        {
            get { return _autoProject; }
            set
            {
                if (value != _autoProject)
                {
                    _autoProject = value;
                    if (_autoProject) Rebuild();
                }
            }
        }

        public int subdivide
        {
            get { return _subdivide; }
            set
            {
                if (value != _subdivide)
                {
                    _subdivide = value;
                    if (_mode == Mode.Accurate) Rebuild();
                }
            }
        }

        public Transform projectTarget
        {
            get
            {
                if (_projectTarget == null) return transform;
                return _projectTarget;
            }
            set
            {
                if (value != _projectTarget)
                {
                    _projectTarget = value;
                    Rebuild();
                }
            }
        }

        public GameObject targetObject
        {
            get
            {
                if (_targetObject)
                    return _targetObject;
                if (!applyTarget) //Temporary check to migrate SplineProjectors that use target
                    return _targetObject;
                _targetObject = applyTarget.gameObject;
                applyTarget = null;
                return _targetObject;
            }

            set
            {
                if (value != _targetObject)
                {
                    _targetObject = value;
                    RefreshTargets();
                    Rebuild();
                }
            }
        }

        protected override void Reset()
        {
            base.Reset();
            _projectTarget = transform;
        }

        public event SplineReachHandler onEndReached;
        public event SplineReachHandler onBeginningReached;

        protected override Transform GetTransform()
        {
            if (!targetObject) return null;
            return targetObject.transform;
        }

        protected override Rigidbody GetRigidbody()
        {
            if (!targetObject) return null;
            return targetObject.GetComponent<Rigidbody>();
        }

        protected override Rigidbody2D GetRigidbody2D()
        {
            if (targetObject == null) return null;
            return targetObject.GetComponent<Rigidbody2D>();
        }


        protected override void LateRun()
        {
            base.LateRun();
            if (!autoProject)
                return;
            if (!projectTarget || lastPosition == projectTarget.position)
                return;
            lastPosition = projectTarget.position;
            CalculateProjection();
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            CalculateProjection();
        }

        protected override void OnSplineChanged()
        {
            if (spline)
            {
                if (_mode == Mode.Accurate)
                {
                    spline.Project(_projectTarget.position,
                    ref _result,
                    clipFrom,
                    clipTo,
                    SplineComputer.EvaluateMode.Calculate,
                    subdivide);
                }
                else
                {
                    spline.Project(_projectTarget.position, ref _result, clipFrom, clipTo);
                }
                _result.percent = ClipPercent(_result.percent);
            }
        }


        void Project()
        {
            if (_mode == Mode.Accurate && spline)
            {
                spline.Project(_projectTarget.position,
                ref _result,
                clipFrom,
                clipTo,
                SplineComputer.EvaluateMode.Calculate,
                subdivide);
                _result.percent = ClipPercent(_result.percent);
            }
            else
            {
                Project(_projectTarget.position, ref _result);
            }
        }

        public void CalculateProjection()
        {
            if (!_projectTarget) return;
            var lastPercent = _result.percent;
            Project();

            if (onBeginningReached != null && _result.percent <= clipFrom)
            {
                if (!Mathf.Approximately((float)lastPercent, (float)_result.percent))
                {
                    onBeginningReached();
                    if (samplesAreLooped)
                    {
                        CheckTriggers(lastPercent, 0.0);
                        CheckNodes(lastPercent, 0.0);
                        lastPercent = 1.0;
                    }
                }
            }
            else if (onEndReached != null && _result.percent >= clipTo)
            {
                if (!Mathf.Approximately((float)lastPercent, (float)_result.percent))
                {
                    onEndReached();
                    if (samplesAreLooped)
                    {
                        CheckTriggers(lastPercent, 1.0);
                        CheckNodes(lastPercent, 1.0);
                        lastPercent = 0.0;
                    }
                }
            }

            CheckTriggers(lastPercent, _result.percent);
            CheckNodes(lastPercent, _result.percent);


            if (targetObject)
            {
                ApplyMotion();
            }

            InvokeTriggers();
            InvokeNodes();
            lastPosition = projectTarget.position;
        }
    }
}
