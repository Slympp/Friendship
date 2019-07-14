using System.Collections;
using Objects.Entities;
using UnityEngine;

namespace Abilities {
    
    [CreateAssetMenu(fileName = "ShieldAbility", menuName = "Abilities/ShieldAbility")]
    public class ShieldAbility : BaseAbility {

        [SerializeField] private GameObject Shield;
        [SerializeField] private float Duration;
        [SerializeField] private Vector3 Offset;

        private GameObject m_Shield;
        
        public override BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            ShieldAbility instance = base.Init(weaponRig, projectileRig, caster) as ShieldAbility;
            if (instance == null) return null;
            
            instance.Shield = Shield;
            instance.Duration = Duration;
            instance.Offset = Offset;
            
            instance.m_Shield = Instantiate(instance.Shield, caster.transform);
            instance.m_Shield.transform.localPosition = Offset;
            instance.m_Shield.SetActive(false);
            
            return instance;
        }

        public override IEnumerator Fire() {
            float elapsed = 0;
            
            m_Shield.SetActive(true);
            while (elapsed < Duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_Shield.SetActive(false);
        }

        public override void OnSwap(bool e) {
            if (!e)
                m_Shield.SetActive(false);
        }
    }
}