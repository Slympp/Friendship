using System.Collections;
using Objects.Entities;
using Objects.Entities.Players;
using UnityEngine;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "RaycastAbility", menuName = "Abilities/RaycastAbility")]
    public class RaycastAbility : BaseAbility {
        [SerializeField] private LayerMask GroundLayerMask;
        [SerializeField] private LayerMask TargetLayerMask;
        [SerializeField] private GameObject Ray;
        [SerializeField] private Vector2 Width;
        [SerializeField] private float ChargeTime;
        [SerializeField] private int DamageValue;
        
        public override BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            RaycastAbility instance = base.Init(weaponRig, projectileRig, caster) as RaycastAbility;
            if (instance == null) return null;
            
            instance.GroundLayerMask = GroundLayerMask;
            instance.TargetLayerMask = TargetLayerMask;
            instance.Ray = Ray;
            instance.Width = Width;
            instance.ChargeTime = ChargeTime;
            instance.DamageValue = DamageValue;
            
            return instance;
        }
        
        public override IEnumerator Fire() {
            yield return new WaitForSeconds(TriggerDelay);
            
            Transform t = caster.transform;
            Vector2 initialDirection = weaponRig.right;
            RaycastHit2D hit = Physics2D.Raycast(weaponRig.position, initialDirection, Mathf.Infinity, GroundLayerMask);
            if (hit.collider != null) {
                
                GameObject obj = Instantiate(Ray, t.transform.position, Quaternion.identity, t);

                if (obj != null) {
                    caster.Projectiles.Add(obj);
                    LineRenderer line = obj.GetComponent<LineRenderer>();
                    line.SetPosition(0, projectileRig.transform.position);
                    line.SetPosition(1, hit.point);

                    float elapsed = 0;
                    while (elapsed < ChargeTime) {
                        line.startWidth = line.endWidth = elapsed.Normalize(Width.x, Width.y, 0, ChargeTime);
                        elapsed += Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                    }
                    
                    hit = Physics2D.Raycast(weaponRig.position, initialDirection, Mathf.Infinity, TargetLayerMask);

                    PlayerController target = null;
                    if (hit.collider != null) target = hit.collider.GetComponentInChildren<PlayerController>();
                    
                    if (target != null) {
                        // TODO: spawn impact FX on target.postion
                        target.Damage(DamageValue, caster);
                    } else {
                        // TODO: spawn impact FX on hit.point
                    }
                    
                    Destroy(obj);
                }
            }
            
            yield return null;
        }
    }
}