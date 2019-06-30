using System.Collections;
using System.Numerics;
using Objects.Entities;
using Objects.Projectiles;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "KamikazeAbility", menuName = "Abilities/KamikazeAbility")]
    public class KamikazeAbility : BaseAbility {
        
        [SerializeField] private LayerMask TargetingLayerMask;
        [SerializeField] private float RushSpeed;
        [SerializeField] private float MinDistanceToExplode;

        [Header("FX")] 
        [SerializeField] private GameObject FX;
        private GameObject m_FX;
        [SerializeField] private float FXLifetime;
        
        public override BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            KamikazeAbility instance = base.Init(weaponRig, projectileRig, caster) as KamikazeAbility;
            if (instance == null) return null;

            instance.TargetingLayerMask = TargetingLayerMask;
            instance.RushSpeed = RushSpeed;
            instance.MinDistanceToExplode = MinDistanceToExplode;

            Transform t = caster.transform;
            instance.m_FX = Instantiate(FX, t.position, Quaternion.identity, t.transform);
            instance.m_FX.SetActive(false);

            instance.FXLifetime = FXLifetime;
     
            return instance;
        }
        
        public override IEnumerator Fire() {
            Rigidbody2D rb = caster.GetComponent<Rigidbody2D>();
            Transform t = caster.transform;
            Vector2 direction = weaponRig.right;
            
            RaycastHit2D hit = Physics2D.Raycast(t.position, direction, Mathf.Infinity, TargetingLayerMask);
            if (hit.collider != null) {
                
                CircleExplodeProjectile projectile = caster.GetComponent<CircleExplodeProjectile>();
                projectile.enabled = true;
                while (!caster.IsDead && caster.gameObject != null) {
                    rb.AddForce(direction * RushSpeed);
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return null;
        }
    }
}