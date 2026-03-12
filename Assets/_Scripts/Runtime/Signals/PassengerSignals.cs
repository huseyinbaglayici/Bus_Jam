using System;
using UnityEngine;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Gameplay.Entities.Passenger;

namespace _Scripts.Runtime.Signals
{
    public class PassengerSignals : MonoSingleton<PassengerSignals>
    {
        public event Action<PassengerEntity> OnHandleTappedPassenger = delegate { };
        public event Action<Vector2Int, PassengerController> OnRegisterPassenger = delegate { };

        public void FireHandleTappedPassenger(PassengerEntity entity) => OnHandleTappedPassenger.Invoke(entity);

        public void FireOnRegisterPassenger(Vector2Int pos, PassengerController controller) =>
            OnRegisterPassenger.Invoke(pos, controller);
    }
}