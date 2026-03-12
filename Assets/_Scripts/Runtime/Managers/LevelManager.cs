using System;
using _Scripts.Runtime.Commands;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Signals;
using _Scripts.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public LevelDataSO currentLevelData;

        private LevelLoaderCommand _levelLoaderCommand;
        private int _currentLevel;
        private bool _isLoading;

        private void Awake() => _levelLoaderCommand = new LevelLoaderCommand(this);

        private void OnEnable()
        {
            SubscribeEvents();
            _currentLevel = SaveSignals.Instance.FireGetLevelId();
            currentLevelData = SaveSignals.Instance.FireOnGetLevelData();
        }

        private int OnSendTimerData() => currentLevelData.Time;

        private void Start() => CoreGameSignals.Instance.FireOnLevelInitialize(_currentLevel);

        #region Event Subscriptions

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize += OnLevelInitializeWrapper;
            CoreGameSignals.Instance.OnNextLevel += OnNextLevelWrapper;
            CoreGameSignals.Instance.OnRestartLevel += OnRestartLevelWrapper;
            ActiveLevelSignals.Instance.OnGetLevelTime += OnSendTimerData;
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= OnLevelInitializeWrapper;
            CoreGameSignals.Instance.OnNextLevel -= OnNextLevelWrapper;
            CoreGameSignals.Instance.OnRestartLevel -= OnRestartLevelWrapper;
            ActiveLevelSignals.Instance.OnGetLevelTime -= OnSendTimerData;
        }

        // UniTaskVoid handler'lar Action delegate'e direkt bağlanamadığı için wrapper kullanıyoruz
        private void OnLevelInitializeWrapper(int level) => OnLevelInitializeHandler(level).Forget();
        private void OnNextLevelWrapper() => OnNextLevelHandler().Forget();
        private void OnRestartLevelWrapper() => OnRestartLevelHandler().Forget();

        #endregion

        #region Level Flow

        private async UniTaskVoid OnLevelInitializeHandler(int level)
        {
            if (_isLoading) return;
            _isLoading = true;
            await LoadGameplayScene(level);
            _isLoading = false;
        }


        private async UniTaskVoid OnNextLevelHandler()
        {
            if (_isLoading) return;
            _isLoading = true;
            _currentLevel++;
            SaveSignals.Instance.FireSaveLevel(_currentLevel);
            await UnloadGameplaySceneAsync();
            CoreGameSignals.Instance.FireOnReset();
            _isLoading = false;
            CoreGameSignals.Instance.FireOnLevelInitialize(_currentLevel);
        }

        private async UniTaskVoid OnRestartLevelHandler()
        {
            if (_isLoading) return;
            _isLoading = true;
            await UnloadGameplaySceneAsync();
            CoreGameSignals.Instance.FireOnReset();
            await LoadGameplayScene(_currentLevel);
            CoreGameSignals.Instance.FireOnPlay();
            _isLoading = false;
        }

        #endregion

        #region Scene Management

        private async UniTask LoadGameplayScene(int level)
        {
            try
            {
                await SceneManager.LoadSceneAsync(ConstantUtil.GameplaySceneName, LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

                Scene gameplayScene = SceneManager.GetSceneByName(ConstantUtil.GameplaySceneName);
                SceneManager.SetActiveScene(gameplayScene);

                await _levelLoaderCommand.ExecuteAsync(level);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask UnloadGameplaySceneAsync()
        {
            Scene gameplayScene = SceneManager.GetSceneByName(ConstantUtil.GameplaySceneName);
            if (!gameplayScene.isLoaded) return;

            await SceneManager.UnloadSceneAsync(ConstantUtil.GameplaySceneName)
                .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        #endregion

        private void OnDisable() => UnsubscribeEvents();
    }
}