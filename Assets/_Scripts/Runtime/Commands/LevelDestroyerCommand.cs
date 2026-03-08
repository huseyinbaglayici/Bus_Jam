using _Scripts.Runtime.Interfaces;
using _Scripts.Runtime.Managers;
using UnityEngine;

namespace _Scripts.Runtime.Commands
{
    public class LevelDestroyerCommand : ICommand
    {
        private readonly LevelManager _levelManager;

        public LevelDestroyerCommand(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        public void Execute()
        {
            if (_levelManager.levelHolder != null)
            {
                foreach (Transform child in _levelManager.levelHolder)
                {
                    Object.Destroy(child.gameObject);
                }
            }

            if (_levelManager.currentLevelData != null)
            {
                Resources.UnloadAsset(_levelManager.currentLevelData);
                _levelManager.currentLevelData = null;
            }
        }
    }
}