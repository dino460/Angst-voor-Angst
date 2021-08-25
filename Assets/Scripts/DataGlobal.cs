using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataGlobal
{

   	public int lastBonfire;

   	public bool hasDash;

   	public float manaLevel;
   	public float lifeLevel;


   	public DataGlobal () {
   		lastBonfire = Global.lastBonfire;
   		hasDash = Global.dashCollected;

   		manaLevel = Global.manaLevel;
   		lifeLevel = Global.lifeLevel;
   	}

}
