using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class BonfireDetector : MonoBehaviour
{
	private Animator anim;

	[SerializeField] private Transform playerPosition;
	[SerializeField] private float maxDistToPlayer;
	//[SerializeField] private GameObject enemyHolder;


	private void Start()
	{
		anim = GetComponent<Animator>();
	}


	private void Update()
	{
		if (Vector2.Distance(transform.position, playerPosition.position) < maxDistToPlayer)
			 anim.SetBool("isActive", true );
		else anim.SetBool("isActive", false);
	}


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			transform.GetChild(2).GetComponent<Animator>().SetInteger("State", 0);
		}
	}


	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			transform.GetChild(2).GetComponent<Animator>().SetInteger("State", 1);
		}
	}


	public void BofireAction()
	{
		Global.life = Global.maxLife;
		Global.mana = Global.maxMana;
		
		// perdao (explicacao no script "enemy")
		int i;
		for(i = 0; i < this.gameObject.name.Length; i++){
            if(this.gameObject.name[i] == '('){
                Global.lastBonfire = int.Parse(this.gameObject.name[i+1].ToString());
                break;
            }
        }
        if(i == this.gameObject.name.Length){
        	Global.lastBonfire = 0;

        }
            	

        SaveSystem.SaveData();
        Debug.Log("Saved!");

        ResurrectEnemies();
	}


	private void ResurrectEnemies()
	{
		/*for(int i = 0; i < enemyHolder.gameObject.transform.childCount; i++)
		{
		   GameObject enemyGameObject = enemyHolder.gameObject.transform.GetChild(i).gameObject;
		   if (enemyGameObject.GetComponent<Enemy>().isDead)
		   {
		   		enemyGameObject.GetComponent<Enemy>().Resurrect();
		   }
		}*/

		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
	}
}
