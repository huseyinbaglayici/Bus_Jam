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

        private int GetLevelId()
        {
            return PlayerPrefs.GetInt(ConstantUtil.LEVEL_KEY, 1);
        }

        private void SaveLevel(int newLevelId)
        {
            PlayerPrefs.SetInt(ConstantUtil.LEVEL_KEY, newLevelId);
            PlayerPrefs.Save();
        }

        private LevelDataSO GetLevelData()
        {
            return Resources.Load<LevelDataSO>($"{ConstantUtil.LEVEL_PATH}{GetLevelId()}");
        }


        private void OnDisable()
        {
            SaveSignals.Instance.OnSaveLevel -= SaveLevel;
            SaveSignals.Instance.OnGetLevelId -= GetLevelId;
            SaveSignals.Instance.OnGetLevelData -= GetLevelData;
        }
    }
}