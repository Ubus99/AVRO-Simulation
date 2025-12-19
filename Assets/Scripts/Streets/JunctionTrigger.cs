using System.Linq;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace Streets
{
    public class JunctionTrigger : MonoBehaviour
    {
        public WeightedPool<SplineContainer> junctionData = new();

        public bool TryGetRandomExit(out SplineContainer exit)
        {
            if (!junctionData.Any(pair => pair.Value > 0))
            {
                exit = null;
                return false;
            }
            exit = junctionData.DrawRandom();
            return true;
        }
    }
}
