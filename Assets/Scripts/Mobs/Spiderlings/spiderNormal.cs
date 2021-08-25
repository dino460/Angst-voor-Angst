using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderNormal : mobCreator
{

    [Header("Components")]
    private CapsuleCollider2D col; // colisor do ataque
    
    public  GameObject       player;

    private enum State       {idle, attack, walk, detect, damaged, dead};

    [SerializeField] private State         state = State.idle;

   	private AudioManager audioManager;


    [Header("Attack Variables")]

    [SerializeField] private float     attackDelay = 1f;
    [SerializeField] private float     fastAttackDelay = 0.1f;
    [SerializeField] private float     attackTimer;

    [SerializeField] private float     maxDistToPlayer = 10f;
    [SerializeField] private float     minDistToPlayer = 2f;

    [SerializeField] private float     attackDamage;

    [SerializeField] private int       health = 3;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private bool      attacking;

    [Header("Move Variables")]
    [SerializeField] private bool      detected;

    [SerializeField] private float     moveSpeed = 10f;
    [SerializeField] private float     runSpeed = 15f; // ativa quando o player está próximo

    [SerializeField] private LayerMask groundLayer;

    [Header("Knockback Variables")]
    [SerializeField] private float     enemyKnockbackTicks;

    private void Start()
    {
        detected = false;
    	attacking = false;
        state = State.idle;
        attackTimer = 0.1f;
        close = false;
        enemyScript.enemyLife = health;
        col = GetComponent<CapsuleCollider2D>();

		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        /*rb.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        Debug.Log("AHHHHH");*/
    }

    private void Update()
    {
        if(Global.playerDead)
        {
            state = State.idle;
            return;
        }

        distToPlayer =  Vector3.Distance(playerPosition.position, transform.position);
        direction = (playerPosition.position - transform.position).normalized;

        if(distToPlayer < minDistToPlayer)
        {
            close = true;
        } 
        else 
        {
            close = false;
        }

        if(state != State.damaged)
        {
	        if (rb.velocity.x > 0f) transform.localScale = new Vector2(-1.5f, 1.5f);
	        else if (rb.velocity.x < 0f) transform.localScale = new Vector2(1.5f, 1.5f);
	    } 
        else
        {
	    	transform.localScale = new Vector2(-Mathf.Sign(direction.x) * 1.5f, 1.5f);
	    }

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
        
        if (!enemyScript.isKnockback) Move();
  		
  		if(close)
  		{
            attackTimer -= Time.fixedDeltaTime;
        } 
        else 
        {
            attackTimer = fastAttackDelay;
        }

        if(attackTimer <= 0f)
        { 
        	attacking = true; // ataque como evento com a animacao
        	attackTimer = attackDelay;
        }

    }

    private void SpiderNormalAttack()
    {
    	float dir = Mathf.Sign(direction.x);
    	
    	if(state != State.damaged)
    	{
	    	Collider2D[] hitCol = Physics2D.OverlapAreaAll((Vector2) transform.position,
	                                                       new Vector2(transform.localScale.x * (-minDistToPlayer), 1.2f) + (Vector2) transform.position,
	    												   playerLayer);
	    	foreach(Collider2D hit in hitCol)
	    	{
	    		if(hit.gameObject.name == "Player")
	    		{
	    			hit.gameObject.GetComponent<PlayerController>().KnockbackPlayer((int) dir);
	                hit.gameObject.GetComponent<PlayerController>().DamagePlayer(attackDamage);
	    		}
	    	}
	    }

    	attacking = false;
        attackTimer = attackDelay;
    }

    private void Move()
    {
        if(state != State.damaged)
        {
            if(!close && state != State.attack) // nao está perto para atacar e não está atacando
            { 
                RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                									 Vector2.right * Mathf.Sign(direction.x),
                									 maxDistToPlayer, 
                									 playerLayer);    
                // basically checks if there is something in the player layer in an infinite line comming from the center of the spider
                // in the direction of the x component of the player

                /* FUTURE CHECK TO SEE IF SPIDER WILL FALL FROM THE EDGE
                Vector2 lowPosition = new Vector2(transform.position.x, transform.position.y - 1f); 
                RaycastHit2D cliffCheck1 = Physics2D.Raycast(transform.position,
                                                            new Vector2(Mathf.Cos(Mathf.PI/3), Mathf.Sin(Mathf.PI/3)),
                                                            0.5f,
                                                            groundLayer);
                RaycastHit2D cliffCheck2 = Physics2D.Raycast(transform.position,
                                                            new Vector2(-Mathf.Cos(Mathf.PI/3), Mathf.Sin(Mathf.PI/3)),
                                                            0.5f,
                                                            groundLayer);
                if(cliffCheck1 || cliffCheck2)
                {
                    rb.velocity = Vector2.zero;
                    detected = false;
                }*/


                if(distToPlayer < maxDistToPlayer && hit)
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
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 10)
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

    private IEnumerator HitColor(){
    	Color originalColor = sr.color;
    	Color newColor = Color.red;
    	sr.color = newColor;
    	yield return new WaitForSeconds(1f);
    	sr.color = originalColor;
    }

    private void StateSwitch()
    {
        if(enemyScript.isDead)
        {
            state = State.dead;
            rb.velocity = new Vector2(0f, 0f);
            transform.localScale = new Vector2(-Mathf.Sign(direction.x) * 1.5f, 1.5f);
            rb.bodyType = RigidbodyType2D.Kinematic;
            col.enabled = false;
            this.enabled = false;
        } 
        else if(enemyScript.isKnockback && state != State.damaged)
        { 
            state = State.damaged;
            attacking = false;
            anim.enabled = false; // to stop attack animation in the middle if it gets hit
            					  // will be renabled again when it goes fom damaged to idle
        } 
        else if(state == State.damaged)
        { 
        	//HitColor(); Did not work for some reason
         	// checks if knockback is done
            if(Mathf.Abs(rb.velocity.x) < 0.01f)
            {
                enemyScript.isKnockback = false;
                state = State.idle;
                if(anim.enabled == false) anim.enabled = true;
            }
        } 
        else if(attacking)
        { 
            state = State.attack;
        } 
        else if(attackTimer > 0 && Mathf.Abs(rb.velocity.x) > 0.01f)
        { 
        // checks after attack if player is still in range
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
    	Gizmos.color = Color.red;
    	Gizmos.DrawLine((Vector2) transform.position, new Vector2(transform.localScale.x * (-minDistToPlayer), 1.2f) + (Vector2) transform.position);	
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + Vector2.right * maxDistToPlayer * Mathf.Sign(direction.x));
    }
}
