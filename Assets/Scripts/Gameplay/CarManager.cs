using System.Collections.Generic;
using car_logic;
using UnityEngine;
using Utils;

namespace Gameplay
{
    public class CarManager : MonoBehaviour
    {
        readonly Dictionary<int, ADSV_AI> _cars = new();

        void Awake()
        {
            ServiceLocator.Instance.TryRegister<CarManager>(this);
        }

        public void AddCar(ADSV_AI car)
        {
            if (!_cars.TryAdd(car.GetInstanceID(), car))
            {
                Debug.LogWarning("tried to register car twice");
            }
        }

        public void RemoveCar(int id)
        {
            Destroy(_cars[id]);
            _cars.Remove(id);
        }

        public ADSV_AI GetCar(int id)
        {
            return _cars[id];
        }
    }
}
