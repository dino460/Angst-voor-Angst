using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossBody : MonoBehaviour
{
	[SerializeField] private SpiderBossBrain spiderBrain;


    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 10)
		{
			if (collision.transform.position.x < transform.position.x)
			{
				collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(-1);
				collision.gameObject.GetComponent<PlayerController>().DamagePlayer(spiderBrain.touch_Damage);
			}
			else
			{
				collision.gameObject.GetComponent<PlayerController>().KnockbackPlayer(1);
				collision.gameObject.GetComponent<PlayerController>().DamagePlayer(spiderBrain.touch_Damage);
			}
		}
	}
}
