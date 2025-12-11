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
            menu.SetActive(false);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            ServiceLocator.Instance.TryGet<OverviewManager>(out var overviewManager);
            overviewManager.OnFocusChange += vehicle => AssignCamera(vehicle.povCamera);
        }

        void AssignCamera(Camera cam)
        {
            videoFeed.SetCamera(cam);
        }

        public void OnObstacleClicked(IPlayerClickable playerClickable, Vector2 pos)
        {
            menu.SetActive(true);
            menu.transform.position = pos;
        }

        public void OnObstacleMissed(Vector2 pos)
        {
            menu.SetActive(false);
            menu.transform.position = pos;
        }
    }
}
