using System;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IInputSignals
    {
        public event Action OnFirstTouchTaken;
        public event Action<UnityEngine.Vector3> OnInputTaken;
    }
}