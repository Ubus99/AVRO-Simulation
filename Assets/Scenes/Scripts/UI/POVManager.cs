using UI;
using UnityEngine;
using Utils;

namespace Scenes.Scripts.UI
{
    public class POVManager : MonoBehaviour
    {
        public GameObject menu;
        public VideoFeed videoFeed;

        void Awake()
        {
            ServiceLocator.Instance.TryRegister<POVManager>(this);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.Instance.TryGet<OverviewManager>(out var overviewManager);
            overviewManager.OnFocusChange += vehicle => AssignCamera(vehicle.povCamera);
        }

        public void OpenAt(Vector2 pos)
        {
            menu.transform.position = pos;
        }

        public void AssignCamera(Camera cam)
        {
            videoFeed.UpdateFeed(cam);
        }
    }
}
