using TMPro;
using UnityEngine;

namespace Ranking {
    public class ScoreboardEntryController : MonoBehaviour {
        [SerializeField] private TMP_Text Name;
        [SerializeField] private TMP_Text Time;
        [SerializeField] private TMP_Text Score;

        public void SetValues(string name, string time, string score) {
            Name.text = name;
            Time.text = time;
            Score.text = score;
        }
    }
}