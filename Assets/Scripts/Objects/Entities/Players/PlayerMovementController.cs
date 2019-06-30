using UnityEngine;

namespace Objects.Entities.Players {
	
	public class PlayerMovementController : MonoBehaviour {

		[Header("Movement")]
		[SerializeField]                 private float      m_JumpForce         = 400f; 
		[Range(0, 2)] [SerializeField]   private float      m_MovementSpeed     = 1f;
		[Range(0, 2)] [SerializeField]   private float      m_InAirSpeed        = 1f;
		[Range(0, .3f)] [SerializeField] private float      m_MovementSmoothing = .05f;
		
		[Header("Collisions")]
		[SerializeField]                 private LayerMask  m_WhatIsGround;
		[SerializeField]                 private Transform  m_GroundCheck;
		[Range(0, 1)]
		[SerializeField] float k_GroundedRadius = .2f;
		private bool m_Grounded = true;
		
		private Rigidbody2D m_Rigidbody2D;
		private bool        m_FacingRight = true;
		private Vector3     m_Velocity    = Vector3.zero;
		private bool m_WasCrouching;
		
		private PlayerAnimatorController m_PlayerAnimatorController;
		private PlayerFXController       m_PlayerFXController;

		private void Awake() {
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			m_PlayerAnimatorController = GetComponent<PlayerAnimatorController>();
			m_PlayerFXController = GetComponent<PlayerFXController>();
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
//					m_PlayerFXController.PlayOnJumpImpactSound();
				}
			}
		}

		public void Move(float movementDelta, string inputSource, float speedModifier = 1) {
			
			if (!m_Grounded)
				movementDelta *= m_InAirSpeed;

			float horizontalMovement = PlayerInputController.Lock(inputSource) ? 0 : movementDelta * 10f * m_MovementSpeed * speedModifier;
			Vector3 targetVelocity = new Vector2(horizontalMovement, m_Rigidbody2D.velocity.y);
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			m_PlayerAnimatorController.SetMoving(!horizontalMovement.Equals(0));

			if (movementDelta > 0 && !m_FacingRight || movementDelta < 0 && m_FacingRight)
				Flip();

			if (m_Grounded) {
				if (Mathf.Abs(m_Rigidbody2D.velocity.x) > 0.1f && !m_PlayerFXController.DustSpawnOnCooldown)
					m_PlayerFXController.SpawnRunningDust(m_GroundCheck.position);

				if (PlayerInputController.Jump(inputSource)) {
					m_Grounded = false;

					if (m_Rigidbody2D.velocity.y <= 0.1f)
						m_PlayerFXController.PlayOnJumpSound();
					
					m_Rigidbody2D.angularVelocity = 0;
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
					m_PlayerAnimatorController.SetJumping(true);
					m_PlayerFXController.SpawnJumpDust(m_GroundCheck.position);
				}
			}
		}

		private void Flip() {
			m_FacingRight = !m_FacingRight;
			transform.rotation = Quaternion.Euler(0, m_FacingRight ? 0 : 180, 0);
		}
		
		void OnDrawGizmosSelected() {
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
		}
	}
}