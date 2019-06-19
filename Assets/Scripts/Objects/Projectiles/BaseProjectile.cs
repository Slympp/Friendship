using UnityEngine;

namespace Objects.Projectiles {
    
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseProjectile : MonoBehaviour {

        [Header("Base")]
        [SerializeField] private LayerMask CollisionMask;
        [SerializeField] private bool hitPlayers;
        [SerializeField] private bool hitEnemies;
        
        void OnTriggerEnter2D(Collider2D c) {
            if (CollisionMask == (CollisionMask | (1 << c.gameObject.layer))) {

                if (c.CompareTag("Ground")) {
                    DestroySelf();
                } else if (hitEnemies && c.CompareTag("Enemy")) {
                    ApplyEffect(c.gameObject, true);
                } else if (hitPlayers && c.CompareTag("Player")) {
                    ApplyEffect(c.gameObject, false);
                }
            }
        }

        private void OnBecameInvisible() {
            DestroySelf();
        }
        
        protected void DestroySelf() {
            Destroy(gameObject);
        }
        
        protected abstract void ApplyEffect(GameObject target, bool isEnemy);

    }
}
