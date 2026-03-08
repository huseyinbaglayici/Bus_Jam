using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.States
{
    public class PassengerMovingState : IState
    {
        private readonly PassengerEntity _entity;
        private readonly Transform _transform;
        private const float Speed = 5f;


        public PassengerMovingState(PassengerEntity entity, Transform transform)
        {
            _entity = entity;
            _transform = transform;
        }

        public void OnEnter()
        {
            _entity.IsTapped = false;
            if (_entity.CurrentTarget == PassengerTargetType.Bus)
            {
                // bus walk logic
            }

            else if (_entity.CurrentTarget == PassengerTargetType.Line)
            {
                // line walk logic
            }
        }

        public void Update()
        {
            _transform.position =
                Vector3.MoveTowards(_transform.position,
                    _entity.TargetWorldPosition,
                    Speed * Time.deltaTime);
        }

        public void OnExit()
        {
        }
    }
}