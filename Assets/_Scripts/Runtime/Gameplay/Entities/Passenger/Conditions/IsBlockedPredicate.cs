using _Scripts.Core;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.Conditions
{
    public class IsBlockedPredicate : IPredicate
    {
        private readonly PassengerEntity _entity;

        public IsBlockedPredicate(PassengerEntity entity)
        {
            _entity = entity;
        }

        public bool Evaluate()
        {
            return _entity.IsTapped && !_entity.IsMoveable;
        }
    }
}