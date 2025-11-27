using UnityEngine;

namespace Dreamteck.Splines
{
    //Use the CreateAssetMenu attribute to add the object to the Create Asset context menu
    //After that, go to Assets/Create/Dreamteck/Splines/... and create the scriptable object
    [CreateAssetMenu(menuName = "Dreamteck/Splines/Object Controller Rules/Spiral Rule")]
    public class ObjectControllerSpiralRule : ObjectControllerCustomRuleBase
    {
        [SerializeField]
        bool _useSplinePercent;

        [SerializeField]
        float _revolve = 360f;

        [SerializeField]
        Vector2 _startSize = Vector2.one;

        [SerializeField]
        Vector2 _endSize = Vector2.one;

        [SerializeField]
        [Range(0f, 1f)]
        float _offset;

        public bool useSplinePercent
        {
            get { return _useSplinePercent; }
            set { _useSplinePercent = value; }
        }

        public float revolve
        {
            get { return _revolve; }
            set { _revolve = value; }
        }

        public Vector2 startSize
        {
            get { return _startSize; }
            set { _startSize = value; }
        }

        public Vector2 endSize
        {
            get { return _endSize; }
            set { _endSize = value; }
        }

        public float offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                if (_offset > 1)
                {
                    _offset -= Mathf.FloorToInt(_offset);
                }
                if (_offset < 0)
                {
                    _offset += Mathf.FloorToInt(-_offset);
                }
            }
        }

        //Override GetOffset, GetRotation and GetScale to implement custom behaviors
        //Use the information from currentSample, currentObjectIndex, totalObjects and currentObjectPercent

        public override Vector3 GetOffset()
        {
            var offset = Quaternion.AngleAxis(_revolve * GetPercent(), Vector3.forward) * Vector3.up;
            var scale = Vector2.Lerp(_startSize, _endSize, GetPercent());
            offset.x *= scale.x;
            offset.y *= scale.y;
            return offset;
        }

        public override Quaternion GetRotation()
        {
            return currentSample.rotation * Quaternion.AngleAxis(_revolve * -GetPercent(), Vector3.forward);
        }

        float GetPercent()
        {
            var percent = _useSplinePercent ? (float)currentSample.percent : currentObjectPercent + _offset;
            if (percent > 1)
            {
                percent -= Mathf.FloorToInt(percent);
            }
            if (percent < 0)
            {
                percent += Mathf.FloorToInt(-percent);
            }
            return percent;
        }
    }
}
