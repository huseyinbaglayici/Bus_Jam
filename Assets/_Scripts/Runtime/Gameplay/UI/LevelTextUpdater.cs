using System;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals;
using TMPro;
using UnityEngine;

namespace _Scripts.Runtime.Gameplay.UI
{
    public class LevelTextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;

        private void OnEnable()
        {
            UISignals.Instance.OnSetLevelValue += OnSetLevelValue;
            OnSetLevelValue(SaveSignals.Instance.FireGetLevelId());
        }

        private void OnSetLevelValue(int levelValue)
        {
            levelText.text = $"LEVEL {levelValue}";
        }

        private void OnDisable()
        {
            if (!ApplicationState.IsQuitting)
                UISignals.Instance.OnSetLevelValue -= OnSetLevelValue;
        }
    }
}