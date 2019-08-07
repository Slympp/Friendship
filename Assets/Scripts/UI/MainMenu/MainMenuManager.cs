using System;
using GameManager;
using Objects.Entities.Players;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour {
        
        [Header("Background")]
        [SerializeField] private GameObject BackgroundNormal;
        [SerializeField] private GameObject BackgroundBlurry;
        [SerializeField] private GameObject BackgroundNoChar;
       
        [Header("Elements")]
        [SerializeField] private GameObject CreditsElement;
        [SerializeField] private GameObject Logo;
        
        [Header("Buttons")]
        [SerializeField] private GameObject Buttons;
        [SerializeField] private Button ButtonStart;
        [SerializeField] private Button ButtonCredits;
        [SerializeField] private Button ButtonQuit;
    
        [Header("Start")]
        [SerializeField] private GameObject StartMenu;

        [SerializeField] private GameObject HowToPlayKeyboard;
        [SerializeField] private GameObject HowToPlayController;
        
        [SerializeField] private Button PlayButton;
        [SerializeField] private Button BackButton;
        [SerializeField] private Button HowToPlayKeyboardButton;
        [SerializeField] private Button HowToPlayControllerButton;

        private bool SoloMode = true;
        [SerializeField] private Button ModeLeft;
        [SerializeField] private Button ModeRight;
        [SerializeField] private TMP_Text ModeText;
        
        [SerializeField] private Button P1Left;
        [SerializeField] private Button P1Right;
        [SerializeField] private TMP_Text P1Text;
        private InputScheme P1InputType = InputScheme.KeyboardA;
        
        [SerializeField] private Button P2Left;
        [SerializeField] private Button P2Right;
        [SerializeField] private TMP_Text P2Text;
        private InputScheme P2InputType = InputScheme.KeyboardC;
        
        [SerializeField] private GameObject P1Indicator;
        [SerializeField] private GameObject P2Slider;

        [Header("Name Input")] 
        [SerializeField] private GameObject NameInput;
        [SerializeField] private TMP_InputField NameInputField;
        [SerializeField] private Button ValidButton;
        [SerializeField] private Button NameInputBackButton;
        [SerializeField] private GameObject NameRequiredError;

        [SerializeField] private GameObject Loading;

        [Header("Audio")] 
        [SerializeField] private AudioClip OnClick;
        [SerializeField] private AudioClip OnHover;
        [SerializeField] private AudioClip OnError;
        [SerializeField] private AudioClip OnStart;
        private AudioSource m_Audio;
        
        void Awake() {

            m_Audio = GetComponent<AudioSource>();
            
            ButtonStart.onClick.AddListener(() => {
                SwapBackground(BackgroundType.NoChar);
                
                Buttons.SetActive(false);
                CreditsElement.SetActive(false);
                StartMenu.SetActive(true);
            });
            
            BackButton.onClick.AddListener(() => {
                SwapBackground(BackgroundType.Normal);
                
                Buttons.SetActive(true);
                StartMenu.SetActive(false);
            });
            
            PlayButton.onClick.AddListener(() => {
                StartMenu.SetActive(false);
                NameInput.SetActive(true);
            });
            
            ValidButton.onClick.AddListener(StartGame);
            NameInputBackButton.onClick.AddListener(() => {
                StartMenu.SetActive(true);
                NameInput.SetActive(false);
                NameRequiredError.SetActive(false);
            });
            
            NameInputField.onValueChanged.AddListener((string s) => {
                NameRequiredError.SetActive(false);
            });
            
            ButtonCredits.onClick.AddListener(Credits);
            ButtonQuit.onClick.AddListener(Quit);

            HowToPlayControllerButton.onClick.AddListener(() => { SwapHowToPlay(false); });
            HowToPlayKeyboardButton.onClick.AddListener(() => { SwapHowToPlay(true); });
            
            ModeLeft.onClick.AddListener(SwapMode);
            ModeRight.onClick.AddListener(SwapMode);
            
            P1Left.onClick.AddListener(() => SwapInputScheme(1, ref P1InputType, -1, P1Text));
            P1Right.onClick.AddListener(() => SwapInputScheme(1, ref P1InputType, 1, P1Text));

            P2Left.onClick.AddListener(() => SwapInputScheme(2, ref P2InputType, -1, P2Text));
            P2Right.onClick.AddListener(() => SwapInputScheme(2, ref P2InputType, 1, P2Text));
        }

        private void SwapMode() {
            SoloMode = !SoloMode;
            ModeText.text = SoloMode ? "Solo" : "Multi";
            P1InputType = SoloMode ? InputScheme.KeyboardA : InputScheme.KeyboardB;
            SwapInputScheme(1, ref P1InputType, 0, P1Text);

            P1Indicator.SetActive(!SoloMode);
            P2Slider.SetActive(!SoloMode);
        }

        private void SwapInputScheme(int id, ref InputScheme type, int modifier, TMP_Text text) {
            
            // TODO: REWORK TO AVOID DUPLICATE INPUTS
            var updatedValue = type + modifier;
            CheckBoundaries(ref updatedValue);
            if (updatedValue != InputScheme.Controller && ((!SoloMode && id == 1 && updatedValue == P2InputType) || (id == 2 && updatedValue == P1InputType)))
                updatedValue += modifier;
            CheckBoundaries(ref updatedValue);

            type = updatedValue;
          
            FormatInput(type, text);
        }

        private void CheckBoundaries(ref InputScheme value) {
            if ((int)value >= Enum.GetNames(typeof(InputScheme)).Length) {
                value = 0;
            } else if ((int) value < 0)
                value = InputScheme.Controller;
        }

        private void FormatInput(InputScheme type, TMP_Text text) {
            switch (type) {
                case InputScheme.Controller:
                    text.text = "controller";
                    break;
                case InputScheme.KeyboardA:
                    text.text = "keyboard a";
                    break;
                case InputScheme.KeyboardB:
                    text.text = "keyboard b";
                    break;
                case InputScheme.KeyboardC:
                    text.text = "keyboard c";
                    break;
            }
        }

        private void SwapHowToPlay(bool keyboard) {
            HowToPlayControllerButton.gameObject.SetActive(keyboard);
            HowToPlayController.SetActive(!keyboard);
            
            HowToPlayKeyboardButton.gameObject.SetActive(!keyboard);
            HowToPlayKeyboard.SetActive(keyboard);
        }

        private void Credits() {

            if (!CreditsElement.activeSelf) {
                SwapBackground(BackgroundType.Blurry);
                CreditsElement.SetActive(true);
            } else {
                SwapBackground(BackgroundType.Normal);
                CreditsElement.SetActive(false);
            }
        }

        private void Quit() {
            Application.Quit();
        }

        private void StartGame() {
            
            if (NameInputField.text.Length <= 0) {
                NameRequiredError.SetActive(true);
                m_Audio.PlayOneShot(OnError);
                return;
            }
            
            SceneLoadingParameters.SoloMode = SoloMode;
            SceneLoadingParameters.Name = NameInputField.text;
            
            SceneLoadingParameters.PlayerOneInputs = (PlayerController.InputType)P1InputType;
            
            PlayerController.InputType p2 = (P1InputType == InputScheme.Controller && P2InputType == InputScheme.Controller) ? PlayerController.InputType.Controller2 : (
                PlayerController.InputType) P2InputType;
            
            SceneLoadingParameters.PlayerTwoInputs = SoloMode ? SceneLoadingParameters.PlayerOneInputs : p2;
            
            NameInput.SetActive(false);
            Loading.SetActive(true);
            
            m_Audio.PlayOneShot(OnStart);
            
            SceneManager.LoadScene("Level1");
        }

        private void SwapBackground(BackgroundType type) {
            BackgroundNormal.SetActive(type == BackgroundType.Normal);
            BackgroundBlurry.SetActive(type == BackgroundType.Blurry);
            BackgroundNoChar.SetActive(type == BackgroundType.NoChar);
        }

        public void OnButtonHover() {
            m_Audio.PlayOneShot(OnHover);
        }

        public void OnButtonClick() {
            m_Audio.PlayOneShot(OnClick);
        }

        private enum BackgroundType {
            Normal,
            Blurry,
            NoChar
        }

        private enum InputScheme {
            KeyboardA = 0,
            KeyboardB = 1,
            KeyboardC = 2,
            Controller = 3
        }
    }
}