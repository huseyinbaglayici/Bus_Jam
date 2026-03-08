using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class PassengerManager : MonoSingleton<PassengerManager>
    {
        private void Start()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            PassengerSignals.Instance.OnHandleTappedPassenger += OnHandlePassenger;
        }

        private void OnHandlePassenger(PassengerEntity entity)
        {
            Debug.LogWarning("entity handled");
        }

        private void UnSubscribeEvents()
        {
            PassengerSignals.Instance.OnHandleTappedPassenger -= OnHandlePassenger;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}