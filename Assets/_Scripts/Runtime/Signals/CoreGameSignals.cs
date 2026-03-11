using System;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class CoreGameSignals : MonoSingleton<CoreGameSignals>, ICoreGameSignals
    {
        public event Action<int> OnLevelInitialize = delegate { };

        public event Action<LevelDataSO> OnLevelDataLoaded = delegate { };

        public event Action<LevelDataSO, int, int> OnGridReady = delegate { };
        public event Action OnPlay = delegate { };
        public event Action OnReset = delegate { };
        public event Action OnNextLevel = delegate { };
        public event Action OnRestartLevel = delegate { };
        public event Action OnLevelSuccessful = delegate { };
        public event Action OnLevelFailed = delegate { };


        public void FireOnLevelInitialize(int levelValue) => OnLevelInitialize?.Invoke(levelValue);

        public void FireOnLevelDataLoaded(LevelDataSO levelData) => OnLevelDataLoaded?.Invoke(levelData);

        public void FireOnGridReady(LevelDataSO levelData, int x, int z) =>
            OnGridReady?.Invoke(levelData, x, z);

        public void FireOnPlay() => OnPlay.Invoke();

        public void FireOnReset() => OnReset.Invoke();

        public void FireOnNextLevel() => OnNextLevel.Invoke();

        public void FireOnRestartLevel() => OnRestartLevel.Invoke();

        public void FireOnLevelSuccessful() => OnLevelSuccessful.Invoke();

        public void FireOnLevelFailed() => OnLevelFailed.Invoke();
    }
}