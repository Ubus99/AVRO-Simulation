using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.Bootstrap
{
    [ExecuteAlways]
    public class BootstrapHelper : MonoBehaviour
    {
        const string BootstrapScenePath = "Assets/Scenes/Bootstrap/BootstrapScene.unity";

        [SerializeField]
        SceneAsset sceneToLoad;

        string _targetPath;

        void Start()
        {
            _targetPath = AssetDatabase.GetAssetPath(sceneToLoad);

            if (Application.isPlaying)
            {
                BootstrapPlayer();
            }
            else
            {
                BootstrapEditor();
            }
        }

        void BootstrapEditor()
        {
            MySceneManager.CloseAllScenes(BootstrapScenePath);
            EditorSceneManager.OpenScene(_targetPath, OpenSceneMode.Additive);
        }

        void BootstrapPlayer()
        {
            var gos = GameObject.FindGameObjectsWithTag("bootstrap");
            foreach (var go in gos) DontDestroyOnLoad(go); // mark bootstrap scene save

            SceneManager.LoadScene(Path.GetFileNameWithoutExtension(_targetPath));
        }
    }
}
