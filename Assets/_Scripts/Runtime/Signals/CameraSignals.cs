using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class CameraSignals : MonoSingleton<CameraSignals>, ICameraSignals
    {
        public event Action<UnityEngine.Vector3> OnSetCameraPosition;
        public event Action<int, int> OnSetCameraZoom;

        public event Action<int, int> OnSetCamera;


        public void FireOnSetCameraPosition(UnityEngine.Vector3 position) => OnSetCameraPosition?.Invoke(position);
        public void FireOnSetCameraZoom(int x, int y) => OnSetCameraZoom?.Invoke(x, y);

        public void FireOnsetCamera(int x, int y) => OnSetCamera?.Invoke(x, y);
    }
}