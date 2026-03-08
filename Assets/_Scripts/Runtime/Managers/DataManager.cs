using System;
using _Scripts.Runtime.Signals;
using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class DataManager : MonoBehaviour
    {
        private const string LEVEL_KEY = "CurrentLevelId";

        private void OnEnable()
        {
            SaveSignals.Instance.OnSaveLevel += SaveLevel;
            SaveSignals.Instance.OnGetLevelId += GetLevelId;
        }

        public int GetLevelId()
        {
            return PlayerPrefs.GetInt(LEVEL_KEY, 0);
        }

        public void SaveLevel(int newLevelId)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, newLevelId);
            PlayerPrefs.Save();
        }


        private void OnDisable()
        {
            SaveSignals.Instance.OnSaveLevel -= SaveLevel;
            SaveSignals.Instance.OnGetLevelId -= GetLevelId;
        }
    }
}