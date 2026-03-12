using System;
using UnityEngine;
using _Scripts.Runtime.Extensions;

namespace _Scripts.Runtime.Signals
{
    public class ActiveLevelSignals : MonoSingleton<ActiveLevelSignals>
    {
        public event Func<int> OnGetBusCount = () => 0;
        public event Func<int> OnGetPassengerLineCount = () => 0;
        public event Func<int> OnGetLevelTime = () => 0;
        public event Action<int> OnSetLevelTime = delegate { };
        public event Func<Vector3> OnGetCenterOfActiveGrid = () => Vector3.zero;

        public int FireOnGetBusCount() => OnGetBusCount.Invoke();
        public int FireOnGetPassengerLineCount() => OnGetPassengerLineCount.Invoke();
        public int FireOnGetLevelTime() => OnGetLevelTime.Invoke();
        public void FireOnSetLevelTime(int time) => OnSetLevelTime.Invoke(time);
        public Vector3 FireOnGetCenterOfActiveGrid() => OnGetCenterOfActiveGrid.Invoke();
    }
}