using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    public bool autoAttacking;
    public Stats statComp;
    public Camera cam;
    public NavMeshAgent agent;
    public static bool stopMovement;
    public Animator valeAnim;
    public float agentProximityRange;
    public float fixDestinationDelay;

    public LayerMask movementLayers;
    public LayerMask enemyLayers;
    private NavMeshHit myNavHit;
    private float currTimer;
    private GameObject enemyTarget;

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

            RaycastHit enemyHit;
            if (Physics.Raycast(ray, out enemyHit, 1000f, enemyLayers) && enemyHit.transform.CompareTag("Enemy"))
            {
                enemyTarget = enemyHit.transform.gameObject;
            }
            else
            {
                autoAttacking = false;
                enemyTarget = null;
            }

            if (NavMesh.SamplePosition(hit.point, out myNavHit, 100, -1) && !autoAttacking)
                StartCoroutine(StoreClick(myNavHit.position));
        }

        if(enemyTarget != null && enemyTarget.GetComponent<Stats>().currentHP > 0 && CalculateDistance(enemyTarget.transform.position, transform.position) <= statComp.attackRange && !autoAttacking)
        {
            agent.SetDestination(transform.position);
            RotateVale(enemyTarget.transform.position);
            autoAttacking = true;
            if(onlyONE == false)
            StartCoroutine(AutoAttacking());
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

    private bool onlyONE;
    private IEnumerator AutoAttacking()
    {
        onlyONE = true;
        if (autoAttacking)
            valeAnim.SetTrigger("Auto");

        while (autoAttacking)
        {
            yield return new WaitForSeconds(statComp.attackDelay);
            if (autoAttacking && enemyTarget.GetComponent<Stats>().currentHP > 0)
            {
                valeAnim.SetTrigger("Auto");
            }
            else
                autoAttacking = false;
        }
        onlyONE = false;
    }

    public void AutoAttack()
    {
        if(enemyTarget != null)
        {
            RotateVale(enemyTarget.transform.position);
            enemyTarget.GetComponent<Stats>().ApplyDamage(statComp.applyVigilStrikes, statComp.attackDamage);
        }
    }

    private void RotateVale(Vector3 destination)
    {
        Vector3 lookDir = destination - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}