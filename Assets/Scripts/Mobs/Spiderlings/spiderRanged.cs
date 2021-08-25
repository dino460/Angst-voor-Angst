using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderRanged : mobCreator
{

	[Header("Components")]
    private CircleCollider2D col; // colisor do ataque

    public GameObject        player;

    public GameObject        RangedGun;

    public AnimationClip     animDeath;
    public AnimationClip     animAttack;


    private enum State {idle, attack, walk, detect, damaged, dead};

    [SerializeField] private State state = State.idle;    


    [Header("Attack Variables")]

    [SerializeField] private float     attackDelay;
    [SerializeField] private float     fastAttackDelay = 0.1f;
    [SerializeField] private float     attackTimer;

    [SerializeField] private float     maxDistToPlayer;
    [SerializeField] private float     minDistToPlayer;

    [SerializeField] private float     attackDamage;

    [SerializeField] private int       health = 3;

    [SerializeField] private LayerMask playerLayer;

                     private bool      attacking;

    [Header("Move Variables")]
    private bool                   detected;

    private Vector3                vectToPlayer;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float runSpeed = 15f; // ativa quando o player está próximo

    [Header("Knockback Variables")]
    [SerializeField] private float enemyKnockbackTicks;

    private void Start()
    {
        detected = false;
    	attacking = false;
        state = State.idle;
        attackTimer = 0.1f;
        close = false;
        //distWalked = 0f;

        enemyScript.enemyLife = health;
        col = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if(Global.playerDead)
        {
            state = State.idle;
            return;
        }

        distToPlayer =  Vector3.Distance(playerPosition.position, transform.position);
        vectToPlayer = playerPosition.position - transform.position;
        direction = (playerPosition.position - transform.position).normalized;

        if(distToPlayer < minDistToPlayer && Mathf.Abs(vectToPlayer.y) < 2f)
        {
            close = true;
        } 
        else
        {
            close = false;
        }

        if(!close)
        {
            if (rb.velocity.x > 0) transform.localScale = new Vector2(-1.5f, 1.5f);
            else if (rb.velocity.x < 0) transform.localScale = new Vector2(1.5f, 1.5f);
        } 
        else
        {
            if(state != State.damaged){
                if (rb.velocity.x > 0) transform.localScale = new Vector2(-1.5f, 1.5f);
                else if (rb.velocity.x < 0) transform.localScale = new Vector2(1.5f, 1.5f);
            } else {
                transform.localScale = new Vector2(-Mathf.Sign(direction.x) * 1.5f, 1.5f);
            } 
        }
       /*if (rb.velocity.x > 0.1f) transform.localScale = new Vector2(-1.5f, 1.5f);
        else if (rb.velocity.x < -0.1f) transform.localScale = new Vector2(1.5f, 1.5f);
        else transform.localScale = new Vector2(-1.5f * direction.x, 1.5f);*/

      

        StateSwitch();
        anim.SetInteger("state", (int) state);
    }

    private void FixedUpdate()
    {
        if(Global.playerDead)
        { // if the player is dead, the spider does not execute code from further than this if
            rb.velocity = Vector2.zero;
            return;
        }
        
        if (!enemyScript.isKnockback)
        { 
            Move();
        }
  		
  		if(close)
        {
            attackTimer -= Time.fixedDeltaTime;
        } 
        else 
        {
            attackTimer = fastAttackDelay;
        }

        if(attackTimer <= 0)
        { // timer acabou
            if(state != State.damaged && rb.velocity.y == 0)
            {
                StartCoroutine(SpiderRangedAttack());
            }
 			attackTimer = attackDelay;
        }

    }

    private IEnumerator SpiderRangedAttack()
    {
        attacking = true;
        yield return new WaitForSeconds(animAttack.length * 0.55f);
        RangedGun.GetComponent<spiderRangedGun>().SpiderRangedFire(direction);
        attacking = false;
        attackTimer = attackDelay;
    }

    private void Move()
    {
         if(state != State.attack && !close)
         { 
                /*RaycastHit2D hit = Physics2D.Raycast(
	                									 transform.position, 
	                									 Vector2.right * Mathf.Sign(direction.x),
	                									 Mathf.Infinity,
	                									 playerLayer
                									 );  */  
                // basically checks if there is something in the player layer in an infinite line comming from the center of the spider
                // in the direction of the x component of the player

                if(distToPlayer < maxDistToPlayer && Mathf.Abs(vectToPlayer.y) < 2f)
                {
                    detected = true;
                    rb.velocity = new Vector2(direction.x * runSpeed, rb.velocity.y);
                } 
                else 
                {
                    detected = false;
                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                }
        } 
        else 
        {
            detected = false;
            rb.velocity = Vector2.zero;
            transform.localScale = new Vector2(Mathf.Sign(direction.x) * -1.5f, 1.5f);
        }
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

        if (collision.tag == "PatrolPoint1")
        {
            moveSpeed = Mathf.Abs(moveSpeed);
        } 
        else if(collision.tag == "PatrolPoint2")
        {
            moveSpeed = -Mathf.Abs(moveSpeed);
        }
    }

    private void StateSwitch()
    {
        if(enemyScript.isDead)
        { // check if spider is dead
            state = State.dead;
            rb.velocity = new Vector2(0f, 0f);
            rb.bodyType = RigidbodyType2D.Kinematic;
            col.enabled = false;
            this.enabled = false;
        } 
        else if(enemyScript.isKnockback && state != State.damaged) 
        { // check if spider has been damaged
            state = State.damaged;
        } 
        else if(state == State.damaged)
        { // checks if knockback is done
            if(Mathf.Abs(rb.velocity.x) < 0.01f)
            {
                enemyScript.isKnockback = false;
                state = State.idle;
            }
        } 
        else if(attacking)
        { // check if its attacking
            state = State.attack;
        } 
        else if(attackTimer > 0 && Mathf.Abs(rb.velocity.x) > 0.01)
        { // checks after attack if player is still in range
            if(detected)
            {
                state = State.detect;
                return;
            }
            state = State.walk;
        } 
        else 
        {
            state = State.idle;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + (Vector2.right * Mathf.Sign(direction.x) * maxDistToPlayer));
    }
}
