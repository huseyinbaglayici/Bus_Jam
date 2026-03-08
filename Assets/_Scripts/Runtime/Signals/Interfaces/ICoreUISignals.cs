using System;
using _Scripts.Runtime.Enums;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ICoreUISignals
    {
        public event Action<UIPanelType, int> OnOpenPanel;
        public event Action<int> OnClosePanel;
        public event Action OnCloseAllPanels;

    }
}