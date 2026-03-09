using System;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ICameraSignals
    {
        public event Action<UnityEngine.Vector3 > OnSetCameraPosition; 
    }
}