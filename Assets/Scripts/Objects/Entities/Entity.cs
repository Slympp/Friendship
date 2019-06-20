using System.Collections;
using UnityEngine;

namespace Objects.Entities {
    public abstract class Entity : MonoBehaviour {

        [Header("Entity")] 
        [SerializeField] protected string Name;
        
        [SerializeField] private int MaxHealth;
        private int m_CurrentHealth;
        private bool m_Rooted;
        
        protected bool IsDead => m_CurrentHealth == 0;

        protected void Init() {
            m_CurrentHealth = MaxHealth;
        }
        
        void FixedUpdate() {
            if (!IsDead && !m_Rooted)
                UpdateMovement();
        }
        
        public virtual void Damage(int value) {

            if (!IsDead) {
                m_CurrentHealth -= value;
                if (m_CurrentHealth < 0)
                    m_CurrentHealth = 0;
            
                Debug.Log($"{Name} damaged {m_CurrentHealth}/{MaxHealth}");
            }
        }

        public void Heal(int value) {

            if (!IsDead) {
                m_CurrentHealth += value;
                if (m_CurrentHealth > MaxHealth)
                    m_CurrentHealth = MaxHealth;
            
                Debug.Log($"{Name} healed {m_CurrentHealth}/{MaxHealth}");
            }
        }
        
        public void Root(float duration) {

            StopCoroutine(nameof(RootOvertime));
            m_Rooted = true;
            StartCoroutine(RootOvertime(duration));
        }
        
        private IEnumerator RootOvertime(float duration) {
            float elapsed = 0;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            m_Rooted = false;
        }
        
        protected abstract void UpdateMovement();
    }
}