using System;
using _Scripts.Runtime.Signals;
using TMPro;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.UI
{
    public class TimerTextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;


        private void OnEnable()
        {
            UISignals.Instance.OnSetTimerValue += OnSetTimerText;
            OnSetTimerText(ActiveLevelSignals.Instance.FireOnGetLevelTime());
        }

        private void OnSetTimerText(int timeCountdown)
        {
            timerText.text = timeCountdown.ToString();
        }

        private void OnDisable()
        {
            UISignals.Instance.OnSetTimerValue -= OnSetTimerText;
        }
    }
}