using System.Collections;
using Objects.Entities;
using Objects.Projectiles;
using UnityEngine;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "MissileAbility", menuName = "Abilities/MissileAbility")]
    public class MissileAbility : BaseAbility {

        [SerializeField] private GameObject BaseProjectile;
        [SerializeField] private float MovementSpeed;
        
        private BaseProjectile m_Projectile;

        public override BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            MissileAbility instance = base.Init(weaponRig, projectileRig, caster) as MissileAbility;
            if (instance == null) return null;
            
            // TODO: Create ProjectilePool
            instance.BaseProjectile = BaseProjectile;
            instance.MovementSpeed = MovementSpeed;
            
            return instance;
        }
        
        public override IEnumerator Fire() {
            Quaternion rotation = Quaternion.Euler(0, WeaponRig.eulerAngles.y, ProjectileRig.eulerAngles.z);
            m_Projectile = Instantiate(BaseProjectile, ProjectileRig.position, rotation).GetComponent<BaseProjectile>();
            m_Projectile.Init(Caster);

            Transform projectileTransform = m_Projectile.transform;
            while (projectileTransform != null) {
                projectileTransform.Translate(MovementSpeed * Time.deltaTime * projectileTransform.right, Space.World);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
