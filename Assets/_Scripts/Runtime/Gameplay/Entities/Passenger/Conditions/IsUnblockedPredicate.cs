using _Scripts.Core;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.Conditions
{
    public class IsUnblockedPredicate : IPredicate
    {
        private readonly PassengerEntity _entity;
        public IsUnblockedPredicate(PassengerEntity entity) => _entity = entity;
    
        public bool Evaluate() => !_entity.IsTapped;
    }
}