using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Splines;

namespace Streets
{
    public class JunctionTrigger : MonoBehaviour
    {
        public SerializedDictionary<SplineContainer, float> junctionData = new();
    }
}
