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

        public void FireOnOpenPanel(UIPanelType panelType, int panelID) => OnOpenPanel.Invoke(panelType, panelID);

        public void FireOnClosePanel(int panelID) => OnClosePanel.Invoke(panelID);

        public void FireOnCloseAllPanels() => OnCloseAllPanels.Invoke();
    }
}