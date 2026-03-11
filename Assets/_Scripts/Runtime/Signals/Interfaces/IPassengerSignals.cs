using System;
using _Scripts.Runtime.Gameplay.Entities.Passenger;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IPassengerSignals
    {
        public event Action<PassengerEntity> OnHandleTappedPassenger;
        public event Action<UnityEngine.Vector2Int,PassengerController> OnRegisterPassenger;
    }
}