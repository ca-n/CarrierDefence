using System;
using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject dummyCamera;
        [SerializeField] private GameObject gameOverScreen;
        [SerializeField] private GameObject upgradeShop;

        [SerializeField] private GameObject[] mainMenuHighscoreNames;
        [SerializeField] private GameObject[] mainMenuHighscoreWaves;
        [SerializeField] private GameObject[] mainMenuHighscoresScores;

        [SerializeField] private Text hudScoreText;
        [SerializeField] private GameObject hudWaveCleared;
        [SerializeField] private Text nextWaveTimeText;
        [SerializeField] private Text hudWaveNumberText;
        [SerializeField] private Text hudEnemiesRemainingText;
        [SerializeField] private Slider hudCarrierHealth;
        [SerializeField] private GameObject hudSoftBoundaryText;

        [SerializeField] private GameObject highscoreText;
        [SerializeField] private Text highscoreNameInputText;
        [SerializeField] private Outline highscoreNameInputOutline;

        [SerializeField] private Button upgradeGunsButton;
        [SerializeField] private Button upgradeBulletsButton;
        [SerializeField] private Button upgradeFireRateButton;
        [SerializeField] private Text upgradeGunsCostText;
        [SerializeField] private Text upgradeBulletsCostText;
        [SerializeField] private Text upgradeFireRateCostText;

        private bool _highlighting;
        private bool _betweenWaves;
        private Highscore _bestHighscore;

        private void Start()
        {
            GameManager.Instance.onGameStateChanged.AddListener(HandleGameStateChanged);
            InstantiateVariables();
        }

        public void InstantiateVariables()
        {
            _highlighting = false;
            _betweenWaves = false;
            PopulateHighscores();
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentGameState == GameState.MainMenu && Input.anyKeyDown)
            {
                GameManager.Instance.StartGame();
            }

            if (Input.GetKeyDown(KeyCode.P) && _betweenWaves && GameManager.Instance.CurrentGameState == GameState.InGame)
            {
                GameManager.Instance.OpenUpgradeShop();
            }

            if (Input.GetKeyDown(KeyCode.Escape) && _betweenWaves &&
                GameManager.Instance.CurrentGameState == GameState.UpgradeShop)
            {
                GameManager.Instance.ExitUpgradeShop();
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            mainMenu.SetActive(state == GameState.MainMenu);
            hud.SetActive(state != GameState.MainMenu);
            pauseMenu.SetActive(state == GameState.PauseMenu);
            dummyCamera.SetActive(state == GameState.MainMenu);
            gameOverScreen.SetActive(state == GameState.GameOver);
            upgradeShop.SetActive(state == GameState.UpgradeShop);
            if (state == GameState.GameOver)
            {
                CheckHighscore();
                HideSoftBoundaryText();
                hudWaveCleared.SetActive(false);
                StopCoroutine(NextWaveCountdown());
            }
        }
        
        public void HandleScoreUpdated(int score)
        {
            hudScoreText.text = score.ToString();
            
            int upgradeGunsCost = ScoreManager.Instance.GetUpgradeGunsCost();
            bool canUpgradeGuns = score >= upgradeGunsCost;
            upgradeGunsButton.interactable = canUpgradeGuns;
            upgradeGunsCostText.text = upgradeGunsCost.ToString();
            upgradeGunsCostText.color = canUpgradeGuns ? Color.green : Color.red;

            int upgradeBulletsCost = ScoreManager.Instance.GetUpgradeBulletsCost();
            bool canUpgradeBullets = score >= upgradeBulletsCost;
            upgradeBulletsButton.interactable = canUpgradeBullets;
            upgradeBulletsCostText.text = upgradeBulletsCost.ToString();
            upgradeBulletsCostText.color = canUpgradeBullets ? Color.green : Color.red;

            int upgradeFireRateCost = ScoreManager.Instance.GetUpgradeFireRateCost();
            bool canUpgradeFireRate = score >= upgradeFireRateCost;
            upgradeFireRateButton.interactable = canUpgradeFireRate;
            upgradeFireRateCostText.text = upgradeFireRateCost.ToString();
            upgradeFireRateCostText.color = canUpgradeFireRate ? Color.green : Color.red;
        }

        public void ShowWaveClearedText()
        {
            _betweenWaves = true;
            hudWaveCleared.SetActive(true);
            StartCoroutine(NextWaveCountdown());
        }

        public void HandleCarrierDamaged(float healthLeft)
        {
            hudCarrierHealth.value = healthLeft;
        }

        private IEnumerator NextWaveCountdown()
        {
            for (int s = 15; s > 0; --s)
            {
                nextWaveTimeText.text = s.ToString();
                yield return new WaitForSeconds(1);
            }
            _betweenWaves = false;
            hudWaveCleared.SetActive(false);
            WaveManager.Instance.NextWave();
        }

        public void UpdateWaveText(int wave, int enemiesRemaining)
        {
            hudWaveNumberText.text = wave.ToString();
            hudEnemiesRemainingText.text = enemiesRemaining.ToString();
        }

        public void UpdateEnemiesRemainingText(string enemyTag)
        {
            int lastRemaining = int.Parse(hudEnemiesRemainingText.text);
            hudEnemiesRemainingText.text = (--lastRemaining).ToString();
        }

        public void OnClickResume()
        {
            GameManager.Instance.TogglePause();
        }

        public void OnClickRestart()
        {
            GameManager.Instance.RestartGame();
        }

        public void OnClickExit()
        {
            GameManager.Instance.QuitGame();
        }

        public void OnClickSubmitScore()
        {
            if (highscoreNameInputText.text.Trim() == "")
            {
                StartCoroutine(HighlightNameInput());
                return;
            }
            string name = highscoreNameInputText.text;
            int wave = WaveManager.Instance.Wave;
            int score = ScoreManager.Instance.Score;
            Highscore hi = new Highscore();
            hi.name = name;
            hi.wave = wave;
            hi.score = score;
            HighscoreStorage.Save(hi);
            GameManager.Instance.RestartGame();
        }

        private IEnumerator HighlightNameInput()
        {
            if (_highlighting) yield break;
            _highlighting = true;
            float increment = 0.125f;
            for (float a = increment; a >= 0; a += increment)
            {
                highscoreNameInputOutline.effectColor = new Color(1, 0, 0, a);
                yield return new WaitForSecondsRealtime(0.05f);
                if (a >= 1) increment *= -1.0f;
            }
            _highlighting = false;
        }

        private void PopulateHighscores()
        {
            List<Highscore> highscores = HighscoreStorage.Load();
            for (int i = 0; i < mainMenuHighscoreNames.Length; ++i)
            {
                if (i >= highscores.Count) break;
                Highscore highscore = highscores[i];
                mainMenuHighscoreNames[i].GetComponent<Text>().text = highscore.name;
                mainMenuHighscoreNames[i].SetActive(true);
                mainMenuHighscoreWaves[i].GetComponent<Text>().text = highscore.wave.ToString();
                mainMenuHighscoreWaves[i].SetActive(true);
                mainMenuHighscoresScores[i].GetComponent<Text>().text = highscore.score.ToString();
                mainMenuHighscoresScores[i].SetActive(true);
            }
            if (highscores.Count != 0) _bestHighscore = highscores[0];
        }

        private void CheckHighscore()
        {
            int wave = WaveManager.Instance.Wave;
            int score = ScoreManager.Instance.Score;
            highscoreText.SetActive(wave > _bestHighscore.wave || wave == _bestHighscore.wave && score > _bestHighscore.score);
        }

        public void OnClickExitUpgrades()
        {
            GameManager.Instance.ExitUpgradeShop();
        }

        public void ShowSoftBoundaryText()
        {
            hudSoftBoundaryText.SetActive(true);
        }

        public void HideSoftBoundaryText()
        {
            hudSoftBoundaryText.SetActive(false);
        }
    }
}