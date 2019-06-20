using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    public class DirectProjectile : BaseProjectile {

        [SerializeField] private int DamageValue;

        protected override void ApplyEffect(GameObject target, TargetType targetType, float multiplier = 1) {
            target.GetComponent<Entity>().Damage(Mathf.FloorToInt(DamageValue * multiplier));
        }
    }
}