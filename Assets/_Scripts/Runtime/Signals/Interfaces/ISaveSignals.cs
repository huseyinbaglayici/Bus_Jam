using System;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ISaveSignals
    {
        public event Action<int> OnSaveLevel;
        public event Func<int> OnGetLevelId;
    }
}