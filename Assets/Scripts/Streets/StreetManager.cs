using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace Streets
{
    public class StreetManager : MonoBehaviour
    {
        bool _dirty;

        void Awake()
        {
            ServiceLocator.Instance.TryRegister<StreetManager>(this);
            _dirty = true;
        }

        public (SplineContainer container, float progress) GetClosestSpline(Vector3 transformPosition)
        {
            return (null, 0);
        }
    }
}
