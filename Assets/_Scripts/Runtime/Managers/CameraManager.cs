using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        private CinemachineVirtualCamera _virtualCamera;
        private const float Multiplier = 1.5f;

        private void Awake() => _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        private void OnEnable()
        {
            CameraSignals.Instance.OnSetCameraPosition += SetCameraPosition;
            CameraSignals.Instance.OnSetCameraZoom += SetCameraZoom;
        }

        private void SetCameraZoom(int gridX, int gridZ)
        {
            float baseY = 10f;
            float xFactor = Mathf.Max(0, gridX - 5) * Multiplier;
            float zFactor = Mathf.Max(0, gridZ - 5) * Multiplier;

            _targetY = baseY + Mathf.Max(xFactor, zFactor);
        }

        private float _targetY = 2f;

        private void SetCameraPosition(Vector3 gridCenter)
        {
            float adjustedX = gridCenter.z + offset.x;
            float adjustedZ = gridCenter.x + offset.z;

            if (gridCenter.x > 6f)
            {
                float excess = gridCenter.x - 6f;
                adjustedZ += excess * Multiplier;
            }

            float zPullback = (_targetY - 10f) * .7f;

            _virtualCamera.transform.position = new Vector3(
                adjustedX,
                _targetY,
                adjustedZ - zPullback
            );
        }

        private void OnDisable()
        {
            if (!ApplicationState.IsQuitting)
            {
                CameraSignals.Instance.OnSetCameraPosition -= SetCameraPosition;
                CameraSignals.Instance.OnSetCameraZoom -= SetCameraZoom;
            }
        }
    }
}