using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiderRangedGun : MonoBehaviour
{

    public GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;

 	public void SpiderRangedFire(Vector3 direction){
 		GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
 		bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
 	}
}
