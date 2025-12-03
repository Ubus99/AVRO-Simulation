using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class SceneLoader : MonoBehaviour
    {
        public string scenePath;
        public Vector2Int grid;
        public float gridSize = 100;

        private bool _dirty;

        private void Update()
        {
            if (!_dirty)
                return;

            for (var i = 0; i < grid.x * grid.y; i++) SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);

            for (var x = 0; x < grid.x; x++)
            for (var y = 0; y < grid.y; y++)
            {
                var gos = SceneManager.GetSceneAt(x + x * y).GetRootGameObjects();
                foreach (var go in gos)
                    go.transform.localPosition = new Vector3(x * gridSize, 0, y * gridSize);
            }

            _dirty = false;
        }

        private void OnValidate()
        {
            _dirty = true;
        }
    }
}