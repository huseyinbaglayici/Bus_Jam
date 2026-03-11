using _Scripts.Core;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.Conditions
{
    public class HasPathPredicate : IPredicate
    {
        private readonly PassengerEntity _entity;

        public HasPathPredicate(PassengerEntity entity)
        {
            _entity = entity;
        }

        public bool Evaluate()
        {
            return _entity.HasPath();
        }
    }
}