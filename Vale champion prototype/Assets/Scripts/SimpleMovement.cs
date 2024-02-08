using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public static bool stopMovement;
    public Animator valeAnim;
    public float rotSpeed;
    public float agentAvoidanceRange;

    public LayerMask ignoreLayers;
    private NavMeshHit myNavHit;

    // Update is called once per frame
    void Update()
    {
        //Update Animation When Walking
        if(agent.velocity.x != 0 || agent.velocity.z != 0)
            valeAnim.SetBool("Walking", true);
        else
            valeAnim.SetBool("Walking", false);

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, ~ignoreLayers);

            //if (Physics.Raycast(ray, out hit, 1000f, ~ignoreLayers))
            //{
            //    Debug.Log("OBJ: " + hit.transform.gameObject.name);
            //}

            if (NavMesh.SamplePosition(hit.point, out myNavHit, 100, -1))
            {
                Vector3 lookDir = myNavHit.position - transform.position;
                float RotateSpeed = Time.deltaTime * rotSpeed;
                lookDir.y = 0;

                Vector3 NewDirectionToLook = Vector3.RotateTowards(transform.forward, lookDir, RotateSpeed, 0.0f);
                

                StartCoroutine(StoreClick(myNavHit.position, NewDirectionToLook));
            }
        }
    }

    private IEnumerator StoreClick(Vector3 targetPos, Vector3 rot)
    {
        while (stopMovement)
        {
            yield return null;
        }
        transform.rotation = Quaternion.LookRotation(rot);
        agent.SetDestination(targetPos);
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6 && !stopMovement)
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

            Vector3 forward = other.gameObject.transform.position.normalized;
            Vector3 toOther = myNavHit.position - transform.position;

            if (Vector3.Dot(forward, toOther) > 0 && CalculateDistance(myNavHit.position, transform.position) < agentAvoidanceRange)
            {
                agent.SetDestination(transform.position);
                valeAnim.SetBool("Walking", false);
            }
            else
            {
                Debug.Log("CLICKED AWAY");
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