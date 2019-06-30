using UnityEngine;

namespace Objects.Entities {
    public abstract class BaseAnimatorController : MonoBehaviour {
        
        protected Animator m_Animator;
        
        protected static readonly int Shoot = Animator.StringToHash("Shoot");
        protected static readonly int Moving = Animator.StringToHash("Moving");
        
        public void Init(Animator animator) {
            m_Animator = animator;
        }
        
        public virtual void TriggerShooting() {
            m_Animator.Play(Shoot, -1, 0);
        }
        
        public void SetMoving(bool v) {
            m_Animator.SetBool(Moving, v);
        }
    }
}