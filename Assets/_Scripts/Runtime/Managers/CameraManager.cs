using _Scripts.Runtime.Signals;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class CameraManager : MonoBehaviour
    {
        private Vector3 _initPos = Vector3.zero;
        [SerializeField] private Vector3 offset;
        private CinemachineVirtualCamera _virtualCamera = null;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (_virtualCamera != null) return;
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        private void OnEnable() => CameraSignals.Instance.OnSetCameraPosition += SetCameraPosition;


        private void SetCameraPosition(Vector3 gridCenter)
        {
            _virtualCamera.transform.position = new Vector3(
                gridCenter.z + offset.x, 
                10, // todo:get y in variable
                gridCenter.x + offset.z 
            );

            
        }

        private void OnDisable() => CameraSignals.Instance.OnSetCameraPosition -= SetCameraPosition;
    }
}