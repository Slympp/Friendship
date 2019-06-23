using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseProjectile : MonoBehaviour {

        [Header("Base")]
        [SerializeField] private LayerMask CollisionMask;
        [SerializeField] protected bool HitPlayers;
        [SerializeField] protected bool HitEnemies;
        [SerializeField] protected bool Pierce;

        protected Entity Caster;

        public void Init(Entity caster) {
            Caster = caster;
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D c) {
            
            if (CollideMask(c)) {
                if (c.CompareTag("Ground") && !c.transform.root.CompareTag("Player")) {
                    // TODO: Add impact FX
                    DestroySelf();
                } else if (HitEnemies && c.CompareTag("Enemy")) {
                    ApplyEffect(c.gameObject.GetComponent<Entity>(), TargetType.Enemy);
                    if (!Pierce)
                        DestroySelf();
                } else if (HitPlayers && c.CompareTag("Player")) {
                    ApplyEffect(c.gameObject.GetComponent<Entity>(), TargetType.Ally);
                    if (!Pierce)
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

        protected abstract void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1f);

        public enum TargetType {
            Ally,
            Enemy
        }
    }
}
