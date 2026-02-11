using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Bootstrap
{
    public class BootstrapHelper : MonoBehaviour
    {
        [SerializeField]
        SceneAsset sceneToLoad;

        string _targetPath;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _targetPath = AssetDatabase.GetAssetPath(sceneToLoad);

            CloseAllScenes();
            EditorSceneManager.OpenScene(_targetPath, OpenSceneMode.Additive);
        }

        void OnValidate()
        {
            Start();
        }

        void CloseAllScenes()
        {
            var count = SceneManager.sceneCount;

            for (var i = count - 1; i >= 0; i--)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.path == _targetPath) continue;
                EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
