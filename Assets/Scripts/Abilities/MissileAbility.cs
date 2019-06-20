using System.Collections;
using Objects.Projectiles;
using UnityEngine;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "MissileAbility", menuName = "Abilities/MissileAbility")]
    public class MissileAbility : BaseAbility {

        [SerializeField] private GameObject BaseProjectile;
        [SerializeField] private float MovementSpeed;
        
        private BaseProjectile m_Projectile;

        public override BaseAbility Init(Transform weaponRig, Transform projectileRig) {

            MissileAbility instance = base.Init(weaponRig, projectileRig) as MissileAbility;
            if (instance == null) return null;
            
            // TODO: Create ProjectilePool
            instance.BaseProjectile = BaseProjectile;
            instance.MovementSpeed = MovementSpeed;
            
            return instance;
        }
        
        public override IEnumerator Fire() {
            Quaternion rotation = Quaternion.Euler(0, WeaponRig.eulerAngles.y, ProjectileRig.eulerAngles.z);
            m_Projectile = Instantiate(BaseProjectile, WeaponRig.position, rotation).GetComponent<BaseProjectile>();

            Transform projectileTransform = m_Projectile.transform;
            while (projectileTransform != null) {
                projectileTransform.Translate(MovementSpeed * Time.deltaTime * projectileTransform.right, Space.World);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
