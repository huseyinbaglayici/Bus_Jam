using _Scripts.Core;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.States
{
    public class PassengerIdleState : IState
    {
        private readonly PassengerEntity _entity;
        
        public PassengerIdleState(PassengerEntity entity)
        {
            _entity = entity;
        }

        public void OnEnter()
        {
        }

        public void Update()
        {
        }

        public void OnExit()
        {
        }
    }
}