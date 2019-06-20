using System.Collections;
using Objects.Projectiles;
using UnityEngine;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "ParabolaAbility", menuName = "Abilities/ParabolaAbility")]
    public class ParabolaAbility : BaseAbility {

        [SerializeField] private GameObject BaseProjectile;
        [SerializeField] private Vector2 ForceImpulse = Vector2.one;
        [SerializeField] private float   Magnitude    = 10f;
        
        private BaseProjectile m_Projectile;
        private Rigidbody2D m_Rigidbody2D;

        public override BaseAbility Init(Transform weaponRig, Transform projectileRig) {

            ParabolaAbility instance = base.Init(weaponRig, projectileRig) as ParabolaAbility;
            if (instance == null) return null;
            
            // TODO: Create ProjectilePool
            instance.BaseProjectile = BaseProjectile;
            instance.ForceImpulse = ForceImpulse;
            instance.Magnitude = Magnitude;
            
            return instance;
        }
        
        public override IEnumerator Fire() {
            Quaternion rotation = Quaternion.Euler(0, WeaponRig.eulerAngles.y, ProjectileRig.eulerAngles.z);
            m_Projectile = Instantiate(BaseProjectile, WeaponRig.position, rotation).GetComponent<BaseProjectile>();

            if (m_Projectile != null) {
                m_Rigidbody2D = m_Projectile.GetComponent<Rigidbody2D>();
            
                if (m_Rigidbody2D != null) {
                    Vector2 direction =  ForceImpulse.x * Magnitude * m_Projectile.transform.right;
                    m_Rigidbody2D.AddForce(direction, ForceMode2D.Impulse);
                }
            }

            yield return null;
        }
    }
}
