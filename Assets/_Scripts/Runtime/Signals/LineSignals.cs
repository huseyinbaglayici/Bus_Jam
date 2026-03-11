using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals.Interfaces;
using UnityEngine;

namespace _Scripts.Runtime.Signals
{
    public class LineSignals : MonoSingleton<LineSignals>, ILineSignals
    {
        public event Func<bool> OnHasAvailableSlot = () => false;
        public event Func<PassengerController, Vector3> OnGetSlotPosition = controller => Vector3.zero;

        public bool FireOnHasAvailableSlot()
        {
            return OnHasAvailableSlot.Invoke();
        }

        public Vector3 FireOnGetSlotPosition(PassengerController controller)
        {
            return OnGetSlotPosition.Invoke(controller);
        }
    }
}