using Objects.Entities;
using Objects.Entities.Enemies;
using UnityEngine;

namespace Objects.Projectiles {
    public class HealingMarkProjectile : DirectProjectile {

        [SerializeField] private float MarkDuration;
        
        protected override void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1) {
            base.ApplyEffect(target, targetType, multiplier);

            if (target is EnemyController controller) {
                controller.Mark(MarkDuration);
            }
        }
    }
}