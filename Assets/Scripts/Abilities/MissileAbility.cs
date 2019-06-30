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
            yield return new WaitForSeconds(TriggerDelay);

            Vector3 euler = weaponRig.eulerAngles;
            Quaternion rotation = Quaternion.Euler(0, euler.y, euler.z);
            m_Projectile = Instantiate(BaseProjectile, projectileRig.position, rotation).GetComponent<BaseProjectile>();
            m_Projectile.Init(caster);
            
            caster.Projectiles.Add(m_Projectile.gameObject);

            Transform projectileTransform = m_Projectile.transform;
            while (projectileTransform != null) {
                projectileTransform.Translate(MovementSpeed * Time.deltaTime * projectileTransform.right, Space.World);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
