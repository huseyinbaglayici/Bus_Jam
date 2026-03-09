using System;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class SaveSignals : MonoSingleton<SaveSignals>, ISaveSignals
    {
        public event Action<int> OnSaveLevel = delegate { };
        public event Func<int> OnGetLevelId = () => 1;

        public event Func<LevelDataSO> OnGetLevelData =  () => null;

        public void FireSaveLevel(int level) => OnSaveLevel.Invoke(level);

        public LevelDataSO FireOnGetLevelData() => OnGetLevelData?.Invoke();

        public int FireGetLevelId() => OnGetLevelId.Invoke();
    }
}