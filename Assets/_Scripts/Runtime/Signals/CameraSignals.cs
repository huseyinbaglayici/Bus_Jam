using System;
using UnityEngine;
using _Scripts.Runtime.Extensions;

namespace _Scripts.Runtime.Signals
{
    public class CameraSignals : MonoSingleton<CameraSignals>
    {
        public event Action<Vector3> OnSetCameraPosition = delegate { };
        public event Action<int, int> OnSetCameraZoom = delegate { };

        public void FireOnSetCameraPosition(Vector3 position) => OnSetCameraPosition.Invoke(position);
        public void FireOnSetCameraZoom(int x, int y) => OnSetCameraZoom.Invoke(x, y);
    }
}