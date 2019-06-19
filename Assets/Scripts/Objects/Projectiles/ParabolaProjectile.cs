using UnityEngine;

namespace Objects.Projectiles {
    public class ParabolaProjectile : BaseProjectile {

        [Header("Parameters")]
        [SerializeField] private Vector2 Force;

        private Rigidbody2D m_Rigidbody2D;

        void Awake() {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Rigidbody2D.AddRelativeForce(new Vector2(Force.x, Force.y), ForceMode2D.Impulse);
        }
        
        protected override void ApplyEffect(GameObject target, bool isEnemy) {
            Destroy(target);
            // target.GetComponent<EntityController>
        }
    }
}
