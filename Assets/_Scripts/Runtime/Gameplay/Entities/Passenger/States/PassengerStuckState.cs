using _Scripts.Core;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.Entities.Passenger.States
{
    public class PassengerStuckState : IState
    {
        private readonly PassengerEntity _entity;
        private readonly Transform _transform;
        private readonly SpriteRenderer _angryEmoji;
        private Vector3 _originalPosition;

        private const float StuckDuration = .7f;
        private const float ShakeDuration = .6f;

        private float _stuckTimer;
        private Tween _shakeTween;

        public PassengerStuckState(PassengerEntity entity, Transform transform, SpriteRenderer angryEmoji)
        {
            _entity = entity;
            _transform = transform;
            _angryEmoji = angryEmoji;
        }

        public void OnEnter()
        {
            _stuckTimer = 0f;
            _originalPosition = _transform.position;
            _angryEmoji.gameObject.SetActive(true);

            _shakeTween?.Kill(true);
            _shakeTween = _transform
                .DOLocalMoveX(0.15f, 0.12f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
        }

        public void Update()
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= StuckDuration)
                _entity.IsTapped = false;
        }

        public void OnExit()
        {
            _shakeTween?.Kill();
            _transform.position = _originalPosition;
            _shakeTween = null;
            _angryEmoji.gameObject.SetActive(false);
        }
    }
}