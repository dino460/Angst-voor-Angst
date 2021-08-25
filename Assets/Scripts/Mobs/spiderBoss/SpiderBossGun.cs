using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBossGun : MonoBehaviour
{
   public GameObject bulletPrefab;
   [SerializeField] private float bulletSpeed;

   public void SpiderRangedFire(Vector2 direction, float damage)
   {
   	bulletPrefab.transform.localScale = new Vector2(2f, 2f);
   	bulletPrefab.GetComponent<spiderRangedBullet>().damage = damage;
   	GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
   	bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
   }
}
