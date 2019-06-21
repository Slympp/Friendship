using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    public class RootGrenadeProjectile : CircleExplodeProjectile {

        [SerializeField] private float RootDuration;
        protected override void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1) {
            base.ApplyEffect(target, targetType, multiplier); 
            
            if (target != null)
                target.Root(RootDuration);
        }
    }
}