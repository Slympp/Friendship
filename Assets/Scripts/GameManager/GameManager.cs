using System.Collections;
using UnityEngine;

namespace GameManager {
    
    [RequireComponent(typeof(UIManager), typeof(AudioManager))]
    public class GameManager : MonoBehaviour {
        [SerializeField] private bool SoloMode;
        [SerializeField] private bool Pause;
        
        public float Score { get; private set; } = 0;
        public float Time { get; private set; } = 0;

        [SerializeField] private float MaxFriendship;
        public float CurrentFriendship { get; private set; } = 0;

        public UIManager m_UIManager { get; private set; }
        public AudioManager m_AudioManager { get; private set; }

        public static GameManager Instance { get; private set; }

        void Awake() {
            
            if (Instance != null)
                Destroy(this);

            Instance = this;
            m_UIManager = GetComponent<UIManager>();
            m_AudioManager = GetComponent<AudioManager>();

            StartCoroutine(nameof(StartTimer));
        }

        private IEnumerator StartTimer() {
            
            while (!Pause) {
                Time += UnityEngine.Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}