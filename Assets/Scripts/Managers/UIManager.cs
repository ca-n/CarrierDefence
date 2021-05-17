using Types;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject hud;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject dummyCamera;
        [SerializeField] private GameObject gameOverScreen;

        [SerializeField] private Text hudScoreText;
        [SerializeField] private Text hudWaveClearedText;
        [SerializeField] private Text hudWaveNumberText;
        [SerializeField] private Text hudEnemiesRemainingText;
        
        public void ShowMainMenu()
        {
            //TODO: SHOW MAIN MENU
        }
        
        public void UpdateScoreText(int score)
        {
            //TODO: UPDATE SCORE
            //TODO: ON ALL SCREENS
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
    }
}