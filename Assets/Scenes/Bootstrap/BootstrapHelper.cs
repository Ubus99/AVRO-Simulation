using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Scenes.Bootstrap
{
    [ExecuteAlways]
    public class BootstrapHelper : MonoBehaviour
    {
        const string BootstrapScenePath = "Assets/Scenes/Bootstrap/BootstrapScene.unity";

#if UNITY_EDITOR
        [SerializeField]
        SceneAsset sceneToLoad;
#endif

        [SerializeField]
        string targetPath;

        void Start()
        {
            if (Application.isPlaying)
            {
                BootstrapPlayer();
            }
            else
            {
                BootstrapEditor();
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            targetPath = AssetDatabase.GetAssetPath(sceneToLoad);
        }
#endif

        void BootstrapEditor()
        {
#if UNITY_EDITOR
            MySceneManager.CloseAllScenes(BootstrapScenePath);
            EditorSceneManager.OpenScene(targetPath, OpenSceneMode.Additive);
#endif
        }

        void BootstrapPlayer()
        {
            var gos = GameObject.FindGameObjectsWithTag("bootstrap");
            foreach (var go in gos) DontDestroyOnLoad(go); // mark bootstrap scene save

            SceneManager.LoadScene(Path.GetFileNameWithoutExtension(targetPath));
        }
    }
}
