using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
	public float enemyLife;
    public bool  isDead = false;
    public bool  isKnockback = false;
    private AudioManager audioManager;
    private Rigidbody2D rb;
    public mobCreator specificScript;

	public float enemyHorizontalKnockback;
	public float enemyVerticalKnockback;

    public ParticleSystem bloodParticles = null; 


    private void Start()
    {
    	rb = GetComponent<Rigidbody2D>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }


    private string randSound;
    private int releaseSound = 0;
    public void DealDamage(float damage)
    {
    	enemyLife -= damage;

    	if (enemyLife <= 0f)
    	{
    		isDead = true;
        }

        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }

        if(gameObject.tag == "Boss"){
            //releaseSound++;
            //if(releaseSound == 3){
                randSound = Random.Range(1, 4).ToString();
                audioManager.Play("BossScreech" + randSound);
                Debug.Log(randSound);
            //}
        }
    }


    public void KnockbackEnemy(float directionMod)
    {
        Vector2 knockbackForce = new Vector2(directionMod * enemyHorizontalKnockback, enemyVerticalKnockback);
        isKnockback = true;
        rb.velocity = knockbackForce;
    }


    /*public void Resurrect()
    {
        isDead = false;
        specificScript.enabled = true;
    }*/
}
