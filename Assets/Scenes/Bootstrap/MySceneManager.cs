using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Scenes.Bootstrap
{
    public static class MySceneManager
    {
        public static void SwitchToScene(string path)
        {
            CloseAllScenes();
            SceneManager.LoadSceneAsync(path, LoadSceneMode.Additive);
        }

        public static void CloseAllScenes(IList<string> exceptPaths)
        {
            var count = SceneManager.sceneCount;

            for (var i = count - 1; i >= 0; i--)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (exceptPaths.Contains(scene.path)) continue;
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        public static void CloseAllScenes(string exceptPath = null)
        {
            CloseAllScenes(exceptPath != null ? new[] { exceptPath } : ArraySegment<string>.Empty);
        }
    }
}
