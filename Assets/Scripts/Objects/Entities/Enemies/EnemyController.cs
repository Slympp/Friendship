using System.Collections;
using UnityEngine;

namespace Objects.Entities.Enemies {
    public class EnemyController : Entity {

        [Range(0, 1)] 
        [SerializeField] private float LifeStealMultiplier = 0.5f;
        [SerializeField] private bool m_LifeStealMarked;
        private IEnumerator m_LifeStealRoutine;
        
        void Awake() {
            Init();
        }

        protected override void UpdateMovement() { }

        public override void Damage(int value, Entity origin) {

            int oldHealth = CurrentHealth;
            base.Damage(value, origin);

            if (m_LifeStealMarked && origin != null) {
                int healValue = Mathf.FloorToInt((oldHealth - CurrentHealth) * LifeStealMultiplier);
                origin.Heal(healValue);
            }
            
            if (IsDead) {
                Debug.Log($"Enemy {Name} is dead");
                Destroy(gameObject);
            }
        }
        
        public void Mark(float duration) {
            if (m_LifeStealRoutine != null)
                StopCoroutine(m_LifeStealRoutine);
            
            m_LifeStealRoutine = MarkOvertime(duration);
            StartCoroutine(m_LifeStealRoutine);
        }
        
        public IEnumerator MarkOvertime(float duration) {
            float elapsed = 0;

            m_LifeStealMarked = true;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_LifeStealMarked = false;
        }
    }
}