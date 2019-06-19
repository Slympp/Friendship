using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Entities.Players {
	
	public class PlayerMovementController : MonoBehaviour {

		[Header("Movement")]
		[SerializeField]                 private float      m_JumpForce         = 400f; 
		[Range(0, 2)] [SerializeField]   private float      m_MovementSpeed     = 1f;
		[Range(0, 2)] [SerializeField]   private float      m_InAirSpeed        = 1f;
		[Range(0, 1)] [SerializeField]   private float      m_CrouchSpeed       = .36f;
		[Range(0, .3f)] [SerializeField] private float      m_MovementSmoothing = .05f;
		[SerializeField]                 private bool       m_AirControl        = false;
		
		[Header("Collisions")]
		[SerializeField]                 private LayerMask  m_WhatIsGround;
		[SerializeField]                 private Transform  m_GroundCheck;
		[Range(0, 1)]
		[SerializeField] float k_GroundedRadius = .2f;
		private bool m_Grounded;
		
		[SerializeField]                 private Transform  m_CeilingCheck;
		[SerializeField]                 private Collider2D m_TopCollider;
		[Range(0, 1)]
		[SerializeField] float k_CeilingRadius = .1f;

		private PlayerAnimatorController m_PlayerAnimatorController;
		
		private Rigidbody2D m_Rigidbody2D;
		private bool        m_FacingRight = true;
		private Vector3     m_Velocity    = Vector3.zero;
		private bool m_WasCrouching;

		private void Awake() {
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			m_PlayerAnimatorController = GetComponent<PlayerAnimatorController>();
		}

		private void FixedUpdate() {
			bool wasGrounded = m_Grounded;
			m_Grounded = false;

			Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
			foreach (Collider2D t in colliders) {
				if (t.gameObject == gameObject) continue;

				m_Grounded = true;
				if (!wasGrounded) {
					m_PlayerAnimatorController.SetJumping(false);
				}
			}
		}


		public void Move(float movementDelta, string inputSource) {

			bool crouch = InputController.Crouch(inputSource);
			
			if (!crouch) {
				if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
					crouch = true;
			}

			if (m_Grounded || m_AirControl) {

				if (crouch) {
					if (!m_WasCrouching) {
						m_WasCrouching = true;
						m_PlayerAnimatorController.SetCrouching(true);
					}

					movementDelta *= m_CrouchSpeed;
					m_TopCollider.enabled = false;
				
				} else {
					m_TopCollider.enabled = true;

					if (m_WasCrouching) {
						
						m_WasCrouching = false;
						m_PlayerAnimatorController.SetCrouching(false);
					}
				}

				if (m_AirControl && !m_Grounded)
					movementDelta *= m_InAirSpeed;

				float horizontalMovement = InputController.Lock(inputSource) ? 0 : movementDelta * 10f * m_MovementSpeed;
				Vector3 targetVelocity = new Vector2(horizontalMovement, m_Rigidbody2D.velocity.y);
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				if (movementDelta > 0 && !m_FacingRight || movementDelta < 0 && m_FacingRight)
					Flip();
			}
		
			if (m_Grounded && InputController.Jump(inputSource)) {
				m_Grounded = false;
				m_Rigidbody2D.angularVelocity = 0;
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
				m_PlayerAnimatorController.SetJumping(true);
			}
		}

		private void Flip() {
			m_FacingRight = !m_FacingRight;
			transform.localRotation = Quaternion.Euler(0, m_FacingRight ? 0 : 180, 0);
		}
		
		void OnDrawGizmosSelected() {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
		}
	}
}