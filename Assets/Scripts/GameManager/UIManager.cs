using System;
using System.Collections;
using System.Collections.Generic;
using Ranking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameManager {
    
    public class UIManager : MonoBehaviour {
        
        [Header("Scores")]
        [SerializeField] private TMP_Text ScoreText;
        [SerializeField] private Animator ScoreAnimator;
        private int ScoreIncreaseAnimation = Animator.StringToHash("ScoreIncrease");
        
        [SerializeField] private float ProgressDuration = 1f;
        [SerializeField] private Image HealerHealthBar;
        [SerializeField] private Image DMGDealerHealthBar;
        [SerializeField] private Image FriendshipBar;

        [SerializeField] private Vector2 HealthBarRadiusBoudaries = new Vector2(0.1f, 0.71f);
        [SerializeField] private Vector2 FriendshipBarBoudaries = new Vector2(0.05f, 0.93f);

        [Header("GameOver Screen")] 
        [SerializeField] private GameObject GameOverScreen;
        [SerializeField] private Button MainMenuButton;
        [SerializeField] private Button QuitButton;
        
        [Header("Win Screen")] 
        [SerializeField] private GameObject WinScreen;
        [SerializeField] private GameObject WinScreenNormal;
        [SerializeField] private GameObject WinScreenRanking;
        [SerializeField] private Button WinRankingButton;
        [SerializeField] private Button WinMainMenuButton;
        [SerializeField] private Button WinRankingBackButton;

        [Header("Ranking")] 
        [SerializeField] private List<ScoreboardEntryController> ScoreboardEntries;

        void Awake() {
            MainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenu"); });
            QuitButton.onClick.AddListener(Application.Quit);
            
            WinRankingButton.onClick.AddListener(() => { SwitchWinScreens(true); });
            WinMainMenuButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenu"); });
            WinRankingBackButton.onClick.AddListener(() => { SwitchWinScreens(); });
        }

        public void EnableWinScreen() {
            WinScreen.SetActive(true);
        }

        private void SwitchWinScreens(bool ranking = false) {
            
            if (ranking)
                GetComponent<ScoreboardManager>().GetEntries();
            
            WinScreenNormal.SetActive(!ranking);
            WinScreenRanking.SetActive(ranking);
        }

        public void UpdateScoreboard(List<ScoreboardManager.Entry> entries) {
            for (int i = 0; i < 5; i++) {
                if (i < entries.Count)
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(entries[i].Time);
                    string minutes = timeSpan.Minutes == 0 ? "00" : timeSpan.Minutes < 10 ? $"0{timeSpan.Minutes}" : $"{timeSpan.Minutes}";
                    string seconds = timeSpan.Seconds == 0 ? "00" : timeSpan.Seconds < 10 ? $"0{timeSpan.Seconds}" : $"{timeSpan.Seconds}";
                    
                    ScoreboardEntries[i].SetValues(entries[i].Name, minutes + ":" + seconds, entries[i].Score.ToString());
                    ScoreboardEntries[i].gameObject.SetActive(true);
                } else
                    ScoreboardEntries[i].gameObject.SetActive(false);
            }
        }
        
        public void EnableGameOver() {
            GameOverScreen.SetActive(true);
        }
        
        public void UpdateScore(float score) {
            ScoreAnimator.Play(ScoreIncreaseAnimation);
            ScoreText.text = $"{Mathf.FloorToInt(score)}";
        }
        
        public void UpdateHealthBar(string player, float newHealth, float maxHealth) {
            float clamped = newHealth.Normalize(HealthBarRadiusBoudaries.x, HealthBarRadiusBoudaries.y, 0, maxHealth);

            if (player == "Healer")
                StartCoroutine(UpdateHealthBarOvertime(new HealthBarUpdateInfos {
                    image = HealerHealthBar,
                    newValue = clamped
                }));
            else if (player == "DMGDealer") 
                StartCoroutine(UpdateHealthBarOvertime(new HealthBarUpdateInfos {
                    image = DMGDealerHealthBar,
                    newValue = clamped
                }));
        }

        private IEnumerator UpdateHealthBarOvertime(HealthBarUpdateInfos i) {
            
            float oldValue = i.image.fillAmount;
            float elapsed = 0;

            while (elapsed < ProgressDuration) {
                elapsed += Time.deltaTime;
                i.image.fillAmount = Mathf.Lerp(oldValue, i.newValue, elapsed / ProgressDuration);
                yield return new WaitForEndOfFrame();
            }
            i.image.fillAmount = i.newValue;
        }

        private struct HealthBarUpdateInfos {
            public Image image;
            public float newValue;
        }

        public void UpdateFriendshipBar(float amount) {
            float clamped = amount.Normalize(FriendshipBarBoudaries.x, FriendshipBarBoudaries.y, 0, 100);
            StartCoroutine(UpdateFriendshipBarOvertime(clamped));
        }

        private IEnumerator UpdateFriendshipBarOvertime(float newValue) {
            float oldValue = FriendshipBar.fillAmount;
            float elapsed = 0;

            while (elapsed < ProgressDuration) {
                elapsed += Time.deltaTime;
                FriendshipBar.fillAmount = Mathf.Lerp(oldValue, newValue, elapsed / ProgressDuration);
                yield return new WaitForEndOfFrame();
            }
            FriendshipBar.fillAmount = newValue;
        }
    }
}