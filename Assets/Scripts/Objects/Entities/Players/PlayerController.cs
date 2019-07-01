using System;
using System.Collections;
using Abilities;
using UnityEngine;

namespace Objects.Entities.Players {
    
    public class PlayerController : Entity {
        
        [Header("Player")]
        [SerializeField] private int PlayerId = 1;

        // TODO: Implement in code
        private InputType InputSource { 
            set => m_InputSource = value.ToString();
        }

        [SerializeField] private string m_InputSource = "Keyboard";
        
        [SerializeField] private Transform HeadRig;

        public BaseAbility DefaultAbility;
        public BaseAbility OffensiveAbility;
        public BaseAbility SupportAbility;
        
        private BaseAbility m_DefaultAbility;
        private BaseAbility m_OffensiveAbility;
        private BaseAbility m_SupportAbility;

        private Vector2 m_Movement;
        private Vector2 m_CachedMovement;
        private Quaternion m_TargetHeadRotation;
        private Quaternion m_TargetWeaponRotation;
        
        private PlayerMovementController m_PlayerMovementController;
        private bool m_OnCooldown;

        private PlayerAnimatorController _mPlayerAnimatorController;
        private PlayerFXController m_PlayerFXController;
        
        private bool        m_Aura;
        private IEnumerator m_AuraRoutine;
        private readonly float m_AuraFireRateModifier = 1.5f;
        private readonly float m_AuraMovementSpeedModifier = 1.2f;

        void Awake() {
            
            Init();

            Transform parent = transform.parent;
            m_PlayerMovementController = parent.GetComponent<PlayerMovementController>();
            m_PlayerFXController = parent.GetComponent<PlayerFXController>();
            
            _mPlayerAnimatorController = parent.GetComponent<PlayerAnimatorController>();
            _mPlayerAnimatorController.Init(GetComponent<Animator>());
            
            m_TargetWeaponRotation = WeaponRig.rotation;
            m_CachedMovement = m_Movement;
            
            GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);

            UpdateAbility(BaseAbility.AbilityType.Default);
            UpdateAbility(BaseAbility.AbilityType.Offensive);
            UpdateAbility(BaseAbility.AbilityType.Support);
        }

        void Update() {
            if (IsDead) return;
            
            m_Movement = PlayerInputController.Movement(m_InputSource).normalized;
            
            UpdateAiming();

            if (PlayerInputController.Shoot(m_InputSource) && !m_DefaultAbility.OnCooldown) {
                _mPlayerAnimatorController.TriggerShooting();
                TriggerAbility(m_DefaultAbility, GetAuraFireRateModifier());
                
            } else if (PlayerInputController.OffensiveAbility(m_InputSource) && !m_OffensiveAbility.OnCooldown) {
                TriggerAbility(m_OffensiveAbility, GetAuraFireRateModifier());
                
            } else if (PlayerInputController.SupportAbility(m_InputSource) && !m_SupportAbility.OnCooldown) {
                TriggerAbility(m_SupportAbility, GetAuraFireRateModifier());
            }
        }

        void UpdateAbility(BaseAbility.AbilityType type) {
            switch (type) {
                case BaseAbility.AbilityType.Default:
                    if (DefaultAbility != null)
                        m_DefaultAbility = DefaultAbility.Init(WeaponRig, ProjectileRig, this);
                    break;
                case BaseAbility.AbilityType.Offensive:
                    if (OffensiveAbility != null)
                        m_OffensiveAbility = OffensiveAbility.Init(WeaponRig, ProjectileRig, this);
                    break;
                case BaseAbility.AbilityType.Support:
                    if (SupportAbility != null)
                        m_SupportAbility = SupportAbility.Init(WeaponRig, ProjectileRig, this);
                    break;
            }
        }

        void UpdateAiming() {

            if (m_Movement != Vector2.zero && m_Movement != m_CachedMovement) {

                m_TargetWeaponRotation = GetUpdatedRotation(-90, 90);
                m_TargetHeadRotation = GetUpdatedRotation(-70, 70);
                m_CachedMovement = m_Movement;
            }

            WeaponRig.localRotation = m_TargetWeaponRotation;
            HeadRig.localRotation = m_TargetHeadRotation;
        }
        
        private Quaternion GetUpdatedRotation(float minAngle, float maxAngle) {
           
            float clampedZ = (float)Math.Round(m_Movement.y * 2, MidpointRounding.AwayFromZero) / 2;
            float zAngle = clampedZ.Normalize(minAngle, maxAngle, -1, 1);

            return Quaternion.Euler(0, 0, zAngle * transform.localScale.x);
        }

        protected override void UpdateMovement() {
            m_PlayerMovementController.Move(m_Movement.x, m_InputSource, GetAuraSpeedModifier());
        }

        public override void Damage(int value, Entity origin) {
            base.Damage(value, origin);
            GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);
        }

        public override void Heal(int value) {
            int oldHealth = CurrentHealth;
            base.Heal(value);

            if (oldHealth != CurrentHealth) {
                m_PlayerFXController.ToggleHealingAura();
            }
        }
        
        public void Aura(float duration) {
            if (m_AuraRoutine != null)
                StopCoroutine(m_AuraRoutine);
            
            m_AuraRoutine = AuraOvertime(duration);
            StartCoroutine(m_AuraRoutine);
        }

        private IEnumerator AuraOvertime(float duration) {
            float elapsed = 0;

            m_Aura = true;
            m_PlayerFXController.ToggleBuffAura(true);
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_PlayerFXController.ToggleBuffAura(false);
            m_Aura = false;
        }

        private float GetAuraSpeedModifier() {
            return m_Aura ? m_AuraMovementSpeedModifier : 1;
        }
        
        private float GetAuraFireRateModifier() {
            return m_Aura ? m_AuraFireRateModifier : 1;
        }

        public enum InputType {
            Keyboard = 0,
            KeyboardLeft,
            KeyboardRight,
            Controller1,
            Controller2
        }
    }
}
