using System;
using System.Collections;
using Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class AbilityCooldownIndicator : MonoBehaviour {

        private Image Indicator;
        private TMP_Text Timer;

        void Awake() {
            Indicator = GetComponentInChildren<Image>();
            Timer = GetComponentInChildren<TMP_Text>();
        }

        public IEnumerator ToggleCooldownIndicator(float duration) {
            
            duration += 1;
            
            Timer.text = Mathf.FloorToInt(duration).ToString();
            Indicator.enabled = true;
            Timer.enabled = true;

            while (duration > 1) {
                Timer.text = Mathf.FloorToInt(duration).ToString();

                duration -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            Indicator.enabled = false;
            Timer.enabled = false;
        }
    }
}