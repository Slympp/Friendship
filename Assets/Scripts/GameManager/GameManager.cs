using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Cinemachine;
using Objects.Entities.Players;
using Ranking;
using UnityEngine;

namespace GameManager {
    
    [RequireComponent(typeof(UIManager), typeof(AudioManager))]
    public class GameManager : MonoBehaviour {
        [SerializeField] private bool Pause;
        [SerializeField] private bool SoloMode;
        [SerializeField] private GameObject DMGDealer;
        [SerializeField] private GameObject Healer;

        [SerializeField] private CinemachineVirtualCamera VirtualCam;
        [SerializeField] private AudioSource MainThemeAudioSource;
        
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

        private bool m_GameEnded;

        private bool m_CanSwap = true;
        private const float SwapDelay = 1f;

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
            
            SoloMode = SceneLoadingParameters.SoloMode;
            
            m_DMGDealer.Input = SceneLoadingParameters.PlayerOneInputs;
            m_Healer.Input = SceneLoadingParameters.PlayerTwoInputs;
        }

        void Start() {
            if (SoloMode)
                ToggleCharacter(m_Healer, false);
        }

        void Update() {
            if (Pause)
                return;
            
            if (!m_GameEnded)
                UpdateGameOver();
            
            UpdateComboAbility();
            UpdateSwapCharacter();
        }

        void UpdateGameOver() {
            
            if (SoloMode && m_MainPlayer.IsDead) {
                if (!m_Healer.IsDead || !m_DMGDealer.IsDead)
                    SwapCharacters();
                else {
                    OnGameEnd();
                    m_UIManager.EnableGameOver();
                }
                
            } else if (m_Healer.IsDead && m_DMGDealer.IsDead) {
                OnGameEnd();
                m_UIManager.EnableGameOver();
            }
        }

        public bool IsPaused() {
            return Pause; 
        }

        public void ToggleIntro() {
            m_UIManager.FadeIntroScreen();
            
            this.Invoke(() => {
                m_UIManager.DisableIntroScreen();
                Pause = false;
                StartCoroutine(nameof(StartTimer));
            }, 1f);
        }

        public void EnableWin() {
            OnGameEnd();
            ScoreboardManager manager = GetComponent<ScoreboardManager>();
            manager.AddEntry(SceneLoadingParameters.Name, Mathf.FloorToInt(Score), Mathf.FloorToInt(Time));
            m_UIManager.EnableWinScreen();
        }

        void OnGameEnd() {
            if (!m_GameEnded) {
                MainThemeAudioSource.Stop();
                m_GameEnded = true;
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
            if (!m_CanSwap || (!m_DMGDealer.isActiveAndEnabled && m_DMGDealer.IsDead) 
                || (!m_Healer.isActiveAndEnabled && m_Healer.IsDead)) return;
            
            if (SoloMode) {
                if (InputController.Swap(m_MainPlayer.InputSource))
                    SwapCharacters();
                
            } else if (InputController.Swap(m_DMGDealer.InputSource) 
                       || InputController.Swap(m_Healer.InputSource))
                SwapCharacters();
        }

        void SwapCharacters() {
            m_CanSwap = false;
            
            Transform oldRoot = m_MainPlayer.transform.root;
            PlayerController oldPlayer = m_MainPlayer;
            
            m_MainPlayer = m_MainPlayer == m_DMGDealer ? m_Healer : m_DMGDealer;

            if (SoloMode) {
                
                Transform newRoot = m_MainPlayer.transform.root;
                newRoot.position = oldRoot.position;
                newRoot.rotation = oldRoot.rotation;

                ToggleCharacter(oldPlayer, false);
                ToggleCharacter(m_MainPlayer, true);
            } else {
                PlayerController.InputType tmp = m_DMGDealer.Input;
                m_DMGDealer.Input = m_Healer.Input;
                m_Healer.Input = tmp;
            }

            VirtualCam.Follow = m_MainPlayer.transform.parent;

            this.Invoke(() => { m_CanSwap = true; }, SwapDelay);
        }

        private void ToggleCharacter(PlayerController player, bool e) {
            Transform root = player.transform.root;
            
            root.GetComponent<Rigidbody2D>().bodyType = e ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
            root.GetComponent<Collider2D>().enabled = e;
            player.enabled = e;
            
            List<SpriteRenderer> sr = root.GetComponentsInChildren<SpriteRenderer>(true).ToList();
            foreach (SpriteRenderer s in sr) {
                s.enabled = e;
            }
            
            player.OnSwap(e);
        }

        private void ComboAbility() {
            CurrentFriendship = 0;
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

        public void MoveToMainPlayer(Transform player) {
            if (m_MainPlayer) {
                player.transform.position = m_MainPlayer.transform.parent.position;
            }
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