using TMPro;
using UnityEngine;

namespace GameManager {
    
    public class UIManager : MonoBehaviour {

        [SerializeField] private TMP_Text TimerText;
        [SerializeField] private TMP_Text ScoreText;

        public void UpdateScore(float score) {
            ScoreText.text = $"Score: {score}";
        }

        public void UpdateTimer(float time) {
            // TODO: seconds to xx:xx:xx
            ScoreText.text = $"Time: {time}";
        }
    }
}