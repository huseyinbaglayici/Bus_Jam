using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals.Interfaces;
using UnityEngine.Events;

namespace _Scripts.Runtime.Signals
{
    public class PassengerSignals : MonoSingleton<PassengerSignals>, IPassengerSignals
    {
        public event Action<PassengerEntity> OnHandleTappedPassenger = delegate { };

        public void FireHandleTappedPassenger(PassengerEntity entity) => OnHandleTappedPassenger?.Invoke(entity);
    }
}