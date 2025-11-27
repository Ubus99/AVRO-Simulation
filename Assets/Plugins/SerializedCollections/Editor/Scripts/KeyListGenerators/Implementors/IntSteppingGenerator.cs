using System;
using System.Collections;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    [KeyListGenerator("Int Stepping", typeof(int))]
    public class IntSteppingGenerator : KeyListGenerator
    {
        [SerializeField]
        int _startIndex;

        [SerializeField]
        int _stepDistance = 10;

        [SerializeField]
        [Min(0)]
        int _stepCount = 1;

        public override IEnumerable GetKeys(Type type)
        {
            for (var i = 0; i <= _stepCount; i++)
            {
                yield return _startIndex + i * _stepDistance;
            }
        }
    }
}
