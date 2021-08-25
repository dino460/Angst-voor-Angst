using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HealthUpgrade : MonoBehaviour
{
	private enum UpgradeType {mana, life}
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private UpgradeType type;
	[SerializeField] private Slider upgradeSlider;

	[SerializeField] private float upgradeLevel;


	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.tag == "Player")
		{
			audioManager.Play("AcquireAbility");
			
			if (type == UpgradeType.mana)
			{
				Global.manaLevel = upgradeLevel;
				Global.maxMana   = Global.baseMana * Global.manaLevel;
				Global.mana      = Global.maxMana;

				upgradeSlider.maxValue = Global.maxMana;
				upgradeSlider.transform.localScale = new Vector2((Global.manaLevel), 
											  upgradeSlider.transform.localScale.y);
			}
			else if (type == UpgradeType.life)
			{
				Global.lifeLevel = upgradeLevel;
				Global.maxLife   = Global.baseLife * Global.lifeLevel;
				Global.life      = Global.maxLife;

				upgradeSlider.maxValue = Global.maxLife;
				upgradeSlider.transform.localScale = new Vector2((Global.lifeLevel), 
											  upgradeSlider.transform.localScale.y);
			}

			Destroy(gameObject);
		}
	}
}
