using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderAreaAttack : mobCreator
{

    [Header("Components")]
    [SerializeField] private CapsuleCollider2D col; // colisor do ataque

    public GameObject                         player;

    private enum State {idle, attack, walk, detect, damaged, dead};

    [SerializeField] private State state = State.idle;

    [Header("Attack Variables")]

    [SerializeField] private float     attackDelay = 1f;
    [SerializeField] private float     fastAttackDelay = 0.1f;
    [SerializeField] private float     attackTimer;

    [SerializeField] private float     maxDistToPlayer = 10f;
    [SerializeField] private float     minDistToPlayer = 2f;

    [SerializeField] private float     attackDamage;

    [SerializeField] private int       health = 3;

    [SerializeField] private LayerMask playerLayer;

    private bool                       attacking;

    [Header("Move Variables")]
    [SerializeField] private bool  detected;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float runSpeed = 15f; // ativa quando o player está próximo

    [Header("Knockback Variables")]
    [SerializeField] private float enemyKnockbackTicks;

    private void Start()
    {
        detected = false;
    	attacking = false;
        state = State.idle;
        close = false;
        enemyScript.enemyLife = health;

        col = GetComponent<CapsuleCollider2D>();
    }

    private void Update(){
        if(Global.playerDead)
        {
            state = State.idle;
            return;
        }

        distToPlayer =  Vector3.Distance(playerPosition.position, transform.position);
        //distWalked += Mathf.Abs(rb.velocity.x * Time.deltaTime); // para que ele mude a direção após andar uma certa distância (temporário)
        direction = (playerPosition.position - transform.position).normalized;

        if(distToPlayer < minDistToPlayer)
        {
            close = true;
        } 
        else 
        {
            close = false;
        }

        if (state != State.attack)
        {
            if(close)
            {
                transform.localScale = (new Vector2(-Mathf.Sign(direction.x) * 1.5f, 1.5f));
            } 
            else 
            {
                if(state != State.damaged)
                {
                    if (rb.velocity.x > 0) transform.localScale = new Vector2(-1.5f, 1.5f);
                    else if (rb.velocity.x < 0) transform.localScale = new Vector2(1.5f, 1.5f);
                } 
                else 
                {
                    transform.localScale = new Vector2(-Mathf.Sign(direction.x) * 1.5f, 1.5f);
                }    
            } 
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
        { // timer acabou
            if(state != State.damaged)
            {
                attacking = true;
            }
            attackTimer = attackDelay;
        }    
    }

    private void SpiderAreaAttack()
    {
        float dir = Mathf.Sign(direction.x);
        
        if(state != State.damaged)
        {
            Collider2D[] hitCol = Physics2D.OverlapAreaAll(new Vector2(transform.localScale.x * (minDistToPlayer), -1.2f) + (Vector2) transform.position,
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
            if(!close && state != State.attack)
            { // nao está perto para atacar e não está atacando
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(direction.x), Mathf.Infinity, playerLayer);    
                // basically checks if there is something in the player layer in an infinite line comming from the center of the spider
                // in the direction of the x component of the player

                if(distToPlayer < maxDistToPlayer)
                {
                    detected = true;
                    //distWalked = 0;
                    rb.velocity = new Vector2(Mathf.Sign(direction.x) * runSpeed, rb.velocity.y);
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
        { 
            state = State.dead;
            rb.velocity = new Vector2(0f, 0f);
            transform.localScale = new Vector2(-Mathf.Sign(direction.x) * 1.5f, 1.5f);
            rb.bodyType = RigidbodyType2D.Kinematic;
            col.enabled = false;
            this.enabled = false;
        } 
        else if(enemyScript.isKnockback && state != State.damaged) // check if spider has been damaged
        { 
            state = State.damaged;
            attacking = false;
            anim.enabled = false; // to stop attack animation in the middle if it gets hit
                                  // will be renabled again when it goes fom damaged to idle
        } 
        else if(state == State.damaged) // checks if knockback is done
        { 
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
            // OK I NEED TO FIND A WAY TO FIX THIS BUG IN A BETTER WAY BUT FOR UGL LITE THIS WILL HAVE TO DO
            // SPIDER GETS STUCK IN DETECTED ANIMATION EVEN THOUGH STATE IS ATTACK      
            state = State.attack;
        } 
        else if(attackTimer > 0f && Mathf.Abs(rb.velocity.x) > 0.01f) // checks after attack if player is still in range
        { 
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
    	Gizmos.DrawLine(new Vector2(transform.localScale.x * (minDistToPlayer), -1.2f) + (Vector2) transform.position,
                        new Vector2(transform.localScale.x * (-minDistToPlayer), 1.2f) + (Vector2) transform.position);	
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + Vector2.right * maxDistToPlayer * Mathf.Sign(direction.x));
    }
}
