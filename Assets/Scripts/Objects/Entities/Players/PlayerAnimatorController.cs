﻿using UnityEngine;

namespace Entities.Players {
    public class PlayerAnimatorController : MonoBehaviour {

        private Animator m_Animator;
 
        private static readonly int Shoot = Animator.StringToHash("Shoot");
        
        private static readonly int Crouching = Animator.StringToHash("Crouching");
        private static readonly int Jumping = Animator.StringToHash("Jumping");
        private static readonly int Moving = Animator.StringToHash("Moving");

        public void Init(Animator animator) {
            m_Animator = animator;
        }

        public void TriggerShooting() {
            m_Animator.Play(Shoot, -1, 0);
        }

        public void SetMoving(bool v) {
            m_Animator.SetBool(Moving, v);
        }
        
        public void SetCrouching(bool v) {
            m_Animator.SetBool(Crouching, v);
        }
        
        public void SetJumping(bool v) {
            m_Animator.SetBool(Jumping, v);
        }
    }
}
