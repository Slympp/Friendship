using System.Collections;
using Abilities;
using Cinemachine;
using Objects.Entities.Players;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;

namespace GameManager {
    
    [RequireComponent(typeof(UIManager), typeof(AudioManager))]
    public class GameManager : MonoBehaviour {
        [SerializeField] private bool Pause;
        [SerializeField] private bool SoloMode;
        [SerializeField] private GameObject DMGDealer;
        [SerializeField] private GameObject Healer;

        [SerializeField] private CinemachineVirtualCamera VirtualCam;
        
        private ComboAbility m_comboAbility;

        private PlayerController m_DMGDealer;
        private PlayerController m_Healer;
        private PlayerController m_MainPlayer;
        
        public float Score { get; private set; } = 0;
        public float Time { get; private set; } = 0;

        private const int MaxFriendship = 100;
        [SerializeField] private float NaturalGenerationDelay = 1f;
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

            m_MainPlayer = m_DMGDealer = DMGDealer.GetComponentInChildren<PlayerController>();
            m_Healer = Healer.GetComponentInChildren<PlayerController>();
            
            m_comboAbility = GetComponent<ComboAbility>();
            m_comboAbility.Player = m_MainPlayer;

            StartCoroutine(nameof(StartTimer));
        }

        void Update() {
            UpdateGameOver();
            UpdateComboAbility();
            UpdateSwapCharacter();
        }

        void UpdateGameOver() {
            
            if (SoloMode) {
                
                if (m_MainPlayer.IsDead && (!m_Healer.IsDead || !m_DMGDealer.IsDead))
                    SwapCharacters();
                else {
                    // TODO: GAMEOVER
                    Debug.Log("GameOver");
                }
            } else if (m_Healer.IsDead && m_DMGDealer.IsDead) {
                // TODO: GAMEOVER
                Debug.Log("GameOver");
            }
        }

        void UpdateComboAbility() {
            if (CurrentFriendship >= MaxFriendship) {
               
                if (SoloMode) {
                    if (!m_MainPlayer.IsDead && InputController.ComboAbility(m_MainPlayer.InputSource))
                        ComboAbility();
                
                } else if (!m_DMGDealer.IsDead && InputController.ComboAbility(m_DMGDealer.InputSource)
                           || !m_Healer.IsDead && InputController.ComboAbility(m_Healer.InputSource))
                    ComboAbility();
            }
        }

        void UpdateSwapCharacter() {
            if ((m_DMGDealer.isActiveAndEnabled && m_DMGDealer.IsDead) 
                || (m_Healer.isActiveAndEnabled && m_Healer.IsDead)) return;
            
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

            VirtualCam.Follow = m_MainPlayer.transform.parent;

            // TODO: Start delay coroutine
        }

        private void ComboAbility() {
            // TODO: remove after test
//            CurrentFriendship = 0;
            m_comboAbility.Use();
        }

        private IEnumerator StartTimer() {
            float naturalGeneration = 0;
            
            while (!Pause) {
                Time += UnityEngine.Time.deltaTime;

                if (naturalGeneration >= NaturalGenerationDelay) {
                    UpdateFriendshipAmount(1);
                    naturalGeneration = 0;
                }
                naturalGeneration += UnityEngine.Time.deltaTime;
                
                yield return new WaitForEndOfFrame();
            }
        }

        public void TogglePause() {
            Pause = !Pause;
        }

        public void UpdateFriendshipAmount(int value) {
            
            int oldValue = CurrentFriendship;
            if (value > 0) {
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
        
        public void UpdateScoreAmount(float value) {
            
            float oldValue = Score;
            if (value > 0)
                Score += value;

            if (oldValue != Score) {
                m_UIManager.UpdateScore(Score);
            }
        }
    }
}