
using UnityEngine;

namespace Objects.Entities.Enemies {
    public class KamikazeEnemyController : EnemyController {
        private bool hasAttack;
        
        protected override void UpdateMovement() {

            if (!hasAttack) {
                if (m_TargetController == null) {
                    Patrol(); 
                } else {
                    Attack();
                    hasAttack = true;
                }
            }
        }
    }
}