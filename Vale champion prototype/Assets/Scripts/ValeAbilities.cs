using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class ValeAbilities : MonoBehaviour
{
    [Header("Requirements")]
    public NavMeshAgent agent;
    public Camera cam;
    public CollisionSensor colSensor;
    public GameObject navMeshWithCollision;
    public GameObject navMeshWithoutCollision;

    [Header("Q Ability")]
    public float qCooldown;
    public float qCurrentCharges;
    public float qMaxCharges;
    public float qNormalDistance;
    public float qDistanceTowardsEnemies;
    public float qSpeed;
    public float qSlowPercent;
    public float qSlowDuration;
    public float qAllyMSIncreasePercent;
    public float qRefundPercent;

    [Header("W Ability")]
    public float wCooldown;
    public float wShieldDuration;
    public float wInvisDuration;
    public float wRefundPercent;

    [Header("E Ability")]
    public float eCooldown;
    public float eSlowPercent;
    public float eSlowDuration;
    public float eRefundPercent;
    public float eQRefundPercent;

    [Header("R Ability")]
    public float rCooldown;
    public float rDuration;
    public float rHealPercent;

    private RaycastHit hit;
    private NavMeshHit navHit;
    public void OnQAbility(InputValue input)
    {
        Debug.Log("Q Ability");
        StartCoroutine(QAbility());
    }
    public void OnWAbility(InputValue input)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        if (hit.transform.gameObject.CompareTag("Ally"))
        {
            hit.transform.gameObject.AddComponent(typeof(Spellshield));
        }
    }
    public void OnEAbility(InputValue input)
    {
        Debug.Log("E Ability");
    }
    public void OnRAbility(InputValue input)
    {
        Debug.Log("R Ability");
        StartCoroutine(RAbility());
    }

    private IEnumerator QAbility()
    {
        //Prevent Player From "Breaking" Out Of Ability
        SimpleMovement.stopMovement = true;
        //Determine Direction By Getting Mouse Position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        //Rotate Character
        Vector3 lookDir = hit.point - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        //Determine If Ability Is Towards Enemy
        bool towardsEnemy = false;
        foreach(Collider obj in colSensor.cols)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = obj.transform.position - transform.position;
            if(Vector3.Dot(forward, toOther.normalized) >= 0.8f)
            {
                towardsEnemy = true;
            }
        }

        //Calculate Point Max Distance Away
        Vector3 targetPos = new Vector3(0, 0, 0);
        if (towardsEnemy)
            targetPos = transform.position + transform.forward * qDistanceTowardsEnemies;
        else
            targetPos = transform.position + transform.forward * qNormalDistance;

        //Determine Closest Point For Potential Travel Through Walls
        NavMesh.SamplePosition(targetPos, out navHit, 100, -1);

        //Disable "Champion" Collision
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        //Set To Ability Speed
        agent.speed = qSpeed;

        //Disable Standard NavMesh To Allow For Potential Wall Traversal
        navMeshWithCollision.SetActive(false);
        navMeshWithoutCollision.SetActive(true);

        //Move Character
        agent.SetDestination(navHit.position);
        float distance = CalculateDistance(navHit.position, transform.position);
        while (distance >= 0.1f)
        {
            distance = CalculateDistance(navHit.position, transform.position);
            yield return null;
        }

        //Reset Agent Speed
        agent.speed = 5;

        //Enable "Champion" Collision
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        //Reset NavMeshes
        navMeshWithCollision.SetActive(true);
        navMeshWithoutCollision.SetActive(false);

        //ReEnable Player Movement
        SimpleMovement.stopMovement = false;
        yield return null;
    }

    private IEnumerator RAbility()
    {
        yield return null;
    }

    private float CalculateDistance(Vector3 destination, Vector3 source)
    {
        float xDiff = destination.x - source.x;
        float zDiff = destination.z - source.z;

        return Mathf.Sqrt((xDiff * xDiff) + (zDiff * zDiff));
    }
}
