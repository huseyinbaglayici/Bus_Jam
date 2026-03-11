using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Gameplay.Entities.Passenger;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class PassengerSignals : MonoSingleton<PassengerSignals>, IPassengerSignals
    {
        public event Action<PassengerEntity> OnHandleTappedPassenger = delegate { };
        public event Action<UnityEngine.Vector2Int, PassengerController> OnRegisterPassenger = delegate { };

        public void FireHandleTappedPassenger(PassengerEntity entity) => OnHandleTappedPassenger?.Invoke(entity);

        public void FireOnRegisterPassenger(UnityEngine.Vector2Int pos, PassengerController controller) =>
            OnRegisterPassenger?.Invoke(pos, controller);
    }
}