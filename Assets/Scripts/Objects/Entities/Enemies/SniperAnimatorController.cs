using UnityEngine;

namespace Objects.Entities.Enemies
{
    public class SniperAnimatorController : EnemyAnimatorController {
        private readonly int Hiding = Animator.StringToHash("Hiding");
        
        public override void ToggleHiding(bool b) {
            m_Animator.SetBool(Hiding, b);
        }
    }
}