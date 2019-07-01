using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Entities.Enemies {
    public class TankEnemyController : EnemyController {
        
        [SerializeField] private LayerMask ShieldMask;
        [SerializeField] private float ShieldRange;

        private List<EnemyController> m_ShieldedEnemies = new List<EnemyController>();
        
        protected override void UpdateMovement() {
            ShieldAround();
            
            if (m_TargetController == null) {
                Patrol();                
            } else {
                if (Vector2.Distance(m_TargetTransform.position, transform.position) < AttackRange) {
                    Attack();
                } else {
                    Vector2 point = ((Vector2) m_TargetTransform.position).NearestPointOnFiniteLine(
                        GetWaypoint(StartWaypoint.position.x), 
                        GetWaypoint(EndWaypoint.position.x)
                    );
                    MoveTowards(point);
                }
            }
        }

        protected override void OnDeath() {
            foreach (EnemyController e in m_ShieldedEnemies) {
                e.Shield(false);
            }
        }

        private void ShieldAround() {
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, ShieldRange, ShieldMask);
            List<EnemyController> enemiesInRange = new List<EnemyController>();

            foreach (Collider2D c in colliders) {
                if (c.CompareTag("Enemy") && c.gameObject != gameObject) {
                    EnemyController enemy = c.GetComponent<EnemyController>();
                    if (enemy == null) return;

                    if (!m_ShieldedEnemies.Contains(enemy)) {
                        enemy.Shield(true);
                        Debug.Log($"{enemy.name} enter shield area");
                        m_ShieldedEnemies.Add(enemy);
                    }
                    
                    enemiesInRange.Add(enemy);
                }
            }

            List<EnemyController> tmpList = m_ShieldedEnemies.ToList();
            foreach (EnemyController e in tmpList) {
                if (!enemiesInRange.Contains(e)) {
                    e.Shield(false);
                    m_ShieldedEnemies.Remove(e);
                }
            }
        }
        
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, ShieldRange);
        }
    }
}