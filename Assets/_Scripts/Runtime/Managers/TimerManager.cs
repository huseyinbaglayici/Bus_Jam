using System;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class TimerManager : MonoBehaviour
    {
        #region variables

        [SerializeField] private int countdown = 45;
        [SerializeField] private bool isCounting;

        #endregion


        private void OnEnable()
        {
            countdown = ActiveLevelSignals.Instance.FireOnGetLevelTime();
            SetTimerData(countdown);
            UISignals.Instance.FireOnSetTimerValue(countdown);
            InputSignals.Instance.OnFirstTouchTaken += CountDownStart;
            CoreGameSignals.Instance.OnLevelSuccessful += CountDownEnd;
            CoreGameSignals.Instance.OnLevelFailed += CountDownEnd;
            CoreGameSignals.Instance.OnReset += OnResetTimer;
        }

        private void SetTimerData(int data)
        {
            countdown = data;
        }

        private void OnResetTimer()
        {
            isCounting = false;
            countdown = ActiveLevelSignals.Instance.FireOnGetLevelTime();
            UISignals.Instance.FireOnSetTimerValue(countdown);
        }

        private void CountDownStart()
        {
            if (isCounting)
                return;
            isCounting = true;
            RunCountDown().Forget();
        }

        private async UniTask RunCountDown()
        {
            while (isCounting && countdown > 0)
            {
                await UniTask.Delay(1000);
                if (!isCounting) break;
                var busCount = ActiveLevelSignals.Instance.FireOnGetBusCount();
                var passengerLineCount = ActiveLevelSignals.Instance.FireOnGetPassengerLineCount();
                if (busCount == 0)
                {
                    CoreGameSignals.Instance.FireOnLevelSuccessful();
                    CountDownEnd();
                    break;
                }

                if (passengerLineCount == 0)
                {
                    CoreGameSignals.Instance.FireOnLevelFailed();
                    CountDownEnd();
                }

                countdown--;
                UISignals.Instance.FireOnSetTimerValue(countdown);
            }

            if (countdown == 0)
            {
                isCounting = false;
                CoreGameSignals.Instance.FireOnLevelFailed();
            }
        }

        private void CountDownEnd() => isCounting = false;

        private void OnInitTimer()
        {
            if (isCounting) return;
            isCounting = true;
            RunCountDown().Forget();
        }

        private void OnDisable()
        {
            InputSignals.Instance.OnFirstTouchTaken -= CountDownStart;
            CoreGameSignals.Instance.OnLevelSuccessful -= CountDownEnd;
            CoreGameSignals.Instance.OnLevelFailed -= CountDownEnd;
            CoreGameSignals.Instance.OnReset -= OnInitTimer;
        }
    }
}