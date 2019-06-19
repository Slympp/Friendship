using System;
using System.Collections;
using UnityEngine;

namespace Entities.Players {
    
    public class PlayerController : MonoBehaviour {
        
        [Header("Player")]
        [SerializeField] private int PlayerId = 1;

        private InputType InputSource { 
            set => m_InputSource = value.ToString();
        }

        // TODO: Remove SerializedField & set input type from UI
        [SerializeField] string m_InputSource = "Keyboard";
        
        [Header("Weapon Testing")]
        [SerializeField] private Transform WeaponRoot;
        [SerializeField] private Transform ProjectileOrigin;
        [SerializeField] private float WeaponRotationSpeed = 1f;
        [SerializeField] private float WeaponCooldown = 0.5f;
        [SerializeField] private GameObject WeaponProjectile;

        private const int MovementSteps = 8;
        private Vector2 m_Movement;
        private Vector2 m_CachedMovement;
        private Quaternion m_TargetRotation;
        
        private PlayerMovementController m_PlayerMovementController;
        private bool m_OnCooldown;
    
        void Awake() {
            m_PlayerMovementController = GetComponent<PlayerMovementController>();
            m_TargetRotation = WeaponRoot.rotation;
            m_CachedMovement = m_Movement;
        }

        void Update() {
            m_Movement = InputController.Movement(m_InputSource).normalized;
            
            UpdateAiming();

            if (InputController.Shoot(m_InputSource) && !m_OnCooldown) {
                StopCoroutine(nameof(Shoot));
                m_OnCooldown = true;
                StartCoroutine(nameof(Shoot));
            }
        }

        void FixedUpdate() {
            UpdateMovement();
        }

        void UpdateAiming() {

            if (m_Movement != Vector2.zero && m_Movement != m_CachedMovement) {

                float yAngle = m_Movement.x.Equals(m_CachedMovement.x) ? 
                    WeaponRoot.eulerAngles.y :
                    m_Movement.x <= -0.01f ? 
                        -180 : 0;
                
                float clampedZ = (float)Math.Round(m_Movement.y * 2, MidpointRounding.AwayFromZero) / 2;
                float zAngle = NormalizeAngle(clampedZ, -90, 90, -1, 1);

                m_TargetRotation = Quaternion.Euler(0, yAngle, zAngle);
                m_CachedMovement = m_Movement;
            }

            WeaponRoot.rotation = Quaternion.Slerp(WeaponRoot.rotation, m_TargetRotation, WeaponRotationSpeed * Time.deltaTime);
        }

        private float NormalizeAngle(float v, float a, float b, float min, float max) {
            return (b - a) * ((v - min) / (max - min)) + a;
        }
        private IEnumerator Shoot() {
            float elapsed = 0;

            Instantiate(WeaponProjectile, ProjectileOrigin.position, Quaternion.Euler(0, 0, WeaponRoot.eulerAngles.z));
            while (elapsed < WeaponCooldown) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_OnCooldown = false;
        }

        void UpdateMovement() {
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
