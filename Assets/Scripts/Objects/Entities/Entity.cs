using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private float OnHitFlashDuration = 0.3f;
        private List<SpriteRenderer> Sprites;
        private IEnumerator m_FlashRoutine;

        private Color m_CurrentColor = Color.white;
        private readonly Color _mFlashColor = Color.red;
        private readonly Color _rootedColor = new Color(0.2116857f, 0.6320754f, 0.2732884f, 1);

        protected void Init() {
            CurrentHealth = MaxHealth;
            Sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
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
                
                FlashOnHit();
                Debug.Log($"{Name} damaged {value} [{updatedHealth}/{MaxHealth}]");

                CurrentHealth = updatedHealth;
            }
        }

        public virtual void Heal(int value) {

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

            m_CurrentColor = Color.white;
            UpdateSpritesColor(m_CurrentColor);
            m_Rooted = false;
        }

        private void FlashOnHit() {
            if (m_FlashRoutine != null)
                StopCoroutine(m_FlashRoutine);
            
            m_FlashRoutine = FlashOnItOvertime();
            StartCoroutine(m_FlashRoutine);
        }

        private IEnumerator FlashOnItOvertime() {
            float elapsed = 0;
            
            if (m_Rooted) {
                m_CurrentColor = _rootedColor;
                UpdateSpritesColor(m_CurrentColor);
            }

            while (elapsed < OnHitFlashDuration * 2) {
                Color color = Color.Lerp(m_CurrentColor, _mFlashColor, Mathf.PingPong(elapsed, OnHitFlashDuration));
                UpdateSpritesColor(color);
                
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        private void UpdateSpritesColor(Color c) {
            foreach (SpriteRenderer r in Sprites) {
                r.color = c;
            }
        }
        
        protected abstract void UpdateMovement();
    }
}