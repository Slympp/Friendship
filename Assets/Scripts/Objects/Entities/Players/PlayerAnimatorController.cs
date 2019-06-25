using UnityEngine;

namespace Objects.Entities.Players {
    public class PlayerAnimatorController : BaseAnimatorController {

        private static readonly int Jumping = Animator.StringToHash("Jumping");
        private static readonly int Crouching = Animator.StringToHash("Crouching");
        
        public void SetCrouching(bool v) {
            m_Animator.SetBool(Crouching, v);
        }
        
        public void SetJumping(bool v) {
            m_Animator.SetBool(Jumping, v);
        }
    }
}
