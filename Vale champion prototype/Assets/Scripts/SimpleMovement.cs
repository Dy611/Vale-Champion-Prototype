using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public static bool stopMovement;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            NavMeshHit myNavHit;
            if (NavMesh.SamplePosition(hit.point, out myNavHit, 100, -1))
            {
                StopCoroutine(StoreClick(myNavHit.position));
                StartCoroutine(StoreClick(myNavHit.position));
            }
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
}
