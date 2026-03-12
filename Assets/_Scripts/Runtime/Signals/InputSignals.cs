using System;
using UnityEngine;
using _Scripts.Runtime.Extensions;

namespace _Scripts.Runtime.Signals
{
    public class InputSignals : MonoSingleton<InputSignals>
    {
        public event Action OnFirstTouchTaken = delegate { };
        public event Action<Vector3> OnInputTaken = delegate { };

        public void FireOnFirstTouchTaken() => OnFirstTouchTaken.Invoke();
        public void FireOnInputTaken(Vector3 point) => OnInputTaken.Invoke(point);
    }
}