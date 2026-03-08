using System;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ICoreGameSignals
    {
        public event Action<int> OnLevelInitialize;
        public event Action OnPlay;
        public event Action OnReset;
        public event Action OnNextLevel;
        public event Action OnRestartLevel;
        public event Action OnLevelSuccessful;
        public event Action OnLevelFailed;
    }
}