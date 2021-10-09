using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject player;

    void Start()
    {
        audioManager.Play("Ambience");
        Transform playerTransform = player.GetComponent<Transform>();
        
        /*
        DataGlobal data = SaveSystem.LoadData();

		if(data != null){
			Global.dashCollected = data.hasDash;
			if(data.lastBonfire == 0){
				playerTransform.position = GameObject.Find("Bonfire_tmp").transform.position;
			} else {
				playerTransform.position = GameObject.Find("Bonfire_tmp (" + data.lastBonfire + ")").transform.position;
				Debug.Log("Bonfire_tmp (" + ((char) data.lastBonfire) + ")");
			}
			Global.manaLevel = data.manaLevel;
			Global.lifeLevel = data.lifeLevel;
		}
        */

		
    }
}
