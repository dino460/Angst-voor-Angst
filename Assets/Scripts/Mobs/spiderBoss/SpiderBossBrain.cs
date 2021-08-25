using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossBrain : MonoBehaviour
{
	[Header("External Components")]
	[SerializeField] private float  	   distanceToPlayer;
	[SerializeField] private Transform     playerTransform;
	[SerializeField] private Transform 	   gunTransform;
	[SerializeField] private SpiderBossGun spiderBossGun;
	[SerializeField] private Vector2       gunToPlayerDirection;
	[SerializeField] private Animator 	   anim;
	[SerializeField] private Rigidbody2D   rb;

	private Enemy enemyScript;
	private Transform parentTransform;


	private enum AttackType {touch, normal, area, ranged, dash}
	[SerializeField] private AttackType attackType;

	private enum State {idle, walking, attacking, dead}
	[SerializeField] private State state = State.idle;


	[Header("Life")]
	[SerializeField] private float maxLife;


	[Header("Attack Ranges")]
	[SerializeField] private float   ranged_Range;
	[SerializeField] private float   normal_Range;
	[SerializeField] private float	 area_Range;
	[SerializeField] private float	 dash_Range;


	[Header("Attack Timers")]
	[SerializeField] private float attackTimer;
	[SerializeField] private float attackTimerThreshold;
	[SerializeField] private bool  isAttacking;
	[SerializeField] private bool  isDashing;


	[Header("Damage Compoents")]
	[SerializeField] private float normal_Damage;
	[SerializeField] private float ranged_Damage;
					 public  float  touch_Damage;
	[SerializeField] private float   area_Damage;
	[SerializeField] private float   dash_Damage;
	[HideInInspector] public float damageToApply;


	[Header("Movement")]
	[SerializeField] private float walkingSpeed;
	[SerializeField] private float    dashSpeed;
	[SerializeField] private float    dashTime;
	[SerializeField] private Vector2 direction;


	private void Awake()
	{
		anim.Play("Spider_Boss_Intro");
	}


	private void Start()
	{
		//rb = GetComponent<Rigidbody2D>();
		enemyScript = GetComponent<Enemy>();
		enemyScript.enemyLife = maxLife;
		parentTransform = transform.parent.transform;
	}


	private void Update()
	{
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Spider_Boss_Intro"))
		{
			direction = (playerTransform.position - transform.position).normalized;
			gunToPlayerDirection = (playerTransform.position - gunTransform.position).normalized;
			parentTransform.localScale = new Vector3 (-direction.x / Mathf.Abs(direction.x), 1f, 1f);
			distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
		
			StateMachine();
			
			anim.SetInteger("state", (int)state);
			anim.SetInteger("attackType", (int)attackType);
		}
	}


	private void FixedUpdate()
	{
		attackTimer += Time.fixedDeltaTime;
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 10)
		{
			if (collision.transform.position.x < transform.position.x)
			{
				collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(-1);
				collision.gameObject.GetComponent<PlayerController>().DamagePlayer(touch_Damage);
			}
			else
			{
				collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(1);
				collision.gameObject.GetComponent<PlayerController>().DamagePlayer(touch_Damage);
			}
		}
	}


	private void StateMachine()
	{
		if(enemyScript.isDead)
		{
			rb.velocity = new Vector2(0f, 0f);
			anim.Play("Spider_Boss_Death");
			this.enabled = false;
		}
		else if (attackTimer >= attackTimerThreshold && !isDashing && !isAttacking)
		{
			if (distanceToPlayer <= area_Range)
			{
				state 		  = State.attacking;
				attackType    = AttackType.area;
				damageToApply = area_Damage;
				anim.Play("Spider_Boss_Area_Attack");
			}
			else if (distanceToPlayer <= normal_Range)
			{
				state 		  = State.attacking;
				attackType    = AttackType.normal;
				damageToApply = normal_Damage;
				anim.Play("Spider_Boss_Normal_Attack");
			}
			else if (distanceToPlayer <= ranged_Range)
			{
				state 		  = State.attacking;
				attackType    = AttackType.ranged;
				damageToApply = area_Damage;
				anim.Play("Spider_Boss_Ranged_Attack");
			}
			else if (distanceToPlayer <= dash_Range)
			{
				state 		  = State.attacking;
				attackType    = AttackType.dash;
				damageToApply = dash_Damage;
				isDashing = true;
				anim.Play("Spider_Boss_Dash_Attack");
			}

			isAttacking = true;
		}
		else if (!isDashing && !isAttacking)
		{
			float relDist_Normal = distanceToPlayer - normal_Range;
			float relDist_Area   = distanceToPlayer -   area_Range;
			float relDist_Ranged = distanceToPlayer - ranged_Range;

			float dif_NormalArea   = normal_Range - area_Range;
			float dif_RangedNormal = ranged_Range - normal_Range;
			float dif_DashRanged   =   dash_Range - ranged_Range;

			float distanceCorrection = 0.5f; // A small distance correction on the checks to stop the flickering. It basically 
											 // creates a spot where it makes this "if" check go to the final "else" and stop
											 // the spider from moving right and left very quickly.

			if(distanceToPlayer <= area_Range){
            	rb.velocity = Vector2.zero;
            	isAttacking = true;
            	state = State.attacking;
            	anim.Play("Spider_Boss_Area_Attack");

            } else if((distanceToPlayer > area_Range - distanceCorrection) && (distanceToPlayer < (normal_Range * 0.8f))){
            	rb.velocity = Vector2.zero;
            	state = State.idle;

            } else if((relDist_Normal < (dif_RangedNormal / 2f) - distanceCorrection) && (distanceToPlayer > normal_Range)){
            	Move(-1);
            	state = State.walking;

            } else if((relDist_Normal > (dif_RangedNormal / 2f)) && (distanceToPlayer < ranged_Range) && (distanceToPlayer > normal_Range)){
            	Move(1);
            	state = State.walking;

            } else if(relDist_Ranged < (dif_DashRanged - distanceCorrection) && (distanceToPlayer > ranged_Range)){
            	rb.velocity = Vector2.zero;
            	isAttacking = true;
            	state = State.attacking;
            	anim.Play("Spider_Boss_Dash_Attack");
                
            } else{
            	rb.velocity = Vector2.zero;
            	state = State.idle;
            }

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Spider_Boss_Idle") && state == State.idle)
    		{
    			anim.Play("Spider_Boss_Idle");
    		}
		}
	}


	private void Move(int directionMod)
    {
    	rb.velocity = new Vector2(walkingSpeed * directionMod * direction.x, rb.velocity.y);

    	if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Spider_Boss_Walk"))
    	{
    		anim.Play("Spider_Boss_Walk");
    	}

    	if (parentTransform.localScale.x > 0f)
    	{
    		anim.SetFloat("speed", (-rb.velocity.x / Mathf.Abs(rb.velocity.x)));
    	}
    	else
    	{
    		anim.SetFloat("speed", (rb.velocity.x / Mathf.Abs(rb.velocity.x)));
    	}
    	
    }


    private void StopToAttack()
    {
    	rb.velocity = Vector2.zero;
    }


	private void ReturnToIdle()
	{
		state = State.idle;
		attackTimer = 0f;
		isAttacking = false;
		damageToApply = touch_Damage;
	}


	private void FireBossGun()
	{
		spiderBossGun.SpiderRangedFire(gunToPlayerDirection, ranged_Damage);
	}


	private void CallDash()
	{
		StartCoroutine(Dash());
	}


	private IEnumerator Dash()
	{
		rb.velocity = new Vector2(dashSpeed * (-parentTransform.localScale.x), 0f);
		yield return new WaitForSeconds(dashTime);
		rb.velocity = Vector2.zero;
		isDashing = false;
		ReturnToIdle();
	}


	private void OnDrawGizmos()
	{
		Gizmos.color = Color.gray;
		Gizmos.DrawWireSphere(transform.position, dash_Range);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, ranged_Range);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, area_Range);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, normal_Range);
	}

	/*
	move if player too far or too close
	move between attacks

	change state to attack when ready
		- attack type is decided by range
		- attack type is differentiable by animation

	normal	<	area	<	ranged	<	 dash
	normal   jump, close gap	ranged	 close bigger gap
	*/
}

// ASK ME!!!
/*
 else if(relDist_Normal < ((dif_AreaNormal / 2f) - distanceCorrection) && (distanceToPlayer > normal_Range)){
            	Move(-1);
            	state = State.walking;

            } else if((relDist_Normal > (dif_AreaNormal / 2f)) && (distanceToPlayer < area_Range) && (distanceToPlayer > normal_Range)){
            	Move(1);
            	state = State.walking;

            } else if((relDist_Area < ((dif_RangedArea / 2f) - distanceCorrection)) && (distanceToPlayer > area_Range)){
            	Move(-1);
            	state = State.walking;

            }*/