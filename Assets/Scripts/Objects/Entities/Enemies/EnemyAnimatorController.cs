using UnityEngine;

namespace Objects.Entities.Enemies {
    public class EnemyAnimatorController : BaseAnimatorController {

        [SerializeField] private bool LoopedAttackAnimation;

        protected readonly int Shooting = Animator.StringToHash("Shooting");
        
        public override void TriggerShooting() {

            if (LoopedAttackAnimation)
                m_Animator.SetBool(Shooting, true);
            else
                base.TriggerShooting();
        }

        public virtual void ToggleHiding(bool b) { }
    }
}