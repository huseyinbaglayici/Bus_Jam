using System;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;
using UnityEngine;

namespace _Scripts.Runtime.Signals
{
    public class BusSignals : MonoSingleton<BusSignals>, IBusSignals
    {
        public event Func<EntityColor> OnGetActiveBusColor = () => EntityColor.Default;
        public event Func<EntityColor, Vector3> OnGetBusPosition = color => Vector3.zero;
        public event Func<EntityColor, bool> OnHasAvailableSlot = _ => false;
        public event Func<EntityColor, Vector3> OnGetSlotPosition = _ => Vector3.zero;
        public event Action OnPassengerBoardedBus = delegate { };
        public event Action<EntityColor> OnBusArrived = delegate { };


        public void FireOnBusArrived(EntityColor color)
        {
            OnBusArrived.Invoke(color);
        }

        public EntityColor FireOnGetActiveBusColor()
        {
            return OnGetActiveBusColor.Invoke();
        }

        public bool FireOnHasAvailableSlot(EntityColor color)
        {
            return OnHasAvailableSlot.Invoke(color);
        }

        public Vector3 FireOnGetBusPosition(EntityColor color)
        {
            return OnGetSlotPosition.Invoke(color);
        }

        public void FireOnPassengerBoardedBus()
        {
            OnPassengerBoardedBus.Invoke();
        }
    }
}