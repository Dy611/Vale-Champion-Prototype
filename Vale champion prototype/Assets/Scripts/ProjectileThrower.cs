using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectile;
    public Transform spawnPoint;
    public float speed;
    public float delay;

    private GameObject spawnedObj;
    private float timeElapsed;

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= delay)
        {
            Destroy(spawnedObj);
            timeElapsed = 0;
            spawnedObj = Instantiate(projectile, spawnPoint);
        }

        if(spawnedObj != null)
        {
            spawnedObj.transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }
    }
}
