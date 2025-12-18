using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Streets
{
    public class JunctionTrigger : MonoBehaviour
    {
        public SerializedDictionary<Street.Address, float> junctionData = new();
    }
}
