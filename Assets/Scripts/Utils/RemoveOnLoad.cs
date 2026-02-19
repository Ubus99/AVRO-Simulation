using UnityEngine;

namespace Utils
{
    public class RemoveOnLoad : MonoBehaviour
    {
        void Start()
        {
            Debug.Log($"Destroying {gameObject.name}");
            Destroy(gameObject);
        }
    }
}
