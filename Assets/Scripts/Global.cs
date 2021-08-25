using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
	/* DEFAULT VALUES */
	public static bool  defaultDash = false;
	public static float defaultManaLevel = 1;
	public static float defaultLifeLevel = 1;
	/* END */


	public static Resolution screenRes;
	public static bool isFullscreen = false;

    public static bool dashCollected = false;
	public static bool playerDead 	 = false;


	/* MANA/LIFE COMPONENTS */

     // Both mana and life are life plot-wise
     // This way is easier to understand in code
     // Also it is easier to implement two life-meters if there are two "lifes"
	public static float life;
	public static float mana;
	public static float maxLife;
	public static float maxMana;

	 // Base values on which life/mana are calculated
	public static int baseMana = 50;
	public static int baseLife = 30;

	 // Modifiers that define maximum life/mana
	public static float manaLevel = defaultManaLevel;
	public static float lifeLevel = defaultLifeLevel;

	 // Maximum mana/life = base value * Level

	/* END */


   	public static int lastBonfire;
}
