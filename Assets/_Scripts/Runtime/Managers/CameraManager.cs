using _Scripts.Runtime.Signals;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        private CinemachineVirtualCamera _virtualCamera;

        private void Awake() => _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        private void OnEnable() => CameraSignals.Instance.OnSetCameraPosition += SetCameraPosition;

        private void SetCameraPosition(Vector3 gridCenter)
        {
            float adjustedY = 10f;
            float adjustedZ = gridCenter.x + offset.z;

            if (gridCenter.x > 6f)
            {
                float excess = gridCenter.x - 6f;
                adjustedY += excess * 2.5f;
                adjustedZ += excess * 1.5f;
            }

            _virtualCamera.transform.position = new Vector3(
                gridCenter.z + offset.x,
                adjustedY,
                adjustedZ
            );
        }

        private void OnDisable() => CameraSignals.Instance.OnSetCameraPosition -= SetCameraPosition;
    }
}