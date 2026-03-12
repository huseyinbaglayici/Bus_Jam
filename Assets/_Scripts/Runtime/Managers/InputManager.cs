using _Scripts.Runtime.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Runtime.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private bool isAvailableForTouch = true;
        [SerializeField] private bool isFirstTouchTaken;

        private Camera _mainCamera;
        private Plane _groundPlane;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _groundPlane = new Plane(Vector3.up, Vector3.zero);
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnPlay += EnableTouch;
            CoreGameSignals.Instance.OnLevelFailed += DisableTouch;
            CoreGameSignals.Instance.OnLevelSuccessful += DisableTouch;
            CoreGameSignals.Instance.OnReset += ResetInput;
            CoreGameSignals.Instance.OnRestartLevel += ResetInput;
            InputSignals.Instance.OnFirstTouchTaken += OnFirstTouchTaken;
        }

        private void Update()
        {
            if (!isAvailableForTouch) return;
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsPointerOverUI()) return;

            if (!isFirstTouchTaken)
            {
                InputSignals.Instance.FireOnFirstTouchTaken();
                CoreGameSignals.Instance.FireOnPlay();
            }

            InputSignals.Instance.FireOnInputTaken(GetWorldPositionFromMouse());
        }

        private void EnableTouch() => isAvailableForTouch = true;
        private void DisableTouch() => isAvailableForTouch = false;
        private void OnFirstTouchTaken() => isFirstTouchTaken = true;

        private void ResetInput()
        {
            isAvailableForTouch = true;
            isFirstTouchTaken = false;
        }

        private Vector3 GetWorldPositionFromMouse()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            return _groundPlane.Raycast(ray, out float distance) ? ray.GetPoint(distance) : Vector3.zero;
        }

        private bool IsPointerOverUI() =>
            EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnPlay -= EnableTouch;
            CoreGameSignals.Instance.OnLevelFailed -= DisableTouch;
            CoreGameSignals.Instance.OnLevelSuccessful -= DisableTouch;
            CoreGameSignals.Instance.OnReset -= ResetInput;
            CoreGameSignals.Instance.OnRestartLevel -= ResetInput;
            InputSignals.Instance.OnFirstTouchTaken -= OnFirstTouchTaken;
        }
    }
}