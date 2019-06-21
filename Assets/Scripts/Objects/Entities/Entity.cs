using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Objects.Entities {
    public abstract class Entity : MonoBehaviour {

        [Header("Entity")] 
        [SerializeField] protected string Name;
        
        [SerializeField] private int MaxHealth;
        protected int CurrentHealth;

        protected bool IsDead => CurrentHealth == 0;
        
        private bool m_Rooted;
        private IEnumerator m_RootRoutine;

        protected void Init() {
            CurrentHealth = MaxHealth;
        }
        
        void FixedUpdate() {
            if (!IsDead && !m_Rooted)
                UpdateMovement();
        }
        
        public virtual void Damage(int value, Entity origin) {

            if (!IsDead) {
                int updatedHealth = CurrentHealth - value;
                if (updatedHealth < 0)
                    updatedHealth = 0;

                Debug.Log($"{Name} damaged {value} [{updatedHealth}/{MaxHealth}]");

                CurrentHealth = updatedHealth;
            }
        }

        public void Heal(int value) {

            if (!IsDead) {
                CurrentHealth += value;
                if (CurrentHealth > MaxHealth)
                    CurrentHealth = MaxHealth;
            
                Debug.Log($"{Name} healed {value} [{CurrentHealth}/{MaxHealth}]");
            }
        }
        
        public void Root(float duration) {
            if (m_RootRoutine != null)
                StopCoroutine(m_RootRoutine);
            
            m_RootRoutine = RootOvertime(duration);
            StartCoroutine(m_RootRoutine);
        }

        private IEnumerator RootOvertime(float duration) {
            float elapsed = 0;

            m_Rooted = true;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_Rooted = false;
        }
        
        protected abstract void UpdateMovement();
    }
}