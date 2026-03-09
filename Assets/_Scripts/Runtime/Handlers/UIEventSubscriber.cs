using _Scripts.Runtime.Enums;
using _Scripts.Runtime.Signals;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Runtime.Handlers
{
    public class UIEventSubscriber : MonoBehaviour
    {
        [SerializeField] private UIEventSubscriptionType type;
        [SerializeField] private Button button;


        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            switch (type)
            {
                case UIEventSubscriptionType.OnEnterLevel:
                    UISignals.Instance.FireOnSetLevelValue(SaveSignals.Instance.FireGetLevelId());
                    break;
                case UIEventSubscriptionType.OnPlay:
                    CoreGameSignals.Instance.FireOnPlay();
                    break;
                case UIEventSubscriptionType.OnNextLevel:
                    CoreGameSignals.Instance.FireOnNextLevel();
                    break;
                case UIEventSubscriptionType.OnRestartLevel:
                    CoreGameSignals.Instance.FireOnRestartLevel();
                    break;
            }
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}