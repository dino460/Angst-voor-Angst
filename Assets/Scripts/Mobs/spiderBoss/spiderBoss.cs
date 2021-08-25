using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderBoss : mobCreator
{

	[Header("Components")]
    private CircleCollider2D            col;
    private GameObject                  player;
    private Animator                    animator;
    [SerializeField] private GameObject BossGun;
    private SpriteRenderer sp;

    [SerializeField] private LayerMask  playerLayer;

    private enum State {idle, walk, attackNormal, attackRanged, attackArea, attackDash, damaged, dead};
	[SerializeField] private State state;
	

	public AnimationClip animNormalAttack;
	public AnimationClip animAreaAttack;
	public AnimationClip animDashAttack;
	public AnimationClip animRangedAttack;


	[Header("Combat Variables")]
	private bool 				  attacking;
	
	[SerializeField] private int  health;

    [SerializeField] private bool raged;

	[SerializeField] private float enemyKnockbackTicks;

    [SerializeField] private float attackNormalDelay;
    [SerializeField] private float attackRangedDelay;
    [SerializeField] private float attackAreaDelay;	
	[SerializeField] private float attackDashDelay;

    [SerializeField] private float attackDamage;

    [SerializeField] private float attackTimer;


	[SerializeField] private float attackNormalDist;
	[SerializeField] private float attackRangedDist;
	[SerializeField] private float attackAreaDist;
	[SerializeField] private float attackDashDist;

	[SerializeField] private float attackNormalRange;
	[SerializeField] private float attackAreaRange;


	[Header("Movement Variables")]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float dashSpeed;

	[SerializeField] private bool  moving;


    private void Start()
    {
        attacking = false;
        state = State.idle;

        attackTimer = 1f;

        animator = GetComponent<Animator>();
        enemyScript.enemyLife = health;
        sp = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(Global.playerDead){
            state = State.idle;
            return;
        }

        distToPlayer =  Vector3.Distance(playerPosition.position, transform.position);
        //distWalked += Mathf.Abs(rb.velocity.x * Time.deltaTime); // para que ele mude a direção após andar uma certa distância (temporário)
        direction = (playerPosition.position - transform.position).normalized;

        transform.localScale = (new Vector2(-Mathf.Sign(direction.x) * 3f, 3f));

/*
        if (rb.velocity.x > 0) transform.localScale = new Vector2(-3f, 3f);
        else if (rb.velocity.x < 0) transform.localScale = new Vector2(3f, 3f);
*/

    }

    private void FixedUpdate()
    {
    	if(Global.playerDead){ // if the player is dead, the spider does not execute code from further than this if
            rb.velocity = Vector2.zero;
            return;
        }
        
        attackTimer -= Time.fixedDeltaTime;

        if(attackTimer <= 0.01f){
            attacking = true;
            moving = false;
        } else {
            moving = true;
            attacking = false;
        }

        StartCoroutine(StateSwitch());
        animator.SetInteger("state", (int) state);
    }

    private IEnumerator StateSwitch()
    {
        if(enemyScript.isDead)
        {
            state = State.dead;
            rb.velocity = new Vector2(0f, 0f);
            rb.isKinematic = true;
            col.enabled = false;
            this.enabled = false;
        } 
        else if(attacking)
        {
        	rb.velocity = Vector2.zero;
            state = State.idle;

            if(distToPlayer < attackNormalDist){
            	state = State.attackNormal;
                animator.SetInteger("state", (int) state);
                yield return BossNormalAttack();
                attackTimer = attackNormalDelay;

            } else if(distToPlayer < attackAreaDist){
            	state = State.attackArea;
                animator.SetInteger("state", (int) state);
                sp.color = new Color(242, 255, 0);
            	yield return BossAreaAttack();
                attackTimer = attackAreaDelay;

            } else if(distToPlayer > attackAreaDist && distToPlayer < attackRangedDist){
            	state = State.attackRanged;
                sp.color = new Color(0, 255, 0);
                animator.SetInteger("state", (int) state);
                yield return BossRangedAttack();
                attackTimer = attackRangedDelay;

            } else if(distToPlayer < attackDashDist){
            	state = State.attackDash;
                sp.color = new Color(255, 0, 200);
                animator.SetInteger("state", (int) state);          
            	yield return BossDashAttack();
                attackTimer = attackDashDelay;
                Debug.Log("1");
            }    

            state = State.idle;
            sp.color = Color.white;

        } 
        else if(moving)
        {
        	state = State.walk;

            if(distToPlayer < attackNormalDist){
            	rb.velocity = Vector2.zero;
            	state = State.idle;
               // Debug.Log("normal");

            } else if(distToPlayer < attackAreaDist || distToPlayer - attackAreaDist < 0.1f){
                sp.color = new Color(242, 255, 0);
            	MoveAreaToNormal();

            } else if(distToPlayer - attackAreaDist > 1f && distToPlayer < attackRangedDist){
            	MoveRangedToDash();
                sp.color = new Color(0, 255, 0);

            } else if(distToPlayer < attackDashDist){
            	rb.velocity = Vector2.zero;
                sp.color = new Color(255, 0, 200);
            	state = State.idle;
                //Debug.Log("dash");

            }    
        } 
    }

    private void MoveAreaToNormal()
    {
    	rb.velocity = new Vector2(moveSpeed * direction.x, rb.velocity.y);
    }

    private void MoveRangedToDash()
    {
    	rb.velocity = new Vector2(moveSpeed * (-direction.x), rb.velocity.y);
    }

    private IEnumerator BossNormalAttack()
    {
        yield return new WaitForSeconds(animNormalAttack.length * 0.6f);
    	Collider2D[] hitCol = Physics2D.OverlapAreaAll((Vector2) transform.position,
    												    new Vector2(direction.x * attackNormalRange, 2.2f) + (Vector2) transform.position,
    												    playerLayer);
    	foreach(Collider2D hit in hitCol){
    		if(hit.gameObject.tag == "Player"){
    			hit.gameObject.GetComponent<PlayerController>().KnockbackPlayer((int) Mathf.Sign(direction.x));
                hit.gameObject.GetComponent<PlayerController>().DamagePlayer(attackDamage);    			
    		}
    	}
        attacking = false;

    }

    private IEnumerator BossAreaAttack()
    {
        yield return new WaitForSeconds(animAreaAttack.length * 0.6f);
    	Collider2D[] hitCol = Physics2D.OverlapAreaAll(new Vector2(direction.x * attackAreaRange, -2.2f) + (Vector2) transform.position,
    												   new Vector2(direction.x * attackAreaRange, 2.2f) + (Vector2) transform.position,
    												   playerLayer);
		foreach(Collider2D hit in hitCol){
			if(hit.gameObject.name == "Player"){
    			hit.gameObject.GetComponent<PlayerController>().KnockbackPlayer((int) Mathf.Sign(direction.x));
                hit.gameObject.GetComponent<PlayerController>().DamagePlayer(attackDamage);
			}
		} 
        attacking = false;

    }

    private IEnumerator BossRangedAttack()
    {
        yield return new WaitForSeconds(animRangedAttack.length * 0.6f);
        transform.localScale = new Vector2(3f * -Mathf.Sign(direction.x), 3f);
        BossGun.GetComponent<spiderRangedGun>().SpiderRangedFire(direction);
        Debug.Log("213124791264");
        attacking = false;

    }

    private IEnumerator BossDashAttack()
    {
        rb.velocity = new Vector2(dashSpeed * direction.x, 1f);
        yield return new WaitForSeconds(animDashAttack.length * 0.6f);
        attacking = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    	if (collision.tag == "Player")
        {
            if (collision.transform.position.x < transform.position.x)
            {
                collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(-1);
                collision.gameObject.GetComponent<PlayerController>().DamagePlayer(attackDamage);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(1);
                collision.gameObject.GetComponent<PlayerController>().DamagePlayer(attackDamage);
            }
        }

        if (collision.tag == "PatrolPoint1"){
            
            moveSpeed = Mathf.Abs(moveSpeed);

        } else if(collision.tag == "PatrolPoint2"){
            
            moveSpeed = -Mathf.Abs(moveSpeed);

        }
    }

    private void OnDrawGizmos(){
      Gizmos.color = Color.red;
      Gizmos.DrawLine((Vector2) transform.position, 
                       new Vector2(-transform.localScale.x * attackNormalRange, 2.2f) + (Vector2) transform.position);
      
      Gizmos.color = Color.gray;
      Gizmos.DrawWireSphere(transform.position, attackDashDist);
      Gizmos.color = Color.magenta;
      Gizmos.DrawWireSphere(transform.position, attackRangedDist);
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(transform.position, attackAreaRange);
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, attackNormalRange);
    }

}

