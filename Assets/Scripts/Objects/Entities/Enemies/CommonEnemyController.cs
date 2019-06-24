using System.Collections.Generic;
using Objects.Entities.Players;
using UnityEngine;

namespace Objects.Entities.Enemies {
    
    public class CommonEnemyController : EnemyController {

        [Header("Targeting")]
        [SerializeField] private List<Transform> Path;
        private List<Vector2> m_Waypoints;

        [SerializeField] private LayerMask TargetingMask;
        [Range(0, 2)]
        [SerializeField] private float RefreshDelay = 0.5f;
        [SerializeField] private float TargetingRange;
        [SerializeField] private float TargetingAngle;
        [SerializeField] private float AttackRange = 5f;
        private PlayerController m_TargetController;
        private Transform m_TargetTransform;


        void Awake() {
            base.Awake();
            
            m_Waypoints = new List<Vector2>();
            foreach (Transform t in Path) {
                m_Waypoints.Add(t.position);
            }
            
            InvokeRepeating(nameof(SetTarget), 0, RefreshDelay);
        }
        
        protected override void UpdateMovement() {
            
            if (m_TargetController == null) {
                Debug.Log("Move towards WP");
                MoveTowards(m_Waypoints[0]);
                // Move towards next point
            } else {
                float distance = Vector2.Distance(m_TargetTransform.position, transform.position);
                if (distance < AttackRange /* && Not on cooldown */) {
                    Debug.Log("Attack");
                    // Look At target on Y axis
                    // Attack
                } else {
                    Debug.Log("Move towards target");
                    Vector2 point = ((Vector2) m_TargetTransform.position).NearestPointOnFiniteLine(m_Waypoints[0], m_Waypoints[1]);
                    MoveTowards(point);
                }
            }
        }

        private void SetTarget() {
            
            m_TargetController = null;
            m_TargetTransform = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, TargetingRange, TargetingMask);
            List<PlayerController> targets = new List<PlayerController>();
            foreach (Collider2D c in colliders) {
                if (c.gameObject == gameObject || c.CompareTag("Enemy")) continue;

                Vector2 targetDir = (c.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(targetDir, transform.right);
                
                if (angle > TargetingAngle) continue;

                PlayerController playerController = c.gameObject.GetComponentInChildren<PlayerController>();
                if (playerController != null) {
                    targets.Add(playerController);
                }
            }

            float maxDistance = TargetingRange;
            foreach (PlayerController p in targets) {
                float distance = Vector2.Distance(p.transform.position, transform.position);
                if (distance < maxDistance) {
                    maxDistance = distance;
                    m_TargetController = p;
                    m_TargetTransform = m_TargetController.transform;
                }
            }

            if (m_TargetController != null) {
                Debug.Log($"{Name} target found: {m_TargetController.name}");
            }
        }
    }
}