using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float delay;
    public NavMeshAgent agent;

    public float timeElapsed;

    // Update is called once per frame
    void Update()
    {
        if(CalculateDistance(startPos, transform.position) <= 0.1f || CalculateDistance(endPos, transform.position) <= 0.1f)
        {
            timeElapsed += Time.deltaTime;
        }

        if(CalculateDistance(startPos, transform.position) <= 0.1f)
        {
            if(timeElapsed >= delay)
            {
                RotateCharacter(endPos);
            }
        }
        else if (CalculateDistance(endPos, transform.position) <= 0.1f)
        {
            if (timeElapsed >= delay)
            {
                RotateCharacter(startPos);
            }
        }
    }

    private void RotateCharacter(Vector3 destination)
    {
        //Rotate Character
        Vector3 lookDir = destination - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);
        timeElapsed = 0;
        agent.SetDestination(destination);
    }

    private float CalculateDistance(Vector3 destination, Vector3 source)
    {
        float xDiff = destination.x - source.x;
        float zDiff = destination.z - source.z;

        return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
    }
}
