using System;
using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class GridSignals : MonoSingleton<GridSignals>, IGridSignals
    {
        public event Func<int, int, GridNode> OnGetNode;
        public event Func<int, int, List<GridNode>> OnCalculatePathToExit;
        public event Action<int, int> OnFreeNode;
        public event Action<int, int> OnPassengerSelected;

        public GridNode FireOnGetNode(int x, int y)
        {
            return OnGetNode?.Invoke(x, y);
        }

        public List<GridNode> FireOnCalculatePathToExit(int x, int y)
        {
            return OnCalculatePathToExit?.Invoke(x, y);
        }

        public void FireOnFreeNode(int x, int y) => OnFreeNode?.Invoke(x, y);
        public void FireOnPassengerSelected(int x, int y) => OnPassengerSelected?.Invoke(x, y);
    }
}