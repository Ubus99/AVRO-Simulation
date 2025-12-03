using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gameplay
{
    [ExecuteInEditMode]
    public class SceneLoader : MonoBehaviour
    {
        public GameObject scenePrefab;
        public Vector2Int grid;
        public float gridSize = 100;

        readonly List<GameObject> _instances = new();
        bool _dirty;

        void Update()
        {
            if (!_dirty)
                return;

            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var go = transform.GetChild(i).gameObject;
                _instances.Remove(go);
                DestroyImmediate(go);
            }

            _instances.Clear();
            for (var x = 0; x < grid.x; x++)
            for (var y = 0; y < grid.y; y++)
            {
                var scene = PrefabUtility.InstantiatePrefab(scenePrefab, transform) as GameObject;
                if (!scene)
                    continue;

                scene.transform.position = new Vector3(x, 0, y) * gridSize;
                _instances.Add(scene);
            }

            _dirty = false;
        }

        void OnValidate()
        {
            _dirty = true;
        }
    }
}
