using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class CameraSignals : MonoSingleton<CameraSignals>, ICameraSignals
    {
        public event Action<UnityEngine.Vector3> OnSetCameraPosition;


        public void FireOnSetCameraPosition(UnityEngine.Vector3 position) => OnSetCameraPosition.Invoke(position);
    }
}