using UnityEngine;

namespace Projectiles {
    public class MissileProjectile : BaseProjectile {

        [Header("Parameters")]
        [SerializeField] private float MovementSpeed;
        
        void FixedUpdate() {
            transform.Translate(MovementSpeed * Time.deltaTime * transform.right, Space.World);
        }
        
        protected override void ApplyEffect(GameObject target, bool isEnemy) {
            Destroy(target);
            // target.GetComponent<EntityController>
        }
    }
}
