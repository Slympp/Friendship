using System;
using Abilities;
using Objects.Entities.Players.Inputs;
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
        [SerializeField] private Transform WeaponRig;
        [SerializeField] private Transform ProjectileRig;
        [SerializeField] private float WeaponRotationSpeed = 1f;

        public BaseAbility DefaultAbility;
        public BaseAbility OffensiveAbility;
        public BaseAbility SupportAbility;
        
        private BaseAbility m_DefaultAbility;
        private BaseAbility m_OffensiveAbility;
        private BaseAbility m_SupportAbility;

        private Vector2 m_Movement;
        private Vector2 m_CachedMovement;
        private Quaternion m_TargetRotation;
        
        private PlayerMovementController m_PlayerMovementController;
        private bool m_OnCooldown;
    
        void Awake() {
            
            Init();
            
            m_PlayerMovementController = GetComponent<PlayerMovementController>();
            m_TargetRotation = WeaponRig.rotation;
            m_CachedMovement = m_Movement;
            
            UpdateAbility(BaseAbility.AbilityType.Default);
            UpdateAbility(BaseAbility.AbilityType.Offensive);
            UpdateAbility(BaseAbility.AbilityType.Support);
        }

        void Update() {
            m_Movement = InputController.Movement(m_InputSource).normalized;
            
            UpdateAiming();

            if (InputController.Shoot(m_InputSource) && !m_DefaultAbility.OnCooldown) {
                TriggerAbility(m_DefaultAbility);
            } else if (InputController.OffensiveAbility(m_InputSource) && !m_OffensiveAbility.OnCooldown) {
                TriggerAbility(m_OffensiveAbility);
            } else if (InputController.SupportAbility(m_InputSource) && !m_SupportAbility.OnCooldown) {
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

                float yAngle = m_Movement.x.Equals(m_CachedMovement.x) ? 
                    WeaponRig.eulerAngles.y :
                    m_Movement.x <= -0.01f ? 
                        -180 : 0;
                
                float clampedZ = (float)Math.Round(m_Movement.y * 2, MidpointRounding.AwayFromZero) / 2;
                float zAngle = clampedZ.Normalize(-90, 90, -1, 1);

                m_TargetRotation = Quaternion.Euler(0, yAngle, zAngle);
                m_CachedMovement = m_Movement;
            }

            WeaponRig.rotation = Quaternion.Slerp(WeaponRig.rotation, m_TargetRotation, WeaponRotationSpeed * Time.deltaTime);
        }
        
        void TriggerAbility(BaseAbility ability) {
            StartCoroutine(ability.TriggerCooldown());
            StartCoroutine(ability.Fire());
        }

        protected override void UpdateMovement() {
            m_PlayerMovementController.Move(m_Movement.x, m_InputSource);
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
