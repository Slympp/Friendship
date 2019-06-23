﻿using System;
using Abilities;
using Entities.Players;
using UnityEngine;

namespace Objects.Entities.Players {
    
    public class PlayerController : Entity {
        
        [Header("Player")]
        [SerializeField] private int PlayerId = 1;

        // TODO: Implement in code
        private InputType InputSource { 
            set => m_InputSource = value.ToString();
        }

        [SerializeField] string m_InputSource = "Keyboard";

        [Header("Abilities")]
        [SerializeField] private Transform HeadRig;
        [SerializeField] private Transform WeaponRig;
        [SerializeField] private Transform ProjectileRig;

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

        private PlayerAnimatorController m_PlayerAnimatorController;
        private PlayerFXController m_PlayerFXController;

        void Awake() {
            
            Init();

            Transform parent = transform.parent;
            m_PlayerMovementController = parent.GetComponent<PlayerMovementController>();
            m_PlayerFXController = parent.GetComponent<PlayerFXController>();
            
            m_PlayerAnimatorController = parent.GetComponent<PlayerAnimatorController>();
            m_PlayerAnimatorController.Init(GetComponent<Animator>());
            
            m_TargetWeaponRotation = WeaponRig.rotation;
            m_CachedMovement = m_Movement;

            UpdateAbility(BaseAbility.AbilityType.Default);
            UpdateAbility(BaseAbility.AbilityType.Offensive);
            UpdateAbility(BaseAbility.AbilityType.Support);
        }

        void Update() {
            m_Movement = PlayerInputController.Movement(m_InputSource).normalized;
            
            UpdateAiming();

            if (PlayerInputController.Shoot(m_InputSource) && !m_DefaultAbility.OnCooldown) {
                m_PlayerAnimatorController.TriggerShooting();
                TriggerAbility(m_DefaultAbility);
                
            } else if (PlayerInputController.OffensiveAbility(m_InputSource) && !m_OffensiveAbility.OnCooldown) {
                TriggerAbility(m_OffensiveAbility);
                
            } else if (PlayerInputController.SupportAbility(m_InputSource) && !m_SupportAbility.OnCooldown) {
                TriggerAbility(m_SupportAbility);
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
        
        void TriggerAbility(BaseAbility ability) {
            StartCoroutine(ability.TriggerCooldown());
            StartCoroutine(ability.Fire());
        }

        protected override void UpdateMovement() {
            m_PlayerMovementController.Move(m_Movement.x, m_InputSource);
        }

        public override void Heal(int value) {
            int oldHealth = CurrentHealth;
            base.Heal(value);

            if (oldHealth != CurrentHealth) {
                m_PlayerFXController.ToggleHealingAura();
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
