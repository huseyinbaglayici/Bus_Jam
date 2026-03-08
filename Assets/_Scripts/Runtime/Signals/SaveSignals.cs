using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class SaveSignals : MonoSingleton<SaveSignals>, ISaveSignals
    {
        public event Action<int> OnSaveLevel = delegate { };
        public event Func<int> OnGetLevelId = () => 0;

        public void FireSaveLevel(int level) => OnSaveLevel.Invoke(level);

        public int FireGetLevelId() => OnGetLevelId.Invoke();
    }
}