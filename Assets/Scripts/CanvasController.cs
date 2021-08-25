using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
	private Animator anim;

    [SerializeField] private Image transitionImage;
    [SerializeField] private GameObject deathMenu;


    private void Start()
    {
    	anim = GetComponent<Animator>();
    }


    private void Update()
    {
        if (Global.playerDead)
        {
        	Fade(1);
            StartCoroutine(showDeathMenu());
        }
    }


    private void Fade(int fadeValue)
    {
    	anim.SetInteger("fadeValue", fadeValue);
    }

    private IEnumerator showDeathMenu()
    {
        yield return new WaitForSeconds(1.5f);
        deathMenu.SetActive(true);
    }
}
