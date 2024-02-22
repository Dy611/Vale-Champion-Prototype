using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    #region Variables
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
    #endregion Variables

    #region Unity Methods
    void Update()
    {
        //Update Animation When Walking
        if (agent.velocity.x != 0 || agent.velocity.z != 0)
            valeAnim.SetBool("Walking", true);
        else
            valeAnim.SetBool("Walking", false);

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
        {
            RaycastHit hit = StandardFunctions.GetMousePosition(movementLayers);
            RaycastHit enemyHit = StandardFunctions.GetMousePosition(enemyLayers);

            if (enemyHit.transform != null && enemyHit.transform.CompareTag("Enemy"))
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

        if(enemyTarget != null && enemyTarget.GetComponent<Stats>().currentHP > 0 && StandardFunctions.CalculateDistance(enemyTarget.transform.position, transform.position) <= statComp.attackRange && !autoAttacking)
        {
            agent.SetDestination(transform.position);
            StandardFunctions.RotateObj(gameObject, enemyTarget.transform.position);
            autoAttacking = true;
            if(onlyONE == false)
            StartCoroutine(AutoAttacking());
        }

        if (StandardFunctions.CalculateDistance(agent.destination, transform.position) < 1)
        {
            currTimer += Time.deltaTime;
            if (currTimer >= fixDestinationDelay)
                agent.SetDestination(transform.position);
        }
        else
            currTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && !stopMovement)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6 && !stopMovement)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 direction = myNavHit.position - transform.position;

            if (Vector3.Dot(forward, direction) > 0 && StandardFunctions.CalculateDistance(myNavHit.position, transform.position) < agentProximityRange)
            {
                agent.SetDestination(transform.position);
                valeAnim.SetBool("Walking", false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6 && !stopMovement)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(myNavHit.position, 0.25f);
    }
    #endregion Unity Methods

    #region Public Methods
    public void AutoAttack()
    {
        if (enemyTarget != null)
        {
            StandardFunctions.RotateObj(gameObject, enemyTarget.transform.position);
            enemyTarget.GetComponent<Stats>().ApplyDamage(statComp.applyVigilStrikes, statComp.attackDamage);
        }
    }
    #endregion Public Methods

    #region Coroutines
    private IEnumerator StoreClick(Vector3 targetPos)
    {
        while (stopMovement)
        {
            yield return null;
        }
        agent.SetDestination(targetPos);
        yield return null;
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
    #endregion Coroutines
}