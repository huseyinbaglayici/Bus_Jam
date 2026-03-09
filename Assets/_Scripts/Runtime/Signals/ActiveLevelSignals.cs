using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class ActiveLevelSignals : MonoSingleton<ActiveLevelSignals>, IActiveLevelSignals
    {
        public event Func<int> OnGetBusCount = () => 0;
        public event Func<int> OnGetPassengerLineCount = () => 0;
        public event Action<int> OnSetLevelTime = delegate { };
        public event Func<int> OnGetLevelTime = () => 0;
        public event Func<UnityEngine.Vector3> OnGetCenterOfActiveGrid = () => UnityEngine.Vector3.zero;

        public int FireOnGetBusCount() => OnGetBusCount.Invoke();
        public int FireOnGetPassengerLineCount() => OnGetPassengerLineCount.Invoke();

        public int FireOnGetLevelTime() => OnGetLevelTime.Invoke();

        public void FireOnSetLevelTime(int level) => OnSetLevelTime.Invoke(level);

        public UnityEngine.Vector3 FireOnGetCenterOfActiveGrid() => UnityEngine.Vector3.zero;
    }
}