using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    public class DirectProjectile : BaseProjectile {

        [SerializeField] protected int DamageValue;
        
        protected override void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1) {
            if (target != null)
                target.Damage(GetDamageValue(multiplier), Caster);
        }

        protected int GetDamageValue(float multiplier) {
            return Mathf.FloorToInt(DamageValue * multiplier);
        }
    }
}