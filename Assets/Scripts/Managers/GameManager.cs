using System.Collections.Generic;
using Events;
using Types;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public GameStateChangedEvent onGameStateChanged;
        
        private readonly string GameScene = "Game";

        [SerializeField] private GameObject[] globalPrefabs;
        private List<GameObject> _globalInstances;
        private List<AsyncOperation> _loadOperations;

        public GameState CurrentGameState { get; private set; }

        private void Start()
        {
            DontDestroyOnLoad(this);
            _globalInstances = new List<GameObject>();
            _loadOperations = new List<AsyncOperation>();
            InstantiateGlobalPrefabs();
            CurrentGameState = GameState.MainMenu;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        }

        private void InstantiateGlobalPrefabs()
        {
            foreach (GameObject prefab in globalPrefabs)
            {
                Debug.Log("Instantiating " + prefab.name, this);
                _globalInstances.Add(Instantiate(prefab, transform));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_globalInstances != null)
            {
                foreach (GameObject instance in _globalInstances)
                {
                    Destroy(instance);
                }
                _globalInstances.Clear();
            }
        }

        private void LoadScene(string sceneName)
        {
            AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadSceneAsync == null)
            {
                Debug.LogError("Unable to load scene: " + sceneName, this);
                return;
            }

            Debug.Log("Loading " + sceneName, this);
            loadSceneAsync.completed += OnLoadSceneComplete;
            _loadOperations.Add(loadSceneAsync);
        }

        private void UnloadScene(string sceneName)
        {
            AsyncOperation unloadSceneAsync = SceneManager.UnloadSceneAsync(sceneName);
            if (unloadSceneAsync == null)
            {
                Debug.LogError("Unable to unload scene: " + sceneName, this);
                return;
            }

            Debug.Log("Unloading " + sceneName, this);
            unloadSceneAsync.completed += OnUnloadSceneComplete;
        }

        private void OnLoadSceneComplete(AsyncOperation loadSceneAsync)
        {
            Debug.Log("Load completed.", this);
            if (_loadOperations.Contains(loadSceneAsync))
            {
                _loadOperations.Remove(loadSceneAsync);
                if (_loadOperations.Count == 0)
                {
                    UpdateGameState(GameState.InGame);
                }
            }
        }

        private void OnUnloadSceneComplete(AsyncOperation unloadSceneAsync)
        {
            Debug.Log("Unload completed.", this);
        }

        void UpdateGameState(GameState newGameState)
        {
            CurrentGameState = newGameState;
            switch (newGameState)
            {
                case GameState.PauseMenu:
                    Time.timeScale = 0;
                    break;
                default:
                    Time.timeScale = 1;
                    break;
            }
            onGameStateChanged.Invoke(newGameState);
        }

        public void StartGame()
        {
            LoadScene(GameScene);
        }

        public void TogglePause()
        {
            switch (CurrentGameState)
            {
                case GameState.InGame:
                    UpdateGameState(GameState.PauseMenu);
                    break;
                case GameState.PauseMenu:
                    UpdateGameState(GameState.InGame);
                    break;
            }
        }

        public void GameOver()
        {
            UpdateGameState(GameState.GameOver);
            UnloadScene(GameScene);
        }

        public void RestartGame()
        {
            UpdateGameState(GameState.MainMenu);
            UnloadScene(GameScene);
        }

        public void QuitGame()
        {
            Debug.Log("Quitting game.");
            Application.Quit();
        }

        public void ResetGameScene()
        {
            UnloadScene(GameScene);
            LoadScene(GameScene);
        }
    }
}