using System;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Extensions;

namespace _Scripts.Runtime.Signals
{
    public class SaveSignals : MonoSingleton<SaveSignals>
    {
        public event Action<int> OnSaveLevel = delegate { };
        public event Func<int> OnGetLevelId = () => 1;
        public event Func<LevelDataSO> OnGetLevelData = () => null;

        public void FireSaveLevel(int level) => OnSaveLevel.Invoke(level);
        public int FireGetLevelId() => OnGetLevelId.Invoke();
        public LevelDataSO FireOnGetLevelData() => OnGetLevelData.Invoke();
    }
}