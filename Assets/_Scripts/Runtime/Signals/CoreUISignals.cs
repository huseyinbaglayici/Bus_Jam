using System;
using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals.Interfaces;

namespace _Scripts.Runtime.Signals
{
    public class CoreUISignals : MonoSingleton<CoreUISignals>, ICoreUISignals
    {
        public event Action<UIPanelType, int> OnOpenPanel = delegate { };
        public event Action<int> OnClosePanel = delegate { };
        public event Action OnCloseAllPanels = delegate { };

        public event Action OnGameInitialize = delegate { };
        public event Action OnGameStart = delegate { };
        public event Action OnGameFailed = delegate { };
        public event Action OnGameSuccesfull = delegate { };

        public void FireOnOpenPanel(UIPanelType panelType, int panelID) => OnOpenPanel.Invoke(panelType, panelID);

        public void FireOnClosePanel(int panelID) => OnClosePanel.Invoke(panelID);

        public void FireOnCloseAllPanels() => OnCloseAllPanels.Invoke();

        public void FireOnGameInitialize() => OnGameInitialize.Invoke();
        public void FireOnGameStart() => OnGameStart.Invoke();
        public void FireOnGameFailed() => OnGameFailed.Invoke();
        public void FireOnGameSuccessful() => OnGameSuccesfull.Invoke();
    }
}