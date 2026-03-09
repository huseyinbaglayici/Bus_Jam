using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.States
{
    public class PassengerMovingState : IState
    {
        private readonly PassengerEntity _entity;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private const float Speed = 5f;

        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");


        public PassengerMovingState(PassengerEntity entity, Transform transform, Animator animator)
        {
            _entity = entity;
            _transform = transform;
            _animator = animator;
        }

        public void OnEnter()
        {
            _entity.IsTapped = false;
            _animator.SetBool(IsMovingHash, true);
            if (_entity.CurrentTarget == PassengerTargetType.Bus)
            {
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
            _animator.SetBool(IsMovingHash, false);
        }
    }
}