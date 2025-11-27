using UnityEngine;

public class SensorManager : MonoBehaviour
{
    public string tagName;
    CarAI carAI;

    void Start()
    {
        carAI = gameObject.transform.parent.GetComponent<CarAI>();
    }

    void OnTriggerEnter(Collider car)
    {
        if (car.gameObject.CompareTag(tagName))
        {
            carAI.move = false;
        }
    }

    void OnTriggerExit(Collider car)
    {
        if (car.gameObject.CompareTag(tagName))
        {
            carAI.move = true;
        }
    }
}
