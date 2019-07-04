using Objects.Entities;
using UnityEngine;

namespace Objects.Projectiles {
    
    public class ComboProjectile : BaseProjectile {

        public float Speed;
        private Vector2 m_Direction = new Vector2(1, -1).normalized;
        
        void Update() {
            transform.Translate(Speed * Time.deltaTime * (Vector3)m_Direction, Space.World);
        }
        
        protected override void ApplyEffect(Entity target, TargetType targetType, float multiplier = 1) {
            target.Damage(1000, Caster);
        }
    }
}