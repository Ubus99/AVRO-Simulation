using System;
using System.Collections;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    [KeyListGenerator("Int Range", typeof(int))]
    public class IntRangeGenerator : KeyListGenerator
    {
        [SerializeField]
        int _startValue = 1;

        [SerializeField]
        int _endValue = 10;

        public override IEnumerable GetKeys(Type type)
        {
            var dir = Math.Sign(_endValue - _startValue);
            dir = dir == 0 ? 1 : dir;
            for (var i = _startValue; i != _endValue; i += dir)
                yield return i;
            yield return _endValue;
        }
    }
}
