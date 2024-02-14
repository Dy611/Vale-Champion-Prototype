using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public static bool stopMovement;
    public Animator valeAnim;
    public float agentProximityRange;
    public float fixDestinationDelay;

    public LayerMask movementLayers;
    private NavMeshHit myNavHit;
    private float currTimer;

    // Update is called once per frame
    void Update()
    {
        //Update Animation When Walking
        if (agent.velocity.x != 0 || agent.velocity.z != 0)
            valeAnim.SetBool("Walking", true);
        else
            valeAnim.SetBool("Walking", false);

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000f, movementLayers);

            if (NavMesh.SamplePosition(hit.point, out myNavHit, 100, -1))
                StartCoroutine(StoreClick(myNavHit.position));
        }

        if (CalculateDistance(agent.destination, transform.position) < 1)
        {
            currTimer += Time.deltaTime;
            if (currTimer >= fixDestinationDelay)
                agent.SetDestination(transform.position);
        }
        else
            currTimer = 0;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && !stopMovement)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6 && !stopMovement)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6 && !stopMovement)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 direction = myNavHit.position - transform.position;

            if (Vector3.Dot(forward, direction) > 0 && CalculateDistance(myNavHit.position, transform.position) < agentProximityRange)
            {
                agent.SetDestination(transform.position);
                valeAnim.SetBool("Walking", false);
            }
        }
    }

    private float CalculateDistance(Vector3 destination, Vector3 source)
    {
        float xDiff = destination.x - source.x;
        float zDiff = destination.z - source.z;

        return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(myNavHit.position, 0.25f);
    }
}