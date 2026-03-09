using System;
using _Scripts.Runtime.Data.UnityObjects;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ISaveSignals
    {
        public event Action<int> OnSaveLevel;
        public event Func<int> OnGetLevelId;

        public event Func<LevelDataSO> OnGetLevelData;
    }
}