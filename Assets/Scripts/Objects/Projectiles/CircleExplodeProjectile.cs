using System.Runtime.InteropServices.WindowsRuntime;
using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    public class CircleExplodeProjectile : BaseProjectile {

        [SerializeField] private float Radius;
        [SerializeField] private float OnDirectHitDamage;
        [SerializeField] private Vector2 OnDistanceDamage;
        
        protected override void OnTriggerEnter2D(Collider2D c) {

            if (CollideMask(c)) {

                if (c.CompareTag("Ground")) {
                    OnAreaEffect();
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

        private void OnAreaEffect() {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, Radius);
            foreach (Collider2D t in colliders) {
                if (t.gameObject == gameObject) continue;

                if (hitEnemies && t.CompareTag("Enemy"))
                    ApplyEffect(t.gameObject, TargetType.Enemy, GetMultiplierFromDistance(t.transform.position));
                else if (hitPlayers && t.CompareTag("Player"))
                    ApplyEffect(t.gameObject, TargetType.Ally, GetMultiplierFromDistance(t.transform.position));
            }
        }

        private float GetMultiplierFromDistance(Vector2 position) {
            return Vector2.Distance(transform.position, position).Normalize(1, 0, 0, Radius);
        }

        protected override void ApplyEffect(GameObject target, TargetType targetType, float multiplier = 1) {
            
            Entity entity = target.GetComponent<Entity>();
            if (entity == null) return;

            entity.Damage(multiplier.Equals(1)
                ? Mathf.FloorToInt(OnDirectHitDamage + OnDistanceDamage.y)
                : Mathf.FloorToInt(multiplier.Normalize(OnDistanceDamage.x, OnDistanceDamage.y, 0, 1)));
        }
    }
}