using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Random = UnityEngine.Random;

namespace Utils
{
    [Serializable]
    public class WeightedPool<T> : SerializedDictionary<T, float>
    {
        public T DrawRandom()
        {
            var available = this
                .Where(pair => pair.Value > 0)
                .OrderBy(pair => pair.Value)
                .ToList();

            var range = available.Sum(pair => pair.Value);
            var i = Random.value * range;

            float acc = 0;
            foreach (var kvp in available)
            {
                if (kvp.Value + acc < i)
                {
                    return kvp.Key;
                }
                acc += kvp.Value;
            }
            return available.First().Key;
        }
    }
}
