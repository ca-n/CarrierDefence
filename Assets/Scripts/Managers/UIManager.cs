using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField] private Text hudWaveClearedText;
        [SerializeField] private Text hudWaveNumberText;
        [SerializeField] private Text hudEnemiesRemainingText;
        [SerializeField] private Object hudCarrierHealth;

        [SerializeField] private GameObject highscoreText;
        [SerializeField] private Text highscoreNameInputText;
        [SerializeField] private Outline highscoreNameInputOutline;
        private bool _highlighting = false;
        private Highscore _bestHighscore;

        private void Start()
        {
            Thread t = new Thread(PopulateHighscores);
            t.Start();
        }

        private void OnGameStateChanged(GameState state)
        {
            mainMenu.SetActive(state == GameState.MainMenu);
            hud.SetActive(state != GameState.MainMenu && state != GameState.UpgradeShop);
            pauseMenu.SetActive(state == GameState.PauseMenu);
            dummyCamera.SetActive(state == GameState.MainMenu);
            gameOverScreen.SetActive(state == GameState.GameOver);
            upgradeShop.SetActive(state == GameState.UpgradeShop);
            if (state == GameState.GameOver) CheckHighscore();
        }
        
        public void UpdateScoreText(int score)
        {
            //TODO: UPDATE SCORE
        }

        public void ShowWaveClearedText()
        {
            //TODO: WAVE CLEARED UI
        }

        public void UpdateWaveText(int wave, int enemiesRemaining)
        {
            //TODO: WAVE CHANGED UI
        }

        public void UpdateEnemiesRemainingText(string enemyTag)
        {
            //TODO: UPDATE ENEMIES REMAINING
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
            Highscore.Save(new Highscore(name, wave, score));
        }

        private IEnumerator HighlightNameInput()
        {
            if (_highlighting) yield break;
            _highlighting = true;
            float increment = 0.125f;
            for (float a = increment; a >= 0; a += increment)
            {
                highscoreNameInputOutline.effectColor = new Color(1, 0, 0, a);
                yield return new WaitForSeconds(0.05f);
                if (a >= 1) increment *= -1.0f;
            }
            _highlighting = false;
        }

        private void PopulateHighscores()
        {
            List<Highscore> highscores = Highscore.LoadAll();
            for (int i = 0; i < mainMenuHighscoreNames.Length; ++i)
            {
                if (i >= highscores.Count) break;
                Highscore highscore = highscores[i];
                mainMenuHighscoreNames[i].GetComponent<Text>().text = highscore.Name;
                mainMenuHighscoreNames[i].SetActive(true);
                mainMenuHighscoreWaves[i].GetComponent<Text>().text = highscore.Wave.ToString();
                mainMenuHighscoreWaves[i].SetActive(true);
                mainMenuHighscoresScores[i].GetComponent<Text>().text = highscore.Score.ToString();
                mainMenuHighscoresScores[i].SetActive(true);
            }
            if (highscores.Count != 0) _bestHighscore = highscores[0];
        }

        private void CheckHighscore()
        {
            int wave = WaveManager.Instance.Wave;
            int score = ScoreManager.Instance.Score;
            highscoreText.SetActive(wave > _bestHighscore.Wave || (wave == _bestHighscore.Wave && score > _bestHighscore.Score));
        }
    }
}