using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectile;
    public Transform spawnPoint;
    public float speed;
    public float delay;
    public float delayOffset;
    public Animator anim;

    private GameObject spawnedObj;
    private float timeElapsed;

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= delay + delayOffset)
        {
            Destroy(spawnedObj);
            timeElapsed = 0;
            anim.SetTrigger("Cast");
        }

        if(spawnedObj != null)
        {
            spawnedObj.transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }
    }

    public void SpawnObj()
    {
        spawnedObj = Instantiate(projectile, spawnPoint);
    }
}
