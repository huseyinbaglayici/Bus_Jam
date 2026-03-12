using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Interfaces;
using _Scripts.Runtime.Managers;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Scripts.Runtime.Commands
{
    public class LevelLoaderCommand : ICommandAsync<int>
    {
        private readonly LevelManager _levelManager;

        public LevelLoaderCommand(LevelManager levelManager) => _levelManager = levelManager;

        public async UniTask ExecuteAsync(int levelIndex)
        {
            string path = $"{ConstantUtil.LevelPath}{levelIndex}";
            var request = Resources.LoadAsync<LevelDataSO>(path);
            await request;

            if (request.asset is not LevelDataSO levelData)
            {
                Debug.LogError($"[LevelLoader] {path} couldn't be loaded.");
                return;
            }

            _levelManager.currentLevelData = levelData;
            ActiveLevelSignals.Instance.FireOnSetLevelTime(levelData.Time);
            CoreGameSignals.Instance.FireOnLevelDataLoaded(levelData);
        }
    }
}