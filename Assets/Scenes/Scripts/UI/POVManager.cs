using UnityEngine;
using Utils;

namespace Scenes.Scripts.UI
{
    public class POVManager : MonoBehaviour
    {
        public GameObject menu;

        void Awake()
        {
            ServiceLocator.Instance.TryRegister<POVManager>(this);
        }

        public void OpenAt(Vector2 pos)
        {
            menu.transform.position = pos;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
