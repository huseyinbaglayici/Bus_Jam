using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class CameraManager : MonoBehaviour
    {
        private Vector3 _initPos = Vector3.zero;
        [SerializeField] private Vector3 offset;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera = null;

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

        private void SetCameraPosition(Vector3 position)
        {
            var z = transform.position.z;
            _virtualCamera.transform.position = new Vector3(position.x + offset.x,
                position.y + offset.y,
                position.z + offset.z);
        }

        private void OnDisable() => CameraSignals.Instance.OnSetCameraPosition -= SetCameraPosition;
    }
}