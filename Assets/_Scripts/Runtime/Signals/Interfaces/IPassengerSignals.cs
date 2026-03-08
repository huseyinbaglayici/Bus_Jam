using System;
using _Scripts.Runtime.Gameplay.Entities.Passenger;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IPassengerSignals
    {
        event Action<PassengerEntity> OnHandleTappedPassenger;
    }
}