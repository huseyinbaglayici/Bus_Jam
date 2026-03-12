using System;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Extensions;
using UnityEngine;

namespace _Scripts.Runtime.Signals
{
    public class BusSignals : MonoSingleton<BusSignals>
    {
        public event Func<EntityColor> OnGetActiveBusColor = () => EntityColor.Default;
        public event Func<EntityColor, Vector3> OnGetBusPosition = _ => Vector3.zero;
        public event Func<EntityColor, bool> OnHasAvailableSlot = _ => false;
        public event Action OnPassengerBoardedBus = delegate { };
        public event Action<EntityColor> OnBusArrived = delegate { };
        public event Action<EntityColor> OnBusLeft = delegate { };
        public event Action<Vector3> OnStationPositionReady = delegate { };

        public event Func<EntityColor> OnGetNextBusColor = () => EntityColor.Default;


        public Vector3 FireOnGetBusPosition(EntityColor color) => OnGetBusPosition.Invoke(color);
        public bool FireOnHasAvailableSlot(EntityColor color) => OnHasAvailableSlot.Invoke(color);
        public void FireOnPassengerBoardedBus() => OnPassengerBoardedBus.Invoke();
        public void FireOnBusArrived(EntityColor color) => OnBusArrived.Invoke(color);
        public void FireOnBusLeft(EntityColor color) => OnBusLeft.Invoke(color);
        public void FireOnStationPositionReady(Vector3 pos) => OnStationPositionReady.Invoke(pos);

        public EntityColor FireOnGetActiveBusColor() => OnGetActiveBusColor.Invoke();
        public EntityColor FireOnGetNextBusColor() => OnGetNextBusColor.Invoke();
    }
}