using UnityEngine;

namespace Dreamteck.Splines.Examples
{
    public class AddForceAlongPath : MonoBehaviour
    {
        public float force = 10f;
        SplineProjector projector;
        Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            projector = GetComponent<SplineProjector>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(projector.result.forward * force, ForceMode.Impulse);
            }
        }
    }
}
