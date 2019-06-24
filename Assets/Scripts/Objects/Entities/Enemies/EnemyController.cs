using System.Collections;
using UnityEngine;

namespace Objects.Entities.Enemies {
    public abstract class EnemyController : Entity {

        [SerializeField] private float MovementSpeed;
        private const float LifeStealMultiplier = 0.5f;
        private bool m_Marked;
        private IEnumerator m_LifeStealRoutine;
        private Rigidbody2D m_Rigidbody;
        
        protected void Awake() {
            Init();

            m_Rigidbody = GetComponent<Rigidbody2D>();
        }
        
        protected void MoveTowards(Vector2 position) {
            
            m_Rigidbody.AddForce(position - (Vector2)transform.position * MovementSpeed);
            transform.rotation = Quaternion.Euler(0, m_Rigidbody.velocity.x >= 0.1f ? 0 : 180, 0); 
        }

        public override void Damage(int value, Entity origin) {
            int oldHealth = CurrentHealth;
            base.Damage(value, origin);

            if (m_Marked && origin != null) {
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

            m_Marked = true;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_Marked = false;
        }
    }
}