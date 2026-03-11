using System;
using _Scripts.Runtime.Gameplay.Entities.Passenger;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ILineSignals
    {
        public event Func<bool> OnHasAvailableSlot;
        public event Func<PassengerController,UnityEngine.Vector3> OnGetSlotPosition;
    }
}