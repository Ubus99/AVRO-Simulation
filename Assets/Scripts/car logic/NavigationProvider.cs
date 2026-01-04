using UnityEngine;

namespace car_logic
{
    public abstract class NavigationProvider : MonoBehaviour
    {
        public float baseSpeed;
        public bool visualize = true;

        public abstract float GetTargetSpeed();

        public abstract void SetTargetSpeed(float speed);

        public abstract void SetTargetLocation(Vector3 position);

        public abstract void Halt();
    }
}
