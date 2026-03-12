using System;
using System.Collections.Generic;
using _Scripts.Runtime.Data.ValueObjects;
using _Scripts.Runtime.Extensions;

namespace _Scripts.Runtime.Signals
{
    public class GridSignals : MonoSingleton<GridSignals>
    {
        public event Func<int, int, GridNode> OnGetNode = (_, _) => null;
        public event Func<int, int, List<GridNode>> OnCalculatePathToExit = (_, _) => null;
        public event Action<int, int> OnFreeNode = delegate { };
        public event Action<int, int> OnPassengerSelected = delegate { };

        public GridNode FireOnGetNode(int x, int y) => OnGetNode.Invoke(x, y);
        public List<GridNode> FireOnCalculatePathToExit(int x, int y) => OnCalculatePathToExit.Invoke(x, y);
        public void FireOnFreeNode(int x, int y) => OnFreeNode.Invoke(x, y);
        public void FireOnPassengerSelected(int x, int y) => OnPassengerSelected.Invoke(x, y);
    }
}