﻿using System;
using System.Collections;
using Abilities;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.Entities.Players {
    
    public class PlayerController : Entity {
        [Header("Player")]
        public InputType Input;
        public string InputSource => Input.ToString();

        [SerializeField] private Transform HeadRig;

        public BaseAbility DefaultAbility;
        public BaseAbility OffensiveAbility;
        public BaseAbility SupportAbility;
        
        private BaseAbility m_DefaultAbility;
        private BaseAbility m_OffensiveAbility;
        private BaseAbility m_SupportAbility;

        [SerializeField] private AbilityCooldownIndicator OffensiveCooldownIndicator;
        [SerializeField] private AbilityCooldownIndicator SupportCooldownIndicator;

        private Vector2 m_Movement;
        private Vector2 m_CachedMovement;
        private Quaternion m_TargetHeadRotation;
        private Quaternion m_TargetWeaponRotation;
        
        private PlayerMovementController m_PlayerMovementController;
        private bool m_OnCooldown;

        private PlayerAnimatorController m_PlayerAnimatorController;
        private PlayerFXController m_PlayerFXController;
        
        private bool        m_Aura;
        private IEnumerator m_AuraRoutine;
        private readonly float m_AuraFireRateModifier = 1.3f;
        private readonly float m_AuraMovementSpeedModifier = 1.2f;

        [Header("Respawn")] 
        [SerializeField] private LayerMask EntityMask;
        [SerializeField] private float RespawnRadius;
        [SerializeField] private float TimeToRespawn;
        private float remainingTimeToRespawn;
        private PlayerController cachedPlayerController;

        void Awake() {
            
            Init();

            Transform parent = transform.parent;
            m_PlayerMovementController = parent.GetComponent<PlayerMovementController>();
            m_PlayerFXController = parent.GetComponent<PlayerFXController>();
            
            m_PlayerAnimatorController = parent.GetComponent<PlayerAnimatorController>();
            m_PlayerAnimatorController.Init(GetComponent<Animator>());
            
            m_TargetWeaponRotation = WeaponRig.rotation;
            m_CachedMovement = m_Movement;
            
            GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);
            
            UpdateAbility(BaseAbility.AbilityType.Default);
            UpdateAbility(BaseAbility.AbilityType.Offensive);
            UpdateAbility(BaseAbility.AbilityType.Support);
        }

        void Update() {
            base.Update();
            
            if (GameManager.GameManager.Instance.IsPaused())
                return;
            
            if (IsDead) {
                UpdateRespawn();
                return;
            }
            
            m_Movement = PlayerInputController.Movement(InputSource).normalized;
            
            UpdateAiming();

            if (PlayerInputController.Shoot(InputSource) && !m_DefaultAbility.OnCooldown) {
                m_PlayerAnimatorController.TriggerShooting();
                TriggerAbility(m_DefaultAbility, GetAuraFireRateModifier());
                
            } else if (PlayerInputController.OffensiveAbility(InputSource)) {
                
                if (m_OffensiveAbility.OnCooldown) {
                    _entityAudioController.OnCooldown();
                    return;
                }
                
                TriggerAbility(m_OffensiveAbility, GetAuraFireRateModifier());

                float cooldown = m_OffensiveAbility.Cooldown / GetAuraFireRateModifier();
                StartCoroutine(OffensiveCooldownIndicator.ToggleCooldownIndicator(cooldown));

            } else if (PlayerInputController.SupportAbility(InputSource)) {
                
                if (m_SupportAbility.OnCooldown) {
                    _entityAudioController.OnCooldown();
                    return;
                }
                
                TriggerAbility(m_SupportAbility, GetAuraFireRateModifier());
                
                float cooldown = m_SupportAbility.Cooldown / GetAuraFireRateModifier();
                StartCoroutine(SupportCooldownIndicator.ToggleCooldownIndicator(cooldown));
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
            m_PlayerMovementController.Move(m_Movement.x, InputSource, GetAuraSpeedModifier());
        }

        public override void Damage(int value, Entity origin) {
            
            if (m_Shielded) return;
            
            base.Damage(value, origin);
            
            _entityAudioController.OnTakeDamage();
            
            GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);

            if (IsDead) {
                _entityAudioController.OnDeath();
                remainingTimeToRespawn = TimeToRespawn;
                m_PlayerFXController.ToggleDeath(true, gameObject);
            }
        }

        public override void Heal(int value) {
            int oldHealth = CurrentHealth;
            base.Heal(value);

            if (oldHealth != CurrentHealth) {
                _entityAudioController.OnHealed();
                m_PlayerFXController.ToggleHealingAura();
                GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);
            }
        }
        
        public void Aura(float duration) {
            if (m_AuraRoutine != null)
                StopCoroutine(m_AuraRoutine);
            
            m_AuraRoutine = AuraOvertime(duration);
            StartCoroutine(m_AuraRoutine);
        }

        public void OnSwap(bool e) {
            _entityAudioController.ToggleWalk(false);
            m_DefaultAbility.OnSwap(e);
            m_OffensiveAbility.OnSwap(e);
            m_SupportAbility.OnSwap(e);
        }

        void UpdateRespawn() {
            PlayerController found = null;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, RespawnRadius, EntityMask);
            foreach (Collider2D c in colliders) {
                if (c.gameObject == gameObject) continue;

                if (c.CompareTag("Player"))
                    found = c.GetComponentInChildren<PlayerController>();
            }

            if (cachedPlayerController != found)
                cachedPlayerController = found;

            if (cachedPlayerController == null) {
                if (remainingTimeToRespawn != TimeToRespawn) {
                    m_PlayerFXController.ToggleReviveProgressBar(false);
                }
                remainingTimeToRespawn = TimeToRespawn;
            } else {
                if (remainingTimeToRespawn == TimeToRespawn)
                    m_PlayerFXController.ToggleReviveProgressBar(true, TimeToRespawn);
                remainingTimeToRespawn -= Time.deltaTime;
            }

            if (remainingTimeToRespawn <= 0 && cachedPlayerController != null) {
                m_PlayerFXController.ToggleReviveProgressBar(false);
                cachedPlayerController.OnReviveAlly();
                OnGetRevived();
            }
        }

        private void OnReviveAlly() {
            int healthReduction = MaxHealth / 2;
            if (CurrentHealth <= healthReduction)
                CurrentHealth = 1;
            else
                CurrentHealth -= healthReduction;
            GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);
        }

        private void OnGetRevived() {
            _entityAudioController.OnRevive();
            CurrentHealth = MaxHealth / 2;
            m_PlayerFXController.ToggleDeath(false, gameObject);
            GameManager.GameManager.Instance.m_UIManager.UpdateHealthBar(Name, CurrentHealth, MaxHealth);
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

        private void OnBecameInvisible() {
            GameManager.GameManager.Instance.MoveToMainPlayer(transform.parent);
        }

        private void OnDrawGizmos() {
            
            if (IsDead) {
                Gizmos.color = cachedPlayerController == null ? Color.red : Color.green;
                Gizmos.DrawWireSphere(transform.position, RespawnRadius);
            }
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
