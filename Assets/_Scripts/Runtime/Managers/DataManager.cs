using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class DataManager : MonoBehaviour
    {
        private void OnEnable()
        {
            SaveSignals.Instance.OnSaveLevel += SaveLevel;
            SaveSignals.Instance.OnGetLevelId += GetLevelId;
            SaveSignals.Instance.OnGetLevelData += GetLevelData;
        }

        private int GetLevelId() => PlayerPrefs.GetInt(ConstantUtil.LevelKey, 1);

        private void SaveLevel(int newLevelId)
        {
            PlayerPrefs.SetInt(ConstantUtil.LevelKey, newLevelId);
            PlayerPrefs.Save();
        }

        private LevelDataSO GetLevelData() =>
            Resources.Load<LevelDataSO>($"{ConstantUtil.LevelPath}{GetLevelId()}");

        private void OnDisable()
        {
            if (!CoreGameSignals.IsAvailable) return;
            SaveSignals.Instance.OnSaveLevel -= SaveLevel;
            SaveSignals.Instance.OnGetLevelId -= GetLevelId;
            SaveSignals.Instance.OnGetLevelData -= GetLevelData;
        }
    }
}