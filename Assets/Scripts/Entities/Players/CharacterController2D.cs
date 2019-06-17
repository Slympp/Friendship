using UnityEngine;

namespace Entities.Players {
	
	public class CharacterController2D : MonoBehaviour {

		[SerializeField]                 private float      m_JumpForce         = 400f;  // Amount of force added when the player jumps.
		[Range(0, 2)] [SerializeField]   private float      m_MovementSpeed     = 1f;    // Amount of maxSpeed applied to crouching movement. 1 = 100%
		[Range(0, 2)] [SerializeField]   private float      m_InAirSpeed        = 1f;    // Amount of maxSpeed applied to crouching movement. 1 = 100%
		[Range(0, 1)] [SerializeField]   private float      m_CrouchSpeed       = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
		[Range(0, .3f)] [SerializeField] private float      m_MovementSmoothing = .05f;  // How much to smooth out the movement
		[SerializeField]                 private bool       m_AirControl        = false; // Whether or not a player can steer while jumping;
		[SerializeField]                 private LayerMask  m_WhatIsGround;              // A mask determining what is ground to the character
		[SerializeField]                 private Transform  m_GroundCheck;               // A position marking where to check if the player is grounded.
		[SerializeField]                 private Transform  m_CeilingCheck;              // A position marking where to check for ceilings
		[SerializeField]                 private Collider2D m_CrouchDisableCollider;     // A collider that will be disabled when crouching

		const   float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
		private bool  m_Grounded;             // Whether or not the player is grounded.
		const   float k_CeilingRadius = .1f;  // Radius of the overlap circle to determine if the player can stand up

		private PlayerAnimatorController m_PlayerAnimatorController;
		
		private Rigidbody2D m_Rigidbody2D;
		private bool        m_FacingRight = true; // For determining which way the player is currently facing.
		private Vector3     m_Velocity    = Vector3.zero;
		private bool m_WasCrouching = false;

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


		public void Move(float movementDelta) {

			bool crouch = InputController.Crouch;
			
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

					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = false;
				
				} else {
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = true;

					if (m_WasCrouching) {
						m_WasCrouching = false;
						m_PlayerAnimatorController.SetCrouching(false);
					}
				}

				if (m_AirControl && !m_Grounded)
					movementDelta *= m_InAirSpeed;

				float horizontalMovement = InputController.Lock ? 0 : movementDelta * 10f * m_MovementSpeed;
				Vector3 targetVelocity = new Vector2(horizontalMovement, m_Rigidbody2D.velocity.y);
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				if (movementDelta > 0 && !m_FacingRight || movementDelta < 0 && m_FacingRight)
					Flip();
			}
		
			if (m_Grounded && InputController.Jump) {
				m_Grounded = false;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				m_PlayerAnimatorController.SetJumping(true);
			}
		}


		private void Flip() {
			m_FacingRight = !m_FacingRight;
			transform.localRotation = Quaternion.Euler(0, m_FacingRight ? 0 : 180, 0);
		}
	}
}