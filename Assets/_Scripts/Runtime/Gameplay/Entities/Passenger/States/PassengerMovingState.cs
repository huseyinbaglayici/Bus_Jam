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
        private const float WaypointReachThreshold = 0.1f;
        private static readonly int IsMovingHash = Animator.StringToHash("isMoving");

        private Vector3 _currentWaypoint;
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
                _currentWaypoint = _entity.PathPoints.Dequeue();
            else
                _isMovementComplete = true;
        }

        public void Update()
        {
            if (_isMovementComplete) return;
            ProcessMovement();
        }

        public void OnExit() => _animator.SetBool(IsMovingHash, false);

        private void ProcessMovement()
        {
            _transform.position = Vector3.MoveTowards(
                _transform.position, _currentWaypoint, Speed * Time.deltaTime);

            Vector3 direction = (_currentWaypoint - _transform.position).normalized;
            if (direction != Vector3.zero)
            {
                _transform.rotation = Quaternion.Slerp(
                    _transform.rotation,
                    Quaternion.LookRotation(direction),
                    RotationSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(_transform.position, _currentWaypoint) <= WaypointReachThreshold)
                AdvanceWaypoint();
        }

        private void AdvanceWaypoint()
        {
            if (_entity.PathPoints.Count > 0)
            {
                _currentWaypoint = _entity.PathPoints.Dequeue();
                return;
            }

            _isMovementComplete = true;
            OnDestinationReached();
        }

        private void OnDestinationReached()
        {
            _entity.PathPoints.Clear();

            switch (_entity.CurrentTarget)
            {
                case PassengerTargetType.Bus:
                    BusSignals.Instance.FireOnPassengerBoardedBus();
                    _transform.gameObject.SetActive(false);
                    break;
                case PassengerTargetType.Line:
                    _entity.CurrentTarget = PassengerTargetType.None;
                    _transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
        }
    }
}