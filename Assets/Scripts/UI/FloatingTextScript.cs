using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloatingTextScript : MonoBehaviour
{
	private Animator anim;


	private void Start()
	{
		anim = GetComponent<Animator>();
	}


    private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			anim.Play("Bonfire_TextFadeOut");
		}
	}
}
