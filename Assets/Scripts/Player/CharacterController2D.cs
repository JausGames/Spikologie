using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 500f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .10f;  // How much to smooth out the movement
	[Range(0, .3f)] [SerializeField] private float m_AirMovementSmoothing = .30f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_GroundKoCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Transform m_LeftCheck;								// Left
	[SerializeField] private Transform m_RightCheck;							// Right
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private bool m_FacingRight = true;                         // For determining which way the player is currently facing.
	[SerializeField] private int jumpNumber = 3;                                // Number of jump
	[SerializeField] private float BONUS_GRAV = 5f;
	[SerializeField] private float BONUS_GRAV_FALL = 10f;
	[SerializeField] private float MINUS_AERIAL_SPEED = 10f;
	[SerializeField] private float forceHorizontal = 0f;
	[SerializeField] private float forceVertical = 0f;
	[SerializeField] private float DODGE_SPEED = 100f;

	const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
	[SerializeField] private bool m_Grounded = false;            // Whether or not the player is grounded.
	[SerializeField] private bool m_KoGrounded = false;            // Whether or not the player is grounded.
	[SerializeField] private bool m_Dodging = false;            // Whether or not the player is grounded.
	[SerializeField] private bool m_OnWall = false;            // Whether or not the player is on wall.
	[SerializeField] private bool m_Spiked = false;            // Whether or not the player is on wall.
	private Vector2 wallDirection = Vector2.zero;            // Whether or not the player is on wall.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;
	[SerializeField] private int actualJump = 0;
	public AnimatorController animatorControl;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnWallEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
		Gizmos.DrawWireSphere(m_CeilingCheck.position, k_CeilingRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(m_LeftCheck.position, 0.1f);
		Gizmos.DrawWireSphere(m_RightCheck.position, 0.1f);
	}
	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		if (OnWallEvent == null)
			OnWallEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		if (!m_Grounded)
		{
				Vector3 vel = m_Rigidbody2D.velocity;
				if (vel.y > 0) vel.y -= BONUS_GRAV * Time.deltaTime;
				if (vel.y <= 0) vel.y -= BONUS_GRAV_FALL * Time.deltaTime;
			m_Rigidbody2D.velocity = vel;
		}
		if (m_Dodging) m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);

		bool wasGrounded = m_Grounded;
		bool wasOnWall = m_OnWall;
		m_Grounded = false;
		wallDirection = Vector2.zero;
		m_OnWall = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] collidersFloor = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		Collider2D[] collidersKoFloor = Physics2D.OverlapCircleAll(m_GroundKoCheck.position, k_GroundedRadius, m_WhatIsGround);
		Collider2D[] collidersWall = Physics2D.OverlapAreaAll(m_LeftCheck.position, m_RightCheck.position , m_WhatIsGround);
		for (int i = 0; i < collidersFloor.Length; i++)
		{
			if (collidersFloor[i].gameObject != gameObject)
			{
				actualJump = jumpNumber;
				m_Grounded = true;
				//m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
		for (int i = 0; i < collidersKoFloor.Length; i++)
		{
			if (collidersKoFloor[i].gameObject != gameObject)
			{
				actualJump = jumpNumber;
				m_KoGrounded = true; 
			}
		}
		for (int i = 0; i < collidersWall.Length; i++)
		{
			if (collidersWall[i].gameObject != gameObject)
			{
				var flt = (collidersWall[i].transform.position - transform.position).x;
				wallDirection = new Vector2(Mathf.Sign(flt), 0);
				m_OnWall = true;
				if (!wasOnWall)
					OnWallEvent.Invoke();
			}
		}

		animatorControl.SetOnWall(m_OnWall);
		animatorControl.SetInAir(!m_Grounded);

	}

	public Vector2 GetWallDirection()
	{
		return wallDirection;
	}

	public void Move(float horizontal, float vertical, bool crouch, bool jump, bool ko, bool spiked)
	{
		forceHorizontal = horizontal;
		forceVertical = vertical;
		m_Spiked = spiked;
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}


		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				horizontal *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}
			var isWalking = false;
			if (!m_Grounded) horizontal = horizontal / 1.1f;
			if ((!ko && !spiked && !m_Dodging) && m_Grounded)
			{
				Debug.Log("CharacterControlelr2D, Move : !ko && !spiked && grounded ");
				if (Mathf.Abs(horizontal) > 0.2f) isWalking = true; 
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(horizontal * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
			}
			if ((!ko && !spiked && !m_Dodging) && !m_Grounded)
			{
				Debug.Log("CharacterControlelr2D, Move : !ko && !spiked && !grounded ");
				Vector3 targetVelocity = new Vector2(m_Rigidbody2D.velocity.x - (Mathf.Sign(m_Rigidbody2D.velocity.x) * MINUS_AERIAL_SPEED) + horizontal * 4f, m_Rigidbody2D.velocity.y);
				targetVelocity = new Vector2(horizontal * 4f, m_Rigidbody2D.velocity.y);
				if (Mathf.Abs(targetVelocity.x) < 5) targetVelocity.x = horizontal * 10f;
				if (vertical < 0) targetVelocity.y += vertical * 9f;
				if (m_Rigidbody2D.velocity.y < 0f && vertical > 0) targetVelocity.y += vertical * 2f;
				// Move the character by finding the target velocity
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_AirMovementSmoothing );
			}
			animatorControl.SetWalking(isWalking);


			// If the input is moving the player right and the player is facing left...
			if (horizontal > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (horizontal < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
	}

	public Rigidbody2D GetRigidBody()
	{
		return m_Rigidbody2D;
	}
	public bool IsGrounded()
	{
		return m_Grounded;
	}
	public void MoveGroundCheck(bool value)
	{
		/*if (value)
		{
			m_GroundKoCheck.localPosition = m_GroundCheck.localPosition;
			m_GroundCheck.localPosition = Vector3.zero;
		}
		if (!value)
		{
			m_GroundCheck.localPosition = new Vector3(0, -0.45f, 0);
			m_GroundKoCheck.localPosition = m_GroundCheck.localPosition - new Vector3(0, -0.45f, 0);
		}*/
	}


	public void Dodge(float dir)
	{
		m_Dodging = true;
		Debug.Log("CharacterController2D, Dodge : Sign : x " + Mathf.Sign(dir));
		if (Mathf.Sign(dir) < 0 && m_FacingRight == true) Flip();
		if (Mathf.Sign(dir) > 0 && m_FacingRight == false) Flip();
		m_Rigidbody2D.velocity = Vector2.zero;
		m_Rigidbody2D.AddForce(Mathf.Sign(dir) * 1000f * DODGE_SPEED * Vector2.right);
	}
	public void EndDodge()
	{
		m_Dodging = false;
	}
	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	public void Jump()
	{
		if (m_Dodging || m_Spiked) return;
		// If the player should jump in from a wall...
		if (m_OnWall && !m_Grounded)
		{
			Debug.Log("Jumping in 2DController :  on wall");
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x, 0f);
			m_Rigidbody2D.AddForce(wallDirection * m_JumpForce * 0.7f + 0.9f * m_JumpForce * Vector2.up);
		}
		// If the player should jump...
		if (m_Grounded)
		{
			//animatorControl.SetInAir(true);
			Debug.Log("Jumping in 2DController :  grounded");
			// Add a vertical force to the player.
			m_Grounded = false;
			actualJump--;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		// If the player should jump in air...
		if (actualJump == 2 && !m_OnWall && !m_Grounded)
		{
			Debug.Log("Jumping in 2DController :  double jump");
			// Add a vertical force to the player.
			actualJump--;
			m_Rigidbody2D.velocity = new Vector3(m_Rigidbody2D.velocity.x, 0f);
			m_Rigidbody2D.AddForce(new Vector2(0f, 0.9f * m_JumpForce));
		}
	}
}
