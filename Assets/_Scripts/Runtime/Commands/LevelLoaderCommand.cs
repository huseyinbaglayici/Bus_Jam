using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Interfaces;
using _Scripts.Runtime.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Scripts.Runtime.Commands
{
    public class LevelLoaderCommand : ICommandAsync<int>
    {
        private readonly LevelManager _levelManager;
        private const string LEVEL_PATH = "Data/SO_Level_Data/Level_";

        public LevelLoaderCommand(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public async UniTask ExecuteAsync(int levelIndexParam)
        {
            string path = $"{LEVEL_PATH}{levelIndexParam}";
            var resourcesRequest = Resources.LoadAsync<LevelDataSO>(path);
            await resourcesRequest;

            LevelDataSO loadedLevelData = resourcesRequest.asset as LevelDataSO;

            if (loadedLevelData != null)
            {
                _levelManager.currentLevelData = loadedLevelData;

                //TODO: Sync with gridManager/or Signals
            }

            else
            {
                Debug.LogError($"[LevelLoader] {path} COULDN'T LOAD");
            }
        }
    }
}