using _Scripts.Core;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Gameplay.Entities.Passenger.Conditions;
using _Scripts.Runtime.Gameplay.Entities.Passenger.States;
using _Scripts.Runtime.Signals;
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


        public void SetMaterial(Material material)
        {
            if(!charModelRenderer) return;
            charModelRenderer.material = material;
        }
        private void Awake()
        {
            Entity = new PassengerEntity(startingColor, startingPosition);
            InitStateMachine();
        }

        private void InitStateMachine()
        {
            _stateMachine = new StateMachine();

            var idleState = new PassengerIdleState(Entity);
            var movingState = new PassengerMovingState(Entity, transform, animator);
            var stuckState = new PassengerStuckState(Entity);

            var hasPathCondition = new HasPathPredicate(Entity);
            var isBlockedCondition = new IsBlockedPredicate(Entity);

            _stateMachine.AddTransition(idleState, movingState, hasPathCondition);
            _stateMachine.AddTransition(idleState, stuckState, isBlockedCondition);

            _stateMachine.SetState(idleState);
        }

        private void Update()
        {
            _stateMachine.Update();
        }
    }
}