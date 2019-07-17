using System.Collections;
using Objects.Entities;
using UnityEngine;

namespace Abilities {
    public abstract class BaseAbility : ScriptableObject {

        public string Name;
        public float Cooldown;
        public float TriggerDelay;
        public AbilityType Type;

        protected Transform weaponRig;
        protected Transform projectileRig;
        protected Entity caster;
        
        public bool OnCooldown { get; private set; }

        public virtual BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            BaseAbility instance = CreateInstance(GetType()) as BaseAbility;
            if (instance == null) return null;
            
            instance.Name = Name;
            instance.Cooldown = Cooldown;
            instance.Type = Type;
            instance.TriggerDelay = TriggerDelay;
            
            instance.weaponRig = weaponRig;
            instance.projectileRig = projectileRig;
            instance.caster = caster;

            return instance;
        }
        
        public abstract IEnumerator Fire();

        public IEnumerator TriggerCooldown(float modifier) {
            float elapsed = 0;
            float cooldown = Cooldown / modifier;
            
            OnCooldown = true;
            while (elapsed < cooldown) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            OnCooldown = false;
        }

        public virtual void OnSwap(bool e) { }

        public enum AbilityType {
            Default,
            Offensive,
            Support
        }
    }
}
