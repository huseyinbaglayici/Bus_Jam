using System;
using UnityEngine;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Gameplay.Entities.Passenger;

namespace _Scripts.Runtime.Signals
{
    public class LineSignals : MonoSingleton<LineSignals>
    {
        public event Func<bool> OnHasAvailableSlot = () => false;
        public event Func<PassengerController, Vector3> OnGetSlotPosition = _ => Vector3.zero;

        public bool FireOnHasAvailableSlot() => OnHasAvailableSlot.Invoke();
        public Vector3 FireOnGetSlotPosition(PassengerController controller) => OnGetSlotPosition.Invoke(controller);
    }
}