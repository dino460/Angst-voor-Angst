using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGates : MonoBehaviour
{
	private bool isRisen = false;
	[SerializeField] private GameObject boss;
	[SerializeField] private Enemy bossEnemyScript;


	private void Update()
	{
		if (bossEnemyScript.isDead)
		{
			Destroy(this.gameObject);
		}
	}


    private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((collision.gameObject.layer == 10 || collision.gameObject.layer == 11) && !isRisen)
		{
			boss.SetActive(true);
			Animator anim = GetComponent<Animator>();
			anim.Play("BossGatesRise");
			isRisen = true;
		}
	}
}
