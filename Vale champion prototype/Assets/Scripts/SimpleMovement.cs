using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public static bool stopMovement;

    private NavMeshHit myNavHit;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (NavMesh.SamplePosition(hit.point, out myNavHit, 100, -1))
            {
                StartCoroutine(StoreClick(myNavHit.position));
            }
        }

        if(CalculateDistance(transform.position, myNavHit.position) >= 0.1f && !stopMovement)
        {
            agent.SetDestination(myNavHit.position);
        }
    }

    private IEnumerator StoreClick(Vector3 targetPos)
    {
        while (stopMovement)
        {
            yield return null;
        }
        agent.SetDestination(targetPos);
        yield return null;
    }

    private float CalculateDistance(Vector3 destination, Vector3 source)
    {
        float xDiff = destination.x - source.x;
        float zDiff = destination.z - source.z;

        return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
    }
}
