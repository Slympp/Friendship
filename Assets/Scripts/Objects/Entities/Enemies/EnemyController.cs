using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Objects.Entities.Players;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Objects.Entities.Enemies {
    public abstract class EnemyController : Entity {

        [Header("Targeting")]
        [SerializeField] protected LayerMask TargetingMask;
        [SerializeField] protected float TargetingRange;
        [SerializeField] protected float TargetingAngle;
        [SerializeField] protected float TargetingOffset;
        
        protected PlayerController m_TargetController;
        protected Transform        m_TargetTransform;

        private const float RefreshDelay = 0.5f;

        [Header("IA")] 
        [SerializeField] private float MovementSpeed;
        [SerializeField] protected float WaypointReachDistance;
        [SerializeField] protected Transform StartWaypoint;
        [SerializeField] protected Transform EndWaypoint;
        protected Transform m_CurrentWaypoint;
        
        [SerializeField] protected float AttackRange = 5f;
        [SerializeField] private BaseAbility Ability;
        protected BaseAbility m_Ability;
        
        protected EnemyAnimatorController m_EnemyAnimatorController;
        
        private const float       LifeStealMultiplier = 0.5f;
        private       bool        m_Marked;
        private       IEnumerator m_LifeStealRoutine;

        protected void Awake() {
            Init();

            m_EnemyAnimatorController = GetComponent<EnemyAnimatorController>();
            m_EnemyAnimatorController.Init(GetComponent<Animator>());
            
            m_Ability = Ability.Init(WeaponRig, ProjectileRig, this);
            
            m_CurrentWaypoint = StartWaypoint;
            InvokeRepeating(nameof(SetTarget), 0, RefreshDelay);
        }

        protected void Attack() {
            m_EnemyAnimatorController.SetMoving(false);
            if (!m_Ability.OnCooldown) {
                m_EnemyAnimatorController.TriggerShooting();
                TriggerAbility(m_Ability);
            }
        }
        
        protected void MoveTowards(Vector2 targetPosition) {
            m_EnemyAnimatorController.SetMoving(true);
            _transform.right = targetPosition - (Vector2)_transform.position;
            _rigidbody2D.AddForce(_transform.right * MovementSpeed);
        }

        public override void Damage(int value, Entity origin) {
            int oldHealth = CurrentHealth;
            base.Damage(value, origin);

            if (m_Marked && origin != null) {
                int healValue = Mathf.FloorToInt((oldHealth - CurrentHealth) * LifeStealMultiplier);
                origin.Heal(healValue);
            }
            
            if (IsDead) {
                Debug.Log($"Enemy {Name} is dead");
                Destroy(gameObject);
            }
        }
        
        public void Mark(float duration) {
            if (m_LifeStealRoutine != null)
                StopCoroutine(m_LifeStealRoutine);
            
            m_LifeStealRoutine = MarkOvertime(duration);
            StartCoroutine(m_LifeStealRoutine);
        }
        
        public IEnumerator MarkOvertime(float duration) {
            float elapsed = 0;

            m_Marked = true;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_Marked = false;
        }
        
        protected void SetTarget() {
            
            m_TargetController = null;
            m_TargetTransform = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, TargetingRange, TargetingMask);
            List<PlayerController> targets = new List<PlayerController>();
            foreach (Collider2D c in colliders) {
                if (c.gameObject == gameObject || c.CompareTag("Enemy")) continue;

                // Check angle
                Vector2 targetDir = (c.transform.position - _transform.position).normalized;
                if (Vector2.Angle(targetDir, transform.right) > TargetingAngle) continue;

                // Check LOS
                RaycastHit2D hit = Physics2D.Raycast(_transform.position, targetDir, AttackRange);
                if (!hit.collider.CompareTag("Player")) continue;
                
                PlayerController playerController = c.gameObject.GetComponentInChildren<PlayerController>();
                if (playerController != null)
                    targets.Add(playerController);
            }

            float maxDistance = TargetingRange;
            foreach (PlayerController p in targets) {
                float distance = Vector2.Distance(p.transform.position, _transform.position);
                if (distance < maxDistance) {
                    maxDistance = distance;
                    m_TargetController = p;
                    m_TargetTransform = m_TargetController.transform;
                }
            }

            if (m_TargetController != null) {
                Vector3 position = m_TargetTransform.position;
                position.y += TargetingOffset;
                Vector2 targetDir = position - _transform.position;
                WeaponRig.localEulerAngles = new Vector3(0f, 0, Vector2.SignedAngle(_transform.right, targetDir));
            }
        }

        protected Vector2 GetWaypoint(float x) {
            return new Vector2(x, transform.position.y);
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(GetWaypoint(StartWaypoint.position.x), GetWaypoint(EndWaypoint.position.x));

            if (m_CurrentWaypoint != null) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_CurrentWaypoint.position, 0.2f);
            }
            
            Gizmos.color = Color.cyan;
            Vector3 right = transform.right;
            Vector3 position = transform.position;
            var topLine = Quaternion.Euler(0, 0, TargetingAngle) * right * TargetingRange + position;
            Debug.DrawLine(position, topLine, Color.cyan);
            var bottomLine = Quaternion.Euler(0, 0, -TargetingAngle) * right * TargetingRange + position;
            Debug.DrawLine(position, bottomLine, Color.cyan);
        }
    }
}