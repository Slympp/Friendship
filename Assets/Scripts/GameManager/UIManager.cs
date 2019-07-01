using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameManager {
    
    public class UIManager : MonoBehaviour {

        [SerializeField] private TMP_Text TimerText;
        [SerializeField] private TMP_Text ScoreText;

        [SerializeField] private float ProgressDuration = 1f;
        [SerializeField] private Image HealerHealthBar;
        [SerializeField] private Image DMGDealerHealthBar;
        [SerializeField] private Image FriendshipBar;

        [SerializeField] private Vector2 HealthBarRadiusBoudaries = new Vector2(0.1f, 0.71f);
        
        public void UpdateScore(float score) {
            ScoreText.text = $"Score: {score}";
        }

        public void UpdateTimer(float time) {
            // TODO: seconds to xx:xx:xx
            ScoreText.text = $"Time: {time}";
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
    }
}