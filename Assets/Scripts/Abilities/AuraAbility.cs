using System.Collections;
using Objects.Entities;
using Objects.Entities.Players;
using UnityEngine;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "AuraAbility", menuName = "Abilities/AuraAbility")]
    public class AuraAbility : BaseAbility {
        
        [SerializeField] private GameObject Indicator;
        private GameObject m_Indicator;
        
        [SerializeField] private float Duration;
        [SerializeField] private float Range;
        [SerializeField] private Vector3 Offset;

        private PlayerController m_Target;
        
        public override BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            AuraAbility instance = base.Init(weaponRig, projectileRig, caster) as AuraAbility;
            if (instance == null) return null;

            Transform t = caster.transform;
            instance.m_Indicator = Instantiate(Indicator, t.position, Quaternion.identity, t);
            instance.m_Indicator.SetActive(false);

            instance.Duration = Duration;
            instance.Range = Range;
            instance.Offset = Offset;
            
            AuraHandler handler = caster.gameObject.AddComponent<AuraHandler>();
            handler.Init(instance, weaponRig);

            return instance;
        }
        
        public override IEnumerator Fire() {
            if (m_Target != null) {
                m_Target.Aura(Duration);
            }
            yield return null;
        }

        public void SetCurrentTarget(PlayerController target) {
            m_Target = target;

            if (m_Target == null) {
                m_Indicator.SetActive(false);
            } else {
                m_Indicator.transform.parent = target.transform;
                m_Indicator.transform.localPosition = Offset;
                m_Indicator.SetActive(true);
            }
        }

        public float GetRange() { return Range; }
    }
}