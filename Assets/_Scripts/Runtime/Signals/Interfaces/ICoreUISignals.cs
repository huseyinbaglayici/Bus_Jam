using System;
using _Scripts.Runtime.Enums;

namespace _Scripts.Runtime.Signals.Interfaces
{
    public interface ICoreUISignals
    {
        public event Action<UIPanelType, int> OnOpenPanel;
        public event Action<int> OnClosePanel;
        public event Action OnCloseAllPanels;
        public event Action OnGameInitialize;
        public event Action OnGameStart;
        public event Action OnGameFailed;
        public event Action OnGameSuccesfull;
    }
}