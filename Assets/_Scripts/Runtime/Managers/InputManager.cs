using System.Collections.Generic;
using _Scripts.Runtime.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Runtime.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private bool _isAvailableForTouch = true;
        [SerializeField] private bool _isFirstTouchTaken;

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnPlay += EnableTouch;
            CoreGameSignals.Instance.OnLevelFailed += DisableInput;
            CoreGameSignals.Instance.OnLevelSuccessful += DisableInput;
            CoreGameSignals.Instance.OnReset += OnReset;
            CoreGameSignals.Instance.OnRestartLevel += OnRestartLevel;
            InputSignals.Instance.OnFirstTouchTaken += SetFirstTouchTaken;
        }

        private void OnRestartLevel()
        {
            OnReset();
        }

        private void EnableTouch() => _isAvailableForTouch = true;
        private void DisableInput() => _isAvailableForTouch = false;
        private void SetFirstTouchTaken() => _isFirstTouchTaken = true;

        private void OnReset()
        {
            _isAvailableForTouch = true;
            _isFirstTouchTaken = false;
        }


        private void Update()
        {
            if (!_isAvailableForTouch) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject()) return;

                if (!_isFirstTouchTaken)
                {
                    InputSignals.Instance.FireOnFirstTouchTaken();
                    CoreGameSignals.Instance.FireOnPlay();
                }

                InputSignals.Instance.FireOnInputTaken(Input.mousePosition);
            }
        }

        private bool IsPointerOverUIObject()
        {
            if (EventSystem.current == null) return false;
            return EventSystem.current.IsPointerOverGameObject();
        }


        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.OnPlay -= EnableTouch;
            CoreGameSignals.Instance.OnLevelFailed -= DisableInput;
            CoreGameSignals.Instance.OnLevelSuccessful -= DisableInput;
            CoreGameSignals.Instance.OnReset -= OnReset;
            CoreGameSignals.Instance.OnRestartLevel -= OnRestartLevel;
            InputSignals.Instance.OnFirstTouchTaken -= SetFirstTouchTaken;
        }
    }
}