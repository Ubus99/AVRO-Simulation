using car_logic;
using UnityEngine;

namespace Streets
{
    [ExecuteAlways]
    [RequireComponent(typeof(Collider))]
    public class StreetEvent : MonoBehaviour
    {
        Collider _trigger;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_trigger.bounds.center, _trigger.bounds.size);
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{other.name} has triggered {gameObject.name}, executing");
            switch (other.transform.root.tag)
            {
                case "Player":
                    var car = other.GetComponentInParent<ADSV_AI>();
                    car.TriggerError();
                    break;
                default:
                    return;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void OnValidate()
        {
            _trigger = GetComponent<Collider>();
            _trigger.isTrigger = true;
        }
    }
}
