using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    #region Variables
    [SerializeField] float delay;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 endPos;

    private float timeElapsed;
    #endregion Variables

    #region Unity Methods
    void Update()
    {
        if(timeElapsed > 0.2f)
            anim.SetBool("Walking", false);

        if(StandardFunctions.CalculateDistance(startPos, transform.position) <= 0.1f || StandardFunctions.CalculateDistance(endPos, transform.position) <= 0.1f)
            timeElapsed += Time.deltaTime;

        if(StandardFunctions.CalculateDistance(startPos, transform.position) <= 0.1f)
        {
            if(timeElapsed >= delay)
                PatrolTo(endPos);
        }
        else if (StandardFunctions.CalculateDistance(endPos, transform.position) <= 0.1f)
        {
            if (timeElapsed >= delay)
                PatrolTo(startPos);
        }
    }
    #endregion Unity Methods

    #region Private Methods
    private void PatrolTo(Vector3 destination)
    {
        //Calculate direction by destination - source
        Vector3 lookDir = destination - transform.position;

        //Remove y to avoid elevation related problems
        lookDir.y = 0;

        //Set rotation
        transform.rotation = Quaternion.LookRotation(lookDir);

        //Reset timer for patrol
        timeElapsed = 0;

        //Move agent to new destination
        agent.SetDestination(destination);

        //Update animator
        anim.SetBool("Walking", true);
    }
    #endregion Private Methods
}