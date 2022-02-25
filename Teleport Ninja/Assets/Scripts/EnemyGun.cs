using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{

    public GameObject bullet;
    public Transform bulletPoint;
    bool shoot;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Shoot()
    {
        if (!shoot)
            StartCoroutine("ShootRepeated");
        shoot = true;
    }

    public void StopShooting()
    {
        shoot = false;
        StopAllCoroutines();
    }

    IEnumerator ShootRepeated()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i=0;i < 50; i++)
        {
            if (!shoot)
                break;
            if (GameManager.Instance.State == GameState.Walking)
            {
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation);
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation * Quaternion.Euler(0, 7, 0));
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation * Quaternion.Euler(0, -7, 0));
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation * Quaternion.Euler(0, 15, 0));
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation * Quaternion.Euler(0, -15, 0));
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation * Quaternion.Euler(0, 18, 0));
                Instantiate(bullet, bulletPoint.position, bulletPoint.rotation * Quaternion.Euler(0, -18, 0));

                yield return new WaitForSeconds(1f);
            }
        }
    }





}
