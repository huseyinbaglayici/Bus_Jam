using System;
using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface IGridSignals
    {
        public event Func<int, int, GridNode> OnGetNode;
        public event Func<int, int, List<GridNode>> OnCalculatePathToExit;

        public event Action<int, int> OnFreeNode;
        public event Action<int, int> OnPassengerSelected;
    }
}