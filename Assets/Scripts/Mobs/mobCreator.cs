using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobCreator : MonoBehaviour
{

    [HideInInspector] public Rigidbody2D rb;
	[HideInInspector] public Animator    anim;
    [HideInInspector] public SpriteRenderer sr;


    				  public Transform   playerPosition;
    [HideInInspector] public float       distToPlayer; 
    				  
    				  public Enemy       enemyScript;

    [HideInInspector] public Vector3     direction;


    [SerializeField]  public bool        close; // verifica se o player está perto


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        enemyScript = GetComponent<Enemy>();
    }
}
