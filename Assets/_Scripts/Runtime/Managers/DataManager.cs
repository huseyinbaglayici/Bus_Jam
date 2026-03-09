using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class DataManager : MonoBehaviour
    {
        private const string LEVEL_KEY = "CurrentLevelId";
        private const string LEVEL_PATH = "Data/SO_Level_Data/Level_";

        private void OnEnable()
        {
            SaveSignals.Instance.OnSaveLevel += SaveLevel;
            SaveSignals.Instance.OnGetLevelId += GetLevelId;
            SaveSignals.Instance.OnGetLevelData += GetLevelData;
        }

        private int GetLevelId()
        {
            return PlayerPrefs.GetInt(LEVEL_KEY, 1);
        }

        private void SaveLevel(int newLevelId)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, newLevelId);
            PlayerPrefs.Save();
        }

        private LevelDataSO GetLevelData()
        {
            return Resources.Load<LevelDataSO>($"{LEVEL_PATH}{GetLevelId()}");
        }


        private void OnDisable()
        {
            SaveSignals.Instance.OnSaveLevel -= SaveLevel;
            SaveSignals.Instance.OnGetLevelId -= GetLevelId;
            SaveSignals.Instance.OnGetLevelData -= GetLevelData;
        }
    }
}