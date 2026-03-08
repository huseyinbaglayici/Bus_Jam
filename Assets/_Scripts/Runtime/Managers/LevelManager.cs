using System;
using _Scripts.Runtime.Commands;
using _Scripts.Runtime.Data.UnityObjects;
using _Scripts.Runtime.Extensions;
using _Scripts.Runtime.Signals;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Runtime.Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        private const string GAMEPLAY_SCENE_NAME = "GameplayScene";

        [SerializeField] internal Transform levelHolder;

        [HideInInspector] public LevelDataSO currentLevelData;
        private LevelLoaderCommand _levelLoaderCommand;
        private LevelDestroyerCommand _levelDestroyerCommand;

        private int _currentLevel;

        private void OnEnable()
        {
            SubscribeEvents();

            _currentLevel = SaveSignals.Instance.FireGetLevelId();
        }

        protected new void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            Init();
            GetHolderIfNull();
        }


        private void Init()
        {
            _levelLoaderCommand = new LevelLoaderCommand(this);
            _levelDestroyerCommand = new LevelDestroyerCommand(this);
        }

        private void GetHolderIfNull()
        {
            if (levelHolder != null) return;
            Debug.LogWarning("[LevelManager] LevelHolder is null >> Creating new LevelHolder !!>>");
            GameObject holderGo = new GameObject("LevelHolder");

            holderGo.transform.SetParent(transform);
            levelHolder = holderGo.transform;
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
        }


        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= OnLevelInitializeWrapper;
            CoreGameSignals.Instance.OnNextLevel -= OnNextLevelWrapper;
            CoreGameSignals.Instance.OnRestartLevel -= OnRestartLevelWrapper;
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
            await LoadGameplayScene(levelParam);
        }

        private async UniTaskVoid OnRestartLevelHandler()
        {
            _levelDestroyerCommand.Execute();
            await UnloadGameplaySceneAsync();
            CoreGameSignals.Instance.FireOnReset();
            await LoadGameplayScene(_currentLevel);

            CoreGameSignals.Instance.FireOnPlay();
        }

        private async UniTaskVoid OnNextLevelHandler()
        {
            _currentLevel++;
            SaveSignals.Instance.FireSaveLevel(_currentLevel);
            _levelDestroyerCommand.Execute();

            await UnloadGameplaySceneAsync();
            CoreGameSignals.Instance.FireOnReset();
            UISignals.Instance.FireOnSetLevelValue(_currentLevel);
        }

        private async UniTask LoadGameplayScene(int levelParam)
        {
            try
            {
                await SceneManager.LoadSceneAsync(GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());

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