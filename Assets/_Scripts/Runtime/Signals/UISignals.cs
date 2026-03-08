using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class UISignals : MonoSingleton<UISignals>, IUISignals
    {
        public event Action<int> OnSetLevelValue = delegate { };
        public event Action<int> OnSetTimerValue = delegate { };

        public void FireOnSetLevelValue(int levelTextId) => OnSetLevelValue.Invoke(levelTextId);

        public void FireOnSetTimerValue(int timerCount) => OnSetTimerValue.Invoke(timerCount);
    }
}