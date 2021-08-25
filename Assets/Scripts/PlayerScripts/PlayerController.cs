using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class PlayerController : MonoBehaviour
{
	private RigidbodyConstraints2D originalConstraints;
	private InputManager controller;
	private Rigidbody2D  rb;
	private Animator	 anim;
	private bool		 jumpIsReleased;
	

	private AudioManager audioManager;
	private float 		 airSpeed;
	private bool 		 onBonfire;
	public  bool 		 insideBossArea;
	public  bool 		 isDamaged;
	private GameObject   currentBonfire;


	// UI Components
	private GameObject 		lowHealthImage;
	private GameObject 		attackButtonImage;
	private GameObject 		dashButtonImage;
	private TextMeshProUGUI actionButtonText;
	

	[SerializeField] private float checkDistance;


	private enum State
	{idle, running, dashing, onAir, attacking, damaged, dead}
	[SerializeField] private State state = State.idle;


	public bool isMenuOpen;
	private InGameMenu inGameMenu;


	[Header("Horizontal Movement Components")]
	[SerializeField] private float horizontalDir;
	[SerializeField] private float currentSpeed;
	[SerializeField] private float moveSpeed;


	[Header("Dash Components")]
	[SerializeField] private float dashDamage;
	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashTime;
	[SerializeField] private float dashAirWaitTime;
	[SerializeField] private bool  canDash;
	[SerializeField] private bool  isDashing;

	
	[Header("GroundCheck Components")]
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private Transform groundCheckPosition;
	[SerializeField] private float	   groundCheckRadius;
					 private bool	   isGrounded;


	[Header("Jump Components")]
	[SerializeField] private float maxGraceTime;
	[SerializeField] private float graceJumpTimer;
	[SerializeField] private float minVerticalSpeed;
	[SerializeField] private float timeToMaxHeight;
	[SerializeField] private float maxHeight;
					 private float verticalSpeed;


	[Header("Attack Components")]
	[SerializeField] private Transform attackPosition1;
	[SerializeField] private Transform attackPosition2;
	[SerializeField] private LayerMask enemyLayer;
	[SerializeField] private bool	   isAttacking;
	[SerializeField] private float	   attackDamage;
	[SerializeField] private float	   attacksPerSecond;
					 private float	   attackTimer;
					 private bool	   canAttack;

	
	[Header("Life Components")]
	[SerializeField] private float	lifeRegen;
	[SerializeField] private float  maxLife;
	[SerializeField] private float  maxMana;
	[SerializeField] private int	tickLimit;
					 private Slider manaSlider;
					 private Slider lifeSlider;
					 private int	tickCount;



	[Header("Knockback Components")]
	[SerializeField] private float horizontalKnockback;
	[SerializeField] private float verticalKnockback;
	[SerializeField] private float knockbackTicks;


	private void Awake()
	{
		isDashing = false;
		isAttacking = false;
		canDash = true;
		canAttack = true;
		currentSpeed = moveSpeed;
		
		// Calculates jump speed based upon accelerated vertical movement formulae
		verticalSpeed = (2 * maxHeight) / timeToMaxHeight;
		
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		originalConstraints = rb.constraints;

		controller = new InputManager();

		controller.Player.HorizontalMovement.performed += _0 =>
			horizontalDir = _0.ReadValue<float>();
		controller.Player.HorizontalMovement.canceled += _0 =>
			horizontalDir = 0f;

		controller.Player.Jump.performed += _0 => Jump(0);
		controller.Player.Jump.canceled  += _0 => Jump(1);

		controller.Player.Dash.performed += _0 => CallDash();

		controller.Player.Attack.performed += _0 => CallAction();

		controller.Player.OpenMenu.performed += _0 => OpenMenuHandler();
	}


	private void Start()
	{
		attackButtonImage = GameObject.Find("Canvas/UIHolder/Controls/AttackButton");
		actionButtonText = GameObject.Find("Canvas/UIHolder/Controls/AttackButton/Text (TMP)").GetComponent<TextMeshProUGUI>();
		dashButtonImage = GameObject.Find("Canvas/UIHolder/Controls/DashButton");
		lowHealthImage = GameObject.Find("Canvas/UIHolder/LowHealthWarning(TMP)");
		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		inGameMenu = GameObject.Find("Canvas/MenuHolder").GetComponent<InGameMenu>();
		lifeSlider = GameObject.Find("Canvas/UIHolder/LifeSlider").GetComponent<Slider>();
		manaSlider = GameObject.Find("Canvas/UIHolder/ManaSlider").GetComponent<Slider>();

		Global.maxLife = Global.baseLife * Global.lifeLevel;
		Global.maxMana = Global.baseMana * Global.manaLevel;
		Global.life = Global.maxLife;
		Global.mana = Global.maxMana;

		lifeSlider.maxValue = Global.baseLife * Global.lifeLevel;
		manaSlider.maxValue = Global.baseMana * Global.manaLevel;
		lifeSlider.value = Global.maxLife;
		manaSlider.value = Global.maxMana;

		isMenuOpen = false;

		// Transfer to GameManager later
		DataGlobal data = SaveSystem.LoadData();

		if(data != null){
			Global.dashCollected = data.hasDash;
			if(data.lastBonfire == 0){
				transform.position = GameObject.Find("Bonfire_tmp").transform.position;
			} else {
				transform.position = GameObject.Find("Bonfire_tmp (" + data.lastBonfire + ")").transform.position;
				Debug.Log("Bonfire_tmp (" + ((char) data.lastBonfire) + ")");
			}
			Global.manaLevel = data.manaLevel;
			Global.lifeLevel = data.lifeLevel;
		}
		// End of transfer code
	}


	private void OnEnable()
	{
		controller.Enable();
	}


	private void Update()
	{
		CheckSlope();

		attackTimer += Time.deltaTime;

		if (Global.mana <= 0f) // Add warning text
		{
			canDash	  = false;
			//canAttack = false;
			lowHealthImage.SetActive(true);
			//attackButtonImage.SetActive(false);
			dashButtonImage.SetActive(false);
		}
		else
		{
			canDash	  = true;
			//canAttack = true;
			lowHealthImage.SetActive(false);
			//attackButtonImage.SetActive(true);
			
			if (Global.dashCollected) dashButtonImage.SetActive(true);
			else dashButtonImage.SetActive(false);
		}

		manaSlider.value = Global.mana;
		lifeSlider.value = Global.life;

		CheckGrounded();

		if (state != State.attacking)
		{
			if (horizontalDir > 0) transform.localScale = new Vector2(1f, 1f);
			else if (horizontalDir < 0) transform.localScale = new Vector2(-1f, 1f);

			if (!isDamaged && !isDashing) HorizontalMove(horizontalDir);
		}
		
		if (state != State.damaged && isGrounded && !isDashing && horizontalDir == 0f)
		{
			rb.velocity = new Vector2(0f, rb.velocity.y);
			rb.constraints = RigidbodyConstraints2D.FreezePositionX |
							 RigidbodyConstraints2D.FreezeRotation;
		}
		else rb.constraints = originalConstraints;

		if (!isGrounded || horizontalDir == 0f) audioManager.Stop("PlayerRun");

		if (state != State.dead) FiniteStateMachine();
		
		anim.SetInteger("state", (int)state);

		airSpeed = rb.velocity.y;
		if (airSpeed == 0) anim.SetFloat("airSpeed", 0f);
		else anim.SetFloat("airSpeed", (airSpeed/Mathf.Abs(airSpeed)));
	}


	private void FixedUpdate()
	{

		if (state == State.damaged)
		{
			if (rb.velocity.x > 0f)
			{
				rb.velocity =
				new Vector2(rb.velocity.x - (horizontalKnockback / knockbackTicks),
							rb.velocity.y);
			}
			else
			{
				rb.velocity =
				new Vector2(rb.velocity.x + (horizontalKnockback / knockbackTicks),
							rb.velocity.y);
			}
		}
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "DashPowerUp")
		{
			Global.dashCollected = true;
			Destroy(collider.gameObject);
			GameObject dashWarning = GameObject.Find("Canvas/UIHolder/DashWarning(TMP)");
			dashWarning.SetActive(true);
			dashWarning.GetComponent<Animator>().Play("DashWarning");
		}
		else if (collider.tag == "Bonfire")
		{
			actionButtonText.SetText("Use");
			currentBonfire = collider.gameObject;
			canAttack = false;
			onBonfire = true;
		}
		else if (collider.tag == "BossArea")
		{
			insideBossArea = true;
		}
	}


	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.tag == "Bonfire")
		{
			actionButtonText.SetText("Strike");
			canAttack = true;
			onBonfire = false;
		}
		else if (collider.tag == "BossArea")
		{
			insideBossArea = false;
		}
	}


	private void HorizontalMove(float xDir)
	{
		if(!isMenuOpen){
			if (!audioManager.isPlaying("PlayerRun") && isGrounded) audioManager.Play("PlayerRun");
			rb.velocity = new Vector2(xDir * currentSpeed, rb.velocity.y);
		}
	}


	private void Jump(int jumpState)
	{
		if(!isMenuOpen){
			if ((isGrounded && rb.velocity.y >= 0 || graceJumpTimer <= maxGraceTime)
				&& jumpState == 0 && state != State.damaged)
			{
				audioManager.Play("PlayerJump");
				rb.velocity = new Vector2(rb.velocity.x, verticalSpeed);
			}
			else if (!isGrounded && !jumpIsReleased && rb.velocity.y >= 0 && jumpState == 1)
			{
				rb.velocity = new Vector2(rb.velocity.x, minVerticalSpeed);
				jumpIsReleased = true;
			}
		}
	}


	private void CheckGrounded()
	{
		isGrounded = Physics2D.OverlapCircle(
						new Vector2(groundCheckPosition.position.x,
									groundCheckPosition.position.y),
									groundCheckRadius, groundLayer);

		if (isGrounded)
		{
			jumpIsReleased = false;
			graceJumpTimer = 0f;
		}
		else
		{
			graceJumpTimer += Time.deltaTime;
		}
	}


	private void CheckSlope()
	{
		if (isDashing)
		{

			if (Physics2D.Raycast(groundCheckPosition.position,
								  new Vector2(transform.localScale.x, 0f),
								  checkDistance, groundLayer) ||
				Physics2D.Raycast(new Vector2(groundCheckPosition.position.x,
											  attackPosition1.position.y - 0.7f), 
								  new Vector2(transform.localScale.x, 0f),
								  checkDistance, groundLayer))
			{
				rb.velocity = new Vector2(0f, 0f);
			}
		}
	}


	private void CallDash()
	{
		if(Global.dashCollected && canDash && !isDashing && !isMenuOpen) StartCoroutine(Dash());
	}


	private IEnumerator Dash()
	{
		// Pre-dash set-up
		isDashing = true;
		state = State.dashing;
		this.gameObject.layer = 11;
		DamagePlayer(dashDamage);
		rb.velocity = Vector2.zero;
		float tmpGravityHolder = rb.gravityScale;
		rb.gravityScale = 0f;

		// Dash
		rb.velocity = new Vector2(dashSpeed * transform.localScale.x, rb.velocity.y);
		yield return new WaitForSeconds(dashTime);

		// Post-dash set-up
		rb.velocity = Vector2.zero;
		yield return new WaitForSeconds(dashAirWaitTime);

		// Clean-up
		this.gameObject.layer = 10;
		rb.gravityScale = tmpGravityHolder;
		isDashing = false;
	}


	public void DamagePlayer(float damage)
	{
		if (Global.mana > 0)
		{
			Global.mana -= damage;
			if (Global.mana <= 0)
			{
				Global.mana = 0;
			}
		}
		else if (Global.mana == 0)
		{
			Global.life -= damage;
			if (Global.life <= 0)
			{
				Global.playerDead = true;
				//Global.dashCollected = false;
				rb.velocity = new Vector2(0f, 0f);
				state = State.dead;
			}
		}
	}


	private void KillPlayer()
	{
		rb.velocity = new Vector2(0f, 0f);
		this.enabled = false;
	}


	private void CallAction()
	{
		if (attackTimer >= (1f / attacksPerSecond) && canAttack && !isMenuOpen)
		{
			attackTimer = 0f;
			isAttacking = true;
		}
		else if (onBonfire)
		{
			currentBonfire.GetComponent<BonfireDetector>().BofireAction();
		}
	}
	
	private void Attack()
	{
		audioManager.Play("PlayerAttack");
			
		Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(attackPosition1.position,
														   attackPosition2.position,
														   enemyLayer);
		foreach (Collider2D enemy in hitEnemies)
		{
			if (enemy.gameObject.tag == "Enemy")
			{
				enemy.gameObject.GetComponent<Enemy>().KnockbackEnemy(
					(enemy.transform.position.x - this.transform.position.x) /
					Mathf.Abs(enemy.transform.position.x - this.transform.position.x));
				
				enemy.gameObject.GetComponent<Enemy>().DealDamage(attackDamage);

				LifeSteal();
			}
			else if (enemy.gameObject.tag == "Boss")
			{
				enemy.gameObject.GetComponent<Enemy>().DealDamage(attackDamage);

				LifeSteal();
			}
		}
	}


	private void LifeSteal()
	{
		if (Global.life >= Global.maxLife)
		{
			Global.mana += lifeRegen;
			if (Global.mana >= Global.maxMana) Global.mana = Global.maxMana;
		}
		else
		{
			Global.life += lifeRegen;
			if (Global.life >= Global.maxLife) Global.life = Global.maxLife;
		}
	}


	private void SetAttackToFalse()
	{
		isAttacking = false;
	}


	public void KnockbackPlayer(int directionMod)
	{
		SetAttackToFalse();
		state = State.damaged;
		isDamaged = true;
		rb.velocity = new Vector2(directionMod * horizontalKnockback, verticalKnockback);
	}


	private void EndKnockback()
	{
		isDamaged = false;
	}


	private void ActivateInvencibility()
	{
		gameObject.layer = 11;
	}


	private void DeactivateInvencibility()
	{
		gameObject.layer = 10;
	}


	private void OpenMenuHandler()
	{
		if (!isMenuOpen)
		{
			inGameMenu.OpenMenu();
			isMenuOpen = true;
		}
		else
		{
			inGameMenu.CloseMenu();
			isMenuOpen = false;
		}
	}


	private void FiniteStateMachine()
	{
		if (state == State.damaged)
		{
			if (Mathf.Abs(rb.velocity.x) <= 0.5f)
			{
				state = State.idle;
			}
		}
		else if (isAttacking)
		{
			state = State.attacking;
		}
		else if (state == State.dashing)
		{
			if (!isDashing) state = State.idle;
		}
		else if (!isGrounded && !isAttacking && !isDashing)
		{
			state = State.onAir;
		}
		else if (Mathf.Abs(rb.velocity.x) > 0.01 && !isAttacking && !isDashing && isGrounded)
		{
			state = State.running;
		}
		else if (!isAttacking && !isDashing)
		{
			state = State.idle;
		}
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(
			new Vector2(groundCheckPosition.position.x,
						groundCheckPosition.position.y),
						groundCheckRadius);

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(new Vector2(attackPosition1.position.x, attackPosition2.position.y),
                        new Vector2(attackPosition2.position.x, attackPosition2.position.y));
        Gizmos.DrawLine(new Vector2(attackPosition1.position.x, attackPosition1.position.y),
                        new Vector2(attackPosition2.position.x, attackPosition1.position.y));
        Gizmos.DrawLine(new Vector2(attackPosition2.position.x, attackPosition2.position.y),
                        new Vector2(attackPosition2.position.x, attackPosition1.position.y));
        Gizmos.DrawLine(new Vector2(attackPosition1.position.x, attackPosition2.position.y),
                        new Vector2(attackPosition1.position.x, attackPosition1.position.y));
	}
}
