using System;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IUISignals
    {
        public event Action<int> OnSetLevelValue;
        public event Action<int> OnSetTimerValue;
        
    }
}