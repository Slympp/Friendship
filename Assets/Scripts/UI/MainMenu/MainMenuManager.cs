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
        private PlayerController.InputType P1InputType = PlayerController.InputType.Keyboard;
        
        [SerializeField] private Button P2Left;
        [SerializeField] private Button P2Right;
        [SerializeField] private TMP_Text P2Text;
        private PlayerController.InputType P2InputType = PlayerController.InputType.KeyboardRight;
        
        [SerializeField] private GameObject P1Indicator;
        [SerializeField] private GameObject P2Slider;

        [Header("Name Input")] 
        [SerializeField] private GameObject NameInput;
        [SerializeField] private TMP_InputField NameInputField;
        [SerializeField] private Button ValidButton;
        [SerializeField] private Button NameInputBackButton;
        [SerializeField] private GameObject NameRequiredError;
        
        void Awake() {
            
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
            
            P1Left.onClick.AddListener(() => SwapInputScheme(ref P1InputType, -1, P1Text));
            P1Right.onClick.AddListener(() => SwapInputScheme(ref P1InputType, 1, P1Text));

            P2Left.onClick.AddListener(() => SwapInputScheme(ref P2InputType, -1, P2Text));
            P2Right.onClick.AddListener(() => SwapInputScheme(ref P2InputType, 1, P2Text));
        }

        private void SwapMode() {
            SoloMode = !SoloMode;
            ModeText.text = SoloMode ? "Solo" : "Multi";
            P1InputType = SoloMode ? PlayerController.InputType.Keyboard : PlayerController.InputType.KeyboardLeft;
            SwapInputScheme(ref P1InputType, 0, P1Text);

            P1Indicator.SetActive(!SoloMode);
            P2Slider.SetActive(!SoloMode);
        }

        private void SwapInputScheme(ref PlayerController.InputType type, int modifier, TMP_Text text) {
            
            // TODO: REWORK TO AVOID DUPLICATE INPUTS
            var updatedValue = type + modifier;
            if (updatedValue < 0)
                type = PlayerController.InputType.Controller1;
            else if ((float) updatedValue >= Enum.GetNames(typeof(PlayerController.InputType)).Length - 1)
                type = PlayerController.InputType.Keyboard;
            else
                type = updatedValue;

            if ((P1InputType == PlayerController.InputType.Controller1 ||
                 P1InputType == PlayerController.InputType.Controller1) &&
                updatedValue == PlayerController.InputType.Controller1) 
            {
                P2InputType = PlayerController.InputType.Controller2;
            } else if (P1InputType == P2InputType)
                SwapInputScheme(ref type, +1, text);

            FormatInput(type, text);
        }

        private void FormatInput(PlayerController.InputType type, TMP_Text text) {
            switch (type) {
                case PlayerController.InputType.Controller1: case PlayerController.InputType.Controller2:
                    text.text = "controller";
                    break;
                case PlayerController.InputType.Keyboard:
                    text.text = "keyboard a";
                    break;
                case PlayerController.InputType.KeyboardLeft:
                    text.text = "keyboard b";
                    break;
                case PlayerController.InputType.KeyboardRight:
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
                return;
            }

            SceneLoadingParameters.SoloMode = SoloMode;
            SceneLoadingParameters.Name = NameInputField.text;
            
            SceneLoadingParameters.PlayerOneInputs = P1InputType;
            SceneLoadingParameters.PlayerTwoInputs = SoloMode ? SceneLoadingParameters.PlayerOneInputs : P2InputType;
            
            SceneManager.LoadScene("Level1");
        }

        private void SwapBackground(BackgroundType type) {
            BackgroundNormal.SetActive(type == BackgroundType.Normal);
            BackgroundBlurry.SetActive(type == BackgroundType.Blurry);
            BackgroundNoChar.SetActive(type == BackgroundType.NoChar);
        }

        private enum BackgroundType {
            Normal,
            Blurry,
            NoChar
        }
    }
}