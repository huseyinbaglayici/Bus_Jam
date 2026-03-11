using System.Collections.Generic;
using _Scripts.Core;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.States
{
    public class PassengerMovingState : IState
    {
        private readonly PassengerEntity _entity;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private const float Speed = 10f;
        private const float RotationSpeed = 20f;

        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");

        private Vector3 _currentWayPoint;
        private bool _isMovementComplete;


        public PassengerMovingState(PassengerEntity entity, Transform transform, Animator animator)
        {
            _entity = entity;
            _transform = transform;
            _animator = animator;
        }

        public void OnEnter()
        {
            _entity.IsTapped = false;
            _isMovementComplete = false;
            _animator.SetBool(IsMovingHash, true);
            if (_entity.PathPoints.Count > 0)
                _currentWayPoint = _entity.PathPoints.Dequeue();
            else
                _isMovementComplete = true;
        }

        public void Update()
        {
            if (_isMovementComplete) return;

            ProcessMovementAndRotation();
        }

        private void ProcessMovementAndRotation()
        {
            _transform.position = Vector3.MoveTowards(_transform.position, _currentWayPoint, Speed * Time.deltaTime);

            Vector3 direction = (_currentWayPoint - _transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _transform.rotation =
                    Quaternion.Slerp(_transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(_transform.position, _currentWayPoint) <= 0.1f)
            {
                GetNextWaypoint();
            }
        }

        private void GetNextWaypoint()
        {
            if (_entity.PathPoints.Count > 0)
            {
                _currentWayPoint = _entity.PathPoints.Dequeue();
            }
            else
            {
                _isMovementComplete = true;
                HandleDestinationReached();
            }
        }

        private void HandleDestinationReached()
        {
            _entity.PathPoints.Clear();
            if (_entity.CurrentTarget == PassengerTargetType.Bus)
            {
                BusSignals.Instance.FireOnPassengerBoardedBus();
                _transform.gameObject.SetActive(false);
            }
            else if (_entity.CurrentTarget == PassengerTargetType.Line)
            {
                _entity.CurrentTarget = PassengerTargetType.None;
                _transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        public void OnExit()
        {
            _animator.SetBool(IsMovingHash, false);
        }
    }
}