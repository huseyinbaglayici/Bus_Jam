using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class InputSignals : MonoSingleton<InputSignals>, IInputSignals
    {
        public event Action OnFirstTouchTaken = delegate { };
        public event Action<UnityEngine.Vector3> OnInputTaken = delegate { };

        public void FireOnFirstTouchTaken()
        {
            OnFirstTouchTaken.Invoke();
        }

        public void FireOnInputTaken(UnityEngine.Vector3 point)
        {
            OnInputTaken.Invoke(point);
        }
    }
}