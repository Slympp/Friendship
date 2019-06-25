using System.Collections;
using Objects.Entities;
using UnityEngine;

namespace Abilities {
    public abstract class BaseAbility : ScriptableObject {

        public string Name;
        public string Description;
        public float Cooldown;
        public float TriggerDelay;
        public AbilityType Type;

        protected Transform WeaponRig;
        protected Transform ProjectileRig;
        protected Entity Caster;
        
        public bool OnCooldown { get; private set; }

        public virtual BaseAbility Init(Transform weaponRig, Transform projectileRig, Entity caster) {

            BaseAbility instance = CreateInstance(GetType()) as BaseAbility;
            if (instance == null) return null;
            
            instance.Name = Name;
            instance.Description = Description;
            instance.Cooldown = Cooldown;
            instance.Type = Type;
            
            instance.WeaponRig = weaponRig;
            instance.ProjectileRig = projectileRig;
            instance.Caster = caster;

            return instance;
        }
        
        public abstract IEnumerator Fire();

        public IEnumerator TriggerCooldown() {
            float elapsed = 0;
            OnCooldown = true;
                
            while (elapsed < Cooldown) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            OnCooldown = false;
        }

        public enum AbilityType {
            Default,
            Offensive,
            Support
        }
    }
}
