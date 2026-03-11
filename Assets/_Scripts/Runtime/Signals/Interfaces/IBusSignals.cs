using System;
using _Scripts.Runtime.Enums;
using UnityEngine;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IBusSignals
    {
        public event Func<EntityColor> OnGetActiveBusColor;
        public event Func<EntityColor, bool> OnHasAvailableSlot;
        public event Func<EntityColor, Vector3> OnGetBusPosition;
        public event Action<EntityColor> OnBusArrived;
    }
}