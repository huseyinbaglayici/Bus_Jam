using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals;

namespace _Scripts.Runtime.Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        protected new void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

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
        }

        private void OnLevelInitialize(int levelValue)
        {
            UISignals.Instance.FireOnSetLevelValue(levelValue);
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.MainMenu, levelValue);
        }

        private void OnPlay()
        {
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Level, 0);
        }

        private void OnLevelSuccessful()
        {
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Win, 1);
        }

        private void OnLevelFailed()
        {
            CoreUISignals.Instance.FireOnOpenPanel(UIPanelType.Fail, 1);
        }


        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= OnLevelInitialize;
            CoreGameSignals.Instance.OnLevelFailed -= OnLevelFailed;
            CoreGameSignals.Instance.OnLevelSuccessful -= OnLevelSuccessful;
            CoreGameSignals.Instance.OnPlay -= OnPlay;
        }
    }
}