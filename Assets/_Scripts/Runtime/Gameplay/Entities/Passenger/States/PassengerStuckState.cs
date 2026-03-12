using _Scripts.Core;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.States
{
    public class PassengerStuckState : IState
    {
        private readonly PassengerEntity _entity;
        private float _stuckTimer;
        private const float StuckDuration = 0.5f;

        public PassengerStuckState(PassengerEntity entity) => _entity = entity;

        public void OnEnter()
        {
            _entity.IsTapped = false;
            _stuckTimer = 0f;
        }

        public void Update() => _stuckTimer += Time.deltaTime;

        public void OnExit() { }
    }
}