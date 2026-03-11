using System;
using _Scripts.Runtime.Data.UnityObjects;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ICoreGameSignals
    {
        public event Action<int> OnLevelInitialize;
        
        public event Action<LevelDataSO> OnLevelDataLoaded;
        public event Action<LevelDataSO,int,int> OnGridReady;
        public event Action OnPlay;
        public event Action OnReset;
        public event Action OnNextLevel;
        public event Action OnRestartLevel;
        public event Action OnLevelSuccessful;
        public event Action OnLevelFailed;
    }
}