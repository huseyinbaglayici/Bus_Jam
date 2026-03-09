using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class UIManager : MonoBehaviour
    {
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void Start() =>
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.MainMenu, 0);


        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize += OnLevelInitialize;
            CoreGameSignals.Instance.OnLevelFailed += OnLevelFailed;
            CoreGameSignals.Instance.OnLevelSuccessful += OnLevelSuccessful;
            CoreGameSignals.Instance.OnPlay += OnPlay;
            CoreGameSignals.Instance.OnRestartLevel += OnRestartLevel;
        }

        private void OnLevelInitialize(int levelValue)
        {
            CoreUISignals.Instance.FireOnCloseAllPanels();
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.MainMenu, 0);
            UISignals.Instance.FireOnSetLevelValue(levelValue);
        }

        private void OnPlay()
        {
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Level, 0);
            UISignals.Instance.FireOnSetLevelValue(SaveSignals.Instance.FireGetLevelId());
        }

        private void OnLevelSuccessful()
        {
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Win, 1);
        }

        private void OnLevelFailed()
        {
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Fail, 1);
        }

        private void OnRestartLevel()
        {
            CoreUISignals.Instance.FireOnCloseAllPanels();
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Level, 0);
        }


        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= OnLevelInitialize;
            CoreGameSignals.Instance.OnPlay -= OnPlay;
            CoreGameSignals.Instance.OnLevelFailed -= OnLevelFailed;
            CoreGameSignals.Instance.OnLevelSuccessful -= OnLevelSuccessful;
            CoreGameSignals.Instance.OnRestartLevel -= OnRestartLevel;
        }
    }
}