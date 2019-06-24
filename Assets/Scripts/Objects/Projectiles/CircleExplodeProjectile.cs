using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    public class CircleExplodeProjectile : BaseProjectile {

        [SerializeField] private float Radius;
        [SerializeField] private float OnDirectHitDamage;
        [SerializeField] private Vector2 OnDistanceDamage;
        
        protected override void OnTriggerEnter2D(Collider2D c) {

            if (CollideMask(c)) {

                if (c.CompareTag("Ground") && !c.transform.root.CompareTag("Player")) {
                    
                    if (ShowOnHitGround)
                        OnHitEffect(transform.position);
                    
                    OnAreaEffect();
                    DestroySelf();
                    
                } else if (HitEnemies && c.CompareTag("Enemy")) {
                    OnHitEffect(c.transform.position);
                    ApplyEffect(c.gameObject.GetComponent<Entity>(), TargetType.Enemy);
                    if (!Pierce)
                        DestroySelf();
                    
                } else if (HitPlayers && c.CompareTag("Player")) {
                    OnHitEffect(c.transform.position);
                    ApplyEffect(c.gameObject.GetComponent<Entity>(), TargetType.Ally);
                    if (!Pierce)
                        DestroySelf();
                }
            }
        }

        private void OnAreaEffect() {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Radius);
            foreach (Collider2D t in colliders) {
                if (t.gameObject == gameObject) continue;

                if (HitEnemies && t.CompareTag("Enemy"))
                    ApplyEffect(t.gameObject.GetComponent<Entity>(), TargetType.Enemy, GetMultiplierFromDistance(t.transform.position));
                else if (HitPlayers && t.CompareTag("Player"))
                    ApplyEffect(t.gameObject.GetComponent<Entity>(), TargetType.Ally, GetMultiplierFromDistance(t.transform.position));
            }
        }

        private float GetMultiplierFromDistance(Vector2 position) {
            return Vector2.Distance(transform.position, position).Normalize(1, 0, 0, Radius);
        }

        protected override void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1) {
            
            if (target != null) {
                target.Damage(
                    multiplier.Equals(1)
                        ? Mathf.FloorToInt(OnDirectHitDamage + OnDistanceDamage.y)
                        : Mathf.FloorToInt(multiplier.Normalize(OnDistanceDamage.x, OnDistanceDamage.y, 0, 1)), 
                    Caster);
            }
        }
    }
}