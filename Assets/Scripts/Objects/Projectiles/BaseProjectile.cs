using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    
    [RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
    public abstract class BaseProjectile : MonoBehaviour {

        [Header("Base")]
        [SerializeField] private LayerMask CollisionMask;
        [SerializeField] protected bool HitPlayers;
        [SerializeField] protected bool HitEnemies;
        [SerializeField] protected bool Pierce;

        [Header("FX")] 
        [SerializeField] private GameObject OnImpactFX;
        [SerializeField] protected bool ShowOnHitGround = true;
        [SerializeField] private float OnImpactFXLifetime;

        [SerializeField] private AudioClip OnImpactSound;
        private AudioSource m_Audio;
        
        protected Entity Caster;

        public void Init(Entity caster) {
            Caster = caster;
            m_Audio = GetComponent<AudioSource>();
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D c) {
            
            if (CollideMask(c)) {
                if (c.CompareTag("Ground") && !c.transform.root.CompareTag("Player")) {
                    if (ShowOnHitGround)
                        OnHitEffect(transform.position);
                    
                    DestroySelf();
                    
                } else if (HitEnemies && c.CompareTag("Enemy")) {
                    OnHitEffect(c.transform.position);
                    ApplyEffect(c.gameObject.GetComponent<Entity>(), TargetType.Enemy);
                    if (!Pierce)
                        DestroySelf();
                    
                } else if (HitPlayers && c.CompareTag("Player")) {
                    OnHitEffect(c.transform.position);
                    ApplyEffect(c.gameObject.GetComponentInChildren<Entity>(), TargetType.Ally);
                    if (!Pierce)
                        DestroySelf();
                }
            }
        }

        protected bool CollideMask(Collider2D c) {
            return CollisionMask == (CollisionMask | (1 << c.gameObject.layer));
        }

        private void OnBecameInvisible() {
            Destroy(gameObject);
        }

        protected void DestroySelf() {
            Destroy(gameObject);
        }

        protected void OnHitEffect(Vector3 position) {
            if (OnImpactSound != null)
                m_Audio.PlayOneShot(OnImpactSound);
            
            if (OnImpactFX != null) {
                GameObject impact = Instantiate(OnImpactFX, position, Quaternion.identity);
                Destroy(impact, OnImpactFXLifetime);
            }
        }
        
        protected abstract void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1f);

        public enum TargetType {
            Ally,
            Enemy
        }
    }
}
