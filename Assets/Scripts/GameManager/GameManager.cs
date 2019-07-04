using System.Collections;
using Abilities;
using Objects.Entities.Players;
using UnityEngine;

namespace GameManager {
    
    [RequireComponent(typeof(UIManager), typeof(AudioManager))]
    public class GameManager : MonoBehaviour {
        [SerializeField] private bool Pause;
        [SerializeField] private bool SoloMode;
        [SerializeField] private GameObject DMGDealer;
        [SerializeField] private GameObject Healer;
        
        private ComboAbility m_comboAbility;

        private PlayerController m_DMGDealer;
        private PlayerController m_Healer;
        private PlayerController m_MainPlayer;
        
        public float Score { get; private set; } = 0;
        public float Time { get; private set; } = 0;

        private const int MaxFriendship = 100;
        public int CurrentFriendship { get; private set; } = 0;
        
        public UIManager m_UIManager { get; private set; }
        public AudioManager m_AudioManager { get; private set; }

        public static GameManager Instance { get; private set; }

        void Awake() {
            
            if (Instance != null)
                Destroy(this);

            Instance = this;
            m_UIManager = GetComponent<UIManager>();
            m_AudioManager = GetComponent<AudioManager>();

            // TODO: remove after test
            CurrentFriendship = MaxFriendship;

            m_MainPlayer = m_DMGDealer = DMGDealer.GetComponentInChildren<PlayerController>();
            m_Healer = Healer.GetComponentInChildren<PlayerController>();
            
            m_comboAbility = GetComponent<ComboAbility>();
            m_comboAbility.Player = m_MainPlayer;

            StartCoroutine(nameof(StartTimer));
        }

        void Update() {

            if (CurrentFriendship >= MaxFriendship) {
               
                if (SoloMode) {
                    if (InputController.ComboAbility(m_MainPlayer.InputSource))
                        ComboAbility();
                
                } else if (InputController.ComboAbility(m_DMGDealer.InputSource) 
                           && InputController.ComboAbility(m_Healer.InputSource))
                    ComboAbility();
            }
            
            if (SoloMode) {
                if (InputController.Swap(m_MainPlayer.InputSource))
                    SwapCharacters();
                
            } else if (InputController.Swap(m_DMGDealer.InputSource) 
                       || InputController.Swap(m_Healer.InputSource))
                SwapCharacters();
        }

        void SwapCharacters() {
            GameObject oldRoot = m_MainPlayer.transform.root.gameObject;
            m_MainPlayer = m_MainPlayer == m_DMGDealer ? m_Healer : m_DMGDealer;

            if (SoloMode) {
                
                GameObject newRoot = m_MainPlayer.transform.root.gameObject;
                newRoot.transform.position = oldRoot.transform.position;

                oldRoot.SetActive(false);
                newRoot.SetActive(true);
            } else {
                PlayerController.InputType tmp = m_DMGDealer.Input;
                m_DMGDealer.Input = m_Healer.Input;
                m_Healer.Input = tmp;
            }
            
            // TODO: Start delay coroutine
        }

        private void ComboAbility() {
            // TODO: remove after test
//            CurrentFriendship = 0;
            m_comboAbility.Use();
        }

        private IEnumerator StartTimer() {
            
            while (!Pause) {
                Time += UnityEngine.Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public void TogglePause() {
            Pause = !Pause;
        }

        public void UpdateFriendshipAmount(int value) {
            
            int oldValue = CurrentFriendship;
            if (value > 0) {
                Debug.Log($"Friendship (+{value})");
                CurrentFriendship += value;
                if (CurrentFriendship > MaxFriendship)
                    CurrentFriendship = MaxFriendship;
                else if (CurrentFriendship < 0)
                    CurrentFriendship = 0;
            }

            if (oldValue != CurrentFriendship) {
                m_UIManager.UpdateFriendshipBar(CurrentFriendship);
            }
        }
    }
}