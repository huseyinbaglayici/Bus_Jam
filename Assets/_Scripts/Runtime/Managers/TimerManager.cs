using _Scripts.Runtime.Signals;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class TimerManager : MonoBehaviour
    {
        [SerializeField] private int countdown;
        [SerializeField] private bool isCounting;

        private void OnEnable()
        {
            InputSignals.Instance.OnFirstTouchTaken += CountDownStart;
            CoreGameSignals.Instance.OnLevelSuccessful += CountDownEnd;
            CoreGameSignals.Instance.OnLevelFailed += CountDownEnd;
            CoreGameSignals.Instance.OnReset += OnResetTimer;

            ResetCountdown();
        }

        private void ResetCountdown()
        {
            isCounting = false;
            countdown = ActiveLevelSignals.Instance.FireOnGetLevelTime();
            UISignals.Instance.FireOnSetTimerValue(countdown);
        }

        private void OnResetTimer() => ResetCountdown();

        private void CountDownStart()
        {
            if (isCounting) return;
            isCounting = true;
            RunCountDown().Forget();
        }

        private void CountDownEnd() => isCounting = false;

        private async UniTask RunCountDown()
        {
            while (isCounting && countdown > 0)
            {
                await UniTask.Delay(1000);
                if (!isCounting) break;

                if (ActiveLevelSignals.Instance.FireOnGetBusCount() == 0)
                {
                    CoreGameSignals.Instance.FireOnLevelSuccessful();
                    CountDownEnd();
                    break;
                }

                countdown--;
                UISignals.Instance.FireOnSetTimerValue(countdown);
            }

            if (countdown <= 0 && isCounting)
            {
                isCounting = false;
                CoreGameSignals.Instance.FireOnLevelFailed();
            }
        }

        private void OnDisable()
        {
            if (!CoreGameSignals.IsAvailable) return;
            InputSignals.Instance.OnFirstTouchTaken -= CountDownStart;
            CoreGameSignals.Instance.OnLevelSuccessful -= CountDownEnd;
            CoreGameSignals.Instance.OnLevelFailed -= CountDownEnd;
            CoreGameSignals.Instance.OnReset -= OnResetTimer;
        }
    }
}