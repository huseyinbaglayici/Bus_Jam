using System;
using _Scripts.Runtime.Commands;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Signals;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        private const string GAMEPLAY_SCENE_NAME = "GameplayScene";


        private bool _isLoading = false;

        public LevelDataSO currentLevelData;
        private LevelLoaderCommand _levelLoaderCommand;

        private int _currentLevel;

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            SubscribeEvents();

            _currentLevel = SaveSignals.Instance.FireGetLevelId();
            currentLevelData = SaveSignals.Instance.FireOnGetLevelData();
        }

        private int OnSendTimerData()
        {
            return currentLevelData.Time;
        }


        private void Init()
        {
            _levelLoaderCommand = new LevelLoaderCommand(this);
        }

        private void Start()
        {
            CoreGameSignals.Instance.FireOnLevelInitialize(_currentLevel);
        }

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


        #region Wrapper functions

        /// <summary>
        ///  i used wrapper functions because i have to sub/ubsub other action signals.
        /// Because of the handlers returns unitaskvoid I cannot link to them with coregamesignals. so I used wrappers
        /// </summary>
        /// <param name="levelParam"></param>
        private void OnLevelInitializeWrapper(int levelParam)
        {
            OnLevelInitializeHandler(levelParam).Forget();
        }

        private void OnNextLevelWrapper()
        {
            OnNextLevelHandler().Forget();
        }

        private void OnRestartLevelWrapper()
        {
            OnRestartLevelHandler().Forget();
        }

        #endregion

        #endregion

        private async UniTaskVoid OnLevelInitializeHandler(int levelParam)
        {
            if (_isLoading) return;
            _isLoading = true;
            await LoadGameplayScene(levelParam);
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

        private async UniTask LoadGameplayScene(int levelParam)
        {
            try
            {
                await SceneManager.LoadSceneAsync(GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

                Scene gameplayScene = SceneManager.GetSceneByName(GAMEPLAY_SCENE_NAME);
                SceneManager.SetActiveScene(gameplayScene);

                await _levelLoaderCommand.ExecuteAsync(levelParam);
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
            Scene gameplayScene = SceneManager.GetSceneByName(GAMEPLAY_SCENE_NAME);
            if (gameplayScene.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(GAMEPLAY_SCENE_NAME)
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
    }
}