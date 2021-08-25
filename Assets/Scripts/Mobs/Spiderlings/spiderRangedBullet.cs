using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderRangedBullet : MonoBehaviour
{
    private Vector3 initialPosition;
    private Rigidbody2D rb;
    public float damage;
    private Enemy enemyScript;

	private void Start(){
		initialPosition = GetComponent<Transform>().position;
		rb = GetComponent<Rigidbody2D>();
		enemyScript = GetComponent<Enemy>();
	}

	private void Update(){
		bool far = Vector3.Distance(GetComponent<Transform>().position, initialPosition) > 40f;
		if(far) Destroy(gameObject);

		if(enemyScript.isDead) Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision){
		if(collision.tag == "Player"){
			if (collision.transform.position.x < transform.position.x)
            {
                collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(-1);
                collision.gameObject.GetComponent<PlayerController>().DamagePlayer(damage);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(1);
                collision.gameObject.GetComponent<PlayerController>().DamagePlayer(damage);
            }
            Destroy(gameObject);
		} else if(collision.tag == "Ground"){
			Destroy(gameObject);
		}
	}


	private void OnCollisionEnter2D(Collision2D collisionInfo)
	{
		if(collisionInfo.gameObject.tag == "Ground"){
			Destroy(gameObject);
		}
	}

}
