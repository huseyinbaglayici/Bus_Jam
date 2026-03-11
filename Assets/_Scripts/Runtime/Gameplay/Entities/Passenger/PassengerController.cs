using System.Collections.Generic;
using _Scripts.Core;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Gameplay.Entities.Passenger.Conditions;
using _Scripts.Runtime.Gameplay.Entities.Passenger.States;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger
{
    public class PassengerController : MonoBehaviour
    {
        private StateMachine _stateMachine;
        public PassengerEntity Entity { get; private set; }

        [SerializeField] private EntityColor startingColor;
        [SerializeField] private Vector2Int startingPosition;
        [SerializeField] private Animator animator;
        [SerializeField] private Renderer charModelRenderer;
        [SerializeField] private GameObject charModel;

        private PassengerIdleState _idleState;
        private PassengerMovingState _movingState;
        private PassengerStuckState _stuckState;

        public void Initialize(Material material, EntityColor entityColor, Vector2Int position)
        {
            if (charModelRenderer)
            {
                charModelRenderer.material = material;
            }

            startingColor = entityColor;
            startingPosition = position;
            Entity = new PassengerEntity(startingColor, startingPosition);
            InitStateMachine();
        }


        private void InitStateMachine()
        {
            _stateMachine = new StateMachine();

            _idleState = new PassengerIdleState(Entity);
            _movingState = new PassengerMovingState(Entity, charModel.transform, animator);
            _stuckState = new PassengerStuckState(Entity);

            var hasPathCondition = new HasPathPredicate(Entity);
            var isBlockedCondition = new IsBlockedPredicate(Entity);

            _stateMachine.AddTransition(_idleState, _movingState, hasPathCondition);
            _stateMachine.AddTransition(_idleState, _stuckState, isBlockedCondition);

            _stateMachine.SetState(_idleState);
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        public void RedirectToBus(Vector3 busPosition)
        {
            Entity.CurrentTarget = PassengerTargetType.Bus;
            Entity.PathPoints.Clear();
            Entity.SetPath(new List<Vector3> { busPosition });

            _stateMachine.SetState(_movingState);
        }
    }
}