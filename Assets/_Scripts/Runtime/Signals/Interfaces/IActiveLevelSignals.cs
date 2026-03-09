using System;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IActiveLevelSignals
    {
        public event Func<int> OnGetBusCount;
        public event Func<int> OnGetPassengerLineCount;
        public event Func<int> OnGetLevelTime;
        public event Func<UnityEngine.Vector3> OnGetCenterOfActiveGrid;
        public event Action<int> OnSetLevelTime;
    }
}