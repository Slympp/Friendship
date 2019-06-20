using UnityEngine;

namespace Objects.Projectiles {
    
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseProjectile : MonoBehaviour {

        [Header("Base")]
        [SerializeField] private LayerMask CollisionMask;
        [SerializeField] protected bool hitPlayers;
        [SerializeField] protected bool hitEnemies;
        [SerializeField] protected bool pierce;
        
        protected virtual void OnTriggerEnter2D(Collider2D c) {
            
            if (CollideMask(c)) {
                if (c.CompareTag("Ground")) {
                    // TODO: Add impact FX
                    DestroySelf();
                } else if (hitEnemies && c.CompareTag("Enemy")) {
                    ApplyEffect(c.gameObject, TargetType.Enemy);
                    if (!pierce)
                        DestroySelf();
                } else if (hitPlayers && c.CompareTag("Player")) {
                    ApplyEffect(c.gameObject, TargetType.Ally);
                    if (!pierce)
                        DestroySelf();
                }
            }
        }

        protected bool CollideMask(Collider2D c) {
            return CollisionMask == (CollisionMask | (1 << c.gameObject.layer));
        }

        private void OnBecameInvisible() {
            DestroySelf();
        }
        
        protected void DestroySelf() {
            Destroy(gameObject);
        }

        protected abstract void ApplyEffect(GameObject target, TargetType targetType, float multiplier = 1f);

        public enum TargetType {
            Ally,
            Enemy
        }
    }
}
