using System.Linq.Expressions;
using GameManager;
using Objects.Entities.Enemies;
using Objects.Entities.Players;
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
        [SerializeField] private GameObject ButtonsMain;
        [SerializeField] private GameObject ButtonsStart;
        
        [Header("Main Buttons")]
        [SerializeField] private Button ButtonStart;
        [SerializeField] private Button ButtonCredits;
        [SerializeField] private Button ButtonQuit;
        
        [Header("Start Buttons")]
        [SerializeField] private Button ButtonHowToPlay;
        [SerializeField] private Button ButtonSolo;
        [SerializeField] private Button ButtonMulti;
        [SerializeField] private Button ButtonBack;

        void Awake() {
            
            ButtonStart.onClick.AddListener(EnableStartMenu);
            ButtonCredits.onClick.AddListener(Credits);
            ButtonQuit.onClick.AddListener(Quit);
            
            ButtonHowToPlay.onClick.AddListener(HowToPlay);
            ButtonSolo.onClick.AddListener(StartSolo);
            ButtonMulti.onClick.AddListener(StartMulti);
            ButtonBack.onClick.AddListener(EnableMainMenu);
        }
        
        private void EnableStartMenu() {
            
            SwapBackground(BackgroundType.Normal);
            
            ButtonsMain.SetActive(false);
            ButtonsStart.SetActive(true);
            
            CreditsElement.SetActive(false);
        }
        
        private void Credits() {
            SwapBackground(BackgroundType.Blurry);
            
            CreditsElement.SetActive(true);
        }

        private void Quit() {
            Application.Quit();
        }

        private void HowToPlay() {
            
        }

        private void StartSolo() {
            SceneLoadingParameters.SoloMode = true;
            SceneLoadingParameters.Name = "Name";
            SceneLoadingParameters.PlayerOneInputs = PlayerController.InputType.Keyboard;

            SceneManager.LoadScene("Level1");
        }
        
        private void StartMulti() {
            SceneLoadingParameters.SoloMode = false;
            SceneLoadingParameters.Name = "Name";
            SceneLoadingParameters.PlayerOneInputs = PlayerController.InputType.Keyboard;
            SceneLoadingParameters.PlayerTwoInputs = PlayerController.InputType.Controller1;
            
            SceneManager.LoadScene("Level1");
        }

        private void EnableMainMenu() {
            
            SwapBackground(BackgroundType.Normal);
            
            ButtonsMain.SetActive(true);
            ButtonsStart.SetActive(false);
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