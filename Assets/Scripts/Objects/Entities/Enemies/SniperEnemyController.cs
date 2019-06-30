using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Entities.Enemies
{
    public class SniperEnemyController : EnemyController {

        [SerializeField] private LayerMask HidingMask;
        [SerializeField] private float HidingRange;
        [SerializeField] private Collider2D Hitbox;

        private bool m_Hiding;
        
        protected override void UpdateMovement() {
            if (ShouldHide()) {
                ToggleHide(true);
            } else {
                ToggleHide(false);

                if (m_TargetController != null)
                    Attack();
            }
        }

        private bool ShouldHide() {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, HidingRange, HidingMask);
            foreach (Collider2D c in colliders) {
                if (c.CompareTag("Player"))
                    return true;
            }
            return false;
        }

        private void ToggleHide(bool b) {
            if (m_Hiding == b) return;

            m_Hiding = b;
            Hitbox.enabled = !m_Hiding;
            m_EnemyAnimatorController.ToggleHiding(m_Hiding);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, HidingRange);
        }
    }
}