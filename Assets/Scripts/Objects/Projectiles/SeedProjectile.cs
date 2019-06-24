using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    public class SeedProjectile : CircleExplodeProjectile {

        [SerializeField] private float RootDuration;
        protected override void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1) {
            if (target != null)
                target.Root(RootDuration);
            
            base.ApplyEffect(target, targetType, multiplier); 
        }
    }
}