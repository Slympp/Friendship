using UnityEngine;

namespace Entities.Players {
    public class PlayerAnimatorController : MonoBehaviour {

        private Animator m_Animator;
 
        private static readonly int Shooting = Animator.StringToHash("Shooting");
        private static readonly int Crouching = Animator.StringToHash("Crouching");
        private static readonly int Jumping = Animator.StringToHash("Jumping");

        void Awake() {
            m_Animator = GetComponent<Animator>();
        }

        public void SetShooting(bool v) {
            m_Animator.SetBool(Shooting, v);
        }
        
        public void SetCrouching(bool v) {
            m_Animator.SetBool(Crouching, v);
        }
        
        public void SetJumping(bool v) {
            m_Animator.SetBool(Jumping, v);
        }
    }
}
