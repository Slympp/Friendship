using UnityEngine;

namespace Objects.Entities.Enemies {
    public class EnemyController : Entity {

        void Awake() {
            Init();
        }

        protected override void UpdateMovement() {
            
        }

        public override void Damage(int value) {
            base.Damage(value);

            if (IsDead) {
                Debug.Log($"Enemy {Name} is dead");
                Destroy(gameObject);
            }
        }
    }
}