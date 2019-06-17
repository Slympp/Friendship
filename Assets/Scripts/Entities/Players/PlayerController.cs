using System.Collections;
using UnityEngine;

namespace Entities.Players {
    
    public class PlayerController : MonoBehaviour {
        
        [SerializeField] private Transform WeaponRoot;
        [SerializeField] private Transform ProjectileOrigin;
        [SerializeField] private float WeaponRotationSpeed = 1f;
        [SerializeField] private float WeaponCooldown = 0.5f;
        [SerializeField] private GameObject WeaponProjectile;

        private const int MovementSteps = 8;
        private Vector2 m_Movement;
        private Vector2 m_CachedMovement;
        private Quaternion m_TargetRotation;
        
        private CharacterController2D m_CharacterController;
        private bool m_OnCooldown;
    
        void Awake() {
            m_CharacterController = GetComponent<CharacterController2D>();
            m_TargetRotation = WeaponRoot.rotation;
            m_CachedMovement = m_Movement;
        }

        void Update() {
            m_Movement = InputController.Movement;
            
            UpdateWeaponRotation();

            if (InputController.Shoot && !m_OnCooldown) {
                StopCoroutine(nameof(Shoot));
                m_OnCooldown = true;
                StartCoroutine(nameof(Shoot));
            }
        }

        void FixedUpdate() {
            UpdateMovement();
        }

        void UpdateWeaponRotation() {

            if (m_Movement != Vector2.zero && m_Movement != m_CachedMovement) {
                float angle = Mathf.Atan2(m_Movement.y, m_Movement.x) * Mathf.Rad2Deg;
                float snappedAngle = Mathf.Round(angle * MovementSteps) / MovementSteps;
                m_TargetRotation = Quaternion.Euler(0, 0, snappedAngle);
                m_CachedMovement = m_Movement;
            }

            WeaponRoot.rotation =
                Quaternion.Slerp(WeaponRoot.rotation, m_TargetRotation, WeaponRotationSpeed * Time.deltaTime);
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
            m_CharacterController.Move(m_Movement.x);
        }
    }
}
